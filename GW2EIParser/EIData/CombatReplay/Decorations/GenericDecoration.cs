using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.EIData
{
    public abstract class GenericDecoration
    {
        public (int start, int end) Lifespan { get; }

        protected GenericDecoration((int start, int end) lifespan)
        {
            Lifespan = lifespan;
        }
        //

        public abstract GenericDecorationSerializable GetCombatReplayJSON(CombatReplayMap map, ParsedLog log);

    }
}
