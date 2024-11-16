using System.Numerics;

namespace GW2EIEvtcParser.EIData;

public class AngleConnector : RotationConnector
{
    /// <summary>
    /// Angle around Z axis in degrees
    /// </summary>
    protected float StartAngle;

    /// <summary>
    /// Angle speed around Z axis in degrees
    /// </summary>
    protected float SpinAngle;

    public AngleConnector(float startAngle)
    {
        StartAngle = startAngle;
        SpinAngle = 0;
    }

    public AngleConnector(in Vector3 facingDirection)
    {
        StartAngle = facingDirection.GetRoundedZRotationDeg();
        SpinAngle = 0;
    }

    public AngleConnector(float startAngle, float spinAngle) : this(startAngle)
    {
        SpinAngle = spinAngle;
    }

    public AngleConnector(in Vector3 facingDirection, float spinAngle) : this(facingDirection)
    {
        SpinAngle = spinAngle;
    }

    public class AngleConnectorDescriptor : RotationConnectorDescriptor
    {
        public readonly IReadOnlyList<float> Angles;
        public AngleConnectorDescriptor(AngleConnector connector, CombatReplayMap map) : base(connector, map)
        {
            Angles = [
                -connector.StartAngle,
                -connector.SpinAngle,
            ];
        }
    }

    public override object GetConnectedTo(CombatReplayMap map, ParsedEvtcLog log)
    {
        return new AngleConnectorDescriptor(this, map);
    }
}
