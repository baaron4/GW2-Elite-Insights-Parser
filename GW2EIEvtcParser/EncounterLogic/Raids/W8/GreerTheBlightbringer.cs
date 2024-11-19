
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;

namespace GW2EIEvtcParser.EncounterLogic;

internal class GreerTheBlightbringer : MountBalrior
{
    public GreerTheBlightbringer(int triggerID) : base(triggerID)
    {
        Extension = "greer";
        EncounterCategoryInformation.InSubCategoryOrder = 0;
        EncounterID |= 0x000001;
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData)
    {
        return "Greer, the Blightbringer";
    }
}
