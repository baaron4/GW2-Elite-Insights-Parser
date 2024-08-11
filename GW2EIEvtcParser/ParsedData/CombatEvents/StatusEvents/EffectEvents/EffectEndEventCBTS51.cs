using System.Collections.Generic;

namespace GW2EIEvtcParser.ParsedData
{
    internal class EffectEndEventCBTS51 : EffectEndEvent
    {
        internal EffectEndEventCBTS51(CombatItem evtcItem, AgentData agentData, IReadOnlyDictionary<long, List<EffectEvent>> effectEventsByTrackingID) : base(evtcItem, agentData)
        {
            Orientation = EffectEventCBTS51.ReadOrientation(evtcItem);
            TrackingID = EffectEventCBTS51.ReadTrackingID(evtcItem);
            if (TrackingID != 0)
            {
                SetEndEventOnStartEvent(effectEventsByTrackingID);
            }
        }
    }
}
