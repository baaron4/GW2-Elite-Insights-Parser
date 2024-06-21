using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.EIData.ProfHelper;
using static GW2EIEvtcParser.EIData.SkillModeDescriptor;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class SpecterHelper
    {

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(EnterShadowShroud, ShadowShroud).UsingBeforeWeaponSwap(true), // Shadow Shroud Enter
            new BuffLossCastFinder(ExitShadowShroud, ShadowShroud).UsingBeforeWeaponSwap(true), // Shadow Shroud Exit
        };

        private static readonly HashSet<long> _shroudTransform = new HashSet<long>
        {
            EnterShadowShroud, ExitShadowShroud,
        };

        public static bool IsShroudTransform(long id)
        {
            return _shroudTransform.Contains(id);
        }

        internal static readonly List<DamageModifierDescriptor> OutgoingDamageModifiers = new List<DamageModifierDescriptor>
        {
        };

        internal static readonly List<DamageModifierDescriptor> IncomingDamageModifiers = new List<DamageModifierDescriptor>
        {
            new BuffOnActorDamageModifier(ShadowShroud, "Shadow Shroud", "-33%", DamageSource.NoPets, -33, DamageType.StrikeAndCondition, DamageType.All, Source.Specter, ByPresence, BuffImages.EnterShadowShroud, DamageModifierMode.PvE).WithBuilds(GW2Builds.November2022Balance),
        };


        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Shadow Shroud", ShadowShroud, Source.Specter, BuffClassification.Other, BuffImages.EnterShadowShroud),
            new Buff("Endless Night", EndlessNight, Source.Specter, BuffClassification.Other, BuffImages.EndlessNight),
            new Buff("Shrouded", Shrouded, Source.Specter, BuffClassification.Support, BuffImages.EnterShadowShroud),
            new Buff("Shrouded Ally", ShroudedAlly, Source.Specter, BuffClassification.Other, BuffImages.Siphon),
            new Buff("Rot Wallow Venom", RotWallowVenom, Source.Specter, BuffStackType.StackingConditionalLoss, 100, BuffClassification.Offensive, BuffImages.DarkSentry),
            new Buff("Consume Shadows", ConsumeShadows, Source.Specter, BuffStackType.StackingConditionalLoss, 5, BuffClassification.Other, BuffImages.ConsumeShadows),
        };

        private static HashSet<int> Minions = new HashSet<int>()
        {
            (int)MinionID.Specter1,
            (int)MinionID.Specter2,
            (int)MinionID.Specter3,
            (int)MinionID.Specter4,
            (int)MinionID.Specter5,
            (int)MinionID.Specter6,
            (int)MinionID.Specter7,
            (int)MinionID.Specter8,
            (int)MinionID.Specter9,
            (int)MinionID.Specter10,
        };

        internal static bool IsKnownMinionID(int id)
        {
            return Minions.Contains(id);
        }

        internal static void ComputeProfessionCombatReplayActors(AbstractPlayer player, ParsedEvtcLog log, CombatReplay replay)
        {
            Color color = Colors.Thief;

            // Well of Gloom
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.SpecterWellOfGloom4, out IReadOnlyList<EffectEvent> wellsOfGloom))
            {
                var skill = new SkillModeDescriptor(player, Spec.Specter, WellOfGloom, SkillModeCategory.Heal);
                foreach (EffectEvent effect in wellsOfGloom)
                {
                    (long, long) lifespan = effect.ComputeLifespan(log, 5000);
                    AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, ParserIcons.EffectWellOfGloom);
                }
            }
            // Well of Bounty
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.SpecterWellOfBounty2, out IReadOnlyList<EffectEvent> wellsOfBounty))
            {
                var skill = new SkillModeDescriptor(player, Spec.Specter, WellOfBounty, SkillModeCategory.ImportantBuffs);
                foreach (EffectEvent effect in wellsOfBounty)
                {
                    (long, long) lifespan = effect.ComputeLifespan(log, 5000);
                    AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, ParserIcons.EffectWellOfBounty);
                }
            }
            // Well of Tears
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.SpecterWellOfTears2, out IReadOnlyList<EffectEvent> wellsOfTears))
            {
                var skill = new SkillModeDescriptor(player, Spec.Specter, WellOfTears);
                foreach (EffectEvent effect in wellsOfTears)
                {
                    (long, long) lifespan = effect.ComputeLifespan(log, 5000);
                    AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, ParserIcons.EffectWellOfTears);
                }
            }
            // Well of Silence
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.SpecterWellOfSilence2, out IReadOnlyList<EffectEvent> wellsOfSilence))
            {
                var skill = new SkillModeDescriptor(player, Spec.Specter, WellOfSilence, SkillModeCategory.CC);
                foreach (EffectEvent effect in wellsOfSilence)
                {
                    (long, long) lifespan = effect.ComputeLifespan(log, 5000);
                    AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, ParserIcons.EffectWellOfSilence);
                }
            }
            // Well of Sorrow
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.SpecterWellOfSorrow2, out IReadOnlyList<EffectEvent> wellsOfSorrow))
            {
                var skill = new SkillModeDescriptor(player, Spec.Specter, WellOfSorrow);
                foreach (EffectEvent effect in wellsOfSorrow)
                {
                    (long, long) lifespan = effect.ComputeLifespan(log, 5000);
                    AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, ParserIcons.EffectWellOfSorrow);
                }
            }
            // Shadowfall
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.SpecterShadowfall2, out IReadOnlyList<EffectEvent> shadowfalls))
            {
                var skill = new SkillModeDescriptor(player, Spec.Specter, Shadowfall);
                foreach (EffectEvent effect in shadowfalls)
                {
                    (long, long) lifespan = effect.ComputeLifespan(log, 2250);
                    AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, ParserIcons.EffectShadowfall);
                }
            }
        }
    }
}
