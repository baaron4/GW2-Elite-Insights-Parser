using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using LuckParser.Models.DataModels;

namespace LuckParser.Models.ParseModels
{
    public abstract class AbstractPlayer
    {
        protected AgentItem agent;
        private String character;
        // Boons
        private List<BoonDistribution> boon_distribution = new List<BoonDistribution>();
        private List<Dictionary<int, long>> boon_presence = new List<Dictionary<int, long>>();
        private Dictionary<int, BoonsGraphModel> boon_points = new Dictionary<int, BoonsGraphModel>();
        // DPS
        protected List<DamageLog> damage_logs = new List<DamageLog>();
        protected List<DamageLog> damage_logsFiltered = new List<DamageLog>();
        private Dictionary<int, List<Point>> dps_graph = new Dictionary<int, List<Point>>();
        // Taken damage
        protected List<DamageLog> damageTaken_logs = new List<DamageLog>();
        // Casts
        protected List<CastLog> cast_logs = new List<CastLog>();
        // Constructor
        public AbstractPlayer(AgentItem agent)
        {
            String[] name = agent.getName().Split('\0');
            character = name[0];
            this.agent = agent;
        }
        // Getters
        public ushort getInstid()
        {
            return agent.getInstid();
        }
        public string getCharacter()
        {
            return character;
        }
        public string getProf()
        {
            return agent.getProf();
        }
        public long getDeath(ParsedLog log, long start, long end)
        {
            long offset = log.getBossData().getFirstAware();
            CombatItem dead = log.getCombatList().FirstOrDefault(x => x.getSrcInstid() == agent.getInstid() && x.isStateChange() == ParseEnum.StateChange.ChangeDead && x.getTime() >= start + offset && x.getTime() <= end + offset);
            if (dead != null && dead.getTime() > 0)
            {
                return dead.getTime();
            }
            return 0;
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

        public List<DamageLog> getDamageLogs(int instidFilter, ParsedLog log, long start, long end)//isntid = 0 gets all logs if specefied sets and returns filterd logs
        {
            if (damage_logs.Count == 0)
            {
                setDamageLogs(log);
            }


            if (damage_logsFiltered.Count == 0)
            {
                setFilteredLogs(log);
            }
            if (instidFilter == 0)
            {
                return damage_logs.Where(x => x.getTime() >= start && x.getTime() <= end).ToList();
            }
            else
            {
                return damage_logsFiltered.Where( x => x.getTime() >= start && x.getTime() <= end).ToList();
            }
        }
        public List<DamageLog> getDamageTakenLogs(ParsedLog log, long start, long end)
        {
            if (damageTaken_logs.Count == 0)
            {
                setDamagetakenLogs(log);
            }
            return damageTaken_logs.Where(x => x.getTime() >= start && x.getTime() <= end).ToList();
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
                setBoonDistribution(log,phases, to_track);
            }
            return boon_presence[phase_index];
        }
        public List<CastLog> getCastLogs(ParsedLog log, long start, long end)
        {
            if (cast_logs.Count == 0)
            {
                setCastLogs(log);
            }
            return cast_logs.Where(x => x.getTime() >= start && x.getTime() <= end).ToList();

        }

        public List<CastLog> getCastLogsActDur(ParsedLog log, long start, long end)
        {
            if (cast_logs.Count == 0)
            {
                setCastLogs(log);
            }
            return cast_logs.Where(x => x.getTime() + x.getActDur() >= start && x.getTime() <= end).ToList();

        }
        public List<DamageLog> getJustPlayerDamageLogs(int instidFilter, ParsedLog log, long start, long end)
        {
            return getDamageLogs(instidFilter, log, start, end).Where(x => x.getInstidt() == agent.getInstid()).ToList();
        }
        // privates
        protected void addDamageLog(long time, ushort instid, CombatItem c, List<DamageLog> toFill)
        {
            if (instid == c.getDstInstid() && c.getIFF().getEnum() == "FOE")
            {
                if (c.isBuffremove() == ParseEnum.BuffRemove.None)
                {
                    if (c.isBuff() == 1 && c.getBuffDmg() != 0)//condi
                    {
                        toFill.Add(new DamageLogCondition(time, c));
                    }
                    else if (c.isBuff() == 0 && c.getValue() != 0)//power
                    {
                        toFill.Add(new DamageLogPower(time, c));
                    }
                    else if (c.getResult() == ParseEnum.Result.Absorb || c.getResult() == ParseEnum.Result.Blind || c.getResult() == ParseEnum.Result.Interrupt)
                    {//Hits that where blinded, invulned, interupts
                        toFill.Add(new DamageLogPower(time, c));
                    }
                }
            }
        }
        protected void addDamageTakenLog(long time, ushort instid, CombatItem c)
        {
            if (instid == c.getSrcInstid())
            {
                if (c.isBuff() == 1 && c.getBuffDmg() != 0)
                {
                    //inco,ing condi dmg not working or just not present?
                    // damagetaken.Add(c.getBuffDmg());
                    damageTaken_logs.Add(new DamageLogCondition(time, c));
                }
                else if (c.isBuff() == 0 && c.getValue() >= 0)
                {
                    damageTaken_logs.Add(new DamageLogPower(time, c));

                }

            }
        }
        // Setters
        protected abstract void setDamageLogs(ParsedLog log);     
        protected abstract void setFilteredLogs(ParsedLog log);
        protected abstract void setCastLogs(ParsedLog log);
        protected abstract void setDamagetakenLogs(ParsedLog log);

        private void setBoonDistribution(ParsedLog log,List<PhaseData> phases, List<Boon> to_track)
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
            long death = getDeath(log, 0, dur);
            foreach (Boon boon in to_track)
            {
                int boonid = boon.getID();
                if (to_use.ContainsKey(boonid))
                {
                    List<BoonLog> logs = to_use[boonid];
                    if (logs.Count == 0)
                    {
                        continue;
                    }
                    if (boon_distribution[0].ContainsKey(boonid))
                    {
                        continue;
                    }
                    BoonSimulator simulator = boon.getSimulator();
                    simulator.simulate(logs, dur);
                    if (death > 0 && getCastLogs(log, death + 1, fight_duration).Count > 0)
                    {
                        simulator.trim(death - log.getBossData().getFirstAware());
                    }
                    else
                    {
                        simulator.trim(dur);
                    }
                    List<BoonSimulationItem> simulation = simulator.getSimulationResult();
                    foreach (BoonSimulationItem simul in simulation)
                    {
                        for (int i = 0; i < phases.Count; i++)
                        {
                            PhaseData phase = phases[i];
                            Dictionary<int, long> presenceDict = boon_presence[i];
                            BoonDistribution distrib = boon_distribution[i];
                            if (!distrib.ContainsKey(boonid))
                            {
                                distrib[boonid] = new Dictionary<ushort, OverAndValue>();
                            }
                            if (!presenceDict.ContainsKey(boonid))
                            {
                                presenceDict[boonid] = simul.getItemDuration(phase.getStart(), phase.getEnd());
                            }
                            else
                            {
                                presenceDict[boonid] += simul.getItemDuration(phase.getStart(), phase.getEnd());
                            }
                            foreach (ushort src in simul.getSrc())
                            {
                                if (!distrib[boonid].ContainsKey(src))
                                {
                                    distrib[boonid][src] = new OverAndValue(simul.getDuration(src, phase.getStart(), phase.getEnd()), simul.getOverstack(src, phase.getStart(), phase.getEnd()));
                                }
                                else
                                {
                                    OverAndValue toModify = distrib[boonid][src];
                                    toModify.value += simul.getDuration(src, phase.getStart(), phase.getEnd());
                                    toModify.overstack += simul.getOverstack(src, phase.getStart(), phase.getEnd());
                                    distrib[boonid][src] = toModify;
                                }
                            }
                        }

                    }
                    // Graphs
                    // full precision
                    List<Point> toFill = new List<Point>();
                    List<Point> toFillPresence = new List<Point>();
                    for (int i = 0; i < dur + 1; i++)
                    {
                        toFill.Add(new Point(i, 0));
                        toFillPresence.Add(new Point(i, 0));
                    }
                    foreach (BoonSimulationItem simul in simulation)
                    {
                        int start = (int)simul.getStart();
                        int end = (int)simul.getEnd();

                        for (int i = start; i <= end; i++)
                        {
                            toFill[i] = new Point(i, simul.getStack(i));
                            toFillPresence[i] = new Point(i, simul.getItemDuration() > 0 ? 1 : 0);                     
                        }
                    }
                    // reduce precision to seconds
                    List<Point> reducedPrecision = new List<Point>();
                    List<Point> boonPresence = boon_presence_points.getBoonChart();
                    for (int i = 0; i <= fight_duration; i++)
                    {
                        reducedPrecision.Add(new Point(i, toFill[1000 * i].Y));
                        if (Boon.getBoonList().Select(x => x.getID()).Contains(boonid))
                            boonPresence[i] = new Point(i, boonPresence[i].Y + toFillPresence[1000 * i].Y);
                    }
                    boon_points[boonid] = new BoonsGraphModel(boon.getName(), reducedPrecision);
                }
            }
            boon_points[-2] = boon_presence_points;
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
                        boon_map[c.getSkillID()].Add(new BoonLog(time, src, c.getValue(), 0));
                    }
                    else if (Boon.removePermission(c.getSkillID(), c.isBuffremove(), c.getIFF().getID()))
                    {
                        if (c.isBuffremove() == ParseEnum.BuffRemove.All)//All
                        {
                            List<BoonLog> loglist = boon_map[c.getSkillID()];
                            for (int cnt = loglist.Count() - 1; cnt >= 0; cnt--)
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
                        else if (c.isBuffremove() == ParseEnum.BuffRemove.Single)//Single
                        {
                            List<BoonLog> loglist = boon_map[c.getSkillID()];
                            int cnt = loglist.Count() - 1;
                            BoonLog curBL = loglist[cnt];
                            if (curBL.getTime() + curBL.getValue() > time)
                            {
                                long subtract = (curBL.getTime() + curBL.getValue()) - time;
                                loglist[cnt].addValue(-subtract);
                                // add removed as overstack
                                loglist[cnt].addOverstack((ushort)subtract);
                            }
                        }
                        else if (c.isBuffremove() == ParseEnum.BuffRemove.Manual)//Manuel
                        {
                            List<BoonLog> loglist = boon_map[c.getSkillID()];
                            for (int cnt = loglist.Count() - 1; cnt >= 0; cnt--)
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
            return boon_map;
        }
        

    }
}
