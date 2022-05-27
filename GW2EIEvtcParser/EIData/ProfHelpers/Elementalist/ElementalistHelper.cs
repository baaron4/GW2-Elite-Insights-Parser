using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class ElementalistHelper
    {
        // TODO - add glyph of elemental power stuff

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(FireAttunementSkill, FireAttunementEffect, EIData.InstantCastFinder.DefaultICD), // Fire
            new BuffGainCastFinder(WaterAttunementSkill, WaterAttunementEffect, EIData.InstantCastFinder.DefaultICD), // Water
            new BuffGainCastFinder(AirAttunementSkill, AirAttunementEffect, EIData.InstantCastFinder.DefaultICD), // Air
            new BuffGainCastFinder(EarthAttunementSkill, EarthAttunementEffect, EIData.InstantCastFinder.DefaultICD), // Earth
            new DamageCastFinder(ArcaneBlast, ArcaneBlast, EIData.InstantCastFinder.DefaultICD), // Arcane Blast
            new BuffGiveCastFinder(AranePowerSkill, ArcanePowerEffect, EIData.InstantCastFinder.DefaultICD), // Arcane Power
            new BuffGainCastFinder(ArcaneShieldSkill, ArcaneShieldEffect, EIData.InstantCastFinder.DefaultICD), // Arcane Shield
            new DamageCastFinder(ArcaneWave, ArcaneWave, EIData.InstantCastFinder.DefaultICD), // Arcane Wave
            new BuffGainCastFinder(MistForm, MistForm, EIData.InstantCastFinder.DefaultICD), // Mist Form
            new DamageCastFinder(SignetOfAirSkill, SignetOfAirSkill, EIData.InstantCastFinder.DefaultICD), // Signet of Air
            new DamageCastFinder(Sunspot, Sunspot, EIData.InstantCastFinder.DefaultICD), // Sunspot
            new DamageCastFinder(EarthBlast, EarthBlast, EIData.InstantCastFinder.DefaultICD), // Earth Blast
            new DamageCastFinder(LightningStrike, LightningStrike, EIData.InstantCastFinder.DefaultICD), // Lightning Strike
            new DamageCastFinder(LightningRod, LightningRod, EIData.InstantCastFinder.DefaultICD), // Lightning Rod
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            // Fire
            new BuffDamageModifier(PersistingFlames, "Persisting Flames", "1% per stack", DamageSource.NoPets, 1.0, DamageType.Strike, DamageType.All, Source.Elementalist, ByStack, "https://wiki.guildwars2.com/images/5/5f/Persisting_Flames.png", 104844 , GW2Builds.EndOfLife, DamageModifierMode.All),
            new BuffDamageModifier(new long[] { FireAttunementEffect, FireWaterAttunement, FireAirAttunement, FireEarthAttunement, DualFireAttunement }, "Pyromancer's Training", "10% while fire attuned", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Elementalist, ByPresence, "https://wiki.guildwars2.com/images/e/e6/Pyromancer%27s_Training.png", 0, GW2Builds.July2019Balance, DamageModifierMode.PvE),
            new BuffDamageModifierTarget(Burning, "Burning Rage", "10% on burning target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Elementalist, ByPresence, "https://wiki.guildwars2.com/images/b/bd/Burning_Rage.png", 0, GW2Builds.July2019Balance, DamageModifierMode.PvE),
            new BuffDamageModifierTarget(Burning, "Pyromancer's Training", "10% on burning target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Elementalist, ByPresence, "https://wiki.guildwars2.com/images/e/e6/Pyromancer%27s_Training.png", GW2Builds.July2019Balance, GW2Builds.EndOfLife, DamageModifierMode.All),
            // Air
            new DamageLogDamageModifier( "Bolt to the Heart", "20% if target <50% HP", DamageSource.NoPets, 20.0, DamageType.Strike, DamageType.All, Source.Elementalist,"https://wiki.guildwars2.com/images/f/f8/Bolt_to_the_Heart.png", (x, log) => x.AgainstUnderFifty, ByPresence, DamageModifierMode.All),
            // Earth
            new BuffDamageModifierTarget(Bleeding, "Serrated Stones", "5% to bleeding target", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Elementalist, ByPresence, "https://wiki.guildwars2.com/images/6/60/Serrated_Stones.png", DamageModifierMode.All),
            // Water
            new DamageLogDamageModifier("Aquamancer's Training", "10% over 90% HP", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Elementalist,"https://wiki.guildwars2.com/images/8/81/Aquamancer%27s_Training.png", (x, log) => x.IsOverNinety, ByPresence, 0, GW2Builds.July2019Balance, DamageModifierMode.PvE),
            new BuffDamageModifierTarget(Vulnerability, new long[] { WaterAttunementEffect, WaterAirAttunement, WaterEarthAttunement, WaterFireAttunement, DualWaterAttunement }, "Piercing Shards w/ Water", "20% on vuln target while on water", DamageSource.NoPets, 20.0, DamageType.Strike, DamageType.All, Source.Elementalist, ByPresence, ByPresence, "https://wiki.guildwars2.com/images/4/4b/Piercing_Shards.png", GW2Builds.July2019Balance, GW2Builds.EndOfLife, DamageModifierMode.PvE),
            new BuffDamageModifierTarget(Vulnerability, new long[] { WaterAttunementEffect, WaterAirAttunement, WaterEarthAttunement, WaterFireAttunement, DualWaterAttunement }, "Piercing Shards", "10% on vuln target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Elementalist, ByPresence, ByAbsence, "https://wiki.guildwars2.com/images/4/4b/Piercing_Shards.png", GW2Builds.July2019Balance, GW2Builds.EndOfLife, DamageModifierMode.PvE),
            new BuffDamageModifierTarget(Vulnerability, new long[] { WaterAttunementEffect, WaterAirAttunement, WaterEarthAttunement, WaterFireAttunement, DualWaterAttunement }, "Piercing Shards w/ Water", "10% on vuln target while on water", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Elementalist, ByPresence, ByPresence, "https://wiki.guildwars2.com/images/4/4b/Piercing_Shards.png", GW2Builds.July2019Balance, GW2Builds.EndOfLife, DamageModifierMode.sPvPWvW),
            new BuffDamageModifierTarget(Vulnerability, new long[] { WaterAttunementEffect, WaterAirAttunement, WaterEarthAttunement, WaterFireAttunement, DualWaterAttunement }, "Piercing Shards", "5% on vuln target", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Elementalist, ByPresence, ByAbsence, "https://wiki.guildwars2.com/images/4/4b/Piercing_Shards.png", GW2Builds.July2019Balance, GW2Builds.EndOfLife, DamageModifierMode.sPvPWvW),
            new BuffDamageModifierTarget(Vulnerability, new long[] { WaterAttunementEffect, WaterAirAttunement, WaterEarthAttunement, WaterFireAttunement, DualWaterAttunement }, "Piercing Shards", "20% on vuln target while on water", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Elementalist, ByPresence, ByPresence, "https://wiki.guildwars2.com/images/4/4b/Piercing_Shards.png", 0, GW2Builds.July2019Balance, DamageModifierMode.PvE),
            new DamageLogDamageModifier("Flow like Water", "10% over >75%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Elementalist, "https://wiki.guildwars2.com/images/0/0a/Flow_like_Water.png", (x, log) => x.From.GetCurrentHealthPercent(log, x.Time) >= 75.0, ByPresence, GW2Builds.July2019Balance, GW2Builds.February2020Balance, DamageModifierMode.All).UsingApproximate(true),
            new DamageLogDamageModifier("Flow like Water", "10% over >75%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Elementalist, "https://wiki.guildwars2.com/images/0/0a/Flow_like_Water.png", (x, log) => x.From.GetCurrentHealthPercent(log, x.Time) >= 75.0, ByPresence, GW2Builds.February2020Balance, GW2Builds.EndOfLife, DamageModifierMode.PvE).UsingApproximate(true),
            new DamageLogDamageModifier("Flow like Water", "5% over >75%", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Elementalist, "https://wiki.guildwars2.com/images/0/0a/Flow_like_Water.png", (x, log) => x.From.GetCurrentHealthPercent(log, x.Time) >= 75.0, ByPresence, GW2Builds.February2020Balance, GW2Builds.EndOfLife, DamageModifierMode.sPvPWvW).UsingApproximate(true),
            //new DamageLogDamageModifier("Flow like Water", "10% over 75% HP", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ParseHelper.Source.Elementalist,"https://wiki.guildwars2.com/images/0/0a/Flow_like_Water.png", x => x.IsOverNinety, ByPresence, GW2Builds.July2019Balance, GW2Builds.EndOfLife),
            // Arcane
            new BuffDamageModifier(NumberOfBoons, "Bountiful Power", "2% per boon", DamageSource.NoPets, 2.0, DamageType.Strike, DamageType.All, Source.Elementalist, ByStack, "https://wiki.guildwars2.com/images/7/75/Bountiful_Power.png", DamageModifierMode.All),
        };


        internal static readonly List<Buff> Buffs = new List<Buff>
        {       
                //signets
                new Buff("Signet of Restoration",SignetOfRestoration, Source.Elementalist, BuffClassification.Other, "https://wiki.guildwars2.com/images/d/dd/Signet_of_Restoration.png"),
                new Buff("Signet of Air",SignetOfAirEffect, Source.Elementalist, BuffClassification.Other, "https://wiki.guildwars2.com/images/9/92/Signet_of_Air.png"),
                new Buff("Signet of Earth",SignetOfEarth, Source.Elementalist, BuffClassification.Other, "https://wiki.guildwars2.com/images/c/ce/Signet_of_Earth.png"),
                new Buff("Signet of Fire",SignetOfFire, Source.Elementalist, BuffClassification.Other, "https://wiki.guildwars2.com/images/b/b0/Signet_of_Fire.png"),
                new Buff("Signet of Water",SignetOfWater, Source.Elementalist, BuffClassification.Other, "https://wiki.guildwars2.com/images/f/fd/Signet_of_Water.png"),
                ///attunements
                // Fire
                new Buff("Fire Attunement", FireAttunementEffect, Source.Elementalist, BuffClassification.Other, "https://wiki.guildwars2.com/images/b/b4/Fire_Attunement.png"),
                // Water
                new Buff("Water Attunement", WaterAttunementEffect, Source.Elementalist, BuffClassification.Other, "https://wiki.guildwars2.com/images/3/31/Water_Attunement.png"),
                // Air
                new Buff("Air Attunement", AirAttunementEffect, Source.Elementalist, BuffClassification.Other, "https://wiki.guildwars2.com/images/9/91/Air_Attunement.png"),
                // Earth
                new Buff("Earth Attunement", EarthAttunementEffect, Source.Elementalist, BuffClassification.Other, "https://wiki.guildwars2.com/images/a/a8/Earth_Attunement.png"),
                //forms
                new Buff("Mist Form",MistForm, Source.Elementalist, BuffClassification.Other, "https://wiki.guildwars2.com/images/1/1b/Mist_Form.png"),
                new Buff("Mist Form 2",MistForm2, Source.Elementalist, BuffClassification.Other, "https://wiki.guildwars2.com/images/1/1b/Mist_Form.png"),
                new Buff("Ride the Lightning",RideTheLightning, Source.Elementalist, BuffClassification.Other, "https://wiki.guildwars2.com/images/5/59/Ride_the_Lightning.png"),
                new Buff("Vapor Form",VaporForm, Source.Elementalist, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/6c/Vapor_Form.png"),
                new Buff("Tornado",Tornado, Source.Elementalist, BuffClassification.Other, "https://wiki.guildwars2.com/images/4/40/Tornado.png"),
                new Buff("Whirlpool",Whirlpool, Source.Elementalist, BuffClassification.Other, "https://wiki.guildwars2.com/images/4/41/Whirlpool.png"),
                new Buff("Electrified Tornado",ElectrifiedTornado, Source.Elementalist, BuffClassification.Other, "https://wiki.guildwars2.com/images/2/29/Chain_Lightning.png"),
                //conjures
                new Buff("Conjure Earth Shield", ConjureEarthShield, Source.Elementalist, BuffClassification.Support, "https://wiki.guildwars2.com/images/7/7a/Conjure_Earth_Shield.png"),
                new Buff("Conjure Flame Axe", ConjureFlameAxe, Source.Elementalist, BuffClassification.Support, "https://wiki.guildwars2.com/images/a/a1/Conjure_Flame_Axe.png"),
                new Buff("Conjure Frost Bow", ConjureFrostBow, Source.Elementalist, BuffClassification.Support, "https://wiki.guildwars2.com/images/c/c3/Conjure_Frost_Bow.png"),
                new Buff("Conjure Lightning Hammer", ConjureLightningHammer, Source.Elementalist, BuffClassification.Support, "https://wiki.guildwars2.com/images/1/1f/Conjure_Lightning_Hammer.png"),
                new Buff("Conjure Fiery Greatsword", ConjureFieryGreatsword, Source.Elementalist, BuffClassification.Support, "https://wiki.guildwars2.com/images/e/e2/Conjure_Fiery_Greatsword.png"),
                // Summons
                new Buff("Lesser Air Elemental Summoned", LesserAirElementalSummoned, Source.Elementalist, BuffClassification.Other, "https://wiki.guildwars2.com/images/a/af/Glyph_of_Lesser_Elementals_%28air%29.png"),
                new Buff("Air Elemental Summoned", AirElementalSummoned, Source.Elementalist, BuffClassification.Other, "https://wiki.guildwars2.com/images/8/84/Glyph_of_Elementals_%28air%29.png"),
                new Buff("Lesser Water Elemental Summoned", LesserWaterElementalSummoned, Source.Elementalist, BuffClassification.Other, "https://wiki.guildwars2.com/images/4/49/Glyph_of_Lesser_Elementals_%28water%29.png"),
                new Buff("Water Elemental Summoned", WaterElementalSummoned, Source.Elementalist, BuffClassification.Other, "https://wiki.guildwars2.com/images/c/c6/Glyph_of_Elementals_%28water%29.png"),
                new Buff("Lesser Fire Elemental Summoned", LesserFireElementalSummoned, Source.Elementalist, BuffClassification.Other, "https://wiki.guildwars2.com/images/1/18/Glyph_of_Lesser_Elementals_%28fire%29.png"),
                new Buff("Fire Elemental Summoned", FireElementalSummoned, Source.Elementalist, BuffClassification.Other, "https://wiki.guildwars2.com/images/0/01/Glyph_of_Elementals_%28fire%29.png"),
                new Buff("Lesser Earth Elemental Summoned", LesserEarthElementalSummoned, Source.Elementalist, BuffClassification.Other, "https://wiki.guildwars2.com/images/0/0f/Glyph_of_Lesser_Elementals_%28earth%29.png"),
                new Buff("Earth Elemental Summoned", EarthElementalSummoned, Source.Elementalist, BuffClassification.Other, "https://wiki.guildwars2.com/images/e/e4/Glyph_of_Elementals_%28earth%29.png"),
                //skills
                new Buff("Arcane Power",ArcanePowerEffect, Source.Elementalist, BuffStackType.Stacking, 6, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/72/Arcane_Power.png"),
                new Buff("Arcane Shield",ArcaneShieldEffect, Source.Elementalist, BuffStackType.Stacking, 25, BuffClassification.Other, "https://wiki.guildwars2.com/images/9/9d/Arcane_Shield.png"),
                new Buff("Renewal of Fire",RenewalOfFire, Source.Elementalist, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/63/Renewal_of_Fire.png"),
                new Buff("Rock Barrier",RockBarrier, Source.Elementalist, BuffClassification.Other, "https://wiki.guildwars2.com/images/d/dd/Rock_Barrier.png"),//750?
                new Buff("Magnetic Wave",MagneticWave, Source.Elementalist, BuffClassification.Other, "https://wiki.guildwars2.com/images/3/32/Magnetic_Wave.png"),
                new Buff("Obsidian Flesh",ObsidianFlesh, Source.Elementalist, BuffClassification.Other, "https://wiki.guildwars2.com/images/c/c1/Obsidian_Flesh.png"),
                new Buff("Persisting Flames",PersistingFlames, Source.Elementalist, BuffStackType.Stacking, 10, BuffClassification.Other, "https://wiki.guildwars2.com/images/5/5f/Persisting_Flames.png", 104844, GW2Builds.EndOfLife),
                new Buff("Fresh Air",FreshAir, Source.Elementalist, BuffClassification.Other, "https://wiki.guildwars2.com/images/d/d8/Fresh_Air.png"),
                new Buff("Soothing Mist", SoothingMist, Source.Elementalist, BuffClassification.Defensive, "https://wiki.guildwars2.com/images/f/f7/Soothing_Mist.png"),
                new Buff("Stone Heart", StoneHeart, Source.Elementalist, BuffClassification.Defensive, "https://wiki.guildwars2.com/images/6/60/Stone_Heart.png"),
        };


        private static readonly HashSet<long> _elementalSwaps = new HashSet<long>
        {
            FireAttunementSkill,WaterAttunementSkill,AirAttunementSkill, EarthAttunementSkill, DualFireAttunement, FireWaterAttunement, FireAirAttunement, FireEarthAttunement, WaterFireAttunement, DualWaterAttunement, WaterAirAttunement, WaterEarthAttunement, AirFireAttunement, AirWaterAttunement, DualAirAttunement, AirEarthAttunement, EarthFireAttunement, EarthWaterAttunement, EarthAirAttunement, DualEarthAttunement
        };

        public static bool IsElementalSwap(long id)
        {
            return _elementalSwaps.Contains(id);
        }

        public static void RemoveDualBuffs(IReadOnlyList<AbstractBuffEvent> buffsPerDst, Dictionary<long, List<AbstractBuffEvent>> buffsByID, SkillData skillData)
        {
            var duals = new HashSet<long>
            {
                DualFireAttunement,
                DualWaterAttunement,
                DualAirAttunement,
                DualEarthAttunement,
            };
            var toClean = new HashSet<long>();
            foreach (AbstractBuffEvent c in buffsPerDst.Where(x => duals.Contains(x.BuffID)))
            {
                toClean.Add(c.BuffID);
                c.Invalidate(skillData);
            }
            foreach (long buffID in toClean)
            {
                buffsByID[buffID].RemoveAll(x => x.BuffID == NoBuff);
            }
        }

        private static HashSet<long> Minions = new HashSet<long>()
        {
            (int)MinionID.LesserAirElemental,
            (int)MinionID.LesserEarthElemental,
            (int)MinionID.LesserFireElemental,
            (int)MinionID.LesserIceElemental,
            (int)MinionID.AirElemental,
            (int)MinionID.EarthElemental,
            (int)MinionID.FireElemental,
            (int)MinionID.IceElemental,
        };
        internal static bool IsKnownMinionID(long id)
        {
            return Minions.Contains(id);
        }
    }
}
