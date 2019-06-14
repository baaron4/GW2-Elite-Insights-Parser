using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class WeaponSwapEvent : AbstractCastEvent
    {
        public WeaponSwapEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData, long offset) : base(evtcItem, agentData, skillData, offset)
        {
            SwappedTo = (int)evtcItem.DstAgent;
            Skill = skillData.Get(SkillItem.WeaponSwapId);
            ExpectedDuration = 50;
            ActualDuration = 50;
        }
    }
}
