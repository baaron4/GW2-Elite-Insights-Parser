namespace LuckParser.Parser.ParsedData.CombatEvents
{
    public abstract class AbstractStatusEvent : AbstractCombatEvent
    {
        public AgentItem Src { get; protected set; }

        public AbstractStatusEvent(CombatItem evtcItem, AgentData agentData, long offset) : base(evtcItem.LogTime, offset)
        {
            Src = agentData.GetAgent(evtcItem.SrcAgent);
#if DEBUG
            OriginalCombatEvent = evtcItem;
#endif
        }

    }
}
