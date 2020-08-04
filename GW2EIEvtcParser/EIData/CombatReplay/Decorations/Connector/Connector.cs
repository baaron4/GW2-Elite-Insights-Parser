namespace GW2EIEvtcParser.EIData
{
    public abstract class Connector
    {
        public abstract object GetConnectedTo(CombatReplayMap map, ParsedEvtcLog log);
    }
}
