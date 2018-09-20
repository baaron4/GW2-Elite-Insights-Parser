using LuckParser.Models.DataModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class Boss : AbstractMasterPlayer
    {
        // Constructors
        public Boss(AgentItem agent) : base(agent)
        {
        }

        public int Health { get; set; } = -1;
        public List<Point> HealthOverTime { get; set; } = new List<Point>();
        public readonly List<Mob> TrashMobs = new List<Mob>();

        public void AddCustomCastLog(CastLog cl, ParsedLog log)
        {
            if (CastLogs.Count == 0)
            {
                GetCastLogs(log, 0, log.FightData.FightEnd);
            }
            CastLogs.Add(cl);
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
            List<ParseEnum.TrashIDS> ids = log.FightData.Logic.GetAdditionalBossData(CombatReplay, GetCastLogs(log, 0, log.FightData.FightDuration), log);
            List<AgentItem> aList = log.AgentData.NPCAgentList.Where(x => ids.Contains(ParseEnum.GetTrashIDS(x.ID))).ToList();
            foreach (AgentItem a in aList)
            {
                Mob mob = new Mob(a);
                mob.InitCombatReplay(log, pollingRate, true, false);
                TrashMobs.Add(mob);
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
            List<Mechanic> bossMechanics = fightData.Logic.MechanicList;
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
                        if (c.DstInstid == InstID)
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
                        if (c.SrcInstid == InstID)
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
                        if (c.SrcInstid == InstID)
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
        //
        private class Serializable
        {
            public string Img { get; set; }
            public string Type { get; set; }
            public int[] Positions { get; set; }
        }

        public override string GetCombatReplayJSON(CombatReplayMap map)
        {
            Serializable aux = new Serializable
            {
                Img = CombatReplay.Icon,
                Type = "Boss",
                Positions = new int[2 * CombatReplay.Positions.Count]
            };
            int i = 0;
            foreach (Point3D pos in CombatReplay.Positions)
            {
                Tuple<int, int> coord = map.GetMapCoord(pos.X, pos.Y);
                aux.Positions[i++] = coord.Item1;
                aux.Positions[i++] = coord.Item2;
            }

            return JsonConvert.SerializeObject(aux);
        }

        public override int GetCombatReplayID()
        {
            return 0;
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