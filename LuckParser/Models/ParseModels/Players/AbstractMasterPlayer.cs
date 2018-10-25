using LuckParser.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public abstract class AbstractMasterPlayer : AbstractPlayer
    {
        public class ExtraBoonData
        {
            public int HitCount { get; }
            public int TotalHitCount { get; }
            public int DamageGain { get; }
            public double Percent { get; }
            public ExtraBoonData (int hitCount, int totalHitCount, int damageGain, double percent)
            {
                HitCount = hitCount;
                TotalHitCount = totalHitCount;
                DamageGain = damageGain;
                Percent = percent;
            }
        };
        // Boons
        public List<Boon> BoonToTrack { get; } = new List<Boon>();
        private readonly List<BoonDistribution> _boonDistribution = new List<BoonDistribution>();
        private readonly List<Dictionary<long, long>> _boonPresence = new List<Dictionary<long, long>>();
        private readonly List<Dictionary<long, long>> _condiPresence = new List<Dictionary<long, long>>();
        private readonly List<Dictionary<ushort, Dictionary<long,List<long>>>> _condiCleanse = new List<Dictionary<ushort, Dictionary<long, List<long>>>>();
        private readonly Dictionary<long, BoonsGraphModel> _boonPoints = new Dictionary<long, BoonsGraphModel>();
        private readonly Dictionary<long, List<ExtraBoonData>> _boonExtra = new Dictionary<long, List<ExtraBoonData>>();
        private readonly Dictionary<Boss, Dictionary<long, List<ExtraBoonData>>> _boonTargetExtra = new Dictionary<Boss, Dictionary<long, List<ExtraBoonData>>>();
        // dps graphs
        public Dictionary<int, List<Point>> DpsGraph { get; } = new Dictionary<int, List<Point>>();
        // Minions
        private readonly Dictionary<string, Minions> _minions = new Dictionary<string, Minions>();
        // Replay
        public CombatReplay CombatReplay { get; protected set; }

        protected AbstractMasterPlayer(AgentItem agent) : base(agent)
        {

        }

        public Dictionary<string, Minions> GetMinions(ParsedLog log)
        {
            if (_minions.Count == 0)
            {
                SetMinions(log);
            }
            return _minions;
        }

        public List<Point> GetDPSGraph(int id)
        {
            if (DpsGraph.TryGetValue(id, out List<Point> res))
            {
                return res;
            }
            return new List<Point>();
        }
        public BoonDistribution GetBoonDistribution(ParsedLog log, int phaseIndex)
        {
            if (_boonDistribution.Count == 0)
            {
                SetBoonDistribution(log);
            }
            return _boonDistribution[phaseIndex];
        }
        public Dictionary<long, BoonsGraphModel> GetBoonGraphs(ParsedLog log)
        {
            if (_boonDistribution.Count == 0)
            {
                SetBoonDistribution(log);
            }
            return _boonPoints;
        }
        public Dictionary<long, long> GetBoonPresence(ParsedLog log, int phaseIndex)
        {
            if (_boonDistribution.Count == 0)
            {
                SetBoonDistribution(log);
            }
            return _boonPresence[phaseIndex];
        }

        protected Dictionary<long, List<long>> GetCondiCleanse(ParsedLog log, int phaseIndex, ushort src)
        {
            if (_condiCleanse.Count == 0)
            {
                SetBoonDistribution(log);
            }
            if (_condiCleanse[phaseIndex].TryGetValue(src,out Dictionary<long,List<long>> dict))
            {
                return dict;
            }
            return new Dictionary<long, List<long>>();
        }

        public Dictionary<long, List<ExtraBoonData>> GetExtraBoonData(ParsedLog log, Boss target)
        {
            if (_boonDistribution.Count == 0)
            {
                SetBoonDistribution(log);
            }
            if (target != null)
            {
                if (_boonTargetExtra.TryGetValue(target, out var res))
                {
                    return res;
                }
                else
                {
                    return new Dictionary<long, List<ExtraBoonData>>();
                }
            }
            return _boonExtra;
        }

        public Dictionary<long, long> GetCondiPresence(ParsedLog log, int phaseIndex)
        {
            if (_boonDistribution.Count == 0)
            {
                SetBoonDistribution(log);
            }
            return _condiPresence[phaseIndex];
        }
        public void InitCombatReplay(ParsedLog log, int pollingRate, bool trim, bool forceInterpolate)
        {
            if (!log.FightData.Logic.CanCombatReplay)
            {
                // no combat replay support on boss
                return;
            }
            if (CombatReplay == null)
            {
                CombatReplay = new CombatReplay();
                SetMovements(log);
                CombatReplay.PollingRate(pollingRate, log.FightData.FightDuration, forceInterpolate);
                if (trim)
                {
                    CombatItem despawnCheck = log.CombatData.AllCombatItems.FirstOrDefault(x => x.SrcAgent == AgentItem.Agent && (x.IsStateChange.IsDead() || x.IsStateChange.IsDespawn()));
                    if (despawnCheck != null)
                    {
                        CombatReplay.Trim(AgentItem.FirstAware - log.FightData.FightStart, despawnCheck.Time - log.FightData.FightStart);
                    }
                    else
                    {
                        CombatReplay.Trim(AgentItem.FirstAware - log.FightData.FightStart, AgentItem.LastAware - log.FightData.FightStart);
                    }
                }
                //SetAdditionalCombatReplayData(log);
            }
        }

        public void ComputeAdditionalCombatReplayData(ParsedLog log)
        {
            if (CombatReplay != null && CombatReplay.Actors.Count == 0)
            {
                SetAdditionalCombatReplayData(log);
            }
        }

        public long GetDeath(ParsedLog log, long start, long end)
        {
            long offset = log.FightData.FightStart;
            CombatItem dead = log.CombatData.GetStatesData(ParseEnum.StateChange.ChangeDead).LastOrDefault(x => x.SrcInstid == InstID && x.Time >= start + offset && x.Time <= end + offset);
            if (dead != null && dead.Time > 0)
            {
                return dead.Time;
            }
            return 0;
        }
        // private getters
        private BoonMap GetBoonMap(ParsedLog log)
        {
            BoonMap boonMap = new BoonMap
            {
                BoonToTrack
            };
            // Fill in Boon Map
            long timeStart = log.FightData.FightStart;
            long agentStart = Math.Max(FirstAware - log.FightData.FightStart,0);
            long agentEnd = Math.Min(LastAware - log.FightData.FightStart, log.FightData.FightDuration);
            foreach (CombatItem c in log.GetBoonDataByDst(InstID))
            {
                long boonId = c.SkillID;
                if (!boonMap.ContainsKey(boonId))
                {
                    continue;
                }
                long time = c.Time - timeStart;
                List<BoonLog> loglist = boonMap[boonId];
                if (c.IsStateChange == ParseEnum.StateChange.BuffInitial && c.Value > 0)
                {
                    ushort src = c.SrcMasterInstid > 0 ? c.SrcMasterInstid : c.SrcInstid;
                    loglist.Add(new BoonApplicationLog(0, src, c.Value));
                }
                else if (c.IsStateChange != ParseEnum.StateChange.BuffInitial && time >= agentStart && time < agentEnd)
                {
                    if (c.IsBuffRemove == ParseEnum.BuffRemove.None)
                    {
                        ushort src = c.SrcMasterInstid > 0 ? c.SrcMasterInstid : c.SrcInstid;
                        loglist.Add(new BoonApplicationLog(time, src, c.Value));
                    }
                    else if (c.IsBuffRemove != ParseEnum.BuffRemove.Manual && time < log.FightData.FightDuration - 50)
                    {
                        loglist.Add(new BoonRemovalLog(time, c.DstInstid, c.Value, c.IsBuffRemove));
                    }
                }
            }
            boonMap.Sort();
            return boonMap;
        }
        // private setters
        private void SetMovements(ParsedLog log)
        {
            foreach (CombatItem c in log.GetMovementData(AgentItem.InstID))
            {
                if (c.Time < FirstAware || c.Time > LastAware)
                {
                    continue;
                }
                long time = c.Time - log.FightData.FightStart;
                byte[] xy = BitConverter.GetBytes(c.DstAgent);
                float x = BitConverter.ToSingle(xy, 0);
                float y = BitConverter.ToSingle(xy, 4);
                if (c.IsStateChange == ParseEnum.StateChange.Position)
                {
                    CombatReplay.Positions.Add(new Point3D(x, y, c.Value, time));
                }
                else if (c.IsStateChange == ParseEnum.StateChange.Velocity)
                {
                    CombatReplay.Velocities.Add(new Point3D(x, y, c.Value, time));
                }
                else if (c.IsStateChange == ParseEnum.StateChange.Rotation)
                {
                    CombatReplay.Rotations.Add(new Point3D(x, y, c.Value, time));
                }
            }
        }
        private void GenerateExtraBoonData(ParsedLog log, long boonid, GenerationSimulationResult buffSimulationGeneration, List<PhaseData> phases)
        {
            switch (boonid)
            {
                // Frost Spirit
                case 50421:
                    foreach (Boss target in log.FightData.Logic.Targets)
                    {
                        if (!_boonTargetExtra.TryGetValue(target, out var extra))
                        {
                            _boonTargetExtra[target] = new Dictionary<long, List<ExtraBoonData>>();
                        }
                        Dictionary<long, List<ExtraBoonData>> dict = _boonTargetExtra[target];
                        if (!dict.TryGetValue(boonid, out var list))
                        {
                            List<ExtraBoonData> extraDataList = new List<ExtraBoonData>();
                            for (int i = 0; i < phases.Count; i++)
                            {
                                List<DamageLog> dmLogs = GetJustPlayerDamageLogs(target, log, phases[i].Start, phases[i].End);
                                int totalDamage = Math.Max(dmLogs.Sum(x => x.Damage), 1);
                                List<DamageLog> effect = dmLogs.Where(x => buffSimulationGeneration.GetStackCount((int)x.Time) > 0 && x.IsCondi == 0).ToList();
                                int damage = (int)(effect.Sum(x => x.Damage) / 21.0);
                                double gain = Math.Round(100.0 * ((double)totalDamage / (totalDamage - damage) - 1.0), 2);
                                extraDataList.Add(new ExtraBoonData(effect.Count, dmLogs.Count(x => x.IsCondi == 0), damage, gain));
                            }
                            dict[boonid] = extraDataList;
                        }                
                    }
                    _boonExtra[boonid] = new List<ExtraBoonData>();
                    for (int i = 0; i < phases.Count; i++)
                    {
                        List<DamageLog> dmLogs = GetJustPlayerDamageLogs(null, log, phases[i].Start, phases[i].End);
                        int totalDamage = Math.Max(dmLogs.Sum(x => x.Damage), 1);
                        List<DamageLog> effect = dmLogs.Where(x => buffSimulationGeneration.GetStackCount((int)x.Time) > 0 && x.IsCondi == 0).ToList();
                        int damage = (int)(effect.Sum(x => x.Damage) / 21.0);
                        double gain = Math.Round(100.0 * ((double)totalDamage / (totalDamage - damage) - 1.0), 2);
                        _boonExtra[boonid].Add(new ExtraBoonData(effect.Count, dmLogs.Count(x => x.IsCondi == 0), damage, gain));
                    }
                    break;
                // GoE
                case 31803:
                    foreach (Boss target in log.FightData.Logic.Targets)
                    {
                        if (!_boonTargetExtra.TryGetValue(target, out var extra))
                        {
                            _boonTargetExtra[target] = new Dictionary<long, List<ExtraBoonData>>();
                        }
                        Dictionary<long, List<ExtraBoonData>> dict = _boonTargetExtra[target];
                        if (!dict.TryGetValue(boonid, out var list))
                        {
                            List<ExtraBoonData> extraDataList = new List<ExtraBoonData>();
                            for (int i = 0; i < phases.Count; i++)
                            {
                                List<DamageLog> dmLogs = GetJustPlayerDamageLogs(target, log, phases[i].Start, phases[i].End);
                                int totalDamage = Math.Max(dmLogs.Sum(x => x.Damage), 1);
                                List<DamageLog> effect = dmLogs.Where(x => buffSimulationGeneration.GetStackCount((int)x.Time) > 0 && x.IsCondi == 0).ToList();
                                int damage = (int)(effect.Sum(x => x.Damage) / 11.0);
                                double gain = Math.Round(100.0 * ((double)totalDamage / (totalDamage - damage) - 1.0), 2);
                                extraDataList.Add(new ExtraBoonData(effect.Count, dmLogs.Count(x => x.IsCondi == 0), damage, gain));
                            }
                            dict[boonid] = extraDataList;
                        }

                    }
                    _boonExtra[boonid] = new List<ExtraBoonData>();
                    for (int i = 0; i < phases.Count; i++)
                    {
                        List<DamageLog> dmLogs = GetJustPlayerDamageLogs(null, log, phases[i].Start, phases[i].End);
                        int totalDamage = Math.Max(dmLogs.Sum(x => x.Damage), 1);
                        List<DamageLog> effect = dmLogs.Where(x => buffSimulationGeneration.GetStackCount((int)x.Time) > 0 && x.IsCondi == 0).ToList();
                        int damage = (int)(effect.Sum(x => x.Damage) / 11.0);
                        double gain = Math.Round(100.0 * ((double)totalDamage / (totalDamage - damage) - 1.0), 2);
                        _boonExtra[boonid].Add(new ExtraBoonData(effect.Count, dmLogs.Count(x => x.IsCondi == 0), damage, gain));
                    }
                    break;
            }
        }
        private void SetBoonDistribution(ParsedLog log)
        {
            List<PhaseData> phases = log.FightData.GetPhases(log);
            BoonMap toUse = GetBoonMap(log);
            long dur = log.FightData.FightDuration;
            int fightDuration = (int)(dur) / 1000;
            HashSet<long> extraDataID = new HashSet<long>
            {
                50421,
                31803
            };
            BoonsGraphModel boonPresenceGraph = new BoonsGraphModel("Number of Boons");
            BoonsGraphModel condiPresenceGraph = new BoonsGraphModel("Number of Conditions");
            HashSet<long> boonIds = new HashSet<long>(Boon.GetBoonList().Select(x => x.ID));
            HashSet<long> condiIds = new HashSet<long>(Boon.GetCondiBoonList().Select(x => x.ID));
            for (int i = 0; i < phases.Count; i++)
            {
                _boonDistribution.Add(new BoonDistribution());
                _boonPresence.Add(new Dictionary<long, long>());
                _condiPresence.Add(new Dictionary<long, long>());
                _condiCleanse.Add(new Dictionary<ushort, Dictionary<long, List<long>>>());
            }

            long death = GetDeath(log, 0, dur) - log.FightData.FightStart;
            foreach (Boon boon in BoonToTrack)
            {
                long boonid = boon.ID;
                if (toUse.TryGetValue(boonid, out var logs) && logs.Count != 0)
                {
                    if (_boonDistribution[0].ContainsKey(boonid))
                    {
                        continue;
                    }
                    bool requireExtraData = extraDataID.Contains(boonid);
                    var simulator = boon.CreateSimulator(log);
                    simulator.Simulate(logs, dur);
                    if (death > 0 && GetCastLogs(log, death + 5000, dur).Count == 0)
                    {
                        simulator.Trim(death);
                    }
                    else
                    {
                        simulator.Trim(dur);
                    }
                    var updateBoonPresence = boonIds.Contains(boonid);
                    var updateCondiPresence = boonid != 873 && condiIds.Contains(boonid);
                    var generationSimulation = simulator.GenerationSimulationResult;
                    var graphSegments = new List<BoonsGraphModel.Segment>();
                    foreach (var simul in generationSimulation.Items)
                    {
                        for (int i = 0; i < phases.Count; i++)
                        {
                            var phase = phases[i];
                            if (!_boonDistribution[i].TryGetValue(boonid, out var distrib))
                            {
                                distrib = new Dictionary<ushort, OverAndValue>();
                                _boonDistribution[i].Add(boonid, distrib);
                            }
                            if (updateBoonPresence)
                                Add(_boonPresence[i], boonid, simul.GetClampedDuration(phase.Start, phase.End));
                            if (updateCondiPresence)
                                Add(_condiPresence[i], boonid, simul.GetClampedDuration(phase.Start, phase.End));
                            foreach (ushort src in simul.GetSrc())
                            {
                                if (distrib.TryGetValue(src, out var toModify))
                                {
                                    toModify.Value += simul.GetSrcDuration(src, phase.Start, phase.End);
                                    distrib[src] = toModify;
                                }
                                else
                                {
                                    distrib.Add(src, new OverAndValue(
                                        simul.GetSrcDuration(src, phase.Start, phase.End),
                                        0));
                                }
                            }
                        }
                        List<BoonsGraphModel.Segment> segments = simul.ToSegment();
                        if (segments.Count > 0)
                        {
                            if (graphSegments.Count == 0)
                            {
                                graphSegments.Add(new BoonsGraphModel.Segment(0, segments.First().Start, 0));
                            } else if (graphSegments.Last().End != segments.First().Start)
                            {
                                graphSegments.Add(new BoonsGraphModel.Segment(graphSegments.Last().End, segments.First().Start, 0));
                            }
                            graphSegments.AddRange(simul.ToSegment());
                        }
                    }
                    foreach (var simul in simulator.OverstackSimulationResult)
                    {
                        for (int i = 0; i < phases.Count; i++)
                        {
                            var phase = phases[i];
                            if (!_boonDistribution[i].TryGetValue(boonid, out var distrib))
                            {
                                distrib = new Dictionary<ushort, OverAndValue>();
                                _boonDistribution[i].Add(boonid, distrib);
                            }
                            if (distrib.TryGetValue(simul.Src, out var toModify))
                            {
                                toModify.Overstack += simul.GetOverstack(phase.Start, phase.End);
                                distrib[simul.Src] = toModify;
                            }
                            else
                            {
                                distrib.Add(simul.Src, new OverAndValue(
                                    0,
                                    simul.GetOverstack(phase.Start, phase.End)));
                            }
                        }
                    }

                    if (updateCondiPresence)
                    {
                        foreach (var simul in simulator.CleanseSimulationResult)
                        {
                            for (int i = 0; i < phases.Count; i++)
                            {
                                var phase = phases[i];
                                long cleanse = simul.GetCleanseDuration(phase.Start, phase.End);
                                if (cleanse > 0)
                                {
                                    if (!_condiCleanse[i].TryGetValue(simul.ProvokedBy, out var dict))
                                    {
                                        dict = new Dictionary<long, List<long>>();
                                        _condiCleanse[i].Add(simul.ProvokedBy, dict);
                                    }
                                    if (!dict.TryGetValue(boonid, out var list))
                                    {
                                        list = new List<long>();
                                        dict.Add(boonid, list);
                                    }
                                    list.Add(cleanse);
                                }
                            }
                        }
                    }
                    if (requireExtraData)
                    {
                        GenerateExtraBoonData(log, boonid, generationSimulation, phases);
                    }
                    if (graphSegments.Count > 0)
                    {
                        graphSegments.Add(new BoonsGraphModel.Segment(graphSegments.Last().End, dur, 0));
                    } else
                    {
                        graphSegments.Add(new BoonsGraphModel.Segment(0, dur, 0));
                    }
                    _boonPoints[boonid] = new BoonsGraphModel(boon.Name, graphSegments);
                    if (updateBoonPresence || updateCondiPresence)
                    {
                        List<BoonsGraphModel.Segment> segmentsToFill = updateBoonPresence ? boonPresenceGraph.BoonChart : condiPresenceGraph.BoonChart;
                        bool firstPass = segmentsToFill.Count == 0;
                        foreach (BoonsGraphModel.Segment seg in _boonPoints[boonid].BoonChart)
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
            _boonPoints[-2] = boonPresenceGraph;
            _boonPoints[-3] = condiPresenceGraph;
        }
        private void SetMinions(ParsedLog log)
        {
            List<AgentItem> combatMinion = log.AgentData.GetAgentByType(AgentItem.AgentType.NPC).Where(x => x.MasterAgent == AgentItem.Agent).ToList();
            Dictionary<string, Minions> auxMinions = new Dictionary<string, Minions>();
            foreach (AgentItem agent in combatMinion)
            {
                string id = agent.Name;
                if (!auxMinions.ContainsKey(id))
                {
                    auxMinions[id] = new Minions(id.GetHashCode());
                }
                auxMinions[id].Add(new Minion(agent));
            }
            foreach (KeyValuePair<string, Minions> pair in auxMinions)
            {
                if (pair.Value.GetDamageLogs(null, log, 0, log.FightData.FightDuration).Count > 0)
                {
                    _minions[pair.Key] = pair.Value;
                }
            }
        }
        protected override void SetDamageLogs(ParsedLog log)
        {
            long agentStart = Math.Max(FirstAware, log.FightData.FightStart);
            long agentEnd = Math.Min(LastAware, log.FightData.FightEnd);
            long timeStart = log.FightData.FightStart;
            foreach (CombatItem c in log.GetDamageData(AgentItem.InstID))
            {
                if (c.Time > agentStart && c.Time < agentEnd)//selecting player or minion as caster
                {
                    long time = c.Time - timeStart;
                    AddDamageLog(time, c);
                }
            }
            Dictionary<string, Minions> minionsList = GetMinions(log);
            foreach (Minions mins in minionsList.Values)
            {
                DamageLogs.AddRange(mins.GetDamageLogs(null, log, 0, log.FightData.FightDuration));
            }
            DamageLogs.Sort((x, y) => x.Time < y.Time ? -1 : 1);
        }
        protected override void SetCastLogs(ParsedLog log)
        {
            long agentStart = Math.Max(FirstAware, log.FightData.FightStart);
            long agentEnd = Math.Min(LastAware, log.FightData.FightEnd);
            long timeStart = log.FightData.FightStart;
            CastLog curCastLog = null;
            foreach (CombatItem c in log.GetCastData(AgentItem.InstID))
            {
                if (!(c.Time > agentStart))
                {
                    continue;
                }
                ParseEnum.StateChange state = c.IsStateChange;
                if (state == ParseEnum.StateChange.Normal)
                {
                    if (c.IsActivation.IsCasting() && c.Time < agentEnd)
                    {
                        long time = c.Time - timeStart;
                        curCastLog = new CastLog(time, c.SkillID, c.Value, c.IsActivation);
                        CastLogs.Add(curCastLog);
                    }
                    else
                    {
                        if (curCastLog != null)
                        {
                            if (curCastLog.SkillId == c.SkillID)
                            {
                                curCastLog.SetEndStatus(c.Value, c.IsActivation);
                                curCastLog = null;
                            }
                        }
                    }


                }
                else if (state == ParseEnum.StateChange.WeaponSwap && c.Time < agentEnd)
                {
                    long time = c.Time - timeStart;
                    CastLog swapLog = new CastLog(time, SkillItem.WeaponSwapId, (int)c.DstAgent, c.IsActivation);
                    CastLogs.Add(swapLog);
                }
            }
        }
        private static void Add<T>(Dictionary<T, long> dictionary, T key, long value)
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
        // abstracts
        protected abstract void SetAdditionalCombatReplayData(ParsedLog log);
        public abstract int GetCombatReplayID();
        public abstract string GetCombatReplayJSON(CombatReplayMap map);
    }
}
