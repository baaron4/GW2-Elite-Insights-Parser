using System;
using GW2EIEvtcParser.EIData;

namespace GW2EIEvtcParser.ParsedData
{
    public abstract class AbstractMovementEvent : AbstractStatusEvent
    {
        private readonly ulong _dstAgent;
        private readonly int _value;

        internal AbstractMovementEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
            _dstAgent = evtcItem.DstAgent;
            _value = evtcItem.Value;
        }

        private static unsafe (float x, float y, float z) UnpackMovementData(ulong packedXY, int intZ)
        {
            return (*(float*)&packedXY, *((float*)&packedXY + 1), *(float*)&intZ);
        }

        public ParametricPoint3D GetParametricPoint3D()
        {
            (var x, var y, var z) = UnpackMovementData(_dstAgent, _value);
            return new ParametricPoint3D(x, y, z, Time);
        }

        public Point3D GetPoint3D()
        {
            return GetParametricPoint3D();
        }

        /// <summary>
        /// Uses <see cref="UnpackMovementData(ulong, int)"/> to get X, Y, Z coordinates.<br></br>
        /// Converts the coordinate points to a <see cref="Point3D"/> to access the class methods.
        /// </summary>
        /// <param name="evt">CombatItem</param>
        /// <returns><see cref="Point3D"/> containing coordinates obtained from <paramref name="evt"/>.</returns>
        internal static Point3D GetPoint3D(CombatItem evt)
        {
            (var x, var y, var z) = UnpackMovementData(evt.DstAgent, evt.Value);
            return new Point3D(x, y, z);
        }

        internal abstract void AddPoint3D(CombatReplay replay);
    }
}
