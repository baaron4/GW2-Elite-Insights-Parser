namespace GW2EIEvtcParser.ParsedData
{
    public class EnterCombatEvent : AbstractStatusEvent
    {
        public int Subgroup { get; }
        internal EnterCombatEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
            Subgroup = (int)evtcItem.DstAgent;
        }

    }
}
