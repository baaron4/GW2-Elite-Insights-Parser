using GW2EIEvtcParser.EIData;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.ParsedData
{
    public class NonDirectHealthDamageEvent : AbstractHealthDamageEvent
    {
        private readonly BuffCycle _cycle;

        public bool IsLifeLeech => _cycle == BuffCycle.NotCycle_DamageToTargetOnHit || _cycle == BuffCycle.NotCycle_DamageToTargetOnStackRemove;

        internal NonDirectHealthDamageEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData, ArcDPSEnums.ConditionResult result) : base(evtcItem, agentData, skillData)
        {
            HealthDamage = evtcItem.BuffDmg;
            IsAbsorbed = result == ConditionResult.InvulByBuff ||
                result == ConditionResult.InvulByPlayerSkill1 ||
                result == ConditionResult.InvulByPlayerSkill2 ||
                result == ConditionResult.InvulByPlayerSkill3;
            HasHit = result == ConditionResult.ExpectedToHit;
            ShieldDamage = evtcItem.IsShields > 0 ? HealthDamage : 0;
            _cycle = GetBuffCycle(evtcItem.IsOffcycle);
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
}
