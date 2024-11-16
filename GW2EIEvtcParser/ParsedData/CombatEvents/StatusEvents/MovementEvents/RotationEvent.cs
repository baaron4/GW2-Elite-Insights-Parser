using GW2EIEvtcParser.EIData;

namespace GW2EIEvtcParser.ParsedData;

public class RotationEvent : MovementEvent
{

    internal RotationEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
    {
    }

    //TODO(Rennorb) @cleanup: hoist upwards or rename
    internal override void AddPoint3D(CombatReplay replay)
    {
        replay.Rotations.Add(GetParametricPoint3D());
    }
}
