using System;
using System.Collections.Generic;
using System.Linq;
using LuckParser.Models.DataModels;

namespace LuckParser.Models.ParseModels
{
    public abstract class AbstractPlayer
    {
        public readonly AgentItem AgentItem;
        public readonly string Character;
        protected readonly List<DamageLog> DamageLogs = new List<DamageLog>();
        protected Dictionary<ushort,List<DamageLog>> DamageLogsByDst = new Dictionary<ushort, List<DamageLog>>();
        //protected List<DamageLog> HealingLogs = new List<DamageLog>();
        //protected List<DamageLog> HealingReceivedLogs = new List<DamageLog>();
        private readonly List<DamageLog> _damageTakenlogs = new List<DamageLog>();
        protected Dictionary<ushort, List<DamageLog>> _damageTakenLogsBySrc = new Dictionary<ushort, List<DamageLog>>();
        protected readonly List<CastLog> CastLogs = new List<CastLog>();

        public uint Toughness => AgentItem.Toughness;
        public uint Condition => AgentItem.Condition;
        public uint Concentration => AgentItem.Concentration;
        public uint Healing => AgentItem.Healing;
        public ushort InstID => AgentItem.InstID;
        public string Prof => AgentItem.Prof;
        public ulong Agent => AgentItem.Agent;
        public long LastAware => AgentItem.LastAware;
        public long FirstAware => AgentItem.FirstAware;
        public ushort ID => AgentItem.ID;
        public uint HitboxHeight => AgentItem.HitboxHeight;
        public uint HitboxWidth => AgentItem.HitboxWidth;

        protected AbstractPlayer(AgentItem agent)
        {
            string[] name = agent.Name.Split('\0');
            Character = name[0];
            AgentItem = agent;
        }
        // Getters
        public List<DamageLog> GetDamageLogs(AbstractPlayer target, ParsedLog log, long start, long end)
        {
            if (DamageLogs.Count == 0)
            {
                SetDamageLogs(log);
                DamageLogsByDst = DamageLogs.GroupBy(x => x.DstInstId).ToDictionary(x => x.Key, x => x.ToList());
            }
            if (target != null)
            {
                if (DamageLogsByDst.TryGetValue(target.InstID, out var list))
                {
                    long targetStart = target.FirstAware - log.FightData.FightStart;
                    long targetEnd = target.LastAware - log.FightData.FightStart;
                    return list.Where(x => x.Time >= start && x.Time > targetStart && x.Time <= end && x.Time < targetEnd).ToList();
                } else
                {
                    return new List<DamageLog>();
                }
            }
            return DamageLogs.Where( x => x.Time >= start && x.Time <= end).ToList();
        }
        public List<DamageLog> GetDamageTakenLogs(AbstractPlayer target, ParsedLog log, long start, long end)
        {
            if (_damageTakenlogs.Count == 0)
            {
                SetDamageTakenLogs(log);
                _damageTakenLogsBySrc = _damageTakenlogs.GroupBy(x => x.SrcInstId).ToDictionary(x => x.Key, x => x.ToList());
            }
            if (target != null)
            {
                if (_damageTakenLogsBySrc.TryGetValue(target.InstID, out var list))
                {
                    long targetStart = target.FirstAware - log.FightData.FightStart;
                    long targetEnd = target.LastAware - log.FightData.FightStart;
                    return list.Where(x => x.Time >= start && x.Time > targetStart && x.Time <= end && x.Time < targetEnd).ToList();
                }
                else
                {
                    return new List<DamageLog>();
                }
            }
            return _damageTakenlogs.Where(x => x.Time >= start && x.Time <= end).ToList();
        }
        /*public List<DamageLog> getHealingLogs(ParsedLog log, long start, long end)//isntid = 0 gets all logs if specified sets and returns filtered logs
        {
            if (healingLogs.Count == 0)
            {
                setHealingLogs(log);
            }
            return healingLogs.Where(x => x.getTime() >= start && x.getTime() <= end).ToList();
        }
        public List<DamageLog> getHealingReceivedLogs(ParsedLog log, long start, long end)
        {
            if (healingReceivedLogs.Count == 0)
            {
                setHealingReceivedLogs(log);
            }
            return healingReceivedLogs.Where(x => x.getTime() >= start && x.getTime() <= end).ToList();
        }*/
        public List<CastLog> GetCastLogs(ParsedLog log, long start, long end)
        {
            if (CastLogs.Count == 0)
            {
                SetCastLogs(log);
            }
            return CastLogs.Where(x => x.Time >= start && x.Time <= end).ToList();

        }

        public List<CastLog> GetCastLogsActDur(ParsedLog log, long start, long end)
        {
            if (CastLogs.Count == 0)
            {
                SetCastLogs(log);
            }
            return CastLogs.Where(x => x.Time + x.ActualDuration >= start && x.Time <= end).ToList();

        }
        public List<DamageLog> GetJustPlayerDamageLogs(AbstractPlayer target, ParsedLog log, long start, long end)
        {
            return GetDamageLogs(target, log, start, end).Where(x => x.SrcInstId == AgentItem.InstID).ToList();
        }
        // privates
        protected void AddDamageLog(long time, CombatItem c)
        {
            if (c.IsBuff != 0)//condi
            {
                DamageLogs.Add(new DamageLogCondition(time, c));
            }
            else if (c.IsBuff == 0)//power
            {
                DamageLogs.Add(new DamageLogPower(time, c));
            }
            else if (c.Result == ParseEnum.Result.Absorb || c.Result == ParseEnum.Result.Blind || c.Result == ParseEnum.Result.Interrupt)
            {//Hits that where blinded, invulned, interrupts
                DamageLogs.Add(new DamageLogPower(time, c));
            }


        }
        protected void AddDamageTakenLog(long time, CombatItem c)
        {
            if (c.IsBuff != 0)
            {
                //inco,ing condi dmg not working or just not present?
                // damagetaken.Add(c.getBuffDmg());
                _damageTakenlogs.Add(new DamageLogCondition(time, c));
            }
            else if (c.IsBuff == 0)
            {
                _damageTakenlogs.Add(new DamageLogPower(time, c));

            }
        }
        /*protected void addHealingLog(long time, CombatItem c)
        {
            if (c.isBuffremove() == ParseEnum.BuffRemove.None)
            {
                if (c.isBuff() == 1 && c.getBuffDmg() != 0)//boon
                {
                    healing_logs.Add(new DamageLogCondition(time, c));
                }
                else if (c.isBuff() == 0 && c.getValue() != 0)//skill
                {
                    healing_logs.Add(new DamageLogPower(time, c));
                }
            }

        }
        protected void addHealingReceivedLog(long time, CombatItem c)
        {
            if (c.isBuff() == 1 && c.getBuffDmg() != 0)
            {
                healing_received_logs.Add(new DamageLogCondition(time, c));
            }
            else if (c.isBuff() == 0 && c.getValue() >= 0)
            {
                healing_received_logs.Add(new DamageLogPower(time, c));

            }
        }*/
        // Setters
        protected abstract void SetDamageLogs(ParsedLog log);     
        protected abstract void SetCastLogs(ParsedLog log);
        protected abstract void SetDamageTakenLogs(ParsedLog log);
        //protected abstract void setHealingLogs(ParsedLog log);
        //protected abstract void setHealingReceivedLogs(ParsedLog log);
    }
}
