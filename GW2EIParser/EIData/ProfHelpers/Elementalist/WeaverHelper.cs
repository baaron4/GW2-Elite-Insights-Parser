using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.EIData
{
    public class WeaverHelper : ElementalistHelper
    {
        private const long FireMajor = 40926;
        private const long FireMinor = 42811;
        private const long WaterMajor = 43236;
        private const long WaterMinor = 43370;
        private const long AirMajor = 41692;
        private const long AirMinor = 43229;
        private const long EarthMajor = 43740;
        private const long EarthMinor = 44822;

        private static readonly Dictionary<long, HashSet<long>> _minorsTranslation = new Dictionary<long, HashSet<long>>
        {
            { FireMinor, new HashSet<long> { WaterFire, AirFire, EarthFire, FireDual}},
            { WaterMinor, new HashSet<long> { FireWater, AirWater, EarthWater, WaterDual}},
            { AirMinor, new HashSet<long> { FireAir, WaterAir, EarthAir, AirDual}},
            { EarthMinor, new HashSet<long> { FireEarth, WaterEarth, AirEarth, EarthDual}},
        };

        private static readonly Dictionary<long, HashSet<long>> _majorsTranslation = new Dictionary<long, HashSet<long>>
        {
            { FireMajor, new HashSet<long> { FireWater, FireAir, FireEarth, FireDual}},
            { WaterMajor, new HashSet<long> { WaterFire, WaterAir, WaterEarth, WaterDual}},
            { AirMajor, new HashSet<long> { AirFire, AirWater, AirEarth, AirDual}},
            { EarthMajor, new HashSet<long> { EarthFire, EarthWater, EarthAir, EarthDual}},
        };

        private static long TranslateWeaverAttunement(List<BuffApplyEvent> buffApplies)
        {
            // check if more than 3 ids are present
            // Seems to happen when the attunement bug happens
            // removed the throw
            /*if (buffApplies.Select(x => x.BuffID).Distinct().Count() > 3)
            {
                throw new InvalidOperationException("Too much buff apply events in TranslateWeaverAttunement");
            }*/
            var duals = new HashSet<long>
            {
                FireDual,
                WaterDual,
                AirDual,
                EarthDual
            };
            HashSet<long> major = null;
            HashSet<long> minor = null;
            foreach (BuffApplyEvent c in buffApplies)
            {
                if (duals.Contains(c.BuffID))
                {
                    return c.BuffID;
                }
                if (_majorsTranslation.ContainsKey(c.BuffID))
                {
                    major = _majorsTranslation[c.BuffID];
                }
                else if (_minorsTranslation.ContainsKey(c.BuffID))
                {
                    minor = _minorsTranslation[c.BuffID];
                }
            }
            if (major == null || minor == null)
            {
                return 0;
            }
            IEnumerable<long> inter = major.Intersect(minor);
            if (inter.Count() != 1)
            {
                throw new InvalidOperationException("Intersection incorrect in TranslateWeaverAttunement");
            }
            return inter.First();
        }

        public static List<AbstractBuffEvent> TransformWeaverAttunements(List<AbstractBuffEvent> buffs, AgentItem a, SkillData skillData)
        {
            var res = new List<AbstractBuffEvent>();
            var attunements = new HashSet<long>
            {
                5585,
                5586,
                5575,
                5580
            };

            // not useful for us
            /*const long fireAir = 45162;
            const long fireEarth = 42756;
            const long fireWater = 45502;
            const long waterAir = 46418;
            const long waterEarth = 42792;
            const long airEarth = 45683;*/

            var weaverAttunements = new HashSet<long>
            {
               FireMajor,
                FireMinor,
                WaterMajor,
                WaterMinor,
                AirMajor,
                AirMinor,
                EarthMajor,
                EarthMinor,

                FireDual,
                WaterDual,
                AirDual,
                EarthDual,

                /*fireAir,
                fireEarth,
                fireWater,
                waterAir,
                waterEarth,
                airEarth,*/
            };
            // first we get rid of standard attunements
            var attuns = buffs.Where(x => attunements.Contains(x.BuffID)).ToList();
            foreach (AbstractBuffEvent c in attuns)
            {
                c.Invalidate(skillData);
            }
            // get all weaver attunements ids and group them by time
            var weaverAttuns = buffs.Where(x => weaverAttunements.Contains(x.BuffID)).ToList();
            if (weaverAttuns.Count == 0)
            {
                return res;
            }
            var groupByTime = new Dictionary<long, List<AbstractBuffEvent>>();
            foreach (AbstractBuffEvent c in weaverAttuns)
            {
                long key = groupByTime.Keys.FirstOrDefault(x => Math.Abs(x - c.Time) < GeneralHelper.ServerDelayConstant);
                if (key != 0)
                {
                    groupByTime[key].Add(c);
                }
                else
                {
                    groupByTime[c.Time] = new List<AbstractBuffEvent>
                            {
                                c
                            };
                }
            }
            long prevID = 0;
            foreach (KeyValuePair<long, List<AbstractBuffEvent>> pair in groupByTime)
            {
                var applies = pair.Value.OfType<BuffApplyEvent>().ToList();
                long curID = TranslateWeaverAttunement(applies);
                foreach (AbstractBuffEvent c in pair.Value)
                {
                    c.Invalidate(skillData);
                }
                if (curID == 0)
                {
                    continue;
                }
                uint curInstanceID = applies.First().BuffInstance;
                res.Add(new BuffApplyEvent(a, a, pair.Key, int.MaxValue, skillData.Get(curID), curInstanceID, true));
                if (prevID != 0)
                {
                    res.Add(new BuffRemoveManualEvent(a, a, pair.Key, int.MaxValue, skillData.Get(prevID)));
                    res.Add(new BuffRemoveAllEvent(a, a, pair.Key, int.MaxValue, skillData.Get(prevID), 1, int.MaxValue));
                }
                prevID = curID;
            }
            return res;
        }
    }
}
