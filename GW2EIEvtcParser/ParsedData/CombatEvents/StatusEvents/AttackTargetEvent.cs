namespace GW2EIEvtcParser.ParsedData
{
    public class AttackTargetEvent : AbstractStatusEvent
    {
        public AgentItem AttackTarget { get; }

        public bool Targetable { get; }

        internal AttackTargetEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
            AttackTarget = Src;
            Src = agentData.GetAgent(evtcItem.DstAgent, evtcItem.Time);
            Targetable = evtcItem.Value == 1;
        }

    }
}
