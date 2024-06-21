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
    internal static class ChronomancerHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(ContinuumSplit, TimeAnchored), // Continuum Split
            new BuffLossCastFinder(ContinuumShift, TimeAnchored), // Continuum Shift
            new EffectCastFinder(SplitSecond, EffectGUIDs.ChronomancerSplitSecond).UsingSrcSpecChecker(Spec.Chronomancer)
                .UsingSecondaryEffectChecker(EffectGUIDs.ChronomancerSeizeTheMomentShatter),
            new EffectCastFinder(Rewinder, EffectGUIDs.ChronomancerRewinder).UsingSrcSpecChecker(Spec.Chronomancer)
                .UsingSecondaryEffectChecker(EffectGUIDs.ChronomancerSeizeTheMomentShatter),
            new EffectCastFinder(TimeSink, EffectGUIDs.ChronomancerTimeSink).UsingSrcSpecChecker(Spec.Chronomancer)
                .UsingSecondaryEffectChecker(EffectGUIDs.ChronomancerSeizeTheMomentShatter),
        };

        internal static readonly List<DamageModifierDescriptor> OutgoingDamageModifiers = new List<DamageModifierDescriptor>
        {
            new BuffOnFoeDamageModifier(Slow, "Danger Time", "10% crit damage on slowed target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Chronomancer, ByPresence, BuffImages.DangerTime, DamageModifierMode.All).UsingChecker((x, log) => x.HasCrit).WithBuilds(GW2Builds.February2018Balance, GW2Builds.December2018Balance),
            new BuffOnFoeDamageModifier(Slow, "Danger Time", "10% crit damage on slowed target", DamageSource.All, 10.0, DamageType.Strike, DamageType.All, Source.Chronomancer, ByPresence, BuffImages.DangerTime, DamageModifierMode.All).UsingChecker((x, log) => x.HasCrit).WithBuilds(GW2Builds.December2018Balance, GW2Builds.May2021Balance),
            new BuffOnActorDamageModifier(Alacrity, "Improved Alacrity", "10% crit under alacrity", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Chronomancer, ByPresence, BuffImages.ImprovedAlacrity, DamageModifierMode.All).UsingChecker((x, log) => x.HasCrit).WithBuilds(GW2Builds.August2022BalanceHotFix),
        };

        internal static readonly List<DamageModifierDescriptor> IncomingDamageModifiers = new List<DamageModifierDescriptor>
        {
        };


        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Time Echo", TimeEcho, Source.Chronomancer, BuffClassification.Other, BuffImages.DejaVu).WithBuilds(GW2Builds.StartOfLife, GW2Builds.SOTOBetaAndSilentSurfNM),
            new Buff("Time Anchored", TimeAnchored, Source.Chronomancer, BuffStackType.Queue, 25, BuffClassification.Other, BuffImages.ContinuumSplit),
            new Buff("Temporal Stasis", TemporalStasis, Source.Chronomancer, BuffClassification.Other, BuffImages.Stun),
        };

        private static HashSet<int> NonCloneMinions = new HashSet<int>();
        internal static bool IsKnownMinionID(int id)
        {
            return NonCloneMinions.Contains(id);
        }

        internal static void ComputeProfessionCombatReplayActors(AbstractPlayer player, ParsedEvtcLog log, CombatReplay replay)
        {
            Color color = Colors.Mesmer;

            // Well of Eternity
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ChronomancerWellOfEternity, out IReadOnlyList<EffectEvent> wellsOfEternity))
            {
                var skill = new SkillModeDescriptor(player, Spec.Chronomancer, WellOfEternity, SkillModeCategory.Heal);
                foreach (EffectEvent effect in wellsOfEternity)
                {
                    (long, long) lifespan = effect.ComputeLifespan(log, 3000);
                    AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, ParserIcons.EffectWellOfEternity);
                }
            }
            // Well of Eternity - Pulses
            if (log.CombatData.TryGetEffectEventsBySrcWithGUIDs(player.AgentItem, new string[] { EffectGUIDs.ChronomancerWellOfEternityPulse, EffectGUIDs.ChronomancerWellOfEternityExplosion }, out IReadOnlyList<EffectEvent> wellsOfEternityPulses))
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
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ChronomancerWellOfAction, out IReadOnlyList<EffectEvent> wellsOfAction))
            {
                var skill = new SkillModeDescriptor(player, Spec.Chronomancer, WellOfAction);
                foreach (EffectEvent effect in wellsOfAction)
                {
                    (long, long) lifespan = effect.ComputeLifespan(log, 3000);
                    var connector = new PositionConnector(effect.Position);
                    replay.Decorations.Add(new CircleDecoration(240, lifespan, color, 0.5, connector).UsingFilled(false).UsingSkillMode(skill));
                    replay.Decorations.Add(new IconDecoration(ParserIcons.EffectWellOfAction, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector).UsingSkillMode(skill));

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
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ChronomancerWellOfCalamity, out IReadOnlyList<EffectEvent> wellsOfCalamity))
            {
                var skill = new SkillModeDescriptor(player, Spec.Chronomancer, WellOfCalamity);
                foreach (EffectEvent effect in wellsOfCalamity)
                {
                    (long, long) lifespan = effect.ComputeLifespan(log, 3000);
                    AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, ParserIcons.EffectWellOfCalamity);
                }
            }
            // Well of Calamity - Pulses
            if (log.CombatData.TryGetEffectEventsBySrcWithGUIDs(player.AgentItem, new string[] { EffectGUIDs.ChronomancerWellOfCalamityPulse, EffectGUIDs.ChronomancerWellOfCalamityExplosion }, out IReadOnlyList<EffectEvent> wellsOfCalamityPulses))
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
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ChronomancerWellOfPrecognition, out IReadOnlyList<EffectEvent> wellsOfPrecognition))
            {
                var skill = new SkillModeDescriptor(player, Spec.Chronomancer, WellOfPrecognition, SkillModeCategory.ImportantBuffs);
                foreach (EffectEvent effect in wellsOfPrecognition)
                {
                    (long, long) lifespan = effect.ComputeLifespan(log, 3000);
                    AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, ParserIcons.EffectWellOfPrecognition);
                }
            }
            // Well of Precognition - Pulses
            if (log.CombatData.TryGetEffectEventsBySrcWithGUIDs(player.AgentItem, new string[] { EffectGUIDs.ChronomancerWellOfPrecognitionPulse, EffectGUIDs.ChronomancerWellOfPrecognitionExplosion }, out IReadOnlyList<EffectEvent> wellsOfPrecognitionPulses))
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
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ChronomancerWellOfSenility, out IReadOnlyList<EffectEvent> wellsOfSenility))
            {
                var skill = new SkillModeDescriptor(player, Spec.Chronomancer, WellOfRecall_Senility);
                foreach (EffectEvent effect in wellsOfSenility)
                {
                    (long, long) lifespan = effect.ComputeLifespan(log, 3000);
                    var connector = new PositionConnector(effect.Position);
                    replay.Decorations.Add(new CircleDecoration(240, lifespan, color, 0.5, connector).UsingFilled(false).UsingSkillMode(skill));
                    replay.Decorations.Add(new IconDecoration(ParserIcons.EffectWellOfSenility, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector).UsingSkillMode(skill));

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
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ChronomancerGravityWell, out IReadOnlyList<EffectEvent> gravityWells))
            {
                var skill = new SkillModeDescriptor(player, Spec.Chronomancer, GravityWell, SkillModeCategory.CC);
                foreach (EffectEvent effect in gravityWells)
                {
                    (long, long) lifespan = effect.ComputeLifespan(log, 3000);
                    AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, ParserIcons.EffectGravityWell);
                }
            }
            // Gravity Well - Pulses
            if (log.CombatData.TryGetEffectEventsBySrcWithGUIDs(player.AgentItem, new string[] { EffectGUIDs.ChronomancerGravityWellPulse, EffectGUIDs.ChronomancerGravityWellExplosion }, out IReadOnlyList<EffectEvent> gravityWellPulses))
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
}
