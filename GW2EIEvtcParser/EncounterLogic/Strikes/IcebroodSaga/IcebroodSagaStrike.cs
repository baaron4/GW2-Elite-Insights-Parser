using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.LogLogic;

internal abstract class IcebroodSagaStrike : StrikeMissionLogic
{
    public IcebroodSagaStrike(int triggerID) : base(triggerID)
    {
        LogID |= LogIDs.StrikeMasks.IBSMask;
    }

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, LogData logData, IReadOnlyCollection<AgentItem> playerAgents)
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
        RewardEvent? reward = rewards.FirstOrDefault(x => strikeRewardIDs.Contains(x.RewardID) && x.Time > logData.LogStart);
        if (reward != null)
        {
            logData.SetSuccess(true, reward.Time);
        }
        else
        {
            NoBouncyChestGenericCheckSucess(combatData, agentData, logData, playerAgents);
        }
    }
}
