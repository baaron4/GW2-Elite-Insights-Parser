namespace GW2EIEvtcParser.ParsedData;

public abstract class DamageEvent : SkillEvent
{

    internal DamageEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
    {
    }

    /*public bool AgainstDowned(ParsedEvtcLog log)
    {
        if (AgainstDownedInternal == -1)
        {
            AgainstDownedInternal = To.IsDowned(log, Time) ? 1 : 0;
        }        
        return AgainstDownedInternal == 1;
    }*/
}
