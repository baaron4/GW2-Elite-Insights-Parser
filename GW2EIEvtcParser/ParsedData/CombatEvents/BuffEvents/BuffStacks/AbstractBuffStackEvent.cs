namespace GW2EIEvtcParser.ParsedData
{
    public abstract class AbstractBuffStackEvent : AbstractBuffEvent
    {
        protected uint BuffInstance { get; set; }

        protected AbstractBuffStackEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, skillData)
        {
            To = agentData.GetAgent(evtcItem.SrcAgent);
        }

        public override void TryFindSrc(ParsedEvtcLog log)
        {
        }
    }
}

