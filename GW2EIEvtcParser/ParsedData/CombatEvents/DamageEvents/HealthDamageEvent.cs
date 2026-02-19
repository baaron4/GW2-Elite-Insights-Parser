namespace GW2EIEvtcParser.ParsedData;

public abstract class HealthDamageEvent : DamageEvent
{
    //
    public int HealthDamage { get; protected set; }
    public int ShieldDamage { get; protected set; }
    public bool HasHit { get; protected set; }
    public bool HasCrit { get; protected set; }
    public bool HasGlanced { get; protected set; }
    public bool IsLifeLeech { get; protected set; } = false;
    public bool IsBlind { get; protected set; }
    public bool IsAbsorbed { get; protected set; }
    public bool IsBlocked { get; protected set; }
    public bool IsEvaded { get; protected set; }
    // Not a damage event
    public bool IsNotADamageEvent { get; protected set; }
    public bool HasInterrupted { get; protected set; }
    public bool HasDowned { get; protected set; }
    public bool HasKilled { get; protected set; }


    internal HealthDamageEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
    {
    }

    internal void NegateShieldDamage()
    {
        //_damage = Damage;
        HealthDamage = Math.Max(HealthDamage - ShieldDamage, 0);
    }
    internal override double GetValue()
    {
        return HealthDamage;
    }

    internal abstract void MakeIntoAbsorbed();

}
