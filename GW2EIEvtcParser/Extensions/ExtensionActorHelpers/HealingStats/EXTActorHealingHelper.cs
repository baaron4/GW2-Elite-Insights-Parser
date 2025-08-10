using System.Diagnostics.CodeAnalysis;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.Extensions.HealingStatsExtensionHandler;

namespace GW2EIEvtcParser.Extensions;

public abstract class EXTActorHealingHelper
{
    protected List<EXTHealingEvent>? HealEvents;
    protected Dictionary<AgentItem, List<EXTHealingEvent>>? HealEventsByDst;
    protected List<EXTHealingEvent>? HealReceivedEvents;
    protected Dictionary<AgentItem, List<EXTHealingEvent>>? HealReceivedEventsBySrc;

    //TODO(Rennorb) @perf
    private readonly Dictionary<EXTHealingType, CachingCollectionWithTarget<List<EXTHealingEvent>>> _typedHealEvents = [];
    private readonly Dictionary<EXTHealingType, CachingCollectionWithTarget<List<EXTHealingEvent>>> _typedIncomingHealEvents = [];

    internal EXTActorHealingHelper()
    {
    }

    [MemberNotNull(nameof(HealEvents))]
    [MemberNotNull(nameof(HealEventsByDst))]
    protected abstract void InitHealEvents(ParsedEvtcLog log);

    [MemberNotNull(nameof(HealReceivedEvents))]
    [MemberNotNull(nameof(HealReceivedEventsBySrc))]
    protected abstract void InitIncomingHealEvents(ParsedEvtcLog log);

    public abstract IEnumerable<EXTHealingEvent> GetOutgoingHealEvents(SingleActor? target, ParsedEvtcLog log, long start, long end);

    public IEnumerable<EXTHealingEvent> GetOutgoingHealEvents(SingleActor? target, ParsedEvtcLog log)
    {
        return GetOutgoingHealEvents(target, log, log.LogData.LogStart, log.LogData.LogEnd);
    }

    public abstract IEnumerable<EXTHealingEvent> GetIncomingHealEvents(SingleActor? target, ParsedEvtcLog log, long start, long end);
    public IEnumerable<EXTHealingEvent> GetIncomingHealEvents(SingleActor? target, ParsedEvtcLog log)
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
