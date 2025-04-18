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

namespace GW2EIEvtcParser.EIData;

internal static class HarbingerHelper
{

    internal static readonly List<InstantCastFinder> InstantCastFinder =
    [
        new BuffGainCastFinder(EnterHarbingerShroud, HarbingerShroud)
            .UsingBeforeWeaponSwap(true),
        new BuffLossCastFinder(ExitHarbingerShroud, HarbingerShroud)
            .UsingBeforeWeaponSwap(true),
        new DamageCastFinder(CascadingCorruption, CascadingCorruption)
            .UsingDisableWithEffectData()
            .UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
        new EffectCastFinderByDst(CascadingCorruption, EffectGUIDs.HarbingerCascadingCorruption)
            .UsingDstSpecChecker(Spec.Harbinger)
            .UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
        new EffectCastFinderByDst(DeathlyHaste, EffectGUIDs.HarbingerDeathlyHaste)
            .UsingDstSpecChecker(Spec.Harbinger)
            .UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
        new EffectCastFinderByDst(ApproachingDoom, EffectGUIDs.HarbingerDoomApproaches)
            .UsingDstSpecChecker(Spec.Harbinger)
            .UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
    ];


    internal static readonly IReadOnlyList<DamageModifierDescriptor> OutgoingDamageModifiers =
    [
        new BuffOnActorDamageModifier(Mod_WickedCorruption, Blight, "Wicked Corruption", "1% per blight stack", DamageSource.NoPets, 1.0, DamageType.Strike, DamageType.All, Source.Harbinger, ByStack, TraitImages.WickedCorruption, DamageModifierMode.All)
            .WithBuilds(GW2Builds.EODBeta1, GW2Builds.EODBeta4),
        new BuffOnActorDamageModifier(Mod_SepticCorruption, Blight, "Septic Corruption", "1% per blight stack", DamageSource.NoPets, 1.0, DamageType.Condition, DamageType.All, Source.Harbinger, ByStack, TraitImages.SepticCorruption, DamageModifierMode.All)
            .WithBuilds(GW2Builds.EODBeta1, GW2Builds.EODBeta4),
        new BuffOnActorDamageModifier(Mod_WickedCorruption, Blight, "Wicked Corruption", "0.5% per blight stack", DamageSource.NoPets, 0.5, DamageType.Strike, DamageType.All, Source.Harbinger, ByStack, TraitImages.WickedCorruption, DamageModifierMode.All)
            .WithBuilds(GW2Builds.EODBeta4, GW2Builds.March2024BalanceAndCerusLegendary),
        new BuffOnActorDamageModifier(Mod_WickedCorruption, Blight, "Wicked Corruption", "0.5% per blight stack", DamageSource.NoPets, 0.5, DamageType.Strike, DamageType.All, Source.Harbinger, ByStack, TraitImages.WickedCorruption, DamageModifierMode.sPvPWvW)
            .WithBuilds(GW2Builds.March2024BalanceAndCerusLegendary),
        new BuffOnActorDamageModifier(Mod_WickedCorruption, Blight, "Wicked Corruption", "1% per blight stack", DamageSource.NoPets, 1, DamageType.Strike, DamageType.All, Source.Harbinger, ByStack, TraitImages.WickedCorruption, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.March2024BalanceAndCerusLegendary),
        new BuffOnFoeDamageModifier(Mod_WickedCorruptionCrit, Torment, "Wicked Corruption (Crit)", "10% critical to foes with torment", DamageSource.NoPets, 10, DamageType.Strike, DamageType.All, Source.Harbinger, ByPresence, TraitImages.WickedCorruption, DamageModifierMode.All)
            .UsingChecker((evt, log) => evt.HasCrit)
            .WithBuilds(GW2Builds.EODRelease, GW2Builds.March2024BalanceAndCerusLegendary),
        new BuffOnFoeDamageModifier(Mod_WickedCorruptionCrit, Torment, "Wicked Corruption (Crit)", "10% critical to foes with torment", DamageSource.NoPets, 10, DamageType.Strike, DamageType.All, Source.Harbinger, ByPresence, TraitImages.WickedCorruption, DamageModifierMode.sPvPWvW)
            .UsingChecker((evt, log) => evt.HasCrit)
            .WithBuilds(GW2Builds.March2024BalanceAndCerusLegendary),
        new BuffOnFoeDamageModifier(Mod_WickedCorruptionCrit, Torment, "Wicked Corruption (Crit)", "12.5% critical to foes with torment", DamageSource.NoPets, 12.5, DamageType.Strike, DamageType.All, Source.Harbinger, ByPresence, TraitImages.WickedCorruption, DamageModifierMode.PvE)
            .UsingChecker((evt, log) => evt.HasCrit)
            .WithBuilds(GW2Builds.March2024BalanceAndCerusLegendary),
        new BuffOnActorDamageModifier(Mod_SepticCorruption, Blight, "Septic Corruption", "0.5% per blight stack", DamageSource.NoPets, 0.5, DamageType.Condition, DamageType.All, Source.Harbinger, ByStack, TraitImages.SepticCorruption, DamageModifierMode.All)
            .WithBuilds(GW2Builds.EODBeta4, GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
        new BuffOnActorDamageModifier(Mod_SepticCorruption, Blight, "Septic Corruption", "0.25% per blight stack", DamageSource.NoPets, 0.25, DamageType.Condition, DamageType.All, Source.Harbinger, ByStack, TraitImages.SepticCorruption, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
        new BuffOnActorDamageModifier(Mod_SepticCorruption, Blight, "Septic Corruption", "0.5% per blight stack", DamageSource.NoPets, 0.5, DamageType.Condition, DamageType.All, Source.Harbinger, ByStack, TraitImages.SepticCorruption, DamageModifierMode.sPvPWvW)
            .WithBuilds(GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers =
    [
        new BuffOnActorDamageModifier(Mod_ImplacableFoe, ImplacableFoe, "Implacable Foe", "-50%", DamageSource.Incoming, -50, DamageType.Strike, DamageType.All, Source.Harbinger, ByPresence, TraitImages.ImplacableFoe, DamageModifierMode.All),
    ];

    internal static readonly IReadOnlyList<Buff> Buffs =
    [
        new Buff("Harbinger Shroud", HarbingerShroud, Source.Harbinger, BuffClassification.Other, SkillImages.HarbingerShroud),
        new Buff("Blight", Blight, Source.Harbinger, BuffStackType.Stacking, 25, BuffClassification.Other, TraitImages.Blight),
        new Buff("Implacable Foe", ImplacableFoe, Source.Harbinger, BuffClassification.Other, TraitImages.ImplacableFoe),
    ];

    private static readonly HashSet<long> _harbingerShroudTransform =
    [
        EnterHarbingerShroud, ExitHarbingerShroud
    ];

    public static bool IsHarbingerShroudTransform(long id)
    {
        return _harbingerShroudTransform.Contains(id);
    }

    internal static void ComputeProfessionCombatReplayActors(PlayerActor player, ParsedEvtcLog log, CombatReplay replay)
    {
        Color color = Colors.Necromancer;

        // Vital Draw
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.HarbingerVitalDrawAoE, out var vitalDraws))
        {
            var skill = new SkillModeDescriptor(player, Spec.Harbinger, VitalDraw, SkillModeCategory.CC);
            foreach (EffectEvent effect in vitalDraws)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 3000);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, EffectImages.EffectVitalDraw);
            }
        }
    }
}
