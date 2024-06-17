using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.ParsedData
{
    public abstract class AbstractBuffApplyEvent : AbstractBuffEvent
    {
        public uint BuffInstance { get; }

        internal AbstractBuffApplyEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, skillData)
        {
            By = agentData.GetAgent(evtcItem.SrcAgent, evtcItem.Time);
            To = agentData.GetAgent(evtcItem.DstAgent, evtcItem.Time);
            BuffInstance = evtcItem.Pad;
        }

        internal AbstractBuffApplyEvent(AgentItem by, AgentItem to, long time, SkillItem buffSkill, IFF iff, uint id) : base(buffSkill, time, iff)
        {
            By = by;
            To = to;
            BuffInstance = id;
        }
    }
}
