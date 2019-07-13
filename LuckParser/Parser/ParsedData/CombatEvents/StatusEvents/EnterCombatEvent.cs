namespace LuckParser.Parser.ParsedData.CombatEvents
{
    public class EnterCombatEvent : AbstractStatusEvent
    {
        public EnterCombatEvent(CombatItem evtcItem, AgentData agentData, long offset) : base(evtcItem, agentData, offset)
        {

        }

    }
}
