namespace GW2EIEvtcParser.ParsedData
{
    public class EffectGUIDEvent : IDToGUIDEvent
    {
        internal static EffectGUIDEvent DummyEffectGUID = new EffectGUIDEvent();
        internal EffectGUIDEvent(CombatItem evtcItem) : base(evtcItem)
        {
        }

        internal EffectGUIDEvent() : base()
        {
        }

    }
}
