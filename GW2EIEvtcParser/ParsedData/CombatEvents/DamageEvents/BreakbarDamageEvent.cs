using System;

namespace GW2EIEvtcParser.ParsedData
{
    public class BreakbarDamageEvent : AbstractDamageEvent<double>
    {
        internal BreakbarDamageEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
            Damage = Math.Round(evtcItem.Value / 10.0, 1);
        }
    }
}
