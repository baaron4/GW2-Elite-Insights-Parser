namespace GW2EIEvtcParser.EIData;

public class PositionToAgentConnectorDescription : AgentConnectorDescription
{
    public readonly IReadOnlyList<float> Position;
    public readonly float Velocity;
    internal PositionToAgentConnectorDescription(PositionToAgentConnector connector, CombatReplayMap map, ParsedEvtcLog log) : base(connector, map, log)
    {
        (float x, float y) = map.GetMapCoordRounded(connector.Position.XY());
        Position = [x, y, connector.StartTime];
        Velocity = connector.Velocity;
    }
}
