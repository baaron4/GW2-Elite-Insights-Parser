using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class Sabetha : SpiritVale
    {
        public Sabetha(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new PlayerBuffApplyMechanic(ShellShocked, "Shell-Shocked", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.DarkGreen), "Launched","Shell-Shocked (launched up to cannons)", "Shell-Shocked",0),
            new PlayerBuffApplyMechanic(SapperBomb, "Sapper Bomb", new MechanicPlotlySetting(Symbols.Circle,Colors.DarkGreen), "Sap Bomb","Got a Sapper Bomb", "Sapper Bomb",0),
            new PlayerBuffApplyMechanic(TimeBomb, "Time Bomb", new MechanicPlotlySetting(Symbols.Circle,Colors.LightOrange), "Timed Bomb","Got a Timed Bomb (Expanding circle)", "Timed Bomb",0),
            /*new PlayerBoonApplyMechanic(31324, "Time Bomb Hit", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.LightOrange), "Timed Bomb Hit","Got hit by Timed Bomb (Expanding circle)", "Timed Bomb Hit",0,
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
            new PlayerBoonApplyMechanic(34152, "Time Bomb Hit", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.LightOrange), "Timed Bomb Hit","Got hit by Timed Bomb (Expanding circle)", "Timed Bomb Hit",0,
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
            new SkillOnPlayerMechanic(Firestorm, "Firestorm", new MechanicPlotlySetting(Symbols.Square,Colors.Red), "Flamewall","Firestorm (killed by Flamewall)", "Flamewall",0, (de, log) => de.HasKilled),
            new HitOnPlayerMechanic(FlakShot, "Flak Shot", new MechanicPlotlySetting(Symbols.HexagramOpen,Colors.LightOrange), "Flak","Flak Shot (Fire Patches)", "Flak Shot",0),
            new HitOnPlayerMechanic(CannonBarrage, "Cannon Barrage", new MechanicPlotlySetting(Symbols.Circle,Colors.Yellow), "Cannon","Cannon Barrage (stood in AoE)", "Cannon Shot",0),
            new HitOnPlayerMechanic(FlameBlast, "Flame Blast", new MechanicPlotlySetting(Symbols.TriangleLeftOpen,Colors.Yellow), "Karde Flame","Flame Blast (Karde's Flamethrower)", "Flamethrower (Karde)",0),
            new HitOnPlayerMechanic(BanditKick, "Kick", new MechanicPlotlySetting(Symbols.TriangleRight,Colors.Magenta), "Kick","Kicked by Bandit", "Bandit Kick",0, (de, log) => !de.To.HasBuff(log, Stability, de.Time - ParserHelper.ServerDelayConstant)),
            new EnemyCastStartMechanic(PlatformQuake, "Platform Quake", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkTeal), "CC","Platform Quake (Breakbar)","Breakbar",0),
            new EnemyCastEndMechanic(PlatformQuake, "Platform Quake", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkGreen), "CCed","Platform Quake (Breakbar broken) ", "CCed",0, (ce, log) => ce.ActualDuration <= 4400),
            new EnemyCastEndMechanic(PlatformQuake, "Platform Quake", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Red), "CC Fail","Platform Quake (Breakbar failed) ", "CC Fail",0, (ce,log) =>  ce.ActualDuration > 4400),
            // Hit by Time Bomb could be implemented by checking if a person is affected by ID 31324 (1st Time Bomb) or 34152 (2nd Time Bomb, only below 50% boss HP) without being attributed a bomb (ID: 31485) 3000ms before (+-50ms). I think the actual heavy hit isn't logged because it may be percentage based. Nothing can be found in the logs.
            });
            Extension = "sab";
            Icon = "https://wiki.guildwars2.com/images/5/54/Mini_Sabetha.png";
            EncounterCategoryInformation.InSubCategoryOrder = 2;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/HXjxqlu.png",
                            (1000, 990),
                            (-8587, -162, -1601, 6753)/*,
                            (-15360, -36864, 15360, 39936),
                            (3456, 11012, 4736, 14212)*/);
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.Sabetha);
            if (mainTarget == null)
            {
                throw new MissingKeyActorsException("Sabetha not found");
            }
            phases[0].AddTarget(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            // Invul check
            phases.AddRange(GetPhasesByInvul(log, 757, mainTarget, true, true));
            var ids = new List<int>
                    {
                       (int) ArcDPSEnums.TrashID.Kernan,
                       (int) ArcDPSEnums.TrashID.Knuckles,
                       (int) ArcDPSEnums.TrashID.Karde,
                    };
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                if (i % 2 == 0)
                {
                    AddTargetsToPhaseAndFit(phase, ids, log);
                    if (phase.Targets.Count > 0)
                    {
                        AbstractSingleActor phaseTar = phase.Targets[0];
                        switch (phaseTar.ID)
                        {
                            case (int)ArcDPSEnums.TrashID.Kernan:
                                phase.Name = "Kernan";
                                break;
                            case (int)ArcDPSEnums.TrashID.Knuckles:
                                phase.Name = "Knuckles";
                                break;
                            case (int)ArcDPSEnums.TrashID.Karde:
                                phase.Name = "Karde";
                                break;
                            default:
                                phase.Name = "Unknown";
                                break;
                        }
                    }
                }
                else
                {
                    int phaseID = (i + 1) / 2;
                    phase.Name = "Phase " + phaseID;
                    phase.AddTarget(mainTarget);
                    switch(phaseID)
                    {
                        case 2:
                            phase.AddTargets(Targets.Where(x => x.ID == (int)ArcDPSEnums.TrashID.Kernan));
                            break;
                        case 3:
                            phase.AddTargets(Targets.Where(x => x.ID == (int)ArcDPSEnums.TrashID.Knuckles));
                            break;
                        case 4:
                            phase.AddTargets(Targets.Where(x => x.ID == (int)ArcDPSEnums.TrashID.Karde));
                            break;
                        default:
                            break;
                    }
                }
            }
            return phases;
        }

        protected override List<int> GetTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.Sabetha,
                (int)ArcDPSEnums.TrashID.Kernan,
                (int)ArcDPSEnums.TrashID.Knuckles,
                (int)ArcDPSEnums.TrashID.Karde,
            };
        }

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            IReadOnlyList<AbstractCastEvent> cls = target.GetCastEvents(log, 0, log.FightData.FightEnd);
            switch (target.ID)
            {
                case (int)ArcDPSEnums.TargetID.Sabetha:
                    var flameWall = cls.Where(x => x.SkillId == Firestorm).ToList();
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
                            replay.Decorations.Add(new RotatedRectangleDecoration(true, 0, width, height, initialDirection, width / 2, (start, start + preCastTime), "rgba(255, 100, 0, 0.2)", new AgentConnector(target)));
                            replay.Decorations.Add(new RotatedRectangleDecoration(true, 0, width, height, initialDirection, width / 2, 360, (start + preCastTime, start + preCastTime + duration), "rgba(255, 50, 0, 0.5)", new AgentConnector(target)));
                        }
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.Kernan:
                    var bulletHail = cls.Where(x => x.SkillId == BulletHail).ToList();
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
                            replay.Decorations.Add(new PieDecoration(true, 0, radius, facing, 28, (firstConeStart, firstConeEnd), "rgba(255,200,0,0.3)", new AgentConnector(target)));
                            replay.Decorations.Add(new PieDecoration(true, 0, radius, facing, 54, (secondConeStart, secondConeEnd), "rgba(255,200,0,0.3)", new AgentConnector(target)));
                            replay.Decorations.Add(new PieDecoration(true, 0, radius, facing, 81, (thirdConeStart, thirdConeEnd), "rgba(255,200,0,0.3)", new AgentConnector(target)));
                        }
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.Knuckles:
                    var breakbar = cls.Where(x => x.SkillId == PlatformQuake).ToList();
                    foreach (AbstractCastEvent c in breakbar)
                    {
                        replay.Decorations.Add(new CircleDecoration(true, 0, 180, ((int)c.Time, (int)c.EndTime), "rgba(0, 180, 255, 0.3)", new AgentConnector(target)));
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.Karde:
                    var flameBlast = cls.Where(x => x.SkillId == FlameBlast).ToList();
                    foreach (AbstractCastEvent c in flameBlast)
                    {
                        int start = (int)c.Time;
                        int end = start + 4000;
                        int radius = 600;
                        Point3D facing = replay.Rotations.LastOrDefault(x => x.Time <= start);
                        if (facing != null)
                        {
                            replay.Decorations.Add(new PieDecoration(true, 0, radius, facing, 60, (start, end), "rgba(255,200,0,0.5)", new AgentConnector(target)));
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.BanditSapper,
                ArcDPSEnums.TrashID.BanditThug,
                ArcDPSEnums.TrashID.BanditArsonist
            };
        }

        internal override void ComputePlayerCombatReplayActors(AbstractPlayer p, ParsedEvtcLog log, CombatReplay replay)
        {
            // timed bombs
            var timedBombs = log.CombatData.GetBuffData(TimeBomb).Where(x => x.To == p.AgentItem && x is BuffApplyEvent).ToList();
            foreach (AbstractBuffEvent c in timedBombs)
            {
                int start = (int)c.Time;
                int end = start + 3000;
                replay.Decorations.Add(new CircleDecoration(false, 0, 280, (start, end), "rgba(255, 150, 0, 0.5)", new AgentConnector(p)));
                replay.Decorations.Add(new CircleDecoration(true, end, 280, (start, end), "rgba(255, 150, 0, 0.5)", new AgentConnector(p)));
            }
            // Sapper bombs
            List<AbstractBuffEvent> sapperBombs = GetFilteredList(log.CombatData, SapperBomb, p, true, true);
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
                    replay.Decorations.Add(new CircleDecoration(false, 0, 180, (sapperStart, sapperEnd), "rgba(200, 255, 100, 0.5)", new AgentConnector(p)));
                    replay.Decorations.Add(new CircleDecoration(true, sapperStart + 5000, 180, (sapperStart, sapperEnd), "rgba(200, 255, 100, 0.5)", new AgentConnector(p)));
                }
            }
        }

        protected override HashSet<int> GetUniqueNPCIDs()
        {
            return new HashSet<int>
            {
                (int)ArcDPSEnums.TargetID.Sabetha,
                (int)ArcDPSEnums.TrashID.Kernan,
                (int)ArcDPSEnums.TrashID.Karde,
                (int)ArcDPSEnums.TrashID.Knuckles,
            };
        }
    }
}
