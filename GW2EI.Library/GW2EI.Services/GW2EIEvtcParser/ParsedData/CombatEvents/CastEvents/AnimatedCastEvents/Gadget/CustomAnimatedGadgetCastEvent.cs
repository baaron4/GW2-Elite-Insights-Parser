namespace GW2EIEvtcParser.ParsedData;

public class CustomAnimatedGadgetCastEvent : AnimatedCastEvent
{
    // Custom
    internal CustomAnimatedGadgetCastEvent(AgentItem caster, SkillItem skill, long start, long dur) : base(caster, skill, start, dur)
    {
    }

    internal CustomAnimatedGadgetCastEvent(AgentItem caster, SkillItem skill, long start, long dur, AgentItem target) : this(caster, skill, start, dur)
    {
        Target = target;
    }
}
