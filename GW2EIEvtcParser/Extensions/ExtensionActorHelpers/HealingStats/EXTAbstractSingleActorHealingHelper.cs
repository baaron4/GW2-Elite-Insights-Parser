using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.Extensions.HealingStatsExtensionHandler;

namespace GW2EIEvtcParser.Extensions
{
    public class EXTAbstractSingleActorHealingHelper : EXTActorHealingHelper
    {
        private AbstractSingleActor _actor { get; }
        private AgentItem _agentItem => _actor.AgentItem;

        private readonly Dictionary<EXTHealingType, CachingCollectionWithTarget<List<EXTAbstractHealingEvent>>> _typedSelfHealEvents = new Dictionary<EXTHealingType, CachingCollectionWithTarget<List<EXTAbstractHealingEvent>>>();

        private readonly Dictionary<EXTHealingType, CachingCollectionWithTarget<int[]>> _healing1S = new Dictionary<EXTHealingType, CachingCollectionWithTarget<int[]>>();
        private readonly Dictionary<EXTHealingType, CachingCollectionWithTarget<int[]>> _healingReceived1S = new Dictionary<EXTHealingType, CachingCollectionWithTarget<int[]>>();

        private CachingCollectionWithTarget<EXTFinalOutgoingHealingStat> _outgoingHealStats { get; set; }
        private CachingCollectionWithTarget<EXTFinalIncomingHealingStat> _incomingHealStats { get; set; }

        internal EXTAbstractSingleActorHealingHelper(AbstractSingleActor actor) : base()
        {
            _actor = actor;
        }

        public override IReadOnlyList<EXTAbstractHealingEvent> GetOutgoingHealEvents(AbstractSingleActor target, ParsedEvtcLog log, long start, long end)
        {
            if (!log.CombatData.HasEXTHealing)
            {
                throw new InvalidOperationException("Healing Stats extension not present");
            }
            if (HealEvents == null)
            {
                HealEvents = new List<EXTAbstractHealingEvent>();
                HealEvents.AddRange(log.CombatData.EXTHealingCombatData.GetHealData(_agentItem).Where(x => x.ToFriendly));
                IReadOnlyDictionary<long, Minions> minionsList = _actor.GetMinions(log);
                foreach (Minions mins in minionsList.Values)
                {
                    HealEvents.AddRange(mins.EXTHealing.GetOutgoingHealEvents(null, log, log.FightData.FightStart, log.FightData.FightEnd));
                }
                HealEvents = HealEvents.OrderBy(x => x.Time).ToList();
                HealEventsByDst = HealEvents.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            }
            if (target != null)
            {
                if (HealEventsByDst.TryGetValue(target.AgentItem, out List<EXTAbstractHealingEvent> list))
                {
                    return list.Where(x => x.Time >= start && x.Time <= end).ToList();
                }
                else
                {
                    return new List<EXTAbstractHealingEvent>();
                }
            }
            return HealEvents.Where(x => x.Time >= start && x.Time <= end).ToList();
        }

        public override IReadOnlyList<EXTAbstractHealingEvent> GetIncomingHealEvents(AbstractSingleActor target, ParsedEvtcLog log, long start, long end)
        {
            if (!log.CombatData.HasEXTHealing)
            {
                throw new InvalidOperationException("Healing Stats extension not present");
            }
            if (HealReceivedEvents == null)
            {
                HealReceivedEvents = new List<EXTAbstractHealingEvent>();
                HealReceivedEvents.AddRange(log.CombatData.EXTHealingCombatData.GetHealReceivedData(_agentItem).Where(x => x.ToFriendly));
                HealReceivedEvents = HealReceivedEvents.OrderBy(x => x.Time).ToList();
                HealReceivedEventsBySrc = HealReceivedEvents.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
            }
            if (target != null)
            {
                if (HealReceivedEventsBySrc.TryGetValue(target.AgentItem, out List<EXTAbstractHealingEvent> list))
                {
                    return list.Where(x => x.Time >= start && x.Time <= end).ToList();
                }
                else
                {
                    return new List<EXTAbstractHealingEvent>();
                }
            }
            return HealReceivedEvents.Where(x => x.Time >= start && x.Time <= end).ToList();
        }

        public IReadOnlyList<EXTAbstractHealingEvent> GetJustActorOutgoingHealEvents(AbstractSingleActor target, ParsedEvtcLog log, long start, long end)
        {
            return GetOutgoingHealEvents(target, log, start, end).Where(x => x.From == _agentItem).ToList();
        }

        internal IReadOnlyList<EXTAbstractHealingEvent> GetJustActorTypedOutgoingHealEvents(AbstractSingleActor target, ParsedEvtcLog log, long start, long end, EXTHealingType healingType)
        {
            if (!_typedSelfHealEvents.TryGetValue(healingType, out CachingCollectionWithTarget<List<EXTAbstractHealingEvent>> healEventsPerPhasePerTarget))
            {
                healEventsPerPhasePerTarget = new CachingCollectionWithTarget<List<EXTAbstractHealingEvent>>(log);
                _typedSelfHealEvents[healingType] = healEventsPerPhasePerTarget;
            }
            if (!healEventsPerPhasePerTarget.TryGetValue(start, end, target, out List<EXTAbstractHealingEvent> dls))
            {
                dls = GetTypedOutgoingHealEvents(target, log, start, end, healingType).Where(x => x.From == _agentItem).ToList();
                healEventsPerPhasePerTarget.Set(start, end, target, dls);
            }
            return dls;
        }

        private static int[] ComputeHealingGraph(IReadOnlyList<EXTAbstractHealingEvent> dls, long start, long end)
        {
            int durationInMS = (int)(end - start);
            int durationInS = durationInMS / 1000;
            var graph = durationInS * 1000 != durationInMS ? new int[durationInS + 2] : new int[durationInS + 1];
            // fill the graph
            int previousTime = 0;
            foreach (EXTAbstractHealingEvent dl in dls)
            {
                int time = (int)Math.Ceiling((dl.Time - start) / 1000.0);
                if (time != previousTime)
                {
                    for (int i = previousTime + 1; i <= time; i++)
                    {
                        graph[i] = graph[previousTime];
                    }
                }
                previousTime = time;
                graph[time] += dl.HealingDone;
            }
            for (int i = previousTime + 1; i < graph.Length; i++)
            {
                graph[i] = graph[previousTime];
            }
            return graph;
        }

        public IReadOnlyList<int> Get1SHealingList(ParsedEvtcLog log, long start, long end, AbstractSingleActor target, EXTHealingType healingType = EXTHealingType.All)
        {
            if (!_healing1S.TryGetValue(healingType, out CachingCollectionWithTarget<int[]> graphs))
            {
                graphs = new CachingCollectionWithTarget<int[]>(log);
                _healing1S[healingType] = graphs;
            }
            if (!graphs.TryGetValue(start, end, target, out int[] graph))
            {
                graph = ComputeHealingGraph(GetTypedOutgoingHealEvents(target, log, start, end, healingType), start, end);
                //
                graphs.Set(start, end, target, graph);
            }
            return graph;
        }

        public IReadOnlyList<int> Get1SHealingReceivedList(ParsedEvtcLog log, long start, long end, AbstractSingleActor target, EXTHealingType healingType = EXTHealingType.All)
        {
            if (!_healingReceived1S.TryGetValue(healingType, out CachingCollectionWithTarget<int[]> graphs))
            {
                graphs = new CachingCollectionWithTarget<int[]>(log);
                _healingReceived1S[healingType] = graphs;
            }
            if (!graphs.TryGetValue(start, end, target, out int[] graph))
            {
                graph = ComputeHealingGraph(GetTypedIncomingHealEvents(target, log, start, end, healingType), start, end);
                //
                graphs.Set(start, end, target, graph);
            }
            return graph;
        }

        public EXTFinalOutgoingHealingStat GetOutgoingHealStats(AbstractSingleActor target, ParsedEvtcLog log, long start, long end)
        {
            if (_outgoingHealStats == null)
            {
                _outgoingHealStats = new CachingCollectionWithTarget<EXTFinalOutgoingHealingStat>(log);
            }
            if (!_outgoingHealStats.TryGetValue(start, end, target, out EXTFinalOutgoingHealingStat value))
            {
                value = new EXTFinalOutgoingHealingStat(log, start, end, _actor, target);
                _outgoingHealStats.Set(start, end, target, value);
            }
            return value;
        }

        public EXTFinalIncomingHealingStat GetIncomingHealStats(AbstractSingleActor target, ParsedEvtcLog log, long start, long end)
        {
            if (_incomingHealStats == null)
            {
                _incomingHealStats = new CachingCollectionWithTarget<EXTFinalIncomingHealingStat>(log);
            }
            if (!_incomingHealStats.TryGetValue(start, end, target, out EXTFinalIncomingHealingStat value))
            {
                value = new EXTFinalIncomingHealingStat(log, start, end, _actor, target);
                _incomingHealStats.Set(start, end, target, value);
            }
            return value;
        }
    }
}
