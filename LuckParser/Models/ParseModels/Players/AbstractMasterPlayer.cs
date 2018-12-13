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
            public int TotalDamage { get; }
            public ExtraBoonData (int hitCount, int totalHitCount, int damageGain, int totalDamage)
            {
                HitCount = hitCount;
                TotalHitCount = totalHitCount;
                DamageGain = damageGain;
                TotalDamage = totalDamage;
            }
        };
        // Boons
        public HashSet<Boon> TrackedBoons { get; } = new HashSet<Boon>();
        private readonly List<BoonDistribution> _boonDistribution = new List<BoonDistribution>();
        private readonly List<Dictionary<long, long>> _boonPresence = new List<Dictionary<long, long>>();
        private readonly List<Dictionary<long, long>> _condiPresence = new List<Dictionary<long, long>>();
        private readonly List<Dictionary<ushort, Dictionary<long,List<long>>>> _condiCleanse = new List<Dictionary<ushort, Dictionary<long, List<long>>>>();
        private readonly Dictionary<long, BoonsGraphModel> _boonPoints = new Dictionary<long, BoonsGraphModel>();
        private readonly Dictionary<long, List<ExtraBoonData>> _boonExtra = new Dictionary<long, List<ExtraBoonData>>();
        private readonly Dictionary<Target, Dictionary<long, List<ExtraBoonData>>> _boonTargetExtra = new Dictionary<Target, Dictionary<long, List<ExtraBoonData>>>();
        // damage list
        public Dictionary<int, List<int>> DamageList1S { get; } = new Dictionary<int, List<int>>();
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

        public List<int> Get1SDamageList(ParsedLog log, int phaseIndex, PhaseData phase, AbstractPlayer target)
        {
            ulong targetId = target != null ? target.Agent : 0;
            int id = (phaseIndex + "_" + targetId + "_1S").GetHashCode();
            if (DamageList1S.TryGetValue(id, out List<int> res))
            {
                return res;
            }
            List<int> dmgList = new List<int>();
            List<DamageLog> damageLogs = GetDamageLogs(target, log, phase.Start, phase.End);
            // fill the graph, full precision
            List<int> dmgListFull = new List<int>();
            for (int i = 0; i <= phase.GetDuration(); i++)
            {
                dmgListFull.Add(0);
            }
            int totalTime = 1;
            int totalDamage = 0;
            foreach (DamageLog dl in damageLogs)
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
            for (; totalTime <= phase.GetDuration(); totalTime++)
            {
                dmgListFull[totalTime] = totalDamage;
            }
            //
            dmgList.Add(0);
            for (int i = 1; i <= phase.GetDuration("s"); i++)
            {
                dmgList.Add(dmgListFull[1000 * i]);
            }
            if (phase.GetDuration("s") * 1000 != phase.GetDuration())
            {
                int lastDamage = dmgListFull[(int)phase.GetDuration()];
                dmgList.Add(lastDamage);
            }
            DamageList1S[id] = dmgList;
            return dmgList;
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

        public Dictionary<long, List<ExtraBoonData>> GetExtraBoonData(ParsedLog log, Target target)
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
                // no combat replay support on fight
                return;
            }
            if (CombatReplay == null)
            {
                CombatReplay = new CombatReplay();
                SetMovements(log);
                CombatReplay.PollingRate(pollingRate, log.FightData.FightDuration, forceInterpolate);
                if (trim)
                {
                    CombatItem despawnCheck = log.CombatData.GetStates(InstID, ParseEnum.StateChange.Despawn,FirstAware,LastAware).LastOrDefault();
                    CombatItem spawnCheck = log.CombatData.GetStates(InstID, ParseEnum.StateChange.Spawn, FirstAware, LastAware).LastOrDefault();
                    CombatItem deathCheck = log.CombatData.GetStates(InstID, ParseEnum.StateChange.ChangeDead, FirstAware, LastAware).LastOrDefault();
                    if (deathCheck != null)
                    {
                        CombatReplay.Trim(log.FightData.ToFightSpace(AgentItem.FirstAware), log.FightData.ToFightSpace(deathCheck.Time));
                    }
                    else if (despawnCheck != null && (spawnCheck == null || spawnCheck.Time < despawnCheck.Time))
                    {
                        CombatReplay.Trim(log.FightData.ToFightSpace(AgentItem.FirstAware), log.FightData.ToFightSpace(despawnCheck.Time));
                    }
                    else
                    {
                        CombatReplay.Trim(log.FightData.ToFightSpace(AgentItem.FirstAware), log.FightData.ToFightSpace(AgentItem.LastAware));
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
            CombatItem dead = log.CombatData.GetStatesData(ParseEnum.StateChange.ChangeDead).LastOrDefault(x => x.SrcInstid == InstID && x.Time >= log.FightData.ToLogSpace(start) && x.Time <= log.FightData.ToLogSpace(end));
            if (dead != null && dead.Time > 0)
            {
                return log.FightData.ToFightSpace(dead.Time);
            }
            return 0;
        }
        // private getters
        private BoonMap GetBoonMap(ParsedLog log)
        {
            BoonMap boonMap = new BoonMap();
            // Fill in Boon Map
            foreach (CombatItem c in log.GetBoonDataByDst(InstID, FirstAware, LastAware))
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
                if (c.Value == 0)
                {
                    continue;
                }
                long time = log.FightData.ToFightSpace(c.Time);
                List<BoonLog> loglist = boonMap[boonId];
                if (c.IsStateChange == ParseEnum.StateChange.BuffInitial)
                {
                    ushort src = c.SrcMasterInstid > 0 ? c.SrcMasterInstid : c.SrcInstid;
                    loglist.Add(new BoonApplicationLog(time, src, c.Value));
                }
                else if (c.IsStateChange != ParseEnum.StateChange.BuffInitial)
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
            //boonMap.Sort();
            foreach (var pair in boonMap)
            {
                TrackedBoons.Add(Boon.BoonsByIds[pair.Key]);
            }
            return boonMap;
        }
        // private setters
        private void SetMovements(ParsedLog log)
        {
            foreach (CombatItem c in log.GetMovementData(InstID, FirstAware, LastAware))
            {
                long time = log.FightData.ToFightSpace(c.Time);
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
                    foreach (Target target in log.FightData.Logic.Targets)
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
                                int totalDamage = dmLogs.Sum(x => x.Damage);
                                List<DamageLog> effect = dmLogs.Where(x => buffSimulationGeneration.GetStackCount((int)x.Time) > 0 && x.IsCondi == 0).ToList();
                                int damage = (int)(effect.Sum(x => x.Damage) / 21.0);
                                extraDataList.Add(new ExtraBoonData(effect.Count, dmLogs.Count(x => x.IsCondi == 0), damage, totalDamage));
                            }
                            dict[boonid] = extraDataList;
                        }                
                    }
                    _boonExtra[boonid] = new List<ExtraBoonData>();
                    for (int i = 0; i < phases.Count; i++)
                    {
                        List<DamageLog> dmLogs = GetJustPlayerDamageLogs(null, log, phases[i].Start, phases[i].End);
                        int totalDamage = dmLogs.Sum(x => x.Damage);
                        List<DamageLog> effect = dmLogs.Where(x => buffSimulationGeneration.GetStackCount((int)x.Time) > 0 && x.IsCondi == 0).ToList();
                        int damage = (int)(effect.Sum(x => x.Damage) / 21.0);
                        _boonExtra[boonid].Add(new ExtraBoonData(effect.Count, dmLogs.Count(x => x.IsCondi == 0), damage, totalDamage));
                    }
                    break;
                // GoE
                case 31803:
                    foreach (Target target in log.FightData.Logic.Targets)
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
                                List<DamageLog> effect = dmLogs.Where(x => buffSimulationGeneration.GetStackCount((int)x.Time) > 0 && x.IsCondi == 0).ToList();
                                int damage = effect.Sum(x => x.Damage);
                                extraDataList.Add(new ExtraBoonData(effect.Count, dmLogs.Count(x => x.IsCondi == 0), damage, 0));
                            }
                            dict[boonid] = extraDataList;
                        }

                    }
                    _boonExtra[boonid] = new List<ExtraBoonData>();
                    for (int i = 0; i < phases.Count; i++)
                    {
                        List<DamageLog> dmLogs = GetJustPlayerDamageLogs(null, log, phases[i].Start, phases[i].End);
                        List<DamageLog> effect = dmLogs.Where(x => buffSimulationGeneration.GetStackCount((int)x.Time) > 0 && x.IsCondi == 0).ToList();
                        int damage = effect.Sum(x => x.Damage);
                        _boonExtra[boonid].Add(new ExtraBoonData(effect.Count, dmLogs.Count(x => x.IsCondi == 0), damage, 0));
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
            BoonsGraphModel boonPresenceGraph = new BoonsGraphModel(Boon.BoonsByIds[Boon.NumberOfBoonsID]);
            BoonsGraphModel condiPresenceGraph = new BoonsGraphModel(Boon.BoonsByIds[Boon.NumberOfConditionsID]);
            HashSet<long> boonIds = new HashSet<long>(Boon.GetBoonList().Select(x => x.ID));
            HashSet<long> condiIds = new HashSet<long>(Boon.GetCondiBoonList().Select(x => x.ID));
            for (int i = 0; i < phases.Count; i++)
            {
                _boonDistribution.Add(new BoonDistribution());
                _boonPresence.Add(new Dictionary<long, long>());
                _condiPresence.Add(new Dictionary<long, long>());
                _condiCleanse.Add(new Dictionary<ushort, Dictionary<long, List<long>>>());
            }

            long death = GetDeath(log, 0, dur);
            foreach (Boon boon in TrackedBoons)
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
                                distrib = new Dictionary<ushort, BoonDistributionItem>();
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
                                    distrib.Add(src, new BoonDistributionItem(
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
                                distrib = new Dictionary<ushort, BoonDistributionItem>();
                                _boonDistribution[i].Add(boonid, distrib);
                            }
                            if (distrib.TryGetValue(simul.Src, out var toModify))
                            {
                                toModify.Overstack += simul.GetOverstack(phase.Start, phase.End);
                                distrib[simul.Src] = toModify;
                            }
                            else
                            {
                                distrib.Add(simul.Src, new BoonDistributionItem(
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
                    _boonPoints[boonid] = new BoonsGraphModel(boon, graphSegments);
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
            _boonPoints[Boon.NumberOfBoonsID] = boonPresenceGraph;
            _boonPoints[Boon.NumberOfConditionsID] = condiPresenceGraph;
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
                if (pair.Value.GetDamageLogs(null, log, log.FightData.ToFightSpace(FirstAware), log.FightData.ToFightSpace(LastAware)).Count > 0)
                {
                    _minions[pair.Key] = pair.Value;
                }
            }
        }
        protected override void SetDamageLogs(ParsedLog log)
        {
            foreach (CombatItem c in log.GetDamageData(InstID, FirstAware, LastAware))
            {
                long time = log.FightData.ToFightSpace(c.Time);
                AddDamageLog(time, c);
            }
            Dictionary<string, Minions> minionsList = GetMinions(log);
            foreach (Minions mins in minionsList.Values)
            {
                DamageLogs.AddRange(mins.GetDamageLogs(null, log, log.FightData.ToFightSpace(FirstAware), log.FightData.ToFightSpace(LastAware)));
            }
            DamageLogs.Sort((x, y) => x.Time.CompareTo(y.Time));
        }
        protected override void SetDamageTakenLogs(ParsedLog log)
        {
            foreach (CombatItem c in log.GetDamageTakenData(InstID, FirstAware, LastAware))
            {
                long time = log.FightData.ToFightSpace(c.Time);
                AddDamageTakenLog(time, c);
            }
        }
        protected override void SetCastLogs(ParsedLog log)
        {
            CastLog curCastLog = null;
            foreach (CombatItem c in log.GetCastData(InstID, FirstAware, LastAware))
            {
                ParseEnum.StateChange state = c.IsStateChange;
                if (state == ParseEnum.StateChange.Normal)
                {                  
                    if (c.IsActivation.IsCasting())
                    {
                        // Missing end activation
                        if (curCastLog != null)
                        {
                            int actDur = curCastLog.SkillId == SkillItem.DodgeId ? 750 : curCastLog.SkillId == SkillItem.WeaponSwapId ? 50 : curCastLog.ExpectedDuration;
                            curCastLog.SetEndStatus(actDur, ParseEnum.Activation.Unknown, log.FightData.FightDuration);
                            curCastLog = null;
                        }
                        long time = log.FightData.ToFightSpace(c.Time);
                        curCastLog = new CastLog(time, c.SkillID, c.Value, c.IsActivation);
                        CastLogs.Add(curCastLog);
                    }
                    else
                    {
                        if (curCastLog != null)
                        {
                            if (curCastLog.SkillId == c.SkillID)
                            {
                                int actDur = curCastLog.SkillId == SkillItem.DodgeId ? 750 : curCastLog.SkillId == SkillItem.WeaponSwapId ? 50 : c.Value;
                                curCastLog.SetEndStatus(actDur, c.IsActivation, log.FightData.FightDuration);
                                curCastLog = null;
                            }
                        }
                    }


                }
                else if (state == ParseEnum.StateChange.WeaponSwap)
                {
                    long time = log.FightData.ToFightSpace(c.Time);
                    CastLog swapLog = new CastLog(time, SkillItem.WeaponSwapId, (int)c.DstAgent, c.IsActivation);
                    if (CastLogs.Count > 0 && (time - CastLogs.Last().Time) < 10 && CastLogs.Last().SkillId == SkillItem.WeaponSwapId)
                    {
                        CastLogs[CastLogs.Count - 1] = swapLog;
                    }
                    else
                    {
                        CastLogs.Add(swapLog);
                    }
                }
            }
            long cloakStart = 0;
            foreach (long time in log.CombatData.GetBuffs(InstID, 40408, FirstAware, LastAware).Select(x => log.FightData.ToFightSpace(x.Time)))
            {
                if (time - cloakStart > 10)
                {
                    CastLog dodgeLog = new CastLog(time, SkillItem.DodgeId, 0, ParseEnum.Activation.Unknown);
                    dodgeLog.SetEndStatus(50, ParseEnum.Activation.Unknown, log.FightData.FightDuration);
                    CastLogs.Add(dodgeLog);
                }
                cloakStart = time;
            }
            CastLogs.Sort((x, y) => x.Time.CompareTo(y.Time));
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
