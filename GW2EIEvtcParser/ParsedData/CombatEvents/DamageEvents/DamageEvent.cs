namespace GW2EIEvtcParser.ParsedData;

public abstract class DamageEvent : SkillEvent
{
    //private int _damage;
    public readonly bool IsOverNinety;
    public readonly bool AgainstUnderFifty;
    public readonly bool IsMoving;
    public readonly bool AgainstMoving;
    public readonly bool IsFlanking;
    public bool AgainstDowned { get; protected set; }

    internal DamageEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
    {
        IsOverNinety = evtcItem.IsNinety > 0;
        AgainstUnderFifty = evtcItem.IsFifty > 0;
        IsMoving = (evtcItem.IsMoving & 1) > 0;
        AgainstMoving = (evtcItem.IsMoving & 2) > 0;
        IsFlanking = evtcItem.IsFlanking > 0;
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
