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
            List<ushort> combatMinionIDList = combatList.Where(x => x.getSrcMasterInstid() == instid).Select(x => x.getSrcInstid()).Distinct().ToList();
            foreach (ushort petid in combatMinionIDList)
            {
                AgentItem agent = agentData.getNPCAgentList().FirstOrDefault(x => x.getInstid() == petid);
                if (agent != null)
                {
                    List<DamageLog> damageLogs = getDamageLogs(0, bossData, combatList, agentData, 0, bossData.getAwareDuration()).Where(x => x.getInstidt() == petid).ToList();
                    if (damageLogs.Count == 0)
                    {
                        continue;
                    }
                    string id = agent.getName();
                    if (!minions.ContainsKey(id))
                    {
                        minions[id] = new Minions();
                    }
                    minions[id].Add(new Minion(instid, agent));
                }
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
