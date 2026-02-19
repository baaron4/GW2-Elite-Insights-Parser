namespace GW2EIEvtcParser.ParsedData;

public class BreakbarRecoveryEvent : SkillEvent
{
    /// <summary>
    /// Will be negative when soft CC overwhelms natural breakbar regeneration
    /// </summary>
    public double BreakbarRecovered { get; protected set; }
    internal BreakbarRecoveryEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
    {
        BreakbarRecovered = Math.Round(evtcItem.Value / 10.0, 1);
    }
    internal override double GetValue()
    {
        return BreakbarRecovered;
    }
}
