using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;

namespace GW2EIEvtcParser.EncounterLogic;

internal abstract class ConvergenceLogic : FightLogic
{
    protected ConvergenceLogic(int triggerID) : base(triggerID)
    {
        ParseMode = ParseModeEnum.Instanced50;
        SkillMode = SkillModeEnum.PvE;
        MechanicList.Add(new MechanicGroup(
        [
            
        ]));
        EncounterCategoryInformation.Category = FightCategory.Convergence;
        EncounterID |= EncounterIDs.EncounterMasks.ConvergenceMask;
    }

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        base.CheckSuccess(combatData, agentData, fightData, playerAgents);

        RewardEvent? reward = combatData.GetRewardEvents().FirstOrDefault(x =>
            x.RewardType == RewardTypes.ConvergenceReward1 ||
            x.RewardType == RewardTypes.ConvergenceReward2);
        if (reward != null)
        {
            fightData.SetSuccess(true, reward.Time);
        }
    }
}
