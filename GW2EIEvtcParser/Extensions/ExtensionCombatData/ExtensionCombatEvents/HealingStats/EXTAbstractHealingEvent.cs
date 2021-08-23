using System;
using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.Extensions.HealingStatsExtensionHandler;

namespace GW2EIEvtcParser.Extensions
{
    public abstract class EXTAbstractHealingEvent : AbstractDamageEvent
    {
        public int HealingDone { get; protected set; }

        public bool SrcIsPeer { get; }
        public bool DstIsPeer { get; }

        internal EXTAbstractHealingEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData, bool hasDuplicate) : base(evtcItem, agentData, skillData)
        {
            if (!hasDuplicate)
            {
                SrcIsPeer = From == CreditedFrom;
            } 
            else
            {
                // TODO: to fill
            }
        }

        public EXTHealingType GetHealingType(ParsedEvtcLog log)
        {
            return log.CombatData.EXTHealingCombatData.GetHealingType(Skill, log);
        }

        public override bool ConditionDamageBased(ParsedEvtcLog log)
        {
            return false;
        }

    }
}
