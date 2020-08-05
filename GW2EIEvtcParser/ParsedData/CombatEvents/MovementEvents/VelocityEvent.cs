using GW2EIEvtcParser.EIData;

namespace GW2EIEvtcParser.ParsedData
{
    public class VelocityEvent : AbstractMovementEvent
    {

        internal VelocityEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
        }

        internal override void AddPoint3D(CombatReplay replay)
        {
            (float x, float y, float z) = Unpack();
            replay.Velocities.Add(new Point3D(x, y, z, Time));
        }
    }
}
