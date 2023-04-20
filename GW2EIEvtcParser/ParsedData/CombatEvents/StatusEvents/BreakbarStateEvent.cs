namespace GW2EIEvtcParser.ParsedData
{
    public class BreakbarStateEvent : AbstractStatusEvent
    {

        public ArcDPSEnums.BreakbarState State { get; }

        internal BreakbarStateEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
            State = ArcDPSEnums.GetBreakbarState(evtcItem.Value);
        }

        internal BreakbarStateEvent(AgentItem agentItem, long time, ArcDPSEnums.BreakbarState state) : base(agentItem, time)
        {
            State = state;
        }

    }
}
