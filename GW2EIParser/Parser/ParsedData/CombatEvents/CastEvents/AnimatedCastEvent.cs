namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public class AnimatedCastEvent : AbstractCastEvent
    {
        public AnimatedCastEvent(CombatItem startItem, CombatItem endItem, AgentData agentData, SkillData skillData) : base(startItem, agentData, skillData)
        {
            Duration = endItem.Value;
            Interrupted = endItem.IsActivation == ParseEnum.Activation.CancelCancel;
            FullAnimation = endItem.IsActivation == ParseEnum.Activation.Reset;
            ReducedAnimation = endItem.IsActivation == ParseEnum.Activation.CancelFire;
            if (Skill.ID == SkillItem.DodgeId)
            {
                Duration = 750;
            }
        }

        public AnimatedCastEvent(CombatItem startItem, AgentData agentData, SkillData skillData, long logEnd) : base(startItem, agentData, skillData)
        {
            Duration = ExpectedDuration;
            if (Duration + Time > logEnd)
            {
                Duration = (int)(logEnd - Time);
            }
            if (Skill.ID == SkillItem.DodgeId)
            {
                Duration = 750;
            }
        }


        public AnimatedCastEvent(long time, SkillItem skill, int duration, AgentItem caster) : base(time, skill, caster)
        {
            Duration = duration;
            ExpectedDuration = duration;
        }
    }
}
