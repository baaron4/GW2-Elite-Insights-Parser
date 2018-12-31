using LuckParser.Models.DataModels;
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

        public override void SetSuccess(ParsedLog log)
        {
            HashSet<int> raidRewardsIds = new HashSet<int>
                {
                    55821,
                    60685
                };
            CombatItem reward = log.CombatData.GetStates(ParseEnum.StateChange.Reward).FirstOrDefault(x => raidRewardsIds.Contains(x.Value));
            if (reward != null)
            {
                log.FightData.Success = true;
                log.FightData.FightEnd = reward.Time;
            }
        }
    }
}
