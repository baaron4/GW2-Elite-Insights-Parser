namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public class RewardEvent : AbstractTimeCombatEvent
    {
        public ulong RewardID { get; }
        public int RewardType { get; }

        public RewardEvent(CombatItem evtcItem) : base(evtcItem.Time)
        {
            RewardID = evtcItem.DstAgent;
            RewardType = evtcItem.Value;
        }

    }
}
