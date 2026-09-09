namespace GW2EIEvtcParser.ParsedData;

public class GliderEvent : StatusEvent
{
    public bool GliderDeployed { get; private set; }

    private GliderEvent? GliderClosed;
    public long GliderClosedTime => GliderClosed?.Time ?? Time;
    internal GliderEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
    {
        GliderDeployed = evtcItem.Value == 1;
    }

    internal bool SetGliderClosed(GliderEvent gliderClosed)
    {
        if (GliderClosed == null)
        {
            GliderClosed = gliderClosed;
            return true;
        }
        return false;
    }

}
