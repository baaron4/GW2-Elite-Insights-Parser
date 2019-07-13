using System.Collections.Generic;
using System.Linq;
using LuckParser.Parser;
using LuckParser.Parser.ParsedData;
using LuckParser.Parser.ParsedData.CombatEvents;

namespace LuckParser.EIData
{
    public abstract class AbstractActor : DummyActor
    {
        // Damage
        protected List<AbstractDamageEvent> DamageLogs;
        protected Dictionary<AgentItem, List<AbstractDamageEvent>> DamageLogsByDst;
        private Dictionary<PhaseData, Dictionary<AbstractActor, List<AbstractDamageEvent>>> _damageLogsPerPhasePerTarget = new Dictionary<PhaseData, Dictionary<AbstractActor, List<AbstractDamageEvent>>>();
        //protected List<DamageLog> HealingLogs = new List<DamageLog>();
        //protected List<DamageLog> HealingReceivedLogs = new List<DamageLog>();
        private List<AbstractDamageEvent> _damageTakenlogs;
        protected Dictionary<AgentItem, List<AbstractDamageEvent>> _damageTakenLogsBySrc;
        // Cast
        protected List<AbstractCastEvent> CastLogs;
        // Boons
        public HashSet<Boon> TrackedBoons { get; } = new HashSet<Boon>();
        protected Dictionary<long, BoonsGraphModel> BoonPoints;

        protected AbstractActor(AgentItem agent) : base(agent)
        {
        }
        // Getters

        public List<AbstractDamageEvent> GetDamageLogs(AbstractActor target, ParsedLog log, long start, long end)
        {
            if (DamageLogs == null)
            {
                DamageLogs = new List<AbstractDamageEvent>();
                SetDamageLogs(log);
                DamageLogsByDst = DamageLogs.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            }
            if (target != null)
            {
                if (DamageLogsByDst.TryGetValue(target.AgentItem, out var list))
                {
                    return list.Where(x => x.Time >= start && x.Time <= end).ToList();
                }
                else
                {
                    return new List<AbstractDamageEvent>();
                }
            }
            return DamageLogs.Where( x => x.Time >= start && x.Time <= end).ToList();
        }

        public List<AbstractDamageEvent> GetDamageLogs(AbstractActor target, ParsedLog log, PhaseData phase)
        {
            if (!_damageLogsPerPhasePerTarget.TryGetValue(phase, out Dictionary<AbstractActor, List<AbstractDamageEvent>> targetDict))
            {
                targetDict = new Dictionary<AbstractActor, List<AbstractDamageEvent>>();
                _damageLogsPerPhasePerTarget[phase] = targetDict;
            }
            if (!targetDict.TryGetValue(target ?? GeneralHelper.NullActor, out List<AbstractDamageEvent> dls))
            {
                dls = GetDamageLogs(target, log, phase.Start, phase.End);
                targetDict[target ?? GeneralHelper.NullActor] = dls;
            }
            return dls;
        }

        public List<AbstractDamageEvent> GetDamageTakenLogs(AbstractActor target, ParsedLog log, long start, long end)
        {
            if (_damageTakenlogs == null)
            {
                _damageTakenlogs = new List<AbstractDamageEvent>();
                SetDamageTakenLogs(log);
                _damageTakenLogsBySrc = _damageTakenlogs.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
            }
            if (target != null)
            {
                if (_damageTakenLogsBySrc.TryGetValue(target.AgentItem, out var list))
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
            return _damageTakenlogs.Where(x => x.Time >= start && x.Time <= end).ToList();
        }

        public Dictionary<long, BoonsGraphModel> GetBoonGraphs(ParsedLog log)
        {
            if (BoonPoints == null)
            {
                SetBoonStatus(log);
            }
            return BoonPoints;
        }
        /*public List<DamageLog> getHealingLogs(ParsedLog log, long start, long end)//isntid = 0 gets all logs if specified sets and returns filtered logs
        {
            if (healingLogs.Count == 0)
            {
                setHealingLogs(log);
            }
            return healingLogs.Where(x => x.getTime() >= start && x.getTime() <= end).ToList();
        }
        public List<DamageLog> getHealingReceivedLogs(ParsedLog log, long start, long end)
        {
            if (healingReceivedLogs.Count == 0)
            {
                setHealingReceivedLogs(log);
            }
            return healingReceivedLogs.Where(x => x.getTime() >= start && x.getTime() <= end).ToList();
        }*/
        public List<AbstractCastEvent> GetCastLogs(ParsedLog log, long start, long end)
        {
            if (CastLogs == null)
            {
                SetCastLogs(log);
            }
            return CastLogs.Where(x => x.Time >= start && x.Time <= end).ToList();

        }

        public List<AbstractCastEvent> GetCastLogsActDur(ParsedLog log, long start, long end)
        {
            if (CastLogs == null)
            {
                SetCastLogs(log);
            }
            return CastLogs.Where(x => x.Time + x.ActualDuration > start && x.Time < end).ToList();

        }
        // privates
        protected void AddDamageLogs(List<AbstractDamageEvent> damageEvents)
        {
            DamageLogs.AddRange(damageEvents.Where(x => x.IFF != ParseEnum.IFF.Friend));
        }

        protected static void Add<T>(Dictionary<T, long> dictionary, T key, long value)
        {
            if (dictionary.TryGetValue(key, out var existing))
            {
                dictionary[key] = existing + value;
            }
            else
            {
                dictionary.Add(key, value);
            }
        }

        protected BoonMap GetBoonMap(ParsedLog log)
        {
            //
            BoonMap boonMap = new BoonMap();
            // Fill in Boon Map
            foreach (AbstractBuffEvent c in log.CombatData.GetBoonDataByDst(AgentItem))
            {
                long boonId = c.BuffID;
                if (!boonMap.ContainsKey(boonId))
                {
                    if (!log.Boons.BoonsByIds.ContainsKey(boonId))
                    {
                        continue;
                    }
                    boonMap.Add(log.Boons.BoonsByIds[boonId]);
                }
                if (!c.IsBoonSimulatorCompliant(log.FightData.FightDuration))
                {
                    continue;
                }
                List<AbstractBuffEvent> loglist = boonMap[boonId];
                c.TryFindSrc(log);
                loglist.Add(c);
            }
            boonMap.Sort();
            foreach (var pair in boonMap)
            {
                TrackedBoons.Add(log.Boons.BoonsByIds[pair.Key]);
            }
            return boonMap;
        }


        /*protected void addHealingLog(long time, CombatItem c)
        {
            if (c.isBuffremove() == ParseEnum.BuffRemove.None)
            {
                if (c.isBuff() == 1 && c.getBuffDmg() != 0)//boon
                {
                    healing_logs.Add(new DamageLogCondition(time, c));
                }
                else if (c.isBuff() == 0 && c.getValue() != 0)//skill
                {
                    healing_logs.Add(new DamageLogPower(time, c));
                }
            }

        }
        protected void addHealingReceivedLog(long time, CombatItem c)
        {
            if (c.isBuff() == 1 && c.getBuffDmg() != 0)
            {
                healing_received_logs.Add(new DamageLogCondition(time, c));
            }
            else if (c.isBuff() == 0 && c.getValue() >= 0)
            {
                healing_received_logs.Add(new DamageLogPower(time, c));

            }
        }*/
        // Setters

        protected virtual void SetDamageTakenLogs(ParsedLog log)
        {
            _damageTakenlogs.AddRange(log.CombatData.GetDamageTakenData(AgentItem));
        }

        protected virtual void SetCastLogs(ParsedLog log)
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


        protected abstract void SetDamageLogs(ParsedLog log);
        protected abstract void SetBoonStatusCleanseWasteData(ParsedLog log, BoonSimulator simulator, long boonid, bool updateCondiPresence);
        protected abstract void SetBoonStatusGenerationData(ParsedLog log, BoonSimulationItem simul, long boonid);
        protected abstract void InitBoonStatusData(ParsedLog log);

        protected void SetBoonStatus(ParsedLog log)
        {
            BoonPoints = new Dictionary<long, BoonsGraphModel>();
            BoonMap toUse = GetBoonMap(log);
            long dur = log.FightData.FightDuration;
            int fightDuration = (int)(dur) / 1000;
            BoonsGraphModel boonPresenceGraph = new BoonsGraphModel(log.Boons.BoonsByIds[ProfHelper.NumberOfBoonsID]);
            BoonsGraphModel condiPresenceGraph = new BoonsGraphModel(log.Boons.BoonsByIds[ProfHelper.NumberOfConditionsID]);
            HashSet<long> boonIds = new HashSet<long>(log.Boons.GetBoonList().Select(x => x.ID));
            HashSet<long> condiIds = new HashSet<long>(log.Boons.GetConditionList().Select(x => x.ID));
            InitBoonStatusData(log);

            DeadEvent death = log.CombatData.GetDeadEvents(AgentItem).LastOrDefault();
            DespawnEvent dc = log.CombatData.GetDespawnEvents(AgentItem).LastOrDefault();
            foreach (Boon boon in TrackedBoons)
            {
                long boonid = boon.ID;
                if (toUse.TryGetValue(boonid, out List<AbstractBuffEvent> logs) && logs.Count != 0)
                {
                    if (BoonPoints.ContainsKey(boonid))
                    {
                        continue;
                    }
                    BoonSimulator simulator = boon.CreateSimulator(log);
                    simulator.Simulate(logs, dur);
                    if (death != null && GetCastLogs(log, death.Time + 5000, dur).Count == 0)
                    {
                        simulator.Trim(death.Time);
                    }
                    else if (dc != null && GetCastLogs(log, dc.Time + 5000, dur).Count == 0)
                    {
                        simulator.Trim(dc.Time);
                    }
                    else
                    {
                        simulator.Trim(dur);
                    }
                    bool updateBoonPresence = boonIds.Contains(boonid);
                    bool updateCondiPresence = condiIds.Contains(boonid);
                    List<BoonsGraphModel.SegmentWithSources> graphSegments = new List<BoonsGraphModel.SegmentWithSources>();
                    foreach (BoonSimulationItem simul in simulator.GenerationSimulation)
                    {
                        SetBoonStatusGenerationData(log, simul, boonid);
                        BoonsGraphModel.SegmentWithSources segment = simul.ToSegment();
                        if (graphSegments.Count == 0)
                        {
                            graphSegments.Add(new BoonsGraphModel.SegmentWithSources(0, segment.Start, 0, GeneralHelper.UnknownAgent));
                        }
                        else if (graphSegments.Last().End != segment.Start)
                        {
                            graphSegments.Add(new BoonsGraphModel.SegmentWithSources(graphSegments.Last().End, segment.Start, 0, GeneralHelper.UnknownAgent));
                        }
                        graphSegments.Add(segment);
                    }
                    SetBoonStatusCleanseWasteData(log, simulator, boonid, updateCondiPresence);
                    if (graphSegments.Count > 0)
                    {
                        graphSegments.Add(new BoonsGraphModel.SegmentWithSources(graphSegments.Last().End, dur, 0, GeneralHelper.UnknownAgent));
                    }
                    else
                    {
                        graphSegments.Add(new BoonsGraphModel.SegmentWithSources(0, dur, 0, GeneralHelper.UnknownAgent));
                    }
                    BoonPoints[boonid] = new BoonsGraphModel(boon, graphSegments);
                    if (updateBoonPresence || updateCondiPresence)
                    {
                        List<BoonsGraphModel.Segment> segmentsToFill = updateBoonPresence ? boonPresenceGraph.BoonChart : condiPresenceGraph.BoonChart;
                        bool firstPass = segmentsToFill.Count == 0;
                        foreach (BoonsGraphModel.Segment seg in BoonPoints[boonid].BoonChart)
                        {
                            long start = seg.Start;
                            long end = seg.End;
                            int value = seg.Value > 0 ? 1 : 0;
                            if (firstPass)
                            {
                                segmentsToFill.Add(new BoonsGraphModel.Segment(start, end, value));
                            }
                            else
                            {
                                for (int i = 0; i < segmentsToFill.Count; i++)
                                {
                                    BoonsGraphModel.Segment curSeg = segmentsToFill[i];
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
                                        segmentsToFill.Insert(i + 1, new BoonsGraphModel.Segment(start, end, curVal + value));
                                        segmentsToFill.Insert(i + 2, new BoonsGraphModel.Segment(end, curEnd, curVal));
                                        break;
                                    }
                                    else
                                    {
                                        curSeg.End = start;
                                        segmentsToFill.Insert(i + 1, new BoonsGraphModel.Segment(start, curEnd, curVal + value));
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
            BoonPoints[ProfHelper.NumberOfBoonsID] = boonPresenceGraph;
            BoonPoints[ProfHelper.NumberOfConditionsID] = condiPresenceGraph;
        }
        //protected abstract void setHealingLogs(ParsedLog log);
        //protected abstract void setHealingReceivedLogs(ParsedLog log);
    }
}
