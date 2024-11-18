namespace GW2EIEvtcParser.ParsedData;

public class RewardEvent : TimeCombatEvent
{
    public readonly ulong RewardID;
    public readonly int RewardType;

    internal RewardEvent(CombatItem evtcItem) : base(evtcItem.Time)
    {
        RewardID = evtcItem.DstAgent;
        RewardType = evtcItem.Value;
    }

}
