using System;
using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.Extensions
{
    public class EXTNonDirectHealingEvent : EXTAbstractHealingEvent
    {

        internal EXTNonDirectHealingEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
            HealingDone = -evtcItem.BuffDmg;
        }

        public override bool ConditionDamageBased(ParsedEvtcLog log)
        {
            return false;
        }
    }
}
