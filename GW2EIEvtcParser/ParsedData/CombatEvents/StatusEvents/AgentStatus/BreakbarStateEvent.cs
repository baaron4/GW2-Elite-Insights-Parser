namespace GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;

public class BreakbarStateEvent : StatusEvent
{

    public readonly BreakbarState State;

    internal BreakbarStateEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
    {
        State = GetBreakbarState(evtcItem);
    }

    internal static BreakbarState GetBreakbarState(CombatItem evtcItem)
    {
        return ArcDPSEnums.GetBreakbarState(evtcItem.Value);
    }

    internal BreakbarStateEvent(AgentItem agentItem, long time, BreakbarState state) : base(agentItem, time)
    {
        State = state;
    }

}
