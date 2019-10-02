namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public class ShardEvent : AbstractMetaDataEvent
    {
        public int ShardID { get; }

        public ShardEvent(CombatItem evtcItem, long offset) : base(evtcItem, offset)
        {
            ShardID = (int)evtcItem.SrcAgent;
        }

    }
}
