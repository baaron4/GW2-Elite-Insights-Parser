namespace GW2EIEvtcParser.ParsedData
{
    public abstract class AbstractBuffRemoveEvent : AbstractBuffEvent
    {
        public int RemovedDuration { get; }

        internal AbstractBuffRemoveEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, skillData)
        {
            RemovedDuration = evtcItem.Value;
            By = agentData.GetAgent(evtcItem.DstAgent);
            To = agentData.GetAgent(evtcItem.SrcAgent);
        }

        internal AbstractBuffRemoveEvent(AgentItem by, AgentItem to, long time, int removedDuration, SkillItem buffSkill) : base(buffSkill, time)
        {
            RemovedDuration = removedDuration;
            By = by;
            To = to;
        }

        internal override void TryFindSrc(ParsedEvtcLog log)
        {
        }
    }
}
