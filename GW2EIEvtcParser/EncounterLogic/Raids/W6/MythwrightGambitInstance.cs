using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class MythwrightGambitInstance : MythwrightGambit
{
    public MythwrightGambitInstance(int triggerID) : base(triggerID)
    {
        EncounterID = EncounterIDs.EncounterMasks.Unsupported;
        Icon = InstanceIconMythwrightGambit;
        Extension = "mythgamb";
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData)
    {
        return "Mythwright Gambit";
    }

    internal override IReadOnlyList<TargetID> GetTargetsIDs()
    {
        return
        [
            // TargetID.ConjuredAmalgamate,
            TargetID.Nikare,
            TargetID.Kenut,
            TargetID.Qadim,
        ];
    }
    internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
    {
        return FightData.EncounterMode.NotApplicable;
    }
}
