namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public abstract class AbstractBuffRemoveEvent : AbstractBuffEvent
    {
        public int RemovedDuration { get; }

        public AbstractBuffRemoveEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, skillData)
        {
            RemovedDuration = evtcItem.Value;
            InternalBy = agentData.GetAgent(evtcItem.DstAgent);
            To = agentData.GetAgent(evtcItem.SrcAgent);
        }

        public AbstractBuffRemoveEvent(AgentItem by, AgentItem to, long time, int removedDuration, SkillItem buffSkill) : base(buffSkill, time)
        {
            RemovedDuration = removedDuration;
            InternalBy = by;
            To = to;
        }

        public override void TryFindSrc(ParsedLog log)
        {
        }
    }
}
