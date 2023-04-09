using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal abstract class IcebroodSagaStrike : StrikeMissionLogic
    {
        public IcebroodSagaStrike(int triggerID) : base(triggerID)
        {
            EncounterID |= EncounterIDs.StrikeMasks.IBSMask;
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            var strikeRewardIDs = new HashSet<ulong>
            {
                RewardIDs.ShiverpeaksPassChests,
                RewardIDs.KodansOldAndCurrentChest,
                RewardIDs.KodansCurrentChest1,
                RewardIDs.KodansCurrentRepeatableChest,
                RewardIDs.KodansCurrentChest2,
                RewardIDs.FraenirRepeatableChest,
                RewardIDs.WhisperRepeatableChest,
                RewardIDs.BoneskinnerRepeatableChest,
            };
            IReadOnlyList<RewardEvent> rewards = combatData.GetRewardEvents();
            RewardEvent reward = rewards.FirstOrDefault(x => strikeRewardIDs.Contains(x.RewardID));
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
