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
        public NonDirectDamageEvent(CombatItem evtcItem, AgentData agentData, BoonsContainer boons, long offset) : base(evtcItem, agentData, offset)
        {
            IsIndirectDamage = true;
            IsCondi = false;
            if (boons.BoonsByIds.TryGetValue(evtcItem.SkillID, out Boon boon))
            {
                IsCondi = (boon.Nature == Boon.BoonNature.Condition);
            }
            Damage = evtcItem.BuffDmg;
            ParseEnum.ConditionResult result = ParseEnum.GetConditionResult(evtcItem.Result);

            IsAbsorb = result == ParseEnum.ConditionResult.InvulByBuff ||
                result == ParseEnum.ConditionResult.InvulByPlayerSkill1 ||
                result == ParseEnum.ConditionResult.InvulByPlayerSkill2 ||
                result == ParseEnum.ConditionResult.InvulByPlayerSkill3;
            IsHit = result == ParseEnum.ConditionResult.ExpectedToHit;
        }
    }
}
