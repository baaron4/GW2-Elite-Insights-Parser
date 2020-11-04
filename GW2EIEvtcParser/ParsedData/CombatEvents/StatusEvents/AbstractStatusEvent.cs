namespace GW2EIEvtcParser.ParsedData
{
    public abstract class AbstractStatusEvent : AbstractTimeCombatEvent
    {
        public AgentItem Src { get; protected set; }

        internal AbstractStatusEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem.Time)
        {
            Src = agentData.GetAgent(evtcItem.SrcAgent);
        }

        internal AbstractStatusEvent(AgentItem src, long time) : base(time)
        {
            Src = src;
        }

    }
}
