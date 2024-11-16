namespace GW2EIEvtcParser.ParsedData;

public class MapIDEvent : AbstractMetaDataEvent
{
    public readonly int MapID;

    internal MapIDEvent(CombatItem evtcItem) : base(evtcItem)
    {
        MapID = (int)evtcItem.SrcAgent;
    }

}
