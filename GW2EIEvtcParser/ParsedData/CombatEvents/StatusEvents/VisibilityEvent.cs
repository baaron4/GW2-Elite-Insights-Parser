namespace GW2EIEvtcParser.ParsedData;

public class VisibilityEvent : StatusEvent
{
    public readonly bool Visible;

    internal VisibilityEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
    {
        Visible = evtcItem.Value == 1;
    }

}
