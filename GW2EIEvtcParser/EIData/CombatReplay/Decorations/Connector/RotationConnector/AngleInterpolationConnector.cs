namespace GW2EIEvtcParser.EIData;

internal class AngleInterpolationConnector : RotationConnector
{
    /// <summary>
    /// Points for angles, in degrees, around Z axis
    /// </summary>
    public readonly IReadOnlyList<ParametricPoint1D> Angles;

    public readonly InterpolationMethod Method;

    public AngleInterpolationConnector(IReadOnlyList<ParametricPoint1D> angles, InterpolationMethod interpolationMethod = InterpolationMethod.Linear)
    {
        Angles = angles;
        Method = interpolationMethod;
    }

    /// <summary>
    /// Will create an AngleInterpolationConnector using the angle around Z axis for each (destination - origin) vectors
    /// </summary>
    /// <param name="originPoints">Origin points</param>
    /// <param name="destinationPoints">Destination</param>
    /// <param name="interpolationMethod"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public AngleInterpolationConnector(IReadOnlyList<ParametricPoint3D> originPoints, IReadOnlyList<ParametricPoint3D> destinationPoints, InterpolationMethod interpolationMethod = InterpolationMethod.Linear)
    {
        if (!originPoints.Any() || !destinationPoints.Any())
        {
            throw new InvalidOperationException("Must at least have one point");
        }

        var angles = new List<ParametricPoint1D>();
        for (int i = 0; i < Math.Min(originPoints.Count, destinationPoints.Count); i++)
        {
            long time = originPoints[i].Time;
            if (time != destinationPoints[i].Time)
            {
                throw new InvalidOperationException("Origin and Destination points must have the same timestamp");
            }

            var facing = destinationPoints[i].XYZ - originPoints[i].XYZ;
            float angle = facing.GetRoundedZRotationDeg();
            angles.Add(new ParametricPoint1D(angle, time));
        }
        Angles = angles;
        Method = interpolationMethod;
    }

    public override ConnectorDescription GetConnectedTo(CombatReplayMap map, ParsedEvtcLog log)
    {
        return new AngleInterpolationConnectorDescription(this, map, log);
    }
}
