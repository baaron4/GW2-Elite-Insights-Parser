namespace GW2EIEvtcParser.ParsedData;

public class ShardEvent : AbstractMetaDataEvent
{
    public readonly int ShardID;

    internal ShardEvent(CombatItem evtcItem) : base(evtcItem)
    {
        ShardID = (int)evtcItem.SrcAgent;
    }

}
