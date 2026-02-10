using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.LogLogic.LogCategories;
using static GW2EIEvtcParser.LogLogic.LogLogicTimeUtils;

namespace GW2EIEvtcParser.LogLogic;

internal abstract class SecretOfTheObscureRaidEncounter : RaidEncounterLogic
{
    public SecretOfTheObscureRaidEncounter(int triggerID) : base(triggerID)
    {
        LogCategoryInformation.SubCategory = SubLogCategory.SotO;
        LogID |= LogIDs.RaidEncounterMasks.SotOMask;
    }

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, LogData logData, IReadOnlyCollection<AgentItem> playerAgents, LogData.LogSuccessHandler successHandler)
    {
        CheckPostEODRewardSuccess(combatData, agentData, logData, playerAgents, successHandler);
    }
}
