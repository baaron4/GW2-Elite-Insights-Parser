namespace GW2EIEvtcParser.EIData;

public class InterpolationConnectorDescription : GeographicalConnectorDescription
{
    public int InterpolationMethod { get; private set; }
    public IReadOnlyList<float> Positions { get; private set; }
    internal InterpolationConnectorDescription(InterpolationConnector connector, CombatReplayMap map, ParsedEvtcLog log) : base(connector, map, log)
    {
        InterpolationMethod = (int)connector.Method;
        var positions = new List<float>();
        foreach (ParametricPoint3D pos in connector.Positions)
        {
            (float x, float y) = map.GetMapCoordRounded(pos.XYZ.XY());
            positions.Add(x);
            positions.Add(y);
            positions.Add(pos.Time);
        }
        Positions = positions;
    }
}
