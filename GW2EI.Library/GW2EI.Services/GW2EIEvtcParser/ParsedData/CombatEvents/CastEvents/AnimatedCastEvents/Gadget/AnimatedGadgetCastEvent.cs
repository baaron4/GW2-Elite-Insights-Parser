namespace GW2EIEvtcParser.ParsedData;

public class AnimatedGadgetCastEvent : AnimatedCastEvent
{
    // Custom
    internal AnimatedGadgetCastEvent(AgentItem caster, SkillItem skill, long start, long dur, AnimationStatus status = AnimationStatus.Full) : base(caster, skill, start, dur, status)
    {
    }

    internal AnimatedGadgetCastEvent(AgentItem caster, SkillItem skill, long start, long dur, AgentItem target, AnimationStatus status = AnimationStatus.Full) : base(caster, skill, start, dur, target, status)
    {
    }
}
