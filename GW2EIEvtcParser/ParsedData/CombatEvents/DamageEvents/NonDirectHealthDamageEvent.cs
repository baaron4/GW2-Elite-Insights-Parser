using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.ParsedData;

public class NonDirectHealthDamageEvent : HealthDamageEvent
{
    internal NonDirectHealthDamageEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData, ConditionResult result) : base(evtcItem, agentData, skillData)
    {
        var cycle = GetBuffCycle(evtcItem.IsOffcycle);
        IsLifeLeech = cycle == BuffCycle.NotCycle_DamageToTargetOnHit || cycle == BuffCycle.NotCycle_DamageToTargetOnStackRemove;
        HealthDamage = evtcItem.BuffDmg;
        IsAbsorbed = result == ConditionResult.InvulByBuff ||
            result == ConditionResult.InvulByPlayerSkill1 ||
            result == ConditionResult.InvulByPlayerSkill2 ||
            result == ConditionResult.InvulByPlayerSkill3;
        HasHit = result == ConditionResult.ExpectedToHit;
        ShieldDamage = evtcItem.IsShields > 0 ? HealthDamage : 0;
        AgainstDowned = evtcItem.Pad1 == 1;
    }

    internal override void MakeIntoAbsorbed()
    {
        HasHit = false;
        IsAbsorbed = true;
        HealthDamage = 0;
        ShieldDamage = 0;
    }
}
