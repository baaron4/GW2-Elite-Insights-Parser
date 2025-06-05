namespace GW2EIEvtcParser.ParsedData;

public class EnterCombatEvent : CombatStatusEvent
{
    internal EnterCombatEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
    {
    }

}
