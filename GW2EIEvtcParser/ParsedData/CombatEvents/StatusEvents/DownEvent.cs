namespace GW2EIEvtcParser.ParsedData
{
    public class DownEvent : AbstractStatusEvent
    {
        public DownEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {

        }

        public DownEvent(AgentItem src, long time) : base(src, time)
        {

        }

    }
}
