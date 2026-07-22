using System.Diagnostics.CodeAnalysis;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal abstract class PlayerBuffRemoveMechanic<T> : BuffRemoveMechanic<T> where T : AbstractBuffRemoveEvent
{
    public PlayerBuffRemoveMechanic(long mechanicID, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, int internalCoolDown) : base(mechanicID, id, plotlySetting, description, severity, internalCoolDown)
    {
    }

    public PlayerBuffRemoveMechanic(long[] mechanicIDs, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, int internalCoolDown) : base(mechanicIDs, id, plotlySetting, description, severity, internalCoolDown)
    {
    }
    protected override bool TryGetActor(ParsedEvtcLog log, AgentItem agentItem, Dictionary<int, SingleActor> regroupedMobs, [NotNullWhen(true)] out SingleActor? actor)
    {
        actor = MechanicHelper.FindPlayerActor(log, agentItem);
        return actor != null;
    }
}
