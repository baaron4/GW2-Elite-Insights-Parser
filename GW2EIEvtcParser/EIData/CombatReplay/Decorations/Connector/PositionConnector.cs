using System;
using System.Collections.Generic;

namespace GW2EIEvtcParser.EIData
{
    internal class PositionConnector : Connector
    {
        protected Point3D Position { get; set; }

        public PositionConnector(Point3D position)
        {
            Position = position;
        }

        protected class PositionConnectorDescriptor : ConnectorDescriptor
        {
            public IReadOnlyList<float> Position { get; private set; }
            public PositionConnectorDescriptor(PositionConnector connector, CombatReplayMap map) : base(connector, map)
            {
                (float x, float y) = map.GetMapCoord(connector.Position.X, connector.Position.Y);
                Position = new List<float>() { x, y };
            }
        }

        public override ConnectorDescriptor GetConnectedTo(CombatReplayMap map, ParsedEvtcLog log)
        {
            return new PositionConnectorDescriptor(this, map);
        }
    }
}
