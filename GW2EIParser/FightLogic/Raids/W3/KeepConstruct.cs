using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using static GW2EIParser.Parser.ParseEnum.TrashIDS;

namespace GW2EIParser.Logic
{
    public class KeepConstruct : RaidLogic
    {
        public KeepConstruct(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new PlayerBuffApplyMechanic(34912, "Fixate", new MechanicPlotlySetting("star","rgb(255,0,255)"), "Fixate","Fixated by Statue", "Fixated",0),
            new PlayerBuffApplyMechanic(34925, "Fixate", new MechanicPlotlySetting("star","rgb(255,0,255)"), "Fixate","Fixated by Statue", "Fixated",0),
            new HitOnPlayerMechanic(35077, "Hail of Fury", new MechanicPlotlySetting("circle-open","rgb(255,0,0)"), "Debris","Hail of Fury (Falling Debris)", "Debris",0),
            new EnemyBuffApplyMechanic(35096, "Compromised", new MechanicPlotlySetting("hexagon","rgb(0,0,255)"), "Rift#","Compromised (Pushed Orb through Rifts)", "Compromised",0),
            new EnemyBuffApplyMechanic(35119, "Magic Blast", new MechanicPlotlySetting("star","rgb(0,255,255)"), "M.B.# 33%","Magic Blast (Orbs eaten by KC) at 33%", "Magic Blast 33%",0, (de, log) => {
                var phases = log.FightData.GetPhases(log).Where(x => x.Name.Contains("%")).ToList();
                if (phases.Count < 2)
                {
                    // no 33% magic blast
                    return false;
                }
                return de.Time >= phases[1].End;
            }),
            new EnemyBuffApplyMechanic(35119, "Magic Blast", new MechanicPlotlySetting("star","rgb(0,255,255)"), "M.B.# 66%","Magic Blast (Orbs eaten by KC) at 66%", "Magic Blast 66%",0, (de, log) => {
                var phases = log.FightData.GetPhases(log).Where(x => x.Name.Contains("%")).ToList();
                if (phases.Count < 1)
                {
                    // no 66% magic blast
                    return false;
                }
                bool condition = de.Time >= phases[0].End;
                if (phases.Count > 1)
                {
                    // must be before 66%-33% phase if it exists
                    condition = condition && de.Time <= phases[1].Start;
                }
                return condition;
            }),
            new SpawnMechanic(16227, "Insidious Projection", new MechanicPlotlySetting("bowtie","rgb(255,0,0)"), "Merge","Insidious Projection spawn (2 Statue merge)", "Merged Statues",0),
            new HitOnPlayerMechanic(35137, "Phantasmal Blades", new MechanicPlotlySetting("hexagram-open","rgb(255,0,255)"), "Pizza","Phantasmal Blades (rotating Attack)", "Phantasmal Blades",0),
            new HitOnPlayerMechanic(34971, "Phantasmal Blades", new MechanicPlotlySetting("hexagram-open","rgb(255,0,255)"), "Pizza","Phantasmal Blades (rotating Attack)", "Phantasmal Blades",0),
            new HitOnPlayerMechanic(35064, "Phantasmal Blades", new MechanicPlotlySetting("hexagram-open","rgb(255,0,255)"), "Pizza","Phantasmal Blades (rotating Attack)", "Phantasmal Blades",0),
            new HitOnPlayerMechanic(35086, "Tower Drop", new MechanicPlotlySetting("circle","rgb(255,140,0)"), "Jump","Tower Drop (KC Jump)", "Tower Drop",0),
            new PlayerBuffApplyMechanic(35103, "Xera's Fury", new MechanicPlotlySetting("circle","rgb(200,140,0)"), "Bomb","Xera's Fury (Large Bombs) application", "Bombs",0),
            new HitOnPlayerMechanic(34914, "Good White Orb", new MechanicPlotlySetting("circle","rgb(200,200,200)"), "GW.Orb","Good White Orb", "Good White Orb",0, (de,log) => de.To.HasBuff(log, 35109, de.Time)),
            new HitOnPlayerMechanic(34972, "Good Red Orb", new MechanicPlotlySetting("circle","rgb(100,0,0)"), "GR.Orb","Good Red Orb", "Good Red Orb",0, (de,log) => de.To.HasBuff(log, 35091, de.Time)),
            new HitOnPlayerMechanic(34914, "Bad White Orb", new MechanicPlotlySetting("circle","rgb(100,100,100)"), "BW.Orb","Bad White Orb", "Bad White Orb",0, (de,log) => !de.To.HasBuff(log, 35109, de.Time)),
            new HitOnPlayerMechanic(34972, "Bad Red Orb", new MechanicPlotlySetting("circle","rgb(200,0,0)"), "BR.Orb","Bad Red Orb", "Bad Red Orb",0, (de,log) => !de.To.HasBuff(log, 35091, de.Time)),
            new HitOnEnemyMechanic(16261, "Core Hit", new MechanicPlotlySetting("star-open","rgb(255,140,0)"), "Core Hit","Core was Hit by Player", "Core Hit",1000)
            });
            Extension = "kc";
            Icon = "https://wiki.guildwars2.com/images/e/ea/Mini_Keep_Construct.png";
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/RZbs21b.png",
                            (1099, 1114),
                            (-5467, 8069, -2282, 11297),
                            (-12288, -27648, 12288, 27648),
                            (1920, 12160, 2944, 14464));
        }

        public override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            long start = 0;
            long end = 0;
            long fightDuration = log.FightData.FightEnd;
            List<PhaseData> phases = GetInitialPhase(log);
            NPC mainTarget = Targets.Find(x => x.ID == (int)ParseEnum.TargetIDS.KeepConstruct);
            if (mainTarget == null)
            {
                throw new InvalidOperationException("Keep Construct not found");
            }
            phases[0].Targets.Add(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            // Main phases 35025
            List<AbstractBuffEvent> kcPhaseInvuls = GetFilteredList(log.CombatData, 35025, mainTarget, true);
            foreach (AbstractBuffEvent c in kcPhaseInvuls)
            {
                if (c is BuffApplyEvent)
                {
                    end = c.Time;
                    phases.Add(new PhaseData(start, end));
                }
                else
                {
                    start = c.Time;
                }
            }
            if (fightDuration - start > GeneralHelper.PhaseTimeLimit && start >= phases.Last().End)
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
            var orbItems = log.CombatData.GetBuffData(35096).Where(x => x.To == mainTarget.AgentItem).ToList();
            // Get number of orbs and filter the list
            start = 0;
            int orbCount = 0;
            var segments = new List<Segment>();
            foreach (AbstractBuffEvent c in orbItems)
            {
                if (c is BuffApplyEvent)
                {
                    if (start == 0)
                    {
                        start = c.Time;
                    }
                    orbCount++;
                }
                else if (start != 0)
                {
                    segments.Add(new Segment(start, Math.Min(c.Time, fightDuration), orbCount));
                    orbCount = 0;
                    start = 0;
                }
            }
            int burnCount = 1;
            foreach (Segment seg in segments)
            {
                var phase = new PhaseData(seg.Start, seg.End, "Burn " + burnCount++ + " (" + seg.Value + " orbs)");
                phase.Targets.Add(mainTarget);
                phases.Add(phase);
            }
            phases.Sort((x, y) => x.Start.CompareTo(y.Start));
            // pre burn phases
            int preBurnCount = 1;
            var preBurnPhase = new List<PhaseData>();
            List<AbstractBuffEvent> kcInvuls = GetFilteredList(log.CombatData, 762, mainTarget, true);
            foreach (AbstractBuffEvent invul in kcInvuls)
            {
                if (invul is BuffApplyEvent)
                {
                    end = invul.Time;
                    PhaseData prevPhase = phases.LastOrDefault(x => x.Start <= end || x.End <= end);
                    if (prevPhase != null)
                    {
                        start = (prevPhase.End >= end ? prevPhase.Start : prevPhase.End) + 1;
                        if (end - start > 1000)
                        {
                            var phase = new PhaseData(start, end, "Pre-Burn " + preBurnCount++);
                            phase.Targets.Add(mainTarget);
                            preBurnPhase.Add(phase);
                        }
                    }
                }
            }
            phases.AddRange(preBurnPhase);
            phases.Sort((x, y) => x.Start.CompareTo(y.Start));
            // add leftover phases
            PhaseData cur = null;
            int leftOverCount = 1;
            var leftOverPhases = new List<PhaseData>();
            for (int i = 0; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                if (phase.Name.Contains("%"))
                {
                    cur = phase;
                }
                else if (phase.Name.Contains("orbs"))
                {
                    if (cur != null)
                    {
                        if (cur.End >= phase.End + 5000 && (i == phases.Count - 1 || phases[i + 1].Name.Contains("%")))
                        {
                            var leftOverPhase = new PhaseData(phase.End + 1, cur.End, "Leftover " + leftOverCount++);
                            leftOverPhase.Targets.Add(mainTarget);
                            leftOverPhases.Add(leftOverPhase);
                        }
                    }
                }
            }
            phases.AddRange(leftOverPhases);
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

        public override void ComputeNPCCombatReplayActors(NPC target, ParsedLog log, CombatReplay replay)
        {
            List<AbstractCastEvent> cls = target.GetCastLogs(log, 0, log.FightData.FightEnd);
            int start = (int)replay.TimeOffsets.start;
            int end = (int)replay.TimeOffsets.end;
            switch (target.ID)
            {
                case (int)ParseEnum.TargetIDS.KeepConstruct:

                    List<AbstractBuffEvent> kcOrbCollect = GetFilteredList(log.CombatData, 35025, target, true);
                    int kcOrbStart = 0, kcOrbEnd = 0;
                    foreach (AbstractBuffEvent c in kcOrbCollect)
                    {
                        if (c is BuffApplyEvent)
                        {
                            kcOrbStart = (int)c.Time;
                        }
                        else
                        {
                            kcOrbEnd = (int)c.Time;
                            replay.Decorations.Add(new CircleDecoration(false, 0, 300, (kcOrbStart, kcOrbEnd), "rgba(255, 0, 0, 0.5)", new AgentConnector(target)));
                            replay.Decorations.Add(new CircleDecoration(true, kcOrbEnd, 300, (kcOrbStart, kcOrbEnd), "rgba(255, 0, 0, 0.5)", new AgentConnector(target)));
                        }
                    }
                    var towerDrop = cls.Where(x => x.SkillId == 35086).ToList();
                    foreach (AbstractCastEvent c in towerDrop)
                    {
                        start = (int)c.Time;
                        end = start + c.ActualDuration;
                        int skillCast = end - 1000;
                        Point3D next = replay.PolledPositions.FirstOrDefault(x => x.Time >= end);
                        Point3D prev = replay.PolledPositions.LastOrDefault(x => x.Time <= end);
                        if (prev != null || next != null)
                        {
                            replay.Decorations.Add(new CircleDecoration(false, 0, 400, (start, skillCast), "rgba(255, 150, 0, 0.5)", new InterpolatedPositionConnector(prev, next, end)));
                            replay.Decorations.Add(new CircleDecoration(true, skillCast, 400, (start, skillCast), "rgba(255, 150, 0, 0.5)", new InterpolatedPositionConnector(prev, next, end)));
                        }
                    }
                    var blades1 = cls.Where(x => x.SkillId == 35064).ToList();
                    var blades2 = cls.Where(x => x.SkillId == 35137).ToList();
                    var blades3 = cls.Where(x => x.SkillId == 34971).ToList();
                    int bladeDelay = 150;
                    int duration = 1000;
                    foreach (AbstractCastEvent c in blades1)
                    {
                        int ticks = (int)Math.Max(0, Math.Min(Math.Ceiling((c.ActualDuration - 1150) / 1000.0), 9));
                        start = (int)c.Time + bladeDelay;
                        Point3D facing = replay.Rotations.LastOrDefault(x => x.Time < start + 1000);
                        if (facing == null)
                        {
                            continue;
                        }
                        replay.Decorations.Add(new CircleDecoration(true, 0, 200, (start, start + (ticks + 1) * 1000), "rgba(255,0,0,0.4)", new AgentConnector(target)));
                        replay.Decorations.Add(new PieDecoration(true, 0, 1600, facing, 360 * 3 / 32, (start, start + 2 * duration), "rgba(255,200,0,0.5)", new AgentConnector(target))); // First blade lasts twice as long
                        for (int i = 1; i < ticks; i++)
                        {
                            replay.Decorations.Add(new PieDecoration(true, 0, 1600, (int)Math.Round(Math.Atan2(facing.Y, facing.X) * 180 / Math.PI + i * 360 / 8), 360 * 3 / 32, (start + 1000 + i * duration, start + 1000 + (i + 1) * duration), "rgba(255,200,0,0.5)", new AgentConnector(target))); // First blade lasts longer
                        }
                    }
                    foreach (AbstractCastEvent c in blades2)
                    {
                        int ticks = (int)Math.Max(0, Math.Min(Math.Ceiling((c.ActualDuration - 1150) / 1000.0), 9));
                        start = (int)c.Time + bladeDelay;
                        Point3D facing = replay.Rotations.LastOrDefault(x => x.Time < start + 1000);
                        if (facing == null)
                        {
                            continue;
                        }
                        replay.Decorations.Add(new CircleDecoration(true, 0, 200, (start, start + (ticks + 1) * 1000), "rgba(255,0,0,0.4)", new AgentConnector(target)));
                        replay.Decorations.Add(new PieDecoration(true, 0, 1600, facing, 360 * 3 / 32, (start, start + 2 * duration), "rgba(255,200,0,0.5)", new AgentConnector(target))); // First blade lasts twice as long
                        replay.Decorations.Add(new PieDecoration(true, 0, 1600, (int)Math.Round(Math.Atan2(-facing.Y, -facing.X) * 180 / Math.PI), 360 * 3 / 32, (start, start + 2 * duration), "rgba(255,200,0,0.5)", new AgentConnector(target))); // First blade lasts twice as long
                        for (int i = 1; i < ticks; i++)
                        {
                            replay.Decorations.Add(new PieDecoration(true, 0, 1600, (int)Math.Round(Math.Atan2(facing.Y, facing.X) * 180 / Math.PI + i * 360 / 8), 360 * 3 / 32, (start + 1000 + i * duration, start + 1000 + (i + 1) * duration), "rgba(255,200,0,0.5)", new AgentConnector(target))); // First blade lasts longer
                            replay.Decorations.Add(new PieDecoration(true, 0, 1600, (int)Math.Round(Math.Atan2(-facing.Y, -facing.X) * 180 / Math.PI + i * 360 / 8), 360 * 3 / 32, (start + 1000 + i * duration, start + 1000 + (i + 1) * duration), "rgba(255,200,0,0.5)", new AgentConnector(target))); // First blade lasts longer
                        }
                    }
                    foreach (AbstractCastEvent c in blades3)
                    {
                        int ticks = (int)Math.Max(0, Math.Min(Math.Ceiling((c.ActualDuration - 1150) / 1000.0), 9));
                        start = (int)c.Time + bladeDelay;
                        Point3D facing = replay.Rotations.LastOrDefault(x => x.Time < start + 1000);
                        if (facing == null)
                        {
                            continue;
                        }
                        replay.Decorations.Add(new CircleDecoration(true, 0, 200, (start, start + (ticks + 1) * 1000), "rgba(255,0,0,0.4)", new AgentConnector(target)));
                        replay.Decorations.Add(new PieDecoration(true, 0, 1600, (int)Math.Round(Math.Atan2(-facing.Y, -facing.X) * 180 / Math.PI), 360 * 3 / 32, (start, start + 2 * duration), "rgba(255,200,0,0.5)", new AgentConnector(target))); // First blade lasts twice as long
                        replay.Decorations.Add(new PieDecoration(true, 0, 1600, (int)Math.Round(Math.Atan2(-facing.Y, -facing.X) * 180 / Math.PI + 120), 360 * 3 / 32, (start, start + 2 * duration), "rgba(255,200,0,0.5)", new AgentConnector(target))); // First blade lasts twice as long
                        replay.Decorations.Add(new PieDecoration(true, 0, 1600, (int)Math.Round(Math.Atan2(-facing.Y, -facing.X) * 180 / Math.PI - 120), 360 * 3 / 32, (start, start + 2 * duration), "rgba(255,200,0,0.5)", new AgentConnector(target))); // First blade lasts twice as long
                        for (int i = 1; i < ticks; i++)
                        {
                            replay.Decorations.Add(new PieDecoration(true, 0, 1600, (int)Math.Round(Math.Atan2(-facing.Y, -facing.X) * 180 / Math.PI + i * 360 / 8), 360 * 3 / 32, (start + 1000 + i * duration, start + 1000 + (i + 1) * duration), "rgba(255,200,0,0.5)", new AgentConnector(target))); // First blade lasts longer
                            replay.Decorations.Add(new PieDecoration(true, 0, 1600, (int)Math.Round(Math.Atan2(-facing.Y, -facing.X) * 180 / Math.PI + i * 360 / 8 + 120), 360 * 3 / 32, (start + 1000 + i * duration, start + 1000 + (i + 1) * duration), "rgba(255,200,0,0.5)", new AgentConnector(target))); // First blade lasts longer
                            replay.Decorations.Add(new PieDecoration(true, 0, 1600, (int)Math.Round(Math.Atan2(-facing.Y, -facing.X) * 180 / Math.PI + i * 360 / 8 - 120), 360 * 3 / 32, (start + 1000 + i * duration, start + 1000 + (i + 1) * duration), "rgba(255,200,0,0.5)", new AgentConnector(target))); // First blade lasts longer
                        }
                    }
                    // phantasms locations
                    var phantasmsID = new HashSet<int>
                    {
                        (int)Jessica,
                        (int)Olson,
                        (int)Engul,
                        (int)Faerla,
                        (int)Caulle,
                        (int)Henley,
                        (int)Galletta,
                        (int)Ianim,
                    };
                    foreach (NPC m in TrashMobs)
                    {
                        if (phantasmsID.Contains(m.ID))
                        {
                            start = (int)m.FirstAware;
                            Point3D pos = m.GetCombatReplayPolledPositions(log).FirstOrDefault();
                            if (pos != null)
                            {
                                replay.Decorations.Add(new CircleDecoration(true, 0, 300, (start - 5000, start), "rgba(220, 50, 0, 0.5)", new PositionConnector(pos)));
                                replay.Decorations.Add(new CircleDecoration(true, start, 300, (start - 5000, start), "rgba(220, 50, 0, 0.5)", new PositionConnector(pos)));
                            }
                        }
                    }
                    break;

                case (int)Core:
                    break;
                case (int)Jessica:
                case (int)Olson:
                case (int)Engul:
                case (int)Faerla:
                case (int)Caulle:
                case (int)Henley:
                case (int)Galletta:
                case (int)Ianim:
                    NPC mainTarget = Targets.Find(x => x.ID == (int)ParseEnum.TargetIDS.KeepConstruct);
                    if (mainTarget == null)
                    {
                        throw new InvalidOperationException("Keep Construct not found");
                    }
                    replay.Decorations.Add(new CircleDecoration(false, 0, 600, (start, end), "rgba(255, 0, 0, 0.5)", new AgentConnector(target)));
                    replay.Decorations.Add(new CircleDecoration(true, 0, 400, (start, end), "rgba(0, 125, 255, 0.5)", new AgentConnector(target)));
                    break;
                case (int)GreenPhantasm:
                    int lifetime = 8000;
                    replay.Decorations.Add(new CircleDecoration(true, 0, 210, (start, start + lifetime), "rgba(0,255,0,0.2)", new AgentConnector(target)));
                    replay.Decorations.Add(new CircleDecoration(true, start + lifetime, 210, (start, start + lifetime), "rgba(0,255,0,0.3)", new AgentConnector(target)));
                    break;
                case (int)RetrieverProjection:
                case (int)InsidiousProjection:
                case (int)UnstableLeyRift:
                case (int)RadiantPhantasm:
                case (int)CrimsonPhantasm:
                    break;
                default:
                    break;
            }

        }

        public override void ComputePlayerCombatReplayActors(Player p, ParsedLog log, CombatReplay replay)
        {
            // Bombs
            List<AbstractBuffEvent> xeraFury = GetFilteredList(log.CombatData, 35103, p, true);
            int xeraFuryStart = 0;
            foreach (AbstractBuffEvent c in xeraFury)
            {
                if (c is BuffApplyEvent)
                {
                    xeraFuryStart = (int)c.Time;
                }
                else
                {
                    int xeraFuryEnd = (int)c.Time;
                    replay.Decorations.Add(new CircleDecoration(true, 0, 550, (xeraFuryStart, xeraFuryEnd), "rgba(200, 150, 0, 0.2)", new AgentConnector(p)));
                    replay.Decorations.Add(new CircleDecoration(true, xeraFuryEnd, 550, (xeraFuryStart, xeraFuryEnd), "rgba(200, 150, 0, 0.4)", new AgentConnector(p)));
                }

            }
            //fixated Statue
            var fixatedStatue = GetFilteredList(log.CombatData, 34912, p, true).Concat(GetFilteredList(log.CombatData, 34925, p, true)).ToList();
            int fixationStatueStart = 0;
            NPC statue = null;
            foreach (AbstractBuffEvent c in fixatedStatue)
            {
                if (c is BuffApplyEvent)
                {
                    fixationStatueStart = (int)c.Time;
                    statue = TrashMobs.FirstOrDefault(x => x.AgentItem == c.By);
                }
                else
                {
                    int fixationStatueEnd = (int)c.Time;
                    if (statue != null)
                    {
                        replay.Decorations.Add(new LineDecoration(0, (fixationStatueStart, fixationStatueEnd), "rgba(255, 0, 255, 0.5)", new AgentConnector(p), new AgentConnector(statue)));
                    }
                }
            }
        }
    }
}
