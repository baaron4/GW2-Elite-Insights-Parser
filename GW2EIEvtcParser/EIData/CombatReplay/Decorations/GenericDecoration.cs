namespace GW2EIEvtcParser.EIData
{
    public abstract class GenericDecoration
    {
        public (int start, int end) Lifespan { get; }

        protected GenericDecoration((int start, int end) lifespan)
        {
            Lifespan = lifespan;
        }
        //

        public abstract GenericDecorationSerializable GetCombatReplayJSON(CombatReplayMap map, ParsedEvtcLog log);

    }
}
