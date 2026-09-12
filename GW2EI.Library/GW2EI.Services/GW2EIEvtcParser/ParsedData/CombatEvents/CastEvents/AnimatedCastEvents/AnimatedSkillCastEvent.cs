using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.ParsedData;

public class AnimatedSkillCastEvent : AnimatedCastEvent
{
    internal AnimatedSkillCastEvent(CombatItem? startItem, AgentData agentData, SkillData skillData, CombatItem? endItem, long maxEnd) : base(startItem, agentData, skillData, endItem, maxEnd)
    {
    }

    // Custom
    internal AnimatedSkillCastEvent(AgentItem caster, SkillItem skill, long start, long dur) : base(caster, skill, start, dur)
    {
    }

    internal AnimatedSkillCastEvent(AgentItem caster, SkillItem skill, long start, long dur, AgentItem effectTarget) : this(caster, skill, start, dur)
    {
        EffectTarget = effectTarget;
    }
}
