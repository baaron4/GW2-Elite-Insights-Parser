namespace GW2EIEvtcParser.ParsedData
{
    public abstract class AbstractBreakbarDamageEvent : AbstractDamageEvent
    {
        public double BreakbarDamage { get; protected set; }
        internal AbstractBreakbarDamageEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
        }
    }
}
