namespace GW2EIEvtcParser.ParsedData
{
    public class TeamChangeEvent : AbstractStatusEvent
    {
        public ulong TeamIDComingFrom { get; }

        public ulong TeamIDInto { get; }

        internal TeamChangeEvent(CombatItem evtcItem, AgentData agentData, int evtcVersion) : base(evtcItem, agentData)
        {
            TeamIDInto = evtcItem.DstAgent;
            if (evtcVersion >= ArcDPSEnums.ArcDPSBuilds.TeamChangeOnDespawn)
            {
                TeamIDComingFrom = (ulong)evtcItem.Value;
            } 
        }

    }
}
