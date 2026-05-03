namespace GW2EIEvtcParser.ParsedData;

public class StunBreakEvent : NonDamageEvent
{
    public int RemainingDuration { get; private set; }
    internal StunBreakEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
    {
        To = From;
        From = ParserHelper._unknownAgent;
        RemainingDuration = evtcItem.Value;
    }
    internal override double GetValue()
    {
        return RemainingDuration;
    }

}
