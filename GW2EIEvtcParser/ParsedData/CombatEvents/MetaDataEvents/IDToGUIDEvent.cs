namespace GW2EIEvtcParser.ParsedData;

public abstract class IDToGUIDEvent : MetaDataEvent
{
    private readonly IDAndGUID Content;
    public GUID ContentGUID => Content.GUID!.Value; // GUID can't be null here, by construction
    public long ContentID => Content.ID;

    public bool IsValid => ContentID >= 0;

    internal IDToGUIDEvent(CombatItem evtcItem) : base(evtcItem)
    {
        //TODO(Rennorb) @explain: Why do the source and destination get packed here?
        Content = new(evtcItem.SkillID, new(evtcItem.SrcAgent, evtcItem.DstAgent));
    }

    internal IDToGUIDEvent() : base()
    {
        Content = new(long.MinValue, new());
    }

}
