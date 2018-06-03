using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace LuckParser.Models.ParseModels
{
    public class Player
    {
        // Fields
        protected ushort instid;
        private String account;
        private String character;
        private String group;
        private String prof;
        private int toughness;
        private int healing;
        private int condition;
        private int dcd = 0;//time in ms the player dcd
        // DPS
        protected List<LogDamage> damage_logs = new List<LogDamage>();
        private List<LogDamage> damage_logsFiltered = new List<LogDamage>();
        private List<int[]> bossdpsGraph = new List<int[]>();
        // Minions
        private List<ushort> combatMinionIDList = new List<ushort>();
        private Dictionary<AgentItem, List<LogDamage>> minion_damage_logs = new Dictionary<AgentItem, List<LogDamage>>();
        private Dictionary<AgentItem, List<LogDamage>> minion_damage_logsFiltered = new Dictionary<AgentItem, List<LogDamage>>();
        // Taken damage
        protected List<LogDamage> damageTaken_logs = new List<LogDamage>();
        protected List<int> damagetaken = new List<int>();
        // Boons
        private BoonDistribution boon_distribution = new BoonDistribution();
        private Dictionary<int, BoonsGraphModel> boon_points = new Dictionary<int, BoonsGraphModel>();
        // Casts
        private List<CastLog> cast_logs = new List<CastLog>();

        // Constructors
        public Player(AgentItem agent)
        {
            this.instid = agent.getInstid();
            String[] name = agent.getName().Split('\0');
            this.character = name[0];
            this.account = name[1];
            this.group = name[2];
            this.prof = agent.getProf();
            this.toughness = agent.getToughness();
            this.healing = agent.getHealing();
            this.condition = agent.getCondition();
        }

        // Getters
        public ushort getInstid()
        {
            return instid;
        }

        public string getAccount()
        {
            return account;
        }

        public string getCharacter()
        {
            return character;
        }

        public string getGroup()
        {
            return group;
        }

        public string getProf()
        {
            return prof;
        }

        public int getToughness()
        {
            return toughness;
        }

        public int getHealing()
        {
            return healing;
        }

        public int getCondition()
        {
            return condition;
        }

        public List<LogDamage> getDamageLogs(int instidFilter,BossData bossData, List<CombatItem> combatList, AgentData agentData)//isntid = 0 gets all logs if specefied sets and returns filterd logs
        {
            if (damage_logs.Count == 0)
            {
                setDamageLogs(bossData, combatList,agentData);
            }
           
           
            if(damage_logsFiltered.Count == 0) {
                setFilteredLogs(bossData, combatList, agentData);
            }
            if (instidFilter == 0)
            {
                return damage_logs;
            }else {
                return damage_logsFiltered;
            }
        }
        public List<int> getDamagetaken( BossData bossData, List<CombatItem> combatList, AgentData agentData,MechanicData m_data)
        {
            if (damagetaken.Count == 0)
            {
                setDamagetaken(bossData, combatList, agentData,m_data);
            }
            return damagetaken;
        }
        public List<LogDamage> getDamageTakenLogs(BossData bossData, List<CombatItem> combatList, AgentData agentData,MechanicData m_data)
        {
            if (damagetaken.Count == 0)
            {
                setDamagetaken(bossData, combatList, agentData,m_data);
            }
            return damageTaken_logs;
        }
        public BoonDistribution getBoonDistribution(BossData bossData, SkillData skillData, List<CombatItem> combatList)
        {
            if (boon_distribution.Count == 0)
            {
                setBoonDistribution(bossData, skillData, combatList);
            }
            return boon_distribution;
        }
        public Dictionary<int, BoonsGraphModel> getBoonGraphs(BossData bossData, SkillData skillData, List<CombatItem> combatList)
        {
            if (boon_points.Count == 0)
            {
                setBoonDistribution(bossData, skillData, combatList);
            }
            return boon_points;
        }
        public int[] getCleanses(BossData bossData, List<CombatItem> combatList, AgentData agentData) {
            long time_start = bossData.getFirstAware();
            int[] cleanse = { 0, 0 };
            foreach (CombatItem c in combatList.Where(x=>x.isStateChange().getID() == 0))
            {
                if (c.isActivation().getID() == 0)
                {
                    if (instid == c.getSrcInstid() && c.getIFF().getEnum() == "FRIEND" && c.isBuffremove().getID() == 1/*|| instid == c.getSrcMasterInstid()*/)//selecting player as remover could be wrong
                    {
                        long time = c.getTime() - time_start;
                        if (time > 0)
                        {
                            if (Boon.getCondiBoonList().Exists(x=>x.getID() == c.getSkillID()))
                            {
                                cleanse[0]++;
                                cleanse[1] += c.getBuffDmg();
                            }

                        }


                    }
                }
            }
            return cleanse;
        }
        public int[] getReses(BossData bossData, List<CombatItem> combatList, AgentData agentData)
        {
            long time_start = bossData.getFirstAware();
            int[] reses = { 0, 0 };
            foreach (CastLog log in cast_logs) {
                if (log.getID() == 1066)
                {
                    reses[0]++;
                    reses[1] += log.getActDur();
                }
            }
            //foreach (CombatItem c in combatList)
            //{
            //    if (instid == c.getDstInstid()/* || instid == c.getSrcMasterInstid()*/)//selecting player most likyl wrong
            //    {
            //        LuckParser.Models.ParseEnums.StateChange state = c.isStateChange();
            //        int time = c.getTime() - time_start;
            //           if (state.getID() == 0 && time > 0)
            //                {
            //                    if (c.getSkillID() == 1066)
            //                    {
            //                        reses[0]++;
            //                        reses[1] += c.getValue();
            //                    }
            //                }
            //    }
            //}
            return reses;
        }
        public List<CastLog> getCastLogs(BossData bossData, List<CombatItem> combatList, AgentData agentData)
        {
            if (cast_logs.Count == 0)
            {
                setCastLogs(bossData, combatList, agentData);
            }
            return cast_logs;

        }
        public Dictionary<AgentItem, List<LogDamage>> getMinionsDamageLogs(int instidFilter, BossData bossData, List<CombatItem> combatList, AgentData agentData)
        {
            if (minion_damage_logs.Count == 0)
            {
                // make sure the keys matches
                foreach (AgentItem agent in minion_damage_logsFiltered.Keys)
                {
                    minion_damage_logs[agent] = new List<LogDamage>();
                }
                setMinionsDamageLogs(0, bossData, combatList, agentData, minion_damage_logs);
            }

            if (minion_damage_logsFiltered.Count == 0)
            {
                // make sure the keys matches
                foreach (AgentItem agent in minion_damage_logs.Keys)
                {
                    minion_damage_logsFiltered[agent] = new List<LogDamage>();
                }
                setMinionsDamageLogs(bossData.getInstid(), bossData, combatList, agentData, minion_damage_logsFiltered);
            }
            if (instidFilter == 0)
            {
                return minion_damage_logs;
            }
            else
            {
                return minion_damage_logsFiltered;
            }
        }
        public List<LogDamage> getJustPlayerDamageLogs(int instidFilter, BossData bossData, List<CombatItem> combatList, AgentData agentData)
        {
            List<ushort> minionList = getCombatMinionList(bossData, combatList, agentData);
            return getDamageLogs(instidFilter, bossData, combatList, agentData).Where(x => !minionList.Contains(x.getInstidt())).ToList();
        }
        public List<int[]> getBossDPSGraph()
        {
            return new List<int[]>(bossdpsGraph);
        }
        // Public methods
        public void setBossDPSGraph(List<int[]> list)
        {
            bossdpsGraph = new List<int[]>(list);
        }
        public int GetDC()
        {
            return dcd;
        }
        public void SetDC(int value)
        {
            dcd = value;
        }
        public long getDeath(List<CombatItem> combatList)
        {
            CombatItem dead = combatList.FirstOrDefault( x => x.getSrcInstid() == instid && x.isStateChange().getEnum() == "CHANGE_DEAD");
            if (dead != null && dead.getTime() > 0)
            {
                return dead.getTime();
            }
            return 0;
        }
        // Privates
        protected void addDamageLog(long time, ushort instid, CombatItem c, List<LogDamage> toFill)
        {
            LuckParser.Models.ParseEnums.StateChange state = c.isStateChange();
            if (instid == c.getDstInstid() && c.getIFF().getEnum() == "FOE")
            {
                if (state.getEnum() == "NORMAL" && c.isBuffremove().getID() == 0)
                {
                    if (c.isBuff() == 1 && c.getBuffDmg() != 0)//condi
                    {
                        toFill.Add(new LogDamageCondition(time, c));
                    }
                    else if (c.isBuff() == 0 && c.getValue() != 0)//power
                    {
                        toFill.Add(new LogDamagePower(time, c));
                    }
                    else if (c.getResult().getID() == 5 || c.getResult().getID() == 6 || c.getResult().getID() == 7)
                    {//Hits that where blinded, invulned, interupts
                        toFill.Add(new LogDamagePower(time, c));
                    }
                }
            }
        }
        protected virtual void setDamageLogs(BossData bossData, List<CombatItem> combatList, AgentData agentData)
        {
            long time_start = bossData.getFirstAware();
            foreach (CombatItem c in combatList)
            {

                if (instid == c.getSrcInstid() || instid == c.getSrcMasterInstid())//selecting player or minion as caster
                {
                    long time = c.getTime() - time_start;
                    foreach (AgentItem item in agentData.getNPCAgentList())
                    {//selecting all
                        addDamageLog(time, item.getInstid(), c, damage_logs);
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
        protected void setDamageTakenLog(long time, ushort instid, CombatItem c)
        {
            LuckParser.Models.ParseEnums.StateChange state = c.isStateChange();
            if (instid == c.getSrcInstid() && c.getIFF().getEnum() == "FOE")
            {
                if (state.getID() == 0)
                {
                    if (c.isBuff() == 1 && c.getBuffDmg() != 0)
                    {
                        //inco,ing condi dmg not working or just not present?
                        // damagetaken.Add(c.getBuffDmg());
                    }
                    else if (c.isBuff() == 0 && c.getValue() != 0)
                    {
                        damagetaken.Add(c.getValue());
                        damageTaken_logs.Add(new LogDamagePower(time, c));

                    }
                    else if (c.isBuff() == 0 && c.getValue() == 0)
                    {
                        damageTaken_logs.Add(new LogDamagePower(time, c));
                    }
                }
            }
        }
        protected virtual void setDamagetaken(BossData bossData, List<CombatItem> combatList, AgentData agentData,MechanicData m_data) {
            long time_start = bossData.getFirstAware();               
            foreach (CombatItem c in combatList) {
                if (instid == c.getDstInstid()) {//selecting player as target
                    long time = c.getTime() - time_start;
                    foreach (AgentItem item in agentData.getNPCAgentList())
                    {//selecting all
                        setDamageTakenLog(time, item.getInstid(), c);
                    }
                }
            }
        }
        private void setBoonDistribution(BossData bossData, SkillData skillData, List<CombatItem> combatList)
        {
            BoonMap to_use = getBoonMap(bossData,skillData,combatList, true);
            List<Boon> boon_to_use = Boon.getAllBuffList();
            boon_to_use.AddRange(Boon.getCondiBoonList());
            long dur = bossData.getLastAware() - bossData.getFirstAware();
            int fight_duration = (int)(dur) / 1000;
            foreach (Boon boon in boon_to_use)
            {
                if (to_use.ContainsKey(boon.getID()))
                {
                    List<LogBoon> logs = to_use[boon.getID()];
                    if (logs.Count == 0)
                    {
                        continue;
                    }
                    if (boon_distribution.ContainsKey(boon.getID())) {
                        continue;
                    }
                    boon_distribution[boon.getID()] = new Dictionary<ushort, OverAndValue>();
                    BoonSimulator simulator = boon.getSimulator();
                    simulator.simulate(logs, dur);
                    if (getDeath(combatList) > 0)
                    {
                        simulator.trim(getDeath(combatList) - bossData.getFirstAware());
                    } else
                    {
                        simulator.trim(dur);
                    }
                    List<BoonSimulationItem> simulation = simulator.getSimulationResult();
                    foreach (BoonSimulationItem simul in simulation)
                    {
                        foreach (ushort src in simul.getSrc())
                        {
                            if (!boon_distribution[boon.getID()].ContainsKey(src))
                            {
                                boon_distribution[boon.getID()][src] = new OverAndValue(simul.getDuration(src), simul.getOverstack(src));
                            }
                            else
                            {
                                OverAndValue toModify = boon_distribution[boon.getID()][src];
                                toModify.value += simul.getDuration(src);
                                toModify.overstack += simul.getOverstack(src);
                                boon_distribution[boon.getID()][src] = toModify;
                            }
                        }
                    }
                    int prev = 0;
                    // full precision
                    List<Point> toFill = new List<Point>();
                    foreach (BoonSimulationItem simul in simulation)
                    {
                        int start = (int)simul.getStart();
                        int end = (int)simul.getEnd();
                        // fill
                        if (toFill.Count < start)
                        {
                            for (int i = prev; i < start; i++)
                            {
                                toFill.Add(new Point(i, 0));
                            }
                        }
                        for (int i = start; i < end; i++)
                        {
                            toFill.Add(new Point(i, simul.getStack(i)));
                        }
                        prev = end;
                    }
                    // fill
                    for (int i = prev; i < dur; i++)
                    {
                        toFill.Add(new Point(i, 0));
                    }
                    // reduce precision to seconds
                    List<Point> reducedPrecision = new List<Point>();
                    for (int i = 0; i < fight_duration; i++)
                    {
                        reducedPrecision.Add(new Point(i, toFill[1000 * i].Y));
                    }
                    boon_points[boon.getID()] = new BoonsGraphModel(boon.getName(), reducedPrecision);
                }
            }
        }
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
                if (c.getValue() == 0 || c.isBuff() != 1 || c.getBuffDmg() > 0)
                {
                    continue;
                }
                if (!boon_map.ContainsKey(c.getSkillID()))
                {
                    continue;
                }
                long time = c.getTime() - time_start;
                if (instid == c.getDstInstid() && time > 0 && time < fight_duration)
                {
                    ushort src = c.getSrcMasterInstid() > 0 ? c.getSrcMasterInstid() : c.getSrcInstid();
                    if (c.isBuffremove().getID() == 0)
                    {
                        boon_map[c.getSkillID()].Add(new LogBoon(time, src, c.getValue(), c.getOverstackValue()));
                    }
                    else if (c.isBuffremove().getID() == 1)//All
                    {
                        List<LogBoon> loglist = boon_map[c.getSkillID()];
                        for (int cnt = loglist.Count() - 1; cnt >= 0; cnt--)
                        {
                            LogBoon curBL = loglist[cnt];
                            if (curBL.getTime() + curBL.getValue() > time)
                            {
                                long subtract = (curBL.getTime() + curBL.getValue()) - time;
                                loglist[cnt].addValue(-subtract);
                            }
                        }

                    }
                    else if (c.isBuffremove().getID() == 2)//Single
                    {
                        List<LogBoon> loglist = boon_map[c.getSkillID()];
                        int cnt = loglist.Count() - 1;
                        LogBoon curBL = loglist[cnt];
                        if (curBL.getTime() + curBL.getValue() > time)
                        {
                            long subtract = (curBL.getTime() + curBL.getValue()) - time;
                            loglist[cnt].addValue(-subtract);
                        }
                    }
                    else if (c.isBuffremove().getID() == 3)//Manuel
                    {
                        List<LogBoon> loglist = boon_map[c.getSkillID()];
                        for (int cnt = loglist.Count() - 1; cnt >= 0; cnt--)
                        {
                            LogBoon curBL = loglist[cnt];
                            long ctime = curBL.getTime() + curBL.getValue();
                            if (ctime > time)
                            {
                                long subtract = (curBL.getTime() + curBL.getValue()) - time;
                                loglist[cnt].addValue(-subtract);
                                break;
                            }
                        }

                    }
                }
            }
            return boon_map;
        }
        private void setCastLogs(BossData bossData, List<CombatItem> combatList, AgentData agentData) {
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
                            else {
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
                } else if (state.getID() == 11) {//Weapon swap
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
        private List<ushort> getCombatMinionList(BossData bossData, List<CombatItem> combatList, AgentData agentData)
        {
            if (combatMinionIDList.Count == 0)
            {
                combatMinionIDList = combatList.Where(x => x.getSrcMasterInstid() == instid && ((x.getValue() != 0 && x.isBuff() == 0) || (x.isBuff() == 1 && x.getBuffDmg() != 0))).Select(x => x.getSrcInstid()).Distinct().ToList();
            }
            return combatMinionIDList;
        }
        private void setMinionsDamageLogs(int instidFilter, BossData bossData, List<CombatItem> combatList, AgentData agentData, Dictionary<AgentItem, List<LogDamage>> toFill)
        {
            List<ushort> minionList = getCombatMinionList(bossData, combatList, agentData);
            foreach (int petid in minionList)
            {
                AgentItem agent = agentData.getNPCAgentList().FirstOrDefault(x => x.getInstid() == petid);
                if (agent != null)
                {
                    List<LogDamage> damageLogs = getDamageLogs(instidFilter, bossData, combatList, agentData).Where(x => x.getSrcAgent() == agent.getAgent()).ToList();
                    if (damageLogs.Count == 0)
                    {
                        continue;
                    }
                    AgentItem key = toFill.Keys.ToList().FirstOrDefault(x => x.getName() == agent.getName());
                    if (key == null)
                    {
                        toFill[agent] = damageLogs;
                    }
                    else
                    {
                        toFill[key].AddRange(damageLogs);
                    }
                }
            }
        }
        
    }
}