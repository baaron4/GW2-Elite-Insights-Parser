using System.Numerics;

namespace GW2EIEvtcParser.EIData;

internal class PositionConnector(in Vector3 position) : GeographicalConnector
{
    public readonly Vector3 Position = position;

    public override ConnectorDescription GetConnectedTo(CombatReplayMap map, ParsedEvtcLog log)
    {
        return new PositionConnectorDescription(this, map, log);
    }
}
