namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public abstract class AbstractBuffStackEvent : AbstractBuffEvent
    {
        protected uint BuffInstance { get; set; }

        protected AbstractBuffStackEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData, long offset) : base(evtcItem, skillData, offset)
        {
            To = agentData.GetAgent(evtcItem.SrcAgent);
        }

        public override void TryFindSrc(ParsedLog log)
        {
        }
    }
}

