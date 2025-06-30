namespace GW2EIEvtcParser.ParsedData;

internal class BreakbarChangeEvent : SkillEvent
{
    public double BreakbarChanged { get; protected set; }
    internal BreakbarChangeEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
    {
        BreakbarChanged = Math.Round(evtcItem.Value / 10.0, 1);
    }
}
