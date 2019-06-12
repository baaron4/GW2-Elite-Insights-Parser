using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class AnimatedCastEvent : AbstractCastEvent
    {
        public AnimatedCastEvent(CombatItem startItem, CombatItem endItem, AgentData agentData, long offset) : base(startItem, agentData, offset)
        {
            ActualDuration = endItem.Value;
            Interrupted = endItem.IsActivation == ParseEnum.Activation.CancelCancel;
            FullAnimation = endItem.IsActivation == ParseEnum.Activation.Reset;
            ReducedAnimation = endItem.IsActivation == ParseEnum.Activation.CancelFire;
            if (SkillId == SkillItem.DodgeId)
            {
                ActualDuration = 750;
            }
        }

        public AnimatedCastEvent(CombatItem startItem, AgentData agentData, long offset, long logEnd) : base(startItem, agentData, offset)
        {
            ActualDuration = ExpectedDuration;
            if (ActualDuration + Time > logEnd - offset)
            {
                ActualDuration = (int)(logEnd - offset - Time);
            }
            if (SkillId == SkillItem.DodgeId)
            {
                ActualDuration = 750;
            }
        }
        

        public AnimatedCastEvent(long time, long skillID, int duration, AgentItem caster) : base(time, skillID, caster)
        {
            ActualDuration = duration;
            ExpectedDuration = duration;
        }
    }
}
