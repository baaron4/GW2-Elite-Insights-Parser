using System;
using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.Extensions
{
    public abstract class EXTAbstractHealingEvent : AbstractDamageEvent
    {
        public int HealingDone { get; protected set; }

        internal EXTAbstractHealingEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
        }

    }
}
