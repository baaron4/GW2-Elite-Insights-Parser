namespace LuckParser.Parser.ParsedData.CombatEvents
{
    public class RewardEvent : AbstractMetaDataEvent
    {
        public ulong RewardID { get; }
        public int RewardType { get; }

        public RewardEvent(CombatItem evtcItem, long offset) : base(evtcItem, offset)
        {
            RewardID = evtcItem.DstAgent;
            RewardType = evtcItem.Value;
        }

    }
}
