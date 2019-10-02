using GW2EIParser.EIData;

namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public class NonDirectDamageEvent : AbstractDamageEvent
    {
        private int _isCondi = -1;

        public NonDirectDamageEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData, long offset) : base(evtcItem, agentData, skillData, offset)
        {
            Damage = evtcItem.BuffDmg;
            ParseEnum.ConditionResult result = ParseEnum.GetConditionResult(evtcItem.Result);

            IsAbsorbed = result == ParseEnum.ConditionResult.InvulByBuff ||
                result == ParseEnum.ConditionResult.InvulByPlayerSkill1 ||
                result == ParseEnum.ConditionResult.InvulByPlayerSkill2 ||
                result == ParseEnum.ConditionResult.InvulByPlayerSkill3;
            HasHit = result == ParseEnum.ConditionResult.ExpectedToHit;
            ShieldDamage = evtcItem.IsShields > 0 ? evtcItem.Value : 0;
        }

        public override bool IsCondi(ParsedLog log)
        {
            if (_isCondi == -1 && log.Buffs.BuffsByIds.TryGetValue(SkillId, out Buff b))
            {
                _isCondi = b.Nature == Buff.BuffNature.Condition ? 1 : 0;
            }
            return _isCondi == 1;
        }
    }
}
