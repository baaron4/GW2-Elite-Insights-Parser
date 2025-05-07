namespace GW2EIEvtcParser.ParsedData;

public class ProjectileGUIDEvent : IDToGUIDEvent
{
    internal static ProjectileGUIDEvent DummyProjectileGUID = new();
    internal ProjectileGUIDEvent(CombatItem evtcItem, EvtcVersionEvent evtcVersion) : base(evtcItem)
    {
    }

    internal ProjectileGUIDEvent() : base()
    {
    }
}

