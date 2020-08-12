using GW2EIEvtcParser.ParsedData;
using System.Collections.Generic;
using System.Linq;

namespace GW2EIEvtcParser.EIData
{
    internal class ElementalistHelper : ProfHelper
    {
        internal static readonly List<InstantCastFinder> ElementalistInstantCastFinders = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(5492, 5585, 500), // Fire
            new BuffGainCastFinder(5493, 5586, 500), // Water
            new BuffGainCastFinder(5494, 5575, 500), // Air
            new BuffGainCastFinder(5495, 5580, 500), // Earth
            new DamageCastFinder(5539, 5539, 500), // Arcane Blast
            new BuffGiveCastFinder(5635, 5582, 500), // Arcane Power
            new BuffGainCastFinder(5641, 5640, 500), // Arcane Shield
            new DamageCastFinder(22572, 22572, 500), // Arcane Wave
            new BuffGainCastFinder(5543, 5543, 500), // Mist Form
            new DamageCastFinder(5572, 5572, 500), // Signet of Air
            new DamageCastFinder(56883, 56883, 500), // Sunspot
            new DamageCastFinder(56885, 56885, 500), // Earth Blast
        };

        private static readonly HashSet<long> _elementalSwaps = new HashSet<long>
        {
            5492,5493,5494, 5495, FireDual, FireWater, FireAir, FireEarth, WaterFire, WaterDual, WaterAir, WaterEarth, AirFire, AirWater, AirDual, AirEarth, EarthFire, EarthWater, EarthAir, EarthDual
        };

        public static bool IsElementalSwap(long id)
        {
            return _elementalSwaps.Contains(id);
        }

        public static void RemoveDualBuffs(List<AbstractBuffEvent> buffsPerDst, SkillData skillData)
        {
            var duals = new HashSet<long>
            {
                FireDual,
                WaterDual,
                AirDual,
                EarthDual,
            };
            foreach (AbstractBuffEvent c in buffsPerDst.Where(x => duals.Contains(x.BuffID)))
            {
                c.Invalidate(skillData);
            }
        }
    }
}
