namespace GW2EIEvtcParser.ParsedData;

public class SpeciesGUIDEvent : IDToGUIDEvent
{
    internal SpeciesGUIDEvent(CombatItem evtcItem, EvtcVersionEvent evtcVersion) : base(evtcItem)
    {
    }
}

