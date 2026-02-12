namespace GW2EIEvtcParser.ParsedData;

public class TeamGUIDEvent : IDToGUIDEvent
{
    internal static TeamGUIDEvent DummyTeamGUID = new();
    public new ulong ContentID => (ulong)base.ContentID;
    internal TeamGUIDEvent(CombatItem evtcItem, EvtcVersionEvent evtcVersion) : base(evtcItem)
    {
    }

    internal TeamGUIDEvent() : base()
    {
    }
}

