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
        protected RaidLogic(ushort triggerID) : base(triggerID)
        {
            Mode = ParseMode.Raid;
        }

        public override void CheckSuccess(CombatData combatData, FightData fightData, HashSet<AgentItem> playerAgents)
        {
            HashSet<int> raidRewardsTypes = new HashSet<int>
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
