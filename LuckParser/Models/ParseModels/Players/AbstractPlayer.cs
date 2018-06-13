using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace LuckParser.Models.ParseModels
{
    public abstract class AbstractPlayer
    {
        protected ushort instid;
        private String character;
        protected String prof;
        // Boons
        private List<BoonDistribution> boon_distribution = new List<BoonDistribution>();
        private List<Dictionary<int, long>> boon_presence = new List<Dictionary<int, long>>();
        private Dictionary<int, BoonsGraphModel> boon_points = new Dictionary<int, BoonsGraphModel>();
        // DPS
        protected List<DamageLog> damage_logs = new List<DamageLog>();
        private List<DamageLog> damage_logsFiltered = new List<DamageLog>();
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
            instid = agent.getInstid();
        }
        // Getters
        public ushort getInstid()
        {
            return instid;
        }
        public string getCharacter()
        {
            return character;
        }
        public long getDeath(BossData bossData, List<CombatItem> combatList, long start, long end)
        {
            long offset = bossData.getFirstAware();
            CombatItem dead = combatList.FirstOrDefault(x => x.getSrcInstid() == instid && x.isStateChange().getEnum() == "CHANGE_DEAD" && x.getTime() >= start + offset && x.getTime() <= end + offset);
            if (dead != null && dead.getTime() > 0)
            {
                return dead.getTime();
            }
            return 0;
        }
        public string getProf()
        {
            return prof;
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

        public List<DamageLog> getDamageLogs(int instidFilter, BossData bossData, List<CombatItem> combatList, AgentData agentData, long start, long end)//isntid = 0 gets all logs if specefied sets and returns filterd logs
        {
            if (damage_logs.Count == 0)
            {
                setDamageLogs(bossData, combatList, agentData);
            }


            if (damage_logsFiltered.Count == 0)
            {
                setFilteredLogs(bossData, combatList, agentData);
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
        public List<DamageLog> getDamageTakenLogs(BossData bossData, List<CombatItem> combatList, AgentData agentData, MechanicData m_data, long start, long end)
        {
            if (damageTaken_logs.Count == 0)
            {
                setDamagetakenLogs(bossData, combatList, agentData, m_data);
            }
            return damageTaken_logs.Where(x => x.getTime() >= start && x.getTime() <= end).ToList();
        }
        public BoonDistribution getBoonDistribution(BossData bossData, SkillData skillData, List<CombatItem> combatList, List<PhaseData> phases, int phase_index)
        {
            if (boon_distribution.Count == 0)
            {
                setBoonDistribution(bossData, skillData, combatList, phases);
            }
            return boon_distribution[phase_index];
        }
        public Dictionary<int, BoonsGraphModel> getBoonGraphs(BossData bossData, SkillData skillData, List<CombatItem> combatList, List<PhaseData> phases)
        {
            if (boon_distribution.Count == 0)
            {
                setBoonDistribution(bossData, skillData, combatList, phases);
            }
            return boon_points;
        }
        public Dictionary<int, long> getBoonPresence(BossData bossData, SkillData skillData, List<CombatItem> combatList, List<PhaseData> phases, int phase_index)
        {
            if (boon_distribution.Count == 0)
            {
                setBoonDistribution(bossData, skillData, combatList, phases);
            }
            return boon_presence[phase_index];
        }
        public List<CastLog> getCastLogs(BossData bossData, List<CombatItem> combatList, AgentData agentData, long start, long end)
        {
            if (cast_logs.Count == 0)
            {
                setCastLogs(bossData, combatList, agentData);
            }
            return cast_logs.Where(x => x.getTime() >= start && x.getTime() <= end).ToList();

        }

        public List<CastLog> getCastLogsActDur(BossData bossData, List<CombatItem> combatList, AgentData agentData, long start, long end)
        {
            if (cast_logs.Count == 0)
            {
                setCastLogs(bossData, combatList, agentData);
            }
            return cast_logs.Where(x => x.getTime() + x.getActDur() >= start && x.getTime() <= end).ToList();

        }
        public List<DamageLog> getJustPlayerDamageLogs(int instidFilter, BossData bossData, List<CombatItem> combatList, AgentData agentData, long start, long end)
        {
            return getDamageLogs(instidFilter, bossData, combatList, agentData, start, end).Where(x => x.getInstidt() == instid).ToList();
        }
        // privates
        protected void addDamageLog(long time, ushort instid, CombatItem c, List<DamageLog> toFill)
        {
            LuckParser.Models.ParseEnums.StateChange state = c.isStateChange();
            if (instid == c.getDstInstid() && c.getIFF().getEnum() == "FOE")
            {
                if (state.getEnum() == "NORMAL" && c.isBuffremove().getID() == 0)
                {
                    if (c.isBuff() == 1 && c.getBuffDmg() != 0)//condi
                    {
                        toFill.Add(new DamageLogCondition(time, c));
                    }
                    else if (c.isBuff() == 0 && c.getValue() != 0)//power
                    {
                        toFill.Add(new DamageLogPower(time, c));
                    }
                    else if (c.getResult().getID() == 5 || c.getResult().getID() == 6 || c.getResult().getID() == 7)
                    {//Hits that where blinded, invulned, interupts
                        toFill.Add(new DamageLogPower(time, c));
                    }
                }
            }
        }
        // Setters
        protected abstract void setDamageLogs(BossData bossData, List<CombatItem> combatList, AgentData agentData);
        protected void setDamageTakenLog(long time, ushort instid, CombatItem c)
        {
            LuckParser.Models.ParseEnums.StateChange state = c.isStateChange();
            if (instid == c.getSrcInstid())
            {
                if (state.getID() == 0)
                {
                    if (c.isBuff() == 1 && c.getBuffDmg() != 0)
                    {
                        //inco,ing condi dmg not working or just not present?
                        // damagetaken.Add(c.getBuffDmg());
                        damageTaken_logs.Add(new DamageLogCondition(time, c));
                    }
                    else if (c.isBuff() == 0 && c.getValue() != 0)
                    {
                        damageTaken_logs.Add(new DamageLogPower(time, c));

                    }
                    else if (c.isBuff() == 0 && c.getValue() == 0)
                    {
                        damageTaken_logs.Add(new DamageLogPower(time, c));
                    }
                }
            }
        }
        private void setFilteredLogs(BossData bossData, List<CombatItem> combatList, AgentData agentData)
        {
            long time_start = bossData.getFirstAware();
            foreach (CombatItem c in combatList)
            {
                if (instid == c.getSrcInstid() || instid == c.getSrcMasterInstid())//selecting player
                {
                    long time = c.getTime() - time_start;
                    addDamageLog(time, bossData.getInstid(), c, damage_logsFiltered);
                }
            }
        }
        private void setBoonDistribution(BossData bossData, SkillData skillData, List<CombatItem> combatList, List<PhaseData> phases)
        {
            BoonMap to_use = getBoonMap(bossData, skillData, combatList, true);
            List<Boon> boon_to_use = Boon.getAllBuffList();
            boon_to_use.AddRange(Boon.getCondiBoonList());
            long dur = bossData.getLastAware() - bossData.getFirstAware();
            int fight_duration = (int)(dur) / 1000;
            // Init boon presence points
            BoonsGraphModel boon_presence_points = new BoonsGraphModel("Number of Boons");
            for (int i = 0; i <= fight_duration; i++)
            {
                boon_presence_points.getBoonChart().Add(new Point(i, 0));
            }
            for (int i = 0; i < phases.Count; i++)
            {
                boon_distribution.Add( new BoonDistribution());
                boon_presence.Add(new Dictionary<int, long>());
            }
            foreach (Boon boon in boon_to_use)
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
                    long death = getDeath(bossData, combatList, 0, dur);
                    if (death > 0)
                    {
                        simulator.trim(death - bossData.getFirstAware());
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
                            if (!distrib.ContainsKey(boonid)) {
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
                                    toModify.value += simul.getDuration(src,phase.getStart(), phase.getEnd());
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
        private void setCastLogs(BossData bossData, List<CombatItem> combatList, AgentData agentData)
        {
            long time_start = bossData.getFirstAware();
            CastLog curCastLog = null;

            foreach (CombatItem c in combatList)
            {
                LuckParser.Models.ParseEnums.StateChange state = c.isStateChange();
                if (state.getID() == 0)
                {
                    if (instid == c.getSrcInstid())//selecting player as caster
                    {
                        if (c.isActivation().getID() > 0)
                        {
                            if (c.isActivation().getID() < 3)
                            {
                                long time = c.getTime() - time_start;
                                curCastLog = new CastLog(time, c.getSkillID(), c.getValue(), c.isActivation());
                            }
                            else
                            {
                                if (curCastLog != null)
                                {
                                    if (curCastLog.getID() == c.getSkillID())
                                    {
                                        curCastLog = new CastLog(curCastLog.getTime(), curCastLog.getID(), curCastLog.getExpDur(), curCastLog.startActivation(), c.getValue(), c.isActivation());
                                        cast_logs.Add(curCastLog);
                                        curCastLog = null;
                                    }
                                }
                            }
                        }
                    }
                }
                else if (state.getID() == 11)
                {//Weapon swap
                    if (instid == c.getSrcInstid())//selecting player as caster
                    {
                        if ((int)c.getDstAgent() == 4 || (int)c.getDstAgent() == 5)
                        {
                            long time = c.getTime() - time_start;
                            curCastLog = new CastLog(time, -2, (int)c.getDstAgent(), c.isActivation());
                            cast_logs.Add(curCastLog);
                            curCastLog = null;
                        }
                    }
                }
            }
        }
        protected abstract void setDamagetakenLogs(BossData bossData, List<CombatItem> combatList, AgentData agentData, MechanicData m_data);
        // private getters
        private BoonMap getBoonMap(BossData bossData, SkillData skillData, List<CombatItem> combatList, bool add_condi)
        {
            BoonMap boon_map = new BoonMap();
            boon_map.add(Boon.getAllBuffList());
            // This only happens for bosses
            if (add_condi)
            {
                boon_map.add(Boon.getCondiBoonList());
            }
            // Fill in Boon Map
            long time_start = bossData.getFirstAware();
            long fight_duration = bossData.getLastAware() - time_start;

            foreach (CombatItem c in combatList)
            {
                if (c.isBuff() != 1 || !boon_map.ContainsKey(c.getSkillID()))
                {
                    continue;
                }
                long time = c.getTime() - time_start;
                ushort dst = c.isBuffremove().getID() == 0 ? c.getDstInstid() : c.getSrcInstid();
                if (instid == dst && time >= 0 && time <= fight_duration)
                {
                    ushort src = c.getSrcMasterInstid() > 0 ? c.getSrcMasterInstid() : c.getSrcInstid();
                    if (c.isBuffremove().getID() == 0)
                    {
                        boon_map[c.getSkillID()].Add(new BoonLog(time, src, c.getValue(), 0));
                    }
                    else if (Boon.removePermission(c.getSkillID(), c.isBuffremove().getID(), c.getIFF().getID()))
                    {
                        if (c.isBuffremove().getID() == 1)//All
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
                        else if (c.isBuffremove().getID() == 2)//Single
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
                        else if (c.isBuffremove().getID() == 3)//Manuel
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
