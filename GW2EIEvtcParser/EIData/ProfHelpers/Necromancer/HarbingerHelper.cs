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


        internal static readonly List<DamageModifierDescriptor> OutgoingDamageModifiers = new List<DamageModifierDescriptor>
        {
            new BuffOnActorDamageModifier(Blight, "Wicked Corruption", "1% per blight stack", DamageSource.NoPets, 1.0, DamageType.Strike, DamageType.All, Source.Harbinger, ByStack, BuffImages.WickedCorruption, DamageModifierMode.All).WithBuilds(GW2Builds.EODBeta1, GW2Builds.EODBeta4),
            new BuffOnActorDamageModifier(Blight, "Septic Corruption", "1% per blight stack", DamageSource.NoPets, 1.0, DamageType.Condition, DamageType.All, Source.Harbinger, ByStack, BuffImages.SepticCorruption, DamageModifierMode.All).WithBuilds(GW2Builds.EODBeta1, GW2Builds.EODBeta4),
            new BuffOnActorDamageModifier(Blight, "Wicked Corruption", "0.5% per blight stack", DamageSource.NoPets, 0.5, DamageType.Strike, DamageType.All, Source.Harbinger, ByStack, BuffImages.WickedCorruption, DamageModifierMode.All).WithBuilds(GW2Builds.EODBeta4, GW2Builds.March2024BalanceAndCerusLegendary),
            new BuffOnActorDamageModifier(Blight, "Wicked Corruption", "0.5% per blight stack", DamageSource.NoPets, 0.5, DamageType.Strike, DamageType.All, Source.Harbinger, ByStack, BuffImages.WickedCorruption, DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.March2024BalanceAndCerusLegendary),
            new BuffOnActorDamageModifier(Blight, "Wicked Corruption", "1% per blight stack", DamageSource.NoPets, 1, DamageType.Strike, DamageType.All, Source.Harbinger, ByStack, BuffImages.WickedCorruption, DamageModifierMode.PvE).WithBuilds(GW2Builds.March2024BalanceAndCerusLegendary),
            new BuffOnFoeDamageModifier(Torment, "Wicked Corruption (Crit)", "10% critical to foes with torment", DamageSource.NoPets, 10, DamageType.Strike, DamageType.All, Source.Harbinger, ByPresence, BuffImages.WickedCorruption, DamageModifierMode.All).UsingChecker((evt, log) => evt.HasCrit).WithBuilds(GW2Builds.EODRelease, GW2Builds.March2024BalanceAndCerusLegendary),
            new BuffOnFoeDamageModifier(Torment, "Wicked Corruption (Crit)", "10% critical to foes with torment", DamageSource.NoPets, 10, DamageType.Strike, DamageType.All, Source.Harbinger, ByPresence, BuffImages.WickedCorruption, DamageModifierMode.sPvPWvW).UsingChecker((evt, log) => evt.HasCrit).WithBuilds(GW2Builds.March2024BalanceAndCerusLegendary),
            new BuffOnFoeDamageModifier(Torment, "Wicked Corruption (Crit)", "12.5% critical to foes with torment", DamageSource.NoPets, 12.5, DamageType.Strike, DamageType.All, Source.Harbinger, ByPresence, BuffImages.WickedCorruption, DamageModifierMode.PvE).UsingChecker((evt, log) => evt.HasCrit).WithBuilds(GW2Builds.March2024BalanceAndCerusLegendary),
            new BuffOnActorDamageModifier(Blight, "Septic Corruption", "0.5% per blight stack", DamageSource.NoPets, 0.5, DamageType.Condition, DamageType.All, Source.Harbinger, ByStack, BuffImages.SepticCorruption, DamageModifierMode.All).WithBuilds(GW2Builds.EODBeta4, GW2Builds.June2023Balance),
            new BuffOnActorDamageModifier(Blight, "Septic Corruption", "0.25% per blight stack", DamageSource.NoPets, 0.25, DamageType.Condition, DamageType.All, Source.Harbinger, ByStack, BuffImages.SepticCorruption, DamageModifierMode.PvE).WithBuilds(GW2Builds.June2023Balance),
            new BuffOnActorDamageModifier(Blight, "Septic Corruption", "0.5% per blight stack", DamageSource.NoPets, 0.5, DamageType.Condition, DamageType.All, Source.Harbinger, ByStack, BuffImages.SepticCorruption, DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.June2023Balance),
        };

        internal static readonly List<DamageModifierDescriptor> IncomingDamageModifiers = new List<DamageModifierDescriptor>
        {
            // Confirm if applied strike and condition
            new BuffOnActorDamageModifier(ImplacableFoe, "Implacable Foe", "-50%", DamageSource.NoPets, -50, DamageType.StrikeAndCondition, DamageType.All, Source.Harbinger, ByPresence, BuffImages.ImplacableFoe, DamageModifierMode.All),
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Harbinger Shroud", HarbingerShroud, Source.Harbinger, BuffClassification.Other, BuffImages.HarbingerShroud),
            new Buff("Blight", Blight, Source.Harbinger, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.Blight),
            new Buff("Implacable Foe", ImplacableFoe, Source.Harbinger, BuffClassification.Other, BuffImages.ImplacableFoe),
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
                var skill = new SkillModeDescriptor(player, Spec.Harbinger, VitalDraw, SkillModeCategory.CC);
                foreach (EffectEvent effect in vitalDraws)
                {
                    (long, long) lifespan = effect.ComputeLifespan(log, 3000);
                    AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, ParserIcons.EffectVitalDraw);
                }
            }
        }
    }
}
