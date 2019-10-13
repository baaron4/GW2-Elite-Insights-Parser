namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public abstract class AbstractMetaDataEvent : AbstractCombatEvent
    {
        public AbstractMetaDataEvent(CombatItem evtcItem, long offset) : base(evtcItem.LogTime, offset)
        {
        }

    }
}
