using System.Diagnostics.CodeAnalysis;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.Extensions.HealingStatsExtensionHandler;

namespace GW2EIEvtcParser.Extensions;

public abstract class EXTActorHealingHelper
{
    protected Dictionary<AgentItem, List<EXTHealingEvent>>? HealEventsByDst;
    protected Dictionary<AgentItem, List<EXTHealingEvent>>? HealReceivedEventsBySrc;

    //TODO_PERF(Rennorb)
    private readonly Dictionary<EXTHealingType, CachingCollectionWithTarget<List<EXTHealingEvent>>> _typedHealEvents = [];
    private readonly Dictionary<EXTHealingType, CachingCollectionWithTarget<List<EXTHealingEvent>>> _typedIncomingHealEvents = [];

    internal EXTActorHealingHelper()
    {
    }

    #region INITIALIZERS
    [MemberNotNull(nameof(HealEventsByDst))]
    protected abstract void InitHealEvents(ParsedEvtcLog log);

    [MemberNotNull(nameof(HealReceivedEventsBySrc))]
    protected abstract void InitIncomingHealEvents(ParsedEvtcLog log);
    #endregion INITIALIZERS

    private CachingCollectionWithTarget<List<EXTHealingEvent>>? HealEventByDstCache;
    public IReadOnlyList<EXTHealingEvent> GetOutgoingHealEvents(SingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        InitHealEvents(log);
        HealEventByDstCache ??= new(log);
        if (!HealEventByDstCache.TryGetValue(start, end, target, out var list))
        {
            if (target != null)
            {
                if (HealEventsByDst.TryGetValue(target.EnglobingAgentItem, out var damageEvents))
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
                list = HealEventsByDst[ParserHelper._nullAgent].Where(x => x.Time >= start && x.Time <= end).ToList();
            }
            HealEventByDstCache.Set(start, end, target, list);
        }
        return list;
    }
    public IReadOnlyList<EXTHealingEvent> GetOutgoingHealEvents(SingleActor? target, ParsedEvtcLog log)
    {
        return GetOutgoingHealEvents(target, log, log.LogData.LogStart, log.LogData.LogEnd);
    }

    private CachingCollectionWithTarget<List<EXTHealingEvent>>? HealReceivedEventBySrcCache;
    public IReadOnlyList<EXTHealingEvent> GetIncomingHealEvents(SingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        InitIncomingHealEvents(log);
        HealReceivedEventBySrcCache ??= new(log);
        if (!HealReceivedEventBySrcCache.TryGetValue(start, end, target, out var list))
        {
            if (target != null)
            {
                if (HealReceivedEventsBySrc.TryGetValue(target.EnglobingAgentItem, out var damageEvents))
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
                list = HealReceivedEventsBySrc[ParserHelper._nullAgent].Where(x => x.Time >= start && x.Time <= end).ToList();
            }
            HealReceivedEventBySrcCache.Set(start, end, target, list);
        }
        return list;
    }
    public IReadOnlyList<EXTHealingEvent> GetIncomingHealEvents(SingleActor? target, ParsedEvtcLog log)
    {
        return GetIncomingHealEvents(target, log, log.LogData.LogStart, log.LogData.LogEnd);
    }

    private static void FilterHealEvents(ParsedEvtcLog log, List<EXTHealingEvent> dls, EXTHealingType healingType)
    {
        switch (healingType)
        {
            case EXTHealingType.HealingPower:
                dls.RemoveAll(x => x.GetHealingType(log) != EXTHealingType.HealingPower);
                break;
            case EXTHealingType.ConversionBased:
                dls.RemoveAll(x => x.GetHealingType(log) != EXTHealingType.ConversionBased);
                break;
            case EXTHealingType.Hybrid:
                dls.RemoveAll(x => x.GetHealingType(log) != EXTHealingType.Hybrid);
                break;
            case EXTHealingType.All:
                break;
            default:
                throw new NotImplementedException("Not implemented healing type " + healingType);
        }
    }

    public IReadOnlyList<EXTHealingEvent> GetTypedOutgoingHealEvents(SingleActor? target, ParsedEvtcLog log, long start, long end, EXTHealingType healingType)
    {
        if (!_typedHealEvents.TryGetValue(healingType, out var healEventsPerPhasePerTarget))
        {
            healEventsPerPhasePerTarget = new CachingCollectionWithTarget<List<EXTHealingEvent>>(log);
            _typedHealEvents[healingType] = healEventsPerPhasePerTarget;
        }
        if (!healEventsPerPhasePerTarget.TryGetValue(start, end, target, out var dls))
        {
            dls = GetOutgoingHealEvents(target, log, start, end).ToList();
            FilterHealEvents(log, dls, healingType);
            healEventsPerPhasePerTarget.Set(start, end, target, dls);
        }
        return dls;
    }

    public IReadOnlyList<EXTHealingEvent> GetTypedIncomingHealEvents(SingleActor? target, ParsedEvtcLog log, long start, long end, EXTHealingType healingType)
    {
        if (!_typedIncomingHealEvents.TryGetValue(healingType, out var healEventsPerPhasePerTarget))
        {
            healEventsPerPhasePerTarget = new CachingCollectionWithTarget<List<EXTHealingEvent>>(log);
            _typedIncomingHealEvents[healingType] = healEventsPerPhasePerTarget;
        }
        if (!healEventsPerPhasePerTarget.TryGetValue(start, end, target, out var dls))
        {
            dls = GetIncomingHealEvents(target, log, start, end).ToList();
            FilterHealEvents(log, dls, healingType);
            healEventsPerPhasePerTarget.Set(start, end, target, dls);
        }
        return dls;
    }
}
