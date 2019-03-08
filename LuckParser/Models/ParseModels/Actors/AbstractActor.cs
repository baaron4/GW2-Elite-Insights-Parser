using System;
using System.Collections.Generic;
using System.Linq;
using LuckParser.Parser;

namespace LuckParser.Models.ParseModels
{
    public abstract class AbstractActor : DummyActor
    {
        // Damage
        protected List<DamageLog> DamageLogs;
        protected Dictionary<AgentItem, List<DamageLog>> DamageLogsByDst;
        //protected List<DamageLog> HealingLogs = new List<DamageLog>();
        //protected List<DamageLog> HealingReceivedLogs = new List<DamageLog>();
        private List<DamageLog> _damageTakenlogs;
        protected Dictionary<AgentItem, List<DamageLog>> _damageTakenLogsBySrc;
        // Cast
        protected List<CastLog> CastLogs;
        // Boons
        public HashSet<Boon> TrackedBoons { get; } = new HashSet<Boon>();
        protected Dictionary<long, BoonsGraphModel> BoonPoints;

        protected AbstractActor(AgentItem agent) : base(agent)
        {
        }
        // Getters

        public long GetDeath(ParsedLog log, long start, long end)
        {
            CombatItem dead = log.CombatData.GetStatesData(InstID, ParseEnum.StateChange.ChangeDead, Math.Max(log.FightData.ToLogSpace(start), FirstAware), Math.Min(log.FightData.ToLogSpace(end), LastAware)).LastOrDefault();
            if (dead != null && dead.Time > 0)
            {
                return log.FightData.ToFightSpace(dead.Time);
            }
            return 0;
        }

        public List<DamageLog> GetDamageLogs(AbstractActor target, ParsedLog log, long start, long end)
        {
            if (DamageLogs == null)
            {
                DamageLogs = new List<DamageLog>();
                SetDamageLogs(log);
                DamageLogsByDst = DamageLogs.GroupBy(x => log.AgentData.GetAgentByInstID(x.DstInstId, log.FightData.ToLogSpace(x.Time))).ToDictionary(x => x.Key, x => x.ToList());
            }
            if (target != null)
            {
                if (DamageLogsByDst.TryGetValue(target.AgentItem, out var list))
                {
                    return list.Where(x => x.Time >= start && x.Time <= end).ToList();
                }
                else
                {
                    return new List<DamageLog>();
                }
            }
            return DamageLogs.Where( x => x.Time >= start && x.Time <= end).ToList();
        }
        public List<DamageLog> GetDamageTakenLogs(AbstractActor target, ParsedLog log, long start, long end)
        {
            if (_damageTakenlogs == null)
            {
                _damageTakenlogs = new List<DamageLog>();
                SetDamageTakenLogs(log);
                _damageTakenLogsBySrc = _damageTakenlogs.GroupBy(x => log.AgentData.GetAgentByInstID(x.SrcInstId, log.FightData.ToLogSpace(x.Time))).ToDictionary(x => x.Key, x => x.ToList());
            }
            if (target != null)
            {
                if (_damageTakenLogsBySrc.TryGetValue(target.AgentItem, out var list))
                {
                    long targetStart = log.FightData.ToFightSpace(target.FirstAware);
                    long targetEnd = log.FightData.ToFightSpace(target.LastAware);
                    return list.Where(x => x.Time >= start && x.Time >= targetStart && x.Time <= end && x.Time <= targetEnd).ToList();
                }
                else
                {
                    return new List<DamageLog>();
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
        public List<CastLog> GetCastLogs(ParsedLog log, long start, long end)
        {
            if (CastLogs == null)
            {
                CastLogs = new List<CastLog>();
                SetCastLogs(log);
            }
            return CastLogs.Where(x => x.Time >= start && x.Time <= end).ToList();

        }

        public List<CastLog> GetCastLogsActDur(ParsedLog log, long start, long end)
        {
            if (CastLogs == null)
            {
                CastLogs = new List<CastLog>();
                SetCastLogs(log);
            }
            return CastLogs.Where(x => x.Time + x.ActualDuration >= start && x.Time <= end).ToList();

        }
        // privates
        protected void AddDamageLog(long time, CombatItem c)
        {        
            if (c.IFF == ParseEnum.IFF.Friend)
            {
                return;
            }
            if (c.IsBuff != 0)//condi
            {
                DamageLogs.Add(new DamageLogCondition(time, c));
            }
            else if (c.IsBuff == 0)//power
            {
                DamageLogs.Add(new DamageLogPower(time, c));
            }
        }
        protected void AddDamageTakenLog(long time, CombatItem c)
        {
            if (c.IsBuff != 0)
            {
                //inco,ing condi dmg not working or just not present?
                // damagetaken.Add(c.getBuffDmg());
                _damageTakenlogs.Add(new DamageLogCondition(time, c));
            }
            else if (c.IsBuff == 0)
            {
                _damageTakenlogs.Add(new DamageLogPower(time, c));

            }
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
            // buff extension ids
            BoonSourceFinder sourceFinder = Boon.GetBoonSourceFinder(log);
            //
            BoonMap boonMap = new BoonMap();
            // Fill in Boon Map
            foreach (CombatItem c in log.CombatData.GetBoonDataByDst(InstID, FirstAware, LastAware))
            {
                long boonId = c.SkillID;
                if (!boonMap.ContainsKey(boonId))
                {
                    if (!Boon.BoonsByIds.ContainsKey(boonId))
                    {
                        continue;
                    }
                    boonMap.Add(Boon.BoonsByIds[boonId]);
                }
                if (c.IsBuffRemove == ParseEnum.BuffRemove.Manual
                    || (c.IsBuffRemove == ParseEnum.BuffRemove.Single && c.IFF == ParseEnum.IFF.Unknown && c.DstInstid == 0)
                    || (c.IsBuffRemove != ParseEnum.BuffRemove.None && c.Value <= 50))
                {
                    continue;
                }
                long time = log.FightData.ToFightSpace(c.Time);
                List<BoonLog> loglist = boonMap[boonId];
                if (c.IsStateChange == ParseEnum.StateChange.BuffInitial)
                {
                    ushort src = c.SrcMasterInstid > 0 ? c.SrcMasterInstid : c.SrcInstid;
                    loglist.Add(new BoonApplicationLog(time, log.AgentData.GetAgentByInstID(src, c.Time), c.Value));
                }
                else if (c.IsStateChange != ParseEnum.StateChange.BuffInitial)
                {
                    if (c.IsBuffRemove == ParseEnum.BuffRemove.None)
                    {
                        ushort src = c.SrcMasterInstid > 0 ? c.SrcMasterInstid : c.SrcInstid;
                        if (c.IsOffcycle > 0)
                        {
                            if (src == 0)
                            {
                                src = sourceFinder.TryFindSrc(this, time, c.Value, log);
                            }
                            loglist.Add(new BoonExtensionLog(time, c.Value, c.OverstackValue - c.Value, log.AgentData.GetAgentByInstID(src, c.Time)));
                        }
                        else
                        {
                            loglist.Add(new BoonApplicationLog(time, log.AgentData.GetAgentByInstID(src, c.Time), c.Value));
                        }
                    }
                    else if (time < log.FightData.FightDuration - 50)
                    {
                        ushort src = c.DstMasterInstid > 0 ? c.DstMasterInstid : c.DstInstid;
                        loglist.Add(new BoonRemovalLog(time, log.AgentData.GetAgentByInstID(src, c.Time), c.Value, c.IsBuffRemove));
                    }
                }
            }
            //boonMap.Sort();
            foreach (var pair in boonMap)
            {
                TrackedBoons.Add(Boon.BoonsByIds[pair.Key]);
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
            foreach (CombatItem c in log.CombatData.GetDamageTakenData(InstID, FirstAware, LastAware))
            {
                long time = log.FightData.ToFightSpace(c.Time);
                AddDamageTakenLog(time, c);
            }
        }

        protected virtual void SetCastLogs(ParsedLog log)
        {
            CastLog curCastLog = null;
            foreach (CombatItem c in log.CombatData.GetCastData(InstID, FirstAware, LastAware))
            {
                ParseEnum.StateChange state = c.IsStateChange;
                if (state == ParseEnum.StateChange.Normal)
                {
                    if (c.IsActivation.StartCasting())
                    {
                        // Missing end activation
                        long time = log.FightData.ToFightSpace(c.Time);
                        if (curCastLog != null)
                        {
                            int actDur = curCastLog.SkillId == SkillItem.DodgeId ? 750 : curCastLog.ExpectedDuration;
                            curCastLog.SetEndStatus(actDur, ParseEnum.Activation.Unknown, time);
                            curCastLog = null;
                        }
                        curCastLog = new CastLog(time, c.SkillID, c.Value, c.IsActivation, Agent, InstID);
                        CastLogs.Add(curCastLog);
                    }
                    else
                    {
                        if (curCastLog != null)
                        {
                            if (curCastLog.SkillId == c.SkillID)
                            {
                                int actDur = curCastLog.SkillId == SkillItem.DodgeId ? 750 : c.Value;
                                curCastLog.SetEndStatus(actDur, c.IsActivation, log.FightData.FightDuration);
                                curCastLog = null;
                            }
                        }
                    }
                }
                else if (state == ParseEnum.StateChange.WeaponSwap)
                {
                    long time = log.FightData.ToFightSpace(c.Time);
                    CastLog swapLog = new CastLog(time, SkillItem.WeaponSwapId, (int)c.DstAgent, c.IsActivation, Agent, InstID);
                    if (CastLogs.Count > 0 && (time - CastLogs.Last().Time) < 10 && CastLogs.Last().SkillId == SkillItem.WeaponSwapId)
                    {
                        CastLogs[CastLogs.Count - 1] = swapLog;
                    }
                    else
                    {
                        CastLogs.Add(swapLog);
                    }
                    swapLog.SetEndStatus(50, ParseEnum.Activation.Unknown, log.FightData.FightDuration);
                }
            }
            long cloakStart = 0;
            foreach (long time in log.CombatData.GetBuffs(InstID, 40408, FirstAware, LastAware).Select(x => log.FightData.ToFightSpace(x.Time)))
            {
                if (time - cloakStart > 10)
                {
                    CastLog dodgeLog = new CastLog(time, SkillItem.DodgeId, 0, ParseEnum.Activation.Unknown, Agent, InstID);
                    dodgeLog.SetEndStatus(50, ParseEnum.Activation.Unknown, log.FightData.FightDuration);
                    CastLogs.Add(dodgeLog);
                }
                cloakStart = time;
            }
            CastLogs.Sort((x, y) => x.Time.CompareTo(y.Time));
        }


        protected abstract void SetDamageLogs(ParsedLog log);
        protected abstract void SetBoonStatusCleanseWasteData(ParsedLog log, BoonSimulator simulator, long boonid, bool updateCondiPresence);
        protected abstract void SetBoonStatusGenerationData(ParsedLog log, BoonSimulationItem simul, long boonid, bool updateBoonPresence, bool updateCondiPresence);
        protected abstract void InitBoonStatusData(ParsedLog log);

        protected void SetBoonStatus(ParsedLog log)
        {
            BoonPoints = new Dictionary<long, BoonsGraphModel>();
            BoonMap toUse = GetBoonMap(log);
            long dur = log.FightData.FightDuration;
            int fightDuration = (int)(dur) / 1000;
            BoonsGraphModel boonPresenceGraph = new BoonsGraphModel(Boon.BoonsByIds[Boon.NumberOfBoonsID]);
            BoonsGraphModel condiPresenceGraph = new BoonsGraphModel(Boon.BoonsByIds[Boon.NumberOfConditionsID]);
            HashSet<long> boonIds = new HashSet<long>(Boon.GetBoonList().Select(x => x.ID));
            HashSet<long> condiIds = new HashSet<long>(Boon.GetCondiBoonList().Select(x => x.ID));
            InitBoonStatusData(log);

            long death = GetDeath(log, 0, dur);
            foreach (Boon boon in TrackedBoons)
            {
                long boonid = boon.ID;
                if (toUse.TryGetValue(boonid, out List<BoonLog> logs) && logs.Count != 0)
                {
                    if (BoonPoints.ContainsKey(boonid))
                    {
                        continue;
                    }
                    BoonSimulator simulator = boon.CreateSimulator(log);
                    simulator.Simulate(logs, dur);
                    if (death > 0 && GetCastLogs(log, death + 5000, dur).Count == 0)
                    {
                        simulator.Trim(death);
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
                        SetBoonStatusGenerationData(log, simul, boonid, updateBoonPresence, updateCondiPresence);
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
            BoonPoints[Boon.NumberOfBoonsID] = boonPresenceGraph;
            BoonPoints[Boon.NumberOfConditionsID] = condiPresenceGraph;
        }
        //protected abstract void setHealingLogs(ParsedLog log);
        //protected abstract void setHealingReceivedLogs(ParsedLog log);
    }
}
