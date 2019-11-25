using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.EIData
{
    public abstract class BackgroundDecoration : GenericDecoration
    {
        public BackgroundDecoration((int start, int end) lifespan) : base(lifespan, null)
        {
        }
   
        public abstract override GenericDecorationSerializable GetCombatReplayJSON(CombatReplayMap map, ParsedLog log);
    }
}
