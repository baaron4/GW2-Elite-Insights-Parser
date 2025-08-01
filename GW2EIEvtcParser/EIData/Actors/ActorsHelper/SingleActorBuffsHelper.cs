﻿using System.Diagnostics.CodeAnalysis;
using GW2EIEvtcParser.EIData.BuffSimulators;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData;

partial class SingleActor
{
    private List<Consumable>? _consumeList;
    // Boons
    private HashSet<Buff>? _trackedBuffs;
    private BuffDictionary? _buffMap;
    private Dictionary<long, BuffGraph>? _buffGraphs;
    private Dictionary<AgentItem, Dictionary<long, BuffGraph>>? _buffGraphsPerAgent;
    private CachingCollection<BuffDistribution>? _buffDistribution;
    private CachingCollection<Dictionary<long, long>>? _buffPresence;
    private CachingCollectionCustom<BuffEnum, (Dictionary<long, BuffStatistics> Buffs, Dictionary<long, BuffStatistics> ActiveBuffs)>? _buffStats;
    private CachingCollectionCustom<BuffEnum, (Dictionary<long, BuffVolumeStatistics> Volumes, Dictionary<long, BuffVolumeStatistics> ActiveVolumes)>? _buffVolumes;
    private CachingCollection<(Dictionary<long, BuffByActorStatistics> Rates, Dictionary<long, BuffByActorStatistics> ActiveRates)>? _buffsDictionary;
    private CachingCollection<(Dictionary<long, BuffVolumeByActorStatistics> Rates, Dictionary<long, BuffVolumeByActorStatistics> ActiveRates)>? _buffVolumesDictionary;
    private Dictionary<long, AbstractBuffSimulator>? _buffSimulators;

    #region DISTRIBUTION
    public BuffDistribution GetBuffDistribution(ParsedEvtcLog log, long start, long end)
    {
        SimulateBuffsAndComputeGraphs(log);

        if (!_buffDistribution.TryGetValue(start, end, out var value))
        {
            value = ComputeBuffDistribution(_buffSimulators, start, end);
            _buffDistribution.Set(start, end, value);
        }

        return value;
    }

    private static BuffDistribution ComputeBuffDistribution(Dictionary<long, AbstractBuffSimulator> buffSimulators, long start, long end)
    {
        var res = new BuffDistribution(buffSimulators.Count, 8); //TODO(Rennorb) @perf: find capacity dependencies

        foreach (var (buff, simulator) in buffSimulators)
        {
            foreach (BuffSimulationItem simul in simulator.GenerationSimulation)
            {
                simul.SetBuffDistributionItem(res, start, end, buff);
            }

            foreach (BuffSimulationItemWasted simul in simulator.WasteSimulationResult)
            {
                simul.SetBuffDistributionItem(res, start, end, buff);
            }

            foreach (BuffSimulationItemOverstack simul in simulator.OverstackSimulationResult)
            {
                simul.SetBuffDistributionItem(res, start, end, buff);
            }
        }

        return res;
    }
    #endregion DISTRIBUTION
    #region PRESENCE
    public IReadOnlyDictionary<long, long> GetBuffPresence(ParsedEvtcLog log, long start, long end)
    {

        SimulateBuffsAndComputeGraphs(log);

        if (!_buffPresence.TryGetValue(start, end, out var value))
        {
            value = ComputeBuffPresence(_buffSimulators!, start, end);
            _buffPresence.Set(start, end, value);
        }
        return value;
    }

    private static Dictionary<long, long> ComputeBuffPresence(Dictionary<long, AbstractBuffSimulator> buffSimulators, long start, long end)
    {
        var buffPresence = new Dictionary<long, long>(buffSimulators.Count);
        foreach (KeyValuePair<long, AbstractBuffSimulator> pair in buffSimulators)
        {
            foreach (BuffSimulationItem simul in pair.Value.GenerationSimulation)
            {
                long duration = simul.GetClampedDuration(start, end);
                if (duration != 0)
                {
                    buffPresence.IncrementValue(pair.Key, duration);
                }
            }
        }
        return buffPresence;
    }
    #endregion PRESENCE
    #region GRAPHS
    public IReadOnlyDictionary<long, BuffGraph> GetBuffGraphs(ParsedEvtcLog log)
    {
        SimulateBuffsAndComputeGraphs(log);
        return _buffGraphs;
    }

    public IReadOnlyDictionary<long, BuffGraph> GetBuffGraphs(ParsedEvtcLog log, SingleActor by)
    {
        AgentItem agent = by.AgentItem;
        SimulateBuffsAndComputeGraphs(log);

        _buffGraphsPerAgent ??= new(8); //TODO(Rennorb) @perf: find capacity dependencies
        if (!_buffGraphsPerAgent.ContainsKey(agent))
        {
            var trackedBuffs = GetTrackedBuffs(log);
            var buffGraphs = new Dictionary<long, BuffGraph>(trackedBuffs.Count);
            _buffGraphsPerAgent![by.AgentItem] = buffGraphs;
            var boonIDs = new HashSet<long>(log.Buffs.BuffsByClassification[BuffClassification.Boon].Select(x => x.ID));
            var condiIDs = new HashSet<long>(log.Buffs.BuffsByClassification[BuffClassification.Condition].Select(x => x.ID));
            //
            var boonPresenceGraph = new BuffGraph(log.Buffs.BuffsByIDs[SkillIDs.NumberOfBoons]);
            var condiPresenceGraph = new BuffGraph(log.Buffs.BuffsByIDs[SkillIDs.NumberOfConditions]);
            //
            foreach (Buff buff in trackedBuffs)
            {
                long buffID = buff.ID;
                if (_buffSimulators.TryGetValue(buff.ID, out var simulator) && !buffGraphs.ContainsKey(buffID))
                {
                    bool updateBoonPresence = boonIDs.Contains(buffID);
                    bool updateCondiPresence = condiIDs.Contains(buffID);
                    var graphSegments = new List<Segment>(simulator.GenerationSimulation.Count + 2);
                    foreach (BuffSimulationItem simul in simulator.GenerationSimulation)
                    {
                        // Graph
                        var segment = simul.ToSegment(by);
                        if (graphSegments.Count == 0)
                        {
                            graphSegments.Add(new Segment(log.FightData.FightStart, segment.Start, 0));
                        }
                        else if (graphSegments.Last().End != segment.Start)
                        {
                            graphSegments.Add(new Segment(graphSegments.Last().End, segment.Start, 0));
                        }
                        graphSegments.Add(segment);
                    }
                    // Graph object creation
                    if (graphSegments.Count > 0)
                    {
                        graphSegments.Add(new Segment(graphSegments.Last().End, log.FightData.FightEnd, 0));
                    }
                    else
                    {
                        graphSegments.Add(new Segment(log.FightData.FightStart, log.FightData.FightEnd, 0));
                    }
                    buffGraphs[buffID] = new BuffGraph(buff, graphSegments);
                    if (updateBoonPresence || updateCondiPresence)
                    {
                        (updateBoonPresence ? boonPresenceGraph : condiPresenceGraph).MergePresenceInto(buffGraphs[buffID].Values);
                    }

                }
            }
            buffGraphs[SkillIDs.NumberOfBoons] = boonPresenceGraph;
            buffGraphs[SkillIDs.NumberOfConditions] = condiPresenceGraph;
        }

        return _buffGraphsPerAgent[agent];
    }
    #endregion GRAPHS
    #region BUFF STATUS
    /// <summary>
    /// Checks if a buff is present on the actor. Given buff id must be in the buff simulator, throws <see cref="InvalidOperationException"/> otherwise
    /// </summary>
    public bool HasBuff(ParsedEvtcLog log, long buffID, long time, long window = 0)
    {
        if (!log.Buffs.BuffsByIDs.ContainsKey(buffID))
        {
            throw new InvalidOperationException($"Buff id {buffID} must be simulated");
        }

        IReadOnlyDictionary<long, BuffGraph> bgms = GetBuffGraphs(log);
        return bgms.TryGetValue(buffID, out var bgm) && bgm.IsPresent(time, window);
    }

    /// <summary>
    /// Checks if a buff is present on the actor and applied by given actor. Given buff id must be in the buff simulator, throws <see cref="InvalidOperationException"/> otherwise
    /// </summary>
    public bool HasBuff(ParsedEvtcLog log, SingleActor by, long buffID, long time)
    {
        if (!log.Buffs.BuffsByIDs.ContainsKey(buffID))
        {
            throw new InvalidOperationException($"Buff id {buffID} must be simulated");
        }

        IReadOnlyDictionary<long, BuffGraph> bgms = GetBuffGraphs(log, by);
        return bgms.TryGetValue(buffID, out var bgm) && bgm.IsPresent(time);
    }

    private static readonly Segment _emptySegment = new(long.MinValue, long.MaxValue, 0);

    private static Segment GetBuffStatus(long buffID, long time, IReadOnlyDictionary<long, BuffGraph> bgms)
    {
        return bgms.TryGetValue(buffID, out var bgm) ? bgm.GetBuffStatus(time) : _emptySegment;
    }

    public Segment GetBuffStatus(ParsedEvtcLog log, long buffID, long time)
    {
        if (!log.Buffs.BuffsByIDs.ContainsKey(buffID))
        {
            throw new InvalidOperationException($"Buff id {buffID} must be simulated");
        }
        return GetBuffStatus(buffID, time, GetBuffGraphs(log));
    }

    public Segment GetBuffStatus(ParsedEvtcLog log, SingleActor by, long buffID, long time)
    {
        if (!log.Buffs.BuffsByIDs.ContainsKey(buffID))
        {
            throw new InvalidOperationException($"Buff id {buffID} must be simulated");
        }
        return GetBuffStatus(buffID, time, GetBuffGraphs(log, by));
    }
    public Segment GetBuffPresenceStatus(ParsedEvtcLog log, long buffID, long time)
    {
        if (!log.Buffs.BuffsByIDs.ContainsKey(buffID))
        {
            throw new InvalidOperationException($"Buff id {buffID} must be simulated");
        }
        var seg = GetBuffStatus(buffID, time, GetBuffGraphs(log));
        if (seg.Value > 0)
        {
            return new Segment(seg.Start, seg.End, 1);
        }
        return seg;
    }

    public Segment GetBuffPresenceStatus(ParsedEvtcLog log, SingleActor by, long buffID, long time)
    {
        if (!log.Buffs.BuffsByIDs.ContainsKey(buffID))
        {
            throw new InvalidOperationException($"Buff id {buffID} must be simulated");
        }
        var seg = GetBuffStatus(buffID, time, GetBuffGraphs(log, by));
        if (seg.Value > 0)
        {
            return new Segment(seg.Start, seg.End, 1);
        }
        return seg;
    }

    private static IReadOnlyList<Segment> GetBuffStatus(long buffID, long start, long end, IReadOnlyDictionary<long, BuffGraph> bgms)
    {
        return bgms.TryGetValue(buffID, out var bgm) ? bgm.GetBuffStatus(start, end).ToList() : [ _emptySegment ];
    }

    /// <exception cref="InvalidOperationException"></exception>
    public IReadOnlyList<Segment> GetBuffStatus(ParsedEvtcLog log, long buffID, long start, long end)
    {
        if (!log.Buffs.BuffsByIDs.ContainsKey(buffID))
        {
            throw new InvalidOperationException($"Buff id {buffID} must be simulated");
        }
        return GetBuffStatus(buffID, start, end, GetBuffGraphs(log));
    }
    public IReadOnlyList<Segment> GetBuffStatus(ParsedEvtcLog log, long buffID)
    {
        return GetBuffStatus(log, buffID, log.FightData.FightStart, log.FightData.FightEnd);
    }

    /// <exception cref="InvalidOperationException"></exception>
    public IReadOnlyList<Segment> GetBuffStatus(ParsedEvtcLog log, SingleActor by, long buffID, long start, long end)
    {
        if (!log.Buffs.BuffsByIDs.ContainsKey(buffID))
        {
            throw new InvalidOperationException($"Buff id {buffID} must be simulated");
        }
        return GetBuffStatus(buffID, start, end, GetBuffGraphs(log, by));
    }
    public IReadOnlyList<Segment> GetBuffStatus(ParsedEvtcLog log, SingleActor by, long buffID)
    {
        return GetBuffStatus(log, by, buffID, log.FightData.FightStart, log.FightData.FightEnd);
    }

    /// <summary>
    /// Creates a <see cref="List{T}"/> of <see cref="Segment"/> of the <paramref name="buffIDs"/> in input.
    /// </summary>
    /// <param name="buffIDs">Buff IDs of which to find the <see cref="Segment"/> of.</param>
    /// <param name="start">Start time to search.</param>
    /// <param name="end">End time to search.</param>
    /// <returns><see cref="List{T}"/> with the <see cref="Segment"/>s found.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public List<Segment> GetBuffStatus(ParsedEvtcLog log, long[] buffIDs, long start, long end)
    {
        //TODO(Rennorb) @perf
        var result = new List<Segment>();
        foreach (long id in buffIDs)
        {
            result.AddRange(GetBuffStatus(log, id, start, end));
        }
        return result;
    }

    public List<Segment> GetBuffStatus(ParsedEvtcLog log, long[] buffIDs)
    {
        return GetBuffStatus(log, buffIDs, log.FightData.FightStart, log.FightData.FightEnd);
    }

    private static void FuseConsecutiveNonZeroAndSetTo1(List<Segment> segments)
    {
        Segment last = segments[0];
        if (last.Value > 0)
        {
            last.Value = 1;
        }
        int lastIndex = 0;
        for (int i = 1; i < segments.Count; i++)
        {
            var current = segments[i];
            if (current.IsEmpty())
            {
                continue;
            }
            if (current.Value > 0)
            {
                current.Value = 1;
            }
            if (current.Value != 0 && last.Value != 0)
            {
                last.End = current.End;
                segments[lastIndex] = last;
            }
            else
            {
                segments[lastIndex] = last;
                last = current;
                lastIndex++;
            }
        }
        segments[lastIndex++] = last;

        segments.RemoveRange(lastIndex, segments.Count - lastIndex);
    }

    /// <exception cref="InvalidOperationException"></exception>
    public IReadOnlyList<Segment> GetBuffPresenceStatus(ParsedEvtcLog log, long buffID, long start, long end)
    {
        if (!log.Buffs.BuffsByIDs.ContainsKey(buffID))
        {
            throw new InvalidOperationException($"Buff id {buffID} must be simulated");
        }
        var presence = GetBuffStatus(buffID, start, end, GetBuffGraphs(log)).ToList();
        FuseConsecutiveNonZeroAndSetTo1(presence);
        return presence;
    }

    public IReadOnlyList<Segment> GetBuffPresenceStatus(ParsedEvtcLog log, long buffID)
    {
        return GetBuffPresenceStatus(log, buffID, log.FightData.FightStart, log.FightData.FightEnd);
    }

    /// <exception cref="InvalidOperationException"></exception>
    public IReadOnlyList<Segment> GetBuffPresenceStatus(ParsedEvtcLog log, SingleActor by, long buffID, long start, long end)
    {
        if (!log.Buffs.BuffsByIDs.ContainsKey(buffID))
        {
            throw new InvalidOperationException($"Buff id {buffID} must be simulated");
        }
        var presence = GetBuffStatus(buffID, start, end, GetBuffGraphs(log, by)).ToList();
        FuseConsecutiveNonZeroAndSetTo1(presence);
        return presence;
    }
    public IReadOnlyList<Segment> GetBuffPresenceStatus(ParsedEvtcLog log, SingleActor by, long buffID)
    {
        return GetBuffPresenceStatus(log, by, buffID, log.FightData.FightStart, log.FightData.FightEnd);
    }
    #endregion BUFF STATUS
    #region STATISTICS
    public IReadOnlyDictionary<long, BuffStatistics> GetBuffs(BuffEnum type, ParsedEvtcLog log, long start, long end)
    {
        _buffStats ??= new(log, BuffEnum.Self, 4);
        if (!_buffStats.TryGetValue(start, end, type, out var pair))
        {
            pair = ComputeBuffs(log, start, end, type);
            _buffStats.Set(start, end, type, pair);
        }
        return pair.Buffs;
    }

    public IReadOnlyDictionary<long, BuffStatistics> GetActiveBuffs(BuffEnum type, ParsedEvtcLog log, long start, long end)
    {
        _buffStats ??= new(log, BuffEnum.Self, 4);
        if (!_buffStats.TryGetValue(start, end, type, out var value))
        {
            value = ComputeBuffs(log, start, end, type);
            _buffStats.Set(start, end, type, value);
        }
        return value.ActiveBuffs;
    }

    public IReadOnlyDictionary<long, BuffVolumeStatistics> GetBuffVolumes(BuffEnum type, ParsedEvtcLog log, long start, long end)
    {
        _buffVolumes ??= new(log, BuffEnum.Self, 4);
        if (!_buffVolumes.TryGetValue(start, end, type, out var value))
        {
            value = ComputeBuffVolumes(log, start, end, type);
            _buffVolumes.Set(start, end, type, value);
        }
        return value.Volumes;
    }

    public IReadOnlyDictionary<long, BuffVolumeStatistics> GetActiveBuffVolumes(BuffEnum type, ParsedEvtcLog log, long start, long end)
    {
        _buffVolumes ??= new(log, BuffEnum.Self, 4);
        if (!_buffVolumes.TryGetValue(start, end, type, out var value))
        {
            value = ComputeBuffVolumes(log, start, end, type);
            _buffVolumes.Set(start, end, type, value);
        }
        return value.ActiveVolumes;
    }
    public IReadOnlyDictionary<long, BuffByActorStatistics> GetBuffsDictionary(ParsedEvtcLog log, long start, long end)
    {
        _buffsDictionary ??= new(log);
        if (!_buffsDictionary.TryGetValue(start, end, out var value))
        {
            value = ComputeBuffsDictionaries(log, start, end);
            _buffsDictionary.Set(start, end, value);
        }
        return value.Rates;
    }

    public IReadOnlyDictionary<long, BuffByActorStatistics> GetActiveBuffsDictionary(ParsedEvtcLog log, long start, long end)
    {
        _buffsDictionary ??= new(log);
        if (!_buffsDictionary.TryGetValue(start, end, out var value))
        {
            value = ComputeBuffsDictionaries(log, start, end);
            _buffsDictionary.Set(start, end, value);
        }
        return value.ActiveRates;
    }

    public IReadOnlyDictionary<long, BuffVolumeByActorStatistics> GetBuffVolumesDictionary(ParsedEvtcLog log, long start, long end)
    {
        _buffVolumesDictionary ??= new(log);
        if (!_buffVolumesDictionary.TryGetValue(start, end, out var value))
        {
            value = ComputeBuffVolumesDictionaries(log, start, end);
            _buffVolumesDictionary.Set(start, end, value);
        }
        return value.Rates;
    }
    public IReadOnlyDictionary<long, BuffVolumeByActorStatistics> GetActiveBuffVolumesDictionary(ParsedEvtcLog log, long start, long end)
    {
        _buffVolumesDictionary ??= new(log);
        if (!_buffVolumesDictionary.TryGetValue(start, end, out var value))
        {
            value = ComputeBuffVolumesDictionaries(log, start, end);
            _buffVolumesDictionary.Set(start, end, value);
        }
        return value.ActiveRates;
    }

    private (Dictionary<long, BuffByActorStatistics> Rates, Dictionary<long, BuffByActorStatistics> RatesActive) ComputeBuffsDictionaries(ParsedEvtcLog log, long start, long end)
    {
        BuffDistribution buffDistribution = GetBuffDistribution(log, start, end);
        var rates = new Dictionary<long, BuffByActorStatistics>();
        var ratesActive = new Dictionary<long, BuffByActorStatistics>();
        long duration = end - start;
        long activeDuration = GetActiveDuration(log, start, end);

        foreach (Buff buff in GetTrackedBuffs(log))
        {
            if (buffDistribution.HasBuffID(buff.ID))
            {
                (rates[buff.ID], ratesActive[buff.ID]) = BuffByActorStatistics.GetBuffByActor(log, buff, buffDistribution, duration, activeDuration);
            }
        }
        return (rates, ratesActive);
    }

    private (Dictionary<long, BuffVolumeByActorStatistics> Rates, Dictionary<long, BuffVolumeByActorStatistics> RatesActive) ComputeBuffVolumesDictionaries(ParsedEvtcLog log, long start, long end)
    {
        var rates = new Dictionary<long, BuffVolumeByActorStatistics>();
        var ratesActive = new Dictionary<long, BuffVolumeByActorStatistics>();

        foreach (Buff buff in GetTrackedBuffs(log))
        {
            (rates[buff.ID], ratesActive[buff.ID]) = BuffVolumeByActorStatistics.GetBuffVolumeByActor(log, buff, this, start, end);
        }
        return (rates, ratesActive);
    }

    #endregion STATISTICS
    #region COMPUTE
    public IReadOnlyCollection<Buff> GetTrackedBuffs(ParsedEvtcLog log)
    {
        if (_trackedBuffs == null)
        {
            ComputeBuffMap(log);
        }
        return _trackedBuffs;
    }

    [MemberNotNull(nameof(_buffMap))]
    [MemberNotNull(nameof(_trackedBuffs))]
    internal void ComputeBuffMap(ParsedEvtcLog log)
    {
        _buffMap = new BuffDictionary(64, 256, 32, 1);
        if (AgentItem.IsUnknown)
        {
            _buffMap.Finalize(log, AgentItem, out _trackedBuffs);
            return;
        }
        // Fill in Boon Map
#if DEBUG
        var test = log.CombatData.GetBuffDataByDst(AgentItem).Where(x => !log.Buffs.BuffsByIDs.ContainsKey(x.BuffID)).GroupBy(x => x.BuffSkill.Name).ToDictionary(x => x.Key, x => x.ToList());
        var test2 = log.CombatData.GetBuffDataByDst(AgentItem).Where(x => log.Buffs.BuffsByIDs.ContainsKey(x.BuffID)).GroupBy(x => x.BuffSkill.Name).ToDictionary(x => x.Key, x => x.ToList());
#endif
        foreach (var buffEvent in log.CombatData.GetBuffDataByDst(AgentItem))
        {
            if (!log.Buffs.BuffsByIDs.TryGetValue(buffEvent.BuffID, out var buff))
            {
                continue;
            }

            if (buffEvent.BuffID != SkillIDs.Regeneration)
            {
                _buffMap.Add(log, buff, buffEvent);
            }
            else
            {
                _buffMap.AddRegen(log, buff, buffEvent);
            }
        }
        _buffMap.Finalize(log, AgentItem, out _trackedBuffs);
    }


    [MemberNotNull(nameof(_buffGraphs))]
    [MemberNotNull(nameof(_buffDistribution))]
    [MemberNotNull(nameof(_buffPresence))]
    [MemberNotNull(nameof(_buffSimulators))]
    internal void SimulateBuffsAndComputeGraphs(ParsedEvtcLog log)
    {
        if (_buffGraphs != null)
        {
#pragma warning disable CS8774 // must have non null
            return;
#pragma warning restore CS8774 // must have non null
        }
        if (_buffMap == null)
        {
            ComputeBuffMap(log);
        }

        var trackedBuffs = GetTrackedBuffs(log);
        _buffGraphs = new Dictionary<long, BuffGraph>(trackedBuffs.Count + 5);
        var boonPresenceGraph = new BuffGraph(log.Buffs.BuffsByIDs[SkillIDs.NumberOfBoons]);
        var activeCombatMinionsGraph = new BuffGraph(log.Buffs.BuffsByIDs[SkillIDs.NumberOfActiveCombatMinions]);
        var numberOfClonesGraph = ProfHelper.CanSummonClones(Spec) ? new BuffGraph(log.Buffs.BuffsByIDs[SkillIDs.NumberOfClones]) : null;
        var numberOfRangerPets = ProfHelper.CanUseRangerPets(Spec) ? new BuffGraph(log.Buffs.BuffsByIDs[SkillIDs.NumberOfRangerPets]) : null;
        var condiPresenceGraph = new BuffGraph(log.Buffs.BuffsByIDs[SkillIDs.NumberOfConditions]);
        var boonIDs = new HashSet<long>(log.Buffs.BuffsByClassification[BuffClassification.Boon].Select(x => x.ID));
        var condiIDs = new HashSet<long>(log.Buffs.BuffsByClassification[BuffClassification.Condition].Select(x => x.ID));

        // Init status
        _buffDistribution = new(log);
        _buffPresence     = new(log);
        _buffSimulators   = new(trackedBuffs.Count * 2);
        var buffStackItemPool = new BuffStackItemPool();
        foreach (Buff buff in trackedBuffs)
        {
            long buffID = buff.ID;
            if (_buffMap.TryGetValue(buffID, out var buffEvents) && buffEvents.Count != 0 && !_buffGraphs.ContainsKey(buffID))
            {
                AbstractBuffSimulator simulator;
                try
                {
                    if (log.CombatData.UseBuffInstanceSimulator && AgentItem.Type == AgentItem.AgentType.NonSquadPlayer)
                    {
                        buffEvents.RemoveAll(x => !x.IsBuffSimulatorCompliant(false));
                        simulator = buff.CreateSimulator(log, buffStackItemPool, true);
                    } 
                    else
                    {
                        simulator = buff.CreateSimulator(log, buffStackItemPool, false);
                    }
                    simulator.Simulate(buffEvents, log.FightData.FightStart, log.FightData.FightEnd);
                }
                catch (EIBuffSimulatorIDException e)
                {
                    // get rid of logs invalid for HasStackIDs false
                    log.UpdateProgressWithCancellationCheck("Parsing: Failed id based simulation on " + Character + " for " + buff.Name + " because " + e.Message);
                    buffEvents.RemoveAll(x => !x.IsBuffSimulatorCompliant(false));
                    simulator = buff.CreateSimulator(log, buffStackItemPool, true);
                    simulator.Simulate(buffEvents, log.FightData.FightStart, log.FightData.FightEnd);
                }
                _buffSimulators[buffID] = simulator;
                bool updateBoonPresence = boonIDs.Contains(buffID);
                bool updateCondiPresence = condiIDs.Contains(buffID); // move
                var graphSegments = new List<Segment>(simulator.GenerationSimulation.Count);
                foreach (BuffSimulationItem simul in simulator.GenerationSimulation)
                {
                    // Graph
                    var segment = simul.ToSegment();
                    if (graphSegments.Count == 0)
                    {
                        graphSegments.Add(new Segment(log.FightData.FightStart, segment.Start, 0));
                    }
                    else if (graphSegments.Last().End != segment.Start)
                    {
                        graphSegments.Add(new Segment(graphSegments.Last().End, segment.Start, 0));
                    }
                    graphSegments.Add(segment);
                }
                // Graph object creation
                if (graphSegments.Count > 0)
                {
                    graphSegments.Add(new Segment(graphSegments.Last().End, log.FightData.FightEnd, 0));
                }
                else
                {
                    graphSegments.Add(new Segment(log.FightData.FightStart, log.FightData.FightEnd, 0));
                }

                _buffGraphs[buffID] = new BuffGraph(buff, graphSegments);
                if (updateBoonPresence || updateCondiPresence)
                {
                    (updateBoonPresence ? boonPresenceGraph : condiPresenceGraph).MergePresenceInto(_buffGraphs[buffID].Values);
                }

            }
        }

        _buffGraphs[SkillIDs.NumberOfBoons] = boonPresenceGraph;
        _buffGraphs[SkillIDs.NumberOfConditions] = condiPresenceGraph;
        foreach (Minions minions in GetMinions(log).Values)
        {
            IReadOnlyList<IReadOnlyList<Segment>> segments = minions.GetLifeSpanSegments(log);
            foreach (IReadOnlyList<Segment> minionsSegments in segments)
            {
                activeCombatMinionsGraph.MergePresenceInto(minionsSegments);
            }
            if (numberOfClonesGraph != null && MesmerHelper.IsClone(minions.ReferenceAgentItem))
            {
                foreach (IReadOnlyList<Segment> minionsSegments in segments)
                {
                    numberOfClonesGraph.MergePresenceInto(minionsSegments);
                }
            }
            if (numberOfRangerPets != null && RangerHelper.IsJuvenilePet(minions.ReferenceAgentItem))
            {
                foreach (IReadOnlyList<Segment> minionsSegments in segments)
                {
                    numberOfRangerPets.MergePresenceInto(minionsSegments);
                }
            }
        }
        if (activeCombatMinionsGraph.Values.Any())
        {
            _buffGraphs[SkillIDs.NumberOfActiveCombatMinions] = activeCombatMinionsGraph;
        }
        if (numberOfClonesGraph != null && numberOfClonesGraph.Values.Any())
        {
            _buffGraphs[SkillIDs.NumberOfClones] = numberOfClonesGraph;
        }
        if (numberOfRangerPets != null && numberOfRangerPets.Values.Any())
        {
            _buffGraphs[SkillIDs.NumberOfRangerPets] = numberOfRangerPets;
        }
    }
    #endregion COMPUTE
    #region CONSUMABLES
    public IReadOnlyList<Consumable> GetConsumablesList(ParsedEvtcLog log, long start, long end)
    {
        if (_consumeList == null)
        {
            SetConsumablesList(log);
        }
        return _consumeList.Where(x => x.Time >= start && x.Time <= end).ToList();
    }

    [MemberNotNull((nameof(_consumeList)))]
    private void SetConsumablesList(ParsedEvtcLog log)
    {
        var consumableList = new List<Buff>(log.Buffs.BuffsByClassification[BuffClassification.Nourishment]);
        consumableList.AddRange(log.Buffs.BuffsByClassification[BuffClassification.Enhancement]);
        consumableList.AddRange(log.Buffs.BuffsByClassification[BuffClassification.OtherConsumable]);
        _consumeList = [];
        foreach (Buff consumable in consumableList)
        {
            foreach (BuffEvent c in log.CombatData.GetBuffData(consumable.ID))
            {
                if (!(c is BuffApplyEvent ba) || !AgentItem.Is(ba.To))
                {
                    continue;
                }
                long time = 0;
                if (!ba.Initial)
                {
                    time = ba.Time;
                }
                if (time <= log.FightData.FightEnd)
                {
                    Consumable? existing = _consumeList.Find(x => x.Time == time && x.Buff.ID == consumable.ID);
                    if (existing != null)
                    {
                        existing.Stack++;
                    }
                    else
                    {
                        _consumeList.Add(new Consumable(consumable, time, ba.AppliedDuration));
                    }
                }
            }
        }
        _consumeList.Sort((x, y) => x.Time.CompareTo(y.Time));

    }
    #endregion CONSUMABLES
}
