using LuckParser.Models.DataModels;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class Boss : AbstractMasterPlayer
    {
        // Constructors
        public Boss(AgentItem agent, bool requirePhases) : base(agent)
        {
            _requirePhases = requirePhases;
        }

        private List<PhaseData> _phases = new List<PhaseData>();
        public readonly List<long> PhaseData = new List<long>();
        private CombatReplayMap _map;
        public readonly List<Mob> ThrashMobs = new List<Mob>();
        private readonly bool _requirePhases;

        public List<PhaseData> GetPhases(ParsedLog log)
        {

            if (_phases.Count == 0)
            {
                long fightDuration = log.FightData.FightDuration;
                if (!_requirePhases)
                {
                    _phases.Add(new PhaseData(0, fightDuration));
                    _phases[0].Name = "Full Fight";
                    return _phases;
                }
                GetCastLogs(log, 0, fightDuration);
                _phases = log.FightData.Logic.GetPhases(this, log, CastLogs);
            }
            return _phases;
        }

        public CombatReplayMap GetCombatMap(ParsedLog log)
        {
            if (_map == null)
            {
                _map = log.FightData.Logic.GetCombatMap();
            }
            return _map;
        }

        // Private Methods

        protected override void SetDamageTakenLogs(ParsedLog log)
        {
            // nothing to do
            /*long time_start = log.getBossData().getFirstAware();
            foreach (CombatItem c in log.getDamageTakenData())
            {
                if (agent.getInstid() == c.getDstInstid() && c.getTime() > log.getBossData().getFirstAware() && c.getTime() < log.getBossData().getLastAware())
                {//selecting player as target
                    long time = c.getTime() - time_start;
                    foreach (AgentItem item in log.AgentData.getAllAgentsList())
                    {//selecting all
                        addDamageTakenLog(time, item.getInstid(), c);
                    }
                }
            }*/
        }

        protected override void SetAdditionalCombatReplayData(ParsedLog log, int pollingRate)
        {
            List<ParseEnum.ThrashIDS> ids = log.FightData.Logic.GetAdditionalData(CombatReplay, GetCastLogs(log, 0, log.FightData.FightDuration), log);
            List<AgentItem> aList = log.AgentData.NPCAgentList.Where(x => ids.Contains(ParseEnum.GetThrashIDS(x.ID))).ToList();
            foreach (AgentItem a in aList)
            {
                Mob mob = new Mob(a);
                mob.InitCombatReplay(log, pollingRate, true, false);
                ThrashMobs.Add(mob);
            }
        }

        protected override void SetCombatReplayIcon(ParsedLog log)
        {
            CombatReplay.Icon = log.FightData.Logic.GetReplayIcon();
        }

        public void AddMechanics(ParsedLog log)
        {
            MechanicData mechData = log.MechanicData;
            FightData fightData = log.FightData;
            List<Mechanic> bossMechanics = fightData.Logic.GetMechanics();
            Dictionary<ushort, AbstractMasterPlayer> regroupedMobs = new Dictionary<ushort, AbstractMasterPlayer>();
            // Boons
            foreach (Mechanic m in bossMechanics.Where(x => x.MechanicType == Mechanic.MechType.EnemyBoon || x.MechanicType == Mechanic.MechType.EnemyBoonStrip))
            {
                Mechanic.CheckSpecialCondition condition = m.SpecialCondition;
                foreach (CombatItem c in log.GetBoonData(m.SkillId))
                {
                    if (condition != null && !condition(new SpecialConditionItem(c)))
                    {
                        continue;
                    }
                    AbstractMasterPlayer amp = null;
                    if (m.MechanicType == Mechanic.MechType.EnemyBoon && c.IsBuffRemove == ParseEnum.BuffRemove.None)
                    {
                        if (c.DstInstid == fightData.InstID)
                        {
                            amp = this;
                        }
                        else
                        {
                            AgentItem a = log.AgentData.GetAgent(c.DstAgent);
                            if (!regroupedMobs.TryGetValue(a.ID, out amp))
                            {
                                amp = new DummyPlayer(a);
                                regroupedMobs.Add(a.ID, amp);
                            }
                        }
                    }
                    else if (m.MechanicType == Mechanic.MechType.EnemyBoonStrip && c.IsBuffRemove == ParseEnum.BuffRemove.Manual)
                    {
                        if (c.SrcInstid == fightData.InstID)
                        {
                            amp = this;
                        }
                        else
                        {
                            AgentItem a = log.AgentData.GetAgent(c.SrcAgent);
                            if (!regroupedMobs.TryGetValue(a.ID, out amp))
                            {
                                amp = new DummyPlayer(a);
                                regroupedMobs.Add(a.ID, amp);
                            }
                        }
                    }
                    if (amp != null)
                    {
                        mechData[m].Add(new MechanicLog(c.Time - fightData.FightStart, m, amp));
                    }

                }
            }
            // Casting
            foreach (Mechanic m in bossMechanics.Where(x => x.MechanicType == Mechanic.MechType.EnemyCastEnd || x.MechanicType == Mechanic.MechType.EnemyCastStart))
            {
                Mechanic.CheckSpecialCondition condition = m.SpecialCondition;
                foreach (CombatItem c in log.GetCastDataById(m.SkillId))
                {
                    if (condition != null && !condition(new SpecialConditionItem(c)))
                    {
                        continue;
                    }
                    AbstractMasterPlayer amp = null;
                    if ((m.MechanicType == Mechanic.MechType.EnemyCastStart && c.IsActivation.IsCasting()) || (m.MechanicType == Mechanic.MechType.EnemyCastEnd && !c.IsActivation.IsCasting()))
                    {
                        if (c.SrcInstid == fightData.InstID)
                        {
                            amp = this;
                        }
                        else
                        {
                            AgentItem a = log.AgentData.GetAgent(c.SrcAgent);
                            if (!regroupedMobs.TryGetValue(a.ID, out amp))
                            {
                                amp = new DummyPlayer(a);
                                regroupedMobs.Add(a.ID, amp);
                            }
                        }
                    }
                    if (amp != null)
                    {
                        mechData[m].Add(new MechanicLog(c.Time - fightData.FightStart, m, amp));
                    }
                }

            }
            // Spawn
            foreach (Mechanic m in bossMechanics.Where(x => x.MechanicType == Mechanic.MechType.Spawn))
            {
                foreach (AgentItem a in log.AgentData.NPCAgentList.Where(x => x.ID == m.SkillId))
                {
                    if (!regroupedMobs.TryGetValue(a.ID, out AbstractMasterPlayer amp))
                    {
                        amp = new DummyPlayer(a);
                        regroupedMobs.Add(a.ID, amp);
                    }
                    mechData[m].Add(new MechanicLog(a.FirstAware - fightData.FightStart, m, amp));
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