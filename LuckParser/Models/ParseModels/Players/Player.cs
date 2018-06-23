using LuckParser.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class Player : AbstractMasterPlayer
    {
        // Fields
        private String account;
        private String group;
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


        public int getToughness()
        {
            return agent.getToughness();
        }

        public int getHealing()
        {
            return agent.getHealing();
        }

        public int getCondition()
        {
            return agent.getCondition();
        }
        // Public methods
        public int[] getCleanses(ParsedLog log, long start, long end) {
            long time_start = log.getBossData().getFirstAware();
            int[] cleanse = { 0, 0 };
            foreach (CombatItem c in log.getCombatList().Where(x=>x.isStateChange() == ParseEnum.StateChange.Normal && x.isBuff() == 1 && x.getTime() >= (start + time_start) && x.getTime() <= (end + time_start)))
            {
                if (c.isActivation() == ParseEnum.Activation.None)
                {
                    if (agent.getInstid() == c.getDstInstid() && c.getIFF() == ParseEnum.IFF.Friend && (c.isBuffremove() != ParseEnum.BuffRemove.None))
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
        public int[] getReses(ParsedLog log, long start, long end)
        {
            List<CastLog> cls = getCastLogs(log, start, end);
            int[] reses = { 0, 0 };
            foreach (CastLog cl in cls) {
                if (cl.getID() == 1066)
                {
                    reses[0]++;
                    reses[1] += cl.getActDur();
                }
            }
            return reses;
        }
        public string[] getWeaponsArray(ParsedLog log)
        {
            if (weapons_array == null)
            {
                EstimateWeapons( log);
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
        public List<int[]> getConsumablesList(ParsedLog log, long start, long end)
        {
            if (consumeList.Count() == 0)
            {
                setConsumablesList(log);
            }
            return consumeList.Where(x => x.getTime() >= start && x.getTime() <= end).Select( x => new int[] { x.getSkillID(), (int)x.getTime() }).ToList() ;
        }
        // Private Methods
        private void EstimateWeapons(ParsedLog log)
        {
            string[] weapons = new string[4];//first 2 for first set next 2 for second set
            List<SkillItem> s_list = log.getSkillData().getSkillList();
            List<CastLog> casting = getCastLogs(log, 0, log.getBossData().getAwareDuration());      
            int swapped = 0;//4 for first set and 5 for next
            List<CastLog> swaps = casting.Where(x => x.getID() == -2).Take(2).ToList();
            // If the player never swapped, assume they are on their first set
            if (swaps.Count == 0)
            {
                swapped = 4;
            }
            // if the player swapped once, check on which set they started
            else if (swaps.Count == 1)
            {
                swapped = swaps.First().getExpDur() == 4 ? 5 : 4;
            }
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
                    if (apiskill.type == "Weapon" && apiskill.professions.Count() > 0)
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
                                continue;
                            }
                        }// 1 handed
                    }

                }
                else if (cl.getID() == -2)
                {
                    int oldswap = swapped;
                    //wepswap  
                    swapped = cl.getExpDur();
                    if (swapped == oldswap)
                    {
                        swapped = 0;
                    }
                    continue;
                }
                if (weapons[0] != null && weapons[1] != null && weapons[2] != null && weapons[3] != null)
                {
                    break;
                }
            }
            weapons_array = weapons;
        }    
        protected override void setDamageLogs(ParsedLog log)
        {
            long time_start = log.getBossData().getFirstAware();
            foreach (CombatItem c in log.getDamageData())
            {
                if (agent.getInstid() == c.getSrcInstid() && c.getTime() > log.getBossData().getFirstAware() && c.getTime() < log.getBossData().getLastAware())//selecting player or minion as caster
                {
                    long time = c.getTime() - time_start;
                    foreach (AgentItem item in log.getAgentData().getNPCAgentList())
                    {//selecting all
                        addDamageLog(time, item.getInstid(), c, damage_logs);
                    }
                }
            }
            Dictionary<string, Minions> min_list = getMinions(log);
            foreach (Minions mins in min_list.Values)
            {
                damage_logs.AddRange(mins.getDamageLogs(0, log, 0, log.getBossData().getAwareDuration()));
            }
            damage_logs.Sort((x, y) => x.getTime() < y.getTime() ? -1 : 1);
        }     
        protected override void setDamagetakenLogs(ParsedLog log) {
            long time_start = log.getBossData().getFirstAware();               
            foreach (CombatItem c in log.getDamageTakenData()) {
                if (agent.getInstid() == c.getDstInstid() && c.getTime() > log.getBossData().getFirstAware() && c.getTime() < log.getBossData().getLastAware()) {//selecting player as target
                    long time = c.getTime() - time_start;
                    foreach (AgentItem item in log.getAgentData().getAllAgentsList())
                    {//selecting all
                        addDamageTakenLog(time, item.getInstid(), c);
                    }
                }
            }
        }  
        private void setConsumablesList(ParsedLog log)
        {
            List<Boon> foodBoon = Boon.getFoodList();
            List<Boon> utilityBoon = Boon.getUtilityList();
            long time_start = log.getBossData().getFirstAware();
            long fight_duration = log.getBossData().getLastAware() - time_start;
            foreach (CombatItem c in log.getCombatList())
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
                if (agent.getInstid() == c.getDstInstid())
                {
                    consumeList.Add(c); 
                }
            }
        }
        
       
    }
}
