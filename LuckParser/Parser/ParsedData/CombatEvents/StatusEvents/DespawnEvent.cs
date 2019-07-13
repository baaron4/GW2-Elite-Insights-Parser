namespace LuckParser.Parser.ParsedData.CombatEvents
{
    public class DespawnEvent : AbstractStatusEvent
    {
        public DespawnEvent(CombatItem evtcItem, AgentData agentData, long offset) : base(evtcItem, agentData, offset)
        {

        }

    }
}
