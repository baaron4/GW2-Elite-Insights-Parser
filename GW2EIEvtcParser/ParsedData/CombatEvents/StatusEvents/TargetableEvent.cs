namespace GW2EIEvtcParser.ParsedData
{
    public class TargetableEvent : AbstractStatusEvent
    {
        public bool Targetable { get; }

        public TargetableEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
            Targetable = evtcItem.DstAgent == 1;
        }

    }
}
