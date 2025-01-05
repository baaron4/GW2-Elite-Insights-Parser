namespace GW2EIEvtcParser.EIData;

public class ScreenSpaceConnectorDescription : ConnectorDescription
{
    public readonly IReadOnlyList<float> Position;
    public readonly bool IsScreenSpace;
    internal ScreenSpaceConnectorDescription(ScreenSpaceConnector connector, CombatReplayMap map, ParsedEvtcLog log) : base(connector, map, log)
    {
        IsScreenSpace = true;
        Position = [connector.Position.X, connector.Position.Y]; 
    }
}
