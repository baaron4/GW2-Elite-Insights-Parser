namespace GW2EIEvtcParser.ParsedData;

public class SkillGUIDEvent : IDToGUIDEvent
{
    internal static SkillGUIDEvent DummySkillGUID = new();
    public long SkillID => ContentID;
    internal SkillGUIDEvent(CombatItem evtcItem, EvtcVersionEvent evtcVersion) : base(evtcItem)
    {
    }

    internal SkillGUIDEvent() : base()
    {
    }
}

