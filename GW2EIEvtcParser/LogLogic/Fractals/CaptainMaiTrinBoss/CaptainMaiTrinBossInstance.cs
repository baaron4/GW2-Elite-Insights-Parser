using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using GW2EIGW2API;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicTimeUtils;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal class CaptainMaiTrinBossInstance : CaptainMaiTrinBoss
{
    public CaptainMaiTrinBossInstance(int triggerID) : base(triggerID)
    {
        LogID = LogIDs.LogMasks.Unsupported;
        Icon = InstanceIconCaptainMaiTrin;
        Extension = "captnmai";
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData, GW2APIController apiController)
    {
        return "Captain Mai Trin Boss Fractal"; ;
    }

    internal override IReadOnlyList<TargetID> GetTargetsIDs()
    {
        return
        [
            TargetID.MaiTrinFract,
        ];
    }
    internal override void CheckSuccess(CombatData combatData, AgentData agentData, LogData logData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        logData.SetSuccess(true, logData.LogEnd);
    }
}
