using GW2EIEvtcParser.ParsedData;
using System.Collections.Generic;
using System.Linq;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal abstract class RaidLogic : FightLogic
    {
        protected enum FallBackMethod { None, Death, CombatExit }

        protected FallBackMethod GenericFallBackMethod { get; set; } = FallBackMethod.Death;

        protected RaidLogic(int triggerID) : base(triggerID)
        {
            Mode = ParseMode.Instanced10;
        }

        protected virtual List<int> GetSuccessCheckIds()
        {
            return new List<int>
            {
                GenericTriggerID
            };
        }

        protected void SetSuccessByCombatExit(HashSet<int> targetIds, CombatData combatData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            var targets = Targets.Where(x => targetIds.Contains(x.ID)).ToList();
            SetSuccessByCombatExit(targets, combatData, fightData, playerAgents);
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            var raidRewardsTypes = new HashSet<int>
                {
                    55821,
                    60685,
                    914,
                    22797
                };
            IReadOnlyList<RewardEvent> rewards = combatData.GetRewardEvents();
            RewardEvent reward = rewards.FirstOrDefault(x => raidRewardsTypes.Contains(x.RewardType));
            if (reward != null)
            {
                fightData.SetSuccess(true, reward.Time);
            }
            else
            {
                switch (GenericFallBackMethod)
                {
                    case FallBackMethod.Death:
                        SetSuccessByDeath(combatData, fightData, playerAgents, true, GetSuccessCheckIds());
                        break;
                    case FallBackMethod.CombatExit:
                        SetSuccessByCombatExit(new HashSet<int>(GetSuccessCheckIds()), combatData, fightData, playerAgents);
                        break;
                    default:
                        break;
                }
            }
        }

        protected override HashSet<int> GetUniqueTargetIDs()
        {
            return new HashSet<int>
            {
                GenericTriggerID
            };
        }
    }
}
