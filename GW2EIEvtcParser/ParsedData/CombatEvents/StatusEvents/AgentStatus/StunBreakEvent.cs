namespace GW2EIEvtcParser.ParsedData;

public class StunBreakEvent : StatusEvent
{
    public int RemainingDuration { get; private set; }
    internal StunBreakEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
    {
        RemainingDuration = evtcItem.Value;
    }

}
