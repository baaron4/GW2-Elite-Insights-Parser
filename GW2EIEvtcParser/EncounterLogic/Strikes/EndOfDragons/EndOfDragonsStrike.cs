using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal abstract class EndOfDragonsStrike : StrikeMissionLogic
    {
        public EndOfDragonsStrike(int triggerID) : base(triggerID)
        {
            EncounterCategoryInformation.SubCategory = SubFightCategory.Cantha;
            EncounterID |= EncounterIDs.StrikeMasks.EODMask;
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            IReadOnlyList<RewardEvent> rewards = combatData.GetRewardEvents();
            RewardEvent reward = rewards.FirstOrDefault(x => x.RewardType == RewardTypes.PostEoDStrikeReward && x.Time > fightData.FightStart);
            if (reward != null)
            {
                fightData.SetSuccess(true, reward.Time);
            }
            else
            {
                NoBouncyChestGenericCheckSucess(combatData, agentData, fightData, playerAgents);
            }
        }
    }
}
