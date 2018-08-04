using LuckParser.Controllers;
using LuckParser.Models.DataModels;
using System;
using System.Collections.Generic;

using System.Globalization;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class Player : AbstractMasterPlayer
    {
        // Fields
        private String account;
        private int group;
        private long dcd = 0;//time in ms the player dcd
       
        private List<long[]> consumeList = new List<long[]>();
        //weaponslist
        private string[] weapons_array;

        // Constructors
        public Player(AgentItem agent, bool noSquad) : base(agent)
        {
            String[] name = agent.getName().Split('\0');
            account = name[1];
            group = noSquad ? 1 : int.Parse(name[2], NumberStyles.Integer, CultureInfo.InvariantCulture);
        }

        // Getters

        public string getAccount()
        {
            return account;
        }
    
        public int getGroup()
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

        public int getConcentration()
        {
            return agent.getConcentration();
        }
        // Public methods
        public int[] getCleanses(ParsedLog log, long start, long end) {
            long time_start = log.getBossData().getFirstAware();
            int[] cleanse = { 0, 0 };
            foreach (CombatItem c in log.getCombatList().Where(x=>x.isStateChange() == ParseEnum.StateChange.Normal && x.isBuff() == 1 && x.getTime() >= (start + time_start) && x.getTime() <= (end + time_start)))
            {
                if (c.isActivation() == ParseEnum.Activation.None)
                {
                    if ((agent.getInstid() == c.getDstInstid() || agent.getInstid() == c.getDstMasterInstid()) && c.getIFF() == ParseEnum.IFF.Friend && (c.isBuffremove() != ParseEnum.BuffRemove.None))
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
        public List<long[]> getConsumablesList(ParsedLog log, long start, long end)
        {
            if (consumeList.Count == 0)
            {
                setConsumablesList(log);
            }
            return consumeList.Where(x => x[1] >= start && x[1] <= end).ToList() ;
        }
        
        // Private Methods
        private void EstimateWeapons(ParsedLog log)
        {
            string[] weapons = new string[4];//first 2 for first set next 2 for second set
            List<SkillItem> s_list = log.getSkillData().getSkillList();
            List<CastLog> casting = getCastLogs(log, 0, log.getBossData().getAwareDuration());      
            int swapped = 0;//4 for first set and 5 for next
            long swappedTime = 0;
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
                if (apiskill != null && cl.getTime() > swappedTime)
                {
                    if (apiskill.type == "Weapon" && apiskill.professions.Count() > 0 && (apiskill.categories == null || (apiskill.categories.Count() == 1 && apiskill.categories[0] == "Phantasm")))
                    {
                        if (apiskill.weapon_type == "Greatsword" || apiskill.weapon_type == "Staff" || apiskill.weapon_type == "Rifle" || apiskill.weapon_type == "Longbow" || apiskill.weapon_type == "Shortbow" || apiskill.weapon_type == "Hammer")
                        {
                            if (swapped == 4)
                            {
                                weapons[0] = apiskill.weapon_type;
                                weapons[1] = "2Hand";
                                continue;
                            }
                            else if (swapped == 5)
                            {
                                weapons[2] = apiskill.weapon_type;
                                weapons[3] = "2Hand";
                                continue;
                            }
                            continue;
                        }//2 handed
                        if (apiskill.weapon_type == "Focus" || apiskill.weapon_type == "Shield" || apiskill.weapon_type == "Torch" || apiskill.weapon_type == "Warhorn")
                        {
                            if (swapped == 4)
                            {

                                weapons[1] = apiskill.weapon_type;
                                continue;
                            }
                            else if (swapped == 5)
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
                                if (swapped == 4)
                                {

                                    weapons[0] = apiskill.weapon_type;
                                    continue;
                                }
                                else if (swapped == 5)
                                {

                                    weapons[2] = apiskill.weapon_type;
                                    continue;
                                }
                                continue;
                            }
                            if (apiskill.slot == "Weapon_4" || apiskill.slot == "Weapon_5")
                            {
                                if (swapped == 4)
                                {

                                    weapons[1] = apiskill.weapon_type;
                                    continue;
                                }
                                else if (swapped == 5)
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
                    //wepswap  
                    swapped = cl.getExpDur();
                    swappedTime = cl.getTime();
                    continue;
                }
            }
            weapons_array = weapons;
        }    
        
        protected override void setDamagetakenLogs(ParsedLog log)
        {
            long time_start = log.getBossData().getFirstAware();               
            foreach (CombatItem c in log.getDamageTakenData()) {
                if (agent.getInstid() == c.getDstInstid() && c.getTime() > log.getBossData().getFirstAware() && c.getTime() < log.getBossData().getLastAware()) {//selecting player as target
                    long time = c.getTime() - time_start;
                    addDamageTakenLog(time, c);
                }
            }
        }  
        private void setConsumablesList(ParsedLog log)
        {
            List<Boon> foodBoon = Boon.getFoodList();
            List<Boon> utilityBoon = Boon.getUtilityList();
            long time_start = log.getBossData().getFirstAware();
            long fight_duration = log.getBossData().getLastAware() - time_start;
            foreach (CombatItem c in log.getBoonData())
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
                if (agent.getInstid() == c.getDstInstid() && time <= fight_duration)
                {
                    consumeList.Add(new long[] { c.getSkillID(), Math.Max(time, 0) }); 
                }
            }
        }

        private static List<CombatItem> getFilteredList(ParsedLog log, long skillID, ushort instid)
        {
            bool needStart = true;
            List<CombatItem> main = log.getBoonData().Where(x => x.getSkillID() == skillID && ((x.getDstInstid() == instid && x.isBuffremove() == ParseEnum.BuffRemove.None) || (x.getSrcInstid() == instid && x.isBuffremove() != ParseEnum.BuffRemove.None))).ToList();
            List<CombatItem> filtered = new List<CombatItem>();
            foreach (CombatItem c in main)
            {
                if (needStart && c.isBuffremove() == ParseEnum.BuffRemove.None)
                {
                    needStart = false;
                    filtered.Add(c);
                }
                else if (!needStart && c.isBuffremove() != ParseEnum.BuffRemove.None)
                {
                    needStart = true;
                    filtered.Add(c);
                }
            }
            return filtered;
        }

        protected override void setAdditionalCombatReplayData(ParsedLog log, int pollingRate)
        {
            // Down and deads
            List<CombatItem> status = log.getCombatData().getStates(getInstid(), ParseEnum.StateChange.ChangeDown, log.getBossData().getFirstAware(), log.getBossData().getLastAware());
            status.AddRange(log.getCombatData().getStates(getInstid(), ParseEnum.StateChange.ChangeUp, log.getBossData().getFirstAware(), log.getBossData().getLastAware()));
            status.AddRange(log.getCombatData().getStates(getInstid(), ParseEnum.StateChange.ChangeDead, log.getBossData().getFirstAware(), log.getBossData().getLastAware()));
            status = status.OrderBy(x => x.getTime()).ToList();
            List<Tuple<long, long>> dead = new List<Tuple<long, long>>();
            List<Tuple<long, long>> down = new List<Tuple<long, long>>();
            for (var i = 0; i < status.Count -1;i++)
            {
                CombatItem cur = status[i];
                CombatItem next = status[i + 1];
                if (cur.isStateChange() == ParseEnum.StateChange.ChangeDown)
                {
                    down.Add(new Tuple<long, long>(cur.getTime() - log.getBossData().getFirstAware(), next.getTime() - log.getBossData().getFirstAware()));
                } else if (cur.isStateChange() == ParseEnum.StateChange.ChangeDead)
                {
                    dead.Add(new Tuple<long, long>(cur.getTime() - log.getBossData().getFirstAware(), next.getTime() - log.getBossData().getFirstAware()));
                }
            }
            // check last value
            if (status.Count > 0)
            {
                CombatItem cur = status.Last();
                if (cur.isStateChange() == ParseEnum.StateChange.ChangeDown)
                {
                    down.Add(new Tuple<long, long>(cur.getTime() - log.getBossData().getFirstAware(), log.getBossData().getAwareDuration()));
                }
                else if (cur.isStateChange() == ParseEnum.StateChange.ChangeDead)
                {
                    dead.Add(new Tuple<long, long>(cur.getTime() - log.getBossData().getFirstAware(), log.getBossData().getAwareDuration()));
                }
            }
            replay.setStatus(down, dead);
            // Boss related stuff
            switch (log.getBossData().getID())
            {
                // VG
                case 15438:
                    break;
                // Gorse
                case 15429:
                    break;
                // Sab
                case 15375:
                    // timed bombs
                    List<CombatItem> timedBombs = log.getBoonData().Where(x => x.getSkillID() == 31485 && (x.getDstInstid() == getInstid() && x.isBuffremove() == ParseEnum.BuffRemove.None)).ToList();
                    foreach(CombatItem c in timedBombs)
                    {
                        int start = (int)(c.getTime() - log.getBossData().getFirstAware());
                        int end = start + 3000;
                        replay.addCircleActor(new FollowingCircle(false, 0, 300, new Tuple<int,int>(start,end), "rgba(255, 150, 0, 0.5)"));
                        replay.addCircleActor(new FollowingCircle(true, end, 300, new Tuple<int, int>(start, end), "rgba(255, 150, 0, 0.5)"));
                    }
                    // Sapper bombs
                    List<CombatItem> sapperBombs = getFilteredList(log,31473,getInstid());
                    int sapperStart = 0;
                    int sapperEnd = 0;
                    foreach(CombatItem c in sapperBombs)
                    {
                        if (c.isBuffremove() == ParseEnum.BuffRemove.None)
                        {
                            sapperStart = (int)(c.getTime() - log.getBossData().getFirstAware());
                        } else
                        {
                            sapperEnd = (int)(c.getTime() - log.getBossData().getFirstAware()); replay.addCircleActor(new FollowingCircle(false, 0, 180, new Tuple<int, int>(sapperStart, sapperEnd), "rgba(200, 255, 100, 0.5)"));
                            replay.addCircleActor(new FollowingCircle(true, sapperStart + 5000, 180, new Tuple<int, int>(sapperStart, sapperEnd), "rgba(200, 255, 100, 0.5)"));
                        }                  
                    }
                    break;
                // Sloth
                case 16123:
                    // Poison
                    List<CombatItem> poisonToDrop = getFilteredList(log,34387,getInstid());
                    int toDropStart = 0;
                    int toDropEnd = 0;
                    foreach (CombatItem c in poisonToDrop)
                    {
                        if (c.isBuffremove() == ParseEnum.BuffRemove.None)
                        {
                            toDropStart = (int)(c.getTime() - log.getBossData().getFirstAware());
                        }
                        else
                        {
                            toDropEnd = (int)(c.getTime() - log.getBossData().getFirstAware()); replay.addCircleActor(new FollowingCircle(false, 0, 180, new Tuple<int, int>(toDropStart, toDropEnd), "rgba(255, 255, 100, 0.5)"));
                            replay.addCircleActor(new FollowingCircle(true, toDropStart + 8000, 180, new Tuple<int, int>(toDropStart, toDropEnd), "rgba(255, 255, 100, 0.5)"));
                            Point3D poisonPos = replay.getPositions().FirstOrDefault(x => x.time >= toDropEnd);
                            replay.addCircleActor(new ImmobileCircle(true, toDropStart + 90000, 900, new Tuple<int, int>(toDropEnd, toDropEnd+90000), "rgba(255, 0, 0, 0.3)", poisonPos));
                        }
                    }
                    // Transformation
                    List<CombatItem> slubTrans = getFilteredList(log, 34362, getInstid());
                    int transfoStart = 0;
                    int transfoEnd = 0;
                    foreach (CombatItem c in slubTrans)
                    {
                        if (c.isBuffremove() == ParseEnum.BuffRemove.None)
                        {
                            transfoStart = (int)(c.getTime() - log.getBossData().getFirstAware());
                        }
                        else
                        {
                            transfoEnd = (int)(c.getTime() - log.getBossData().getFirstAware()); replay.addCircleActor(new FollowingCircle(true, 0, 120, new Tuple<int, int>(transfoStart, transfoEnd), "rgba(0, 80, 255, 0.3)"));
                        }
                    }
                    // fixated
                    List<CombatItem> fixatedSloth = getFilteredList(log, 34508, getInstid());
                    int fixatedSlothStart = 0;
                    int fixatedSlothEnd = 0;
                    foreach (CombatItem c in fixatedSloth)
                    {
                        if (c.isBuffremove() == ParseEnum.BuffRemove.None)
                        {
                            fixatedSlothStart = (int)(c.getTime() - log.getBossData().getFirstAware());
                        }
                        else
                        {
                            fixatedSlothEnd = (int)(c.getTime() - log.getBossData().getFirstAware()); replay.addCircleActor(new FollowingCircle(true, 0, 120, new Tuple<int, int>(fixatedSlothStart, fixatedSlothEnd), "rgba(255, 80, 255, 0.3)"));
                        }
                    }
                    break;
                // Matthias
                case 16115:
                    // Corruption
                    // Well of profane
                    break;
                // KC
                case 16235:
                    // bombs
                    break;
                // Xera
                case 16246:
                    break;
                // Cairn
                case 17194:
                    // shared agony
                    break;
                // MO
                case 17172:
                    break;
                // Samarog
                case 17188:
                    // big bomb
                    // small bomb
                    // fixated
                    break;
                // Deimos
                case 17154:
                    // teleport zone
                    break;
                // SH
                case 0x4D37:
                    break;
                // Dhuum
                case 0x4BFA:
                    // spirit transform
                    // bomb
                    break;
                // MAMA
                case 0x427D:
                    break;
                // Siax
                case 0x4284:
                    break;
                // Ensolyss
                case 0x4234:
                    break;
                // Skorvald
                case 0x44E0:
                    break;
                // Artsariiv
                case 0x461D:
                    break;
                // Arkk
                case 0x455F:
                    break;
            }
        }

        protected override void setCombatReplayIcon(ParsedLog log)
        {
            replay.setIcon(HTMLHelper.GetLink(getProf()));
        }

        /*protected override void setHealingLogs(ParsedLog log)
        {
            long time_start = log.getBossData().getFirstAware();
            foreach (CombatItem c in log.getHealingData())
            {
                if (agent.getInstid() == c.getSrcInstid() && c.getTime() > log.getBossData().getFirstAware() && c.getTime() < log.getBossData().getLastAware())//selecting player or minion as caster
                {
                    long time = c.getTime() - time_start;
                    addHealingLog(time, c);
                }
            }
            Dictionary<string, Minions> min_list = getMinions(log);
            foreach (Minions mins in min_list.Values)
            {
                healing_logs.AddRange(mins.getHealingLogs(log, 0, log.getBossData().getAwareDuration()));
            }
            healing_logs.Sort((x, y) => x.getTime() < y.getTime() ? -1 : 1);
        }

        protected override void setHealingReceivedLogs(ParsedLog log)
        {
            long time_start = log.getBossData().getFirstAware();
            foreach (CombatItem c in log.getHealingReceivedData())
            {
                if (agent.getInstid() == c.getDstInstid() && c.getTime() > log.getBossData().getFirstAware() && c.getTime() < log.getBossData().getLastAware())
                {//selecting player as target
                    long time = c.getTime() - time_start;
                    addHealingReceivedLog(time, c);
                }
            }
        }*/
    }
}
