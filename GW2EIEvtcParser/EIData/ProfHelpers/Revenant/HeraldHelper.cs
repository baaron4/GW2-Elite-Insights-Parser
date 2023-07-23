using System.Collections.Generic;
using GW2EIEvtcParser.EIData.Buffs;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class HeraldHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(LegendaryDragonStanceSkill, LegendaryDragonStanceBuff), // Legendary Dragon Stance
            new BuffGainCastFinder(FacetOfNatureSkill, FacetOfNatureBuff), // Facet of Nature
            new BuffGainCastFinder(FacetOfDarknessSkill, FacetOfDarknessUW), // Facet of Darkness
            new BuffGainCastFinder(FacetOfElementsSkill, FacetOfElementsBuff), // Facet of Elements
            new BuffGainCastFinder(FacetOfStrengthSkill, FacetOfStrengthBuff), // Facet of Strength
            new BuffGainCastFinder(FacetOfChaosSkill, FacetOfChaosBuff), // Facet of Chaos
            new DamageCastFinder(CallOfTheDragon, CallOfTheDragon), // Call of the Dragon
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifier(NumberOfBoons, "Reinforced Potency", "1% per boon", DamageSource.NoPets, 1.0, DamageType.Strike, DamageType.All, Source.Herald, ByStack, BuffImages.EnvoyOfSustenance, DamageModifierMode.All).WithBuilds(GW2Builds.StartOfLife, GW2Builds.August2022Balance),
            new BuffDamageModifier(NumberOfBoons, "Reinforced Potency", "1% per boon", DamageSource.NoPets, 1.0, DamageType.Strike, DamageType.All, Source.Herald, ByStack, BuffImages.EnvoyOfSustenance, DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.August2022Balance),
            new BuffDamageModifier(NumberOfBoons, "Reinforced Potency", "1.5% per boon", DamageSource.NoPets, 1.5, DamageType.Strike, DamageType.All, Source.Herald, ByStack, BuffImages.EnvoyOfSustenance, DamageModifierMode.PvE).WithBuilds(GW2Builds.August2022Balance),
            //
            new BuffDamageModifier(BurstOfStrength, "Burst of Strength", "15%", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Herald, ByPresence, BuffImages.BurstOfStrength, DamageModifierMode.All).WithBuilds(GW2Builds.August2018Balance, GW2Builds.July2019Balance),
            new BuffDamageModifier(BurstOfStrength, "Burst of Strength", "25%", DamageSource.NoPets, 25.0, DamageType.Strike, DamageType.All, Source.Herald, ByPresence, BuffImages.BurstOfStrength, DamageModifierMode.PvE).WithBuilds(GW2Builds.July2019Balance, GW2Builds.November2022Balance),
            new BuffDamageModifier(BurstOfStrength, "Burst of Strength", "15%", DamageSource.NoPets, 15.0, DamageType.StrikeAndCondition, DamageType.All, Source.Herald, ByPresence, BuffImages.BurstOfStrength, DamageModifierMode.PvE).WithBuilds(GW2Builds.November2022Balance),
            new BuffDamageModifier(BurstOfStrength, "Burst of Strength", "15%", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Herald, ByPresence, BuffImages.BurstOfStrength, DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.July2019Balance, GW2Builds.February2020Balance),
            new BuffDamageModifier(BurstOfStrength, "Burst of Strength", "7%", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Herald, ByPresence, BuffImages.BurstOfStrength, DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.February2020Balance, GW2Builds.November2022Balance),
            new BuffDamageModifier(BurstOfStrength, "Burst of Strength", "7%", DamageSource.NoPets, 7.0, DamageType.StrikeAndCondition, DamageType.All, Source.Herald, ByPresence, BuffImages.BurstOfStrength, DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.November2022Balance),
            // 
            new BuffDamageModifier(new long[] { FacetOfChaosBuff, FacetOfDarknessUW, FacetOfElementsBuff, FacetOfNatureBuff, FacetOfStrengthBuff, FacetOfLight }, "Forceful Persistence (Facets)", "4% per active Facet", DamageSource.NoPets, 4.0, DamageType.Strike, DamageType.All, Source.Herald, ByMultiPresence, BuffImages.ForcefulPersistence, DamageModifierMode.PvE).WithBuilds(GW2Builds.August2018Balance, GW2Builds.June2022Balance),
            new BuffDamageModifier(new long[] { FacetOfChaosBuff, FacetOfDarknessUW, FacetOfElementsBuff, FacetOfNatureBuff, FacetOfStrengthBuff, FacetOfLight }, "Forceful Persistence (Facets)", "3% per active Facet", DamageSource.NoPets, 3.0, DamageType.Strike, DamageType.All, Source.Herald, ByMultiPresence, BuffImages.ForcefulPersistence, DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.August2018Balance),
            new BuffDamageModifier(new long[] { FacetOfChaosBuff, FacetOfDarknessUW, FacetOfElementsBuff, FacetOfNatureBuff, FacetOfStrengthBuff, FacetOfLight }, "Forceful Persistence (Facets)", "5% per active Facet", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Herald, ByMultiPresence, BuffImages.ForcefulPersistence, DamageModifierMode.PvE).WithBuilds(GW2Builds.June2022Balance),
            //new BuffDamageModifier(new long[] { 27273, 27581, 28001}, "Forceful Persistence", "13% if active upkeep", DamageSource.NoPets, 13.0, DamageType.Power, DamageType.All, Source.Herald, ByPresence, BuffImages.ForcefulPersistence, GW2Builds.August2018Balance, GW2Builds.EndOfLife, DamageModifierMode.All), // Hammers, Embrace, Impossible Odds but how to track Protective Solace?
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {         
            // Skills
            new Buff("Crystal Hibernation", CrystalHibernation, Source.Herald, BuffClassification.Other, BuffImages.CrystalHibernation).WithBuilds(GW2Builds.StartOfLife, GW2Builds.SOTOBetaAndSilentSurfNM),
            // Facets
            new Buff("Facet of Light", FacetOfLight, Source.Herald, BuffClassification.Other, BuffImages.FacetOfLight),
            new Buff("Facet of Light (Traited)", FacetOfLightTraited, Source.Herald, BuffClassification.Other, BuffImages.FacetOfLight), //Lingering buff with Draconic Echo trait
            new Buff("Infuse Light", InfuseLight, Source.Herald, BuffClassification.Defensive, BuffImages.InfuseLight),
            new Buff("Facet of Darkness", FacetOfDarknessUW, Source.Herald, BuffClassification.Other, BuffImages.FacetOfDarkness),
            new Buff("Facet of Darkness (Traited)", FacetOfDarknessTraited, Source.Herald, BuffClassification.Other, BuffImages.FacetOfDarkness), //Lingering buff with Draconic Echo trait
            new Buff("Facet of Elements", FacetOfElementsBuff, Source.Herald, BuffClassification.Other, BuffImages.FacetOfElements),
            new Buff("Facet of Elements (Traited)", FacetOfElementsTraited, Source.Herald, BuffClassification.Other, BuffImages.FacetOfElements), //Lingering buff with Draconic Echo trait
            new Buff("Facet of Strength", FacetOfStrengthBuff, Source.Herald, BuffClassification.Other, BuffImages.FacetOfStrength),
            new Buff("Facet of Strength (Traited)", FacetOfStrengthTraited, Source.Herald, BuffClassification.Other, BuffImages.FacetOfStrength), //Lingering buff with Draconic Echo trait
            new Buff("Facet of Chaos", FacetOfChaosBuff, Source.Herald, BuffClassification.Other, BuffImages.FacetOfChaos),
            new Buff("Facet of Chaos (Traited)", FacetOfChaosTraited, Source.Herald, BuffClassification.Other, BuffImages.FacetOfChaos),
            new Buff("Facet of Nature", FacetOfNatureBuff, Source.Herald, BuffClassification.Other, BuffImages.FacetOfNature),
            new Buff("Facet of Nature (Traited)", FacetOfNatureTraited, Source.Herald, BuffClassification.Other, BuffImages.FacetOfNature), //Lingering buff with Draconic Echo trait
            new Buff("Facet of Nature-Assassin", FacetOfNatureAssassin, Source.Herald, BuffClassification.Offensive, BuffImages.FacetOfNatureAssassin),
            new Buff("Facet of Nature-Dragon", FacetOfNatureDragon, Source.Herald, BuffClassification.Support, BuffImages.FacetOfNatureDragon),
            new Buff("Facet of Nature-Demon", FacetOfNatureDemon, Source.Herald, BuffClassification.Support, BuffImages.FacetOfNatureDemon),
            new Buff("Facet of Nature-Dwarf", FacetOfNatureDwarf, Source.Herald, BuffClassification.Defensive, BuffImages.FacetOfNatureDwarf),
            new Buff("Facet of Nature-Centaur", FacetOfNatureCentaur, Source.Herald, BuffClassification.Defensive, BuffImages.FacetOfNatureCentaur),
            new Buff("Naturalistic Resonance", NaturalisticResonance, Source.Herald, BuffClassification.Defensive, BuffImages.FacetOfNature),
            new Buff("Legendary Dragon Stance", LegendaryDragonStanceBuff, Source.Herald, BuffClassification.Other, BuffImages.LegendaryDragonStance),
            new Buff("Hardening Persistence", HardeningPersistence, Source.Herald, BuffStackType.Stacking, 10, BuffClassification.Other, BuffImages.HardeningPersistence),
            new Buff("Soothing Bastion", SoothingBastion, Source.Herald, BuffClassification.Other, BuffImages.SoothingBastion),
            new Buff("Burst of Strength", BurstOfStrength, Source.Herald, BuffClassification.Other, BuffImages.BurstOfStrength),
            new Buff("Rising Momentum", RisingMomentum, Source.Herald, BuffStackType.Stacking, 10, BuffClassification.Other, BuffImages.RisingMomentum),
        };
    }
}
