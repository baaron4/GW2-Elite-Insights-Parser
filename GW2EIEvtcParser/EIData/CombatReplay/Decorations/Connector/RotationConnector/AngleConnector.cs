using System.Numerics;

namespace GW2EIEvtcParser.EIData;

internal class AngleConnector : RotationConnector
{
    /// <summary>
    /// Angle around Z axis in degrees
    /// </summary>
    public readonly float StartAngle;

    /// <summary>
    /// Angle speed around Z axis in degrees
    /// </summary>
    public readonly float SpinAngle;

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

    public override ConnectorDescription GetConnectedTo(CombatReplayMap map, ParsedEvtcLog log)
    {
        return new AngleConnectorDescription(this, map, log);
    }
}
