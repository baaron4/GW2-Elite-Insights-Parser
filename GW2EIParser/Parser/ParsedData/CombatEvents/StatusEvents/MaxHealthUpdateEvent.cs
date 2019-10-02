namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public class MaxHealthUpdateEvent : AbstractStatusEvent
    {
        public int MaxHealth { get; }

        public MaxHealthUpdateEvent(CombatItem evtcItem, AgentData agentData, long offset) : base(evtcItem, agentData, offset)
        {
            MaxHealth = (int)evtcItem.DstAgent;
        }

    }
}
