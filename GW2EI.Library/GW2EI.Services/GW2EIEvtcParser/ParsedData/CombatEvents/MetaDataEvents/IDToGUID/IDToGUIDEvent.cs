namespace GW2EIEvtcParser.ParsedData;

public abstract class IDToGUIDEvent : MetaDataEvent
{
    internal readonly Guid GUID;
    public string GUIDString => GUID.ToString("N").ToUpperInvariant();
    public readonly long ContentID;

    public bool IsValid => ContentID >= 0;

    internal IDToGUIDEvent(CombatItem evtcItem) : base(evtcItem)
    {
        GUID = new GUIDWrapper(evtcItem.SrcAgent, evtcItem.DstAgent, true).GUID;
        ContentID = evtcItem.SkillID;
    }

    public Guid GetGUIDStruct()
    {
        return GUID;
    }

    protected IDToGUIDEvent() : base()
    {
        GUID = new();
        ContentID = long.MinValue;
    }

}
