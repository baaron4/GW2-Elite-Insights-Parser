namespace GW2EIEvtcParser.ParsedData
{
    public class Last90BeforeDownEvent : AbstractStatusEvent
    {
        public long TimeSinceLast90 { get; }
        internal Last90BeforeDownEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
            TimeSinceLast90 = (long)evtcItem.DstAgent;
        }

    }
}
