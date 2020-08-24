using GW2EIEvtcParser.ParsedData;
using System.Collections.Generic;
using System.Linq;

namespace GW2EIEvtcParser.EIData
{
    internal class ElementalistHelper : ProfHelper
    {
        internal static readonly List<InstantCastFinder> ElementalistInstantCastFinders = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(5492, 5585, InstantCastFinder.DefaultICD), // Fire
            new BuffGainCastFinder(5493, 5586, InstantCastFinder.DefaultICD), // Water
            new BuffGainCastFinder(5494, 5575, InstantCastFinder.DefaultICD), // Air
            new BuffGainCastFinder(5495, 5580, InstantCastFinder.DefaultICD), // Earth
            new DamageCastFinder(5539, 5539, InstantCastFinder.DefaultICD), // Arcane Blast
            new BuffGiveCastFinder(5635, 5582, InstantCastFinder.DefaultICD), // Arcane Power
            //new BuffGainCastFinder(5641, 5640, InstantCastFinder.DefaultICD), // Arcane Shield - indiscernable from lesser version
            new DamageCastFinder(22572, 22572, InstantCastFinder.DefaultICD), // Arcane Wave
            new BuffGainCastFinder(5543, 5543, InstantCastFinder.DefaultICD), // Mist Form
            new DamageCastFinder(5572, 5572, InstantCastFinder.DefaultICD), // Signet of Air
            new DamageCastFinder(56883, 56883, InstantCastFinder.DefaultICD), // Sunspot
            new DamageCastFinder(56885, 56885, InstantCastFinder.DefaultICD), // Earth Blast
            new DamageCastFinder(5561, 5561, InstantCastFinder.DefaultICD), // Lightning Strike
        };

        private static readonly HashSet<long> _elementalSwaps = new HashSet<long>
        {
            5492,5493,5494, 5495, Buff.FireDual, Buff.FireWater, Buff.FireAir, Buff.FireEarth, Buff.WaterFire, Buff.WaterDual, Buff.WaterAir, Buff.WaterEarth, Buff.AirFire, Buff.AirWater, Buff.AirDual, Buff.AirEarth, Buff.EarthFire, Buff.EarthWater, Buff.EarthAir, Buff.EarthDual
        };

        public static bool IsElementalSwap(long id)
        {
            return _elementalSwaps.Contains(id);
        }

        public static void RemoveDualBuffs(List<AbstractBuffEvent> buffsPerDst, SkillData skillData)
        {
            var duals = new HashSet<long>
            {
                Buff.FireDual,
                Buff.WaterDual,
                Buff.AirDual,
                Buff.EarthDual,
            };
            foreach (AbstractBuffEvent c in buffsPerDst.Where(x => duals.Contains(x.BuffID)))
            {
                c.Invalidate(skillData);
            }
        }
    }
}
