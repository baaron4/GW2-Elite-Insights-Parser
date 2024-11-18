namespace GW2EIEvtcParser.ParsedData;

public class GliderEvent : StatusEvent
{
    public bool GliderDeployed { get; private set; }
    internal GliderEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
    {
        GliderDeployed = evtcItem.Value == 1;
    }

}
