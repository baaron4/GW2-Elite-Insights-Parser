using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData.Buffs;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
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

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
        };


        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Shadow Shroud", ShadowShroud, Source.Specter, BuffClassification.Other, BuffImages.EnterShadowShroud),
            new Buff("Endless Night", EndlessNight, Source.Specter, BuffClassification.Other, BuffImages.EndlessNight),
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
                    (long, long) lifespan = ProfHelper.ComputeEffectLifespan(log, effect, 5000);
                    var connector = new PositionConnector(effect.Position);
                    replay.Decorations.Add(new CircleDecoration(240, lifespan, color.WithAlpha(0.5f).ToString(), connector).UsingFilled(false).UsingSkillMode(skill));
                    replay.Decorations.Add(new IconDecoration(ParserIcons.EffectWellOfGloom, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector).UsingSkillMode(skill));
                }
            }
            // Well of Bounty
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.SpecterWellOfBounty2, out IReadOnlyList<EffectEvent> wellsOfBounty))
            {
                var skill = new SkillModeDescriptor(player, Spec.Specter, WellOfBounty);
                foreach (EffectEvent effect in wellsOfBounty)
                {
                    (long, long) lifespan = ProfHelper.ComputeEffectLifespan(log, effect, 5000);
                    var connector = new PositionConnector(effect.Position);
                    replay.Decorations.Add(new CircleDecoration(240, lifespan, color.WithAlpha(0.5f).ToString(), connector).UsingFilled(false).UsingSkillMode(skill));
                    replay.Decorations.Add(new IconDecoration(ParserIcons.EffectWellOfBounty, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector).UsingSkillMode(skill));
                }
            }
            // Well of Tears
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.SpecterWellOfTears2, out IReadOnlyList<EffectEvent> wellsOfTears))
            {
                var skill = new SkillModeDescriptor(player, Spec.Specter, WellOfTears);
                foreach (EffectEvent effect in wellsOfTears)
                {
                    (long, long) lifespan = ProfHelper.ComputeEffectLifespan(log, effect, 5000);
                    var connector = new PositionConnector(effect.Position);
                    replay.Decorations.Add(new CircleDecoration(240, lifespan, color.WithAlpha(0.5f).ToString(), connector).UsingFilled(false).UsingSkillMode(skill));
                    replay.Decorations.Add(new IconDecoration(ParserIcons.EffectWellOfTears, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector).UsingSkillMode(skill));
                }
            }
            // Well of Silence
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.SpecterWellOfSilence2, out IReadOnlyList<EffectEvent> wellsOfSilence))
            {
                var skill = new SkillModeDescriptor(player, Spec.Specter, WellOfSilence);
                foreach (EffectEvent effect in wellsOfSilence)
                {
                    (long, long) lifespan = ProfHelper.ComputeEffectLifespan(log, effect, 5000);
                    var connector = new PositionConnector(effect.Position);
                    replay.Decorations.Add(new CircleDecoration(240, lifespan, color.WithAlpha(0.5f).ToString(), connector).UsingFilled(false).UsingSkillMode(skill));
                    replay.Decorations.Add(new IconDecoration(ParserIcons.EffectWellOfSilence, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector).UsingSkillMode(skill));
                }
            }
            // Well of Sorrow
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.SpecterWellOfSorrow2, out IReadOnlyList<EffectEvent> wellsOfSorrow))
            {
                var skill = new SkillModeDescriptor(player, Spec.Specter, WellOfSorrow);
                foreach (EffectEvent effect in wellsOfSorrow)
                {
                    (long, long) lifespan = ProfHelper.ComputeEffectLifespan(log, effect, 5000);
                    var connector = new PositionConnector(effect.Position);
                    replay.Decorations.Add(new CircleDecoration(240, lifespan, color.WithAlpha(0.5f).ToString(), connector).UsingFilled(false).UsingSkillMode(skill));
                    replay.Decorations.Add(new IconDecoration(ParserIcons.EffectWellOfSorrow, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector).UsingSkillMode(skill));
                }
            }
            // Shadowfall
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.SpecterShadowfall2, out IReadOnlyList<EffectEvent> shadowfalls))
            {
                var skill = new SkillModeDescriptor(player, Spec.Specter, Shadowfall);
                foreach (EffectEvent effect in shadowfalls)
                {
                    (long, long) lifespan = ProfHelper.ComputeEffectLifespan(log, effect, 2250);
                    var connector = new PositionConnector(effect.Position);
                    replay.Decorations.Add(new CircleDecoration(240, lifespan, color.WithAlpha(0.5f).ToString(), connector).UsingFilled(false).UsingSkillMode(skill));
                    replay.Decorations.Add(new IconDecoration(ParserIcons.EffectShadowfall, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector).UsingSkillMode(skill));
                }
            }
        }
    }
}
