using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public abstract class AbstractBuffRemoveEvent : AbstractBuffEvent
    {
        public AbstractBuffRemoveEvent(CombatItem evtcItem, AgentData agentData, long offset) : base(evtcItem, offset)
        {
            By = agentData.GetAgentByInstID(evtcItem.DstMasterInstid > 0 ? evtcItem.DstMasterInstid : evtcItem.DstInstid, evtcItem.LogTime);
            To = agentData.GetAgentByInstID(evtcItem.SrcInstid, evtcItem.LogTime);
        }

        public AbstractBuffRemoveEvent(AgentItem by, AgentItem to, long time, int removedDuration, long buffID) : base(removedDuration, buffID, time)
        {
            By = by;
            To = to;
        }

        public override void TryFindSrc(ParsedLog log)
        {
        }
    }
}
