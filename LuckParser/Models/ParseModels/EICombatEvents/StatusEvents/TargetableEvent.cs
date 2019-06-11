using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class TargetableEvent : AbstractStatusEvent
    {
        public bool Targetable { get; }

        public TargetableEvent(CombatItem evtcItem, AgentData agentData, long offset) : base(evtcItem, agentData, offset)
        {
            Targetable = evtcItem.DstAgent == 1;
        }

    }
}
