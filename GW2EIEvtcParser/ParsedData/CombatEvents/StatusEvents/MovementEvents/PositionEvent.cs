using GW2EIEvtcParser.EIData;

namespace GW2EIEvtcParser.ParsedData;

public class PositionEvent : MovementEvent
{
    internal PositionEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
    {
    }

    internal override void AddPoint3D(CombatReplay replay)
    {
        ParametricPoint3D point = GetParametricPoint3D();
        if (point.XYZ == default || point.IsNaNOrInfinity() || point.XYZ.XY().LengthSquared() > 16e8) // XY bigger than 40000
        {
            return;
        }
        replay.AddPosition(point);
    }
}
