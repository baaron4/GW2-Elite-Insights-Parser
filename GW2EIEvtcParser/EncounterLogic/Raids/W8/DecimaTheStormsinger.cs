
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;

namespace GW2EIEvtcParser.EncounterLogic;

internal class DecimaTheStormsinger : MountBalrior
{
    public DecimaTheStormsinger(int triggerID) : base(triggerID)
    {
        Extension = "decima";
        EncounterCategoryInformation.InSubCategoryOrder = 1;
        EncounterID |= 0x000002;
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData)
    {
        return "Decima, the Stormsinger";
    }
}
