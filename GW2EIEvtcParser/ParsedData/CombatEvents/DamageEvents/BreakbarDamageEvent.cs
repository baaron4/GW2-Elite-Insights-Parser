namespace GW2EIEvtcParser.ParsedData
{
    public class BreakbarDamageEvent : AbstractBaseDamageEvent
    {
        internal BreakbarDamageEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
            Damage = evtcItem.Value;
        }
    }
}
