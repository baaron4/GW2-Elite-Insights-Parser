namespace GW2EIEvtcParser.ParsedData;

public class AnimatedSkillCastEvent : AnimatedCastEvent
{
    internal AnimatedSkillCastEvent(CombatItem? startItem, AgentData agentData, SkillData skillData, CombatItem? endItem, long maxEnd) : base(startItem, agentData, skillData, endItem, maxEnd)
    {
    }

    // Custom
    internal AnimatedSkillCastEvent(AgentItem caster, SkillItem skill, long start, long dur, AnimationStatus status = AnimationStatus.Full) : base(caster, skill, start, dur, status)
    {
    }

    internal AnimatedSkillCastEvent(AgentItem caster, SkillItem skill, long start, long dur, AgentItem target, AnimationStatus status = AnimationStatus.Full) : base(caster, skill, start, dur, target, status)
    {
    }
}
