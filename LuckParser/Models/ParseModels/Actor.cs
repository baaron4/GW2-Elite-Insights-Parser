using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public abstract class Actor
    {
        // properties
        private String character;
        protected ushort instid;
        protected List<DamageLog> damage_logs = new List<DamageLog>();
        protected List<DamageLog> damage_logsFiltered = new List<DamageLog>();
        protected List<CastLog> cast_logs = new List<CastLog>();

        public Actor(AgentItem agent)
        {
            String[] name = agent.getName().Split('\0');
            character = name[0];
            instid = agent.getInstid();
        }

        public ushort getInstid()
        {
            return instid;
        }
        public string getCharacter()
        {
            return character;
        }

        public List<DamageLog> getDamageLogs(int instidFilter, BossData bossData, List<CombatItem> combatList, AgentData agentData, long start, long end)//isntid = 0 gets all logs if specefied sets and returns filterd logs
        {
            if (damage_logs.Count == 0)
            {
                setDamageLogs(bossData, combatList, agentData);
            }


            if (damage_logsFiltered.Count == 0)
            {
                setFilteredDamageLogs(bossData, combatList, agentData);
            }
            if (instidFilter == 0)
            {
                return damage_logs.Where(x => x.getTime() >= start && x.getTime() <= end).ToList();
            }
            else
            {
                return damage_logsFiltered.Where(x => x.getTime() >= start && x.getTime() <= end).ToList();
            }
        }

        public List<CastLog> getCastLogs(BossData bossData, List<CombatItem> combatList, AgentData agentData, long start, long end)
        {
            if (cast_logs.Count == 0)
            {
                setCastLogs(bossData, combatList, agentData);
            }
            return cast_logs.Where(x => x.getTime() >= start && x.getTime() <= end).ToList();

        }

        public List<CastLog> getCastLogsActDur(BossData bossData, List<CombatItem> combatList, AgentData agentData, long start, long end)
        {
            if (cast_logs.Count == 0)
            {
                setCastLogs(bossData, combatList, agentData);
            }
            return cast_logs.Where(x => x.getTime() + x.getActDur() >= start && x.getTime() <= end).ToList();

        }
        // Privates
        private void addDamageLog(long time, ushort instid, CombatItem c, List<DamageLog> toFill)
        {
            LuckParser.Models.ParseEnums.StateChange state = c.isStateChange();
            if (instid == c.getDstInstid() && c.getIFF().getEnum() == "FOE")
            {
                if (state.getEnum() == "NORMAL" && c.isBuffremove().getID() == 0)
                {
                    if (c.isBuff() == 1 && c.getBuffDmg() != 0)//condi
                    {
                        toFill.Add(new DamageLogCondition(time, c));
                    }
                    else if (c.isBuff() == 0 && c.getValue() != 0)//power
                    {
                        toFill.Add(new DamageLogPower(time, c));
                    }
                    else if (c.getResult().getID() == 5 || c.getResult().getID() == 6 || c.getResult().getID() == 7)
                    {//Hits that where blinded, invulned, interupts
                        toFill.Add(new DamageLogPower(time, c));
                    }
                }
            }
        }
        private void setFilteredDamageLogs(BossData bossData, List<CombatItem> combatList, AgentData agentData)
        {
            long time_start = bossData.getFirstAware();
            foreach (CombatItem c in combatList)
            {
                if (instid == c.getSrcInstid() || instid == c.getSrcMasterInstid())//selecting player
                {
                    long time = c.getTime() - time_start;
                    addDamageLog(time, bossData.getInstid(), c, damage_logsFiltered);
                }
            }
        }

        private void setDamageLogs(BossData bossData, List<CombatItem> combatList, AgentData agentData)
        {
            long time_start = bossData.getFirstAware();
            foreach (CombatItem c in combatList)
            {

                if (instid == c.getSrcInstid() || instid == c.getSrcMasterInstid())//selecting player or minion as caster
                {
                    long time = c.getTime() - time_start;
                    foreach (AgentItem item in agentData.getAllAgentsList())
                    {//selecting all
                        addDamageLog(time, item.getInstid(), c, damage_logs);
                    }
                }

            }
        }

        private void setCastLogs(BossData bossData, List<CombatItem> combatList, AgentData agentData)
        {
            long time_start = bossData.getFirstAware();
            CastLog curCastLog = null;

            foreach (CombatItem c in combatList)
            {
                LuckParser.Models.ParseEnums.StateChange state = c.isStateChange();
                if (state.getID() == 0)
                {
                    if (instid == c.getSrcInstid())//selecting player as caster
                    {
                        if (c.isActivation().getID() > 0)
                        {
                            if (c.isActivation().getID() < 3)
                            {
                                long time = c.getTime() - time_start;
                                curCastLog = new CastLog(time, c.getSkillID(), c.getValue(), c.isActivation());
                            }
                            else
                            {
                                if (curCastLog != null)
                                {
                                    if (curCastLog.getID() == c.getSkillID())
                                    {
                                        curCastLog = new CastLog(curCastLog.getTime(), curCastLog.getID(), curCastLog.getExpDur(), curCastLog.startActivation(), c.getValue(), c.isActivation());
                                        cast_logs.Add(curCastLog);
                                        curCastLog = null;
                                    }
                                }
                            }
                        }
                    }
                }
                else if (state.getID() == 11)
                {//Weapon swap
                    if (instid == c.getSrcInstid())//selecting player as caster
                    {
                        if ((int)c.getDstAgent() == 4 || (int)c.getDstAgent() == 5)
                        {
                            long time = c.getTime() - time_start;
                            curCastLog = new CastLog(time, -2, (int)c.getDstAgent(), c.isActivation());
                            cast_logs.Add(curCastLog);
                            curCastLog = null;
                        }
                    }
                }
            }
        }

    }
}
