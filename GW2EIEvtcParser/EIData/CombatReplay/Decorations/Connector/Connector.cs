using System.Text.Json.Serialization;

namespace GW2EIEvtcParser.EIData;

public abstract class Connector
{

    public enum InterpolationMethod
    {
        Linear = 0
    }
    public abstract object GetConnectedTo(CombatReplayMap map, ParsedEvtcLog log);
}
