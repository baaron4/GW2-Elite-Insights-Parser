using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public abstract class AbstractDamageEvent : AbstractCombatEvent
    {
        public AgentItem From { get; }
        public AgentItem MasterFrom { get; }
        public AgentItem To { get; }
        public AgentItem MasterTo { get; }

        public long SkillID { get; }

        public long Damage { get; protected set; }

        public AbstractDamageEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem)
        {
            From = agentData.GetAgentByInstID(evtcItem.SrcInstid, evtcItem.Time);
            MasterFrom = evtcItem.SrcMasterInstid > 0 ? agentData.GetAgentByInstID(evtcItem.SrcMasterInstid, evtcItem.Time) : null;
            To = agentData.GetAgentByInstID(evtcItem.DstInstid, evtcItem.Time);
            MasterTo = evtcItem.DstMasterInstid > 0 ? agentData.GetAgentByInstID(evtcItem.DstMasterInstid, evtcItem.Time) : null;
            SkillID = evtcItem.SkillID;
        }

        public void OverrideTime(long time)
        {
            Time = time;
        }

    }
}
