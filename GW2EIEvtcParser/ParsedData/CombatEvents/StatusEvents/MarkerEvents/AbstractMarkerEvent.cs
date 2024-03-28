namespace GW2EIEvtcParser.ParsedData
{
    public abstract class AbstractMarkerEvent : AbstractStatusEvent
    {
        public int MarkerID { get; }

        internal AbstractMarkerEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
            MarkerID = evtcItem.Value;
        }

    }
}
