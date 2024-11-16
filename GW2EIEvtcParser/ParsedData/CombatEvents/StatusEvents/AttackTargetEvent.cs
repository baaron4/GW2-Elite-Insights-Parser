namespace GW2EIEvtcParser.ParsedData;

public class AttackTargetEvent : StatusEvent
{
    public readonly AgentItem AttackTarget;

    public readonly bool Targetable;

    internal AttackTargetEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
    {
        AttackTarget = Src;
        Src = agentData.GetAgent(evtcItem.DstAgent, evtcItem.Time);
        Targetable = evtcItem.Value == 1;
    }

}
