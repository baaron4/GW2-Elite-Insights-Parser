using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.Buff;

namespace GW2EIEvtcParser.EIData
{
    public abstract class AbstractSingleActor : AbstractActor
    {
        public int UniqueID => AgentItem.UniqueID;
        // Boons
        public HashSet<Buff> TrackedBuffs { get; } = new HashSet<Buff>();
        private BuffDictionary _buffMap;
        protected Dictionary<long, BuffsGraphModel> BuffPoints { get; set; }
        private readonly List<BuffDistribution> _buffDistribution = new List<BuffDistribution>();
        private readonly List<Dictionary<long, long>> _buffPresence = new List<Dictionary<long, long>>();
        private List<Dictionary<long, FinalBuffsDictionary>> _buffsDictionary;
        private List<Dictionary<long, FinalBuffsDictionary>> _buffsActiveDictionary;
        // damage list
        private CachingCollection<List<int>> _damageList1S;
        private CachingCollection<List<double>> _breakbarDamageList1S;
        private CachingCollection<List<AbstractHealthDamageEvent>> _hitSelfDamageEventsPerPhasePerTarget;
        private CachingCollection<List<AbstractHealthDamageEvent>> _powerHitSelfDamageEventsPerPhasePerTarget;
        private CachingCollection<List<AbstractHealthDamageEvent>> _conditionHitSelfDamageEventsPerPhasePerTarget;
        //status
        private List<Segment> _healthUpdates { get; set; }
        private List<Segment> _barrierUpdates { get; set; }
        private List<(long start, long end)> _deads;
        private List<(long start, long end)> _downs;
        private List<(long start, long end)> _dcs;
        // Minions
        private Dictionary<long, Minions> _minions;
        // Replay
        protected CombatReplay CombatReplay { get; set; }
        // Statistics
        private CachingCollection<FinalDPS> _dps;
        private CachingCollection<FinalDefenses> _defenses;
        private CachingCollection<FinalGameplayStats> _gameplayStats;
        private List<FinalSupportAll> _supportAll;
        private Dictionary<AbstractSingleActor, List<FinalSupport>> _supportTarget;

        protected AbstractSingleActor(AgentItem agent) : base(agent)
        {
        }

        // Status

        public (List<(long start, long end)>, List<(long start, long end)>, List<(long start, long end)>) GetStatus(ParsedEvtcLog log)
        {
            if (_deads == null)
            {
                _deads = new List<(long start, long end)>();
                _downs = new List<(long start, long end)>();
                _dcs = new List<(long start, long end)>();
                AgentItem.GetAgentStatus(_deads, _downs, _dcs, log.CombatData, log.FightData);
            }
            return (_deads, _downs, _dcs);
        }

        public long GetActiveDuration(ParsedEvtcLog log, long start, long end)
        {
            List<(long start, long end)> dead;
            List<(long start, long end)> down;
            List<(long start, long end)> dc;
            (dead, down, dc) = GetStatus(log);
            return (end - start) -
                dead.Sum(x =>
                {
                    if (x.start <= end && x.end >= start)
                    {
                        long s = Math.Max(x.start, start);
                        long e = Math.Min(x.end, end);
                        return e - s;
                    }
                    return 0;
                }) -
                dc.Sum(x =>
                {
                    if (x.start <= end && x.end >= start)
                    {
                        long s = Math.Max(x.start, start);
                        long e = Math.Min(x.end, end);
                        return e - s;
                    }
                    return 0;
                });
        }

        public abstract string GetIcon();

        public List<Segment> GetHealthUpdates(ParsedEvtcLog log)
        {
            if (_healthUpdates == null)
            {
                _healthUpdates = Segment.FromStates(log.CombatData.GetHealthUpdateEvents(AgentItem).Select(x => x.ToState()).ToList(), 0, log.FightData.FightEnd);
            }
            return _healthUpdates;
        }

        public List<Segment> GetBarrierUpdates(ParsedEvtcLog log)
        {
            if (_barrierUpdates == null)
            {
                _barrierUpdates = Segment.FromStates(log.CombatData.GetBarrierUpdateEvents(AgentItem).Select(x => x.ToState()).ToList(), 0, log.FightData.FightEnd);
            }
            return _barrierUpdates;
        }

        // Minions
        public Dictionary<long, Minions> GetMinions(ParsedEvtcLog log)
        {
            if (_minions == null)
            {
                _minions = new Dictionary<long, Minions>();
                // npcs, species id based
                var combatMinion = log.AgentData.GetAgentByType(AgentItem.AgentType.NPC).Where(x => x.Master != null && x.GetFinalMaster() == AgentItem).ToList();
                var auxMinions = new Dictionary<long, Minions>();
                foreach (AgentItem agent in combatMinion)
                {
                    long id = agent.ID;
                    if (auxMinions.TryGetValue(id, out Minions values))
                    {
                        values.AddMinion(new NPC(agent));
                    }
                    else
                    {
                        auxMinions[id] = new Minions(this, new NPC(agent));
                    }
                }
                foreach (KeyValuePair<long, Minions> pair in auxMinions)
                {
                    if (pair.Value.GetDamageEvents(null, log, 0, log.FightData.FightEnd).Count > 0 || pair.Value.GetCastEvents(log, 0, log.FightData.FightEnd).Any(x => x.SkillId != SkillItem.WeaponSwapId && x.SkillId != SkillItem.MirageCloakDodgeId))
                    {
                        _minions[pair.Value.AgentItem.UniqueID] = pair.Value;
                    }
                }
                // gadget, string based
                var combatGadgetMinion = log.AgentData.GetAgentByType(AgentItem.AgentType.Gadget).Where(x => x.Master != null && x.GetFinalMaster() == AgentItem).ToList();
                var auxGadgetMinions = new Dictionary<string, Minions>();
                foreach (AgentItem agent in combatGadgetMinion)
                {
                    string id = agent.Name;
                    if (auxGadgetMinions.TryGetValue(id, out Minions values))
                    {
                        values.AddMinion(new NPC(agent));
                    }
                    else
                    {
                        auxGadgetMinions[id] = new Minions(this, new NPC(agent));
                    }
                }
                foreach (KeyValuePair<string, Minions> pair in auxGadgetMinions)
                {
                    if (pair.Value.GetDamageEvents(null, log, 0, log.FightData.FightEnd).Count > 0 || pair.Value.GetCastEvents(log, 0, log.FightData.FightEnd).Any(x => x.SkillId != SkillItem.WeaponSwapId && x.SkillId != SkillItem.MirageCloakDodgeId))
                    {
                        _minions[pair.Value.AgentItem.UniqueID] = pair.Value;
                    }
                }
            }
            return _minions;
        }

        // Graph

        public List<int> Get1SDamageList(ParsedEvtcLog log, long start, long end, AbstractActor target)
        {
            if (_damageList1S == null)
            {
                _damageList1S = new CachingCollection<List<int>>(log);
            }
            if (_damageList1S.TryGetValue(start, end, target, out List<int> res))
            {
                return res;
            }
            var dmgList = new List<int>();
            int durationInMS = (int)(end - start);
            int durationInS = durationInMS / 1000;
            List<AbstractHealthDamageEvent> damageEvents = GetDamageEvents(target, log, start, end);
            // fill the graph, full precision
            var dmgListFull = new int[durationInMS + 1];
            int totalTime = 1;
            int totalDamage = 0;
            foreach (AbstractHealthDamageEvent dl in damageEvents)
            {
                int time = (int)(dl.Time - start);
                // fill
                for (; totalTime < time; totalTime++)
                {
                    dmgListFull[totalTime] = totalDamage;
                }
                totalDamage += dl.HealthDamage;
                dmgListFull[totalTime] = totalDamage;
            }
            // fill
            for (; totalTime <= durationInMS; totalTime++)
            {
                dmgListFull[totalTime] = totalDamage;
            }
            //
            dmgList.Add(0);
            for (int i = 1; i <= durationInS; i++)
            {
                dmgList.Add(dmgListFull[1000 * i]);
            }
            if (durationInS * 1000 != durationInMS)
            {
                int lastDamage = dmgListFull[durationInMS];
                dmgList.Add(lastDamage);
            }
            _damageList1S.Set(start, end, target, dmgList);
            return dmgList;
        }

        public List<double> Get1SBreakbarDamageList(ParsedEvtcLog log,long start, long end, AbstractActor target)
        {
            if (!log.CombatData.HasBreakbarDamageData)
            {
                return null;
            }
            if (_breakbarDamageList1S == null)
            {
                _breakbarDamageList1S = new CachingCollection<List<double>>(log);
            }
            if (_breakbarDamageList1S.TryGetValue(start, end, target, out List<double> res))
            {
                return res;
            }
            var brkDmgList = new List<double>();
            int durationInMS = (int)(end - start);
            int durationInS = durationInMS / 1000;     
            List<AbstractBreakbarDamageEvent> breakbarDamageEvents = GetBreakbarDamageEvents(target, log, start, end);
            // fill the graph, full precision
            var brkDmgListFull = new double[durationInMS + 1];
            int totalTime = 1;
            double totalDamage = 0;
            foreach (DirectBreakbarDamageEvent dl in breakbarDamageEvents)
            {
                int time = (int)(dl.Time - start);
                // fill
                for (; totalTime < time; totalTime++)
                {
                    brkDmgListFull[totalTime] = totalDamage;
                }
                totalDamage = Math.Round(totalDamage + dl.BreakbarDamage, 1);
                brkDmgListFull[totalTime] = totalDamage;
            }
            // fill
            for (; totalTime <= durationInMS; totalTime++)
            {
                brkDmgListFull[totalTime] = totalDamage;
            }
            //
            brkDmgList.Add(0);
            for (int i = 1; i <= durationInS; i++)
            {
                brkDmgList.Add(brkDmgListFull[1000 * i]);
            }
            if (durationInS * 1000 != durationInMS)
            {
                double lastDamage = brkDmgListFull[durationInMS];
                brkDmgList.Add(lastDamage);
            }
            _breakbarDamageList1S.Set(start, end, target,brkDmgList);
            return brkDmgList;
        }

        // Buffs
        public BuffDistribution GetBuffDistribution(ParsedEvtcLog log, int phaseIndex)
        {
            if (BuffPoints == null)
            {
                SetBuffStatus(log);
            }
            return _buffDistribution[phaseIndex];
        }

        public Dictionary<long, long> GetBuffPresence(ParsedEvtcLog log, int phaseIndex)
        {
            if (BuffPoints == null)
            {
                SetBuffStatus(log);
            }
            return _buffPresence[phaseIndex];
        }

        public Dictionary<long, BuffsGraphModel> GetBuffGraphs(ParsedEvtcLog log)
        {
            if (BuffPoints == null)
            {
                SetBuffStatus(log);
            }
            return BuffPoints;
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
                return bgm.IsPresent(time, ParserHelper.ServerDelayConstant);
            }
            else
            {
                return false;
            }
        }

        public void ComputeBuffMap(ParsedEvtcLog log)
        {
            if (_buffMap == null)
            {
                //
                _buffMap = new BuffDictionary();
                // Fill in Boon Map
#if DEBUG
                var test = log.CombatData.GetBuffData(AgentItem).Where(x => !log.Buffs.BuffsByIds.ContainsKey(x.BuffID)).GroupBy(x => x.BuffSkill.Name).ToDictionary(x => x.Key, x => x.ToList());
#endif
                foreach (AbstractBuffEvent c in log.CombatData.GetBuffData(AgentItem))
                {
                    long boonId = c.BuffID;
                    if (!_buffMap.ContainsKey(boonId))
                    {
                        if (!log.Buffs.BuffsByIds.ContainsKey(boonId))
                        {
                            continue;
                        }
                        _buffMap.Add(log.Buffs.BuffsByIds[boonId]);
                    }
                    if (!c.IsBuffSimulatorCompliant(log.FightData.FightEnd, log.CombatData.HasStackIDs))
                    {
                        continue;
                    }
                    List<AbstractBuffEvent> loglist = _buffMap[boonId];
                    c.TryFindSrc(log);
                    loglist.Add(c);
                }
                // add buff remove all for each despawn events
                foreach (DespawnEvent dsp in log.CombatData.GetDespawnEvents(AgentItem))
                {
                    foreach (KeyValuePair<long, List<AbstractBuffEvent>> pair in _buffMap)
                    {
                        pair.Value.Add(new BuffRemoveAllEvent(ParserHelper._unknownAgent, AgentItem, dsp.Time, int.MaxValue, log.SkillData.Get(pair.Key), BuffRemoveAllEvent.FullRemoval, int.MaxValue));
                    }
                }
                _buffMap.Sort();
                foreach (KeyValuePair<long, List<AbstractBuffEvent>> pair in _buffMap)
                {
                    TrackedBuffs.Add(log.Buffs.BuffsByIds[pair.Key]);
                }
            }
        }


        protected void SetBuffStatus(ParsedEvtcLog log)
        {
            BuffPoints = new Dictionary<long, BuffsGraphModel>();
            ComputeBuffMap(log);
            BuffDictionary buffMap = _buffMap;
            long dur = log.FightData.FightEnd;
            int fightDuration = (int)(dur) / 1000;
            var boonPresenceGraph = new BuffsGraphModel(log.Buffs.BuffsByIds[NumberOfBoonsID]);
            var activeCombatMinionsGraph = new BuffsGraphModel(log.Buffs.BuffsByIds[NumberOfActiveCombatMinions]);
            var condiPresenceGraph = new BuffsGraphModel(log.Buffs.BuffsByIds[NumberOfConditionsID]);
            var boonIds = new HashSet<long>(log.Buffs.BuffsByNature[BuffNature.Boon].Select(x => x.ID));
            var condiIds = new HashSet<long>(log.Buffs.BuffsByNature[BuffNature.Condition].Select(x => x.ID));
            // Init status
            List<PhaseData> phases = log.FightData.GetPhases(log);
            for (int i = 0; i < phases.Count; i++)
            {
                _buffDistribution.Add(new BuffDistribution());
                _buffPresence.Add(new Dictionary<long, long>());
            }
            foreach (Buff buff in TrackedBuffs)
            {
                long boonid = buff.ID;
                if (buffMap.TryGetValue(boonid, out List<AbstractBuffEvent> logs) && logs.Count != 0)
                {
                    if (BuffPoints.ContainsKey(boonid))
                    {
                        continue;
                    }
                    AbstractBuffSimulator simulator = buff.CreateSimulator(log);
                    simulator.Simulate(logs, dur);
                    simulator.Trim(dur);
                    bool updateBoonPresence = boonIds.Contains(boonid);
                    bool updateCondiPresence = condiIds.Contains(boonid);
                    var graphSegments = new List<Segment>();
                    foreach (BuffSimulationItem simul in simulator.GenerationSimulation)
                    {
                        // Generation
                        for (int i = 0; i < phases.Count; i++)
                        {
                            PhaseData phase = phases[i];
                            long value = simul.GetClampedDuration(phase.Start, phase.End);
                            if (value == 0)
                            {
                                continue;
                            }
                            Add(_buffPresence[i], boonid, value);
                            simul.SetBuffDistributionItem(_buffDistribution[i], phase.Start, phase.End, boonid, log);
                        }
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
                    // Cleanse and Wastes
                    var extraSimulations = new List<AbstractSimulationItem>(simulator.OverstackSimulationResult);
                    extraSimulations.AddRange(simulator.WasteSimulationResult);
                    foreach (AbstractSimulationItem simul in extraSimulations)
                    {
                        for (int i = 0; i < phases.Count; i++)
                        {
                            PhaseData phase = phases[i];
                            simul.SetBuffDistributionItem(_buffDistribution[i], phase.Start, phase.End, boonid, log);
                        }
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
                    BuffPoints[boonid] = new BuffsGraphModel(buff, graphSegments);
                    if (updateBoonPresence || updateCondiPresence)
                    {
                        (updateBoonPresence ? boonPresenceGraph : condiPresenceGraph).MergePresenceInto(BuffPoints[boonid].BuffChart);
                    }

                }
            }
            BuffPoints[NumberOfBoonsID] = boonPresenceGraph;
            BuffPoints[NumberOfConditionsID] = condiPresenceGraph;
            foreach (Minions minions in GetMinions(log).Values)
            {
                foreach (List<Segment> minionsSegments in minions.GetLifeSpanSegments(log))
                {
                    activeCombatMinionsGraph.MergePresenceInto(minionsSegments);
                }
            }
            if (activeCombatMinionsGraph.BuffChart.Any())
            {
                BuffPoints[NumberOfActiveCombatMinions] = activeCombatMinionsGraph;
            }
        }

        public Dictionary<long, FinalBuffsDictionary> GetBuffsDictionary(ParsedEvtcLog log, int phaseIndex)
        {
            if (_buffsDictionary == null)
            {
                SetBuffsDictionary(log);
            }
            return _buffsDictionary[phaseIndex];
        }

        public List<Dictionary<long, FinalBuffsDictionary>> GetBuffsDictionary(ParsedEvtcLog log)
        {
            if (_buffsDictionary == null)
            {
                SetBuffsDictionary(log);
            }
            return _buffsDictionary;
        }

        private void SetBuffsDictionary(ParsedEvtcLog log)
        {
            _buffsDictionary = new List<Dictionary<long, FinalBuffsDictionary>>();
            _buffsActiveDictionary = new List<Dictionary<long, FinalBuffsDictionary>>();
            List<PhaseData> phases = log.FightData.GetPhases(log);
            for (int phaseIndex = 0; phaseIndex < phases.Count; phaseIndex++)
            {
                BuffDistribution buffDistribution = GetBuffDistribution(log, phaseIndex);
                var rates = new Dictionary<long, FinalBuffsDictionary>();
                var ratesActive = new Dictionary<long, FinalBuffsDictionary>();
                _buffsDictionary.Add(rates);
                _buffsActiveDictionary.Add(ratesActive);

                PhaseData phase = phases[phaseIndex];
                long phaseDuration = phase.DurationInMS;
                long activePhaseDuration = GetActiveDuration(log, phase.Start, phase.End);

                foreach (Buff buff in TrackedBuffs)
                {
                    if (buffDistribution.HasBuffID(buff.ID))
                    {
                        (rates[buff.ID], ratesActive[buff.ID]) = FinalBuffsDictionary.GetFinalBuffsDictionary(log, buff, buffDistribution, phaseDuration, activePhaseDuration);
                    }
                }
            }
        }

        //
        protected void SetMovements(ParsedEvtcLog log)
        {
            foreach (AbstractMovementEvent movementEvent in log.CombatData.GetMovementData(AgentItem))
            {
                movementEvent.AddPoint3D(CombatReplay);
            }
        }

        public List<Point3D> GetCombatReplayPolledPositions(ParsedEvtcLog log)
        {
            if (CombatReplay == null)
            {
                InitCombatReplay(log);
            }
            return CombatReplay.PolledPositions;
        }

        protected abstract void InitCombatReplay(ParsedEvtcLog log);

        protected void TrimCombatReplay(ParsedEvtcLog log)
        {
            DespawnEvent despawnCheck = log.CombatData.GetDespawnEvents(AgentItem).LastOrDefault();
            SpawnEvent spawnCheck = log.CombatData.GetSpawnEvents(AgentItem).LastOrDefault();
            DeadEvent deathCheck = log.CombatData.GetDeadEvents(AgentItem).LastOrDefault();
            if (deathCheck != null)
            {
                CombatReplay.Trim(AgentItem.FirstAware, deathCheck.Time);
            }
            else if (despawnCheck != null && (spawnCheck == null || spawnCheck.Time < despawnCheck.Time))
            {
                CombatReplay.Trim(AgentItem.FirstAware, despawnCheck.Time);
            }
            else
            {
                CombatReplay.Trim(AgentItem.FirstAware, AgentItem.LastAware);
            }
        }

        public List<GenericDecoration> GetCombatReplayActors(ParsedEvtcLog log)
        {
            if (!log.CanCombatReplay)
            {
                // no combat replay support on fight
                return new List<GenericDecoration>();
            }
            if (CombatReplay == null)
            {
                InitCombatReplay(log);
            }
            if (CombatReplay.NoActors)
            {
                CombatReplay.NoActors = false;
                if (!IsFakeActor)
                {
                    InitAdditionalCombatReplayData(log);
                }
            }
            return CombatReplay.Decorations;
        }
        protected abstract void InitAdditionalCombatReplayData(ParsedEvtcLog log);

        public abstract AbstractSingleActorSerializable GetCombatReplayJSON(CombatReplayMap map, ParsedEvtcLog log);

        // Cast logs
        public override List<AbstractCastEvent> GetCastEvents(ParsedEvtcLog log, long start, long end)
        {
            if (CastEvents == null)
            {
                SetCastEvents(log);
            }
            return CastEvents.Where(x => x.Time >= start && x.Time <= end).ToList();

        }
        public override List<AbstractCastEvent> GetIntersectingCastEvents(ParsedEvtcLog log, long start, long end)
        {
            if (CastEvents == null)
            {
                SetCastEvents(log);
            }
            return CastEvents.Where(x => KeepIntersectingCastLog(x, start, end)).ToList();

        }
        protected void SetCastEvents(ParsedEvtcLog log)
        {
            CastEvents = new List<AbstractCastEvent>(log.CombatData.GetAnimatedCastData(AgentItem));
            CastEvents.AddRange(log.CombatData.GetInstantCastData(AgentItem));
            foreach (WeaponSwapEvent wepSwap in log.CombatData.GetWeaponSwapData(AgentItem))
            {
                if (CastEvents.Count > 0 && (wepSwap.Time - CastEvents.Last().Time) < ParserHelper.ServerDelayConstant && CastEvents.Last().SkillId == SkillItem.WeaponSwapId)
                {
                    CastEvents[CastEvents.Count - 1] = wepSwap;
                }
                else
                {
                    CastEvents.Add(wepSwap);
                }
            }
            CastEvents.Sort((x, y) =>
            {
                int compare = x.Time.CompareTo(y.Time);
                if (compare == 0 && x.SkillId != y.SkillId)
                {
                    if (y.Skill.IsSwap)
                    {
                        return 1;
                    }
                    if (x.Skill.IsSwap)
                    {
                        return -1;
                    }
                }
                return compare;
            });
        }

        // DPS Stats

        public FinalDPS GetDPS(AbstractSingleActor target, ParsedEvtcLog log,  long start, long end)
        {
            if (_dps == null)
            {
                _dps = new CachingCollection<FinalDPS>(log);
            }
            if (!_dps.TryGetValue(start, end, target, out FinalDPS value)) 
            {
                value = new FinalDPS(log, start, end, this, target);
                _dps.Set(start, end, target, value);
            }
            return value;
        }

        public FinalDPS GetDPS(ParsedEvtcLog log, long start, long end)
        {
            return GetDPS(null, log, start, end);
        }

        // Defense Stats

        public FinalDefenses GetDefenses(AbstractSingleActor target, ParsedEvtcLog log,  long start, long end)
        {
            if (_defenses == null)
            {
                _defenses = new CachingCollection<FinalDefenses>(log);
            }
            if (!_defenses.TryGetValue(start, end, target, out FinalDefenses value))
            {
                value = target != null ? new FinalDefenses(log, start, end, this, target) : new FinalDefensesAll(log, start, end, this);
                _defenses.Set(start, end, target, value);
            }
            return value;
        }

        public FinalDefensesAll GetDefenses(ParsedEvtcLog log, long start, long end)
        {
            return GetDefenses(null, log, start, end) as FinalDefensesAll;
        }

        // Gameplay Stats

        public FinalGameplayStatsAll GetGameplayStats(ParsedEvtcLog log, long start, long end)
        {
            return GetGameplayStats(null, log, start, end) as FinalGameplayStatsAll;
        }

        public FinalGameplayStats GetGameplayStats(AbstractSingleActor target, ParsedEvtcLog log, long start, long end)
        {
            if (_gameplayStats == null)
            {
                _gameplayStats = new CachingCollection<FinalGameplayStats>(log);
            }
            if (!_gameplayStats.TryGetValue(start, end, target, out FinalGameplayStats value))
            {
                value = target != null ? new FinalGameplayStats(log, start, end, this, target) : new FinalGameplayStatsAll(log, start, end, this);
                _gameplayStats.Set(start, end, target, value);
            }
            return value;
        }

        // Support stats
        public FinalSupportAll GetSupport(ParsedEvtcLog log, int phaseIndex)
        {
            return GetSupport(log)[phaseIndex];
        }

        public List<FinalSupportAll> GetSupport(ParsedEvtcLog log)
        {
            if (_supportAll == null)
            {
                _supportAll = new List<FinalSupportAll>();
                List<PhaseData> phases = log.FightData.GetPhases(log);
                for (int phaseIndex = 0; phaseIndex < phases.Count; phaseIndex++)
                {
                    PhaseData phase = phases[phaseIndex];
                    var final = new FinalSupportAll(log, phase, this);
                    _supportAll.Add(final);
                }
            }
            return _supportAll;
        }

        public FinalSupport GetSupport(ParsedEvtcLog log, AbstractSingleActor target, int phaseIndex)
        {
            return GetSupport(log, target)[phaseIndex];
        }

        public List<FinalSupport> GetSupport(ParsedEvtcLog log, AbstractSingleActor target)
        {
            if (target == null)
            {
                return new List<FinalSupport>(GetSupport(log));
            }
            if (_supportTarget == null)
            {
                _supportTarget = new Dictionary<AbstractSingleActor, List<FinalSupport>>();
            }
            if (_supportTarget.TryGetValue(target, out List<FinalSupport> list))
            {
                return list;
            }
            _supportTarget[target] = new List<FinalSupport>();
            foreach (PhaseData phase in log.FightData.GetPhases(log))
            {
                _supportTarget[target].Add(new FinalSupport(log, phase, this, target));
            }
            return _supportTarget[target];
        }


        // Damage logs
        public override List<AbstractHealthDamageEvent> GetDamageEvents(AbstractActor target, ParsedEvtcLog log, long start, long end)
        {
            if (DamageEvents == null)
            {
                DamageEvents = new List<AbstractHealthDamageEvent>();
                DamageEvents.AddRange(log.CombatData.GetDamageData(AgentItem).Where(x => x.IFF != ArcDPSEnums.IFF.Friend));
                Dictionary<long, Minions> minionsList = GetMinions(log);
                foreach (Minions mins in minionsList.Values)
                {
                    DamageEvents.AddRange(mins.GetDamageEvents(null, log, 0, log.FightData.FightEnd));
                }
                DamageEvents.Sort((x, y) => x.Time.CompareTo(y.Time));
                DamageEventByDst = DamageEvents.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            }
            if (target != null)
            {
                if (DamageEventByDst.TryGetValue(target.AgentItem, out List<AbstractHealthDamageEvent> list))
                {
                    return list.Where(x => x.Time >= start && x.Time <= end).ToList();
                }
                else
                {
                    return new List<AbstractHealthDamageEvent>();
                }
            }
            return DamageEvents.Where(x => x.Time >= start && x.Time <= end).ToList();
        }

        public List<AbstractHealthDamageEvent> GetJustActorDamageEvents(AbstractActor target, ParsedEvtcLog log, long start, long end)
        {
            return GetDamageEvents(target, log, start, end).Where(x => x.From == AgentItem).ToList();
        }

        public List<AbstractBreakbarDamageEvent> GetJustActorBreakbarDamageEvents(AbstractActor target, ParsedEvtcLog log, long start, long end)
        {
            return GetBreakbarDamageEvents(target, log, start, end).Where(x => x.From == AgentItem).ToList();
        }

        public override List<AbstractBreakbarDamageEvent> GetBreakbarDamageEvents(AbstractActor target, ParsedEvtcLog log, long start, long end)
        {
            if (BreakbarDamageEvents == null)
            {
                BreakbarDamageEvents = new List<AbstractBreakbarDamageEvent>();
                BreakbarDamageEvents.AddRange(log.CombatData.GetBreakbarDamageData(AgentItem).Where(x => x.IFF != ArcDPSEnums.IFF.Friend));
                Dictionary<long, Minions> minionsList = GetMinions(log);
                foreach (Minions mins in minionsList.Values)
                {
                    BreakbarDamageEvents.AddRange(mins.GetBreakbarDamageEvents(null, log, 0, log.FightData.FightEnd));
                }
                BreakbarDamageEvents.Sort((x, y) => x.Time.CompareTo(y.Time));
                BreakbarDamageEventsByDst = BreakbarDamageEvents.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            }
            if (target != null)
            {
                if (BreakbarDamageEventsByDst.TryGetValue(target.AgentItem, out List<AbstractBreakbarDamageEvent> list))
                {
                    return list.Where(x => x.Time >= start && x.Time <= end).ToList();
                }
                else
                {
                    return new List<AbstractBreakbarDamageEvent>();
                }
            }
            return BreakbarDamageEvents.Where(x => x.Time >= start && x.Time <= end).ToList();
        }

        public override List<AbstractHealthDamageEvent> GetDamageTakenEvents(AbstractActor target, ParsedEvtcLog log, long start, long end)
        {
            if (DamageTakenEvents == null)
            {
                DamageTakenEvents = new List<AbstractHealthDamageEvent>();
                DamageTakenEvents.AddRange(log.CombatData.GetDamageTakenData(AgentItem));
                DamageTakenEventsBySrc = DamageTakenEvents.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
            }
            if (target != null)
            {
                if (DamageTakenEventsBySrc.TryGetValue(target.AgentItem, out List<AbstractHealthDamageEvent> list))
                {
                    long targetStart = target.FirstAware;
                    long targetEnd = target.LastAware;
                    return list.Where(x => x.Time >= start && x.Time >= targetStart && x.Time <= end && x.Time <= targetEnd).ToList();
                }
                else
                {
                    return new List<AbstractHealthDamageEvent>();
                }
            }
            return DamageTakenEvents.Where(x => x.Time >= start && x.Time <= end).ToList();
        }

        public override List<AbstractBreakbarDamageEvent> GetBreakbarDamageTakenEvents(AbstractActor target, ParsedEvtcLog log, long start, long end)
        {
            if (BreakbarDamageTakenEvents == null)
            {
                BreakbarDamageTakenEvents = new List<AbstractBreakbarDamageEvent>();
                BreakbarDamageTakenEvents.AddRange(log.CombatData.GetBreakbarDamageTakenData(AgentItem));
                BreakbarDamageTakenEventsBySrc = BreakbarDamageTakenEvents.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
            }
            if (target != null)
            {
                if (BreakbarDamageTakenEventsBySrc.TryGetValue(target.AgentItem, out List<AbstractBreakbarDamageEvent> list))
                {
                    long targetStart = target.FirstAware;
                    long targetEnd = target.LastAware;
                    return list.Where(x => x.Time >= start && x.Time >= targetStart && x.Time <= end && x.Time <= targetEnd).ToList();
                }
                else
                {
                    return new List<AbstractBreakbarDamageEvent>();
                }
            }
            return BreakbarDamageTakenEvents.Where(x => x.Time >= start && x.Time <= end).ToList();
        }

        /// <summary>
        /// cached method for damage modifiers
        /// </summary>
        internal List<AbstractHealthDamageEvent> GetJustActorHitDamageEvents(AbstractActor target, ParsedEvtcLog log, long start, long end)
        {
            if (_hitSelfDamageEventsPerPhasePerTarget == null)
            {
                _hitSelfDamageEventsPerPhasePerTarget = new CachingCollection<List<AbstractHealthDamageEvent>>(log);
            }
            if (!_hitSelfDamageEventsPerPhasePerTarget.TryGetValue(start, end, target, out List<AbstractHealthDamageEvent> dls))
            {
                dls = GetHitDamageEvents(target, log, start, end).Where(x => x.From == AgentItem).ToList();
                _hitSelfDamageEventsPerPhasePerTarget.Set(start, end, target, dls);
            }
            return dls;
        }

        internal List<AbstractHealthDamageEvent> GetJustActorConditionHitDamageEvents(AbstractActor target, ParsedEvtcLog log, long start, long end)
        {
            if (_conditionHitSelfDamageEventsPerPhasePerTarget == null)
            {
                _conditionHitSelfDamageEventsPerPhasePerTarget = new CachingCollection<List<AbstractHealthDamageEvent>>(log);
            }
            if (!_conditionHitSelfDamageEventsPerPhasePerTarget.TryGetValue(start, end, target, out List<AbstractHealthDamageEvent> dls))
            {
                dls = GetJustActorHitDamageEvents(target, log, start, end).Where(x => x.IsCondi(log)).ToList();
                _conditionHitSelfDamageEventsPerPhasePerTarget.Set(start, end, target, dls);
            }
            return dls;
        }

        internal List<AbstractHealthDamageEvent> GetJustActorPowerHitDamageEvents(AbstractActor target, ParsedEvtcLog log, long start, long end)
        {
            if (_powerHitSelfDamageEventsPerPhasePerTarget == null)
            {
                _powerHitSelfDamageEventsPerPhasePerTarget = new CachingCollection<List<AbstractHealthDamageEvent>>(log);
            }
            if (!_powerHitSelfDamageEventsPerPhasePerTarget.TryGetValue(start, end, target, out List<AbstractHealthDamageEvent> dls))
            {
                dls = GetJustActorHitDamageEvents(target, log, start, end).Where(x => !x.IsCondi(log)).ToList();
                _powerHitSelfDamageEventsPerPhasePerTarget.Set(start, end, target, dls);
            }
            return dls;
        }
    }
}
