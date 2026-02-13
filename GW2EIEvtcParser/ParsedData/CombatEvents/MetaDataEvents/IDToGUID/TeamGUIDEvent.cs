namespace GW2EIEvtcParser.ParsedData;

public class TeamGUIDEvent : IDToGUIDEvent
{
    internal static TeamGUIDEvent DummyTeamGUID = new();
    public ulong TeamID => (ulong)ContentID;
    internal TeamGUIDEvent(CombatItem evtcItem, EvtcVersionEvent evtcVersion) : base(evtcItem)
    {
    }

    internal TeamGUIDEvent() : base()
    {
    }
}

