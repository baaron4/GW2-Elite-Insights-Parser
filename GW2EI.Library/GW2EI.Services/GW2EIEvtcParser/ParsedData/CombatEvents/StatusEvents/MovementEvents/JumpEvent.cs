namespace GW2EIEvtcParser.ParsedData;

public class JumpEvent : StatusEvent
{
    public readonly bool OnLanding;
    public readonly byte SomethingBehaviorRelated;

    public JumpEvent? Landing { get; private set; }

    public long LandingTime => Landing?.Time ?? Time;
    internal JumpEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
    {
        OnLanding = evtcItem.DstAgent == 0;
        if (OnLanding)
        {
            Landing = this;
        }
        SomethingBehaviorRelated = evtcItem.IsOffcycle;
    }

    internal bool SetLanding(JumpEvent landing)
    {
        if (Landing == null)
        {
            Landing = landing;
            return true;
        }
        return false;
    }

}
