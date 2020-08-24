using System;

namespace GW2EIEvtcParser.ParsedData
{
    public class InstantCastEvent : AbstractCastEvent
    {

        internal InstantCastEvent(long time, SkillItem skill, AgentItem caster) : base(time, skill, caster)
        {
            Status = AnimationStatus.Instant;
            ActualDuration = 0;
            ExpectedDuration = 0;
        }

        internal InstantCastEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
            Status = AnimationStatus.Instant;
            ActualDuration = 0;
            ExpectedDuration = 0;
        }
    }
}
