using GW2EIEvtcParser.EIData;

namespace GW2EIEvtcParser.ParsedData;

public class TeleportEvent : MovementEvent
{
    public readonly uint SomethingBehaviorRelated1;
    public readonly byte SomethingBehaviorRelated2;
    internal TeleportEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
    {
        SomethingBehaviorRelated1 = evtcItem.OverstackValue;
        SomethingBehaviorRelated2 = evtcItem.IsOffcycle;
    }

    internal override void AddPoint3D(CombatReplay replay)
    {
        ParametricPoint3D point = GetParametricPoint3D();
        if (point.XYZ == default || point.IsNaNOrInfinity() || point.XYZ.XY().LengthSquared() > 16e8) // XY bigger than 40000
        {
            return;
        }
        replay.AddTeleport(point);
    }
}
