using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.EIData
{
    public abstract class GenericDecoration
    {
        public (int start, int end) Lifespan { get; }
        public Connector ConnectedTo { get; }

        protected GenericDecoration((int start, int end) lifespan, Connector connector)
        {
            Lifespan = lifespan;
            ConnectedTo = connector;
        }
        //

        public abstract GenericDecorationSerializable GetCombatReplayJSON(CombatReplayMap map, ParsedLog log);

    }
}
