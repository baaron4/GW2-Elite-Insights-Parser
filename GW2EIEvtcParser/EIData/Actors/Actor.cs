using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EIData;

public abstract class Actor
{
    protected readonly AgentItem AgentItem;
    public string Character { get; protected set; }

    public int UniqueID => AgentItem.UniqueID;
    public uint Toughness => AgentItem.Toughness;
    public uint Condition => AgentItem.Condition;
    public uint Concentration => AgentItem.Concentration;
    public uint Healing => AgentItem.Healing;
    public ParserHelper.Spec Spec => AgentItem.Spec;
    public ParserHelper.Spec BaseSpec => AgentItem.BaseSpec;
    public long LastAware => AgentItem.LastAware;
    public long FirstAware => AgentItem.FirstAware;
    public int ID => AgentItem.ID;
    public uint HitboxHeight => AgentItem.HitboxHeight;
    public uint HitboxWidth => AgentItem.HitboxWidth;
    public bool IsFakeActor => AgentItem.IsFake;
    public ushort InstID => AgentItem.EnglobingAgentItem.InstID;
    // Damage
    protected Dictionary<AgentItem, List<HealthDamageEvent>>? DamageEventByDst;

    protected Dictionary<AgentItem, List<HealthDamageEvent>>? DamageTakenEventsBySrc;
    // Breakbar Damage
    protected Dictionary<AgentItem, List<BreakbarDamageEvent>>? BreakbarDamageEventsByDst;

    protected Dictionary<AgentItem, List<BreakbarDamageEvent>>? BreakbarDamageTakenEventsBySrc;
    // Crowd Control
    protected Dictionary<AgentItem, List<CrowdControlEvent>>? OutgoingCrowdControlEventsByDst;

    protected Dictionary<AgentItem, List<CrowdControlEvent>>? IncomingCrowdControlEventsBySrc;
    // Cast
    protected List<CastEvent>? CastEvents;

    protected Actor(AgentItem agent)
    {
        string[] name = agent.Name.Split('\0');
        Character = name[0];
        AgentItem = agent;
    }
    #region Initializers

    [MemberNotNull(nameof(CastEvents))]
    protected abstract void InitCastEvents(ParsedEvtcLog log);
    [MemberNotNull(nameof(DamageEventByDst))]
    protected abstract void InitDamageEvents(ParsedEvtcLog log);
    [MemberNotNull(nameof(DamageTakenEventsBySrc))]
    protected abstract void InitDamageTakenEvents(ParsedEvtcLog log);
    [MemberNotNull(nameof(BreakbarDamageEventsByDst))]
    protected abstract void InitBreakbarDamageEvents(ParsedEvtcLog log);
    [MemberNotNull(nameof(BreakbarDamageTakenEventsBySrc))]
    protected abstract void InitBreakbarDamageTakenEvents(ParsedEvtcLog log);
    [MemberNotNull(nameof(OutgoingCrowdControlEventsByDst))]
    protected abstract void InitOutgoingCrowdControlEvents(ParsedEvtcLog log);
    [MemberNotNull(nameof(IncomingCrowdControlEventsBySrc))]
    protected abstract void InitIncomingCrowdControlEvents(ParsedEvtcLog log);
    #endregion Initializers
    #region Species
    public bool IsUnamedSpecies()
    {
        return AgentItem.IsUnamedSpecies();
    }

    public bool IsNonIdentifiedSpecies()
    {
        return AgentItem.IsNonIdentifiedSpecies();
    }

    public bool IsSpecies(int id)
    {
        return AgentItem.IsSpecies(id);
    }

    public bool IsSpecies(TargetID id)
    {
        return AgentItem.IsSpecies(id);
    }

    public bool IsSpecies(MinionID id)
    {
        return AgentItem.IsSpecies(id);
    }

    public bool IsSpecies(ChestID id)
    {
        return AgentItem.IsSpecies(id);
    }

    public bool IsAnySpecies(IEnumerable<int> ids)
    {
        return AgentItem.IsAnySpecies(ids);
    }

    public bool IsAnySpecies(IEnumerable<TargetID> ids)
    {
        return AgentItem.IsAnySpecies(ids);
    }

    public bool IsAnySpecies(IEnumerable<MinionID> ids)
    {
        return AgentItem.IsAnySpecies(ids);
    }

    public bool IsAnySpecies(IEnumerable<ChestID> ids)
    {
        return AgentItem.IsAnySpecies(ids);
    }
    #endregion Species
    #region AwareTimes
    public bool InAwareTimes(long time)
    {
        return AgentItem.InAwareTimes(time);
    }
    public bool InAwareTimes(Actor other)
    {
        return AgentItem.InAwareTimes(other);
    }
    public bool InAwareTimes(AgentItem other)
    {
        return AgentItem.InAwareTimes(other);
    }
    #endregion AwareTimes

    #region Damage
    private CachingCollectionWithTarget<List<HealthDamageEvent>>? DamageEventByDstCache;
    public IReadOnlyList<HealthDamageEvent> GetDamageEvents(SingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        InitDamageEvents(log);
        DamageEventByDstCache ??= new(log);
        if (!DamageEventByDstCache.TryGetValue(start, end, target, out var list))
        {
            if (DamageEventByDstCache.TryGetEnglobingValue(start, end, target, out var englobingList))
            {
                list = englobingList.Where(x => x.Time >= start && x.Time <= end).ToList();
            } 
            else
            {
                if (target != null)
                {
                    if (DamageEventByDst.TryGetValue(target.EnglobingAgentItem, out var damageEvents))
                    {
                        long targetStart = target.FirstAware;
                        long targetEnd = target.LastAware;
                        list = damageEvents.Where(x => x.Time >= start && x.Time >= targetStart && x.Time <= end && x.Time <= targetEnd).ToList();
                    }
                    else
                    {
                        list = [];
                    }
                }
                else
                {
                    list = DamageEventByDst[ParserHelper._nullAgent].Where(x => x.Time >= start && x.Time <= end).ToList();
                }
            }
            DamageEventByDstCache.Set(start, end, target, list);
        }
        return list;
    }

    public IReadOnlyList<HealthDamageEvent> GetDamageEvents(SingleActor? target, ParsedEvtcLog log)
    {
        return GetDamageEvents(target, log, log.LogData.LogStart, log.LogData.LogEnd);
    }
    private readonly Dictionary<ParserHelper.DamageType, CachingCollectionWithTarget<List<HealthDamageEvent>>> _typedHitDamageEvents = [];
    public IReadOnlyList<HealthDamageEvent> GetHitDamageEvents(SingleActor? target, ParsedEvtcLog log, long start, long end, ParserHelper.DamageType damageType)
    {
        if (!_typedHitDamageEvents.TryGetValue(damageType, out var hitDamageEventsPerPhasePerTarget))
        {
            hitDamageEventsPerPhasePerTarget = new CachingCollectionWithTarget<List<HealthDamageEvent>>(log);
            _typedHitDamageEvents[damageType] = hitDamageEventsPerPhasePerTarget;
        }

        if (!hitDamageEventsPerPhasePerTarget.TryGetValue(start, end, target, out var dls))
        {
            dls = GetDamageEvents(target, log, start, end).Where(x => x.HasHit).ToList();
            FilterDamageEvents(log, dls, damageType);
            hitDamageEventsPerPhasePerTarget.Set(start, end, target, dls);
        }
        return dls;
    }

    private readonly Dictionary<ParserHelper.DamageType, CachingCollectionWithTarget<List<HealthDamageEvent>>> _typedHitDamageTakenEvents = [];
    public IReadOnlyList<HealthDamageEvent> GetHitDamageTakenEvents(SingleActor? target, ParsedEvtcLog log, long start, long end, ParserHelper.DamageType damageType)
    {
        if (!_typedHitDamageTakenEvents.TryGetValue(damageType, out var hitDamageTakenEventsPerPhasePerTarget))
        {
            hitDamageTakenEventsPerPhasePerTarget = new CachingCollectionWithTarget<List<HealthDamageEvent>>(log);
            _typedHitDamageTakenEvents[damageType] = hitDamageTakenEventsPerPhasePerTarget;
        }
        if (!hitDamageTakenEventsPerPhasePerTarget.TryGetValue(start, end, target, out var dls))
        {
            dls = GetDamageTakenEvents(target, log, start, end).Where(x => x.HasHit).ToList();
            FilterDamageEvents(log, dls, damageType);
            hitDamageTakenEventsPerPhasePerTarget.Set(start, end, target, dls);
        }
        return dls;
    }

    private CachingCollectionWithTarget<List<HealthDamageEvent>>? DamageTakenEventsBySrcCache;
    public IReadOnlyList<HealthDamageEvent> GetDamageTakenEvents(SingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        InitDamageTakenEvents(log);
        DamageTakenEventsBySrcCache ??= new(log);
        if (!DamageTakenEventsBySrcCache.TryGetValue(start, end, target, out var list))
        {
            if (target != null)
            {
                if (DamageTakenEventsBySrc.TryGetValue(target.EnglobingAgentItem, out var damageTakenEvents))
                {
                    long targetStart = target.FirstAware;
                    long targetEnd = target.LastAware;
                    list = damageTakenEvents.Where(x => x.Time >= start && x.Time >= targetStart && x.Time <= end && x.Time <= targetEnd).ToList();
                }
                else
                {
                    list = [];
                }
            }
            else
            {
                list = DamageTakenEventsBySrc[ParserHelper._nullAgent].Where(x => x.Time >= start && x.Time <= end).ToList();
            }
            DamageTakenEventsBySrcCache.Set(start, end, target, list);
        }
        return list;
    }

    public IReadOnlyList<HealthDamageEvent> GetDamageTakenEvents(SingleActor? target, ParsedEvtcLog log)
    {
        return GetDamageTakenEvents(target, log, log.LogData.LogStart, log.LogData.LogEnd);
    }
    #endregion Damage
    #region BreakbarDamage

    private CachingCollectionWithTarget<List<BreakbarDamageEvent>>? BreakbarDamageEventByDstCache;
    public IReadOnlyList<BreakbarDamageEvent> GetBreakbarDamageEvents(SingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        InitBreakbarDamageEvents(log);
        BreakbarDamageEventByDstCache ??= new(log);
        if (!BreakbarDamageEventByDstCache.TryGetValue(start, end, target, out var list))
        {
            if (target != null)
            {
                if (BreakbarDamageEventsByDst.TryGetValue(target.EnglobingAgentItem, out var breakbarDamageEvents))
                {
                    long targetStart = target.FirstAware;
                    long targetEnd = target.LastAware;
                    list = breakbarDamageEvents.Where(x => x.Time >= start && x.Time >= targetStart && x.Time <= end && x.Time <= targetEnd).ToList();
                }
                else
                {
                    list = [];
                }
            }
            else
            {
                list = BreakbarDamageEventsByDst[ParserHelper._nullAgent].Where(x => x.Time >= start && x.Time <= end).ToList();
            }
            BreakbarDamageEventByDstCache.Set(start, end, target, list);
        }
        return list;
    }
    public IReadOnlyList<BreakbarDamageEvent> GetBreakbarDamageEvents(SingleActor? target, ParsedEvtcLog log)
    {
        return GetBreakbarDamageEvents(target, log, log.LogData.LogStart, log.LogData.LogEnd);
    }

    private CachingCollectionWithTarget<List<BreakbarDamageEvent>>? BreakbarDamageTakenEventBySrcCache;
    public IReadOnlyList<BreakbarDamageEvent> GetBreakbarDamageTakenEvents(SingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        InitBreakbarDamageTakenEvents(log);
        BreakbarDamageTakenEventBySrcCache ??= new(log);
        if (!BreakbarDamageTakenEventBySrcCache.TryGetValue(start, end, target, out var list))
        {
            if (target != null)
            {
                if (BreakbarDamageTakenEventsBySrc.TryGetValue(target.EnglobingAgentItem, out var breakbarDamageEvents))
                {
                    long targetStart = target.FirstAware;
                    long targetEnd = target.LastAware;
                    list = breakbarDamageEvents.Where(x => x.Time >= start && x.Time >= targetStart && x.Time <= end && x.Time <= targetEnd).ToList();
                }
                else
                {
                    list = [];
                }
            }
            else
            {
                list = BreakbarDamageTakenEventsBySrc[ParserHelper._nullAgent].Where(x => x.Time >= start && x.Time <= end).ToList();
            }
            BreakbarDamageTakenEventBySrcCache.Set(start, end, target, list);
        }
        return list;
    }
    public IReadOnlyList<BreakbarDamageEvent> GetBreakbarDamageTakenEvents(SingleActor? target, ParsedEvtcLog log)
    {
        return GetBreakbarDamageTakenEvents(target, log, log.LogData.LogStart, log.LogData.LogEnd);
    }
    #endregion BreakbarDamage
    #region CrowdControl

    private CachingCollectionWithTarget<List<CrowdControlEvent>>? OutgoingCrowdControlEventByDstCache;
    public IReadOnlyList<CrowdControlEvent> GetOutgoingCrowdControlEvents(SingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        InitOutgoingCrowdControlEvents(log);
        OutgoingCrowdControlEventByDstCache ??= new(log);
        if (!OutgoingCrowdControlEventByDstCache.TryGetValue(start, end, target, out var list))
        {
            if (target != null)
            {
                if (OutgoingCrowdControlEventsByDst.TryGetValue(target.EnglobingAgentItem, out var ccEvents))
                {
                    long targetStart = target.FirstAware;
                    long targetEnd = target.LastAware;
                    list = ccEvents.Where(x => x.Time >= start && x.Time >= targetStart && x.Time <= end && x.Time <= targetEnd).ToList();
                }
                else
                {
                    list = [];
                }
            }
            else
            {
                list = OutgoingCrowdControlEventsByDst[ParserHelper._nullAgent].Where(x => x.Time >= start && x.Time <= end).ToList();
            }
            OutgoingCrowdControlEventByDstCache.Set(start, end, target, list);
        }
        return list;
    }
    public IReadOnlyList<CrowdControlEvent> GetOutgoingCrowdControlEvents(SingleActor? target, ParsedEvtcLog log)
    {
        return GetOutgoingCrowdControlEvents(target, log, log.LogData.LogStart, log.LogData.LogEnd);
    }

    private CachingCollectionWithTarget<List<CrowdControlEvent>>? IncomingCrowdControlEventBySrcCache;
    public IReadOnlyList<CrowdControlEvent> GetIncomingCrowdControlEvents(SingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        InitIncomingCrowdControlEvents(log);
        IncomingCrowdControlEventBySrcCache ??= new(log);
        if (!IncomingCrowdControlEventBySrcCache.TryGetValue(start, end, target, out var list))
        {
            if (target != null)
            {
                if (IncomingCrowdControlEventsBySrc.TryGetValue(target.EnglobingAgentItem, out var ccEvents))
                {
                    long targetStart = target.FirstAware;
                    long targetEnd = target.LastAware;
                    list = ccEvents.Where(x => x.Time >= start && x.Time >= targetStart && x.Time <= end && x.Time <= targetEnd).ToList();
                }
                else
                {
                    list = [];
                }
            }
            else
            {
                list = IncomingCrowdControlEventsBySrc[ParserHelper._nullAgent].Where(x => x.Time >= start && x.Time <= end).ToList();
            }
            IncomingCrowdControlEventBySrcCache.Set(start, end, target, list);
        }
        return list;
    }
    public IReadOnlyList<CrowdControlEvent> GetIncomingCrowdControlEvents(SingleActor? target, ParsedEvtcLog log)
    {
        return GetIncomingCrowdControlEvents(target, log, log.LogData.LogStart, log.LogData.LogEnd);
    }
    #endregion CrowdControl
    #region Cast
    private CachingCollection<List<CastEvent>>? _castEventsCache;
    public IReadOnlyList<CastEvent> GetCastEvents(ParsedEvtcLog log, long start, long end)
    {
        if (CastEvents == null)
        {
            InitCastEvents(log);
        }
        _castEventsCache ??= new CachingCollection<List<CastEvent>>(log);
        if (!_castEventsCache.TryGetValue(start, end, out var list))
        {
            list = CastEvents.Where(x => x.Time >= start && x.Time <= end).ToList();
            _castEventsCache.Set(start, end, list);
        }
        return list;

    }
    public IReadOnlyList<CastEvent> GetCastEvents(ParsedEvtcLog log)
    {
        return GetCastEvents(log, log.LogData.LogStart, log.LogData.LogEnd);
    }

    private CachingCollection<List<CastEvent>>? _intersectingCastEventsCache;
    public IReadOnlyList<CastEvent> GetIntersectingCastEvents(ParsedEvtcLog log, long start, long end)
    {
        if (CastEvents == null)
        {
            InitCastEvents(log);
        }
        _intersectingCastEventsCache ??= new CachingCollection<List<CastEvent>>(log);
        if (!_intersectingCastEventsCache.TryGetValue(start, end, out var list))
        {
            list = CastEvents.Where(x => KeepIntersectingCastLog(x, start, end)).ToList();
            _intersectingCastEventsCache.Set(start, end, list);
        }
        return list;

    }
    public IReadOnlyList<CastEvent> GetIntersectingCastEvents(ParsedEvtcLog log)
    {
        return GetIntersectingCastEvents(log, log.LogData.LogStart, log.LogData.LogEnd);
    }
    #endregion Cast
    protected static bool KeepIntersectingCastLog(CastEvent evt, long start, long end)
    {
        return (evt.Time >= start && evt.Time <= end) || // start inside
            (evt.EndTime >= start && evt.EndTime <= end) || // end inside
            (evt.Time <= start && evt.EndTime >= end); // start before, end after
    }

    private static void FilterDamageEvents(ParsedEvtcLog log, List<HealthDamageEvent> dls, ParserHelper.DamageType damageType)
    {
        switch (damageType)
        {
            case ParserHelper.DamageType.Power:
                dls.RemoveAll(x => x.ConditionDamageBased(log));
                break;
            case ParserHelper.DamageType.Strike:
                dls.RemoveAll(x => x is NonDirectHealthDamageEvent);
                break;
            case ParserHelper.DamageType.LifeLeech:
                dls.RemoveAll(x => !x.IsLifeLeech);
                break;
            case ParserHelper.DamageType.Condition:
                dls.RemoveAll(x => !x.ConditionDamageBased(log));
                break;
            case ParserHelper.DamageType.StrikeAndCondition:
                dls.RemoveAll(x => x is NonDirectHealthDamageEvent && !x.ConditionDamageBased(log));
                break;
            case ParserHelper.DamageType.ConditionAndLifeLeech:
                dls.RemoveAll(x => !x.ConditionDamageBased(log) && !x.IsLifeLeech);
                break;
            case ParserHelper.DamageType.StrikeAndLifeLeech:
                dls.RemoveAll(x => x is NonDirectHealthDamageEvent && !x.IsLifeLeech);
                break;
            case ParserHelper.DamageType.StrikeAndConditionAndLifeLeech:
                dls.RemoveAll(x => x is NonDirectHealthDamageEvent && !x.ConditionDamageBased(log) && !x.IsLifeLeech);
                break;
            case ParserHelper.DamageType.All:
                break;
            default:
                throw new NotImplementedException("Not implemented damage type " + damageType);
        }
    }
}

public static partial class ListExt
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SortByFirstAware<T>(this List<T> list)  where T : Actor
    {
        list.AsSpan().SortStable((a, b) => a.FirstAware.CompareTo(b.FirstAware));
    }
}
