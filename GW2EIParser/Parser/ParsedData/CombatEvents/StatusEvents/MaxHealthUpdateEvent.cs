namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public class MaxHealthUpdateEvent : AbstractStatusEvent
    {
        public int MaxHealth { get; }

        public MaxHealthUpdateEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
            MaxHealth = (int)evtcItem.DstAgent;
        }

    }
}
