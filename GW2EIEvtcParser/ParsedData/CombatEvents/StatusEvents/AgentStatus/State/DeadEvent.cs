namespace GW2EIEvtcParser.ParsedData;

public class DeadEvent : StatusEvent
{
    internal DeadEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
    {

    }

    internal DeadEvent(AgentItem src, long time) : base(src, time)
    {

    }

}
