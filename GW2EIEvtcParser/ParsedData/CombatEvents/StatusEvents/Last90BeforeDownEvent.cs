namespace GW2EIEvtcParser.ParsedData;

public class Last90BeforeDownEvent : AbstractStatusEvent
{
    public readonly long TimeSinceLast90;
    internal Last90BeforeDownEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
    {
        TimeSinceLast90 = (long)evtcItem.DstAgent;
    }

}
