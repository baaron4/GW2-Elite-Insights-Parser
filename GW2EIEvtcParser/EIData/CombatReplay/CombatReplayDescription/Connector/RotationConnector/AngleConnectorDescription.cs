namespace GW2EIEvtcParser.EIData;

public class AngleConnectorDescription : RotationConnectorDescription
{
    public readonly float Angle;
    internal AngleConnectorDescription(AngleConnector connector, CombatReplayMap map, ParsedEvtcLog log) : base(connector, map, log)
    {
        Angle = -connector.Angle;
    }
}
