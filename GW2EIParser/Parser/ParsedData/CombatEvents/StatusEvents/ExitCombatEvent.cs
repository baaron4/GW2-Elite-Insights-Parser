namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public class ExitCombatEvent : AbstractStatusEvent
    {
        public ExitCombatEvent(CombatItem evtcItem, AgentData agentData, long offset) : base(evtcItem, agentData, offset)
        {

        }

    }
}
