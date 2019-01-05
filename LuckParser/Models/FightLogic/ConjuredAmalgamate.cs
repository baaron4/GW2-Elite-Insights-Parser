using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static LuckParser.Models.DataModels.ParseEnum.TrashIDS;

namespace LuckParser.Models.Logic
{
    public class ConjuredAmalgamate : RaidLogic
    {
        public ConjuredAmalgamate(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new Mechanic(52173, "Pulverize", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("square","rgb(255,140,0)"), "Arm Slam","Pulverize (Arm Slam)", "Arm Slam",0),
            new Mechanic(52086, "Junk Absorption", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("circle-open","rgb(150,0,150)"), "Balls","Junk Absorption (Purple Balls during collect)", "Purple Balls",0),
            new Mechanic(52878, "Junk Fall", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("circle-open","rgb(255,150,0)"), "Junk","Junk Fall (Falling Debris)", "Junk Fall",0),
            new Mechanic(52120, "Junk Fall", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("circle-open","rgb(255,150,0)"), "Junk","Junk Fall (Falling Debris)", "Junk Fall",0),
            new Mechanic(52161, "Ruptured Ground", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("square-open","rgb(0,255,255)"), "Ground","Ruptured Ground (Relics after Junk Wall)", "Ruptured Ground",0,(condition => condition.DamageLog.Damage > 0)),
            new Mechanic(52656, "Tremor", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("circle-open","rgb(255,0,0)"), "Tremor","Tremor (Field adjacent to Arm Slam)", "Near Arm Slam",0,(condition => condition.DamageLog.Damage > 0)),
            new Mechanic(52150, "Junk Torrent", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("square-open","rgb(255,0,0)"), "Wall","Junk Torrent (Moving Wall)", "Junk Torrent (Wall)",0,(condition => condition.DamageLog.Damage > 0)),
            }); 
            Extension = "ca";
            IconUrl = "https://i.imgur.com/eLyIWd2.png";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/9PJB5Ky.png",
                            Tuple.Create(1414, 2601),
                            Tuple.Create(-5064, -15030, -2864, -10830),
                            Tuple.Create(-21504, -21504, 24576, 24576),
                            Tuple.Create(13440, 14336, 15360, 16256));
        }

        protected override List<ushort> GetFightTargetsIDs()
        {
            return new List<ushort>
            {
                (ushort)ParseEnum.TargetIDS.ConjuredAmalgamate,
                (ushort)ParseEnum.TargetIDS.CARightArm,
                (ushort)ParseEnum.TargetIDS.CALeftArm
            };
        }

        protected override List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>()
            {
                ConjuredGreatsword,
                ConjuredShield
            };
        }

        public override void SpecialParse(FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            Random rnd = new Random();
            ulong agent = 0;
            while (agentData.AgentValues.Contains(agent) || agent == 0)
            {
                agent = (ulong)rnd.Next(Int32.MaxValue/2, Int32.MaxValue);
            }
            ushort instID = 0;
            while (agentData.InstIDValues.Contains(instID) || instID == 0)
            {
                instID = (ushort)rnd.Next(ushort.MaxValue / 2, ushort.MaxValue);
            }
            AgentItem sword = new AgentItem(agent, "Conjured Sword\0:Conjured Sword\050", "Sword", AgentItem.AgentType.Player, 0, 0, 0, 0, 20, 20)
            {
                InstID = instID,
                LastAware = combatData.Last().Time,
                FirstAware = combatData.First().Time,
                MasterAgent = 0
            };
            agentData.AddCustomAgent(sword);
            foreach(CombatItem cl in combatData)
            {
                if (cl.SkillID == 52370 && cl.IsStateChange == ParseEnum.StateChange.Normal && cl.IsBuffRemove == ParseEnum.BuffRemove.None &&
                                        ((cl.IsBuff == 1 && cl.BuffDmg >= 0 && cl.Value == 0) ||
                                        (cl.IsBuff == 0 && cl.Value >= 0)) && cl.DstInstid != 0 && cl.IFF == ParseEnum.IFF.Foe)
                {
                    cl.SrcAgent = sword.Agent;
                    cl.SrcInstid = sword.InstID;
                    cl.SrcMasterInstid = 0;
                }
            }
        }

        public override void ComputeAdditionalThrashMobData(Mob mob, ParsedLog log)
        {
            switch (mob.ID)
            {
                case (ushort)ConjuredGreatsword:
                    break;
                case (ushort)ConjuredShield:
                    CombatReplay replay = mob.CombatReplay;
                    List<CombatItem> shield = GetFilteredList(log, 53003, mob, true);
                    int shieldStart = 0;
                    foreach (CombatItem c in shield)
                    {
                        if (c.IsBuffRemove == ParseEnum.BuffRemove.None)
                        {
                            shieldStart = (int)(log.FightData.ToFightSpace(c.Time));
                        }
                        else
                        {
                            int shieldEnd = (int)(log.FightData.ToFightSpace(c.Time));
                            Tuple<int, int> lifespan = new Tuple<int, int>(shieldStart, shieldEnd);
                            int radius = 100;
                            replay.Actors.Add(new CircleActor(true, 0, radius, lifespan, "rgba(0, 150, 255, 0.3)", new AgentConnector(mob)));
                        }
                    }
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }

        public override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            Target ca = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.ConjuredAmalgamate);
            if (ca == null)
            {
                throw new InvalidOperationException("Conjurate Amalgamate not found");
            }
            phases[0].Targets.Add(ca);
            if (!requirePhases)
            {
                return phases;
            }
            phases.AddRange(GetPhasesByInvul(log, 52255, ca, true, false));
            phases.RemoveAll(x => x.GetDuration() < 1000);
            for (int i = 1; i < phases.Count; i++)
            {
                string name;
                PhaseData phase = phases[i];
                if (i % 2 == 1)
                {
                    name = "Arm Phase";
                }
                else
                {
                    name = "Burn Phase";
                    phase.Targets.Add(ca);
                }
                phase.Name = name;
            }
            Target leftArm = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.CALeftArm);
            if (leftArm != null)
            {
                List<long> leftArmDown = log.GetBoonData(52430).Where(x => x.IsBuffRemove == ParseEnum.BuffRemove.All && x.SrcInstid == leftArm.InstID).Select(x => log.FightData.ToFightSpace(x.Time)).ToList();
                for (int i = 1; i < phases.Count; i += 2)
                {
                    PhaseData phase = phases[i];
                    if (leftArmDown.Exists(x => phase.InInterval(x)))
                    {
                        phase.Name = "Left " + phase.Name;
                        phase.Targets.Add(leftArm);
                    }
                }
            }
            Target rightArm = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.CARightArm);
            if (rightArm != null)
            {
                List<long> rightArmDown = log.GetBoonData(52430).Where(x => x.IsBuffRemove == ParseEnum.BuffRemove.All && x.SrcInstid == rightArm.InstID).Select(x => log.FightData.ToFightSpace(x.Time)).ToList();
                for (int i = 1; i < phases.Count; i += 2)
                {
                    PhaseData phase = phases[i];
                    if (rightArmDown.Exists(x => phase.InInterval(x)))
                    {
                        if (phase.Name.Contains("Left"))
                        {
                            phase.Name = "Both Arms Phase";
                        }
                        else
                        {
                            phase.Name = "Right " + phase.Name;
                        }
                        phase.Targets.Add(rightArm);
                    }
                }
            }
            return phases;
        }

        public override void ComputeAdditionalTargetData(Target target, ParsedLog log)
        {
            CombatReplay replay = target.CombatReplay;
            List<Point3D> facings = replay.Rotations;
            if (facings.Any())
            {
                replay.Actors.Add(new FacingActor(new Tuple<int, int>((int)replay.TimeOffsets.Item1, (int)replay.TimeOffsets.Item2), new AgentConnector(target), facings));
            }
            List<CastLog> cls = target.GetCastLogs(log, 0, log.FightData.FightDuration);

            switch (target.ID)
            {
                case (ushort)ParseEnum.TargetIDS.ConjuredAmalgamate:
                    List<CombatItem> shield = GetFilteredList(log, 53003, target, true);
                    int shieldStart = 0;
                    foreach (CombatItem c in shield)
                    {
                        if (c.IsBuffRemove == ParseEnum.BuffRemove.None)
                        {
                            shieldStart = (int)(log.FightData.ToFightSpace(c.Time));
                        }
                        else
                        {
                            int shieldEnd = (int)(log.FightData.ToFightSpace(c.Time));
                            Tuple<int, int> lifespan = new Tuple<int, int>(shieldStart, shieldEnd);
                            int radius = 500;
                            replay.Actors.Add(new CircleActor(true, 0, radius, lifespan, "rgba(0, 150, 255, 0.3)", new AgentConnector(target)));
                        }
                    }
                    break;
                case (ushort)ParseEnum.TargetIDS.CALeftArm:
                case (ushort)ParseEnum.TargetIDS.CARightArm:
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }

        public override void ComputeAdditionalPlayerData(Player p, ParsedLog log)
        {
            CombatReplay replay = p.CombatReplay;
            List<CastLog> cls = p.GetCastLogs(log, 0, log.FightData.FightDuration);
            List<CastLog> shieldCast = cls.Where(x => x.SkillId == 52780).ToList();
            foreach (CastLog c in shieldCast)
            {
                int start = (int)c.Time;
                int duration = 10000;
                Tuple<int, int> lifespan = new Tuple<int, int>(start, start + duration);
                int radius = 300;
                Point3D shieldNextPos = replay.Positions.FirstOrDefault(x => x.Time >= start);
                Point3D shieldPrevPos = replay.Positions.LastOrDefault(x => x.Time <= start);
                if (shieldNextPos != null || shieldPrevPos != null)
                {
                    replay.Actors.Add(new CircleActor(true, 0, radius, lifespan, "rgba(255, 0, 255, 0.1)", new InterpolatedPositionConnector(shieldPrevPos, shieldNextPos, start)));
                    replay.Actors.Add(new CircleActor(false, 0, radius, lifespan, "rgba(255, 0, 255, 0.3)", new InterpolatedPositionConnector(shieldPrevPos, shieldNextPos, start)));
                }
            }
        }

        public override int IsCM(ParsedLog log)
        {
            Target target = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.ConjuredAmalgamate);
            if (target == null)
            {
                throw new InvalidOperationException("Target for CM detection not found");
            }
            return log.CombatData.GetBoonData(53075).Count > 0 ? 1 : 0;
        }
    }
}
