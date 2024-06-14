namespace GW2EIEvtcParser.ParsedData
{
    public class TeamChangeEvent : AbstractStatusEvent
    {
        public ulong TeamID { get; }

        public ulong TeamIDInto { get; }

        internal TeamChangeEvent(CombatItem evtcItem, AgentData agentData, int evtcVersion) : base(evtcItem, agentData)
        {
            if (evtcVersion >= ArcDPSEnums.ArcDPSBuilds.TeamChangeOnDespawn)
            {
                TeamID = (ulong)evtcItem.Value;
                TeamIDInto = evtcItem.DstAgent;
            } 
            else
            {
                TeamID = evtcItem.DstAgent;
            }
        }

    }
}
