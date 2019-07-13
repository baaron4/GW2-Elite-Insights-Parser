namespace LuckParser.Parser.ParsedData.CombatEvents
{
    public class TeamChangeEvent : AbstractStatusEvent
    {
        public ulong TeamID { get; }

        public TeamChangeEvent(CombatItem evtcItem, AgentData agentData, long offset) : base(evtcItem, agentData, offset)
        {
            TeamID = evtcItem.DstAgent;
        }

    }
}
