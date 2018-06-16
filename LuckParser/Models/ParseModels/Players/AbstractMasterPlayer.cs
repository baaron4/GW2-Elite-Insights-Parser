using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public abstract class AbstractMasterPlayer : AbstractPlayer
    {

        // Minions
        private Dictionary<string, Minions> minions = new Dictionary<string, Minions>();

        public AbstractMasterPlayer(AgentItem agent) : base(agent)
        {

        }


        public Dictionary<string, Minions> getMinions(BossData bossData, List<CombatItem> combatList, AgentData agentData)
        {
            if (minions.Count == 0)
            {
                setMinions(bossData, combatList, agentData);
            }
            return minions;
        }

        private void setMinions(BossData bossData, List<CombatItem> combatList, AgentData agentData)
        {
            List<AgentItem> combatMinion = agentData.getNPCAgentList().Where(x => x.getMasterAgent() == agent.getAgent()).ToList();
            foreach (AgentItem agent in combatMinion)
            {
                if (agent != null)
                {
                    string id = agent.getName();
                    if (!minions.ContainsKey(id))
                    {
                        minions[id] = new Minions();
                    }
                    minions[id].Add(new Minion(agent.getInstid(), agent));
                }
            }
        }

        protected override void setFilteredLogs(BossData bossData, List<CombatItem> combatList, AgentData agentData)
        {
            long time_start = bossData.getFirstAware();
            foreach (CombatItem c in combatList)
            {
                if (agent.getInstid() == c.getSrcInstid() && c.getTime() > agent.getFirstAware() && c.getTime() < agent.getLastAware())
                {
                    long time = c.getTime() - time_start;
                    addDamageLog(time, bossData.getInstid(), c, damage_logsFiltered);
                }
            }
            Dictionary<string, Minions> min_list = getMinions(bossData, combatList, agentData);
            foreach (Minions mins in min_list.Values)
            {
                damage_logsFiltered.AddRange(mins.getDamageLogs(bossData.getInstid(), bossData, combatList, agentData, 0, bossData.getAwareDuration()));
            }
        }

        protected override void setCastLogs(BossData bossData, List<CombatItem> combatList, AgentData agentData)
        {
            long time_start = bossData.getFirstAware();
            CastLog curCastLog = null;

            foreach (CombatItem c in combatList)
            {
                LuckParser.Models.ParseEnums.StateChange state = c.isStateChange();
                if (state.getID() == 0)
                {
                    if (agent.getInstid() == c.getSrcInstid() && c.getTime() > agent.getFirstAware() && c.getTime() < agent.getLastAware())//selecting player as caster
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
                    if (agent.getInstid() == c.getSrcInstid() && c.getTime() > agent.getFirstAware() && c.getTime() < agent.getLastAware())//selecting player as caster
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
