using LuckParser.Controllers;
using LuckParser.Models.DataModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

using System.Globalization;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class Player : AbstractMasterPlayer
    {
        // Fields
        public readonly string Account;
        public readonly int Group;
        public long Disconnected { get; set; }//time in ms the player dcd
       
        private readonly List<Tuple<Boon,long,int>> _consumeList = new List<Tuple<Boon, long,int>>();
        //weaponslist
        private string[] _weaponsArray;

        // Constructors
        public Player(AgentItem agent, bool noSquad) : base(agent)
        {
            String[] name = agent.Name.Split('\0');
            Account = name[1];
            Group = noSquad ? 1 : int.Parse(name[2], NumberStyles.Integer, CultureInfo.InvariantCulture);
        }
        
        // Public methods
        public int[] GetCleanses(ParsedLog log, int phaseIndex) {
            int[] cleanse = { 0, 0 };
            foreach (Player p in log.PlayerList)
            {
                foreach(List<long> list in p.GetCondiCleanse(log,phaseIndex, InstID).Values)
                {
                    cleanse[0] += list.Count;
                    cleanse[1] += (int)list.Sum();
                }
            }
            return cleanse;
        }
        public int[] GetReses(ParsedLog log, long start, long end)
        {
            List<CastLog> cls = GetCastLogs(log, start, end);
            int[] reses = { 0, 0 };
            foreach (CastLog cl in cls) {
                if (cl.SkillId == SkillItem.ResurrectId)
                {
                    reses[0]++;
                    reses[1] += cl.ActualDuration;
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

        public List<Tuple<Boon, long, int>> GetConsumablesList(ParsedLog log, long start, long end)
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
            if (Prof == "Sword")
            {
                _weaponsArray = new string[]
                {
                    "Sword",
                    "2Hand",
                    null,
                    null
                };
                return;
            }
            string[] weapons = new string[4];//first 2 for first set next 2 for second set
            SkillData skillList = log.SkillData;
            List<CastLog> casting = GetCastLogs(log, 0, log.FightData.FightDuration);      
            int swapped = 0;//4 for first set and 5 for next
            long swappedTime = 0;
            List<CastLog> swaps = casting.Where(x => x.SkillId == SkillItem.WeaponSwapId).Take(2).ToList();
            // If the player never swapped, assume they are on their first set
            if (swaps.Count == 0)
            {
                swapped = 4;
            }
            // if the player swapped once, check on which set they started
            else if (swaps.Count == 1)
            {
                swapped = swaps.First().ExpectedDuration == 4 ? 5 : 4;
            }
            foreach (CastLog cl in casting)
            {
                GW2APISkill apiskill = skillList.Get(cl.SkillId)?.ApiSkill;
                if (apiskill != null && cl.Time > swappedTime)
                {
                    if (apiskill.type == "Weapon" && apiskill.professions.Count() > 0 && (apiskill.categories == null || (apiskill.categories.Count() == 1 && apiskill.categories[0] == "Phantasm")))
                    {
                        if (apiskill.dual_wield != null)
                        {
                            if (swapped == 4)
                            {
                                weapons[0] = apiskill.weapon_type;
                                weapons[1] = apiskill.dual_wield;
                            }
                            else if (swapped == 5)
                            {
                                weapons[2] = apiskill.weapon_type;
                                weapons[3] = apiskill.dual_wield;
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
                else if (cl.SkillId == SkillItem.WeaponSwapId)
                {
                    //wepswap  
                    swapped = cl.ExpectedDuration;
                    swappedTime = cl.Time;
                    continue;
                }
            }
            _weaponsArray = weapons;
        }    
        
        protected override void SetDamageTakenLogs(ParsedLog log)
        {
            long timeStart = log.FightData.FightStart;               
            foreach (CombatItem c in log.GetDamageTakenData(AgentItem.InstID)) {
                if (c.Time > log.FightData.FightStart && c.Time < log.FightData.FightEnd) {//selecting player as target
                    long time = c.Time - timeStart;
                    AddDamageTakenLog(time, c);
                }
            }
        }  
        private void SetConsumablesList(ParsedLog log)
        {
            List<Boon> consumableList = Boon.GetConsumableList();
            long timeStart = log.FightData.FightStart;
            long fightDuration = log.FightData.FightEnd - timeStart;
            foreach (Boon consumable in consumableList)
            {
                foreach (CombatItem c in log.GetBoonData(consumable.ID))
                {
                    if (c.IsBuffRemove != ParseEnum.BuffRemove.None || (c.IsBuff != 18 && c.IsBuff != 1) || AgentItem.InstID != c.DstInstid)
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
                        _consumeList.Add(new Tuple<Boon, long, int>(consumable, time, c.Value));
                    }
                }
            }
            _consumeList.Sort((x, y) => x.Item2.CompareTo(y.Item2));

        }

        protected override void SetAdditionalCombatReplayData(ParsedLog log)
        {
            CombatReplay.Icon = GeneralHelper.GetProfIcon(Prof);
            // Down and deads
            List<CombatItem> status = log.CombatData.GetStates(InstID, ParseEnum.StateChange.ChangeDown, log.FightData.FightStart, log.FightData.FightEnd);
            status.AddRange(log.CombatData.GetStates(InstID, ParseEnum.StateChange.ChangeUp, log.FightData.FightStart, log.FightData.FightEnd));
            status.AddRange(log.CombatData.GetStates(InstID, ParseEnum.StateChange.ChangeDead, log.FightData.FightStart, log.FightData.FightEnd));
            status.AddRange(log.CombatData.GetStates(InstID, ParseEnum.StateChange.Spawn, log.FightData.FightStart, log.FightData.FightEnd));
            status.AddRange(log.CombatData.GetStates(InstID, ParseEnum.StateChange.Despawn, log.FightData.FightStart, log.FightData.FightEnd));
            status = status.OrderBy(x => x.Time).ToList();
            List<Tuple<long, long>> dead = CombatReplay.Deads;
            List<Tuple<long, long>> down = CombatReplay.Downs;
            List<Tuple<long, long>> dc = CombatReplay.DCs;
            for (var i = 0; i < status.Count -1;i++)
            {
                CombatItem cur = status[i];
                CombatItem next = status[i + 1];
                if (cur.IsStateChange.IsDown())
                {
                    down.Add(new Tuple<long, long>(cur.Time - log.FightData.FightStart, next.Time - log.FightData.FightStart));
                } else if (cur.IsStateChange.IsDead())
                {
                    dead.Add(new Tuple<long, long>(cur.Time - log.FightData.FightStart, next.Time - log.FightData.FightStart));
                } else if (cur.IsStateChange.IsDespawn())
                {
                    dc.Add(new Tuple<long, long>(cur.Time - log.FightData.FightStart, next.Time - log.FightData.FightStart));
                }
            }
            // check last value
            if (status.Count > 0)
            {
                CombatItem cur = status.Last();
                if (cur.IsStateChange.IsDown())
                {
                    down.Add(new Tuple<long, long>(cur.Time - log.FightData.FightStart, log.FightData.FightDuration));
                }
                else if (cur.IsStateChange.IsDead())
                {
                    dead.Add(new Tuple<long, long>(cur.Time - log.FightData.FightStart, log.FightData.FightDuration));
                }
                else if (cur.IsStateChange.IsDespawn())
                {
                    dc.Add(new Tuple<long, long>(cur.Time - log.FightData.FightStart, log.FightData.FightDuration));
                }
            }
            // Boss related stuff
            log.FightData.Logic.ComputeAdditionalPlayerData(this, log);
        }

        //
        private class Serializable
        {
            public int Group { get; set; }
            public string Img { get; set; }
            public string Type { get; set; }
            public int ID { get; set; }
            public int[] Positions { get; set; }
            public long[] Dead { get; set; }
            public long[] Down { get; set; }
        }

        public override string GetCombatReplayJSON(CombatReplayMap map)
        {
            Serializable aux = new Serializable
            {
                Group = Group,
                Img = CombatReplay.Icon,
                Type = "Player",
                ID = InstID,
                Positions = new int[2 * CombatReplay.Positions.Count],
                Dead = new long[2 * CombatReplay.Deads.Count],
                Down = new long[2 * CombatReplay.Downs.Count]
            };
            int i = 0;
            foreach (Point3D pos in CombatReplay.Positions)
            {
                Tuple<int, int> coord = map.GetMapCoord(pos.X, pos.Y);
                aux.Positions[i++] = coord.Item1;
                aux.Positions[i++] = coord.Item2;
            }
            i = 0;
            foreach (Tuple<long,long> status in CombatReplay.Deads)
            {
                aux.Dead[i++] = status.Item1;
                aux.Dead[i++] = status.Item2;
            }
            i = 0;
            foreach (Tuple<long, long> status in CombatReplay.Downs)
            {
                aux.Down[i++] = status.Item1;
                aux.Down[i++] = status.Item2;
            }

            return JsonConvert.SerializeObject(aux);
        }

        public override int GetCombatReplayID()
        {
            return InstID;
        }


        /*protected override void setHealingLogs(ParsedLog log)
        {
            long time_start = log.getBossData().getFirstAware();
            foreach (CombatItem c in log.getHealingData())
            {
                if (agent.InstID == c.getSrcInstid() && c.getTime() > log.getBossData().getFirstAware() && c.getTime() < log.getBossData().getLastAware())//selecting player or minion as caster
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
                if (agent.InstID == c.getDstInstid() && c.getTime() > log.getBossData().getFirstAware() && c.getTime() < log.getBossData().getLastAware())
                {//selecting player as target
                    long time = c.getTime() - time_start;
                    addHealingReceivedLog(time, c);
                }
            }
        }*/
    }
}
