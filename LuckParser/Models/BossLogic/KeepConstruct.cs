using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models
{
    public class KeepConstruct : RaidLogic
    {
        public KeepConstruct()
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new Mechanic(34912, "Fixate", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.KeepConstruct, "symbol:'star',color:'rgb(255,0,255)',", "Fixt","Fixated by Statue", "Fixated",0),
            new Mechanic(34925, "Fixate", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.KeepConstruct, "symbol:'star',color:'rgb(255,0,255)',", "Fixt","Fixated by Statue", "Fixated",0),
            new Mechanic(35077, "Hail of Fury", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.KeepConstruct, "symbol:'circle-open',color:'rgb(255,0,0)',", "Debris","Hail of Fury (Falling Debris)", "Debris",0),
            new Mechanic(35096, "Compromised", Mechanic.MechType.EnemyBoon, ParseEnum.BossIDS.KeepConstruct, "symbol:'hexagon',color:'rgb(0,0,255)',", "Rift#","Compromised (Pushed Orb through Rifts)", "Compromised",0),
            new Mechanic(16227, "Insidious Projection", Mechanic.MechType.Spawn, ParseEnum.BossIDS.KeepConstruct, "symbol:'bowtie',color:'rgb(255,0,0)',", "Merge","Insidious Projection spawn (2 Statue merge)", "Merged Statues",0),
            new Mechanic(35137, "Phantasmal Blades", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.KeepConstruct, "symbol:'hexagram-open',color:'rgb(255,0,255)',", "PhBlds","Phantasmal Blades (rotating Attack)", "Phantasmal Blades",0),
            new Mechanic(34971, "Phantasmal Blades", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.KeepConstruct, "symbol:'hexagram-open',color:'rgb(255,0,255)',", "PhBlds","Phantasmal Blades (rotating Attack)", "Phantasmal Blades",0),
            new Mechanic(35064, "Phantasmal Blades", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.KeepConstruct, "symbol:'hexagram-open',color:'rgb(255,0,255)',", "PhBlds","Phantasmal Blades (rotating Attack)", "Phantasmal Blades",0),
            new Mechanic(35086, "Tower Drop", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.KeepConstruct, "symbol:'circle',color:'rgb(255,140,0)',", "Jump","Tower Drop (KC Jump)", "Tower Drop",0),
            new Mechanic(35103, "Xera's Fury", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.KeepConstruct, "symbol:'circle',color:'rgb(200,140,0)',", "Bmb","Xera's Fury (Large Bombs) application", "Bombs",0),
            new Mechanic(16261, "Core Hit", Mechanic.MechType.HitOnEnemy, ParseEnum.BossIDS.KeepConstruct, "symbol:'star-open',color:'rgb(255,140,0)',", "C.Hit","Core was Hit by Player", "Core Hit",1000)
            });
            Extension = "kc";
            IconUrl = "https://wiki.guildwars2.com/images/e/ea/Mini_Keep_Construct.png";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/Fj1HyM0.png",
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
            if (!requirePhases)
            {
                return phases;
            }
            // Main phases
            List<CastLog> castLogs = log.Boss.GetCastLogs(log, 0, log.FightData.FightEnd);
            List<CastLog> clsKC = castLogs.Where(x => x.SkillId == 35048).ToList();
            foreach (CastLog cl in clsKC)
            {
                end = cl.Time;
                phases.Add(new PhaseData(start, end));
                start = end + cl.ActualDuration;
            }
            if (fightDuration - start > 5000 && start >= phases.Last().End)
            {
                phases.Add(new PhaseData(start, fightDuration));
                start = fightDuration;
            }
            for (int i = 1; i < phases.Count; i++)
            {
                phases[i].Name = "Phase " + i;
            }
            // add burn phases
            int offset = phases.Count;
            List<CombatItem> orbItems = log.GetBoonData(35096).Where(x => x.DstInstid == log.Boss.InstID).ToList();
            // Get number of orbs and filter the list
            List<CombatItem> orbItemsFiltered = new List<CombatItem>();
            Dictionary<long, int> orbs = new Dictionary<long, int>();
            foreach (CombatItem c in orbItems)
            {
                long time = c.Time - log.FightData.FightStart;
                if (!orbs.ContainsKey(time))
                {
                    orbs[time] = 0;
                }
                if (c.IsBuffRemove == ParseEnum.BuffRemove.None)
                {
                    orbs[time] = orbs[time] + 1;
                }
                if (orbItemsFiltered.Count > 0)
                {
                    CombatItem last = orbItemsFiltered.Last();
                    if (last.Time != c.Time)
                    {
                        orbItemsFiltered.Add(c);
                    }
                }
                else
                {
                    orbItemsFiltered.Add(c);
                }

            }
            foreach (CombatItem c in orbItemsFiltered)
            {
                if (c.IsBuffRemove == ParseEnum.BuffRemove.None)
                {
                    start = c.Time - log.FightData.FightStart;
                }
                else
                {
                    end = c.Time - log.FightData.FightStart;
                    phases.Add(new PhaseData(start, end));
                }
            }
            if (fightDuration - start > 5000 && start >= phases.Last().End)
            {
                phases.Add(new PhaseData(start, fightDuration));
                start = fightDuration;
            }
            for (int i = offset; i < phases.Count; i++)
            {
                phases[i].Name = "Burn " + (i - offset + 1) + " (" + orbs[phases[i].Start] + " orbs)";
                phases[i].DrawArea = true;
                phases[i].DrawStart = true;
                phases[i].DrawEnd = true;
            }
            phases.Sort((x, y) => (x.Start < y.Start) ? -1 : 1);
            phases.Last().DrawEnd = false;
            return phases;
        }

        public override List<ParseEnum.TrashIDS> GetAdditionalBossData(CombatReplay replay, List<CastLog> cls, ParsedLog log)
        {
            // TODO: needs arc circles for blades
            List<ParseEnum.TrashIDS> ids = new List<ParseEnum.TrashIDS>
                    {
                        ParseEnum.TrashIDS.Core,
                        ParseEnum.TrashIDS.Jessica,
                        ParseEnum.TrashIDS.Olson,
                        ParseEnum.TrashIDS.Engul,
                        ParseEnum.TrashIDS.Faerla,
                        ParseEnum.TrashIDS.Caulle,
                        ParseEnum.TrashIDS.Henley,
                        ParseEnum.TrashIDS.Galletta,
                        ParseEnum.TrashIDS.Ianim,
                        ParseEnum.TrashIDS.GreenPhantasm,
                        ParseEnum.TrashIDS.InsidiousProjection,
                        ParseEnum.TrashIDS.UnstableLeyRift,
                        ParseEnum.TrashIDS.RadiantPhantasm,
                        ParseEnum.TrashIDS.CrimsonPhantasm,
                        ParseEnum.TrashIDS.RetrieverProjection
                    };
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
                    replay.Actors.Add(new CircleActor(false, 0, 300, new Tuple<int, int>(start, end), "rgba(255, 0, 0, 0.5)"));
                    replay.Actors.Add(new CircleActor(true, end, 300, new Tuple<int, int>(start, end), "rgba(255, 0, 0, 0.5)"));
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
                    replay.Actors.Add(new CircleActor(false, 0, 400, new Tuple<int, int>(start, skillCast), "rgba(255, 150, 0, 0.5)", prev, next, end));
                    replay.Actors.Add(new CircleActor(true, skillCast, 400, new Tuple<int, int>(start, skillCast), "rgba(255, 150, 0, 0.5)", prev, next, end));
                }
            }
            List<CastLog> blades1 = cls.Where(x => x.SkillId == 35064).ToList();
            List<CastLog> blades2 = cls.Where(x => x.SkillId == 35137).ToList();
            List<CastLog> blades3 = cls.Where(x => x.SkillId == 34971).ToList();
            int bladeDelay = 150;
            int duration = 1000;
            foreach (CastLog c in blades1)
            {
                int ticks = (int)Math.Max(0,Math.Min(Math.Ceiling((c.ActualDuration-1150) / 1000.0), 9));
                int start = (int)c.Time + bladeDelay;
                Point3D facing = replay.Rotations.LastOrDefault(x => x.Time < start + 1000);
                replay.Actors.Add(new CircleActor(true, 0, 200, new Tuple<int, int>(start, start + (ticks+1) * 1000), "rgba(255,0,0,0.4)"));
                replay.Actors.Add(new PieActor(true, 0, 1600, facing, 360 * 3 / 32, new Tuple<int,int>(start,start+2*duration),"rgba(255,200,0,0.5)")); // First blade lasts twice as long
                for (int i = 1; i < ticks; i++)
                {
                    replay.Actors.Add(new PieActor(true, 0, 1600, (int)Math.Round(Math.Atan2(-facing.Y, facing.X) * 180 / Math.PI - i * 360 / 8), 360 * 3 / 32, new Tuple<int, int>(start + 1000 + i * duration, start + 1000 + (i + 1) * duration), "rgba(255,200,0,0.5)")); // First blade lasts longer
                }
            }
            foreach (CastLog c in blades2)
            {
                int ticks = (int)Math.Max(0, Math.Min(Math.Ceiling((c.ActualDuration - 1150) / 1000.0), 9));
                int start = (int)c.Time + bladeDelay;
                Point3D facing = replay.Rotations.LastOrDefault(x => x.Time < start+1000);
                replay.Actors.Add(new CircleActor(true, 0, 200, new Tuple<int, int>(start, start + (ticks + 1) * 1000), "rgba(255,0,0,0.4)"));
                replay.Actors.Add(new PieActor(true, 0, 1600, facing, 360 * 3 / 32, new Tuple<int, int>(start, start + 2 * duration), "rgba(255,200,0,0.5)")); // First blade lasts twice as long
                replay.Actors.Add(new PieActor(true, 0, 1600, (int)Math.Round(Math.Atan2(facing.Y, -facing.X) * 180 / Math.PI), 360 * 3 / 32, new Tuple<int, int>(start, start + 2 * duration), "rgba(255,200,0,0.5)")); // First blade lasts twice as long
                for (int i = 1; i < ticks; i++)
                {
                    replay.Actors.Add(new PieActor(true, 0, 1600, (int)Math.Round(Math.Atan2(-facing.Y, facing.X) * 180 / Math.PI - i * 360 / 8), 360 * 3 / 32, new Tuple<int, int>(start+ 1000 + i * duration, start + 1000 + (i + 1) * duration), "rgba(255,200,0,0.5)")); // First blade lasts longer
                    replay.Actors.Add(new PieActor(true, 0, 1600, (int)Math.Round(Math.Atan2(facing.Y, -facing.X) * 180 / Math.PI - i * 360 / 8), 360 * 3 / 32, new Tuple<int, int>(start + 1000 + i * duration, start + 1000 + (i + 1) * duration), "rgba(255,200,0,0.5)")); // First blade lasts longer
                }
            }
            foreach (CastLog c in blades3)
            {
                int ticks = (int)Math.Max(0, Math.Min(Math.Ceiling((c.ActualDuration - 1150) / 1000.0), 9));
                Console.WriteLine("Ticks: {0} Dura: {1}" ,ticks,c.ActualDuration);
                int start = (int)c.Time + bladeDelay;
                Point3D facing = replay.Rotations.LastOrDefault(x => x.Time < start + 1000);
                replay.Actors.Add(new CircleActor(true, 0, 200, new Tuple<int, int>(start, start + (ticks + 1) * 1000), "rgba(255,0,0,0.4)"));
                replay.Actors.Add(new PieActor(true, 0, 1600, (int)Math.Round(Math.Atan2(facing.Y, -facing.X) * 180 / Math.PI), 360 * 3 / 32, new Tuple<int, int>(start, start + 2 * duration), "rgba(255,200,0,0.5)")); // First blade lasts twice as long
                replay.Actors.Add(new PieActor(true, 0, 1600, (int)Math.Round(Math.Atan2(facing.Y, -facing.X) * 180 / Math.PI + 120), 360 * 3 / 32, new Tuple<int, int>(start, start + 2 * duration), "rgba(255,200,0,0.5)")); // First blade lasts twice as long
                replay.Actors.Add(new PieActor(true, 0, 1600, (int)Math.Round(Math.Atan2(facing.Y, -facing.X) * 180 / Math.PI - 120), 360 * 3 / 32, new Tuple<int, int>(start, start + 2 * duration), "rgba(255,200,0,0.5)")); // First blade lasts twice as long
                for (int i = 1; i < ticks; i++)
                {
                    replay.Actors.Add(new PieActor(true, 0, 1600, (int)Math.Round(Math.Atan2(facing.Y, -facing.X) * 180 / Math.PI - i * 360 / 8), 360 * 3 / 32, new Tuple<int, int>(start + 1000 + i * duration, start + 1000 + (i + 1) * duration), "rgba(255,200,0,0.5)")); // First blade lasts longer
                    replay.Actors.Add(new PieActor(true, 0, 1600, (int)Math.Round(Math.Atan2(facing.Y, -facing.X) * 180 / Math.PI - i * 360 / 8 + 120), 360 * 3 / 32, new Tuple<int, int>(start + 1000 + i * duration, start + 1000 + (i + 1) * duration), "rgba(255,200,0,0.5)")); // First blade lasts longer
                    replay.Actors.Add(new PieActor(true, 0, 1600, (int)Math.Round(Math.Atan2(facing.Y, -facing.X) * 180 / Math.PI - i * 360 / 8 - 120), 360 * 3 / 32, new Tuple<int, int>(start + 1000 + i * duration, start + 1000 + (i + 1) * duration), "rgba(255,200,0,0.5)")); // First blade lasts longer
                }
            }

            return ids;
        }

        public override void GetAdditionalPlayerData(CombatReplay replay, Player p, ParsedLog log)
        {
            // Bombs
            List<CombatItem> xeraFury = GetFilteredList(log, 35103, p.InstID);
            int xeraFuryStart = 0;
            foreach (CombatItem c in xeraFury)
            {
                if (c.IsBuffRemove == ParseEnum.BuffRemove.None)
                {
                    xeraFuryStart = (int)(c.Time - log.FightData.FightStart);
                }
                else
                {
                    int xeraFuryEnd = (int)(c.Time - log.FightData.FightStart);
                    replay.Actors.Add(new CircleActor(true, 0, 550, new Tuple<int, int>(xeraFuryStart, xeraFuryEnd), "rgba(200, 150, 0, 0.2)"));
                    replay.Actors.Add(new CircleActor(true, xeraFuryEnd, 550, new Tuple<int, int>(xeraFuryStart, xeraFuryEnd), "rgba(200, 150, 0, 0.4)"));
                }

            }
        }

        public override string GetReplayIcon()
        {
            return "https://i.imgur.com/Kq0kL07.png";
        }
    }
}
