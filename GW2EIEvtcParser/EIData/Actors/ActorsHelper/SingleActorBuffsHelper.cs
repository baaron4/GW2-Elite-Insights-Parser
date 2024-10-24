using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using GW2EIEvtcParser.EIData.BuffSimulators;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    /// <summary> A segment of time with type <see cref="double"/> with inclusive start and inclusive end. </summary>
    using Segment = GenericSegment<double>;

    partial class AbstractSingleActor
    {
        private List<Consumable>? _consumeList;
        // Boons
        private HashSet<Buff>? _trackedBuffs;
        private BuffDictionary? _buffMap;
        private Dictionary<long, BuffsGraphModel>? _buffGraphs;
        private Dictionary<AgentItem, Dictionary<long, BuffsGraphModel>>? _buffGraphsPerAgent;
        private CachingCollection<BuffDistribution>? _buffDistribution;
        private CachingCollection<Dictionary<long, long>>? _buffPresence;
        private CachingCollectionCustom<BuffEnum, (Dictionary<long, FinalActorBuffs> Buffs, Dictionary<long, FinalActorBuffs> ActiveBuffs)>? _buffStats;
        private CachingCollectionCustom<BuffEnum, (Dictionary<long, FinalActorBuffVolumes> Volumes, Dictionary<long, FinalActorBuffVolumes> ActiveVolumes)>? _buffVolumes;
        private CachingCollection<(Dictionary<long, FinalBuffsDictionary> Rates, Dictionary<long, FinalBuffsDictionary> ActiveRates)>? _buffsDictionary;
        private CachingCollection<(Dictionary<long, FinalBuffVolumesDictionary> Rates, Dictionary<long, FinalBuffVolumesDictionary> ActiveRates)>? _buffVolumesDictionary;
        private Dictionary<long, AbstractBuffSimulator>? _buffSimulators;

        public BuffDistribution GetBuffDistribution(ParsedEvtcLog log, long start, long end)
        {
            if (_buffDistribution == null)
            {
                ComputeBuffGraphs(log);
            }

            if (!_buffDistribution.TryGetValue(start, end, out var value))
            {
                value = ComputeBuffDistribution(_buffSimulators!, start, end);
                _buffDistribution.Set(start, end, value);
            }

            return value;
        }

        private static BuffDistribution ComputeBuffDistribution(Dictionary<long, AbstractBuffSimulator> buffSimulators, long start, long end)
        {
            var res = new BuffDistribution(buffSimulators.Count, 8); //TODO(Rennorb) @perf: find capacity dependencies

            foreach (var (buff, simulator) in buffSimulators)
            {
                foreach (BuffSimulationItem simul in simulator.GenerationSimulation)
                {
                    simul.SetBuffDistributionItem(res, start, end, buff);
                }

                foreach (BuffSimulationItemWasted simul in simulator.WasteSimulationResult)
                {
                    simul.SetBuffDistributionItem(res, start, end, buff);
                }

                foreach (BuffSimulationItemOverstack simul in simulator.OverstackSimulationResult)
                {
                    simul.SetBuffDistributionItem(res, start, end, buff);
                }
            }

            return res;
        }

        public IReadOnlyDictionary<long, long> GetBuffPresence(ParsedEvtcLog log, long start, long end)
        {
            if (_buffPresence == null)
            {
                ComputeBuffGraphs(log);
            }

            if (!_buffPresence.TryGetValue(start, end, out var value))
            {
                value = ComputeBuffPresence(_buffSimulators, start, end);
                _buffPresence.Set(start, end, value);
            }
            return value;
        }

        private Dictionary<long, long> ComputeBuffPresence(Dictionary<long, AbstractBuffSimulator> buffSimulators, long start, long end)
        {
            var buffPresence = new Dictionary<long, long>(buffSimulators.Count);
            foreach (KeyValuePair<long, AbstractBuffSimulator> pair in buffSimulators)
            {
                foreach (BuffSimulationItem simul in pair.Value.GenerationSimulation)
                {
                    long value = simul.GetClampedDuration(start, end);
                    if (value != 0)
                    {
                        buffPresence.IncrementValue(pair.Key, value);
                    }
                }
            }
            return buffPresence;
        }

        public IReadOnlyDictionary<long, BuffsGraphModel> GetBuffGraphs(ParsedEvtcLog log)
        {
            if (_buffGraphs == null)
            {
                ComputeBuffGraphs(log);
            }

            return _buffGraphs;
        }

        public IReadOnlyDictionary<long, BuffsGraphModel> GetBuffGraphs(ParsedEvtcLog log, AbstractSingleActor by)
        {
            AgentItem agent = by.AgentItem;
            if (_buffGraphs == null)
            {
                ComputeBuffGraphs(log);
            }

            _buffGraphsPerAgent ??= new(8); //TODO(Rennorb) @perf: find capacity dependencies
            if (!_buffGraphsPerAgent.ContainsKey(agent))
            {
                SetBuffGraphs(log, by);
            }

            return _buffGraphsPerAgent[agent];
        }

        /// <summary>
        /// Checks if a buff is present on the actor. Given buff id must be in the buff simulator, throws <see cref="InvalidOperationException"/> otherwise
        /// </summary>
        public bool HasBuff(ParsedEvtcLog log, long buffId, long time, long window = 0)
        {
            if (!log.Buffs.BuffsByIds.ContainsKey(buffId))
            {
                throw new InvalidOperationException("Buff id must be simulated");
            }

            IReadOnlyDictionary<long, BuffsGraphModel> bgms = GetBuffGraphs(log);
            return bgms.TryGetValue(buffId, out BuffsGraphModel bgm) && bgm.IsPresent(time, window);
        }

        /// <summary>
        /// Checks if a buff is present on the actor and applied by given actor. Given buff id must be in the buff simulator, throws <see cref="InvalidOperationException"/> otherwise
        /// </summary>
        public bool HasBuff(ParsedEvtcLog log, AbstractSingleActor by, long buffId, long time)
        {
            if (!log.Buffs.BuffsByIds.ContainsKey(buffId))
            {
                throw new InvalidOperationException("Buff id must be simulated");
            }

            IReadOnlyDictionary<long, BuffsGraphModel> bgms = GetBuffGraphs(log, by);
            return bgms.TryGetValue(buffId, out BuffsGraphModel bgm) && bgm.IsPresent(time);
        }

        private static readonly Segment _emptySegment = new(long.MinValue, long.MaxValue, 0);

        private static Segment GetBuffStatus(long buffId, long time, IReadOnlyDictionary<long, BuffsGraphModel> bgms)
        {
            return bgms.TryGetValue(buffId, out BuffsGraphModel bgm) ? bgm.GetBuffStatus(time) : _emptySegment;
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
            return bgms.TryGetValue(buffId, out BuffsGraphModel bgm) ? bgm.GetBuffStatus(start, end).ToList() : [ _emptySegment ];
        }

        /// <exception cref="InvalidOperationException"></exception>
        public IReadOnlyList<Segment> GetBuffStatus(ParsedEvtcLog log, long buffId, long start, long end)
        {
            if (!log.Buffs.BuffsByIds.ContainsKey(buffId))
            {
                throw new InvalidOperationException("Buff id must be simulated");
            }
            return GetBuffStatus(buffId, start, end, GetBuffGraphs(log));
        }

        /// <exception cref="InvalidOperationException"></exception>
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
            _buffStats ??= new(log, BuffEnum.Self, 8, 2, 4);  //TODO(Rennorb) @perf: find capacity dependenceis
            if (!_buffStats.TryGetValue(start, end, type, out var pair))
            {
                pair = this.ComputeBuffs(log, start, end, type);
                _buffStats.Set(start, end, type, pair);
            }
            return pair.Buffs;
        }

        public IReadOnlyDictionary<long, FinalActorBuffs> GetActiveBuffs(BuffEnum type, ParsedEvtcLog log, long start, long end)
        {
            _buffStats ??= new(log, BuffEnum.Self, 8, 2, 4); //TODO(Rennorb) @perf: find capacity dependenceis
            if (!_buffStats.TryGetValue(start, end, type, out var value))
            {
                value = this.ComputeBuffs(log, start, end, type);
                _buffStats.Set(start, end, type, value);
            }
            return value.ActiveBuffs;
        }

        public IReadOnlyDictionary<long, FinalActorBuffVolumes> GetBuffVolumes(BuffEnum type, ParsedEvtcLog log, long start, long end)
        {
            _buffVolumes ??= new(log, BuffEnum.Self, 4, 2, 4); //TODO(Rennorb) @perf: find capacity dependenceis
            if (!_buffVolumes.TryGetValue(start, end, type, out var value))
            {
                value = this.ComputeBuffVolumes(log, start, end, type);
                _buffVolumes.Set(start, end, type, value);
            }
            return value.Volumes;
        }

        public IReadOnlyDictionary<long, FinalActorBuffVolumes> GetActiveBuffVolumes(BuffEnum type, ParsedEvtcLog log, long start, long end)
        {
            _buffVolumes ??= new(log, BuffEnum.Self, 4, 2, 4);  //TODO(Rennorb) @perf: find capacity dependenceis
            if (!_buffVolumes.TryGetValue(start, end, type, out var value))
            {
                value = this.ComputeBuffVolumes(log, start, end, type);
                _buffVolumes.Set(start, end, type, value);
            }
            return value.ActiveVolumes;
        }

        public IReadOnlyCollection<Buff> GetTrackedBuffs(ParsedEvtcLog log)
        {
            if (_trackedBuffs == null)
            {
                ComputeBuffMap(log);
            }
            return _trackedBuffs;
        }

        [MemberNotNull(nameof(_buffMap))]
        [MemberNotNull(nameof(_trackedBuffs))]
        internal void ComputeBuffMap(ParsedEvtcLog log)
        {
            _buffMap = new BuffDictionary(64, 256, 32, 1);
            if (this.AgentItem == _unknownAgent)
            {
                _buffMap.Finalize(log, this.AgentItem, out _trackedBuffs);
                return;
            }
            // Fill in Boon Map
#if DEBUG
            var test = log.CombatData.GetBuffDataByDst(this.AgentItem).Where(x => !log.Buffs.BuffsByIds.ContainsKey(x.BuffID)).GroupBy(x => x.BuffSkill.Name).ToDictionary(x => x.Key, x => x.ToList());
            var test2 = log.CombatData.GetBuffDataByDst(this.AgentItem).Where(x => log.Buffs.BuffsByIds.ContainsKey(x.BuffID)).GroupBy(x => x.BuffSkill.Name).ToDictionary(x => x.Key, x => x.ToList());
#endif
            foreach (var buffEvent in log.CombatData.GetBuffDataByDst(this.AgentItem))
            {
                if (!log.Buffs.BuffsByIds.TryGetValue(buffEvent.BuffID, out var buff))
                {
                    continue;
                }

                if (buffEvent.BuffID != SkillIDs.Regeneration)
                {
                    _buffMap.Add(log, buff, buffEvent);
                }
                else
                {
                    _buffMap.AddRegen(log, buff, buffEvent);
                }
            }
            _buffMap.Finalize(log, this.AgentItem, out _trackedBuffs);
        }


        [MemberNotNull(nameof(_buffGraphs))]
        [MemberNotNull(nameof(_buffDistribution))]
        [MemberNotNull(nameof(_buffPresence))]
        [MemberNotNull(nameof(_buffSimulators))]
        internal void ComputeBuffGraphs(ParsedEvtcLog log)
        {
            if (_buffMap == null)
            {
                ComputeBuffMap(log);
            }

            var trackedBuffs = GetTrackedBuffs(log);
            _buffGraphs = new Dictionary<long, BuffsGraphModel>(trackedBuffs.Count + 5);
            var boonPresenceGraph = new BuffsGraphModel(log.Buffs.BuffsByIds[SkillIDs.NumberOfBoons]);
            var activeCombatMinionsGraph = new BuffsGraphModel(log.Buffs.BuffsByIds[SkillIDs.NumberOfActiveCombatMinions]);
            var numberOfClonesGraph = ProfHelper.CanSummonClones(this.Spec) ? new BuffsGraphModel(log.Buffs.BuffsByIds[SkillIDs.NumberOfClones]) : null;
            var numberOfRangerPets = ProfHelper.CanUseRangerPets(this.Spec) ? new BuffsGraphModel(log.Buffs.BuffsByIds[SkillIDs.NumberOfRangerPets]) : null;
            var condiPresenceGraph = new BuffsGraphModel(log.Buffs.BuffsByIds[SkillIDs.NumberOfConditions]);
            var boonIds = new HashSet<long>(log.Buffs.BuffsByClassification[BuffClassification.Boon].Select(x => x.ID));
            var condiIds = new HashSet<long>(log.Buffs.BuffsByClassification[BuffClassification.Condition].Select(x => x.ID));

            // Init status
            _buffDistribution = new(log, 8, 2); //TODO(Rennorb) @perf: find capacity dependencies
            _buffPresence     = new(log, 8, 2); //TODO(Rennorb) @perf: find capacity dependencies
            _buffSimulators   = new(trackedBuffs.Count * 5);

            foreach (Buff buff in trackedBuffs)
            {
                long buffID = buff.ID;
                if (_buffMap.TryGetValue(buffID, out var buffEvents) && buffEvents.Count != 0 && !_buffGraphs.ContainsKey(buffID))
                {
                    AbstractBuffSimulator simulator;
                    try
                    {
                        if (log.CombatData.UseBuffInstanceSimulator && AgentItem.Type == AgentItem.AgentType.NonSquadPlayer)
                        {
                            buffEvents.RemoveAll(x => !x.IsBuffSimulatorCompliant(false));
                            simulator = buff.CreateSimulator(log, true);
                        } 
                        else
                        {
                            simulator = buff.CreateSimulator(log, false);
                        }
                        simulator.Simulate(buffEvents, log.FightData.FightStart, log.FightData.FightEnd);
                    }
                    catch (EIBuffSimulatorIDException e)
                    {
                        // get rid of logs invalid for HasStackIDs false
                        log.UpdateProgressWithCancellationCheck("Parsing: Failed id based simulation on " + this.Character + " for " + buff.Name + " because " + e.Message);
                        buffEvents.RemoveAll(x => !x.IsBuffSimulatorCompliant(false));
                        simulator = buff.CreateSimulator(log, true);
                        simulator.Simulate(buffEvents, log.FightData.FightStart, log.FightData.FightEnd);
                    }
                    _buffSimulators[buffID] = simulator;
                    bool updateBoonPresence = boonIds.Contains(buffID);
                    bool updateCondiPresence = condiIds.Contains(buffID); // move
                    var graphSegments = new List<Segment>(simulator.GenerationSimulation.Count);
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
            foreach (Minions minions in this.GetMinions(log).Values)
            {
                IReadOnlyList<IReadOnlyList<Segment>> segments = minions.GetLifeSpanSegments(log);
                foreach (IReadOnlyList<Segment> minionsSegments in segments)
                {
                    activeCombatMinionsGraph.MergePresenceInto(minionsSegments);
                }
                if (numberOfClonesGraph != null && MesmerHelper.IsClone(minions.ReferenceAgentItem))
                {
                    foreach (IReadOnlyList<Segment> minionsSegments in segments)
                    {
                        numberOfClonesGraph.MergePresenceInto(minionsSegments);
                    }
                }
                if (numberOfRangerPets != null && RangerHelper.IsJuvenilePet(minions.ReferenceAgentItem))
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
            if (numberOfClonesGraph != null && numberOfClonesGraph.BuffChart.Any())
            {
                _buffGraphs[SkillIDs.NumberOfClones] = numberOfClonesGraph;
            }
            if (numberOfRangerPets != null && numberOfRangerPets.BuffChart.Any())
            {
                _buffGraphs[SkillIDs.NumberOfRangerPets] = numberOfRangerPets;
            }
        }

        private void SetBuffGraphs(ParsedEvtcLog log, AbstractSingleActor by)
        {
            var trackedBuffs = GetTrackedBuffs(log);
            var buffGraphs = new Dictionary<long, BuffsGraphModel>(trackedBuffs.Count);
            _buffGraphsPerAgent[by.AgentItem] = buffGraphs;
            BuffDictionary buffMap = _buffMap;
            var boonIds = new HashSet<long>(log.Buffs.BuffsByClassification[BuffClassification.Boon].Select(x => x.ID));
            var condiIds = new HashSet<long>(log.Buffs.BuffsByClassification[BuffClassification.Condition].Select(x => x.ID));
            //
            var boonPresenceGraph = new BuffsGraphModel(log.Buffs.BuffsByIds[SkillIDs.NumberOfBoons]);
            var condiPresenceGraph = new BuffsGraphModel(log.Buffs.BuffsByIds[SkillIDs.NumberOfConditions]);
            //
            foreach (Buff buff in trackedBuffs)
            {
                long buffID = buff.ID;
                if (_buffSimulators.TryGetValue(buff.ID, out AbstractBuffSimulator simulator) && !buffGraphs.ContainsKey(buffID))
                {
                    bool updateBoonPresence = boonIds.Contains(buffID);
                    bool updateCondiPresence = condiIds.Contains(buffID);
                    var graphSegments = new List<Segment>(simulator.GenerationSimulation.Count + 2);
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
            _buffsDictionary ??= new(log, 4, 2); //TODO(Rennorb) @perf: find capacity dependencies
            if (!_buffsDictionary.TryGetValue(start, end, out var value))
            {
                value = ComputeBuffsDictionaries(log, start, end);
                _buffsDictionary.Set(start, end, value);
            }
            return value.Rates;
        }

        public IReadOnlyDictionary<long, FinalBuffsDictionary> GetActiveBuffsDictionary(ParsedEvtcLog log, long start, long end)
        {
            _buffsDictionary ??= new(log, 4, 2); //TODO(Rennorb) @perf: find capacity dependencies
            if (!_buffsDictionary.TryGetValue(start, end, out var value))
            {
                value = ComputeBuffsDictionaries(log, start, end);
                _buffsDictionary.Set(start, end, value);
            }
            return value.ActiveRates;
        }

        public IReadOnlyDictionary<long, FinalBuffVolumesDictionary> GetBuffVolumesDictionary(ParsedEvtcLog log, long start, long end)
        {
            _buffVolumesDictionary ??= new(log, 4, 2); //TODO(Rennorb) @perf: find capacity dependencies
            if (!_buffVolumesDictionary.TryGetValue(start, end, out var value))
            {
                value = ComputeBuffVolumesDictionaries(log, start, end);
                _buffVolumesDictionary.Set(start, end, value);
            }
            return value.Rates;
        }
        public IReadOnlyDictionary<long, FinalBuffVolumesDictionary> GetActiveBuffVolumesDictionary(ParsedEvtcLog log, long start, long end)
        {
            _buffVolumesDictionary ??= new(log, 4, 2); //TODO(Rennorb) @perf: find capacity dependencies
            if (!_buffVolumesDictionary.TryGetValue(start, end, out var value))
            {
                value = ComputeBuffVolumesDictionaries(log, start, end);
                _buffVolumesDictionary.Set(start, end, value);
            }
            return value.ActiveRates;
        }

        private (Dictionary<long, FinalBuffsDictionary> Rates, Dictionary<long, FinalBuffsDictionary> RatesActive) ComputeBuffsDictionaries(ParsedEvtcLog log, long start, long end)
        {
            BuffDistribution buffDistribution = GetBuffDistribution(log, start, end);
            var rates = new Dictionary<long, FinalBuffsDictionary>();
            var ratesActive = new Dictionary<long, FinalBuffsDictionary>();
            long duration = end - start;
            long activeDuration = this.GetActiveDuration(log, start, end);

            foreach (Buff buff in GetTrackedBuffs(log))
            {
                if (buffDistribution.HasBuffID(buff.ID))
                {
                    (rates[buff.ID], ratesActive[buff.ID]) = FinalBuffsDictionary.GetFinalBuffsDictionary(log, buff, buffDistribution, duration, activeDuration);
                }
            }
            return (rates, ratesActive);
        }

        private (Dictionary<long, FinalBuffVolumesDictionary> Rates, Dictionary<long, FinalBuffVolumesDictionary> RatesActive) ComputeBuffVolumesDictionaries(ParsedEvtcLog log, long start, long end)
        {
            var rates = new Dictionary<long, FinalBuffVolumesDictionary>();
            var ratesActive = new Dictionary<long, FinalBuffVolumesDictionary>();

            foreach (Buff buff in GetTrackedBuffs(log))
            {
                (rates[buff.ID], ratesActive[buff.ID]) = FinalBuffVolumesDictionary.GetFinalBuffVolumesDictionary(log, buff, this, start, end);
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

    }
}
