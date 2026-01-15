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
    }
    internal override IReadOnlyList<TargetID> GetTargetsIDs()
    {
        return new[] { GetTargetID(GenericTriggerID) };
    }
    internal override LogData.StartStatus GetLogStartStatus(CombatData combatData, AgentData agentData, LogData logData)
    {
        if (IsInstance)
        {
            return base.GetLogStartStatus(combatData, agentData, logData);
        }
        if (TargetHPPercentUnderThreshold(GenericTriggerID, logData.LogStart, combatData, Targets))
        {
            return LogData.StartStatus.Late;
        }
        return LogData.StartStatus.Normal;
    }
}
