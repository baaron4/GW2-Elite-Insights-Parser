
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class UraTheSteamshrieker : MountBalrior
{
    public UraTheSteamshrieker(int triggerID) : base(triggerID)
    {
        Extension = "ura";
        Icon = EncounterIconUra;
        EncounterCategoryInformation.InSubCategoryOrder = 2;
        EncounterID |= 0x000003;
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData)
    {
        return "Ura, the Steamshrieker";
    }
}
