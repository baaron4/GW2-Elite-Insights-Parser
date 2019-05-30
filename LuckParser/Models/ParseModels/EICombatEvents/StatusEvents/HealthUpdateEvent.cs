using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class HealthUpdateEvent : AbstractStatusEvent
    {
        public double HPPercent { get; }

        public HealthUpdateEvent(CombatItem evtcItem, AgentData agentData, long offset) : base(evtcItem, agentData, offset)
        {
            HPPercent = evtcItem.DstAgent / 100.0;
        }

    }
}
