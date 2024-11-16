using System.Numerics;

namespace GW2EIEvtcParser.EIData;

public class PositionConnector(in Vector3 position) : GeographicalConnector
{
    protected Vector3 Position = position;

    public class PositionConnectorDescriptor : GeographicalConnectorDescriptor
    {
        public readonly IReadOnlyList<float> Position;
        public PositionConnectorDescriptor(PositionConnector connector, CombatReplayMap map) : base(connector, map)
        {
            (float x, float y) = map.GetMapCoordRounded(connector.Position.XY());
            Position = [ x, y ]; //TODO(Rennorb) @perf
        }
    }

    public override object GetConnectedTo(CombatReplayMap map, ParsedEvtcLog log)
    {
        return new PositionConnectorDescriptor(this, map);
    }
}
