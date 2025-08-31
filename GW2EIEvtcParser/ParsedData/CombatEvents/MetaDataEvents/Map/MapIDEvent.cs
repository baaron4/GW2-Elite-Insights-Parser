namespace GW2EIEvtcParser.ParsedData;

public class MapIDEvent : MetaDataEvent
{
    public readonly long Time;
    public readonly int MapID;

    internal MapIDEvent(CombatItem evtcItem) : base(evtcItem)
    {
        MapID = GetMapID(evtcItem);
        Time = evtcItem.Time;
    }

    internal static int GetMapID(CombatItem evtcItem)
    {
        return (int)evtcItem.SrcAgent;
    }

}
