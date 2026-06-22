using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.ParsedData;

public abstract class AbstractBuffRemoveEvent : BuffEvent
{
    public int RemovedDuration { get; protected set; }

    internal AbstractBuffRemoveEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, skillData)
    {
        RemovedDuration = evtcItem.Value;
        By = agentData.GetAgent(evtcItem.DstAgent, evtcItem.Time);
        To = agentData.GetAgent(evtcItem.SrcAgent, evtcItem.Time);
    }

    internal AbstractBuffRemoveEvent(AgentItem by, AgentItem to, long time, int removedDuration, SkillItem buffSkill, IFF iff) : base(buffSkill, time, iff)
    {
        RemovedDuration = removedDuration;
        By = by.EnglobingAgentItem;
        To = to.EnglobingAgentItem;
    }
}
