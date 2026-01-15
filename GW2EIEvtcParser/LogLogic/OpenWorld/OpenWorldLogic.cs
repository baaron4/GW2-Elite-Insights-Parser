using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal abstract class OpenWorldLogic : LogLogic
{
    public OpenWorldLogic(int triggerID) : base(triggerID)
    {
        ParseMode = ParseModeEnum.OpenWorld;
        SkillMode = SkillModeEnum.PvE;
        LogCategoryInformation.Category = LogCategories.LogCategory.OpenWorld;
        LogCategoryInformation.SubCategory = LogCategories.SubLogCategory.OpenWorld;
        LogID |= LogIDs.LogMasks.OpenWorldMask;
    }
    internal override IReadOnlyList<TargetID>  GetTargetsIDs()
    {
        return new[] { GetTargetID(GenericTriggerID) };
    }
    internal override LogData.Mode GetLogMode(CombatData combatData, AgentData agentData, LogData logData)
    {
        return LogData.Mode.NotApplicable;
    }
}
