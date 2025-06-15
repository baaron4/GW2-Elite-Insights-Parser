using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class HallOfChainsInstance : HallOfChains
{
    public HallOfChainsInstance(int triggerID) : base(triggerID)
    {
        EncounterID = EncounterIDs.Unknown;
        Icon = InstanceIconHallOfChains;
        Extension = "hallchains";
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData)
    {
        return "Hall Of Chains";
    }

    protected override IReadOnlyList<TargetID> GetTargetsIDs()
    {
        return
        [
            TargetID.SoullessHorror,
            TargetID.BrokenKing,
            TargetID.EaterOfSouls,
            TargetID.EyeOfFate,
            TargetID.EyeOfJudgement,
            TargetID.Dhuum,
        ];
    }
}
