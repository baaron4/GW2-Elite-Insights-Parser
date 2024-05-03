using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.Extensions.HealingStatsExtensionHandler;

namespace GW2EIEvtcParser.Extensions
{
    public abstract class EXTActorHealingHelper
    {
        protected List<EXTAbstractHealingEvent> HealEvents { get; set; }
        protected Dictionary<AgentItem, List<EXTAbstractHealingEvent>> HealEventsByDst { get; set; }
        protected List<EXTAbstractHealingEvent> HealReceivedEvents { get; set; }
        protected Dictionary<AgentItem, List<EXTAbstractHealingEvent>> HealReceivedEventsBySrc { get; set; }


        private readonly Dictionary<EXTHealingType, CachingCollectionWithTarget<List<EXTAbstractHealingEvent>>> _typedHealEvents = new Dictionary<EXTHealingType, CachingCollectionWithTarget<List<EXTAbstractHealingEvent>>>();
        private readonly Dictionary<EXTHealingType, CachingCollectionWithTarget<List<EXTAbstractHealingEvent>>> _typedIncomingHealEvents = new Dictionary<EXTHealingType, CachingCollectionWithTarget<List<EXTAbstractHealingEvent>>>();

        internal EXTActorHealingHelper()
        {
        }

        public abstract IReadOnlyList<EXTAbstractHealingEvent> GetOutgoingHealEvents(AbstractSingleActor target, ParsedEvtcLog log, long start, long end);

        public abstract IReadOnlyList<EXTAbstractHealingEvent> GetIncomingHealEvents(AbstractSingleActor target, ParsedEvtcLog log, long start, long end);

        private static void FilterHealEvents(ParsedEvtcLog log, List<EXTAbstractHealingEvent> dls, EXTHealingType healingType)
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

        public IReadOnlyList<EXTAbstractHealingEvent> GetTypedOutgoingHealEvents(AbstractSingleActor target, ParsedEvtcLog log, long start, long end, EXTHealingType healingType)
        {
            if (!_typedHealEvents.TryGetValue(healingType, out CachingCollectionWithTarget<List<EXTAbstractHealingEvent>> healEventsPerPhasePerTarget))
            {
                healEventsPerPhasePerTarget = new CachingCollectionWithTarget<List<EXTAbstractHealingEvent>>(log);
                _typedHealEvents[healingType] = healEventsPerPhasePerTarget;
            }
            if (!healEventsPerPhasePerTarget.TryGetValue(start, end, target, out List<EXTAbstractHealingEvent> dls))
            {
                dls = GetOutgoingHealEvents(target, log, start, end).ToList();
                FilterHealEvents(log, dls, healingType);
                healEventsPerPhasePerTarget.Set(start, end, target, dls);
            }
            return dls;
        }

        public IReadOnlyList<EXTAbstractHealingEvent> GetTypedIncomingHealEvents(AbstractSingleActor target, ParsedEvtcLog log, long start, long end, EXTHealingType healingType)
        {
            if (!_typedIncomingHealEvents.TryGetValue(healingType, out CachingCollectionWithTarget<List<EXTAbstractHealingEvent>> healEventsPerPhasePerTarget))
            {
                healEventsPerPhasePerTarget = new CachingCollectionWithTarget<List<EXTAbstractHealingEvent>>(log);
                _typedIncomingHealEvents[healingType] = healEventsPerPhasePerTarget;
            }
            if (!healEventsPerPhasePerTarget.TryGetValue(start, end, target, out List<EXTAbstractHealingEvent> dls))
            {
                dls = GetIncomingHealEvents(target, log, start, end).ToList();
                FilterHealEvents(log, dls, healingType);
                healEventsPerPhasePerTarget.Set(start, end, target, dls);
            }
            return dls;
        }
    }
}
