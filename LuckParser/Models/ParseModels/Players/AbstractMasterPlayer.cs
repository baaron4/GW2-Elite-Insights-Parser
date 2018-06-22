using LuckParser.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public abstract class AbstractMasterPlayer : AbstractPlayer
    {
        // Rotation
        private List<RotationItem> rotation = new List<RotationItem>();
        // Minions
        private Dictionary<string, Minions> minions = new Dictionary<string, Minions>();

        public AbstractMasterPlayer(AgentItem agent) : base(agent)
        {

        }


        public Dictionary<string, Minions> getMinions(ParsedLog log)
        {
            if (minions.Count == 0)
            {
                setMinions(log);
            }
            return minions;
        }

        public List<RotationItem> getRotation(ParsedLog log, bool icons)
        {
            if (rotation.Count == 0)
            {
                setRotation(log, icons);
            }
            return rotation;
        }

        private void setRotation(ParsedLog log, bool icons)
        {
            List<CastLog> cls = getCastLogs(log, 0, log.getBossData().getAwareDuration());
            foreach (CastLog cl in cls)
            {
                RotationItem rot = new RotationItem();
                rot.findName(log.getSkillData(), cl.getID());
                rot.setDuration(cl.getActDur());
                rot.setEndStatus(cl.endActivation());
                rot.setStartStatus(cl.startActivation());
            }
        }

        private void setMinions(ParsedLog log)
        {
            List<AgentItem> combatMinion = log.getAgentData().getNPCAgentList().Where(x => x.getMasterAgent() == agent.getAgent()).ToList();
            foreach (AgentItem agent in combatMinion)
            {
                string id = agent.getName();
                if (!minions.ContainsKey(id))
                {
                    minions[id] = new Minions(id.GetHashCode());
                }
                minions[id].Add(new Minion(agent.getInstid(), agent));
            }
        }

        protected override void setFilteredLogs(ParsedLog log)
        {
            long time_start = log.getBossData().getFirstAware();
            foreach (CombatItem c in log.getDamageData())
            {
                if (agent.getInstid() == c.getSrcInstid() && c.getTime() > log.getBossData().getFirstAware() && c.getTime() < log.getBossData().getLastAware())
                {
                    long time = c.getTime() - time_start;
                    addDamageLog(time, log.getBossData().getInstid(), c, damage_logsFiltered);
                }
            }
            Dictionary<string, Minions> min_list = getMinions(log);
            foreach (Minions mins in min_list.Values)
            {
                damage_logsFiltered.AddRange(mins.getDamageLogs(log.getBossData().getInstid(), log, 0, log.getBossData().getAwareDuration()));
            }
            damage_logsFiltered.Sort((x, y) => x.getTime() < y.getTime() ? -1 : 1);
        }

        protected override void setCastLogs(ParsedLog log)
        {
            long time_start = log.getBossData().getFirstAware();
            CastLog curCastLog = null;

            foreach (CombatItem c in log.getCombatList())
            {
                if (! (c.getTime() > log.getBossData().getFirstAware() && c.getTime() < log.getBossData().getLastAware()))
                {
                    continue;
                }
                LuckParser.Models.ParseEnums.StateChange state = c.isStateChange();
                if (state.getID() == 0)
                {
                    if (agent.getInstid() == c.getSrcInstid())//selecting player as caster
                    {
                        if (c.isActivation() != ParseEnum.Activation.None)
                        {
                            if (DataModels.ParseEnum.casting(c.isActivation()))
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
                    if (agent.getInstid() == c.getSrcInstid())//selecting player as caster
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
