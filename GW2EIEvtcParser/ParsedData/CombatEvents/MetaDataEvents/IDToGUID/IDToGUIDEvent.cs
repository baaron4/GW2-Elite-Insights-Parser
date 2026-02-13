namespace GW2EIEvtcParser.ParsedData;

public abstract class IDToGUIDEvent : MetaDataEvent
{
    public readonly GUID GUID;
    protected readonly long ContentID;

    public bool IsValid => ContentID >= 0;

    internal IDToGUIDEvent(CombatItem evtcItem) : base(evtcItem)
    {
        GUID = new(evtcItem.SrcAgent, evtcItem.DstAgent);
        ContentID = evtcItem.SkillID;
    }

    internal IDToGUIDEvent() : base()
    {
        GUID = new();
        ContentID = long.MinValue;
    }

}
