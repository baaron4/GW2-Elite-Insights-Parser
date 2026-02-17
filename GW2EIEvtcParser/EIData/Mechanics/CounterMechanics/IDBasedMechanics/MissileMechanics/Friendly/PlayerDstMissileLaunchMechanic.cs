using System.Diagnostics.CodeAnalysis;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal class PlayerDstMissileLaunchMechanic : DstMissileLaunchMechanic
{

    public PlayerDstMissileLaunchMechanic(long mechanicID, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : this([mechanicID], plotlySetting, shortName, description, fullName, internalCoolDown)
    {
    }

    public PlayerDstMissileLaunchMechanic(long[] mechanicIDs, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(mechanicIDs, plotlySetting, shortName, description, fullName, internalCoolDown)
    {
    }
    protected override bool TryGetActor(ParsedEvtcLog log, AgentItem agentItem, Dictionary<int, SingleActor> regroupedMobs, [NotNullWhen(true)] out SingleActor? actor)
    {
        actor = MechanicHelper.FindPlayerActor(log, agentItem);
        return actor != null;
    }
}
