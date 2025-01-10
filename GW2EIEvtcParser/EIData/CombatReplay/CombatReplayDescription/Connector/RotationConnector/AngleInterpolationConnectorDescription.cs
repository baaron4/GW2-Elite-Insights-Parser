namespace GW2EIEvtcParser.EIData;

public class AngleInterpolationConnectorDescription : RotationConnectorDescription
{
    public int InterpolationMethod { get; private set; }
    public IReadOnlyList<float> Angles { get; private set; }
    internal AngleInterpolationConnectorDescription(AngleInterpolationConnector connector, CombatReplayMap map, ParsedEvtcLog log) : base(connector, map, log)
    {
        InterpolationMethod = (int)connector.Method;
        var angles = new List<float>();
        foreach (ParametricPoint1D angle in connector.Angles)
        {
            angles.Add(-angle.X);
            angles.Add(angle.Time);
        }
        Angles = angles;
    }
}
