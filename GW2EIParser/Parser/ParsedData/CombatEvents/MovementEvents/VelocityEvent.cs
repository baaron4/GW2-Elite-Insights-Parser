using GW2EIParser.EIData;

namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public class VelocityEvent : AbstractMovementEvent
    {

        public VelocityEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
        }

        public override void AddPoint3D(CombatReplay replay)
        {
            (float x, float y, float z) = Unpack();
            replay.Velocities.Add(new Point3D(x, y, z, Time));
        }
    }
}
