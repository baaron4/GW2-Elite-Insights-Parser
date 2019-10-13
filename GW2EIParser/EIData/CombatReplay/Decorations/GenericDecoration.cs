using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.EIData
{
    public abstract class GenericDecoration
    {
        public (int start, int end) Lifespan { get; }
        protected Connector ConnectedTo { get; set; }

        protected GenericDecoration((int start, int end) lifespan, Connector connector)
        {
            Lifespan = lifespan;
            ConnectedTo = connector;
        }
        //
        public class GenericDecorationSerializable
        {
            public string Type { get; set; }
            public long Start { get; set; }
            public long End { get; set; }
            public object ConnectedTo { get; set; }
        }

        public abstract GenericDecorationSerializable GetCombatReplayJSON(CombatReplayMap map, ParsedLog log);

    }
}
