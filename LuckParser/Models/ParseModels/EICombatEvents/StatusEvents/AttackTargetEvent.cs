using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class AttackTargetEvent : AbstractStatusEvent
    {
        public AgentItem AttackTarget { get; }

        public AttackTargetEvent(CombatItem evtcItem, AgentData agentData, long offset) : base(evtcItem, agentData, offset)
        {
            AttackTarget = agentData.GetAgent(evtcItem.SrcAgent, evtcItem.LogTime);
            Src = agentData.GetAgent(evtcItem.DstAgent, evtcItem.LogTime);
        }

    }
}
