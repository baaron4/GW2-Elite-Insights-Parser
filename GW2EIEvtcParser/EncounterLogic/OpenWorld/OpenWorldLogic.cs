using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal abstract class OpenWorldLogic : FightLogic
{
    public OpenWorldLogic(int triggerID) : base(triggerID)
    {
        ParseMode = ParseModeEnum.OpenWorld;
        SkillMode = SkillModeEnum.PvE;
        EncounterCategoryInformation.Category = EncounterCategory.FightCategory.OpenWorld;
        EncounterCategoryInformation.SubCategory = EncounterCategory.SubFightCategory.OpenWorld;
        EncounterID |= EncounterIDs.EncounterMasks.OpenWorldMask;
    }
    internal override IReadOnlyList<TargetID>  GetTargetsIDs()
    {
        return new[] { GetTargetID(GenericTriggerID) };
    }
    internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
    {
        return FightData.EncounterMode.NotApplicable;
    }
}
