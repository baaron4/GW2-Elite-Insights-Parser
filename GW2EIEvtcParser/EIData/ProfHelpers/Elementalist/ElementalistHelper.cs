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

internal static class ElementalistHelper
{

    internal static readonly List<InstantCastFinder> InstantCastFinder =
    [
        new BuffGainCastFinder(FireAttunementSkill, FireAttunementBuff),
        new BuffGainCastFinder(WaterAttunementSkill, WaterAttunementBuff),
        new BuffGainCastFinder(AirAttunementSkill, AirAttunementBuff),
        new BuffGainCastFinder(EarthAttunementSkill, EarthAttunementBuff),

        new BuffGainCastFinder(GlyphOfElementalPowerFireSkill, GlyphOfElementalPowerFireBuff),
        new BuffGainCastFinder(GlyphOfElementalPowerWaterSkill, GlyphOfElementalPowerWaterBuff),
        new BuffGainCastFinder(GlyphOfElementalPowerAirSkill, GlyphOfElementalPowerAirBuff),
        new BuffGainCastFinder(GlyphOfElementalPowerEarthSkill, GlyphOfElementalPowerEarthBuff),
        new DamageCastFinder(ArcaneBlast, ArcaneBlast),
        new BuffGiveCastFinder(ArcanePowerSkill, ArcanePowerBuff),
        new BuffGainCastFinder(ArcaneShieldSkill, ArcaneShieldBuff),
        new DamageCastFinder(ArcaneWave, ArcaneWave),
        new BuffGainCastFinder(MistForm, MistForm),
        new DamageCastFinder(SignetOfAirSkill, SignetOfAirSkill)
            .UsingDisableWithEffectData(),
        new DamageCastFinder(Sunspot, Sunspot)
            .UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Unconditional),
        new DamageCastFinder(FlameExpulsion, FlameExpulsion)
            .UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Unconditional),
        new DamageCastFinder(EarthenBlast, EarthenBlast)
            .UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Unconditional),
        new EffectCastFinderByDst(SignetOfAirSkill, EffectGUIDs.ElementalistSignetOfAir)
            .UsingDstBaseSpecChecker(Spec.Elementalist),
        new DamageCastFinder(LightningRod, LightningRod)
            .UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
        new DamageCastFinder(LightningFlash, LightningFlash),
        new EffectCastFinderByDst(ArmorOfEarth, EffectGUIDs.ElementalistArmorOfEarth1)
            .UsingDstBaseSpecChecker(Spec.Elementalist),
        //new EffectCastFinderByDst(CleansingFire, EffectGUIDs.ElementalistCleansingFire).UsingChecker((evt, combatData, agentData, skillData) => evt.Dst.BaseSpec == Spec.Elementalist && evt.Src == evt.Dst),
        new EXTHealingCastFinder(HealingRipple, HealingRipple)
            .UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Unconditional),
        new EXTHealingCastFinder(HealingRippleWvW, HealingRippleWvW)
            .UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Unconditional),
        new EXTHealingCastFinder(FlowLikeWaterHealing, FlowLikeWaterHealing)
            .UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
        // Elementals
        new MinionCommandCastFinder(FireElementalFlameBarrage, (int) MinionID.FireElemental),
        new MinionCommandCastFinder(WaterElementalCrashingWaves, (int) MinionID.IceElemental),
        new MinionCommandCastFinder(AirElementalShockingBolt, (int) MinionID.AirElemental),
        new MinionCommandCastFinder(EarthElementalStomp, (int) MinionID.EarthElemental),
        // Hammer
        new BuffGainCastFinder(FlameWheelSkill, FlameWheelBuff)
            .UsingToNotSpecChecker(Spec.Weaver)
            .UsingChecker((ba, combatData, agentData, skillData) => !combatData.IsCasting(GrandFinale, ba.To, ba.Time))
            .WithBuilds(GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
        new BuffGainCastFinder(IcyCoilSkill, IcyCoilBuff)
            .UsingToNotSpecChecker(Spec.Weaver)
            .UsingChecker((ba, combatData, agentData, skillData) => !combatData.IsCasting(GrandFinale, ba.To, ba.Time))
            .WithBuilds(GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
        new BuffGainCastFinder(CrescentWindSkill, CrescentWindBuff)
            .UsingToNotSpecChecker(Spec.Weaver)
            .UsingChecker((ba, combatData, agentData, skillData) => !combatData.IsCasting(GrandFinale, ba.To, ba.Time))
            .WithBuilds(GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
        new BuffGainCastFinder(RockyLoopSkill, RockyLoopBuff)
            .UsingToNotSpecChecker(Spec.Weaver)
            .UsingChecker((ba, combatData, agentData, skillData) => !combatData.IsCasting(GrandFinale, ba.To, ba.Time))
            .WithBuilds(GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
        // Scepter
        new DamageCastFinder(LightningStrike, LightningStrike),
        new MissileCastFinder(Hurl, Hurl)
            .UsingICD(900), // Projectiles shoot in 800ms
        // Spear
        new BuffGainCastFinder(EnergizeSkill, EnergizeBuff),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> OutgoingDamageModifiers =
    [
        // Fire
        // - Persisting Flames
        new BuffOnActorDamageModifier(Mod_PersistingFlames, PersistingFlames, "Persisting Flames", "2% per stack", DamageSource.NoPets, 2.0, DamageType.Strike, DamageType.All, Source.Elementalist, ByStack, TraitImages.PersistingFlames, DamageModifierMode.All)
            .WithBuilds( GW2Builds.February2025Balance),
        new BuffOnActorDamageModifier(Mod_PersistingFlames, PersistingFlames, "Persisting Flames", "1% per stack", DamageSource.NoPets, 1.0, DamageType.Strike, DamageType.All, Source.Elementalist, ByStack, TraitImages.PersistingFlames, DamageModifierMode.All)
            .WithBuilds(GW2Builds.July2020Balance, GW2Builds.February2025Balance),
        // - Pyromancer's Training
        new BuffOnActorDamageModifier(Mod_PyromancersTraining, [FireAttunementBuff, FireWaterAttunement, FireAirAttunement, FireEarthAttunement, DualFireAttunement], "Pyromancer's Training", "10% while fire attuned", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Elementalist, ByPresence, TraitImages.PyromancersTraining, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.July2019Balance),
        new BuffOnFoeDamageModifier(Mod_PyromancersTraining, Burning, "Pyromancer's Training", "10% on burning target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Elementalist, ByPresence, TraitImages.PyromancersTraining, DamageModifierMode.All)
            .WithBuilds(GW2Builds.July2019Balance),
        // - Burning Rage
        new BuffOnFoeDamageModifier(Mod_BurningRage, Burning, "Burning Rage", "10% on burning target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Elementalist, ByPresence, TraitImages.BurningRage, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.July2019Balance),
        
        // Air
        // - Bolt to the Heart
        new DamageLogDamageModifier(Mod_BoltToTheHeart, "Bolt to the Heart", "20% if target <50% HP", DamageSource.NoPets, 20.0, DamageType.Strike, DamageType.All, Source.Elementalist, TraitImages.BoltToTheHeart, (x, log) => x.AgainstUnderFifty, DamageModifierMode.All),
        
        // Earth
        // - Serrated Stones
        new BuffOnFoeDamageModifier(Mod_SerratedStones, Bleeding, "Serrated Stones", "5% to bleeding target", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Elementalist, ByPresence, TraitImages.SerratedStones, DamageModifierMode.All),
        
        // Water
        // - Aquamancer's Training
        new DamageLogDamageModifier(Mod_AquamancersTraining, "Aquamancer's Training", "10% if hp >=90%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Elementalist, TraitImages.AquamancersTraining, (x, log) => x.IsOverNinety, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.July2019Balance),
        // - Piercing Shards
        new BuffOnFoeDamageModifier(Mod_PiercingShardsWater, Vulnerability, "Piercing Shards w/ Water", "20% on vuln target while on water", DamageSource.NoPets, 20.0, DamageType.Strike, DamageType.All, Source.Elementalist, ByPresence, TraitImages.PiercingShards, DamageModifierMode.PvE)
            .UsingSrcCheckerByPresence([ WaterAttunementBuff, WaterAirAttunement, WaterEarthAttunement, WaterFireAttunement, DualWaterAttunement ])
            .WithBuilds(GW2Builds.July2019Balance),
        new BuffOnFoeDamageModifier(Mod_PiercingShardsNoWater, Vulnerability, "Piercing Shards", "10% on vuln target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Elementalist, ByPresence, TraitImages.PiercingShards, DamageModifierMode.PvE)
            .UsingSrcCheckerByAbsence([ WaterAttunementBuff, WaterAirAttunement, WaterEarthAttunement, WaterFireAttunement, DualWaterAttunement ])
            .WithBuilds(GW2Builds.July2019Balance),
        new BuffOnFoeDamageModifier(Mod_PiercingShardsWater, Vulnerability, "Piercing Shards w/ Water", "10% on vuln target while on water", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Elementalist, ByPresence, TraitImages.PiercingShards, DamageModifierMode.sPvPWvW)
            .UsingSrcCheckerByPresence([ WaterAttunementBuff, WaterAirAttunement, WaterEarthAttunement, WaterFireAttunement, DualWaterAttunement ])
            .WithBuilds(GW2Builds.July2019Balance),
        new BuffOnFoeDamageModifier(Mod_PiercingShardsNoWater, Vulnerability, "Piercing Shards", "5% on vuln target", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Elementalist, ByPresence, TraitImages.PiercingShards, DamageModifierMode.sPvPWvW)
            .UsingSrcCheckerByAbsence([ WaterAttunementBuff, WaterAirAttunement, WaterEarthAttunement, WaterFireAttunement, DualWaterAttunement ])
            .WithBuilds(GW2Builds.July2019Balance),
        new BuffOnFoeDamageModifier(Mod_PiercingShardsWater, Vulnerability, "Piercing Shards", "20% on vuln target while on water", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Elementalist, ByPresence, TraitImages.PiercingShards, DamageModifierMode.PvE)
            .UsingSrcCheckerByPresence([ WaterAttunementBuff, WaterAirAttunement, WaterEarthAttunement, WaterFireAttunement, DualWaterAttunement ])
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.July2019Balance),
        // - Flow like Water
        new DamageLogDamageModifier(Mod_FlowLikeWater, "Flow like Water", "10% if hp >=75%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Elementalist, TraitImages.FlowLikeWater, (x, log) => x.From.GetCurrentHealthPercent(log, x.Time) >= 75.0, DamageModifierMode.All)
            .WithBuilds(GW2Builds.July2019Balance, GW2Builds.February2020Balance)
            .UsingApproximate(),
        new DamageLogDamageModifier(Mod_FlowLikeWater, "Flow like Water", "10% if hp >=75%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Elementalist, TraitImages.FlowLikeWater, (x, log) => x.From.GetCurrentHealthPercent(log, x.Time) >= 75.0, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.February2020Balance, GW2Builds.June2024Balance)
            .UsingApproximate(),
        new DamageLogDamageModifier(Mod_FlowLikeWater, "Flow like Water", "5% if hp >=75%", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Elementalist, TraitImages.FlowLikeWater, (x, log) => x.From.GetCurrentHealthPercent(log, x.Time) >= 75.0, DamageModifierMode.sPvPWvW)
            .WithBuilds(GW2Builds.February2020Balance, GW2Builds.June2024Balance),
        new DamageLogDamageModifier(Mod_FlowLikeWater10, "Flow like Water (>= 50%)", "10% if hp >=50%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Elementalist, TraitImages.FlowLikeWater, (x, log) => x.From.GetCurrentHealthPercent(log, x.Time) >= 50.0, DamageModifierMode.All)
            .WithBuilds(GW2Builds.June2024Balance),
        new DamageLogDamageModifier(Mod_FlowLikeWater5, "Flow like Water (< 50%)", "5% if hp <50%", DamageSource.NoPets, 5, DamageType.Strike, DamageType.All, Source.Elementalist, TraitImages.FlowLikeWater, (x, log) => x.From.GetCurrentHealthPercent(log, x.Time) < 50, DamageModifierMode.All)
            .WithBuilds(GW2Builds.June2024Balance)
            .UsingApproximate(),

        // Arcane
        // - Bountiful Power
        new BuffOnActorDamageModifier(Mod_BountifulPower, NumberOfBoons, "Bountiful Power", "2% per boon", DamageSource.NoPets, 2.0, DamageType.Strike, DamageType.All, Source.Elementalist, ByStack, TraitImages.BountifulPower, DamageModifierMode.All),
        // - Storm Soul
        new BuffOnFoeDamageModifier(Mod_StormSoul, [Stun, Daze, Knockdown, Fear, Taunt], "Stormsoul", "10% to disabled foes", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.Strike, Source.Elementalist, ByPresence, TraitImages.Stormsoul, DamageModifierMode.All)
            .WithBuilds(GW2Builds.December2018Balance)
            .UsingApproximate(),
        new BuffOnFoeDamageModifier(Mod_StormSoulDefiant, [Stun, Daze, Knockdown, Fear, Taunt], "Stormsoul", "10% to defiant foes", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.Strike, Source.Elementalist, ByAbsence, TraitImages.Stormsoul, DamageModifierMode.All)
            .UsingChecker((x, log) => x.To.GetCurrentBreakbarState(log, x.Time) != BreakbarState.None)
            .WithBuilds(GW2Builds.November2022Balance)
            .UsingApproximate(),

        // Hammer
        new BuffOnActorDamageModifier(Mod_FlameWheel, FlameWheelBuff, "Flame Wheel", "10%", DamageSource.NoPets, 10.0, DamageType.StrikeAndCondition, DamageType.All, Source.Elementalist, ByPresence, SkillImages.FlameWheel, DamageModifierMode.All)
            .WithBuilds(GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM, GW2Builds.November2023Balance),
        new BuffOnActorDamageModifier(Mod_FlameWheel, FlameWheelBuff, "Flame Wheel", "10%", DamageSource.NoPets, 10.0, DamageType.StrikeAndCondition, DamageType.All, Source.Elementalist, ByPresence, SkillImages.FlameWheel, DamageModifierMode.sPvPWvW)
            .WithBuilds(GW2Builds.November2023Balance),
        new BuffOnActorDamageModifier(Mod_FlameWheel, FlameWheelBuff, "Flame Wheel", "15%", DamageSource.NoPets, 15.0, DamageType.StrikeAndCondition, DamageType.All, Source.Elementalist, ByPresence, SkillImages.FlameWheel, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.November2023Balance, GW2Builds.June2025Balance),
        new BuffOnActorDamageModifier(Mod_FlameWheel, FlameWheelBuff, "Flame Wheel", "5%", DamageSource.NoPets, 5.0, DamageType.StrikeAndCondition, DamageType.All, Source.Elementalist, ByPresence, SkillImages.FlameWheel, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.June2025Balance),

        // Pistol
        new BuffOnActorDamageModifier(Mod_RagingRicochet, RagingRichochetBuff, "Raging Ricochet", "5%", DamageSource.NoPets, 5.0, DamageType.Condition, DamageType.All, Source.Elementalist, ByPresence, SkillImages.RagingRicochet, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.February2024NewWeapons),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers =
    [
        // Skills
        // - Signet of Earth
        new BuffOnActorDamageModifier(Mod_SignetOfEarth, SignetOfEarth, "Signet of Earth", "-10% damage", DamageSource.Incoming, -10, DamageType.Strike, DamageType.All, Source.Elementalist, ByPresence, SkillImages.SignetOfEarth, DamageModifierMode.All),
        
        // Earth
        // - Stone Flesh
        new BuffOnActorDamageModifier(Mod_StoneFlesh, [EarthAttunementBuff, FireEarthAttunement, WaterEarthAttunement, EarthAirAttunement, DualEarthAttunement], "Stone Flesh", "-7% damage while attuned to earth", DamageSource.Incoming, -7, DamageType.Strike, DamageType.All, Source.Elementalist, ByPresence, TraitImages.StoneFlesh, DamageModifierMode.All),
        // - Geomancer's Training
        new DamageLogDamageModifier(Mod_GeomancersTraining, "Geomancer's Training", "-10% damage from foes within 360 range", DamageSource.Incoming, -10.0, DamageType.Strike, DamageType.All, Source.Elementalist, TraitImages.GeomancersTraining, (x, log) => TargetWithinRangeChecker(x, log, 360), DamageModifierMode.All)
            .UsingApproximate()
            .WithBuilds(GW2Builds.July2019Balance),

        // Focus
        new CounterOnActorDamageModifier(Mod_ObsidianFlesh, ObsidianFlesh, "Obsidian Flesh", "Invulnerable", DamageSource.Incoming, DamageType.Strike, DamageType.All, Source.Elementalist, SkillImages.ObsidianFlesh, DamageModifierMode.All)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.July2018Balance),
        new CounterOnActorDamageModifier(Mod_ObsidianFlesh, ObsidianFlesh, "Obsidian Flesh", "Invulnerable", DamageSource.Incoming, DamageType.All, DamageType.All, Source.Elementalist, SkillImages.ObsidianFlesh, DamageModifierMode.All)
            .WithBuilds(GW2Builds.July2018Balance),
    ];


    internal static readonly IReadOnlyList<Buff> Buffs =
    [       
        // Signets
        new Buff("Signet of Restoration", SignetOfRestoration, Source.Elementalist, BuffClassification.Other, SkillImages.SignetOfRestoration),
        new Buff("Signet of Air", SignetOfAirBuff, Source.Elementalist, BuffClassification.Other, SkillImages.SignetOfAir),
        new Buff("Signet of Earth", SignetOfEarth, Source.Elementalist, BuffClassification.Other, SkillImages.SignetOfEarth),
        new Buff("Signet of Fire", SignetOfFire, Source.Elementalist, BuffClassification.Other, SkillImages.SignetOfFire),
        new Buff("Signet of Water", SignetOfWater, Source.Elementalist, BuffClassification.Other, SkillImages.SignetOfWater),
        // Attunements
        new Buff("Fire Attunement", FireAttunementBuff, Source.Elementalist, BuffClassification.Other, SkillImages.FireAttunement),
        new Buff("Water Attunement", WaterAttunementBuff, Source.Elementalist, BuffClassification.Other, SkillImages.WaterAttunement),
        new Buff("Air Attunement", AirAttunementBuff, Source.Elementalist, BuffClassification.Other, SkillImages.AirAttunement),
        new Buff("Earth Attunement", EarthAttunementBuff, Source.Elementalist, BuffClassification.Other, SkillImages.EarthAttunement),
        // Forms
        new Buff("Mist Form", MistForm, Source.Elementalist, BuffClassification.Other, SkillImages.MistForm),
        new Buff("Mist Form 2", MistForm2, Source.Elementalist, BuffClassification.Other, SkillImages.MistForm),
        new Buff("Ride the Lightning",RideTheLightningBuff, Source.Elementalist, BuffClassification.Other, SkillImages.RideTheLightning),
        new Buff("Vapor Form", VaporForm, Source.Elementalist, BuffClassification.Other, SkillImages.VaporForm),
        new Buff("Tornado", Tornado, Source.Elementalist, BuffClassification.Other, SkillImages.Tornado),
        new Buff("Whirlpool", Whirlpool, Source.Elementalist, BuffClassification.Other, SkillImages.Whirlpool),
        new Buff("Electrified Tornado", ElectrifiedTornado, Source.Elementalist, BuffClassification.Other, SkillImages.ChainLightning),
        new Buff("Arcane Lightning", ArcaneLightning, Source.Elementalist, BuffClassification.Other, TraitImages.ElementalSurge),
        // Conjures
        new Buff("Conjure Earth Shield", ConjureEarthShield, Source.Elementalist, BuffClassification.Support, SkillImages.ConjureEarthShield),
        new Buff("Conjure Flame Axe", ConjureFlameAxe, Source.Elementalist, BuffClassification.Support, SkillImages.ConjureFlameAxe),
        new Buff("Conjure Frost Bow", ConjureFrostBow, Source.Elementalist, BuffClassification.Support, SkillImages.ConjureFrostBow),
        new Buff("Conjure Lightning Hammer", ConjureLightningHammer, Source.Elementalist, BuffClassification.Support, SkillImages.ConjureLightningHammer),
        new Buff("Conjure Fiery Greatsword", ConjureFieryGreatsword, Source.Elementalist, BuffClassification.Support, SkillImages.ConjureFieryGreatsword),
        new Buff("Freeze 1", Freeze1, Source.Elementalist, BuffClassification.Other, BuffImages.Stun),
        new Buff("Freeze 2", Freeze2, Source.Elementalist, BuffClassification.Other, BuffImages.Stun),
        // Summons
        new Buff("Lesser Air Elemental Summoned", LesserAirElementalSummoned, Source.Elementalist, BuffStackType.StackingConditionalLoss, 5, BuffClassification.Other, SkillImages.GlyphOfLesserElementalsAir),
        new Buff("Air Elemental Summoned", AirElementalSummoned, Source.Elementalist, BuffClassification.Other, SkillImages.GlyphOfElementalsAir),
        new Buff("Lesser Water Elemental Summoned", LesserWaterElementalSummoned, Source.Elementalist, BuffStackType.StackingConditionalLoss, 5, BuffClassification.Other, SkillImages.GlyphOfLesserElementalsWater),
        new Buff("Water Elemental Summoned", WaterElementalSummoned, Source.Elementalist, BuffClassification.Other, SkillImages.GlyphOfElementalsWater),
        new Buff("Lesser Fire Elemental Summoned", LesserFireElementalSummoned, Source.Elementalist, BuffStackType.StackingConditionalLoss, 5, BuffClassification.Other, SkillImages.GlyphOfLesserElementalsFire),
        new Buff("Fire Elemental Summoned", FireElementalSummoned, Source.Elementalist, BuffClassification.Other, SkillImages.GlyphOfElementalsFire),
        new Buff("Lesser Earth Elemental Summoned", LesserEarthElementalSummoned, Source.Elementalist, BuffStackType.StackingConditionalLoss, 5, BuffClassification.Other, SkillImages.GlyphOfLesserElementalsEarth),
        new Buff("Earth Elemental Summoned", EarthElementalSummoned, Source.Elementalist, BuffClassification.Other, SkillImages.GlyphOfElementalsEarth),
        // Skills
        new Buff("Arcane Power", ArcanePowerBuff, Source.Elementalist, BuffStackType.Stacking, 6, BuffClassification.Other, SkillImages.ArcanePower),
        new Buff("Arcane Power (Ferocity)", ArcanePowerFerocityBuff, Source.Elementalist, BuffClassification.Other, SkillImages.ArcanePower),
        new Buff("Arcane Shield", ArcaneShieldBuff, Source.Elementalist, BuffStackType.Stacking, 25, BuffClassification.Other, SkillImages.ArcaneShield),
        new Buff("Renewal of Fire", RenewalOfFire, Source.Elementalist, BuffClassification.Other, SkillImages.RenewalOfFire),
        new Buff("Rock Barrier", RockBarrier, Source.Elementalist, BuffClassification.Other, SkillImages.RockBarrier),
        new Buff("Magnetic Wave", MagneticWave, Source.Elementalist, BuffClassification.Other, SkillImages.MagneticWave),
        new Buff("Obsidian Flesh", ObsidianFlesh, Source.Elementalist, BuffClassification.Other, SkillImages.ObsidianFlesh),
        new Buff("Persisting Flames", PersistingFlames, Source.Elementalist, BuffStackType.Stacking, 10, BuffClassification.Other, TraitImages.PersistingFlames)
            .WithBuilds(GW2Builds.July2020Balance),
        new Buff("Fresh Air", FreshAir, Source.Elementalist, BuffClassification.Other, TraitImages.FreshAir),
        new Buff("Soothing Mist", SoothingMist, Source.Elementalist, BuffClassification.Defensive, TraitImages.SoothingMist)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2023Balance),
        new Buff("Soothing Mist", SoothingMist, Source.Elementalist, BuffStackType.Queue, 9, BuffClassification.Defensive, TraitImages.SoothingMist)
            .WithBuilds(GW2Builds.May2023Balance),
        new Buff("Stone Heart", StoneHeart, Source.Elementalist, BuffClassification.Defensive, TraitImages.StoneHeart),
        new Buff("Glyph of Elemental Power (Fire)", GlyphOfElementalPowerFireBuff, Source.Elementalist, BuffStackType.StackingConditionalLoss, 5, BuffClassification.Other, SkillImages.GlyphOfElementalPowerFire),
        new Buff("Glyph of Elemental Power (Air)", GlyphOfElementalPowerAirBuff, Source.Elementalist, BuffStackType.StackingConditionalLoss, 5, BuffClassification.Other, SkillImages.GlyphOfElementalPowerAir),
        new Buff("Glyph of Elemental Power (Water)", GlyphOfElementalPowerWaterBuff, Source.Elementalist, BuffStackType.StackingConditionalLoss, 5, BuffClassification.Other, SkillImages.GlyphOfElementalPowerWater),
        new Buff("Glyph of Elemental Power (Earth)", GlyphOfElementalPowerEarthBuff, Source.Elementalist, BuffStackType.StackingConditionalLoss, 5, BuffClassification.Other, SkillImages.GlyphOfElementalPowerEarth),
        // Hammer
        new Buff("Flame Wheel", FlameWheelBuff, Source.Elementalist, BuffClassification.Other, SkillImages.FlameWheel)
            .WithBuilds(GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
        new Buff("Icy Coil", IcyCoilBuff, Source.Elementalist, BuffClassification.Other, SkillImages.IcyCoil)
            .WithBuilds( GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
        new Buff("Crescent Wind", CrescentWindBuff, Source.Elementalist, BuffClassification.Other, SkillImages.CrescentWind)
            .WithBuilds(GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
        new Buff("Rocky Loop", RockyLoopBuff, Source.Elementalist, BuffClassification.Other, SkillImages.RockyLoop)
            .WithBuilds(GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
        // Pistol
        new Buff("Fire Bullet", FireBullet, Source.Elementalist, BuffClassification.Other, SkillImages.ScorchingShot)
            .WithBuilds(GW2Builds.February2024NewWeapons),
        new Buff("Ice Bullet", IceBullet, Source.Elementalist, BuffClassification.Other, SkillImages.SoothingSplash)
            .WithBuilds( GW2Builds.February2024NewWeapons),
        new Buff("Air Bullet", AirBullet, Source.Elementalist, BuffClassification.Other, SkillImages.ElectricExposure)
            .WithBuilds(GW2Builds.February2024NewWeapons),
        new Buff("Earth Bullet", EarthBullet, Source.Elementalist, BuffClassification.Other, SkillImages.PiercingPebble)
            .WithBuilds(GW2Builds.February2024NewWeapons),
        new Buff("Raging Ricochet", RagingRichochetBuff, Source.Elementalist, BuffStackType.Queue, 9, BuffClassification.Other, SkillImages.RagingRicochet)
            .WithBuilds( GW2Builds.February2024NewWeapons),
        new Buff("Dazing Discharge", DazingDischargeBuff, Source.Elementalist, BuffClassification.Other, SkillImages.DazingDischarge)
            .WithBuilds(GW2Builds.February2024NewWeapons),
        new Buff("Shattering Stone", ShatteringStoneBuff, Source.Elementalist, BuffStackType.StackingConditionalLoss, 25, BuffClassification.Other, SkillImages.ShatteringStone)
            .WithBuilds(GW2Builds.February2024NewWeapons),
        // Spear
        new Buff("Seethe", SeetheBuff, Source.Elementalist, BuffClassification.Other, SkillImages.Seethe),
        new Buff("Ripple", RippleBuff, Source.Elementalist, BuffClassification.Other, SkillImages.Ripple),
        new Buff("Energize", EnergizeBuff, Source.Elementalist, BuffClassification.Other, SkillImages.Energize),
        new Buff("Fulgor", FulgorBuff, Source.Elementalist, BuffStackType.StackingUniquePerSrc, 999, BuffClassification.Other, SkillImages.Fulgor),
        new Buff("Harden", HardenBuff, Source.Elementalist, BuffClassification.Other, SkillImages.Harden),
    ];


    private static readonly HashSet<long> _attunements =
    [
        FireAttunementSkill, WaterAttunementSkill, AirAttunementSkill, EarthAttunementSkill
    ];

    public static bool IsAttunementSwap(long id)
    {
        return _attunements.Contains(id);
    }

    public static void RemoveDualBuffs(IReadOnlyList<BuffEvent> buffsPerDst, Dictionary<long, List<BuffEvent>> buffsByID, SkillData skillData)
    {
        var duals = new HashSet<long>
        {
            DualFireAttunement,
            DualWaterAttunement,
            DualAirAttunement,
            DualEarthAttunement,
        };
        var toClean = new HashSet<long>();
        foreach (BuffEvent c in buffsPerDst.Where(x => duals.Contains(x.BuffID)))
        {
            toClean.Add(c.BuffID);
            c.Invalidate(skillData);
        }
        foreach (long buffID in toClean)
        {
            buffsByID[buffID].RemoveAll(x => x.BuffID == NoBuff);
        }
    }

    private static readonly HashSet<int> Minions =
    [
        (int)MinionID.LesserAirElemental,
        (int)MinionID.LesserEarthElemental,
        (int)MinionID.LesserFireElemental,
        (int)MinionID.LesserIceElemental,
        (int)MinionID.AirElemental,
        (int)MinionID.EarthElemental,
        (int)MinionID.FireElemental,
        (int)MinionID.IceElemental,
    ];
    internal static bool IsKnownMinionID(int id)
    {
        return Minions.Contains(id);
    }

    internal static void ComputeProfessionCombatReplayActors(PlayerActor player, ParsedEvtcLog log, CombatReplay replay)
    {
        Color color = Colors.Elementalist;

        // Meteor Shower - Outer circle
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ElementalistMeteorShowerCircle, out var meteorShowerCircles))
        {
            var skill = new SkillModeDescriptor(player, Spec.Elementalist, MeteorShower);
            foreach (EffectEvent effect in meteorShowerCircles)
            {
                (long, long) lifespan = effect.ComputeDynamicLifespan(log, 9000);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 360, EffectImages.EffectMeteorShower);
            }
            // The meteors
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ElementalistMeteorShowerMeteor, out var meteorShowersMeteors))
            {
                foreach (EffectEvent effect in meteorShowersMeteors)
                {
                    (long start, long end) = effect.ComputeLifespan(log, 1330);
                    var connector = new PositionConnector(effect.Position);
                    // -750 to make the decoration faster than in game
                    replay.Decorations.Add(new CircleDecoration(180, (end - 750, end), color, 0.5, connector).UsingFilled(false).UsingSkillMode(skill));
                    replay.Decorations.Add(new CircleDecoration(180, (end - 750, end), color, 0.2, connector).UsingFilled(false).UsingGrowingEnd(end, true).UsingSkillMode(skill));
                }
            }
        }

        // Static Field (Staff)
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ElementalistStaticFieldStaff, out var staticFieldsStaff))
        {
            var skill = new SkillModeDescriptor(player, Spec.Elementalist, StaticFieldStaff, SkillModeCategory.CC);
            foreach (EffectEvent effect in staticFieldsStaff)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 4000);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 180, EffectImages.EffectStaticField);
            }
        }

        // Static Field (Lightning Hammer)
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ElementalistStaticFieldLightningHammer, out var staticFieldsHammer))
        {
            var skill = new SkillModeDescriptor(player, Spec.Elementalist, StaticFieldLightingHammer, SkillModeCategory.CC);
            foreach (EffectEvent effect in staticFieldsHammer)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 4000);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, EffectImages.EffectStaticField);
            }
        }

        // Updraft
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ElementalistUpdraft2, out var updrafts))
        {
            var skill = new SkillModeDescriptor(player, Spec.Elementalist, Updraft, SkillModeCategory.CC);
            foreach (EffectEvent effect in updrafts)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 750);
                var connector = new PositionConnector(effect.Position);
                var circle = (CircleDecoration)new CircleDecoration(240, lifespan, color, 0.3, connector).UsingFilled(false).UsingSkillMode(skill);
                replay.Decorations.AddWithGrowing(circle, lifespan.Item2, true);
                replay.Decorations.Add(new IconDecoration(EffectImages.EffectUpdraft, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector).UsingSkillMode(skill));
            }
        }

        // Firestorm
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ElementalistFirestorm, out var firestorms))
        {
            var firestormCasts = player.GetAnimatedCastEvents(log).Where(x => x.SkillID == FirestormGlyphOfStorms || x.SkillID == FirestormFieryGreatsword).ToList();
            foreach (EffectEvent effect in firestorms)
            {
                SkillModeDescriptor skill;
                string icon;
                var firestormCastsOnEffect = firestormCasts.Where(x => effect.Time - ServerDelayConstant > x.Time && x.EndTime > effect.Time + ServerDelayConstant);
                if (firestormCastsOnEffect.Count() == 1)
                {
                    skill = new SkillModeDescriptor(player, Spec.Necromancer, firestormCastsOnEffect.First().SkillID);
                    icon = skill.SkillID == FirestormGlyphOfStorms ? EffectImages.EffectFirestormGlyph : EffectImages.EffectFirestormFieryGreatsword;
                }
                else
                {
                    skill = new SkillModeDescriptor(player, Spec.Elementalist, FirestormGlyphOfStormsOrFieryGreatsword);
                    icon = EffectImages.EffectFirestormGlyphOrFieryGreatsword;
                }
                (long, long) lifespan = effect.ComputeDynamicLifespan(log, 10000);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, icon);
            }
        }

        // Geyser
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ElementalistGeyser, out var geysers))
        {
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ElementalistGeyserSplash, out var geyserSplash))
            {
                var skill = new SkillModeDescriptor(player, Spec.Elementalist, GeyserStaffElementalist, SkillModeCategory.Heal);
                foreach (EffectEvent effect in geysers)
                {
                    if (!geyserSplash.Any(x => Math.Abs(x.Time - effect.Time) < ServerDelayConstant))
                    {
                        continue;
                    }
                    (long, long) lifespan = effect.ComputeLifespan(log, 4000);
                    AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, EffectImages.EffectGeyser);
                }
            }
        }

        // Meteor
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ElementalistMeteor, out var meteors))
        {
            var skill = new SkillModeDescriptor(player, Spec.Elementalist, Meteor);
            foreach (EffectEvent effect in meteors)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 1000);
                AddDoughnutSkillDecoration(replay, effect, color, skill, lifespan, 120, 240, EffectImages.EffectMeteor);
            }
        }

        // Etching: Volcano
        var etchingVolcanoEffects = new []
        {
            EffectGUIDs.ElementalistEtchingVolcanoTier0,
            EffectGUIDs.ElementalistEtchingVolcanoTier1,
            EffectGUIDs.ElementalistEtchingVolcanoTier2,
            EffectGUIDs.ElementalistEtchingVolcanoTier3,
            EffectGUIDs.ElementalistEtchingVolcanoPerfect,
        };
        if (log.CombatData.TryGetEffectEventsBySrcWithGUIDs(player.AgentItem, etchingVolcanoEffects, out var etchingVolcano))
        {
            AddEtchingDecorations(log, player, replay, color, etchingVolcano, EtchingVolcano, EffectImages.EffectEtchingVolcano);
        }

        // Lesser Volcano
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ElementalistLesserVolcano, out var lesserVolcano))
        {
            var skill = new SkillModeDescriptor(player, Spec.Elementalist, LesserVolcano);
            foreach (EffectEvent effect in lesserVolcano)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 4400);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 360, EffectImages.EffectLesserVolcano);
                // Hits landing from the volcano
                AddVolcanoProjectileHitDecorations(log, player, replay, skill, color, effect.Time, effect.Duration);
            }
        }

        // Volcano
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ElementalistVolcano, out var volcano))
        {
            var skill = new SkillModeDescriptor(player, Spec.Elementalist, Volcano);
            foreach (EffectEvent effect in volcano)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 4500);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 360, EffectImages.EffectVolcano);
                // Hits landing from the volcano
                AddVolcanoProjectileHitDecorations(log, player, replay, skill, color, effect.Time, effect.Duration);
            }
        }

        // Undertow
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ElementalistUndertow, out var undertows))
        {
            var skill = new SkillModeDescriptor(player, Spec.Elementalist, Undertow, SkillModeCategory.CC);
            foreach (EffectEvent effect in undertows)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 1000); // logged duration of 0 - overriding it to 1000 like the others
                AddDoughnutSkillDecoration(replay, effect, color, skill, lifespan, 120, 240, EffectImages.EffectUndertow);
            }
        }

        // Etching: Jökulhlaup
        var etchingJokulhlaupEffects = new[]
        {
            EffectGUIDs.ElementalistEtchingJokulhlaupTier0,
            EffectGUIDs.ElementalistEtchingJokulhlaupTier1,
            EffectGUIDs.ElementalistEtchingJokulhlaupTier2,
            EffectGUIDs.ElementalistEtchingJokulhlaupTier3,
            EffectGUIDs.ElementalistEtchingJokulhlaupPerfect,
        };
        if (log.CombatData.TryGetEffectEventsBySrcWithGUIDs(player.AgentItem, etchingJokulhlaupEffects, out var etchingJokulhlaup))
        {
            AddEtchingDecorations(log, player, replay, color, etchingJokulhlaup, EtchingJokulhlaup, EffectImages.EffectEtchingJokulhlaup);
        }

        // Fulgor
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ElementalistFulgor, out var fulgors))
        {
            var skill = new SkillModeDescriptor(player, Spec.Elementalist, FulgorSkill);
            foreach (EffectEvent effect in fulgors)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 4000);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 180, EffectImages.EffectFulgor);
            }
        }

        // Twister
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ElementalistTwister, out var twisters))
        {
            var skill = new SkillModeDescriptor(player, Spec.Elementalist, Twister, SkillModeCategory.CC);
            foreach (EffectEvent effect in twisters)
            {
                (long, long) lifespan = effect.ComputeDynamicLifespan(log, 1000);
                AddDoughnutSkillDecoration(replay, effect, color, skill, lifespan, 120, 240, EffectImages.EffectTwister);
            }
        }

        // Etching: Derecho
        var etchingDerechoEffects = new[]
        {
            EffectGUIDs.ElementalistEtchingDerechoTier0,
            EffectGUIDs.ElementalistEtchingDerechoTier1,
            EffectGUIDs.ElementalistEtchingDerechoTier2,
            EffectGUIDs.ElementalistEtchingDerechoTier3,
            EffectGUIDs.ElementalistEtchingDerechoPerfect,
        };
        if (log.CombatData.TryGetEffectEventsBySrcWithGUIDs(player.AgentItem, etchingDerechoEffects, out var etchingDerecho))
        {
            AddEtchingDecorations(log, player, replay, color, etchingDerecho, EtchingDerecho, EffectImages.EffectEtchingDerecho);
        }

        // Fissure
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ElementalistFissure, out var fissures))
        {
            var skill = new SkillModeDescriptor(player, Spec.Elementalist, Fissure);
            foreach (EffectEvent effect in fissures)
            {
                (long, long) lifespan = effect.ComputeDynamicLifespan(log, 1000); // logged duration of 0 - overriding it to 1000 like the others
                AddDoughnutSkillDecoration(replay, effect, color, skill, lifespan, 120, 240, EffectImages.EffectFissure);
            }
        }

        // Etching: Haboob
        var etchingHaboobEffects = new[]
        {
            EffectGUIDs.ElementalistEtchingHaboobTier0,
            EffectGUIDs.ElementalistEtchingHaboobTier1,
            EffectGUIDs.ElementalistEtchingHaboobTier2,
            EffectGUIDs.ElementalistEtchingHaboobTier3,
            EffectGUIDs.ElementalistEtchingHaboobPerfect,
        };
        if (log.CombatData.TryGetEffectEventsBySrcWithGUIDs(player.AgentItem, etchingHaboobEffects, out var etchingHaboob))
        {
            AddEtchingDecorations(log, player, replay, color, etchingHaboob, EtchingHaboob, EffectImages.EffectEtchingHaboob);
        }
    }

    /// <summary>
    /// Adds AoEs displaying the projectile hits for <see cref="LesserVolcano"/> and <see cref="Volcano"/>.
    /// </summary>
    /// <param name="log">The log.</param>
    /// <param name="player">The player casting.</param>
    /// <param name="replay">The Combat Replay.</param>
    /// <param name="skill">The casted skill.</param>
    /// <param name="color">The specialization color.</param>
    /// <param name="volcanoStartTime">The Volcano effect start time.</param>
    /// <param name="volcanoDuraiton">The Volcano effect duration.</param>
    private static void AddVolcanoProjectileHitDecorations(ParsedEvtcLog log, PlayerActor player, CombatReplay replay, SkillModeDescriptor skill, Color color, long volcanoStartTime, long volcanoDuraiton)
    {
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ElementalistVolcanoHits, out var volcanoHits))
        {
            foreach (EffectEvent hitEffect in volcanoHits.Where(x => x.Time > volcanoStartTime && x.Time < volcanoStartTime + volcanoDuraiton))
            {
                (long, long) lifespanHit = hitEffect.ComputeLifespan(log, 500); // Logged duration of 0, setting 500 as a visual display
                var connector = new PositionConnector(hitEffect.Position);
                replay.Decorations.Add(new CircleDecoration(200, lifespanHit, color, 0.5, connector).UsingFilled(false).UsingSkillMode(skill));
            }
        }
    }

    /// <summary>
    /// Adds AoEs for the 4 Etchings.
    /// </summary>
    /// <param name="log">The log.</param>
    /// <param name="player">The player casting.</param>
    /// <param name="replay">The Combat Replay.</param>
    /// <param name="color">The specialization color.</param>
    /// <param name="effects">The etching effects.</param>
    /// <param name="skillID">The etching skill ID.</param>
    /// <param name="icon">The etching icon.</param>
    private static void AddEtchingDecorations(ParsedEvtcLog log, PlayerActor player, CombatReplay replay, Color color, IReadOnlyList<EffectEvent> effects, long skillID, string icon)
    {
        var skill = new SkillModeDescriptor(player, Spec.Elementalist, skillID);
        foreach (EffectEvent effect in effects)
        {
            (long, long) lifespan = effect.ComputeDynamicLifespan(log, 7000);
            AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 360, icon);
        }
    }
}
