using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.LogLogic.LogLogicTimeUtils;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal abstract class StoryInstance : LogLogic
{
    public StoryInstance(int triggerID) : base(triggerID)
    {
        LogCategoryInformation.Category = LogCategories.LogCategory.Story;
        LogCategoryInformation.SubCategory = LogCategories.SubLogCategory.Story;
        LogID |= LogIDs.LogMasks.StoryInstanceMask;
    }
    internal override LogData.Mode GetLogMode(CombatData combatData, AgentData agentData, LogData logData)
    {
        return LogData.Mode.Story;
    }

    internal override long GetLogOffset(EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData)
    {
        return GetGenericLogOffset(logData);
    }
    internal override IReadOnlyList<TargetID>  GetTargetsIDs()
    {
        return new[] { GetTargetID(GenericTriggerID) };
    }
}
