using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class DeepstoneInstance : FractalLogic
{
    public DeepstoneInstance(int triggerID) : base(triggerID)
    {
        EncounterID = EncounterIDs.Unknown;
        Icon = InstanceIconDeepstone;
        Extension = "deepstone";
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData)
    {
        return "Deepstone";
    }

    protected override IReadOnlyList<TargetID> GetTargetsIDs()
    {
        return
        [
            TargetID.TheVoice,
        ];
    }
}
