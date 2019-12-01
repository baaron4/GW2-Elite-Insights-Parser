namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public class HealthUpdateEvent : AbstractStatusEvent
    {
        public double HPPercent { get; }

        public HealthUpdateEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
            HPPercent = evtcItem.DstAgent / 100.0;
        }

    }
}
