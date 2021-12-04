using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    internal class SingleActorBuffsHelper : AbstractSingleActorHelper
    {
        private List<Consumable> _consumeList;
        // Boons
        private HashSet<Buff> _trackedBuffs;
        private BuffDictionary _buffMap;
        private Dictionary<long, BuffsGraphModel> _buffGraphs { get; set; }
        private CachingCollection<BuffDistribution> _buffDistribution;
        private CachingCollection<Dictionary<long, long>> _buffPresence;
        private CachingCollectionCustom<BuffEnum, Dictionary<long, FinalActorBuffs>[]> _buffStats;
        private CachingCollection<(Dictionary<long, FinalBuffsDictionary>, Dictionary<long, FinalBuffsDictionary>)> _buffsDictionary;
        private readonly Dictionary<long, AbstractBuffSimulator> _buffSimulators = new Dictionary<long, AbstractBuffSimulator>();

        public SingleActorBuffsHelper(AbstractSingleActor actor) : base(actor)
        {
        }


        public BuffDistribution GetBuffDistribution(ParsedEvtcLog log, long start, long end)
        {
            if (_buffGraphs == null)
            {
                SetBuffGraphs(log);
            }
            if (!_buffDistribution.TryGetValue(start, end, out BuffDistribution value))
            {
                value = ComputeBuffDistribution(start, end);
                _buffDistribution.Set(start, end, value);
            }
            return value;
        }

        private BuffDistribution ComputeBuffDistribution(long start, long end)
        {
            var res = new BuffDistribution();
            foreach (KeyValuePair<long, AbstractBuffSimulator> pair in _buffSimulators)
            {
                foreach (BuffSimulationItem simul in pair.Value.GenerationSimulation)
                {
                    simul.SetBuffDistributionItem(res, start, end, pair.Key);
                }
                foreach (BuffSimulationItemWasted simul in pair.Value.WasteSimulationResult)
                {
                    simul.SetBuffDistributionItem(res, start, end, pair.Key);
                }
                foreach (BuffSimulationItemOverstack simul in pair.Value.OverstackSimulationResult)
                {
                    simul.SetBuffDistributionItem(res, start, end, pair.Key);
                }
            }
            return res;
        }

        public Dictionary<long, long> GetBuffPresence(ParsedEvtcLog log, long start, long end)
        {
            if (_buffGraphs == null)
            {
                SetBuffGraphs(log);
            }
            if (!_buffPresence.TryGetValue(start, end, out Dictionary<long, long> value))
            {
                value = ComputeBuffPresence(start, end);
                _buffPresence.Set(start, end, value);
            }
            return value;
        }

        private Dictionary<long, long> ComputeBuffPresence(long start, long end)
        {
            var buffPresence = new Dictionary<long, long>();
            foreach (KeyValuePair<long, AbstractBuffSimulator> pair in _buffSimulators)
            {
                foreach (BuffSimulationItem simul in pair.Value.GenerationSimulation)
                {
                    long value = simul.GetClampedDuration(start, end);
                    if (value == 0)
                    {
                        continue;
                    }
                    Add(buffPresence, pair.Key, value);
                }
            }
            return buffPresence;
        }

        public Dictionary<long, BuffsGraphModel> GetBuffGraphs(ParsedEvtcLog log)
        {
            if (_buffGraphs == null)
            {
                SetBuffGraphs(log);
            }
            return _buffGraphs;
        }

        /// <summary>
        /// Checks if a buff is present on the actor. Given buff id must be in the buff simulator, throws <see cref="InvalidOperationException"/> otherwise
        /// </summary>
        /// <param name="log"></param>
        /// <param name="buffId"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool HasBuff(ParsedEvtcLog log, long buffId, long time)
        {
            if (!log.Buffs.BuffsByIds.ContainsKey(buffId))
            {
                throw new InvalidOperationException("Buff id must be simulated");
            }
            Dictionary<long, BuffsGraphModel> bgms = GetBuffGraphs(log);
            if (bgms.TryGetValue(buffId, out BuffsGraphModel bgm))
            {
                return bgm.IsPresent(time);
            }
            else
            {
                return false;
            }
        }

        public IReadOnlyDictionary<long, FinalActorBuffs> GetBuffs(BuffEnum type, ParsedEvtcLog log, long start, long end)
        {
            if (_buffStats == null)
            {
                _buffStats = new CachingCollectionCustom<BuffEnum, Dictionary<long, FinalActorBuffs>[]>(log, BuffEnum.Self);
            }
            if (!_buffStats.TryGetValue(start, end, type, out Dictionary<long, FinalActorBuffs>[] value))
            {
                value = Actor.ComputeBuffs(log, start, end, type);
                _buffStats.Set(start, end, type, value);
            }
            return value[0];
        }

        public IReadOnlyDictionary<long, FinalActorBuffs> GetActiveBuffs(BuffEnum type, ParsedEvtcLog log, long start, long end)
        {
            if (_buffStats == null)
            {
                _buffStats = new CachingCollectionCustom<BuffEnum, Dictionary<long, FinalActorBuffs>[]>(log, BuffEnum.Self);
            }
            if (!_buffStats.TryGetValue(start, end, type, out Dictionary<long, FinalActorBuffs>[] value))
            {
                value = Actor.ComputeBuffs(log, start, end, type);
                _buffStats.Set(start, end, type, value);
            }
            return value[1];
        }

        public IReadOnlyCollection<Buff> GetTrackedBuffs(ParsedEvtcLog log)
        {
            if (_buffMap == null)
            {
                ComputeBuffMap(log);
            }
            return _trackedBuffs;
        }

        private void ComputeBuffMap(ParsedEvtcLog log)
        {
            //
            _buffMap = new BuffDictionary();
            if (Actor.AgentItem == ParserHelper._unknownAgent)
            {
                _buffMap.Finalize(log, Actor.AgentItem, out _trackedBuffs);
                return;
            }
            // Fill in Boon Map
#if DEBUG
            var test = log.CombatData.GetBuffData(Actor.AgentItem).Where(x => !log.Buffs.BuffsByIds.ContainsKey(x.BuffID)).GroupBy(x => x.BuffSkill.Name).ToDictionary(x => x.Key, x => x.ToList());
#endif
            foreach (AbstractBuffEvent buffEvent in log.CombatData.GetBuffData(Actor.AgentItem))
            {
                long buffID = buffEvent.BuffID;
                if (!log.Buffs.BuffsByIds.ContainsKey(buffID))
                {
                    continue;
                }
                Buff buff = log.Buffs.BuffsByIds[buffID];
                _buffMap.Add(log, buff, buffEvent);
            }
            _buffMap.Finalize(log, Actor.AgentItem, out _trackedBuffs);
        }


        private void SetBuffGraphs(ParsedEvtcLog log)
        {
            _buffGraphs = new Dictionary<long, BuffsGraphModel>();
            if (_buffMap == null)
            {
                ComputeBuffMap(log);
            }
            BuffDictionary buffMap = _buffMap;
            long dur = log.FightData.FightEnd;
            int fightDuration = (int)(dur) / 1000;
            var boonPresenceGraph = new BuffsGraphModel(log.Buffs.BuffsByIds[NumberOfBoonsID]);
            var activeCombatMinionsGraph = new BuffsGraphModel(log.Buffs.BuffsByIds[NumberOfActiveCombatMinionsID]);
            BuffsGraphModel numberOfClonesGraph = null;
            var canSummonClones = ProfHelper.CanSummonClones(Actor.Spec);
            if (canSummonClones)
            {
                numberOfClonesGraph = new BuffsGraphModel(log.Buffs.BuffsByIds[NumberOfClonesID]);
            }
            var condiPresenceGraph = new BuffsGraphModel(log.Buffs.BuffsByIds[NumberOfConditionsID]);
            var boonIds = new HashSet<long>(log.Buffs.BuffsByNature[BuffNature.Boon].Select(x => x.ID));
            var condiIds = new HashSet<long>(log.Buffs.BuffsByNature[BuffNature.Condition].Select(x => x.ID));
            // Init status
            _buffDistribution = new CachingCollection<BuffDistribution>(log);
            _buffPresence = new CachingCollection<Dictionary<long, long>>(log);
            foreach (Buff buff in GetTrackedBuffs(log))
            {
                long buffID = buff.ID;
                if (buffMap.TryGetValue(buffID, out List<AbstractBuffEvent> buffEvents) && buffEvents.Count != 0 && !_buffGraphs.ContainsKey(buffID))
                {
                    AbstractBuffSimulator simulator;
                    try
                    {
                        simulator = buff.CreateSimulator(log, false);
                        simulator.Simulate(buffEvents, dur);
                    }
                    catch (EIBuffSimulatorIDException)
                    {
                        // get rid of logs invalid for HasStackIDs false
                        log.UpdateProgressWithCancellationCheck("Failed id based simulation on " + Actor.Character + " for " + buff.Name);
                        buffEvents.RemoveAll(x => !x.IsBuffSimulatorCompliant(log.FightData.FightEnd, false));
                        simulator = buff.CreateSimulator(log, true);
                        simulator.Simulate(buffEvents, dur);
                    }
                    _buffSimulators[buffID] = simulator;
                    bool updateBoonPresence = boonIds.Contains(buffID);
                    bool updateCondiPresence = condiIds.Contains(buffID);
                    var graphSegments = new List<Segment>();
                    foreach (BuffSimulationItem simul in simulator.GenerationSimulation)
                    {
                        // Graph
                        var segment = simul.ToSegment();
                        if (graphSegments.Count == 0)
                        {
                            graphSegments.Add(new Segment(0, segment.Start, 0));
                        }
                        else if (graphSegments.Last().End != segment.Start)
                        {
                            graphSegments.Add(new Segment(graphSegments.Last().End, segment.Start, 0));
                        }
                        graphSegments.Add(segment);
                    }
                    // Graph object creation
                    if (graphSegments.Count > 0)
                    {
                        graphSegments.Add(new Segment(graphSegments.Last().End, dur, 0));
                    }
                    else
                    {
                        graphSegments.Add(new Segment(0, dur, 0));
                    }
                    _buffGraphs[buffID] = new BuffsGraphModel(buff, graphSegments);
                    if (updateBoonPresence || updateCondiPresence)
                    {
                        (updateBoonPresence ? boonPresenceGraph : condiPresenceGraph).MergePresenceInto(_buffGraphs[buffID].BuffChart);
                    }

                }
            }
            _buffGraphs[NumberOfBoonsID] = boonPresenceGraph;
            _buffGraphs[NumberOfConditionsID] = condiPresenceGraph;
            foreach (Minions minions in Actor.GetMinions(log).Values)
            {
                IReadOnlyList<IReadOnlyList<Segment>> segments = minions.GetLifeSpanSegments(log);
                foreach (IReadOnlyList<Segment> minionsSegments in segments)
                {
                    activeCombatMinionsGraph.MergePresenceInto(minionsSegments);
                }
                if (canSummonClones && MesmerHelper.IsClone(minions.ReferenceAgentItem))
                {
                    foreach (IReadOnlyList<Segment> minionsSegments in segments)
                    {
                        numberOfClonesGraph.MergePresenceInto(minionsSegments);
                    }
                }
            }
            if (activeCombatMinionsGraph.BuffChart.Any())
            {
                _buffGraphs[NumberOfActiveCombatMinionsID] = activeCombatMinionsGraph;
            }
            if (canSummonClones && numberOfClonesGraph.BuffChart.Any())
            {
                _buffGraphs[NumberOfClonesID] = numberOfClonesGraph;
            }
        }

        public Dictionary<long, FinalBuffsDictionary> GetBuffsDictionary(ParsedEvtcLog log, long start, long end)
        {
            if (_buffsDictionary == null)
            {
                _buffsDictionary = new CachingCollection<(Dictionary<long, FinalBuffsDictionary>, Dictionary<long, FinalBuffsDictionary>)>(log);
            }
            if (!_buffsDictionary.TryGetValue(start, end, out (Dictionary<long, FinalBuffsDictionary>, Dictionary<long, FinalBuffsDictionary>) value))
            {
                value = ComputeBuffsDictionary(log, start, end);
                _buffsDictionary.Set(start, end, value);
            }
            return value.Item1;
        }

        private (Dictionary<long, FinalBuffsDictionary>, Dictionary<long, FinalBuffsDictionary>) ComputeBuffsDictionary(ParsedEvtcLog log, long start, long end)
        {
            BuffDistribution buffDistribution = GetBuffDistribution(log, start, end);
            var rates = new Dictionary<long, FinalBuffsDictionary>();
            var ratesActive = new Dictionary<long, FinalBuffsDictionary>();
            long duration = end - start;
            long activeDuration = Actor.GetActiveDuration(log, start, end);

            foreach (Buff buff in GetTrackedBuffs(log))
            {
                if (buffDistribution.HasBuffID(buff.ID))
                {
                    (rates[buff.ID], ratesActive[buff.ID]) = FinalBuffsDictionary.GetFinalBuffsDictionary(log, buff, buffDistribution, duration, activeDuration);
                }
            }
            return (rates, ratesActive);
        }


        public IReadOnlyList<Consumable> GetConsumablesList(ParsedEvtcLog log, long start, long end)
        {
            if (_consumeList == null)
            {
                SetConsumablesList(log);
            }
            return _consumeList.Where(x => x.Time >= start && x.Time <= end).ToList();
        }
        private void SetConsumablesList(ParsedEvtcLog log)
        {
            IReadOnlyList<Buff> consumableList = log.Buffs.BuffsByNature[BuffNature.Consumable];
            _consumeList = new List<Consumable>();
            long fightDuration = log.FightData.FightEnd;
            foreach (Buff consumable in consumableList)
            {
                foreach (AbstractBuffEvent c in log.CombatData.GetBuffData(consumable.ID))
                {
                    if (!(c is BuffApplyEvent ba) || AgentItem != ba.To)
                    {
                        continue;
                    }
                    long time = 0;
                    if (!ba.Initial)
                    {
                        time = ba.Time;
                    }
                    if (time <= fightDuration)
                    {
                        Consumable existing = _consumeList.Find(x => x.Time == time && x.Buff.ID == consumable.ID);
                        if (existing != null)
                        {
                            existing.Stack++;
                        }
                        else
                        {
                            _consumeList.Add(new Consumable(consumable, time, ba.AppliedDuration));
                        }
                    }
                }
            }
            _consumeList.Sort((x, y) => x.Time.CompareTo(y.Time));

        }

        ///

        protected static void Add<T>(Dictionary<T, long> dictionary, T key, long value)
        {
            if (dictionary.TryGetValue(key, out long existing))
            {
                dictionary[key] = existing + value;
            }
            else
            {
                dictionary.Add(key, value);
            }
        }

    }
}
