namespace GW2EIEvtcParser.ParsedData;

public class BreakbarDamageEvent : SkillEvent
{
    public double BreakbarDamage { get; protected set; }
    internal BreakbarDamageEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
    {
        BreakbarDamage = Math.Round(evtcItem.Value / 10.0, 1);

        // generic breakbar damage is changed amount rather than damage, need to flip sign
        if (SkillID == skillData.GenericBreakbarID)
        {
            BreakbarDamage *= -1.0;
        }
    }
}
