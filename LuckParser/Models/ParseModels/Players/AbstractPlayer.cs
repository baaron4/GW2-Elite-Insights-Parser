using System;
using System.Collections.Generic;
using System.Linq;
using LuckParser.Models.DataModels;

namespace LuckParser.Models.ParseModels
{
    public abstract class AbstractPlayer
    {
        protected readonly AgentItem AgentItem;
        public readonly String Character;
        protected readonly List<DamageLog> DamageLogs = new List<DamageLog>();
        protected Dictionary<ushort,List<DamageLog>> DamageLogsByDst = new Dictionary<ushort, List<DamageLog>>();
        //protected List<DamageLog> HealingLogs = new List<DamageLog>();
        //protected List<DamageLog> HealingReceivedLogs = new List<DamageLog>();
        private readonly List<DamageLog> _damageTakenlogs = new List<DamageLog>();
        protected readonly List<CastLog> CastLogs = new List<CastLog>();

        public int Toughness => AgentItem.Toughness;
        public int Condition => AgentItem.Condition;
        public int Concentration => AgentItem.Concentration;
        public int Healing => AgentItem.Healing;
        public ushort InstID => AgentItem.InstID;
        public string Prof => AgentItem.Prof;
        public ulong Agent => AgentItem.Agent;
        public long LastAware => AgentItem.LastAware;
        public long FirstAware => AgentItem.FirstAware;

        protected AbstractPlayer(AgentItem agent)
        {
            String[] name = agent.Name.Split('\0');
            Character = name[0];
            AgentItem = agent;
        }
        // Getters
        public List<DamageLog> GetDamageLogs(ushort dstFilter, ParsedLog log, long start, long end)
        {
            if (DamageLogs.Count == 0)
            {
                SetDamageLogs(log);
                DamageLogsByDst = DamageLogs.GroupBy(x => x.DstInstId).ToDictionary(x => x.Key, x => x.ToList());
            }
            if (DamageLogsByDst.TryGetValue(dstFilter, out var list))
            {
                return list.Where(x => x.Time >= start && x.Time <= end).ToList();
            }
            return DamageLogs.Where( x => x.Time >= start && x.Time <= end).ToList();
        }
        public List<DamageLog> GetDamageLogs(List<AgentItem> redirection, ParsedLog log, long start, long end)
        {
            if (redirection.Count == 0)
            {
                return GetDamageLogs(log.Boss.InstID, log, start, end);
            }
            List<DamageLog> dls = GetDamageLogs(0, log, start, end);
            List<DamageLog> res = new List<DamageLog>();
            foreach (AgentItem a in redirection)
            {
                res.AddRange(dls.Where(x => x.DstInstId == a.InstID && x.Time >= a.FirstAware - log.FightData.FightStart && x.Time <= a.LastAware - log.FightData.FightStart));
            }
            res.Sort((x, y) => x.Time < y.Time ? -1 : 1);
            return res;
        }
        public List<DamageLog> GetDamageTakenLogs(ParsedLog log, long start, long end)
        {
            if (_damageTakenlogs.Count == 0)
            {
                SetDamageTakenLogs(log);
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
        public List<DamageLog> GetJustPlayerDamageLogs(ushort instidFilter, ParsedLog log, long start, long end)
        {
            return GetDamageLogs(instidFilter, log, start, end).Where(x => x.SrcInstId == AgentItem.InstID).ToList();
        }

        public List<DamageLog> GetJustPlayerDamageLogs(List<AgentItem> redirection, ParsedLog log, long start, long end)
        {
            if (redirection.Count == 0)
            {
                return GetJustPlayerDamageLogs(log.Boss.InstID, log, start, end);
            }
            List<DamageLog> dls = GetJustPlayerDamageLogs(0, log, start, end);
            List<DamageLog> res = new List<DamageLog>();
            foreach (AgentItem a in redirection)
            {
                res.AddRange(dls.Where(x => x.DstInstId == a.InstID && x.Time >= a.FirstAware - log.FightData.FightStart && x.Time <= a.LastAware - log.FightData.FightStart));
            }
            res.Sort((x, y) => x.Time < y.Time ? -1 : 1);
            return res;
        }
        // privates
        protected void AddDamageLog(long time, CombatItem c)
        {
            if (c.IsBuffRemove == ParseEnum.BuffRemove.None)
            {
                if (c.IsBuff == 1 && c.BuffDmg != 0)//condi
                {
                    DamageLogs.Add(new DamageLogCondition(time, c));
                }
                else if (c.IsBuff == 0 && c.Value != 0)//power
                {
                    DamageLogs.Add(new DamageLogPower(time, c));
                }
                else if (c.Result == ParseEnum.Result.Absorb || c.Result == ParseEnum.Result.Blind || c.Result == ParseEnum.Result.Interrupt)
                {//Hits that where blinded, invulned, interrupts
                    DamageLogs.Add(new DamageLogPower(time, c));
                }
            }

        }
        protected void AddDamageTakenLog(long time, CombatItem c)
        {
            if (c.IsBuff == 1 && c.BuffDmg != 0)
            {
                //inco,ing condi dmg not working or just not present?
                // damagetaken.Add(c.getBuffDmg());
                _damageTakenlogs.Add(new DamageLogCondition(time, c));
            }
            else if (c.IsBuff == 0 && c.Value >= 0)
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
