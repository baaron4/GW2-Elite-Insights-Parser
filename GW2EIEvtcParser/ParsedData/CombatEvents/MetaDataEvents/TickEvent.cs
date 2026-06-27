namespace GW2EIEvtcParser.ParsedData;

public class TickEvent : MetaDataEvent
{
    public readonly long Time;
    public readonly ulong TickInterpolated;
    public readonly ulong TickSinceLastUpdate;
    internal TickEvent(CombatItem evtcItem) : base(evtcItem)
    {
        Time = evtcItem.Time;
        TickInterpolated = evtcItem.SrcAgent;
        TickSinceLastUpdate = evtcItem.DstAgent;
    }

}
