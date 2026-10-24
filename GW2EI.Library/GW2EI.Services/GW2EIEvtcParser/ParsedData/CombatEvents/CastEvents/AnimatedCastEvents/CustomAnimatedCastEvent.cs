namespace GW2EIEvtcParser.ParsedData;

public class CustomAnimatedCastEvent : AnimatedCastEvent
{
    // Custom
    internal CustomAnimatedCastEvent(AgentItem caster, SkillItem skill, long start, long dur, AnimationStatus status) : base(caster, skill, start, dur)
    {
        Status = status;
    }

    internal CustomAnimatedCastEvent(AgentItem caster, SkillItem skill, long start, long dur, AnimationStatus status, AgentItem effectTarget) : this(caster, skill, start, dur, status)
    {
        EffectTarget = effectTarget;
    }
}
