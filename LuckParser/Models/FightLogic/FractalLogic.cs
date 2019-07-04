using LuckParser.Parser;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LuckParser.Models.Logic
{
    public abstract class FractalLogic : FightLogic
    {
        protected FractalLogic(ushort triggerID) : base (triggerID)
        { 
            Mode = ParseMode.Fractal;
            MechanicList.AddRange(new List<Mechanic>
            {
            new PlayerBoonApplyMechanic(37695, "Flux Bomb", new MechanicPlotlySetting("circle","rgb(150,0,255)",10), "Flux","Flux Bomb application", "Flux Bomb",0),
            new HitOnPlayerMechanic(36393, "Flux Bomb", new MechanicPlotlySetting("circle-open","rgb(150,0,255)",10), "Flux dmg","Flux Bomb hit", "Flux Bomb dmg",0),
            new SpawnMechanic(19684, "Fractal Vindicator", new MechanicPlotlySetting("star-diamond-open","rgb(0,0,0)",10), "Vindicator","Fractal Vindicator spawned", "Vindicator spawn",0),
            });
        }

        public override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            // generic method for fractals
            List<PhaseData> phases = GetInitialPhase(log);
            Target mainTarget = Targets.Find(x => x.ID == TriggerID);
            if (mainTarget == null)
            {
                throw new InvalidOperationException("Main target of the fight not found");
            }
            phases[0].Targets.Add(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            phases.AddRange(GetPhasesByInvul(log, 762, mainTarget, false, true));
            phases.RemoveAll(x => x.DurationInMS < 1000);
            for (int i = 1; i < phases.Count; i++)
            {
                phases[i].Name = "Phase " + i;
                phases[i].Targets.Add(mainTarget);
            }
            return phases;
        }

        protected override HashSet<ushort> GetUniqueTargetIDs()
        {
            return new HashSet<ushort>
            {
                TriggerID
            };
        }

        protected void SetSuccessByBuffCount(CombatData combatData, AgentData agentData, FightData fightData, HashSet<AgentItem> playerAgents, Target target, long buffID, int count)
        {
            long lastAware = fightData.ToFightSpace(target.LastAwareLogTime);
            List<AbstractBuffEvent> invulsTarget = GetFilteredList(combatData, buffID, target, true);
            if (invulsTarget.Count == count)
            {
                AbstractBuffEvent last = invulsTarget.Last();
                if (!(last is BuffApplyEvent))
                {
                    SetSuccessByCombatExit(target, combatData, fightData, playerAgents);
                }
            }
        }

        public override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, HashSet<AgentItem> playerAgents)
        {
            // check reward
            Target mainTarget = Targets.Find(x => x.ID == TriggerID);
            if (mainTarget == null)
            {
                throw new InvalidOperationException("Main target of the fight not found");
            }
            RewardEvent reward = combatData.GetRewardEvents().LastOrDefault();
            AbstractDamageEvent lastDamageTaken = combatData.GetDamageTakenData(mainTarget.AgentItem).LastOrDefault(x => (x.Damage > 0) && (playerAgents.Contains(x.From) || playerAgents.Contains(x.MasterFrom)));
            if (lastDamageTaken != null)
            {
                if (reward != null && lastDamageTaken.Time - reward.Time < 100)
                {
                    fightData.SetSuccess(true, fightData.ToLogSpace(Math.Min(lastDamageTaken.Time, reward.Time)));
                }
                else
                {
                    SetSuccessByDeath(combatData, fightData,playerAgents, true, TriggerID);
                    if (fightData.Success)
                    {
                        fightData.SetSuccess(true, Math.Min(fightData.FightEndLogTime, fightData.ToLogSpace(lastDamageTaken.Time)));
                    }
                }
            }
        }

    }
}
