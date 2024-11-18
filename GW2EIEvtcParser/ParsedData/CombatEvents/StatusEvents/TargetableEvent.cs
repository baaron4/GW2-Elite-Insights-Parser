namespace GW2EIEvtcParser.ParsedData;

public class TargetableEvent : StatusEvent
{
    public readonly bool Targetable;

    internal TargetableEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
    {
        Targetable = evtcItem.DstAgent == 1;
    }

}
