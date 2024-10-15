using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    /// <summary> A segment of time with type <see cref="double"/> with inclusive start and inclusive end. </summary>
    using Segment = GenericSegment<double>;

    partial class AbstractSingleActor
    {
        private readonly Dictionary<ParserHelper.DamageType, CachingCollectionWithTarget<int[]>> _damageList1S = new();
        private readonly Dictionary<ParserHelper.DamageType, CachingCollectionWithTarget<int[]>> _damageTakenList1S = new();

        private CachingCollectionWithTarget<double[]>? _breakbarDamageList1S;
        private CachingCollectionWithTarget<double[]>? _breakbarDamageTakenList1S;
        private IReadOnlyList<Segment>? _healthUpdates;
        private IReadOnlyList<Segment>? _breakbarPercentUpdates;
        private IReadOnlyList<Segment>? _barrierUpdates;



        public IReadOnlyList<Segment> GetHealthUpdates(ParsedEvtcLog log)
        {
            if (_healthUpdates == null)
            {
                var events = log.CombatData.GetHealthUpdateEvents(AgentItem);
                _healthUpdates = ListFromStates(events.Select(x => x.ToState()), events.Count, log.FightData.FightStart, log.FightData.FightEnd);
            }

            return _healthUpdates;
        }

        public IReadOnlyList<Segment> GetBreakbarPercentUpdates(ParsedEvtcLog log)
        {
            if (_breakbarPercentUpdates == null)
            {
                var events = log.CombatData.GetBreakbarPercentEvents(AgentItem);
                _breakbarPercentUpdates = ListFromStates(events.Select(x => x.ToState()), events.Count, log.FightData.FightStart, log.FightData.FightEnd);
            }

            return _breakbarPercentUpdates;
        }

        public IReadOnlyList<Segment> GetBarrierUpdates(ParsedEvtcLog log)
        {
            if (_barrierUpdates == null)
            {
                var events = log.CombatData.GetBarrierUpdateEvents(AgentItem);
                _barrierUpdates = ListFromStates(events.Select(x => x.ToState()), events.Count, log.FightData.FightStart, log.FightData.FightEnd);
            }

            return _barrierUpdates;
        }

        //TODO(Rennorb) @cleanup
        static IReadOnlyList<Segment> ListFromStates(IEnumerable<(long Start, double State)> states, int stateCount, long min, long max)
        {
            if (stateCount == 0)
            {
                return [ ];
            }

            //TODO(Rennorb) @perf
            var res = new List<Segment>(stateCount);
            double lastValue = states.First().State;
            foreach ((long start, double state) in states)
            {
                long end = Math.Min(Math.Max(start, min), max);
                if (res.Count == 0)
                {
                    res.Add(new Segment(0, end, lastValue));
                }
                else
                {
                    res.Add(new Segment(res.Last().End, end, lastValue));
                }
                lastValue = state;
            }
            res.Add(new Segment(res.Last().End, max, lastValue));
            
            //TODO(Rennorb) @perf
            res.RemoveAll(x => x.Start >= x.End);
            res.FuseConsecutive();

            return res;
        }

        private static int[] ComputeDamageGraph(IEnumerable<AbstractHealthDamageEvent> dls, long start, long end)
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

        public IReadOnlyList<int> Get1SDamageList(ParsedEvtcLog log, long start, long end, AbstractSingleActor? target, ParserHelper.DamageType damageType = DamageType.All)
        {
            if (!_damageList1S.TryGetValue(damageType, out CachingCollectionWithTarget<int[]> graphs))
            {
                graphs = new CachingCollectionWithTarget<int[]>(log);
                _damageList1S[damageType] = graphs;
            }

            if (!graphs.TryGetValue(start, end, target, out var graph))
            {
                graph = ComputeDamageGraph(this.GetHitDamageEvents(target, log, start, end, damageType), start, end);
                //
                graphs.Set(start, end, target, graph);
            }

            return graph;
        }

        public IReadOnlyList<int> Get1SDamageTakenList(ParsedEvtcLog log, long start, long end, AbstractSingleActor? target, ParserHelper.DamageType damageType = DamageType.All)
        {
            if (!_damageTakenList1S.TryGetValue(damageType, out CachingCollectionWithTarget<int[]> graphs))
            {
                graphs = new CachingCollectionWithTarget<int[]>(log);
                _damageTakenList1S[damageType] = graphs;
            }

            if (!graphs.TryGetValue(start, end, target, out var graph))
            {
                graph = ComputeDamageGraph(this.GetHitDamageTakenEvents(target, log, start, end, damageType), start, end);
                //
                graphs.Set(start, end, target, graph); ;
            }

            return graph;
        }

        private static double[] ComputeBreakbarDamageGraph(IEnumerable<BreakbarDamageEvent> dls, long start, long end)
        {
            int durationInMS = (int)(end - start);
            int durationInS = durationInMS / 1000;
            double[] graph = durationInS * 1000 != durationInMS ? new double[durationInS + 2] : new double[durationInS + 1];
            // fill the graph
            int previousTime = 0;
            foreach (BreakbarDamageEvent dl in dls)
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

        public IReadOnlyList<double>? Get1SBreakbarDamageList(ParsedEvtcLog log, long start, long end, AbstractSingleActor? target)
        {
            if (!log.CombatData.HasBreakbarDamageData)
            {
                return null;
            }

            _breakbarDamageList1S ??= new CachingCollectionWithTarget<double[]>(log);

            if (_breakbarDamageList1S.TryGetValue(start, end, target, out var res))
            {
                return res;
            }

            var brkDmgList = ComputeBreakbarDamageGraph(this.GetBreakbarDamageEvents(target, log, start, end), start, end);
            _breakbarDamageList1S.Set(start, end, target, brkDmgList);
            return brkDmgList;
        }

        public IReadOnlyList<double>? Get1SBreakbarDamageTakenList(ParsedEvtcLog log, long start, long end, AbstractSingleActor? target)
        {
            if (!log.CombatData.HasBreakbarDamageData)
            {
                return null;
            }

            _breakbarDamageTakenList1S ??= new CachingCollectionWithTarget<double[]>(log);

            if (_breakbarDamageTakenList1S.TryGetValue(start, end, target, out var res))
            {
                return res;
            }

            var brkDmgList = ComputeBreakbarDamageGraph(this.GetBreakbarDamageTakenEvents(target, log, start, end), start, end);
            _breakbarDamageTakenList1S.Set(start, end, target, brkDmgList);
            return brkDmgList;
        }

        private static double GetPercentValue(IReadOnlyList<Segment> segments, long time)
        {
            int foundIndex = segments.BinarySearchRecursive(time, 0, segments.Count - 1);
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
