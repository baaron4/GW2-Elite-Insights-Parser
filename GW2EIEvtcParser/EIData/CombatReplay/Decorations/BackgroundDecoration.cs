namespace GW2EIEvtcParser.EIData
{
    internal abstract class BackgroundDecoration : GenericDecoration
    {
        public BackgroundDecoration((long start, long end) lifespan) : base(lifespan)
        {
        }

        public abstract override GenericDecorationCombatReplayDescription GetCombatReplayDescription(CombatReplayMap map, ParsedEvtcLog log);
    }
}
