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

        private List<PhaseData> _phases = new List<PhaseData>();
        private List<long> _phaseData = new List<long>();
        private CombatReplayMap _map = null;
        private List<Mob> _thrashMobs = new List<Mob>();

        public List<PhaseData> GetPhases(ParsedLog log, bool getAllPhases)
        {

            if (_phases.Count == 0)
            {
                long fightDuration = log.GetBossData().GetAwareDuration();
                if (!getAllPhases)
                {
                    _phases.Add(new PhaseData(0, fightDuration));
                    _phases[0].SetName("Full Fight");
                    return _phases;
                }
                GetCastLogs(log, 0, fightDuration);
                _phases = log.GetBossData().GetBossBehavior().GetPhases(this, log, CastLogs);
            }
            return _phases;
        }

        public void AddPhaseData(long data)
        {
            _phaseData.Add(data);
        }

        public List<long> GetPhaseData()
        {
            return _phaseData;
        }

        public CombatReplayMap GetCombatMap(ParsedLog log)
        {
            if (_map == null)
            {
                _map = log.GetBossData().GetBossBehavior().GetCombatMap();
            }
            return _map;
        }

        public List<Mob> GetThrashMobs()
        {
            return _thrashMobs;
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
            List<ParseEnum.ThrashIDS> ids = log.GetBossData().GetBossBehavior().GetAdditionalData(Replay, GetCastLogs(log, 0, log.GetBossData().GetAwareDuration()), log);
            List<AgentItem> aList = log.GetAgentData().GetNPCAgentList().Where(x => ids.Contains(ParseEnum.GetThrashIDS(x.GetID()))).ToList();
            foreach (AgentItem a in aList)
            {
                Mob mob = new Mob(a);
                mob.InitCombatReplay(log, pollingRate, true, false);
                _thrashMobs.Add(mob);
            }
        }

        protected override void SetCombatReplayIcon(ParsedLog log)
        {
            Replay.SetIcon(log.GetBossData().GetBossBehavior().GetReplayIcon());
        }

        public override void AddMechanics(ParsedLog log)
        {
            MechanicData mechData = log.GetMechanicData();
            BossData bossData = log.GetBossData();
            List<Mechanic> bossMechanics = bossData.GetBossBehavior().GetMechanics();
            // Boons
            List<Mechanic> enemyBoons = bossMechanics.Where(x => x.GetMechType() == Mechanic.MechType.EnemyBoon || x.GetMechType() == Mechanic.MechType.EnemyBoonStrip).ToList();
            foreach (Mechanic m in enemyBoons)
            {
                Mechanic.SpecialCondition condition = m.GetSpecialCondition();
                foreach (CombatItem c in log.GetBoonData())
                {
                    if (m.GetSkill() == c.GetSkillID())
                    {
                        if (condition != null && !condition(c.GetValue()))
                        {
                            continue;
                        }
                        AbstractMasterPlayer amp = null;
                        if (m.GetMechType() == Mechanic.MechType.EnemyBoon && c.IsBuffremove() == ParseEnum.BuffRemove.None)
                        {
                            if (c.GetDstInstid() == bossData.GetInstid())
                            {
                                amp = this;
                            }
                            else
                            {
                                amp = new Mob(log.GetAgentData().GetAgent(c.GetDstAgent()));
                            }
                        } else if (m.GetMechType() == Mechanic.MechType.EnemyBoonStrip && c.IsBuffremove() == ParseEnum.BuffRemove.Manual)
                        {
                            if (c.GetSrcInstid() == bossData.GetInstid())
                            {
                                amp = this;
                            }
                            else
                            {
                                amp = new Mob(log.GetAgentData().GetAgent(c.GetSrcAgent()));
                            }

                        }
                        if (amp != null)
                        {
                            mechData[m].Add(new MechanicLog(c.GetTime() - bossData.GetFirstAware(), m, amp));
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
                    if (skill == c.GetSkillID())
                    {
                        if (condition != null && !condition(c.GetValue()))
                        {
                            continue;
                        }
                        AbstractMasterPlayer amp = null;
                        if ((m.GetMechType() == Mechanic.MechType.EnemyCastStart && c.IsActivation().IsCasting()) || (m.GetMechType() == Mechanic.MechType.EnemyCastEnd && !c.IsActivation().IsCasting()))
                        {
                            if (c.GetSrcInstid() == bossData.GetInstid())
                            {
                                amp = this;
                            }
                            else
                            {
                                amp = new DummyPlayer(log.GetAgentData().GetAgent(c.GetSrcAgent()));
                            }
                        }
                        if (amp != null)
                        {
                            mechData[m].Add(new MechanicLog(c.GetTime() - bossData.GetFirstAware(), m, amp));
                        }
                    }
                }
            }
            // Spawn
            List<Mechanic> spawnMech = bossMechanics.Where(x => x.GetMechType() == Mechanic.MechType.Spawn).ToList();
            foreach (Mechanic m in spawnMech)
            {
                foreach (AgentItem a in log.GetAgentData().GetNPCAgentList().Where(x => x.GetID() == m.GetSkill()))
                {
                    AbstractMasterPlayer amp = new DummyPlayer(a);
                    mechData[m].Add(new MechanicLog(a.GetFirstAware() - bossData.GetFirstAware(), m, amp));
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