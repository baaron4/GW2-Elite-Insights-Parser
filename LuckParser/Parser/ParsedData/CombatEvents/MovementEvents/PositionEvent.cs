using LuckParser.EIData;

namespace LuckParser.Parser.ParsedData.CombatEvents
{
    public class PositionEvent : AbstractMovementEvent
    {

        public PositionEvent(CombatItem evtcItem, AgentData agentData, long offset) : base(evtcItem, agentData, offset)
        {
        }

        public override void AddPoint3D(CombatReplay replay)
        {
            (float x, float y, float z) = Unpack();
            replay.Positions.Add(new Point3D(x, y, z, Time));

        }
    }
}
