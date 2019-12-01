namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public abstract class AbstractStatusEvent : AbstractTimeCombatEvent
    {
        public AgentItem Src { get; protected set; }

        public AbstractStatusEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem.Time)
        {
            Src = agentData.GetAgent(evtcItem.SrcAgent);
        }

    }
}
