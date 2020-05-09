using System.Collections.Generic;
using System.Linq;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using static GW2EIParser.EIData.Buff;

namespace GW2EIParser.EIData
{
    public abstract class AbstractSingleActor : AbstractActor
    {
        // Boons
        public HashSet<Buff> TrackedBuffs { get; } = new HashSet<Buff>();
        private BuffDictionary _buffMap;
        protected Dictionary<long, BuffsGraphModel> BuffPoints { get; set; }
        private readonly List<BuffDistribution> _boonDistribution = new List<BuffDistribution>();
        private readonly List<Dictionary<long, long>> _buffPresence = new List<Dictionary<long, long>>();
        private List<Dictionary<long, FinalBuffsDictionary>> _buffsDictionary;
        private List<Dictionary<long, FinalBuffsDictionary>> _buffsActiveDictionary;
        // damage list
        private readonly Dictionary<int, List<int>> _damageList1S = new Dictionary<int, List<int>>();
        private readonly Dictionary<PhaseData, Dictionary<AbstractActor, List<AbstractDamageEvent>>> _selfDamageLogsPerPhasePerTarget = new Dictionary<PhaseData, Dictionary<AbstractActor, List<AbstractDamageEvent>>>();
        //status
        protected List<double[]> HealthUpdates { get; } = new List<double[]>();
        private List<(long start, long end)> _deads;
        private List<(long start, long end)> _downs;
        private List<(long start, long end)> _dcs;
        // Minions
        private Dictionary<long, Minions> _minions;
        // Replay
        protected CombatReplay CombatReplay { get; set; }
        // Statistics
        private List<FinalDPS> _dpsAll;
        private Dictionary<AbstractSingleActor, List<FinalDPS>> _dpsTarget;
        private List<FinalDefensesAll> _defenses;
        private Dictionary<AbstractSingleActor, List<FinalDefenses>> _defensesTarget;
        private Dictionary<AbstractSingleActor, List<FinalGameplayStats>> _statsTarget;
        private List<FinalGameplayStatsAll> _statsAll;
        private List<FinalSupportAll> _supportAll;
        private Dictionary<AbstractSingleActor, List<FinalSupport>> _supportTarget;

        protected AbstractSingleActor(AgentItem agent) : base(agent)
        {
        }

        // Status

        public (List<(long start, long end)>, List<(long start, long end)>, List<(long start, long end)>) GetStatus(ParsedLog log)
        {
            if (_deads == null)
            {
                _deads = new List<(long start, long end)>();
                _downs = new List<(long start, long end)>();
                _dcs = new List<(long start, long end)>();
                AgentItem.GetAgentStatus(_deads, _downs, _dcs, log);
            }
            return (_deads, _downs, _dcs);
        }

        // Minions
        public Dictionary<long, Minions> GetMinions(ParsedLog log)
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
                    if (pair.Value.GetDamageLogs(null, log, 0, log.FightData.FightEnd).Count > 0 || pair.Value.GetCastLogs(log, 0, log.FightData.FightEnd).Count > 0)
                    {
                        _minions[pair.Value.AgentItem.UniqueID.GetHashCode()] = pair.Value;
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
                    if (pair.Value.GetDamageLogs(null, log, 0, log.FightData.FightEnd).Count > 0 || pair.Value.GetCastLogs(log, 0, log.FightData.FightEnd).Count > 0)
                    {
                        _minions[pair.Value.AgentItem.UniqueID.GetHashCode()] = pair.Value;
                    }
                }
            }
            return _minions;
        }

        // Graph

        public List<int> Get1SDamageList(ParsedLog log, int phaseIndex, PhaseData phase, AbstractActor target)
        {
            ulong targetId = target != null ? target.Agent : 0;
            int id = (phaseIndex + "_" + targetId + "_1S").GetHashCode();
            if (_damageList1S.TryGetValue(id, out List<int> res))
            {
                return res;
            }
            var dmgList = new List<int>();
            List<AbstractDamageEvent> damageLogs = GetDamageLogs(target, log, phase);
            // fill the graph, full precision
            var dmgListFull = new List<int>();
            for (int i = 0; i <= phase.DurationInMS; i++)
            {
                dmgListFull.Add(0);
            }
            int totalTime = 1;
            int totalDamage = 0;
            foreach (AbstractDamageEvent dl in damageLogs)
            {
                int time = (int)(dl.Time - phase.Start);
                // fill
                for (; totalTime < time; totalTime++)
                {
                    dmgListFull[totalTime] = totalDamage;
                }
                totalDamage += dl.Damage;
                dmgListFull[totalTime] = totalDamage;
            }
            // fill
            for (; totalTime <= phase.DurationInMS; totalTime++)
            {
                dmgListFull[totalTime] = totalDamage;
            }
            //
            dmgList.Add(0);
            for (int i = 1; i <= phase.DurationInS; i++)
            {
                dmgList.Add(dmgListFull[1000 * i]);
            }
            if (phase.DurationInS * 1000 != phase.DurationInMS)
            {
                int lastDamage = dmgListFull[(int)phase.DurationInMS];
                dmgList.Add(lastDamage);
            }
            _damageList1S[id] = dmgList;
            return dmgList;
        }

        protected void Fill1SHPGraph(ParsedLog log, List<PhaseData> phases, List<HealthUpdateEvent> hpEvents)
        {
            // fill the graph, full precision
            var listFull = new List<double>();
            for (int i = 0; i <= phases[0].DurationInMS; i++)
            {
                listFull.Add(100.0);
            }
            int totalTime = 0;
            double curHealth = 100.0; foreach (HealthUpdateEvent e in hpEvents)
            {
                int time = (int)e.Time;
                if (time < 0)
                {
                    continue;
                }
                if (time > phases[0].DurationInMS)
                {
                    break;
                }
                for (; totalTime < time; totalTime++)
                {
                    listFull[totalTime] = curHealth;
                }
                curHealth = e.HPPercent;
                listFull[time] = curHealth;
            }
            curHealth = hpEvents.Count > 0 ? hpEvents.Last().HPPercent : curHealth;
            // fill
            for (; totalTime <= phases[0].DurationInMS; totalTime++)
            {
                listFull[totalTime] = curHealth;
            }
            foreach (PhaseData phase in phases)
            {
                int seconds = (int)phase.DurationInS;
                bool needsLastPoint = seconds * 1000 != phase.DurationInMS;
                double[] hps = new double[seconds + (needsLastPoint ? +2 : 1)];
                int time = (int)phase.Start;
                int i;
                for (i = 0; i <= seconds; i++)
                {
                    hps[i] = listFull[time];
                    time += 1000;
                }
                if (needsLastPoint)
                {
                    hps[i] = listFull[(int)phase.End];
                }
                HealthUpdates.Add(hps);
            }
        }

        public abstract List<double[]> Get1SHealthGraph(ParsedLog log);

        // Buffs
        public BuffDistribution GetBuffDistribution(ParsedLog log, int phaseIndex)
        {
            if (BuffPoints == null)
            {
                SetBuffStatus(log);
            }
            return _boonDistribution[phaseIndex];
        }

        public Dictionary<long, long> GetBuffPresence(ParsedLog log, int phaseIndex)
        {
            if (BuffPoints == null)
            {
                SetBuffStatus(log);
            }
            return _buffPresence[phaseIndex];
        }

        public Dictionary<long, BuffsGraphModel> GetBuffGraphs(ParsedLog log)
        {
            if (BuffPoints == null)
            {
                SetBuffStatus(log);
            }
            return BuffPoints;
        }

        public void ComputeBuffMap(ParsedLog log)
        {
            if (_buffMap == null)
            {
                //
                _buffMap = new BuffDictionary();
                // Fill in Boon Map
#if DEBUG
                var test = log.CombatData.GetBuffDataByDst(AgentItem).Where(x => !log.Buffs.BuffsByIds.ContainsKey(x.BuffID)).GroupBy(x => x.BuffSkill.Name).ToDictionary(x => x.Key, x => x.ToList());
#endif
                foreach (AbstractBuffEvent c in log.CombatData.GetBuffDataByDst(AgentItem))
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
                        pair.Value.Add(new BuffRemoveAllEvent(GeneralHelper.UnknownAgent, AgentItem, dsp.Time, int.MaxValue, log.SkillData.Get(pair.Key), BuffRemoveAllEvent.FullRemoval, int.MaxValue));
                    }
                }
                _buffMap.Sort();
                foreach (KeyValuePair<long, List<AbstractBuffEvent>> pair in _buffMap)
                {
                    TrackedBuffs.Add(log.Buffs.BuffsByIds[pair.Key]);
                }
            }
        }


        protected void SetBuffStatus(ParsedLog log)
        {
            BuffPoints = new Dictionary<long, BuffsGraphModel>();
            ComputeBuffMap(log);
            BuffDictionary buffMap = _buffMap;
            long dur = log.FightData.FightEnd;
            int fightDuration = (int)(dur) / 1000;
            var boonPresenceGraph = new BuffsGraphModel(log.Buffs.BuffsByIds[ProfHelper.NumberOfBoonsID]);
            var condiPresenceGraph = new BuffsGraphModel(log.Buffs.BuffsByIds[ProfHelper.NumberOfConditionsID]);
            var boonIds = new HashSet<long>(log.Buffs.BuffsByNature[BuffNature.Boon].Select(x => x.ID));
            var condiIds = new HashSet<long>(log.Buffs.BuffsByNature[BuffNature.Condition].Select(x => x.ID));
            InitBuffStatusData(log);
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
                    var graphSegments = new List<BuffSegment>();
                    foreach (BuffSimulationItem simul in simulator.GenerationSimulation)
                    {
                        SetBuffStatusGenerationData(log, simul, boonid);
                        BuffSegment segment = simul.ToSegment();
                        if (graphSegments.Count == 0)
                        {
                            graphSegments.Add(new BuffSegment(0, segment.Start, 0));
                        }
                        else if (graphSegments.Last().End != segment.Start)
                        {
                            graphSegments.Add(new BuffSegment(graphSegments.Last().End, segment.Start, 0));
                        }
                        graphSegments.Add(segment);
                    }
                    SetBuffStatusCleanseWasteData(log, simulator, boonid);
                    if (graphSegments.Count > 0)
                    {
                        graphSegments.Add(new BuffSegment(graphSegments.Last().End, dur, 0));
                    }
                    else
                    {
                        graphSegments.Add(new BuffSegment(0, dur, 0));
                    }
                    BuffPoints[boonid] = new BuffsGraphModel(buff, graphSegments);
                    if (updateBoonPresence || updateCondiPresence)
                    {
                        List<BuffSegment> segmentsToFill = updateBoonPresence ? boonPresenceGraph.BuffChart : condiPresenceGraph.BuffChart;
                        bool firstPass = segmentsToFill.Count == 0;
                        foreach (BuffSegment seg in BuffPoints[boonid].BuffChart)
                        {
                            long start = seg.Start;
                            long end = seg.End;
                            int value = seg.Value > 0 ? 1 : 0;
                            if (firstPass)
                            {
                                segmentsToFill.Add(new BuffSegment(start, end, value));
                            }
                            else
                            {
                                for (int i = 0; i < segmentsToFill.Count; i++)
                                {
                                    BuffSegment curSeg = segmentsToFill[i];
                                    long curEnd = curSeg.End;
                                    long curStart = curSeg.Start;
                                    int curVal = curSeg.Value;
                                    if (curStart > end)
                                    {
                                        break;
                                    }
                                    if (curEnd < start)
                                    {
                                        continue;
                                    }
                                    if (end <= curEnd)
                                    {
                                        curSeg.End = start;
                                        segmentsToFill.Insert(i + 1, new BuffSegment(start, end, curVal + value));
                                        segmentsToFill.Insert(i + 2, new BuffSegment(end, curEnd, curVal));
                                        break;
                                    }
                                    else
                                    {
                                        curSeg.End = start;
                                        segmentsToFill.Insert(i + 1, new BuffSegment(start, curEnd, curVal + value));
                                        start = curEnd;
                                        i++;
                                    }
                                }
                            }
                        }
                        if (updateBoonPresence)
                        {
                            boonPresenceGraph.FuseSegments();
                        }
                        else
                        {
                            condiPresenceGraph.FuseSegments();
                        }
                    }

                }
            }
            BuffPoints[ProfHelper.NumberOfBoonsID] = boonPresenceGraph;
            BuffPoints[ProfHelper.NumberOfConditionsID] = condiPresenceGraph;
        }

        private void InitBuffStatusData(ParsedLog log)
        {
            List<PhaseData> phases = log.FightData.GetPhases(log);
            for (int i = 0; i < phases.Count; i++)
            {
                _boonDistribution.Add(new BuffDistribution());
                _buffPresence.Add(new Dictionary<long, long>());
            }
        }

        private void SetBuffStatusCleanseWasteData(ParsedLog log, AbstractBuffSimulator simulator, long boonid)
        {
            List<PhaseData> phases = log.FightData.GetPhases(log);
            var extraSimulations = new List<AbstractSimulationItem>(simulator.OverstackSimulationResult);
            extraSimulations.AddRange(simulator.WasteSimulationResult);
            foreach (AbstractSimulationItem simul in extraSimulations)
            {
                for (int i = 0; i < phases.Count; i++)
                {
                    PhaseData phase = phases[i];
                    simul.SetBuffDistributionItem(_boonDistribution[i], phase.Start, phase.End, boonid, log);
                }
            }
        }

        private void SetBuffStatusGenerationData(ParsedLog log, BuffSimulationItem simul, long boonid)
        {
            List<PhaseData> phases = log.FightData.GetPhases(log);
            for (int i = 0; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                Add(_buffPresence[i], boonid, simul.GetClampedDuration(phase.Start, phase.End));
                simul.SetBuffDistributionItem(_boonDistribution[i], phase.Start, phase.End, boonid, log);
            }
        }


        public Dictionary<long, FinalBuffsDictionary> GetBuffsDictionary(ParsedLog log, int phaseIndex)
        {
            if (_buffsDictionary == null)
            {
                SetBuffsDictionary(log);
            }
            return _buffsDictionary[phaseIndex];
        }

        public List<Dictionary<long, FinalBuffsDictionary>> GetBuffsDictionary(ParsedLog log)
        {
            if (_buffsDictionary == null)
            {
                SetBuffsDictionary(log);
            }
            return _buffsDictionary;
        }

        private void SetBuffsDictionary(ParsedLog log)
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
                long activePhaseDuration = phase.GetActorActiveDuration(this, log);

                foreach (Buff buff in TrackedBuffs)
                {
                    if (buffDistribution.ContainsKey(buff.ID))
                    {
                        (rates[buff.ID], ratesActive[buff.ID]) = FinalBuffsDictionary.GetFinalBuffsDictionary(log, buff, buffDistribution, phaseDuration, activePhaseDuration);
                    }
                }
            }
        }

        //
        protected void SetMovements(ParsedLog log)
        {
            foreach (AbstractMovementEvent movementEvent in log.CombatData.GetMovementData(AgentItem))
            {
                movementEvent.AddPoint3D(CombatReplay);
            }
        }

        public List<int> GetCombatReplayTimes(ParsedLog log)
        {
            if (CombatReplay == null)
            {
                InitCombatReplay(log);
            }
            return CombatReplay.Times;
        }

        public List<Point3D> GetCombatReplayPolledPositions(ParsedLog log)
        {
            if (CombatReplay == null)
            {
                InitCombatReplay(log);
            }
            return CombatReplay.PolledPositions;
        }

        public List<Point3D> GetCombatReplayActivePositions(ParsedLog log)
        {
            if (CombatReplay == null)
            {
                InitCombatReplay(log);
            }
            (List<(long start, long end)> deads, _ , List<(long start, long end)> dcs) = GetStatus(log);
            return CombatReplay.GetActivePositions(deads, dcs);
        }

        protected abstract void InitCombatReplay(ParsedLog log);

        protected void TrimCombatReplay(ParsedLog log)
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

        public List<GenericDecoration> GetCombatReplayActors(ParsedLog log)
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


        public int GetCombatReplayID(ParsedLog log)
        {
            if (CombatReplay == null)
            {
                InitCombatReplay(log);
            }
            return AgentItem.UniqueID.GetHashCode();
        }
        protected abstract void InitAdditionalCombatReplayData(ParsedLog log);

        public abstract AbstractSingleActorSerializable GetCombatReplayJSON(CombatReplayMap map, ParsedLog log);

        // Cast logs
        public override List<AbstractCastEvent> GetCastLogs(ParsedLog log, long start, long end)
        {
            if (CastLogs == null)
            {
                SetCastLogs(log);
            }
            return CastLogs.Where(x => x.Time >= start && x.Time <= end).ToList();

        }
        public override List<AbstractCastEvent> GetCastLogsActDur(ParsedLog log, long start, long end)
        {
            if (CastLogs == null)
            {
                SetCastLogs(log);
            }
            return CastLogs.Where(x => x.Time >= start && x.Time + x.ActualDuration <= end).ToList();

        }
        protected void SetCastLogs(ParsedLog log)
        {
            CastLogs = new List<AbstractCastEvent>(log.CombatData.GetAnimatedCastData(AgentItem));
            foreach (WeaponSwapEvent wepSwap in log.CombatData.GetWeaponSwapData(AgentItem))
            {
                if (CastLogs.Count > 0 && (wepSwap.Time - CastLogs.Last().Time) < GeneralHelper.ServerDelayConstant && CastLogs.Last().SkillId == SkillItem.WeaponSwapId)
                {
                    CastLogs[CastLogs.Count - 1] = wepSwap;
                }
                else
                {
                    CastLogs.Add(wepSwap);
                }
            }
            CastLogs.Sort((x, y) => x.Time.CompareTo(y.Time));
        }

        // DPS Stats

        public FinalDPS GetDPSAll(ParsedLog log, int phaseIndex)
        {
            return GetDPSAll(log)[phaseIndex];
        }

        public List<FinalDPS> GetDPSAll(ParsedLog log)
        {
            if (_dpsAll == null)
            {
                _dpsAll = new List<FinalDPS>();
                foreach (PhaseData phase in log.FightData.GetPhases(log))
                {
                    _dpsAll.Add(new FinalDPS(log, phase, this, null));
                }
            }
            return _dpsAll;
        }

        public FinalDPS GetDPSTarget(ParsedLog log, int phaseIndex, AbstractSingleActor target)
        {
            return GetDPSTarget(log, target)[phaseIndex];
        }

        public List<FinalDPS> GetDPSTarget(ParsedLog log, AbstractSingleActor target)
        {
            if (target == null)
            {
                return GetDPSAll(log);
            }
            if (_dpsTarget == null)
            {
                _dpsTarget = new Dictionary<AbstractSingleActor, List<FinalDPS>>();
            }
            if (_dpsTarget.TryGetValue(target, out List<FinalDPS> list))
            {
                return list;
            }
            _dpsTarget[target] = new List<FinalDPS>();
            foreach (PhaseData phase in log.FightData.GetPhases(log))
            {
                _dpsTarget[target].Add(new FinalDPS(log, phase, this, target));
            }
            return _dpsTarget[target];
        }

        // Defense Stats

        public FinalDefensesAll GetDefenses(ParsedLog log, int phaseIndex)
        {
            return GetDefenses(log)[phaseIndex];
        }

        public List<FinalDefensesAll> GetDefenses(ParsedLog log)
        {
            if (_defenses == null)
            {
                _defenses = new List<FinalDefensesAll>();
                foreach (PhaseData phase in log.FightData.GetPhases(log))
                {
                    _defenses.Add(new FinalDefensesAll(log, phase, this));
                }
            }
            return _defenses;
        }

        public FinalDefenses GetDefenses(ParsedLog log, AbstractSingleActor target, int phaseIndex)
        {
            return GetDefenses(log, target)[phaseIndex];
        }

        public List<FinalDefenses> GetDefenses(ParsedLog log, AbstractSingleActor target)
        {
            if (_defensesTarget == null)
            {
                return new List<FinalDefenses>(GetDefenses(log));
            }
            if (_defensesTarget == null)
            {
                _defensesTarget = new Dictionary<AbstractSingleActor, List<FinalDefenses>>();
            }
            if (_defensesTarget.TryGetValue(target, out List<FinalDefenses> list))
            {
                return list;
            }
            _defensesTarget[target] = new List<FinalDefenses>();
            foreach (PhaseData phase in log.FightData.GetPhases(log))
            {
                _defensesTarget[target].Add(new FinalDefenses(log, phase, this, target));
            }
            return _defensesTarget[target];
        }

        // Gameplay Stats


        public FinalGameplayStatsAll GetGameplayStats(ParsedLog log, int phaseIndex)
        {
            return GetGameplayStats(log)[phaseIndex];
        }

        public FinalGameplayStats GetGameplayStats(ParsedLog log, int phaseIndex, AbstractSingleActor target)
        {
            if (target == null)
            {
                return GetGameplayStats(log, phaseIndex);
            }
            return GetGameplayStats(log, target)[phaseIndex];
        }

        public List<FinalGameplayStatsAll> GetGameplayStats(ParsedLog log)
        {
            if (_statsAll == null)
            {
                _statsAll = new List<FinalGameplayStatsAll>();
                foreach (PhaseData phase in log.FightData.GetPhases(log))
                {
                    _statsAll.Add(new FinalGameplayStatsAll(log, phase, this));
                }
            }
            return _statsAll;
        }

        public List<FinalGameplayStats> GetGameplayStats(ParsedLog log, AbstractSingleActor target)
        {
            if (target == null)
            {
                return new List<FinalGameplayStats>(GetGameplayStats(log));
            }
            if (_statsTarget == null)
            {
                _statsTarget = new Dictionary<AbstractSingleActor, List<FinalGameplayStats>>();
            }
            if (_statsTarget.TryGetValue(target, out List<FinalGameplayStats> list))
            {
                return list;
            }
            _statsTarget[target] = new List<FinalGameplayStats>();
            foreach (PhaseData phase in log.FightData.GetPhases(log))
            {
                _statsTarget[target].Add(new FinalGameplayStats(log, phase, this, target));
            }
            return _statsTarget[target];
        }

        // Support stats
        public FinalSupportAll GetSupport(ParsedLog log, int phaseIndex)
        {
            return GetSupport(log)[phaseIndex];
        }

        public List<FinalSupportAll> GetSupport(ParsedLog log)
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

        public FinalSupport GetSupport(ParsedLog log, AbstractSingleActor target, int phaseIndex)
        {
            return GetSupport(log, target)[phaseIndex];
        }

        public List<FinalSupport> GetSupport(ParsedLog log, AbstractSingleActor target)
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
        public override List<AbstractDamageEvent> GetDamageLogs(AbstractActor target, ParsedLog log, long start, long end)
        {
            if (DamageLogs == null)
            {
                DamageLogs = new List<AbstractDamageEvent>();
                DamageLogs.AddRange(log.CombatData.GetDamageData(AgentItem).Where(x => x.IFF != ParseEnum.IFF.Friend));
                Dictionary<long, Minions> minionsList = GetMinions(log);
                foreach (Minions mins in minionsList.Values)
                {
                    DamageLogs.AddRange(mins.GetDamageLogs(null, log, 0, log.FightData.FightEnd));
                }
                DamageLogs.Sort((x, y) => x.Time.CompareTo(y.Time));
                DamageLogsByDst = DamageLogs.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            }
            if (target != null)
            {
                if (DamageLogsByDst.TryGetValue(target.AgentItem, out List<AbstractDamageEvent> list))
                {
                    return list.Where(x => x.Time >= start && x.Time <= end).ToList();
                } 
                else
                {
                    return new List<AbstractDamageEvent>();
                }
            }
            return DamageLogs.Where(x => x.Time >= start && x.Time <= end).ToList();
        }

        public override List<AbstractDamageEvent> GetDamageTakenLogs(AbstractActor target, ParsedLog log, long start, long end)
        {
            if (DamageTakenlogs == null)
            {
                DamageTakenlogs = new List<AbstractDamageEvent>();
                DamageTakenlogs.AddRange(log.CombatData.GetDamageTakenData(AgentItem));
                DamageTakenLogsBySrc = DamageTakenlogs.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
            }
            if (target != null)
            {
                if (DamageTakenLogsBySrc.TryGetValue(target.AgentItem, out List<AbstractDamageEvent> list))
                {
                    long targetStart = target.FirstAware;
                    long targetEnd = target.LastAware;
                    return list.Where(x => x.Time >= start && x.Time >= targetStart && x.Time <= end && x.Time <= targetEnd).ToList();
                }
                else
                {
                    return new List<AbstractDamageEvent>();
                }
            }
            return DamageTakenlogs.Where(x => x.Time >= start && x.Time <= end).ToList();
        }

        public List<AbstractDamageEvent> GetJustPlayerDamageLogs(AbstractActor target, ParsedLog log, PhaseData phase)
        {
            if (!_selfDamageLogsPerPhasePerTarget.TryGetValue(phase, out Dictionary<AbstractActor, List<AbstractDamageEvent>> targetDict))
            {
                targetDict = new Dictionary<AbstractActor, List<AbstractDamageEvent>>();
                _selfDamageLogsPerPhasePerTarget[phase] = targetDict;
            }
            if (!targetDict.TryGetValue(target ?? GeneralHelper.NullActor, out List<AbstractDamageEvent> dls))
            {
                dls = GetDamageLogs(target, log, phase).Where(x => x.From == AgentItem).ToList();
                targetDict[target ?? GeneralHelper.NullActor] = dls;
            }
            return dls;
        }
    }
}
