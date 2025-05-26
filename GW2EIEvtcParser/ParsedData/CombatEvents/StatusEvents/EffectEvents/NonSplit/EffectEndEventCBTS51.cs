namespace GW2EIEvtcParser.ParsedData;

internal class EffectEndEventCBTS51 : EffectEndEvent
{
    private void SetEndEventOnStartEvent(IReadOnlyDictionary<long, List<EffectEvent>> effectEventsByTrackingID)
    {
        if (effectEventsByTrackingID.TryGetValue(TrackingID, out var effectEvents))
        {
            EffectEvent? startEvent = effectEvents.LastOrDefault(x => x.Time <= Time);
            startEvent?.SetDynamicEndTime(this);
        }
    }
    internal EffectEndEventCBTS51(CombatItem evtcItem, AgentData agentData, IReadOnlyDictionary<long, List<EffectEvent>> effectEventsByTrackingID) : base(evtcItem, agentData)
    {
        TrackingID = EffectEventCBTS51.ReadTrackingID(evtcItem);
        if (TrackingID != 0)
        {
            SetEndEventOnStartEvent(effectEventsByTrackingID);
        }
    }
}
