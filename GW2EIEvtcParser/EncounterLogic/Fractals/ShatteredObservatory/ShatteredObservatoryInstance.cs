using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class ShatteredObservatoryInstance : ShatteredObservatory
{
    public ShatteredObservatoryInstance(int triggerID) : base(triggerID)
    {
        EncounterID = EncounterIDs.EncounterMasks.Unsupported;
        Icon = InstanceIconShatteredObservatory;
        Extension = "shatrdobs";
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData)
    {
        return "Shattered Observatory";
    }

    internal override IReadOnlyList<TargetID> GetTargetsIDs()
    {
        return
        [
            TargetID.Skorvald,
            //TargetID.Artsariiv,
            TargetID.Arkk,
        ];
    }
}
