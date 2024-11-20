namespace GW2EIEvtcParser.EIData;

public class AngleConnectorDescription : RotationConnectorDescription
{
    public readonly IReadOnlyList<float> Angles;
    internal AngleConnectorDescription(AngleConnector connector, CombatReplayMap map, ParsedEvtcLog log) : base(connector, map, log)
    {
        Angles = [
            -connector.StartAngle,
            -connector.SpinAngle,
        ];
    }
}
