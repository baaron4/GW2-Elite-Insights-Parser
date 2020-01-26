using GW2EIParser.EIData;

namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public class PositionEvent : AbstractMovementEvent
    {

        public PositionEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
        }

        public override void AddPoint3D(CombatReplay replay)
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
