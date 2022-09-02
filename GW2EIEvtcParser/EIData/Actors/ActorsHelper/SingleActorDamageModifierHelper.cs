using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    internal class SingleActorDamageModifierHelper : AbstractSingleActorHelper
    {
        private CachingCollectionWithTarget<Dictionary<string, DamageModifierStat>> _damageModifiersPerTargets;
        private CachingCollectionWithTarget<Dictionary<string, List<DamageModifierEvent>>> _damageModifierEventsPerTargets;

        public SingleActorDamageModifierHelper(AbstractSingleActor actor) : base(actor)
        {
        }

        private Dictionary<string, DamageModifierStat> ComputeDamageModifierStats(AbstractSingleActor target, ParsedEvtcLog log, long start, long end)
        {
            // Check if damage mods against target
            if (_damageModifierEventsPerTargets.TryGetValue(log.FightData.FightStart, log.FightData.FightEnd, target, out Dictionary<string, List<DamageModifierEvent>> events))
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
                _damageModifiersPerTargets.Set(start, end, target, res);
                return res;
            }
            // Check if we already filled the cache, that means no damage modifiers against given target
            else if (_damageModifierEventsPerTargets.TryGetValue(log.FightData.FightStart, log.FightData.FightEnd, null, out events))
            {
                var res = new Dictionary<string, DamageModifierStat>();
                _damageModifiersPerTargets.Set(start, end, target, res);
                return res;
            }
            return null;
        }

        public IReadOnlyDictionary<string, DamageModifierStat> GetDamageModifierStats(AbstractSingleActor target, ParsedEvtcLog log, long start, long end)
        {
            if (!log.ParserSettings.ComputeDamageModifiers || Actor.IsFakeActor)
            {
                return new Dictionary<string, DamageModifierStat>();
            }
            if (_damageModifiersPerTargets == null)
            {
                _damageModifiersPerTargets = new CachingCollectionWithTarget<Dictionary<string, DamageModifierStat>>(log);
                _damageModifierEventsPerTargets = new CachingCollectionWithTarget<Dictionary<string, List<DamageModifierEvent>>>(log);
            }
            if (_damageModifiersPerTargets.TryGetValue(start, end, target, out Dictionary<string, DamageModifierStat> res))
            {
                return res;
            }
            res = ComputeDamageModifierStats(target, log, start, end);
            if (res != null)
            {
                return res;
            }
            //
            var damageMods = new List<DamageModifier>();
            if (log.DamageModifiers.DamageModifiersPerSource.TryGetValue(Source.Item, out IReadOnlyList<DamageModifier> list))
            {
                damageMods.AddRange(list);
            }
            if (log.DamageModifiers.DamageModifiersPerSource.TryGetValue(Source.Gear, out list))
            {
                damageMods.AddRange(list);
            }
            if (log.DamageModifiers.DamageModifiersPerSource.TryGetValue(Source.Common, out list))
            {
                damageMods.AddRange(list);
            }
            if (log.DamageModifiers.DamageModifiersPerSource.TryGetValue(Source.FightSpecific, out list))
            {
                damageMods.AddRange(list);
            }
            damageMods.AddRange(log.DamageModifiers.GetModifiersPerSpec(Actor.Spec));
            //
            var damageModifierEvents = new List<DamageModifierEvent>();
            foreach (DamageModifier damageMod in damageMods)
            {
                damageModifierEvents.AddRange(damageMod.ComputeDamageModifier(Actor, log));
            }
            damageModifierEvents.Sort((x, y) => x.Time.CompareTo(y.Time));
            var damageModifiersEvents = damageModifierEvents.GroupBy(y => y.DamageModifier.Name).ToDictionary(y => y.Key, y => y.ToList());
            _damageModifierEventsPerTargets.Set(log.FightData.FightStart, log.FightData.FightEnd, null, damageModifiersEvents);
            var damageModifiersEventsByTarget = damageModifierEvents.GroupBy(x => x.Dst).ToDictionary(x => x.Key, x => x.GroupBy(y => y.DamageModifier.Name).ToDictionary(y => y.Key, y => y.ToList()));
            foreach (AgentItem actor in damageModifiersEventsByTarget.Keys)
            {
                _damageModifierEventsPerTargets.Set(log.FightData.FightStart, log.FightData.FightEnd, log.FindActor(actor), damageModifiersEventsByTarget[actor]);
            }
            //
            res = ComputeDamageModifierStats(target, log, start, end);
            return res;
        }

        public IReadOnlyCollection<string> GetPresentDamageModifier(ParsedEvtcLog log)
        {
            return new HashSet<string>(GetDamageModifierStats(null, log, log.FightData.FightStart, log.FightData.FightEnd).Keys);
        }

    }
}
