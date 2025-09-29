using System.IO.Compression;
using System.Linq;
using System.Numerics;
using System.Text;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.LogLogic;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using GW2EIGW2API;
using Tracing;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SpeciesIDs;

[assembly: CLSCompliant(false)]
namespace GW2EIEvtcParser;

public class EvtcParser
{

    //Main data storage after binary parse
    private LogData _logData;
    private AgentData _agentData;
    private readonly List<AgentItem> _allAgentsList;
    private SkillData _skillData;
    private readonly List<CombatItem> _combatItems;
    private readonly Dictionary<ulong, ulong> ArcDPSAgentRedirection = [];
    private List<Player> _playerList;
    private byte _revision;
    private ushort _id;
    private long _logStartOffset;
    private long _logEndTime;
    private EvtcVersionEvent _evtcVersion;
    private ulong _gw2Build;
    private readonly EvtcParserSettings _parserSettings;
    private readonly GW2APIController _apiController;
    private readonly Dictionary<uint, ExtensionHandler> _enabledExtensions;

    public EvtcParser(EvtcParserSettings parserSettings, GW2APIController apiController)
    {
        _apiController = apiController;
        _parserSettings = parserSettings;
        _allAgentsList = [];
        _combatItems = [];
        _playerList = [];
        _logStartOffset = long.MinValue;
        _logEndTime = 0;
        _enabledExtensions = [];
    }

    #region Main Parse Method

    /// <summary>
    /// Parses the given log. <br></br>
    /// On parsing failure, <see cref="ParsingFailureReason"/> will be filled with the reason of the failure and the method will return <see langword="null"/>.
    /// </summary>
    /// <param name="operation">Operation object bound to the UI.</param>
    /// <param name="evtc">The path to the log to parse.</param>
    /// <param name="parsingFailureReason">The reason why the parsing failed, if applicable.</param>
    /// <param name="multiThreadAcceleration">Will preprocess buff simulation using multi threading.</param>
    /// <returns>The <see cref="ParsedEvtcLog"/> log.</returns>
    /// <exception cref="EvtcFileException"></exception>
    public ParsedEvtcLog? ParseLog(ParserController operation, FileInfo evtc, out ParsingFailureReason? parsingFailureReason, bool multiThreadAcceleration = false)
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
            ParsedEvtcLog? evtcLog;
            using var fs = new FileStream(evtc.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
            if (SupportedFileFormats.IsCompressedFormat(evtc.Name))
            {
                using var arch = new ZipArchive(fs, ZipArchiveMode.Read);
                if (arch.Entries.Count != 1)
                {
                    throw new EvtcFileException("Invalid Archive");
                }

                using Stream data = arch.Entries[0].Open();
                using var ms = new MemoryStream();
                data.CopyTo(ms);
                ms.Position = 0;
                evtcLog = ParseLog(operation, ms, out parsingFailureReason, multiThreadAcceleration);
            }
            else
            {
                evtcLog = ParseLog(operation, fs, out parsingFailureReason, multiThreadAcceleration);
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
    /// <param name="multiThreadAcceleration">Will preprocess buff simulation using multi threading.</param>
    /// <returns>The <see cref="ParsedEvtcLog"/> log.</returns>
    public ParsedEvtcLog? ParseLog(ParserController operation, Stream evtcStream, out ParsingFailureReason? parsingFailureReason, bool multiThreadAcceleration = false)
    {
        parsingFailureReason = null;
        try
        {
            using BinaryReader reader = CreateReader(evtcStream);
            operation.UpdateProgressWithCancellationCheck("Parsing: Reading Binary");
            operation.UpdateProgressWithCancellationCheck("Parsing: Parsing log data");
            ParseLogData(reader, operation);
            operation.UpdateProgressWithCancellationCheck("Parsing: Parsing agent data");
            ParseAgentData(reader, operation);
            operation.UpdateProgressWithCancellationCheck("Parsing: Parsing skill data");
            ParseSkillData(reader, operation);
            operation.UpdateProgressWithCancellationCheck("Parsing: Parsing combat list");
            ParseCombatList(reader, operation);
            operation.UpdateProgressWithCancellationCheck("Parsing: Linking agents to combat list");
            CompleteAgents(operation);
            operation.UpdateProgressWithCancellationCheck("Parsing: Preparing data for log generation");
            PreProcessEvtcData(operation);
            operation.UpdateProgressWithCancellationCheck("Parsing: Data parsed");
            var log = new ParsedEvtcLog(_evtcVersion, _logData, _agentData, _skillData, _combatItems, _playerList, _enabledExtensions, _parserSettings, operation);

            if (multiThreadAcceleration)
            {
                using var _t = new AutoTrace("Buffs?");

                IReadOnlyList<PhaseData> phases = log.LogData.GetPhases(log);
                operation.UpdateProgressWithCancellationCheck("Parsing: Multi threading");

                var friendliesAndTargets = new List<SingleActor>(log.Friendlies.Count + log.LogData.Logic.Targets.Count);
                friendliesAndTargets.AddRange(log.Friendlies);
                friendliesAndTargets.AddRange(log.LogData.Logic.Targets);
                Trace.TrackAverageStat("friendliesAndTargets", friendliesAndTargets.Count);

                var friendliesAndTargetsAndMobs = new List<SingleActor>(log.LogData.Logic.TrashMobs.Count + friendliesAndTargets.Count);
                friendliesAndTargetsAndMobs.AddRange(log.LogData.Logic.TrashMobs);
                friendliesAndTargetsAndMobs.AddRange(friendliesAndTargets);
                Trace.TrackAverageStat("friendliesAndTargetsAndMobs", friendliesAndTargetsAndMobs.Count);

                var hasEnglobingAgents = friendliesAndTargetsAndMobs.Any(x => x.AgentItem.IsEnglobedAgent);

                _t.Log("Paralell phases");
                foreach (SingleActor actor in friendliesAndTargetsAndMobs)
                {
                    _t.SetAverageTimeStart();
                    // Init cache
                    log.FindActor(actor.EnglobingAgentItem);
                    _t.TrackAverageTime("Find actor cache");
                    actor.ComputeBuffMap(log);
                    _t.TrackAverageTime("Buff Map");
                    actor.GetMinions(log);
                    _t.TrackAverageTime("Minion");
                }
                _t.Log("friendliesAndTargetsAndMobs GetMinions");
                Parallel.ForEach(friendliesAndTargets, actor => actor.GetStatus(log));
                _t.Log("friendliesAndTargets GetStatus");
                if (hasEnglobingAgents)
                {
                    var friendliesAndTargetsEnglobed = friendliesAndTargets
                        .Where(x => x.AgentItem.IsEnglobedAgent);
                    var friendliesAndTargetsEnglobing = friendliesAndTargetsEnglobed
                        .DistinctBy(x => x.EnglobingAgentItem)
                        .Select(x => log.FindActor(x.EnglobingAgentItem));
                    var friendliesAndTargetsNonEnglobed = friendliesAndTargets
                        .Where(x => !x.AgentItem.IsEnglobedAgent);

                    var friendliesAndTargetsAndMobsEnglobed = friendliesAndTargetsAndMobs
                        .Where(x => x.AgentItem.IsEnglobedAgent);
                    var friendliesAndTargetsAndMobsEnglobing = friendliesAndTargetsAndMobsEnglobed
                        .DistinctBy(x => x.EnglobingAgentItem)
                        .Select(x => log.FindActor(x.EnglobingAgentItem));
                    var friendliesAndTargetsAndMobsNonEnglobed = friendliesAndTargetsAndMobs
                        .Where(x => !x.AgentItem.IsEnglobedAgent);

                    Parallel.ForEach(friendliesAndTargetsAndMobsEnglobing, actor => actor.SimulateBuffsAndComputeGraphs(log));
                    Parallel.ForEach(friendliesAndTargetsAndMobsNonEnglobed, actor => actor.SimulateBuffsAndComputeGraphs(log));
                    _t.Log("friendliesAndTargetsAndMobs ComputeBuffGraphs");

                    Parallel.ForEach(friendliesAndTargetsEnglobing, actor =>
                    {
                        var englobeds = friendliesAndTargetsAndMobsEnglobed.Where(x => x.EnglobingAgentItem == actor.AgentItem);
                        foreach (PhaseData phase in phases)
                        {
                            actor.GetBuffDistribution(log, phase.Start, phase.End);
                            foreach (var englobed in englobeds)
                            {
                                englobed.GetBuffDistribution(log, phase.Start, phase.End);
                            }
                        }
                    });
                    Parallel.ForEach(friendliesAndTargetsNonEnglobed, actor =>
                    {
                        foreach (PhaseData phase in phases)
                        {
                            actor.GetBuffDistribution(log, phase.Start, phase.End);
                        }
                    });
                    _t.Log("friendliesAndTargets GetBuffDistribution");

                    Parallel.ForEach(friendliesAndTargetsEnglobing, actor =>
                    {
                        var englobeds = friendliesAndTargetsAndMobsEnglobed.Where(x => x.EnglobingAgentItem == actor.AgentItem);
                        foreach (PhaseData phase in phases)
                        {
                            actor.GetBuffPresence(log, phase.Start, phase.End);
                            foreach (var englobed in englobeds)
                            {
                                englobed.GetBuffPresence(log, phase.Start, phase.End);
                            }
                        }
                    });
                    Parallel.ForEach(friendliesAndTargetsNonEnglobed, actor =>
                    {
                        foreach (PhaseData phase in phases)
                        {
                            actor.GetBuffPresence(log, phase.Start, phase.End);
                        }
                    });
                    _t.Log("friendliesAndTargets GetBuffPresence");

                    Parallel.ForEach(friendliesAndTargetsEnglobing, actor =>
                    {
                        var englobeds = friendliesAndTargetsAndMobsEnglobed.Where(x => x.EnglobingAgentItem == actor.AgentItem);
                        actor.GetBuffGraphs(log);
                        foreach (var englobed in englobeds)
                        {
                            englobed.GetBuffGraphs(log);
                        }
                    });
                    _t.Log("friendliesAndTargetsAndMobs englobed ComputeBuffGraphs");
                }
                else
                {
                    Parallel.ForEach(friendliesAndTargetsAndMobs, actor => actor.SimulateBuffsAndComputeGraphs(log));
                    _t.Log("friendliesAndTargetsAndMobs ComputeBuffGraphs");

                    Parallel.ForEach(friendliesAndTargets, actor =>
                    {
                        foreach (PhaseData phase in phases)
                        {
                            actor.GetBuffDistribution(log, phase.Start, phase.End);
                        }
                    });
                    _t.Log("friendliesAndTargets GetBuffDistribution");

                    Parallel.ForEach(friendliesAndTargets, actor =>
                    {
                        foreach (PhaseData phase in phases)
                        {
                            actor.GetBuffPresence(log, phase.Start, phase.End);
                        }
                    });
                    _t.Log("friendliesAndTargets GetBuffPresence");
                }
                Parallel.ForEach(log.Friendlies, actor =>
                {
                    foreach (PhaseData phase in phases)
                    {
                        // To create the caches
                        foreach (var p in log.PlayerList)
                        {
                            actor.GetBuffApplyEventsOnByIDInternal(log, phase.Start, phase.End, 0, p);
                            actor.GetBuffRemoveAllEventsByByIDInternal(log, phase.Start, phase.End, 0, p);
                            actor.GetBuffRemoveAllEventsFromByIDInternal(log, phase.Start, phase.End, 0, p);
                        }
                    }
                });
                _t.Log("Friendlies accelerator caches");
                //
                //Parallel.ForEach(log.PlayerList, player => player.GetDamageModifierStats(log, null));
                Parallel.ForEach(log.Friendlies, actor =>
                {
                    foreach (PhaseData phase in phases)
                    {
                        actor.GetBuffs(BuffEnum.Self, log, phase.Start, phase.End);
                    }
                });
                Parallel.ForEach(log.Friendlies, actor =>
                {
                    foreach (PhaseData phase in phases)
                    {
                        actor.GetBuffVolumes(BuffEnum.Self, log, phase.Start, phase.End);
                    }
                });
                _t.Log("Friendlies GetBuffs Self");
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
                        actor.GetBuffVolumes(BuffEnum.Group, log, phase.Start, phase.End);
                    }
                });
                _t.Log("PlayerList GetBuffs Group");
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
                        actor.GetBuffVolumes(BuffEnum.OffGroup, log, phase.Start, phase.End);
                    }
                });
                _t.Log("PlayerList GetBuffs OffGroup");
                Parallel.ForEach(log.PlayerList, actor =>
                {
                    foreach (PhaseData phase in phases)
                    {
                        actor.GetBuffs(BuffEnum.Squad, log, phase.Start, phase.End);
                    }
                });
                Parallel.ForEach(log.PlayerList, actor =>
                {
                    foreach (PhaseData phase in phases)
                    {
                        actor.GetBuffVolumes(BuffEnum.Squad, log, phase.Start, phase.End);
                    }
                });
                _t.Log("PlayerList GetBuffs Squad");
                Parallel.ForEach(log.LogData.Logic.Targets, actor =>
                {
                    foreach (PhaseData phase in phases)
                    {
                        actor.GetBuffs(BuffEnum.Self, log, phase.Start, phase.End);
                    }
                });
                _t.Log("LogData.Logic.Targets GetBuffs Self");
                Parallel.ForEach(log.Friendlies, actor =>
                {
                    // To initialize cache
                    foreach (PhaseData phase in phases)
                    {
                        actor.GetDamageEvents(null, log, phase.Start, phase.End);
                        foreach (var target in log.LogData.Logic.Targets)
                        {
                            actor.GetDamageEvents(target, log, phase.Start, phase.End);
                        }
                    }
                });
                _t.Log("PlayerList GetDamageEvents");
            }

            return log;
        }
        catch (Exception ex)
        {
#if DEBUG
            Console.Error.WriteLine(ex);
#endif

            parsingFailureReason = new ParsingFailureReason(ex);
            return null;
        }
    }

    #endregion Main Parse Method

    #region Sub Parse Methods

    /// <summary>
    /// Parses high level log related data.
    /// </summary>
    /// <param name="reader">Reads binary values from the evtc.</param>
    /// <param name="operation">Operation object bound to the UI.</param>
    /// <exception cref="EvtcFileException"></exception>
    private void ParseLogData(BinaryReader reader, ParserController operation)
    {
        using var _t = new AutoTrace("Log Data");
        // 12 bytes: arc build version
        using var evtcVersion = GetCharArrayPooled(reader, 12, false);
        if (!MemoryExtensions.StartsWith(evtcVersion, "EVTC".AsSpan()) || !int.TryParse(evtcVersion[4..], out int headerVersion))
        {
            throw new EvtcFileException("Not EVTC");
        }
        _evtcVersion = new EvtcVersionEvent(headerVersion);
        operation.UpdateProgressWithCancellationCheck("Parsing: ArcDPS Build " + evtcVersion.AsSpan().ToString());

        // 1 byte: revision
        _revision = reader.ReadByte();
        operation.UpdateProgressWithCancellationCheck("Parsing: ArcDPS Combat Item Revision " + _revision);

        // 2 bytes: log ID
        _id = reader.ReadUInt16();
        operation.UpdateProgressWithCancellationCheck("Parsing: Trigger ID " + _id);
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
        string spec = _apiController.GetSpec(prof, elite);
        if (spec == GW2APIController.UNKNOWN_SPEC)
        {
            operation.UpdateProgressWithCancellationCheck("Parsing: Missing or outdated GW2 API Cache or unknown player spec");
        }
        return spec;
    }

    /// <summary>
    /// Parses agent related data.
    /// </summary>
    /// <param name="reader">Reads binary values from the evtc.</param>
    /// <param name="operation">Operation object bound to the UI.</param>
    private void ParseAgentData(BinaryReader reader, ParserController operation)
    {
        using var _t = new AutoTrace("Agent Data");
        // 4 bytes: player count
        uint agentCount = reader.ReadUInt32();

        operation.UpdateProgressWithCancellationCheck("Parsing: Agent Count " + agentCount);
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
            Spec agentProf = ProfToSpec(GetAgentProfString(prof, isElite, operation)); //TODO(Rennorb) @perf: Drop the 3 wrappers around what we are actually doing here.
            AgentItem.AgentType type;
            ushort ID = 0;
            switch (agentProf)
            {
                case Spec.NPC:
                    ID = (ushort)(prof > ushort.MaxValue ? 0 : prof);
                    type = AgentItem.AgentType.NPC;
                    break;

                case Spec.Gadget:
                    ID = (ushort)(prof & 0x0000ffff);
                    type = AgentItem.AgentType.Gadget;
                    break;

                default:
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
        using var _t = new AutoTrace("Skill Data");
        _skillData = new SkillData(_apiController, _evtcVersion);
        // 4 bytes: player count
        uint skillCount = reader.ReadUInt32();
        operation.UpdateProgressWithCancellationCheck("Parsing: Skill Count " + skillCount);
        //TempData["Debug"] += "Skill Count:" + skill_count.ToString();
        // 68 bytes: each skill
        for (int i = 0; i < skillCount; i++)
        {
            // 4 bytes: skill ID
            int skillID = reader.ReadInt32();
            // 64 bytes: name
            string name = GetString(reader, 64);
            //Save
            _skillData.Add(skillID, name);
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
        ushort skillID = reader.ReadUInt16();

        // 2 bytes: src_instid
        ushort srcInstid = reader.ReadUInt16();

        // 2 bytes: dst_instid
        ushort dstInstid = reader.ReadUInt16();

        // 2 bytes: src_master_instid
        ushort srcMasterInstid = reader.ReadUInt16();

        // 9 bytes: garbage
        //NOTE(Rennorb): Avoid allocating array by splitting up the reads.
        _ = reader.ReadInt64();
        _ = reader.ReadByte();

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
        return new CombatItem(time, srcAgent, dstAgent, value, buffDmg, overstackValue, skillID,
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
        uint skillID = reader.ReadUInt32();

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
        return new CombatItem(time, srcAgent, dstAgent, value, buffDmg, overstackValue, skillID,
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
        using var _t = new AutoTrace("Combat List");
        // 64 bytes: each combat
        long cbtItemCount = (reader.BaseStream.Length - reader.BaseStream.Position) / 64;
        operation.UpdateProgressWithCancellationCheck("Parsing: Combat Event Count " + cbtItemCount);
        int discardedCbtEvents = 0;
        bool keepOnlyExtensionEvents = false;
        int stopAtLogEndEvent = _id == (int)TargetID.Instance ? 1 : -1;
        var extensionEvents = new List<CombatItem>(5000);
        int mapID = -1;
        int currentMapID = -1;
        for (long i = 0; i < cbtItemCount; i++)
        {
            CombatItem combatItem = _revision > 0 ? ReadCombatItemRev1(reader) : ReadCombatItem(reader);
            if (stopAtLogEndEvent == -1 && 
                combatItem.IsStateChange == StateChange.SquadCombatStart)           
            {
                if (SquadCombatStartEvent.GetLogType(combatItem) == LogType.Map)
                {
                    _id = (int)TargetID.Instance;
                    operation.UpdateProgressWithCancellationCheck("Parsing: Correcting boss log to instance log");
                    stopAtLogEndEvent = 1;
                } 
                else
                {
                    stopAtLogEndEvent = 0;
                }
            }
            if (combatItem.IsStateChange == StateChange.MapID)
            {
                mapID = MapIDEvent.GetMapID(combatItem);
                currentMapID = mapID;
            }
            if (combatItem.IsStateChange == StateChange.MapChange)
            {
                currentMapID = MapIDEvent.GetMapID(combatItem);
            }
            if (combatItem.IsStateChange == StateChange.AgentChange)
            {
                ArcDPSAgentRedirection[combatItem.SrcAgent] = combatItem.DstAgent;
            }
            if (!IsValid(combatItem, mapID, currentMapID, operation) || (keepOnlyExtensionEvents && !combatItem.IsExtension))
            {
                discardedCbtEvents++;
                continue;
            }

            if (combatItem.IsStateChange == StateChange.ArcBuild)
            {
                EvtcVersionEvent oldEvent = _evtcVersion;
                try
                {
                    _evtcVersion = new EvtcVersionEvent(combatItem);
                }
                catch
                {
                    _evtcVersion = oldEvent;
                }

                continue;
            }

            if (combatItem.HasTime())
            {
                if (_logStartOffset == long.MinValue)
                {
                    _logStartOffset = combatItem.Time;
                }
                combatItem.OverrideTime(combatItem.Time - _logStartOffset);
                _logEndTime = combatItem.Time;
            }

            _combatItems.Add(combatItem);
            if (combatItem.IsExtension)
            {
                extensionEvents.Add(combatItem);
            }

            if (combatItem.IsStateChange == StateChange.GWBuild && GW2BuildEvent.GetBuild(combatItem) != 0)
            {
                _gw2Build = GW2BuildEvent.GetBuild(combatItem);
            }

            if (combatItem.IsStateChange == StateChange.SquadCombatEnd && stopAtLogEndEvent <= 0 )
            {
                keepOnlyExtensionEvents = true;
            }
        }
        extensionEvents.ForEach(x =>
        {
            if (x.HasTime(_enabledExtensions))
            {
                x.OverrideTime(x.Time - _logStartOffset);
            }
        });
        operation.UpdateProgressWithCancellationCheck("Parsing: Combat Event Discarded " + discardedCbtEvents);
        if (_combatItems.Count == 0)
        {
            throw new EvtcCombatEventException("No combat events found");
        }
        if (_logEndTime < _parserSettings.TooShortLimit)
        {
            throw new TooShortException(_logEndTime, _parserSettings.TooShortLimit);
        }
        // 24 hours
        if (_logEndTime > 86400000)
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
    private bool IsValid(CombatItem combatItem, long expectedMapID, long currentMapID, ParserController operation)
    {
        if (expectedMapID != -1 && expectedMapID != currentMapID && (combatItem.SrcIsAgent(_enabledExtensions) || combatItem.DstIsAgent(_enabledExtensions)))
        {
            // ignore events linked to an agent that are not on current map
            return false;
        }
        if (combatItem.IsStateChange == StateChange.HealthUpdate && HealthUpdateEvent.GetHealthPercent(combatItem) > 200)
        {
            // DstAgent should be target health % times 100, values higher than 10000 are unlikely. 
            // If it is more than 200% health ignore this record
            return false;
        }
        if (combatItem.IsExtension)
        {
            // Generic versioning check, we expect that the first event that'll be sent by an addon will always be meta data
            // Can't be ExtensionCombat
            if (combatItem.Pad == 0 && combatItem.IsStateChange == StateChange.Extension)
            {
                ExtensionHandler? handler = ExtensionHelper.GetExtensionHandler(combatItem);
                if (handler != null)
                {
                    _enabledExtensions[handler.Signature] = handler;
                    operation.UpdateProgressWithCancellationCheck("Parsing: Encountered supported extension " + handler.Name + " on " + handler.Version);
                }
                // No need to keep that event, it'll be immediately parsed by the handler
                return false;
            }
            else
            {
                return _enabledExtensions.ContainsKey(combatItem.Pad);
            }
        }
        if (combatItem.SrcInstid == 0 && combatItem.DstAgent == 0 && combatItem.SrcAgent == 0 && combatItem.DstInstid == 0 && combatItem.IFF == IFF.Unknown && !combatItem.IsEffect && !combatItem.IsMissile)
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

        if (ag.LastAware == long.MaxValue)
        {
            ag.OverrideAwareTimes(logTime, logTime);
        }
        else
        {
            ag.OverrideAwareTimes(Math.Min(ag.FirstAware, logTime), Math.Max(ag.LastAware, logTime));
        }
        return true;
    }

    /// <summary>
    /// Find the master of a minion.
    /// </summary>
    /// <param name="logTime">Log time.</param>
    /// <param name="masterInstid">SpeciesID of the master.</param>
    /// <param name="minionAgent"></param>
    private void FindAgentMaster(long logTime, ushort masterInstid, ulong minionAgent)
    {
        AgentItem master = _agentData.GetAgentByInstID(masterInstid, logTime);
        if (!master.IsUnknown)
        {
            AgentItem minion = _agentData.GetAgent(minionAgent, logTime);
            if (!minion.IsUnknown)
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
        //Create squad players
        var noSquads = _logData.Logic.ParseMode == LogLogic.LogLogic.ParseModeEnum.Instanced5 || _logData.Logic.ParseMode == LogLogic.LogLogic.ParseModeEnum.sPvP;
        IReadOnlyList<AgentItem> playerAgentList = _agentData.GetAgentByType(AgentItem.AgentType.Player);
        foreach (AgentItem playerAgent in playerAgentList)
        {
            if (playerAgent.InstID == 0 || playerAgent.LastAware == long.MaxValue)
            {
                operation.UpdateProgressWithCancellationCheck("Parsing: Skipping invalid player");
                continue;
            }
            var player = new Player(playerAgent, noSquads);
            _playerList.Add(player);
        }
        if (_playerList.Count == 0)
        {
            throw new EvtcAgentException("No valid players");
        }
        if (_playerList.Exists(x => x.Group == 0))
        {
            _playerList.ForEach(x => x.MakeSquadless());
        }
        _playerList = _playerList.OrderBy(a => a.Character).ToList();
        if (_parserSettings.AnonymousPlayers)
        {
            operation.UpdateProgressWithCancellationCheck("Parsing: Anonymous players");
            for (int i = 0; i < _playerList.Count; i++)
            {
                _playerList[i].Anonymize(i + 1);
            }
            var allPlayerAgents = _agentData.GetAgentByType(AgentItem.AgentType.Player).ToList();
            allPlayerAgents.AddRange(_agentData.GetAgentByType(AgentItem.AgentType.NonSquadPlayer));
            var playerAgents = new HashSet<AgentItem>(_playerList.Select(x => x.AgentItem));
            playerAgents.UnionWith(playerAgents.Select(x => x.EnglobingAgentItem).ToList());
            int playerOffset = _playerList.Count + 1;
            foreach (AgentItem playerAgent in allPlayerAgents.OrderBy(x => x.InstID))
            {
                if (!playerAgents.Contains(playerAgent))
                {
                    if (playerAgent.Type == AgentItem.AgentType.Player)
                    {
                        new Player(playerAgent, noSquads).Anonymize(playerOffset++);
                    }
                    else
                    {
                        new PlayerNonSquad(playerAgent).Anonymize(playerOffset++);
                    }
                }
                foreach (var regrouped in playerAgent.Regrouped)
                {
                    if (!playerAgents.Contains(regrouped.Merged))
                    {
                        if (regrouped.Merged.Type == AgentItem.AgentType.Player)
                        {
                            new Player(regrouped.Merged, noSquads).Anonymize(playerOffset++);
                        }
                        else
                        {
                            new PlayerNonSquad(regrouped.Merged).Anonymize(playerOffset++);
                        }
                    }
                }
                foreach (var merge in playerAgent.Merges)
                {
                    if (!playerAgents.Contains(merge.Merged))
                    {
                        if (merge.Merged.Type == AgentItem.AgentType.Player)
                        {
                            new Player(merge.Merged, noSquads).Anonymize(playerOffset++);
                        }
                        else
                        {
                            new PlayerNonSquad(merge.Merged).Anonymize(playerOffset++);
                        }
                    }
                }
            }
        }
        uint minToughness = _playerList.Min(x => x.Toughness);
        if (minToughness > 0)
        {
            operation.UpdateProgressWithCancellationCheck("Parsing: Adjusting player toughness scores");
            uint maxToughness = _playerList.Max(x => x.Toughness);
            foreach (Player p in _playerList)
            {
                p.AgentItem.OverrideToughness((ushort)Math.Round(10.0 * (p.AgentItem.Toughness - minToughness) / Math.Max(1.0, maxToughness - minToughness)));
            }
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
        using var _t = new AutoTrace("Linking Agents to list");
        var allAgentValues = new HashSet<ulong>(
            _combatItems.Where(x => x.SrcIsAgent())
            .Select(x => x.SrcAgent)
            .Concat(_combatItems.Where(x => x.DstIsAgent())
                    .Select(x => x.DstAgent)
                    )
        );
        allAgentValues.ExceptWith(_allAgentsList.Select(x => x.Agent));
        allAgentValues.Remove(0);
        operation.UpdateProgressWithCancellationCheck("Parsing: Creating " + allAgentValues.Count + " missing agents");
        foreach (ulong missingAgentValue in allAgentValues)
        {
            _allAgentsList.Add(new AgentItem(missingAgentValue, "UNKNOWN " + missingAgentValue, Spec.NPC, NonIdentifiedSpecies, AgentItem.AgentType.NPC, 0, 0, 0, 0, 0, 0));
        }
        var agentsLookup = _allAgentsList.GroupBy(x => x.Agent).ToDictionary(x => x.Key, x => {
            var res = x.ToList();
            res.SortByFirstAware();
            return res;
        });
        //var agentsLookup = _allAgentsList.ToDictionary(x => x.Agent);
        // Set Agent instid, firstAware and lastAware
        var invalidSrcCombatItems = new HashSet<CombatItem>();
        var invalidDstCombatItems = new HashSet<CombatItem>();
        var orphanedSrcInstidCombatItems = new List<CombatItem>();
        var orphanedDstInstidCombatItems = new List<CombatItem>();
        foreach (CombatItem c in _combatItems)
        {
            if (c.SrcIsAgent())
            {
                if (ArcDPSAgentRedirection.TryGetValue(c.SrcAgent, out ulong newAgent))
                {
                    c.OverrideSrcAgent(newAgent);
                }
                if (agentsLookup.TryGetValue(c.SrcAgent, out var agents))
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
                        invalidSrcCombatItems.Add(c);
                    }
                } 
                else if (c.SrcInstid > 0)
                {
                    orphanedSrcInstidCombatItems.Add(c);
                }
            }
            if (c.DstIsAgent())
            {
                if (ArcDPSAgentRedirection.TryGetValue(c.DstAgent, out ulong newAgent))
                {
                    c.OverrideDstAgent(newAgent);
                }
                if (agentsLookup.TryGetValue(c.DstAgent, out var agents))
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
                        invalidDstCombatItems.Add(c);
                    }
                } 
                else if (c.DstInstid > 0)
                {
                    orphanedDstInstidCombatItems.Add(c);
                }
            }
        }
        if (orphanedSrcInstidCombatItems.Count > 0 || orphanedDstInstidCombatItems.Count > 0)
        {
            var agentsInstidLookup = _allAgentsList.GroupBy(x => x.InstID).ToDictionary(x => x.Key, x => {
                var res = x.ToList();
                res.SortByFirstAware();
                return res;
            });
            foreach (var c in orphanedSrcInstidCombatItems)
            {
                if (agentsInstidLookup.TryGetValue(c.SrcInstid, out var candidates))
                {
                    var candidate = candidates.FirstOrDefault(x => x.InAwareTimes(c.Time - 300) || x.InAwareTimes(c.Time + 300));
                    if (candidate != null)
                    {
                        c.OverrideSrcAgent(candidate.Agent);
                        UpdateAgentData(candidate, c.Time, 0, false);
                    }
                }
            }
            foreach (var c in orphanedDstInstidCombatItems)
            {
                if (agentsInstidLookup.TryGetValue(c.DstInstid, out var candidates))
                {
                    var candidate = candidates.FirstOrDefault(x => x.InAwareTimes(c.Time - 300) || x.InAwareTimes(c.Time + 300));
                    if (candidate != null)
                    {
                        c.OverrideDstAgent(candidate.Agent);
                        UpdateAgentData(candidate, c.Time, 0, false);
                    }
                }
            }
        }
        var invalidCombatItems = invalidSrcCombatItems.Intersect(invalidDstCombatItems).ToHashSet();
        if (invalidCombatItems.Count != 0)
        {
#if DEBUG2
            throw new InvalidDataException("Must remove " + invalidCombatItems.Count + " invalid combat items");
#else
            operation.UpdateProgressWithCancellationCheck("Removing " + invalidCombatItems.Count + " invalid combat items");
            _combatItems.RemoveAll(invalidCombatItems.Contains);
#endif
        }
        _allAgentsList.RemoveAll(x => !(x.LastAware != long.MaxValue && x.LastAware - x.FirstAware >= 0));
        operation.UpdateProgressWithCancellationCheck("Parsing: Keeping " + _allAgentsList.Count + " agents");
        _agentData = new AgentData(_apiController, _allAgentsList);
        operation.UpdateProgressWithCancellationCheck("Parsing: Adding environment agent");
        _agentData.AddCustomNPCAgent(0, _logEndTime, "Environment", Spec.NPC, TargetID.Environment, true);

        // Adjust extension events if needed
        if (_enabledExtensions.Count != 0)
        {
            operation.UpdateProgressWithCancellationCheck("Parsing: Adjusting extension events");
            foreach (CombatItem combatItem in _combatItems)
            {
                if (combatItem.IsExtension)
                {
                    if (_enabledExtensions.TryGetValue(combatItem.Pad, out var handler))
                    {
                        handler.AdjustCombatEvent(combatItem, _agentData);
                    }
                }

            }
        }

        operation.UpdateProgressWithCancellationCheck("Parsing: Linking minions to their masters");
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

        operation.UpdateProgressWithCancellationCheck("Parsing: Regrouping Agents");
        AgentManipulationHelper.RegroupSameAgentsAndDetermineTeams(_agentData, _combatItems, _evtcVersion, _enabledExtensions);

        if (_agentData.GetAgentByType(AgentItem.AgentType.Player).Count == 0)
        {
            throw new EvtcAgentException("No players found");
        }

        operation.UpdateProgressWithCancellationCheck("Parsing: Adjusting minion names");
        foreach (AgentItem agent in _agentData.GetAgentByType(AgentItem.AgentType.NPC))
        {
            if (agent.Master != null)
            {
                ProfHelper.AdjustMinionName(agent);
            }
        }

        _logData = new LogData(_id, _agentData, _combatItems, _parserSettings, _logStartOffset, _logEndTime, _evtcVersion);
        bool splitByEnterCombatEvents = _logData.Logic.IsInstance || _logData.Logic.ParseMode == LogLogic.LogLogic.ParseModeEnum.WvW || _logData.Logic.ParseMode == LogLogic.LogLogic.ParseModeEnum.OpenWorld;
        if (splitByEnterCombatEvents || _agentData.GetAgentByType(AgentItem.AgentType.Player).Any(x => x.Regrouped.Count > 0))
        {
            var enterAndExitCombatEvents = _combatItems.Where(x => x.IsStateChange == StateChange.EnterCombat || x.IsStateChange == StateChange.ExitCombat);
            var enterCombatEvents = enterAndExitCombatEvents.Where(x => x.IsStateChange == StateChange.EnterCombat).Select(x => new EnterCombatEvent(x, _agentData)).GroupBy(x => x.Src).ToDictionary(x => x.Key, x => x.ToList());
            var exitCombatEvents = enterAndExitCombatEvents.Where(x => x.IsStateChange == StateChange.ExitCombat).Select(x => new ExitCombatEvent(x, _agentData)).GroupBy(x => x.Src).ToDictionary(x => x.Key, x => x.ToList());
            operation.UpdateProgressWithCancellationCheck("Parsing: Splitting players per spec and subgroup");
            foreach (var playerAgentItem in _agentData.GetAgentByType(AgentItem.AgentType.Player))
            {
                if (enterCombatEvents.TryGetValue(playerAgentItem, out var enterCombatEventsForAgent))
                {
                    if (exitCombatEvents.TryGetValue(playerAgentItem, out var exitCombatEventsForAgent))
                    {
                        AgentManipulationHelper.SplitPlayerPerSpecSubgroupAndSwap(enterCombatEventsForAgent, exitCombatEventsForAgent, _enabledExtensions, _agentData, playerAgentItem, splitByEnterCombatEvents);
                    } 
                    else 
                    {    
                        AgentManipulationHelper.SplitPlayerPerSpecSubgroupAndSwap(enterCombatEventsForAgent, [], _enabledExtensions, _agentData, playerAgentItem, splitByEnterCombatEvents);
                    }
                } 
                else
                {
                    AgentManipulationHelper.SplitPlayerPerSpecSubgroupAndSwap([], [], _enabledExtensions, _agentData, playerAgentItem, splitByEnterCombatEvents);
                }
            }
        }


        operation.UpdateProgressWithCancellationCheck("Parsing: Creating players");
        CompletePlayers(operation);
    }

    /// <summary>
    /// Applies the EncounterStart offset to all time based related information (CombatEvent Time, First and Last Awares, etc.).
    /// </summary>
    private void OffsetEvtcData()
    {
        long offset = _logData.Logic.GetLogOffset(_evtcVersion, _logData, _agentData, _combatItems);
        if (offset == 0)
        {
            return;
        }
        // apply offset to everything
        foreach (CombatItem c in _combatItems)
        {
            if (c.HasTime(_enabledExtensions))
            {
                c.OverrideTime(c.Time - offset);
            }
        }
        _agentData.ApplyOffset(offset);

        _logData.ApplyOffset(offset);
    }

    /// <summary>
    /// Pre process evtc data for EI.
    /// </summary>
    /// <param name="operation">Operation object bound to the UI.</param>
    /// <exception cref="EvtcAgentException"></exception>
    /// <exception cref="MissingKeyActorsException"></exception>
    private void PreProcessEvtcData(ParserController operation)
    {
        using var _t = new AutoTrace("Prepare Data for output");
        operation.UpdateProgressWithCancellationCheck("Parsing: Identifying critical agents");
        _logData.Logic.HandleCriticalAgents(_evtcVersion, _logData, _agentData, _combatItems, _enabledExtensions);
        operation.UpdateProgressWithCancellationCheck("Parsing: Offseting time");
        OffsetEvtcData();
        operation.UpdateProgressWithCancellationCheck("Parsing: Offset of " + (_logData.LogStartOffset) + " ms added");

        operation.UpdateProgressWithCancellationCheck("Parsing: Encounter specific processing");
        _logData.Logic.EIEvtcParse(_gw2Build, _evtcVersion, _logData, _agentData, _combatItems, _enabledExtensions);
        if (!_logData.Logic.Targets.Any())
        {
            throw new MissingKeyActorsException("No Targets found");
        }
    }

    /// <summary>
    /// Read bytes from the <paramref name="reader"/> and convert them to <see cref="string"/>.
    /// </summary>
    /// <param name="reader">Reads binary values from the evtc.</param>
    /// <param name="length">Length of bytes to read.</param>
    /// <param name="nullTerminated">if set the string will become truncated at the first null byte</param>
    /// <returns><see cref="string"/> in <see cref="Encoding.UTF8"/>.</returns>
    private static string GetString(BinaryReader reader, int length, bool nullTerminated = true)
    {
        using var buffer = GetByteArrayPooled(reader, length, nullTerminated);
        return Encoding.UTF8.GetString(buffer);
    }

    /// <summary>
    /// Read bytes from the <paramref name="reader"/>.
    /// The returned value should be disposed at some point to return it to the pool.
    /// </summary>
    /// <param name="reader">Reads binary values from the evtc.</param>
    /// <param name="length">Length of bytes to read.</param>
    /// <param name="nullTerminated">if set the array will become truncated at the first null byte</param>
    private static ArrayPoolReturner<byte> GetByteArrayPooled(BinaryReader reader, int length, bool nullTerminated = true)
    {
        var buffer = new ArrayPoolReturner<byte>(length); // TODO use own reader for direct access 
        buffer.Length = 0; // reuse the length, don't need to remember the original
        while (length-- > 0)
        {
            var b = buffer.Array[buffer.Length] = reader.ReadByte();
            if (nullTerminated && b == 0) { break; }
            buffer.Length++;
        }
        // consume remaining length
        for (; length > sizeof(UInt64); length -= sizeof(UInt64))
        {
            reader.ReadUInt64();
        }
        while (length-- > 0)
        {
            reader.ReadByte();
        }
        return buffer;
    }

    /// <summary>
    /// Read bytes from the <paramref name="reader"/> and individually cast them to chars.
    /// The returned value should be disposed at some point to return it to the pool.
    /// </summary>
    /// <param name="reader">Reads binary values from the evtc.</param>
    /// <param name="length">Length of bytes to read.</param>
    /// <param name="nullTerminated">if set the array will become truncated at the first null byte</param>
    private static ArrayPoolReturner<char> GetCharArrayPooled(BinaryReader reader, int length, bool nullTerminated = true)
    {
        var buffer = new ArrayPoolReturner<char>(length); // TODO use own reader for direct access 
        buffer.Length = 0; // reuse the length, don't need to remember the original
        while (length-- > 0)
        {
            var b = reader.ReadByte();
            buffer.Array[buffer.Length] = (char)b;
            if (nullTerminated && b == 0) { break; }
            buffer.Length++;
        }
        return buffer;
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
