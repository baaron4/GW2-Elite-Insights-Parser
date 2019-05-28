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
        public ParseEnum.ConditionResult Result { get; }
        public bool IsCondi { get; }

        public NonDirectDamageEvent(CombatItem evtcItem, AgentData agentData, BoonsContainer boons) : base(evtcItem, agentData)
        {
            Result = ParseEnum.GetConditionResult(evtcItem.Result);
            if (boons.BoonsByIds.TryGetValue(evtcItem.SkillID, out Boon boon))
            {
                IsCondi = (boon.Nature == Boon.BoonNature.Condition);
            }
            Damage = evtcItem.BuffDmg;
        }

    }
}
