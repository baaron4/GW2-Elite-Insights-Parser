namespace GW2EIEvtcParser.ParsedData;

public abstract class IDToGUIDEvent : MetaDataEvent
{
    public readonly Guid GUID;
    public readonly long ContentID;

    public bool IsValid => ContentID >= 0;

    internal IDToGUIDEvent(CombatItem evtcItem) : base(evtcItem)
    {
        GUID = new GUIDWrapper(evtcItem.SrcAgent, evtcItem.DstAgent, true).GUID;
        ContentID = evtcItem.SkillID;
    }

    protected IDToGUIDEvent() : base()
    {
        GUID = new();
        ContentID = long.MinValue;
    }

}
