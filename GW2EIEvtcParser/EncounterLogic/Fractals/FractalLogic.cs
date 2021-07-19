using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal abstract class FractalLogic : FightLogic
    {
        protected FractalLogic(int triggerID) : base(triggerID)
        {
            Mode = ParseMode.Instanced5;
            MechanicList.AddRange(new List<Mechanic>
            {
            new PlayerBuffApplyMechanic(37695, "Flux Bomb", new MechanicPlotlySetting("circle","rgb(150,0,255)",10), "Flux","Flux Bomb application", "Flux Bomb",0),
            new HitOnPlayerMechanic(36393, "Flux Bomb", new MechanicPlotlySetting("circle-open","rgb(150,0,255)",10), "Flux dmg","Flux Bomb hit", "Flux Bomb dmg",0),
            new SpawnMechanic(19684, "Fractal Vindicator", new MechanicPlotlySetting("star-diamond-open","rgb(0,0,0)",10), "Vindicator","Fractal Vindicator spawned", "Vindicator spawn",0),
            });
            EncounterCategoryInformation.Category = FightCategory.Fractal;
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            // generic method for fractals
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.ID == GenericTriggerID);
            if (mainTarget == null)
            {
                throw new MissingKeyActorsException("Main target of the fight not found");
            }
            phases[0].AddTarget(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            phases.AddRange(GetPhasesByInvul(log, 762, mainTarget, false, true));
            for (int i = 1; i < phases.Count; i++)
            {
                phases[i].Name = "Phase " + i;
                phases[i].AddTarget(mainTarget);
            }
            return phases;
        }

        protected override HashSet<int> GetUniqueTargetIDs()
        {
            return new HashSet<int>
            {
                GenericTriggerID
            };
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            // check reward
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.ID == GenericTriggerID);
            if (mainTarget == null)
            {
                throw new MissingKeyActorsException("Main target of the fight not found");
            }
            RewardEvent reward = combatData.GetRewardEvents().LastOrDefault();
            AbstractHealthDamageEvent lastDamageTaken = combatData.GetDamageTakenData(mainTarget.AgentItem).LastOrDefault(x => (x.HealthDamage > 0) && playerAgents.Contains(x.From.GetFinalMaster()));
            if (lastDamageTaken != null)
            {
                if (reward != null && Math.Abs(lastDamageTaken.Time - reward.Time) < 100)
                {
                    fightData.SetSuccess(true, Math.Min(lastDamageTaken.Time, reward.Time));
                }
                else
                {
                    SetSuccessByDeath(combatData, fightData, playerAgents, true, GenericTriggerID);
                }
            }
        }
        protected static long GetFightOffsetByFirstInvulFilter(FightData fightData, AgentData agentData, List<CombatItem> combatData, int targetID, long invulID, long invulGainOffset)
        {
            // Find target
            AgentItem target = agentData.GetNPCsByID(targetID).FirstOrDefault();
            if (target == null)
            {
                throw new MissingKeyActorsException("Main target of the fight not found");
            }
            // check invul gain at the start of the fight (initial or with a small threshold)
            CombatItem invulGain = combatData.FirstOrDefault(x => x.DstMatchesAgent(target) && x.IsBuffApply() && x.SkillID == invulID);
            // get invul lost
            CombatItem invulLost = combatData.FirstOrDefault(x => x.Time >= fightData.LogStart && x.SrcMatchesAgent(target) && x.IsBuffRemove == ArcDPSEnums.BuffRemove.All && x.SkillID == invulID);
            if (invulGain != null && invulGain.Time - fightData.LogStart < invulGainOffset && invulLost != null && invulLost.Time > invulGain.Time)
            {
                return invulLost.Time + 1;
            }
            else if (invulLost != null)
            {
                CombatItem enterCombat = combatData.FirstOrDefault(x => x.SrcMatchesAgent(target) && x.IsStateChange == ArcDPSEnums.StateChange.EnterCombat && Math.Abs(x.Time - invulLost.Time) < ParserHelper.ServerDelayConstant);
                if (enterCombat != null)
                {
                    return enterCombat.Time + 1;
                }
            }
            return fightData.LogStart;
        }

    }
}
