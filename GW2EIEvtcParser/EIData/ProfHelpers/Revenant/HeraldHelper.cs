using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.DamageModifierIDs;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData;

internal static class HeraldHelper
{
    internal static readonly List<InstantCastFinder> InstantCastFinder =
    [
        new BuffGainCastFinder(LegendaryDragonStanceSkill, LegendaryDragonStanceBuff),
        new BuffGainCastFinder(FacetOfNatureSkill, FacetOfNatureBuff),
        new BuffGainCastFinder(FacetOfDarknessSkill, FacetOfDarknessBuff),
        new BuffGainCastFinder(FacetOfElementsSkill, FacetOfElementsBuff),
        new BuffGainCastFinder(FacetOfStrengthSkill, FacetOfStrengthBuff),
        new BuffGainCastFinder(FacetOfChaosSkill, FacetOfChaosBuff),
        new DamageCastFinder(CallOfTheDragon, CallOfTheDragon),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> OutgoingDamageModifiers =
    [
        new BuffOnActorDamageModifier(Mod_ReinforcedPotency, NumberOfBoons, "Reinforced Potency", "1% per boon", DamageSource.NoPets, 1.0, DamageType.Strike, DamageType.All, Source.Herald, ByStack, TraitImages.EnvoyOfSustenance, DamageModifierMode.All)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.August2022Balance),
        new BuffOnActorDamageModifier(Mod_ReinforcedPotency, NumberOfBoons, "Reinforced Potency", "1% per boon", DamageSource.NoPets, 1.0, DamageType.Strike, DamageType.All, Source.Herald, ByStack, TraitImages.EnvoyOfSustenance, DamageModifierMode.sPvPWvW)
            .WithBuilds(GW2Builds.August2022Balance, GW2Builds.June2024Balance),
        new BuffOnActorDamageModifier(Mod_ReinforcedPotency, NumberOfBoons, "Reinforced Potency", "1.5% per boon", DamageSource.NoPets, 1.5, DamageType.Strike, DamageType.All, Source.Herald, ByStack, TraitImages.EnvoyOfSustenance, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.August2022Balance, GW2Builds.June2024Balance),
        new BuffOnActorDamageModifier(Mod_ReinforcedPotency, NumberOfBoons, "Reinforced Potency", "1.5% per boon", DamageSource.NoPets, 1.5, DamageType.Strike, DamageType.All, Source.Herald, ByStack, TraitImages.EnvoyOfSustenance, DamageModifierMode.PvEsPvP)
            .WithBuilds(GW2Builds.June2024Balance, GW2Builds.February2025BalancePatch),
        new BuffOnActorDamageModifier(Mod_ReinforcedPotency, NumberOfBoons, "Reinforced Potency", "1.0% per boon", DamageSource.NoPets, 1.0, DamageType.Strike, DamageType.All, Source.Herald, ByStack, TraitImages.EnvoyOfSustenance, DamageModifierMode.WvW)
            .WithBuilds(GW2Builds.June2024Balance, GW2Builds.February2025BalancePatch),
        new BuffOnActorDamageModifier(Mod_ReinforcedPotency, NumberOfBoons, "Reinforced Potency", "1.5% per boon", DamageSource.NoPets, 1.5, DamageType.Strike, DamageType.All, Source.Herald, ByStack, TraitImages.EnvoyOfSustenance, DamageModifierMode.sPvP)
            .WithBuilds(GW2Builds.February2025BalancePatch),
        new BuffOnActorDamageModifier(Mod_ReinforcedPotency, NumberOfBoons, "Reinforced Potency", "1.0% per boon", DamageSource.NoPets, 1.0, DamageType.Strike, DamageType.All, Source.Herald, ByStack, TraitImages.EnvoyOfSustenance, DamageModifierMode.PvEsPvP)
            .WithBuilds(GW2Builds.February2025BalancePatch),
        //
        new BuffOnActorDamageModifier(Mod_BurstOfStrength, BurstOfStrength, "Burst of Strength", "15%", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Herald, ByPresence, SkillImages.BurstOfStrength, DamageModifierMode.All)
            .WithBuilds(GW2Builds.August2018Balance, GW2Builds.July2019Balance),
        new BuffOnActorDamageModifier(Mod_BurstOfStrength, BurstOfStrength, "Burst of Strength", "25%", DamageSource.NoPets, 25.0, DamageType.Strike, DamageType.All, Source.Herald, ByPresence, SkillImages.BurstOfStrength, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.July2019Balance, GW2Builds.November2022Balance),
        new BuffOnActorDamageModifier(Mod_BurstOfStrength, BurstOfStrength, "Burst of Strength", "15%", DamageSource.NoPets, 15.0, DamageType.StrikeAndCondition, DamageType.All, Source.Herald, ByPresence, SkillImages.BurstOfStrength, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.November2022Balance, GW2Builds.February2025BalancePatch),
        new BuffOnActorDamageModifier(Mod_BurstOfStrength, BurstOfStrength, "Burst of Strength", "15%", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Herald, ByPresence, SkillImages.BurstOfStrength, DamageModifierMode.sPvPWvW)
            .WithBuilds(GW2Builds.July2019Balance, GW2Builds.February2020Balance),
        new BuffOnActorDamageModifier(Mod_BurstOfStrength, BurstOfStrength, "Burst of Strength", "7%", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Herald, ByPresence, SkillImages.BurstOfStrength, DamageModifierMode.sPvPWvW)
            .WithBuilds(GW2Builds.February2020Balance, GW2Builds.November2022Balance),
        new BuffOnActorDamageModifier(Mod_BurstOfStrength, BurstOfStrength, "Burst of Strength", "7%", DamageSource.NoPets, 7.0, DamageType.StrikeAndCondition, DamageType.All, Source.Herald, ByPresence, SkillImages.BurstOfStrength, DamageModifierMode.sPvPWvW)
            .WithBuilds(GW2Builds.November2022Balance),
        new BuffOnActorDamageModifier(Mod_BurstOfStrength, BurstOfStrength, "Burst of Strength (Strike)", "10%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Herald, ByPresence, SkillImages.BurstOfStrength, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.February2025BalancePatch),
        new BuffOnActorDamageModifier(Mod_BurstOfStrengthCondition, BurstOfStrength, "Burst of Strength (Condition)", "5%", DamageSource.NoPets, 5.0, DamageType.Condition, DamageType.All, Source.Herald, ByPresence, SkillImages.BurstOfStrength, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.February2025BalancePatch),
        // 
        new BuffOnActorDamageModifier(Mod_ForcefulPersistenceFacets, [FacetOfChaosBuff, FacetOfDarknessBuff, FacetOfElementsBuff, FacetOfNatureBuff, FacetOfStrengthBuff, FacetOfLightBuff], "Forceful Persistence (Facets)", "3% per active Facet", DamageSource.NoPets, 3.0, DamageType.Strike, DamageType.All, Source.Herald, ByMultiPresence, TraitImages.ForcefulPersistence, DamageModifierMode.sPvPWvW)
            .WithBuilds(GW2Builds.August2018Balance, GW2Builds.October2024Balance),
        new BuffOnActorDamageModifier(Mod_ForcefulPersistenceFacets, [FacetOfChaosBuff, FacetOfDarknessBuff, FacetOfElementsBuff, FacetOfNatureBuff, FacetOfStrengthBuff, FacetOfLightBuff], "Forceful Persistence (Facets)", "4% per active Facet", DamageSource.NoPets, 4.0, DamageType.Strike, DamageType.All, Source.Herald, ByMultiPresence, TraitImages.ForcefulPersistence, DamageModifierMode.sPvPWvW)
            .WithBuilds(GW2Builds.October2024Balance),
        new BuffOnActorDamageModifier(Mod_ForcefulPersistenceFacets, [FacetOfChaosBuff, FacetOfDarknessBuff, FacetOfElementsBuff, FacetOfNatureBuff, FacetOfStrengthBuff, FacetOfLightBuff], "Forceful Persistence (Facets)", "4% per active Facet", DamageSource.NoPets, 4.0, DamageType.Strike, DamageType.All, Source.Herald, ByMultiPresence, TraitImages.ForcefulPersistence, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.August2018Balance, GW2Builds.June2022Balance),
        new BuffOnActorDamageModifier(Mod_ForcefulPersistenceFacets, [FacetOfChaosBuff, FacetOfDarknessBuff, FacetOfElementsBuff, FacetOfNatureBuff, FacetOfStrengthBuff, FacetOfLightBuff], "Forceful Persistence (Facets)", "5% per active Facet", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Herald, ByMultiPresence, TraitImages.ForcefulPersistence, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.June2022Balance, GW2Builds.November2023Balance),
        new BuffOnActorDamageModifier(Mod_ForcefulPersistenceFacets, [FacetOfChaosBuff, FacetOfDarknessBuff, FacetOfElementsBuff, FacetOfNatureBuff, FacetOfStrengthBuff, FacetOfLightBuff], "Forceful Persistence (Facets)", "7% per active Facet", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Herald, ByMultiPresence, TraitImages.ForcefulPersistence, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.November2023Balance, GW2Builds.February2025BalancePatch),
        new BuffOnActorDamageModifier(Mod_ForcefulPersistenceFacets, [FacetOfChaosBuff, FacetOfDarknessBuff, FacetOfElementsBuff, FacetOfNatureBuff, FacetOfStrengthBuff, FacetOfLightBuff], "Forceful Persistence (Facets)", "10% per active Facet", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Herald, ByMultiPresence, TraitImages.ForcefulPersistence, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.February2025BalancePatch),
        //new BuffDamageModifier(new long[] { 27273, 27581, 28001}, "Forceful Persistence", "13% if active upkeep", DamageSource.NoPets, 13.0, DamageType.Power, DamageType.All, Source.Herald, ByPresence, BuffImages.ForcefulPersistence, GW2Builds.August2018Balance, DamageModifierMode.All), // Hammers, Embrace, Impossible Odds but how to track Protective Solace?
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers =
    [
        new BuffOnActorDamageModifier(Mod_FacetOfNatureDwarf, FacetOfNatureDwarf, "Facet of Nature - Dwarf", "-10%", DamageSource.NoPets, -10.0, DamageType.Strike, DamageType.All, Source.Common, ByPresence, SkillImages.FacetOfNatureDwarf, DamageModifierMode.All).WithBuilds(GW2Builds.August2018Balance),
        new BuffOnActorDamageModifier(Mod_HardeningPersistence, HardeningPersistence, "Hardening Persistence", "-1% per stack", DamageSource.NoPets, -1.0, DamageType.Strike, DamageType.All, Source.Herald, ByStack, TraitImages.HardeningPersistence, DamageModifierMode.All).WithBuilds(GW2Builds.August2018Balance, GW2Builds.July2019Balance),
        new BuffOnActorDamageModifier(Mod_HardeningPersistence, HardeningPersistence, "Hardening Persistence", "-1.5% per stack", DamageSource.NoPets, -1.5, DamageType.Strike, DamageType.All, Source.Herald, ByStack, TraitImages.HardeningPersistence, DamageModifierMode.All)
            .WithBuilds(GW2Builds.July2019Balance),
    ];

    internal static readonly IReadOnlyList<Buff> Buffs =
    [         
        // Skills
        new Buff("Crystal Hibernation", CrystalHibernation, Source.Herald, BuffClassification.Other, SkillImages.CrystalHibernation)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
        // Facets
        new Buff("Facet of Light", FacetOfLightBuff, Source.Herald, BuffClassification.Other, SkillImages.FacetOfLight),
        new Buff("Facet of Light (Traited)", FacetOfLightTraited, Source.Herald, BuffClassification.Other, SkillImages.FacetOfLight), //Lingering buff with Draconic Echo trait
        new Buff("Infuse Light", InfuseLight, Source.Herald, BuffClassification.Defensive, SkillImages.InfuseLight),
        new Buff("Facet of Darkness", FacetOfDarknessBuff, Source.Herald, BuffClassification.Other, SkillImages.FacetOfDarkness),
        new Buff("Facet of Darkness (Traited)", FacetOfDarknessBuffTraited, Source.Herald, BuffClassification.Other, SkillImages.FacetOfDarkness), //Lingering buff with Draconic Echo trait
        new Buff("Facet of Elements", FacetOfElementsBuff, Source.Herald, BuffClassification.Other, SkillImages.FacetOfElements),
        new Buff("Facet of Elements (Traited)", FacetOfElementsTraited, Source.Herald, BuffClassification.Other, SkillImages.FacetOfElements), //Lingering buff with Draconic Echo trait
        new Buff("Facet of Strength", FacetOfStrengthBuff, Source.Herald, BuffClassification.Other, SkillImages.FacetOfStrength),
        new Buff("Facet of Strength (Traited)", FacetOfStrengthTraited, Source.Herald, BuffClassification.Other, SkillImages.FacetOfStrength), //Lingering buff with Draconic Echo trait
        new Buff("Facet of Chaos", FacetOfChaosBuff, Source.Herald, BuffClassification.Other, SkillImages.FacetOfChaos),
        new Buff("Facet of Chaos (Traited)", FacetOfChaosTraited, Source.Herald, BuffClassification.Other, SkillImages.FacetOfChaos),
        new Buff("Facet of Nature", FacetOfNatureBuff, Source.Herald, BuffClassification.Other, SkillImages.FacetOfNature),
        new Buff("Facet of Nature (Traited)", FacetOfNatureTraited, Source.Herald, BuffClassification.Other, SkillImages.FacetOfNature), //Lingering buff with Draconic Echo trait
        new Buff("Facet of Nature-Assassin", FacetOfNatureAssassin, Source.Herald, BuffClassification.Offensive, SkillImages.FacetOfNatureAssassin),
        new Buff("Facet of Nature-Dragon", FacetOfNatureDragon, Source.Herald, BuffClassification.Support, SkillImages.FacetOfNatureDragon),
        new Buff("Facet of Nature-Demon", FacetOfNatureDemon, Source.Herald, BuffClassification.Support, SkillImages.FacetOfNatureDemon),
        new Buff("Facet of Nature-Dwarf", FacetOfNatureDwarf, Source.Herald, BuffClassification.Defensive, SkillImages.FacetOfNatureDwarf),
        new Buff("Facet of Nature-Centaur", FacetOfNatureCentaur, Source.Herald, BuffClassification.Defensive, SkillImages.FacetOfNatureCentaur),
        new Buff("Naturalistic Resonance", NaturalisticResonance, Source.Herald, BuffClassification.Defensive, SkillImages.FacetOfNature),
        new Buff("Legendary Dragon Stance", LegendaryDragonStanceBuff, Source.Herald, BuffClassification.Other, SkillImages.LegendaryDragonStance),
        new Buff("Hardening Persistence", HardeningPersistence, Source.Herald, BuffStackType.Stacking, 10, BuffClassification.Other, TraitImages.HardeningPersistence),
        new Buff("Soothing Bastion", SoothingBastion, Source.Herald, BuffClassification.Other, TraitImages.SoothingBastion),
        new Buff("Burst of Strength", BurstOfStrength, Source.Herald, BuffClassification.Other, SkillImages.BurstOfStrength),
        new Buff("Rising Momentum", RisingMomentum, Source.Herald, BuffStackType.Stacking, 10, BuffClassification.Other, TraitImages.RisingMomentum),
    ];
}
