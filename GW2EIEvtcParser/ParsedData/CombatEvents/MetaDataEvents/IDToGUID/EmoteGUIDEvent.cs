namespace GW2EIEvtcParser.ParsedData;

public class EmoteGUIDEvent : IDToGUIDEvent
{
    internal static EmoteGUIDEvent DummyEmoteGUID = new();
    public long EmoteID => ContentID;
    internal EmoteGUIDEvent(CombatItem evtcItem, EvtcVersionEvent evtcVersion) : base(evtcItem)
    {
    }

    internal EmoteGUIDEvent() : base()
    {
    }
}

