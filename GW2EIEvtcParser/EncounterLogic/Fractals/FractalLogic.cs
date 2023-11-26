using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal abstract class FractalLogic : FightLogic
    {
        protected FractalLogic(int triggerID) : base(triggerID)
        {
            Mode = ParseMode.Instanced5;
            MechanicList.AddRange(new List<Mechanic>
            {
            new PlayerDstBuffApplyMechanic(FluxBombBuff, "Flux Bomb", new MechanicPlotlySetting(Symbols.Circle,Colors.Purple,10), "Flux","Flux Bomb application", "Flux Bomb",0),
            new PlayerDstHitMechanic(FluxBombSkill, "Flux Bomb", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Purple,10), "Flux dmg","Flux Bomb hit", "Flux Bomb dmg",0), // No longer tracking damage
            new SpawnMechanic((int)ArcDPSEnums.TrashID.FractalVindicator, "Fractal Vindicator", new MechanicPlotlySetting(Symbols.StarDiamondOpen,Colors.Black,10), "Vindicator","Fractal Vindicator spawned", "Vindicator spawn",0),
            new PlayerDstBuffApplyMechanic(DebilitatedToxicSickness, "Debilitated", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Pink, 10), "Debil.A", "Debilitated Application (Toxic Sickness)", "Received Debilitated", 0),
            /* Not trackable due to health % damage for now
            new PlayerDstHitMechanic(ToxicSickness, "Toxic Sickness", new MechanicPlotlySetting(Symbols.TriangleUpOpen, Colors.DarkGreen, 10), "ToxSick.H", "Hit by Toxic Sickness", "Toxic Sickness Hit", 0),
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
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(GenericTriggerID));
            if (mainTarget == null)
            {
                throw new MissingKeyActorsException("Main target of the fight not found");
            }
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

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>()
            {
                ArcDPSEnums.TrashID.FractalAvenger,
                ArcDPSEnums.TrashID.FractalVindicator,
                ArcDPSEnums.TrashID.TheMossman,
                ArcDPSEnums.TrashID.InspectorEllenKiel,
                ArcDPSEnums.TrashID.ChampionRabbit,
                ArcDPSEnums.TrashID.JadeMawTentacle,
                ArcDPSEnums.TrashID.AwakenedAbomination,
            };
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            // check reward
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(GenericTriggerID));
            if (mainTarget == null)
            {
                throw new MissingKeyActorsException("Main target of the fight not found");
            }
            RewardEvent reward = combatData.GetRewardEvents().LastOrDefault(x => x.RewardType == 13);
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
            CombatItem logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == ArcDPSEnums.StateChange.LogStartNPCUpdate);
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
            CombatItem invulLost = combatData.FirstOrDefault(x => x.SrcMatchesAgent(target) && x.IsBuffRemove == ArcDPSEnums.BuffRemove.All && x.SkillID == invulID && x.Time >= startToUse);
            // invul gain at the start and invul loss matches the gained invul
            if (invulGain != null && (invulGain.IsStateChange == ArcDPSEnums.StateChange.BuffInitial || invulGain.Time - target.FirstAware < 200) && invulLost != null && invulLost.Time >= invulGain.Time)
            {
                return invulLost.Time + 1;
            }
            else if (invulLost != null && (invulGain == null || invulLost.Time < invulGain.Time))
            {
                // only invul lost, missing buff apply event
                CombatItem enterCombat = combatData.FirstOrDefault(x => x.SrcMatchesAgent(target) && x.IsStateChange == ArcDPSEnums.StateChange.EnterCombat && x.Time >= startToUse);
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
            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.ToxicSicknessPuke1, out IReadOnlyList<EffectEvent> sicknessEffects))
            {
                var sicknessEffectsOnPlayer = sicknessEffects.Where(x => x.Dst == p.AgentItem).ToList();

                foreach (EffectEvent sicknessEffect in sicknessEffectsOnPlayer)
                {
                    if (replay.Rotations.Any())
                    {
                        int duration = 4000;
                        int radius = 600;
                        int openingAngle = 36;
                        int effectStart = (int)sicknessEffect.Time;
                        int effectEnd = effectStart + duration;
                        var rotationConnector = new AgentFacingConnector(p);
                        var connector = new AgentConnector(p);
                        replay.Decorations.Add(new PieDecoration(true, 0, radius, openingAngle, (effectStart, effectEnd), "rgba(0, 100, 0, 0.2)", connector).UsingRotationConnector(rotationConnector));
                        replay.Decorations.Add(new PieDecoration(true, 0, radius, openingAngle, (effectEnd, effectEnd + 200), "rgba(0, 100, 0, 0.4)", connector).UsingRotationConnector(rotationConnector));
                    }
                }
            }
        }

        protected static void AddFractalScaleEvent(ulong gw2Build, List<CombatItem> combatData, IReadOnlyList<(ulong build, byte scale)> scales)
        {
            if (combatData.Any(x => x.IsStateChange == ArcDPSEnums.StateChange.FractalScale))
            {
                return;
            }
            var orderedScales = scales.OrderByDescending(x => x.build).ToList();
            foreach ((ulong build, byte scale) in orderedScales)
            {
                if (gw2Build >= build)
                {
                    combatData.Add(new CombatItem(0, scale, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, (byte)ArcDPSEnums.StateChange.FractalScale, 0, 0, 0, 0));
                    break;
                }
            }
        }

        internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log)
        {
            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.SmallFluxBomb, out IReadOnlyList<EffectEvent> fluxBombEffects))
            {
                foreach (EffectEvent fluxEffect in fluxBombEffects)
                {
                    int duration = 5000;
                    int start = (int)fluxEffect.Time;
                    int effectEnd = start + duration;
                    EnvironmentDecorations.Add(new CircleDecoration(true, 0, 120, (start, effectEnd), "rgba(0, 0, 255, 0.1)", new PositionConnector(fluxEffect.Position)));
                    EnvironmentDecorations.Add(new DoughnutDecoration(false, 0, 119, 121, (start, effectEnd), "rgba(255, 0, 0, 0.2)", new PositionConnector(fluxEffect.Position)));

                    int pulseDuration = 1000;
                    int pulse = start + pulseDuration;
                    int previousPulse = start;
                    for (int pulses = 0; pulses < 5; pulses++)
                    {
                        EnvironmentDecorations.Add(new CircleDecoration(true, pulse, 120, (previousPulse, pulse), "rgba(0, 0, 255, 0.1)", new PositionConnector(fluxEffect.Position)));
                        previousPulse = pulse;
                        pulse += pulseDuration;
                    }
                }
            }
        }

    }
}
