using LuckParser.Models;
//recomend CTRL+M+O to collapse all
using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;

//recommend CTRL+M+O to collapse all
namespace LuckParser.Controllers
{
    public class Parser
    {
        private readonly GW2APIController _aPIController = new GW2APIController();

        //Main data storage after binary parse
        private LogData _logData;
        private BossData _bossData;
        private readonly AgentData _agentData = new AgentData();
        private readonly SkillData _skillData = new SkillData();
        private readonly CombatData _combatData = new CombatData();
        private List<Player> _playerList = new List<Player>();
        private Boss _boss;
        private byte _revision;

        // Public Methods
        public LogData GetLogData()
        {
            return _logData;
        }
        public BossData GetBossData()
        {
            return _bossData;
        }

        public ParsedLog GetParsedLog()
        {
            return new ParsedLog(_logData, _bossData, _agentData, _skillData, _combatData, _playerList, _boss);
        }

        //Main Parse method------------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Parses the given log
        /// </summary>
        /// <param name="row">GridRow object bound to the UI</param>
        /// <param name="evtc">The path to the log to parse</param>
        /// <returns></returns>
        public void ParseLog(GridRow row, string evtc)
        {
            using(var fs = new FileStream(evtc, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                if(evtc.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                {
                    using(var arch = new ZipArchive(fs, ZipArchiveMode.Read))
                    {
                        if(arch.Entries.Count != 1)
                        {
                            throw new CancellationException(row, new InvalidDataException("Invalid Archive"));
                        }
                        using(var data = arch.Entries[0].Open())
                        {
                            ParseLog(row, data);
                        }
                    }
                }
                else
                {
                    ParseLog(row, fs);
                }
            }
        }

        private void ParseLog(GridRow row, Stream stream)
        {
            try
            {
                row.BgWorker.ThrowIfCanceled(row);
                row.BgWorker.UpdateProgress(row, "15% - Parsing boss data...", 15);
                ParseBossData(stream);
                row.BgWorker.ThrowIfCanceled(row);
                row.BgWorker.UpdateProgress(row, "20% - Parsing agent data...", 20);
                ParseAgentData(stream);
                row.BgWorker.ThrowIfCanceled(row);
                row.BgWorker.UpdateProgress(row, "25% - Parsing skill data...", 25);
                ParseSkillData(stream);
                row.BgWorker.ThrowIfCanceled(row);
                row.BgWorker.UpdateProgress(row, "30% - Parsing combat list...", 30);
                ParseCombatList(stream);                
                row.BgWorker.ThrowIfCanceled(row);
                row.BgWorker.UpdateProgress(row, "35% - Pairing data...", 35);
                FillMissingData();
                row.BgWorker.ThrowIfCanceled(row);
            }
            catch(Exception ex) when (!(ex is CancellationException))
            {
                throw new CancellationException(row, ex);
            }
        }

        private static BinaryReader CreateReader(Stream stream)
        {
            return new BinaryReader(stream, System.Text.Encoding.UTF8, leaveOpen: true);
        }

        private static bool TryRead(Stream stream, byte[] data)
        {
            int offset = 0;
            int count  = data.Length;
            while(count > 0)
            {
                var bytesRead = stream.Read(data, offset, count);
                if(bytesRead == 0)
                {
                    return false;
                }
                offset += bytesRead;
                count  -= bytesRead;
            }
            return true;
        }

        //sub Parse methods
        /// <summary>
        /// Parses boss related data
        /// </summary>
        private void ParseBossData(Stream stream)
        {
            using (var reader = CreateReader(stream))
            {
                // 12 bytes: arc build version
                var buildVersion = ParseHelper.GetString(stream, 12);
                _logData = new LogData(buildVersion);

                // 1 byte: skip
                _revision = reader.ReadByte();

                // 2 bytes: boss instance ID
                ushort id = reader.ReadUInt16();
                // 1 byte: position
                ParseHelper.SafeSkip(stream, 1);

                //Save
                _bossData = new BossData(id);
            }
        }

        /// <summary>
        /// Parses agent related data
        /// </summary>
        private void ParseAgentData(Stream stream)
        {
            using (var reader = CreateReader(stream))
            {
                // 4 bytes: player count
                int playerCount = reader.ReadInt32();

                // 96 bytes: each player
                for (int i = 0; i < playerCount; i++)
                {
                    // 8 bytes: agent
                    ulong agent = reader.ReadUInt64();

                    // 4 bytes: profession
                    uint prof = reader.ReadUInt32();

                    // 4 bytes: is_elite
                    uint isElite = reader.ReadUInt32();

                    // 2 bytes: toughness
                    int toughness = reader.ReadInt16();
                    // 2 bytes: healing
                    int concentration = reader.ReadInt16();
                    // 2 bytes: healing
                    int healing = reader.ReadInt16();
                    ParseHelper.SafeSkip(stream, 2);
                    // 2 bytes: condition
                    int condition = reader.ReadInt16();
                    ParseHelper.SafeSkip(stream, 2);
                    // 68 bytes: name
                    String name = ParseHelper.GetString(stream, 68, false);
                    //Save
                    Agent a = new Agent(agent, name, prof, isElite);
                    var agentProf = a.GetProf(_logData.GetBuildVersion(), _aPIController);
                    switch(agentProf)
                    {
                        case "NPC":
                            // NPC
                            _agentData.AddItem(new AgentItem(agent, name, a.GetName() + ":" + prof.ToString().PadLeft(5, '0'), toughness, healing, condition, concentration), agentProf);
                            break;
                            // Gadget
                        case "GDG":
                            _agentData.AddItem(new AgentItem(agent, name, a.GetName() + ":" + (prof & 0x0000ffff).ToString().PadLeft(5, '0'),toughness, healing, condition,concentration), agentProf);
                            break;
                        default:
                            // Player
                            _agentData.AddItem(new AgentItem(agent, name, agentProf, toughness, healing, condition,concentration), agentProf);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Parses skill related data
        /// </summary>
        private void ParseSkillData(Stream stream)
        {
            var apiController = new GW2APIController();
            using (var reader = CreateReader(stream))
            {
                // 4 bytes: player count
                int skillCount = reader.ReadInt32();
                //TempData["Debug"] += "Skill Count:" + skill_count.ToString();
                // 68 bytes: each skill
                for(int i = 0; i < skillCount; i++)
                {
                    // 4 bytes: skill ID
                    int skillId = reader.ReadInt32();

                    // 64 bytes: name
                    var name = ParseHelper.GetString(stream, 64);
                    if(skillId != 0 && int.TryParse(name, out int n) && n == skillId)
                    {
                        //was it a known buff?
                        foreach(Boon b in Boon.GetAll())
                        {
                            if(skillId == b.GetID())
                            {
                                name = b.GetName();
                            }
                        }
                    }
                    //Save

                    var skill = new SkillItem(skillId, name);

                    skill.SetGW2APISkill(apiController);
                    _skillData.Add(skill);
                }
            }
        }

        private static CombatItem ReadCombatItem(BinaryReader reader)
        {
            // 8 bytes: time
            long time = reader.ReadInt64();

            // 8 bytes: src_agent
            ulong srcAgent = reader.ReadUInt64();

            // 8 bytes: dst_agent
            ulong dstAgent = reader.ReadUInt64();

            // 4 bytes: value
            int value = reader.ReadInt32();

            // 4 bytes: buff_dmg
            int buffDmg = reader.ReadInt32();

            // 2 bytes: overstack_value
            ushort overstackValue = reader.ReadUInt16();

            // 2 bytes: skill_id
            ushort skillId = reader.ReadUInt16();

            // 2 bytes: src_instid
            ushort srcInstid = reader.ReadUInt16();

            // 2 bytes: dst_instid
            ushort dstInstid = reader.ReadUInt16();

            // 2 bytes: src_master_instid
            ushort srcMasterInstid = reader.ReadUInt16();

            // 9 bytes: garbage
            ParseHelper.SafeSkip(reader.BaseStream, 9);

            // 1 byte: iff
            ParseEnum.IFF iff = ParseEnum.GetIFF(reader.ReadByte());

            // 1 byte: buff
            ushort buff = reader.ReadByte();

            // 1 byte: result
            ParseEnum.Result result = ParseEnum.GetResult(reader.ReadByte());

            // 1 byte: is_activation
            ParseEnum.Activation isActivation = ParseEnum.GetActivation(reader.ReadByte());

            // 1 byte: is_buffremove
            ParseEnum.BuffRemove isBuffRemove = ParseEnum.GetBuffRemove(reader.ReadByte());

            // 1 byte: is_ninety
            ushort isNinety = reader.ReadByte();

            // 1 byte: is_fifty
            ushort isFifty = reader.ReadByte();

            // 1 byte: is_moving
            ushort isMoving = reader.ReadByte();

            // 1 byte: is_statechange
            ParseEnum.StateChange isStateChange = ParseEnum.GetStateChange(reader.ReadByte());

            // 1 byte: is_flanking
            ushort isFlanking = reader.ReadByte();

            // 1 byte: is_flanking
            ushort isShields = reader.ReadByte();
            // 2 bytes: garbage
            ParseHelper.SafeSkip(reader.BaseStream, 2);

            //save
            // Add combat
            return new CombatItem(time, srcAgent, dstAgent, value, buffDmg, overstackValue, skillId,
                srcInstid, dstInstid, srcMasterInstid,0, iff, buff, result, isActivation, isBuffRemove,
                isNinety, isFifty, isMoving, isStateChange, isFlanking, isShields);
        }

        private static CombatItem ReadCombatItemRev1(BinaryReader reader)
        {
            // 8 bytes: time
            long time = reader.ReadInt64();

            // 8 bytes: src_agent
            ulong srcAgent = reader.ReadUInt64();

            // 8 bytes: dst_agent
            ulong dstAgent = reader.ReadUInt64();

            // 4 bytes: value
            int value = reader.ReadInt32();

            // 4 bytes: buff_dmg
            int buffDmg = reader.ReadInt32();

            // 2 bytes: overstack_value
            uint overstackValue = reader.ReadUInt32();

            // 2 bytes: skill_id
            uint skillId = reader.ReadUInt32();

            // 2 bytes: src_instid
            ushort srcInstid = reader.ReadUInt16();

            // 2 bytes: dst_instid
            ushort dstInstid = reader.ReadUInt16();

            // 2 bytes: src_master_instid
            ushort srcMasterInstid = reader.ReadUInt16();
            // 2 bytes: dst_master_instid
            ushort dstmasterInstid = reader.ReadUInt16();

            // 1 byte: iff
            ParseEnum.IFF iff = ParseEnum.GetIFF(reader.ReadByte());

            // 1 byte: buff
            ushort buff = reader.ReadByte();

            // 1 byte: result
            ParseEnum.Result result = ParseEnum.GetResult(reader.ReadByte());

            // 1 byte: is_activation
            ParseEnum.Activation isActivation = ParseEnum.GetActivation(reader.ReadByte());

            // 1 byte: is_buffremove
            ParseEnum.BuffRemove isBuffRemove = ParseEnum.GetBuffRemove(reader.ReadByte());

            // 1 byte: is_ninety
            ushort isNinety = reader.ReadByte();

            // 1 byte: is_fifty
            ushort isFifty = reader.ReadByte();

            // 1 byte: is_moving
            ushort isMoving = reader.ReadByte();

            // 1 byte: is_statechange
            ParseEnum.StateChange isStateChange = ParseEnum.GetStateChange(reader.ReadByte());

            // 1 byte: is_flanking
            ushort isFlanking = reader.ReadByte();

            // 1 byte: is_flanking
            ushort IsShields = reader.ReadByte();
            // 5 bytes: offcycle (?) + garbage
            ParseHelper.SafeSkip(reader.BaseStream, 5);

            //save
            // Add combat
            return new CombatItem(time, srcAgent, dstAgent, value, buffDmg, overstackValue, skillId,
                srcInstid, dstInstid, srcMasterInstid, dstmasterInstid, iff, buff, result, isActivation, isBuffRemove,
                isNinety, isFifty, isMoving, isStateChange, isFlanking, IsShields);
        }

        /// <summary>
        /// Parses combat related data
        /// </summary>
        private void ParseCombatList(Stream stream)
        {
            // 64 bytes: each combat
            var data = new byte[64];
            using(var ms     = new MemoryStream(data, writable: false))
            using(var reader = CreateReader(ms))
            {
                while(true)
                {
                    if(!TryRead(stream, data)) break;
                    ms.Seek(0, SeekOrigin.Begin);
                    _combatData.Add( _revision > 0 ? ReadCombatItemRev1(reader) : ReadCombatItem(reader));
                }
            }
            _combatData.RemoveAll(x => x.SrcInstid == 0 && x.DstAgent == 0 && x.SrcAgent == 0 && x.DstInstid == 0 && x.IFF == ParseEnum.IFF.Unknown);
        }
        
        /// <summary>
        /// Parses all the data again and link related stuff to each other
        /// </summary>
        private void FillMissingData()
        {
            var agentsLookup = _agentData.GetAllAgentsList().ToDictionary(a => a.GetAgent());
            bool golemMode = _bossData.GetBossBehavior().GetMode() == BossLogic.ParseMode.Golem;
            bool raidMode = _bossData.GetBossBehavior().GetMode() == BossLogic.ParseMode.Raid;
            bool fractalMode = _bossData.GetBossBehavior().GetMode() == BossLogic.ParseMode.Fractal;
            // Set Agent instid, firstAware and lastAware
            foreach (CombatItem c in _combatData)
            {
                if(agentsLookup.TryGetValue(c.SrcAgent, out var a))
                {
                    if (a.GetInstid() == 0 && c.IsStateChange.IsSpawn())
                    {
                        a.SetInstid(c.SrcInstid);
                    }
                    if (a.GetInstid() != 0)
                    {
                        if (a.GetFirstAware() == 0)
                        {
                            a.SetFirstAware(c.Time);
                            a.SetLastAware(c.Time);
                        }
                        else
                        {
                            a.SetLastAware(c.Time);
                        }
                    }
                }
            }

            foreach (CombatItem c in _combatData)
            {
                if (c.SrcMasterInstid != 0)
                {
                    var master = _agentData.GetAllAgentsList().Find(x => x.GetInstid() == c.SrcMasterInstid && x.GetFirstAware() < c.Time && c.Time < x.GetLastAware());
                    if (master != null)
                    {
                        if(agentsLookup.TryGetValue(c.SrcAgent, out var minion) && minion.GetFirstAware() < c.Time && c.Time < minion.GetLastAware())
                        {
                            minion.SetMasterAgent(master.GetAgent());
                        }
                    }
                }
            }

            _agentData.Clean();

            // Set Boss data agent, instid, firstAware, lastAware and name
            List<AgentItem> npcList = _agentData.GetNPCAgentList();
            HashSet<ulong> multipleBoss = new HashSet<ulong>();
            foreach (AgentItem NPC in npcList)
            {
                if (NPC.GetID() == _bossData.GetID())
                {
                    if (_bossData.GetAgent() == 0)
                    {
                        _bossData.SetAgent(NPC.GetAgent());
                        _bossData.SetInstid(NPC.GetInstid());
                        _bossData.SetName(NPC.GetName());
                        _bossData.SetTough(NPC.GetToughness());
                    }
                    multipleBoss.Add(NPC.GetAgent());
                }
            }
            if (multipleBoss.Count > 1)
            {
                _agentData.CleanInstid(_bossData.GetInstid());
            }
            AgentItem bossAgent = _agentData.GetAgent(_bossData.GetAgent());
            _boss = new Boss(bossAgent);
            List<Point> bossHealthOverTime = new List<Point>();
            // a hack for buggy golem logs
            if (golemMode)
            {
                AgentItem otherGolem = npcList.Find(x => x.GetID() == 19603);
                foreach (CombatItem c in _combatData)
                {
                    // redirect all attacks to the main golem
                    if (c.DstAgent == 0 && c.DstInstid == 0 && c.IsStateChange == ParseEnum.StateChange.Normal && c.IFF == ParseEnum.IFF.Foe && c.IsActivation == ParseEnum.Activation.None)
                    {
                        c.DstAgent = bossAgent.GetAgent();
                        c.DstInstid = bossAgent.GetInstid();
                    }
                    // redirect buff initial to main golem
                    if (otherGolem != null && c.IsBuff == 18 && c.DstInstid == otherGolem.GetInstid())
                    {
                        c.DstInstid = bossAgent.GetInstid();
                    }
                }

            }
            // Grab values threw combat data
            foreach (CombatItem c in _combatData)
            {
                if (c.SrcInstid == _bossData.GetInstid() && c.IsStateChange == ParseEnum.StateChange.MaxHealthUpdate)//max health update
                {
                    _bossData.SetHealth((int)c.DstAgent);

                }
                switch(c.IsStateChange)
                {
                    case ParseEnum.StateChange.PointOfView:
                        if (_logData.GetPOV() == "N/A")//Point of View
                        {
                            ulong povAgent = c.SrcAgent;
                            if(agentsLookup.TryGetValue(povAgent, out var p))
                            {
                                _logData.SetPOV(p.GetName());
                            }
                        }
                        break;
                    case ParseEnum.StateChange.LogStart:
                        _logData.SetLogStart(c.Value);
                        _bossData.SetFirstAware(c.Time);
                        break;
                    case ParseEnum.StateChange.LogEnd:
                        _logData.SetLogEnd(c.Value);
                        _bossData.SetLastAware(c.Time);
                        break;
                    case ParseEnum.StateChange.HealthUpdate:
                        //set health update
                        if (c.SrcInstid == _bossData.GetInstid())
                        {
                            bossHealthOverTime.Add(new Point ( (int)(c.Time - _bossData.GetFirstAware()), (int)c.DstAgent ));
                        }
                        break;
                }
            }

            // Dealing with second half of Xera | ((22611300 * 0.5) + (25560600 * 0.5)
            if (_bossData.GetID() == 16246)
            {
                foreach (AgentItem NPC in npcList)
                {
                    if (NPC.GetID() == 16286)
                    {
                        bossHealthOverTime = new List<Point>();//reset boss health over time
                        int xera2Instid = NPC.GetInstid();
                        _bossData.SetHealth(24085950);
                        _boss.AddPhaseData(NPC.GetFirstAware());
                        _bossData.SetLastAware(NPC.GetLastAware());
                        foreach (CombatItem c in _combatData)
                        {
                            if (c.SrcInstid == xera2Instid)
                            {
                                c.SrcInstid = _bossData.GetInstid();
                                c.SrcAgent = _bossData.GetAgent();
                            }
                            if (c.DstInstid == xera2Instid)
                            {
                                c.DstInstid = _bossData.GetInstid();
                                c.DstAgent = _bossData.GetAgent();
                            }
                            //set health update
                            if (c.SrcInstid == _bossData.GetInstid() && c.IsStateChange == ParseEnum.StateChange.HealthUpdate)
                            {
                                bossHealthOverTime.Add(new Point ( (int)(c.Time - _bossData.GetFirstAware()), (int)c.DstAgent ));
                            }
                        }
                        break;
                    }
                }
            }
            //Dealing with Deimos split
            if (_bossData.GetID() == 17154)
            {
                List<AgentItem> deimosGadgets = _agentData.GetGadgetAgentList().Where(x => x.GetFirstAware() > bossAgent.GetLastAware() && x.GetName().Contains("Deimos")).OrderBy(x => x.GetLastAware()).ToList();
                if (deimosGadgets.Count > 0)
                {
                    AgentItem NPC = deimosGadgets.Last();
                    int deimos2Instid = NPC.GetInstid();
                    long oldAware = bossAgent.GetLastAware();
                    _boss.AddPhaseData(NPC.GetFirstAware() >= oldAware ? NPC.GetFirstAware() : oldAware);
                    //List<CombatItem> fuckyou = combat_list.Where(x => x.getDstInstid() == deimos2Instid ).ToList().Sum(x);
                    //int stop = 0;
                    foreach (CombatItem c in _combatData)
                    {
                        if (c.Time > oldAware)
                        {
                            if (c.SrcInstid == deimos2Instid)
                            {
                                c.SrcInstid = _bossData.GetInstid();
                                c.SrcAgent = _bossData.GetAgent();

                            }
                            if (c.DstInstid == deimos2Instid)
                            {
                                c.DstInstid = _bossData.GetInstid();
                                c.DstAgent = _bossData.GetAgent();
                            }
                        }

                    }
                }
            }
            _bossData.SetHealthOverTime(bossHealthOverTime);//after xera in case of change

            if (raidMode)
            {
                // Put non reward stuff in this as we find them
                HashSet<int> notRaidRewardsIds = new HashSet<int>
                {
                    13
                };
                CombatItem reward = _combatData.Find(x => x.IsStateChange == ParseEnum.StateChange.Reward && !notRaidRewardsIds.Contains(x.Value));
                if (reward != null)
                {
                    _logData.SetBossKill(true);
                    _bossData.SetLastAware(reward.Time);
                }
            } else if (fractalMode) {
                CombatItem reward = _combatData.Find(x => x.IsStateChange == ParseEnum.StateChange.Reward);
                if (reward != null)
                {
                    _logData.SetBossKill(true);
                    _bossData.SetLastAware(reward.Time);
                } else
                {
                    // for skorvald, as CM and normal ids are the same
                    CombatItem killed = _combatData.Find(x => x.SrcInstid == _bossData.GetInstid() && x.IsStateChange.IsDead());
                    if (killed != null)
                    {
                        _logData.SetBossKill(true);
                        _bossData.SetLastAware(killed.Time);
                    }
                }
            } else
            {
                CombatItem killed = _combatData.Find(x => x.SrcInstid == _bossData.GetInstid() && x.IsStateChange.IsDead());
                if (killed != null)
                {
                    _logData.SetBossKill(true);
                    _bossData.SetLastAware(killed.Time);
                }
            }

            if (golemMode && bossHealthOverTime.Count > 0)
            {
                _logData.SetBossKill(bossHealthOverTime.Last().Y < 200);
                _bossData.SetLastAware(bossHealthOverTime.Last().X + _bossData.GetFirstAware());
            }
            //players
            if (_playerList.Count == 0)
            {

                //Fix Disconected players
                var playerAgentList = _agentData.GetPlayerAgentList();

                foreach (AgentItem playerAgent in playerAgentList)
                {
                    if (playerAgent.GetInstid() == 0)
                    {
                        CombatItem tst = _combatData.Find(x => x.SrcAgent == playerAgent.GetAgent());
                        if (tst == null)
                        {
                            tst = _combatData.Find(x => x.DstAgent == playerAgent.GetAgent());
                            playerAgent.SetInstid(tst == null ? ushort.MaxValue : tst.DstInstid);
                        }
                        else
                        {
                            playerAgent.SetInstid(tst.SrcInstid);
                        }
                    }
                    List<CombatItem> lp = _combatData.GetStates(playerAgent.GetInstid(), ParseEnum.StateChange.Despawn, _bossData.GetFirstAware(), _bossData.GetLastAware());
                    Player player = new Player(playerAgent, fractalMode);
                    bool skip = false;
                    foreach (Player p in _playerList)
                    {
                        if (p.GetAccount() == player.GetAccount())//is this a copy of original?
                        {
                            skip = true;
                        }
                    }
                    if (skip)
                    {
                        continue;
                    }
                    if (lp.Count > 0)
                    {
                        //make all actions of other instances to original instid
                        foreach (AgentItem extra in npcList)
                        {
                            if (extra.GetAgent() == playerAgent.GetAgent())
                            {
                                var extraLoginId = extra.GetInstid();
                                foreach (CombatItem c in _combatData)
                                {
                                    if (c.SrcInstid == extraLoginId)
                                    {
                                        c.SrcInstid = playerAgent.GetInstid();
                                    }
                                    if (c.DstInstid == extraLoginId)
                                    {
                                        c.DstInstid = playerAgent.GetInstid();
                                    }
                                }
                                break;
                            }
                        }

                        player.SetDC(lp[0].Time);
                        _playerList.Add(player);
                    }
                    else//didnt dc
                    {
                        if (player.GetDC() == 0)
                        {
                            _playerList.Add(player);
                        }

                    }
                }

            }
            if (_bossData.GetFirstAware() == 0)
            {
                _bossData.SetFirstAware(bossAgent.GetFirstAware());
            }
            if (_bossData.GetLastAware() == long.MaxValue)
            {
                _bossData.SetLastAware(bossAgent.GetLastAware());
            }
            // Sort
            _playerList = _playerList.OrderBy(a => a.GetGroup()).ToList();
            // Check CM
            _bossData.SetCM(_combatData);
            _combatData.Validate(_bossData);
        }
    }
}
