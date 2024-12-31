namespace GW2EIEvtcParser.EIData;

public abstract class Connector
{

    public enum InterpolationMethod
    {
        Linear = 0,
        Step = 1,
    }
    public abstract ConnectorDescription GetConnectedTo(CombatReplayMap map, ParsedEvtcLog log);
}
