using System.Collections.Generic;
using System;

namespace GW2EIEvtcParser.EIData
{
    internal abstract class Connector
    {
        private Point3D Translation { get; set; }

        private bool TranslationAfterRotation { get; set; }
        public abstract class ConnectorDescriptor
        {
            public IReadOnlyList<float> Translation { get; private set; }
            public bool TranslationAfterRotation { get; private set; }

            public ConnectorDescriptor(Connector connector, CombatReplayMap map)
            {
                TranslationAfterRotation = connector.TranslationAfterRotation;
                //
                if (connector.Translation != null)
                {
                    var positions = new List<float>();
                    (float x, float y) = map.GetMapCoord(connector.Translation.X, connector.Translation.Y);
                    positions.Add(x);
                    positions.Add(y);
                    Translation = positions;
                }
            }
        }

        /// <summary>
        /// Adds an offset by the specified amount in the orientation given in <b>radians</b>. 
        /// </summary>
        public Connector WithOffset(float orientation, float amount, bool afterRotation = false)
        {
            Point3D offset = amount * new Point3D(-(float)Math.Sin(orientation), (float)Math.Cos(orientation));
            Translation = offset;
            TranslationAfterRotation = afterRotation;
            return this;
        }

        /// <summary>
        /// Adds an offset by the specified Point3D. 
        /// </summary>
        public Connector WithOffset(Point3D offset, bool afterRotation = false)
        {
            Translation = offset;
            TranslationAfterRotation = afterRotation;
            return this;
        }

        public abstract ConnectorDescriptor GetConnectedTo(CombatReplayMap map, ParsedEvtcLog log);
    }
}
