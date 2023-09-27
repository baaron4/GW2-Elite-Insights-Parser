using System.Collections.Generic;
using System;

namespace GW2EIEvtcParser.EIData
{
    internal abstract class GeographicalConnector : Connector
    {
        private Point3D Offset { get; set; }

        private bool OffsetAfterRotation { get; set; }
        public abstract class GeographicalConnectorDescriptor
        {
            public IReadOnlyList<float> Offset { get; private set; }
            public bool OffsetAfterRotation { get; private set; }

            public GeographicalConnectorDescriptor(GeographicalConnector connector, CombatReplayMap map)
            {
                //
                if (connector.Offset != null)
                {
                    OffsetAfterRotation = connector.OffsetAfterRotation;
                    var positions = new List<float>
                    {
                        connector.Offset.X,
                        connector.Offset.Y
                    };
                    Offset = positions;
                }
            }
        }

        /// <summary>
        /// Adds an offset by the specified amount in the orientation given in <b>radians</b>. 
        /// </summary>
        public GeographicalConnector WithOffset(float orientation, float amount, bool afterRotation)
        {
            orientation *= -1; // game is indirect
            Point3D offset = amount * new Point3D((float)Math.Cos(orientation), (float)Math.Sin(orientation));
            Offset = offset;
            OffsetAfterRotation = afterRotation;
            return this;
        }

        /// <summary>
        /// Adds an offset by the specified Point3D. 
        /// </summary>
        public GeographicalConnector WithOffset(Point3D offset, bool afterRotation)
        {
            Offset = offset;
            OffsetAfterRotation = afterRotation;
            return this;
        }
    }
}
