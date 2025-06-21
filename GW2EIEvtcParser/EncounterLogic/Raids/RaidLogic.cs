using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal abstract class RaidLogic : FightLogic
{

    protected RaidLogic(int triggerID) : base(triggerID)
    {
        ParseMode = ParseModeEnum.Instanced10;
        SkillMode = SkillModeEnum.PvE;
        EncounterCategoryInformation.Category = FightCategory.Raid;
        EncounterID |= EncounterIDs.EncounterMasks.RaidMask;
    }

    internal static RewardEvent? GetOldRaidReward2Event(CombatData combatData, long start, long end)
    {
        return combatData.GetRewardEvents().FirstOrDefault(x => x.RewardType == RewardTypes.OldRaidReward2 && x.Time > start && x.Time < end);
    }

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        if (IsInstance)
        {
            // Raid instances remember last status, killing last boss is not an indication of a successful instance
            fightData.SetSuccess(true, fightData.FightEnd);
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
        RewardEvent? reward = rewards.FirstOrDefault(x => raidRewardsTypes.Contains(x.RewardType) && x.Time > fightData.FightStart);
        if (reward != null)
        {
            fightData.SetSuccess(true, reward.Time);
        }
        else
        {
            NoBouncyChestGenericCheckSucess(combatData, agentData, fightData, playerAgents);
        }
    }

    internal override FightData.EncounterStartStatus GetEncounterStartStatus(CombatData combatData, AgentData agentData, FightData fightData)
    {
        if (IsInstance)
        {
            return base.GetEncounterStartStatus(combatData, agentData, fightData);
        }
        if (TargetHPPercentUnderThreshold(GenericTriggerID, fightData.FightStart, combatData, Targets))
        {
            return FightData.EncounterStartStatus.Late;
        }
        return FightData.EncounterStartStatus.Normal;
    }

    internal override IReadOnlyList<TargetID>  GetTargetsIDs()
    {
        return new[] { GetTargetID(GenericTriggerID) };
    }
}
