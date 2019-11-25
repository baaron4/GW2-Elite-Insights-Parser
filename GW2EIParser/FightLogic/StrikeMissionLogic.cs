using System.Collections.Generic;
using System.Linq;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.Logic
{
    public abstract class StrikeMissionLogic : FightLogic
    {

        protected StrikeMissionLogic(ushort triggerID) : base(triggerID)
        {
            Mode = ParseMode.Raid;
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
                fightData.SetSuccess(true, fightData.ToLogSpace(reward.Time));
            }
            else
            {
                SetSuccessByDeath(combatData, fightData, playerAgents, true, GenericTriggerID);
            }
        }

        protected override HashSet<ushort> GetUniqueTargetIDs()
        {
            return new HashSet<ushort>
            {
                GenericTriggerID
            };
        }
    }
}
