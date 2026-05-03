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

    internal NonDirectHealthDamageEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData, DamageResult result) : base(evtcItem, agentData, skillData)
    {
        HealthDamage = evtcItem.BuffDmg;

        IsLifeLeech = result == DamageResult.BuffNotCycle_DamageToTargetOnHit || result == DamageResult.BuffNotCycle_DamageToTargetOnStackRemove;
        AgainstDowned = evtcItem.IsOffcycle == 1;
        IsAbsorbed = result == DamageResult.DirectOrBuffAbsorb || result == DamageResult.DirectOrBuffInvert;
        ShieldDamage = evtcItem.IsShields > 0 ?
            evtcItem.OverstackValue > 0 ? (int)evtcItem.OverstackValue : HealthDamage
            : 
            0;
        HasHit = result == DamageResult.BuffCycle || result == DamageResult.BuffNotCycle || 
            result == DamageResult.BuffNotCycle_DamageToSourceOnHit || IsLifeLeech;
    }

    internal override void MakeIntoAbsorbed()
    {
        HasHit = false;
        IsLifeLeech = false;
        IsAbsorbed = true;

        HealthDamage = 0;
        ShieldDamage = 0;
    }
}
