using System;
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
    internal static class ChronomancerHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(ContinuumSplit, TimeAnchored), // Continuum Split
            new BuffLossCastFinder(ContinuumShift, TimeAnchored), // Continuum Shift
            new EffectCastFinder(SplitSecond, EffectGUIDs.ChronomancerSplitSecond).UsingChecker((evt, combatData, agentData, skillData) => {
                if (evt.Src.Spec != Spec.Chronomancer)
                {
                    return false;
                }
                // Clones also trigger this effect but it is sourced to the master, we need additional checks
                // Seize the moment makes for a very clear and clean check 
                if (combatData.TryGetEffectEventsByGUID(EffectGUIDs.ChronomancerSeizeTheMomentShatter, out IReadOnlyList<EffectEvent> shatterEvents) && shatterEvents.Any(x => x.Src == evt.Src && Math.Abs(x.Time - evt.Time) < ServerDelayConstant && x.Position.Distance2DToPoint(evt.Position) < 0.1))
                {
                    return true;
                }
                return false;
                // This is not reliable enough, leaving the code commented
                //Otherwise, we check the position
                /*IEnumerable<PositionEvent> positionEvents = combatData.GetMovementData(evt.Src).OfType<PositionEvent>();
                PositionEvent prevPositionEvent = positionEvents.LastOrDefault(x => x.Time <= evt.Time);
                if (prevPositionEvent == null)
                {
                    return false;
                }
                PositionEvent nextPositionEvent = positionEvents.FirstOrDefault(x => x.Time >= evt.Time && x.Time <= prevPositionEvent.Time + ArcDPSPollingRate + ServerDelayConstant);
                Point3D currentPosition;
                if (nextPositionEvent != null)
                {

                    (var xPrevPos, var yPrevPos, _) = prevPositionEvent.Unpack();
                    (var xNextPos, var yNextPos, _) = nextPositionEvent.Unpack();
                    float ratio = (float)(evt.Time - prevPositionEvent.Time) / (nextPositionEvent.Time - prevPositionEvent.Time);
                    var prevPosition = new Point3D(xPrevPos, yPrevPos, 0);
                    var nextPosition = new Point3D(xNextPos, yNextPos, 0);
                    currentPosition = new Point3D(prevPosition, nextPosition, ratio, 0);
                } 
                else
                {
                    (var xPos, var yPos, _) = prevPositionEvent.Unpack();
                    currentPosition = new Point3D(xPos, yPos, 0);
                }
                // Allow an error a little bit below half the hitbox width of a player (48)
                if  (currentPosition.Distance2DToPoint(evt.Position) < 15) {
                    return true;
                }*/
            }).UsingNotAccurate(true),
            new EffectCastFinder(Rewinder, EffectGUIDs.ChronomancerRewinder).UsingSrcSpecChecker(Spec.Chronomancer),
            new EffectCastFinder(TimeSink, EffectGUIDs.ChronomancerTimeSink).UsingSrcSpecChecker(Spec.Chronomancer),
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifierTarget(Slow, "Danger Time", "10% crit damage on slowed target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Chronomancer, ByPresence, BuffImages.DangerTime, DamageModifierMode.All).UsingChecker((x, log) => x.HasCrit).WithBuilds(GW2Builds.February2018Balance, GW2Builds.December2018Balance),
            new BuffDamageModifierTarget(Slow, "Danger Time", "10% crit damage on slowed target", DamageSource.All, 10.0, DamageType.Strike, DamageType.All, Source.Chronomancer, ByPresence, BuffImages.DangerTime, DamageModifierMode.All).UsingChecker((x, log) => x.HasCrit).WithBuilds(GW2Builds.December2018Balance, GW2Builds.May2021Balance),
            new BuffDamageModifier(Alacrity, "Improved Alacrity", "10% crit under alacrity", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Chronomancer, ByPresence, BuffImages.ImprovedAlacrity, DamageModifierMode.All).UsingChecker((x, log) => x.HasCrit).WithBuilds(GW2Builds.August2022BalanceHotFix),
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
                    (int, int) lifespan = ProfHelper.ComputeEffectLifespan(log, effect, 3000);
                    var connector = new PositionConnector(effect.Position);
                    replay.Decorations.Add(new CircleDecoration(false, 0, 240, lifespan, color.WithAlpha(0.5f).ToString(), connector).UsingSkillMode(skill));
                    replay.Decorations.Add(new IconDecoration(ParserIcons.EffectWellOfEternity, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector).UsingSkillMode(skill));
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
                    replay.Decorations.Add(new CircleDecoration(false, effectTimeEnd, 240, (effectTimeStart, effectTimeEnd), color.WithAlpha(0.5f).ToString(), connector).UsingSkillMode(skill));
                }
            }

            // Well of Action
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ChronomancerWellOfAction, out IReadOnlyList<EffectEvent> wellsOfAction))
            {
                var skill = new SkillModeDescriptor(player, Spec.Chronomancer, WellOfAction);
                foreach (EffectEvent effect in wellsOfAction)
                {
                    (int, int) lifespan = ProfHelper.ComputeEffectLifespan(log, effect, 3000);
                    var connector = new PositionConnector(effect.Position);
                    replay.Decorations.Add(new CircleDecoration(false, 0, 240, lifespan, color.WithAlpha(0.5f).ToString(), connector).UsingSkillMode(skill));
                    replay.Decorations.Add(new IconDecoration(ParserIcons.EffectWellOfAction, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector).UsingSkillMode(skill));
                    
                    // Well pulses - Hard coded because the effects don't have a Src
                    int pulseTimeDelay = 0;
                    for (int i = 0; i < 4; i++)
                    {
                        int effectTimeStart = (int)effect.Time + pulseTimeDelay;
                        int effectTimeEnd = effectTimeStart + 300;
                        if (effectTimeStart > lifespan.Item2) { break; }
                        if (i < 3)
                        {
                            // Pulse inwards
                            replay.Decorations.Add(new CircleDecoration(false, -effectTimeEnd, 240, (effectTimeStart, effectTimeEnd), color.WithAlpha(0.5f).ToString(), connector).UsingSkillMode(skill));
                        }
                        else
                        {
                            // Final pulse outwards
                            replay.Decorations.Add(new CircleDecoration(false, effectTimeEnd, 240, (effectTimeStart, effectTimeEnd), color.WithAlpha(0.5f).ToString(), connector).UsingSkillMode(skill));
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
                    (int, int) lifespan = ProfHelper.ComputeEffectLifespan(log, effect, 3000);
                    var connector = new PositionConnector(effect.Position);
                    replay.Decorations.Add(new CircleDecoration(false, 0, 240, lifespan, color.WithAlpha(0.5f).ToString(), connector).UsingSkillMode(skill));
                    replay.Decorations.Add(new IconDecoration(ParserIcons.EffectWellOfCalamity, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector).UsingSkillMode(skill));
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
                    replay.Decorations.Add(new CircleDecoration(false, -effectTimeEnd, 240, (effectTimeStart, effectTimeEnd), color.WithAlpha(0.5f).ToString(), connector).UsingSkillMode(skill));
                }
            }

            // Well of Precognition
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ChronomancerWellOfPrecognition, out IReadOnlyList<EffectEvent> wellsOfPrecognition))
            {
                var skill = new SkillModeDescriptor(player, Spec.Chronomancer, WellOfPrecognition, SkillModeCategory.ImportantBuffs);
                foreach (EffectEvent effect in wellsOfPrecognition)
                {
                    (int, int) lifespan = ProfHelper.ComputeEffectLifespan(log, effect, 3000);
                    var connector = new PositionConnector(effect.Position);
                    replay.Decorations.Add(new CircleDecoration(false, 0, 240, lifespan, color.WithAlpha(0.5f).ToString(), connector).UsingSkillMode(skill));
                    replay.Decorations.Add(new IconDecoration(ParserIcons.EffectWellOfPrecognition, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector).UsingSkillMode(skill));
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
                    replay.Decorations.Add(new CircleDecoration(false, effectTimeEnd, 240, (effectTimeStart, effectTimeEnd), color.WithAlpha(0.5f).ToString(), connector).UsingSkillMode(skill));
                }
            }

            // Well of Senility
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ChronomancerWellOfSenility, out IReadOnlyList<EffectEvent> wellsOfSenility))
            {
                var skill = new SkillModeDescriptor(player, Spec.Chronomancer, WellOfRecall_Senility);
                foreach (EffectEvent effect in wellsOfSenility)
                {
                    (int, int) lifespan = ProfHelper.ComputeEffectLifespan(log, effect, 3000);
                    var connector = new PositionConnector(effect.Position);
                    replay.Decorations.Add(new CircleDecoration(false, 0, 240, lifespan, color.WithAlpha(0.5f).ToString(), connector).UsingSkillMode(skill));
                    replay.Decorations.Add(new IconDecoration(ParserIcons.EffectWellOfSenility, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector).UsingSkillMode(skill));

                    // Well pulses - Hard coded because the effects don't have a Src
                    int pulseTimeDelay = 0;
                    for (int i = 0; i < 4; i++)
                    {
                        int effectTimeStart = (int)effect.Time + pulseTimeDelay;
                        int effectTimeEnd = effectTimeStart + 300;
                        if (effectTimeStart > lifespan.Item2) { break; }
                        if (i < 3)
                        {
                            // Pulse inwards
                            replay.Decorations.Add(new CircleDecoration(false, -effectTimeEnd, 240, (effectTimeStart, effectTimeEnd), color.WithAlpha(0.5f).ToString(), connector).UsingSkillMode(skill));
                        }
                        else
                        {
                            // Final pulse outwards
                            replay.Decorations.Add(new CircleDecoration(false, effectTimeEnd, 240, (effectTimeStart, effectTimeEnd), color.WithAlpha(0.5f).ToString(), connector).UsingSkillMode(skill));
                        }
                        pulseTimeDelay += 1000;
                    }
                }
            }

            // Gravity Well
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ChronomancerGravityWell, out IReadOnlyList<EffectEvent> gravityWells))
            {
                var skill = new SkillModeDescriptor(player, Spec.Chronomancer, GravityWell);
                foreach (EffectEvent effect in gravityWells)
                {
                    (int, int) lifespan = ProfHelper.ComputeEffectLifespan(log, effect, 3000);
                    var connector = new PositionConnector(effect.Position);
                    replay.Decorations.Add(new CircleDecoration(false, 0, 240, lifespan, color.WithAlpha(0.5f).ToString(), connector).UsingSkillMode(skill));
                    replay.Decorations.Add(new IconDecoration(ParserIcons.EffectGravityWell, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector).UsingSkillMode(skill));
                }
            }
            // Gravity Well - Pulses
            if (log.CombatData.TryGetEffectEventsBySrcWithGUIDs(player.AgentItem, new string[] { EffectGUIDs.ChronomancerGravityWellPulse, EffectGUIDs.ChronomancerGravityWellExplosion }, out IReadOnlyList<EffectEvent> gravityWellPulses))
            {
                var skill = new SkillModeDescriptor(player, Spec.Chronomancer, GravityWell);
                foreach (EffectEvent effect in gravityWellPulses)
                {
                    int effectTimeStart = (int)effect.Time;
                    int effectTimeEnd = effectTimeStart + 1000;
                    var connector = new PositionConnector(effect.Position);
                    replay.Decorations.Add(new CircleDecoration(false, -effectTimeEnd, 240, (effectTimeStart, effectTimeEnd), color.WithAlpha(0.5f).ToString(), connector).UsingSkillMode(skill));
                }
            }
        }
    }
}
