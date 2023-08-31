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
            ParametricPoint3D point = GetParametricPoint3D();
            if (point.X == 0.0f && point.Y == 0.0f && point.Z == 0.0f)
            {
                return;
            }
            replay.Positions.Add(point);
        }
    }
}
