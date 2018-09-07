using LuckParser.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public abstract class AbstractMasterPlayer : AbstractPlayer
    {
        // Boons
        public readonly List<Boon> BoonToTrack = new List<Boon>();
        private readonly List<BoonDistribution> _boonDistribution = new List<BoonDistribution>();
        private readonly List<Dictionary<long, long>> _boonPresence = new List<Dictionary<long, long>>();
        private readonly List<Dictionary<long, long>> _condiPresence = new List<Dictionary<long, long>>();
        private readonly Dictionary<long, BoonsGraphModel> _boonPoints = new Dictionary<long, BoonsGraphModel>();
        private readonly Dictionary<long, Dictionary<int, string[]>> _boonExtra = new Dictionary<long, Dictionary<int, string[]>>();
        // dps graphs
        public readonly Dictionary<int, List<Point>> DpsGraph = new Dictionary<int, List<Point>>();
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

        public Dictionary<long, Dictionary<int, string[]>> GetExtraBoonData(ParsedLog log)
        {
            if (_boonDistribution.Count == 0)
            {
                SetBoonDistribution(log);
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
                SetCombatReplayIcon(log);
                if (trim)
                {
                    CombatItem despawnCheck = log.CombatData.FirstOrDefault(x => x.SrcAgent == Agent.Agent && (x.IsStateChange.IsDead() || x.IsStateChange.IsDespawn()));
                    if (despawnCheck != null)
                    {
                        CombatReplay.Trim(Agent.FirstAware - log.FightData.FightStart, despawnCheck.Time - log.FightData.FightStart);
                    }
                    else
                    {
                        CombatReplay.Trim(Agent.FirstAware - log.FightData.FightStart, Agent.LastAware - log.FightData.FightStart);
                    }
                }
                SetAdditionalCombatReplayData(log, pollingRate);
            }
        }

        public long GetDeath(ParsedLog log, long start, long end)
        {
            long offset = log.FightData.FightStart;
            CombatItem dead = log.CombatData.LastOrDefault(x => x.SrcInstid == Agent.InstID && x.IsStateChange.IsDead() && x.Time >= start + offset && x.Time <= end + offset);
            if (dead != null && dead.Time > 0)
            {
                return dead.Time;
            }
            return 0;
        }
        // private getters
        private BoonMap GetBoonMap(ParsedLog log, HashSet<long> boonIds, HashSet<long> condiIds, HashSet<long> offIds, HashSet<long> defIds)
        {
            BoonMap boonMap = new BoonMap
            {
                BoonToTrack
            };
            // Fill in Boon Map
            long timeStart = log.FightData.FightStart;
            HashSet<long> tableIds = new HashSet<long> (boonIds);
            tableIds.UnionWith(condiIds);
            tableIds.UnionWith(offIds);
            tableIds.UnionWith(defIds);
            foreach(CombatItem c in log.GetBoonDataByDst(Agent.InstID))
            {
                long boonId = c.SkillID;
                if (!boonMap.ContainsKey(boonId))
                {
                    continue;
                }
                long time = c.Time - timeStart;
                // don't add buff initial table boons and buffs in non golem mode, for others overstack is irrelevant
                if (c.IsStateChange == ParseEnum.StateChange.BuffInitial && (log.IsBenchmarkMode() || !tableIds.Contains(boonId)))
                {
                    List<BoonLog> loglist = boonMap[boonId];
                    loglist.Add(new BoonLog(0, 0, long.MaxValue, 0));
                }
                else if (c.IsStateChange != ParseEnum.StateChange.BuffInitial && time >= 0 && time < log.FightData.FightDuration)
                {
                    if (c.IsBuffRemove == ParseEnum.BuffRemove.None)
                    {
                        ushort src = c.SrcMasterInstid > 0 ? c.SrcMasterInstid : c.SrcInstid;
                        List<BoonLog> loglist = boonMap[boonId];

                        if (loglist.Count == 0 && c.OverstackValue > 0)
                        {
                            loglist.Add(new BoonLog(0, 0, time, 0));
                        }
                        loglist.Add(new BoonLog(time, src, c.Value, 0));
                    }
                    else if (Boon.RemovePermission(boonId, c.IsBuffRemove, c.IFF) && time < log.FightData.FightDuration - 50)
                    {
                        if (c.IsBuffRemove == ParseEnum.BuffRemove.All)//All
                        {
                            List<BoonLog> loglist = boonMap[boonId];
                            if (loglist.Count == 0)
                            {
                                loglist.Add(new BoonLog(0, 0, time, 0));
                            }
                            else
                            {
                                for (int cnt = loglist.Count - 1; cnt >= 0; cnt--)
                                {
                                    BoonLog curBL = loglist[cnt];
                                    if (curBL.GetOverstack() == 0 && curBL.GetTime() + curBL.GetValue() > time)
                                    {
                                        long subtract = (curBL.GetTime() + curBL.GetValue()) - time;
                                        curBL.AddValue(-subtract);
                                        // add removed as overstack
                                        curBL.AddOverstack((uint)subtract);
                                    }
                                }
                            }
                        }
                        else if (c.IsBuffRemove == ParseEnum.BuffRemove.Single)//Single
                        {
                            List<BoonLog> loglist = boonMap[boonId];
                            if (loglist.Count == 0)
                            {
                                loglist.Add(new BoonLog(0, 0, time, 0));
                            }
                            else
                            {
                                int cnt = loglist.Count - 1;
                                BoonLog curBL = loglist[cnt];
                                if (curBL.GetOverstack() == 0 && curBL.GetTime() + curBL.GetValue() > time)
                                {
                                    long subtract = (curBL.GetTime() + curBL.GetValue()) - time;
                                    curBL.AddValue(-subtract);
                                    // add removed as overstack
                                    curBL.AddOverstack((uint)subtract);
                                }
                            }
                        }
                        else if (c.IsBuffRemove == ParseEnum.BuffRemove.Manual)//Manuel
                        {
                            List<BoonLog> loglist = boonMap[boonId];
                            if (loglist.Count == 0)
                            {
                                loglist.Add(new BoonLog(0, 0, time, 0));
                            }
                            else
                            {
                                for (int cnt = loglist.Count - 1; cnt >= 0; cnt--)
                                {
                                    BoonLog curBL = loglist[cnt];
                                    long ctime = curBL.GetTime() + curBL.GetValue();
                                    if (curBL.GetOverstack() == 0 && ctime > time)
                                    {
                                        long subtract = (curBL.GetTime() + curBL.GetValue()) - time;
                                        curBL.AddValue(-subtract);
                                        // add removed as overstack
                                        curBL.AddOverstack((uint)subtract);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }   
            return boonMap;
        }
        // private setters
        private void SetMovements(ParsedLog log)
        {
            foreach (CombatItem c in log.GetMovementData(Agent.InstID))
            {
                long time = c.Time - log.FightData.FightStart;
                byte[] xy = BitConverter.GetBytes(c.DstAgent);
                float x = BitConverter.ToSingle(xy, 0);
                float y = BitConverter.ToSingle(xy, 4);
                if (c.IsStateChange == ParseEnum.StateChange.Position)
                {
                    CombatReplay.AddPosition(new Point3D(x, y, c.Value, time));
                }
                else
                {
                    CombatReplay.AddVelocity(new Point3D(x, y, c.Value, time));
                }
            }
        }
        private void GenerateExtraBoonData(ParsedLog log, long boonid, GenerationSimulationResult buffSimulationGeneration, List<PhaseData> phases)
        {

            switch (boonid)
            {
                // Frost Spirit
                case 50421:
                    _boonExtra[boonid] = new Dictionary<int, string[]>();
                    for (int i = 0; i < phases.Count; i++)
                    {
                        List<DamageLog> dmLogs = GetJustPlayerDamageLogs(0, log, phases[i].Start, phases[i].End);
                        int totalDamage = Math.Max(dmLogs.Sum(x => x.GetDamage()), 1);
                        int totalBossDamage = Math.Max(dmLogs.Where(x => x.GetDstInstidt() == log.FightData.InstID).Sum(x => x.GetDamage()), 1);
                        List<DamageLog> effect = dmLogs.Where(x => buffSimulationGeneration.GetStackCount((int)x.GetTime()) > 0 && x.IsCondi() == 0).ToList();
                        List<DamageLog> effectBoss = effect.Where(x => x.GetDstInstidt() == log.FightData.InstID).ToList();
                        int damage = (int)(effect.Sum(x => x.GetDamage()) / 21.0);
                        int bossDamage = (int)(effectBoss.Sum(x => x.GetDamage()) / 21.0);
                        double gain = Math.Round(100.0 * ((double)totalDamage / (totalDamage - damage) - 1.0), 2);
                        double gainBoss = Math.Round(100.0 * ((double)totalBossDamage / (totalBossDamage - bossDamage) - 1.0), 2);
                        string gainText = effect.Count + " out of " + dmLogs.Count(x => x.IsCondi() == 0) + " hits <br> Pure Frost Spirit Damage: "
                                + damage + "<br> Effective Damage Increase: " + gain + "%";
                        string gainBossText = effectBoss.Count + " out of " + dmLogs.Count(x => x.GetDstInstidt() == log.FightData.InstID && x.IsCondi() == 0) + " hits <br> Pure Frost Spirit Damage: "
                                + bossDamage + "<br> Effective Damage Increase: " + gainBoss + "%";
                        _boonExtra[boonid][i] = new [] { gainText, gainBossText };
                    }
                    break;
                // Kalla Elite
                case 45026:
                    _boonExtra[boonid] = new Dictionary<int, string[]>();
                    for (int i = 0; i < phases.Count; i++)
                    {
                        List<DamageLog> dmLogs = GetJustPlayerDamageLogs(0, log, phases[i].Start, phases[i].End);
                        int totalDamage = Math.Max(dmLogs.Sum(x => x.GetDamage()), 1);
                        int totalBossDamage = Math.Max(dmLogs.Where(x => x.GetDstInstidt() == log.FightData.InstID).Sum(x => x.GetDamage()), 1);
                        int effectCount = dmLogs.Count(x => buffSimulationGeneration.GetStackCount((int)x.GetTime()) > 0 && x.IsCondi() == 0);
                        int effectBossCount = dmLogs.Count(x => buffSimulationGeneration.GetStackCount((int)x.GetTime()) > 0 && x.IsCondi() == 0 && x.GetDstInstidt() == log.FightData.InstID);
                        int damage = (int)(effectCount * (325 + 3000 * 0.04));
                        int bossDamage = (int)(effectBossCount * (325 + 3000 * 0.04));
                        double gain = Math.Round(100.0 * ((double)(totalDamage + damage) / totalDamage - 1.0), 2);
                        double gainBoss = Math.Round(100.0 * ((double)(totalBossDamage + bossDamage) / totalBossDamage - 1.0), 2);
                        string gainText = effectCount + " out of " + dmLogs.Count(x => x.IsCondi() == 0) + " hits <br> Estimated Soulcleave Damage: "
                                + damage + "<br> Estimated Damage Increase: " + gain + "%";
                        string gainBossText = effectBossCount + " out of " + dmLogs.Count(x => x.GetDstInstidt() == log.FightData.InstID && x.IsCondi() == 0) + " hits <br> Estimated Soulcleave Damage: "
                                + bossDamage + "<br> Estimated Damage Increase: " + gainBoss + "%";
                        _boonExtra[boonid][i] = new [] { gainText, gainBossText };
                    }
                    break;
                // GoE
                case 31803:
                    _boonExtra[boonid] = new Dictionary<int, string[]>();
                    for (int i = 0; i < phases.Count; i++)
                    {
                        List<DamageLog> dmLogs = GetJustPlayerDamageLogs(0, log, phases[i].Start, phases[i].End);
                        int totalDamage = Math.Max(dmLogs.Sum(x => x.GetDamage()), 1);
                        int totalBossDamage = Math.Max(dmLogs.Where(x => x.GetDstInstidt() == log.FightData.InstID).Sum(x => x.GetDamage()), 1);
                        List<DamageLog> effect = dmLogs.Where(x => buffSimulationGeneration.GetStackCount((int)x.GetTime()) > 0 && x.IsCondi() == 0).ToList();
                        List<DamageLog> effectBoss = effect.Where(x => x.GetDstInstidt() == log.FightData.InstID).ToList();
                        int damage = (int)(effect.Sum(x => x.GetDamage()) / 11.0);
                        int bossDamage = (int)(effectBoss.Sum(x => x.GetDamage()) / 11.0);
                        double gain = Math.Round(100.0 * ((double)totalDamage / (totalDamage - damage) - 1.0), 2);
                        double gainBoss = Math.Round(100.0 * ((double)totalBossDamage / (totalBossDamage - bossDamage) - 1.0), 2);
                        string gainText = effect.Count + " out of " + dmLogs.Count(x => x.IsCondi() == 0) + " hits <br> Pure GoE Damage: "
                                + damage + "<br> Effective Damage Increase: " + gain + "%";
                        string gainBossText = effectBoss.Count + " out of " + dmLogs.Count(x => x.GetDstInstidt() == log.FightData.InstID && x.IsCondi() == 0) + " hits <br> Pure GoE Damage: "
                                + bossDamage + "<br> Effective Damage Increase: " + gainBoss + "%";
                        _boonExtra[boonid][i] = new [] { gainText, gainBossText };
                    }
                    break;
            }
        }
        private void SetBoonDistribution(ParsedLog log)
        {
            HashSet<long> boonIds = new HashSet<long>(Boon.GetBoonList().Select(x => x.ID));
            HashSet<long> condiIds = new HashSet<long>(Boon.GetCondiBoonList().Select(x => x.ID));
            HashSet<long> defIds = new HashSet<long>(Boon.GetDefensiveTableList().Select(x => x.ID));
            HashSet<long> offIds = new HashSet<long>(Boon.GetOffensiveTableList().Select(x => x.ID));
            List<PhaseData> phases = log.Boss.GetPhases(log);
            BoonMap toUse = GetBoonMap(log, boonIds, condiIds, defIds, offIds);
            long dur = log.FightData.FightDuration;
            int fightDuration = (int)(dur) / 1000;
            HashSet<long> extraDataID = new HashSet<long>
            {
                50421,
                45026,
                31803
            };
            BoonsGraphModel boonPresenceGraph = new BoonsGraphModel("Number of Boons");
            BoonsGraphModel condiPresenceGraph = new BoonsGraphModel("Number of Conditions");
            for (int i = 0; i < phases.Count; i++)
            {
                _boonDistribution.Add(new BoonDistribution());
                _boonPresence.Add(new Dictionary<long, long>());
                _condiPresence.Add(new Dictionary<long, long>());
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
                    if (death > 0 && GetCastLogs(log, death + 5000, fightDuration).Count == 0)
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
            List<AgentItem> combatMinion = log.AgentData.NPCAgentList.Where(x => x.MasterAgent == Agent.Agent).ToList();
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
                if (pair.Value.GetDamageLogs(0, log, 0, log.FightData.FightDuration).Count > 0)
                {
                    _minions[pair.Key] = pair.Value;
                }
            }
        }
        protected override void SetDamageLogs(ParsedLog log)
        {
            long timeStart = log.FightData.FightStart;
            foreach (CombatItem c in log.GetDamageData(Agent.InstID))
            {
                if (c.Time > log.FightData.FightStart && c.Time < log.FightData.FightEnd)//selecting player or minion as caster
                {
                    long time = c.Time - timeStart;
                    AddDamageLog(time, c);
                }
            }
            Dictionary<string, Minions> minionsList = GetMinions(log);
            foreach (Minions mins in minionsList.Values)
            {
                DamageLogs.AddRange(mins.GetDamageLogs(0, log, 0, log.FightData.FightDuration));
            }
            DamageLogs.Sort((x, y) => x.GetTime() < y.GetTime() ? -1 : 1);
        }
        protected override void SetCastLogs(ParsedLog log)
        {
            long timeStart = log.FightData.FightStart;
            CastLog curCastLog = null;
            foreach (CombatItem c in log.GetCastData(Agent.InstID))
            {
                if (!(c.Time > log.FightData.FightStart && c.Time < log.FightData.FightEnd))
                {
                    continue;
                }
                ParseEnum.StateChange state = c.IsStateChange;
                if (state == ParseEnum.StateChange.Normal)
                {
                    if (c.IsActivation.IsCasting())
                    {
                        long time = c.Time - timeStart;
                        curCastLog = new CastLog(time, c.SkillID, c.Value, c.IsActivation);
                        CastLogs.Add(curCastLog);
                    }
                    else
                    {
                        if (curCastLog != null)
                        {
                            if (curCastLog.GetID() == c.SkillID)
                            {
                                curCastLog.SetEndStatus(c.Value, c.IsActivation);
                                curCastLog = null;
                            }
                        }
                    }


                }
                else if (state == ParseEnum.StateChange.WeaponSwap)
                {//Weapon swap
                    if ((int)c.DstAgent == 4 || (int)c.DstAgent == 5)
                    {
                        long time = c.Time - timeStart;
                        CastLog swapLog = new CastLog(time, SkillItem.WeaponSwapId, (int)c.DstAgent, c.IsActivation);
                        CastLogs.Add(swapLog);
                    }

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
        protected abstract void SetAdditionalCombatReplayData(ParsedLog log, int pollingRate);
        protected abstract void SetCombatReplayIcon(ParsedLog log);
    }
}
