namespace GW2EIEvtcParser.EIData;

public class PositionConnectorDescription : GeographicalConnectorDescription
{
    public readonly IReadOnlyList<float> Position;
    internal PositionConnectorDescription(PositionConnector connector, CombatReplayMap map, ParsedEvtcLog log) : base(connector, map, log)
    {
        (float x, float y) = map.GetMapCoordRounded(connector.Position.XY());
        Position = [x, y]; //TODO_PERF(Rennorb)
    }
}
