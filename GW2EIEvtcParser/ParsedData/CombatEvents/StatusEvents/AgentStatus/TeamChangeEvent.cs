namespace GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;

public class TeamChangeEvent : StatusEvent
{
    public readonly ulong TeamIDComingFrom;

    public readonly ulong TeamIDInto;

    internal TeamChangeEvent(CombatItem evtcItem, AgentData agentData, EvtcVersionEvent evtcVersion) : base(evtcItem, agentData)
    {
        TeamIDInto = GetTeamIDInto(evtcItem);
        if (evtcVersion.Build >= ArcDPSBuilds.TeamChangeOnDespawn)
        {
            TeamIDComingFrom = GetTeamIDComingFrom(evtcItem);
        }
    }

    internal static ulong GetTeamIDInto(CombatItem evtcItem)
    {
        return evtcItem.DstAgent;
    }

    internal static ulong GetTeamIDComingFrom(CombatItem evtcItem)
    {
        return (ulong)evtcItem.Value;
    }

}
