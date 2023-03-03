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
            new PlayerDstBuffApplyMechanic(FluxBombEffect, "Flux Bomb", new MechanicPlotlySetting(Symbols.Circle,Colors.Purple,10), "Flux","Flux Bomb application", "Flux Bomb",0),
            new PlayerDstHitMechanic(FluxBombSkill, "Flux Bomb", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Purple,10), "Flux dmg","Flux Bomb hit", "Flux Bomb dmg",0),
            new SpawnMechanic((int)ArcDPSEnums.TrashID.FractalVindicator, "Fractal Vindicator", new MechanicPlotlySetting(Symbols.StarDiamondOpen,Colors.Black,10), "Vindicator","Fractal Vindicator spawned", "Vindicator spawn",0),
            });
            EncounterCategoryInformation.Category = FightCategory.Fractal;
            EncounterID |= EncounterIDs.EncounterMasks.FractalMask;
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            // generic method for fractals
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecy(GenericTriggerID));
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

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            // check reward
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecy(GenericTriggerID));
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
        protected long GetFightOffsetByFirstInvulFilter(FightData fightData, AgentData agentData, List<CombatItem> combatData, int targetID, long invulID, long invulGainOffset)
        {         
            long startToUse = GetGenericFightOffset(fightData);
            CombatItem logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == ArcDPSEnums.StateChange.LogStartNPCUpdate);
            if (logStartNPCUpdate != null)
            {
                startToUse = GetEnterCombatTime(fightData, agentData, combatData, logStartNPCUpdate.Time);
            }
            // Find target
            AgentItem target = agentData.GetNPCsByID(targetID).FirstOrDefault();
            if (target == null)
            {
                throw new MissingKeyActorsException("Main target of the fight not found");
            }
            // check first invul gain at the start of the fight
            CombatItem invulGain = combatData.FirstOrDefault(x => x.DstMatchesAgent(target) && x.IsBuffApply() && x.SkillID == invulID && x.Time >= startToUse);
            // get invul lost
            CombatItem invulLost = combatData.FirstOrDefault(x => x.SrcMatchesAgent(target) && x.IsBuffRemove == ArcDPSEnums.BuffRemove.All && x.SkillID == invulID && x.Time >= startToUse);
            // invul loss matches the gained invul
            if (invulGain != null && invulLost != null && invulLost.Time > invulGain.Time)
            {
                // check against offset
                if (invulGain.Time - startToUse < invulGainOffset)
                {
                    return invulLost.Time + 1;
                }
            }
            else if (invulLost != null)
            {
                // only invul lost, missing buff apply event
                CombatItem enterCombat = combatData.FirstOrDefault(x => x.SrcMatchesAgent(target) && x.IsStateChange == ArcDPSEnums.StateChange.EnterCombat && x.Time >= startToUse);
                // verify that first enter combat matches the moment invul is lost
                if (enterCombat != null && Math.Abs(enterCombat.Time - invulLost.Time) < ParserHelper.ServerDelayConstant)
                {
                    return invulLost.Time + 1;
                }
            }
            return startToUse;
        }

        internal override void ComputePlayerCombatReplayActors(AbstractPlayer p, ParsedEvtcLog log, CombatReplay replay)
        {
            var knownEffectsIDs = new HashSet<long>();
            EffectGUIDEvent sickness = log.CombatData.GetEffectGUIDEvent(EffectGUIDs.ToxicSicknessPuke1);

            if (sickness != null)
            {
                var sicknessEffects = log.CombatData.GetEffectEventsByEffectID(sickness.ContentID).Where(x => x.Dst == p.AgentItem).ToList();
                knownEffectsIDs.Add(sickness.ContentID);

                foreach (EffectEvent sicknessEffect in sicknessEffects)
                {
                    if (replay.Rotations.Count > 0)
                    {
                        int duration = 4000;
                        int radius = 600;
                        int openingAngle = 36;
                        int effectStart = (int)sicknessEffect.Time;
                        int effectEnd = effectStart + duration;
                        replay.Decorations.Add(new FacingPieDecoration((effectStart, effectEnd), new AgentConnector(p), replay.PolledRotations, radius, openingAngle, "rgba(250, 120, 0, 0.2)"));
                        replay.Decorations.Add(new FacingPieDecoration((effectEnd, effectEnd + 200), new AgentConnector(p), replay.PolledRotations, radius, openingAngle, "rgba(250, 120, 0, 0.4)"));
                    }
                }
            }
        }

    }
}
