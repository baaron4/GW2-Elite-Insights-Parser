namespace GW2EIEvtcParser.ParsedData;

public class MapChangeEvent : MapIDEvent
{
    public readonly int OldMapID;

    internal MapChangeEvent(CombatItem evtcItem) : base(evtcItem)
    {
        OldMapID = (int)evtcItem.DstAgent;
    }

}
