namespace GW2EIEvtcParser.ParsedData;

public class TickRateEvent : MetaDataEvent
{
    public readonly long Time;
    public readonly ulong TickRate;
    internal TickRateEvent(CombatItem evtcItem) : base(evtcItem)
    {
        Time = evtcItem.Time;
        TickRate = evtcItem.SrcAgent;
    }

}
