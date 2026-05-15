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
        var events = combatData.Where(x => x.SrcMatchesAgent(AttackTarget) && x.IsStateChange == ArcDPSEnums.StateChange.Targetable).ToList();
        List<TargetableEvent> tarEvts = new(events.Count);
        foreach (var e in events)
        {
            var tarEvt = new TargetableEvent(e, agentData);
            if (tarEvts.Count > 0 && tarEvts[^1].Targetable == tarEvt.Targetable)
            {
                continue;
            }
            tarEvts.Add(tarEvt);
        }
        return tarEvts;
    }

}
