using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

public class Minions : Actor
{
    private readonly List<NPC> _minionList;
    public AgentItem ReferenceAgentItem => AgentItem;

    public IReadOnlyList<NPC> MinionList => _minionList;
    public readonly SingleActor Master;
    public readonly EXTMinionsHealingHelper EXTHealing;
    public readonly EXTMinionsBarrierHelper EXTBarrier;

    internal Minions(SingleActor master, NPC firstMinion) : base(firstMinion.AgentItem)
    {
        _minionList = [firstMinion];
        Master = master;
        EXTHealing = new EXTMinionsHealingHelper(this);
        EXTBarrier = new EXTMinionsBarrierHelper(this);
        Character = firstMinion.Character;
#if DEBUG
        Character += " (" + ID + ")";
#endif
    }

    internal void AddMinion(NPC minion)
    {
        _minionList.Add(minion);
    }

    #region DAMAGE

    protected override void InitDamageEvents(ParsedEvtcLog log)
    {
        if (DamageEvents == null || DamageEventByDst == null)
        {
            DamageEvents = new List<HealthDamageEvent>(_minionList.Count); //TODO(Rennorb) @perf: find average complexity
            foreach (NPC minion in _minionList)
            {
                DamageEvents.AddRange(minion.GetDamageEvents(null, log, Master.FirstAware, Master.LastAware));
            }
            DamageEvents.SortByTime();
            DamageEventByDst = DamageEvents.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
        }
    }
    protected override void InitDamageTakenEvents(ParsedEvtcLog log)
    {
        if (DamageTakenEvents == null || DamageTakenEventsBySrc == null)
        {
            DamageTakenEvents = new List<HealthDamageEvent>(_minionList.Count); //TODO(Rennorb) @perf: find average complexity
            foreach (NPC minion in _minionList)
            {
                DamageTakenEvents.AddRange(minion.GetDamageTakenEvents(null, log, Master.FirstAware, Master.LastAware));
            }
            DamageTakenEvents.SortByTime();
            DamageTakenEventsBySrc = DamageTakenEvents.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
        }
    }
    #endregion DAMAGE

    #region BREAKBAR DAMAGE

#pragma warning disable CS8774 // must have non null value when exiting
    protected override void InitBreakbarDamageEvents(ParsedEvtcLog log)
    {
        if (BreakbarDamageEvents == null)
        {
            BreakbarDamageEvents = new List<BreakbarDamageEvent>(_minionList.Count); //TODO(Rennorb) @perf: find average complexity
            foreach (NPC minion in _minionList)
            {
                BreakbarDamageEvents.AddRange(minion.GetBreakbarDamageEvents(null, log, Master.FirstAware, Master.LastAware));
            }
            BreakbarDamageEvents.SortByTime();
            BreakbarDamageEventsByDst = BreakbarDamageEvents.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
        }
    }
#pragma warning restore CS8774 // must have non null value when exiting
    public override IEnumerable<BreakbarDamageEvent> GetBreakbarDamageEvents(SingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        InitBreakbarDamageEvents(log);

        if (target != null)
        {
            if (BreakbarDamageEventsByDst.TryGetValue(target.EnglobingAgentItem, out var list))
            {
                long targetStart = target.FirstAware;
                long targetEnd = target.LastAware;
                return list.Where(x => x.Time >= start && x.Time >= targetStart && x.Time <= end && x.Time <= targetEnd);
            }
            else
            {
                return [ ];
            }
        }

        return BreakbarDamageEvents.Where(x => x.Time >= start && x.Time <= end);
    }

#pragma warning disable CS8774 // must have non null value when exiting
    protected override void InitBreakbarDamageTakenEvents(ParsedEvtcLog log)
    {
        if (BreakbarDamageTakenEvents == null)
        {
            BreakbarDamageTakenEvents = new List<BreakbarDamageEvent>(_minionList.Count); //TODO(Rennorb) @perf: find average complexity
            foreach (NPC minion in _minionList)
            {
                BreakbarDamageTakenEvents.AddRange(minion.GetBreakbarDamageTakenEvents(null, log, Master.FirstAware, Master.LastAware));
            }
            BreakbarDamageTakenEvents.SortByTime();
            BreakbarDamageTakenEventsBySrc = BreakbarDamageTakenEvents.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
        }
    }
#pragma warning restore CS8774 // must have non null value when exiting
    public override IEnumerable<BreakbarDamageEvent> GetBreakbarDamageTakenEvents(SingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        InitBreakbarDamageTakenEvents(log);

        if (target != null)
        {
            if (BreakbarDamageTakenEventsBySrc.TryGetValue(target.EnglobingAgentItem, out var list))
            {
                long targetStart = target.FirstAware;
                long targetEnd = target.LastAware;
                return list.Where(x => x.Time >= start && x.Time >= targetStart && x.Time <= end && x.Time <= targetEnd);
            }
            else
            {
                return [ ];
            }
        }

        return BreakbarDamageTakenEvents.Where(x => x.Time >= start && x.Time <= end);
    }
    #endregion BREAKBAR DAMAGE


    #region CROWD CONTROL

#pragma warning disable CS8774 // must have non null value when exiting
    protected override void InitOutgoingCrowdControlEvents(ParsedEvtcLog log)
    {
        if (OutgoingCrowdControlEvents == null)
        {
            OutgoingCrowdControlEvents = new List<CrowdControlEvent>(_minionList.Count); //TODO(Rennorb) @perf: find average complexity
            foreach (NPC minion in _minionList)
            {
                OutgoingCrowdControlEvents.AddRange(minion.GetOutgoingCrowdControlEvents(null, log, Master.FirstAware, Master.LastAware));
            }
            OutgoingCrowdControlEvents.SortByTime();
            OutgoingCrowdControlEventsByDst = OutgoingCrowdControlEvents.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
        }
    }
#pragma warning restore CS8774 // must have non null value when exiting
    public override IEnumerable<CrowdControlEvent> GetOutgoingCrowdControlEvents(SingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        InitOutgoingCrowdControlEvents(log);

        if (target != null)
        {
            if (OutgoingCrowdControlEventsByDst.TryGetValue(target.EnglobingAgentItem, out var list))
            {
                long targetStart = target.FirstAware;
                long targetEnd = target.LastAware;
                return list.Where(x => x.Time >= start && x.Time >= targetStart && x.Time <= end && x.Time <= targetEnd);
            }
            else
            {
                return [ ];
            }
        }

        return OutgoingCrowdControlEvents.Where(x => x.Time >= start && x.Time <= end);
    }

#pragma warning disable CS8774 // must have non null value when exiting
    protected override void InitIncomingCrowdControlEvents(ParsedEvtcLog log)
    {
        if (IncomingCrowdControlEvents == null)
        {
            IncomingCrowdControlEvents = new List<CrowdControlEvent>(_minionList.Count); //TODO(Rennorb) @perf: find average complexity
            foreach (NPC minion in _minionList)
            {
                IncomingCrowdControlEvents.AddRange(minion.GetIncomingCrowdControlEvents(null, log, Master.FirstAware, Master.LastAware));
            }
            IncomingCrowdControlEvents.SortByTime();
            IncomingCrowdControlEventsBySrc = IncomingCrowdControlEvents.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
        }
    }
#pragma warning restore CS8774 // must have non null value when exiting
    public override IEnumerable<CrowdControlEvent> GetIncomingCrowdControlEvents(SingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        InitIncomingCrowdControlEvents(log);

        if (target != null)
        {
            if (IncomingCrowdControlEventsBySrc.TryGetValue(target.EnglobingAgentItem, out var list))
            {
                long targetStart = target.FirstAware;
                long targetEnd = target.LastAware;
                return list.Where(x => x.Time >= start && x.Time >= targetStart && x.Time <= end && x.Time <= targetEnd);
            }
            else
            {
                return [ ];
            }
        }

        return IncomingCrowdControlEvents.Where(x => x.Time >= start && x.Time <= end);
    }
    #endregion CROWD CONTROL

    #region CAST
    protected override void InitCastEvents(ParsedEvtcLog log)
    {
        CastEvents = new List<CastEvent>(_minionList.Count); //TODO(Rennorb) @perf: find average complexity
        foreach (NPC minion in _minionList)
        {
            CastEvents.AddRange(minion.GetCastEvents(log, Master.FirstAware, Master.LastAware));
        }
        CastEvents.SortByTimeThenSwap();
    }

    private CachingCollection<long> _intersectingCastTimeCache;
    public long GetIntersectingCastTime(ParsedEvtcLog log, long start, long end)
    {
        _intersectingCastTimeCache ??= new(log);
        if (!_intersectingCastTimeCache.TryGetValue(start, end, out var time))
        {
            time = GetIntersectingCastEvents(log).Sum(cl => Math.Min(cl.EndTime, end) - Math.Max(cl.Time, start));
            _intersectingCastTimeCache.Set(start, end, time);
        }
        return time;
    }
    #endregion CAST

    internal bool IsActive(ParsedEvtcLog log)
    {
        if (ParserHelper.IsKnownMinionID(ReferenceAgentItem, Master.Spec))
        {
            return true;
        }
        if (log.CombatData.HasEXTHealing && EXTHealing.GetOutgoingHealEvents(null, log).Any())
        {
            return true;
        }
        if (log.CombatData.HasEXTBarrier && EXTBarrier.GetOutgoingBarrierEvents(null, log).Any())
        {
            return true;
        }
        if (GetDamageEvents(null, log).Any())
        {
            return true;
        }
        if (GetCastEvents(log).Any(x => x.SkillID != SkillIDs.WeaponStow && x.SkillID != SkillIDs.WeaponDraw && x.SkillID != SkillIDs.WeaponSwap && x.SkillID != SkillIDs.MirageCloakDodge))
        {
            return true;
        }
        /*if (_minionList[0] is NPC && _minionList.Any(x => log.CombatData.GetMovementData(x.AgentItem).Any()))
        {
            return true;
        }*/
        return false;
    }

    internal IReadOnlyList<IReadOnlyList<Segment>> GetLifeSpanSegments(ParsedEvtcLog log)
    {
        var minionsSegments = new List<List<Segment>>();
        long logEnd = log.LogData.LogEnd;
        foreach (NPC minion in _minionList)
        {
            var minionSegments = new List<Segment>();
            long start = Math.Max(Math.Max(minion.FirstAware, Master.FirstAware), 0);
            // Find end
            long end = Math.Min(minion.LastAware, Master.LastAware);
            DeadEvent? dead = log.CombatData.GetDeadEvents(minion.AgentItem).LastOrDefault();
            if (dead != null)
            {
                end = Math.Min(dead.Time, end);
            }
            DespawnEvent? despawn = log.CombatData.GetDespawnEvents(minion.AgentItem).LastOrDefault();
            if (despawn != null)
            {
                end = Math.Min(despawn.Time, end);
            }
            //
            end = Math.Min(end, logEnd);
            minionSegments.Add(new Segment(log.LogData.LogStart, start, 0));
            minionSegments.Add(new Segment(start, end, 1));
            minionSegments.Add(new Segment(end, logEnd, 0));
            minionSegments.RemoveAll(x => x.Start > x.End);
            minionsSegments.Add(minionSegments);
        }
        return minionsSegments;
    }
}
