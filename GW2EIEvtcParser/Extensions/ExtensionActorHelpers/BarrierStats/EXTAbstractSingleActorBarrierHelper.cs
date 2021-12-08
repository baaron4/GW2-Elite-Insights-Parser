using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.Extensions.HealingStatsExtensionHandler;

namespace GW2EIEvtcParser.Extensions
{
    public class EXTAbstractSingleActorBarrierHelper : EXTActorBarrierHelper
    {
        private AbstractSingleActor _actor { get; }
        private AgentItem _agentItem => _actor.AgentItem;

        private CachingCollectionWithTarget<int[]> _healing1S;

        private CachingCollectionWithTarget<EXTFinalOutgoingHealingStat> _outgoingHealStats { get; set; }
        private CachingCollectionWithTarget<EXTFinalIncomingHealingStat> _incomingHealStats { get; set; }

        internal EXTAbstractSingleActorBarrierHelper(AbstractSingleActor actor) : base()
        {
            _actor = actor;
        }

        public override IReadOnlyList<EXTAbstractBarrierEvent> GetOutgoingBarrierEvents(AbstractSingleActor target, ParsedEvtcLog log, long start, long end)
        {
            if (!log.CombatData.HasEXTBarrier)
            {
                throw new InvalidOperationException("Healing Stats extension not present");
            }
            if (HealEvents == null)
            {
                HealEvents = new List<EXTAbstractBarrierEvent>();
                HealEvents.AddRange(log.CombatData.EXTBarrierCombatData.GetBarrierData(_agentItem).Where(x => x.ToFriendly));
                IReadOnlyDictionary<long, Minions> minionsList = _actor.GetMinions(log);
                foreach (Minions mins in minionsList.Values)
                {
                    HealEvents.AddRange(mins.EXTBarrier.GetOutgoingBarrierEvents(null, log, 0, log.FightData.FightEnd));
                }
                HealEvents = HealEvents.OrderBy(x => x.Time).ToList();
                HealEventsByDst = HealEvents.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            }
            if (target != null)
            {
                if (HealEventsByDst.TryGetValue(target.AgentItem, out List<EXTAbstractBarrierEvent> list))
                {
                    return list.Where(x => x.Time >= start && x.Time <= end).ToList();
                }
                else
                {
                    return new List<EXTAbstractBarrierEvent>();
                }
            }
            return HealEvents.Where(x => x.Time >= start && x.Time <= end).ToList();
        }

        public override IReadOnlyList<EXTAbstractBarrierEvent> GetIncomingBarrierEvents(AbstractSingleActor target, ParsedEvtcLog log, long start, long end)
        {
            if (!log.CombatData.HasEXTBarrier)
            {
                throw new InvalidOperationException("Healing Stats extension not present");
            }
            if (HealReceivedEvents == null)
            {
                HealReceivedEvents = new List<EXTAbstractBarrierEvent>();
                HealReceivedEvents.AddRange(log.CombatData.EXTBarrierCombatData.GetBarrierReceivedData(_agentItem).Where(x => x.ToFriendly));
                HealReceivedEvents = HealReceivedEvents.OrderBy(x => x.Time).ToList();
                HealReceivedEventsBySrc = HealReceivedEvents.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
            }
            if (target != null)
            {
                if (HealReceivedEventsBySrc.TryGetValue(target.AgentItem, out List<EXTAbstractBarrierEvent> list))
                {
                    return list.Where(x => x.Time >= start && x.Time <= end).ToList();
                }
                else
                {
                    return new List<EXTAbstractBarrierEvent>();
                }
            }
            return HealReceivedEvents.Where(x => x.Time >= start && x.Time <= end).ToList();
        }

        public IReadOnlyList<EXTAbstractBarrierEvent> GetJustActorOutgoingHealEvents(AbstractSingleActor target, ParsedEvtcLog log, long start, long end)
        {
            return GetOutgoingBarrierEvents(target, log, start, end).Where(x => x.From == _agentItem).ToList();
        }

        public IReadOnlyList<int> Get1SHealingList(ParsedEvtcLog log, long start, long end, AbstractSingleActor target)
        {
            if (_healing1S == null)
            {
                _healing1S = new CachingCollectionWithTarget<int[]>(log);
            }
            if (!_healing1S.TryGetValue(start, end, target, out int[] graph))
            {
                int durationInMS = (int)(end - start);
                int durationInS = durationInMS / 1000;
                graph = durationInS * 1000 != durationInMS ? new int[durationInS + 2] : new int[durationInS + 1];
                // fill the graph
                int previousTime = 0;
                foreach (EXTAbstractBarrierEvent dl in GetOutgoingBarrierEvents(target, log, start, end))
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
                    graph[time] += dl.BarrierGiven;
                }
                for (int i = previousTime + 1; i < graph.Length; i++)
                {
                    graph[i] = graph[previousTime];
                }
                //
                _healing1S.Set(start, end, target, graph);
            }
            return graph;
        }

        public EXTFinalOutgoingHealingStat GetOutgoingBarrierStats(AbstractSingleActor target, ParsedEvtcLog log, long start, long end)
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

        public EXTFinalIncomingHealingStat GetIncomingBarrierStats(AbstractSingleActor target, ParsedEvtcLog log, long start, long end)
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
