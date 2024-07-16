namespace GW2EIEvtcParser.ParsedData
{
    public class AttackTargetEvent : AbstractStatusEvent
    {
        public AgentItem AttackTarget { get; }

        public bool Targetable { get; }

        internal AttackTargetEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
            AttackTarget = Src;
            Src = GetTargeted(evtcItem, agentData);
            Targetable = IsTargetable(evtcItem);
        }

        internal static AgentItem GetAttackTarget(CombatItem evtcItem, AgentData agentData)
        {
            return agentData.GetAgent(evtcItem.SrcAgent, evtcItem.Time);
        }

        internal static AgentItem GetTargeted(CombatItem evtcItem, AgentData agentData)
        {
            return agentData.GetAgent(evtcItem.DstAgent, evtcItem.Time);
        }

        internal static bool IsTargetable(CombatItem evtcItem)
        {
            return evtcItem.Value == 1;
        }

    }
}
