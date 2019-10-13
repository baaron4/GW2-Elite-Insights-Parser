using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.EIData
{
    public abstract class Connector
    {
        public abstract object GetConnectedTo(CombatReplayMap map, ParsedLog log);
    }
}
