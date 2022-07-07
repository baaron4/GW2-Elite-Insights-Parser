using GW2EIEvtcParser.EIData;

namespace GW2EIEvtcParser.ParsedData
{
    public class RotationEvent : AbstractMovementEvent
    {

        internal RotationEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
        }

        internal override void AddPoint3D(CombatReplay replay)
        {
            (float x, float y, _) = Unpack();
            replay.Rotations.Add(new Point3D(x, y, 0, Time));
        }
    }
}
