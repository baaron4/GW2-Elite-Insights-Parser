namespace GW2EIEvtcParser.ParsedData;

public class ExitCombatEvent : CombatStatusEvent
{
    internal ExitCombatEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
    {
    }

}
