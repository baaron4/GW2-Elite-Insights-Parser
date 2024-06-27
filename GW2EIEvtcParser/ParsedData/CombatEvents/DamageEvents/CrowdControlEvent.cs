using System;

namespace GW2EIEvtcParser.ParsedData
{
    public class CrowdControlEvent : AbstractDamageEvent
    {
        //

        internal CrowdControlEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
        }

        public override bool ConditionDamageBased(ParsedEvtcLog log)
        {
            return false;
        }
    }
}
