namespace GW2EIEvtcParser.ParsedData;

public class DespawnEvent : StatusEvent
{
    internal DespawnEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
    {

    }

    internal DespawnEvent(AgentItem src, long time) : base(src, time)
    {

    }

}
