using System.Numerics;

namespace GW2EIEvtcParser.EIData;

internal class AngleConnector : RotationConnector
{
    /// <summary>
    /// Angle around Z axis in degrees
    /// </summary>
    public readonly float Angle;

    public AngleConnector(float angle)
    {
        Angle = angle;
    }

    public AngleConnector(in Vector3 facingDirection)
    {
        Angle = facingDirection.GetRoundedZRotationDeg();
    }

    public override ConnectorDescription GetConnectedTo(CombatReplayMap map, ParsedEvtcLog log)
    {
        return new AngleConnectorDescription(this, map, log);
    }
}
