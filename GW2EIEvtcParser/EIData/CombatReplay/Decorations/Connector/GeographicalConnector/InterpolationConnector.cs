namespace GW2EIEvtcParser.EIData;

internal class InterpolationConnector : GeographicalConnector
{
    public readonly IReadOnlyList<ParametricPoint3D> Positions;

    public readonly InterpolationMethod Method;

    public InterpolationConnector(IReadOnlyList<ParametricPoint3D> positions, InterpolationMethod interpolationMethod = InterpolationMethod.Linear)
    {
        if (!positions.Any())
        {
            throw new InvalidOperationException("Must at least have one point");
        }
        Positions = positions;
        Method = interpolationMethod;
    }

    public override ConnectorDescription GetConnectedTo(CombatReplayMap map, ParsedEvtcLog log)
    {
        return new InterpolationConnectorDescription(this, map, log);
    }
}
