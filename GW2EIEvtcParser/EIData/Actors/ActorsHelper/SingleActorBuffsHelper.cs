using System.Diagnostics.CodeAnalysis;
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
    private CachingCollectionWithTarget<IReadOnlyDictionary<long, long>>? _buffPresenceBy;
    private CachingCollectionCustom<BuffEnum, (Dictionary<long, BuffStatistics> Buffs, Dictionary<long, BuffStatistics> ActiveBuffs)>? _buffStats;
    private CachingCollectionCustom<BuffEnum, (Dictionary<long, BuffVolumeStatistics> Volumes, Dictionary<long, BuffVolumeStatistics> ActiveVolumes)>? _buffVolumes;
    private CachingCollection<(Dictionary<long, BuffByActorStatistics> Rates, Dictionary<long, BuffByActorStatistics> ActiveRates)>? _buffsDictionary;
    private CachingCollection<(Dictionary<long, BuffVolumeByActorStatistics> Rates, Dictionary<long, BuffVolumeByActorStatistics> ActiveRates)>? _buffVolumesDictionary;
    private Dictionary<long, AbstractBuffSimulator>? _buffSimulators;

    #region ACCELERATORS
    private CachingCollectionWithAgentTarget<Dictionary<long, List<AbstractBuffApplyEvent>>>? _buffApplyByIDAccelerator;
    private HashSet<long>? PresentApplyOnBuffIDs;

    private void InitBuffApplyByIDDict(ParsedEvtcLog log, long start, long end)
    {
        List<AbstractBuffApplyEvent> curBuffApplies;
        if (_buffApplyByIDAccelerator!.TryGetEnglobingValue(start, end, null, out var englobingNullDict))
        {
            curBuffApplies = englobingNullDict.Values.SelectMany(x => x.ToList()).Where(x => x.Time >= start && x.Time <= end).ToList();
        }
        else
        {
            curBuffApplies = log.CombatData.GetBuffApplyDataByDst(AgentItem).Where(x => x.Time >= start && x.Time <= end).ToList();
        }
        var nullDict = curBuffApplies.GroupBy(x => x.BuffID).ToDictionary(x => x.Key, x => x.ToList());
        _buffApplyByIDAccelerator.Set(start, end, null, nullDict);
        var dictByCreditedBy = curBuffApplies.GroupBy(x => x.CreditedBy).ToDictionary(x => x.Key, x => x.ToList());
        foreach (var pair in dictByCreditedBy)
        {
            var dictToSet = pair.Value.GroupBy(x => x.BuffID).ToDictionary(x => x.Key, x => x.ToList());
            _buffApplyByIDAccelerator.Set(start, end, pair.Key, dictToSet);
        }
    }
    [MemberNotNull(nameof(PresentApplyOnBuffIDs))]
    [MemberNotNull(nameof(_buffApplyByIDAccelerator))]
    internal IReadOnlyList<AbstractBuffApplyEvent> GetBuffApplyEventsOnByIDInternal(ParsedEvtcLog log, long start, long end, long buffID, SingleActor? creditedBy)
    {
        if (PresentApplyOnBuffIDs == null)
        {
            var allBuffApplies = log.CombatData.GetBuffApplyDataByDst(AgentItem);
            PresentApplyOnBuffIDs = [.. allBuffApplies.Select(x => x.BuffID)];
        }
        _buffApplyByIDAccelerator ??= new CachingCollectionWithAgentTarget<Dictionary<long, List<AbstractBuffApplyEvent>>>(log);
        var creditedByAgentItem = creditedBy?.AgentItem;
        if (!_buffApplyByIDAccelerator.TryGetValue(start, end, creditedByAgentItem, out var dict))
        {
            if (!_buffApplyByIDAccelerator.TryGetValue(start, end, null, out var nullDict))
            {
                InitBuffApplyByIDDict(log, start, end);
            }
            if (creditedBy != null && creditedBy.AgentItem.IsEnglobedAgent)
            {
                var englobingCreditedBy = creditedBy.EnglobingAgentItem;
                if (_buffApplyByIDAccelerator.TryGetValue(start, end, englobingCreditedBy, out var englobingDict))
                {
                    dict = [];
                    foreach (var pair in englobingDict)
                    {
                        dict[pair.Key] = pair.Value.Where(x => x.Time >= creditedBy.FirstAware && x.Time <= creditedBy.LastAware).ToList();
                    }
                }
                else
                {
                    dict = [];
                }
                _buffApplyByIDAccelerator.Set(start, end, creditedByAgentItem, dict);
            }
            else
            {
                if (!(_buffApplyByIDAccelerator.TryGetValue(start, end, creditedByAgentItem, out dict)))
                {
                    dict = [];
                    _buffApplyByIDAccelerator.Set(start, end, creditedByAgentItem, dict);
                }
            }
        }
        if (dict.TryGetValue(buffID, out var list))
        {
            return list;
        }
        return [];
    }

    public IReadOnlyList<AbstractBuffApplyEvent> GetBuffApplyEventsOnByID(ParsedEvtcLog log, long start, long end, long buffID, SingleActor? creditedBy)
    {
        if (PresentApplyOnBuffIDs != null && !PresentApplyOnBuffIDs.Contains(buffID))
        {
            return [];
        }
        return GetBuffApplyEventsOnByIDInternal(log, start, end, buffID, creditedBy);
    }

    private CachingCollectionWithAgentTarget<Dictionary<long, List<BuffRemoveAllEvent>>>? _buffRemoveAllByByIDAccelerator;
    private HashSet<long>? PresentRemovedByBuffIDs;

    private void InitBuffRemoveAllByByIDDict(ParsedEvtcLog log, long start, long end)
    {
        List<BuffRemoveAllEvent> curBuffRemoves;
        if (_buffRemoveAllByByIDAccelerator!.TryGetEnglobingValue(start, end, null, out var englobingNullDict))
        {
            curBuffRemoves = englobingNullDict.Values.SelectMany(x => x.ToList()).Where(x => x.Time >= start && x.Time <= end).ToList();
        }
        else
        {
            curBuffRemoves = log.CombatData.GetBuffRemoveAllDataBySrc(AgentItem).Where(x => x.Time >= start && x.Time <= end).ToList();
        }
        var nullDict = curBuffRemoves.GroupBy(x => x.BuffID).ToDictionary(x => x.Key, x => x.ToList());
        _buffRemoveAllByByIDAccelerator!.Set(start, end, null, nullDict);
        var dictByTo = curBuffRemoves.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
        foreach (var pair in dictByTo)
        {
            var dictToSet = pair.Value.GroupBy(x => x.BuffID).ToDictionary(x => x.Key, x => x.ToList());
            _buffRemoveAllByByIDAccelerator.Set(start, end, pair.Key, dictToSet);
        }
    }

    [MemberNotNull(nameof(PresentRemovedByBuffIDs))]
    [MemberNotNull(nameof(_buffRemoveAllByByIDAccelerator))]
    internal IReadOnlyList<BuffRemoveAllEvent> GetBuffRemoveAllEventsByByIDInternal(ParsedEvtcLog log, long start, long end, long buffID, SingleActor? removedFrom)
    {
        if (PresentRemovedByBuffIDs == null)
        {
            var allBuffRemoves = log.CombatData.GetBuffRemoveAllDataBySrc(AgentItem);
            PresentRemovedByBuffIDs = [.. allBuffRemoves.Select(x => x.BuffID)];
        }
        _buffRemoveAllByByIDAccelerator ??= new CachingCollectionWithAgentTarget<Dictionary<long, List<BuffRemoveAllEvent>>>(log);
        var removedFromAgentItem = removedFrom?.AgentItem;
        if (!_buffRemoveAllByByIDAccelerator.TryGetValue(start, end, removedFromAgentItem, out var dict))
        {
            if (!_buffRemoveAllByByIDAccelerator.TryGetValue(start, end, null, out var nullDict))
            {
                InitBuffRemoveAllByByIDDict(log, start, end);
            }
            if (removedFrom != null && removedFrom.AgentItem.IsEnglobedAgent)
            {
                var englobingRemovedFrom = removedFrom.EnglobingAgentItem;
                if (_buffRemoveAllByByIDAccelerator.TryGetValue(start, end, englobingRemovedFrom, out var englobingDict))
                {
                    dict = [];
                    foreach (var pair in englobingDict)
                    {
                        dict[pair.Key] = pair.Value.Where(x => x.Time >= removedFrom.FirstAware && x.Time <= removedFrom.LastAware).ToList();
                    }
                } 
                else
                {
                    dict = [];
                }
                _buffRemoveAllByByIDAccelerator.Set(start, end, removedFromAgentItem, dict);
            } 
            else
            {
                if (!(_buffRemoveAllByByIDAccelerator.TryGetValue(start, end, removedFromAgentItem, out dict)))
                {
                    dict = [];
                    _buffRemoveAllByByIDAccelerator.Set(start, end, removedFromAgentItem, dict);
                }
            }
        }
        if (dict.TryGetValue(buffID, out var list))
        {
            return list;
        }
        return [];
    }


    internal IReadOnlyCollection<long> GetBuffRemoveAllEventsByPresentBuffIDs(ParsedEvtcLog log)
    {
        GetBuffRemoveAllEventsByByIDInternal(log, log.LogData.LogStart, log.LogData.LogEnd, 0, null);
        return PresentRemovedByBuffIDs;
    }
    internal IReadOnlyList<BuffRemoveAllEvent> GetBuffRemoveAllEventsByByID(ParsedEvtcLog log, long start, long end, long buffID, SingleActor? removedFrom)
    {
        if (PresentRemovedByBuffIDs != null && !PresentRemovedByBuffIDs.Contains(buffID))
        {
            return [];
        }
        return GetBuffRemoveAllEventsByByIDInternal(log, start, end, buffID, removedFrom);
    }


    private CachingCollectionWithAgentTarget<Dictionary<long, List<BuffRemoveAllEvent>>>? _buffRemoveAllFromByIDAccelerator;
    private HashSet<long>? PresentRemovedFromBuffIDs;

    private void InitBuffRemoveAllFromByIDDict(ParsedEvtcLog log, long start, long end)
    {
        List<BuffRemoveAllEvent> curBuffRemoves;
        if (_buffRemoveAllFromByIDAccelerator!.TryGetEnglobingValue(start, end, null, out var englobingNullDict))
        {
            curBuffRemoves = englobingNullDict.Values.SelectMany(x => x.ToList()).Where(x => x.Time >= start && x.Time <= end).ToList();
        }
        else
        {
            curBuffRemoves = log.CombatData.GetBuffRemoveAllDataByDst(AgentItem).Where(x => x.Time >= start && x.Time <= end).ToList();
        }
        var nullDict = curBuffRemoves.GroupBy(x => x.BuffID).ToDictionary(x => x.Key, x => x.ToList());
        _buffRemoveAllFromByIDAccelerator!.Set(start, end, null, nullDict);
        var dictByTo = curBuffRemoves.GroupBy(x => x.CreditedBy).ToDictionary(x => x.Key, x => x.ToList());
        foreach (var pair in dictByTo)
        {
            var dictToSet = pair.Value.GroupBy(x => x.BuffID).ToDictionary(x => x.Key, x => x.ToList());
            _buffRemoveAllFromByIDAccelerator.Set(start, end, pair.Key, dictToSet);
        }
    }


    [MemberNotNull(nameof(PresentRemovedFromBuffIDs))]
    [MemberNotNull(nameof(_buffRemoveAllFromByIDAccelerator))]
    internal IReadOnlyList<BuffRemoveAllEvent> GetBuffRemoveAllEventsFromByIDInternal(ParsedEvtcLog log, long start, long end, long buffID, SingleActor? removedBy)
    {
        if (PresentRemovedFromBuffIDs == null)
        {
            var allBuffRemoves = log.CombatData.GetBuffRemoveAllDataByDst(AgentItem);
            PresentRemovedFromBuffIDs = [.. allBuffRemoves.Select(x => x.BuffID)];
        }
        _buffRemoveAllFromByIDAccelerator ??= new CachingCollectionWithAgentTarget<Dictionary<long, List<BuffRemoveAllEvent>>>(log);
        var removedByAgentItem = removedBy?.AgentItem;
        if (!_buffRemoveAllFromByIDAccelerator.TryGetValue(start, end, removedByAgentItem, out var dict))
        {
            if (!_buffRemoveAllFromByIDAccelerator.TryGetValue(start, end, null, out var nullDict))
            {
                InitBuffRemoveAllFromByIDDict(log, start, end);
            }
            if (removedBy != null && removedBy.AgentItem.IsEnglobedAgent)
            {
                var englobingRemovedBy = removedBy.EnglobingAgentItem;
                if (_buffRemoveAllFromByIDAccelerator.TryGetValue(start, end, englobingRemovedBy, out var englobingDict))
                {
                    dict = [];
                    foreach (var pair in englobingDict)
                    {
                        dict[pair.Key] = pair.Value.Where(x => x.Time >= removedBy.FirstAware && x.Time <= removedBy.LastAware).ToList();
                    }
                }
                else
                {
                    dict = [];
                }
                _buffRemoveAllFromByIDAccelerator.Set(start, end, removedByAgentItem, dict);
            }
            else
            {
                if (!(_buffRemoveAllFromByIDAccelerator.TryGetValue(start, end, removedByAgentItem, out dict)))
                {
                    dict = [];
                    _buffRemoveAllFromByIDAccelerator.Set(start, end, removedByAgentItem, dict);
                }
            }
        }
        if (dict.TryGetValue(buffID, out var list))
        {
            return list;
        }
        return [];
    }

    internal IReadOnlyList<BuffRemoveAllEvent> GetBuffRemoveAllEventsFromByID(ParsedEvtcLog log, long start, long end, long buffID, SingleActor? removedBy)
    {
        if (PresentRemovedFromBuffIDs != null && !PresentRemovedFromBuffIDs.Contains(buffID))
        {
            return [];
        }
        return GetBuffRemoveAllEventsFromByIDInternal(log, start, end, buffID, removedBy);
    }



    private CachingCollectionCustom<AbstractBuffSimulator, List<BuffSimulationItem>>? _buffGenerationSimulationItemsCache;

    private IReadOnlyList<BuffSimulationItem> GetBuffGenerationSimulationItems(ParsedEvtcLog log, AbstractBuffSimulator simulator, long start, long end)
    {
        _buffGenerationSimulationItemsCache ??= new(log, null!, 50); // we don't care about the null equivalent, simulator can't be null
        if (!_buffGenerationSimulationItemsCache.TryGetValue(start, end, simulator, out var list))
        {
            list = simulator.GenerationSimulation.Where(x => x.GetClampedDuration(start, end) > 0).ToList();
            _buffGenerationSimulationItemsCache.Set(start, end, simulator, list);
        }
        return list;
    }

    #endregion ACCELERATORS

    #region DISTRIBUTION
    public BuffDistribution GetBuffDistribution(ParsedEvtcLog log, long start, long end)
    {
        SimulateBuffsAndComputeGraphs(log);

        if (!_buffDistribution.TryGetValue(start, end, out var value))
        {
            if (AgentItem.IsEnglobedAgent)
            {
                value = log.FindActor(EnglobingAgentItem).GetBuffDistribution(log, Math.Max(start, FirstAware), Math.Min(end, LastAware));
            } 
            else
            {
                value = ComputeBuffDistribution(log, _buffSimulators, start, end);
            }
            _buffDistribution.Set(start, end, value);
        }

        return value;
    }

    private BuffDistribution ComputeBuffDistribution(ParsedEvtcLog log, Dictionary<long, AbstractBuffSimulator> buffSimulators, long start, long end)
    {
        var res = new BuffDistribution(buffSimulators.Count, 8); //TODO_PERF(Rennorb) @find capacity dependencies
        foreach (var (buff, simulator) in buffSimulators)
        {
            foreach (BuffSimulationItem simul in GetBuffGenerationSimulationItems(log, simulator, start, end))
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
    /// <summary>
    /// Returns a dictionnary of <BuffID, PresenceValue> for given interval by provided actor
    /// This dictionary will only container buff ids for intensity stacking buffs
    /// </summary>
    /// <param name="log"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="by"></param>
    /// <returns>dictionnary of <BuffID, PresenceValue></returns>
    public IReadOnlyDictionary<long, long> GetBuffPresence(ParsedEvtcLog log, long start, long end, SingleActor? by = null)
    {

        SimulateBuffsAndComputeGraphs(log);

        if (!_buffPresenceBy.TryGetValue(start, end, by, out var value))
        {
            if (by != null && by.AgentItem.IsEnglobedAgent)
            {
                var byActor = log.FindActor(by.AgentItem.EnglobingAgentItem);
                if (AgentItem.IsEnglobedAgent)
                {
                    value = log.FindActor(EnglobingAgentItem).GetBuffPresence(log, Math.Max(start, Math.Max(FirstAware, by.FirstAware)), Math.Min(end, Math.Min(LastAware, by.LastAware)), byActor);
                } 
                else
                {
                    value = ComputeBuffPresence(log, _buffSimulators, Math.Max(start, by.FirstAware), Math.Min(end, by.LastAware), byActor);
                }
            }
            else if (AgentItem.IsEnglobedAgent)
            {
                value = log.FindActor(EnglobingAgentItem).GetBuffPresence(log, Math.Max(start, FirstAware), Math.Min(end, LastAware), by);
            }
            else
            {
                value = ComputeBuffPresence(log, _buffSimulators, start, end, by);
            }
            _buffPresenceBy.Set(start, end, by, value);
        }
        return value;
    }

    private Dictionary<long, long> ComputeBuffPresence(ParsedEvtcLog log, Dictionary<long, AbstractBuffSimulator> buffSimulators, long start, long end, SingleActor? by)
    {
        var buffPresence = new Dictionary<long, long>(buffSimulators.Count);
        foreach (var (buff, simulator) in buffSimulators)
        {
            if (simulator.Buff.Type == BuffType.Intensity)
            {
                foreach (BuffSimulationItem simul in GetBuffGenerationSimulationItems(log, simulator, start, end))
                {
                    long duration = by != null ? simul.GetClampedDuration(start, end, by) : simul.GetClampedDuration(start, end);
                    if (duration != 0)
                    {
                        buffPresence.IncrementValue(buff, duration);
                    }
                }
            }
        }
        return buffPresence;
    }
    #endregion PRESENCE
    #region GRAPHS

    private static BuffGraph BuildBuffGraphInAwareTimesFromEnglobingGraph(ParsedEvtcLog log, BuffGraph buffGraph, long firstAware, long lastAware)
    {
        if (firstAware >= lastAware || buffGraph.Values.Count == 0)
        {
            return new BuffGraph(log.Buffs.BuffsByIDs[buffGraph.Buff.ID]);
        }
        var newGraph = new List<Segment>(buffGraph.Values.Count);
        Segment? prevSegment = null;
        foreach (var segment in buffGraph.Values)
        {
            var newSeg = new Segment(prevSegment?.End ?? segment.Start, segment.End, segment.Value);
            if (segment.Value > 0)
            {
                // if not within aware
                if (newSeg.End < firstAware || newSeg.Start > lastAware)
                {
                    newSeg.Value = 0;
                }
                else // intersecting
                {
                    if (newSeg.Start < firstAware)
                    {
                        var beforeSeg = new Segment(newSeg.Start, firstAware, 0);
                        newGraph.Add(beforeSeg);
                        newSeg = new Segment(firstAware, newSeg.End, newSeg.Value);
                    }
                    if (newSeg.End > lastAware)
                    {
                        var insideSeg = new Segment(newSeg.Start, lastAware, newSeg.Value);
                        newGraph.Add(insideSeg);
                        newSeg = new Segment(lastAware, newSeg.End, 0);
                    }
                }
            }
            newGraph.Add(newSeg);
            prevSegment = newSeg;
        }
        newGraph.RemoveAll(x => x.IsEmpty());
        return new BuffGraph(log.Buffs.BuffsByIDs[buffGraph.Buff.ID], newGraph);
    }
    public IReadOnlyDictionary<long, BuffGraph> GetBuffGraphs(ParsedEvtcLog log)
    {
        SimulateBuffsAndComputeGraphs(log);
        if (AgentItem.IsEnglobedAgent && _buffGraphs.Count == 0)
        {
            var graphs = log.FindActor(EnglobingAgentItem).GetBuffGraphs(log);
            foreach (var graph in graphs)
            {
                BuffGraph buffGraph = graph.Value;    
                _buffGraphs[graph.Key] = BuildBuffGraphInAwareTimesFromEnglobingGraph(log, buffGraph, FirstAware, LastAware);
            }
        }
        return _buffGraphs;
    }

    public IReadOnlyDictionary<long, BuffGraph> GetBuffGraphs(ParsedEvtcLog log, SingleActor by)
    {
        SimulateBuffsAndComputeGraphs(log);
        _buffGraphsPerAgent ??= new(8); //TODO_PERF(Rennorb) @find capacity dependencies
        AgentItem agent = by.AgentItem;
        if (!_buffGraphsPerAgent.TryGetValue(agent, out var result))
        {
            if (AgentItem.IsEnglobedAgent || by.AgentItem.IsEnglobedAgent)
            {
                var graphs = log.FindActor(EnglobingAgentItem).GetBuffGraphs(log, log.FindActor(by.EnglobingAgentItem));
                var buffGraphs = new Dictionary<long, BuffGraph>(graphs.Count);
                _buffGraphsPerAgent[agent] = buffGraphs;
                result = buffGraphs;
                foreach (var graph in graphs)
                {
                    BuffGraph buffGraph = graph.Value;
                    buffGraphs[graph.Key] = BuildBuffGraphInAwareTimesFromEnglobingGraph(log, buffGraph, Math.Max(FirstAware, by.FirstAware), Math.Min(LastAware, by.LastAware));
                }
            }
            else
            {
                var trackedBuffs = GetTrackedBuffs(log);
                var buffGraphs = new Dictionary<long, BuffGraph>(trackedBuffs.Count);
                _buffGraphsPerAgent[agent] = buffGraphs;
                result = buffGraphs;

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
                                graphSegments.Add(new Segment(log.LogData.LogStart, segment.Start, 0));
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
                            graphSegments.Add(new Segment(graphSegments.Last().End, log.LogData.LogEnd, 0));
                        }
                        else
                        {
                            graphSegments.Add(new Segment(log.LogData.LogStart, log.LogData.LogEnd, 0));
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
        }
        return result;
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
        return GetBuffStatus(log, buffID, log.LogData.LogStart, log.LogData.LogEnd);
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
        return GetBuffStatus(log, by, buffID, log.LogData.LogStart, log.LogData.LogEnd);
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
        //TODO_PERF(Rennorb)
        List<Segment> result = [];
        foreach (long id in buffIDs)
        {
            result.AddRange(GetBuffStatus(log, id, start, end));
        }
        return result;
    }

    public List<Segment> GetBuffStatus(ParsedEvtcLog log, long[] buffIDs)
    {
        return GetBuffStatus(log, buffIDs, log.LogData.LogStart, log.LogData.LogEnd);
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
        return GetBuffPresenceStatus(log, buffID, log.LogData.LogStart, log.LogData.LogEnd);
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
        return GetBuffPresenceStatus(log, by, buffID, log.LogData.LogStart, log.LogData.LogEnd);
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

        foreach (Buff buff in GetTrackedBuffs(log))
        {
            if (buffDistribution.HasBuffID(buff.ID))
            {
                (rates[buff.ID], ratesActive[buff.ID]) = BuffByActorStatistics.GetBuffByActor(log, buff, this, start, end, buffDistribution);
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
            if (AgentItem.IsEnglobedAgent)
            {
                _trackedBuffs = log.FindActor(EnglobingAgentItem).GetTrackedBuffs(log).ToHashSet();
            } 
            else
            {
                ComputeBuffMap(log);
            }
        }
        return _trackedBuffs;
    }
    [MemberNotNull(nameof(_buffMap))]
    [MemberNotNull(nameof(_trackedBuffs))]
    internal void ComputeBuffMap(ParsedEvtcLog log)
    {
        if (AgentItem.IsEnglobedAgent)
        {
            var actor = log.FindActor(EnglobingAgentItem);
            actor.ComputeBuffMap(log);
            _buffMap = new BuffDictionary(64, 256, 32, 1);
            _trackedBuffs = actor._trackedBuffs;
            return;
        }
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
    [MemberNotNull(nameof(_buffPresenceBy))]
    [MemberNotNull(nameof(_buffSimulators))]
    internal void SimulateBuffsAndComputeGraphs(ParsedEvtcLog log)
    {
        if (_buffGraphs != null)
        {
#pragma warning disable CS8774 // must have non null
            return;
#pragma warning restore CS8774 // must have non null
        }
        if (AgentItem.IsEnglobedAgent)
        {
            log.FindActor(EnglobingAgentItem).SimulateBuffsAndComputeGraphs(log);
            _buffGraphs = [];
            _buffDistribution = new CachingCollection<BuffDistribution>(log);
            _buffPresenceBy = new CachingCollectionWithTarget<IReadOnlyDictionary<long, long>>(log);
            _buffSimulators = [];
            return;
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
        _buffPresenceBy   = new(log);
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
                    simulator.Simulate(buffEvents, log.LogData.LogStart, log.LogData.LogEnd);
                }
                catch (EIBuffSimulatorIDException e)
                {
                    // get rid of logs invalid for HasStackIDs false
                    log.UpdateProgressWithCancellationCheck("Parsing: Failed id based simulation on " + Character + " for " + buff.Name + " because " + e.Message);
                    buffEvents.RemoveAll(x => !x.IsBuffSimulatorCompliant(false));
                    simulator = buff.CreateSimulator(log, buffStackItemPool, true);
                    simulator.Simulate(buffEvents, log.LogData.LogStart, log.LogData.LogEnd);
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
                        graphSegments.Add(new Segment(log.LogData.LogStart, segment.Start, 0));
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
                    graphSegments.Add(new Segment(graphSegments.Last().End, log.LogData.LogEnd, 0));
                }
                else
                {
                    graphSegments.Add(new Segment(log.LogData.LogStart, log.LogData.LogEnd, 0));
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
        foreach (Minions minions in GetMinions(log))
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
        return GetConsumablesList(log).Where(x => x.Time >= start && x.Time <= end).ToList();
    }

    public IReadOnlyList<Consumable> GetConsumablesList(ParsedEvtcLog log)
    {
        if (_consumeList == null)
        {
            SetConsumablesList(log);
        }
        return _consumeList;
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
                if (c is not BuffApplyEvent ba || !AgentItem.Is(ba.To))
                {
                    continue;
                }
                long time = ba.Time;
                if (time <= log.LogData.LogEnd)
                {
                    Consumable? existing = _consumeList.Find(x => Math.Abs(x.Time - time) < ServerDelayConstant && x.Buff.ID == consumable.ID);
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
