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
            switch (ParseEnum.getBossIDS(log.getBossData().getID()))
            {
                // VG
                case ParseEnum.BossIDS.ValeGuardian:
                    break;
                // Gorse
                case ParseEnum.BossIDS.Gorseval:
                    break;
                // Sab
                case ParseEnum.BossIDS.Sabetha:
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
                case ParseEnum.BossIDS.Slothasor:
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
                            transfoEnd = (int)(c.getTime() - log.getBossData().getFirstAware());
                            replay.addCircleActor(new FollowingCircle(true, 0, 120, new Tuple<int, int>(transfoStart, transfoEnd), "rgba(0, 80, 255, 0.3)"));
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
                            fixatedSlothEnd = (int)(c.getTime() - log.getBossData().getFirstAware());
                            replay.addCircleActor(new FollowingCircle(true, 0, 120, new Tuple<int, int>(fixatedSlothStart, fixatedSlothEnd), "rgba(255, 80, 255, 0.3)"));
                        }
                    }
                    break;
                // Matthias
                case ParseEnum.BossIDS.Matthias:
                    // Corruption
                    List<CombatItem> corruptedMatthias = getFilteredList(log, 34416, getInstid());
                    corruptedMatthias.AddRange(getFilteredList(log, 34473, getInstid()));
                    int corruptedMatthiasStart = 0;
                    int corruptedMatthiasEnd = 0;
                    foreach (CombatItem c in corruptedMatthias)
                    {
                        if (c.isBuffremove() == ParseEnum.BuffRemove.None)
                        {
                            corruptedMatthiasStart = (int)(c.getTime() - log.getBossData().getFirstAware());
                        }
                        else
                        {
                            corruptedMatthiasEnd = (int)(c.getTime() - log.getBossData().getFirstAware());
                            replay.addCircleActor(new FollowingCircle(true, 0, 120, new Tuple<int, int>(corruptedMatthiasStart, corruptedMatthiasEnd), "rgba(255, 150, 0, 0.5)"));
                            Point3D wellPosition = replay.getPositions().FirstOrDefault(x => x.time >= corruptedMatthiasEnd);
                            replay.addCircleActor(new ImmobileCircle(true, 0, 120, new Tuple<int, int>(corruptedMatthiasEnd, corruptedMatthiasEnd + 100000), "rgba(0, 0, 0, 0.3)", wellPosition));
                            replay.addCircleActor(new ImmobileCircle(true, corruptedMatthiasEnd + 100000, 120, new Tuple<int, int>(corruptedMatthiasEnd, corruptedMatthiasEnd + 100000), "rgba(0, 0, 0, 0.3)", wellPosition));
                        }
                    }
                    // Well of profane
                    List<CombatItem> wellMatthias = getFilteredList(log, 34450, getInstid());
                    int wellMatthiasStart = 0;
                    int wellMatthiasEnd = 0;
                    foreach (CombatItem c in wellMatthias)
                    {
                        if (c.isBuffremove() == ParseEnum.BuffRemove.None)
                        {
                            wellMatthiasStart = (int)(c.getTime() - log.getBossData().getFirstAware());
                        }
                        else
                        {
                            wellMatthiasEnd = (int)(c.getTime() - log.getBossData().getFirstAware());
                            replay.addCircleActor(new FollowingCircle(false, 0, 180, new Tuple<int, int>(wellMatthiasStart, wellMatthiasEnd), "rgba(150, 255, 80, 0.5)"));
                            replay.addCircleActor(new FollowingCircle(true, wellMatthiasStart+9000, 180, new Tuple<int, int>(wellMatthiasStart, wellMatthiasEnd), "rgba(150, 255, 80, 0.5)"));
                            Point3D wellPosition = replay.getPositions().FirstOrDefault(x => x.time >= wellMatthiasEnd);
                            replay.addCircleActor(new ImmobileCircle(true, 0, 300, new Tuple<int, int>(wellMatthiasEnd, wellMatthiasEnd + 90000), "rgba(255, 0, 50, 0.5)", wellPosition));
                        }
                    }
                    // Sacrifice
                    List<CombatItem> sacrificeMatthias = getFilteredList(log, 34442, getInstid());
                    int sacrificeMatthiasStart = 0;
                    int sacrificeMatthiasEnd = 0;
                    foreach (CombatItem c in sacrificeMatthias)
                    {
                        if (c.isBuffremove() == ParseEnum.BuffRemove.None)
                        {
                            sacrificeMatthiasStart = (int)(c.getTime() - log.getBossData().getFirstAware());
                        }
                        else
                        {
                            sacrificeMatthiasEnd = (int)(c.getTime() - log.getBossData().getFirstAware());
                            replay.addCircleActor(new FollowingCircle(true, 0, 120, new Tuple<int, int>(sacrificeMatthiasStart, sacrificeMatthiasEnd), "rgba(0, 150, 250, 0.5)"));
                            replay.addCircleActor(new FollowingCircle(true, sacrificeMatthiasStart + 10000, 120, new Tuple<int, int>(sacrificeMatthiasStart, sacrificeMatthiasEnd), "rgba(0, 150, 250, 0.5)"));
                        }
                    }
                    break;
                // KC
                case ParseEnum.BossIDS.KeepConstruct:
                    // bombs
                    break;
                // Xera
                case ParseEnum.BossIDS.Xera:
                    break;
                // Cairn
                case ParseEnum.BossIDS.Cairn:
                    // shared agony
                    List<CombatItem> agony = getFilteredList(log, 38049, getInstid());
                    int agonyStart = 0;
                    int agonyEnd = 0;
                    foreach (CombatItem c in agony)
                    {
                        if (c.isBuffremove() == ParseEnum.BuffRemove.None)
                        {
                            agonyStart = (int)(c.getTime() - log.getBossData().getFirstAware());
                        }
                        else
                        {
                            agonyEnd = (int)(c.getTime() - log.getBossData().getFirstAware());
                            replay.addCircleActor(new FollowingCircle(false, 0, 180, new Tuple<int, int>(agonyStart, agonyEnd), "rgba(255, 0, 0, 0.5)"));
                        }
                    }
                    break;
                // MO
                case ParseEnum.BossIDS.MursaatOverseer:
                    break;
                // Samarog
                case ParseEnum.BossIDS.Samarog:
                    // big bomb
                    List<CombatItem> bigbomb = getFilteredList(log, 37966, getInstid());
                    int bigStart = 0;
                    int bigEnd = 0;
                    foreach (CombatItem c in bigbomb)
                    {
                        if (c.isBuffremove() == ParseEnum.BuffRemove.None)
                        {
                            bigStart = (int)(c.getTime() - log.getBossData().getFirstAware());
                        }
                        else
                        {
                            bigEnd = (int)(c.getTime() - log.getBossData().getFirstAware());
                            replay.addCircleActor(new FollowingCircle(true, 0, 300, new Tuple<int, int>(bigStart, bigEnd), "rgba(150, 80, 0, 0.2)"));
                            replay.addCircleActor(new FollowingCircle(true, bigEnd, 300, new Tuple<int, int>(bigStart, bigEnd), "rgba(150, 80, 0, 0.2)"));
                        }
                    }
                    // small bomb
                    List<CombatItem> smallbomb = getFilteredList(log, 38247, getInstid());
                    int smallStart = 0;
                    int smallEnd = 0;
                    foreach (CombatItem c in smallbomb)
                    {
                        if (c.isBuffremove() == ParseEnum.BuffRemove.None)
                        {
                            smallStart = (int)(c.getTime() - log.getBossData().getFirstAware());
                        }
                        else
                        {
                            smallEnd = (int)(c.getTime() - log.getBossData().getFirstAware());
                            replay.addCircleActor(new FollowingCircle(true, 0, 80, new Tuple<int, int>(smallStart, smallEnd), "rgba(80, 150, 0, 0.3)"));
                        }
                    }
                    // fixated
                    List<CombatItem> fixatedSam = getFilteredList(log, 37868, getInstid());
                    int fixatedSamStart = 0;
                    int fixatedSamEnd = 0;
                    foreach (CombatItem c in fixatedSam)
                    {
                        if (c.isBuffremove() == ParseEnum.BuffRemove.None)
                        {
                            fixatedSamStart = (int)(c.getTime() - log.getBossData().getFirstAware());
                        }
                        else
                        {
                            fixatedSamEnd = (int)(c.getTime() - log.getBossData().getFirstAware());
                            replay.addCircleActor(new FollowingCircle(true, 0, 80, new Tuple<int, int>(fixatedSamStart, fixatedSamEnd), "rgba(255, 80, 255, 0.3)"));
                        }
                    }
                    break;
                // Deimos
                case ParseEnum.BossIDS.Deimos:
                    // teleport zone
                    List<CombatItem> tpDeimos = getFilteredList(log, 37730, getInstid());
                    int tpStart = 0;
                    int tpEnd = 0;
                    foreach (CombatItem c in tpDeimos)
                    {
                        if (c.isBuffremove() == ParseEnum.BuffRemove.None)
                        {
                            tpStart = (int)(c.getTime() - log.getBossData().getFirstAware());
                        }
                        else
                        {
                            tpEnd = (int)(c.getTime() - log.getBossData().getFirstAware());
                            replay.addCircleActor(new FollowingCircle(true, 0, 180, new Tuple<int, int>(tpStart, tpEnd), "rgba(0, 150, 0, 0.3)"));
                            replay.addCircleActor(new FollowingCircle(true, tpEnd, 180, new Tuple<int, int>(tpStart, tpEnd), "rgba(0, 150, 0, 0.3)"));
                        }
                    }
                    break;
                // SH
                case ParseEnum.BossIDS.SoullessHorror:
                    break;
                // Dhuum
                case ParseEnum.BossIDS.Dhuum:
                    // spirit transform
                    List<CombatItem> spiritTransform = log.getBoonData().Where(x => x.getDstInstid() == getInstid() && x.getSkillID() == 46950 && x.isBuffremove() == ParseEnum.BuffRemove.None).ToList();
                    foreach (CombatItem c in spiritTransform)
                    {
                        int duration = 15000;
                        int start = (int)(c.getTime() - log.getBossData().getFirstAware());
                        if (log.getBossData().getHealthOverTime().FirstOrDefault(x => x.X > start).Y < 1050)
                        {
                            duration = 30000;
                        }
                        CombatItem removedBuff = log.getBoonData().FirstOrDefault(x => x.getSrcInstid() == getInstid() && x.getSkillID() == 48281 && x.isBuffremove() == ParseEnum.BuffRemove.All && x.getTime() > c.getTime() && x.getTime() < c.getTime() + duration);
                        int end = start + duration;
                        if (removedBuff != null)
                        {
                            end = (int)(removedBuff.getTime() - log.getBossData().getFirstAware());
                        }
                        replay.addCircleActor(new FollowingCircle(true, 0, 100, new Tuple<int, int>(start, end), "rgba(0, 50, 200, 0.3)"));
                        replay.addCircleActor(new FollowingCircle(true, start + duration, 100, new Tuple<int, int>(start, end), "rgba(0, 50, 200, 0.5)"));
                    }
                    // bomb
                    List<CombatItem> bombDhuum = getFilteredList(log, 47646, getInstid());
                    int bombDhuumStart = 0;
                    int bombDhuumEnd = 0;
                    foreach (CombatItem c in bombDhuum)
                    {
                        if (c.isBuffremove() == ParseEnum.BuffRemove.None)
                        {
                            bombDhuumStart = (int)(c.getTime() - log.getBossData().getFirstAware());
                        }
                        else
                        {
                            bombDhuumEnd = (int)(c.getTime() - log.getBossData().getFirstAware());
                            replay.addCircleActor(new FollowingCircle(true, 0, 100, new Tuple<int, int>(bombDhuumStart, bombDhuumEnd), "rgba(80, 180, 0, 0.3)"));
                            replay.addCircleActor(new FollowingCircle(true, bombDhuumStart + 13000, 100, new Tuple<int, int>(bombDhuumStart, bombDhuumEnd), "rgba(80, 180, 0, 0.5)"));
                        }
                    }
                    break;
                // MAMA
                case ParseEnum.BossIDS.MAMA:
                    break;
                // Siax
                case ParseEnum.BossIDS.Siax:
                    break;
                // Ensolyss
                case ParseEnum.BossIDS.Ensolyss:
                    break;
                // Skorvald
                case ParseEnum.BossIDS.Skorvald:
                    break;
                // Artsariiv
                case ParseEnum.BossIDS.Artsariiv:
                    break;
                // Arkk
                case ParseEnum.BossIDS.Arkk:
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
