using System;
using GW2EIEvtcParser.EIData;

namespace GW2EIEvtcParser.ParsedData
{
    public class PositionEvent : AbstractMovementEvent
    {
        internal PositionEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
        }

        internal Point3D ToPoint()
        {
            (float x, float y, float z) = Unpack();
            return new Point3D(x, y, z);
        }

        internal ParametricPoint3D ToParametricPoint()
        {
            (float x, float y, float z) = Unpack();
            return new ParametricPoint3D(x, y, z, Time);
        }

        internal override void AddPoint3D(CombatReplay replay)
        {
            (float x, float y, float z) = Unpack();
            if (x == 0.0f && y == 0.0f && z == 0.0f)
            {
                return;
            }
            replay.Positions.Add(ToParametricPoint());
        }
    }
}
