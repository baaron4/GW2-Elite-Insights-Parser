using System.Numerics;

namespace GW2EIEvtcParser.EIData;

public class PositionConnector(in Vector3 position) : GeographicalConnector
{
    protected Vector3 Position = position;

    public class PositionConnectorDescriptor : GeographicalConnectorDescriptor
    {
        public IReadOnlyList<float> Position { get; private set; }
        public PositionConnectorDescriptor(PositionConnector connector, CombatReplayMap map) : base(connector, map)
        {
            (float x, float y) = map.GetMapCoord(connector.Position.X, connector.Position.Y);
            Position = new List<float>() { x, y }; //TODO(Rennorb) @perf
        }
    }

    public override object GetConnectedTo(CombatReplayMap map, ParsedEvtcLog log)
    {
        return new PositionConnectorDescriptor(this, map);
    }
}
