namespace LuckParser.Parser.ParsedData.CombatEvents
{
    public class AliveEvent : AbstractStatusEvent
    {
        public AliveEvent(CombatItem evtcItem, AgentData agentData, long offset) : base(evtcItem, agentData, offset)
        {

        }

    }
}
