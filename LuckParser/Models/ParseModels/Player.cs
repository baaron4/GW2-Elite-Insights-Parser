using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckParser.Models.ParseModels
{
    public class Player : AbstractPlayer
    {
        // Fields
        private String account;
        private String group;
        private String prof;
        private int toughness;
        private int healing;
        private int condition;
        private long dcd = 0;//time in ms the player dcd
       
        private List<CombatItem> consumeList = new List<CombatItem>();
        //weaponslist
        private string[] weapons_array;
        // Constructors
        public Player(AgentItem agent) : base(agent)
        {
            String[] name = agent.getName().Split('\0');
            account = name[1];
            group = name[2];
            prof = agent.getProf();
            toughness = agent.getToughness();
            healing = agent.getHealing();
            condition = agent.getCondition();
        }

        // Getters

        public string getAccount()
        {
            return account;
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
        // Public methods
        public int[] getCleanses(BossData bossData, List<CombatItem> combatList, AgentData agentData, long start, long end) {
            long time_start = bossData.getFirstAware();
            int[] cleanse = { 0, 0 };
            foreach (CombatItem c in combatList.Where(x=>x.isStateChange().getID() == 0 && x.isBuff() == 1 && x.getTime() >= (start + time_start) && x.getTime() < (end + time_start)))
            {
                if (c.isActivation().getID() == 0)
                {
                    if (instid == c.getDstInstid() && c.getIFF().getEnum() == "FRIEND" && (c.isBuffremove().getID() == 1)/*|| instid == c.getSrcMasterInstid()*/)//selecting player as remover could be wrong
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
        public int[] getReses(BossData bossData, List<CombatItem> combatList, AgentData agentData, long start, long end)
        {
            List<CastLog> cls = getCastLogs(bossData, combatList, agentData, start, end);
            int[] reses = { 0, 0 };
            foreach (CastLog log in cls) {
                if (log.getID() == 1066)
                {
                    reses[0]++;
                    reses[1] += log.getActDur();
                }
            }
            return reses;
        }
        public string[] getWeaponsArray(SkillData s_data, CombatData c_data, BossData b_data, AgentData a_data)
        {
            if (weapons_array == null)
            {
                EstimateWeapons( s_data,  c_data,  b_data,  a_data);
            }
            return weapons_array;
        }
        
        public long GetDC()
        {
            return dcd;
        }
        public void SetDC(long value)
        {
            dcd = value;
        }
        public List<int[]> getConsumablesList(BossData bossData, SkillData skillData, List<CombatItem> combatList, long start, long end)
        {
            if (consumeList.Count() == 0)
            {
                setConsumablesList( bossData, skillData, combatList);
            }
            return consumeList.Where(x => x.getTime() >= start && x.getTime() < end).Select( x => new int[] { x.getSkillID(), (int)x.getTime() }).ToList() ;
        }
        // Private Methods
        private void EstimateWeapons(SkillData s_data, CombatData c_data, BossData b_data, AgentData a_data)
        {
            string[] weapons = new string[4];//first 2 for first set next 2 for second set
            List<SkillItem> s_list = s_data.getSkillList();
            List<CastLog> casting = getCastLogs(b_data, c_data.getCombatList(), a_data, 0, b_data.getAwareDuration());
            int swapped = 0;//4 for first set and 5 for next
            foreach (CastLog cl in casting)
            {
                GW2APISkill apiskill = null;
                SkillItem skill = s_list.FirstOrDefault(x => x.getID() == cl.getID());
                if (skill != null)
                {
                    apiskill = skill.GetGW2APISkill();
                }
                if (apiskill != null)
                {
                    if (apiskill.type == "Weapon")
                    {
                        if (apiskill.weapon_type == "Greatsword" || apiskill.weapon_type == "Staff" || apiskill.weapon_type == "Rifle" || apiskill.weapon_type == "Longbow" || apiskill.weapon_type == "Shortbow" || apiskill.weapon_type == "Hammer")
                        {
                            if (swapped == 4 && (weapons[0] == null && weapons[1] == null))
                            {
                                weapons[0] = apiskill.weapon_type;
                                weapons[1] = "2Hand";
                                continue;
                            }
                            else if (swapped == 5 && (weapons[2] == null && weapons[3] == null))
                            {
                                weapons[2] = apiskill.weapon_type;
                                weapons[3] = "2Hand";
                                continue;
                            }

                            //if (weapons[0] == null && weapons[1] == null)
                            //{
                            //    weapons[0] = apiskill.weapon_type;
                            //    weapons[1] = "2Hand";
                            //}
                            //else if (weapons[2] == null && weapons[3] == null)
                            //{
                            //    weapons[2] = apiskill.weapon_type;
                            //    weapons[3] = "2Hand";
                            //}
                            continue;
                        }//2 handed
                        if (apiskill.weapon_type == "Focus" || apiskill.weapon_type == "Shield" || apiskill.weapon_type == "Torch" || apiskill.weapon_type == "Warhorn")
                        {
                            if (swapped == 4 && (weapons[1] == null))
                            {

                                weapons[1] = apiskill.weapon_type;
                                continue;
                            }
                            else if (swapped == 5 && (weapons[3] == null))
                            {

                                weapons[3] = apiskill.weapon_type;
                                continue;
                            }
                            //if (weapons[1] == null)
                            //{

                            //    weapons[1] = apiskill.weapon_type;
                            //}
                            //else if (weapons[3] == null)
                            //{

                            //    weapons[3] = apiskill.weapon_type;
                            //}
                            continue;
                        }//OffHand
                        if (apiskill.weapon_type == "Axe" || apiskill.weapon_type == "Dagger" || apiskill.weapon_type == "Mace" || apiskill.weapon_type == "Pistol" || apiskill.weapon_type == "Sword" || apiskill.weapon_type == "Scepter")
                        {
                            if (apiskill.slot == "Weapon_1" || apiskill.slot == "Weapon_2" || apiskill.slot == "Weapon_3")
                            {
                                if (swapped == 4 && (weapons[0] == null))
                                {

                                    weapons[0] = apiskill.weapon_type;
                                    continue;
                                }
                                else if (swapped == 5 && (weapons[2] == null))
                                {

                                    weapons[2] = apiskill.weapon_type;
                                    continue;
                                }
                                //if (weapons[0] == null)
                                //{

                                //    weapons[0] = apiskill.weapon_type;
                                //}
                                //else if (weapons[2] == null)
                                //{

                                //    weapons[2] = apiskill.weapon_type;
                                //}
                                continue;
                            }
                            if (apiskill.slot == "Weapon_4" || apiskill.slot == "Weapon_5")
                            {
                                if (swapped == 4 && (weapons[1] == null))
                                {

                                    weapons[1] = apiskill.weapon_type;
                                    continue;
                                }
                                else if (swapped == 5 && (weapons[3] == null))
                                {

                                    weapons[3] = apiskill.weapon_type;
                                    continue;
                                }
                                //if (weapons[1] == null)
                                //{

                                //    weapons[1] = apiskill.weapon_type;
                                //}
                                //else if (weapons[3] == null)
                                //{

                                //    weapons[3] = apiskill.weapon_type;
                                //}
                                continue;
                            }
                        }//1 handed


                    }

                }
                else if (cl.getID() == -2)
                {
                    //wepswap  
                    swapped = cl.getExpDur();
                    continue;
                }
                if (weapons[0] != null && weapons[1] != null && weapons[2] != null && weapons[3] != null)
                {
                    break;
                }
            }
            weapons_array = weapons;
        }
        protected override void setDamageLogs(BossData bossData, List<CombatItem> combatList, AgentData agentData)
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
        protected override void setDamagetakenLogs(BossData bossData, List<CombatItem> combatList, AgentData agentData,MechanicData m_data) {
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
                        consumeList.Add(c);
                   // }
                   
                   
                }
            }
        }
        
       
    }
}
