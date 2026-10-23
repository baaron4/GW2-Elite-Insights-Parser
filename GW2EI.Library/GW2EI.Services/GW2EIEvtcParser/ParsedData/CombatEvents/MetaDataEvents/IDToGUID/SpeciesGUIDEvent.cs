namespace GW2EIEvtcParser.ParsedData;

public class SpeciesGUIDEvent : IDToGUIDEvent
{
    internal static SpeciesGUIDEvent DummySpeciesGUID = new();
    public long SpeciesID => ContentID;
    internal SpeciesGUIDEvent(CombatItem evtcItem) : base(evtcItem)
    {
    }

    private SpeciesGUIDEvent() : base()
    {
    }
}

