using System;

namespace GW2EIEvtcParser.ParsedData
{
    public class BreakbarDamageEvent : AbstractBaseDamageEvent
    {
        public double BreakbarDamage { get; }
        internal BreakbarDamageEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
            BreakbarDamage = Math.Round(evtcItem.Value / 10.0, 1);
        }
    }
}
