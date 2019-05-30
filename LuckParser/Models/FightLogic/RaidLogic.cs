using LuckParser.Parser;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.Logic
{
    public abstract class RaidLogic : FightLogic
    {
        protected RaidLogic(ushort triggerID, AgentData agentData) : base(triggerID, agentData)
        {
            Mode = ParseMode.Raid;
        }

        public override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, HashSet<AgentItem> playerAgents)
        {
            HashSet<int> raidRewardsIds = new HashSet<int>
                {
                    55821,
                    60685,
                    914
                };
            CombatItem reward = combatData.GetStates(ParseEnum.StateChange.Reward).FirstOrDefault(x => raidRewardsIds.Contains(x.Value));
            if (reward != null)
            {
                fightData.SetSuccess(true, reward.LogTime);
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
