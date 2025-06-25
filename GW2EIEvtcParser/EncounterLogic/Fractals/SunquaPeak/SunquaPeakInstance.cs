using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;
internal class SunquaPeakInstance : SunquaPeak
{
    public SunquaPeakInstance(int triggerID) : base(triggerID)
    {
        EncounterID = EncounterIDs.EncounterMasks.Unsupported;
        Icon = InstanceIconSunquaPeak;
        Extension = "snqpeak";
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData)
    {
        return "Sunqua Peak";
    }

    internal override IReadOnlyList<TargetID> GetTargetsIDs()
    {
        return
        [
            TargetID.AiKeeperOfThePeak,
            TargetID.AiKeeperOfThePeak2,
        ];
    }
    internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        fightData.SetSuccess(true, fightData.FightEnd);
    }
}
