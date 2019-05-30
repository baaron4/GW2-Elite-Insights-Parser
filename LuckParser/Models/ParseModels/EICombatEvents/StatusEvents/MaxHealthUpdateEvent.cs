using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class MaxHealthUpdateEvent : AbstractStatusEvent
    {
        public ulong MaxHealth { get; }

        public MaxHealthUpdateEvent(CombatItem evtcItem, AgentData agentData, long offset) : base(evtcItem, agentData, offset)
        {
            MaxHealth = evtcItem.DstAgent;
        }

    }
}
