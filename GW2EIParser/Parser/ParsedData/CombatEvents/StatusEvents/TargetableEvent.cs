namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public class TargetableEvent : AbstractStatusEvent
    {
        public bool Targetable { get; }

        public TargetableEvent(CombatItem evtcItem, AgentData agentData, long offset) : base(evtcItem, agentData, offset)
        {
            Targetable = evtcItem.DstAgent == 1;
        }

    }
}
