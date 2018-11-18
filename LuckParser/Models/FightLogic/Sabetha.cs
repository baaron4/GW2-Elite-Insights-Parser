using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static LuckParser.Models.DataModels.ParseEnum.TrashIDS;

namespace LuckParser.Models
{
    public class Sabetha : RaidLogic
    {
        public Sabetha(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new Mechanic(34108, "Shell-Shocked", Mechanic.MechType.PlayerBoon, ParseEnum.TargetIDS.Sabetha, "symbol:'circle-open',color:'rgb(0,128,0)'", "Lnchd","Shell-Shocked (launched up to cannons)", "Shell-Shocked",0),
            new Mechanic(31473, "Sapper Bomb", Mechanic.MechType.PlayerBoon, ParseEnum.TargetIDS.Sabetha, "symbol:'circle',color:'rgb(0,128,0)'", "SBmb","Got a Sapper Bomb", "Sapper Bomb",0),
            new Mechanic(31485, "Time Bomb", Mechanic.MechType.PlayerBoon, ParseEnum.TargetIDS.Sabetha, "symbol:'circle-open',color:'rgb(255,0,0)'", "TBmb","Got a Timed Bomb (Expanding circle)", "Timed Bomb",0),
            new Mechanic(31332, "Firestorm", Mechanic.MechType.SkillOnPlayer, ParseEnum.TargetIDS.Sabetha, "symbol:'square',color:'rgb(255,0,0)'", "Flmwll","Firestorm (killed by Flamewall)", "Flamewall",0),
            new Mechanic(31544, "Flak Shot", Mechanic.MechType.SkillOnPlayer, ParseEnum.TargetIDS.Sabetha, "symbol:'hexagram-open',color:'rgb(255,140,0)'", "Flak","Flak Shot (Fire Patches)", "Flak Shot",0),
            new Mechanic(31643, "Cannon Barrage", Mechanic.MechType.SkillOnPlayer, ParseEnum.TargetIDS.Sabetha, "symbol:'circle',color:'rgb(255,200,0)'", "Cannon","Cannon Barrage (stood in AoE)", "Cannon Shot",0),
            new Mechanic(31761, "Flame Blast", Mechanic.MechType.SkillOnPlayer, ParseEnum.TargetIDS.Sabetha, "symbol:'triangle-left-open',color:'rgb(255,200,0)'", "Flmthrwr","Flame Blast (Karde's Flamethrower)", "Flamethrower (Karde)",0),
            new Mechanic(31408, "Kick", Mechanic.MechType.SkillOnPlayer, ParseEnum.TargetIDS.Sabetha, "symbol:'triangle-right',color:'rgb(255,0,255)'", "Kick","Kicked by Bandit", "Bandit Kick",0),
            new Mechanic(31763, "Platform Quake", Mechanic.MechType.EnemyCastStart, ParseEnum.TargetIDS.Sabetha, "symbol:'diamond-tall',color:'rgb(0,160,150)'", "CC","Platform Quake (Breakbar)","Breakbar",0),
            new Mechanic(31763, "Platform Quake", Mechanic.MechType.EnemyCastEnd, ParseEnum.TargetIDS.Sabetha, "symbol:'diamond-tall',color:'rgb(0,160,0)'", "CCed","Platform Quake (Breakbar broken) ", "CCed",0,(condition => condition.CombatItem.Value <=4400)),
            new Mechanic(31763, "Platform Quake", Mechanic.MechType.EnemyCastEnd, ParseEnum.TargetIDS.Sabetha, "symbol:'diamond-tall',color:'rgb(255,0,0)'", "CC.Fail","Platform Quake (Breakbar failed) ", "CC Fail",0,(condition => condition.CombatItem.Value >4400)),
            // Hit by Time Bomb could be implemented by checking if a person is affected by ID 31324 (1st Time Bomb) or 34152 (2nd Time Bomb, only below 50% boss HP) without being attributed a bomb (ID: 31485) 3000ms before (+-50ms). I think the actual heavy hit isn't logged because it may be percentage based. Nothing can be found in the logs.
            });
            Extension = "sab";
            IconUrl = "https://wiki.guildwars2.com/images/5/54/Mini_Sabetha.png";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/FwpMbYf.png",
                            Tuple.Create(2790, 2763),
                            Tuple.Create(-8587, -162, -1601, 6753),
                            Tuple.Create(-15360, -36864, 15360, 39936),
                            Tuple.Create(3456, 11012, 4736, 14212));
        }

        public override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            long start = 0;
            long end = 0;
            long fightDuration = log.FightData.FightDuration;
            List<PhaseData> phases = GetInitialPhase(log);
            Target mainTarget = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.Sabetha);
            if (mainTarget == null)
            {
                throw new InvalidOperationException("Main target of the fight not found");
            }
            phases[0].Targets.Add(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            // Invul check
            List<CombatItem> invulsSab = GetFilteredList(log, 757, mainTarget);
            for (int i = 0; i < invulsSab.Count; i++)
            {
                CombatItem c = invulsSab[i];
                if (c.IsBuffRemove == ParseEnum.BuffRemove.None)
                {
                    end = c.Time - log.FightData.FightStart;
                    phases.Add(new PhaseData(start, end));
                    if (i == invulsSab.Count - 1)
                    {
                        mainTarget.AddCustomCastLog(new CastLog(end, -5, (int)(fightDuration - end), ParseEnum.Activation.None, (int)(fightDuration - end), ParseEnum.Activation.None), log);
                    }
                }
                else
                {
                    start = c.Time - log.FightData.FightStart;
                    phases.Add(new PhaseData(end, start));
                    mainTarget.AddCustomCastLog(new CastLog(end, -5, (int)(start - end), ParseEnum.Activation.None, (int)(start - end), ParseEnum.Activation.None), log);
                }
            }
            if (fightDuration - start > 5000 && start >= phases.Last().End)
            {
                phases.Add(new PhaseData(start, fightDuration));
            }
            string[] namesSab = new [] { "Phase 1", "Kernan", "Phase 2", "Knuckles", "Phase 3", "Karde", "Phase 4" };
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                phase.Name = namesSab[i - 1];
                if (i == 2 || i == 4 || i == 6)
                {
                    List<ushort> ids = new List<ushort>
                    {
                       (ushort) Kernan,
                       (ushort) Knuckles,
                       (ushort) Karde,
                    };
                    AddTargetsToPhase(phase, ids, log);
                } else
                {
                    phase.Targets.Add(mainTarget);
                    Target addTarget;
                    switch (i)
                    {
                        case 3:
                            addTarget = Targets.Find(x => x.ID == (ushort)Kernan);
                            if (addTarget == null)
                            {
                                throw new InvalidOperationException("Kernan not found when we should have been able to");
                            }
                            phase.Targets.Add(addTarget);
                            break;
                        case 5:
                            addTarget = Targets.Find(x => x.ID == (ushort)Knuckles);
                            if (addTarget == null)
                            {
                                throw new InvalidOperationException("Knuckles not found when we should have been able to");
                            }
                            phase.Targets.Add(addTarget);
                            break;
                        case 7:
                            addTarget = Targets.Find(x => x.ID == (ushort)Karde);
                            if (addTarget == null)
                            {
                                throw new InvalidOperationException("Karde not found when we should have been able to");
                            }
                            phase.Targets.Add(addTarget);
                            break;
                    }
                }
            }
            return phases;
        }

        protected override List<ushort> GetFightTargetsIDs()
        {
            return new List<ushort>
            {
                (ushort)ParseEnum.TargetIDS.Sabetha,
                (ushort)Kernan,
                (ushort)Knuckles,
                (ushort)Karde,
            };
        }

        public override void ComputeAdditionalTargetData(Target target, ParsedLog log)
        {
            CombatReplay replay = target.CombatReplay;
            List<CastLog> cls = target.GetCastLogs(log, 0, log.FightData.FightDuration);
            switch (target.ID)
            {
                case (ushort)ParseEnum.TargetIDS.Sabetha:
                    List<CastLog> flameWall = cls.Where(x => x.SkillId == 31332).ToList();
                    foreach (CastLog c in flameWall)
                    {
                        int start = (int)c.Time;
                        int preCastTime = 2800;
                        int duration = 10000;
                        int width = 1300; int height = 60;
                        Point3D facing = replay.Rotations.LastOrDefault(x => x.Time <= start);
                        if (facing != null)
                        {
                            int initialDirection = (int)(Math.Atan2(facing.Y, facing.X) * 180 / Math.PI);
                            replay.Actors.Add(new RotatedRectangleActor(true, 0, width, height, initialDirection, width / 2, new Tuple<int, int>(start, start + preCastTime), "rgba(255, 100, 0, 0.2)", new AgentConnector(target)));
                            replay.Actors.Add(new RotatedRectangleActor(true, 0, width, height, initialDirection, width / 2, 360, new Tuple<int, int>(start + preCastTime, start + preCastTime + duration), "rgba(255, 50, 0, 0.5)", new AgentConnector(target)));
                        }
                    }
                    break;
                case (ushort)Kernan:
                    List<CastLog> bulletHail = cls.Where(x => x.SkillId == 31721).ToList();
                    foreach (CastLog c in bulletHail)
                    {
                        int start = (int)c.Time;
                        int firstConeStart = start;
                        int secondConeStart = start + 800;
                        int thirdConeStart = start + 1600;
                        int firstConeEnd = firstConeStart + 400;
                        int secondConeEnd = secondConeStart + 400;
                        int thirdConeEnd = thirdConeStart + 400;
                        int radius = 1500;
                        Point3D facing = replay.Rotations.LastOrDefault(x => x.Time <= start);
                        if (facing != null)
                        {
                            replay.Actors.Add(new PieActor(true, 0, radius, facing, 28, new Tuple<int, int>(firstConeStart, firstConeEnd), "rgba(255,200,0,0.3)",new AgentConnector(target)));
                            replay.Actors.Add(new PieActor(true, 0, radius, facing, 54, new Tuple<int, int>(secondConeStart, secondConeEnd), "rgba(255,200,0,0.3)", new AgentConnector(target)));
                            replay.Actors.Add(new PieActor(true, 0, radius, facing, 81, new Tuple<int, int>(thirdConeStart, thirdConeEnd), "rgba(255,200,0,0.3)", new AgentConnector(target)));
                        }
                    }
                    break;
                case (ushort)Knuckles:
                    List<CastLog> breakbar = cls.Where(x => x.SkillId == 31763).ToList();
                    foreach (CastLog c in breakbar)
                    {
                        replay.Actors.Add(new CircleActor(true, 0, 180, new Tuple<int, int>((int)c.Time, (int)c.Time + c.ActualDuration), "rgba(0, 180, 255, 0.3)", new AgentConnector(target)));
                    }
                    break;
                case (ushort)Karde:
                    List<CastLog> flameBlast = cls.Where(x => x.SkillId == 31761).ToList();
                    foreach (CastLog c in flameBlast)
                    {
                        int start = (int)c.Time;
                        int end = start + 4000;
                        int radius = 600;
                        Point3D facing = replay.Rotations.LastOrDefault(x => x.Time <= start);
                        if (facing != null)
                        {
                            replay.Actors.Add(new PieActor(true, 0, radius, facing, 60, new Tuple<int, int>(start, end), "rgba(255,200,0,0.5)", new AgentConnector(target)));
                        }
                    }
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }

        protected override List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>
            {
                BanditSapper,
                BanditThug,
                BanditArsonist
            };
        }

        public override void ComputeAdditionalThrashMobData(Mob mob, ParsedLog log)
        {
            switch (mob.ID)
            {
                case (ushort)BanditArsonist:
                case (ushort)BanditThug:
                case (ushort)BanditSapper:
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }

        public override void ComputeAdditionalPlayerData(Player p, ParsedLog log)
        {
            // timed bombs
            CombatReplay replay = p.CombatReplay;
            List<CombatItem> timedBombs = log.GetBoonData(31485).Where(x => x.DstInstid == p.InstID && x.IsBuffRemove == ParseEnum.BuffRemove.None).ToList();
            foreach (CombatItem c in timedBombs)
            {
                int start = (int)(c.Time - log.FightData.FightStart);
                int end = start + 3000;
                replay.Actors.Add(new CircleActor(false, 0, 280, new Tuple<int, int>(start, end), "rgba(255, 150, 0, 0.5)", new AgentConnector(p)));
                replay.Actors.Add(new CircleActor(true, end, 280, new Tuple<int, int>(start, end), "rgba(255, 150, 0, 0.5)", new AgentConnector(p)));
            }
            // Sapper bombs
            List<CombatItem> sapperBombs = GetFilteredList(log, 31473, p);
            int sapperStart = 0;
            foreach (CombatItem c in sapperBombs)
            {
                if (c.IsBuffRemove == ParseEnum.BuffRemove.None)
                {
                    sapperStart = (int)(c.Time - log.FightData.FightStart);
                }
                else
                {
                    int sapperEnd = (int)(c.Time - log.FightData.FightStart); replay.Actors.Add(new CircleActor(false, 0, 180, new Tuple<int, int>(sapperStart, sapperEnd), "rgba(200, 255, 100, 0.5)", new AgentConnector(p)));
                    replay.Actors.Add(new CircleActor(true, sapperStart + 5000, 180, new Tuple<int, int>(sapperStart, sapperEnd), "rgba(200, 255, 100, 0.5)", new AgentConnector(p)));
                }
            }
        }
    }
}
