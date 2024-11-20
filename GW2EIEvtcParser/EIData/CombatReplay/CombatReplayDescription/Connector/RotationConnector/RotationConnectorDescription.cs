namespace GW2EIEvtcParser.EIData;

public abstract class RotationConnectorDescription : ConnectorDescription
{
    internal RotationConnectorDescription(RotationConnector connector, CombatReplayMap map, ParsedEvtcLog log) : base(connector, map, log)
    {
    }
}
