namespace GW2EIEvtcParser.EIData;

public abstract class GeographicalConnectorDescription : ConnectorDescription
{
    public readonly IReadOnlyList<float>? Offset;
    public readonly bool OffsetAfterRotation;
    public readonly bool IsScreenSpace;
    internal GeographicalConnectorDescription(GeographicalConnector connector, CombatReplayMap map, ParsedEvtcLog log) : base(connector, map, log)
    {
        if (connector.Offset.HasValue)
        {
            OffsetAfterRotation = connector.OffsetAfterRotation;
            Offset = [
                connector.Offset.Value.X,
                connector.InvertYOffset ? -connector.Offset.Value.Y : connector.Offset.Value.Y,
            ];
        }
        IsScreenSpace = false;
    }
}
