using GW2EIEvtcParser.ParsedData;
using System.Collections.Generic;
using System.Linq;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;

namespace GW2EIEvtcParser.EIData
{
    internal class ElementalistHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(5492, 5585, EIData.InstantCastFinder.DefaultICD), // Fire
            new BuffGainCastFinder(5493, 5586, EIData.InstantCastFinder.DefaultICD), // Water
            new BuffGainCastFinder(5494, 5575, EIData.InstantCastFinder.DefaultICD), // Air
            new BuffGainCastFinder(5495, 5580, EIData.InstantCastFinder.DefaultICD), // Earth
            new DamageCastFinder(5539, 5539, EIData.InstantCastFinder.DefaultICD), // Arcane Blast
            new BuffGiveCastFinder(5635, 5582, EIData.InstantCastFinder.DefaultICD), // Arcane Power
            //new BuffGainCastFinder(5641, 5640, InstantCastFinder.DefaultICD), // Arcane Shield - indiscernable from lesser version
            new DamageCastFinder(22572, 22572, EIData.InstantCastFinder.DefaultICD), // Arcane Wave
            new BuffGainCastFinder(5543, 5543, EIData.InstantCastFinder.DefaultICD), // Mist Form
            new DamageCastFinder(5572, 5572, EIData.InstantCastFinder.DefaultICD), // Signet of Air
            new DamageCastFinder(56883, 56883, EIData.InstantCastFinder.DefaultICD), // Sunspot
            new DamageCastFinder(56885, 56885, EIData.InstantCastFinder.DefaultICD), // Earth Blast
            new DamageCastFinder(5561, 5561, EIData.InstantCastFinder.DefaultICD), // Lightning Strike
        };


        internal static readonly List<Buff> Buffs = new List<Buff>
        {       
                //signets
                new Buff("Signet of Restoration",739, ParserHelper.Source.Elementalist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/dd/Signet_of_Restoration.png"),
                new Buff("Signet of Air",5590, ParserHelper.Source.Elementalist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/9/92/Signet_of_Air.png"),
                new Buff("Signet of Earth",5592, ParserHelper.Source.Elementalist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/ce/Signet_of_Earth.png"),
                new Buff("Signet of Fire",5544, ParserHelper.Source.Elementalist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/b0/Signet_of_Fire.png"),
                new Buff("Signet of Water",5591, ParserHelper.Source.Elementalist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/f/fd/Signet_of_Water.png"),
                ///attunements
                // Fire
                new Buff("Fire Attunement", 5585, ParserHelper.Source.Elementalist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/b4/Fire_Attunement.png"),
                // Water
                new Buff("Water Attunement", 5586, ParserHelper.Source.Elementalist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/3/31/Water_Attunement.png"),
                // Air
                new Buff("Air Attunement", 5575, ParserHelper.Source.Elementalist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/9/91/Air_Attunement.png"),
                // Earth
                new Buff("Earth Attunement", 5580, ParserHelper.Source.Elementalist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/a/a8/Earth_Attunement.png"),
                //forms
                new Buff("Mist Form",5543, ParserHelper.Source.Elementalist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/1b/Mist_Form.png"),
                new Buff("Ride the Lightning",5588, ParserHelper.Source.Elementalist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/5/59/Ride_the_Lightning.png"),
                new Buff("Vapor Form",5620, ParserHelper.Source.Elementalist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/6/6c/Vapor_Form.png"),
                new Buff("Tornado",5583, ParserHelper.Source.Elementalist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/4/40/Tornado.png"),
                //conjures
                new Buff("Conjure Earth Shield", 15788, ParserHelper.Source.Elementalist, BuffNature.SupportBuffTable, "https://wiki.guildwars2.com/images/7/7a/Conjure_Earth_Shield.png"),
                new Buff("Conjure Flame Axe", 15789, ParserHelper.Source.Elementalist, BuffNature.SupportBuffTable, "https://wiki.guildwars2.com/images/a/a1/Conjure_Flame_Axe.png"),
                new Buff("Conjure Frost Bow", 15790, ParserHelper.Source.Elementalist, BuffNature.SupportBuffTable, "https://wiki.guildwars2.com/images/c/c3/Conjure_Frost_Bow.png"),
                new Buff("Conjure Lightning Hammer", 15791, ParserHelper.Source.Elementalist, BuffNature.SupportBuffTable, "https://wiki.guildwars2.com/images/1/1f/Conjure_Lightning_Hammer.png"),
                new Buff("Conjure Fiery Greatsword", 15792, ParserHelper.Source.Elementalist, BuffNature.SupportBuffTable, "https://wiki.guildwars2.com/images/e/e2/Conjure_Fiery_Greatsword.png"),
                //skills
                new Buff("Arcane Power",5582, ParserHelper.Source.Elementalist, BuffStackType.Stacking, 6, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/7/72/Arcane_Power.png"),
                new Buff("Arcane Shield",5640, ParserHelper.Source.Elementalist, BuffStackType.Stacking, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/9/9d/Arcane_Shield.png"),
                new Buff("Renewal of Fire",5764, ParserHelper.Source.Elementalist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/6/63/Renewal_of_Fire.png"),
                new Buff("Rock Barrier",34633, ParserHelper.Source.Elementalist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/dd/Rock_Barrier.png"),//750?
                new Buff("Magnetic Wave",15794, ParserHelper.Source.Elementalist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/3/32/Magnetic_Wave.png"),
                new Buff("Obsidian Flesh",5667, ParserHelper.Source.Elementalist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/c1/Obsidian_Flesh.png"),
                new Buff("Grinding Stones",51658, ParserHelper.Source.Elementalist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/3/3d/Grinding_Stones.png"),
                new Buff("Static Charge",31487, ParserHelper.Source.Elementalist, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/4/4b/Overload_Air.png"),
                new Buff("Persisting Flames",13342, ParserHelper.Source.Elementalist, BuffStackType.Stacking, 10, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/5/5f/Persisting_Flames.png", 104844, ulong.MaxValue),
                new Buff("Fresh Air",34241, ParserHelper.Source.Elementalist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/d8/Fresh_Air.png"),
                new Buff("Soothing Mist", 5587, ParserHelper.Source.Elementalist, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/f/f7/Soothing_Mist.png"),
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
