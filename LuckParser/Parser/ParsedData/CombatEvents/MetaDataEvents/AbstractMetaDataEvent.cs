namespace LuckParser.Parser.ParsedData.CombatEvents
{
    public abstract class AbstractMetaDataEvent : AbstractCombatEvent
    {
        public ulong Data { get; }

        public AbstractMetaDataEvent(CombatItem evtcItem, long offset) : base(evtcItem.LogTime, offset)
        {
#if DEBUG
            OriginalCombatEvent = evtcItem;
#endif
            Data = evtcItem.SrcAgent;
        }

    }
}
