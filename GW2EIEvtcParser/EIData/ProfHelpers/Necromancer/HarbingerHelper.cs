using System.Collections.Generic;
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
    internal static class HarbingerHelper
    {

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(EnterHarbingerShroud, HarbingerShroud).UsingBeforeWeaponSwap(true),
            new BuffLossCastFinder(ExitHarbingerShroud, HarbingerShroud).UsingBeforeWeaponSwap(true),
            new DamageCastFinder(CascadingCorruption, CascadingCorruption).UsingDisableWithEffectData().UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
            new EffectCastFinderByDst(CascadingCorruption, EffectGUIDs.HarbingerCascadingCorruption).UsingDstSpecChecker(Spec.Harbinger).UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
            new EffectCastFinderByDst(DeathlyHaste, EffectGUIDs.HarbingerDeathlyHaste).UsingDstSpecChecker(Spec.Harbinger).UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
            new EffectCastFinderByDst(DoomApproaches, EffectGUIDs.HarbingerDoomApproaches).UsingDstSpecChecker(Spec.Harbinger).UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
        };


        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifier(Blight, "Wicked Corruption", "1% per blight stack", DamageSource.NoPets, 1.0, DamageType.Strike, DamageType.All, Source.Harbinger, ByStack, BuffImages.WickedCorruption, DamageModifierMode.All).WithBuilds(GW2Builds.EODBeta1, GW2Builds.EODBeta4),
            new BuffDamageModifier(Blight, "Septic Corruption", "1% per blight stack", DamageSource.NoPets, 1.0, DamageType.Condition, DamageType.All, Source.Harbinger, ByStack, BuffImages.SepticCorruption, DamageModifierMode.All).WithBuilds(GW2Builds.EODBeta1, GW2Builds.EODBeta4),
            new BuffDamageModifier(Blight, "Wicked Corruption", "0.5% per blight stack", DamageSource.NoPets, 0.5, DamageType.Strike, DamageType.All, Source.Harbinger, ByStack, BuffImages.WickedCorruption, DamageModifierMode.All).WithBuilds(GW2Builds.EODBeta4),
            new BuffDamageModifier(Blight, "Septic Corruption", "0.5% per blight stack", DamageSource.NoPets, 0.5, DamageType.Condition, DamageType.All, Source.Harbinger, ByStack, BuffImages.SepticCorruption, DamageModifierMode.All).WithBuilds(GW2Builds.EODBeta4, GW2Builds.June2023Balance),
            new BuffDamageModifier(Blight, "Septic Corruption", "0.25% per blight stack", DamageSource.NoPets, 0.25, DamageType.Condition, DamageType.All, Source.Harbinger, ByStack, BuffImages.SepticCorruption, DamageModifierMode.PvE).WithBuilds(GW2Builds.June2023Balance),
            new BuffDamageModifier(Blight, "Septic Corruption", "0.5% per blight stack", DamageSource.NoPets, 0.5, DamageType.Condition, DamageType.All, Source.Harbinger, ByStack, BuffImages.SepticCorruption, DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.June2023Balance),
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Harbinger Shroud", HarbingerShroud, Source.Harbinger, BuffClassification.Other, BuffImages.HarbingerShroud),
            new Buff("Blight", Blight, Source.Harbinger, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.Blight),
        };

        private static readonly HashSet<long> _harbingerShroudTransform = new HashSet<long>
        {
            EnterHarbingerShroud, ExitHarbingerShroud
        };

        public static bool IsHarbingerShroudTransform(long id)
        {
            return _harbingerShroudTransform.Contains(id);
        }

        internal static void ComputeProfessionCombatReplayActors(AbstractPlayer player, ParsedEvtcLog log, CombatReplay replay)
        {
            Color color = Colors.Necromancer;

            // Vital Draw
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.HarbingerVitalDrawAoE, out IReadOnlyList<EffectEvent> vitalDraws))
            {
                var skill = new SkillModeDescriptor(player, Spec.Harbinger, VitalDraw);
                foreach (EffectEvent effect in vitalDraws)
                {
                    (long, long) lifespan = ProfHelper.ComputeEffectLifespan(log, effect, 3000);
                    var connector = new PositionConnector(effect.Position);
                    replay.Decorations.Add(new CircleDecoration(240, lifespan, color.WithAlpha(0.5f).ToString(), connector).UsingFilled(false).UsingSkillMode(skill));
                    replay.Decorations.Add(new IconDecoration(ParserIcons.EffectVitalDraw, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector).UsingSkillMode(skill));
                }
            }
        }
    }
}
