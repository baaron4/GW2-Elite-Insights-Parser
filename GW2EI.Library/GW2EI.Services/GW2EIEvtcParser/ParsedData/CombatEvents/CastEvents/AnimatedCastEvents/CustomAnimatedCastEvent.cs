using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.ParsedData;

public class CustomAnimatedCastEvent : AnimatedCastEvent
{
    // Custom
    internal CustomAnimatedCastEvent(AgentItem caster, SkillItem skill, long start, long dur) : base(caster, skill, start, dur)
    {
    }

    internal CustomAnimatedCastEvent(AgentItem caster, SkillItem skill, long start, long dur, AgentItem effectTarget) : this(caster, skill, start, dur)
    {
        EffectTarget = effectTarget;
    }
}
