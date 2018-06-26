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
        //int is time
        private List<Point3D> positions = new List<Point3D>();
        private List<Point3D> velocities = new List<Point3D>();
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

        public List<Point3D> getPositionList(ParsedLog log)
        {
            if (positions.Count == 0)
            {
                this.setMovements(log);
            }

            return positions;

        }
        public List<Point3D> getVelocityList(ParsedLog log)
        {
            if (velocities.Count == 0)
            {
                this.setMovements(log);
            }
            return velocities;

        }

        private void setMovements(ParsedLog log)
        {
            foreach(CombatItem c in log.getMovementData())
            {
                if (c.getSrcInstid() != agent.getID())
                {
                    continue;
                }
                long time = c.getTime() - log.getBossData().getFirstAware();
                if (time < 0)
                {
                    continue;
                }
                byte[] xy = BitConverter.GetBytes(c.getDstAgent());
                float X = BitConverter.ToSingle(xy, 0);
                float Y = BitConverter.ToSingle(xy, 4);
                if (c.isStateChange() == ParseEnum.StateChange.Position)
                {
                    positions.Add(new Point3D(X, Y, c.getValue(), time));
                } else
                {
                    velocities.Add(new Point3D(X, Y, c.getValue(), time));
                }
            }
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
            foreach (CombatItem c in log.getCastData())
            {
                if (! (c.getTime() > log.getBossData().getFirstAware() && c.getTime() < log.getBossData().getLastAware()))
                {
                    continue;
                }
                ParseEnum.StateChange state = c.isStateChange();
                if (state == ParseEnum.StateChange.Normal)
                {
                    if (agent.getInstid() == c.getSrcInstid())//selecting player as caster
                    {
                        if (c.isActivation().IsCasting())
                        {
                            long time = c.getTime() - time_start;
                            curCastLog = new CastLog(time, c.getSkillID(), c.getValue(), c.isActivation());
                            cast_logs.Add(curCastLog);
                        }
                        else
                        {
                            if (curCastLog != null)
                            {
                                if (curCastLog.getID() == c.getSkillID())
                                {
                                    curCastLog.setEndStatus(c.getValue(), c.isActivation());                               
                                    curCastLog = null;
                                }
                            }
                        }

                    }
                }
                else if (state == ParseEnum.StateChange.WeaponSwap)
                {//Weapon swap
                    if (agent.getInstid() == c.getSrcInstid())//selecting player as caster
                    {
                        if ((int)c.getDstAgent() == 4 || (int)c.getDstAgent() == 5)
                        {
                            long time = c.getTime() - time_start;
                            CastLog swapLog = new CastLog(time, -2, (int)c.getDstAgent(), c.isActivation());
                            cast_logs.Add(swapLog);
                        }
                    }
                }
            }
        }

    }
}
