namespace GW2EIEvtcParser.ParsedData;

public class CustomAnimatedCastEvent : AnimatedCastEvent
{
    // Custom
    internal CustomAnimatedCastEvent(AgentItem caster, SkillItem skill, long start, long dur, AnimationStatus status = AnimationStatus.Full) : base(caster, skill, start, dur, status)
    {
    }

    internal CustomAnimatedCastEvent(AgentItem caster, SkillItem skill, long start, long dur, AgentItem effectTarget, AnimationStatus status = AnimationStatus.Full) : base(caster, skill, start, dur, effectTarget, status)
    {
    }
}
