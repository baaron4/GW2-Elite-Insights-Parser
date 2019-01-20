using LuckParser.Controllers;
using LuckParser.Parser;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

using System.Globalization;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class Player : AbstractMasterActor
    {
        public class Consumable
        {
            public Boon Item { get; }
            public long Time { get; }
            public int Duration { get; }
            public int Stack { get; set; }

            public Consumable(Boon item, long time, int duration)
            {
                Item = item;
                Time = time;
                Duration = duration;
                Stack = 1;
            }
        }

        public class DeathRecap
        {
            public class DeathRecapDamageItem
            {
                public long Skill;
                public bool Condi;
                public string Src;
                public int Damage;
                public int Time;
            }

            public int Time;
            public List<DeathRecapDamageItem> ToDown;
            public List<DeathRecapDamageItem> ToKill;
        }
        // Fields
        public readonly string Account;
        public readonly int Group;
       
        private readonly List<Consumable> _consumeList = new List<Consumable>();
        private List<DeathRecap> _deathRecaps = new List<DeathRecap>();
        //weaponslist
        private string[] _weaponsArray;

        // Constructors
        public Player(AgentItem agent, bool noSquad) : base(agent)
        {
            string[] name = agent.Name.Split('\0');
            Account = name[1];
            Group = noSquad ? 1 : int.Parse(name[2], NumberStyles.Integer, CultureInfo.InvariantCulture);
        }
        
        // Public methods
        public int[] GetCleanses(ParsedLog log, int phaseIndex) {
            int[] cleanse = { 0, 0 };
            foreach (Player p in log.PlayerList)
            {
                foreach(List<long> list in p.GetCondiCleanse(log,phaseIndex, AgentItem).Values)
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

        public List<DeathRecap> GetDeathRecaps(ParsedLog log)
        {
            if(_deathRecaps == null)
            {
                return null;
            }
            if (_deathRecaps.Count == 0)
            {
                SetDeathRecaps(log);
            }
            return _deathRecaps;
        }

        public string[] GetWeaponsArray(ParsedLog log)
        {
            if (_weaponsArray == null)
            {
                EstimateWeapons( log);
            }
            return _weaponsArray;
        }

        public List<Consumable> GetConsumablesList(ParsedLog log, long start, long end)
        {
            if (_consumeList.Count == 0)
            {
                SetConsumablesList(log);
            }
            return _consumeList.Where(x => x.Time >= start && x.Time <= end).ToList() ;
        }
        
        // Private Methods

        private void SetDeathRecaps(ParsedLog log)
        {
            List<DeathRecap> res = _deathRecaps;
            List<CombatItem> deads = log.CombatData.GetStatesData(InstID, ParseEnum.StateChange.ChangeDead, log.FightData.FightStart, log.FightData.FightEnd);
            List<CombatItem> downs = log.CombatData.GetStatesData(InstID, ParseEnum.StateChange.ChangeDown, log.FightData.FightStart, log.FightData.FightEnd);
            long lastTime = log.FightData.FightStart;
            List<DamageLog> damageLogs = GetDamageTakenLogs(null, log, 0, log.FightData.FightDuration);
            foreach (CombatItem dead in deads)
            {
                DeathRecap recap = new DeathRecap()
                {
                    Time = (int)(log.FightData.ToFightSpace(dead.Time))
                };
                CombatItem downed = downs.LastOrDefault(x => x.Time <= dead.Time && x.Time >= lastTime);
                if (downed != null)
                {
                    List<DamageLog> damageToDown = damageLogs.Where(x => x.Time < log.FightData.ToFightSpace(downed.Time) && x.Damage > 0 && x.Time > log.FightData.ToFightSpace(lastTime)).ToList();
                    recap.ToDown = damageToDown.Count > 0 ? new List<DeathRecap.DeathRecapDamageItem>() : null;
                    int damage = 0;
                    for (int i = damageToDown.Count - 1; i >= 0; i--)
                    {
                        DamageLog dl = damageToDown[i];
                        AgentItem ag = log.AgentData.GetAgentByInstID(dl.SrcInstId, log.FightData.ToLogSpace(dl.Time));
                        DeathRecap.DeathRecapDamageItem item = new DeathRecap.DeathRecapDamageItem()
                        {
                            Time = (int)dl.Time,
                            Condi = dl.IsCondi,
                            Skill = dl.SkillId,
                            Damage = dl.Damage,
                            Src = ag != null ? ag.Name.Replace("\u0000", "").Split(':')[0] : ""
                        };
                        damage += dl.Damage;
                        recap.ToDown.Add(item);
                        if (damage > 20000)
                        {
                            break;
                        }
                    }
                    List<DamageLog> damageToKill = damageLogs.Where(x => x.Time > log.FightData.ToFightSpace(downed.Time) && x.Time < log.FightData.ToFightSpace(dead.Time) && x.Damage > 0 && x.Time > log.FightData.ToFightSpace(lastTime)).ToList();
                    recap.ToKill = damageToKill.Count > 0 ? new List<DeathRecap.DeathRecapDamageItem>() : null;
                    for (int i = damageToKill.Count - 1; i >= 0; i--)
                    {
                        DamageLog dl = damageToKill[i];
                        AgentItem ag = log.AgentData.GetAgentByInstID(dl.SrcInstId, log.FightData.ToLogSpace(dl.Time));
                        DeathRecap.DeathRecapDamageItem item = new DeathRecap.DeathRecapDamageItem()
                        {
                            Time = (int)dl.Time,
                            Condi = dl.IsCondi,
                            Skill = dl.SkillId,
                            Damage = dl.Damage,
                            Src = ag != null ? ag.Name.Replace("\u0000", "").Split(':')[0] : ""
                        };
                        recap.ToKill.Add(item);
                    }
                }
                else
                {
                    recap.ToDown = null;
                    List<DamageLog> damageToKill = damageLogs.Where(x => x.Time < log.FightData.ToFightSpace(dead.Time) && x.Damage > 0 && x.Time > log.FightData.ToFightSpace(lastTime)).ToList();
                    recap.ToKill = damageToKill.Count > 0 ? new List<DeathRecap.DeathRecapDamageItem>() : null;
                    int damage = 0;
                    for (int i = damageToKill.Count - 1; i >= 0; i--)
                    {
                        DamageLog dl = damageToKill[i];
                        AgentItem ag = log.AgentData.GetAgentByInstID(dl.SrcInstId, log.FightData.ToLogSpace(dl.Time));
                        DeathRecap.DeathRecapDamageItem item = new DeathRecap.DeathRecapDamageItem()
                        {
                            Time = (int)dl.Time,
                            Condi = dl.IsCondi,
                            Skill = dl.SkillId,
                            Damage = dl.Damage,
                            Src = ag != null ? ag.Name.Replace("\u0000", "").Split(':')[0] : ""
                        };
                        damage += dl.Damage;
                        recap.ToKill.Add(item);
                        if (damage > 20000)
                        {
                            break;
                        }
                    }
                }
                lastTime = dead.Time;
                res.Add(recap);
            }
            if (_deathRecaps.Count == 0)
            {
                _deathRecaps = null;
            }
        }

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
            // if the player swapped, check on which set they started
            else
            {
                swapped = swaps.First().ExpectedDuration == 4 ? 5 : 4;
            }
            foreach (CastLog cl in casting)
            {
                if (cl.ActualDuration == 0 && cl.SkillId != SkillItem.WeaponSwapId)
                {
                    continue;
                }
                SkillItem skill = skillList.Get(cl.SkillId);
                if (!skill.EstimateWeapons(weapons, swapped, cl.Time > swappedTime) && cl.SkillId == SkillItem.WeaponSwapId)
                {
                    //wepswap  
                    swapped = cl.ExpectedDuration;
                    swappedTime = cl.Time;
                }
            }
            _weaponsArray = weapons;
        }    
        
        private void SetConsumablesList(ParsedLog log)
        {
            List<Boon> consumableList = Boon.GetConsumableList();
            long fightDuration = log.FightData.FightDuration;
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
                        time = log.FightData.ToFightSpace(c.Time);
                    }
                    if (time <= fightDuration)
                    {
                        Consumable existing = _consumeList.Find(x => x.Time == time && x.Item.ID == consumable.ID);
                        if (existing != null)
                        {
                            existing.Stack++;
                        } else
                        {
                            _consumeList.Add(new Consumable(consumable, time, c.Value));
                        }
                    }
                }
            }
            _consumeList.Sort((x, y) => x.Time.CompareTo(y.Time));

        }

        protected override void SetAdditionalCombatReplayData(ParsedLog log)
        {
            CombatReplay.Icon = GeneralHelper.GetProfIcon(Prof);
            // Down and deads
            List<(long, long)> dead = CombatReplay.Deads;
            List<(long, long)> down = CombatReplay.Downs;
            List<(long, long)> dc = CombatReplay.DCs;
            log.CombatData.GetAgentStatus(FirstAware, LastAware, InstID, dead, down, dc);
            // Fight related stuff
            log.FightData.Logic.ComputeAdditionalPlayerData(this, log);
            List<Point3D> facings = CombatReplay.Rotations;
            if (facings.Any())
            {
                CombatReplay.Actors.Add(new FacingActor(((int)CombatReplay.TimeOffsets.start, (int)CombatReplay.TimeOffsets.end), new AgentConnector(this), facings));
            }
        }

        //
        private class Serializable
        {
            public int Group { get; set; }
            public string Img { get; set; }
            public string Type { get; set; }
            public int ID { get; set; }
            public double[] Positions { get; set; }
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
                ID = GetCombatReplayID(),
                Positions = new double[2 * CombatReplay.Positions.Count],
                Dead = new long[2 * CombatReplay.Deads.Count],
                Down = new long[2 * CombatReplay.Downs.Count]
            };
            int i = 0;
            foreach (Point3D pos in CombatReplay.Positions)
            {
                (double x, double y) = map.GetMapCoord(pos.X, pos.Y);
                aux.Positions[i++] = x;
                aux.Positions[i++] = y;
            }
            i = 0;
            foreach ((long start, long end) in CombatReplay.Deads)
            {
                aux.Dead[i++] = start;
                aux.Dead[i++] = end;
            }
            i = 0;
            foreach ((long start, long end) in CombatReplay.Downs)
            {
                aux.Down[i++] = start;
                aux.Down[i++] = end;
            }

            return JsonConvert.SerializeObject(aux);
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
