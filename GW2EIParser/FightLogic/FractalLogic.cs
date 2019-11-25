using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.Logic
{
    public abstract class FractalLogic : FightLogic
    {
        protected FractalLogic(ushort triggerID) : base(triggerID)
        {
            Mode = ParseMode.Fractal;
            MechanicList.AddRange(new List<Mechanic>
            {
            new PlayerBuffApplyMechanic(37695, "Flux Bomb", new MechanicPlotlySetting("circle","rgb(150,0,255)",10), "Flux","Flux Bomb application", "Flux Bomb",0),
            new HitOnPlayerMechanic(36393, "Flux Bomb", new MechanicPlotlySetting("circle-open","rgb(150,0,255)",10), "Flux dmg","Flux Bomb hit", "Flux Bomb dmg",0),
            new SpawnMechanic(19684, "Fractal Vindicator", new MechanicPlotlySetting("star-diamond-open","rgb(0,0,0)",10), "Vindicator","Fractal Vindicator spawned", "Vindicator spawn",0),
            });
        }

        public override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            // generic method for fractals
            List<PhaseData> phases = GetInitialPhase(log);
            NPC mainTarget = Targets.Find(x => x.ID == GenericTriggerID);
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
                GenericTriggerID
            };
        }

        protected static void SetSuccessByBuffCount(CombatData combatData, FightData fightData, HashSet<AgentItem> playerAgents, NPC target, long buffID, int count)
        {
            if (target == null)
            {
                return;
            }
            List<AbstractBuffEvent> invulsTarget = GetFilteredList(combatData, buffID, target, true);
            if (invulsTarget.Count == count)
            {
                AbstractBuffEvent last = invulsTarget.Last();
                if (!(last is BuffApplyEvent))
                {
                    SetSuccessByCombatExit(new List<NPC> { target }, combatData, fightData, playerAgents);
                }
            }
        }

        public override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, HashSet<AgentItem> playerAgents)
        {
            // check reward
            NPC mainTarget = Targets.Find(x => x.ID == GenericTriggerID);
            if (mainTarget == null)
            {
                throw new InvalidOperationException("Main target of the fight not found");
            }
            RewardEvent reward = combatData.GetRewardEvents().LastOrDefault();
            AbstractDamageEvent lastDamageTaken = combatData.GetDamageTakenData(mainTarget.AgentItem).LastOrDefault(x => (x.Damage > 0) && (playerAgents.Contains(x.From) || playerAgents.Contains(x.From.Master)));
            if (lastDamageTaken != null)
            {
                if (reward != null && lastDamageTaken.Time - reward.Time < 100)
                {
                    fightData.SetSuccess(true, fightData.ToLogSpace(Math.Min(lastDamageTaken.Time, reward.Time)));
                }
                else
                {
                    SetSuccessByDeath(combatData, fightData, playerAgents, true, GenericTriggerID);
                    if (fightData.Success)
                    {
                        fightData.SetSuccess(true, Math.Min(fightData.FightEndLogTime, fightData.ToLogSpace(lastDamageTaken.Time)));
                    }
                }
            }
        }

    }
}
