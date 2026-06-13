namespace GW2EIEvtcParser.ParsedData;

public class VisibilityEvent : StatusEvent
{
    public readonly bool Visible;

    public readonly bool Gadget;

    internal VisibilityEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
    {
        Visible = evtcItem.IsStateChange == ArcDPSEnums.StateChange.Targetable ? evtcItem.Value == 1 : evtcItem.DstAgent == 1;
        Gadget = evtcItem.IsStateChange == ArcDPSEnums.StateChange.GadgetNameVisible;
    }

}
