using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.EIData
{
    public abstract class BackgroundActor : GenericActor
    {
        public BackgroundActor((int start, int end) lifespan) : base(lifespan, null)
        {
        }

        protected class BackgroundSerializable : GenericActorSerializable
        {
        }

        public abstract override GenericActorSerializable GetCombatReplayJSON(CombatReplayMap map, ParsedLog log);
    }
}
