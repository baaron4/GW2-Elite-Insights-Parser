using System.Collections.Generic;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.Extensions;

public abstract class EXTActorBarrierHelper
{
    protected List<EXTAbstractBarrierEvent>? BarrierEvents;
    protected Dictionary<AgentItem, List<EXTAbstractBarrierEvent>>? BarrierEventsByDst;
    protected List<EXTAbstractBarrierEvent>? BarrierReceivedEvents;
    protected Dictionary<AgentItem, List<EXTAbstractBarrierEvent>>? BarrierReceivedEventsBySrc;

    internal EXTActorBarrierHelper()
    {
    }

    public abstract IEnumerable<EXTAbstractBarrierEvent> GetOutgoingBarrierEvents(AbstractSingleActor target, ParsedEvtcLog log, long start, long end);

    public abstract IEnumerable<EXTAbstractBarrierEvent> GetIncomingBarrierEvents(AbstractSingleActor target, ParsedEvtcLog log, long start, long end);

}
