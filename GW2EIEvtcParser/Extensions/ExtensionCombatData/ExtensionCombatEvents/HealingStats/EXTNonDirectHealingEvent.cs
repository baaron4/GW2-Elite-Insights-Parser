using System;
using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.Extensions
{
    public class EXTNonDirectHealingEvent : EXTAbstractHealingEvent
    {

        internal EXTNonDirectHealingEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData, bool hasDuplicate = false) : base(evtcItem, agentData, skillData, hasDuplicate)
        {
            HealingDone = -evtcItem.BuffDmg;
            AgainstDowned = evtcItem.Pad1 == 1;
        }
    }
}
