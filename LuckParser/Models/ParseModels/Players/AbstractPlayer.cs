using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;
using LuckParser.Models.DataModels;

namespace LuckParser.Models.ParseModels
{
    public abstract class AbstractPlayer
    {
        protected AgentItem agent;
        private String character;
        // DPS
        protected List<DamageLog> damage_logs = new List<DamageLog>();
        private List<DamageLog> damage_logsFiltered = new List<DamageLog>();
        // Heal
        //protected List<DamageLog> healing_logs = new List<DamageLog>();
        //protected List<DamageLog> healing_received_logs = new List<DamageLog>();
        // Taken damage
        protected List<DamageLog> damageTaken_logs = new List<DamageLog>();
        // Casts
        protected List<CastLog> cast_logs = new List<CastLog>();
        // Constructor
        public AbstractPlayer(AgentItem agent)
        {
            String[] name = agent.GetName().Split('\0');
            character = name[0];
            this.agent = agent;
        }
        // Getters
        public ushort GetInstid()
        {
            return agent.GetInstid();
        }
        public string GetCharacter()
        {
            return character;
        }
        public string GetProf()
        {
            return agent.GetProf();
        }

        public List<DamageLog> GetDamageLogs(int instidFilter, ParsedLog log, long start, long end)//isntid = 0 gets all logs if specefied sets and returns filterd logs
        {
            if (damage_logs.Count == 0)
            {
                SetDamageLogs(log);
            }


            if (damage_logsFiltered.Count == 0)
            {
                damage_logsFiltered = damage_logs.Where(x => x.GetDstInstidt() == instidFilter).ToList();
            }
            if (instidFilter == 0)
            {
                return damage_logs.Where(x => x.GetTime() >= start && x.GetTime() <= end).ToList();
            }
            return damage_logsFiltered.Where( x => x.GetTime() >= start && x.GetTime() <= end).ToList();
        }
        public List<DamageLog> GetDamageLogs(List<AgentItem> redirection, ParsedLog log, long start, long end)
        {
            if (redirection.Count == 0)
            {
                return GetDamageLogs(log.GetBossData().GetInstid(), log, start, end);
            }
            List<DamageLog> dls = GetDamageLogs(0, log, start, end);
            List<DamageLog> res = new List<DamageLog>();
            foreach (AgentItem a in redirection)
            {
                res.AddRange(dls.Where(x => x.GetDstInstidt() == a.GetInstid() && x.GetTime() >= a.GetFirstAware() - log.GetBossData().GetFirstAware() && x.GetTime() <= a.GetLastAware() - log.GetBossData().GetFirstAware()));
            }
            res.Sort((x, y) => x.GetTime() < y.GetTime() ? -1 : 1);
            return res;
        }
        public List<DamageLog> GetDamageTakenLogs(ParsedLog log, long start, long end)
        {
            if (damageTaken_logs.Count == 0)
            {
                SetDamagetakenLogs(log);
            }
            return damageTaken_logs.Where(x => x.GetTime() >= start && x.GetTime() <= end).ToList();
        }
        /*public List<DamageLog> getHealingLogs(ParsedLog log, long start, long end)//isntid = 0 gets all logs if specefied sets and returns filterd logs
        {
            if (healing_logs.Count == 0)
            {
                setHealingLogs(log);
            }
            return healing_logs.Where(x => x.getTime() >= start && x.getTime() <= end).ToList();
        }
        public List<DamageLog> getHealingReceivedLogs(ParsedLog log, long start, long end)
        {
            if (healing_received_logs.Count == 0)
            {
                setHealingReceivedLogs(log);
            }
            return healing_received_logs.Where(x => x.getTime() >= start && x.getTime() <= end).ToList();
        }*/
        public List<CastLog> GetCastLogs(ParsedLog log, long start, long end)
        {
            if (cast_logs.Count == 0)
            {
                SetCastLogs(log);
            }
            return cast_logs.Where(x => x.GetTime() >= start && x.GetTime() <= end).ToList();

        }

        public List<CastLog> GetCastLogsActDur(ParsedLog log, long start, long end)
        {
            if (cast_logs.Count == 0)
            {
                SetCastLogs(log);
            }
            return cast_logs.Where(x => x.GetTime() + x.GetActDur() >= start && x.GetTime() <= end).ToList();

        }
        public List<DamageLog> GetJustPlayerDamageLogs(int instidFilter, ParsedLog log, long start, long end)
        {
            return GetDamageLogs(instidFilter, log, start, end).Where(x => x.GetSrcInstidt() == agent.GetInstid()).ToList();
        }

        public List<DamageLog> GetJustPlayerDamageLogs(List<AgentItem> redirection, ParsedLog log, long start, long end)
        {
            if (redirection.Count == 0)
            {
                return GetJustPlayerDamageLogs(log.GetBossData().GetInstid(), log, start, end);
            }
            List<DamageLog> dls = GetJustPlayerDamageLogs(0, log, start, end);
            List<DamageLog> res = new List<DamageLog>();
            foreach (AgentItem a in redirection)
            {
                res.AddRange(dls.Where(x => x.GetDstInstidt() == a.GetInstid() && x.GetTime() >= a.GetFirstAware() - log.GetBossData().GetFirstAware() && x.GetTime() <= a.GetLastAware() - log.GetBossData().GetFirstAware()));
            }
            res.Sort((x, y) => x.GetTime() < y.GetTime() ? -1 : 1);
            return res;
        }
        // privates
        protected void AddDamageLog(long time, CombatItem c)
        {
            if (c.IsBuffremove() == ParseEnum.BuffRemove.None)
            {
                if (c.IsBuff() == 1 && c.GetBuffDmg() != 0)//condi
                {
                    damage_logs.Add(new DamageLogCondition(time, c));
                }
                else if (c.IsBuff() == 0 && c.GetValue() != 0)//power
                {
                    damage_logs.Add(new DamageLogPower(time, c));
                }
                else if (c.GetResult() == ParseEnum.Result.Absorb || c.GetResult() == ParseEnum.Result.Blind || c.GetResult() == ParseEnum.Result.Interrupt)
                {//Hits that where blinded, invulned, interupts
                    damage_logs.Add(new DamageLogPower(time, c));
                }
            }

        }
        protected void AddDamageTakenLog(long time, CombatItem c)
        {
            if (c.IsBuff() == 1 && c.GetBuffDmg() != 0)
            {
                //inco,ing condi dmg not working or just not present?
                // damagetaken.Add(c.getBuffDmg());
                damageTaken_logs.Add(new DamageLogCondition(time, c));
            }
            else if (c.IsBuff() == 0 && c.GetValue() >= 0)
            {
                damageTaken_logs.Add(new DamageLogPower(time, c));

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
        protected abstract void SetDamagetakenLogs(ParsedLog log);
        //protected abstract void setHealingLogs(ParsedLog log);
        //protected abstract void setHealingReceivedLogs(ParsedLog log);
    }
}
