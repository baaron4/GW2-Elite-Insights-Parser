using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static LuckParser.Models.DataModels.ParseEnum.TrashIDS;

namespace LuckParser.Models
{
    public class SoullessHorror : RaidLogic
    {
        public SoullessHorror(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {

            new Mechanic(47327, "Vortex Slash", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.SoullessHorror, "symbol:'circle',color:'rgb(255,140,0)'", "D.In","Vortex Slash (Inner Donut hit)", "Inner Donut",0),
            new Mechanic(48432, "Vortex Slash", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.SoullessHorror, "symbol:'circle-open',color:'rgb(255,140,0)'", "D.Out","Vortex Slash (Outer Donut hit)", "Outer Donut", 0),
            new Mechanic(47430, "Soul Rift", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.SoullessHorror, "symbol:'circle-open',color:'rgb(255,0,0)'", "Golem","Soul Rift (stood in Golem Aoe)", "Golem Aoe",0),
            new Mechanic(48363, "Quad Slash", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.SoullessHorror, "symbol:'star-diamond-open',color:'rgb(255,140,0)'", "Slcs1","Quad Slash (4 Slices, First hit)", "4 Slices 1",0),
            new Mechanic(47915, "Quad Slash", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.SoullessHorror, "symbol:'star-square-open',color:'rgb(255,140,0)'", "Slcs2","Quad Slash (4 Slices, Second hit)", "4 Slices 2",0),
            new Mechanic(47363, "Spinning Slash", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.SoullessHorror, "symbol:'star-triangle-up-open',color:'rgb(128,0,0)'", "Scth","Spinning Slash (hit by Scythe)", "Scythe",0),
            new Mechanic(48500, "Death Bloom", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.SoullessHorror, "symbol:'octagon',color:'rgb(255,140,0)'", "8Slcs","Death Bloom (8 Slices)", "8 Slices",0),
            new Mechanic(47434, "Fixated", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.SoullessHorror, "symbol:'star',color:'rgb(255,0,255)'", "Fix","Fixated (Special Action Key)", "Fixated",0),
            new Mechanic(47414, "Necrosis", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.SoullessHorror, "symbol:'star-open',color:'rgb(255,0,255)'", "Necr","Necrosis (Tanking Debuff)", "Necrosis Debuff",0),
            new Mechanic(48327, "Corrupt the Living", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.SoullessHorror, "symbol:'circle',color:'rgb(255,0,0)'", "Spin","Corrupt the Living (Torment+Poisen Spin)", "Torment+Poisen Spin",0),
            new Mechanic(47756, "Wurm Spit", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.SoullessHorror, "symbol:'diamond-open',color:'rgb(0,128,128)'", "Spit","Wurm Spit", "Wurm Spit",0),
            new Mechanic(48662, "Howling Death", Mechanic.MechType.EnemyCastStart, ParseEnum.BossIDS.SoullessHorror, "symbol:'diamond-tall',color:'rgb(0,160,150)'", "CC","Howling Death (Breakbar)", "Breakbar",0),
            new Mechanic(48662, "Howling Death", Mechanic.MechType.EnemyCastEnd, ParseEnum.BossIDS.SoullessHorror, "symbol:'diamond-tall',color:'rgb(0,160,0)'", "CCed","Howling Death (Breakbar) broken", "CCed",0,(condition => condition.CombatItem.Value <=6800)),
            new Mechanic(48662, "Howling Death", Mechanic.MechType.EnemyCastEnd, ParseEnum.BossIDS.SoullessHorror, "symbol:'diamond-tall',color:'rgb(255,0,0)'", "CC.Fail","Howling Death (Breakbar failed) ", "CC Fail",0,(condition => condition.CombatItem.Value >6800)),

            });
            Extension = "sh";
            IconUrl = "https://wiki.guildwars2.com/images/d/d4/Mini_Desmina.png";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/A45pVJy.png",
                            Tuple.Create(3657, 3657),
                            Tuple.Create(-12223, -771, -8932, 2420),
                            Tuple.Create(-21504, -12288, 24576, 12288),
                            Tuple.Create(19072, 15484, 20992, 16508));
        }

        protected override List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>
            {
                Scythe,
                TormentedDead,
                SurgingSoul
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
                case (ushort)Scythe:
                    replay.Actors.Add(new CircleActor(true, 0, 80, lifespan, "rgba(255, 0, 0, 0.5)"));
                    replay.Icon = "https://i.imgur.com/INCGLIK.png";
                    break;
                case (ushort)TormentedDead:
                    if (replay.Positions.Count == 0)
                    {
                        break;
                    }
                    replay.Actors.Add(new CircleActor(true, 0, 400, new Tuple<int, int>(end, end + 60000), "rgba(255, 0, 0, 0.5)", replay.Positions.Last()));
                    replay.Icon = "https://i.imgur.com/1J2BTFg.png";
                    break;
                case (ushort)SurgingSoul:
                    replay.Icon = "https://i.imgur.com/k79t7ZA.png";
                    List<Point3D> positions = replay.Positions;
                    if (positions.Count < 2)
                    {
                        break;
                    }
                    if (positions[1].X < -12000 || positions[1].X > -9250)
                    {
                        replay.Actors.Add(new RectangleActor(true, 0, 240, 660, lifespan, "rgba(255,100,0,0.5)"));
                        break;
                    }
                    else if (positions[1].Y < -525 || positions[1].Y > 2275)
                    {
                        replay.Actors.Add(new RectangleActor(true, 0, 645, 238, lifespan, "rgba(255,100,0,0.5)"));
                        break;
                    }
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }

        public override void ComputeAdditionalBossData(Boss boss, ParsedLog log)
        {
            CombatReplay replay = boss.CombatReplay;
            List<CastLog> cls = boss.GetCastLogs(log, 0, log.FightData.FightDuration);
            switch (boss.ID)
            {
                case (ushort)ParseEnum.BossIDS.SoullessHorror:
                    replay.Icon = "https://i.imgur.com/jAiRplg.png";
                    List<CastLog> howling = cls.Where(x => x.SkillId == 48662).ToList();
                    foreach (CastLog c in howling)
                    {
                        int start = (int)c.Time;
                        int end = start + c.ActualDuration;
                        replay.Actors.Add(new CircleActor(true, (int)c.Time + c.ExpectedDuration, 180, new Tuple<int, int>(start, end), "rgba(0, 180, 255, 0.3)"));
                        replay.Actors.Add(new CircleActor(true, 0, 180, new Tuple<int, int>(start, end), "rgba(0, 180, 255, 0.3)"));
                    }
                    List<CastLog> vortex = cls.Where(x => x.SkillId == 47327).ToList();
                    foreach (CastLog c in vortex)
                    {
                        int start = (int)c.Time;
                        int end = start + 4000;
                        Point3D next = replay.Positions.FirstOrDefault(x => x.Time >= start);
                        Point3D prev = replay.Positions.LastOrDefault(x => x.Time <= start);
                        if (next != null || prev != null)
                        {
                            replay.Actors.Add(new CircleActor(false, 0, 380, new Tuple<int, int>(start, end), "rgba(255, 150, 0, 0.5)", prev, next, start));
                            replay.Actors.Add(new CircleActor(true, end, 380, new Tuple<int, int>(start, end), "rgba(255, 150, 0, 0.5)", prev, next, start));
                            replay.Actors.Add(new DoughnutActor(true, 0, 380, 760, new Tuple<int, int>(end, end + 1000), "rgba(255, 150, 0, 0.5)", prev, next, start));
                        }
                    }
                    List<CastLog> deathBloom = cls.Where(x => x.SkillId == 48500).ToList();
                    foreach (CastLog c in deathBloom)
                    {
                        int start = (int)c.Time;
                        int end = start + c.ActualDuration;
                        Point3D facing = replay.Rotations.FirstOrDefault(x => x.Time >= start);
                        if (facing == null)
                        {
                            continue;
                        }
                        for (int i = 0; i < 8; i++)
                        {
                            replay.Actors.Add(new PieActor(true, 0, 3500, (int)(Math.Atan2(-facing.Y, facing.X) * 180 / Math.PI + i * 360 / 8), 360 / 12, new Tuple<int, int>(start, end), "rgba(255,200,0,0.5)"));
                        }

                    }
                    List<CastLog> quad1 = cls.Where(x => x.SkillId == 48363).ToList();
                    List<CastLog> quad2 = cls.Where(x => x.SkillId == 47915).ToList();
                    foreach (CastLog c in quad1)
                    {
                        int start = (int)c.Time;
                        int end = start + c.ActualDuration;
                        Point3D facing = replay.Rotations.FirstOrDefault(x => x.Time >= start);
                        if (facing == null)
                        {
                            continue;
                        }
                        for (int i = 0; i < 4; i++)
                        {
                            replay.Actors.Add(new PieActor(true, 0, 3500, (int)(Math.Atan2(-facing.Y, facing.X) * 180 / Math.PI + i * 360 / 4), 360 / 12, new Tuple<int, int>(start, end), "rgba(255,200,0,0.5)"));
                        }

                    }
                    foreach (CastLog c in quad2)
                    {
                        int start = (int)c.Time;
                        int end = start + c.ActualDuration;
                        Point3D facing = replay.Rotations.FirstOrDefault(x => x.Time >= start);
                        if (facing == null)
                        {
                            continue;
                        }
                        for (int i = 0; i < 4; i++)
                        {
                            replay.Actors.Add(new PieActor(true, 0, 3500, (int)(Math.Atan2(-facing.Y, facing.X) * 180 / Math.PI + 45 + i * 360 / 4), 360 / 12, new Tuple<int, int>(start, end), "rgba(255,200,0,0.5)"));
                        }

                    }
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
            
        }

        public override int IsCM(ParsedLog log)
        {
            List<CombatItem> necrosis = log.CombatData.GetBoonData(47414).Where(x => x.IsBuffRemove == ParseEnum.BuffRemove.None).ToList();
            if (necrosis.Count == 0)
            {
                return 0;
            }
            // split necrosis
            Dictionary<ushort, List<CombatItem>> splitNecrosis = new Dictionary<ushort, List<CombatItem>>();
            foreach (CombatItem c in necrosis)
            {
                ushort inst = c.DstInstid;
                if (!splitNecrosis.ContainsKey(inst))
                {
                    splitNecrosis.Add(inst, new List<CombatItem>());
                }
                splitNecrosis[inst].Add(c);
            }
            List<CombatItem> longestNecrosis = splitNecrosis.Values.First(l => l.Count == splitNecrosis.Values.Max(x => x.Count));
            long minDiff = long.MaxValue;
            for (int i = 0; i < longestNecrosis.Count - 1; i++)
            {
                CombatItem cur = longestNecrosis[i];
                CombatItem next = longestNecrosis[i + 1];
                long timeDiff = next.Time - cur.Time;
                if (timeDiff > 1000 && minDiff > timeDiff)
                {
                    minDiff = timeDiff;
                }
            }
            return (minDiff < 11000) ? 1 : 0;
        }
    }
}
