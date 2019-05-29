using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class BuffApplyEvent : AbstractBuffEvent
    {
        public BuffApplyEvent(CombatItem evtcItem, AgentData agentData, long offset) : base(evtcItem, offset)
        {
            Src = agentData.GetAgentByInstID(evtcItem.SrcMasterInstid > 0 ? evtcItem.SrcMasterInstid : evtcItem.SrcInstid, evtcItem.Time);
            Dst = agentData.GetAgentByInstID(evtcItem.DstInstid, evtcItem.Time);
        }

        public override void TryFindSrc(ParsedLog log)
        {
        }

        public override void UpdateSimulator(BoonSimulator simulator)
        {
            simulator.Add(Value, Src, Time);
        }
    }
}
