namespace GW2EIEvtcParser.EIData
{
    internal abstract class BackgroundDecoration : GenericDecoration
    {
        public BackgroundDecoration((int start, int end) lifespan) : base(lifespan)
        {
        }

        public abstract override GenericDecorationSerializable GetCombatReplayJSON(CombatReplayMap map, ParsedEvtcLog log);
    }
}
