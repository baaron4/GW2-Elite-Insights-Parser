namespace GW2EIEvtcParser.ParsedData;

internal abstract class EffectEndEvent : AbstractEffectEvent
{

    internal EffectEndEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
    {
    }

    protected void SetEndEventOnStartEvent(IReadOnlyDictionary<long, List<EffectEvent>> effectEventsByTrackingID)
    {
        if (effectEventsByTrackingID.TryGetValue(TrackingID, out var effectEvents))
        {
            EffectEvent? startEvent = effectEvents.LastOrDefault(x => x.Time <= Time);
            startEvent?.SetDynamicEndTime(this);
        }
    }
}
