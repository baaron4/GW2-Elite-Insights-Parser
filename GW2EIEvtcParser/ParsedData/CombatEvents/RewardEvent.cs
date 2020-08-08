namespace GW2EIEvtcParser.ParsedData
{
    public class RewardEvent : AbstractTimeCombatEvent
    {
        public ulong RewardID { get; }
        public int RewardType { get; }

        internal RewardEvent(CombatItem evtcItem) : base(evtcItem.Time)
        {
            RewardID = evtcItem.DstAgent;
            RewardType = evtcItem.Value;
        }

    }
}
