using System.Collections.Generic;
using System.Linq;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.Logic
{
    public abstract class StrikeMissionLogic : FightLogic
    {

        protected StrikeMissionLogic(int triggerID) : base(triggerID)
        {
            Mode = ParseMode.Raid;
        }

        protected virtual void SetSuccessByDeath(CombatData combatData, FightData fightData, HashSet<AgentItem> playerAgents, bool all)
        {
            SetSuccessByDeath(combatData, fightData, playerAgents, all, GenericTriggerID);
        }

        public override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, HashSet<AgentItem> playerAgents)
        {
            var strikeRewardIDs = new HashSet<ulong>
                {
                    993
                };
            List<RewardEvent> rewards = combatData.GetRewardEvents();
            RewardEvent reward = rewards.FirstOrDefault(x => strikeRewardIDs.Contains(x.RewardID));
            if (reward != null)
            {
                fightData.SetSuccess(true, reward.Time);
            }
            else
            {
                SetSuccessByDeath(combatData, fightData, playerAgents, true);
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
