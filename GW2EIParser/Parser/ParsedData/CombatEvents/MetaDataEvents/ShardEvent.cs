namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public class ShardEvent : AbstractMetaDataEvent
    {
        public int ShardID { get; }

        public ShardEvent(CombatItem evtcItem) : base(evtcItem)
        {
            ShardID = (int)evtcItem.SrcAgent;
        }

    }
}
