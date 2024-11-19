
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;

namespace GW2EIEvtcParser.EncounterLogic;

internal class UraTheSteamshrieker : MountBalrior
{
    public UraTheSteamshrieker(int triggerID) : base(triggerID)
    {
        Extension = "ura";
        EncounterCategoryInformation.InSubCategoryOrder = 2;
        EncounterID |= 0x000003;
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData)
    {
        return "Ura, the Steamshrieker";
    }
}
