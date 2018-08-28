using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models
{
    public abstract class RaidLogic : BossLogic
    {
        protected RaidLogic()
        {
            Mode = ParseMode.Raid;
        }

        public override void SetSuccess(CombatData combatData, LogData logData, BossData bossData)
        {
            // Put non reward stuff in this as we find them
            HashSet<int> notRaidRewardsIds = new HashSet<int>
                {
                    13
                };
            CombatItem reward = combatData.Find(x => x.IsStateChange == ParseEnum.StateChange.Reward && !notRaidRewardsIds.Contains(x.Value));
            if (reward != null)
            {
                logData.SetBossKill(true);
                bossData.SetLastAware(reward.Time);
            }
        }
    }
}
