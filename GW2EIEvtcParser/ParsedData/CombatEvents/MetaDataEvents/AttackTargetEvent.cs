namespace GW2EIEvtcParser.ParsedData;

public class AttackTargetEvent : MetaDataEvent
{
    public readonly AgentItem Src;
    public readonly AgentItem AttackTarget;

    private readonly bool Targetable;

    private IReadOnlyList<TargetableEvent>? _targetableEvents;

    internal AttackTargetEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem)
    {
        AttackTarget = agentData.GetAgent(evtcItem.SrcAgent, evtcItem.Time);
        Src = agentData.GetAgent(evtcItem.DstAgent, evtcItem.Time);
        Targetable = evtcItem.Value == 1;
    }

    public IReadOnlyList<TargetableEvent> GetTargetableEvents(CombatData combatData)
    {
        _targetableEvents ??= combatData.GetTargetableEventsBySrc(AttackTarget);
        return _targetableEvents;
    }

    public IReadOnlyList<TargetableEvent> GetTargetableEvents(ParsedEvtcLog log)
    {
        return GetTargetableEvents(log.CombatData);
    }

    public IReadOnlyList<TargetableEvent> GetTargetableEvents(IReadOnlyList<CombatItem> combatData, AgentData agentData)
    {
        return combatData.Where(x => x.SrcMatchesAgent(AttackTarget) && x.IsStateChange == ArcDPSEnums.StateChange.Targetable).Select(x => new TargetableEvent(x, agentData)).ToList();
    }

}
