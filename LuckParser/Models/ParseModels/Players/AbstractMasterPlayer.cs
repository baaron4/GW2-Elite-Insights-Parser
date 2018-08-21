using LuckParser.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public abstract class AbstractMasterPlayer : AbstractPlayer
    {
        // Boons
        private List<BoonDistribution> boon_distribution = new List<BoonDistribution>();
        private List<Dictionary<long, long>> boon_presence = new List<Dictionary<long, long>>();
        private List<Dictionary<long, long>> condi_presence = new List<Dictionary<long, long>>();
        private Dictionary<long, BoonsGraphModel> boon_points = new Dictionary<long, BoonsGraphModel>();
        private Dictionary<long, Dictionary<int, string[]>> boon_extra = new Dictionary<long, Dictionary<int, string[]>>();
        // dps graphs
        private Dictionary<int, List<Point>> dps_graph = new Dictionary<int, List<Point>>();
        // Minions
        private Dictionary<string, Minions> minions = new Dictionary<string, Minions>();
        // Replay
        protected CombatReplay replay = null;

        public AbstractMasterPlayer(AgentItem agent) : base(agent)
        {

        }

        public Dictionary<string, Minions> GetMinions(ParsedLog log)
        {
            if (minions.Count == 0)
            {
                SetMinions(log);
            }
            return minions;
        }
        public void AddDPSGraph(int id, List<Point> graph)
        {
            dps_graph[id] = graph;
        }
        public List<Point> GetDPSGraph(int id)
        {
            if (dps_graph.ContainsKey(id))
            {
                return dps_graph[id];
            }
            return new List<Point>();
        }
        public BoonDistribution GetBoonDistribution(ParsedLog log, List<PhaseData> phases, List<Boon> to_track, int phase_index)
        {
            if (boon_distribution.Count == 0)
            {
                SetBoonDistribution(log, phases, to_track);
            }
            return boon_distribution[phase_index];
        }
        public Dictionary<long, BoonsGraphModel> GetBoonGraphs(ParsedLog log, List<PhaseData> phases, List<Boon> to_track)
        {
            if (boon_distribution.Count == 0)
            {
                SetBoonDistribution(log, phases, to_track);
            }
            return boon_points;
        }
        public Dictionary<long, long> GetBoonPresence(ParsedLog log, List<PhaseData> phases, List<Boon> to_track, int phase_index)
        {
            if (boon_distribution.Count == 0)
            {
                SetBoonDistribution(log, phases, to_track);
            }
            return boon_presence[phase_index];
        }

        public Dictionary<long, Dictionary<int, string[]>> GetExtraBoonData(ParsedLog log, List<PhaseData> phases, List<Boon> to_track)
        {
            if (boon_distribution.Count == 0)
            {
                SetBoonDistribution(log, phases, to_track);
            }
            return boon_extra;
        }

        public Dictionary<long, long> GetCondiPresence(ParsedLog log, List<PhaseData> phases, List<Boon> to_track, int phase_index)
        {
            if (boon_distribution.Count == 0)
            {
                SetBoonDistribution(log, phases, to_track);
            }
            return condi_presence[phase_index];
        }
        public void InitCombatReplay(ParsedLog log, int pollingRate, bool trim, bool forceInterpolate)
        {
            if (log.GetMovementData().Count == 0)
            {
                // no movement data, old arc version
                return;
            }
            if (replay == null)
            {
                replay = new CombatReplay();
                SetMovements(log);
                replay.PollingRate(pollingRate, log.GetBossData().GetAwareDuration(), forceInterpolate);
                SetCombatReplayIcon(log);
                if (trim)
                {
                    CombatItem test = log.GetCombatList().FirstOrDefault(x => x.GetSrcAgent() == agent.GetAgent() && (x.IsStateChange().IsDead() || x.IsStateChange().IsDespawn()));
                    if (test != null)
                    {
                        replay.Trim(agent.GetFirstAware() - log.GetBossData().GetFirstAware(), test.GetTime() - log.GetBossData().GetFirstAware());
                    }
                    else
                    {
                        replay.Trim(agent.GetFirstAware() - log.GetBossData().GetFirstAware(), agent.GetLastAware() - log.GetBossData().GetFirstAware());
                    }
                }
                SetAdditionalCombatReplayData(log, pollingRate);
            }
        }
        public CombatReplay GetCombatReplay()
        {
            return replay;
        }

        public long GetDeath(ParsedLog log, long start, long end)
        {
            long offset = log.GetBossData().GetFirstAware();
            CombatItem dead = log.GetCombatList().LastOrDefault(x => x.GetSrcInstid() == agent.GetInstid() && x.IsStateChange().IsDead() && x.GetTime() >= start + offset && x.GetTime() <= end + offset);
            if (dead != null && dead.GetTime() > 0)
            {
                return dead.GetTime();
            }
            return 0;
        }

        public abstract void AddMechanics(ParsedLog log);

        // private getters
        private BoonMap GetBoonMap(ParsedLog log, List<Boon> to_track)
        {
            BoonMap boon_map = new BoonMap
            {
                to_track
            };
            // Fill in Boon Map
            long time_start = log.GetBossData().GetFirstAware();
            List<long> tableIds = Boon.GetBoonList().Select(x => x.GetID()).ToList();
            tableIds.AddRange(Boon.GetOffensiveTableList().Select(x => x.GetID()));
            tableIds.AddRange(Boon.GetDefensiveTableList().Select(x => x.GetID()));
            tableIds.AddRange(Boon.GetCondiBoonList().Select(x => x.GetID()));
            foreach (CombatItem c in log.GetBoonData())
            {
                if (!boon_map.ContainsKey(c.GetSkillID()))
                {
                    continue;
                }
                long time = c.GetTime() - time_start;
                ushort dst = c.IsBuffremove() == ParseEnum.BuffRemove.None ? c.GetDstInstid() : c.GetSrcInstid();
                if (agent.GetInstid() == dst)
                {
                    // don't add buff initial table boons and buffs in non golem mode, for others overstack is irrevelant
                    if (c.IsStateChange() == ParseEnum.StateChange.BuffInitial && (log.IsBenchmarkMode() || !tableIds.Contains(c.GetSkillID())))
                    {
                        List<BoonLog> loglist = boon_map[c.GetSkillID()];
                        loglist.Add(new BoonLog(0, 0, long.MaxValue, 0));
                    }
                    else if (time >= 0 && time < log.GetBossData().GetAwareDuration())
                    {
                        if (c.IsBuffremove() == ParseEnum.BuffRemove.None)
                        {
                            ushort src = c.GetSrcMasterInstid() > 0 ? c.GetSrcMasterInstid() : c.GetSrcInstid();
                            List<BoonLog> loglist = boon_map[c.GetSkillID()];

                            if (loglist.Count == 0 && c.GetOverstackValue() > 0)
                            {
                                loglist.Add(new BoonLog(0, 0, time, 0));
                            }
                            loglist.Add(new BoonLog(time, src, c.GetValue(), 0));
                        }
                        else if (Boon.RemovePermission(c.GetSkillID(), c.IsBuffremove(), c.GetIFF()) && time < log.GetBossData().GetAwareDuration() - 50)
                        {
                            if (c.IsBuffremove() == ParseEnum.BuffRemove.All)//All
                            {
                                List<BoonLog> loglist = boon_map[c.GetSkillID()];
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
                            else if (c.IsBuffremove() == ParseEnum.BuffRemove.Single)//Single
                            {
                                List<BoonLog> loglist = boon_map[c.GetSkillID()];
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
                            else if (c.IsBuffremove() == ParseEnum.BuffRemove.Manual)//Manuel
                            {
                                List<BoonLog> loglist = boon_map[c.GetSkillID()];
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
            }
            return boon_map;
        }
        private void SetMovements(ParsedLog log)
        {
            foreach (CombatItem c in log.GetMovementData())
            {
                if (c.GetSrcInstid() != agent.GetInstid())
                {
                    continue;
                }
                long time = c.GetTime() - log.GetBossData().GetFirstAware();
                byte[] xy = BitConverter.GetBytes(c.GetDstAgent());
                float X = BitConverter.ToSingle(xy, 0);
                float Y = BitConverter.ToSingle(xy, 4);
                if (c.IsStateChange() == ParseEnum.StateChange.Position)
                {
                    replay.AddPosition(new Point3D(X, Y, c.GetValue(), time));
                }
                else
                {
                    replay.AddVelocity(new Point3D(X, Y, c.GetValue(), time));
                }
            }
        }
        protected abstract void SetAdditionalCombatReplayData(ParsedLog log, int pollingRate);
        protected abstract void SetCombatReplayIcon(ParsedLog log);

        private void GenerateExtraBoonData(ParsedLog log, long boonid, Point[] accurateUptime, List<PhaseData> phases)
        {

            switch (boonid)
            {
                // Frost Spirit
                case 50421:
                    boon_extra[boonid] = new Dictionary<int, string[]>();
                    for (int i = 0; i < phases.Count; i++)
                    {
                        List<DamageLog> dmLogs = GetJustPlayerDamageLogs(0, log, phases[i].GetStart(), phases[i].GetEnd());
                        int totalDamage = Math.Max(dmLogs.Sum(x => x.GetDamage()), 1);
                        int totalBossDamage = Math.Max(dmLogs.Where(x => x.GetDstInstidt() == log.GetBossData().GetInstid()).Sum(x => x.GetDamage()), 1);
                        List<DamageLog> effect = dmLogs.Where(x => accurateUptime[(int)x.GetTime()].Y > 0 && x.IsCondi() == 0).ToList();
                        List<DamageLog> effectBoss = effect.Where(x => x.GetDstInstidt() == log.GetBossData().GetInstid()).ToList();
                        int damage = (int)(effect.Sum(x => x.GetDamage()) / 21.0);
                        int bossDamage = (int)(effectBoss.Sum(x => x.GetDamage()) / 21.0);
                        double gain = Math.Round(100.0 * ((double)totalDamage / (totalDamage - damage) - 1.0), 2);
                        double gainBoss = Math.Round(100.0 * ((double)totalBossDamage / (totalBossDamage - bossDamage) - 1.0), 2);
                        string gainText = effect.Count + " out of " + dmLogs.Count(x => x.IsCondi() == 0) + " hits <br> Pure Frost Spirit Damage: "
                                + damage + "<br> Effective Damage Increase: " + gain + "%";
                        string gainBossText = effectBoss.Count + " out of " + dmLogs.Count(x => x.GetDstInstidt() == log.GetBossData().GetInstid() && x.IsCondi() == 0) + " hits <br> Pure Frost Spirit Damage: "
                                + bossDamage + "<br> Effective Damage Increase: " + gainBoss + "%";
                        boon_extra[boonid][i] = new string[] { gainText, gainBossText };
                    }
                    break;
                // Kalla Elite
                case 45026:
                    boon_extra[boonid] = new Dictionary<int, string[]>();
                    for (int i = 0; i < phases.Count; i++)
                    {
                        List<DamageLog> dmLogs = GetJustPlayerDamageLogs(0, log, phases[i].GetStart(), phases[i].GetEnd());
                        int totalDamage = Math.Max(dmLogs.Sum(x => x.GetDamage()), 1);
                        int totalBossDamage = Math.Max(dmLogs.Where(x => x.GetDstInstidt() == log.GetBossData().GetInstid()).Sum(x => x.GetDamage()), 1);
                        int effectCount = dmLogs.Where(x => accurateUptime[(int)x.GetTime()].Y > 0 && x.IsCondi() == 0).Count();
                        int effectBossCount = dmLogs.Where(x => accurateUptime[(int)x.GetTime()].Y > 0 && x.IsCondi() == 0 && x.GetDstInstidt() == log.GetBossData().GetInstid()).Count();
                        int damage = (int)(effectCount * (325 + 3000 * 0.04));
                        int bossDamage = (int)(effectBossCount * (325 + 3000 * 0.04));
                        double gain = Math.Round(100.0 * ((double)(totalDamage + damage) / totalDamage - 1.0), 2);
                        double gainBoss = Math.Round(100.0 * ((double)(totalBossDamage + bossDamage) / totalBossDamage - 1.0), 2);
                        string gainText = effectCount + " out of " + dmLogs.Count(x => x.IsCondi() == 0) + " hits <br> Estimated Soulcleave Damage: "
                                + damage + "<br> Estimated Damage Increase: " + gain + "%";
                        string gainBossText = effectBossCount + " out of " + dmLogs.Count(x => x.GetDstInstidt() == log.GetBossData().GetInstid() && x.IsCondi() == 0) + " hits <br> Estimated Soulcleave Damage: "
                                + bossDamage + "<br> Estimated Damage Increase: " + gainBoss + "%";
                        boon_extra[boonid][i] = new string[] { gainText, gainBossText };
                    }
                    break;
                // GoE
                case 31803:
                    boon_extra[boonid] = new Dictionary<int, string[]>();
                    for (int i = 0; i < phases.Count; i++)
                    {
                        List<DamageLog> dmLogs = GetJustPlayerDamageLogs(0, log, phases[i].GetStart(), phases[i].GetEnd());
                        int totalDamage = Math.Max(dmLogs.Sum(x => x.GetDamage()), 1);
                        int totalBossDamage = Math.Max(dmLogs.Where(x => x.GetDstInstidt() == log.GetBossData().GetInstid()).Sum(x => x.GetDamage()), 1);
                        List<DamageLog> effect = dmLogs.Where(x => accurateUptime[(int)x.GetTime()].Y > 0 && x.IsCondi() == 0).ToList();
                        List<DamageLog> effectBoss = effect.Where(x => x.GetDstInstidt() == log.GetBossData().GetInstid()).ToList();
                        int damage = (int)(effect.Sum(x => x.GetDamage()) / 11.0);
                        int bossDamage = (int)(effectBoss.Sum(x => x.GetDamage()) / 11.0);
                        double gain = Math.Round(100.0 * ((double)totalDamage / (totalDamage - damage) - 1.0), 2);
                        double gainBoss = Math.Round(100.0 * ((double)totalBossDamage / (totalBossDamage - bossDamage) - 1.0), 2);
                        string gainText = effect.Count + " out of " + dmLogs.Count(x => x.IsCondi() == 0) + " hits <br> Pure GoE Damage: "
                                + damage + "<br> Effective Damage Increase: " + gain + "%";
                        string gainBossText = effectBoss.Count + " out of " + dmLogs.Count(x => x.GetDstInstidt() == log.GetBossData().GetInstid() && x.IsCondi() == 0) + " hits <br> Pure GoE Damage: "
                                + bossDamage + "<br> Effective Damage Increase: " + gainBoss + "%";
                        boon_extra[boonid][i] = new string[] { gainText, gainBossText };
                    }
                    break;
                default:
                    break;
            }
        }

        private void SetBoonDistribution(ParsedLog log, List<PhaseData> phases, List<Boon> to_track)
        {
            BoonMap to_use = GetBoonMap(log, to_track);
            long dur = log.GetBossData().GetAwareDuration();
            int fight_duration = (int)(dur) / 1000;
            // Init boon/condi presence points
            BoonsGraphModel boon_presence_points = new BoonsGraphModel("Number of Boons");
            BoonsGraphModel condi_presence_points = new BoonsGraphModel("Number of Conditions");
            HashSet<long> extraDataID = new HashSet<long>
            {
                50421,
                45026,
                31803
            };
            for (int i = 0; i <= fight_duration; i++)
            {
                boon_presence_points.GetBoonChart().Add(new Point(i, 0));
                condi_presence_points.GetBoonChart().Add(new Point(i, 0));
            }
            for (int i = 0; i < phases.Count; i++)
            {
                boon_distribution.Add(new BoonDistribution());
                boon_presence.Add(new Dictionary<long, long>());
                condi_presence.Add(new Dictionary<long, long>());
            }

            var toFill = new Point[dur + 1];
            var toFillPresence = new Point[dur + 1];

            long death = GetDeath(log, 0, dur) - log.GetBossData().GetFirstAware();

            foreach (Boon boon in to_track)
            {
                long boonid = boon.GetID();
                if (to_use.TryGetValue(boonid, out var logs) && logs.Count != 0)
                {
                    if (boon_distribution[0].ContainsKey(boonid))
                    {
                        continue;
                    }
                    bool requireExtraData = extraDataID.Contains(boonid);
                    var simulator = boon.CreateSimulator(log);
                    simulator.Simulate(logs, dur);
                    if (death > 0 && GetCastLogs(log, death + 5000, fight_duration).Count == 0)
                    {
                        simulator.Trim(death);
                    }
                    else
                    {
                        simulator.Trim(dur);
                    }
                    var simulation = simulator.GetSimulationResult();
                    var updateBoonPresence = Boon.GetBoonList().Any(x => x.GetID() == boonid);
                    var updateCondiPresence = boonid != 873 && Boon.GetCondiBoonList().Any(x => x.GetID() == boonid);
                    foreach (var simul in simulation)
                    {
                        for (int i = 0; i < phases.Count; i++)
                        {
                            var phase = phases[i];
                            if (!boon_distribution[i].TryGetValue(boonid, out var distrib))
                            {
                                distrib = new Dictionary<ushort, OverAndValue>();
                                boon_distribution[i].Add(boonid, distrib);
                            }
                            if (updateBoonPresence)
                                Add(boon_presence[i], boonid, simul.GetItemDuration(phase.GetStart(), phase.GetEnd()));
                            if (updateCondiPresence)
                                Add(condi_presence[i], boonid, simul.GetItemDuration(phase.GetStart(), phase.GetEnd()));
                            foreach (ushort src in simul.GetSrc())
                            {
                                if (distrib.TryGetValue(src, out var toModify))
                                {
                                    toModify.value += simul.GetDuration(src, phase.GetStart(), phase.GetEnd());
                                    toModify.overstack += simul.GetOverstack(src, phase.GetStart(), phase.GetEnd());
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
                    // full precision
                    for (int i = 0; i <= dur; i++)
                    {
                        toFill[i] = new Point(i, 0);
                        if (updateBoonPresence || updateCondiPresence)
                        {
                            toFillPresence[i] = new Point(i, 0);
                        }
                    }
                    foreach (var simul in simulation)
                    {
                        int start = (int)simul.GetStart();
                        int end = (int)simul.GetEnd();

                        bool present = simul.GetItemDuration() > 0;
                        for (int i = start; i <= end; i++)
                        {
                            toFill[i] = new Point(i, simul.GetStack(i));
                            if (updateBoonPresence || updateCondiPresence)
                            {
                                toFillPresence[i] = new Point(i, present ? 1 : 0);
                            }
                        }
                    }
                    if (requireExtraData)
                    {
                        GenerateExtraBoonData(log, boonid, toFill, phases);
                    }
                    // reduce precision to seconds
                    var reducedPrecision = new List<Point>(capacity: fight_duration + 1);
                    var boonPresence = boon_presence_points.GetBoonChart();
                    var condiPresence = condi_presence_points.GetBoonChart();
                    if (replay != null && (updateCondiPresence || updateBoonPresence || Boon.GetDefensiveTableList().Any(x => x.GetID() == boonid) || Boon.GetOffensiveTableList().Any(x => x.GetID() == boonid)))
                    {
                        foreach (int time in replay.GetTimes())
                        {
                            replay.AddBoon(boonid, toFill[time].Y);
                        }

                    }
                    for (int i = 0; i <= fight_duration; i++)
                    {
                        reducedPrecision.Add(new Point(i, toFill[1000 * i].Y));
                        if (updateBoonPresence)
                        {
                            boonPresence[i] = new Point(i, boonPresence[i].Y + toFillPresence[1000 * i].Y);
                        }
                        if (updateCondiPresence)
                        {
                            condiPresence[i] = new Point(i, condiPresence[i].Y + toFillPresence[1000 * i].Y);
                        }
                    }
                    boon_points[boonid] = new BoonsGraphModel(boon.GetName(), reducedPrecision);
                }
            }
            boon_points[-2] = boon_presence_points;
            boon_points[-3] = condi_presence_points;
        }
        private void SetMinions(ParsedLog log)
        {
            List<AgentItem> combatMinion = log.GetAgentData().GetNPCAgentList().Where(x => x.GetMasterAgent() == agent.GetAgent()).ToList();
            Dictionary<string, Minions> auxMinions = new Dictionary<string, Minions>();
            foreach (AgentItem agent in combatMinion)
            {
                string id = agent.GetName();
                if (!auxMinions.ContainsKey(id))
                {
                    auxMinions[id] = new Minions(id.GetHashCode());
                }
                auxMinions[id].Add(new Minion(agent.GetInstid(), agent));
            }
            foreach (KeyValuePair<string, Minions> pair in auxMinions)
            {
                if (pair.Value.GetDamageLogs(0, log, 0, log.GetBossData().GetAwareDuration()).Count > 0)
                {
                    minions[pair.Key] = pair.Value;
                }
            }
        }

        protected override void SetDamageLogs(ParsedLog log)
        {
            long time_start = log.GetBossData().GetFirstAware();
            foreach (CombatItem c in log.GetDamageData())
            {
                if (agent.GetInstid() == c.GetSrcInstid() && c.GetTime() > log.GetBossData().GetFirstAware() && c.GetTime() < log.GetBossData().GetLastAware())//selecting player or minion as caster
                {
                    long time = c.GetTime() - time_start;
                    AddDamageLog(time, c);
                }
            }
            Dictionary<string, Minions> min_list = GetMinions(log);
            foreach (Minions mins in min_list.Values)
            {
                damage_logs.AddRange(mins.GetDamageLogs(0, log, 0, log.GetBossData().GetAwareDuration()));
            }
            damage_logs.Sort((x, y) => x.GetTime() < y.GetTime() ? -1 : 1);
        }
        protected override void SetCastLogs(ParsedLog log)
        {
            long time_start = log.GetBossData().GetFirstAware();
            CastLog curCastLog = null;
            foreach (CombatItem c in log.GetCastData())
            {
                if (!(c.GetTime() > log.GetBossData().GetFirstAware() && c.GetTime() < log.GetBossData().GetLastAware()))
                {
                    continue;
                }
                ParseEnum.StateChange state = c.IsStateChange();
                if (state == ParseEnum.StateChange.Normal)
                {
                    if (agent.GetInstid() == c.GetSrcInstid())//selecting player as caster
                    {
                        if (c.IsActivation().IsCasting())
                        {
                            long time = c.GetTime() - time_start;
                            curCastLog = new CastLog(time, c.GetSkillID(), c.GetValue(), c.IsActivation());
                            cast_logs.Add(curCastLog);
                        }
                        else
                        {
                            if (curCastLog != null)
                            {
                                if (curCastLog.GetID() == c.GetSkillID())
                                {
                                    curCastLog.SetEndStatus(c.GetValue(), c.IsActivation());
                                    curCastLog = null;
                                }
                            }
                        }

                    }
                }
                else if (state == ParseEnum.StateChange.WeaponSwap)
                {//Weapon swap
                    if (agent.GetInstid() == c.GetSrcInstid())//selecting player as caster
                    {
                        if ((int)c.GetDstAgent() == 4 || (int)c.GetDstAgent() == 5)
                        {
                            long time = c.GetTime() - time_start;
                            CastLog swapLog = new CastLog(time, -2, (int)c.GetDstAgent(), c.IsActivation());
                            cast_logs.Add(swapLog);
                        }
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

    }
}
