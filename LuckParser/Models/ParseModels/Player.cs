using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckParser.Models.ParseModels
{
    public class Player
    {
        // Fields
        private int instid;
        private String account;
        private String character;
        private String group;
        private String prof;
        private int toughness;
        private int healing;
        private int condition;
        private List<DamageLog> damage_logs = new List<DamageLog>();
        private List<DamageLog> damage_logsFiltered = new List<DamageLog>();
        private List<DamageLog> damageTaken_logs = new List<DamageLog>();
        private List<int> damagetaken = new List<int>();
        private Dictionary<int, BoonMap> boon_map = new Dictionary<int, BoonMap>();
        private List<CastLog> cast_logs = new List<CastLog>();
        private List<int> combatMinionIDList;
        private List<int[]> bossdpsGraph = new List<int[]>();

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
                setFilteredLogs(bossData, combatList, agentData,instidFilter);
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
        public List<BoonMap> getboonGen(BossData bossData, SkillData skillData, List<CombatItem> combatList, AgentData agentData,List<int> trgtPID)
        {
            Dictionary<int, BoonMap> boonGen = new Dictionary<int, BoonMap>();
            int time_start = bossData.getFirstAware();
            int fight_duration = bossData.getLastAware() - time_start;
            int here = 0, there= 0 , everywhere = 0, huh = 0;
            // Initialize Boon Map with every Boon
            foreach (Boon boon in Boon.getAllProfList())
            {
                BoonMap map = new BoonMap(boon.getName(), boon.getID(),new List<BoonLog>());
                boonGen[boon.getID()] = map;
                // boon_map.put(boon.getName(), new ArrayList<BoonLog>());
            }
            foreach (CombatItem c in combatList)
            {
                if (c.getValue() == 0)
                {
                    continue;
                }
                string state = c.isStateChange().getEnum();
                int srcID = c.getSrcInstid();
                int dstID = c.getDstInstid();
                int time = c.getTime() - time_start;
                if ((instid == srcID || instid == c.getSrcMasterInstid()) && state == "NORMAL" && time > 0 && time < fight_duration)//selecting player or minion as caster
                {                  
                    here++;
                    foreach (AgentItem item in agentData.getPlayerAgentList())
                    {//selecting all
                        if (item.getInstid() == dstID /*&& c.getIFF().getEnum() == "FRIEND"*/)//Make sure target is friendly existing Agent
                        {
                            there++;
                            foreach (int id in trgtPID) {//Make sure trgt is within paramaters
                                if (id == dstID)
                                {
                                    everywhere++;
                                    if (c.isBuff() == 1 && c.isBuffremove().getID() == 0)
                                    {//Buff application
                                        huh++;
                                        if (boonGen.ContainsKey(c.getSkillID())) 
                                        {
                                            boonGen[c.getSkillID()].getBoonLog().Add(new BoonLog(time, c.getValue(), c.getOverstackValue()));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }             
            }
            return boonGen.Values.ToList();
        }
        public int[] getCleanses(BossData bossData, List<CombatItem> combatList, AgentData agentData) {
            int time_start = bossData.getFirstAware();
            int[] cleanse = { 0, 0 };
            foreach (CombatItem c in combatList.Where(x=>x.isStateChange().getID() == 0))
            {
                if (c.isActivation().getID() == 0)
                {
                    if (instid == c.getSrcInstid() && c.getIFF().getEnum() == "FRIEND" && c.isBuffremove().getID() == 1/*|| instid == c.getSrcMasterInstid()*/)//selecting player as remover could be wrong
                    {
                        int time = c.getTime() - time_start;
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
            int time_start = bossData.getFirstAware();
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
        public List<int> getCombatMinionList(BossData bossData, List<CombatItem> combatList, AgentData agentData) {
            if (combatMinionIDList == null) {
                combatMinionIDList = combatList.Where(x => x.getSrcMasterInstid() == instid &&(( x.getValue() != 0 &&x.isBuff() ==0)||(x.isBuff() == 1 && x.getBuffDmg() != 0))).Select(x => (int)x.getSrcInstid()).Distinct().ToList();
                //int test = 0;
            }
            return combatMinionIDList;
        }
        public List<DamageLog> getMinionDamageLogs(int srcagent,BossData bossData, List<CombatItem> combatList, AgentData agentData) {
            List<DamageLog> dls = getDamageLogs(0,bossData, combatList,agentData).Where(x => x.getSrcAgent() == srcagent).ToList();
            return dls;
        }
        public List<DamageLog> getJustPlayerDamageLogs( BossData bossData, List<CombatItem> combatList, AgentData agentData)
        {
            List<int> minionList = getCombatMinionList(bossData, combatList, agentData);
            List<DamageLog> dls = new List<DamageLog>();
            foreach (DamageLog dl in getDamageLogs(0, bossData, combatList, agentData)) {
                if (combatMinionIDList.Contains(dl.getInstidt()))
                {
                    continue;
                }
                else {
                    dls.Add(dl);
                }
            }
            return dls;
        }
        public List<int[]> getBossDPSGraph() {
            return new List<int[]>(bossdpsGraph);
        }

        // Private Methods
        private void setDamageLogs(BossData bossData, List<CombatItem> combatList,AgentData agentData)
        {
            int time_start = bossData.getFirstAware();
            bool combatStart = false;
            bool combatEnd = false;
            foreach (CombatItem c in combatList)
            {
                if (combatStart == false)
                {
                    if (bossData.getInstid() == c.getSrcInstid() &&  c.isStateChange().getID() == 1)
                    {//Make sure combat has started

                        combatStart = true;
                    }
                }
                if (combatEnd == false && combatStart == true)
                {
                    if (bossData.getInstid() == c.getSrcInstid() && c.isStateChange().getID() == 2)
                    {//Make sure combat had ended
                        combatEnd = true;
                    }
                }
               
                    if (instid == c.getSrcInstid() || instid == c.getSrcMasterInstid())//selecting player or minion as caster
                    {
                        LuckParser.Models.ParseEnums.StateChange state = c.isStateChange();
                        int time = c.getTime() - time_start;
                        foreach (AgentItem item in agentData.getNPCAgentList())
                        {//selecting all
                            if (item.getInstid() == c.getDstInstid() )
                            {
                                if (c.getIFF().getEnum() == "FOE")
                                {
                                    if (state.getID() == 0 && c.isBuffremove().getID() == 0)
                                    {
                                    
                                        if (c.isBuff() == 1 && c.getBuffDmg() != 0)//condi
                                        {

                                            damage_logs.Add(new DamageLog(time,(int)c.getSrcAgent(),c.getSrcInstid(), c.getBuffDmg(), c.getSkillID(), c.isBuff(),
                                                    c.getResult(), c.isNinety(), c.isMoving(), c.isFlanking(), c.isActivation()));
                                        }
                                        else if (c.isBuff() == 0 && c.getValue() != 0)//power
                                        {
                                            damage_logs.Add(new DamageLog(time, (int)c.getSrcAgent(), c.getSrcInstid(), c.getValue(), c.getSkillID(), c.isBuff(),
                                                    c.getResult(), c.isNinety(), c.isMoving(), c.isFlanking(), c.isActivation()));
                                        }
                                        else if (c.getResult().getID() == 5 || c.getResult().getID() == 6 || c.getResult().getID() == 7)
                                        {//Hits that where blinded, invulned, interupts

                                            damage_logs.Add(new DamageLog(time, (int)c.getSrcAgent(), c.getSrcInstid(), c.getValue(), c.getSkillID(), c.isBuff(),
                                                    c.getResult(), c.isNinety(), c.isMoving(), c.isFlanking(), c.isActivation()));
                                        }
                                    }
                                }
                            }
                        }
                    }
                
            }
        }
        private void setFilteredLogs(BossData bossData, List<CombatItem> combatList, AgentData agentData,int instidFilter)
        {
            List<DamageLog> filterDLog = new List<DamageLog>();
            int time_start = bossData.getFirstAware();
            foreach (CombatItem c in combatList)
            {
                if (instid == c.getSrcInstid() || instid == c.getSrcMasterInstid())//selecting player
                {
                    LuckParser.Models.ParseEnums.StateChange state = c.isStateChange();
                    int time = c.getTime() - time_start;
                    if (bossData.getInstid() == c.getDstInstid() && c.getIFF().getEnum() == "FOE")//selecting boss
                    {
                        
                        if (state.getEnum() == "NORMAL" && c.isBuffremove().getID() == 0)
                        {
                           
                            if (c.isBuff() == 1 && c.getBuffDmg() != 0)
                            {
                               
                                filterDLog.Add(new DamageLog(time, c.getBuffDmg(), c.getSkillID(), c.isBuff(),
                                        c.getResult(), c.isNinety(), c.isMoving(), c.isFlanking(), c.isActivation()));
                            }
                            else if (c.isBuff() == 0 && c.getValue() != 0)
                            {
                                /*if (time > 300000)
                                {
                                    int fuck = 0;
                                }*/
                                filterDLog.Add(new DamageLog(time, c.getValue(), c.getSkillID(), c.isBuff(),
                                        c.getResult(), c.isNinety(), c.isMoving(), c.isFlanking(), c.isActivation()));
                            }
                        }
                    }
                }
            }
            damage_logsFiltered = filterDLog;
        }
        private void setDamagetaken(BossData bossData, List<CombatItem> combatList, AgentData agentData,MechanicData m_data) {
            int time_start = bossData.getFirstAware();
            
           
            foreach (CombatItem c in combatList) {
                if (instid == c.getDstInstid()) {//selecting player as target
                    LuckParser.Models.ParseEnums.StateChange state = c.isStateChange();
                    int time = c.getTime() - time_start;
                    foreach (AgentItem item in agentData.getNPCAgentList())
                    {//selecting all
                        if (item.getInstid() == c.getSrcInstid() && c.getIFF().getEnum() == "FOE")
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
                                    damageTaken_logs.Add(new DamageLog(time,(int)c.getSrcAgent(),c.getSrcInstid(), c.getValue(), c.getSkillID(), c.isBuff(),
                                           c.getResult(), c.isNinety(), c.isMoving(), c.isFlanking(), c.isActivation(),c.isShields()));
                                  
                                }
                                else if (c.isBuff() == 0  && c.getValue() == 0)
                                {
                                  
                                    damageTaken_logs.Add(new DamageLog(time,(int)c.getSrcAgent(),c.getSrcInstid(), c.getBuffDmg(), c.getSkillID(), c.isBuff(),
                                           c.getResult(), c.isNinety(), c.isMoving(), c.isFlanking(), c.isActivation(),c.isShields()));
                                }
                            }
                        }
                    }
                }
            }
        }
        private void setBoonMap(BossData bossData, SkillData skillData, List<CombatItem> combatList, bool add_condi)
        {
            // Initialize Boon Map with every Boon
            foreach (Boon boon in Boon.getAllProfList())
            {
                BoonMap map = new BoonMap(boon.getName(), boon.getID(), new List<BoonLog>());
                boon_map[boon.getID()] = map;
            }
            if (add_condi)
            {
                foreach (Boon boon in Boon.getCondiBoonList())
                {
                    BoonMap map = new BoonMap(boon.getName(), boon.getID(), new List<BoonLog>());
                    boon_map[boon.getID()] = map;
                }
            }
            // Fill in Boon Map
            int time_start = bossData.getFirstAware();
            int fight_duration = bossData.getLastAware() - time_start;
            foreach (CombatItem c in combatList)
            {
                if (c.getValue() == 0)
                {
                    continue;
                }
                int time = c.getTime() - time_start;
                if (instid == c.getDstInstid() && time > 0 && time < fight_duration)
                {
                    if (c.isBuff() == 1 && c.isBuffremove().getID() == 0)
                    {
                        if (boon_map.ContainsKey(c.getSkillID()))
                        {
                            boon_map[c.getSkillID()].getBoonLog().Add(new BoonLog(time, c.getValue(), c.getOverstackValue()));
                        }
                    }
                    else if (c.isBuffremove().getID() == 1)//All
                    {
                        if (boon_map.ContainsKey(c.getSkillID()))
                        {
                            List<BoonLog> loglist = boon_map[c.getSkillID()].getBoonLog();
                            for (int cnt = loglist.Count() - 1; cnt >= 0; cnt--)
                            {
                                BoonLog curBL = loglist[cnt];
                                if (curBL.getTime() + curBL.getValue() > time)
                                {
                                    int subtract = (curBL.getTime() + curBL.getValue()) - time;
                                    loglist[cnt] = new BoonLog(curBL.getTime(), curBL.getValue() - subtract, curBL.getOverstack() + subtract);
                                }
                            }
                        }
                    }
                    else if (c.isBuffremove().getID() == 2)//Single
                    {
                        if (boon_map.ContainsKey(c.getSkillID()))
                        {
                            List<BoonLog> loglist = boon_map[c.getSkillID()].getBoonLog();
                            int cnt = loglist.Count() - 1;
                            BoonLog curBL = loglist[cnt];
                            if (curBL.getTime() + curBL.getValue() > time)
                            {
                                int subtract = (curBL.getTime() + curBL.getValue()) - time;
                                loglist[cnt] = new BoonLog(curBL.getTime(), curBL.getValue() - subtract, curBL.getOverstack() + subtract);
                                break;
                            }
                        }
                    }
                    else if (c.isBuffremove().getID() == 3)//Manuel
                    {
                        if (boon_map.ContainsKey(c.getSkillID()))
                        {
                            List<BoonLog> loglist = boon_map[c.getSkillID()].getBoonLog();
                            for (int cnt = loglist.Count() - 1; cnt >= 0; cnt--)
                            {
                                BoonLog curBL = loglist[cnt];
                                if (curBL.getTime() + curBL.getValue() > time)
                                {
                                    int subtract = (curBL.getTime() + curBL.getValue()) - time;
                                    loglist[cnt] = new BoonLog(curBL.getTime(), curBL.getValue() - subtract, curBL.getOverstack() + subtract);
                                    break;
                                }
                            }
                        }
                    }
                }
            }

        }       
        public List<CastLog> getCastLogs( BossData bossData, List<CombatItem> combatList, AgentData agentData)
        {
            if (cast_logs.Count == 0)
            {
                setCastLogs(bossData, combatList, agentData);
            }
            return cast_logs;
        
        }
        private void setCastLogs(BossData bossData, List<CombatItem> combatList, AgentData agentData) {
            int time_start = bossData.getFirstAware();
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
                                int time = c.getTime() - time_start;
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
                            int time = c.getTime() - time_start;
                            curCastLog = new CastLog(time, -2, (int)c.getDstAgent(), c.isActivation());
                            cast_logs.Add(curCastLog);
                            curCastLog = null;
                        }
                    }
                }
            }
           
        }
        public void setBossDPSGraph(List<int[]> list) {
           bossdpsGraph = new List<int[]>(list);
        }
    }
}