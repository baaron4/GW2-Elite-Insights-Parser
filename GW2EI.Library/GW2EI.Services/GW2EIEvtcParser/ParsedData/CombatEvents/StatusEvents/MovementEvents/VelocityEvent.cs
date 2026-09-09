using GW2EIEvtcParser.EIData;

namespace GW2EIEvtcParser.ParsedData;

public class VelocityEvent : MovementEvent
{

    public float Velocity => Point3D.Length();

    internal VelocityEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
    {
    }

    internal override void AddPoint3D(CombatReplay replay)
    {
        var velocity = GetParametricPoint3D();
        if (velocity.IsNaNOrInfinity())
        {
            return;
        }
        replay.AddVelocity(velocity);
    }
}
