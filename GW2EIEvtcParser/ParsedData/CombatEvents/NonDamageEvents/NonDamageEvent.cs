namespace GW2EIEvtcParser.ParsedData;

public abstract class NonDamageEvent : SkillEvent
{

    internal NonDamageEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
    {
        AgainstDowned = evtcItem.IsOffcycle == 1;
    }
}
