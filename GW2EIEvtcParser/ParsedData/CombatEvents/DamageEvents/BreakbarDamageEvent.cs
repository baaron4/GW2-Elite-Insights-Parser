namespace GW2EIEvtcParser.ParsedData
{
    public class BreakbarDamageEvent : AbstractBaseDamageEvent
    {
        public int BreakbarDamage { get; protected set; }
        internal BreakbarDamageEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
            BreakbarDamage = evtcItem.Value;
        }
    }
}
