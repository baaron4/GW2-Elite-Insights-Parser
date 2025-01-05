using System.Numerics;

namespace GW2EIEvtcParser.EIData;

internal class ScreenSpaceConnector(in Vector2 position) : Connector
{
    public readonly Vector2 Position = position;

    public override ConnectorDescription GetConnectedTo(CombatReplayMap map, ParsedEvtcLog log)
    {
        return new ScreenSpaceConnectorDescription(this, map, log);
    }
}
