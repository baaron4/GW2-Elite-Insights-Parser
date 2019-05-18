using LuckParser.Controllers;
using LuckParser.Exceptions;
using LuckParser.Models;
//recommend CTRL+M+O to collapse all
using LuckParser.Parser;
using LuckParser.Models.Logic;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using LuckParser.Setting;

//recommend CTRL+M+O to collapse all
namespace LuckParser.Parser
{
    class ParsingController
    {
        private readonly GW2APIController _aPIController = new GW2APIController();

        //Main data storage after binary parse
        private LogData _logData;
        private FightData _fightData;
        private AgentData _agentData;
        private readonly List<AgentItem> _allAgentsList = new List<AgentItem>();
        private readonly SkillData _skillData = new SkillData();
        private List<CombatItem> _combatItems = new List<CombatItem>();
        private List<Player> _playerList = new List<Player>();
        private byte _revision;
        private ushort _id;

        public ParsingController()
        {
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
            using (var fs = new FileStream(evtc, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                if (GeneralHelper.IsCompressedFormat(evtc))
                {
                    using (var arch = new ZipArchive(fs, ZipArchiveMode.Read))
                    {
                        if (arch.Entries.Count != 1)
                        {
                            throw new CancellationException(row, new InvalidDataException("Invalid Archive"));
                        }
                        using (var data = arch.Entries[0].Open())
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
            Target legacyTarget = _fightData.Logic.Targets.Find(x => x.ID == _fightData.ID);
            if (legacyTarget == null)
            {
                legacyTarget = new Target(GeneralHelper.UnknownAgent);
            }
            return new ParsedLog(_logData, _fightData, _agentData, _skillData, new CombatData(_combatItems, _fightData, _playerList), _playerList, legacyTarget);
        }

        private void ParseLog(GridRow row, Stream stream)
        {
#if !DEBUG
            try
            {
#endif
                row.BgWorker.ThrowIfCanceled(row);
                row.BgWorker.UpdateProgress(row, "15% - Parsing fight data...", 15);
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
#if !DEBUG
        }
            catch (Exception ex) when (!(ex is CancellationException))
            {
                throw new CancellationException(row, ex);
            }
#endif
        }

        private BinaryReader CreateReader(Stream stream)
        {
            return new BinaryReader(stream, new System.Text.UTF8Encoding(), leaveOpen: true);
        }

        private bool TryRead(Stream stream, byte[] data)
        {
            int offset = 0;
            int count = data.Length;
            while (count > 0)
            {
                var bytesRead = stream.Read(data, offset, count);
                if (bytesRead == 0)
                {
                    return false;
                }
                offset += bytesRead;
                count -= bytesRead;
            }
            return true;
        }

        //sub Parse methods
        /// <summary>
        /// Parses fight related data
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

                // 2 bytes: fight instance ID
                _id = reader.ReadUInt16();
                // 1 byte: position
                ParseHelper.SafeSkip(stream, 1);
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
                    uint hbWidth = (uint)2 * reader.ReadUInt16();
                    // 2 bytes: condition
                    uint condition = reader.ReadUInt16();
                    // 2 bytes: hitbox height
                    uint hbHeight = (uint)2 * reader.ReadUInt16();
                    // 68 bytes: name
                    string name = ParseHelper.GetString(stream, 68, false);
                    //Save
                    string agentProf = GeneralHelper.GetAgentProfString(_logData.BuildVersion, _aPIController, prof, isElite);
                    AgentItem.AgentType type;
                    ushort ID = 0;
                    switch (agentProf)
                    {
                        case "NPC":
                            // NPC
                            try
                            {
                                ID = ushort.Parse(prof.ToString().PadLeft(5, '0'));
                            }
                            catch (FormatException)
                            {
                                ID = 0;
                            }
                            type = AgentItem.AgentType.NPC;
                            break;
                        case "GDG":
                            // Gadget
                            try
                            {
                                ID = ushort.Parse((prof & 0x0000ffff).ToString().PadLeft(5, '0'));
                            }
                            catch (FormatException)
                            {
                                ID = 0;
                            }
                            type = AgentItem.AgentType.Gadget;
                            break;
                        default:
                            // Player
                            type = AgentItem.AgentType.Player;
                            break;
                    }
                    _allAgentsList.Add(new AgentItem(agent, name, agentProf, ID, type, toughness, healing, condition, concentration, hbWidth, hbHeight));
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
                uint skillCount = reader.ReadUInt32();
                //TempData["Debug"] += "Skill Count:" + skill_count.ToString();
                // 68 bytes: each skill
                for (int i = 0; i < skillCount; i++)
                {
                    // 4 bytes: skill ID
                    int skillId = reader.ReadInt32();
                    // 64 bytes: name
                    var name = ParseHelper.GetString(stream, 64);
                    //Save
                    var skill = new SkillItem(skillId, name, apiController);
                    _skillData.Add(skill);
                }
            }
        }

        private CombatItem ReadCombatItem(BinaryReader reader)
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
            byte result = reader.ReadByte();

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
            // 1 byte: is_flanking
            byte isOffcycle = reader.ReadByte();
            // 1 bytes: garbage
            ParseHelper.SafeSkip(reader.BaseStream, 1);

            //save
            // Add combat
            return new CombatItem(time, srcAgent, dstAgent, value, buffDmg, overstackValue, skillId,
                srcInstid, dstInstid, srcMasterInstid, 0, iff, buff, result, isActivation, isBuffRemove,
                isNinety, isFifty, isMoving, isStateChange, isFlanking, isShields, isOffcycle);
        }

        private CombatItem ReadCombatItemRev1(BinaryReader reader)
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

            // 4 bytes: overstack_value
            uint overstackValue = reader.ReadUInt32();

            // 4 bytes: skill_id
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
            byte result = reader.ReadByte();

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
            // 1 byte: is_flanking
            byte isOffcycle = reader.ReadByte();
            // 5 bytes: offcycle (?) + garbage
            ParseHelper.SafeSkip(reader.BaseStream, 4);

            //save
            // Add combat
            return new CombatItem(time, srcAgent, dstAgent, value, buffDmg, overstackValue, skillId,
                srcInstid, dstInstid, srcMasterInstid, dstmasterInstid, iff, buff, result, isActivation, isBuffRemove,
                isNinety, isFifty, isMoving, isStateChange, isFlanking, isShields, isOffcycle);
        }

        /// <summary>
        /// Parses combat related data
        /// </summary>
        private void ParseCombatList(Stream stream)
        {
            // 64 bytes: each combat
            var data = new byte[64];
            using (var ms = new MemoryStream(data, writable: false))
            using (var reader = CreateReader(ms))
            {
                while (true)
                {
                    if (!TryRead(stream, data)) break;
                    ms.Seek(0, SeekOrigin.Begin);
                    CombatItem combatItem = _revision > 0 ? ReadCombatItemRev1(reader) : ReadCombatItem(reader);
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
                // DstAgent should be target health % times 100, values higher than 10000 are unlikely. 
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
            var agentsLookup = _allAgentsList.GroupBy(x => x.Agent).ToDictionary(x => x.Key, x => x.ToList()); ;
            // Set Agent instid, firstAware and lastAware
            foreach (CombatItem c in _combatItems)
            {
                if (agentsLookup.TryGetValue(c.SrcAgent, out var agentList))
                {
                    foreach (AgentItem agent in agentList)
                    {
                        if (agent.InstID == 0)
                        {
                            agent.InstID = c.SrcInstid;
                            if (agent.FirstAware == 0)
                            {
                                agent.FirstAware = c.Time;
                            }
                            agent.LastAware = c.Time;
                            break;
                        }
                        else if (agent.InstID == c.SrcInstid)
                        {
                            agent.LastAware = c.Time;
                            break;
                        }
                    }
                }
            }

            foreach (CombatItem c in _combatItems)
            {
                if (c.SrcMasterInstid != 0)
                {
                    var master = _allAgentsList.Find(x => x.InstID == c.SrcMasterInstid && x.FirstAware <= c.Time && c.Time <= x.LastAware);
                    if (master != null)
                    {
                        if (agentsLookup.TryGetValue(c.SrcAgent, out var minionList))
                        {
                            foreach (AgentItem minion in minionList)
                            {
                                if (minion.FirstAware <= c.Time && c.Time <= minion.LastAware)
                                {
                                    minion.MasterAgent = (master.Agent);
                                }
                            }
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
                if (playerAgent.InstID == 0 || playerAgent.FirstAware == 0 || playerAgent.LastAware == long.MaxValue)
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
                try
                {
                    bool skip = false;
                    Player player = new Player(playerAgent, _fightData.Logic.Mode == FightLogic.ParseMode.Fractal);
                    foreach (Player p in _playerList)
                    {
                        if (p.Account == player.Account)// same player
                        {
                            if (p.Character == player.Character) // same character, can be fused
                            {
                                skip = true;
                                Random rnd = new Random();
                                ulong agent = 0;
                                while (_agentData.AgentValues.Contains(agent) || agent == 0)
                                {
                                    agent = (ulong)rnd.Next(Int32.MaxValue / 2, Int32.MaxValue);
                                }
                                ushort instid = 0;
                                while (_agentData.InstIDValues.Contains(instid) || instid == 0)
                                {
                                    instid = (ushort)rnd.Next(ushort.MaxValue / 2, ushort.MaxValue);
                                }
                                foreach (CombatItem c in _combatItems)
                                {
                                    if (c.DstAgent == p.Agent || player.Agent == c.DstAgent)
                                    {
                                        c.OverrideDstValues(agent, instid);
                                    }
                                    if (c.SrcAgent == p.Agent || player.Agent == c.SrcAgent)
                                    {
                                        c.OverrideSrcValues(agent, instid);
                                    }
                                }
                                p.AgentItem.InstID = instid;
                                p.AgentItem.Agent = agent;
                                p.AgentItem.FirstAware = Math.Min(p.AgentItem.FirstAware, player.AgentItem.FirstAware);
                                p.AgentItem.LastAware = Math.Max(p.AgentItem.LastAware, player.AgentItem.LastAware);
                                _agentData.Refresh();
                                break;
                            }
                            // different character in raid mode, discard it as it can't have any influence, otherwise add as a separate entity
                            else if (_fightData.Logic.Mode == FightLogic.ParseMode.Raid)
                            {
                                skip = true;
                                break;
                            }
                        }
                    }
                    if (!skip)
                    {
                        _playerList.Add(player);
                    }
                } catch (InvalidPlayerException ex)
                {
                    if (_fightData.Logic.Mode != FightLogic.ParseMode.WvW)
                    {
                        throw ex;
                    }
                    // the players are enemy
                    /*if (!ex.Squadless)
                    {
                        _fightData.Logic.Targets.Add(new Target(playerAgent));
                    }*/
                }
            }
        }
        /// <summary>
        /// Parses all the data again and link related stuff to each other
        /// </summary>
        private void FillMissingData()
        {
            CompleteAgents();
            _fightData = new FightData(_id, _agentData);
            _fightData.Logic.ComputeFightTargets(_agentData, _fightData, _combatItems);
            if (_combatItems.Count > 0)
            {
                _fightData.FightStart = _combatItems.Min(x => x.Time);
                _fightData.FightEnd = _combatItems.Max(x => x.Time);
            }
            else
            {
                throw new InvalidDataException("No combat events found");
            }
            // Dealing with special cases
            _fightData.Logic.SpecialParse(_fightData, _agentData, _combatItems);
            // Grab values threw combat data
            foreach (CombatItem c in _combatItems)
            {
                switch (c.IsStateChange)
                {
                    case ParseEnum.StateChange.PointOfView:
                        if (_logData.PoV == "N/A")//Point of View
                        {
                            ulong povAgent = c.SrcAgent;
                            _logData.SetPOV(_agentData.GetAgent(povAgent, c.Time).Name);
                        }
                        break;
                    case ParseEnum.StateChange.GWBuild:
                        _logData.GW2Version = c.SrcAgent;
                        break;
                    case ParseEnum.StateChange.LogStart:
                        _logData.SetLogStart(c.Value);
                        break;
                    case ParseEnum.StateChange.LogEnd:
                        _logData.SetLogEnd(c.Value);
                        break;
                    case ParseEnum.StateChange.MaxHealthUpdate:
                        _fightData.Logic.SetMaxHealth(c.SrcInstid, c.Time, (int)c.DstAgent);
                        break;
                    case ParseEnum.StateChange.HealthUpdate:
                        //set health update
                        _fightData.Logic.AddHealthUpdate(c.SrcInstid, c.Time, c.Time, (int)c.DstAgent);
                        break;
                }
            }
            if (_logData.LogStart != LogData.DefaultTimeValue && _logData.LogEnd == LogData.DefaultTimeValue)
            {
                _logData.SetLogEnd(_logData.LogStartUnixSeconds + _combatItems.Last().Time - _combatItems.First().Time);
            }
            else if (_logData.LogStart == LogData.DefaultTimeValue && _logData.LogEnd != LogData.DefaultTimeValue)
            {
                _logData.SetLogStart(_logData.LogEndUnixSeconds - (_combatItems.Last().Time - _combatItems.First().Time));
            }
            //players
            CompletePlayers();
            _playerList = _playerList.OrderBy(a => a.Group).ToList();
            if (Properties.Settings.Default.Anonymous)
            {
                for (int i = 0; i < _playerList.Count; i++)
                {
                    _playerList[i].Anonymize(i + 1);
                }
            }
        }
    }
}
