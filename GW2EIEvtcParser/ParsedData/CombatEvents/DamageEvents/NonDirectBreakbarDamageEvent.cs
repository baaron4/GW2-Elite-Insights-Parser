using System;

namespace GW2EIEvtcParser.ParsedData
{
    public class NonDirectBreakbarDamageEvent : AbstractBreakbarDamageEvent
    {
        internal NonDirectBreakbarDamageEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
            Damage = Math.Round(evtcItem.BuffDmg / 10.0, 1);
        }
    }
}
