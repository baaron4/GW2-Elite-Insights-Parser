using System;
using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.Extensions.HealingStatsExtensionHandler;

namespace GW2EIEvtcParser.Extensions
{
    public abstract class EXTAbstractHealingExtensionEvent : AbstractDamageEvent
    {

        public bool SrcIsPeer { get; }
        public bool DstIsPeer { get; }

        protected const byte SrcPeerMask = 128;
        protected const byte DstPeerMask = 64;

        internal EXTAbstractHealingExtensionEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
            SrcIsPeer = (evtcItem.IsOffcycle & SrcPeerMask) > 0;
            DstIsPeer = (evtcItem.IsOffcycle & DstPeerMask) > 0;
            if (!SrcIsPeer && !DstIsPeer)
            {
                SrcIsPeer = true;
            }
        }

        public override bool ConditionDamageBased(ParsedEvtcLog log)
        {
            return false;
        }

    }
}
