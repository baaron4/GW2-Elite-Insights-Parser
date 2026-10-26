namespace GW2EIEvtcParser.ParsedData;

public class TeamGUIDEvent : IDToGUIDEvent
{
    internal static TeamGUIDEvent DummyTeamGUID = new();
    public ulong TeamID => (ulong)ContentID;
    internal TeamGUIDEvent(CombatItem evtcItem) : base(evtcItem)
    {
    }

    private TeamGUIDEvent() : base()
    {
    }
}

