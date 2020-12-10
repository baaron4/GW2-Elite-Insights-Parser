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
        private readonly Dictionary<int, List<int>> _damageList1S = new Dictionary<int, List<int>>();
        private readonly Dictionary<int, List<double>> _breakbarDamageList1S = new Dictionary<int, List<double>>();
        private readonly Dictionary<PhaseData, Dictionary<AbstractActor, List<AbstractHealthDamageEvent>>> _selfDamageLogsPerPhasePerTarget = new Dictionary<PhaseData, Dictionary<AbstractActor, List<AbstractHealthDamageEvent>>>();
        //status
        private List<Segment> _healthUpdates { get; set; }
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

        public abstract string GetIcon();     

        public List<Segment> GetHealthUpdates(ParsedEvtcLog log)
        {
            if (_healthUpdates == null)
            {
                _healthUpdates = Segment.FromStates(log.CombatData.GetHealthUpdateEvents(AgentItem).Select(x => x.ToState()).ToList(), 0, log.FightData.FightEnd);
            }
            return _healthUpdates;
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
                    if (pair.Value.GetDamageLogs(null, log, 0, log.FightData.FightEnd).Count > 0 || pair.Value.GetCastLogs(log, 0, log.FightData.FightEnd).Any(x => x.SkillId != SkillItem.WeaponSwapId && x.SkillId != SkillItem.MirageCloakDodgeId))
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
                    if (pair.Value.GetDamageLogs(null, log, 0, log.FightData.FightEnd).Count > 0 || pair.Value.GetCastLogs(log, 0, log.FightData.FightEnd).Any(x => x.SkillId != SkillItem.WeaponSwapId && x.SkillId != SkillItem.MirageCloakDodgeId))
                    {
                        _minions[pair.Value.AgentItem.UniqueID] = pair.Value;
                    }
                }
            }
            return _minions;
        }

        // Graph

        public List<int> Get1SDamageList(ParsedEvtcLog log, int phaseIndex, PhaseData phase, AbstractActor target)
        {
            ulong targetId = target != null ? target.Agent : 0;
            int id = (phaseIndex + "_" + targetId + "_1S").GetHashCode();
            if (_damageList1S.TryGetValue(id, out List<int> res))
            {
                return res;
            }
            var dmgList = new List<int>();
            List<AbstractHealthDamageEvent> damageLogs = GetDamageLogs(target, log, phase.Start, phase.End);
            // fill the graph, full precision
            var dmgListFull = new List<int>();
            for (int i = 0; i <= phase.DurationInMS; i++)
            {
                dmgListFull.Add(0);
            }
            int totalTime = 1;
            int totalDamage = 0;
            foreach (AbstractHealthDamageEvent dl in damageLogs)
            {
                int time = (int)(dl.Time - phase.Start);
                // fill
                for (; totalTime < time; totalTime++)
                {
                    dmgListFull[totalTime] = totalDamage;
                }
                totalDamage += dl.HealthDamage;
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

        public List<double> Get1SBreakbarDamageList(ParsedEvtcLog log, int phaseIndex, PhaseData phase, AbstractActor target)
        {
            if (!log.CombatData.HasBreakbarDamageData)
            {
                return null;
            }
            ulong targetId = target != null ? target.Agent : 0;
            int id = (phaseIndex + "_" + targetId + "_1S").GetHashCode();
            if (_breakbarDamageList1S.TryGetValue(id, out List<double> res))
            {
                return res;
            }
            var brkDmgList = new List<double>();
            List<AbstractBreakbarDamageEvent> breakbarDamageLogs = GetBreakbarDamageLogs(target, log, phase.Start, phase.End);
            // fill the graph, full precision
            var brkDmgListFull = new List<double>();
            for (int i = 0; i <= phase.DurationInMS; i++)
            {
                brkDmgListFull.Add(0);
            }
            int totalTime = 1;
            double totalDamage = 0;
            foreach (DirectBreakbarDamageEvent dl in breakbarDamageLogs)
            {
                int time = (int)(dl.Time - phase.Start);
                // fill
                for (; totalTime < time; totalTime++)
                {
                    brkDmgListFull[totalTime] = totalDamage;
                }
                totalDamage = Math.Round(totalDamage + dl.BreakbarDamage, 1);
                brkDmgListFull[totalTime] = totalDamage;
            }
            // fill
            for (; totalTime <= phase.DurationInMS; totalTime++)
            {
                brkDmgListFull[totalTime] = totalDamage;
            }
            //
            brkDmgList.Add(0);
            for (int i = 1; i <= phase.DurationInS; i++)
            {
                brkDmgList.Add(brkDmgListFull[1000 * i]);
            }
            if (phase.DurationInS * 1000 != phase.DurationInMS)
            {
                double lastDamage = brkDmgListFull[(int)phase.DurationInMS];
                brkDmgList.Add(lastDamage);
            }
            _breakbarDamageList1S[id] = brkDmgList;
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
                            var value = simul.GetClampedDuration(phase.Start, phase.End);
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
                        (updateBoonPresence? boonPresenceGraph : condiPresenceGraph).MergePresenceInto(BuffPoints[boonid].BuffChart);
                    }

                }
            }
            BuffPoints[NumberOfBoonsID] = boonPresenceGraph;
            BuffPoints[NumberOfConditionsID] = condiPresenceGraph;
            foreach(Minions minions in GetMinions(log).Values)
            {
                foreach(List<Segment> minionsSegments in minions.GetLifeSpanSegments(log))
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
                long activePhaseDuration = phase.GetActorActiveDuration(this, log);

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
        public override List<AbstractCastEvent> GetCastLogs(ParsedEvtcLog log, long start, long end)
        {
            if (CastLogs == null)
            {
                SetCastLogs(log);
            }
            return CastLogs.Where(x => x.Time >= start && x.Time <= end).ToList();

        }
        public override List<AbstractCastEvent> GetIntersectingCastLogs(ParsedEvtcLog log, long start, long end)
        {
            if (CastLogs == null)
            {
                SetCastLogs(log);
            }
            return CastLogs.Where(x => KeepIntersectingCastLog(x, start, end)).ToList();

        }
        protected void SetCastLogs(ParsedEvtcLog log)
        {
            CastLogs = new List<AbstractCastEvent>(log.CombatData.GetAnimatedCastData(AgentItem));
            CastLogs.AddRange(log.CombatData.GetInstantCastData(AgentItem));
            foreach (WeaponSwapEvent wepSwap in log.CombatData.GetWeaponSwapData(AgentItem))
            {
                if (CastLogs.Count > 0 && (wepSwap.Time - CastLogs.Last().Time) < ParserHelper.ServerDelayConstant && CastLogs.Last().SkillId == SkillItem.WeaponSwapId)
                {
                    CastLogs[CastLogs.Count - 1] = wepSwap;
                }
                else
                {
                    CastLogs.Add(wepSwap);
                }
            }
            CastLogs.Sort((x, y) =>
            {
                var compare = x.Time.CompareTo(y.Time);
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

        public FinalDPS GetDPSAll(ParsedEvtcLog log, int phaseIndex)
        {
            return GetDPSAll(log)[phaseIndex];
        }

        public List<FinalDPS> GetDPSAll(ParsedEvtcLog log)
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

        public FinalDPS GetDPSTarget(ParsedEvtcLog log, int phaseIndex, AbstractSingleActor target)
        {
            return GetDPSTarget(log, target)[phaseIndex];
        }

        public List<FinalDPS> GetDPSTarget(ParsedEvtcLog log, AbstractSingleActor target)
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

        public FinalDefensesAll GetDefenses(ParsedEvtcLog log, int phaseIndex)
        {
            return GetDefenses(log)[phaseIndex];
        }

        public List<FinalDefensesAll> GetDefenses(ParsedEvtcLog log)
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

        public FinalDefenses GetDefenses(ParsedEvtcLog log, AbstractSingleActor target, int phaseIndex)
        {
            return GetDefenses(log, target)[phaseIndex];
        }

        public List<FinalDefenses> GetDefenses(ParsedEvtcLog log, AbstractSingleActor target)
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


        public FinalGameplayStatsAll GetGameplayStats(ParsedEvtcLog log, int phaseIndex)
        {
            return GetGameplayStats(log)[phaseIndex];
        }

        public FinalGameplayStats GetGameplayStats(ParsedEvtcLog log, int phaseIndex, AbstractSingleActor target)
        {
            if (target == null)
            {
                return GetGameplayStats(log, phaseIndex);
            }
            return GetGameplayStats(log, target)[phaseIndex];
        }

        public List<FinalGameplayStatsAll> GetGameplayStats(ParsedEvtcLog log)
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

        public List<FinalGameplayStats> GetGameplayStats(ParsedEvtcLog log, AbstractSingleActor target)
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
        public override List<AbstractHealthDamageEvent> GetDamageLogs(AbstractActor target, ParsedEvtcLog log, long start, long end)
        {
            if (DamageLogs == null)
            {
                DamageLogs = new List<AbstractHealthDamageEvent>();
                DamageLogs.AddRange(log.CombatData.GetDamageData(AgentItem).Where(x => x.IFF != ArcDPSEnums.IFF.Friend));
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
                if (DamageLogsByDst.TryGetValue(target.AgentItem, out List<AbstractHealthDamageEvent> list))
                {
                    return list.Where(x => x.Time >= start && x.Time <= end).ToList();
                } 
                else
                {
                    return new List<AbstractHealthDamageEvent>();
                }
            }
            return DamageLogs.Where(x => x.Time >= start && x.Time <= end).ToList();
        }

        public List<AbstractHealthDamageEvent> GetJustActorDamageLogs(AbstractActor target, ParsedEvtcLog log, long start, long end)
        {
            return GetDamageLogs(target, log, start, end).Where(x => x.From == AgentItem).ToList();
        }

        public List<AbstractBreakbarDamageEvent> GetJustActorBreakbarDamageLogs(AbstractActor target, ParsedEvtcLog log, long start, long end)
        {
            return GetBreakbarDamageLogs(target, log, start, end).Where(x => x.From == AgentItem).ToList();
        }

        public override List<AbstractBreakbarDamageEvent> GetBreakbarDamageLogs(AbstractActor target, ParsedEvtcLog log, long start, long end)
        {
            if (BreakbarDamageLogs == null)
            {
                BreakbarDamageLogs = new List<AbstractBreakbarDamageEvent>();
                BreakbarDamageLogs.AddRange(log.CombatData.GetBreakbarDamageData(AgentItem).Where(x => x.IFF != ArcDPSEnums.IFF.Friend));
                Dictionary<long, Minions> minionsList = GetMinions(log);
                foreach (Minions mins in minionsList.Values)
                {
                    BreakbarDamageLogs.AddRange(mins.GetBreakbarDamageLogs(null, log, 0, log.FightData.FightEnd));
                }
                BreakbarDamageLogs.Sort((x, y) => x.Time.CompareTo(y.Time));
                BreakbarDamageLogsByDst = BreakbarDamageLogs.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            }
            if (target != null)
            {
                if (BreakbarDamageLogsByDst.TryGetValue(target.AgentItem, out List<AbstractBreakbarDamageEvent> list))
                {
                    return list.Where(x => x.Time >= start && x.Time <= end).ToList();
                }
                else
                {
                    return new List<AbstractBreakbarDamageEvent>();
                }
            }
            return BreakbarDamageLogs.Where(x => x.Time >= start && x.Time <= end).ToList();
        }

        public override List<AbstractHealthDamageEvent> GetDamageTakenLogs(AbstractActor target, ParsedEvtcLog log, long start, long end)
        {
            if (DamageTakenlogs == null)
            {
                DamageTakenlogs = new List<AbstractHealthDamageEvent>();
                DamageTakenlogs.AddRange(log.CombatData.GetDamageTakenData(AgentItem));
                DamageTakenLogsBySrc = DamageTakenlogs.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
            }
            if (target != null)
            {
                if (DamageTakenLogsBySrc.TryGetValue(target.AgentItem, out List<AbstractHealthDamageEvent> list))
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
            return DamageTakenlogs.Where(x => x.Time >= start && x.Time <= end).ToList();
        }

        public override List<AbstractBreakbarDamageEvent> GetBreakbarDamageTakenLogs(AbstractActor target, ParsedEvtcLog log, long start, long end)
        {
            if (BreakbarDamageTakenLogs == null)
            {
                BreakbarDamageTakenLogs = new List<AbstractBreakbarDamageEvent>();
                BreakbarDamageTakenLogs.AddRange(log.CombatData.GetBreakbarDamageTakenData(AgentItem));
                BreakbarDamageTakenLogsBySrc = BreakbarDamageTakenLogs.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
            }
            if (target != null)
            {
                if (BreakbarDamageTakenLogsBySrc.TryGetValue(target.AgentItem, out List<AbstractBreakbarDamageEvent> list))
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
            return BreakbarDamageTakenLogs.Where(x => x.Time >= start && x.Time <= end).ToList();
        }

        /// <summary>
        /// cached method for damage modifiers
        /// </summary>
        internal List<AbstractHealthDamageEvent> GetJustActorHitDamageLogs(AbstractActor target, ParsedEvtcLog log, PhaseData phase)
        {
            if (!_selfDamageLogsPerPhasePerTarget.TryGetValue(phase, out Dictionary<AbstractActor, List<AbstractHealthDamageEvent>> targetDict))
            {
                targetDict = new Dictionary<AbstractActor, List<AbstractHealthDamageEvent>>();
                _selfDamageLogsPerPhasePerTarget[phase] = targetDict;
            }
            if (!targetDict.TryGetValue(target ?? ParserHelper._nullActor, out List<AbstractHealthDamageEvent> dls))
            {
                dls = GetHitDamageLogs(target, log, phase).Where(x => x.From == AgentItem).ToList();
                targetDict[target ?? ParserHelper._nullActor] = dls;
            }
            return dls;
        }
    }
}
