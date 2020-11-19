using System;

namespace GW2EIEvtcParser.ParsedData
{
    public abstract class AbstractBreakbarDamageEvent : AbstractDamageEvent<double>
    {
        internal AbstractBreakbarDamageEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
        }
    }
}
