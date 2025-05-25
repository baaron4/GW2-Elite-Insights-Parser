using System.Numerics;

namespace GW2EIEvtcParser.EIData;

internal class SpinningConnector : AngleConnector
{

    /// <summary>
    /// Total angle traversed around Z axis in degrees
    /// </summary>
    public readonly float SpinAngle;


    public SpinningConnector(float startAngle, float spinAngle) : base(startAngle)
    {
        SpinAngle = spinAngle;
    }

    public SpinningConnector(in Vector3 startFacingDirection, float spinAngle) : base(startFacingDirection)
    {
        SpinAngle = spinAngle;
    }

    public override ConnectorDescription GetConnectedTo(CombatReplayMap map, ParsedEvtcLog log)
    {
        return new SpinningConnectorDescription(this, map, log);
    }
}
