using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.DamageModifierIDs;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData;

internal static class CatalystHelper
{

    internal static readonly List<InstantCastFinder> InstantCastFinder =
    [
        new BuffGainCastFinder(FlameWheelSkill, FlameWheelBuff)
            .UsingChecker((ba, combatData, agentData, skillData) => !combatData.IsCasting(GrandFinale, ba.To, ba.Time))
            .WithBuilds(GW2Builds.EODBeta4, GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
        new BuffGainCastFinder(IcyCoilSkill, IcyCoilBuff)
            .UsingChecker((ba, combatData, agentData, skillData) => !combatData.IsCasting(GrandFinale, ba.To, ba.Time))
            .WithBuilds(GW2Builds.EODBeta4, GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
        new BuffGainCastFinder(CrescentWindSkill, CrescentWindBuff)
            .UsingChecker((ba, combatData, agentData, skillData) => !combatData.IsCasting(GrandFinale, ba.To, ba.Time))
            .WithBuilds(GW2Builds.EODBeta4, GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
        new BuffGainCastFinder(RockyLoopSkill, RockyLoopBuff)
            .UsingChecker((ba, combatData, agentData, skillData) => !combatData.IsCasting(GrandFinale, ba.To, ba.Time))
            .WithBuilds(GW2Builds.EODBeta4, GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
        new BuffGainCastFinder(InvigoratingAirSkill, InvigoratingAirBuff)
            .WithBuilds(GW2Builds.EODBeta4, GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
        new EffectCastFinder(DeployJadeSphereFire, EffectGUIDs.CatalystDeployFireJadeSphere)
            .UsingSrcSpecChecker(Spec.Catalyst),
        new EffectCastFinder(DeployJadeSphereAir, EffectGUIDs.CatalystDeployAirJadeSphere)
            .UsingSrcSpecChecker(Spec.Catalyst),
        new EffectCastFinder(DeployJadeSphereWater, EffectGUIDs.CatalystDeployWaterJadeSphere)
            .UsingSrcSpecChecker(Spec.Catalyst),
        new EffectCastFinder(DeployJadeSphereEarth, EffectGUIDs.CatalystDeployEarthJadeSphere)
            .UsingSrcSpecChecker(Spec.Catalyst)
    ];


    internal static readonly IReadOnlyList<DamageModifierDescriptor> OutgoingDamageModifiers =
    [
        new BuffOnActorDamageModifier(Mod_FlameWheel, FlameWheelBuff, "Flame Wheel", "5%", DamageSource.NoPets, 5.0, DamageType.StrikeAndCondition, DamageType.All, Source.Catalyst, ByPresence, SkillImages.FlameWheel, DamageModifierMode.All)
            .WithBuilds(GW2Builds.EODBeta2, GW2Builds.March2022Balance2),
        new BuffOnActorDamageModifier(Mod_FlameWheel, FlameWheelBuff, "Flame Wheel", "10%", DamageSource.NoPets, 10.0, DamageType.StrikeAndCondition, DamageType.All, Source.Catalyst, ByPresence, SkillImages.FlameWheel, DamageModifierMode.All)
            .WithBuilds(GW2Builds.March2022Balance2, GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
        new BuffOnActorDamageModifier(Mod_RelentlessFire, RelentlessFire, "Relentless Fire", "15%", DamageSource.NoPets, 15.0, DamageType.StrikeAndCondition, DamageType.All, Source.Catalyst, ByPresence, SkillImages.RelentlessFire, DamageModifierMode.All)
            .WithBuilds(GW2Builds.EODBeta2, GW2Builds.March2022Balance),
        new BuffOnActorDamageModifier(Mod_RelentlessFire, RelentlessFire, "Relentless Fire", "10%", DamageSource.NoPets, 10.0, DamageType.StrikeAndCondition, DamageType.All, Source.Catalyst, ByPresence, SkillImages.RelentlessFire, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.March2022Balance),
        new BuffOnActorDamageModifier(Mod_RelentlessFire, RelentlessFire, "Relentless Fire", "15%", DamageSource.NoPets, 15.0, DamageType.StrikeAndCondition, DamageType.All, Source.Catalyst, ByPresence, SkillImages.RelentlessFire, DamageModifierMode.sPvPWvW)
            .WithBuilds(GW2Builds.March2022Balance),
        new BuffOnActorDamageModifier(Mod_EmpoweringAuras, EmpoweringAuras, "Empowering Auras", "2% per stack", DamageSource.NoPets, 2.0, DamageType.StrikeAndCondition, DamageType.All, Source.Catalyst, ByStack, TraitImages.EmpoweringAuras, DamageModifierMode.All)
            .WithBuilds(GW2Builds.EODBeta2, GW2Builds.November2022Balance),
        new BuffOnActorDamageModifier(Mod_EmpoweringAuras, EmpoweringAuras, "Empowering Auras", "2% per stack", DamageSource.NoPets, 2.0, DamageType.StrikeAndCondition, DamageType.All, Source.Catalyst, ByStack, TraitImages.EmpoweringAuras, DamageModifierMode.sPvPWvW)
            .WithBuilds(GW2Builds.November2022Balance, GW2Builds.September2023Balance),
        new BuffOnActorDamageModifier(Mod_EmpoweringAuras, EmpoweringAuras, "Empowering Auras", "3% per stack", DamageSource.NoPets, 3.0, DamageType.StrikeAndCondition, DamageType.All, Source.Catalyst, ByStack, TraitImages.EmpoweringAuras, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.November2022Balance, GW2Builds.September2023Balance),
        new BuffOnActorDamageModifier(Mod_EmpoweringAuras, EmpoweringAuras, "Empowering Auras", "2% per stack", DamageSource.NoPets, 2.0, DamageType.StrikeAndCondition, DamageType.All, Source.Catalyst, ByStack, TraitImages.EmpoweringAuras, DamageModifierMode.All)
            .WithBuilds(GW2Builds.September2023Balance),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers =
    [
        new BuffOnActorDamageModifier(Mod_HardenedAuras, HardenedAuras, "Hardened Auras", "-2% damage per stack", DamageSource.Incoming, -2, DamageType.Strike, DamageType.All, Source.Catalyst, ByStack, TraitImages.HardenedAuras, DamageModifierMode.All),// TODO Check if strike only
    ];


    internal static readonly IReadOnlyList<Buff> Buffs =
    [
        new Buff("Flame Wheel", FlameWheelBuff, Source.Catalyst, BuffClassification.Other, SkillImages.FlameWheel)
            .WithBuilds(GW2Builds.EODBeta1, GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
        new Buff("Icy Coil", IcyCoilBuff, Source.Catalyst, BuffClassification.Other, SkillImages.IcyCoil)
            .WithBuilds(GW2Builds.EODBeta1, GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
        new Buff("Crescent Wind", CrescentWindBuff, Source.Catalyst, BuffClassification.Other, SkillImages.CrescentWind)
            .WithBuilds(GW2Builds.EODBeta1, GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
        new Buff("Rocky Loop", RockyLoopBuff, Source.Catalyst, BuffClassification.Other, SkillImages.RockyLoop)
            .WithBuilds(GW2Builds.EODBeta1, GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
        new Buff("Relentless Fire", RelentlessFire, Source.Catalyst, BuffClassification.Other, SkillImages.RelentlessFire),
        new Buff("Shattering Ice", ShatteringIce, Source.Catalyst, BuffClassification.Other, SkillImages.ShatteringIce),
        new Buff("Invigorating Air", InvigoratingAirBuff, Source.Catalyst, BuffClassification.Other, SkillImages.InvigoratingAir),
        new Buff("Immutable Stone", ImmutableStoneBuff, Source.Catalyst, BuffClassification.Other, SkillImages.ImmutableStone),
        new Buff("Fortified Earth", FortifiedEarth, Source.Catalyst, BuffClassification.Other, SkillImages.FortifiedEarth),
        new Buff("Elemental Celerity", ElementalCelerity, Source.Catalyst, BuffClassification.Other, SkillImages.ElementalCelerity),
        new Buff("Soothing Water", SoothingWaterBuff, Source.Catalyst, BuffClassification.Other, SkillImages.SoothingWater),
        new Buff("Elemental Empowerment", ElementalEmpowerment, Source.Catalyst, BuffStackType.Stacking, 10, BuffClassification.Other, TraitImages.ElementalEmpowerment),
        new Buff("Empowering Auras", EmpoweringAuras, Source.Catalyst, BuffStackType.Stacking, 5, BuffClassification.Other, TraitImages.EmpoweringAuras),
        new Buff("Hardened Auras", HardenedAuras, Source.Catalyst, BuffStackType.StackingConditionalLoss, 5, BuffClassification.Other, TraitImages.HardenedAuras),
    ];

    internal static void ComputeProfessionCombatReplayActors(PlayerActor player, ParsedEvtcLog log, CombatReplay replay)
    {
        Color color = Colors.Elementalist;

        AddJadeSphereDecoration(player, log, replay, color, EffectGUIDs.CatalystDeployFireJadeSphere, DeployJadeSphereFire, EffectImages.EffectDeployJadeSphereFire);
        AddJadeSphereDecoration(player, log, replay, color, EffectGUIDs.CatalystDeployWaterJadeSphere, DeployJadeSphereWater, EffectImages.EffectDeployJadeSphereWater);
        AddJadeSphereDecoration(player, log, replay, color, EffectGUIDs.CatalystDeployAirJadeSphere, DeployJadeSphereAir, EffectImages.EffectDeployJadeSphereAir);
        AddJadeSphereDecoration(player, log, replay, color, EffectGUIDs.CatalystDeployEarthJadeSphere, DeployJadeSphereEarth, EffectImages.EffectDeployJadeSphereEarth);
    }

    internal static void AddJadeSphereDecoration(PlayerActor player, ParsedEvtcLog log, CombatReplay replay, Color color, GUID effect, long skillId, string icon)
    {
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, effect, out var events))
        {
            var skill = new SkillModeDescriptor(player, Spec.Catalyst, skillId);
            foreach (EffectEvent @event in events)
            {
                (long, long) lifespan = @event.ComputeLifespan(log, 5000);
                var connector = new PositionConnector(@event.Position);
                replay.Decorations.Add(new CircleDecoration(240, lifespan, color, 0.5, connector).UsingFilled(false).UsingSkillMode(skill));
                replay.Decorations.Add(new CircleDecoration(360, lifespan, color, 0.3, connector).UsingFilled(false).UsingSkillMode(skill));
                replay.Decorations.Add(new IconDecoration(icon, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector).UsingSkillMode(skill));
            }
        }
    }
}
