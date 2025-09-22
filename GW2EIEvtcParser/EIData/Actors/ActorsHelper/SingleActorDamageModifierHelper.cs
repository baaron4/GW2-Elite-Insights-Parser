using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData;

partial class SingleActor
{
    private CachingCollectionWithTarget<Dictionary<int, DamageModifierStat>>? _outgoingDamageModifierStatsPerTargets;
    private CachingCollectionWithTarget<Dictionary<int, List<DamageModifierEvent>>>? _outgoingDamageModifierEventsPerTargets;
    private CachingCollectionWithTarget<Dictionary<int, DamageModifierStat>>? _incomingDamageModifierStatsPerTargets;
    private CachingCollectionWithTarget<Dictionary<int, List<DamageModifierEvent>>>? _incomingDamageModifierEventsPerTargets;

    private Dictionary<int, DamageModifierStat> ComputeDamageModifierStats(SingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        if (_outgoingDamageModifierEventsPerTargets!.TryGetValue(start, end, target, out var events))
        {
            var res = new Dictionary<int, DamageModifierStat>();
            foreach (KeyValuePair<int, List<DamageModifierEvent>> pair in events)
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
                    var typeHits = damageMod.GetHitDamageEvents(this, log, target, start, end);
                    res[pair.Key] = new DamageModifierStat(count, typeHits.Count, sum, totalDamage);
                }
            }
            _outgoingDamageModifierStatsPerTargets!.Set(start, end, target, res);
            return res;
        }
        else
        {
            var res = new Dictionary<int, DamageModifierStat>();
            _outgoingDamageModifierStatsPerTargets!.Set(start, end, target, res);
            return res;
        }
    }

    public IReadOnlyDictionary<int, DamageModifierStat> GetOutgoingDamageModifierStats(SingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        if (!log.ParserSettings.ComputeDamageModifiers || IsFakeActor)
        {
            return new Dictionary<int, DamageModifierStat>();
        }

        if (_outgoingDamageModifierStatsPerTargets == null)
        {
            _outgoingDamageModifierStatsPerTargets = new CachingCollectionWithTarget<Dictionary<int, DamageModifierStat>>(log);
            _outgoingDamageModifierEventsPerTargets = new CachingCollectionWithTarget<Dictionary<int, List<DamageModifierEvent>>>(log);
        }

        if (_outgoingDamageModifierStatsPerTargets.TryGetValue(start, end, target, out var res))
        {
            return res;
        }
        if (_outgoingDamageModifierEventsPerTargets!.IsEmpty())
        {
            var damageMods = new List<OutgoingDamageModifier>(40);
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

            damageMods.AddRange(log.DamageModifiers.GetOutgoingModifiersPerSpec(Spec));

            var damageModifierEvents = new List<DamageModifierEvent>(damageMods.Count * 150);
            foreach (OutgoingDamageModifier damageMod in damageMods)
            {
                damageModifierEvents.AddRange(damageMod.ComputeDamageModifier(this, log));
            }
            damageModifierEvents.SortByTime();


            var damageModifiersEvents = damageModifierEvents.GroupBy(y => y.DamageModifier.ID).ToDictionary(y => y.Key, y => y.ToList());
            _outgoingDamageModifierEventsPerTargets!.Set(log.LogData.LogStart, log.LogData.LogEnd, null, damageModifiersEvents);

            foreach (var modsByActor in damageModifierEvents.GroupBy(x => x.Dst))
            {
                var actor = modsByActor.Key;
                var events = modsByActor.GroupBy(y => y.DamageModifier.ID).ToDictionary(y => y.Key, y => y.ToList());
                _outgoingDamageModifierEventsPerTargets.Set(log.LogData.LogStart, log.LogData.LogEnd, log.FindActor(actor), events);
            }
        }

        if (!_outgoingDamageModifierEventsPerTargets.TryGetValue(start, end, null, out _))
        {
            if (!_outgoingDamageModifierEventsPerTargets.TryGetEnglobingValue(start, end, null, out var allEvents))
            {
                _outgoingDamageModifierEventsPerTargets.TryGetValue(log.LogData.LogStart, log.LogData.LogEnd, null, out allEvents);
            }
            var curDamageModifierEvents = allEvents!.Values.SelectMany(x => x.ToList()).Where(x => x.Time >= start && x.Time <= end).ToList();
            curDamageModifierEvents.SortByTime();
            var curDamageModifiersEvents = curDamageModifierEvents.GroupBy(y => y.DamageModifier.ID).ToDictionary(y => y.Key, y => y.ToList());
            _outgoingDamageModifierEventsPerTargets!.Set(start, end, null, curDamageModifiersEvents);

            foreach (var eventsByTarget in curDamageModifierEvents.GroupBy(x => x.Dst))
            {
                var actor = eventsByTarget.Key;
                var events = eventsByTarget.GroupBy(y => y.DamageModifier.ID).ToDictionary(y => y.Key, y => y.ToList());
                _outgoingDamageModifierEventsPerTargets.Set(start, end, log.FindActor(actor), events);
            }
        }
        return ComputeDamageModifierStats(target, log, start, end);
        
    }

    private Dictionary<int, DamageModifierStat> ComputeIncomingDamageModifierStats(SingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        if (_incomingDamageModifierEventsPerTargets!.TryGetValue(start, end, target, out var events))
        {
            var res = new Dictionary<int, DamageModifierStat>(events.Count);
            foreach (KeyValuePair<int, List<DamageModifierEvent>> pair in events)
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
                    var typeHits = damageMod.GetHitDamageEvents(this, log, target, start, end);
                    res[pair.Key] = new DamageModifierStat(count, typeHits.Count, sum, totalDamage);
                }
            }
            _incomingDamageModifierStatsPerTargets!.Set(start, end, target, res);

            return res;
        }
        else
        {
            var res = new Dictionary<int, DamageModifierStat>();
            _incomingDamageModifierStatsPerTargets!.Set(start, end, target, res);
            return res;
        }
    }

    public IReadOnlyDictionary<int, DamageModifierStat> GetIncomingDamageModifierStats(SingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        if (!log.ParserSettings.ComputeDamageModifiers || IsFakeActor)
        {
            return new Dictionary<int, DamageModifierStat>();
        }

        if (_incomingDamageModifierStatsPerTargets == null)
        {
            _incomingDamageModifierStatsPerTargets = new CachingCollectionWithTarget<Dictionary<int, DamageModifierStat>>(log);
            _incomingDamageModifierEventsPerTargets = new CachingCollectionWithTarget<Dictionary<int, List<DamageModifierEvent>>>(log);
        }

        if (_incomingDamageModifierStatsPerTargets.TryGetValue(start, end, target, out var res))
        {
            return res;
        }

        if (_incomingDamageModifierEventsPerTargets!.IsEmpty())
        {
            var damageMods = new List<IncomingDamageModifier>(32);
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
            damageMods.AddRange(log.DamageModifiers.GetIncomingModifiersPerSpec(Spec));

            var damageModifierEvents = new List<DamageModifierEvent>(damageMods.Count * 60);
            foreach (IncomingDamageModifier damageMod in damageMods)
            {
                damageModifierEvents.AddRange(damageMod.ComputeDamageModifier(this, log));
            }
            damageModifierEvents.SortByTime();

            var damageModifiersEvents = damageModifierEvents.GroupBy(y => y.DamageModifier.ID).ToDictionary(y => y.Key, y => y.ToList());
            _incomingDamageModifierEventsPerTargets!.Set(log.LogData.LogStart, log.LogData.LogEnd, null, damageModifiersEvents);

            foreach (var eventsByTarget in damageModifierEvents.GroupBy(x => x.Src))
            {
                var actor = eventsByTarget.Key;
                var events = eventsByTarget.GroupBy(y => y.DamageModifier.ID).ToDictionary(y => y.Key, y => y.ToList());
                _incomingDamageModifierEventsPerTargets.Set(log.LogData.LogStart, log.LogData.LogEnd, log.FindActor(actor), events);
            }
        }

        if (!_incomingDamageModifierEventsPerTargets.TryGetValue(start, end, null, out _))
        {
            if (!_incomingDamageModifierEventsPerTargets.TryGetEnglobingValue(start, end, null, out var allEvents))
            {
                _incomingDamageModifierEventsPerTargets.TryGetValue(log.LogData.LogStart, log.LogData.LogEnd, null, out allEvents);
            }
            var curDamageModifierEvents = allEvents!.Values.SelectMany(x => x.ToList()).Where(x => x.Time >= start && x.Time <= end).ToList();
            curDamageModifierEvents.SortByTime();
            var curDamageModifiersEvents = curDamageModifierEvents.GroupBy(y => y.DamageModifier.ID).ToDictionary(y => y.Key, y => y.ToList());
            _incomingDamageModifierEventsPerTargets!.Set(start, end, null, curDamageModifiersEvents);

            foreach (var eventsByTarget in curDamageModifierEvents.GroupBy(x => x.Src))
            {
                var actor = eventsByTarget.Key;
                var events = eventsByTarget.GroupBy(y => y.DamageModifier.ID).ToDictionary(y => y.Key, y => y.ToList());
                _incomingDamageModifierEventsPerTargets.Set(start, end, log.FindActor(actor), events);
            }
        }
        return ComputeIncomingDamageModifierStats(target, log, start, end);       
    }


    public IReadOnlyCollection<int> GetPresentOutgoingDamageModifier(ParsedEvtcLog log)
    {
        return new HashSet<int>(GetOutgoingDamageModifierStats(null, log, log.LogData.LogStart, log.LogData.LogEnd).Keys);
    }

    public IReadOnlyCollection<int> GetPresentIncomingDamageModifier(ParsedEvtcLog log)
    {
        return new HashSet<int>(GetIncomingDamageModifierStats(null, log, log.LogData.LogStart, log.LogData.LogEnd).Keys);
    }

}
