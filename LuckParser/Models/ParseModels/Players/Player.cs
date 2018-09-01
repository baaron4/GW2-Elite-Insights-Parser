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
        private readonly string _account;
        private readonly int _group;
        private long _dcd;//time in ms the player dcd
       
        private readonly List<Tuple<Boon,long>> _consumeList = new List<Tuple<Boon, long>>();
        //weaponslist
        private string[] _weaponsArray;

        // Constructors
        public Player(AgentItem agent, bool noSquad) : base(agent)
        {
            String[] name = agent.GetName().Split('\0');
            _account = name[1];
            _group = noSquad ? 1 : int.Parse(name[2], NumberStyles.Integer, CultureInfo.InvariantCulture);
        }

        // Getters

        public string GetAccount()
        {
            return _account;
        }
    
        public int GetGroup()
        {
            return _group;
        }

        public int GetToughness()
        {
            return Agent.GetToughness();
        }

        public int GetHealing()
        {
            return Agent.GetHealing();
        }

        public int GetCondition()
        {
            return Agent.GetCondition();
        }

        public int GetConcentration()
        {
            return Agent.GetConcentration();
        }
        // Public methods
        public int[] GetCleanses(ParsedLog log, long start, long end) {
            long timeStart = log.GetBossData().GetFirstAware();
            int[] cleanse = { 0, 0 };
            List<Boon> condiList = Boon.GetCondiBoonList();
            foreach (CombatItem c in log.GetCombatList().Where(x=>x.IsStateChange == ParseEnum.StateChange.Normal && x.IsBuff == 1 && x.Time >= (start + timeStart) && x.Time <= (end + timeStart)))
            {
                if (c.IsActivation == ParseEnum.Activation.None)
                {
                    if ((Agent.GetInstid() == c.DstInstid || Agent.GetInstid() == c.DstMasterInstid) && c.IFF == ParseEnum.IFF.Friend && (c.IsBuffRemove != ParseEnum.BuffRemove.None))
                    {
                        long time = c.Time - timeStart;
                        if (time > 0)
                        {
                            if (condiList.Exists(x=>x.GetID() == c.SkillID))
                            {
                                cleanse[0]++;
                                cleanse[1] += c.BuffDmg;
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
                if (cl.GetID() == SkillItem.ResurrectId)
                {
                    reses[0]++;
                    reses[1] += cl.GetActDur();
                }
            }
            return reses;
        }
        public string[] GetWeaponsArray(ParsedLog log)
        {
            if (_weaponsArray == null)
            {
                EstimateWeapons( log);
            }
            return _weaponsArray;
        }
        
        public long GetDC()
        {
            return _dcd;
        }
        public void SetDC(long value)
        {
            _dcd = value;
        }
        public List<Tuple<Boon, long>> GetConsumablesList(ParsedLog log, long start, long end)
        {
            if (_consumeList.Count == 0)
            {
                SetConsumablesList(log);
            }
            return _consumeList.Where(x => x.Item2 >= start && x.Item2 <= end).ToList() ;
        }
        
        // Private Methods
        private void EstimateWeapons(ParsedLog log)
        {
            string[] weapons = new string[4];//first 2 for first set next 2 for second set
            SkillData skillList = log.GetSkillData();
            List<CastLog> casting = GetCastLogs(log, 0, log.GetBossData().GetAwareDuration());      
            int swapped = 0;//4 for first set and 5 for next
            long swappedTime = 0;
            List<CastLog> swaps = casting.Where(x => x.GetID() == SkillItem.WeaponSwapId).Take(2).ToList();
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
                GW2APISkill apiskill = skillList.Get(cl.GetID())?.GetGW2APISkill();
                if (apiskill != null && cl.GetTime() > swappedTime)
                {
                    if (apiskill.type == "Weapon" && apiskill.professions.Count() > 0 && (apiskill.categories == null || (apiskill.categories.Count() == 1 && apiskill.categories[0] == "Phantasm")))
                    {
                        if (apiskill.duel_wield != null)
                        {
                            if (swapped == 4)
                            {
                                weapons[0] = apiskill.weapon_type;
                                weapons[1] = apiskill.duel_wield;
                            }
                            else if (swapped == 5)
                            {
                                weapons[2] = apiskill.weapon_type;
                                weapons[3] = apiskill.duel_wield;
                            }
                        }
                        else if (apiskill.weapon_type == "Greatsword" || apiskill.weapon_type == "Staff" || apiskill.weapon_type == "Rifle" || apiskill.weapon_type == "Longbow" || apiskill.weapon_type == "Shortbow" || apiskill.weapon_type == "Hammer")
                        {
                            if (swapped == 4)
                            {
                                weapons[0] = apiskill.weapon_type;
                                weapons[1] = "2Hand";
                            }
                            else if (swapped == 5)
                            {
                                weapons[2] = apiskill.weapon_type;
                                weapons[3] = "2Hand";
                            }
                        }//2 handed
                        else if (apiskill.weapon_type == "Focus" || apiskill.weapon_type == "Shield" || apiskill.weapon_type == "Torch" || apiskill.weapon_type == "Warhorn")
                        {
                            if (swapped == 4)
                            {

                                weapons[1] = apiskill.weapon_type;
                            }
                            else if (swapped == 5)
                            {

                                weapons[3] = apiskill.weapon_type;
                            }
                        }//OffHand
                        else if (apiskill.weapon_type == "Axe" || apiskill.weapon_type == "Dagger" || apiskill.weapon_type == "Mace" || apiskill.weapon_type == "Pistol" || apiskill.weapon_type == "Sword" || apiskill.weapon_type == "Scepter")
                        {
                            if (apiskill.slot == "Weapon_1" || apiskill.slot == "Weapon_2" || apiskill.slot == "Weapon_3")
                            {
                                if (swapped == 4)
                                {

                                    weapons[0] = apiskill.weapon_type;
                                }
                                else if (swapped == 5)
                                {

                                    weapons[2] = apiskill.weapon_type;
                                }
                            }
                            if (apiskill.slot == "Weapon_4" || apiskill.slot == "Weapon_5")
                            {
                                if (swapped == 4)
                                {

                                    weapons[1] = apiskill.weapon_type;
                                }
                                else if (swapped == 5)
                                {

                                    weapons[3] = apiskill.weapon_type;
                                }
                            }
                        }// 1 handed
                    }

                }
                else if (cl.GetID() == SkillItem.WeaponSwapId)
                {
                    //wepswap  
                    swapped = cl.GetExpDur();
                    swappedTime = cl.GetTime();
                    continue;
                }
            }
            _weaponsArray = weapons;
        }    
        
        protected override void SetDamagetakenLogs(ParsedLog log)
        {
            long timeStart = log.GetBossData().GetFirstAware();               
            foreach (CombatItem c in log.GetDamageTakenData()) {
                if (Agent.GetInstid() == c.DstInstid && c.Time > log.GetBossData().GetFirstAware() && c.Time < log.GetBossData().GetLastAware()) {//selecting player as target
                    long time = c.Time - timeStart;
                    AddDamageTakenLog(time, c);
                }
            }
        }  
        private void SetConsumablesList(ParsedLog log)
        {
            List<Boon> foodBoon = Boon.GetFoodList();
            List<Boon> utilityBoon = Boon.GetUtilityList();
            long timeStart = log.GetBossData().GetFirstAware();
            long fightDuration = log.GetBossData().GetLastAware() - timeStart;
            foreach (CombatItem c in log.GetBoonData())
            {
                if ( c.IsBuffRemove != ParseEnum.BuffRemove.None || (c.IsBuff != 18 && c.IsBuff != 1) || Agent.GetInstid() != c.DstInstid)
                {
                    continue;
                }
                var food = foodBoon.FirstOrDefault(x => x.GetID() == c.SkillID);
                var utility = utilityBoon.FirstOrDefault(x => x.GetID() == c.SkillID);
                if (food == null && utility == null)
                {
                    continue;
                }
                long time = 0;
                if (c.IsBuff != 18)
                {
                    time = c.Time - timeStart;
                }
                if (time <= fightDuration)
                {
                    _consumeList.Add(new Tuple<Boon, long>(food ?? utility, time)); 
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
            status = status.OrderBy(x => x.Time).ToList();
            List<Tuple<long, long>> dead = new List<Tuple<long, long>>();
            List<Tuple<long, long>> down = new List<Tuple<long, long>>();
            List<Tuple<long, long>> dc = new List<Tuple<long, long>>();
            for (var i = 0; i < status.Count -1;i++)
            {
                CombatItem cur = status[i];
                CombatItem next = status[i + 1];
                if (cur.IsStateChange.IsDown())
                {
                    down.Add(new Tuple<long, long>(cur.Time - log.GetBossData().GetFirstAware(), next.Time - log.GetBossData().GetFirstAware()));
                } else if (cur.IsStateChange.IsDead())
                {
                    dead.Add(new Tuple<long, long>(cur.Time - log.GetBossData().GetFirstAware(), next.Time - log.GetBossData().GetFirstAware()));
                } else if (cur.IsStateChange.IsDespawn())
                {
                    dc.Add(new Tuple<long, long>(cur.Time - log.GetBossData().GetFirstAware(), next.Time - log.GetBossData().GetFirstAware()));
                }
            }
            // check last value
            if (status.Count > 0)
            {
                CombatItem cur = status.Last();
                if (cur.IsStateChange.IsDown())
                {
                    down.Add(new Tuple<long, long>(cur.Time - log.GetBossData().GetFirstAware(), log.GetBossData().GetAwareDuration()));
                }
                else if (cur.IsStateChange.IsDead())
                {
                    dead.Add(new Tuple<long, long>(cur.Time - log.GetBossData().GetFirstAware(), log.GetBossData().GetAwareDuration()));
                }
                else if (cur.IsStateChange.IsDespawn())
                {
                    dc.Add(new Tuple<long, long>(cur.Time - log.GetBossData().GetFirstAware(), log.GetBossData().GetAwareDuration()));
                }
            }
            Replay.SetStatus(down, dead, dc);
            // Boss related stuff
            log.GetBossData().GetBossBehavior().GetAdditionalPlayerData(Replay, this, log);
        }

        protected override void SetCombatReplayIcon(ParsedLog log)
        {
            Replay.SetIcon(HTMLHelper.GetLink(GetProf()));
        }

        public void AddMechanics(ParsedLog log)
        {
            MechanicData mechData = log.GetMechanicData();
            BossData bossData = log.GetBossData();
            CombatData combatData = log.GetCombatData();
            List<Mechanic> bossMechanics = bossData.GetBossBehavior().GetMechanics();
            long start = bossData.GetFirstAware();
            long end = bossData.GetLastAware();
            // Player status
            List<Mechanic> playerStatus = bossMechanics.Where(x => x.GetMechType() == Mechanic.MechType.PlayerStatus).ToList();
            foreach (Mechanic mech in playerStatus)
            {
                List<CombatItem> toUse = new List<CombatItem>();
                switch (mech.GetSkill()) {
                    case -2:
                        toUse = combatData.GetStates(GetInstid(), ParseEnum.StateChange.ChangeDead, start, end);                 
                        break;
                    case -3:
                        toUse = combatData.GetStates(GetInstid(), ParseEnum.StateChange.ChangeDown, start, end);
                        break;
                    case SkillItem.ResurrectId:
                        toUse = log.GetCastData().Where(x => x.SkillID == SkillItem.ResurrectId && x.SrcInstid == GetInstid() && x.IsActivation.IsCasting()).ToList();
                        break;
                }
                foreach (CombatItem pnt in toUse)
                {
                    mechData[mech].Add(new MechanicLog(pnt.Time - start, mech, this));
                }

            }
            //Player hit
            List<DamageLog> dls = GetDamageTakenLogs(log, 0, bossData.GetAwareDuration());
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
                        mechData[mech].Add(new MechanicLog(dLog.GetTime(), mech, this));

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
                    if (condition != null && !condition(c.Value))
                    {
                        continue;
                    }
                    if (mech.GetMechType() == Mechanic.MechType.PlayerBoonRemove)
                    {
                        if (c.SkillID == mech.GetSkill() && c.IsBuffRemove == ParseEnum.BuffRemove.Manual && GetInstid() == c.SrcInstid)
                        {
                            mechData[mech].Add(new MechanicLog(c.Time - start, mech, this));
                        }
                    } else
                    {

                        if (c.SkillID == mech.GetSkill() && c.IsBuffRemove == ParseEnum.BuffRemove.None && GetInstid() == c.DstInstid)
                        {
                            mechData[mech].Add(new MechanicLog(c.Time - start, mech, this));
                            if (mech.GetMechType() == Mechanic.MechType.PlayerOnPlayer)
                            {
                                mechData[mech].Add(new MechanicLog(c.Time - start, mech, log.GetPlayerList().FirstOrDefault(x => x.GetInstid() == c.SrcInstid)));
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
                        mechData[mech].Add(new MechanicLog(dl.GetTime(), mech, this));
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
