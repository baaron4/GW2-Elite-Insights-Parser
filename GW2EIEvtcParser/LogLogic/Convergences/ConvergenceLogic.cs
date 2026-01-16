using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.LogLogic.LogCategories;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.LogLogic;

internal abstract class ConvergenceLogic : LogLogic
{
    protected ConvergenceLogic(int triggerID) : base(triggerID)
    {
        ParseMode = ParseModeEnum.Instanced50;
        SkillMode = SkillModeEnum.PvE;
        MechanicList.Add(new MechanicGroup(
        [
            new PlayerDstBuffApplyMechanic([KryptisEssence, CalibratedEssence], new MechanicPlotlySetting(Symbols.CircleOpenDot, Colors.LightBlue), "Essence", "Collected Essence", "Essence Gain", 0),
        ]));
        LogCategoryInformation.Category = LogCategory.Convergence;
        LogID |= LogIDs.LogMasks.ConvergenceMask;
    }

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, LogData logData, IReadOnlyCollection<AgentItem> playerAgents, LogData.LogSuccessHandler successHandler)
    {
        RewardEvent? reward = combatData.GetRewardEvents().FirstOrDefault(x =>
            x.RewardType == RewardTypes.ConvergenceReward1 ||
            x.RewardType == RewardTypes.ConvergenceReward2);
        if (reward != null)
        {
            successHandler.SetSuccess(true, reward.Time);
        }
    }
}
