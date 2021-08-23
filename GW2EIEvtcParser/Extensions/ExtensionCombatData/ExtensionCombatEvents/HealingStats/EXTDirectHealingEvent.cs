using System;
using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.Extensions
{
    public class EXTDirectHealingEvent : EXTAbstractHealingEvent
    {

        internal EXTDirectHealingEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData, bool hasDuplicate = false) : base(evtcItem, agentData, skillData, hasDuplicate)
        {
            HealingDone = -evtcItem.Value;
            AgainstDowned = evtcItem.IsOffcycle == 1;
        }
    }
}
