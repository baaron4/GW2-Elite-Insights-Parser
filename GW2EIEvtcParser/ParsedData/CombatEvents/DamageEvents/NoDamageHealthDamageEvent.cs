using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.ParsedData;

public class NoDamageHealthDamageEvent : HealthDamageEvent
{
    internal NoDamageHealthDamageEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData, DamageResult result) : base(evtcItem, agentData, skillData)
    {
        HasDowned = result == DamageResult.Downed;
        HasKilled = result == DamageResult.KillingBlow;
        HasInterrupted = result == DamageResult.Interrupt;
        IsNotADamageEvent = true;
    }

    internal override void MakeIntoAbsorbed()
    {
        // Nothing to do
    }
}
