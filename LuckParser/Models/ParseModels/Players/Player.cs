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
       
        private List<Tuple<Boon,long>> consumeList = new List<Tuple<Boon, long>>();
        //weaponslist
        private string[] weapons_array;

        // Constructors
        public Player(AgentItem agent, bool noSquad) : base(agent)
        {
            String[] name = agent.GetName().Split('\0');
            account = name[1];
            group = noSquad ? 1 : int.Parse(name[2], NumberStyles.Integer, CultureInfo.InvariantCulture);
        }

        // Getters

        public string GetAccount()
        {
            return account;
        }
    
        public int GetGroup()
        {
            return group;
        }

        public int GetToughness()
        {
            return agent.GetToughness();
        }

        public int GetHealing()
        {
            return agent.GetHealing();
        }

        public int GetCondition()
        {
            return agent.GetCondition();
        }

        public int GetConcentration()
        {
            return agent.GetConcentration();
        }
        // Public methods
        public int[] GetCleanses(ParsedLog log, long start, long end) {
            long time_start = log.GetBossData().GetFirstAware();
            int[] cleanse = { 0, 0 };
            foreach (CombatItem c in log.GetCombatList().Where(x=>x.IsStateChange() == ParseEnum.StateChange.Normal && x.IsBuff() == 1 && x.GetTime() >= (start + time_start) && x.GetTime() <= (end + time_start)))
            {
                if (c.IsActivation() == ParseEnum.Activation.None)
                {
                    if ((agent.GetInstid() == c.GetDstInstid() || agent.GetInstid() == c.GetDstMasterInstid()) && c.GetIFF() == ParseEnum.IFF.Friend && (c.IsBuffremove() != ParseEnum.BuffRemove.None))
                    {
                        long time = c.GetTime() - time_start;
                        if (time > 0)
                        {
                            if (Boon.GetCondiBoonList().Exists(x=>x.GetID() == c.GetSkillID()))
                            {
                                cleanse[0]++;
                                cleanse[1] += c.GetBuffDmg();
                            }
                        }
                    }
                }
            }
            return cleanse;
        }
        public int[] GetReses(ParsedLog log, long start, long end)
        {
            List<CastLog> cls = GetCastLogs(log, start, end);
            int[] reses = { 0, 0 };
            foreach (CastLog cl in cls) {
                if (cl.GetID() == 1066)
                {
                    reses[0]++;
                    reses[1] += cl.GetActDur();
                }
            }
            return reses;
        }
        public string[] GetWeaponsArray(ParsedLog log)
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
        public List<Tuple<Boon, long>> GetConsumablesList(ParsedLog log, long start, long end)
        {
            if (consumeList.Count == 0)
            {
                SetConsumablesList(log);
            }
            return consumeList.Where(x => x.Item2 >= start && x.Item2 <= end).ToList() ;
        }
        
        // Private Methods
        private void EstimateWeapons(ParsedLog log)
        {
            string[] weapons = new string[4];//first 2 for first set next 2 for second set
            SkillData s_list = log.GetSkillData();
            List<CastLog> casting = GetCastLogs(log, 0, log.GetBossData().GetAwareDuration());      
            int swapped = 0;//4 for first set and 5 for next
            long swappedTime = 0;
            List<CastLog> swaps = casting.Where(x => x.GetID() == -2).Take(2).ToList();
            // If the player never swapped, assume they are on their first set
            if (swaps.Count == 0)
            {
                swapped = 4;
            }
            // if the player swapped once, check on which set they started
            else if (swaps.Count == 1)
            {
                swapped = swaps.First().GetExpDur() == 4 ? 5 : 4;
            }
            foreach (CastLog cl in casting)
            {
                GW2APISkill apiskill = null;
                SkillItem skill = s_list.FirstOrDefault(x => x.GetID() == cl.GetID());
                if (skill != null)
                {
                    apiskill = skill.GetGW2APISkill();
                }
                if (apiskill != null && cl.GetTime() > swappedTime)
                {
                    if (apiskill.Type == "Weapon" && apiskill.Professions.Count() > 0 && (apiskill.Categories == null || (apiskill.Categories.Count() == 1 && apiskill.Categories[0] == "Phantasm")))
                    {
                        if (apiskill.Weapon_type == "Greatsword" || apiskill.Weapon_type == "Staff" || apiskill.Weapon_type == "Rifle" || apiskill.Weapon_type == "Longbow" || apiskill.Weapon_type == "Shortbow" || apiskill.Weapon_type == "Hammer")
                        {
                            if (swapped == 4)
                            {
                                weapons[0] = apiskill.Weapon_type;
                                weapons[1] = "2Hand";
                                continue;
                            }
                            else if (swapped == 5)
                            {
                                weapons[2] = apiskill.Weapon_type;
                                weapons[3] = "2Hand";
                                continue;
                            }
                            continue;
                        }//2 handed
                        if (apiskill.Weapon_type == "Focus" || apiskill.Weapon_type == "Shield" || apiskill.Weapon_type == "Torch" || apiskill.Weapon_type == "Warhorn")
                        {
                            if (swapped == 4)
                            {

                                weapons[1] = apiskill.Weapon_type;
                                continue;
                            }
                            else if (swapped == 5)
                            {

                                weapons[3] = apiskill.Weapon_type;
                                continue;
                            }
                            continue;
                        }//OffHand
                        if (apiskill.Weapon_type == "Axe" || apiskill.Weapon_type == "Dagger" || apiskill.Weapon_type == "Mace" || apiskill.Weapon_type == "Pistol" || apiskill.Weapon_type == "Sword" || apiskill.Weapon_type == "Scepter")
                        {
                            if (apiskill.Slot == "Weapon_1" || apiskill.Slot == "Weapon_2" || apiskill.Slot == "Weapon_3")
                            {
                                if (swapped == 4)
                                {

                                    weapons[0] = apiskill.Weapon_type;
                                    continue;
                                }
                                else if (swapped == 5)
                                {

                                    weapons[2] = apiskill.Weapon_type;
                                    continue;
                                }
                                continue;
                            }
                            if (apiskill.Slot == "Weapon_4" || apiskill.Slot == "Weapon_5")
                            {
                                if (swapped == 4)
                                {

                                    weapons[1] = apiskill.Weapon_type;
                                    continue;
                                }
                                else if (swapped == 5)
                                {

                                    weapons[3] = apiskill.Weapon_type;
                                    continue;
                                }
                                continue;
                            }
                        }// 1 handed
                    }

                }
                else if (cl.GetID() == -2)
                {
                    //wepswap  
                    swapped = cl.GetExpDur();
                    swappedTime = cl.GetTime();
                    continue;
                }
            }
            weapons_array = weapons;
        }    
        
        protected override void SetDamagetakenLogs(ParsedLog log)
        {
            long time_start = log.GetBossData().GetFirstAware();               
            foreach (CombatItem c in log.GetDamageTakenData()) {
                if (agent.GetInstid() == c.GetDstInstid() && c.GetTime() > log.GetBossData().GetFirstAware() && c.GetTime() < log.GetBossData().GetLastAware()) {//selecting player as target
                    long time = c.GetTime() - time_start;
                    AddDamageTakenLog(time, c);
                }
            }
        }  
        private void SetConsumablesList(ParsedLog log)
        {
            List<Boon> foodBoon = Boon.GetFoodList();
            List<Boon> utilityBoon = Boon.GetUtilityList();
            long time_start = log.GetBossData().GetFirstAware();
            long fight_duration = log.GetBossData().GetLastAware() - time_start;
            foreach (CombatItem c in log.GetBoonData())
            {
                if ( c.IsBuffremove() != ParseEnum.BuffRemove.None || (c.IsBuff() != 18 && c.IsBuff() != 1) || agent.GetInstid() != c.GetDstInstid())
                {
                    continue;
                }
                var food = foodBoon.FirstOrDefault(x => x.GetID() == c.GetSkillID());
                var utility = utilityBoon.FirstOrDefault(x => x.GetID() == c.GetSkillID());
                if (food == null && utility == null)
                {
                    continue;
                }
                long time = c.GetTime() - time_start;
                if (time <= fight_duration)
                {
                    consumeList.Add(new Tuple<Boon, long>(food ?? utility, time)); 
                }
            }
        }

        protected override void SetAdditionalCombatReplayData(ParsedLog log, int pollingRate)
        {
            // Down and deads
            List<CombatItem> status = log.GetCombatData().GetStates(GetInstid(), ParseEnum.StateChange.ChangeDown, log.GetBossData().GetFirstAware(), log.GetBossData().GetLastAware());
            status.AddRange(log.GetCombatData().GetStates(GetInstid(), ParseEnum.StateChange.ChangeUp, log.GetBossData().GetFirstAware(), log.GetBossData().GetLastAware()));
            status.AddRange(log.GetCombatData().GetStates(GetInstid(), ParseEnum.StateChange.ChangeDead, log.GetBossData().GetFirstAware(), log.GetBossData().GetLastAware()));
            status.AddRange(log.GetCombatData().GetStates(GetInstid(), ParseEnum.StateChange.Spawn, log.GetBossData().GetFirstAware(), log.GetBossData().GetLastAware()));
            status.AddRange(log.GetCombatData().GetStates(GetInstid(), ParseEnum.StateChange.Despawn, log.GetBossData().GetFirstAware(), log.GetBossData().GetLastAware()));
            status = status.OrderBy(x => x.GetTime()).ToList();
            List<Tuple<long, long>> dead = new List<Tuple<long, long>>();
            List<Tuple<long, long>> down = new List<Tuple<long, long>>();
            List<Tuple<long, long>> dc = new List<Tuple<long, long>>();
            for (var i = 0; i < status.Count -1;i++)
            {
                CombatItem cur = status[i];
                CombatItem next = status[i + 1];
                if (cur.IsStateChange().IsDown())
                {
                    down.Add(new Tuple<long, long>(cur.GetTime() - log.GetBossData().GetFirstAware(), next.GetTime() - log.GetBossData().GetFirstAware()));
                } else if (cur.IsStateChange().IsDead())
                {
                    dead.Add(new Tuple<long, long>(cur.GetTime() - log.GetBossData().GetFirstAware(), next.GetTime() - log.GetBossData().GetFirstAware()));
                } else if (cur.IsStateChange().IsDespawn())
                {
                    dc.Add(new Tuple<long, long>(cur.GetTime() - log.GetBossData().GetFirstAware(), next.GetTime() - log.GetBossData().GetFirstAware()));
                }
            }
            // check last value
            if (status.Count > 0)
            {
                CombatItem cur = status.Last();
                if (cur.IsStateChange().IsDown())
                {
                    down.Add(new Tuple<long, long>(cur.GetTime() - log.GetBossData().GetFirstAware(), log.GetBossData().GetAwareDuration()));
                }
                else if (cur.IsStateChange().IsDead())
                {
                    dead.Add(new Tuple<long, long>(cur.GetTime() - log.GetBossData().GetFirstAware(), log.GetBossData().GetAwareDuration()));
                }
                else if (cur.IsStateChange().IsDespawn())
                {
                    dc.Add(new Tuple<long, long>(cur.GetTime() - log.GetBossData().GetFirstAware(), log.GetBossData().GetAwareDuration()));
                }
            }
            replay.SetStatus(down, dead, dc);
            // Boss related stuff
            log.GetBossData().GetBossBehavior().GetAdditionalPlayerData(replay, this, log);
        }

        protected override void SetCombatReplayIcon(ParsedLog log)
        {
            replay.SetIcon(HTMLHelper.GetLink(GetProf()));
        }

        public override void AddMechanics(ParsedLog log)
        {
            MechanicData mech_data = log.GetMechanicData();
            BossData boss_data = log.GetBossData();
            CombatData combat_data = log.GetCombatData();
            List<Mechanic> bossMechanics = boss_data.GetBossBehavior().GetMechanics();
            long start = boss_data.GetFirstAware();
            long end = boss_data.GetLastAware();
            // Player status
            List<Mechanic> playerStatus = bossMechanics.Where(x => x.GetMechType() == Mechanic.MechType.PlayerStatus).ToList();
            foreach (Mechanic mech in playerStatus)
            {
                List<CombatItem> toUse = new List<CombatItem>();
                switch (mech.GetSkill()) {
                    case -2:
                        toUse = combat_data.GetStates(GetInstid(), ParseEnum.StateChange.ChangeDead, start, end);                 
                        break;
                    case -3:
                        toUse = combat_data.GetStates(GetInstid(), ParseEnum.StateChange.ChangeDown, start, end);
                        break;
                    case 1066:
                        toUse = log.GetCastData().Where(x => x.GetSkillID() == 1066 && x.GetSrcInstid() == GetInstid() && x.IsActivation().IsCasting()).ToList();
                        break;
                    default:
                        break;
                }
                foreach (CombatItem pnt in toUse)
                {
                    mech_data[mech].Add(new MechanicLog(pnt.GetTime() - start, mech, this));
                }

            }
            //Player hit
            List<DamageLog> dls = GetDamageTakenLogs(log, 0, boss_data.GetAwareDuration());
            List<Mechanic> skillOnPlayer = bossMechanics.Where(x => x.GetMechType() == Mechanic.MechType.SkillOnPlayer).ToList();
            foreach (Mechanic mech in skillOnPlayer)
            {
                Mechanic.SpecialCondition condition = mech.GetSpecialCondition();
                foreach (DamageLog dLog in dls)
                {
                    if (condition != null && !condition(dLog.GetDamage()))
                    {
                        continue;
                    }
                    if (dLog.GetID() == mech.GetSkill() && dLog.GetResult().IsHit())
                    {
                        mech_data[mech].Add(new MechanicLog(dLog.GetTime(), mech, this));

                    }
                }
            }
            // Player boon
            List<Mechanic> playerBoon = bossMechanics.Where(x => x.GetMechType() == Mechanic.MechType.PlayerBoon || x.GetMechType() == Mechanic.MechType.PlayerOnPlayer || x.GetMechType() == Mechanic.MechType.PlayerBoonRemove).ToList();
            foreach (Mechanic mech in playerBoon)
            {
                Mechanic.SpecialCondition condition = mech.GetSpecialCondition();
                foreach (CombatItem c in log.GetBoonData())
                {
                    if (condition != null && !condition(c.GetValue()))
                    {
                        continue;
                    }
                    if (mech.GetMechType() == Mechanic.MechType.PlayerBoonRemove)
                    {
                        if (c.GetSkillID() == mech.GetSkill() && c.IsBuffremove() == ParseEnum.BuffRemove.Manual && GetInstid() == c.GetSrcInstid())
                        {
                            mech_data[mech].Add(new MechanicLog(c.GetTime() - start, mech, this));
                        }
                    } else
                    {

                        if (c.GetSkillID() == mech.GetSkill() && c.IsBuffremove() == ParseEnum.BuffRemove.None && GetInstid() == c.GetDstInstid())
                        {
                            mech_data[mech].Add(new MechanicLog(c.GetTime() - start, mech, this));
                            if (mech.GetMechType() == Mechanic.MechType.PlayerOnPlayer)
                            {
                                mech_data[mech].Add(new MechanicLog(c.GetTime() - start, mech, log.GetPlayerList().FirstOrDefault(x => x.GetInstid() == c.GetSrcInstid())));
                            }
                        }
                    }
                }
            }
            // Hitting enemy
            List<Mechanic> enemyHit = bossMechanics.Where(x => x.GetMechType() == Mechanic.MechType.HitOnEnemy).ToList();
            foreach (Mechanic mech in enemyHit)
            {
                Mechanic.SpecialCondition condition = mech.GetSpecialCondition();
                List<AgentItem> agents = log.GetAgentData().GetAgents((ushort)mech.GetSkill());
                foreach (AgentItem a in agents)
                {
                    foreach (DamageLog dl in GetDamageLogs(0,log,0,log.GetBossData().GetAwareDuration()))
                    {
                        if (dl.GetDstInstidt() != a.GetInstid() || dl.IsCondi() > 0 || dl.GetTime() < a.GetFirstAware() - start || dl.GetTime() > a.GetLastAware() - start || (condition != null && !condition(dl.GetDamage())))
                        {
                            continue;
                        }
                        mech_data[mech].Add(new MechanicLog(dl.GetTime(), mech, this));
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
