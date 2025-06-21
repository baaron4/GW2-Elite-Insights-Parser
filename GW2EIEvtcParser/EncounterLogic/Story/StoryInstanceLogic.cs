using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal abstract class StoryInstance : FightLogic
{
    public StoryInstance(int triggerID) : base(triggerID)
    {
        EncounterCategoryInformation.Category = EncounterCategory.FightCategory.Story;
        EncounterCategoryInformation.SubCategory = EncounterCategory.SubFightCategory.Story;
        EncounterID |= EncounterIDs.EncounterMasks.StoryInstanceMask;
    }
    internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
    {
        return FightData.EncounterMode.Story;
    }

    internal override long GetFightOffset(EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData)
    {
        return GetGenericFightOffset(fightData);
    }
    internal override IReadOnlyList<TargetID>  GetTargetsIDs()
    {
        return new[] { GetTargetID(GenericTriggerID) };
    }
}
