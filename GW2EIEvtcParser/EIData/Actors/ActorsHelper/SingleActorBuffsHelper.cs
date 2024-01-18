using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData.BuffSimulators;
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
        private Dictionary<AgentItem,Dictionary<long, BuffsGraphModel>> _buffGraphsPerAgent { get; set; }
        private CachingCollection<BuffDistribution> _buffDistribution;
        private CachingCollection<Dictionary<long, long>> _buffPresence;
        private CachingCollectionCustom<BuffEnum, Dictionary<long, FinalActorBuffs>[]> _buffStats;
        private CachingCollection<Dictionary<long, FinalBuffsDictionary>[]> _buffsDictionary;
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

        public IReadOnlyDictionary<long, long> GetBuffPresence(ParsedEvtcLog log, long start, long end)
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

        public IReadOnlyDictionary<long, BuffsGraphModel> GetBuffGraphs(ParsedEvtcLog log)
        {
            if (_buffGraphs == null)
            {
                SetBuffGraphs(log);
            }
            return _buffGraphs;
        }

        public IReadOnlyDictionary<long, BuffsGraphModel> GetBuffGraphs(ParsedEvtcLog log, AbstractSingleActor by)
        {
            AgentItem agent = by.AgentItem;
            if (_buffGraphs == null)
            {
                SetBuffGraphs(log);
            } 
            if (_buffGraphsPerAgent == null)
            {
                _buffGraphsPerAgent = new Dictionary<AgentItem, Dictionary<long, BuffsGraphModel>>();
            }
            if (!_buffGraphsPerAgent.ContainsKey(agent))
            {
                SetBuffGraphs(log, by);
            }
            return _buffGraphsPerAgent[agent];
        }

        /// <summary>
        /// Checks if a buff is present on the actor. Given buff id must be in the buff simulator, throws <see cref="InvalidOperationException"/> otherwise
        /// </summary>
        /// <param name="log"></param>
        /// <param name="buffId"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool HasBuff(ParsedEvtcLog log, long buffId, long time, long window = 0)
        {
            if (!log.Buffs.BuffsByIds.ContainsKey(buffId))
            {
                throw new InvalidOperationException("Buff id must be simulated");
            }
            IReadOnlyDictionary<long, BuffsGraphModel> bgms = GetBuffGraphs(log);
            if (bgms.TryGetValue(buffId, out BuffsGraphModel bgm))
            {
                return bgm.IsPresent(time, window);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if a buff is present on the actor and applied by given actor. Given buff id must be in the buff simulator, throws <see cref="InvalidOperationException"/> otherwise
        /// </summary>
        /// <param name="log"></param>
        /// <param name="by"></param>
        /// <param name="buffId"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool HasBuff(ParsedEvtcLog log, AbstractSingleActor by, long buffId, long time)
        {
            if (!log.Buffs.BuffsByIds.ContainsKey(buffId))
            {
                throw new InvalidOperationException("Buff id must be simulated");
            }
            IReadOnlyDictionary<long, BuffsGraphModel> bgms = GetBuffGraphs(log, by);
            if (bgms.TryGetValue(buffId, out BuffsGraphModel bgm))
            {
                return bgm.IsPresent(time);
            }
            else
            {
                return false;
            }
        }

        private static readonly Segment _emptySegment = new Segment(long.MinValue, long.MaxValue, 0);

        private static Segment GetBuffStatus(long buffId, long time, IReadOnlyDictionary<long, BuffsGraphModel> bgms)
        {
            if (bgms.TryGetValue(buffId, out BuffsGraphModel bgm))
            {
                return bgm.GetBuffStatus(time);
            }
            else
            {
                return _emptySegment;
            }
        }

        public Segment GetBuffStatus(ParsedEvtcLog log, long buffId, long time)
        {
            if (!log.Buffs.BuffsByIds.ContainsKey(buffId))
            {
                throw new InvalidOperationException("Buff id must be simulated");
            }
            return GetBuffStatus(buffId, time, GetBuffGraphs(log));
        }

        public Segment GetBuffStatus(ParsedEvtcLog log, AbstractSingleActor by, long buffId, long time)
        {
            if (!log.Buffs.BuffsByIds.ContainsKey(buffId))
            {
                throw new InvalidOperationException("Buff id must be simulated");
            }
            return GetBuffStatus(buffId, time, GetBuffGraphs(log, by));
        }

        private static IReadOnlyList<Segment> GetBuffStatus(long buffId, long start, long end, IReadOnlyDictionary<long, BuffsGraphModel> bgms)
        {
            if (bgms.TryGetValue(buffId, out BuffsGraphModel bgm))
            {
                return bgm.GetBuffStatus(start, end);
            }
            else
            {
                return new List<Segment>()
                {
                    _emptySegment
                };
            }
        }

        public IReadOnlyList<Segment> GetBuffStatus(ParsedEvtcLog log, long buffId, long start, long end)
        {
            if (!log.Buffs.BuffsByIds.ContainsKey(buffId))
            {
                throw new InvalidOperationException("Buff id must be simulated");
            }
            return GetBuffStatus(buffId, start, end, GetBuffGraphs(log));
        }

        public IReadOnlyList<Segment> GetBuffStatus(ParsedEvtcLog log, AbstractSingleActor by, long buffId, long start, long end)
        {
            if (!log.Buffs.BuffsByIds.ContainsKey(buffId))
            {
                throw new InvalidOperationException("Buff id must be simulated");
            }
            return GetBuffStatus(buffId, start, end, GetBuffGraphs(log, by));
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
            if (Actor.AgentItem == _unknownAgent)
            {
                _buffMap.Finalize(log, Actor.AgentItem, out _trackedBuffs);
                return;
            }
            // Fill in Boon Map
#if DEBUG
            var test = log.CombatData.GetBuffDataByDst(Actor.AgentItem).Where(x => !log.Buffs.BuffsByIds.ContainsKey(x.BuffID)).GroupBy(x => x.BuffSkill.Name).ToDictionary(x => x.Key, x => x.ToList());
#endif
            var buffEventsDict = log.CombatData.GetBuffDataByDst(Actor.AgentItem).GroupBy(x => x.BuffID).ToDictionary(x => x.Key, x => x.ToList());
            foreach (KeyValuePair<long, List<AbstractBuffEvent>> buffEventPair in buffEventsDict)
            {
                long buffID = buffEventPair.Key;
                if (!log.Buffs.BuffsByIds.ContainsKey(buffID))
                {
                    continue;
                }
                foreach (AbstractBuffEvent buffEvent in buffEventPair.Value)
                {
                    Buff buff = log.Buffs.BuffsByIds[buffID];
                    if (buffID != SkillIDs.Regeneration)
                    {
                        _buffMap.Add(log, buff, buffEvent);
                    }
                    else
                    {
                        _buffMap.AddRegen(log, buff, buffEvent);
                    }
                }
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
            var boonPresenceGraph = new BuffsGraphModel(log.Buffs.BuffsByIds[SkillIDs.NumberOfBoons]);
            var activeCombatMinionsGraph = new BuffsGraphModel(log.Buffs.BuffsByIds[SkillIDs.NumberOfActiveCombatMinions]);
            BuffsGraphModel numberOfClonesGraph = null;
            bool canSummonClones = ProfHelper.CanSummonClones(Actor.Spec);
            if (canSummonClones)
            {
                numberOfClonesGraph = new BuffsGraphModel(log.Buffs.BuffsByIds[SkillIDs.NumberOfClones]);
            }
            BuffsGraphModel numberOfRangerPets = null;
            bool canUseRangerPets = ProfHelper.CanUseRangerPets(Actor.Spec);
            if (canUseRangerPets)
            {
                numberOfRangerPets = new BuffsGraphModel(log.Buffs.BuffsByIds[SkillIDs.NumberOfRangerPets]);
            }
            var condiPresenceGraph = new BuffsGraphModel(log.Buffs.BuffsByIds[SkillIDs.NumberOfConditions]);
            var boonIds = new HashSet<long>(log.Buffs.BuffsByClassification[BuffClassification.Boon].Select(x => x.ID));
            var condiIds = new HashSet<long>(log.Buffs.BuffsByClassification[BuffClassification.Condition].Select(x => x.ID));
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
                        simulator.Simulate(buffEvents, log.FightData.FightStart, log.FightData.FightEnd);
                    }
                    catch (EIBuffSimulatorIDException e)
                    {
                        // get rid of logs invalid for HasStackIDs false
                        log.UpdateProgressWithCancellationCheck("Failed id based simulation on " + Actor.Character + " for " + buff.Name + " because " + e.Message);
                        buffEvents.RemoveAll(x => !x.IsBuffSimulatorCompliant(false));
                        simulator = buff.CreateSimulator(log, true);
                        simulator.Simulate(buffEvents, log.FightData.FightStart, log.FightData.FightEnd);
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
                            graphSegments.Add(new Segment(log.FightData.FightStart, segment.Start, 0));
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
                        graphSegments.Add(new Segment(graphSegments.Last().End, log.FightData.FightEnd, 0));
                    }
                    else
                    {
                        graphSegments.Add(new Segment(log.FightData.FightStart, log.FightData.FightEnd, 0));
                    }

                    _buffGraphs[buffID] = new BuffsGraphModel(buff, graphSegments);
                    if (updateBoonPresence || updateCondiPresence)
                    {
                        (updateBoonPresence ? boonPresenceGraph : condiPresenceGraph).MergePresenceInto(_buffGraphs[buffID].BuffChart);
                    }

                }
            }
            _buffGraphs[SkillIDs.NumberOfBoons] = boonPresenceGraph;
            _buffGraphs[SkillIDs.NumberOfConditions] = condiPresenceGraph;
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
                if (canUseRangerPets && RangerHelper.IsJuvenilePet(minions.ReferenceAgentItem))
                {
                    foreach (IReadOnlyList<Segment> minionsSegments in segments)
                    {
                        numberOfRangerPets.MergePresenceInto(minionsSegments);
                    }
                }
            }
            if (activeCombatMinionsGraph.BuffChart.Any())
            {
                _buffGraphs[SkillIDs.NumberOfActiveCombatMinions] = activeCombatMinionsGraph;
            }
            if (canSummonClones && numberOfClonesGraph.BuffChart.Any())
            {
                _buffGraphs[SkillIDs.NumberOfClones] = numberOfClonesGraph;
            }
            if (canUseRangerPets && numberOfRangerPets.BuffChart.Any())
            {
                _buffGraphs[SkillIDs.NumberOfRangerPets] = numberOfRangerPets;
            }
        }

        private void SetBuffGraphs(ParsedEvtcLog log, AbstractSingleActor by)
        {
            var buffGraphs = new Dictionary<long, BuffsGraphModel>();
            _buffGraphsPerAgent[by.AgentItem] = buffGraphs;
            BuffDictionary buffMap = _buffMap;
            var boonIds = new HashSet<long>(log.Buffs.BuffsByClassification[BuffClassification.Boon].Select(x => x.ID));
            var condiIds = new HashSet<long>(log.Buffs.BuffsByClassification[BuffClassification.Condition].Select(x => x.ID));
            //
            var boonPresenceGraph = new BuffsGraphModel(log.Buffs.BuffsByIds[SkillIDs.NumberOfBoons]);
            var condiPresenceGraph = new BuffsGraphModel(log.Buffs.BuffsByIds[SkillIDs.NumberOfConditions]);
            //
            foreach (Buff buff in GetTrackedBuffs(log))
            {
                long buffID = buff.ID;
                if (_buffSimulators.TryGetValue(buff.ID, out AbstractBuffSimulator simulator) && !buffGraphs.ContainsKey(buffID))
                {
                    bool updateBoonPresence = boonIds.Contains(buffID);
                    bool updateCondiPresence = condiIds.Contains(buffID);
                    var graphSegments = new List<Segment>();
                    foreach (BuffSimulationItem simul in simulator.GenerationSimulation)
                    {
                        // Graph
                        var segment = simul.ToSegment(by);
                        if (graphSegments.Count == 0)
                        {
                            graphSegments.Add(new Segment(log.FightData.FightStart, segment.Start, 0));
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
                        graphSegments.Add(new Segment(graphSegments.Last().End, log.FightData.FightEnd, 0));
                    }
                    else
                    {
                        graphSegments.Add(new Segment(log.FightData.FightStart, log.FightData.FightEnd, 0));
                    }
                    buffGraphs[buffID] = new BuffsGraphModel(buff, graphSegments);
                    if (updateBoonPresence || updateCondiPresence)
                    {
                        (updateBoonPresence ? boonPresenceGraph : condiPresenceGraph).MergePresenceInto(buffGraphs[buffID].BuffChart);
                    }

                }
            }
            buffGraphs[SkillIDs.NumberOfBoons] = boonPresenceGraph;
            buffGraphs[SkillIDs.NumberOfConditions] = condiPresenceGraph;
        }

        public IReadOnlyDictionary<long, FinalBuffsDictionary> GetBuffsDictionary(ParsedEvtcLog log, long start, long end)
        {
            if (_buffsDictionary == null)
            {
                _buffsDictionary = new CachingCollection<Dictionary<long, FinalBuffsDictionary>[]>(log);
            }
            if (!_buffsDictionary.TryGetValue(start, end, out Dictionary<long, FinalBuffsDictionary>[] value))
            {
                value = ComputeBuffsDictionary(log, start, end);
                _buffsDictionary.Set(start, end, value);
            }
            return value[0];
        }

        public IReadOnlyDictionary<long, FinalBuffsDictionary> GetActiveBuffsDictionary(ParsedEvtcLog log, long start, long end)
        {
            if (_buffsDictionary == null)
            {
                _buffsDictionary = new CachingCollection<Dictionary<long, FinalBuffsDictionary>[]>(log);
            }
            if (!_buffsDictionary.TryGetValue(start, end, out Dictionary<long, FinalBuffsDictionary>[] value))
            {
                value = ComputeBuffsDictionary(log, start, end);
                _buffsDictionary.Set(start, end, value);
            }
            return value[1];
        }

        private Dictionary<long, FinalBuffsDictionary>[] ComputeBuffsDictionary(ParsedEvtcLog log, long start, long end)
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
            return new Dictionary<long, FinalBuffsDictionary>[] { rates, ratesActive };
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
            var consumableList = new List<Buff>(log.Buffs.BuffsByClassification[BuffClassification.Nourishment]);
            consumableList.AddRange(log.Buffs.BuffsByClassification[BuffClassification.Enhancement]);
            consumableList.AddRange(log.Buffs.BuffsByClassification[BuffClassification.OtherConsumable]);
            _consumeList = new List<Consumable>();
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
                    if (time <= log.FightData.FightEnd)
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
