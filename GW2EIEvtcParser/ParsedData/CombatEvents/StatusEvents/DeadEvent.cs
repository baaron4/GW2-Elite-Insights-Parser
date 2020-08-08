namespace GW2EIEvtcParser.ParsedData
{
    public class DeadEvent : AbstractStatusEvent
    {
        internal DeadEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {

        }

        internal DeadEvent(AgentItem src, long time) : base(src, time)
        {

        }

    }
}
