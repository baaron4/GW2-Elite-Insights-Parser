namespace GW2EIEvtcParser.ParsedData;

public class VisibilityEvent : StatusEvent
{
    public readonly bool Visible;

    internal VisibilityEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
    {
        Visible = evtcItem.IsStateChange == ArcDPSEnums.StateChange.StealthChange ? evtcItem.DstAgent == 1 : evtcItem.Value == 1;
    }

}
