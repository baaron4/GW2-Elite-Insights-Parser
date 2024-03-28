using System.Collections.Generic;
using System.Linq;

namespace GW2EIEvtcParser.ParsedData
{
    public class MarkerEndEvent : AbstractMarkerEvent
    {

        internal MarkerEndEvent(CombatItem evtcItem, AgentData agentData, Dictionary<AgentItem, List<MarkerEvent>> markerEventsByAgents) : base(evtcItem, agentData)
        {
            SetEndEventOnStartEvent(markerEventsByAgents);
        }
        private void SetEndEventOnStartEvent(Dictionary<AgentItem, List<MarkerEvent>> markerEventsByAgents)
        {
            if (markerEventsByAgents.TryGetValue(Src, out List<MarkerEvent> markerEvents))
            {
                MarkerEvent lastMarker = markerEvents.LastOrDefault();
                lastMarker?.SetEndTime(Time);
            }
        }
    }
}
