using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.LogLogic.LogCategories;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicTimeUtils;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal abstract class RaidWingLogic : RaidLogic
{

    protected RaidWingLogic(int triggerID) : base(triggerID)
    {
        LogCategoryInformation.Category = LogCategory.RaidWing;
        LogID |= LogIDs.LogMasks.RaidWingMask;
    }

    internal static RewardEvent? GetOldRaidReward2Event(CombatData combatData, long start, long end)
    {
        return combatData.GetRewardEvents().FirstOrDefault(x => x.RewardType == RewardTypes.OldRaidReward2 && x.Time > start && x.Time < end);
    }

    protected virtual (long downAndOutID, TargetID targetID) GetDownAndOutIDs()
    {
        return (SkillIDs.NoBuff, TargetID.Unknown);
    }

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, LogData logData, IReadOnlyCollection<AgentItem> playerAgents, LogData.LogSuccessHandler successHandler)
    {
        if (IsInstance)
        {
            successHandler.SetSuccess(true, GetFinalMapChangeTime(logData, combatData));
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
            successHandler.SetSuccess(true, reward.Time);
        }
        else
        {
            NoBouncyChestGenericCheckSucess(combatData, agentData, logData, playerAgents, successHandler);
        }
    }

    internal override void SetInstanceBuffs(ParsedEvtcLog log, List<InstanceBuff> instanceBuffs)
    {
        base.SetInstanceBuffs(log, instanceBuffs);
        (long downAndOutID, TargetID targetID) = GetDownAndOutIDs();
        if (downAndOutID != SkillIDs.NoBuff && targetID != TargetID.Unknown)
        {
            var mainPhase = log.LogData.GetMainPhase(log);
            var encounterPhase = log.LogData.GetEncounterPhases(log).FirstOrDefault(x => x.Success && x.Targets.Keys.Any(x => x.IsSpecies(targetID)));
            if (encounterPhase != null)
            {
                foreach (var player in log.PlayerList)
                {
                    if (encounterPhase.IntersectsWindow(player.FirstAware, player.LastAware) && player.HasBuff(log, downAndOutID, encounterPhase.End, 100))
                    {
                        instanceBuffs.Add(new InstanceBuff(log.Buffs.BuffsByIDs[downAndOutID], 1, mainPhase));
                        break;
                    }
                }
            }
        }
    }
}
