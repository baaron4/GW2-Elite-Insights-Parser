namespace GW2EIEvtcParser.EIData;

public class SpinningConnectorDescription : AngleConnectorDescription
{
    public readonly float SpinAngle;
    internal SpinningConnectorDescription(SpinningConnector connector, CombatReplayMap map, ParsedEvtcLog log) : base(connector, map, log)
    {
        SpinAngle = -connector.SpinAngle;
    }
}
