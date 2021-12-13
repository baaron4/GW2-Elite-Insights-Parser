using System;
using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.Extensions.HealingStatsExtensionHandler;

namespace GW2EIEvtcParser.Extensions
{
    public abstract class EXTAbstractBarrierEvent : EXTAbstractHealingExtensionEvent
    {
        public int BarrierGiven { get; protected set; }

        internal EXTAbstractBarrierEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
        }

    }
}
