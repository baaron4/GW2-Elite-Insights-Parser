using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.LogLogic.LogCategories;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicTimeUtils;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal abstract class RaidLogic : LogLogic
{

    protected RaidLogic(int triggerID) : base(triggerID)
    {
        ParseMode = ParseModeEnum.Instanced10;
        SkillMode = SkillModeEnum.PvE;
        LogCategoryInformation.Category = LogCategory.Raid;
        LogID |= LogIDs.LogMasks.RaidMask;
    }

    internal static RewardEvent? GetOldRaidReward2Event(CombatData combatData, long start, long end)
    {
        return combatData.GetRewardEvents().FirstOrDefault(x => x.RewardType == RewardTypes.OldRaidReward2 && x.Time > start && x.Time < end);
    }

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, LogData logData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        if (IsInstance)
        {
            logData.SetSuccess(true, GetFinalMapChangeTime(logData, combatData));
            return;
        }
        var raidRewardsTypes = new HashSet<int>();
        if (combatData.GetGW2BuildEvent().Build < GW2Builds.June2019RaidRewards)
        {
            raidRewardsTypes = [RewardTypes.OldRaidReward1, RewardTypes.OldRaidReward2];
        }
        else
        {
            raidRewardsTypes = [RewardTypes.CurrentRaidReward];
        }
        IReadOnlyList<RewardEvent> rewards = combatData.GetRewardEvents();
        RewardEvent? reward = rewards.FirstOrDefault(x => raidRewardsTypes.Contains(x.RewardType) && x.Time > logData.LogStart);
        if (reward != null)
        {
            logData.SetSuccess(true, reward.Time);
        }
        else
        {
            NoBouncyChestGenericCheckSucess(combatData, agentData, logData, playerAgents);
        }
    }

    internal override LogData.LogStartStatus GetLogStartStatus(CombatData combatData, AgentData agentData, LogData logData)
    {
        if (IsInstance)
        {
            return base.GetLogStartStatus(combatData, agentData, logData);
        }
        if (TargetHPPercentUnderThreshold(GenericTriggerID, logData.LogStart, combatData, Targets))
        {
            return LogData.LogStartStatus.Late;
        }
        return LogData.LogStartStatus.Normal;
    }

    internal override IReadOnlyList<TargetID>  GetTargetsIDs()
    {
        return new[] { GetTargetID(GenericTriggerID) };
    }
}
