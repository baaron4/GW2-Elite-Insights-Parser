namespace GW2EIEvtcParser.ParsedData;

public class CustomAnimatedCastEvent : AnimatedCastEvent
{
    // Custom
    internal CustomAnimatedCastEvent(AgentItem caster, SkillItem skill, long start, long dur, AnimationStatus status = AnimationStatus.Full) : base(caster, skill, start, dur, status)
    {
    }

    internal CustomAnimatedCastEvent(AgentItem caster, SkillItem skill, long start, long dur, AgentItem target, AnimationStatus status = AnimationStatus.Full) : base(caster, skill, start, dur, target, status)
    {
    }
}
