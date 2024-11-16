namespace GW2EIEvtcParser.ParsedData;

public class DespawnEvent : StatusEvent
{
    internal DespawnEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
    {

    }

}
