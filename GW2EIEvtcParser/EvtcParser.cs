using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.EncounterLogic;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using GW2EIGW2API;
using GW2EIGW2API.GW2API;
using static GW2EIEvtcParser.ParserHelper;

[assembly: CLSCompliant(false)]
namespace GW2EIEvtcParser
{
    public class EvtcParser
    {

        //Main data storage after binary parse
        private FightData _fightData;
        private AgentData _agentData;
        private readonly List<AgentItem> _allAgentsList;
        private SkillData _skillData;
        private readonly List<CombatItem> _combatItems;
        private List<Player> _playerList;
        private byte _revision;
        private ushort _id;
        private long _logStartTime;
        private long _logEndTime;
        private int _evtcVersion;
        private ulong _gw2Build;
        private readonly EvtcParserSettings _parserSettings;
        private readonly GW2APIController _apiController;
        private readonly Dictionary<uint, AbstractExtensionHandler> _enabledExtensions;

        public EvtcParser(EvtcParserSettings parserSettings, GW2APIController apiController)
        {
            _apiController = apiController;
            _parserSettings = parserSettings;
            _allAgentsList = new List<AgentItem>();
            _combatItems = new List<CombatItem>();
            _playerList = new List<Player>();
            _logStartTime = 0;
            _logEndTime = 0;
            _enabledExtensions = new Dictionary<uint, AbstractExtensionHandler>();
        }

        #region Main Parse Method

        /// <summary>
        /// Parses the given log. <br></br>
        /// On parsing failure, <see cref="ParsingFailureReason"/> will be filled with the reason of the failure and the method will return <see langword="null"/>.
        /// </summary>
        /// <param name="operation">Operation object bound to the UI.</param>
        /// <param name="evtc">The path to the log to parse.</param>
        /// <param name="parsingFailureReason">The reason why the parsing failed, if applicable.</param>
        /// <param name="multiThreadAccelerationForBuffs">Will preprocess buff simulation using multi threading.</param>
        /// <returns>The <see cref="ParsedEvtcLog"/> log.</returns>
        /// <exception cref="EvtcFileException"></exception>
        public ParsedEvtcLog ParseLog(ParserController operation, FileInfo evtc, out ParsingFailureReason parsingFailureReason, bool multiThreadAccelerationForBuffs = false)
        {
            parsingFailureReason = null;
            try
            {
                if (!evtc.Exists)
                {
                    throw new EvtcFileException("File " + evtc.FullName + " does not exist");
                }
                if (!SupportedFileFormats.IsSupportedFormat(evtc.Name))
                {
                    throw new EvtcFileException("Not EVTC");
                }
                ParsedEvtcLog evtcLog;
                using (var fs = new FileStream(evtc.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    if (SupportedFileFormats.IsCompressedFormat(evtc.Name))
                    {
                        using (var arch = new ZipArchive(fs, ZipArchiveMode.Read))
                        {
                            if (arch.Entries.Count != 1)
                            {
                                throw new EvtcFileException("Invalid Archive");
                            }
                            using (Stream data = arch.Entries[0].Open())
                            {
                                using (var ms = new MemoryStream())
                                {
                                    data.CopyTo(ms);
                                    ms.Position = 0;
                                    evtcLog = ParseLog(operation, ms, out parsingFailureReason, multiThreadAccelerationForBuffs);
                                };
                            }
                        }
                    }
                    else
                    {
                        evtcLog = ParseLog(operation, fs, out parsingFailureReason, multiThreadAccelerationForBuffs);
                    }
                }
                return evtcLog;
            }
            catch (Exception ex)
            {
                parsingFailureReason = new ParsingFailureReason(ex);
                return null;
            }
        }

        /// <summary>
        /// Parses the given log. <br></br>
        /// On parsing failure, <see cref="ParsingFailureReason"/> will be filled with the reason of the failure and the method will return <see langword="null"/>.
        /// </summary>
        /// <param name="operation">Operation object bound to the UI.</param>
        /// <param name="evtcStream">The stream of the log.</param>
        /// <param name="parsingFailureReason">The reason why the parsing failed, if applicable.</param>
        /// <param name="multiThreadAccelerationForBuffs">Will preprocess buff simulation using multi threading.</param>
        /// <returns>The <see cref="ParsedEvtcLog"/> log.</returns>
        public ParsedEvtcLog ParseLog(ParserController operation, Stream evtcStream, out ParsingFailureReason parsingFailureReason, bool multiThreadAccelerationForBuffs = false)
        {
            parsingFailureReason = null;
            try
            {
                using (BinaryReader reader = CreateReader(evtcStream))
                {
                    operation.UpdateProgressWithCancellationCheck("Reading Binary");
                    operation.UpdateProgressWithCancellationCheck("Parsing fight data");
                    ParseFightData(reader, operation);
                    operation.UpdateProgressWithCancellationCheck("Parsing agent data");
                    ParseAgentData(reader, operation);
                    operation.UpdateProgressWithCancellationCheck("Parsing skill data");
                    ParseSkillData(reader, operation);
                    operation.UpdateProgressWithCancellationCheck("Parsing combat list");
                    ParseCombatList(reader, operation);
                    operation.UpdateProgressWithCancellationCheck("Linking agents to combat list");
                    CompleteAgents(operation);
                    operation.UpdateProgressWithCancellationCheck("Preparing data for log generation");
                    PreProcessEvtcData(operation);
                    operation.UpdateProgressWithCancellationCheck("Data parsed");
                    var log = new ParsedEvtcLog(_evtcVersion, _fightData, _agentData, _skillData, _combatItems, _playerList, _enabledExtensions, _parserSettings, operation);
                    //
                    if (multiThreadAccelerationForBuffs)
                    {
                        IReadOnlyList<PhaseData> phases = log.FightData.GetPhases(log);
                        operation.UpdateProgressWithCancellationCheck("Multi threading");
                        var friendliesAndTargets = new List<AbstractSingleActor>(log.Friendlies);
                        friendliesAndTargets.AddRange(log.FightData.Logic.Targets);
                        var friendliesAndTargetsAndMobs = new List<AbstractSingleActor>(log.FightData.Logic.TrashMobs);
                        friendliesAndTargetsAndMobs.AddRange(friendliesAndTargets);
                        foreach (AbstractSingleActor actor in friendliesAndTargetsAndMobs)
                        {
                            // that part can't be // due to buff extensions
                            actor.GetTrackedBuffs(log);
                            actor.GetMinions(log);
                        }
                        Parallel.ForEach(friendliesAndTargets, actor => actor.GetStatus(log));
                        /*if (log.CombatData.HasMovementData)
                        {
                            // init all positions
                            Parallel.ForEach(friendliesAndTargetsAndMobs, actor => actor.GetCombatReplayPolledPositions(log));
                        }*/
                        Parallel.ForEach(friendliesAndTargetsAndMobs, actor => actor.GetBuffGraphs(log));
                        Parallel.ForEach(friendliesAndTargets, actor =>
                        {
                            foreach (PhaseData phase in phases)
                            {
                                actor.GetBuffDistribution(log, phase.Start, phase.End);
                            }
                        });
                        Parallel.ForEach(friendliesAndTargets, actor =>
                        {
                            foreach (PhaseData phase in phases)
                            {
                                actor.GetBuffPresence(log, phase.Start, phase.End);
                            }
                        });
                        //
                        //Parallel.ForEach(log.PlayerList, player => player.GetDamageModifierStats(log, null));
                        Parallel.ForEach(log.Friendlies, actor =>
                        {
                            foreach (PhaseData phase in phases)
                            {
                                actor.GetBuffs(BuffEnum.Self, log, phase.Start, phase.End);
                            }
                        });
                        Parallel.ForEach(log.PlayerList, actor =>
                        {
                            foreach (PhaseData phase in phases)
                            {
                                actor.GetBuffs(BuffEnum.Group, log, phase.Start, phase.End);
                            }
                        });
                        Parallel.ForEach(log.PlayerList, actor =>
                        {
                            foreach (PhaseData phase in phases)
                            {
                                actor.GetBuffs(BuffEnum.OffGroup, log, phase.Start, phase.End);
                            }
                        });
                        Parallel.ForEach(log.PlayerList, actor =>
                        {
                            foreach (PhaseData phase in phases)
                            {
                                actor.GetBuffs(BuffEnum.Squad, log, phase.Start, phase.End);
                            }
                        });
                        Parallel.ForEach(log.FightData.Logic.Targets, actor =>
                        {
                            foreach (PhaseData phase in phases)
                            {
                                actor.GetBuffs(BuffEnum.Self, log, phase.Start, phase.End);
                            }
                        });
                    }
                    //
                    return log;
                }
            }
            catch (Exception ex)
            {
                parsingFailureReason = new ParsingFailureReason(ex);
                return null;
            }
        }

        #endregion Main Parse Method

        #region Sub Parse Methods

        /// <summary>
        /// Parses fight related data.
        /// </summary>
        /// <param name="reader">Reads binary values from the evtc.</param>
        /// <param name="operation">Operation object bound to the UI.</param>
        /// <exception cref="EvtcFileException"></exception>
        private void ParseFightData(BinaryReader reader, ParserController operation)
        {
            // 12 bytes: arc build version
            string evtcVersion = GetString(reader, 12);
            if (!evtcVersion.StartsWith("EVTC") || !int.TryParse(new string(evtcVersion.Where(char.IsDigit).ToArray()), out _evtcVersion))
            {
                throw new EvtcFileException("Not EVTC");
            }
            operation.UpdateProgressWithCancellationCheck("ArcDPS Build " + evtcVersion);

            // 1 byte: revision
            _revision = reader.ReadByte();
            operation.UpdateProgressWithCancellationCheck("ArcDPS Combat Item Revision " + _revision);

            // 2 bytes: fight instance ID
            _id = reader.ReadUInt16();
            operation.UpdateProgressWithCancellationCheck("Fight Instance " + _id);
            // 1 byte: skip
            _ = reader.ReadByte();
        }

        /// <summary>
        /// Get the Agent Profession as <see cref="string"/>.
        /// </summary>
        /// <param name="prof"></param>
        /// <param name="elite"></param>
        /// <param name="operation">Operation object bound to the UI.</param>
        /// <returns></returns>
        /// <exception cref="EvtcAgentException"></exception>
        private string GetAgentProfString(uint prof, uint elite, ParserController operation)
        {
            // Non player agents - Gadgets = GDG
            if (elite == 0xFFFFFFFF)
            {
                return (prof & 0xffff0000) == 0xffff0000 ? "GDG" : "NPC";
            }
            // Old way - Base Profession
            else if (elite == 0)
            {
                switch (prof)
                {
                    case 1: return "Guardian";
                    case 2: return "Warrior";
                    case 3: return "Engineer";
                    case 4: return "Ranger";
                    case 5: return "Thief";
                    case 6: return "Elementalist";
                    case 7: return "Mesmer";
                    case 8: return "Necromancer";
                    case 9: return "Revenant";
                    default: return "Unknown";
                }
            }
            // Old way - Elite Specialization (HoT)
            else if (elite == 1)
            {
                switch (prof)
                {
                    case 1: return "Dragonhunter";
                    case 2: return "Berserker";
                    case 3: return "Scrapper";
                    case 4: return "Druid";
                    case 5: return "Daredevil";
                    case 6: return "Tempest";
                    case 7: return "Chronomancer";
                    case 8: return "Reaper";
                    case 9: return "Herald";
                    default: return "Unknown";
                }
            }
            // Current way
            else
            {
                GW2APISpec spec = _apiController.GetAPISpec((int)elite);
                if (spec == null)
                {
                    operation.UpdateProgressWithCancellationCheck("Missing or outdated GW2 API Cache or unknown player spec");
                    return "Unknown";
                }
                return spec.Elite ? spec.Name : spec.Profession;
            }
            throw new EvtcAgentException("Unexpected profession pattern in evtc");
        }

        /// <summary>
        /// Parses agent related data.
        /// </summary>
        /// <param name="reader">Reads binary values from the evtc.</param>
        /// <param name="operation">Operation object bound to the UI.</param>
        private void ParseAgentData(BinaryReader reader, ParserController operation)
        {
            // 4 bytes: player count
            uint agentCount = reader.ReadUInt32();

            operation.UpdateProgressWithCancellationCheck("Agent Count " + agentCount);
            // 96 bytes: each player
            for (int i = 0; i < agentCount; i++)
            {
                // 8 bytes: agent
                ulong agent = reader.ReadUInt64();

                // 4 bytes: profession
                uint prof = reader.ReadUInt32();

                // 4 bytes: is_elite
                uint isElite = reader.ReadUInt32();

                // 2 bytes: toughness
                ushort toughness = reader.ReadUInt16();
                // 2 bytes: healing
                ushort concentration = reader.ReadUInt16();
                // 2 bytes: healing
                ushort healing = reader.ReadUInt16();
                // 2 bytes: hitbox width
                uint hbWidth = (uint)(2 * reader.ReadUInt16());
                // 2 bytes: condition
                ushort condition = reader.ReadUInt16();
                // 2 bytes: hitbox height
                uint hbHeight = (uint)(2 * reader.ReadUInt16());
                // 68 bytes: name
                string name = GetString(reader, 68, false);
                //Save
                Spec agentProf = ProfToSpec(GetAgentProfString(prof, isElite, operation));
                AgentItem.AgentType type;
                ushort ID = 0;
                switch (agentProf)
                {
                    case Spec.NPC:
                        // NPC
                        if (!ushort.TryParse(prof.ToString().PadLeft(5, '0'), out ID))
                        {
                            ID = 0;
                        }
                        type = AgentItem.AgentType.NPC;
                        break;
                    case Spec.Gadget:
                        // Gadget
                        if (!ushort.TryParse((prof & 0x0000ffff).ToString().PadLeft(5, '0'), out ID))
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

        /// <summary>
        /// Parses skill related data
        /// </summary>
        /// <param name="reader">Reads binary values from the evtc.</param>
        /// <param name="operation">Operation object bound to the UI.</param>
        private void ParseSkillData(BinaryReader reader, ParserController operation)
        {
            _skillData = new SkillData(_apiController, _evtcVersion);
            // 4 bytes: player count
            uint skillCount = reader.ReadUInt32();
            operation.UpdateProgressWithCancellationCheck("Skill Count " + skillCount);
            //TempData["Debug"] += "Skill Count:" + skill_count.ToString();
            // 68 bytes: each skill
            for (int i = 0; i < skillCount; i++)
            {
                // 4 bytes: skill ID
                int skillId = reader.ReadInt32();
                // 64 bytes: name
                string name = GetString(reader, 64);
                //Save
                _skillData.Add(skillId, name);
            }
        }

        /// <summary>
        /// Reads the <see cref="CombatItem"/> binary.<br></br>
        /// Old version when header[12] == 0.
        /// </summary>
        /// <param name="reader">Reads binary values from the evtc.</param>
        /// <returns><see cref="CombatItem"/></returns>
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
            _ = reader.ReadBytes(9);

            // 1 byte: iff
            byte iff = reader.ReadByte();

            // 1 byte: buff
            byte buff = reader.ReadByte();

            // 1 byte: result
            byte result = reader.ReadByte();

            // 1 byte: is_activation
            byte isActivation = reader.ReadByte();

            // 1 byte: is_buffremove
            byte isBuffRemove = reader.ReadByte();

            // 1 byte: is_ninety
            byte isNinety = reader.ReadByte();

            // 1 byte: is_fifty
            byte isFifty = reader.ReadByte();

            // 1 byte: is_moving
            byte isMoving = reader.ReadByte();

            // 1 byte: is_statechange
            byte isStateChange = reader.ReadByte();

            // 1 byte: is_flanking
            byte isFlanking = reader.ReadByte();

            // 1 byte: is_flanking
            byte isShields = reader.ReadByte();
            // 1 byte: is_flanking
            byte isOffcycle = reader.ReadByte();
            // 1 bytes: garbage
            _ = reader.ReadByte();

            //save
            // Add combat
            return new CombatItem(time, srcAgent, dstAgent, value, buffDmg, overstackValue, skillId,
                srcInstid, dstInstid, srcMasterInstid, 0, iff, buff, result, isActivation, isBuffRemove,
                isNinety, isFifty, isMoving, isStateChange, isFlanking, isShields, isOffcycle, 0);
        }

        /// <summary>
        /// Reads the <see cref="CombatItem"/> binary.<br></br>
        /// Current version when header[12] == 1.
        /// </summary>
        /// <param name="reader">Reads binary values from the evtc.</param>
        /// <returns><see cref="CombatItem"/></returns>
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
            byte iff = reader.ReadByte();

            // 1 byte: buff
            byte buff = reader.ReadByte();

            // 1 byte: result
            byte result = reader.ReadByte();

            // 1 byte: is_activation
            byte isActivation = reader.ReadByte();

            // 1 byte: is_buffremove
            byte isBuffRemove = reader.ReadByte();

            // 1 byte: is_ninety
            byte isNinety = reader.ReadByte();

            // 1 byte: is_fifty
            byte isFifty = reader.ReadByte();

            // 1 byte: is_moving
            byte isMoving = reader.ReadByte();

            // 1 byte: is_statechange
            byte isStateChange = reader.ReadByte();

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
        /// Parses combat related data.
        /// </summary>
        /// <param name="reader">Reads binary values from the evtc.</param>
        /// <param name="operation">Operation object bound to the UI.</param>
        /// <exception cref="EvtcCombatEventException"></exception>
        /// <exception cref="TooShortException"></exception>
        /// <exception cref="TooLongException"></exception>
        private void ParseCombatList(BinaryReader reader, ParserController operation)
        {
            // 64 bytes: each combat
            long cbtItemCount = (reader.BaseStream.Length - reader.BaseStream.Position) / 64;
            operation.UpdateProgressWithCancellationCheck("Combat Event Count " + cbtItemCount);
            int discardedCbtEvents = 0;
            bool keepOnlyExtensionEvents = false;
            for (long i = 0; i < cbtItemCount; i++)
            {
                CombatItem combatItem = _revision > 0 ? ReadCombatItemRev1(reader) : ReadCombatItem(reader);
                if (!IsValid(combatItem, operation) || (keepOnlyExtensionEvents && !combatItem.IsExtension))
                {
                    discardedCbtEvents++;
                    continue;
                }
                if (combatItem.HasTime())
                {
                    if (_logStartTime == 0)
                    {
                        _logStartTime = combatItem.Time;
                    }
                    _logEndTime = combatItem.Time;
                }
                _combatItems.Add(combatItem);
                if (combatItem.IsStateChange == ArcDPSEnums.StateChange.GWBuild && combatItem.SrcAgent != 0)
                {
                    _gw2Build = combatItem.SrcAgent;
                }
                if (combatItem.IsStateChange == ArcDPSEnums.StateChange.LogEnd)
                {
                    keepOnlyExtensionEvents = true;
                }
            }
            operation.UpdateProgressWithCancellationCheck("Combat Event Discarded " + discardedCbtEvents);
            if (!_combatItems.Any())
            {
                throw new EvtcCombatEventException("No combat events found");
            }
            if (_logEndTime - _logStartTime < _parserSettings.TooShortLimit)
            {
                throw new TooShortException(_logEndTime - _logStartTime, _parserSettings.TooShortLimit);
            }
            // 24 hours
            if (_logEndTime - _logStartTime > 86400000)
            {
                throw new TooLongException();
            }
        }

        /// <summary>
        /// Checks if the <see cref="CombatItem"/> contains valid data and should be used.
        /// </summary>
        /// <param name="combatItem"><see cref="CombatItem"/> data to validate.</param>
        /// <param name="operation">Operation object bound to the UI.</param>
        /// <returns>Returns <see langword="true"/> if the <see cref="CombatItem"/> is valid, otherwise <see langword="false"/>.</returns>
        private bool IsValid(CombatItem combatItem, ParserController operation)
        {
            if (combatItem.IsStateChange == ArcDPSEnums.StateChange.HealthUpdate && combatItem.DstAgent > 20000)
            {
                // DstAgent should be target health % times 100, values higher than 10000 are unlikely. 
                // If it is more than 200% health ignore this record
                return false;
            }
            if (combatItem.IsExtension)
            {
                // Generic versioning check, we expect that the first event that'll be sent by an addon will always be meta data
                // Can't be ExtensionCombat
                if (combatItem.Pad == 0 && combatItem.IsStateChange == ArcDPSEnums.StateChange.Extension)
                {
                    AbstractExtensionHandler handler = ExtensionHelper.GetExtensionHandler(combatItem);
                    if (handler != null)
                    {
                        _enabledExtensions[handler.Signature] = handler;
                        operation.UpdateProgressWithCancellationCheck("Encountered supported extension " + handler.Name + " on " + handler.Version);
                    }
                    // No need to keep that event, it'll be immediately parsed by the handler
                    return false;
                } 
                else
                {
                    return _enabledExtensions.ContainsKey(combatItem.Pad);
                }
            }
            if (combatItem.SrcInstid == 0 && combatItem.DstAgent == 0 && combatItem.SrcAgent == 0 && combatItem.DstInstid == 0 && combatItem.IFF == ArcDPSEnums.IFF.Unknown && !combatItem.IsEffect)
            {
                return false;
            }
            return IsSupportedStateChange(combatItem.IsStateChange);
        }

        /// <summary>
        /// Sets an Agent InstID if not already set and updates its aware times.
        /// </summary>
        /// <param name="ag">Agent to update.</param>
        /// <param name="logTime">Time to set as aware time.</param>
        /// <param name="instid"></param>
        /// <param name="checkInstid"></param>
        /// <returns></returns>
        private static bool UpdateAgentData(AgentItem ag, long logTime, ushort instid, bool checkInstid)
        {       
            if (instid != 0)
            {
                if (ag.InstID == 0)
                {
                    ag.SetInstid(instid);
                }
                else if (checkInstid && ag.InstID != instid)
                {
                    return false;
                }
            }
            
            if (ag.FirstAware == 0)
            {
                ag.OverrideAwareTimes(logTime, logTime);
            }
            else
            {
                ag.OverrideAwareTimes(Math.Min(ag.FirstAware, logTime), Math.Max(ag.LastAware,logTime));
            }
            return true;
        }

        /// <summary>
        /// Find the master of a minion.
        /// </summary>
        /// <param name="logTime">Log time.</param>
        /// <param name="masterInstid">Id of the master.</param>
        /// <param name="minionAgent"></param>
        private void FindAgentMaster(long logTime, ushort masterInstid, ulong minionAgent)
        {
            AgentItem master = _agentData.GetAgentByInstID(masterInstid, logTime);
            if (master != _unknownAgent)
            {
                AgentItem minion = _agentData.GetAgent(minionAgent, logTime);
                if (minion != _unknownAgent && minion.Master == null)
                {
                    minion.SetMaster(master);
                }
            }
        }

        /// <summary>
        /// Complete the players agent data.
        /// </summary>
        /// <param name="operation">Operation object bound to the UI.</param>
        private void CompletePlayers(ParserController operation)
        {
            var toRemove = new HashSet<AgentItem>();
            //Handle squad players
            IReadOnlyList<AgentItem> playerAgentList = _agentData.GetAgentByType(AgentItem.AgentType.Player);
            foreach (AgentItem playerAgent in playerAgentList)
            {
                if (playerAgent.InstID == 0 || playerAgent.FirstAware == 0 || playerAgent.LastAware == long.MaxValue)
                {
                    operation.UpdateProgressWithCancellationCheck("Skipping invalid player");
                    continue;
                }
                bool skip = false;
                var player = new Player(playerAgent, _fightData.Logic.Mode == FightLogic.ParseMode.Instanced5 || _fightData.Logic.Mode == FightLogic.ParseMode.sPvP);
                foreach (Player p in _playerList)
                {
                    if (p.Account == player.Account)// same player
                    {
                        if (p.Character == player.Character) // same character, can be fused
                        {
                            skip = true;
                            toRemove.Add(playerAgent);
                            operation.UpdateProgressWithCancellationCheck("Merging player");
                            RedirectAllEvents(_combatItems, _enabledExtensions, _agentData, player.AgentItem, p.AgentItem);
                            p.AgentItem.OverrideAwareTimes(Math.Min(p.AgentItem.FirstAware, player.AgentItem.FirstAware), Math.Max(p.AgentItem.LastAware, player.AgentItem.LastAware));
                            break;
                        }
                    }
                }
                if (!skip)
                {
                    _playerList.Add(player);
                }
            }
            _playerList = _playerList.OrderBy(a => a.Group).ToList();
            if (_playerList.Exists(x => x.Group == 0))
            {
                _playerList.ForEach(x => x.MakeSquadless());
            }
            uint minToughness = _playerList.Min(x => x.Toughness);
            if (minToughness > 0)
            {
                operation.UpdateProgressWithCancellationCheck("Adjusting player toughness scores");
                uint maxToughness = _playerList.Max(x => x.Toughness);
                foreach (Player p in _playerList)
                {
                    p.AgentItem.OverrideToughness((ushort)Math.Round(10.0 * (p.AgentItem.Toughness - minToughness) / Math.Max(1.0, maxToughness - minToughness)));
                }
            }
            // Handle non squad players
            IReadOnlyList<AgentItem> nonSquadPlayerAgents = _agentData.GetAgentByType(AgentItem.AgentType.NonSquadPlayer);
            if (nonSquadPlayerAgents.Any())
            {
                var encounteredNonSquadPlayerInstIDs = new HashSet<ushort>();
                var teamChangeDict = _combatItems.Where(x => x.IsStateChange == ArcDPSEnums.StateChange.TeamChange).GroupBy(x => x.SrcAgent).ToDictionary(x => x.Key, x => x.ToList());
                //
                IReadOnlyList<AgentItem> squadPlayers = _agentData.GetAgentByType(AgentItem.AgentType.Player);
                ulong greenTeam = ulong.MaxValue;
                var greenTeams = new List<ulong>();
                foreach (AgentItem a in squadPlayers)
                {
                    if (teamChangeDict.TryGetValue(a.Agent, out List<CombatItem> teamChangeList))
                    {
                        greenTeams.AddRange(teamChangeList.Where(x => x.SrcMatchesAgent(a)).Select(x => x.DstAgent));
                    }
                }
                if (greenTeams.Any())
                {
                    greenTeam = greenTeams.GroupBy(x => x).OrderByDescending(x => x.Count()).Select(x => x.Key).First();
                }
                var playersToMerge = new Dictionary<AgentItem, AgentItem>();
                var agentsToPlayersToMerge = new Dictionary<ulong, AgentItem>();

                //
                var uniqueNonSquadPlayers = new List<AgentItem>();
                foreach (AgentItem nonSquadPlayer in nonSquadPlayerAgents)
                {
                    if (teamChangeDict.TryGetValue(nonSquadPlayer.Agent, out List<CombatItem> teamChangeList))
                    {
                        nonSquadPlayer.OverrideIsNotInSquadFriendlyPlayer(teamChangeList.Where(x => x.SrcMatchesAgent(nonSquadPlayer)).Select(x => x.DstAgent).Any(x => x == greenTeam));
                    }
                    if (!encounteredNonSquadPlayerInstIDs.Contains(nonSquadPlayer.InstID))
                    {
                        uniqueNonSquadPlayers.Add(nonSquadPlayer);
                        encounteredNonSquadPlayerInstIDs.Add(nonSquadPlayer.InstID);
                    }
                    else
                    {
                        // we merge
                        AgentItem mainPlayer = uniqueNonSquadPlayers.FirstOrDefault(x => x.InstID == nonSquadPlayer.InstID);
                        playersToMerge[nonSquadPlayer] = mainPlayer;
                        agentsToPlayersToMerge[nonSquadPlayer.Agent] = nonSquadPlayer;
                    }
                }
                if (playersToMerge.Any())
                {
                    foreach (CombatItem c in _combatItems)
                    {
                        if (agentsToPlayersToMerge.TryGetValue(c.SrcAgent, out AgentItem nonSquadPlayer) && c.SrcMatchesAgent(nonSquadPlayer, _enabledExtensions))
                        {
                            AgentItem mainPlayer = playersToMerge[nonSquadPlayer];
                            c.OverrideSrcAgent(mainPlayer.Agent);
                        }
                        if (agentsToPlayersToMerge.TryGetValue(c.DstAgent, out nonSquadPlayer) && c.DstMatchesAgent(nonSquadPlayer, _enabledExtensions))
                        {
                            AgentItem mainPlayer = playersToMerge[nonSquadPlayer];
                            c.OverrideDstAgent(mainPlayer.Agent);
                        }
                    }
                    foreach (KeyValuePair<AgentItem, AgentItem> pair in playersToMerge)
                    {
                        AgentItem nonSquadPlayer = pair.Key;
                        AgentItem mainPlayer = pair.Value;
                        _agentData.SwapMasters(nonSquadPlayer, mainPlayer);
                        mainPlayer.OverrideAwareTimes(Math.Min(nonSquadPlayer.FirstAware, mainPlayer.FirstAware), Math.Max(nonSquadPlayer.LastAware, mainPlayer.LastAware));
                        toRemove.Add(nonSquadPlayer);
                    }
                }
            }
            //
            if (toRemove.Any())
            {
                _agentData.RemoveAllFrom(toRemove);
            }
        }

        /// <summary>
        /// Complete the agents data.
        /// </summary>
        /// <param name="operation">Operation object bound to the UI.</param>
        /// <exception cref="InvalidDataException"></exception>
        /// <exception cref="EvtcAgentException"></exception>
        private void CompleteAgents(ParserController operation)
        {
            var allAgentValues = new HashSet<ulong> ( _combatItems.Where(x => x.SrcIsAgent()).Select(x => x.SrcAgent) );
            allAgentValues.UnionWith(_combatItems.Where(x => x.DstIsAgent()).Select(x => x.DstAgent));
            allAgentValues.ExceptWith(_allAgentsList.Select(x => x.Agent));
            allAgentValues.Remove(0);
            operation.UpdateProgressWithCancellationCheck("Creating " + allAgentValues.Count + " missing agents");
            foreach (ulong missingAgentValue in allAgentValues)
            {
                _allAgentsList.Add(new AgentItem(missingAgentValue, "UNKNOWN " + missingAgentValue, Spec.NPC, 0, AgentItem.AgentType.NPC, 0, 0, 0, 0, 0, 0));
            }
            var agentsLookup = _allAgentsList.GroupBy(x => x.Agent).ToDictionary(x => x.Key, x => x.ToList());
            //var agentsLookup = _allAgentsList.ToDictionary(x => x.Agent);
            // Set Agent instid, firstAware and lastAware
            var invalidCombatItems = new HashSet<CombatItem>();
            foreach (CombatItem c in _combatItems)
            {
                if (c.SrcIsAgent())
                {
                    if (agentsLookup.TryGetValue(c.SrcAgent, out List<AgentItem> agents))
                    {
                        bool updatedAgent = false;
                        foreach (AgentItem agent in agents)
                        {
                            updatedAgent = UpdateAgentData(agent, c.Time, c.SrcInstid, agents.Count > 1);
                            if (updatedAgent)
                            {
                                break;
                            }
                        }
                        // this means that this particular combat item does not point to a proper agent
                        if (!updatedAgent && c.SrcInstid != 0)
                        {
                            invalidCombatItems.Add(c);
                        }
                    }
                }
                if (c.DstIsAgent())
                {
                    if (agentsLookup.TryGetValue(c.DstAgent, out List<AgentItem> agents))
                    {
                        bool updatedAgent = false;
                        foreach (AgentItem agent in agents)
                        {
                            updatedAgent = UpdateAgentData(agent, c.Time, c.DstInstid, agents.Count > 1);
                            if (updatedAgent)
                            {
                                break;
                            }
                        }
                        // this means that this particular combat item does not point to a proper agent
                        if (!updatedAgent && c.DstInstid != 0)
                        {
                            invalidCombatItems.Add(c);
                        }
                    }
                }
            }
            if (invalidCombatItems.Any())
            {
#if DEBUG
                throw new InvalidDataException("Must remove " + invalidCombatItems.Count + " invalid combat items");
#else
                operation.UpdateProgressWithCancellationCheck("Removing " + invalidCombatItems.Count + " invalid combat items");
                _combatItems.RemoveAll(x => invalidCombatItems.Contains(x));
#endif
            }
            _allAgentsList.RemoveAll(x => !(x.LastAware - x.FirstAware >= 0 && x.FirstAware != 0 && x.LastAware != long.MaxValue) && (x.Type != AgentItem.AgentType.Player && x.Type != AgentItem.AgentType.NonSquadPlayer));
            operation.UpdateProgressWithCancellationCheck("Keeping " + _allAgentsList.Count + " agents");
            _agentData = new AgentData(_allAgentsList);

            if (_agentData.GetAgentByType(AgentItem.AgentType.Player).Count == 0)
            {
                throw new EvtcAgentException("No players found");
            }

            operation.UpdateProgressWithCancellationCheck("Linking minions to their masters");
            foreach (CombatItem c in _combatItems)
            {
                if (c.SrcIsAgent() && c.SrcMasterInstid != 0)
                {
                    FindAgentMaster(c.Time, c.SrcMasterInstid, c.SrcAgent);
                }
                if (c.DstIsAgent() && c.DstMasterInstid != 0)
                {
                    FindAgentMaster(c.Time, c.DstMasterInstid, c.DstAgent);
                }
            }

            operation.UpdateProgressWithCancellationCheck("Adjusting minion names");
            foreach (AgentItem agent in _agentData.GetAgentByType(AgentItem.AgentType.NPC))
            {
                if (agent.Master != null)
                {
                    ProfHelper.AdjustMinionName(agent);
                }
            }

            // Adjust extension events if needed
            if (_enabledExtensions.Any())
            {
                operation.UpdateProgressWithCancellationCheck("Adjust extension events");
                foreach (CombatItem combatItem in _combatItems)
                {
                    if (combatItem.IsExtension)
                    {
                        if (_enabledExtensions.TryGetValue(combatItem.Pad, out AbstractExtensionHandler handler))
                        {
                            handler.AdjustCombatEvent(combatItem, _agentData);
                        }
                    }

                }
            }

            _fightData = new FightData(_id, _agentData, _combatItems, _parserSettings, _logStartTime, _logEndTime, _evtcVersion);

            operation.UpdateProgressWithCancellationCheck("Creating players");
            CompletePlayers(operation);
        }

        /// <summary>
        /// Applies the EncounterStart offset to all time based related information (CombatEvent Time, First and Last Awares, etc.).
        /// </summary>
        private void OffsetEvtcData()
        {
            long offset = _fightData.Logic.GetFightOffset(_evtcVersion, _fightData, _agentData, _combatItems);
            // apply offset to everything
            foreach (CombatItem c in _combatItems)
            {
                if (c.HasTime(_enabledExtensions))
                {
                    c.OverrideTime(c.Time - offset);
                }
            }
            foreach (AgentItem a in _allAgentsList)
            {
                a.OverrideAwareTimes(a.FirstAware - offset, a.LastAware - offset);
            }
            //
            _fightData.ApplyOffset(offset);
        }

        /// <summary>
        /// Pre process evtc data for EI.
        /// </summary>
        /// <param name="operation">Operation object bound to the UI.</param>
        /// <exception cref="EvtcAgentException"></exception>
        /// <exception cref="MissingKeyActorsException"></exception>
        private void PreProcessEvtcData(ParserController operation)
        {
            operation.UpdateProgressWithCancellationCheck("Offset time");
            OffsetEvtcData();
            operation.UpdateProgressWithCancellationCheck("Offset of " + (_fightData.FightStartOffset) + " ms added");
            operation.UpdateProgressWithCancellationCheck("Adding environment agent");
            _agentData.AddCustomNPCAgent(_fightData.LogStart, _fightData.LogEnd, "Environment", Spec.NPC, ArcDPSEnums.TrashID.Environment, true);
            // Removal of players present before the fight but not during
            var agentsToRemove = new HashSet<AgentItem>();
            foreach (Player p in _playerList)
            {
                if (p.LastAware < 0)
                {
                    agentsToRemove.Add(p.AgentItem);
                    operation.UpdateProgressWithCancellationCheck("Removing player from player list (gone before fight start)");
                }
            }
            //
            if (_fightData.Logic.Mode == FightLogic.ParseMode.Instanced10)
            {
                foreach (Player p in _playerList)
                {
                    // check for players who have spawned after fight start
                    if (p.FirstAware > 100)
                    {
                        // look for a spawn event close to first aware
                        CombatItem spawnEvent = _combatItems.FirstOrDefault(x => x.IsStateChange == ArcDPSEnums.StateChange.Spawn 
                            && x.SrcMatchesAgent(p.AgentItem) && x.Time <= p.FirstAware + 500);
                        if (spawnEvent != null)
                        {
                            var damageEvents = _combatItems.Where(x => x.IsDamage() && x.SrcMatchesAgent(p.AgentItem)).ToList();
                            if (!damageEvents.Any())
                            {
                                agentsToRemove.Add(p.AgentItem);
                                operation.UpdateProgressWithCancellationCheck("Removing player from player list (spawned after fight start in 10 men content)");
                            }
                        }
                    }
                }
            }
            _playerList.RemoveAll(x => agentsToRemove.Contains(x.AgentItem));
            if (_playerList.Count == 0)
            {
                throw new EvtcAgentException("No valid players");
            }
            //
            operation.UpdateProgressWithCancellationCheck("Encounter specific processing");
            _fightData.Logic.EIEvtcParse(_gw2Build, _evtcVersion, _fightData, _agentData, _combatItems, _enabledExtensions);
            if (!_fightData.Logic.Targets.Any())
            {
                throw new MissingKeyActorsException("No Targets found");
            }
            operation.UpdateProgressWithCancellationCheck("Player count: " + _playerList.Count);
            operation.UpdateProgressWithCancellationCheck("Friendlies count: " + _fightData.Logic.NonPlayerFriendlies.Count);
            operation.UpdateProgressWithCancellationCheck("Targets count: " + _fightData.Logic.Targets.Count);
            operation.UpdateProgressWithCancellationCheck("Trash Mobs count: " + _fightData.Logic.TrashMobs.Count);
        }

        /// <summary>
        /// Read bytes from the <paramref name="reader"/> and convert them to <see cref="string"/>.
        /// </summary>
        /// <param name="reader">Reads binary values from the evtc.</param>
        /// <param name="length">Length of bytes to read.</param>
        /// <param name="nullTerminated"></param>
        /// <returns><see cref="string"/> in <see cref="Encoding.UTF8"/>.</returns>
        private static string GetString(BinaryReader reader, int length, bool nullTerminated = true)
        {
            byte[] bytes = reader.ReadBytes(length);
            if (nullTerminated)
            {
                for (int i = 0; i < length; ++i)
                {
                    if (bytes[i] == 0)
                    {
                        length = i;
                        break;
                    }
                }
            }
            return Encoding.UTF8.GetString(bytes, 0, length);
        }

        /// <summary>
        /// Creates a <see cref="BinaryReader"/> of the <see cref="Stream"/> source.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>Initialized <see cref="BinaryReader"/> in <see cref="UTF8Encoding"/>.</returns>
        private static BinaryReader CreateReader(Stream stream)
        {
            return new BinaryReader(stream, new UTF8Encoding(), leaveOpen: true);
        }

        #endregion Sub Parse Methods

    }
}
