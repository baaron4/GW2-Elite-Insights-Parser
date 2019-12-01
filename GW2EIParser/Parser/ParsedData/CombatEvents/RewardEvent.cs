namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public class RewardEvent : AbstractTimeCombatEvent
    {
        public ulong RewardID { get; }
        public int RewardType { get; }

        public RewardEvent(CombatItem evtcItem, long offset) : base(evtcItem.LogTime, offset)
        {
            RewardID = evtcItem.DstAgent;
            RewardType = evtcItem.Value;
        }

    }
}
