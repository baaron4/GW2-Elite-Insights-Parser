using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.EIData.ProfHelper;
using static GW2EIEvtcParser.EIData.SkillModeDescriptor;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.DamageModifierIDs;

namespace GW2EIEvtcParser.EIData;

internal static class SpecterHelper
{

    internal static readonly List<InstantCastFinder> InstantCastFinder =
    [
        new BuffGainCastFinder(EnterShadowShroud, ShadowShroud).UsingBeforeWeaponSwap(true), // Shadow Shroud Enter
        new BuffLossCastFinder(ExitShadowShroud, ShadowShroud).UsingBeforeWeaponSwap(true), // Shadow Shroud Exit
    ];

    private static readonly HashSet<long> _shroudTransform =
    [
        EnterShadowShroud, ExitShadowShroud,
    ];

    public static bool IsShroudTransform(long id)
    {
        return _shroudTransform.Contains(id);
    }

    internal static readonly IReadOnlyList<DamageModifierDescriptor> OutgoingDamageModifiers = [];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers =
    [
        new BuffOnActorDamageModifier(Mod_ShadowShroud, ShadowShroud, "Shadow Shroud", "-33%", DamageSource.NoPets, -33, DamageType.StrikeAndCondition, DamageType.All, Source.Specter, ByPresence, SkillImages.EnterShadowShroud, DamageModifierMode.PvE).WithBuilds(GW2Builds.November2022Balance),
    ];


    internal static readonly IReadOnlyList<Buff> Buffs =
    [
        new Buff("Shadow Shroud", ShadowShroud, Source.Specter, BuffClassification.Other, SkillImages.EnterShadowShroud),
        new Buff("Endless Night", EndlessNight, Source.Specter, BuffClassification.Other, SkillImages.EndlessNight),
        new Buff("Shrouded", Shrouded, Source.Specter, BuffStackType.Stacking, 25, BuffClassification.Support, SkillImages.EnterShadowShroud),
        new Buff("Shrouded Ally", ShroudedAlly, Source.Specter, BuffClassification.Other, SkillImages.Siphon),
        new Buff("Rot Wallow Venom", RotWallowVenom, Source.Specter, BuffStackType.StackingConditionalLoss, 100, BuffClassification.Offensive, TraitImages.DarkSentry),
        new Buff("Consume Shadows", ConsumeShadows, Source.Specter, BuffStackType.StackingConditionalLoss, 5, BuffClassification.Other, TraitImages.ConsumeShadows),
    ];

    private static HashSet<int> Minions =
    [
        (int)MinionID.SpecterAsura1,
        (int)MinionID.SpecterHuman1,
        (int)MinionID.SpecterAsura2,
        (int)MinionID.SpecterSylvari1,
        (int)MinionID.SpecterHuman2,
        (int)MinionID.SpecterNorn1,
        (int)MinionID.SpecterCharr1,
        (int)MinionID.SpecterSylvari2,
        (int)MinionID.SpecterCharr2,
        (int)MinionID.SpecterNorn2,
    ];

    internal static bool IsKnownMinionID(int id)
    {
        return Minions.Contains(id);
    }

    internal static void ComputeProfessionCombatReplayActors(PlayerActor player, ParsedEvtcLog log, CombatReplay replay)
    {
        Color color = Colors.Thief;

        // Well of Gloom
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.SpecterWellOfGloom4, out var wellsOfGloom))
        {
            var skill = new SkillModeDescriptor(player, Spec.Specter, WellOfGloom, SkillModeCategory.Heal);
            foreach (EffectEvent effect in wellsOfGloom)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 5000);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, EffectImages.EffectWellOfGloom);
            }
        }
        // Well of Bounty
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.SpecterWellOfBounty2, out var wellsOfBounty))
        {
            var skill = new SkillModeDescriptor(player, Spec.Specter, WellOfBounty, SkillModeCategory.ImportantBuffs);
            foreach (EffectEvent effect in wellsOfBounty)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 5000);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, EffectImages.EffectWellOfBounty);
            }
        }
        // Well of Tears
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.SpecterWellOfTears2, out var wellsOfTears))
        {
            var skill = new SkillModeDescriptor(player, Spec.Specter, WellOfTears);
            foreach (EffectEvent effect in wellsOfTears)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 5000);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, EffectImages.EffectWellOfTears);
            }
        }
        // Well of Silence
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.SpecterWellOfSilence2, out var wellsOfSilence))
        {
            var skill = new SkillModeDescriptor(player, Spec.Specter, WellOfSilence, SkillModeCategory.CC);
            foreach (EffectEvent effect in wellsOfSilence)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 5000);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, EffectImages.EffectWellOfSilence);
            }
        }
        // Well of Sorrow
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.SpecterWellOfSorrow2, out var wellsOfSorrow))
        {
            var skill = new SkillModeDescriptor(player, Spec.Specter, WellOfSorrow);
            foreach (EffectEvent effect in wellsOfSorrow)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 5000);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, EffectImages.EffectWellOfSorrow);
            }
        }
        // Shadowfall
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.SpecterShadowfall2, out var shadowfalls))
        {
            var skill = new SkillModeDescriptor(player, Spec.Specter, Shadowfall);
            foreach (EffectEvent effect in shadowfalls)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 2250);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, EffectImages.EffectShadowfall);
            }
        }
    }
}
