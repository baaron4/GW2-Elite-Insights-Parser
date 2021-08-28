using GW2EIEvtcParser.EIData;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.ParsedData
{
    public class NonDirectHealthDamageEvent : AbstractHealthDamageEvent
    {
        private int _isCondi = -1;

        private readonly BuffCycle _cycle;

        public bool IsLifeLeech => _cycle == BuffCycle.NotCycle_DamageToTargetOnHit || _cycle == BuffCycle.NotCycle_DamageToTargetOnStackRemove;

        internal NonDirectHealthDamageEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData, ArcDPSEnums.ConditionResult result) : base(evtcItem, agentData, skillData)
        {
            HealthDamage = evtcItem.BuffDmg;
            IsAbsorbed = result == ArcDPSEnums.ConditionResult.InvulByBuff ||
                result == ArcDPSEnums.ConditionResult.InvulByPlayerSkill1 ||
                result == ArcDPSEnums.ConditionResult.InvulByPlayerSkill2 ||
                result == ArcDPSEnums.ConditionResult.InvulByPlayerSkill3;
            HasHit = result == ArcDPSEnums.ConditionResult.ExpectedToHit;
            ShieldDamage = evtcItem.IsShields > 0 ? HealthDamage : 0;
            _cycle = GetBuffCycle(evtcItem.IsOffcycle);
            AgainstDowned = evtcItem.Pad1 == 1;
        }

        public override bool ConditionDamageBased(ParsedEvtcLog log)
        {
            if (_isCondi == -1 && log.Buffs.BuffsByIds.TryGetValue(SkillId, out Buff b))
            {
                _isCondi = b.Nature == Buff.BuffNature.Condition ? 1 : 0;
            }
            return _isCondi == 1;
        }
    }
}
