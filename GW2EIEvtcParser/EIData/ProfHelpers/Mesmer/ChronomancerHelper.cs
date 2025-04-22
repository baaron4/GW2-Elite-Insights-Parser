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

internal static class ChronomancerHelper
{
    internal static readonly List<InstantCastFinder> InstantCastFinder =
    [
        new BuffGainCastFinder(ContinuumSplit, TimeAnchored),
        new BuffLossCastFinder(ContinuumShift, TimeAnchored),
        new EffectCastFinder(SplitSecond, EffectGUIDs.ChronomancerSplitSecond)
            .UsingSecondaryEffectChecker(EffectGUIDs.ChronomancerSeizeTheMomentShatter)
            .UsingSrcSpecChecker(Spec.Chronomancer),
        new EffectCastFinder(Rewinder, EffectGUIDs.ChronomancerRewinder)
            .UsingSecondaryEffectChecker(EffectGUIDs.ChronomancerSeizeTheMomentShatter)
            .UsingSrcSpecChecker(Spec.Chronomancer),
        new EffectCastFinder(TimeSink, EffectGUIDs.ChronomancerTimeSink)
            .UsingSecondaryEffectChecker(EffectGUIDs.ChronomancerSeizeTheMomentShatter)
            .UsingSrcSpecChecker(Spec.Chronomancer),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> OutgoingDamageModifiers =
    [
        new BuffOnFoeDamageModifier(Mod_DangerTime, Slow, "Danger Time", "10% crit damage on slowed target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Chronomancer, ByPresence, TraitImages.DangerTime, DamageModifierMode.All)
            .UsingChecker((x, log) => x.HasCrit)
            .WithBuilds(GW2Builds.February2018Balance, GW2Builds.December2018Balance),
        new BuffOnFoeDamageModifier(Mod_DangerTime, Slow, "Danger Time", "10% crit damage on slowed target", DamageSource.All, 10.0, DamageType.Strike, DamageType.All, Source.Chronomancer, ByPresence, TraitImages.DangerTime, DamageModifierMode.All)
            .UsingChecker((x, log) => x.HasCrit)
            .WithBuilds(GW2Builds.December2018Balance, GW2Builds.May2021Balance),
        new BuffOnActorDamageModifier(Mod_ImprovedAlacrity, Alacrity, "Improved Alacrity", "10% crit under alacrity", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Chronomancer, ByPresence, TraitImages.ImprovedAlacrity, DamageModifierMode.All)
            .UsingChecker((x, log) => x.HasCrit)
            .WithBuilds(GW2Builds.August2022BalanceHotFix),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers = [];


    internal static readonly IReadOnlyList<Buff> Buffs =
    [
        new Buff("Time Echo", TimeEcho, Source.Chronomancer, BuffClassification.Other, SkillImages.DejaVu)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
        new Buff("Time Anchored", TimeAnchored, Source.Chronomancer, BuffStackType.Queue, 25, BuffClassification.Other, SkillImages.ContinuumSplit),
        new Buff("Temporal Stasis", TemporalStasis, Source.Chronomancer, BuffClassification.Other, BuffImages.Stun),
    ];

    private static readonly HashSet<int> NonCloneMinions = [];
    internal static bool IsKnownMinionID(int id)
    {
        return NonCloneMinions.Contains(id);
    }

    internal static void ComputeProfessionCombatReplayActors(PlayerActor player, ParsedEvtcLog log, CombatReplay replay)
    {
        Color color = Colors.Mesmer;

        // Well of Eternity
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ChronomancerWellOfEternity, out var wellsOfEternity))
        {
            var skill = new SkillModeDescriptor(player, Spec.Chronomancer, WellOfEternity, SkillModeCategory.Heal);
            foreach (EffectEvent effect in wellsOfEternity)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 3000);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, EffectImages.EffectWellOfEternity);
            }
        }
        // Well of Eternity - Pulses
        if (log.CombatData.TryGetEffectEventsBySrcWithGUIDs(player.AgentItem, [ EffectGUIDs.ChronomancerWellOfEternityPulse, EffectGUIDs.ChronomancerWellOfEternityExplosion ], out var wellsOfEternityPulses))
        {
            var skill = new SkillModeDescriptor(player, Spec.Chronomancer, WellOfEternity, SkillModeCategory.Heal);
            foreach (EffectEvent effect in wellsOfEternityPulses)
            {
                int effectTimeStart = (int)effect.Time;
                int effectTimeEnd = effectTimeStart + 1000;
                var connector = new PositionConnector(effect.Position);
                replay.Decorations.Add(new CircleDecoration(240, (effectTimeStart, effectTimeEnd), color, 0.5, connector).UsingFilled(false).UsingGrowingEnd(effectTimeEnd).UsingSkillMode(skill));
            }
        }

        // Well of Action
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ChronomancerWellOfAction, out var wellsOfAction))
        {
            var skill = new SkillModeDescriptor(player, Spec.Chronomancer, WellOfAction);
            foreach (EffectEvent effect in wellsOfAction)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 3000);
                var connector = new PositionConnector(effect.Position);
                replay.Decorations.Add(new CircleDecoration(240, lifespan, color, 0.5, connector).UsingFilled(false).UsingSkillMode(skill));
                replay.Decorations.Add(new IconDecoration(EffectImages.EffectWellOfAction, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector).UsingSkillMode(skill));

                // Well pulses - Hard coded because the effects don't have a Src
                int pulseTimeDelay = 0;
                for (int i = 0; i < 4; i++)
                {
                    int effectTimeStart = (int)effect.Time + pulseTimeDelay;
                    int effectTimeEnd = effectTimeStart + 300;
                    if (effectTimeStart > lifespan.Item2) { break; }
                    var circle = (CircleDecoration)new CircleDecoration(240, (effectTimeStart, effectTimeEnd), color, 0.5, connector).UsingFilled(false).UsingSkillMode(skill);
                    if (i < 3)
                    {
                        // Pulse inwards
                        replay.Decorations.Add(circle.UsingGrowingEnd(effectTimeEnd, true));
                    }
                    else
                    {
                        // Final pulse outwards
                        replay.Decorations.Add(circle.UsingGrowingEnd(effectTimeEnd));
                    }
                    pulseTimeDelay += 1000;
                }
            }
        }

        // Well of Calamity
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ChronomancerWellOfCalamity, out var wellsOfCalamity))
        {
            var skill = new SkillModeDescriptor(player, Spec.Chronomancer, WellOfCalamity);
            foreach (EffectEvent effect in wellsOfCalamity)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 3000);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, EffectImages.EffectWellOfCalamity);
            }
        }
        // Well of Calamity - Pulses
        if (log.CombatData.TryGetEffectEventsBySrcWithGUIDs(player.AgentItem, [EffectGUIDs.ChronomancerWellOfCalamityPulse, EffectGUIDs.ChronomancerWellOfCalamityExplosion], out var wellsOfCalamityPulses))
        {
            var skill = new SkillModeDescriptor(player, Spec.Chronomancer, WellOfCalamity);
            foreach (EffectEvent effect in wellsOfCalamityPulses)
            {
                int effectTimeStart = (int)effect.Time;
                int effectTimeEnd = effectTimeStart + 1000;
                var connector = new PositionConnector(effect.Position);
                replay.Decorations.Add(new CircleDecoration(240, (effectTimeStart, effectTimeEnd), color, 0.5, connector).UsingFilled(false).UsingGrowingEnd(effectTimeEnd, true).UsingSkillMode(skill));
            }
        }

        // Well of Precognition
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ChronomancerWellOfPrecognition, out var wellsOfPrecognition))
        {
            var skill = new SkillModeDescriptor(player, Spec.Chronomancer, WellOfPrecognition, SkillModeCategory.ImportantBuffs);
            foreach (EffectEvent effect in wellsOfPrecognition)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 3000);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, EffectImages.EffectWellOfPrecognition);
            }
        }
        // Well of Precognition - Pulses
        if (log.CombatData.TryGetEffectEventsBySrcWithGUIDs(player.AgentItem, [EffectGUIDs.ChronomancerWellOfPrecognitionPulse, EffectGUIDs.ChronomancerWellOfPrecognitionExplosion], out var wellsOfPrecognitionPulses))
        {
            var skill = new SkillModeDescriptor(player, Spec.Chronomancer, WellOfPrecognition, SkillModeCategory.ImportantBuffs);
            foreach (EffectEvent effect in wellsOfPrecognitionPulses)
            {
                int effectTimeStart = (int)effect.Time;
                int effectTimeEnd = effectTimeStart + 1000;
                var connector = new PositionConnector(effect.Position);
                replay.Decorations.Add(new CircleDecoration(240, (effectTimeStart, effectTimeEnd), color, 0.5, connector).UsingFilled(false).UsingGrowingEnd(effectTimeEnd).UsingSkillMode(skill));
            }
        }

        // Well of Senility
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ChronomancerWellOfSenility, out var wellsOfSenility))
        {
            var skill = new SkillModeDescriptor(player, Spec.Chronomancer, WellOfRecall_Senility);
            foreach (EffectEvent effect in wellsOfSenility)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 3000);
                var connector = new PositionConnector(effect.Position);
                replay.Decorations.Add(new CircleDecoration(240, lifespan, color, 0.5, connector).UsingFilled(false).UsingSkillMode(skill));
                replay.Decorations.Add(new IconDecoration(EffectImages.EffectWellOfSenility, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector).UsingSkillMode(skill));

                // Well pulses - Hard coded because the effects don't have a Src
                int pulseTimeDelay = 0;
                for (int i = 0; i < 4; i++)
                {
                    int effectTimeStart = (int)effect.Time + pulseTimeDelay;
                    int effectTimeEnd = effectTimeStart + 300;
                    if (effectTimeStart > lifespan.Item2) { break; }
                    var circle = (CircleDecoration)new CircleDecoration(240, (effectTimeStart, effectTimeEnd), color, 0.5, connector).UsingFilled(false).UsingSkillMode(skill);
                    if (i < 3)
                    {
                        // Pulse inwards
                        replay.Decorations.Add(circle.UsingGrowingEnd(effectTimeEnd, true));
                    }
                    else
                    {
                        // Final pulse outwards
                        replay.Decorations.Add(circle.UsingGrowingEnd(effectTimeEnd));
                    }
                    pulseTimeDelay += 1000;
                }
            }
        }

        // Gravity Well
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ChronomancerGravityWell, out var gravityWells))
        {
            var skill = new SkillModeDescriptor(player, Spec.Chronomancer, GravityWell, SkillModeCategory.CC);
            foreach (EffectEvent effect in gravityWells)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 3000);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, EffectImages.EffectGravityWell);
            }
        }
        // Gravity Well - Pulses
        if (log.CombatData.TryGetEffectEventsBySrcWithGUIDs(player.AgentItem, [EffectGUIDs.ChronomancerGravityWellPulse, EffectGUIDs.ChronomancerGravityWellExplosion], out var gravityWellPulses))
        {
            var skill = new SkillModeDescriptor(player, Spec.Chronomancer, GravityWell, SkillModeCategory.CC);
            foreach (EffectEvent effect in gravityWellPulses)
            {
                int effectTimeStart = (int)effect.Time;
                int effectTimeEnd = effectTimeStart + 1000;
                var connector = new PositionConnector(effect.Position);
                replay.Decorations.Add(new CircleDecoration(240, (effectTimeStart, effectTimeEnd), color, 0.5, connector).UsingFilled(false).UsingGrowingEnd(effectTimeEnd, true).UsingSkillMode(skill));
            }
        }
    }
}
