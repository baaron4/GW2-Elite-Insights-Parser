namespace GW2EIEvtcParser.EIData;

public class InterpolationConnector : GeographicalConnector
{
    protected IReadOnlyList<ParametricPoint3D> Positions;

    private readonly InterpolationMethod _method;

    public InterpolationConnector(IReadOnlyList<ParametricPoint3D> positions, InterpolationMethod interpolationMethod = InterpolationMethod.Linear)
    {
        if (!positions.Any())
        {
            throw new InvalidOperationException("Must at least have one point");
        }
        Positions = positions;
        _method = interpolationMethod;
    }

    public class InterpolationConnectorDescriptor : GeographicalConnectorDescriptor
    {
        public int InterpolationMethod { get; private set; }
        public IReadOnlyList<float> Positions { get; private set; }
        public InterpolationConnectorDescriptor(InterpolationConnector connector, CombatReplayMap map) : base(connector, map)
        {
            InterpolationMethod = (int)connector._method;
            var positions = new List<float>();
            foreach (ParametricPoint3D pos in connector.Positions)
            {
                (float x, float y) = map.GetMapCoordRounded(pos.Value.XY());
                positions.Add(x);
                positions.Add(y);
                positions.Add(pos.Time);
            }
            Positions = positions;
        }
    }

    public override object GetConnectedTo(CombatReplayMap map, ParsedEvtcLog log)
    {
        return new InterpolationConnectorDescriptor(this, map);
    }
}
