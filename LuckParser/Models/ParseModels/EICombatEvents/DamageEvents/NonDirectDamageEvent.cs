using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class NonDirectDamageEvent : AbstractDamageEvent
    {
        private int _isCondi = -1;

        public NonDirectDamageEvent(CombatItem evtcItem, AgentData agentData, long offset) : base(evtcItem, agentData, offset)
        {
            Damage = evtcItem.BuffDmg;
            ParseEnum.ConditionResult result = ParseEnum.GetConditionResult(evtcItem.Result);

            IsAbsorb = result == ParseEnum.ConditionResult.InvulByBuff ||
                result == ParseEnum.ConditionResult.InvulByPlayerSkill1 ||
                result == ParseEnum.ConditionResult.InvulByPlayerSkill2 ||
                result == ParseEnum.ConditionResult.InvulByPlayerSkill3;
            IsHit = result == ParseEnum.ConditionResult.ExpectedToHit;
        }

        public override bool IsCondi(ParsedLog log)
        {
            if (_isCondi == -1 && log.Boons.BoonsByIds.TryGetValue(SkillId, out Boon b))
            {
                _isCondi = b.Nature == Boon.BoonNature.Condition ? 1 : 0;
            }
            return _isCondi == 1;
        }
    }
}
