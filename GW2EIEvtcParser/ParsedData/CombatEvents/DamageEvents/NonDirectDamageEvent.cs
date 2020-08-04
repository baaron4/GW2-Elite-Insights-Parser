using GW2EIEvtcParser.EIData;
using GW2EIUtils;

namespace GW2EIEvtcParser.ParsedData
{
    public class NonDirectDamageEvent : AbstractDamageEvent
    {
        private int _isCondi = -1;

        public NonDirectDamageEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
            Damage = evtcItem.BuffDmg;
            ArcDPSEnums.ConditionResult result = ArcDPSEnums.GetConditionResult(evtcItem.Result);

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
