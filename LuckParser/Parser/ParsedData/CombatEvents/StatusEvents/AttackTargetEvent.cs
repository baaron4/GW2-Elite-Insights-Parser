namespace LuckParser.Parser.ParsedData.CombatEvents
{
    public class AttackTargetEvent : AbstractStatusEvent
    {
        public AgentItem AttackTarget { get; }

        public AttackTargetEvent(CombatItem evtcItem, AgentData agentData, long offset) : base(evtcItem, agentData, offset)
        {
            AttackTarget = agentData.GetAgent(evtcItem.SrcAgent, evtcItem.LogTime);
            Src = agentData.GetAgent(evtcItem.DstAgent, evtcItem.LogTime);
        }

    }
}
