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
            CanCombatReplay = true;
        }

        public override void SetSuccess(ParsedLog log)
        {
            // Put non reward stuff in this as we find them
            HashSet<int> notRaidRewardsIds = new HashSet<int>
                {
                    13
                };
            CombatItem reward = log.CombatData.GetStatesData(ParseEnum.StateChange.Reward).LastOrDefault(x =>!notRaidRewardsIds.Contains(x.Value));
            if (reward != null)
            {
                log.LogData.Success = true;
                log.FightData.FightEnd = reward.Time;
            }
        }
    }
}
