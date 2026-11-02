using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData;

partial class SingleActor
{
    private CachingCollectionWithTarget<Dictionary<int, DamageModifierStat>>? _outgoingDamageModifierStatsPerTargets;
    private CachingCollectionWithAgentTarget<Dictionary<int, List<DamageModifierEvent>>>? _outgoingDamageModifierEventsPerTargets;
    private CachingCollectionWithTarget<Dictionary<int, DamageModifierStat>>? _incomingDamageModifierStatsPerTargets;
    private CachingCollectionWithAgentTarget<Dictionary<int, List<DamageModifierEvent>>>? _incomingDamageModifierEventsPerTargets;

    #region UTILS
    private static Dictionary<int, List<DamageModifierEvent>> FetchDamageModifierEventsFromDictionary(CachingCollectionWithAgentTarget<Dictionary<int, List<DamageModifierEvent>>> dict, SingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        if (dict.TryGetValue(start, end, target?.AgentItem, out var result))
        {
            return result;
        }
        // Handle englobing targets
        if (target != null && target.AgentItem.IsEnglobedAgent)
        {
            if (dict.TryGetValue(start, end, target.EnglobingAgentItem, out result))
            {
                var englobedResult = new Dictionary<int, List<DamageModifierEvent>>(result.Count);
                foreach (var pair in result)
                {
                    var newValue = pair.Value.Where(x => target.InAwareTimes(x.Time)).ToList();
                    if (newValue.Count > 0)
                    {
                        englobedResult[pair.Key] = newValue;
                    }
                }
                dict.Set(start, end, target.AgentItem, englobedResult);
                return englobedResult;
            }
            else
            {
                return [];
            }
        }
        else
        {
            return [];
        }
    }

    private delegate List<DamageModifier> DamageModifierFetcher(ParsedEvtcLog log, SingleActor actor);
    private static void FillDamageModifierEventsDict(CachingCollectionWithAgentTarget<Dictionary<int, List<DamageModifierEvent>>> dict, ParsedEvtcLog log, SingleActor src, long start, long end, bool useDst, DamageModifierFetcher damageModifierFetcher)
    {
        if (dict.IsEmpty())
        {
            var damageMods = damageModifierFetcher(log, src);

            var damageModifierEvents = new List<DamageModifierEvent>(damageMods.Count * 150);
            foreach (DamageModifier damageMod in damageMods)
            {
                damageModifierEvents.AddRange(damageMod.ComputeDamageModifier(src, log));
            }
            damageModifierEvents.SortByTime();


            var damageModifiersEvents = damageModifierEvents.GroupBy(y => y.DamageModifier.ID).ToDictionary(y => y.Key, y => y.ToList());
            dict.Set(log.LogData.LogStart, log.LogData.LogEnd, null, damageModifiersEvents);

            foreach (var modsByActor in damageModifierEvents.GroupBy(x => useDst ? x.Dst : x.Src))
            {
                var actor = modsByActor.Key;
                var events = modsByActor.GroupBy(y => y.DamageModifier.ID).ToDictionary(y => y.Key, y => y.ToList());
                dict.Set(log.LogData.LogStart, log.LogData.LogEnd, actor, events);
            }
        }

        if (!dict.TryGetValue(start, end, null, out _))
        {
            if (dict.TryGetEnglobingValue(start, end, null, out var allEvents) || dict.TryGetValue(log.LogData.LogStart, log.LogData.LogEnd, null, out allEvents))
            {
                var curDamageModifierEvents = allEvents.Values.SelectMany(x => x.ToList()).Where(x => x.Time >= start && x.Time <= end).ToList();
                curDamageModifierEvents.SortByTime();
                var curDamageModifiersEvents = curDamageModifierEvents.GroupBy(y => y.DamageModifier.ID).ToDictionary(y => y.Key, y => y.ToList());
                dict.Set(start, end, null, curDamageModifiersEvents);

                foreach (var eventsByTarget in curDamageModifierEvents.GroupBy(x => useDst ? x.Dst : x.Src))
                {
                    var actor = eventsByTarget.Key;
                    var events = eventsByTarget.GroupBy(y => y.DamageModifier.ID).ToDictionary(y => y.Key, y => y.ToList());
                    dict.Set(start, end, actor, events);
                }
            }
        }
    }

    private delegate Dictionary<int, List<DamageModifierEvent>> DamageModifierEventsFetcher(SingleActor? target, ParsedEvtcLog log, long start, long end);
    private Dictionary<int, DamageModifierStat> ComputeDamageModifierStats(CachingCollectionWithTarget<Dictionary<int, DamageModifierStat>> dict, SingleActor? target, ParsedEvtcLog log, long start, long end, DamageModifierEventsFetcher damageModifierEventsFetcher)
    {
        if (dict.TryGetValue(start, end, target, out var res))
        {
            return res;
        }
        res = new Dictionary<int, DamageModifierStat>();
        foreach (var pair in damageModifierEventsFetcher(target, log, start, end))
        {
            DamageModifier? damageMod = pair.Value.FirstOrDefault()?.DamageModifier;
            if (damageMod != null)
            {
                var eventsToUse = pair.Value;
                double sum = 0;
                int count = 0;
                foreach (var damageEvent in eventsToUse)
                {
                    sum += damageEvent.DamageGain;
                    count++;
                }
                int totalDamage = damageMod.GetTotalDamage(this, log, target, start, end);
                var typeHits = damageMod.GetDamageEvents(this, log, target, start, end);
                res[pair.Key] = new DamageModifierStat(count, typeHits.Count, sum, totalDamage);
            }
        }
        dict.Set(start, end, target, res);
        return res;
    }
    #endregion UTILS
    #region OUTGOING
    private Dictionary<int, List<DamageModifierEvent>> GetOutgoingDamageModifierEvents(SingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        _outgoingDamageModifierEventsPerTargets ??= new(AgentItem, log);
        FillDamageModifierEventsDict(_outgoingDamageModifierEventsPerTargets, log, this, start, end, true, (log, actor) =>
        {
            var damageMods = new List<DamageModifier>(40);
            if (log.DamageModifiers.OutgoingDamageModifiersPerSource.TryGetValue(Source.Item, out var list))
            {
                damageMods.AddRange(list);
            }

            if (log.DamageModifiers.OutgoingDamageModifiersPerSource.TryGetValue(Source.Gear, out list))
            {
                damageMods.AddRange(list);
            }

            if (log.DamageModifiers.OutgoingDamageModifiersPerSource.TryGetValue(Source.Common, out list))
            {
                damageMods.AddRange(list);
            }

            if (log.DamageModifiers.OutgoingDamageModifiersPerSource.TryGetValue(Source.EncounterSpecific, out list))
            {
                damageMods.AddRange(list);
            }
            // If this is a player that is englobing, that means we can expect spec to change during the course of the log
            // ignore spec specific damage modifiers for them
            if (!AgentItem.IsEnglobingAgent && AgentItem.IsPlayer)
            {
                damageMods.AddRange(log.DamageModifiers.GetPersonalOutgoingModifiersPerSpec(Spec));
            }
            return damageMods;
        });
        return FetchDamageModifierEventsFromDictionary(_outgoingDamageModifierEventsPerTargets, target, log, start, end);
    }
    private Dictionary<int, DamageModifierStat> ComputeOutgoingDamageModifierStats(SingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        _outgoingDamageModifierStatsPerTargets ??= new(AgentItem, log);
        return ComputeDamageModifierStats(_outgoingDamageModifierStatsPerTargets, target, log, start, end, GetOutgoingDamageModifierEvents);
    }

    public IReadOnlyDictionary<int, DamageModifierStat> GetOutgoingDamageModifierStats(SingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        if (!log.ParserSettings.CanComputeDamageModifiers || IsFakeActor)
        {
            return new Dictionary<int, DamageModifierStat>();
        }
        return ComputeOutgoingDamageModifierStats(target, log, start, end);

    }

    public IReadOnlyCollection<int> GetPresentOutgoingDamageModifier(ParsedEvtcLog log)
    {
        return new HashSet<int>(GetOutgoingDamageModifierStats(null, log, log.LogData.LogStart, log.LogData.LogEnd).Keys);
    }
    #endregion OUTGOING
    #region INCOMING
    private Dictionary<int, List<DamageModifierEvent>> GetIncomingDamageModifierEvents(SingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        _incomingDamageModifierEventsPerTargets ??= new(AgentItem, log);
        FillDamageModifierEventsDict(_incomingDamageModifierEventsPerTargets, log, this, start, end, false, (log, actor) =>
        {
            var damageMods = new List<DamageModifier>(40);
            if (log.DamageModifiers.IncomingDamageModifiersPerSource.TryGetValue(Source.Item, out var list))
            {
                damageMods.AddRange(list);
            }

            if (log.DamageModifiers.IncomingDamageModifiersPerSource.TryGetValue(Source.Gear, out list))
            {
                damageMods.AddRange(list);
            }

            if (log.DamageModifiers.IncomingDamageModifiersPerSource.TryGetValue(Source.Common, out list))
            {
                damageMods.AddRange(list);
            }

            if (log.DamageModifiers.IncomingDamageModifiersPerSource.TryGetValue(Source.EncounterSpecific, out list))
            {
                damageMods.AddRange(list);
            }
            // If this is a player that is englobing, that means we can expect spec to change during the course of the log
            // ignore spec specific damage modifiers for them
            if (!AgentItem.IsEnglobingAgent && AgentItem.IsPlayer)
            {
                damageMods.AddRange(log.DamageModifiers.GetPersonalIncomingModifiersPerSpec(Spec));
            }
            return damageMods;
        });
        return FetchDamageModifierEventsFromDictionary(_incomingDamageModifierEventsPerTargets, target, log, start, end);
    }

    private Dictionary<int, DamageModifierStat> ComputeIncomingDamageModifierStats(SingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        _incomingDamageModifierStatsPerTargets ??= new(AgentItem, log);
        return ComputeDamageModifierStats(_incomingDamageModifierStatsPerTargets, target, log, start, end, GetIncomingDamageModifierEvents);
    }

    public IReadOnlyDictionary<int, DamageModifierStat> GetIncomingDamageModifierStats(SingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        if (!log.ParserSettings.CanComputeDamageModifiers || IsFakeActor)
        {
            return new Dictionary<int, DamageModifierStat>();
        }
        return ComputeIncomingDamageModifierStats(target, log, start, end);

    }

    public IReadOnlyCollection<int> GetPresentIncomingDamageModifier(ParsedEvtcLog log)
    {
        return new HashSet<int>(GetIncomingDamageModifierStats(null, log, log.LogData.LogStart, log.LogData.LogEnd).Keys);
    }
    #endregion INCOMING
}
