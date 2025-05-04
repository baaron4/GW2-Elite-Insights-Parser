namespace GW2EIEvtcParser.ParsedData;

public abstract class IDToGUIDEvent : MetaDataEvent
{
    public readonly GUID ContentGUID;
    public readonly long ContentID;

    public bool IsValid => ContentID >= 0;

    internal IDToGUIDEvent(CombatItem evtcItem) : base(evtcItem)
    {
        //TODO(Rennorb) @explain: Why do the source and destination get packed here?
        ContentGUID = new(evtcItem.SrcAgent, evtcItem.DstAgent);
        ContentID = evtcItem.SkillID;
    }

    internal IDToGUIDEvent() : base()
    {
        ContentGUID = new();
        ContentID = long.MinValue;
    }

}
