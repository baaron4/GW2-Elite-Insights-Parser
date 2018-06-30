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
        private List<Dictionary<int, long>> boon_presence = new List<Dictionary<int, long>>();
        private Dictionary<int, BoonsGraphModel> boon_points = new Dictionary<int, BoonsGraphModel>();
        // dps graphs
        private Dictionary<int, List<Point>> dps_graph = new Dictionary<int, List<Point>>();
        // Rotation
        private List<RotationItem> rotation = new List<RotationItem>();
        // Minions
        private Dictionary<string, Minions> minions = new Dictionary<string, Minions>();
        // Replay
        private CombatReplay replay = null;

        public AbstractMasterPlayer(AgentItem agent) : base(agent)
        {

        }

        public Dictionary<string, Minions> getMinions(ParsedLog log)
        {
            if (minions.Count == 0)
            {
                setMinions(log);
            }
            return minions;
        }  
        public void addDPSGraph(int id, List<Point> graph)
        {
            dps_graph[id] = graph;
        }
        public List<Point> getDPSGraph(int id)
        {
            if (dps_graph.ContainsKey(id))
            {
                return dps_graph[id];
            }
            return new List<Point>();
        }
        public List<RotationItem> getRotation(ParsedLog log, bool icons)
        {
            if (rotation.Count == 0)
            {
                setRotation(log, icons);
            }
            return rotation;
        }
        public BoonDistribution getBoonDistribution(ParsedLog log, List<PhaseData> phases, List<Boon> to_track, int phase_index)
        {
            if (boon_distribution.Count == 0)
            {
                setBoonDistribution(log, phases, to_track);
            }
            return boon_distribution[phase_index];
        }
        public Dictionary<int, BoonsGraphModel> getBoonGraphs(ParsedLog log, List<PhaseData> phases, List<Boon> to_track)
        {
            if (boon_distribution.Count == 0)
            {
                setBoonDistribution(log, phases, to_track);
            }
            return boon_points;
        }
        public Dictionary<int, long> getBoonPresence(ParsedLog log, List<PhaseData> phases, List<Boon> to_track, int phase_index)
        {
            if (boon_distribution.Count == 0)
            {
                setBoonDistribution(log, phases, to_track);
            }
            return boon_presence[phase_index];
        }
        public void initCombatReplay(ParsedLog log)
        {
            if (replay == null)
            {
                replay = new CombatReplay();
                setMovements(log);
            }
        }
        public CombatReplay getCombatReplay()
        {
            return replay;
        }
        // private getters
        private BoonMap getBoonMap(ParsedLog log, List<Boon> to_track)
        {
            BoonMap boon_map = new BoonMap();
            boon_map.add(to_track);
            // Fill in Boon Map
            long time_start = log.getBossData().getFirstAware();

            foreach (CombatItem c in log.getBoonData())
            {
                if (!boon_map.ContainsKey(c.getSkillID()))
                {
                    continue;
                }
                long time = c.getTime() - time_start;
                ushort dst = c.isBuffremove() == ParseEnum.BuffRemove.None ? c.getDstInstid() : c.getSrcInstid();
                if (agent.getInstid() == dst && time > 0 && time < log.getBossData().getAwareDuration())
                {
                    ushort src = c.getSrcMasterInstid() > 0 ? c.getSrcMasterInstid() : c.getSrcInstid();
                    if (c.isBuffremove() == ParseEnum.BuffRemove.None)
                    {
                        List<BoonLog> loglist = boon_map[c.getSkillID()];
                        if (loglist.Count == 0 && c.getOverstackValue() > 0)
                        {
                            loglist.Add(new BoonLog(0, src, time, 0));
                        }
                        loglist.Add(new BoonLog(time, src, c.getValue(), 0));
                    }
                    else if (Boon.removePermission(c.getSkillID(), c.isBuffremove(), c.getIFF()))
                    {
                        if (c.isBuffremove() == ParseEnum.BuffRemove.All)//All
                        {
                            List<BoonLog> loglist = boon_map[c.getSkillID()];
                            if (loglist.Count == 0)
                            {
                                loglist.Add(new BoonLog(0, src, time, 0));
                            }
                            else
                            {
                                for (int cnt = loglist.Count - 1; cnt >= 0; cnt--)
                                {
                                    BoonLog curBL = loglist[cnt];
                                    if (curBL.getTime() + curBL.getValue() > time)
                                    {
                                        long subtract = (curBL.getTime() + curBL.getValue()) - time;
                                        loglist[cnt].addValue(-subtract);
                                        // add removed as overstack
                                        loglist[cnt].addOverstack((ushort)subtract);
                                    }
                                }
                            }
                        }
                        else if (c.isBuffremove() == ParseEnum.BuffRemove.Single)//Single
                        {
                            List<BoonLog> loglist = boon_map[c.getSkillID()];
                            if (loglist.Count == 0)
                            {
                                loglist.Add(new BoonLog(0, src, time, 0));
                            }
                            else
                            {
                                int cnt = loglist.Count - 1;
                                BoonLog curBL = loglist[cnt];
                                if (curBL.getTime() + curBL.getValue() > time)
                                {
                                    long subtract = (curBL.getTime() + curBL.getValue()) - time;
                                    loglist[cnt].addValue(-subtract);
                                    // add removed as overstack
                                    loglist[cnt].addOverstack((ushort)subtract);
                                }
                            }
                        }
                        else if (c.isBuffremove() == ParseEnum.BuffRemove.Manual)//Manuel
                        {
                            List<BoonLog> loglist = boon_map[c.getSkillID()];
                            if (loglist.Count == 0)
                            {
                                loglist.Add(new BoonLog(0, src, time, 0));
                            }
                            else
                            {
                                for (int cnt = loglist.Count - 1; cnt >= 0; cnt--)
                                {
                                    BoonLog curBL = loglist[cnt];
                                    long ctime = curBL.getTime() + curBL.getValue();
                                    if (ctime > time)
                                    {
                                        long subtract = (curBL.getTime() + curBL.getValue()) - time;
                                        loglist[cnt].addValue(-subtract);
                                        // add removed as overstack
                                        loglist[cnt].addOverstack((ushort)subtract);
                                        break;
                                    }
                                }
                            }

                        }
                    }

                }
            }
            return boon_map;
        }
        private void setMovements(ParsedLog log)
        {
            foreach (CombatItem c in log.getMovementData())
            {
                if (c.getSrcInstid() != agent.getInstid())
                {
                    continue;
                }
                long time = c.getTime() - log.getBossData().getFirstAware();
                if (time < 0)
                {
                    continue;
                }
                byte[] xy = BitConverter.GetBytes(c.getDstAgent());
                float X = BitConverter.ToSingle(xy, 0);
                float Y = BitConverter.ToSingle(xy, 4);
                replay.addTime((int)time);
                if (c.isStateChange() == ParseEnum.StateChange.Position)
                {
                    replay.addPosition(new Point3D(X, Y, c.getValue()));
                }
                else
                {
                    replay.addVelocity(new Point3D(X, Y, c.getValue()));
                }
            }
        }
        private void setBoonDistribution(ParsedLog log, List<PhaseData> phases, List<Boon> to_track)
        {
            BoonMap to_use = getBoonMap(log, to_track);
            long dur = log.getBossData().getAwareDuration();
            int fight_duration = (int)(dur) / 1000;
            // Init boon presence points
            BoonsGraphModel boon_presence_points = new BoonsGraphModel("Number of Boons");
            for (int i = 0; i <= fight_duration; i++)
            {
                boon_presence_points.getBoonChart().Add(new Point(i, 0));
            }
            for (int i = 0; i < phases.Count; i++)
            {
                boon_distribution.Add(new BoonDistribution());
                boon_presence.Add(new Dictionary<int, long>());
            }

            var toFill = new Point[dur + 1];
            var toFillPresence = new Point[dur + 1];

            long death = getDeath(log, 0, dur);

            foreach (Boon boon in to_track)
            {
                int boonid = boon.getID();
                if (to_use.TryGetValue(boonid, out var logs) && logs.Count != 0)
                {
                    if (boon_distribution[0].ContainsKey(boonid))
                    {
                        continue;
                    }
                    var simulator = boon.CreateSimulator();
                    simulator.simulate(logs, dur);
                    if (death > 0 && getCastLogs(log, death + 1, fight_duration).Count > 0)
                    {
                        simulator.trim(death - log.getBossData().getFirstAware());
                    }
                    else
                    {
                        simulator.trim(dur);
                    }
                    var simulation = simulator.getSimulationResult();
                    foreach (var simul in simulation)
                    {
                        for (int i = 0; i < phases.Count; i++)
                        {
                            var phase = phases[i];
                            var presenceDict = boon_presence[i];
                            if (!boon_distribution[i].TryGetValue(boonid, out var distrib))
                            {
                                distrib = new Dictionary<ushort, OverAndValue>();
                                boon_distribution[i].Add(boonid, distrib);
                            }
                            Add(presenceDict, boonid, simul.getItemDuration(phase.getStart(), phase.getEnd()));
                            foreach (ushort src in simul.getSrc())
                            {
                                if (distrib.TryGetValue(src, out var toModify))
                                {
                                    toModify.value += simul.getDuration(src, phase.getStart(), phase.getEnd());
                                    toModify.overstack += simul.getOverstack(src, phase.getStart(), phase.getEnd());
                                    distrib[src] = toModify;
                                }
                                else
                                {
                                    distrib.Add(src, new OverAndValue(
                                        simul.getDuration(src, phase.getStart(), phase.getEnd()),
                                        simul.getOverstack(src, phase.getStart(), phase.getEnd())));
                                }
                            }
                        }
                    }
                    // Graphs
                    // full precision
                    for (int i = 0; i <= dur; i++)
                    {
                        toFill[i] = new Point(i, 0);
                        toFillPresence[i] = new Point(i, 0);
                    }
                    foreach (var simul in simulation)
                    {
                        int start = (int)simul.getStart();
                        int end = (int)simul.getEnd();

                        bool present = simul.getItemDuration() > 0;
                        for (int i = start; i <= end; i++)
                        {
                            toFill[i] = new Point(i, simul.getStack(i));
                            toFillPresence[i] = new Point(i, present ? 1 : 0);
                        }
                    }
                    // reduce precision to seconds
                    var reducedPrecision = new List<Point>(capacity: fight_duration + 1);
                    var boonPresence = boon_presence_points.getBoonChart();
                    var updateBoonPresence = Boon.getBoonList().Any(x => x.getID() == boonid);
                    for (int i = 0; i <= fight_duration; i++)
                    {
                        reducedPrecision.Add(new Point(i, toFill[1000 * i].Y));
                        if (updateBoonPresence)
                        {
                            boonPresence[i] = new Point(i, boonPresence[i].Y + toFillPresence[1000 * i].Y);
                        }
                    }
                    boon_points[boonid] = new BoonsGraphModel(boon.getName(), reducedPrecision);
                }
            }
            boon_points[-2] = boon_presence_points;
        }
        private void setRotation(ParsedLog log, bool icons)
        {
            List<CastLog> cls = getCastLogs(log, 0, log.getBossData().getAwareDuration());
            foreach (CastLog cl in cls)
            {
                RotationItem rot = new RotationItem();
                rot.findName(log.getSkillData(), cl.getID());
                rot.setDuration(cl.getActDur());
                rot.setEndStatus(cl.endActivation());
                rot.setStartStatus(cl.startActivation());
            }
        }
        private void setMinions(ParsedLog log)
        {
            List<AgentItem> combatMinion = log.getAgentData().getNPCAgentList().Where(x => x.getMasterAgent() == agent.getAgent()).ToList();
            foreach (AgentItem agent in combatMinion)
            {
                string id = agent.getName();
                if (!minions.ContainsKey(id))
                {
                    minions[id] = new Minions(id.GetHashCode());
                }
                minions[id].Add(new Minion(agent.getInstid(), agent));
            }
        }

        protected override void setFilteredLogs(ParsedLog log)
        {
            long time_start = log.getBossData().getFirstAware();
            foreach (CombatItem c in log.getDamageData())
            {
                if (agent.getInstid() == c.getSrcInstid() && c.getTime() > log.getBossData().getFirstAware() && c.getTime() < log.getBossData().getLastAware())
                {
                    long time = c.getTime() - time_start;
                    addDamageLog(time, log.getBossData().getInstid(), c, damage_logsFiltered);
                }
            }
            Dictionary<string, Minions> min_list = getMinions(log);
            foreach (Minions mins in min_list.Values)
            {
                damage_logsFiltered.AddRange(mins.getDamageLogs(log.getBossData().getInstid(), log, 0, log.getBossData().getAwareDuration()));
            }
            damage_logsFiltered.Sort((x, y) => x.getTime() < y.getTime() ? -1 : 1);
        }
        protected override void setCastLogs(ParsedLog log)
        {
            long time_start = log.getBossData().getFirstAware();
            CastLog curCastLog = null;
            foreach (CombatItem c in log.getCastData())
            {
                if (! (c.getTime() > log.getBossData().getFirstAware() && c.getTime() < log.getBossData().getLastAware()))
                {
                    continue;
                }
                ParseEnum.StateChange state = c.isStateChange();
                if (state == ParseEnum.StateChange.Normal)
                {
                    if (agent.getInstid() == c.getSrcInstid())//selecting player as caster
                    {
                        if (c.isActivation().IsCasting())
                        {
                            long time = c.getTime() - time_start;
                            curCastLog = new CastLog(time, c.getSkillID(), c.getValue(), c.isActivation());
                            cast_logs.Add(curCastLog);
                        }
                        else
                        {
                            if (curCastLog != null)
                            {
                                if (curCastLog.getID() == c.getSkillID())
                                {
                                    curCastLog.setEndStatus(c.getValue(), c.isActivation());                               
                                    curCastLog = null;
                                }
                            }
                        }

                    }
                }
                else if (state == ParseEnum.StateChange.WeaponSwap)
                {//Weapon swap
                    if (agent.getInstid() == c.getSrcInstid())//selecting player as caster
                    {
                        if ((int)c.getDstAgent() == 4 || (int)c.getDstAgent() == 5)
                        {
                            long time = c.getTime() - time_start;
                            CastLog swapLog = new CastLog(time, -2, (int)c.getDstAgent(), c.isActivation());
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
