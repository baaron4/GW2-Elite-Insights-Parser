using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class NightmareInstance : Nightmare
{
    public NightmareInstance(int triggerID) : base(triggerID)
    {
        EncounterID = 0;
        Icon = InstanceIconNightmare;
        Extension = "nightmare";
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData)
    {
        return "Nightmare";
    }

    internal override IReadOnlyList<TargetID> GetTargetsIDs()
    {
        return
        [
            TargetID.MAMA,
            TargetID.Siax,
            TargetID.Ensolyss,
        ];
    }
}
