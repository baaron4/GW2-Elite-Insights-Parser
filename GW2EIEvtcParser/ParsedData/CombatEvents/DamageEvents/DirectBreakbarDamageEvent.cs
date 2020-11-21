using System;

namespace GW2EIEvtcParser.ParsedData
{
    public class DirectBreakbarDamageEvent : AbstractBreakbarDamageEvent
    {
        internal DirectBreakbarDamageEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
            BreakbarDamage = evtcItem.Value / 10.0;
        }

        public override bool IsCondi(ParsedEvtcLog log)
        {
            return false;
        }
    }
}
