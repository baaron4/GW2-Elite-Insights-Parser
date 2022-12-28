using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal abstract class RaidLogic : FightLogic
    {

        protected RaidLogic(int triggerID) : base(triggerID)
        {
            Mode = ParseMode.Instanced10;
            EncounterCategoryInformation.Category = FightCategory.Raid;
            EncounterID |= EncounterIDs.EncounterMasks.RaidMask; 
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            var raidRewardsTypes = new HashSet<int>();
            ulong build = combatData.GetBuildEvent().Build;
            if (build < 97235)
            {
                raidRewardsTypes = new HashSet<int>
                {
                    // Old types, on each kill
                    55821,
                    60685
                };
            }
            else
            {
                raidRewardsTypes = new HashSet<int>
                {
                    // New types, once per week
                    22797
                };
            }
            IReadOnlyList<RewardEvent> rewards = combatData.GetRewardEvents();
            RewardEvent reward = rewards.FirstOrDefault(x => raidRewardsTypes.Contains(x.RewardType));
            if (reward != null)
            {
                fightData.SetSuccess(true, reward.Time);
            }
            else
            {
                NoBouncyChestGenericCheckSucess(combatData, agentData, fightData, playerAgents);
            }
        }

        protected override HashSet<int> GetUniqueNPCIDs()
        {
            return new HashSet<int>
            {
                GenericTriggerID
            };
        }
    }
}
