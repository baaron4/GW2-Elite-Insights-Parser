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
        protected readonly List<Boon> BoonToTrack = new List<Boon>();
        private readonly List<BoonDistribution> _boonDistribution = new List<BoonDistribution>();
        private readonly List<Dictionary<long, long>> _boonPresence = new List<Dictionary<long, long>>();
        private readonly List<Dictionary<long, long>> _condiPresence = new List<Dictionary<long, long>>();
        private readonly Dictionary<long, BoonsGraphModel> _boonPoints = new Dictionary<long, BoonsGraphModel>();
        private readonly Dictionary<long, Dictionary<int, string[]>> _boonExtra = new Dictionary<long, Dictionary<int, string[]>>();
        // dps graphs
        private readonly Dictionary<int, List<Point>> _dpsGraphs = new Dictionary<int, List<Point>>();
        // Minions
        private readonly Dictionary<string, Minions> _minions = new Dictionary<string, Minions>();
        // Replay
        protected CombatReplay Replay;

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
        public void AddDPSGraph(int id, List<Point> graph)
        {
            _dpsGraphs[id] = graph;
        }
        public List<Point> GetDPSGraph(int id)
        {
            if (_dpsGraphs.ContainsKey(id))
            {
                return _dpsGraphs[id];
            }
            return new List<Point>();
        }
        public BoonDistribution GetBoonDistribution(ParsedLog log, List<PhaseData> phases, int phaseIndex)
        {
            if (_boonDistribution.Count == 0)
            {
                SetBoonDistribution(log, phases);
            }
            return _boonDistribution[phaseIndex];
        }
        public Dictionary<long, BoonsGraphModel> GetBoonGraphs(ParsedLog log, List<PhaseData> phases)
        {
            if (_boonDistribution.Count == 0)
            {
                SetBoonDistribution(log, phases);
            }
            return _boonPoints;
        }
        public List<Boon> GetBoonToTrack()
        {
            return BoonToTrack;
        }
        public Dictionary<long, long> GetBoonPresence(ParsedLog log, List<PhaseData> phases, int phaseIndex)
        {
            if (_boonDistribution.Count == 0)
            {
                SetBoonDistribution(log, phases);
            }
            return _boonPresence[phaseIndex];
        }

        public Dictionary<long, Dictionary<int, string[]>> GetExtraBoonData(ParsedLog log, List<PhaseData> phases)
        {
            if (_boonDistribution.Count == 0)
            {
                SetBoonDistribution(log, phases);
            }
            return _boonExtra;
        }

        public Dictionary<long, long> GetCondiPresence(ParsedLog log, List<PhaseData> phases, int phaseIndex)
        {
            if (_boonDistribution.Count == 0)
            {
                SetBoonDistribution(log, phases);
            }
            return _condiPresence[phaseIndex];
        }
        public void InitCombatReplay(ParsedLog log, int pollingRate, bool trim, bool forceInterpolate)
        {
            if (!log.GetBossData().GetBossBehavior().CanCombatReplay)
            {
                // no combat replay support on boss
                return;
            }
            if (Replay == null)
            {
                Replay = new CombatReplay();
                SetMovements(log);
                Replay.PollingRate(pollingRate, log.GetBossData().GetAwareDuration(), forceInterpolate);
                SetCombatReplayIcon(log);
                if (trim)
                {
                    CombatItem despawnCheck = log.GetCombatList().FirstOrDefault(x => x.SrcAgent == Agent.Agent && (x.IsStateChange.IsDead() || x.IsStateChange.IsDespawn()));
                    if (despawnCheck != null)
                    {
                        Replay.Trim(Agent.FirstAware - log.GetBossData().GetFirstAware(), despawnCheck.Time - log.GetBossData().GetFirstAware());
                    }
                    else
                    {
                        Replay.Trim(Agent.FirstAware - log.GetBossData().GetFirstAware(), Agent.LastAware - log.GetBossData().GetFirstAware());
                    }
                }
                SetAdditionalCombatReplayData(log, pollingRate);
            }
        }
        public CombatReplay GetCombatReplay()
        {
            return Replay;
        }

        public long GetDeath(ParsedLog log, long start, long end)
        {
            long offset = log.GetBossData().GetFirstAware();
            CombatItem dead = log.GetCombatList().LastOrDefault(x => x.SrcInstid == Agent.InstID && x.IsStateChange.IsDead() && x.Time >= start + offset && x.Time <= end + offset);
            if (dead != null && dead.Time > 0)
            {
                return dead.Time;
            }
            return 0;
        }

        public void SetBoonToTrack(List<List<Boon>> boonToTrack)
        {
            if (BoonToTrack.Count > 0)
            {
                return;
            }
            foreach(List<Boon> lBoon in boonToTrack)
            {
                BoonToTrack.AddRange(lBoon);
            }
        }
        // private getters
        private BoonMap GetBoonMap(ParsedLog log, HashSet<long> boonIds, HashSet<long> condiIds, HashSet<long> offIds, HashSet<long> defIds)
        {
            BoonMap boonMap = new BoonMap
            {
                BoonToTrack
            };
            // Fill in Boon Map
            long timeStart = log.GetBossData().GetFirstAware();
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
                else if (c.IsStateChange != ParseEnum.StateChange.BuffInitial && time >= 0 && time < log.GetBossData().GetAwareDuration())
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
                    else if (Boon.RemovePermission(boonId, c.IsBuffRemove, c.IFF) && time < log.GetBossData().GetAwareDuration() - 50)
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
                long time = c.Time - log.GetBossData().GetFirstAware();
                byte[] xy = BitConverter.GetBytes(c.DstAgent);
                float x = BitConverter.ToSingle(xy, 0);
                float y = BitConverter.ToSingle(xy, 4);
                if (c.IsStateChange == ParseEnum.StateChange.Position)
                {
                    Replay.AddPosition(new Point3D(x, y, c.Value, time));
                }
                else
                {
                    Replay.AddVelocity(new Point3D(x, y, c.Value, time));
                }
            }
        }
        private void GenerateExtraBoonData(ParsedLog log, long boonid, BoonSimulationResult boonSimulation, List<PhaseData> phases)
        {

            switch (boonid)
            {
                // Frost Spirit
                case 50421:
                    _boonExtra[boonid] = new Dictionary<int, string[]>();
                    for (int i = 0; i < phases.Count; i++)
                    {
                        List<DamageLog> dmLogs = GetJustPlayerDamageLogs(0, log, phases[i].GetStart(), phases[i].GetEnd());
                        int totalDamage = Math.Max(dmLogs.Sum(x => x.GetDamage()), 1);
                        int totalBossDamage = Math.Max(dmLogs.Where(x => x.GetDstInstidt() == log.GetBossData().GetInstid()).Sum(x => x.GetDamage()), 1);
                        List<DamageLog> effect = dmLogs.Where(x => boonSimulation.GetBoonStackCount((int)x.GetTime()) > 0 && x.IsCondi() == 0).ToList();
                        List<DamageLog> effectBoss = effect.Where(x => x.GetDstInstidt() == log.GetBossData().GetInstid()).ToList();
                        int damage = (int)(effect.Sum(x => x.GetDamage()) / 21.0);
                        int bossDamage = (int)(effectBoss.Sum(x => x.GetDamage()) / 21.0);
                        double gain = Math.Round(100.0 * ((double)totalDamage / (totalDamage - damage) - 1.0), 2);
                        double gainBoss = Math.Round(100.0 * ((double)totalBossDamage / (totalBossDamage - bossDamage) - 1.0), 2);
                        string gainText = effect.Count + " out of " + dmLogs.Count(x => x.IsCondi() == 0) + " hits <br> Pure Frost Spirit Damage: "
                                + damage + "<br> Effective Damage Increase: " + gain + "%";
                        string gainBossText = effectBoss.Count + " out of " + dmLogs.Count(x => x.GetDstInstidt() == log.GetBossData().GetInstid() && x.IsCondi() == 0) + " hits <br> Pure Frost Spirit Damage: "
                                + bossDamage + "<br> Effective Damage Increase: " + gainBoss + "%";
                        _boonExtra[boonid][i] = new [] { gainText, gainBossText };
                    }
                    break;
                // Kalla Elite
                case 45026:
                    _boonExtra[boonid] = new Dictionary<int, string[]>();
                    for (int i = 0; i < phases.Count; i++)
                    {
                        List<DamageLog> dmLogs = GetJustPlayerDamageLogs(0, log, phases[i].GetStart(), phases[i].GetEnd());
                        int totalDamage = Math.Max(dmLogs.Sum(x => x.GetDamage()), 1);
                        int totalBossDamage = Math.Max(dmLogs.Where(x => x.GetDstInstidt() == log.GetBossData().GetInstid()).Sum(x => x.GetDamage()), 1);
                        int effectCount = dmLogs.Count(x => boonSimulation.GetBoonStackCount((int)x.GetTime()) > 0 && x.IsCondi() == 0);
                        int effectBossCount = dmLogs.Count(x => boonSimulation.GetBoonStackCount((int)x.GetTime()) > 0 && x.IsCondi() == 0 && x.GetDstInstidt() == log.GetBossData().GetInstid());
                        int damage = (int)(effectCount * (325 + 3000 * 0.04));
                        int bossDamage = (int)(effectBossCount * (325 + 3000 * 0.04));
                        double gain = Math.Round(100.0 * ((double)(totalDamage + damage) / totalDamage - 1.0), 2);
                        double gainBoss = Math.Round(100.0 * ((double)(totalBossDamage + bossDamage) / totalBossDamage - 1.0), 2);
                        string gainText = effectCount + " out of " + dmLogs.Count(x => x.IsCondi() == 0) + " hits <br> Estimated Soulcleave Damage: "
                                + damage + "<br> Estimated Damage Increase: " + gain + "%";
                        string gainBossText = effectBossCount + " out of " + dmLogs.Count(x => x.GetDstInstidt() == log.GetBossData().GetInstid() && x.IsCondi() == 0) + " hits <br> Estimated Soulcleave Damage: "
                                + bossDamage + "<br> Estimated Damage Increase: " + gainBoss + "%";
                        _boonExtra[boonid][i] = new [] { gainText, gainBossText };
                    }
                    break;
                // GoE
                case 31803:
                    _boonExtra[boonid] = new Dictionary<int, string[]>();
                    for (int i = 0; i < phases.Count; i++)
                    {
                        List<DamageLog> dmLogs = GetJustPlayerDamageLogs(0, log, phases[i].GetStart(), phases[i].GetEnd());
                        int totalDamage = Math.Max(dmLogs.Sum(x => x.GetDamage()), 1);
                        int totalBossDamage = Math.Max(dmLogs.Where(x => x.GetDstInstidt() == log.GetBossData().GetInstid()).Sum(x => x.GetDamage()), 1);
                        List<DamageLog> effect = dmLogs.Where(x => boonSimulation.GetBoonStackCount((int)x.GetTime()) > 0 && x.IsCondi() == 0).ToList();
                        List<DamageLog> effectBoss = effect.Where(x => x.GetDstInstidt() == log.GetBossData().GetInstid()).ToList();
                        int damage = (int)(effect.Sum(x => x.GetDamage()) / 11.0);
                        int bossDamage = (int)(effectBoss.Sum(x => x.GetDamage()) / 11.0);
                        double gain = Math.Round(100.0 * ((double)totalDamage / (totalDamage - damage) - 1.0), 2);
                        double gainBoss = Math.Round(100.0 * ((double)totalBossDamage / (totalBossDamage - bossDamage) - 1.0), 2);
                        string gainText = effect.Count + " out of " + dmLogs.Count(x => x.IsCondi() == 0) + " hits <br> Pure GoE Damage: "
                                + damage + "<br> Effective Damage Increase: " + gain + "%";
                        string gainBossText = effectBoss.Count + " out of " + dmLogs.Count(x => x.GetDstInstidt() == log.GetBossData().GetInstid() && x.IsCondi() == 0) + " hits <br> Pure GoE Damage: "
                                + bossDamage + "<br> Effective Damage Increase: " + gainBoss + "%";
                        _boonExtra[boonid][i] = new [] { gainText, gainBossText };
                    }
                    break;
            }
        }
        private void SetBoonDistribution(ParsedLog log, List<PhaseData> phases)
        {
            HashSet<long> boonIds = new HashSet<long>(Boon.GetBoonList().Select(x => x.GetID()));
            HashSet<long> condiIds = new HashSet<long>(Boon.GetCondiBoonList().Select(x => x.GetID()));
            HashSet<long> defIds = new HashSet<long>(Boon.GetDefensiveTableList().Select(x => x.GetID()));
            HashSet<long> offIds = new HashSet<long>(Boon.GetOffensiveTableList().Select(x => x.GetID()));
            BoonMap toUse = GetBoonMap(log, boonIds, condiIds, defIds, offIds);
            long dur = log.GetBossData().GetAwareDuration();
            int fightDuration = (int)(dur) / 1000;
            // Init boon/condi presence points
            BoonsGraphModel boonPresencePoints = new BoonsGraphModel("Number of Boons");
            BoonsGraphModel condiPresencePoints = new BoonsGraphModel("Number of Conditions");
            HashSet<long> extraDataID = new HashSet<long>
            {
                50421,
                45026,
                31803
            };
            for (int i = 0; i <= fightDuration; i++)
            {
                boonPresencePoints.GetBoonChart().Add(new Point(i, 0));
                condiPresencePoints.GetBoonChart().Add(new Point(i, 0));
            }
            for (int i = 0; i < phases.Count; i++)
            {
                _boonDistribution.Add(new BoonDistribution());
                _boonPresence.Add(new Dictionary<long, long>());
                _condiPresence.Add(new Dictionary<long, long>());
            }

            long death = GetDeath(log, 0, dur) - log.GetBossData().GetFirstAware();
            foreach (Boon boon in BoonToTrack)
            {
                long boonid = boon.GetID();
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
                    var simulation = simulator.GetSimulationResult();
                    var updateBoonPresence = boonIds.Contains(boonid);
                    var updateCondiPresence = boonid != 873 && condiIds.Contains(boonid);
                    foreach (var simul in simulation.Items)
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
                                Add(_boonPresence[i], boonid, simul.GetItemDuration(phase.GetStart(), phase.GetEnd()));
                            if (updateCondiPresence)
                                Add(_condiPresence[i], boonid, simul.GetItemDuration(phase.GetStart(), phase.GetEnd()));
                            foreach (ushort src in simul.GetSrc())
                            {
                                if (distrib.TryGetValue(src, out var toModify))
                                {
                                    toModify.Value += simul.GetDuration(src, phase.GetStart(), phase.GetEnd());
                                    toModify.Overstack += simul.GetOverstack(src, phase.GetStart(), phase.GetEnd());
                                    distrib[src] = toModify;
                                }
                                else
                                {
                                    distrib.Add(src, new OverAndValue(
                                        simul.GetDuration(src, phase.GetStart(), phase.GetEnd()),
                                        simul.GetOverstack(src, phase.GetStart(), phase.GetEnd())));
                                }
                            }
                        }
                    }

                    // Graphs
                    if (requireExtraData)
                    {
                        GenerateExtraBoonData(log, boonid, simulation, phases);
                    }
                    // Precision is reduced to seconds
                    var graphPoints = new List<Point>(capacity: fightDuration + 1);
                    var boonPresence = boonPresencePoints.GetBoonChart();
                    var condiPresence = condiPresencePoints.GetBoonChart();
                    /*if (Replay != null && (updateCondiPresence || updateBoonPresence || defIds.Contains(boonid) || offIds.Contains(boonid)))
                    {
                        foreach (int time in Replay.GetTimes())
                        {
                            Replay.AddBoon(boonid, simulation.GetBoonStackCount(time));
                        }

                    }*/
                    for (int i = 0; i <= fightDuration; i++)
                    {
                        graphPoints.Add(new Point(i, simulation.GetBoonStackCount(1000 * i)));
                        if (updateBoonPresence)
                        {
                            boonPresence[i] = new Point(i, boonPresence[i].Y + (simulation.GetEffectPresence(1000 * i) ? 1 : 0));
                        }
                        if (updateCondiPresence)
                        {
                            condiPresence[i] = new Point(i, condiPresence[i].Y + (simulation.GetEffectPresence(1000 * i) ? 1 : 0));
                        }
                    }
                    _boonPoints[boonid] = new BoonsGraphModel(boon.GetName(), graphPoints);
                }
            }
            _boonPoints[-2] = boonPresencePoints;
            _boonPoints[-3] = condiPresencePoints;
        }
        private void SetMinions(ParsedLog log)
        {
            List<AgentItem> combatMinion = log.GetAgentData().GetNPCAgentList().Where(x => x.MasterAgent == Agent.Agent).ToList();
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
                if (pair.Value.GetDamageLogs(0, log, 0, log.GetBossData().GetAwareDuration()).Count > 0)
                {
                    _minions[pair.Key] = pair.Value;
                }
            }
        }
        protected override void SetDamageLogs(ParsedLog log)
        {
            long timeStart = log.GetBossData().GetFirstAware();
            foreach (CombatItem c in log.GetDamageData(Agent.InstID))
            {
                if (c.Time > log.GetBossData().GetFirstAware() && c.Time < log.GetBossData().GetLastAware())//selecting player or minion as caster
                {
                    long time = c.Time - timeStart;
                    AddDamageLog(time, c);
                }
            }
            Dictionary<string, Minions> minionsList = GetMinions(log);
            foreach (Minions mins in minionsList.Values)
            {
                DamageLogs.AddRange(mins.GetDamageLogs(0, log, 0, log.GetBossData().GetAwareDuration()));
            }
            DamageLogs.Sort((x, y) => x.GetTime() < y.GetTime() ? -1 : 1);
        }
        protected override void SetCastLogs(ParsedLog log)
        {
            long timeStart = log.GetBossData().GetFirstAware();
            CastLog curCastLog = null;
            foreach (CombatItem c in log.GetCastData(Agent.InstID))
            {
                if (!(c.Time > log.GetBossData().GetFirstAware() && c.Time < log.GetBossData().GetLastAware()))
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
