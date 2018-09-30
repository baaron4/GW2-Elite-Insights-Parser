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
    class Parser
    {
        private readonly GW2APIController _aPIController = new GW2APIController();

        //Main data storage after binary parse
        private LogData _logData;
        private FightData _fightData;
        private AgentData _agentData;
        private readonly List<AgentItem> _allAgentsList = new List<AgentItem>();
        private readonly SkillData _skillData = new SkillData();
        private readonly SettingsContainer _settings;
        private List<CombatItem> _combatItems = new List<CombatItem>();
        private List<Player> _playerList = new List<Player>();
        private Boss _boss;
        private byte _revision;

        public Parser(SettingsContainer settings)
        {
            _settings = settings;
        }

        //Main Parse method------------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Parses the given log
        /// </summary>
        /// <param name="row">GridRow object bound to the UI</param>
        /// <param name="evtc">The path to the log to parse</param>
        /// <returns>the ParsedLog</returns>
        public ParsedLog ParseLog(GridRow row, string evtc)
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
            return new ParsedLog(_logData, _fightData, _agentData, _skillData, new CombatData(_combatItems, _fightData), _playerList, _boss);
        }

        private void ParseLog(GridRow row, Stream stream)
        {
            try
            {
                row.BgWorker.ThrowIfCanceled(row);
                row.BgWorker.UpdateProgress(row, "15% - Parsing boss data...", 15);
                ParseFightData(stream);
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
            catch (Exception ex) when (!(ex is CancellationException))
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
        private void ParseFightData(Stream stream)
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
                _fightData = new FightData(id, _settings.ParsePhases);
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
                    uint toughness = reader.ReadUInt16();
                    // 2 bytes: healing
                    uint concentration = reader.ReadUInt16();
                    // 2 bytes: healing
                    uint healing = reader.ReadUInt16();
                    // 2 bytes: hitbox width
                    uint hbWidth = reader.ReadUInt16();
                    // 2 bytes: condition
                    uint condition = reader.ReadUInt16();
                    // 2 bytes: hitbox height
                    uint hbHeight = reader.ReadUInt16();
                    // 68 bytes: name
                    String name = ParseHelper.GetString(stream, 68, false);
                    //Save
                    Agent a = new Agent(agent, name, prof, isElite);
                    string agentProf = a.GetProf(_logData.BuildVersion, _aPIController);
                    AgentItem.AgentType type;
                    string profession;
                    switch(agentProf)
                    {
                        case "NPC":
                            // NPC
                            profession = a.Name + ":" + prof.ToString().PadLeft(5, '0');
                            type = AgentItem.AgentType.NPC;
                            break;
                        case "GDG":
                            // Gadget
                            profession = a.Name + ":" + (prof & 0x0000ffff).ToString().PadLeft(5, '0');
                            type = AgentItem.AgentType.Gadget;
                            break;
                        default:
                            // Player
                            profession = agentProf;
                            type = AgentItem.AgentType.Player;
                            break;
                    }
                    _allAgentsList.Add(new AgentItem(agent, name, profession, type, toughness, healing, condition, concentration, hbWidth, hbHeight));
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
                        if (Boon.BoonsByIds.TryGetValue(skillId, out Boon boon))
                        {
                            name = boon.Name;
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
            byte buff = reader.ReadByte();

            // 1 byte: result
            ParseEnum.Result result = ParseEnum.GetResult(reader.ReadByte());

            // 1 byte: is_activation
            ParseEnum.Activation isActivation = ParseEnum.GetActivation(reader.ReadByte());

            // 1 byte: is_buffremove
            ParseEnum.BuffRemove isBuffRemove = ParseEnum.GetBuffRemove(reader.ReadByte());

            // 1 byte: is_ninety
            byte isNinety = reader.ReadByte();

            // 1 byte: is_fifty
            byte isFifty = reader.ReadByte();

            // 1 byte: is_moving
            byte isMoving = reader.ReadByte();

            // 1 byte: is_statechange
            ParseEnum.StateChange isStateChange = ParseEnum.GetStateChange(reader.ReadByte());

            // 1 byte: is_flanking
            byte isFlanking = reader.ReadByte();

            // 1 byte: is_flanking
            byte isShields = reader.ReadByte();
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
            byte buff = reader.ReadByte();

            // 1 byte: result
            ParseEnum.Result result = ParseEnum.GetResult(reader.ReadByte());

            // 1 byte: is_activation
            ParseEnum.Activation isActivation = ParseEnum.GetActivation(reader.ReadByte());

            // 1 byte: is_buffremove
            ParseEnum.BuffRemove isBuffRemove = ParseEnum.GetBuffRemove(reader.ReadByte());

            // 1 byte: is_ninety
            byte isNinety = reader.ReadByte();

            // 1 byte: is_fifty
            byte isFifty = reader.ReadByte();

            // 1 byte: is_moving
            byte isMoving = reader.ReadByte();

            // 1 byte: is_statechange
            ParseEnum.StateChange isStateChange = ParseEnum.GetStateChange(reader.ReadByte());

            // 1 byte: is_flanking
            byte isFlanking = reader.ReadByte();

            // 1 byte: is_flanking
            byte IsShields = reader.ReadByte();
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
                    _combatItems.Add(combatItem);
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

        private void CompleteAgents()
        {
            var agentsLookup = _allAgentsList.ToDictionary(a => a.Agent);
            // Set Agent instid, firstAware and lastAware
            foreach (CombatItem c in _combatItems)
            {
                if (agentsLookup.TryGetValue(c.SrcAgent, out var a))
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

            foreach (CombatItem c in _combatItems)
            {
                if (c.SrcMasterInstid != 0)
                {
                    var master = _allAgentsList.Find(x => x.InstID == c.SrcMasterInstid && x.FirstAware < c.Time && c.Time < x.LastAware);
                    if (master != null)
                    {
                        if (agentsLookup.TryGetValue(c.SrcAgent, out var minion) && minion.FirstAware < c.Time && c.Time < minion.LastAware)
                        {
                            minion.MasterAgent = (master.Agent);
                        }
                    }
                }
            }
            _allAgentsList.RemoveAll(x => !(x.InstID != 0 && x.LastAware - x.FirstAware >= 0 && x.FirstAware != 0 && x.LastAware != long.MaxValue) && x.Type != AgentItem.AgentType.Player);
            _agentData = new AgentData(_allAgentsList);
        }
        private void CompletePlayers()
        {
            //Fix Disconnected players
            var playerAgentList = _agentData.GetAgentByType(AgentItem.AgentType.Player);

            foreach (AgentItem playerAgent in playerAgentList)
            {
                if (playerAgent.InstID == 0)
                {
                    CombatItem tst = _combatItems.Find(x => x.SrcAgent == playerAgent.Agent);
                    if (tst == null)
                    {
                        tst = _combatItems.Find(x => x.DstAgent == playerAgent.Agent);
                        if (tst == null)
                        {
                            continue;
                        }
                        playerAgent.InstID = tst.DstInstid;
                    }
                    else
                    {
                        playerAgent.InstID = tst.SrcInstid;
                    }
                    playerAgent.FirstAware = _fightData.FightStart;
                    playerAgent.LastAware = _fightData.FightEnd;
                }
                List<CombatItem> lp = _combatItems.Where(x => x.IsStateChange == ParseEnum.StateChange.Despawn && x.SrcInstid == playerAgent.InstID && x.Time <= _fightData.FightEnd && x.Time >= _fightData.FightStart).ToList();
                Player player = new Player(playerAgent, _fightData.Logic.Mode == BossLogic.ParseMode.Fractal);
                bool skip = false;
                foreach (Player p in _playerList)
                {
                    if (p.Account == player.Account)//is this a copy of original?
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
                    foreach (AgentItem extra in _agentData.GetAgentByType(AgentItem.AgentType.NPC))
                    {
                        if (extra.Agent == playerAgent.Agent)
                        {
                            var extraLoginId = extra.InstID;
                            foreach (CombatItem c in _combatItems)
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

                    player.Disconnected = lp[0].Time;
                    _playerList.Add(player);
                }
                else//didn't dc
                {
                    if (player.Disconnected == 0)
                    {
                        _playerList.Add(player);
                    }

                }
            }
        }
        /// <summary>
        /// Parses all the data again and link related stuff to each other
        /// </summary>
        private void FillMissingData()
        {
            CompleteAgents();
            _fightData.Logic.ComputeFightTargets(_agentData, _fightData, _combatItems);
            _boss = _fightData.Logic.Targets.Find(x => x.ID == _fightData.ID);
            if (_boss == null)
            {
                _boss = new Boss(new AgentItem(0, "UNKNOWN"));
            }
            // Dealing with special cases
            _fightData.Logic.SpecialParse(_fightData, _agentData, _combatItems);
            // Grab values threw combat data
            foreach (CombatItem c in _combatItems)
            {
                switch(c.IsStateChange)
                {
                    case ParseEnum.StateChange.PointOfView:
                        if (_logData.PoV == "N/A")//Point of View
                        {
                            ulong povAgent = c.SrcAgent;
                            _logData.SetPOV(_agentData.GetAgent(povAgent).Name);                          
                        }
                        break;
                    case ParseEnum.StateChange.LogStart:
                        _logData.SetLogStart(c.Value);
                        if (_fightData.FightStart == 0)
                        {
                            _fightData.FightStart = c.Time;
                        }
                        break;
                    case ParseEnum.StateChange.LogEnd:
                        _logData.SetLogEnd(c.Value);
                        _fightData.FightEnd = c.Time;
                        break;
                    case ParseEnum.StateChange.MaxHealthUpdate:
                        _fightData.Logic.SetMaxHealth(c.SrcInstid, c.Time, (int)c.DstAgent);
                        break;
                    case ParseEnum.StateChange.HealthUpdate:
                        //set health update
                        _fightData.Logic.AddHealthUpdate(c.SrcInstid,c.Time, (int)(c.Time - _fightData.FightStart), (int)c.DstAgent);
                        break;
                }
            }

            //players
            if (_playerList.Count == 0)
            {
                CompletePlayers();               
            }
            if (_fightData.FightStart == 0 && _combatItems.Count > 0)
            {
                _fightData.FightStart = _combatItems.First().Time;
            }
            if (_fightData.FightEnd== long.MaxValue && _combatItems.Count > 0)
            {
                _fightData.FightEnd = _combatItems.Last().Time;
            }
            _playerList = _playerList.OrderBy(a => a.Group).ToList();
            
        }
    }
}
