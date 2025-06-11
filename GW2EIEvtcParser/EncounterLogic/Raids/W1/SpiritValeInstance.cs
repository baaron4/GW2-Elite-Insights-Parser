using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class SpiritValeInstance : SpiritVale
{
    public SpiritValeInstance(int triggerID) : base(triggerID)
    {
        EncounterID = EncounterIDs.EncounterMasks.Unsupported;
        Icon = InstanceIconSpiritVale;
        Extension = "sprtvale";
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData)
    {
        return "Spirit Vale";
    }

    internal override IReadOnlyList<TargetID> GetTargetsIDs()
    {
        return
        [
            TargetID.ValeGuardian,
            //TargetID.EtherealBarrier,
            TargetID.Gorseval,
            TargetID.Sabetha,
        ];
    }
}
