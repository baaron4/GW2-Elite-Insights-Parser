namespace GW2EIEvtcParser.ParsedData
{
    public class AttackTargetEvent : AbstractStatusEvent
    {
        public AgentItem AttackTarget { get; }

        public AttackTargetEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
            AttackTarget = Src;
            Src = agentData.GetAgent(evtcItem.DstAgent);
        }

    }
}
