namespace GW2EIEvtcParser.ParsedData;

public class CustomAnimatedCastEvent : AnimatedCastEvent
{
    internal CustomAnimatedCastEvent(AgentItem caster, SkillItem skill, long start, long dur) : base(caster, skill, start, dur)
    {
    }

    internal CustomAnimatedCastEvent(AgentItem caster, SkillItem skill, long start, long dur, AgentItem effectTarget) : this(caster, skill, start, dur)
    {
        EffectTarget = effectTarget;
    }
    // Custom
    internal CustomAnimatedCastEvent(AgentItem caster, SkillItem skill, long start, long dur, AnimationStatus status) : this(caster, skill, start, dur)
    {
        Status = status;
    }

    internal CustomAnimatedCastEvent(AgentItem caster, SkillItem skill, long start, long dur, AnimationStatus status, AgentItem effectTarget) : this(caster, skill, start, dur, effectTarget)
    {
        Status = status;
    }
}
