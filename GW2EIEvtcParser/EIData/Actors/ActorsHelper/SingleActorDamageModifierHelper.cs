using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    internal class SingleActorDamageModifierHelper : AbstractSingleActorHelper
    {
        private CachingCollectionWithTarget<Dictionary<string, DamageModifierStat>> _outgoingDamageModifiersPerTargets;
        private CachingCollectionWithTarget<Dictionary<string, List<DamageModifierEvent>>> _outgoingDamageModifierEventsPerTargets;
        private CachingCollectionWithTarget<Dictionary<string, DamageModifierStat>> _incomingDamageModifiersPerTargets;
        private CachingCollectionWithTarget<Dictionary<string, List<DamageModifierEvent>>> _incomingDamageModifierEventsPerTargets;

        public SingleActorDamageModifierHelper(AbstractSingleActor actor) : base(actor)
        {
        }

        private Dictionary<string, DamageModifierStat> ComputeDamageModifierStats(AbstractSingleActor target, ParsedEvtcLog log, long start, long end)
        {
            // Check if damage mods against target
            if (_outgoingDamageModifierEventsPerTargets.TryGetValue(log.FightData.FightStart, log.FightData.FightEnd, target, out Dictionary<string, List<DamageModifierEvent>> events))
            {
                var res = new Dictionary<string, DamageModifierStat>();
                foreach (KeyValuePair<string, List<DamageModifierEvent>> pair in events)
                {
                    DamageModifier damageMod = pair.Value.FirstOrDefault()?.DamageModifier;
                    if (damageMod != null)
                    {
                        var eventsToUse = pair.Value.Where(x => x.Time >= start && x.Time <= end).ToList();
                        int totalDamage = damageMod.GetTotalDamage(Actor, log, target, start, end);
                        IReadOnlyList<AbstractHealthDamageEvent> typeHits = damageMod.GetHitDamageEvents(Actor, log, target, start, end);
                        res[pair.Key] = new DamageModifierStat(eventsToUse.Count, typeHits.Count, eventsToUse.Sum(x => x.DamageGain), totalDamage);
                    }
                }
                _outgoingDamageModifiersPerTargets.Set(start, end, target, res);
                return res;
            }
            // Check if we already filled the cache, that means no damage modifiers against given target
            else if (_outgoingDamageModifierEventsPerTargets.TryGetValue(log.FightData.FightStart, log.FightData.FightEnd, null, out events))
            {
                var res = new Dictionary<string, DamageModifierStat>();
                _outgoingDamageModifiersPerTargets.Set(start, end, target, res);
                return res;
            }
            return null;
        }

        public IReadOnlyDictionary<string, DamageModifierStat> GetOutgoingDamageModifierStats(AbstractSingleActor target, ParsedEvtcLog log, long start, long end)
        {
            if (!log.ParserSettings.ComputeDamageModifiers || Actor.IsFakeActor)
            {
                return new Dictionary<string, DamageModifierStat>();
            }
            if (_outgoingDamageModifiersPerTargets == null)
            {
                _outgoingDamageModifiersPerTargets = new CachingCollectionWithTarget<Dictionary<string, DamageModifierStat>>(log);
                _outgoingDamageModifierEventsPerTargets = new CachingCollectionWithTarget<Dictionary<string, List<DamageModifierEvent>>>(log);
            }
            if (_outgoingDamageModifiersPerTargets.TryGetValue(start, end, target, out Dictionary<string, DamageModifierStat> res))
            {
                return res;
            }
            res = ComputeDamageModifierStats(target, log, start, end);
            if (res != null)
            {
                return res;
            }
            //
            var damageMods = new List<OutgoingDamageModifier>();
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
            damageMods.AddRange(log.DamageModifiers.GetOutgoingModifiersPerSpec(Actor.Spec));
            //
            var damageModifierEvents = new List<DamageModifierEvent>();
            foreach (OutgoingDamageModifier damageMod in damageMods)
            {
                damageModifierEvents.AddRange(damageMod.ComputeDamageModifier(Actor, log));
            }
            damageModifierEvents.Sort((x, y) => x.Time.CompareTo(y.Time));
            var damageModifiersEvents = damageModifierEvents.GroupBy(y => y.DamageModifier.Name).ToDictionary(y => y.Key, y => y.ToList());
            _outgoingDamageModifierEventsPerTargets.Set(log.FightData.FightStart, log.FightData.FightEnd, null, damageModifiersEvents);
            var damageModifiersEventsByTarget = damageModifierEvents.GroupBy(x => x.Dst).ToDictionary(x => x.Key, x => x.GroupBy(y => y.DamageModifier.Name).ToDictionary(y => y.Key, y => y.ToList()));
            foreach (AgentItem actor in damageModifiersEventsByTarget.Keys)
            {
                _outgoingDamageModifierEventsPerTargets.Set(log.FightData.FightStart, log.FightData.FightEnd, log.FindActor(actor), damageModifiersEventsByTarget[actor]);
            }
            //
            res = ComputeDamageModifierStats(target, log, start, end);
            return res;
        }

        public IReadOnlyCollection<string> GetPresentOutgoingDamageModifier(ParsedEvtcLog log)
        {
            return new HashSet<string>(GetOutgoingDamageModifierStats(null, log, log.FightData.FightStart, log.FightData.FightEnd).Keys);
        }

        private Dictionary<string, DamageModifierStat> ComputeIncomingDamageModifierStats(AbstractSingleActor target, ParsedEvtcLog log, long start, long end)
        {
            // Check if damage mods against target
            if (_incomingDamageModifierEventsPerTargets.TryGetValue(log.FightData.FightStart, log.FightData.FightEnd, target, out Dictionary<string, List<DamageModifierEvent>> events))
            {
                var res = new Dictionary<string, DamageModifierStat>();
                foreach (KeyValuePair<string, List<DamageModifierEvent>> pair in events)
                {
                    DamageModifier damageMod = pair.Value.FirstOrDefault()?.DamageModifier;
                    if (damageMod != null)
                    {
                        var eventsToUse = pair.Value.Where(x => x.Time >= start && x.Time <= end).ToList();
                        int totalDamage = damageMod.GetTotalDamage(Actor, log, target, start, end);
                        IReadOnlyList<AbstractHealthDamageEvent> typeHits = damageMod.GetHitDamageEvents(Actor, log, target, start, end);
                        res[pair.Key] = new DamageModifierStat(eventsToUse.Count, typeHits.Count, eventsToUse.Sum(x => x.DamageGain), totalDamage);
                    }
                }
                _incomingDamageModifiersPerTargets.Set(start, end, target, res);
                return res;
            }
            // Check if we already filled the cache, that means no damage modifiers against given target
            else if (_incomingDamageModifierEventsPerTargets.TryGetValue(log.FightData.FightStart, log.FightData.FightEnd, null, out events))
            {
                var res = new Dictionary<string, DamageModifierStat>();
                _incomingDamageModifiersPerTargets.Set(start, end, target, res);
                return res;
            }
            return null;
        }

        public IReadOnlyDictionary<string, DamageModifierStat> GetIncomingDamageModifierStats(AbstractSingleActor target, ParsedEvtcLog log, long start, long end)
        {
            if (!log.ParserSettings.ComputeDamageModifiers || Actor.IsFakeActor)
            {
                return new Dictionary<string, DamageModifierStat>();
            }
            if (_incomingDamageModifiersPerTargets == null)
            {
                _incomingDamageModifiersPerTargets = new CachingCollectionWithTarget<Dictionary<string, DamageModifierStat>>(log);
                _incomingDamageModifierEventsPerTargets = new CachingCollectionWithTarget<Dictionary<string, List<DamageModifierEvent>>>(log);
            }
            if (_incomingDamageModifiersPerTargets.TryGetValue(start, end, target, out Dictionary<string, DamageModifierStat> res))
            {
                return res;
            }
            res = ComputeIncomingDamageModifierStats(target, log, start, end);
            if (res != null)
            {
                return res;
            }
            //
            var damageMods = new List<IncomingDamageModifier>();
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
            damageMods.AddRange(log.DamageModifiers.GetIncomingModifiersPerSpec(Actor.Spec));
            //
            var damageModifierEvents = new List<DamageModifierEvent>();
            foreach (IncomingDamageModifier damageMod in damageMods)
            {
                damageModifierEvents.AddRange(damageMod.ComputeDamageModifier(Actor, log));
            }
            damageModifierEvents.Sort((x, y) => x.Time.CompareTo(y.Time));
            var damageModifiersEvents = damageModifierEvents.GroupBy(y => y.DamageModifier.Name).ToDictionary(y => y.Key, y => y.ToList());
            _incomingDamageModifierEventsPerTargets.Set(log.FightData.FightStart, log.FightData.FightEnd, null, damageModifiersEvents);
            var damageModifiersEventsByTarget = damageModifierEvents.GroupBy(x => x.Src).ToDictionary(x => x.Key, x => x.GroupBy(y => y.DamageModifier.Name).ToDictionary(y => y.Key, y => y.ToList()));
            foreach (AgentItem actor in damageModifiersEventsByTarget.Keys)
            {
                _incomingDamageModifierEventsPerTargets.Set(log.FightData.FightStart, log.FightData.FightEnd, log.FindActor(actor), damageModifiersEventsByTarget[actor]);
            }
            //
            res = ComputeIncomingDamageModifierStats(target, log, start, end);
            return res;
        }

        public IReadOnlyCollection<string> GetPresentIncomingDamageModifier(ParsedEvtcLog log)
        {
            return new HashSet<string>(GetIncomingDamageModifierStats(null, log, log.FightData.FightStart, log.FightData.FightEnd).Keys);
        }

    }
}
