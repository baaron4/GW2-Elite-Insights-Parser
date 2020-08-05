namespace GW2EIEvtcParser.ParsedData
{
    public class MaxHealthUpdateEvent : AbstractStatusEvent
    {
        public int MaxHealth { get; }

        internal MaxHealthUpdateEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
            MaxHealth = (int)evtcItem.DstAgent;
        }

    }
}
