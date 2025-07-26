using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.ParsedData;

public abstract class AbstractBuffApplyEvent : BuffEvent
{
    public readonly uint BuffInstance;

    internal AbstractBuffApplyEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, skillData)
    {
        By = agentData.GetAgent(evtcItem.SrcAgent, evtcItem.Time);
        To = agentData.GetAgent(evtcItem.DstAgent, evtcItem.Time);
        BuffInstance = evtcItem.Pad;
    }

    internal AbstractBuffApplyEvent(AgentItem by, AgentItem to, long time, SkillItem buffSkill, IFF iff, uint id) : base(buffSkill, time, iff)
    {
        By = by.EnglobingAgentItem ?? by;
        To = to.EnglobingAgentItem ?? to;
        BuffInstance = id;
    }
}
