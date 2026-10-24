namespace GW2EIEvtcParser.ParsedData;

public class CustomAnimatedGadgetCastEvent : AnimatedCastEvent
{
    // Custom
    internal CustomAnimatedGadgetCastEvent(AgentItem caster, SkillItem skill, long start, long dur, AnimationStatus status = AnimationStatus.Full) : base(caster, skill, start, dur, status)
    {
    }

    internal CustomAnimatedGadgetCastEvent(AgentItem caster, SkillItem skill, long start, long dur, AgentItem effectTarget, AnimationStatus status = AnimationStatus.Full) : this(caster, skill, start, dur, status)
    {
        EffectTarget = effectTarget;
    }
}
