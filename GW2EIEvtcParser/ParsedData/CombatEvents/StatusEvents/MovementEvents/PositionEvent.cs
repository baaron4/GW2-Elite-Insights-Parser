using GW2EIEvtcParser.EIData;

namespace GW2EIEvtcParser.ParsedData
{
    public class PositionEvent : AbstractMovementEvent
    {

        internal PositionEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
        }

        internal override void AddPoint3D(CombatReplay replay)
        {
            (float x, float y, float z) = Unpack();
            if (x == 0.0f && y == 0.0f && z == 0.0f)
            {
                return;
            }
            replay.Positions.Add(new Point3D(x, y, z, Time));

        }
    }
}
