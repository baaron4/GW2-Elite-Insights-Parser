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
        private readonly ParseEnum.ConditionResult _result;

        public NonDirectDamageEvent(CombatItem evtcItem, AgentData agentData, BoonsContainer boons, long offset) : base(evtcItem, agentData, offset)
        {
            IsIndirectDamage = true;
            IsCondi = false;
            if (boons.BoonsByIds.TryGetValue(evtcItem.SkillID, out Boon boon))
            {
                IsCondi = (boon.Nature == Boon.BoonNature.Condition);
            }
            Damage = evtcItem.BuffDmg;
            _result = ParseEnum.GetConditionResult(evtcItem.Result);
        }

        public override bool IsAbsorb()
        {
            return _result == ParseEnum.ConditionResult.InvulByBuff || 
                _result == ParseEnum.ConditionResult.InvulByPlayerSkill1 ||
                _result == ParseEnum.ConditionResult.InvulByPlayerSkill2 ||
                _result == ParseEnum.ConditionResult.InvulByPlayerSkill3;
        }

        public override bool IsBlind()
        {
            return false;
        }

        public override bool IsBlock()
        {
            return false;
        }

        public override bool IsCrit()
        {
            return false;
        }

        public override bool IsDowned()
        {
            return false;
        }

        public override bool IsEvade()
        {
            return false;
        }

        public override bool IsGlance()
        {
            return false;
        }

        public override bool IsHit()
        {
            return _result == ParseEnum.ConditionResult.ExpectedToHit;
        }

        public override bool IsInterrupt()
        {
            return false;
        }

        public override bool IsKilled()
        {
            return false;
        }
    }
}
