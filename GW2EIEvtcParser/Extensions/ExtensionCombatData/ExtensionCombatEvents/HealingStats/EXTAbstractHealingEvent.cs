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

        protected const byte SrcPeerMask = 128;
        protected const byte DstPeerMask = 64;

        internal EXTAbstractHealingEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
            SrcIsPeer = (evtcItem.IsOffcycle & SrcPeerMask) > 0;
            DstIsPeer = (evtcItem.IsOffcycle & DstPeerMask) > 0;
            if (!SrcIsPeer && !DstIsPeer)
            {
                SrcIsPeer = true;
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
