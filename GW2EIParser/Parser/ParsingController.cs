using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using GW2EIParser.Controllers;
using GW2EIParser.EIData;
//recommend CTRL+M+O to collapse all
using GW2EIParser.Logic;
using GW2EIParser.Parser.ParsedData;

//recommend CTRL+M+O to collapse all
namespace GW2EIParser.Parser
{
    public class ParsingController
    {

        //Main data storage after binary parse
        private FightData _fightData;
        private AgentData _agentData;
        private readonly List<AgentItem> _allAgentsList = new List<AgentItem>();
        private readonly SkillData _skillData = new SkillData();
        private readonly List<CombatItem> _combatItems = new List<CombatItem>();
        private List<Player> _playerList = new List<Player>();
        private byte _revision;
        private ushort _id;
        private long _logStartTime = 0;
        private long _logEndTime = 0;
        private string _buildVersion;
        private readonly ParserSettings _parserSettings;

        public ParsingController(ParserSettings parserSettings)
        {
            _parserSettings = parserSettings;
        }

        //Main Parse method------------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Parses the given log
        /// </summary>
        /// <param name="operation">Operation object bound to the UI</param>
        /// <param name="evtc">The path to the log to parse</param>
        /// <returns>the ParsedLog</returns>
        public ParsedLog ParseLog(OperationController operation, string evtc)
        {
            operation.UpdateProgress("Reading Binary");
            using (var fs = new FileStream(evtc, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                if (ProgramHelper.IsCompressedFormat(evtc))
                {
                    using (var arch = new ZipArchive(fs, ZipArchiveMode.Read))
                    {
                        if (arch.Entries.Count != 1)
                        {
                            throw new InvalidDataException("Invalid Archive");
                        }
                        using (Stream data = arch.Entries[0].Open())
                        {
                            using (var ms = new MemoryStream())
                            {
                                data.CopyTo(ms);
                                ms.Position = 0;
                                ParseLog(operation, ms);
                            };
                        }
                    }
                }
                else
                {
                    ParseLog(operation, fs);
                }
            }
            operation.ThrowIfCanceled();
            operation.UpdateProgress("Data parsed");
            return new ParsedLog(_buildVersion, _fightData, _agentData, _skillData, _combatItems, _playerList, _logEndTime - _logStartTime, _parserSettings);
        }

        private void ParseLog(OperationController operation, Stream stream)
        {
            operation.ThrowIfCanceled();
            operation.UpdateProgress("Parsing fight data");
            ParseFightData(stream);
            operation.ThrowIfCanceled();
            operation.UpdateProgress("Parsing agent data");
            ParseAgentData(stream);
            operation.ThrowIfCanceled();
            operation.UpdateProgress("Parsing skill data");
            ParseSkillData(stream);
            operation.ThrowIfCanceled();
            operation.UpdateProgress("Parsing combat list");
            ParseCombatList(stream);
            operation.ThrowIfCanceled();
            operation.UpdateProgress("Linking agents to combat list");
            CompleteAgents();
            operation.ThrowIfCanceled();
            operation.UpdateProgress("Preparing data for log generation");
            PreProcessEvtcData();
            operation.ThrowIfCanceled();
        }

        private static BinaryReader CreateReader(Stream stream)
        {
            return new BinaryReader(stream, new System.Text.UTF8Encoding(), leaveOpen: true);
        }

        /*private bool TryRead(Stream stream, byte[] data)
        {
            int offset = 0;
            int count = data.Length;
            while (count > 0)
            {
                int bytesRead = stream.Read(data, offset, count);
                if (bytesRead == 0)
                {
                    return false;
                }
                offset += bytesRead;
                count -= bytesRead;
            }
            return true;
        }*/

        //sub Parse methods
        /// <summary>
        /// Parses fight related data
        /// </summary>
        private void ParseFightData(Stream stream)
        {
            using (BinaryReader reader = CreateReader(stream))
            {
                // 12 bytes: arc build version
                _buildVersion = ParseHelper.GetString(stream, 12);

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
            using (BinaryReader reader = CreateReader(stream))
            {            // 4 bytes: player count
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
                    string agentProf = GW2APIController.GetAgentProfString(prof, isElite);
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
            using (BinaryReader reader = CreateReader(stream))
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
                    string name = ParseHelper.GetString(stream, 64);
                    //Save
                    var skill = new SkillItem(skillId, name);
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
                isNinety, isFifty, isMoving, isStateChange, isFlanking, isShields, isOffcycle, 0);
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
            // 4 bytes: pad
            uint pad = reader.ReadUInt32();

            //save
            // Add combat
            return new CombatItem(time, srcAgent, dstAgent, value, buffDmg, overstackValue, skillId,
                srcInstid, dstInstid, srcMasterInstid, dstmasterInstid, iff, buff, result, isActivation, isBuffRemove,
                isNinety, isFifty, isMoving, isStateChange, isFlanking, isShields, isOffcycle, pad);
        }

        /// <summary>
        /// Parses combat related data
        /// </summary>
        private void ParseCombatList(Stream stream)
        {
            // 64 bytes: each combat
            using (BinaryReader reader = CreateReader(stream))
            {
                long cbtItemCount = (reader.BaseStream.Length - reader.BaseStream.Position) / 64;
                for (long i = 0; i < cbtItemCount; i++)
                {
                    CombatItem combatItem = _revision > 0 ? ReadCombatItemRev1(reader) : ReadCombatItem(reader);
                    if (!IsValid(combatItem))
                    {
                        continue;
                    }
                    if (combatItem.IsStateChange.HasTime())
                    {
                        if (_logStartTime == 0)
                        {
                            _logStartTime = combatItem.Time;
                        }
                        _logEndTime = combatItem.Time;
                    }
                    _combatItems.Add(combatItem);
                    if (combatItem.IsStateChange == ParseEnum.StateChange.LogEnd)
                    {
                        break;
                    }
                }
            }
            if (!_combatItems.Any())
            {
                throw new InvalidDataException("No combat events found");
            }
        }

        /// <summary>
        /// Returns true if the combat item contains valid data and should be used, false otherwise
        /// </summary>
        /// <param name="combatItem"></param>
        /// <returns>true if the combat item is valid</returns>
        private static bool IsValid(CombatItem combatItem)
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
            return combatItem.IsStateChange != ParseEnum.StateChange.Unknown;
        }
        private static void UpdateAgentData(AgentItem ag, long logTime, ushort instid)
        {
            if (ag.InstID == 0)
            {
                ag.SetInstid(instid);
            }
            if (ag.FirstAware == 0)
            {
                ag.OverrideAwareTimes(logTime, logTime);
            }
            else
            {
                ag.OverrideAwareTimes(ag.FirstAware, logTime);
            }
        }

        private void FindAgentMaster(long logTime, ushort masterInstid, ulong minionAgent)
        {
            AgentItem master = _agentData.GetAgentByInstID(masterInstid, logTime);
            if (master != GeneralHelper.UnknownAgent)
            {
                AgentItem minion = _agentData.GetAgent(minionAgent);
                if (minion != GeneralHelper.UnknownAgent && minion.Master == null) 
                {
                    if (minion.FirstAware <= logTime && logTime <= minion.LastAware)
                    {
                        minion.SetMaster(master);
                    }
                }
            }
        }


        private void CompletePlayers()
        {
            //Fix Disconnected players
            List<AgentItem> playerAgentList = _agentData.GetAgentByType(AgentItem.AgentType.Player);
            bool refresh = false;
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
                        playerAgent.SetInstid(tst.DstInstid);
                        refresh = true;
                    }
                    else
                    {
                        playerAgent.SetInstid(tst.SrcInstid);
                        refresh = true;
                    }
                    playerAgent.OverrideAwareTimes(_logStartTime, _logEndTime);
                }
                bool skip = false;
                var player = new Player(playerAgent, _fightData.Logic.Mode == FightLogic.ParseMode.Fractal, false);
                foreach (Player p in _playerList)
                {
                    if (p.Account == player.Account)// same player
                    {
                        if (p.Character == player.Character) // same character, can be fused
                        {
                            skip = true;
                            ulong agent = p.Agent;
                            foreach (CombatItem c in _combatItems)
                            {
                                if (player.Agent == c.DstAgent && c.IsStateChange.DstIsAgent())
                                {
                                    c.OverrideDstAgent(agent);
                                }
                                if (player.Agent == c.SrcAgent && c.IsStateChange.SrcIsAgent())
                                {
                                    c.OverrideSrcAgent(agent);
                                }
                            }
                            _agentData.SwapMasters(player.AgentItem, p.AgentItem);
                            p.AgentItem.OverrideAwareTimes(Math.Min(p.AgentItem.FirstAware, player.AgentItem.FirstAware), Math.Max(p.AgentItem.LastAware, player.AgentItem.LastAware));
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
            }
            if (_parserSettings.AnonymousPlayer)
            {
                for (int i = 0; i < _playerList.Count; i++)
                {
                    _playerList[i].Anonymize(i + 1);
                }
            }
            _playerList = _playerList.OrderBy(a => a.Group).ToList();
            if (refresh)
            {
                _agentData.Refresh();
            }
        }

        private void CompleteAgents()
        {
            var agentsLookup = _allAgentsList.GroupBy(x => x.Agent).ToDictionary(x => x.Key, x => x.ToList().First());
            //var agentsLookup = _allAgentsList.ToDictionary(x => x.Agent);
            // Set Agent instid, firstAware and lastAware
            foreach (CombatItem c in _combatItems)
            {
                if (c.IsStateChange.SrcIsAgent())
                {
                    if (agentsLookup.TryGetValue(c.SrcAgent, out AgentItem agent))
                    {
                        UpdateAgentData(agent, c.Time, c.SrcInstid);
                    }
                }
                if (c.IsStateChange.DstIsAgent())
                {
                    if (agentsLookup.TryGetValue(c.DstAgent, out AgentItem agent))
                    {
                        UpdateAgentData(agent, c.Time, c.DstInstid);
                    }
                }
            }
            _allAgentsList.RemoveAll(x => !(x.InstID != 0 && x.LastAware - x.FirstAware >= 0 && x.FirstAware != 0 && x.LastAware != long.MaxValue) && (x.Type != AgentItem.AgentType.Player && x.Type != AgentItem.AgentType.EnemyPlayer));
            _agentData = new AgentData(_allAgentsList);

            _fightData = new FightData(_id, _agentData, _logStartTime, _logEndTime);

            CompletePlayers();

            foreach (CombatItem c in _combatItems)
            {
                if (c.SrcMasterInstid != 0)
                {
                    FindAgentMaster(c.Time, c.SrcMasterInstid, c.SrcAgent);
                }
                if (c.DstMasterInstid != 0)
                {
                    FindAgentMaster(c.Time, c.DstMasterInstid, c.DstAgent);
                }
            }
            if (_agentData.GetAgentByType(AgentItem.AgentType.Player).Count == 0)
            {
                throw new InvalidDataException("No players found");
            }
        }

        private void OffsetEvtcData()
        {
            long offset = _fightData.Logic.GetFightOffset(_fightData, _agentData, _combatItems);
            // apply offset to everything
            foreach (CombatItem c in _combatItems)
            {
                c.OverrideTime(c.Time - offset);
            }
            foreach (AgentItem a in _allAgentsList)
            {
                a.OverrideAwareTimes(a.FirstAware - offset, a.LastAware - offset);
            }
        }

        /// <summary>
        /// Pre process evtc data for EI
        /// </summary>
        private void PreProcessEvtcData()
        {
            OffsetEvtcData();
            _fightData.Logic.EIEvtcParse(_fightData, _agentData, _combatItems, _playerList);
            if (!_fightData.Logic.Targets.Any())
            {
                throw new InvalidDataException("No Targets found");
            }
        }
    }
}
