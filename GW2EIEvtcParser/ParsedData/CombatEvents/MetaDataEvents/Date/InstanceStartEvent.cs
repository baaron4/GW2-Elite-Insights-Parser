namespace GW2EIEvtcParser.ParsedData;

public class InstanceStartEvent : MetaDataEvent
{
    public readonly long TimeOffsetFromInstanceCreation;

    public readonly string? InstanceIP = null;

    internal InstanceStartEvent(CombatItem evtcItem, long logStart) : base(evtcItem)
    {
        TimeOffsetFromInstanceCreation = logStart - (long)evtcItem.SrcAgent;
        if (evtcItem.Value != 0)
        {
            long item1 = (evtcItem.Value & 0xFF000000) >> 24;
            long item2 = (evtcItem.Value & 0x00FF0000) >> 16;
            long item3 = (evtcItem.Value & 0x0000FF00) >> 8;
            long item4 = (evtcItem.Value & 0x000000FF);
            InstanceIP = $"{item1}.{item2}.{item3}.{item4}";
        }
    }

}
