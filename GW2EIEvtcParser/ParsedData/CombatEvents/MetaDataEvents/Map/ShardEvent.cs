namespace GW2EIEvtcParser.ParsedData;

public class ShardEvent : MetaDataEvent
{
    public readonly int ShardID;

    internal ShardEvent(CombatItem evtcItem) : base(evtcItem)
    {
        ShardID = (int)evtcItem.SrcAgent;
    }

}
