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

        private static (float x, float y, float z) UnpackMovementData(ulong packedXY, int intZ)
        {
            byte[] xyBytes = BitConverter.GetBytes(packedXY);
            byte[] zBytes = BitConverter.GetBytes(intZ);
            float x = BitConverter.ToSingle(xyBytes, 0);
            float y = BitConverter.ToSingle(xyBytes, 4);
            float z = BitConverter.ToSingle(zBytes, 0);

            return (x, y, z);
        }

        public ParametricPoint3D GetParametricPoint3D()
        {
            (var x, var y, var z) = UnpackMovementData(_dstAgent, _value);
            return new ParametricPoint3D(x, y, z, Time);
        }

        public Point3D GetPoint3D()
        {
            return GetPoint3D(_dstAgent, _value);
        }

        /// <summary>
        /// Uses <see cref="UnpackMovementData(ulong, int)"/> to get X, Y, Z coordinates.<br></br>
        /// Converts the coordinate points to a <see cref="Point3D"/> to access the class methods.
        /// </summary>
        /// <param name="packedXY"></param>
        /// <param name="intZ"></param>
        /// <returns><see cref="Point3D"/> containing coordinates obtained from <paramref name="packedXY"/> and <paramref name="intZ"/>.</returns>
        internal static Point3D GetPoint3D(ulong packedXY, int intZ)
        {
            (var x, var y, var z) = UnpackMovementData(packedXY, intZ);
            return new Point3D(x, y, z);
        }

        internal abstract void AddPoint3D(CombatReplay replay);
    }
}
