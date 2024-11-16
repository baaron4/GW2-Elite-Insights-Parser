using GW2EIEvtcParser.ParsedData;

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
    // Damage
    protected List<HealthDamageEvent>? DamageEvents;
    protected Dictionary<AgentItem, List<HealthDamageEvent>>? DamageEventByDst;
    private readonly Dictionary<ParserHelper.DamageType, CachingCollectionWithTarget<List<HealthDamageEvent>>> _typedHitDamageEvents = [];
    private readonly Dictionary<ParserHelper.DamageType, CachingCollectionWithTarget<List<HealthDamageEvent>>> _typedHitDamageTakenEvents = [];
    protected List<HealthDamageEvent>? DamageTakenEvents;
    protected Dictionary<AgentItem, List<HealthDamageEvent>>? DamageTakenEventsBySrc;
    // Breakbar Damage
    protected List<BreakbarDamageEvent>? BreakbarDamageEvents;
    protected Dictionary<AgentItem, List<BreakbarDamageEvent>>? BreakbarDamageEventsByDst;
    protected List<BreakbarDamageEvent>? BreakbarDamageTakenEvents;
    protected Dictionary<AgentItem, List<BreakbarDamageEvent>>? BreakbarDamageTakenEventsBySrc;
    // Crowd Control
    protected List<CrowdControlEvent>? OutgoingCrowdControlEvents;
    protected Dictionary<AgentItem, List<CrowdControlEvent>>? OutgoingCrowdControlEventsByDst;
    protected List<CrowdControlEvent>? IncomingCrowdControlEvents;
    protected Dictionary<AgentItem, List<CrowdControlEvent>>? IncomingCrowdControlEventsBySrc;
    // Cast
    protected List<CastEvent>? CastEvents;

    protected Actor(AgentItem agent)
    {
        string[] name = agent.Name.Split('\0');
        Character = name[0];
        AgentItem = agent;
    }

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

    public bool IsSpecies(ArcDPSEnums.TrashID id)
    {
        return AgentItem.IsSpecies(id);
    }

    public bool IsSpecies(ArcDPSEnums.TargetID id)
    {
        return AgentItem.IsSpecies(id);
    }

    public bool IsSpecies(ArcDPSEnums.MinionID id)
    {
        return AgentItem.IsSpecies(id);
    }

    public bool IsSpecies(ArcDPSEnums.ChestID id)
    {
        return AgentItem.IsSpecies(id);
    }

    public bool IsAnySpecies(IEnumerable<int> ids)
    {
        return AgentItem.IsAnySpecies(ids);
    }

    public bool IsAnySpecies(IEnumerable<ArcDPSEnums.TrashID> ids)
    {
        return AgentItem.IsAnySpecies(ids);
    }

    public bool IsAnySpecies(IEnumerable<ArcDPSEnums.TargetID> ids)
    {
        return AgentItem.IsAnySpecies(ids);
    }

    public bool IsAnySpecies(IEnumerable<ArcDPSEnums.MinionID> ids)
    {
        return AgentItem.IsAnySpecies(ids);
    }

    public bool IsAnySpecies(IEnumerable<ArcDPSEnums.ChestID> ids)
    {
        return AgentItem.IsAnySpecies(ids);
    }

    // Getters
    // Damage logs
    public abstract IEnumerable<HealthDamageEvent> GetDamageEvents(SingleActor? target, ParsedEvtcLog log, long start, long end);

    public abstract IEnumerable<BreakbarDamageEvent> GetBreakbarDamageEvents(SingleActor target, ParsedEvtcLog log, long start, long end);
    public abstract IEnumerable<CrowdControlEvent> GetOutgoingCrowdControlEvents(SingleActor target, ParsedEvtcLog log, long start, long end);

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
                dls.RemoveAll(x => x is NonDirectHealthDamageEvent ndhd && !ndhd.IsLifeLeech);
                break;
            case ParserHelper.DamageType.Condition:
                dls.RemoveAll(x => !x.ConditionDamageBased(log));
                break;
            case ParserHelper.DamageType.StrikeAndCondition:
                dls.RemoveAll(x => x is NonDirectHealthDamageEvent && !x.ConditionDamageBased(log));
                break;
            case ParserHelper.DamageType.StrikeAndConditionAndLifeLeech:
                dls.RemoveAll(x => x is NonDirectHealthDamageEvent ndhd && !x.ConditionDamageBased(log) && !ndhd.IsLifeLeech);
                break;
            case ParserHelper.DamageType.All:
                break;
            default:
                throw new NotImplementedException("Not implemented damage type " + damageType);
        }
    }
    public IEnumerable<HealthDamageEvent> GetHitDamageEvents(SingleActor? target, ParsedEvtcLog log, long start, long end, ParserHelper.DamageType damageType)
    {
        if (!_typedHitDamageEvents.TryGetValue(damageType, out CachingCollectionWithTarget<List<HealthDamageEvent>> hitDamageEventsPerPhasePerTarget))
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

    public IEnumerable<HealthDamageEvent> GetHitDamageTakenEvents(SingleActor? target, ParsedEvtcLog log, long start, long end, ParserHelper.DamageType damageType)
    {
        if (!_typedHitDamageTakenEvents.TryGetValue(damageType, out CachingCollectionWithTarget<List<HealthDamageEvent>> hitDamageTakenEventsPerPhasePerTarget))
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

    public abstract IEnumerable<HealthDamageEvent> GetDamageTakenEvents(SingleActor? target, ParsedEvtcLog log, long start, long end);

    public abstract IEnumerable<BreakbarDamageEvent> GetBreakbarDamageTakenEvents(SingleActor target, ParsedEvtcLog log, long start, long end);
    public abstract IEnumerable<CrowdControlEvent> GetIncomingCrowdControlEvents(SingleActor target, ParsedEvtcLog log, long start, long end);

    // Cast logs
    public abstract IEnumerable<CastEvent> GetCastEvents(ParsedEvtcLog log, long start, long end);
    public abstract IEnumerable<CastEvent> GetIntersectingCastEvents(ParsedEvtcLog log, long start, long end);
    // privates

    protected static bool KeepIntersectingCastLog(CastEvent evt, long start, long end)
    {
        return (evt.Time >= start && evt.Time <= end) || // start inside
            (evt.EndTime >= start && evt.EndTime <= end) || // end inside
            (evt.Time <= start && evt.EndTime >= end); // start before, end after
    }
}

public static partial class ListExt
{
    public static void SortByFirstAware<T>(this List<T> list)  where T : Actor
    {
        StableSort<T>.fluxsort(list.AsSpan(), (a, b) => a.FirstAware.CompareTo(b.FirstAware));
    }
}
