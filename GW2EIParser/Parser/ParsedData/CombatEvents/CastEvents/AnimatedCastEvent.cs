using System;

namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public class AnimatedCastEvent : AbstractCastEvent
    {
        private readonly int _scaledActualDuration;
        //private readonly int _effectHappenedDuration;

        private AnimatedCastEvent(CombatItem startItem, AgentData agentData, SkillData skillData) : base(startItem, agentData, skillData)
        {
            UnderQuickness = startItem.IsActivation == ParseEnum.Activation.Quickness;
            ExpectedDuration = startItem.BuffDmg > 0 ? startItem.BuffDmg : startItem.Value;
            //_effectHappenedDuration = startItem.Value;
        }


        public AnimatedCastEvent(CombatItem startItem, CombatItem endItem, AgentData agentData, SkillData skillData) : this(startItem, agentData, skillData)
        {
            ActualDuration = endItem.Value;
            _scaledActualDuration = endItem.BuffDmg;
            if (Skill.ID == SkillItem.DodgeId)
            {
                ExpectedDuration = 750;
                ActualDuration = 750;
            }
            switch (endItem.IsActivation)
            {
                case ParseEnum.Activation.CancelCancel:
                    Status = AnimationStatus.INTERRUPTED;
                    SavedDuration = -ActualDuration;
                    break;
                case ParseEnum.Activation.Reset:
                    Status = AnimationStatus.FULL;
                    break;
                case ParseEnum.Activation.CancelFire:
                    if (_scaledActualDuration > 0)
                    {
                        double ratio = (double)ActualDuration / _scaledActualDuration;
                        int nonScaledExpectedDuration = (int)Math.Round(ExpectedDuration * ratio);
                        SavedDuration = Math.Max(nonScaledExpectedDuration - ActualDuration, 0);
                    } 
                    else
                    {
                        SavedDuration = Math.Max(ExpectedDuration - ActualDuration, 0);
                    }
                    Status = AnimationStatus.REDUCED;
                    break;
            }
        }

        public AnimatedCastEvent(CombatItem startItem, AgentData agentData, SkillData skillData, long logEnd) : this(startItem, agentData, skillData)
        {
            if (Skill.ID == SkillItem.DodgeId)
            {
                ExpectedDuration = 750;
            }
            ActualDuration = ExpectedDuration;
            if (ActualDuration + Time > logEnd)
            {
                ActualDuration = (int)(logEnd - Time);
            }
        }


        public AnimatedCastEvent(long time, SkillItem skill, int duration, AgentItem caster) : base(time, skill, caster)
        {
            ActualDuration = duration;
            ExpectedDuration = duration;
        }
    }
}
