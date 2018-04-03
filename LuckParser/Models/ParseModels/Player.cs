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
        private List<BoonMap> boon_map = new List<BoonMap>();
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
                setFilteredLogs(bossData, combatList, agentData,instidFilter);
            }
            if (instidFilter == 0)
            {
                return damage_logs;
            }else {
                return damage_logsFiltered;
            }
        }
        public List<int> getDamagetaken( BossData bossData, List<CombatItem> combatList, AgentData agentData)
        {
            if (damagetaken.Count == 0)
            {
                setDamagetaken(bossData, combatList, agentData);
            }
            return damagetaken;
        }
        public List<DamageLog> getDamageTakenLogs(BossData bossData, List<CombatItem> combatList, AgentData agentData)
        {
            if (damagetaken.Count == 0)
            {
                setDamagetaken(bossData, combatList, agentData);
            }
            return damageTaken_logs;
        }
        public List<BoonMap> getBoonMap(BossData bossData, SkillData skillData, List<CombatItem> combatList)
        {
            if (boon_map.Count == 0)
            {
                setBoonMap(bossData, skillData, combatList);
            }
            return boon_map;
        }

        public List<BoonMap> getboonGen(BossData bossData, SkillData skillData, List<CombatItem> combatList, AgentData agentData,List<int> trgtPID)
        {
            List<BoonMap> boonGen = new List<BoonMap>();
            int time_start = bossData.getFirstAware();
            int fight_duration = bossData.getLastAware() - time_start;
            int here = 0, there= 0 , everywhere = 0, huh = 0;
            // Initialize Boon Map with every Boon
            foreach (Boon boon in Boon.getList())
            {
                BoonMap map = new BoonMap(boon.getName(), new List<BoonLog>());
                boonGen.Add(map);
                // boon_map.put(boon.getName(), new ArrayList<BoonLog>());
            }

            foreach (CombatItem c in combatList)
            {

                LuckParser.Models.ParseEnums.StateChange state = c.isStateChange();
                int time = c.getTime() - time_start;
                if (instid == c.getSrcInstid() && state.getEnum() == "NORMAL" && time > 0 && time < fight_duration/*|| instid == c.getSrcMasterInstid()*/)//selecting player or minion as caster
                {
                    here++;
                    foreach (AgentItem item in agentData.getPlayerAgentList())
                    {//selecting all
                        if (item.getInstid() == c.getDstInstid() /*&& c.getIFF().getEnum() == "FRIEND"*/)//Make sure target is friendly existing Agent
                        {
                            there++;
                            foreach (int id in trgtPID) {//Make sure trgt is within paramaters
                                if (id == c.getDstInstid()) {
                                    everywhere++;
                                        if (c.isBuffremove().getID() == 0 && c.isBuff() > 0 && c.getBuffDmg() == 0 && c.getValue() > 0) {//Buff application
                                            huh++;
                                            String skill_name = skillData.getName(c.getSkillID());
                                            int count = 0;
                                           
                                            foreach (BoonMap bm in boonGen.ToList())
                                            {
                                                if (skill_name.Contains(bm.getName()))
                                                {
                                                        List<BoonLog> loglist = bm.getBoonLog();
                                                        loglist.Add(new BoonLog(time, c.getValue(), c.getOverstackValue()));
                                                        bm.setBoonLog(loglist);
                                                       
                                                        boonGen[count] = bm;
                                                }
                                                count++;

                                            }
                                        }
                                    
                                }
                            }
                           
                        }
                    }

                }
                
            }
            return boonGen;
        }
        public int[] getCleanses(BossData bossData, List<CombatItem> combatList, AgentData agentData) {
            int time_start = bossData.getFirstAware();
            int[] cleanse = { 0, 0 };
            foreach (CombatItem c in combatList)
            {
                if (instid == c.getSrcInstid() && c.getIFF().getEnum() == "FRIEND" && c.isBuffremove().getID() == 3/*|| instid == c.getSrcMasterInstid()*/)//selecting player as remover could be wrong
                {
                    int time = c.getTime() - time_start;
                    if (time > 0) {
                        if (Boon.getCondiList().Contains(c.getSkillID()) ) { }
                        cleanse[0]++;
                        cleanse[1] += c.getValue();
                    }
                   

                }
            }
            return cleanse;
        }
        public int[] getReses(BossData bossData, List<CombatItem> combatList, AgentData agentData)
        {
            int time_start = bossData.getFirstAware();
            int[] reses = { 0, 0 };
            foreach (DamageLog log in damage_logs) {
                if (log.getID() == 1066)
                {
                    reses[0]++;
                    reses[1] += log.getDamage();
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
                if (combatStart && !combatEnd )
                {
                    if (instid == c.getSrcInstid() || instid == c.getSrcMasterInstid())//selecting player or minion as caster
                    {
                        LuckParser.Models.ParseEnums.StateChange state = c.isStateChange();
                        int time = c.getTime() - time_start;
                        foreach (AgentItem item in agentData.getNPCAgentList())
                        {//selecting all
                            if (item.getInstid() == c.getDstInstid() && c.getIFF().getEnum() == "FOE")
                            {
                                if (state.getID() == 0)
                                {
                                    if (c.isBuff() == 1 && c.getBuffDmg() != 0)
                                    {

                                        damage_logs.Add(new DamageLog(time, c.getBuffDmg(), c.getSkillID(), c.isBuff(),
                                                c.getResult(), c.isNinety(), c.isMoving(), c.isFlanking(), c.isActivation()));
                                    }
                                    else if (c.isBuff() == 0 && c.getValue() != 0)
                                    {
                                        damage_logs.Add(new DamageLog(time, c.getValue(), c.getSkillID(), c.isBuff(),
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
                        if (state.getEnum() == "NORMAL")
                        {
                            if (c.isBuff() == 1 && c.getBuffDmg() != 0)
                            {
                                filterDLog.Add(new DamageLog(time, c.getBuffDmg(), c.getSkillID(), c.isBuff(),
                                        c.getResult(), c.isNinety(), c.isMoving(), c.isFlanking(), c.isActivation()));
                            }
                            else if (c.isBuff() == 0 && c.getValue() != 0)
                            {
                                filterDLog.Add(new DamageLog(time, c.getValue(), c.getSkillID(), c.isBuff(),
                                        c.getResult(), c.isNinety(), c.isMoving(), c.isFlanking(), c.isActivation()));
                            }
                        }
                    }
                }
            }
            damage_logsFiltered = filterDLog;
        }
        public void setDamagetaken(BossData bossData, List<CombatItem> combatList, AgentData agentData) {
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
                                    damageTaken_logs.Add(new DamageLog(time, c.getValue(), c.getSkillID(), c.isBuff(),
                                           c.getResult(), c.isNinety(), c.isMoving(), c.isFlanking(), c.isActivation()));
                                }
                                else if (c.isBuff() == 0  && c.getValue() == 0)
                                {
                                  
                                    damageTaken_logs.Add(new DamageLog(time, c.getBuffDmg(), c.getSkillID(), c.isBuff(),
                                           c.getResult(), c.isNinety(), c.isMoving(), c.isFlanking(), c.isActivation()));
                                }
                            }
                        }
                    }
                }
            }
        }
        public void setBoonMap(BossData bossData, SkillData skillData, List<CombatItem> combatList)
        {

            // Initialize Boon Map with every Boon
            foreach (Boon boon in Boon.getMainList())
            {
                BoonMap map = new BoonMap(boon.getName(), new List<BoonLog>());
                boon_map.Add(map);
               // boon_map.put(boon.getName(), new ArrayList<BoonLog>());
            }
            foreach (Boon boon in Boon.getAllProfList())
            {
                BoonMap map = new BoonMap(boon.getName(), new List<BoonLog>());
                boon_map.Add(map);
                // boon_map.put(boon.getName(), new ArrayList<BoonLog>());
            }

            // Fill in Boon Map
            int time_start = bossData.getFirstAware();
            int fight_duration = bossData.getLastAware() - time_start;
            foreach (CombatItem c in combatList)
            {
                if (instid == c.getDstInstid())
                {
                    String skill_name = skillData.getName(c.getSkillID());
                    if (c.isBuff() == 1 && c.getValue() > 0 && c.isBuffremove().getID() == 0)
                    {
                        int count = 0;
                        foreach (BoonMap bm in boon_map.ToList())
                        {
                            if (skill_name.Contains(bm.getName()))
                            {
                                int time = c.getTime() - time_start;
                                if (time < fight_duration)
                                {
                                    List<BoonLog> loglist = bm.getBoonLog();
                                    loglist.Add(new BoonLog(time, c.getValue(), c.getOverstackValue()));
                                    bm.setBoonLog(loglist);
                                    
                                    boon_map[count] = bm;
                                }
                                else
                                {
                                    break;
                                }

                            }
                            count++;

                        }
                    }else
                    if (c.isBuffremove().getID() == 1 && c.getValue() > 0)//All
                    {
                        //finding correct boonmap
                        int count = 0;
                        foreach (BoonMap bm in boon_map.ToList())
                        {
                            if (skill_name.Contains(bm.getName()))
                            {
                                //make sure log is within fight time
                                int time = c.getTime() - time_start;
                                if (time < fight_duration)
                                {
                                    List<BoonLog> loglist = bm.getBoonLog();

                                    for (int cnt = loglist.Count()-1; cnt >= 0; cnt--) {
                                        BoonLog curBL = loglist[cnt];
                                        if (curBL.getTime() + curBL.getValue() > time) {
                                            int subtract = (curBL.getTime() + curBL.getValue()) - time;
                                            loglist[cnt] = new BoonLog(curBL.getTime(),curBL.getValue()- subtract,curBL.getOverstack()+subtract);
                                        }
                                    }
                                   // loglist.Add(new BoonLog(time, c.getValue(), c.getOverstackValue()));
                                    bm.setBoonLog(loglist);
                                    boon_map[count] = bm;
                                }
                                else
                                {
                                    break;
                                }

                            }
                            count++;

                        }
                    }
                    else if (c.isBuffremove().getID() == 2 && c.getValue() > 0)//Single
                    {
                        //finding correct boonmap
                        int count = 0;
                        foreach (BoonMap bm in boon_map.ToList())
                        {
                            if (skill_name.Contains(bm.getName()))
                            {
                                //make sure log is within fight time
                                int time = c.getTime() - time_start;
                                if (time < fight_duration)
                                {
                                    List<BoonLog> loglist = bm.getBoonLog();

                                    for (int cnt = loglist.Count() - 1; cnt >= 0; cnt--)
                                    {
                                        BoonLog curBL = loglist[cnt];
                                        if (curBL.getTime() + curBL.getValue() > time)
                                        {
                                            int subtract = (curBL.getTime() + curBL.getValue()) -time;
                                            loglist[cnt] = new BoonLog(curBL.getTime(), curBL.getValue() - subtract, curBL.getOverstack() + subtract);
                                            break;
                                        }
                                    }
                                    // loglist.Add(new BoonLog(time, c.getValue(), c.getOverstackValue()));
                                    bm.setBoonLog(loglist);
                                    boon_map[count] = bm;
                                    break;
                                }
                                else
                                {
                                    break;
                                }

                            }
                            count++;

                        }
                    }
                    else if (c.isBuffremove().getID() == 3 && c.getValue() > 0)//Manuel
                    {
                        //finding correct boonmap
                        int count = 0;
                        foreach (BoonMap bm in boon_map.ToList())
                        {
                            if (skill_name.Contains(bm.getName()))
                            {
                                //make sure log is within fight time
                                int time = c.getTime() - time_start;
                                if (time < fight_duration)
                                {
                                    List<BoonLog> loglist = bm.getBoonLog();

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
                                    // loglist.Add(new BoonLog(time, c.getValue(), c.getOverstackValue()));
                                    bm.setBoonLog(loglist);
                                    boon_map[count] = bm;
                                    break;
                                }
                                else
                                {
                                    break;
                                }

                            }
                            count++;

                        }
                    }
                }
            }
            int test = 0;
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
                    int time = c.getTime() - time_start;
                    curCastLog = new CastLog(time, -2, (int)c.getDstAgent(), c.isActivation());
                }
            }
           
        }

    }
}