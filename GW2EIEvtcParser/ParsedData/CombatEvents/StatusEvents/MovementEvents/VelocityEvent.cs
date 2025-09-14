using GW2EIEvtcParser.EIData;

namespace GW2EIEvtcParser.ParsedData;

public class VelocityEvent : MovementEvent
{

    internal VelocityEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
    {
    }

    internal override void AddPoint3D(CombatReplay replay)
    {
        replay.AddVelocity(GetParametricPoint3D());
    }
}
