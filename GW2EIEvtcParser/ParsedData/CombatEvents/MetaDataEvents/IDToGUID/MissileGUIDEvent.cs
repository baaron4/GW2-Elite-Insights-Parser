namespace GW2EIEvtcParser.ParsedData;

public class MissileGUIDEvent : IDToGUIDEvent
{
    internal static MissileGUIDEvent DummyMissileGUID = new();
    internal MissileGUIDEvent(CombatItem evtcItem, EvtcVersionEvent evtcVersion) : base(evtcItem)
    {
    }

    internal MissileGUIDEvent() : base()
    {
    }
}

