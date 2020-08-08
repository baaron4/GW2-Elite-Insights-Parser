namespace GW2EIEvtcParser.ParsedData
{
    public class AttackTargetEvent : AbstractStatusEvent
    {
        public AgentItem AttackTarget { get; }

        internal AttackTargetEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
            AttackTarget = Src;
            Src = agentData.GetAgent(evtcItem.DstAgent);
        }

    }
}
