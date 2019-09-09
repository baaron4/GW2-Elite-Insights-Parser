namespace LuckParser.Parser.ParsedData.CombatEvents
{
    public class AnimatedCastEvent : AbstractCastEvent
    {
        public AnimatedCastEvent(CombatItem startItem, CombatItem endItem, AgentData agentData, SkillData skillData, long offset) : base(startItem, agentData, skillData, offset)
        {
            ActualDuration = endItem.Value;
            Interrupted = endItem.IsActivation == ParseEnum.EvtcActivation.CancelCancel;
            FullAnimation = endItem.IsActivation == ParseEnum.EvtcActivation.Reset;
            ReducedAnimation = endItem.IsActivation == ParseEnum.EvtcActivation.CancelFire;
            if (Skill.ID == SkillItem.DodgeId)
            {
                ActualDuration = 750;
            }
        }

        public AnimatedCastEvent(CombatItem startItem, AgentData agentData, SkillData skillData, long offset, long logEnd) : base(startItem, agentData, skillData, offset)
        {
            ActualDuration = ExpectedDuration;
            if (ActualDuration + Time > logEnd - offset)
            {
                ActualDuration = (int)(logEnd - offset - Time);
            }
            if (Skill.ID == SkillItem.DodgeId)
            {
                ActualDuration = 750;
            }
        }


        public AnimatedCastEvent(long time, SkillItem skill, int duration, AgentItem caster) : base(time, skill, caster)
        {
            ActualDuration = duration;
            ExpectedDuration = duration;
        }
    }
}
