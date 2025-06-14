using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal abstract class UnknownFightLogic : FightLogic
{
    public UnknownFightLogic(int triggerID) : base(triggerID)
    {
        EncounterID = 0;
        SkillMode = SkillModeEnum.PvE;
        EncounterCategoryInformation.Category = FightCategory.UnknownEncounter;
        EncounterCategoryInformation.SubCategory = SubFightCategory.UnknownEncounter;
    }

    internal override void UpdatePlayersSpecAndGroup(IReadOnlyList<Player> players, CombatData combatData, FightData fightData)
    {
        // We don't know how an unknown fight could operate.
    }

    internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
    {
        return FightData.EncounterMode.Unknown;
    }
    protected override Dictionary<TargetID, int> GetTargetsSortIDs()
    {
        return [];
    }
}
