namespace GW2EIEvtcParser.ParsedData;

public class BreakbarRecoveryEvent : SkillEvent
{
    public double BreakbarRecovered { get; protected set; }
    internal BreakbarRecoveryEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
    {
        BreakbarRecovered = Math.Round(evtcItem.Value / 10.0, 1);
        if (BreakbarRecovered < 0)
        {
            BreakbarRecovered *= -1;
        }
    }
}
