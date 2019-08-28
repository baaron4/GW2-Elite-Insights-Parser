using System;
using System.Collections.Generic;
using System.Linq;
using LuckParser.EIData;
using LuckParser.Parser;
using LuckParser.Parser.ParsedData.CombatEvents;
using static LuckParser.Parser.ParseEnum.TrashIDS;

namespace LuckParser.Logic
{
    public class Sabetha : RaidLogic
    {
        public Sabetha(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new PlayerBoonApplyMechanic(34108, "Shell-Shocked", new MechanicPlotlySetting("circle-open","rgb(0,128,0)"), "Launched","Shell-Shocked (launched up to cannons)", "Shell-Shocked",0),
            new PlayerBoonApplyMechanic(31473, "Sapper Bomb", new MechanicPlotlySetting("circle","rgb(0,128,0)"), "Sap Bomb","Got a Sapper Bomb", "Sapper Bomb",0),
            new PlayerBoonApplyMechanic(31485, "Time Bomb", new MechanicPlotlySetting("circle","rgb(255,140,0)"), "Timed Bomb","Got a Timed Bomb (Expanding circle)", "Timed Bomb",0),
            /*new PlayerBoonApplyMechanic(31324, "Time Bomb Hit", new MechanicPlotlySetting("circle-open","rgb(255,140,0)"), "Timed Bomb Hit","Got hit by Timed Bomb (Expanding circle)", "Timed Bomb Hit",0,
                (ba, log) =>
                {
                    List<AbstractBuffEvent> timedBombRemoved = log.CombatData.GetBoonData(31485).Where(x => x.To == ba.To && x is BuffRemoveAllEvent && Math.Abs(ba.Time - x.Time) <= 50).ToList();
                    if (timedBombRemoved.Count > 0)
                    {
                        return false;
                    }
                    return true;
               }
            }),
            new PlayerBoonApplyMechanic(34152, "Time Bomb Hit", new MechanicPlotlySetting("circle-open","rgb(255,140,0)"), "Timed Bomb Hit","Got hit by Timed Bomb (Expanding circle)", "Timed Bomb Hit",0,
                (ba, log) =>
                {
                    List<AbstractBuffEvent> timedBombRemoved = log.CombatData.GetBoonData(31485).Where(x => x.To == ba.To && x is BuffRemoveAllEvent && Math.Abs(ba.Time - x.Time) <= 50).ToList();
                    if (timedBombRemoved.Count > 0)
                    {
                        return false;
                    }
                    return true;
               }
            }),*/
            new HitOnPlayerMechanic(31332, "Firestorm", new MechanicPlotlySetting("square","rgb(255,0,0)"), "Flamewall","Firestorm (killed by Flamewall)", "Flamewall",0),
            new HitOnPlayerMechanic(31544, "Flak Shot", new MechanicPlotlySetting("hexagram-open","rgb(255,140,0)"), "Flak","Flak Shot (Fire Patches)", "Flak Shot",0),
            new HitOnPlayerMechanic(31643, "Cannon Barrage", new MechanicPlotlySetting("circle","rgb(255,200,0)"), "Cannon","Cannon Barrage (stood in AoE)", "Cannon Shot",0),
            new HitOnPlayerMechanic(31761, "Flame Blast", new MechanicPlotlySetting("triangle-left-open","rgb(255,200,0)"), "Karde Flame","Flame Blast (Karde's Flamethrower)", "Flamethrower (Karde)",0),
            new HitOnPlayerMechanic(31408, "Kick", new MechanicPlotlySetting("triangle-right","rgb(255,0,255)"), "Kick","Kicked by Bandit", "Bandit Kick",0, (de, log) => !de.To.HasBuff(log, 1122, de.Time)),
            new EnemyCastStartMechanic(31763, "Platform Quake", new MechanicPlotlySetting("diamond-tall","rgb(0,160,150)"), "CC","Platform Quake (Breakbar)","Breakbar",0),
            new EnemyCastEndMechanic(31763, "Platform Quake", new MechanicPlotlySetting("diamond-tall","rgb(0,160,0)"), "CCed","Platform Quake (Breakbar broken) ", "CCed",0, (ce, log) => ce.ActualDuration <= 4400),
            new EnemyCastEndMechanic(31763, "Platform Quake", new MechanicPlotlySetting("diamond-tall","rgb(255,0,0)"), "CC Fail","Platform Quake (Breakbar failed) ", "CC Fail",0, (ce,log) =>  ce.ActualDuration > 4400),
            // Hit by Time Bomb could be implemented by checking if a person is affected by ID 31324 (1st Time Bomb) or 34152 (2nd Time Bomb, only below 50% boss HP) without being attributed a bomb (ID: 31485) 3000ms before (+-50ms). I think the actual heavy hit isn't logged because it may be percentage based. Nothing can be found in the logs.
            });
            Extension = "sab";
            IconUrl = "https://wiki.guildwars2.com/images/5/54/Mini_Sabetha.png";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/FwpMbYf.png",
                            (2790, 2763),
                            (-8587, -162, -1601, 6753),
                            (-15360, -36864, 15360, 39936),
                            (3456, 11012, 4736, 14212));
        }

        public override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
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
            phases.AddRange(GetPhasesByInvul(log, 757, mainTarget, true, true));
            string[] namesSab = new[] { "Phase 1", "Kernan", "Phase 2", "Knuckles", "Phase 3", "Karde", "Phase 4" };
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                phase.Name = namesSab[i - 1];
                if (i == 2 || i == 4 || i == 6)
                {
                    var ids = new List<ushort>
                    {
                       (ushort) Kernan,
                       (ushort) Knuckles,
                       (ushort) Karde,
                    };
                    AddTargetsToPhase(phase, ids, log);
                }
                else
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

        public override void ComputeTargetCombatReplayActors(Target target, ParsedLog log, CombatReplay replay)
        {
            List<AbstractCastEvent> cls = target.GetCastLogs(log, 0, log.FightData.FightDuration);
            switch (target.ID)
            {
                case (ushort)ParseEnum.TargetIDS.Sabetha:
                    var flameWall = cls.Where(x => x.SkillId == 31332).ToList();
                    foreach (AbstractCastEvent c in flameWall)
                    {
                        int start = (int)c.Time;
                        int preCastTime = 2800;
                        int duration = 10000;
                        int width = 1300; int height = 60;
                        Point3D facing = replay.Rotations.LastOrDefault(x => x.Time <= start);
                        if (facing != null)
                        {
                            int initialDirection = (int)(Math.Atan2(facing.Y, facing.X) * 180 / Math.PI);
                            replay.Actors.Add(new RotatedRectangleActor(true, 0, width, height, initialDirection, width / 2, (start, start + preCastTime), "rgba(255, 100, 0, 0.2)", new AgentConnector(target)));
                            replay.Actors.Add(new RotatedRectangleActor(true, 0, width, height, initialDirection, width / 2, 360, (start + preCastTime, start + preCastTime + duration), "rgba(255, 50, 0, 0.5)", new AgentConnector(target)));
                        }
                    }
                    break;
                case (ushort)Kernan:
                    var bulletHail = cls.Where(x => x.SkillId == 31721).ToList();
                    foreach (AbstractCastEvent c in bulletHail)
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
                            replay.Actors.Add(new PieActor(true, 0, radius, facing, 28, (firstConeStart, firstConeEnd), "rgba(255,200,0,0.3)", new AgentConnector(target)));
                            replay.Actors.Add(new PieActor(true, 0, radius, facing, 54, (secondConeStart, secondConeEnd), "rgba(255,200,0,0.3)", new AgentConnector(target)));
                            replay.Actors.Add(new PieActor(true, 0, radius, facing, 81, (thirdConeStart, thirdConeEnd), "rgba(255,200,0,0.3)", new AgentConnector(target)));
                        }
                    }
                    break;
                case (ushort)Knuckles:
                    var breakbar = cls.Where(x => x.SkillId == 31763).ToList();
                    foreach (AbstractCastEvent c in breakbar)
                    {
                        replay.Actors.Add(new CircleActor(true, 0, 180, ((int)c.Time, (int)c.Time + c.ActualDuration), "rgba(0, 180, 255, 0.3)", new AgentConnector(target)));
                    }
                    break;
                case (ushort)Karde:
                    var flameBlast = cls.Where(x => x.SkillId == 31761).ToList();
                    foreach (AbstractCastEvent c in flameBlast)
                    {
                        int start = (int)c.Time;
                        int end = start + 4000;
                        int radius = 600;
                        Point3D facing = replay.Rotations.LastOrDefault(x => x.Time <= start);
                        if (facing != null)
                        {
                            replay.Actors.Add(new PieActor(true, 0, radius, facing, 60, (start, end), "rgba(255,200,0,0.5)", new AgentConnector(target)));
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

        public override void ComputePlayerCombatReplayActors(Player p, ParsedLog log, CombatReplay replay)
        {
            // timed bombs
            var timedBombs = log.CombatData.GetBoonData(31485).Where(x => x.To == p.AgentItem && x is BuffApplyEvent).ToList();
            foreach (AbstractBuffEvent c in timedBombs)
            {
                int start = (int)c.Time;
                int end = start + 3000;
                replay.Actors.Add(new CircleActor(false, 0, 280, (start, end), "rgba(255, 150, 0, 0.5)", new AgentConnector(p)));
                replay.Actors.Add(new CircleActor(true, end, 280, (start, end), "rgba(255, 150, 0, 0.5)", new AgentConnector(p)));
            }
            // Sapper bombs
            List<AbstractBuffEvent> sapperBombs = GetFilteredList(log.CombatData, 31473, p, true);
            int sapperStart = 0;
            foreach (AbstractBuffEvent c in sapperBombs)
            {
                if (c is BuffApplyEvent)
                {
                    sapperStart = (int)c.Time;
                }
                else
                {
                    int sapperEnd = (int)c.Time;
                    replay.Actors.Add(new CircleActor(false, 0, 180, (sapperStart, sapperEnd), "rgba(200, 255, 100, 0.5)", new AgentConnector(p)));
                    replay.Actors.Add(new CircleActor(true, sapperStart + 5000, 180, (sapperStart, sapperEnd), "rgba(200, 255, 100, 0.5)", new AgentConnector(p)));
                }
            }
        }

        protected override HashSet<ushort> GetUniqueTargetIDs()
        {
            return new HashSet<ushort>
            {
                (ushort)ParseEnum.TargetIDS.Sabetha,
                (ushort)Kernan,
                (ushort)Karde,
                (ushort)Knuckles,
            };
        }
    }
}
