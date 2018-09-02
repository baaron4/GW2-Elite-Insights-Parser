using LuckParser.Models;
//recommend CTRL+M+O to collapse all
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
                    // 2 bytes: hitbox width
                    int hbWidth = reader.ReadInt16();
                    // 2 bytes: condition
                    int condition = reader.ReadInt16();
                    // 2 bytes: hitbox height
                    int hbHeight = reader.ReadInt16();
                    // 68 bytes: name
                    String name = ParseHelper.GetString(stream, 68, false);
                    //Save
                    Agent a = new Agent(agent, name, prof, isElite);
                    string agentProf = a.GetProf(_logData.GetBuildVersion(), _aPIController);
                    string profession;
                    switch(agentProf)
                    {
                        case "NPC":
                            // NPC
                            profession = a.GetName() + ":" + prof.ToString().PadLeft(5, '0');
                            break;
                        case "GDG":
                            // Gadget
                            profession = a.GetName() + ":" + (prof & 0x0000ffff).ToString().PadLeft(5, '0');
                            break;
                        default:
                            // Player
                            profession = agentProf;
                            break;
                    }
                    _agentData.AddItem(new AgentItem(agent, name, profession, toughness, healing, condition, concentration, hbWidth, hbHeight), agentProf);
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
                    CombatItem combatItem  = _revision > 0 ? ReadCombatItemRev1(reader) : ReadCombatItem(reader);
                    if (!IsValid(combatItem)) continue;
                    _combatData.Add(combatItem);
                }
            }
        }

        /// <summary>
        /// Returns true if the combat item contains valid data and should be used, false otherwise
        /// </summary>
        /// <param name="combatItem"></param>
        /// <returns>true if the combat item is valid</returns>
        private bool IsValid(CombatItem combatItem)
        {
            if (combatItem.IsStateChange == ParseEnum.StateChange.HealthUpdate && combatItem.DstAgent > 20000)
            {
                // DstAgent should be boss health % times 100, values higher than 10000 are unlikely. 
                // If it is more than 200% health ignore this record
                return false;
            }
            if (combatItem.SrcInstid == 0 && combatItem.DstAgent == 0 && combatItem.SrcAgent == 0 && combatItem.DstInstid == 0 && combatItem.IFF == ParseEnum.IFF.Unknown)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Parses all the data again and link related stuff to each other
        /// </summary>
        private void FillMissingData()
        {
            var agentsLookup = _agentData.GetAllAgentsList().ToDictionary(a => a.Agent);
            // Set Agent instid, firstAware and lastAware
            foreach (CombatItem c in _combatData)
            {
                if(agentsLookup.TryGetValue(c.SrcAgent, out var a))
                {
                    if (a.InstID == 0 && c.IsStateChange.IsSpawn())
                    {
                        a.InstID = c.SrcInstid;
                    }
                    if (a.InstID != 0)
                    {
                        if (a.FirstAware == 0)
                        {
                            a.FirstAware = c.Time;
                            a.LastAware = c.Time;
                        }
                        else
                        {
                            a.LastAware = c.Time;
                        }
                    }
                }
            }

            foreach (CombatItem c in _combatData)
            {
                if (c.SrcMasterInstid != 0)
                {
                    var master = _agentData.GetAllAgentsList().Find(x => x.InstID == c.SrcMasterInstid && x.FirstAware < c.Time && c.Time < x.LastAware);
                    if (master != null)
                    {
                        if(agentsLookup.TryGetValue(c.SrcAgent, out var minion) && minion.FirstAware < c.Time && c.Time < minion.LastAware)
                        {
                            minion.MasterAgent = (master.Agent);
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
                if (NPC.ID == _bossData.GetID())
                {
                    if (_bossData.GetAgent() == 0)
                    {
                        _bossData.SetAgent(NPC.Agent);
                        _bossData.SetInstid(NPC.InstID);
                        _bossData.SetName(NPC.Name);
                        _bossData.SetTough(NPC.Toughness);
                    }
                    multipleBoss.Add(NPC.Agent);
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
            if (_bossData.GetBossBehavior().GetMode() == BossLogic.ParseMode.Golem)
            {
                foreach (CombatItem c in _combatData)
                {
                    // redirect all attacks to the main golem
                    if (c.DstAgent == 0 && c.DstInstid == 0 && c.IsStateChange == ParseEnum.StateChange.Normal && c.IFF == ParseEnum.IFF.Foe && c.IsActivation == ParseEnum.Activation.None)
                    {
                        c.DstAgent = bossAgent.Agent;
                        c.DstInstid = bossAgent.InstID;
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
                                _logData.SetPOV(p.Name);
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
                    if (NPC.ID == 16286)
                    {
                        bossHealthOverTime = new List<Point>();//reset boss health over time
                        int xera2Instid = NPC.InstID;
                        _bossData.SetHealth(24085950);
                        _boss.AddPhaseData(NPC.FirstAware);
                        _bossData.SetLastAware(NPC.LastAware);
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
                List<AgentItem> deimosGadgets = _agentData.GetGadgetAgentList().Where(x => x.FirstAware > bossAgent.LastAware && x.Name.Contains("Deimos")).OrderBy(x => x.LastAware).ToList();
                if (deimosGadgets.Count > 0)
                {
                    AgentItem NPC = deimosGadgets.Last();
                    int deimos2Instid = NPC.InstID;
                    long oldAware = bossAgent.LastAware;
                    _boss.AddPhaseData(NPC.FirstAware >= oldAware ? NPC.FirstAware : oldAware);
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
            _combatData.Validate(_bossData);
            _bossData.SetHealthOverTime(bossHealthOverTime);//after xera in case of change
            _bossData.SetSuccess(_combatData, _logData);
            _bossData.SetCM(_combatData);

            //players
            if (_playerList.Count == 0)
            {

                //Fix Disconnected players
                var playerAgentList = _agentData.GetPlayerAgentList();

                foreach (AgentItem playerAgent in playerAgentList)
                {
                    if (playerAgent.InstID == 0)
                    {
                        CombatItem tst = _combatData.Find(x => x.SrcAgent == playerAgent.Agent);
                        if (tst == null)
                        {
                            tst = _combatData.Find(x => x.DstAgent == playerAgent.Agent);
                            playerAgent.InstID = tst == null ? ushort.MaxValue : tst.DstInstid;
                        }
                        else
                        {
                            playerAgent.InstID = tst.SrcInstid;
                        }
                    }
                    List<CombatItem> lp = _combatData.GetStates(playerAgent.InstID, ParseEnum.StateChange.Despawn, _bossData.GetFirstAware(), _bossData.GetLastAware());
                    Player player = new Player(playerAgent, _bossData.GetBossBehavior().GetMode() == BossLogic.ParseMode.Fractal);
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
                            if (extra.Agent == playerAgent.Agent)
                            {
                                var extraLoginId = extra.InstID;
                                foreach (CombatItem c in _combatData)
                                {
                                    if (c.SrcInstid == extraLoginId)
                                    {
                                        c.SrcInstid = playerAgent.InstID;
                                    }
                                    if (c.DstInstid == extraLoginId)
                                    {
                                        c.DstInstid = playerAgent.InstID;
                                    }
                                }
                                break;
                            }
                        }

                        player.SetDC(lp[0].Time);
                        _playerList.Add(player);
                    }
                    else//didn't dc
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
                _bossData.SetFirstAware(bossAgent.FirstAware);
            }
            if (_bossData.GetLastAware() == long.MaxValue)
            {
                _bossData.SetLastAware(bossAgent.LastAware);
            }
            _playerList = _playerList.OrderBy(a => a.GetGroup()).ToList();
            
        }
    }
}
