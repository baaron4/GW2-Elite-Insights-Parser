namespace GW2EIEvtcParser.ParsedData
{
    public class MarkerEvent : AbstractStatusEvent
    {
        public int MarkerID { get; }

        internal MarkerEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
            MarkerID = evtcItem.Value;
        }

    }
}
