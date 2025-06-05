using System.Numerics;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.ParsedData;

internal class EffectEventGroundRemove : EffectEndEvent
{
    private void SetEndEventOnStartEvent(IReadOnlyDictionary<long, List<EffectEventGroundCreate>> effectEventsByTrackingID)
    {
        if (effectEventsByTrackingID.TryGetValue(TrackingID, out var effectEvents))
        {
            EffectEvent? startEvent = effectEvents.LastOrDefault(x => x.Time <= Time);
            startEvent?.SetDynamicEndTime(this);
        }
    }
    internal EffectEventGroundRemove(CombatItem evtcItem, AgentData agentData, IReadOnlyDictionary<long, List<EffectEventGroundCreate>> effectEventsByTrackingID) : base(evtcItem, agentData)
    {
        TrackingID = evtcItem.Pad;
        if (TrackingID != 0)
        {
            SetEndEventOnStartEvent(effectEventsByTrackingID);
        }
    }

}
