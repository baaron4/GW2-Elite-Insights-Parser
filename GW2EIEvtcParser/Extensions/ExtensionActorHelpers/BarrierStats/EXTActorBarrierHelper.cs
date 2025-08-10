using System.Diagnostics.CodeAnalysis;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.Extensions;

public abstract class EXTActorBarrierHelper
{
    protected List<EXTBarrierEvent>? BarrierEvents;
    protected Dictionary<AgentItem, List<EXTBarrierEvent>>? BarrierEventsByDst;
    protected List<EXTBarrierEvent>? BarrierReceivedEvents;
    protected Dictionary<AgentItem, List<EXTBarrierEvent>>? BarrierReceivedEventsBySrc;

    internal EXTActorBarrierHelper()
    {
    }


    [MemberNotNull(nameof(BarrierEvents))]
    [MemberNotNull(nameof(BarrierEventsByDst))]
    protected abstract void InitBarrierEvents(ParsedEvtcLog log);

    [MemberNotNull(nameof(BarrierReceivedEvents))]
    [MemberNotNull(nameof(BarrierReceivedEventsBySrc))]
    protected abstract void InitIncomingBarrierEvents(ParsedEvtcLog log);

    public abstract IEnumerable<EXTBarrierEvent> GetOutgoingBarrierEvents(SingleActor? target, ParsedEvtcLog log, long start, long end);
    public IEnumerable<EXTBarrierEvent> GetOutgoingBarrierEvents(SingleActor? target, ParsedEvtcLog log)
    {
        return GetOutgoingBarrierEvents(target, log, log.LogData.LogStart, log.LogData.LogEnd);
    }

    public abstract IEnumerable<EXTBarrierEvent> GetIncomingBarrierEvents(SingleActor? target, ParsedEvtcLog log, long start, long end);
    public IEnumerable<EXTBarrierEvent> GetIncomingBarrierEvents(SingleActor? target, ParsedEvtcLog log)
    {
        return GetIncomingBarrierEvents(target, log, log.LogData.LogStart, log.LogData.LogEnd);
    }

}
