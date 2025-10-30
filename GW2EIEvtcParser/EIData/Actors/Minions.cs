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
        if (DamageEventByDst == null)
        {
            var damageEvents = new List<HealthDamageEvent>(_minionList.Count); //TODO_PERF(Rennorb) @find average complexity
            foreach (NPC minion in _minionList)
            {
                damageEvents.AddRange(minion.GetDamageEvents(null, log, Master.FirstAware, Master.LastAware));
            }
            damageEvents.SortByTime();
            DamageEventByDst = damageEvents.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            DamageEventByDst[ParserHelper._nullAgent] = damageEvents;
        }
    }
    protected override void InitDamageTakenEvents(ParsedEvtcLog log)
    {
        if (DamageTakenEventsBySrc == null)
        {
            var damageTakenEvents = new List<HealthDamageEvent>(_minionList.Count); //TODO_PERF(Rennorb) @find average complexity
            foreach (NPC minion in _minionList)
            {
                damageTakenEvents.AddRange(minion.GetDamageTakenEvents(null, log, Master.FirstAware, Master.LastAware));
            }
            damageTakenEvents.SortByTime();
            DamageTakenEventsBySrc = damageTakenEvents.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
            DamageTakenEventsBySrc[ParserHelper._nullAgent] = damageTakenEvents;
        }
    }
    #endregion DAMAGE

    #region BREAKBAR DAMAGE

    protected override void InitBreakbarDamageEvents(ParsedEvtcLog log)
    {
        if (BreakbarDamageEventsByDst == null)
        {
            var breakbarDamageEvents = new List<BreakbarDamageEvent>(_minionList.Count); //TODO_PERF(Rennorb) @find average complexity
            foreach (NPC minion in _minionList)
            {
                breakbarDamageEvents.AddRange(minion.GetBreakbarDamageEvents(null, log, Master.FirstAware, Master.LastAware));
            }
            breakbarDamageEvents.SortByTime();
            BreakbarDamageEventsByDst = breakbarDamageEvents.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            BreakbarDamageEventsByDst[ParserHelper._nullAgent] = breakbarDamageEvents;
        }
    }

    protected override void InitBreakbarDamageTakenEvents(ParsedEvtcLog log)
    {
        if (BreakbarDamageTakenEventsBySrc == null)
        {
            var breakbarDamageTakenEvents = new List<BreakbarDamageEvent>(_minionList.Count); //TODO_PERF(Rennorb) @find average complexity
            foreach (NPC minion in _minionList)
            {
                breakbarDamageTakenEvents.AddRange(minion.GetBreakbarDamageTakenEvents(null, log, Master.FirstAware, Master.LastAware));
            }
            breakbarDamageTakenEvents.SortByTime();
            BreakbarDamageTakenEventsBySrc = breakbarDamageTakenEvents.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
            BreakbarDamageTakenEventsBySrc[ParserHelper._nullAgent] = breakbarDamageTakenEvents;
        }
    }
    #endregion BREAKBAR DAMAGE


    #region CROWD CONTROL

    protected override void InitOutgoingCrowdControlEvents(ParsedEvtcLog log)
    {
        if (OutgoingCrowdControlEventsByDst == null)
        {
            var outgoingCrowdControlEvents = new List<CrowdControlEvent>(_minionList.Count); //TODO_PERF(Rennorb) @find average complexity
            foreach (NPC minion in _minionList)
            {
                outgoingCrowdControlEvents.AddRange(minion.GetOutgoingCrowdControlEvents(null, log, Master.FirstAware, Master.LastAware));
            }
            outgoingCrowdControlEvents.SortByTime();
            OutgoingCrowdControlEventsByDst = outgoingCrowdControlEvents.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            OutgoingCrowdControlEventsByDst[ParserHelper._nullAgent] = outgoingCrowdControlEvents;
        }
    }
    protected override void InitIncomingCrowdControlEvents(ParsedEvtcLog log)
    {
        if (IncomingCrowdControlEventsBySrc == null)
        {
            var incomingCrowdControlEvents = new List<CrowdControlEvent>(_minionList.Count); //TODO_PERF(Rennorb) @find average complexity
            foreach (NPC minion in _minionList)
            {
                incomingCrowdControlEvents.AddRange(minion.GetIncomingCrowdControlEvents(null, log, Master.FirstAware, Master.LastAware));
            }
            incomingCrowdControlEvents.SortByTime();
            IncomingCrowdControlEventsBySrc = incomingCrowdControlEvents.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
            IncomingCrowdControlEventsBySrc[ParserHelper._nullAgent] = incomingCrowdControlEvents;
        }
    }
    #endregion CROWD CONTROL

    #region CAST
    protected override void InitCastEvents(ParsedEvtcLog log)
    {
        CastEvents = new List<CastEvent>(_minionList.Count); //TODO_PERF(Rennorb) @find average complexity
        foreach (NPC minion in _minionList)
        {
            CastEvents.AddRange(minion.GetCastEvents(log, Master.FirstAware, Master.LastAware));
        }
        CastEvents.SortByTimeThenSwap();
    }

    private CachingCollection<long>? _intersectingCastTimeCache;
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
