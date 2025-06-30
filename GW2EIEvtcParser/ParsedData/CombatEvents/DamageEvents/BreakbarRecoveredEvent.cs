namespace GW2EIEvtcParser.ParsedData;

public class BreakbarRecoveredEvent : SkillEvent
{
    public double BreakbarRecovered { get; protected set; }
    internal BreakbarRecoveredEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
    {
        BreakbarRecovered = Math.Round(evtcItem.Value / 10.0, 1);
    }
}
