using System.Numerics;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.DamageModifierIDs;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.EIData.ProfHelper;
using static GW2EIEvtcParser.EIData.SkillModeDescriptor;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EIData;

internal static class RevenantHelper
{
    internal static readonly List<InstantCastFinder> InstantCastFinder =
    [
        new BuffGainCastFinder(LegendaryAssassinStanceSkill, LegendaryAssassinStanceBuff),
        new BuffGainCastFinder(LegendaryDemonStanceSkill, LegendaryDemonStanceBuff),
        new BuffGainCastFinder(LegendaryDwarfStanceSkill, LegendaryDwarfStanceBuff),
        new BuffGainCastFinder(LegendaryCentaurStanceSkill, LegendaryCentaurStanceBuff),
        new BuffGainCastFinder(ImpossibleOddsSkill, ImpossibleOddsBuff)
            .UsingICD(500),
        new BuffLossCastFinder(RelinquishPower, ImpossibleOddsBuff)
            .UsingICD(500),
        new BuffGainCastFinder(VengefulHammersSkill, VengefulHammersBuff),
        new BuffLossCastFinder(ReleaseHammers, VengefulHammersBuff),
        new BuffLossCastFinder(ResistTheDarkness, EmbraceTheDarkness),
        new DamageCastFinder(InvokingTorment, InvokingTorment)
            .WithBuilds(GW2Builds.February2020Balance)
            .UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Unconditional),
        new DamageCastFinder(CallOfTheAssassin, CallOfTheAssassin),
        new DamageCastFinder(CallOfTheDwarf, CallOfTheDwarf),
        new DamageCastFinder(CallOfTheDemon, CallOfTheDemon),
        new DamageCastFinder(LesserBanishEnchantment, LesserBanishEnchantment)
            .WithBuilds(GW2Builds.December2018Balance, GW2Builds.February2020Balance)
            .UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
        new EXTHealingCastFinder(CallOfTheCentaur, CallOfTheCentaur),
        new EffectCastFinder(ProjectTranquility, EffectGUIDs.RevenantTabletAutoHeal)
            .UsingChecker((evt, combatData, agentData, skillData) => evt.Src.IsSpecies(MinionID.VentariTablet)),
        new EffectCastFinderByDst(VentarisWill, EffectGUIDs.RevenantTabletVentarisWill)
            .WithMinions()
            .UsingChecker((evt, combatData, agentData, skillData) => evt.Dst.IsSpecies(MinionID.VentariTablet)),
        new EffectCastFinderByDst(NaturalHarmony, EffectGUIDs.RevenantNaturalHarmony)
            .WithMinions()
            .UsingChecker((evt, combatData, agentData, skillData) => evt.Dst.IsSpecies(MinionID.VentariTablet))
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.June2022Balance),
        new EffectCastFinder(NaturalHarmony, EffectGUIDs.RevenantNaturalHarmony)
            .WithMinions()
            .UsingChecker((evt, combatData, agentData, skillData) => evt.Src.IsSpecies(MinionID.VentariTablet))
            .WithBuilds( GW2Builds.June2022Balance),
        new EffectCastFinder(PurifyingEssence, EffectGUIDs.RevenantPurifyingEssence)
            .WithMinions()
            .UsingChecker((evt, combatData, agentData, skillData) => evt.Src.IsSpecies(MinionID.VentariTablet))
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.June2022Balance),
        new EffectCastFinder(PurifyingEssence, EffectGUIDs.RevenantPurifyingEssence)
            .UsingSrcBaseSpecChecker(Spec.Revenant)
            .WithBuilds(GW2Builds.June2022Balance),
        new EffectCastFinder(EnergyExpulsion, EffectGUIDs.RevenantEnergyExpulsion)
            .WithMinions()
            .UsingChecker((evt, combatData, agentData, skillData) => evt.Src.IsSpecies(MinionID.VentariTablet))
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.June2022Balance),
        new EffectCastFinder(EnergyExpulsion, EffectGUIDs.RevenantEnergyExpulsion)
            .UsingSrcBaseSpecChecker(Spec.Revenant)
            .WithBuilds(GW2Builds.June2022Balance),
        new EffectCastFinder(ProtectiveSolaceSkill, EffectGUIDs.RevenantProtectiveSolace)
            .UsingSrcBaseSpecChecker(Spec.Revenant)
            .UsingChecker((evt, combatData, agentData, skillData) => evt.IsAroundDst && evt.Dst.IsSpecies(MinionID.VentariTablet)),
        new EffectCastFinder(BlitzMinesDrop, EffectGUIDs.RevenantSpearBlitzMines1)
            .UsingSrcBaseSpecChecker(Spec.Revenant),
        new EffectCastFinder(BlitzMines, EffectGUIDs.RevenantSpearBlitzMinesDetonation1)
            .UsingSecondaryEffectChecker(EffectGUIDs.RevenantSpearBlitzMinesDetonation2)
            .UsingSrcBaseSpecChecker(Spec.Revenant),
    ];


    internal static readonly IReadOnlyList<DamageModifierDescriptor> OutgoingDamageModifiers =
    [
        // Retribution
        // - Dwarven Battle Training
        new BuffOnFoeDamageModifier(Mod_DwarvenBattleTraining, Weakness, "Dwarven Battle Training", "10% on weakened target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Revenant, ByPresence, TraitImages.DwarvenBattleTraining, DamageModifierMode.All)
            .WithBuilds(GW2Builds.December2018Balance),
        // - Vicious Reprisal
        new BuffOnActorDamageModifier(Mod_ViciousReprisal, Retaliation, "Vicious Reprisal", "10% under retaliation", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Revenant, ByPresence, TraitImages.ViciousReprisal, DamageModifierMode.All)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2021Balance),
        new BuffOnActorDamageModifier(Mod_ViciousReprisal, Resolution, "Vicious Reprisal", "10% under resolution", DamageSource.NoPets, 10.0, DamageType.StrikeAndCondition, DamageType.All, Source.Revenant, ByPresence, TraitImages.ViciousReprisal, DamageModifierMode.All)
            .WithBuilds(GW2Builds.May2021Balance),
        
        // Invocation
        // - Ferocious Aggression
        new BuffOnActorDamageModifier(Mod_FerociousAggression, Fury, "Ferocious Aggression", "7% under fury", DamageSource.NoPets, 7.0, DamageType.StrikeAndCondition, DamageType.All, Source.Revenant, ByPresence, TraitImages.FerociousAggression, DamageModifierMode.All)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2021Balance),
        new BuffOnActorDamageModifier(Mod_FerociousAggression, Fury, "Ferocious Aggression", "7% under fury", DamageSource.NoPets, 7.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.Revenant, ByPresence, TraitImages.FerociousAggression, DamageModifierMode.All)
            .WithBuilds(GW2Builds.May2021Balance, GW2Builds.August2022Balance),
        new BuffOnActorDamageModifier(Mod_FerociousAggression, Fury, "Ferocious Aggression", "7% under fury", DamageSource.NoPets, 7.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.Revenant, ByPresence, TraitImages.FerociousAggression, DamageModifierMode.sPvPWvW)
            .WithBuilds(GW2Builds.August2022Balance),
        new BuffOnActorDamageModifier(Mod_FerociousAggression, Fury, "Ferocious Aggression", "10% under fury", DamageSource.NoPets, 10.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.Revenant, ByPresence, TraitImages.FerociousAggression, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.August2022Balance),
        // - Rising Tide
        new DamageLogDamageModifier(Mod_RisingTide, "Rising Tide", "7% if hp >=90%", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Revenant, TraitImages.RisingTide, (x, log) => x.IsOverNinety, DamageModifierMode.All)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.August2022Balance),
        new DamageLogDamageModifier(Mod_RisingTide, "Rising Tide", "7% if hp >=90%", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Revenant, TraitImages.RisingTide, (x, log) => x.IsOverNinety, DamageModifierMode.sPvPWvW)
            .WithBuilds(GW2Builds.August2022Balance),
        new DamageLogDamageModifier(Mod_RisingTide, "Rising Tide", "10% if hp >=90%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Revenant, TraitImages.RisingTide, (x, log) => x.IsOverNinety, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.August2022Balance, GW2Builds.November2022Balance),
        new DamageLogDamageModifier(Mod_RisingTide, "Rising Tide", "10% if hp >=75%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Revenant, TraitImages.RisingTide, (x, log) => x.From.GetCurrentHealthPercent(log, x.Time) >= 75.0, DamageModifierMode.PvE)
            .UsingApproximate()
            .WithBuilds(GW2Builds.November2022Balance),
        
        // Devastation
        // - Vicious Lacerations
        new BuffOnActorDamageModifier(Mod_ViciousLacerations, ViciousLacerations, "Vicious Lacerations", "3% per Stack", DamageSource.NoPets, 3.0, DamageType.Strike, DamageType.All, Source.Revenant, ByStack, TraitImages.ViciousLacerations, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.October2018Balance, GW2Builds.February2020Balance),
        new BuffOnActorDamageModifier(Mod_ViciousLacerations, ViciousLacerations, "Vicious Lacerations", "2% per Stack", DamageSource.NoPets, 2.0, DamageType.Strike, DamageType.All, Source.Revenant, ByStack, TraitImages.ViciousLacerations, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.October2018Balance),
        // - Unsuspecting Strikes
        new DamageLogDamageModifier(Mod_UnsuspectingStrikes, "Unsuspecting Strikes", "25% if target hp > 80%", DamageSource.NoPets, 25.0, DamageType.Strike, DamageType.All, Source.Revenant, TraitImages.ViciousLacerations, (x,log) => x.To.GetCurrentHealthPercent(log, x.Time) > 80, DamageModifierMode.PvE )
            .UsingApproximate()
            .WithBuilds(GW2Builds.February2020Balance, GW2Builds.May2021BalanceHotFix),
        new DamageLogDamageModifier(Mod_UnsuspectingStrikes, "Unsuspecting Strikes", "20% if target hp > 80%", DamageSource.NoPets, 20.0, DamageType.Strike, DamageType.All, Source.Revenant, TraitImages.ViciousLacerations, (x,log) => x.To.GetCurrentHealthPercent(log, x.Time) > 80, DamageModifierMode.PvE )
            .UsingApproximate()
            .WithBuilds(GW2Builds.May2021BalanceHotFix),
        new DamageLogDamageModifier(Mod_UnsuspectingStrikes, "Unsuspecting Strikes", "10% if target hp > 80%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Revenant, TraitImages.ViciousLacerations, (x,log) => x.To.GetCurrentHealthPercent(log, x.Time) > 80, DamageModifierMode.sPvPWvW )
            .UsingApproximate()
            .WithBuilds(GW2Builds.February2020Balance),
        // - Targeted Destruction
        new BuffOnFoeDamageModifier(Mod_TargetedDestruction, Vulnerability, "Targeted Destruction", "0.5% per stack vuln", DamageSource.NoPets, 0.5, DamageType.Strike, DamageType.All, Source.Revenant, ByStack, TraitImages.TargetedDestruction, DamageModifierMode.All)
            .WithBuilds(GW2Builds.March2019Balance),
        new BuffOnFoeDamageModifier(Mod_TargetedDestruction, Vulnerability, "Targeted Destruction", "10.0% if vuln", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Revenant, ByPresence, TraitImages.TargetedDestruction, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.October2018Balance, GW2Builds.March2019Balance),
        new BuffOnFoeDamageModifier(Mod_TargetedDestruction, Vulnerability, "Targeted Destruction", "7.0% if vuln", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Revenant, ByPresence, TraitImages.TargetedDestruction, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.October2018Balance),
        // - Swift Termination
        new DamageLogDamageModifier(Mod_SwiftTermination, "Swift Termination", "20% if target <50%", DamageSource.NoPets, 20.0, DamageType.Strike, DamageType.All, Source.Revenant, TraitImages.SwiftTermination, (x, log) => x.AgainstUnderFifty, DamageModifierMode.All),
        // Brutality
        new BuffOnFoeDamageModifier(Mod_Brutality, [Stability, Protection], "Brutality", "15% against protection and stability", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Revenant, ByPresence, TraitImages.Brutality, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.June2025Balance),
        new BuffOnFoeDamageModifier(Mod_Brutality, [Stability, Protection], "Brutality", "10% against protection and stability", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Revenant, ByPresence, TraitImages.Brutality, DamageModifierMode.sPvPWvW)
            .WithBuilds(GW2Builds.June2025Balance),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers =
    [
        // Legendary Dwarf
        // - Rite of the Great Dwarf
        new BuffOnActorDamageModifier(Mod_RiteOfTheGreatDwarfCondition, RiteOfTheGreatDwarf, "Rite of the Great Dwarf (condition)", "-50%", DamageSource.Incoming, -50.0, DamageType.Condition, DamageType.All, Source.Common, ByPresence, SkillImages.RiteOfTheGreatDwarf, DamageModifierMode.All),
        new BuffOnActorDamageModifier(Mod_RiteOfTheGreatDwarfStrike, RiteOfTheGreatDwarf, "Rite of the Great Dwarf (strike)", "-50%", DamageSource.Incoming, -50.0, DamageType.Strike, DamageType.All, Source.Common, ByPresence, SkillImages.RiteOfTheGreatDwarf, DamageModifierMode.All),
        new BuffOnActorDamageModifier(Mod_RiteOfTheGreatDwarfEcho, RiteOfTheGreatDwarfAncientEcho, "Rite of the Great Dwarf (Ancient Echo)", "-50%", DamageSource.Incoming, -50.0, DamageType.Strike, DamageType.All, Source.Common, ByPresence, SkillImages.RiteOfTheGreatDwarf, DamageModifierMode.All),
        // - Vengeful Hammers
        new BuffOnActorDamageModifier(Mod_VengefulHammers, VengefulHammersBuff, "Vengeful Hammers", "-20%", DamageSource.Incoming, -20.0, DamageType.StrikeAndCondition, DamageType.All, Source.Revenant, ByPresence, SkillImages.RiteOfTheGreatDwarf, DamageModifierMode.All),
        
        // Corruption
        // - Demonic Resistance
        new BuffOnActorDamageModifier(Mod_DemonicResistance, Resistance, "Demonic Resistance", "-20%", DamageSource.Incoming, -20.0, DamageType.Strike, DamageType.All, Source.Revenant, ByPresence, TraitImages.DemonicResistance, DamageModifierMode.All)
            .WithBuilds(GW2Builds.August2018Balance, GW2Builds.February2020Balance),
        new BuffOnActorDamageModifier(Mod_DemonicResistance, Resistance, "Demonic Resistance", "-20%", DamageSource.Incoming, -20.0, DamageType.Strike, DamageType.All, Source.Revenant, ByPresence, TraitImages.DemonicResistance, DamageModifierMode.sPvPWvW)
            .WithBuilds(GW2Builds.February2020Balance, GW2Builds.July2020Balance),
        new BuffOnActorDamageModifier(Mod_DemonicResistance, Resistance, "Demonic Resistance", "-10%", DamageSource.Incoming, -10.0, DamageType.Strike, DamageType.All, Source.Revenant, ByPresence, TraitImages.DemonicResistance, DamageModifierMode.sPvPWvW)
            .WithBuilds(GW2Builds.July2020Balance),
        new BuffOnActorDamageModifier(Mod_DemonicResistance, Resistance, "Demonic Resistance", "-33%", DamageSource.Incoming, -33.0, DamageType.Strike, DamageType.All, Source.Revenant, ByPresence, TraitImages.DemonicResistance, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.February2020Balance, GW2Builds.May2021Balance),
        new BuffOnActorDamageModifier(Mod_DemonicResistance, Resistance, "Demonic Resistance", "-20%", DamageSource.Incoming, -20.0, DamageType.Strike, DamageType.All, Source.Revenant, ByPresence, TraitImages.DemonicResistance, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.May2021Balance),

        // Retribution
        // - Close Quarters
        new DamageLogDamageModifier(Mod_CloseQuarters, "Close Quarters", "-10% from foes beyond 360 range", DamageSource.Incoming, -10.0, DamageType.Strike, DamageType.All, Source.Revenant, TraitImages.CloseQuarters, (x, log) => !TargetWithinRangeChecker(x, log, 360, false), DamageModifierMode.All)
            .UsingApproximate(),
        // - Determined Resolution
        new BuffOnActorDamageModifier(Mod_DeterminedResolution, Stability, "Determined Resolution", "-15% under stability", DamageSource.Incoming, -15.0, DamageType.Strike, DamageType.All, Source.Revenant, ByPresence, TraitImages.DeterminedResolution, DamageModifierMode.All)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.February2020Balance),
        new BuffOnActorDamageModifier(Mod_DeterminedResolution, Vigor, "Determined Resolution", "-15 under vigor", DamageSource.Incoming, -15.0, DamageType.Strike, DamageType.All, Source.Revenant, ByPresence, TraitImages.DeterminedResolution, DamageModifierMode.All)
            .WithBuilds(GW2Builds.February2020Balance, GW2Builds.May2021Balance),
        new BuffOnActorDamageModifier(Mod_DeterminedResolution, Resolution, "Determined Resolution", "-15% under resolution", DamageSource.Incoming, -15.0, DamageType.Strike, DamageType.All, Source.Revenant, ByPresence, TraitImages.DeterminedResolution, DamageModifierMode.All)
            .WithBuilds(GW2Builds.May2021Balance, GW2Builds.May2021BalanceHotFix),
        new BuffOnActorDamageModifier(Mod_DeterminedResolution, Resolution, "Determined Resolution", "-10% under resolution", DamageSource.Incoming, -10.0, DamageType.Strike, DamageType.All, Source.Revenant, ByPresence, TraitImages.DeterminedResolution, DamageModifierMode.All)
            .WithBuilds(GW2Builds.May2021BalanceHotFix, GW2Builds.May2025BalanceHotFix),
        new BuffOnActorDamageModifier(Mod_DeterminedResolution, Resolution, "Determined Resolution", "-7% under resolution", DamageSource.Incoming, -7.0, DamageType.Strike, DamageType.All, Source.Revenant, ByPresence, TraitImages.DeterminedResolution, DamageModifierMode.sPvP)
            .WithBuilds(GW2Builds.May2025BalanceHotFix),
        new BuffOnActorDamageModifier(Mod_DeterminedResolution, Resolution, "Determined Resolution", "-10% under resolution", DamageSource.Incoming, -10.0, DamageType.Strike, DamageType.All, Source.Revenant, ByPresence, TraitImages.DeterminedResolution, DamageModifierMode.PvEWvW)
            .WithBuilds(GW2Builds.May2025BalanceHotFix),
        // Resolute Evasion
        new BuffOnActorDamageModifier(Mod_ResoluteEvasion, ResoluteEvasion, "Resolute Evasion", "-10%", DamageSource.Incoming, -10.0, DamageType.Strike, DamageType.All, Source.Revenant, ByPresence, TraitImages.ResoluteEvasion, DamageModifierMode.All)
            .WithBuilds(GW2Builds.October2024Balance, GW2Builds.May2025BalanceHotFix),
        new BuffOnActorDamageModifier(Mod_ResoluteEvasion, ResoluteEvasion, "Resolute Evasion", "-10%", DamageSource.Incoming, -10.0, DamageType.Strike, DamageType.All, Source.Revenant, ByPresence, TraitImages.ResoluteEvasion, DamageModifierMode.PvEWvW)
            .WithBuilds(GW2Builds.May2025BalanceHotFix),
        new BuffOnActorDamageModifier(Mod_ResoluteEvasion, ResoluteEvasion, "Resolute Evasion", "-5%", DamageSource.Incoming, -5.0, DamageType.Strike, DamageType.All, Source.Revenant, ByPresence, TraitImages.ResoluteEvasion, DamageModifierMode.sPvP)
            .WithBuilds(GW2Builds.May2025BalanceHotFix),
        // Salvation
        // - Unyielding Devotion (Spirit Buff)
        new BuffOnActorDamageModifier(Mod_UnyieldingSpirit, UnyieldingSpirit, "Unyielding Spirit", "-15%", DamageSource.Incoming, -15.0, DamageType.Strike, DamageType.All, Source.Revenant, ByPresence, TraitImages.UnyieldingDevotion, DamageModifierMode.All)
            .WithBuilds(GW2Builds.April2019Balance, GW2Builds.July2022FractalInstabilitiesRework),
        new BuffOnActorDamageModifier(Mod_UnyieldingSpirit, UnyieldingSpirit, "Unyielding Spirit", "-15%", DamageSource.Incoming, -15.0, DamageType.Strike, DamageType.All, Source.Revenant, ByPresence, TraitImages.UnyieldingDevotion, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.July2022FractalInstabilitiesRework),
        new BuffOnActorDamageModifier(Mod_UnyieldingSpirit, UnyieldingSpirit, "Unyielding Spirit", "-10%", DamageSource.Incoming, -10.0, DamageType.Strike, DamageType.All, Source.Revenant, ByPresence, TraitImages.UnyieldingDevotion, DamageModifierMode.sPvPWvW)
            .WithBuilds(GW2Builds.July2022FractalInstabilitiesRework),
    ];

    internal static readonly IReadOnlyList<Buff> Buffs =
    [
        new Buff("Vengeful Hammers", VengefulHammersBuff, Source.Revenant, BuffClassification.Other, SkillImages.VengefulHammers),
        new Buff("Rite of the Great Dwarf", RiteOfTheGreatDwarf, Source.Revenant, BuffClassification.Defensive, SkillImages.RiteOfTheGreatDwarf),
        new Buff("Rite of the Great Dwarf (Ancient Echo)", RiteOfTheGreatDwarfAncientEcho, Source.Revenant, BuffClassification.Defensive, SkillImages.RiteOfTheGreatDwarf),
        new Buff("Embrace the Darkness", EmbraceTheDarkness, Source.Revenant, BuffClassification.Other, SkillImages.EmbraceTheDarkness),
        new Buff("Enchanted Daggers", EnchantedDaggers, Source.Revenant, BuffStackType.Stacking, 25, BuffClassification.Other, SkillImages.EnchantedDaggers),
        new Buff("Phase Traversal", PhaseTraversal, Source.Revenant, BuffStackType.Stacking, 25, BuffClassification.Other, SkillImages.PhaseTraversal),
        new Buff("Tranquil", Tranquil, Source.Revenant, BuffClassification.Other, SkillImages.ProjectTranquility),
        new Buff("Impossible Odds", ImpossibleOddsBuff, Source.Revenant, BuffClassification.Other, SkillImages.ImpossibleOdds),
        new Buff("Jade", Jade, Source.Revenant, BuffClassification.Other, BuffImages.Stun),
        new Buff("Legendary Centaur Stance", LegendaryCentaurStanceBuff, Source.Revenant, BuffClassification.Other, SkillImages.LegendaryCentaurStance),
        new Buff("Legendary Dwarf Stance", LegendaryDwarfStanceBuff, Source.Revenant, BuffClassification.Other, SkillImages.LegendaryDwarfStance),
        new Buff("Legendary Demon Stance", LegendaryDemonStanceBuff, Source.Revenant, BuffClassification.Other, SkillImages.LegendaryDemonStance),
        new Buff("Legendary Assassin Stance", LegendaryAssassinStanceBuff, Source.Revenant, BuffClassification.Other, SkillImages.LegendaryAssassinStance),
        new Buff("Crystal Hibernation", CrystalHibernation, Source.Revenant, BuffClassification.Other, SkillImages.CrystalHibernation)
            .WithBuilds(GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
        new Buff("Imperial Guard", ImperialGuard, Source.Revenant, BuffStackType.Stacking, 5, BuffClassification.Other, SkillImages.ImperialGuard)
            .WithBuilds(GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
        // Traits
        new Buff("Vicious Lacerations", ViciousLacerations, Source.Revenant, BuffStackType.Stacking, 3, BuffClassification.Other, TraitImages.ViciousLacerations)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.February2020Balance),
        new Buff("Assassin's Presence", AssassinsPresence, Source.Revenant, BuffClassification.Offensive, TraitImages.AssassinsPresence)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.June2022Balance),
        new Buff("Expose Defenses", ExposeDefenses, Source.Revenant, BuffClassification.Other, TraitImages.MutilateDefenses),
        new Buff("Invoking Harmony", InvokingHarmony, Source.Revenant, BuffClassification.Other, TraitImages.InvokingHarmony),
        new Buff("Unyielding Spirit", UnyieldingSpirit, Source.Revenant, BuffClassification.Other, TraitImages.UnyieldingDevotion)
            .WithBuilds(GW2Builds.April2019Balance),
        new Buff("Selfless Amplification", SelflessAmplification, Source.Revenant, BuffClassification.Other, TraitImages.SelflessAmplification),
        new Buff("Battle Scars", BattleScars, Source.Revenant, BuffStackType.StackingConditionalLoss, 25, BuffClassification.Other, TraitImages.ThrillOfCombat)
            .WithBuilds(GW2Builds.February2020Balance),
        new Buff("Steadfast Rejuvenation", SteadfastRejuvenation, Source.Revenant, BuffStackType.Stacking, 10, BuffClassification.Other, TraitImages.SteadfastRejuvenation),
        new Buff("Resolute Evasion", ResoluteEvasion, Source.Revenant, BuffClassification.Other, TraitImages.ResoluteEvasion),
        // Scepter
        new Buff("Blossoming Aura", BlossomingAuraBuff, Source.Revenant, BuffStackType.Stacking, 25, BuffClassification.Other, SkillImages.BlossomingAura),
        // Spear
        new Buff("Crushing Abyss", CrushingAbyss, Source.Revenant, BuffStackType.Stacking, 5, BuffClassification.Other, SkillImages.CrushingAbyss),
    ];

    private static readonly HashSet<long> _legendSwaps =
    [
        LegendaryAssassinStanceSkill,
        LegendaryDemonStanceSkill,
        LegendaryDwarfStanceSkill,
        LegendaryCentaurStanceSkill,
    ];

    public static bool IsLegendSwap(long id)
    {
        return _legendSwaps.Contains(id);
    }

    private static readonly HashSet<int> Minions =
    [
        (int)MinionID.VentariTablet
    ];
    internal static bool IsKnownMinionID(int id)
    {
        return Minions.Contains(id);
    }

    public static void ProcessGadgets(IReadOnlyList<AgentItem> players, CombatData combatData, AgentData agentData)
    {
        var allTablets = new HashSet<AgentItem>();
        if (combatData.TryGetEffectEventsByGUID(EffectGUIDs.RevenantTabletAutoHeal, out var tabletHealEffectEvents))
        {
            allTablets.UnionWith(tabletHealEffectEvents.Select(x => x.Src));
        }
        if (combatData.TryGetEffectEventsByGUID(EffectGUIDs.RevenantTabletVentarisWill, out var ventarisWillEffectEvents))
        {
            allTablets.UnionWith(ventarisWillEffectEvents.Where(x => x.IsAroundDst).Select(x => x.Dst));
        }
        if (combatData.TryGetEffectEventsByGUID(EffectGUIDs.RevenantProtectiveSolace, out var protectiveSolaceEffectEvents))
        {
            allTablets.UnionWith(protectiveSolaceEffectEvents.Where(x => x.IsAroundDst).Select(x => x.Dst));
        }
        foreach (AgentItem tablet in allTablets)
        {
            tablet.OverrideType(AgentItem.AgentType.NPC, agentData);
            tablet.OverrideID(MinionID.VentariTablet, agentData);
            tablet.OverrideName("Ventari's Tablet");
        }
    }

    internal static void ComputeProfessionCombatReplayActors(PlayerActor player, ParsedEvtcLog log, CombatReplay replay)
    {
        Color color = Colors.Revenant;

        // Inspiring Reinforcement
        var inspiringReinforcementSkill = new SkillModeDescriptor(player, Spec.Revenant, InspiringReinforcement, SkillModeCategory.ImportantBuffs);
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.RevenantInspiringReinforcementPart, out var inspiringReinforcementParts))
        {

            foreach (EffectEvent effect in inspiringReinforcementParts)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 5000);
                var connector = new PositionConnector(effect.Position);
                var rotationConnector = new AngleConnector(effect.Rotation.Z);
                replay.Decorations.Add(new RectangleDecoration(240, 360, lifespan, Colors.DarkTeal.WithAlpha(0.1f).ToString(), connector).UsingRotationConnector(rotationConnector).UsingSkillMode(inspiringReinforcementSkill));
            }
        }
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.RevenantInspiringReinforcement, out var inspiringReinforcements))
        {
            foreach (EffectEvent effect in inspiringReinforcements)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 5000);
                var connector = (PositionConnector)new PositionConnector(effect.Position).WithOffset(new(0, -420.0f, 0), true); // 900 units in front, 60 behind
                var rotationConnector = new AngleConnector(effect.Rotation.Z);
                replay.Decorations.Add(new RectangleDecoration(240, 960, lifespan, color, 0.5, connector)
                    .UsingFilled(false)
                    .UsingRotationConnector(rotationConnector)
                    .UsingSkillMode(inspiringReinforcementSkill));
                replay.Decorations.Add(new IconDecoration(EffectImages.EffectInspiringReinforcement, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector)
                    .UsingRotationConnector(rotationConnector)
                    .UsingSkillMode(inspiringReinforcementSkill));
            }
        }

        // Protective Solace
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.RevenantProtectiveSolace, out var protectiveSolaceEffectEvents))
        {
            var skill = new SkillModeDescriptor(player, Spec.Revenant, ProtectiveSolaceSkill, SkillModeCategory.ProjectileManagement);
            foreach (EffectEvent effect in protectiveSolaceEffectEvents.Where(x => x.IsAroundDst))
            {
                (long, long) lifespan = effect.ComputeDynamicLifespan(log, 0, effect.Dst, ProtectiveSolaceTabletBuff); // manually disabled or when no more resources
                var connector = new AgentConnector(effect.Dst);
                replay.Decorations.Add(new CircleDecoration(240, lifespan, color, 0.2, connector)
                    .UsingSkillMode(skill));
                replay.Decorations.Add(new IconDecoration(EffectImages.EffectProtectiveSolace, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector)
                    .UsingSkillMode(skill));
            }
        }

        // Eternity's Requiem
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.RevenantEternitysRequiemOnPlayer, out var eternitysRequiem))
        {
            var skill = new SkillModeDescriptor(player, Spec.Revenant, EternitysRequiem);
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.RevenantEternitysRequiemHit, out var eternitysRequiemHits))
            {
                foreach (EffectEvent effect in eternitysRequiem)
                {
                    var positions = new List<Vector3>();
                    foreach (EffectEvent hitEffect in eternitysRequiemHits.Where(x => x.Time >= effect.Time && x.Time <= effect.Time + 2800 && !x.IsAroundDst))
                    {
                        positions.Add(hitEffect.Position);
                        (long, long) lifespanHit = hitEffect.ComputeLifespan(log, 1000);
                        var connector = new PositionConnector(hitEffect.Position);
                        replay.Decorations.Add(new CircleDecoration(120, lifespanHit, color, 0.5, connector).UsingFilled(false).UsingSkillMode(skill));
                    }
                    if (positions.Count > 0)
                    {
                        (long, long) lifespan = (effect.Time, effect.Time + 2800);
                        var centralConnector = new PositionConnector(positions.Average());
                        replay.Decorations.Add(new IconDecoration(EffectImages.EffectEternitysRequiem, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, centralConnector).UsingSkillMode(skill));
                        // TODO: Find a way to tell the user that the circle is approximative.
                        //replay.Decorations.Add(new CircleDecoration(360, lifespan, color, 0.5, centralConnector).UsingFilled(false).UsingSkillMode(skill));
                    }
                }
            }
        }

        // Coalescence of Ruin
        if (log.CombatData.TryGetEffectEventsBySrcWithGUIDs(player.AgentItem,
            [EffectGUIDs.RevenantCoalescenceOfRuin, EffectGUIDs.RevenantCoalescenceOfRuinLast], out var coalescenceOfRuin))
        {
            var skill = new SkillModeDescriptor(player, Spec.Revenant, CoalescenceOfRuin);
            foreach (EffectEvent effect in coalescenceOfRuin)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 2000);
                var connector = new PositionConnector(effect.Position);
                var rotation = new AngleConnector(effect.Rotation.Z);
                replay.Decorations.Add(new RectangleDecoration(180, 400, lifespan, color, 0.5, connector).UsingFilled(false).UsingRotationConnector(rotation).UsingSkillMode(skill));
                replay.Decorations.Add(new IconDecoration(EffectImages.EffectCoalescenceOfRuin, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector).UsingSkillMode(skill));
            }
        }

        // Drop the Hammer
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.RevenantDropTheHammer, out var dropTheHammer))
        {
            var skill = new SkillModeDescriptor(player, Spec.Revenant, DropTheHammer, SkillModeCategory.CC);
            foreach (EffectEvent effect in dropTheHammer)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 1220);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, EffectImages.EffectDropTheHammer);
            }
        }

        // Abyssal Blitz (Mines)
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.RevenantSpearBlitzMines2, out var abyssalBlitzMines))
        {
            var skill = new SkillModeDescriptor(player, Spec.Revenant, BlitzMines);
            foreach (EffectEvent effect in abyssalBlitzMines)
            {
                (long, long) lifespan = effect.ComputeDynamicLifespan(log, 7000);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 120, EffectImages.EffectAbyssalBlitzMines);
            }
        }

        // Abyssal Blot
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.RevenantSpearAbyssalBlot, out var abyssalBlots))
        {
            var skillCC = new SkillModeDescriptor(player, Spec.Revenant, AbyssalBlot, SkillModeCategory.CC);
            var skillDamage= new SkillModeDescriptor(player, Spec.Revenant, AbyssalBlot);
            foreach (EffectEvent effect in abyssalBlots)
            {
                (long start, long end) lifespan = effect.ComputeLifespan(log, 3000);
                (long start, long end) lifespanCC = (lifespan.start, lifespan.start + 500);
                (long start, long end) lifespanDamage = (lifespanCC.end, lifespan.end);
                // CC on first pulse
                AddCircleSkillDecoration(replay, effect, color, skillCC, lifespanCC, 240, EffectImages.EffectAbyssalBlot);
                AddCircleSkillDecoration(replay, effect, color, skillDamage, lifespanDamage, 240, EffectImages.EffectAbyssalBlot);
            }
        }

        // Abyssal Raze
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.RevenantSpearAbyssalRaze, out var abyssalRazes))
        {
            var skill = new SkillModeDescriptor(player, Spec.Revenant, AbyssalRaze);
            foreach (EffectEvent effect in abyssalRazes)
            {
                uint radius = 180;
                long warningDuration = 800; // Overriding logged effect duration of 5100
                var connector = new PositionConnector(effect.Position);
                (long start, long end) lifespanWarning = (effect.Time, effect.Time + warningDuration);
                var circle = (CircleDecoration)new CircleDecoration(radius, lifespanWarning, color, 0.5, connector).UsingFilled(false).UsingSkillMode(skill);
                replay.Decorations.AddWithGrowing(circle, lifespanWarning.end);
                replay.Decorations.Add(new IconDecoration(EffectImages.EffectAbyssalRaze, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespanWarning, connector).UsingSkillMode(skill));
                // Hit indicator
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.RevenantSpearAbyssalRazeHit, out var abyssalRazeHits))
                {
                    var hit = abyssalRazeHits.FirstOrDefault(x => x.Time > effect.Time && x.Time < effect.Time + 1000);
                    if (hit != null)
                    {
                        (long start, long end) lifespanHit = (lifespanWarning.end, lifespanWarning.end + 500);
                        replay.Decorations.Add(new CircleDecoration(radius, lifespanHit, color, 0.5, connector).UsingSkillMode(skill));
                        replay.Decorations.Add(new IconDecoration(EffectImages.EffectAbyssalRaze, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespanHit, connector).UsingSkillMode(skill));
                    }
                }
            }
        }
    }
}
