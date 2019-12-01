namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public class TeamChangeEvent : AbstractStatusEvent
    {
        public ulong TeamID { get; }

        public TeamChangeEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
            TeamID = evtcItem.DstAgent;
        }

    }
}
