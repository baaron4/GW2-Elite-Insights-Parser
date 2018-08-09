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

        protected override void setAdditionalCombatReplayData(ParsedLog log, int pollingRate)
        {
            // Down and deads
            List<CombatItem> status = log.getCombatData().getStates(getInstid(), ParseEnum.StateChange.ChangeDown, log.getBossData().getFirstAware(), log.getBossData().getLastAware());
            status.AddRange(log.getCombatData().getStates(getInstid(), ParseEnum.StateChange.ChangeUp, log.getBossData().getFirstAware(), log.getBossData().getLastAware()));
            status.AddRange(log.getCombatData().getStates(getInstid(), ParseEnum.StateChange.ChangeDead, log.getBossData().getFirstAware(), log.getBossData().getLastAware()));
            status.AddRange(log.getCombatData().getStates(getInstid(), ParseEnum.StateChange.Spawn, log.getBossData().getFirstAware(), log.getBossData().getLastAware()));
            status.AddRange(log.getCombatData().getStates(getInstid(), ParseEnum.StateChange.Despawn, log.getBossData().getFirstAware(), log.getBossData().getLastAware()));
            status = status.OrderBy(x => x.getTime()).ToList();
            List<Tuple<long, long>> dead = new List<Tuple<long, long>>();
            List<Tuple<long, long>> down = new List<Tuple<long, long>>();
            List<Tuple<long, long>> dc = new List<Tuple<long, long>>();
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
                } else if (cur.isStateChange() == ParseEnum.StateChange.Despawn)
                {
                    dc.Add(new Tuple<long, long>(cur.getTime() - log.getBossData().getFirstAware(), next.getTime() - log.getBossData().getFirstAware()));
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
                else if (cur.isStateChange() == ParseEnum.StateChange.Despawn)
                {
                    dc.Add(new Tuple<long, long>(cur.getTime() - log.getBossData().getFirstAware(), log.getBossData().getAwareDuration()));
                }
            }
            replay.setStatus(down, dead, dc);
            // Boss related stuff
            log.getBossData().getBossBehavior().getAdditionalPlayerData(replay, this, log);
        }

        protected override void setCombatReplayIcon(ParsedLog log)
        {
            replay.setIcon(HTMLHelper.GetLink(getProf()));
        }

        public override void addMechanics(ParsedLog log)
        {
            MechanicData mech_data = log.getMechanicData();
            BossData boss_data = log.getBossData();
            CombatData combat_data = log.getCombatData();
            List<Mechanic> bossMechanics = boss_data.getBossBehavior().getMechanics();
            SkillData skill_data = log.getSkillData();
            // downs
            List<CombatItem> down = combat_data.getStates(getInstid(), ParseEnum.StateChange.ChangeDown, boss_data.getFirstAware(), boss_data.getLastAware());
            foreach (CombatItem pnt in down)
            {
                mech_data.AddItem(new MechanicLog((long)Math.Round((pnt.getTime() - boss_data.getFirstAware()) / 1000f), 0, "DOWN", 0, this, mech_data.GetPLoltyShape("DOWN")));
            }
            // deads
            List<CombatItem> dead = combat_data.getStates(getInstid(), ParseEnum.StateChange.ChangeDead, boss_data.getFirstAware(), boss_data.getLastAware());
            foreach (CombatItem pnt in dead)
            {
                mech_data.AddItem(new MechanicLog((long)Math.Round((pnt.getTime() - boss_data.getFirstAware()) / 1000f), 0, "DEAD", 0, this, mech_data.GetPLoltyShape("DEAD")));
            }
            //Player hit
            List<DamageLog> dls = getDamageTakenLogs(log, 0, boss_data.getAwareDuration());
            List<Mechanic> skillOnPlayer = bossMechanics.Where(x => x.GetMechType() == Mechanic.MechType.SkillOnPlayer).ToList();
            foreach (Mechanic mech in skillOnPlayer)
            {
                MechanicLog prevMech = null;
                foreach (DamageLog dLog in dls)
                {
                    string name = skill_data.getName(dLog.getID());
                    if (dLog.getID() == mech.GetSkill() && dLog.getResult().IsHit())
                    {
                        //Prevent multi hit attacks form multi registering
                        if (prevMech != null && dLog.getTime() == prevMech.GetTime())
                        {
                            continue;
                        }
                        prevMech = new MechanicLog(dLog.getTime(), dLog.getID(), mech.GetName(), dLog.getDamage(), this, mech.GetPlotly());
                        mech_data.AddItem(prevMech);

                    }
                }
            }
            // Player boon
            List<Mechanic> playerBoon = bossMechanics.Where(x => x.GetMechType() == Mechanic.MechType.PlayerBoon).ToList();
            playerBoon.AddRange(bossMechanics.Where(x => x.GetMechType() == Mechanic.MechType.PlayerOnPlayer));
            foreach (Mechanic mech in playerBoon)
            {
                foreach (CombatItem c in log.getBoonData())
                {
                    if (c.getSkillID() == mech.GetSkill() && c.getValue() > 0 && c.isBuffremove() == ParseEnum.BuffRemove.None && c.getResult().IsHit() && getInstid() == c.getDstInstid())
                    {
                        String name = skill_data.getName(c.getSkillID());
                        mech_data.AddItem(new MechanicLog(c.getTime() - boss_data.getFirstAware(), c.getSkillID(), mech.GetName(), c.getValue(), this, mech.GetPlotly()));
                    }
                }
            }
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
