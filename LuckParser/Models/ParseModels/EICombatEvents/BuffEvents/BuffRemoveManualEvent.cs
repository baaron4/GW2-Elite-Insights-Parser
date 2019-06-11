using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class BuffRemoveManualEvent : AbstractBuffRemoveEvent
    {
        public BuffRemoveManualEvent(CombatItem evtcItem, AgentData agentData, long offset) : base(evtcItem, agentData, offset)
        {
        }

        public BuffRemoveManualEvent(AgentItem by, AgentItem to, long time, int removedDuration, long buffID) : base(by, to, time, removedDuration, buffID)
        {
        }

        public override bool IsBoonSimulatorCompliant(long fightEnd)
        {
            return false; // don't consider manual remove events
        }

        public override void UpdateSimulator(BoonSimulator simulator)
        {
        }
    }
}
