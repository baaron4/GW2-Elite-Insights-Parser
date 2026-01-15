using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.LogLogic.LogCategories;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal abstract class UnknownEncounterLogic : LogLogic
{
    public UnknownEncounterLogic(int triggerID) : base(triggerID)
    {
        LogID = LogIDs.Unknown;
        SkillMode = SkillModeEnum.PvE;
        LogCategoryInformation.Category = LogCategory.UnknownEncounter;
        LogCategoryInformation.SubCategory = SubLogCategory.UnknownEncounter;
    }

    internal override void UpdatePlayersSpecAndGroup(IReadOnlyList<Player> players, CombatData combatData, LogData logData)
    {
        // We don't know how an unknown fight could operate.
    }

    internal override LogData.Mode GetLogMode(CombatData combatData, AgentData agentData, LogData logData)
    {
        return LogData.Mode.Unknown;
    }
    internal override Dictionary<TargetID, int> GetTargetsSortIDs()
    {
        return [];
    }
}
