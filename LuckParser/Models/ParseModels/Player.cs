using System;
using System.Collections.Generic;
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
        protected List<DamageLog> damage_logs = new List<DamageLog>();
        private List<DamageLog> damage_logsFiltered = new List<DamageLog>();
        private List<int[]> bossdpsGraph = new List<int[]>();
        // Minions
        private List<ushort> combatMinionIDList = new List<ushort>();
        private Dictionary<AgentItem, List<DamageLog>> minion_damage_logs = new Dictionary<AgentItem, List<DamageLog>>();
        private Dictionary<AgentItem, List<DamageLog>> minion_damage_logsFiltered = new Dictionary<AgentItem, List<DamageLog>>();
        // Taken damage
        protected List<DamageLog> damageTaken_logs = new List<DamageLog>();
        protected List<int> damagetaken = new List<int>();
        // Boons
        private Dictionary<int, BoonMap> boon_map = new Dictionary<int, BoonMap>();
        private List<int[]> consumeList = new List<int[]>();
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
        public int getInstid()
        {
            return instid;
        }

        public String getAccount()
        {
            return account;
        }

        public String getCharacter()
        {
            return character;
        }

        public String getGroup()
        {
            return group;
        }

        public String getProf()
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

        public List<DamageLog> getDamageLogs(int instidFilter,BossData bossData, List<CombatItem> combatList, AgentData agentData)//isntid = 0 gets all logs if specefied sets and returns filterd logs
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
        public List<DamageLog> getDamageTakenLogs(BossData bossData, List<CombatItem> combatList, AgentData agentData,MechanicData m_data)
        {
            if (damagetaken.Count == 0)
            {
                setDamagetaken(bossData, combatList, agentData,m_data);
            }
            return damageTaken_logs;
        }
        public List<BoonMap> getBoonMap(BossData bossData, SkillData skillData, List<CombatItem> combatList)
        {
            if (boon_map.Count == 0)
            {
                setBoonMap(bossData, skillData, combatList, false);
            }
            return boon_map.Values.ToList();
        }
        public List<BoonMap> getCondiBoonMap(BossData bossData, SkillData skillData, List<CombatItem> combatList)
        {
            if (boon_map.Count == 0)
            {
                setBoonMap(bossData, skillData, combatList, true);
            }
            return boon_map.Values.ToList();
        }
        public List<BoonMap> getBoonGen(BossData bossData, SkillData skillData, List<CombatItem> combatList, AgentData agentData,List<int> trgtPID)
        {
            Dictionary<int, BoonMap> boonGen = new Dictionary<int, BoonMap>();
            long time_start = bossData.getFirstAware();
            long fight_duration = bossData.getLastAware() - time_start;
            // Initialize Boon Map with every Boon
            foreach (Boon boon in Boon.getAllBuffList())
            {
                BoonMap map = new BoonMap(boon.getName(), boon.getID(), new List<BoonLog>());
                boonGen[boon.getID()] = map;
                // boon_map.put(boon.getName(), new ArrayList<BoonLog>());
            }
            foreach (CombatItem c in combatList)
            {
                if (c.getValue() == 0 || c.isBuff() != 1 || c.getBuffDmg() > 0)
                {
                    continue;
                }
                if (!boonGen.ContainsKey(c.getSkillID()))
                {
                    continue;
                }
                string state = c.isStateChange().getEnum();
                int srcID = c.getSrcInstid();
                int dstID = c.getDstInstid();
                long time = c.getTime() - time_start;
                if ((instid == srcID || instid == c.getSrcMasterInstid()) && state == "NORMAL" && time > 0 && time < fight_duration)//selecting player or minion as caster
                {
                    foreach (int id in trgtPID)
                    {//Make sure trgt is within paramaters
                        if (id == dstID)
                        {
                            if (c.isBuffremove().getID() == 0)
                            {//Buff application
                                boonGen[c.getSkillID()].getBoonLog().Add(new BoonLog(time, c.getValue(), c.getOverstackValue()));
                            }
                        }
                    }

                }

            }
            return boonGen.Values.ToList();
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
        public Dictionary<AgentItem, List<DamageLog>> getMinionsDamageLogs(int instidFilter, BossData bossData, List<CombatItem> combatList, AgentData agentData)
        {
            if (minion_damage_logs.Count == 0)
            {
                // make sure the keys matches
                foreach (AgentItem agent in minion_damage_logsFiltered.Keys)
                {
                    minion_damage_logs[agent] = new List<DamageLog>();
                }
                setMinionsDamageLogs(0, bossData, combatList, agentData, minion_damage_logs);
            }

            if (minion_damage_logsFiltered.Count == 0)
            {
                // make sure the keys matches
                foreach (AgentItem agent in minion_damage_logs.Keys)
                {
                    minion_damage_logsFiltered[agent] = new List<DamageLog>();
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
        public List<DamageLog> getJustPlayerDamageLogs(int instidFilter, BossData bossData, List<CombatItem> combatList, AgentData agentData)
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
        public List<int[]> getConsumablesList(BossData bossData, SkillData skillData, List<CombatItem> combatList)
        {
            if (consumeList.Count() == 0)
            {
                setConsumablesList( bossData, skillData, combatList);
            }
            return consumeList;
        }
        // Private Methods

        protected void addDamageLog(long time, ushort instid, CombatItem c, List<DamageLog> toFill)
        {
            LuckParser.Models.ParseEnums.StateChange state = c.isStateChange();
            if (instid == c.getDstInstid() && c.getIFF().getEnum() == "FOE")
            {
                if (state.getEnum() == "NORMAL" && c.isBuffremove().getID() == 0)
                {
                    if (c.isBuff() == 1 && c.getBuffDmg() != 0)//condi
                    {
                        toFill.Add(new CondiDamageLog(time, c));
                    }
                    else if (c.isBuff() == 0 && c.getValue() != 0)//power
                    {
                        toFill.Add(new PowerDamageLog(time, c));
                    }
                    else if (c.getResult().getID() == 5 || c.getResult().getID() == 6 || c.getResult().getID() == 7)
                    {//Hits that where blinded, invulned, interupts
                        toFill.Add(new PowerDamageLog(time, c));
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
                        damageTaken_logs.Add(new PowerDamageLog(time, c));

                    }
                    else if (c.isBuff() == 0 && c.getValue() == 0)
                    {
                        damageTaken_logs.Add(new PowerDamageLog(time, c));
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
        private void setConsumablesList(BossData bossData, SkillData skillData, List<CombatItem> combatList)
        {
            List<Boon> foodBoon = Boon.getFoodList();
            List<Boon> utilityBoon = Boon.getUtilityList();
            long time_start = bossData.getFirstAware();
            long fight_duration = bossData.getLastAware() - time_start;
            foreach (CombatItem c in combatList)
            {
                if ( c.isBuff() != 18 && c.isBuff() != 1)
                {
                    continue;
                }
                
                if (foodBoon.FirstOrDefault(x => x.getID() == c.getSkillID()) == null  && utilityBoon.FirstOrDefault(x => x.getID() == c.getSkillID()) == null)
                {
                    continue;
                }
                long time = c.getTime() - time_start;
                if (instid == c.getDstInstid())
                {
                   // if (c.isBuffremove().getID() == 0)
                    //{
                        consumeList.Add(new int[] { c.getSkillID(), (int)time });
                   // }
                   
                   
                }
            }
        }
        private void setBoonMap(BossData bossData, SkillData skillData, List<CombatItem> combatList, bool add_condi)
        {
            // Initialize Boon Map with every Boon
            foreach (Boon boon in Boon.getAllBuffList())
            {
                BoonMap map = new BoonMap(boon.getName(), boon.getID(), new List<BoonLog>());
                boon_map[boon.getID()] = map;
            }
            // This only happens for bosses
            if (add_condi)
            {
                foreach (Boon boon in Boon.getCondiBoonList())
                {
                    BoonMap map = new BoonMap(boon.getName(), boon.getID(), new List<BoonLog>());
                    boon_map[boon.getID()] = map;
                }
            }
            // Fill in Boon Map
            long time_start = bossData.getFirstAware();
            long fight_duration = bossData.getLastAware() - time_start;
            
            foreach (CombatItem c in combatList)
            {
                if (c.getValue() == 0 || c.isBuff() != 1  || c.getBuffDmg() > 0)
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
                    if (c.isBuffremove().getID() == 0)
                    {
                        boon_map[c.getSkillID()].getBoonLog().Add(new BoonLog(time, c.getValue()));
                    }
                    else if (c.isBuffremove().getID() == 1)//All
                    {
                        List<BoonLog> loglist = boon_map[c.getSkillID()].getBoonLog();
                        for (int cnt = loglist.Count() - 1; cnt >= 0; cnt--)
                        {
                            BoonLog curBL = loglist[cnt];
                            if (curBL.getTime() + curBL.getValue() > time)
                            {
                                long subtract = (curBL.getTime() + curBL.getValue()) - time;
                                loglist[cnt] = new BoonLog(curBL.getTime(), curBL.getValue() - subtract);
                            }
                        }

                    }
                    else if (c.isBuffremove().getID() == 2)//Single
                    {
                        List<BoonLog> loglist = boon_map[c.getSkillID()].getBoonLog();
                        int cnt = loglist.Count() - 1;
                        BoonLog curBL = loglist[cnt];
                        if (curBL.getTime() + curBL.getValue() > time)
                        {
                            long subtract = (curBL.getTime() + curBL.getValue()) - time;
                            loglist[cnt] = new BoonLog(curBL.getTime(), curBL.getValue() - subtract);
                        }
                    }
                    else if (c.isBuffremove().getID() == 3)//Manuel
                    {
                        List<BoonLog> loglist = boon_map[c.getSkillID()].getBoonLog();
                        for (int cnt = loglist.Count() - 1; cnt >= 0; cnt--)
                        {
                            BoonLog curBL = loglist[cnt];
                            long ctime = curBL.getTime() + curBL.getValue();
                            if (ctime > time)
                            {
                                long subtract = (curBL.getTime() + curBL.getValue()) - time;
                                loglist[cnt] = new BoonLog(curBL.getTime(), curBL.getValue() - subtract);
                                break;
                            }
                        }

                    }
                }
            }

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
        private List<DamageLog> getMinionDamageLogs(int instidFilter, long srcagent, BossData bossData, List<CombatItem> combatList, AgentData agentData)
        {
            List<DamageLog> dls = getDamageLogs(instidFilter, bossData, combatList, agentData).Where(x => x.getSrcAgent() == srcagent).ToList();
            return dls;
        }
        private void setMinionsDamageLogs(int instidFilter, BossData bossData, List<CombatItem> combatList, AgentData agentData, Dictionary<AgentItem, List<DamageLog>> toFill)
        {
            List<ushort> minionList = getCombatMinionList(bossData, combatList, agentData);
            foreach (int petid in minionList)
            {
                AgentItem agent = agentData.getNPCAgentList().FirstOrDefault(x => x.getInstid() == petid);
                if (agent != null)
                {
                    List<DamageLog> damageLogs = getMinionDamageLogs(instidFilter, agent.getAgent(), bossData, combatList, agentData);
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