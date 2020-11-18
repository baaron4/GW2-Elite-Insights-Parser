using GW2EIEvtcParser.EIData;

namespace GW2EIEvtcParser.ParsedData
{
    public class NonDirectDamageEvent : AbstractHealthDamageEvent
    {
        private int _isCondi = -1;

        internal NonDirectDamageEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData, ArcDPSEnums.ConditionResult result) : base(evtcItem, agentData, skillData)
        {
            Damage = evtcItem.BuffDmg;
            IsAbsorbed = result == ArcDPSEnums.ConditionResult.InvulByBuff ||
                result == ArcDPSEnums.ConditionResult.InvulByPlayerSkill1 ||
                result == ArcDPSEnums.ConditionResult.InvulByPlayerSkill2 ||
                result == ArcDPSEnums.ConditionResult.InvulByPlayerSkill3;
            HasHit = result == ArcDPSEnums.ConditionResult.ExpectedToHit;
            ShieldDamage = evtcItem.IsShields > 0 ? Damage : 0;
        }

        public override bool IsCondi(ParsedEvtcLog log)
        {
            if (_isCondi == -1 && log.Buffs.BuffsByIds.TryGetValue(SkillId, out Buff b))
            {
                _isCondi = b.Nature == Buff.BuffNature.Condition ? 1 : 0;
            }
            return _isCondi == 1;
        }
    }
}
