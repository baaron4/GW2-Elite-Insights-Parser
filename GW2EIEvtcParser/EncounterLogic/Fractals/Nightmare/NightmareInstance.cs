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
        EncounterID = EncounterIDs.Unknown;
        Icon = InstanceIconNightmare;
        Extension = "nightmare";
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData)
    {
        return "Nightmare";
    }

    protected override IReadOnlyList<TargetID> GetTargetsIDs()
    {
        return
        [
            TargetID.MAMA,
            TargetID.Siax,
            TargetID.Ensolyss,
        ];
    }
    internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        fightData.SetSuccess(true, fightData.FightEnd);
    }
}
