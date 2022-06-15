
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal abstract class IBSStrike : StrikeMissionLogic
    {
        public IBSStrike(int triggerID) : base(triggerID)
        {
            EncounterID |= EncounterIDs.StrikeMasks.IBSMask;
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            var strikeRewardIDs = new HashSet<ulong>
                {
                    993
                };
            IReadOnlyList<RewardEvent> rewards = combatData.GetRewardEvents();
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
    }
}
