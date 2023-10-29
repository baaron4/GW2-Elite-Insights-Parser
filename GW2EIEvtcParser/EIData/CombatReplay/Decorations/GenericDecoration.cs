namespace GW2EIEvtcParser.EIData
{
    public abstract class GenericDecoration
    {
        public (int start, int end) Lifespan { get; }

        protected GenericDecoration((long start, long end) lifespan)
        {
            Lifespan = ((int)lifespan.start, (int)lifespan.end);
        }
        //

        public abstract GenericDecorationCombatReplayDescription GetCombatReplayDescription(CombatReplayMap map, ParsedEvtcLog log);

    }
}
