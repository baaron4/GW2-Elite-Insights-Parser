using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.Extensions.HealingStatsExtensionHandler;

namespace GW2EIEvtcParser.Extensions
{
    public abstract class EXTActorBarrierHelper
    {
        protected List<EXTAbstractBarrierEvent> BarrierEvents { get; set; }
        protected Dictionary<AgentItem, List<EXTAbstractBarrierEvent>> BarrierEventsByDst { get; set; }
        protected List<EXTAbstractBarrierEvent> BarrierReceivedEvents { get; set; }
        protected Dictionary<AgentItem, List<EXTAbstractBarrierEvent>> BarrierReceivedEventsBySrc { get; set; }

        internal EXTActorBarrierHelper()
        {
        }

        public abstract IReadOnlyList<EXTAbstractBarrierEvent> GetOutgoingBarrierEvents(AbstractSingleActor target, ParsedEvtcLog log, long start, long end);

        public abstract IReadOnlyList<EXTAbstractBarrierEvent> GetIncomingBarrierEvents(AbstractSingleActor target, ParsedEvtcLog log, long start, long end);

    }
}
