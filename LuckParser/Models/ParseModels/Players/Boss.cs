using LuckParser.Controllers;
using LuckParser.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class Boss : AbstractMasterPlayer
    {
        // Constructors
        public Boss(AgentItem agent) : base(agent)
        {
        }

        private List<PhaseData> phases = new List<PhaseData>();
        private List<long> phaseData = new List<long>();
        private CombatReplayMap map = null;
        private List<Mob> thrashMobs = new List<Mob>();

        public List<PhaseData> getPhases(ParsedLog log, bool getAllPhases)
        {

            if (phases.Count == 0)
            {
                long fight_dur = log.GetBossData().getAwareDuration();
                if (!getAllPhases)
                {
                    phases.Add(new PhaseData(0, fight_dur));
                    phases[0].setName("Full Fight");
                    return phases;
                }
                GetCastLogs(log, 0, fight_dur);
                phases = log.GetBossData().getBossBehavior().getPhases(this, log, cast_logs);
            }
            return phases;
        }

        public void addPhaseData(long data)
        {
            phaseData.Add(data);
        }

        public List<long> getPhaseData()
        {
            return phaseData;
        }

        public CombatReplayMap getCombatMap(ParsedLog log)
        {
            if (map == null)
            {
                map = log.GetBossData().getBossBehavior().getCombatMap();
            }
            return map;
        }

        public List<Mob> getThrashMobs()
        {
            return thrashMobs;
        }

        // Private Methods

        protected override void SetDamagetakenLogs(ParsedLog log)
        {
            // nothing to do
            /*long time_start = log.getBossData().getFirstAware();
            foreach (CombatItem c in log.getDamageTakenData())
            {
                if (agent.getInstid() == c.getDstInstid() && c.getTime() > log.getBossData().getFirstAware() && c.getTime() < log.getBossData().getLastAware())
                {//selecting player as target
                    long time = c.getTime() - time_start;
                    foreach (AgentItem item in log.getAgentData().getAllAgentsList())
                    {//selecting all
                        addDamageTakenLog(time, item.getInstid(), c);
                    }
                }
            }*/
        }

        protected override void SetAdditionalCombatReplayData(ParsedLog log, int pollingRate)
        {
            List<ParseEnum.ThrashIDS> ids = log.GetBossData().getBossBehavior().getAdditionalData(replay, GetCastLogs(log, 0, log.GetBossData().getAwareDuration()), log);
            List<AgentItem> aList = log.GetAgentData().getNPCAgentList().Where(x => ids.Contains(ParseEnum.getThrashIDS(x.getID()))).ToList();
            foreach (AgentItem a in aList)
            {
                Mob mob = new Mob(a);
                mob.InitCombatReplay(log, pollingRate, true, false);
                thrashMobs.Add(mob);
            }
        }

        protected override void SetCombatReplayIcon(ParsedLog log)
        {
            replay.setIcon(log.GetBossData().getBossBehavior().getReplayIcon());
        }

        public override void AddMechanics(ParsedLog log)
        {
            MechanicData mech_data = log.GetMechanicData();
            BossData boss_data = log.GetBossData();
            List<Mechanic> bossMechanics = boss_data.getBossBehavior().getMechanics();
            // Boons
            List<Mechanic> enemyBoons = bossMechanics.Where(x => x.GetMechType() == Mechanic.MechType.EnemyBoon || x.GetMechType() == Mechanic.MechType.EnemyBoonStrip).ToList();
            foreach (Mechanic m in enemyBoons)
            {
                Mechanic.SpecialCondition condition = m.GetSpecialCondition();
                foreach (CombatItem c in log.GetBoonData())
                {
                    if (m.GetSkill() == c.getSkillID())
                    {
                        if (condition != null && !condition(c.getValue()))
                        {
                            continue;
                        }
                        AbstractMasterPlayer amp = null;
                        if (m.GetMechType() == Mechanic.MechType.EnemyBoon && c.isBuffremove() == ParseEnum.BuffRemove.None)
                        {
                            if (c.getDstInstid() == boss_data.getInstid())
                            {
                                amp = this;
                            }
                            else
                            {
                                amp = new Mob(log.GetAgentData().GetAgent(c.getDstAgent()));
                            }
                        } else if (m.GetMechType() == Mechanic.MechType.EnemyBoonStrip && c.isBuffremove() == ParseEnum.BuffRemove.Manual)
                        {
                            if (c.getSrcInstid() == boss_data.getInstid())
                            {
                                amp = this;
                            }
                            else
                            {
                                amp = new Mob(log.GetAgentData().GetAgent(c.getSrcAgent()));
                            }

                        }
                        if (amp != null)
                        {
                            mech_data[m].Add(new MechanicLog(c.getTime() - boss_data.getFirstAware(), m, amp));
                        }
                    }
                }
            }
            // Casting
            List<Mechanic> enemyCasts = bossMechanics.Where(x => x.GetMechType() == Mechanic.MechType.EnemyCastEnd || x.GetMechType() == Mechanic.MechType.EnemyCastStart).ToList();
            foreach (Mechanic m in enemyCasts)
            {
                Mechanic.SpecialCondition condition = m.GetSpecialCondition();
                foreach (CombatItem c in log.GetCastData())
                {
                    long skill = m.GetSkill();
                    if (skill == c.getSkillID())
                    {
                        if (condition != null && !condition(c.getValue()))
                        {
                            continue;
                        }
                        AbstractMasterPlayer amp = null;
                        if ((m.GetMechType() == Mechanic.MechType.EnemyCastStart && c.isActivation().IsCasting()) || (m.GetMechType() == Mechanic.MechType.EnemyCastEnd && !c.isActivation().IsCasting()))
                        {
                            if (c.getSrcInstid() == boss_data.getInstid())
                            {
                                amp = this;
                            }
                            else
                            {
                                amp = new DummyPlayer(log.GetAgentData().GetAgent(c.getSrcAgent()));
                            }
                        }
                        if (amp != null)
                        {
                            mech_data[m].Add(new MechanicLog(c.getTime() - boss_data.getFirstAware(), m, amp));
                        }
                    }
                }
            }
            // Spawn
            List<Mechanic> spawnMech = bossMechanics.Where(x => x.GetMechType() == Mechanic.MechType.Spawn).ToList();
            foreach (Mechanic m in spawnMech)
            {
                foreach (AgentItem a in log.GetAgentData().getNPCAgentList().Where(x => x.getID() == m.GetSkill()))
                {
                    AbstractMasterPlayer amp = new DummyPlayer(a);
                    mech_data[m].Add(new MechanicLog(a.getFirstAware() - boss_data.getFirstAware(), m, amp));
                }
            }
        }

        /*protected override void setHealingLogs(ParsedLog log)
        {
            // nothing to do
        }

        protected override void setHealingReceivedLogs(ParsedLog log)
        {
            // nothing to do
        }*/
    }
}