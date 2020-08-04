namespace GW2EIEvtcParser.ParsedData
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
