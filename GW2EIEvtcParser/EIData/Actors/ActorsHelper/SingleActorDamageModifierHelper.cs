using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData;

partial class AbstractSingleActor
{
    private CachingCollectionWithTarget<Dictionary<string, DamageModifierStat>>? _outgoingDamageModifiersPerTargets;
    private CachingCollectionWithTarget<Dictionary<string, List<DamageModifierEvent>>>? _outgoingDamageModifierEventsPerTargets;
    private CachingCollectionWithTarget<Dictionary<string, DamageModifierStat>>? _incomingDamageModifiersPerTargets;
    private CachingCollectionWithTarget<Dictionary<string, List<DamageModifierEvent>>>? _incomingDamageModifierEventsPerTargets;

    private Dictionary<string, DamageModifierStat>? ComputeDamageModifierStats(AbstractSingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        // Check if damage mods against target
        if (_outgoingDamageModifierEventsPerTargets!.TryGetValue(log.FightData.FightStart, log.FightData.FightEnd, target, out var events))
        {
            var res = new Dictionary<string, DamageModifierStat>();
            foreach (KeyValuePair<string, List<DamageModifierEvent>> pair in events)
            {
                DamageModifier? damageMod = pair.Value.FirstOrDefault()?.DamageModifier;
                if (damageMod != null)
                {
                    var eventsToUse = pair.Value.Where(x => x.Time >= start && x.Time <= end).ToList();
                    int totalDamage = damageMod.GetTotalDamage(this, log, target, start, end);
                    var typeHits = damageMod.GetHitDamageEvents(this, log, target, start, end);
                    res[pair.Key] = new DamageModifierStat(eventsToUse.Count, typeHits.Count(), eventsToUse.Sum(x => x.DamageGain), totalDamage);
                }
            }
            _outgoingDamageModifiersPerTargets!.Set(start, end, target, res);
            return res;
        }
        // Check if we already filled the cache, that means no damage modifiers against given target
        else if (_outgoingDamageModifierEventsPerTargets.TryGetValue(log.FightData.FightStart, log.FightData.FightEnd, null, out events))
        {
            var res = new Dictionary<string, DamageModifierStat>();
            _outgoingDamageModifiersPerTargets!.Set(start, end, target, res);
            return res;
        }

        return null;
    }

    public IReadOnlyDictionary<string, DamageModifierStat> GetOutgoingDamageModifierStats(AbstractSingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        if (!log.ParserSettings.ComputeDamageModifiers || IsFakeActor)
        {
            return new Dictionary<string, DamageModifierStat>();
        }

        if (_outgoingDamageModifiersPerTargets == null)
        {
            _outgoingDamageModifiersPerTargets = new CachingCollectionWithTarget<Dictionary<string, DamageModifierStat>>(log, 8, 2);
            _outgoingDamageModifierEventsPerTargets = new CachingCollectionWithTarget<Dictionary<string, List<DamageModifierEvent>>>(log, 1, 1);
        }

        if (_outgoingDamageModifiersPerTargets.TryGetValue(start, end, target, out var res))
        {
            return res;
        }

        res = ComputeDamageModifierStats(target, log, start, end);
        if (res != null)
        {
            return res;
        }

        var damageMods = new List<OutgoingDamageModifier>(40);
        if (log.DamageModifiers.OutgoingDamageModifiersPerSource.TryGetValue(Source.Item, out IReadOnlyList<OutgoingDamageModifier> list))
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

        if (log.DamageModifiers.OutgoingDamageModifiersPerSource.TryGetValue(Source.FightSpecific, out list))
        {
            damageMods.AddRange(list);
        }

        damageMods.AddRange(log.DamageModifiers.GetOutgoingModifiersPerSpec(this.Spec));

        var damageModifierEvents = new List<DamageModifierEvent>(damageMods.Count * 150);
        foreach (OutgoingDamageModifier damageMod in damageMods)
        {
            damageModifierEvents.AddRange(damageMod.ComputeDamageModifier(this, log));
        }
        damageModifierEvents.SortByTime();


        var damageModifiersEvents = damageModifierEvents.GroupBy(y => y.DamageModifier.Name).ToDictionary(y => y.Key, y => y.ToList());
        _outgoingDamageModifierEventsPerTargets!.Set(log.FightData.FightStart, log.FightData.FightEnd, null, damageModifiersEvents);

        foreach (var modsByActor in damageModifierEvents.GroupBy(x => x.Dst))
        {
            var actor = modsByActor.Key;
            var events = modsByActor.GroupBy(y => y.DamageModifier.Name).ToDictionary(y => y.Key, y => y.ToList());
            _outgoingDamageModifierEventsPerTargets.Set(log.FightData.FightStart, log.FightData.FightEnd, log.FindActor(actor), events);
        }
        
        return ComputeDamageModifierStats(target, log, start, end)!;
    }

    public IReadOnlyCollection<string> GetPresentOutgoingDamageModifier(ParsedEvtcLog log)
    {
        return new HashSet<string>(GetOutgoingDamageModifierStats(null, log, log.FightData.FightStart, log.FightData.FightEnd).Keys);
    }

    private Dictionary<string, DamageModifierStat>? ComputeIncomingDamageModifierStats(AbstractSingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        // Check if damage mods against target
        if (_incomingDamageModifierEventsPerTargets!.TryGetValue(log.FightData.FightStart, log.FightData.FightEnd, target, out var events))
        {
            var res = new Dictionary<string, DamageModifierStat>(events.Count);
            foreach (KeyValuePair<string, List<DamageModifierEvent>> pair in events)
            {
                DamageModifier? damageMod = pair.Value.FirstOrDefault()?.DamageModifier;
                if (damageMod != null)
                {
                    var eventsToUse = pair.Value.Where(x => x.Time >= start && x.Time <= end).ToList();
                    int totalDamage = damageMod.GetTotalDamage(this, log, target, start, end);
                    var typeHits = damageMod.GetHitDamageEvents(this, log, target, start, end);
                    res[pair.Key] = new DamageModifierStat(eventsToUse.Count, typeHits.Count(), eventsToUse.Sum(x => x.DamageGain), totalDamage);
                }
            }
            _incomingDamageModifiersPerTargets!.Set(start, end, target, res);

            return res;
        }
        // Check if we already filled the cache, that means no damage modifiers against given target
        else if (_incomingDamageModifierEventsPerTargets.TryGetValue(log.FightData.FightStart, log.FightData.FightEnd, null, out events))
        {
            var res = new Dictionary<string, DamageModifierStat>();
            _incomingDamageModifiersPerTargets!.Set(start, end, target, res);
            return res;
        }
        return null;
    }

    public IReadOnlyDictionary<string, DamageModifierStat> GetIncomingDamageModifierStats(AbstractSingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        if (!log.ParserSettings.ComputeDamageModifiers || this.IsFakeActor)
        {
            return new Dictionary<string, DamageModifierStat>();
        }

        if (_incomingDamageModifiersPerTargets == null)
        {
            _incomingDamageModifiersPerTargets = new CachingCollectionWithTarget<Dictionary<string, DamageModifierStat>>(log, 8, 2);
            _incomingDamageModifierEventsPerTargets = new CachingCollectionWithTarget<Dictionary<string, List<DamageModifierEvent>>>(log, 1, 1);
        }

        if (_incomingDamageModifiersPerTargets.TryGetValue(start, end, target, out var res))
        {
            return res;
        }

        res = ComputeIncomingDamageModifierStats(target, log, start, end);
        if (res != null)
        {
            return res;
        }

        var damageMods = new List<IncomingDamageModifier>(32);
        if (log.DamageModifiers.IncomingDamageModifiersPerSource.TryGetValue(Source.Item, out IReadOnlyList<IncomingDamageModifier> list))
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

        if (log.DamageModifiers.IncomingDamageModifiersPerSource.TryGetValue(Source.FightSpecific, out list))
        {
            damageMods.AddRange(list);
        }
        damageMods.AddRange(log.DamageModifiers.GetIncomingModifiersPerSpec(this.Spec));
        
        var damageModifierEvents = new List<DamageModifierEvent>(damageMods.Count * 60);
        foreach (IncomingDamageModifier damageMod in damageMods)
        {
            damageModifierEvents.AddRange(damageMod.ComputeDamageModifier(this, log));
        }
        damageModifierEvents.SortByTime();

        var damageModifiersEvents = damageModifierEvents.GroupBy(y => y.DamageModifier.Name).ToDictionary(y => y.Key, y => y.ToList());
        _incomingDamageModifierEventsPerTargets!.Set(log.FightData.FightStart, log.FightData.FightEnd, null, damageModifiersEvents);

        foreach (var eventsByTarget in damageModifierEvents.GroupBy(x => x.Src))
        {
            var actor = eventsByTarget.Key;
            var events = eventsByTarget.GroupBy(y => y.DamageModifier.Name).ToDictionary(y => y.Key, y => y.ToList());
            _incomingDamageModifierEventsPerTargets.Set(log.FightData.FightStart, log.FightData.FightEnd, log.FindActor(actor), events);
        }

        return ComputeIncomingDamageModifierStats(target, log, start, end)!;
    }

    public IReadOnlyCollection<string> GetPresentIncomingDamageModifier(ParsedEvtcLog log)
    {
        return new HashSet<string>(GetIncomingDamageModifierStats(null, log, log.FightData.FightStart, log.FightData.FightEnd).Keys);
    }

}
