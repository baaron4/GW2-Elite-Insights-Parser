using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal abstract class FractalLogic : FightLogic
    {
        protected FractalLogic(int triggerID) : base(triggerID)
        {
            ParseMode = ParseModeEnum.Instanced5;
            SkillMode = SkillModeEnum.PvE;
            MechanicList.AddRange(new List<Mechanic>
            {
            new PlayerDstBuffApplyMechanic(FluxBombBuff, "Flux Bomb", new MechanicPlotlySetting(Symbols.Circle,Colors.Purple,10), "Flux","Flux Bomb application", "Flux Bomb",0),
            new PlayerDstHitMechanic(FluxBombSkill, "Flux Bomb", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Purple,10), "Flux dmg","Flux Bomb hit", "Flux Bomb dmg",0), // No longer tracking damage
            new SpawnMechanic((int)TrashID.FractalVindicator, "Fractal Vindicator", new MechanicPlotlySetting(Symbols.StarDiamondOpen,Colors.Black,10), "Vindicator","Fractal Vindicator spawned", "Vindicator spawn",0),
            new PlayerDstBuffApplyMechanic(DebilitatedToxicSickness, "Debilitated", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Pink, 10), "Debil.A", "Debilitated Application (Toxic Sickness)", "Received Debilitated", 0),
            new PlayerSrcEffectMechanic(new [] { EffectGUIDs.ToxicSicknessOldIndicator, EffectGUIDs.ToxicSicknessNewIndicator }, "Toxic Sickness", new MechanicPlotlySetting(Symbols.TriangleUpOpen, Colors.LightOrange, 10), "ToxSick.A", "Toxic Sickness Application", "Toxic Sickness Application", 0),
            new PlayerSrcBuffApplyMechanic(DebilitatedToxicSickness, "Toxic Sickness", new MechanicPlotlySetting(Symbols.TriangleLeftOpen, Colors.LightOrange, 10), "ToxSick.HitTo", "Hit another player with Toxic Sickness", "Toxic Sickness Hit To Player", 0).UsingChecker((bae, log) => bae.To.IsPlayer),
            new PlayerDstBuffApplyMechanic(DebilitatedToxicSickness, "Toxic Sickness", new MechanicPlotlySetting(Symbols.TriangleRightOpen, Colors.LightOrange, 10), "ToxSick.HitBy", "Got hit by Toxic Sickness", "Toxic Sickness Hit By Player", 0).UsingChecker((bae, log) => bae.By.IsPlayer),
            /* Not trackable due to health % damage for now
            new PlayerDstHitMechanic(ToxicTrail, "Toxic Trail", new MechanicPlotlySetting(Symbols.CircleOpenDot, Colors.DarkGreen, 10), "ToxTrail.H", "Hit by Toxic Trail", "Toxic Trail Hit", 0),
            new PlayerDstHitMechanic(ExplodeLastLaugh, "Explode", new MechanicPlotlySetting(Symbols.CircleOpenDot, Colors.Orange, 10), "Explode.H", "Hit by Last Laugh Explode", "Last Laugh Hit", 0),
            new PlayerDstHitMechanic(WeBleedFireBig, "We Bleed Fire", new MechanicPlotlySetting(Symbols.Star, Colors.LightRed, 10), "BleedFireB.H", "Hit by We Bleed Fire (Big)", "Big Bleed Fire Hit", 0),
            new PlayerDstHitMechanic(WeBleedFireSmall, "We Bleed Fire", new MechanicPlotlySetting(Symbols.StarOpen, Colors.LightRed, 10), "BleedFireS.H", "Hit by We Bleed Fire (Small)", "Small Bleed FIre Hit", 0),
             */
            });
            EncounterCategoryInformation.Category = FightCategory.Fractal;
            EncounterID |= EncounterIDs.EncounterMasks.FractalMask;
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            // generic method for fractals
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(GenericTriggerID)) ?? throw new MissingKeyActorsException("Main target of the fight not found");
            phases[0].AddTarget(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            phases.AddRange(GetPhasesByInvul(log, Determined762, mainTarget, false, true));
            for (int i = 1; i < phases.Count; i++)
            {
                phases[i].Name = "Phase " + i;
                phases[i].AddTarget(mainTarget);
            }
            return phases;
        }

        protected override HashSet<int> GetUniqueNPCIDs()
        {
            return new HashSet<int>
            {
                GenericTriggerID
            };
        }

        protected override List<TrashID> GetTrashMobsIDs()
        {
            return new List<TrashID>()
            {
                TrashID.FractalAvenger,
                TrashID.FractalVindicator,
                TrashID.TheMossman,
                TrashID.InspectorEllenKiel,
                TrashID.ChampionRabbit,
                TrashID.JadeMawTentacle,
                TrashID.AwakenedAbomination,
            };
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            // check reward
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(GenericTriggerID)) ?? throw new MissingKeyActorsException("Main target of the fight not found");
            RewardEvent reward = combatData.GetRewardEvents().LastOrDefault(x => x.RewardType == RewardTypes.Daily && x.Time > fightData.FightStart);
            AbstractHealthDamageEvent lastDamageTaken = combatData.GetDamageTakenData(mainTarget.AgentItem).LastOrDefault(x => (x.HealthDamage > 0) && playerAgents.Contains(x.From.GetFinalMaster()));
            if (lastDamageTaken != null)
            {
                if (reward != null && Math.Abs(lastDamageTaken.Time - reward.Time) < 1000)
                {
                    fightData.SetSuccess(true, Math.Min(lastDamageTaken.Time, reward.Time));
                }
                else
                {
                    NoBouncyChestGenericCheckSucess(combatData, agentData, fightData, playerAgents);
                }
            }
        }
        protected static long GetFightOffsetByFirstInvulFilter(FightData fightData, AgentData agentData, List<CombatItem> combatData, int targetID, long invulID)
        {
            long startToUse = GetGenericFightOffset(fightData);
            CombatItem logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.LogNPCUpdate);
            AgentItem target;
            if (logStartNPCUpdate != null)
            {
                target = agentData.GetNPCsByIDAndAgent(targetID, logStartNPCUpdate.DstAgent).FirstOrDefault() ?? agentData.GetNPCsByID(targetID).FirstOrDefault();
                startToUse = GetEnterCombatTime(fightData, agentData, combatData, logStartNPCUpdate.Time, targetID, logStartNPCUpdate.DstAgent);
            }
            else
            {
                target = agentData.GetNPCsByID(targetID).FirstOrDefault();
            }
            if (target == null)
            {
                throw new MissingKeyActorsException("Main target of the fight not found");
            }
            // check first invul gain
            CombatItem invulGain = combatData.FirstOrDefault(x => x.DstMatchesAgent(target) && x.IsBuffApply() && x.SkillID == invulID && x.Time >= startToUse);
            // get invul lost
            CombatItem invulLost = combatData.FirstOrDefault(x => x.SrcMatchesAgent(target) && x.IsBuffRemove == BuffRemove.All && x.SkillID == invulID && x.Time >= startToUse);
            // invul gain at the start and invul loss matches the gained invul
            if (invulGain != null && (invulGain.IsStateChange == StateChange.BuffInitial || invulGain.Time - target.FirstAware < 200) && invulLost != null && invulLost.Time >= invulGain.Time)
            {
                return invulLost.Time + 1;
            }
            else if (invulLost != null && (invulGain == null || invulLost.Time < invulGain.Time))
            {
                // only invul lost, missing buff apply event
                CombatItem enterCombat = combatData.FirstOrDefault(x => x.SrcMatchesAgent(target) && x.IsStateChange == StateChange.EnterCombat && x.Time >= startToUse);
                // no buff apply -> target was invul the whole time
                if (enterCombat != null && enterCombat.Time >= invulLost.Time)
                {
                    return invulLost.Time + 1;
                }
            }
            return startToUse;
        }

        internal override void ComputePlayerCombatReplayActors(AbstractPlayer p, ParsedEvtcLog log, CombatReplay replay)
        {
            base.ComputePlayerCombatReplayActors(p, log, replay);

            // Toxic Sickness
            var toxicSicknessGUIDs = new List<string>() { EffectGUIDs.ToxicSicknessOldIndicator, EffectGUIDs.ToxicSicknessNewIndicator };
            foreach (string guid in toxicSicknessGUIDs)
            {
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(p.AgentItem, guid, out IReadOnlyList<EffectEvent> toxicSickenss))
                {
                    foreach (EffectEvent effect in toxicSickenss)
                    {
                        (long start, long end) lifespan = (effect.Time, effect.Time + 4000);
                        (long start, long end) lifespanPuke = (lifespan.end, lifespan.end + 200);
                        var rotation = new AgentFacingConnector(p);
                        var connector = new AgentConnector(p);
                        replay.Decorations.Add(new PieDecoration(600, 36, lifespan, Colors.DarkGreen, 0.2, connector).UsingRotationConnector(rotation));
                        replay.Decorations.Add(new PieDecoration(600, 36, lifespanPuke, Colors.DarkGreen, 0.4, connector).UsingRotationConnector(rotation));
                    }
                }
            }

            // Flux Bomb on selected player
            var fluxBombApplies = p.GetBuffStatus(log, FluxBombBuff, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0).ToList();
            foreach (Segment segment in fluxBombApplies)
            {
                replay.AddDecorationWithGrowing(new CircleDecoration(120, segment, Colors.LightOrange, 0.2, new AgentConnector(p)), segment.End);
            }
        }

        protected static void AddFractalScaleEvent(ulong gw2Build, List<CombatItem> combatData, IReadOnlyList<(ulong build, byte scale)> scales)
        {
            if (combatData.Any(x => x.IsStateChange == StateChange.FractalScale))
            {
                return;
            }
            var orderedScales = scales.OrderByDescending(x => x.build).ToList();
            foreach ((ulong build, byte scale) in orderedScales)
            {
                if (gw2Build >= build)
                {
                    combatData.Add(new CombatItem(0, scale, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, (byte)StateChange.FractalScale, 0, 0, 0, 0));
                    break;
                }
            }
        }

        internal override FightData.EncounterStartStatus GetEncounterStartStatus(CombatData combatData, AgentData agentData, FightData fightData)
        {
            if (TargetHPPercentUnderThreshold(GenericTriggerID, fightData.FightStart, combatData, Targets))
            {
                return FightData.EncounterStartStatus.Late;
            }
            return FightData.EncounterStartStatus.Normal;
        }

        internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log)
        {
            base.ComputeEnvironmentCombatReplayDecorations(log);

            // Flux bomb on the ground
            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.SmallFluxBomb, out IReadOnlyList<EffectEvent> fluxBombs))
            {
                foreach (EffectEvent effect in fluxBombs)
                {
                    (long start, long end) lifespan = effect.ComputeDynamicLifespan(log, 5000);
                    var circle = new CircleDecoration(120, lifespan, Colors.Blue, 0.1, new PositionConnector(effect.Position));
                    EnvironmentDecorations.Add(circle);
                    EnvironmentDecorations.Add(circle.GetBorderDecoration(Colors.Red, 0.2));

                    int pulseDuration = 1000;
                    long pulse = lifespan.start + pulseDuration;
                    long previousPulse = lifespan.start;
                    for (int pulses = 0; pulses < 5; pulses++)
                    {
                        EnvironmentDecorations.Add(new CircleDecoration(120, (previousPulse, pulse), Colors.Blue, 0.1, new PositionConnector(effect.Position)).UsingGrowingEnd(pulse));
                        previousPulse = pulse;
                        pulse += pulseDuration;
                    }
                }
            }
        }

        /// <summary>
        /// Add AoE decorations based on the distance to the caster. <br></br>
        /// Used for Caustic Barrage (Siax, Ensolyss) and Solar Bolt (Skorvald).
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="environmentDecorations">The decorations list.</param>
        /// <param name="effectGUID">The <see cref="EffectGUIDs"/>.</param>
        /// <param name="target">The target casting.</param>
        /// <param name="distanceThreshold">Threshold distance of the effect from the caster.</param>
        /// <param name="onDistanceSuccessDuration">Duration of the AoE effects closer to the caster.</param>
        /// <param name="onDistanceFailDuration">Duration of the AoE effects farther away from the caster.</param>
        protected static void AddDistanceCorrectedOrbDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations, string effectGUID, TargetID target, double distanceThreshold, long onDistanceSuccessDuration, long onDistanceFailDuration)
        {
            if (!log.AgentData.TryGetFirstAgentItem(target, out AgentItem agent))
            {
                return;
            }
            if (log.CombatData.TryGetEffectEventsByGUID(effectGUID, out IReadOnlyList<EffectEvent> effects))
            {
                foreach (EffectEvent effect in effects)
                {
                    (long start, long end) lifespan = (effect.Time, effect.Time + effect.Duration);
                    // Correcting the duration of the effects for CTBS 45, based on the distance from the target casting the mechanic.
                    if (effect is EffectEventCBTS45)
                    {
                        Point3D agentPos = agent.GetCurrentPosition(log, effect.Time);
                        if (agentPos == null)
                        {
                            continue;
                        }
                        var distance = effect.Position.DistanceToPoint(agentPos);
                        if (distance < distanceThreshold)
                        {
                            lifespan.end = effect.Time + onDistanceSuccessDuration;
                        }
                        else
                        {
                            lifespan.end = effect.Time + onDistanceFailDuration;
                        }
                    }
                    environmentDecorations.Add(new CircleDecoration(100, lifespan, Colors.Orange, 0.3, new PositionConnector(effect.Position)));
                }
            }
        }
    }
}
