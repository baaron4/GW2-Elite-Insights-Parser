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
        public WeaponSwapEvent(CombatItem evtcItem, AgentData agentData, long offset) : base(evtcItem, agentData, offset)
        {
            SwappedTo = (int)evtcItem.DstAgent;
            SkillId = SkillItem.WeaponSwapId;
            ExpectedDuration = 50;
            ActualDuration = 50;
        }
    }
}
