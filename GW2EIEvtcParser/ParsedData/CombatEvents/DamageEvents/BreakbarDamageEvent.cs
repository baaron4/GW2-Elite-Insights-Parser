namespace GW2EIEvtcParser.ParsedData;

public class BreakbarDamageEvent : SkillEvent
{
    public double BreakbarDamage { get; protected set; }
    internal BreakbarDamageEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
    {
        BreakbarDamage = Math.Round(evtcItem.Value / 10.0, 1);
        if (BreakbarDamage < 0)
        {
            BreakbarDamage *= -1;
        }
    }
}
