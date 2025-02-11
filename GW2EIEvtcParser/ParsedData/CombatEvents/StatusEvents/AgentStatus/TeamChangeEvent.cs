﻿namespace GW2EIEvtcParser.ParsedData;

public class TeamChangeEvent : StatusEvent
{
    public readonly ulong TeamIDComingFrom;

    public readonly ulong TeamIDInto;

    internal TeamChangeEvent(CombatItem evtcItem, AgentData agentData, EvtcVersionEvent evtcVersion) : base(evtcItem, agentData)
    {
        TeamIDInto = GetTeamIDInto(evtcItem);
        if (evtcVersion.Build >= ArcDPSEnums.ArcDPSBuilds.TeamChangeOnDespawn)
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
