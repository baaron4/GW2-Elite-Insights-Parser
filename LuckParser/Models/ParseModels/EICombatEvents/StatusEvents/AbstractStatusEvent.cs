using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public abstract class AbstractStatusEvent : AbstractCombatEvent
    {
        public AgentItem Src { get; }

        public AbstractStatusEvent(CombatItem evtcItem, AgentData agentData, long offset) : base(evtcItem.LogTime, offset)
        {
            Src = agentData.GetAgent(evtcItem.SrcAgent, evtcItem.LogTime);
        }

    }
}
