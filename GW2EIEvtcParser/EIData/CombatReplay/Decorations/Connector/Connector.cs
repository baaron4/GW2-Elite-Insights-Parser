using System.Collections.Generic;
using System;

namespace GW2EIEvtcParser.EIData
{
    internal abstract class Connector
    {
        private Point3D Offset { get; set; }

        private bool OffsetAfterRotation { get; set; }
        public abstract class ConnectorDescriptor
        {
            public IReadOnlyList<float> Offset { get; private set; }
            public bool OffsetAfterRotation { get; private set; }

            public ConnectorDescriptor(Connector connector, CombatReplayMap map)
            {
                OffsetAfterRotation = connector.OffsetAfterRotation;
                //
                if (connector.Offset != null)
                {
                    var positions = new List<float>();
                    (float x, float y) = map.GetMapCoord(connector.Offset.X, connector.Offset.Y);
                    positions.Add(x);
                    positions.Add(y);
                    Offset = positions;
                }
            }
        }

        /// <summary>
        /// Adds an offset by the specified amount in the orientation given in <b>radians</b>. 
        /// </summary>
        public Connector WithOffset(float orientation, float amount, bool afterRotation = false)
        {
            Point3D offset = amount * new Point3D(-(float)Math.Sin(orientation), (float)Math.Cos(orientation));
            Offset = offset;
            OffsetAfterRotation = afterRotation;
            return this;
        }

        /// <summary>
        /// Adds an offset by the specified Point3D. 
        /// </summary>
        public Connector WithOffset(Point3D offset, bool afterRotation = false)
        {
            Offset = offset;
            OffsetAfterRotation = afterRotation;
            return this;
        }

        public abstract ConnectorDescriptor GetConnectedTo(CombatReplayMap map, ParsedEvtcLog log);
    }
}
