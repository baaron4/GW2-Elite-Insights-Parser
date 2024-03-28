namespace GW2EIEvtcParser.ParsedData
{
    public abstract class AbstractMarkerEvent : AbstractStatusEvent
    {

        internal AbstractMarkerEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
        }

    }
}
