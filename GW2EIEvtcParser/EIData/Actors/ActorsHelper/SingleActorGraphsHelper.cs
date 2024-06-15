using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    internal class SingleActorGraphsHelper : AbstractSingleActorHelper
    {
        private readonly Dictionary<ParserHelper.DamageType, CachingCollectionWithTarget<int[]>> _damageList1S = new Dictionary<ParserHelper.DamageType, CachingCollectionWithTarget<int[]>>();
        private readonly Dictionary<ParserHelper.DamageType, CachingCollectionWithTarget<int[]>> _damageTakenList1S = new Dictionary<ParserHelper.DamageType, CachingCollectionWithTarget<int[]>>();

        private CachingCollectionWithTarget<double[]> _breakbarDamageList1S;
        private CachingCollectionWithTarget<double[]> _breakbarDamageTakenList1S;
        private List<Segment> _healthUpdates { get; set; }
        private List<Segment> _breakbarPercentUpdates { get; set; }
        private List<Segment> _barrierUpdates { get; set; }

        public SingleActorGraphsHelper(AbstractSingleActor actor) : base(actor)
        {
        }



        public IReadOnlyList<Segment> GetHealthUpdates(ParsedEvtcLog log)
        {
            if (_healthUpdates == null)
            {
                _healthUpdates = Segment.FromStates(log.CombatData.GetHealthUpdateEvents(AgentItem).Select(x => x.ToState()).ToList(), log.FightData.FightStart, log.FightData.FightEnd);
            }
            return _healthUpdates;
        }

        public IReadOnlyList<Segment> GetBreakbarPercentUpdates(ParsedEvtcLog log)
        {
            if (_breakbarPercentUpdates == null)
            {
                _breakbarPercentUpdates = Segment.FromStates(log.CombatData.GetBreakbarPercentEvents(AgentItem).Select(x => x.ToState()).ToList(), log.FightData.FightStart, log.FightData.FightEnd);
            }
            return _breakbarPercentUpdates;
        }

        public IReadOnlyList<Segment> GetBarrierUpdates(ParsedEvtcLog log)
        {
            if (_barrierUpdates == null)
            {
                _barrierUpdates = Segment.FromStates(log.CombatData.GetBarrierUpdateEvents(AgentItem).Select(x => x.ToState()).ToList(), log.FightData.FightStart, log.FightData.FightEnd);
            }
            return _barrierUpdates;
        }

        private static int[] ComputeDamageGraph(IReadOnlyList<AbstractHealthDamageEvent> dls, long start, long end)
        {
            int durationInMS = (int)(end - start);
            int durationInS = durationInMS / 1000;
            var graph = durationInS * 1000 != durationInMS ? new int[durationInS + 2] : new int[durationInS + 1];
            // fill the graph
            int previousTime = 0;
            foreach (AbstractHealthDamageEvent dl in dls)
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
                graph[time] += dl.HealthDamage;
            }
            for (int i = previousTime + 1; i < graph.Length; i++)
            {
                graph[i] = graph[previousTime];
            }
            return graph;
        }

        public IReadOnlyList<int> Get1SDamageList(ParsedEvtcLog log, long start, long end, AbstractSingleActor target, ParserHelper.DamageType damageType = DamageType.All)
        {
            if (!_damageList1S.TryGetValue(damageType, out CachingCollectionWithTarget<int[]> graphs))
            {
                graphs = new CachingCollectionWithTarget<int[]>(log);
                _damageList1S[damageType] = graphs;
            }
            if (!graphs.TryGetValue(start, end, target, out int[] graph))
            {
                graph = ComputeDamageGraph(Actor.GetHitDamageEvents(target, log, start, end, damageType), start, end);
                //
                graphs.Set(start, end, target, graph);
            }
            return graph;
        }

        public IReadOnlyList<int> Get1SDamageTakenList(ParsedEvtcLog log, long start, long end, AbstractSingleActor target, ParserHelper.DamageType damageType = DamageType.All)
        {
            if (!_damageTakenList1S.TryGetValue(damageType, out CachingCollectionWithTarget<int[]> graphs))
            {
                graphs = new CachingCollectionWithTarget<int[]>(log);
                _damageTakenList1S[damageType] = graphs;
            }
            if (!graphs.TryGetValue(start, end, target, out int[] graph))
            {
                graph = ComputeDamageGraph(Actor.GetHitDamageTakenEvents(target, log, start, end, damageType), start, end);
                //
                graphs.Set(start, end, target, graph); ;
            }
            return graph;
        }

        private static double[] ComputeBreakbarDamageGraph(IReadOnlyList<AbstractBreakbarDamageEvent> dls, long start, long end)
        {
            int durationInMS = (int)(end - start);
            int durationInS = durationInMS / 1000;
            double[] graph = durationInS * 1000 != durationInMS ? new double[durationInS + 2] : new double[durationInS + 1];
            // fill the graph
            int previousTime = 0;
            foreach (AbstractBreakbarDamageEvent dl in dls)
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
                graph[time] += dl.BreakbarDamage;
            }
            for (int i = previousTime + 1; i < graph.Length; i++)
            {
                graph[i] = graph[previousTime];
            }
            return graph;
        }

        public IReadOnlyList<double> Get1SBreakbarDamageList(ParsedEvtcLog log, long start, long end, AbstractSingleActor target)
        {
            if (!log.CombatData.HasBreakbarDamageData)
            {
                return null;
            }
            if (_breakbarDamageList1S == null)
            {
                _breakbarDamageList1S = new CachingCollectionWithTarget<double[]>(log);
            }
            if (_breakbarDamageList1S.TryGetValue(start, end, target, out double[] res))
            {
                return res;
            }
            var brkDmgList = ComputeBreakbarDamageGraph(Actor.GetBreakbarDamageEvents(target, log, start, end), start, end);
            _breakbarDamageList1S.Set(start, end, target, brkDmgList);
            return brkDmgList;
        }

        public IReadOnlyList<double> Get1SBreakbarDamageTakenList(ParsedEvtcLog log, long start, long end, AbstractSingleActor target)
        {
            if (!log.CombatData.HasBreakbarDamageData)
            {
                return null;
            }
            if (_breakbarDamageTakenList1S == null)
            {
                _breakbarDamageTakenList1S = new CachingCollectionWithTarget<double[]>(log);
            }
            if (_breakbarDamageTakenList1S.TryGetValue(start, end, target, out double[] res))
            {
                return res;
            }
            var brkDmgList = ComputeBreakbarDamageGraph(Actor.GetBreakbarDamageTakenEvents(target, log, start, end), start, end);
            _breakbarDamageTakenList1S.Set(start, end, target, brkDmgList);
            return brkDmgList;
        }

        private static double GetPercentValue(IReadOnlyList<Segment> segments, long time)
        {
            int foundIndex = Segment.BinarySearchRecursive(segments, time, 0, segments.Count - 1);
            Segment found = segments[foundIndex];
            if (found.ContainsPoint(time))
            {
                return found.Value;
            }
            return -1.0;
        }

        public double GetCurrentHealthPercent(ParsedEvtcLog log, long time)
        {
            IReadOnlyList<Segment> hps = GetHealthUpdates(log);
            if (!hps.Any())
            {
                return -1.0;
            }
            return GetPercentValue(hps, time);
        }

        public double GetCurrentBarrierPercent(ParsedEvtcLog log, long time)
        {
            IReadOnlyList<Segment> barriers = GetBarrierUpdates(log);
            if (!barriers.Any())
            {
                return -1.0;
            }
            return GetPercentValue(barriers, time);
        }

    }
}
