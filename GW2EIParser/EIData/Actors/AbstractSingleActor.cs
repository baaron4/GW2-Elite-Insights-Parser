using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.Models;
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
        protected Dictionary<long, BuffsGraphModel> BuffPoints { get; set; }
        private readonly List<BuffDistribution> _boonDistribution = new List<BuffDistribution>();
        private readonly List<Dictionary<long, long>> _buffPresence = new List<Dictionary<long, long>>();
        // damage list
        private readonly Dictionary<int, List<int>> _damageList1S = new Dictionary<int, List<int>>();
        private readonly Dictionary<PhaseData, Dictionary<AbstractActor, List<AbstractDamageEvent>>> _selfDamageLogsPerPhasePerTarget = new Dictionary<PhaseData, Dictionary<AbstractActor, List<AbstractDamageEvent>>>();
        //status
        private List<(long start, long end)> _deads;
        private List<(long start, long end)> _downs;
        private List<(long start, long end)> _dcs;
        // Minions
        private Dictionary<long, Minions> _minions;
        // Replay
        protected CombatReplay CombatReplay { get; set; }
        public string Icon { get; }
        // Statistics
        private List<FinalDPS> _dpsAll;
        private Dictionary<AbstractSingleActor, List<FinalDPS>> _dpsTarget;
        private List<FinalDefenses> _defenses;
        private Dictionary<AbstractSingleActor, List<FinalStats>> _statsTarget;
        private List<FinalStatsAll> _statsAll;

        protected AbstractSingleActor(AgentItem agent) : base(agent)
        {
            Icon = GeneralHelper.GetIcon(this);
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
                var combatMinion = log.AgentData.GetAgentByType(AgentItem.AgentType.NPC).Where(x => x.Master == AgentItem).ToList();
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
                        auxMinions[id] = new Minions(new NPC(agent));
                    }
                }
                foreach (KeyValuePair<long, Minions> pair in auxMinions)
                {
                    if (pair.Value.GetDamageLogs(null, log, 0, log.FightData.FightDuration).Count > 0 || pair.Value.GetCastLogs(log, 0, log.FightData.FightDuration).Count > 0)
                    {
                        _minions[pair.Key] = pair.Value;
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

        protected BuffDictionary GetBuffMap(ParsedLog log)
        {
            //
            var buffMapMap = new BuffDictionary();
            // Fill in Boon Map
            foreach (AbstractBuffEvent c in log.CombatData.GetBuffDataByDst(AgentItem))
            {
                long boonId = c.BuffID;
                if (!buffMapMap.ContainsKey(boonId))
                {
                    if (!log.Buffs.BuffsByIds.ContainsKey(boonId))
                    {
                        continue;
                    }
                    buffMapMap.Add(log.Buffs.BuffsByIds[boonId]);
                }
                if (!c.IsBuffSimulatorCompliant(log.FightData.FightDuration))
                {
                    continue;
                }
                List<AbstractBuffEvent> loglist = buffMapMap[boonId];
                c.TryFindSrc(log);
                loglist.Add(c);
            }
            // add buff remove all for each despawn events
            foreach (DespawnEvent dsp in log.CombatData.GetDespawnEvents(AgentItem))
            {
                foreach (KeyValuePair<long, List<AbstractBuffEvent>> pair in buffMapMap)
                {
                    pair.Value.Add(new BuffRemoveAllEvent(GeneralHelper.UnknownAgent, AgentItem, dsp.Time, int.MaxValue, log.SkillData.Get(pair.Key), BuffRemoveAllEvent.FullRemoval, int.MaxValue));
                }
            }
            buffMapMap.Sort();
            foreach (KeyValuePair<long, List<AbstractBuffEvent>> pair in buffMapMap)
            {
                TrackedBuffs.Add(log.Buffs.BuffsByIds[pair.Key]);
            }
            return buffMapMap;
        }


        protected void SetBuffStatus(ParsedLog log)
        {
            BuffPoints = new Dictionary<long, BuffsGraphModel>();
            BuffDictionary toUse = GetBuffMap(log);
            long dur = log.FightData.FightDuration;
            int fightDuration = (int)(dur) / 1000;
            var boonPresenceGraph = new BuffsGraphModel(log.Buffs.BuffsByIds[ProfHelper.NumberOfBoonsID]);
            var condiPresenceGraph = new BuffsGraphModel(log.Buffs.BuffsByIds[ProfHelper.NumberOfConditionsID]);
            var boonIds = new HashSet<long>(log.Buffs.BuffsByNature[BuffNature.Boon].Select(x => x.ID));
            var condiIds = new HashSet<long>(log.Buffs.BuffsByNature[BuffNature.Condition].Select(x => x.ID));
            InitBuffStatusData(log);
            foreach (Buff buff in TrackedBuffs)
            {
                long boonid = buff.ID;
                if (toUse.TryGetValue(boonid, out List<AbstractBuffEvent> logs) && logs.Count != 0)
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
                    SetBuffStatusCleanseWasteData(log, simulator, boonid, updateCondiPresence);
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

        private void SetBuffStatusCleanseWasteData(ParsedLog log, AbstractBuffSimulator simulator, long boonid, bool updateCondiPresence)
        {
            List<PhaseData> phases = log.FightData.GetPhases(log);
            var extraSimulations = new List<AbstractBuffSimulationItem>(simulator.OverstackSimulationResult);
            extraSimulations.AddRange(simulator.WasteSimulationResult);
            foreach (AbstractBuffSimulationItem simul in extraSimulations)
            {
                for (int i = 0; i < phases.Count; i++)
                {
                    PhaseData phase = phases[i];
                    simul.SetBoonDistributionItem(_boonDistribution[i], phase.Start, phase.End, boonid, log);
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
                simul.SetBoonDistributionItem(_boonDistribution[i], phase.Start, phase.End, boonid, log);
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
            (List<(long start, long end)> deads, List<(long start, long end)> downs, List<(long start, long end)> dcs) = GetStatus(log);
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
                CombatReplay.Trim(log.FightData.ToFightSpace(AgentItem.FirstAwareLogTime), deathCheck.Time);
            }
            else if (despawnCheck != null && (spawnCheck == null || spawnCheck.Time < despawnCheck.Time))
            {
                CombatReplay.Trim(log.FightData.ToFightSpace(AgentItem.FirstAwareLogTime), despawnCheck.Time);
            }
            else
            {
                CombatReplay.Trim(log.FightData.ToFightSpace(AgentItem.FirstAwareLogTime), log.FightData.ToFightSpace(AgentItem.LastAwareLogTime));
            }
        }

        public List<GenericDecoration> GetCombatReplayActors(ParsedLog log)
        {
            if (!log.CanCombatReplay || IsFakeActor)
            {
                // no combat replay support on fight
                return null;
            }
            if (CombatReplay == null)
            {
                InitCombatReplay(log);
            }
            if (CombatReplay.NoActors)
            {
                CombatReplay.NoActors = false;
                InitAdditionalCombatReplayData(log);
            }
            return CombatReplay.Decorations;
        }

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
            CastLogs = new List<AbstractCastEvent>(log.CombatData.GetCastData(AgentItem));
            foreach (WeaponSwapEvent wepSwap in log.CombatData.GetWeaponSwapData(AgentItem))
            {
                if (CastLogs.Count > 0 && (wepSwap.Time - CastLogs.Last().Time) < 10 && CastLogs.Last().SkillId == SkillItem.WeaponSwapId)
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


        public int GetCombatReplayID(ParsedLog log)
        {
            if (CombatReplay == null)
            {
                InitCombatReplay(log);
            }
            return AgentItem.UniqueID.GetHashCode();
        }
        protected abstract void InitAdditionalCombatReplayData(ParsedLog log);

        public abstract class AbstractMasterActorSerializable
        {
            public string Img { get; set; }
            public string Type { get; set; }
            public int ID { get; set; }
            public List<double> Positions { get; set; }
        }
        public abstract AbstractMasterActorSerializable GetCombatReplayJSON(CombatReplayMap map, ParsedLog log);

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
            return GetDPSTarget(log,target)[phaseIndex];
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

        public FinalDefenses GetDefenses(ParsedLog log, int phaseIndex)
        {
            return GetDefenses(log)[phaseIndex];
        }

        public List<FinalDefenses> GetDefenses(ParsedLog log)
        {
            if (_defenses == null)
            {
                _defenses = new List<FinalDefenses>();
                foreach (PhaseData phase in log.FightData.GetPhases(log))
                {
                    _defenses.Add(new FinalDefenses(log, phase, this, null));
                }
            }
            return _defenses;
        }

        // Gameplay Stats


        public FinalStatsAll GetStatsAll(ParsedLog log, int phaseIndex)
        {
            return GetStatsAll(log)[phaseIndex];
        }

        public FinalStats GetStatsTarget(ParsedLog log, int phaseIndex, AbstractSingleActor target)
        {
            if (target == null)
            {
                return GetStatsAll(log, phaseIndex);
            }
            return GetStatsTarget(log, target)[phaseIndex];
        }

        public List<FinalStatsAll> GetStatsAll(ParsedLog log)
        {
            if (_statsAll == null)
            {
                _statsAll = new List<FinalStatsAll>();
                foreach (PhaseData phase in log.FightData.GetPhases(log))
                {
                    _statsAll.Add(new FinalStatsAll(log, phase, this));
                }
            }
            return _statsAll;
        }

        public List<FinalStats> GetStatsTarget(ParsedLog log, AbstractSingleActor target)
        {
            if (target == null)
            {
                return new List<FinalStats>(GetStatsAll(log));
            }
            if (_statsTarget == null)
            {
                _statsTarget = new Dictionary<AbstractSingleActor, List<FinalStats>>();
            }
            if (_statsTarget.TryGetValue(target, out List<FinalStats> list))
            {
                return list;
            }
            _statsTarget[target] = new List<FinalStats>();
            foreach (PhaseData phase in log.FightData.GetPhases(log))
            {
                _statsTarget[target].Add(new FinalStats(log, phase, this, target));
            }
            return _statsTarget[target];
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
                    DamageLogs.AddRange(mins.GetDamageLogs(null, log, 0, log.FightData.FightDuration));
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
                    long targetStart = log.FightData.ToFightSpace(target.FirstAwareLogTime);
                    long targetEnd = log.FightData.ToFightSpace(target.LastAwareLogTime);
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
