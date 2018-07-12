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
            String[] name = agent.getName().Split('\0');
            character = name[0];
            this.agent = agent;
        }
        // Getters
        public ushort getInstid()
        {
            return agent.getInstid();
        }
        public string getCharacter()
        {
            return character;
        }
        public string getProf()
        {
            return agent.getProf();
        }
        public long getDeath(ParsedLog log, long start, long end)
        {
            long offset = log.getBossData().getFirstAware();
            CombatItem dead = log.getCombatList().FirstOrDefault(x => x.getSrcInstid() == agent.getInstid() && x.isStateChange() == ParseEnum.StateChange.ChangeDead && x.getTime() >= start + offset && x.getTime() <= end + offset);
            if (dead != null && dead.getTime() > 0)
            {
                return dead.getTime();
            }
            return 0;
        }

        public List<DamageLog> getDamageLogs(int instidFilter, ParsedLog log, long start, long end)//isntid = 0 gets all logs if specefied sets and returns filterd logs
        {
            if (damage_logs.Count == 0)
            {
                setDamageLogs(log);
            }


            if (damage_logsFiltered.Count == 0)
            {
                damage_logsFiltered = damage_logs.Where(x => x.getDstInstidt() == instidFilter).ToList();
            }
            if (instidFilter == 0)
            {
                return damage_logs.Where(x => x.getTime() >= start && x.getTime() <= end).ToList();
            }
            return damage_logsFiltered.Where( x => x.getTime() >= start && x.getTime() <= end).ToList();
        }
        public List<DamageLog> getDamageTakenLogs(ParsedLog log, long start, long end)
        {
            if (damageTaken_logs.Count == 0)
            {
                setDamagetakenLogs(log);
            }
            return damageTaken_logs.Where(x => x.getTime() >= start && x.getTime() <= end).ToList();
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
        public List<CastLog> getCastLogs(ParsedLog log, long start, long end)
        {
            if (cast_logs.Count == 0)
            {
                setCastLogs(log);
            }
            return cast_logs.Where(x => x.getTime() >= start && x.getTime() <= end).ToList();

        }

        public List<CastLog> getCastLogsActDur(ParsedLog log, long start, long end)
        {
            if (cast_logs.Count == 0)
            {
                setCastLogs(log);
            }
            return cast_logs.Where(x => x.getTime() + x.getActDur() >= start && x.getTime() <= end).ToList();

        }
        public List<DamageLog> getJustPlayerDamageLogs(int instidFilter, ParsedLog log, long start, long end)
        {
            return getDamageLogs(instidFilter, log, start, end).Where(x => x.getInstidt() == agent.getInstid()).ToList();
        }
        // privates
        protected void addDamageLog(long time, CombatItem c)
        {
            if (c.isBuffremove() == ParseEnum.BuffRemove.None)
            {
                if (c.isBuff() == 1 && c.getBuffDmg() != 0)//condi
                {
                    damage_logs.Add(new DamageLogCondition(time, c));
                }
                else if (c.isBuff() == 0 && c.getValue() != 0)//power
                {
                    damage_logs.Add(new DamageLogPower(time, c));
                }
                else if (c.getResult() == ParseEnum.Result.Absorb || c.getResult() == ParseEnum.Result.Blind || c.getResult() == ParseEnum.Result.Interrupt)
                {//Hits that where blinded, invulned, interupts
                    damage_logs.Add(new DamageLogPower(time, c));
                }
            }

        }
        protected void addDamageTakenLog(long time, CombatItem c)
        {
            if (c.isBuff() == 1 && c.getBuffDmg() != 0)
            {
                //inco,ing condi dmg not working or just not present?
                // damagetaken.Add(c.getBuffDmg());
                damageTaken_logs.Add(new DamageLogCondition(time, c));
            }
            else if (c.isBuff() == 0 && c.getValue() >= 0)
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
        protected abstract void setDamageLogs(ParsedLog log);     
        protected abstract void setCastLogs(ParsedLog log);
        protected abstract void setDamagetakenLogs(ParsedLog log);
        //protected abstract void setHealingLogs(ParsedLog log);
        //protected abstract void setHealingReceivedLogs(ParsedLog log);
    }
}
