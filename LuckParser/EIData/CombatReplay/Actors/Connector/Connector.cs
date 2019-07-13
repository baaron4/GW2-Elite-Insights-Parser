using LuckParser.Parser;

namespace LuckParser.EIData
{
    public abstract class Connector
    {
        public abstract object GetConnectedTo(CombatReplayMap map, ParsedLog log);
    }
}
