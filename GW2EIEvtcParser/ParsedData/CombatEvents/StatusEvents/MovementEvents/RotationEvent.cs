using System.Drawing;
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
        var rotation = GetParametricPoint3D();
        if (rotation.IsNaNOrInfinity())
        {
            return;
        }
        replay.AddRotation(rotation);
    }
}
