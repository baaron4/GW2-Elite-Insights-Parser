using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static LuckParser.Models.DataModels.ParseEnum.TrashIDS;

namespace LuckParser.Models.Logic
{
    public class KeepConstruct : RaidLogic
    {
        public KeepConstruct(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new Mechanic(34912, "Fixate", Mechanic.MechType.PlayerBoon, new MechanicPlotlySetting("star","rgb(255,0,255)"), "Fixt","Fixated by Statue", "Fixated",0),
            new Mechanic(34925, "Fixate", Mechanic.MechType.PlayerBoon, new MechanicPlotlySetting("star","rgb(255,0,255)"), "Fixt","Fixated by Statue", "Fixated",0),
            new Mechanic(35077, "Hail of Fury", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("circle-open","rgb(255,0,0)"), "Debris","Hail of Fury (Falling Debris)", "Debris",0),
            new Mechanic(35096, "Compromised", Mechanic.MechType.EnemyBoon, new MechanicPlotlySetting("hexagon","rgb(0,0,255)"), "Rift#","Compromised (Pushed Orb through Rifts)", "Compromised",0),
            new Mechanic(16227, "Insidious Projection", Mechanic.MechType.Spawn, new MechanicPlotlySetting("bowtie","rgb(255,0,0)"), "Merge","Insidious Projection spawn (2 Statue merge)", "Merged Statues",0),
            new Mechanic(35137, "Phantasmal Blades", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("hexagram-open","rgb(255,0,255)"), "PhBlds","Phantasmal Blades (rotating Attack)", "Phantasmal Blades",0),
            new Mechanic(34971, "Phantasmal Blades", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("hexagram-open","rgb(255,0,255)"), "PhBlds","Phantasmal Blades (rotating Attack)", "Phantasmal Blades",0),
            new Mechanic(35064, "Phantasmal Blades", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("hexagram-open","rgb(255,0,255)"), "PhBlds","Phantasmal Blades (rotating Attack)", "Phantasmal Blades",0),
            new Mechanic(35086, "Tower Drop", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("circle","rgb(255,140,0)"), "Jump","Tower Drop (KC Jump)", "Tower Drop",0),
            new Mechanic(35103, "Xera's Fury", Mechanic.MechType.PlayerBoon, new MechanicPlotlySetting("circle","rgb(200,140,0)"), "Bmb","Xera's Fury (Large Bombs) application", "Bombs",0),
            new Mechanic(16261, "Core Hit", Mechanic.MechType.HitOnEnemy, new MechanicPlotlySetting("star-open","rgb(255,140,0)"), "C.Hit","Core was Hit by Player", "Core Hit",1000)
            });
            Extension = "kc";
            IconUrl = "https://wiki.guildwars2.com/images/e/ea/Mini_Keep_Construct.png";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/RZbs21b.png",
                            Tuple.Create(1099, 1114),
                            Tuple.Create(-5467, 8069, -2282, 11297),
                            Tuple.Create(-12288, -27648, 12288, 27648),
                            Tuple.Create(1920, 12160, 2944, 14464));
        }

        public override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            long start = 0;
            long end = 0;
            long fightDuration = log.FightData.FightDuration;
            List<PhaseData> phases = GetInitialPhase(log);
            Target mainTarget = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.KeepConstruct);
            if (mainTarget == null)
            {
                throw new InvalidOperationException("Main target of the fight not found");
            }
            phases[0].Targets.Add(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            // Main phases 34894
            List<CastLog> castLogs = mainTarget.GetCastLogs(log, 0, log.FightData.FightEnd);
            List<CastLog> magicCharge = castLogs.Where(x => x.SkillId == 35048).ToList();
            List<CastLog> magicBlast = castLogs.Where(x => x.SkillId == 34894).ToList();
            foreach (CastLog cl in magicCharge)
            {
                end = cl.Time;
                phases.Add(new PhaseData(start, end));
                CastLog blast = magicBlast.FirstOrDefault(x => x.Time >= cl.Time);
                if (blast != null)
                {
                    start = blast.Time + blast.ActualDuration;
                }
                else
                {
                    start = end + cl.ActualDuration;
                }
            }
            if (fightDuration - start > 5000 && start >= phases.Last().End)
            {
                phases.Add(new PhaseData(start, fightDuration));
                start = fightDuration;
            }
            for (int i = 1; i < phases.Count; i++)
            {
                phases[i].Name = "Phase " + i;
                phases[i].Targets.Add(mainTarget);
            }
            // add burn phases
            int offset = phases.Count;
            List<CombatItem> orbItems = log.GetBoonData(35096).Where(x => x.DstInstid == mainTarget.InstID).ToList();
            // Get number of orbs and filter the list
            start = 0;
            int orbCount = 0;
            List<BoonsGraphModel.Segment> segments = new List<BoonsGraphModel.Segment>();
            foreach (CombatItem c in orbItems)
            {
                if (c.IsBuffRemove == ParseEnum.BuffRemove.None)
                {
                    if (start == 0)
                    {
                        start = log.FightData.ToFightSpace(c.Time);
                    }
                    orbCount++;
                }
                else if (start != 0)
                {
                    segments.Add(new BoonsGraphModel.Segment(start, Math.Min(log.FightData.ToFightSpace(c.Time), fightDuration), orbCount));
                    orbCount = 0;
                    start = 0;
                }
            }
            int burnCount = 1;
            foreach (var seg in segments)
            {
                var phase = new PhaseData(seg.Start, seg.End)
                {
                    Name = "Burn " + burnCount++ + " (" + seg.Value + " orbs)",
                };
                phase.Targets.Add(mainTarget);
                phases.Add(phase);
            }
            phases.Sort((x, y) => x.Start.CompareTo(y.Start));
            return phases;
        }

        protected override List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>
            {
                Core,
                Jessica,
                Olson,
                Engul,
                Faerla,
                Caulle,
                Henley,
                Galletta,
                Ianim,
                GreenPhantasm,
                InsidiousProjection,
                UnstableLeyRift,
                RadiantPhantasm,
                CrimsonPhantasm,
                RetrieverProjection
            };
        }

        public override void ComputeAdditionalThrashMobData(Mob mob, ParsedLog log)
        {
            CombatReplay replay = mob.CombatReplay;
            int start = (int)replay.TimeOffsets.Item1;
            int end = (int)replay.TimeOffsets.Item2;
            Tuple<int, int> lifespan = new Tuple<int, int>(start, end);
            switch (mob.ID)
            {
                case (ushort)Core:
                    break;
                case (ushort)Jessica:
                case (ushort)Olson:
                case (ushort)Engul:
                case (ushort)Faerla:
                case (ushort)Caulle:
                case (ushort)Henley:
                case (ushort)Galletta:
                case (ushort)Ianim:
                    replay.Actors.Add(new CircleActor(false, 0, 600, lifespan, "rgba(255, 0, 0, 0.5)", new AgentConnector(mob)));
                    replay.Actors.Add(new CircleActor(true, 0, 400, lifespan, "rgba(0, 125, 255, 0.5)", new AgentConnector(mob)));
                    break;
                case (ushort)GreenPhantasm:
                    int lifetime = 8000;
                    replay.Actors.Add(new CircleActor(true, 0, 210, new Tuple<int, int>(start, start + lifetime), "rgba(0,255,0,0.2)", new AgentConnector(mob)));
                    replay.Actors.Add(new CircleActor(true, start + lifetime, 210, new Tuple<int, int>(start, start + lifetime), "rgba(0,255,0,0.3)", new AgentConnector(mob)));
                    break;
                case (ushort)RetrieverProjection:
                case (ushort)InsidiousProjection:
                case (ushort)UnstableLeyRift:
                case (ushort)RadiantPhantasm:
                case (ushort)CrimsonPhantasm:
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }

        public override void ComputeAdditionalTargetData(Target target, ParsedLog log)
        {
            CombatReplay replay = target.CombatReplay;
            List<CastLog> cls = target.GetCastLogs(log, 0, log.FightData.FightDuration);
            switch (target.ID)
            {
                case (ushort)ParseEnum.TargetIDS.KeepConstruct:
                    List<CastLog> magicCharge = cls.Where(x => x.SkillId == 35048).ToList();
                    List<CastLog> magicExplode = cls.Where(x => x.SkillId == 34894).ToList();
                    for (var i = 0; i < magicCharge.Count; i++)
                    {
                        CastLog charge = magicCharge[i];
                        if (i < magicExplode.Count)
                        {
                            CastLog fire = magicExplode[i];
                            int start = (int)charge.Time;
                            int end = (int)fire.Time + fire.ActualDuration;
                            replay.Actors.Add(new CircleActor(false, 0, 300, new Tuple<int, int>(start, end), "rgba(255, 0, 0, 0.5)", new AgentConnector(target)));
                            replay.Actors.Add(new CircleActor(true, end, 300, new Tuple<int, int>(start, end), "rgba(255, 0, 0, 0.5)", new AgentConnector(target)));
                        }
                    }
                    List<CastLog> towerDrop = cls.Where(x => x.SkillId == 35086).ToList();
                    foreach (CastLog c in towerDrop)
                    {
                        int start = (int)c.Time;
                        int end = start + c.ActualDuration;
                        int skillCast = end - 1000;
                        Point3D next = replay.Positions.FirstOrDefault(x => x.Time >= end);
                        Point3D prev = replay.Positions.LastOrDefault(x => x.Time <= end);
                        if (prev != null || next != null)
                        {
                            replay.Actors.Add(new CircleActor(false, 0, 400, new Tuple<int, int>(start, skillCast), "rgba(255, 150, 0, 0.5)", new InterpolatedPositionConnector(prev, next, end)));
                            replay.Actors.Add(new CircleActor(true, skillCast, 400, new Tuple<int, int>(start, skillCast), "rgba(255, 150, 0, 0.5)", new InterpolatedPositionConnector(prev, next, end)));
                        }
                    }
                    List<CastLog> blades1 = cls.Where(x => x.SkillId == 35064).ToList();
                    List<CastLog> blades2 = cls.Where(x => x.SkillId == 35137).ToList();
                    List<CastLog> blades3 = cls.Where(x => x.SkillId == 34971).ToList();
                    int bladeDelay = 150;
                    int duration = 1000;
                    foreach (CastLog c in blades1)
                    {
                        int ticks = (int)Math.Max(0, Math.Min(Math.Ceiling((c.ActualDuration - 1150) / 1000.0), 9));
                        int start = (int)c.Time + bladeDelay;
                        Point3D facing = replay.Rotations.LastOrDefault(x => x.Time < start + 1000);
                        if (facing == null)
                        {
                            continue;
                        }
                        replay.Actors.Add(new CircleActor(true, 0, 200, new Tuple<int, int>(start, start + (ticks + 1) * 1000), "rgba(255,0,0,0.4)", new AgentConnector(target)));
                        replay.Actors.Add(new PieActor(true, 0, 1600, facing, 360 * 3 / 32, new Tuple<int, int>(start, start + 2 * duration), "rgba(255,200,0,0.5)", new AgentConnector(target))); // First blade lasts twice as long
                        for (int i = 1; i < ticks; i++)
                        {
                            replay.Actors.Add(new PieActor(true, 0, 1600, (int)Math.Round(Math.Atan2(facing.Y, facing.X) * 180 / Math.PI + i * 360 / 8), 360 * 3 / 32, new Tuple<int, int>(start + 1000 + i * duration, start + 1000 + (i + 1) * duration), "rgba(255,200,0,0.5)", new AgentConnector(target))); // First blade lasts longer
                        }
                    }
                    foreach (CastLog c in blades2)
                    {
                        int ticks = (int)Math.Max(0, Math.Min(Math.Ceiling((c.ActualDuration - 1150) / 1000.0), 9));
                        int start = (int)c.Time + bladeDelay;
                        Point3D facing = replay.Rotations.LastOrDefault(x => x.Time < start + 1000);
                        if (facing == null)
                        {
                            continue;
                        }
                        replay.Actors.Add(new CircleActor(true, 0, 200, new Tuple<int, int>(start, start + (ticks + 1) * 1000), "rgba(255,0,0,0.4)", new AgentConnector(target)));
                        replay.Actors.Add(new PieActor(true, 0, 1600, facing, 360 * 3 / 32, new Tuple<int, int>(start, start + 2 * duration), "rgba(255,200,0,0.5)", new AgentConnector(target))); // First blade lasts twice as long
                        replay.Actors.Add(new PieActor(true, 0, 1600, (int)Math.Round(Math.Atan2(-facing.Y, -facing.X) * 180 / Math.PI), 360 * 3 / 32, new Tuple<int, int>(start, start + 2 * duration), "rgba(255,200,0,0.5)", new AgentConnector(target))); // First blade lasts twice as long
                        for (int i = 1; i < ticks; i++)
                        {
                            replay.Actors.Add(new PieActor(true, 0, 1600, (int)Math.Round(Math.Atan2(facing.Y, facing.X) * 180 / Math.PI + i * 360 / 8), 360 * 3 / 32, new Tuple<int, int>(start + 1000 + i * duration, start + 1000 + (i + 1) * duration), "rgba(255,200,0,0.5)", new AgentConnector(target))); // First blade lasts longer
                            replay.Actors.Add(new PieActor(true, 0, 1600, (int)Math.Round(Math.Atan2(-facing.Y, -facing.X) * 180 / Math.PI + i * 360 / 8), 360 * 3 / 32, new Tuple<int, int>(start + 1000 + i * duration, start + 1000 + (i + 1) * duration), "rgba(255,200,0,0.5)", new AgentConnector(target))); // First blade lasts longer
                        }
                    }
                    foreach (CastLog c in blades3)
                    {
                        int ticks = (int)Math.Max(0, Math.Min(Math.Ceiling((c.ActualDuration - 1150) / 1000.0), 9));
                        int start = (int)c.Time + bladeDelay;
                        Point3D facing = replay.Rotations.LastOrDefault(x => x.Time < start + 1000);
                        if (facing == null)
                        {
                            continue;
                        }
                        replay.Actors.Add(new CircleActor(true, 0, 200, new Tuple<int, int>(start, start + (ticks + 1) * 1000), "rgba(255,0,0,0.4)", new AgentConnector(target)));
                        replay.Actors.Add(new PieActor(true, 0, 1600, (int)Math.Round(Math.Atan2(-facing.Y, -facing.X) * 180 / Math.PI), 360 * 3 / 32, new Tuple<int, int>(start, start + 2 * duration), "rgba(255,200,0,0.5)", new AgentConnector(target))); // First blade lasts twice as long
                        replay.Actors.Add(new PieActor(true, 0, 1600, (int)Math.Round(Math.Atan2(-facing.Y, -facing.X) * 180 / Math.PI + 120), 360 * 3 / 32, new Tuple<int, int>(start, start + 2 * duration), "rgba(255,200,0,0.5)", new AgentConnector(target))); // First blade lasts twice as long
                        replay.Actors.Add(new PieActor(true, 0, 1600, (int)Math.Round(Math.Atan2(-facing.Y, -facing.X) * 180 / Math.PI - 120), 360 * 3 / 32, new Tuple<int, int>(start, start + 2 * duration), "rgba(255,200,0,0.5)", new AgentConnector(target))); // First blade lasts twice as long
                        for (int i = 1; i < ticks; i++)
                        {
                            replay.Actors.Add(new PieActor(true, 0, 1600, (int)Math.Round(Math.Atan2(-facing.Y, -facing.X) * 180 / Math.PI + i * 360 / 8), 360 * 3 / 32, new Tuple<int, int>(start + 1000 + i * duration, start + 1000 + (i + 1) * duration), "rgba(255,200,0,0.5)", new AgentConnector(target))); // First blade lasts longer
                            replay.Actors.Add(new PieActor(true, 0, 1600, (int)Math.Round(Math.Atan2(-facing.Y, -facing.X) * 180 / Math.PI + i * 360 / 8 + 120), 360 * 3 / 32, new Tuple<int, int>(start + 1000 + i * duration, start + 1000 + (i + 1) * duration), "rgba(255,200,0,0.5)", new AgentConnector(target))); // First blade lasts longer
                            replay.Actors.Add(new PieActor(true, 0, 1600, (int)Math.Round(Math.Atan2(-facing.Y, -facing.X) * 180 / Math.PI + i * 360 / 8 - 120), 360 * 3 / 32, new Tuple<int, int>(start + 1000 + i * duration, start + 1000 + (i + 1) * duration), "rgba(255,200,0,0.5)", new AgentConnector(target))); // First blade lasts longer
                        }
                    }
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
            
        }

        public override void ComputeAdditionalPlayerData(Player p, ParsedLog log)
        {
            // Bombs
            CombatReplay replay = p.CombatReplay;
            List<CombatItem> xeraFury = GetFilteredList(log, 35103, p);
            int xeraFuryStart = 0;
            foreach (CombatItem c in xeraFury)
            {
                if (c.IsBuffRemove == ParseEnum.BuffRemove.None)
                {
                    xeraFuryStart = (int)(log.FightData.ToFightSpace(c.Time));
                }
                else
                {
                    int xeraFuryEnd = (int)(log.FightData.ToFightSpace(c.Time));
                    replay.Actors.Add(new CircleActor(true, 0, 550, new Tuple<int, int>(xeraFuryStart, xeraFuryEnd), "rgba(200, 150, 0, 0.2)", new AgentConnector(p)));
                    replay.Actors.Add(new CircleActor(true, xeraFuryEnd, 550, new Tuple<int, int>(xeraFuryStart, xeraFuryEnd), "rgba(200, 150, 0, 0.4)", new AgentConnector(p)));
                }

            }
            //fixated Statue
            List<CombatItem> fixatedStatue = GetFilteredList(log, 34912, p).Concat(GetFilteredList(log, 34925, p)).ToList();
            int fixationStatueStart = 0;
            Mob statue = null;
            foreach (CombatItem c in fixatedStatue)
            {
                if (c.IsBuffRemove == ParseEnum.BuffRemove.None)
                {
                    fixationStatueStart = (int)(log.FightData.ToFightSpace(c.Time));
                    statue = TrashMobs.FirstOrDefault(x => x.Agent == c.SrcAgent);
                }
                else
                {
                    int fixationStatueEnd = (int)(log.FightData.ToFightSpace(c.Time));
                    Tuple<int, int> duration = new Tuple<int, int>(fixationStatueStart, fixationStatueEnd);
                    if (statue != null)
                    {
                        replay.Actors.Add(new LineActor(0, 10, duration, "rgba(255, 0, 255, 0.5)", new AgentConnector(p), new AgentConnector(statue)));
                    }
                }
            }
        }
    }
}
