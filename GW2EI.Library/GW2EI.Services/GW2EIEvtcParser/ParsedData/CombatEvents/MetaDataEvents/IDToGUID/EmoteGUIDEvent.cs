namespace GW2EIEvtcParser.ParsedData;

public class EmoteGUIDEvent : IDToGUIDEvent
{
    internal static EmoteGUIDEvent DummyEmoteGUID = new();
    public long EmoteID => ContentID;
    internal EmoteGUIDEvent(CombatItem evtcItem) : base(evtcItem)
    {
    }

    private EmoteGUIDEvent() : base()
    {
    }
}

