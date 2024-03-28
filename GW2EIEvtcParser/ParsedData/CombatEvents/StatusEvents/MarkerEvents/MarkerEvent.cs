namespace GW2EIEvtcParser.ParsedData
{
    public class MarkerEvent : AbstractMarkerEvent
    {
        public int MarkerID { get; }

        internal MarkerEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
            MarkerID = evtcItem.Value;
        }

    }
}
