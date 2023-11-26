using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.ParsedData
{
    public abstract class EffectEndEvent : AbstractEffectEvent
    {

        internal EffectEndEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
        }

        protected void SetEndEventOnStartEvent(IReadOnlyDictionary<long, List<EffectEvent>> effectEventsByTrackingID)
        {
            if (effectEventsByTrackingID.TryGetValue(TrackingID, out List<EffectEvent> effectEvents))
            {
                EffectEvent startEvent = effectEvents.LastOrDefault(x => x.Time <= Time);
                if (startEvent != null)
                {
                    startEvent.SetEndEvent(this);
                }
            }
        }
    }
}
