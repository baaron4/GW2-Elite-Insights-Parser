using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class WeaverHelper : ElementalistHelper
    {
        // Weaver attunements
        public const long FireWater = -5;
        public const long FireAir = -6;
        public const long FireEarth = -7;
        public const long WaterFire = -8;
        public const long WaterAir = -9;
        public const long WaterEarth = -10;
        public const long AirFire = -11;
        public const long AirWater = -12;
        public const long AirEarth = -13;
        public const long EarthFire = -14;
        public const long EarthWater = -15;
        public const long EarthAir = -16;

        private const long _fireMajor = 40926;
        private const long _fireMinor = 42811;
        private const long _waterMajor = 43236;
        private const long _waterMinor = 43370;
        private const long _airMajor = 41692;
        private const long _airMinor = 43229;
        private const long _earthMajor = 43740;
        private const long _earthMinor = 44822;

        public const long FireDual = 43470;
        public const long WaterDual = 41166;
        public const long AirDual = 42264;
        public const long EarthDual = 44857;

        private static Dictionary<long, HashSet<long>> _minorsTranslation = new Dictionary<long, HashSet<long>>
        {
            { _fireMinor, new HashSet<long> { WaterFire, AirFire, EarthFire}},
            { _waterMinor, new HashSet<long> { FireWater, AirWater, EarthWater}},
            { _airMinor, new HashSet<long> { FireAir, WaterAir, EarthAir}},
            { _earthMinor, new HashSet<long> { FireEarth, WaterEarth, AirEarth}},
        };

        private static Dictionary<long, HashSet<long>> _majorsTranslation = new Dictionary<long, HashSet<long>>
        {
            { _fireMajor, new HashSet<long> { FireWater, FireAir, FireEarth}},
            { _waterMajor, new HashSet<long> { WaterFire, WaterAir, WaterEarth}},
            { _airMajor, new HashSet<long> { AirFire, AirWater, AirEarth}},
            { _earthMajor, new HashSet<long> { EarthFire, EarthWater, EarthAir}},
        };

        private static long TranslateWeaverAttunement(List<CombatItem> buffApplies)
        {
            if (buffApplies.Count > 3)
            {
                throw new InvalidOperationException("Too much buff apply events in TranslateWeaverAttunement");
            }
            HashSet<long> duals = new HashSet<long>
            {
                FireDual,
                WaterDual,
                AirDual,
                EarthDual
            };
            HashSet<long> majors = new HashSet<long>
            {
                _fireMajor,
                _waterMajor,
                _airMajor,
                _earthMajor
            };
            HashSet<long> minors = new HashSet<long>
            {
                _fireMinor,
                _waterMinor,
                _airMinor,
                _earthMinor
            };
            HashSet<long> major = null;
            HashSet<long> minor = null;
            foreach (CombatItem c in buffApplies)
            {
                if (duals.Contains(c.SkillID))
                {
                    return c.SkillID;
                }
                if (majors.Contains(c.SkillID))
                {
                    major = _majorsTranslation[c.SkillID];
                }
                else if (minors.Contains(c.SkillID))
                {
                    minor = _minorsTranslation[c.SkillID];
                }
            }
            if (major == null || minor == null)
            {
                throw new InvalidOperationException("Missing major or minor in TranslateWeaverAttunement");
            }
            IEnumerable<long> inter = major.Intersect(minor);
            if (inter.Count() != 1)
            {
                throw new InvalidOperationException("Intersection incorrect in TranslateWeaverAttunement");
            }
            return inter.First();
        }

        public static void TransformWeaverAttunements(Player p, Dictionary<ushort, List<CombatItem>> buffsPerInst)
        {
            HashSet<long> attunements = new HashSet<long>
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

            HashSet<long> weaverAttunements = new HashSet<long>
            {
               _fireMajor,
                _fireMinor,
                _waterMajor,
                _waterMinor,
                _airMajor,
                _airMinor,
                _earthMajor,
                _earthMinor,

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
            List<CombatItem> buffs = buffsPerInst[p.InstID].Where(x => x.Time <= p.LastAware && x.Time >= p.FirstAware).ToList();
            // first we get rid of standard attunements
            List<CombatItem> attuns = buffs.Where(x => attunements.Contains(x.SkillID)).ToList();
            foreach (CombatItem c in attuns)
            {
                c.OverrideSkillID(Boon.NoBuff);
            }
            // get all weaver attunements ids and group them by time
            List<CombatItem> weaverAttuns = buffs.Where(x => weaverAttunements.Contains(x.SkillID)).ToList();
            if (weaverAttuns.Count == 0)
            {
                return;
            }
            Dictionary<long, List<CombatItem>> groupByTime = new Dictionary<long, List<CombatItem>>();
            foreach (CombatItem c in weaverAttuns)
            {
                long key = groupByTime.Keys.FirstOrDefault(x => Math.Abs(x - c.Time) < 10);
                if (key != 0)
                {
                    groupByTime[key].Add(c);
                }
                else
                {
                    groupByTime[c.Time] = new List<CombatItem>
                            {
                                c
                            };
                }
            }
            long prevID = 0;
            foreach(List<CombatItem> items in groupByTime.Values)
            {
                List<CombatItem> applies = items.Where(x => x.IsBuffRemove == Parser.ParseEnum.BuffRemove.None).ToList();
                List<CombatItem> removals = items.Where(x => x.IsBuffRemove != Parser.ParseEnum.BuffRemove.None).ToList();
                long curID = TranslateWeaverAttunement(applies);
                for (int i = 0; i < applies.Count; i++)
                {
                    if (i == 0)
                    {
                        applies[i].OverrideSkillID(curID);
                    }
                    else
                    {
                        applies[i].OverrideSkillID(Boon.NoBuff);
                    }
                }
                bool removeAll = false;
                bool removeManual = false;
                foreach (CombatItem c in removals)
                {
                    if (!removeAll && c.IsBuffRemove == Parser.ParseEnum.BuffRemove.All)
                    {
                        c.OverrideSkillID(prevID);
                        removeAll = true;
                    }
                    else if (!removeManual && c.IsBuffRemove == Parser.ParseEnum.BuffRemove.Manual)
                    {
                        c.OverrideSkillID(prevID);
                        removeManual = true;
                    }
                    else
                    {
                        c.OverrideSkillID(Boon.NoBuff);
                    }
                }
                prevID = curID;
            }
        }
    }
}
