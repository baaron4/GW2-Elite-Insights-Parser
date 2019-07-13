namespace LuckParser.Parser.ParsedData.CombatEvents
{
    public class SpawnEvent : AbstractStatusEvent
    {
        public SpawnEvent(CombatItem evtcItem, AgentData agentData, long offset) : base(evtcItem, agentData, offset)
        {

        }

    }
}
