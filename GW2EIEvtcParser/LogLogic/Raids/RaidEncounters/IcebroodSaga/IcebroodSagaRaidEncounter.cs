using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.LogLogic.LogLogicTimeUtils;

namespace GW2EIEvtcParser.LogLogic;

internal abstract class IcebroodSagaRaidEncounter : RaidEncounterLogic
{
    public IcebroodSagaRaidEncounter(int triggerID) : base(triggerID)
    {
        LogID |= LogIDs.RaidEncounterMasks.IBSMask;
    }

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, LogData logData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        if (IsInstance)
        {
            logData.SetSuccess(true, GetFinalMapChangeTime(logData, combatData));
            return;
        }
        var sraidEncounterRewardIDs = new HashSet<ulong>
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
        RewardEvent? reward = rewards.FirstOrDefault(x => sraidEncounterRewardIDs.Contains(x.RewardID) && x.Time > logData.LogStart);
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
