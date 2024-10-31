namespace GW2EIEvtcParser.ParsedData;

public abstract class IDToGUIDEvent : AbstractMetaDataEvent
{
    public readonly GUID ContentGUID;
    public readonly long ContentID;

    internal IDToGUIDEvent(CombatItem evtcItem) : base(evtcItem)
    {
        //TODO(Rennorb) @explain: Why do the source and destination get packed here?
        this.ContentGUID = new(evtcItem.SrcAgent, evtcItem.DstAgent);
        this.ContentID = evtcItem.SkillID;
    }

    internal IDToGUIDEvent() : base()
    {
        ContentGUID = new();
        ContentID = -1;
    }

}
