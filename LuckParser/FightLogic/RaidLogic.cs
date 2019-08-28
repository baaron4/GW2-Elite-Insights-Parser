using System.Collections.Generic;
using System.Linq;
using LuckParser.Parser.ParsedData;
using LuckParser.Parser.ParsedData.CombatEvents;

namespace LuckParser.Logic
{
    public abstract class RaidLogic : FightLogic
    {
        protected enum FallBackMethod { None, Death, CombatExit }

        protected FallBackMethod GenericFallBackMethod = FallBackMethod.Death;

        protected RaidLogic(ushort triggerID) : base(triggerID)
        {
            Mode = ParseMode.Raid;
        }

        protected virtual List<ushort> GetSuccessCheckIds()
        {
            return new List<ushort>
            {
                TriggerID
            };
        }

        public override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, HashSet<AgentItem> playerAgents)
        {
            var raidRewardsTypes = new HashSet<int>
                {
                    55821,
                    60685,
                    914,
                    22797
                };
            List<RewardEvent> rewards = combatData.GetRewardEvents();
            RewardEvent reward = rewards.FirstOrDefault(x => raidRewardsTypes.Contains(x.RewardType));
            if (reward != null)
            {
                fightData.SetSuccess(true, fightData.ToLogSpace(reward.Time));
            }
            else
            {
                switch (GenericFallBackMethod)
                {
                    case FallBackMethod.Death:
                        SetSuccessByDeath(combatData, fightData, playerAgents, true, GetSuccessCheckIds());
                        break;
                    case FallBackMethod.CombatExit:
                        SetSuccessByCombatExit(new HashSet<ushort>(GetSuccessCheckIds()), combatData, fightData, playerAgents);
                        break;
                    default:
                        break;
                }
            }
        }

        protected override HashSet<ushort> GetUniqueTargetIDs()
        {
            return new HashSet<ushort>
            {
                TriggerID
            };
        }
    }
}
