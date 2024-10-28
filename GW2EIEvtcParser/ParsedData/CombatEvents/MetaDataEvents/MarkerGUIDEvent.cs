namespace GW2EIEvtcParser.ParsedData
{
    public class MarkerGUIDEvent : IDToGUIDEvent
    {
        internal static MarkerGUIDEvent DummyMarkerGUID = new MarkerGUIDEvent();
        internal MarkerGUIDEvent(CombatItem evtcItem) : base(evtcItem)
        {
        }
        internal MarkerGUIDEvent() : base()
        {
        }

    }
}
