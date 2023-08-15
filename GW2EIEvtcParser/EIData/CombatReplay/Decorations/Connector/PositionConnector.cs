using System;

namespace GW2EIEvtcParser.EIData
{
    internal class PositionConnector : Connector
    {
        protected Point3D Position { get; set; }

        public PositionConnector(Point3D position)
        {
            Position = position;
        }

        public override object GetConnectedTo(CombatReplayMap map, ParsedEvtcLog log)
        {
            (float x, float y) = map.GetMapCoord(Position.X, Position.Y);
            return new float[2] { x, y };
        }

        /// <summary>
        /// Creates a new <see cref="PositionConnector"/> offset by the specified amount in the orientation given in <b>radians</b>. 
        /// </summary>
        public PositionConnector WithOffset(float orientation, float amount)
        {
            Point3D offset = amount * new Point3D(-(float)Math.Sin(orientation), (float)Math.Cos(orientation));
            return new PositionConnector(Position + offset);
        }
    }
}
