using System;

namespace GW2EIEvtcParser.ParsedData
{
    public class DirectBreakbarDamageEvent : AbstractBreakbarDamageEvent
    {
        internal DirectBreakbarDamageEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
            Damage = Math.Round(evtcItem.Value / 10.0, 1);
        }

        public override bool IsCondi(ParsedEvtcLog log)
        {
            return false;
        }
    }
}
