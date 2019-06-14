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
        public int RemovedDuration { get; }

        public AbstractBuffRemoveEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData, long offset) : base(evtcItem, skillData, offset)
        {
            RemovedDuration = evtcItem.Value;
            By = agentData.GetAgentByInstID(evtcItem.DstMasterInstid > 0 ? evtcItem.DstMasterInstid : evtcItem.DstInstid, evtcItem.LogTime);
            ByMinion = evtcItem.DstMasterInstid > 0 ? agentData.GetAgentByInstID(evtcItem.DstInstid, evtcItem.LogTime) : null;
            To = agentData.GetAgentByInstID(evtcItem.SrcInstid, evtcItem.LogTime);
        }

        public AbstractBuffRemoveEvent(AgentItem by, AgentItem to, long time, int removedDuration, SkillItem buffSkill) : base(buffSkill, time)
        {
            RemovedDuration = removedDuration;
            By = by;
            To = to;
        }

        public override void TryFindSrc(ParsedLog log)
        {
        }
    }
}
