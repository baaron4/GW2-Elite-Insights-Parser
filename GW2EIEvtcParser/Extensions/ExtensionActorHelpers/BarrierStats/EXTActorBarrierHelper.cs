using System.Diagnostics.CodeAnalysis;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.Extensions;

public abstract class EXTActorBarrierHelper
{
    protected Dictionary<AgentItem, List<EXTBarrierEvent>>? BarrierEventsByDst;
    protected Dictionary<AgentItem, List<EXTBarrierEvent>>? BarrierReceivedEventsBySrc;

    internal EXTActorBarrierHelper()
    {
    }

    #region INITIALIZERS
    [MemberNotNull(nameof(BarrierEventsByDst))]
    protected abstract void InitBarrierEvents(ParsedEvtcLog log);

    [MemberNotNull(nameof(BarrierReceivedEventsBySrc))]
    protected abstract void InitIncomingBarrierEvents(ParsedEvtcLog log);
    #endregion INITIALIZERS

    private CachingCollectionWithTarget<List<EXTBarrierEvent>>? BarrierEventByDstCache;
    public IReadOnlyList<EXTBarrierEvent> GetOutgoingBarrierEvents(SingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        InitBarrierEvents(log);
        BarrierEventByDstCache ??= new(log);
        if (!BarrierEventByDstCache.TryGetValue(start, end, target, out var list))
        {
            if (target != null)
            {
                if (BarrierEventsByDst.TryGetValue(target.EnglobingAgentItem, out var damageEvents))
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
                list = BarrierEventsByDst[ParserHelper._nullAgent].Where(x => x.Time >= start && x.Time <= end).ToList();
            }
            BarrierEventByDstCache.Set(start, end, target, list);
        }
        return list;
    }
    public IReadOnlyList<EXTBarrierEvent> GetOutgoingBarrierEvents(SingleActor? target, ParsedEvtcLog log)
    {
        return GetOutgoingBarrierEvents(target, log, log.LogData.LogStart, log.LogData.LogEnd);
    }


    private CachingCollectionWithTarget<List<EXTBarrierEvent>>? BarrierReceivedEventBySrcCache;
    public IReadOnlyList<EXTBarrierEvent> GetIncomingBarrierEvents(SingleActor? target, ParsedEvtcLog log, long start, long end)
    {
        InitIncomingBarrierEvents(log);
        BarrierReceivedEventBySrcCache ??= new(log);
        if (!BarrierReceivedEventBySrcCache.TryGetValue(start, end, target, out var list))
        {
            if (target != null)
            {
                if (BarrierReceivedEventsBySrc.TryGetValue(target.EnglobingAgentItem, out var damageEvents))
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
                list = BarrierReceivedEventsBySrc[ParserHelper._nullAgent].Where(x => x.Time >= start && x.Time <= end).ToList();
            }
            BarrierReceivedEventBySrcCache.Set(start, end, target, list);
        }
        return list;
    }
    public IEnumerable<EXTBarrierEvent> GetIncomingBarrierEvents(SingleActor? target, ParsedEvtcLog log)
    {
        return GetIncomingBarrierEvents(target, log, log.LogData.LogStart, log.LogData.LogEnd);
    }

}
