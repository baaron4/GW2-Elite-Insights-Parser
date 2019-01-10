using LuckParser.Parser;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static LuckParser.Parser.ParseEnum.TrashIDS;

namespace LuckParser.Models.Logic
{
    public class ValeGuardian : RaidLogic
    {
        public ValeGuardian(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new Mechanic(31860, "Unstable Magic Spike", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("circle","rgb(0,0,255)"), "Split TP","Unstable Magic Spike (Green Guard Teleport)","Green Guard TP",500),
            new Mechanic(31392, "Unstable Magic Spike", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("circle","rgb(0,0,255)"), "Boss TP","Unstable Magic Spike (Boss Teleport)", "Boss TP",500),
            new Mechanic(31340, "Distributed Magic", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("circle","rgb(0,128,0)"), "Green","Distributed Magic (Stood in Green)", "Green Team",0),
            new Mechanic(31340, "Distributed Magic", Mechanic.MechType.EnemyCastStart, new MechanicPlotlySetting("circle-open","rgb(0,128,255)") , "Green Cast B","Distributed Magic (Green Field appeared in Blue Sector)", "Green in Blue",0),
            new Mechanic(31391, "Distributed Magic", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("circle","rgb(0,128,0)"), "Green","Distributed Magic (Stood in Green)", "Green Team",0),
            new Mechanic(31391, "Distributed Magic", Mechanic.MechType.EnemyCastStart, new MechanicPlotlySetting("circle-open","rgb(255,128,0)"), "Green Cast R","Distributed Magic (Green Field appeared in Red Sector)", "Green in Red",0),
            new Mechanic(31529, "Distributed Magic", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("circle","rgb(0,128,0)"), "Green","Distributed Magic (Stood in Green)", "Green Team", 0),
            new Mechanic(31750, "Distributed Magic", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("circle","rgb(0,128,0)"), "Green","Distributed Magic (Stood in Green)", "Green Team",0),
            new Mechanic(31750, "Distributed Magic", Mechanic.MechType.EnemyCastStart, new MechanicPlotlySetting("circle-open","rgb(0,255,0)"), "Green Cast G","Distributed Magic (Green Field appeared in Green Sector)", "Green in Green",0),
            new Mechanic(31886, "Magic Pulse", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("circle-open","rgb(255,0,0)"), "Seeker","Magic Pulse (Hit by Seeker)", "Seeker",0),
            new Mechanic(31695, "Pylon Attunement: Red", Mechanic.MechType.PlayerBoon, new MechanicPlotlySetting("square","rgb(255,0,0)"), "Attune R","Pylon Attunement: Red", "Red Attuned",0),
            new Mechanic(31317, "Pylon Attunement: Blue", Mechanic.MechType.PlayerBoon, new MechanicPlotlySetting("square","rgb(0,0,255)"), "Attune B","Pylon Attunement: Blue", "Blue Attuned",0),
            new Mechanic(31852, "Pylon Attunement: Green", Mechanic.MechType.PlayerBoon, new MechanicPlotlySetting("square","rgb(0,128,0)"), "Attune G","Pylon Attunement: Green", "Green Attuned",0),
            new Mechanic(31413, "Blue Pylon Power", Mechanic.MechType.EnemyBoonStrip, new MechanicPlotlySetting("square-open","rgb(0,0,255)"), "Invuln Strip","Blue Guard Invuln was stripped", "Blue Invuln Strip",0),
            new Mechanic(31539, "Unstable Pylon", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("hexagram-open","rgb(255,0,0)"), "Floor R","Unstable Pylon (Red Floor dmg)", "Floor dmg",0),
            new Mechanic(31828, "Unstable Pylon", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("hexagram-open","rgb(0,0,255)"), "Floor B","Unstable Pylon (Blue Floor dmg)", "Floor dmg",0),
            new Mechanic(31884, "Unstable Pylon", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("hexagram-open","rgb(0,128,0)"), "Floor G","Unstable Pylon (Green Floor dmg)", "Floor dmg",0),
            new Mechanic(31419, "Magic Storm", Mechanic.MechType.EnemyCastStart, new MechanicPlotlySetting("diamond-tall","rgb(0,160,150)"), "CC","Magic Storm (Breakbar)","Breakbar",0),
            new Mechanic(31419, "Magic Storm", Mechanic.MechType.EnemyCastEnd, new MechanicPlotlySetting("diamond-tall","rgb(0,160,0)"), "CCed","Magic Storm (Breakbar broken) ", "CCed",0,(condition => condition.CombatItem.Value <=8544)),
            new Mechanic(31419, "Magic Storm", Mechanic.MechType.EnemyCastEnd, new MechanicPlotlySetting("diamond-tall","rgb(255,0,0)"), "CC Fail","Magic Storm (Breakbar failed) ", "CC Fail",0,(condition => condition.CombatItem.Value >8544)),
            });
            Extension = "vg";
            IconUrl = "https://wiki.guildwars2.com/images/f/fb/Mini_Vale_Guardian.png";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/W7MocGz.png",
                            (889, 889),
                            (-6365, -22213, -3150, -18999),
                            (-15360, -36864, 15360, 39936),
                            (3456, 11012, 4736, 14212));
        }

        protected override List<ushort> GetFightTargetsIDs()
        {
            return new List<ushort>
            {
                (ushort)ParseEnum.TargetIDS.ValeGuardian,
                (ushort)RedGuardian,
                (ushort)BlueGuardian,
                (ushort)GreenGuardian
            };
        }

        public override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            Target mainTarget = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.ValeGuardian);
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
            string[] namesVG = new[] { "Phase 1", "Split 1", "Phase 2", "Split 2", "Phase 3" };
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                phase.Name = namesVG[i - 1];
                if (i == 2 || i == 4)
                {
                    List<ushort> ids = new List<ushort>
                    {
                       (ushort) BlueGuardian,
                       (ushort) GreenGuardian,
                       (ushort) RedGuardian
                    };
                    AddTargetsToPhase(phase, ids, log);
                }
                else
                {
                    phase.Targets.Add(mainTarget);
                }
            }
            return phases;
        }

        protected override List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>
            {
               Seekers
            };
        }

        public override void ComputeAdditionalThrashMobData(Mob mob, ParsedLog log)
        {
            switch (mob.ID)
            {
                case (ushort)Seekers:
                    (int, int) lifespan = ((int)mob.CombatReplay.TimeOffsets.start, (int)mob.CombatReplay.TimeOffsets.end);
                    mob.CombatReplay.Actors.Add(new CircleActor(false, 0, 180, lifespan, "rgba(255, 0, 0, 0.5)", new AgentConnector(mob)));
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }

        public override void ComputeAdditionalTargetData(Target target, ParsedLog log)
        {
            CombatReplay replay = target.CombatReplay;
            List<CastLog> cls = target.GetCastLogs(log, 0, log.FightData.FightDuration);
            (int, int) lifespan = ((int)target.CombatReplay.TimeOffsets.start, (int)target.CombatReplay.TimeOffsets.end);
            switch (target.ID)
            {
                case (ushort)ParseEnum.TargetIDS.ValeGuardian:
                    List<CastLog> magicStorms = cls.Where(x => x.SkillId == 31419).ToList();
                    foreach (CastLog c in magicStorms)
                    {
                        replay.Actors.Add(new CircleActor(true, 0, 100, ((int)c.Time, (int)c.Time + c.ActualDuration), "rgba(0, 180, 255, 0.3)", new AgentConnector(target)));
                    }
                    int distributedMagicDuration = 6700;
                    int arenaRadius = 1600;
                    int impactDuration = 110;
                    List<CastLog> distributedMagicGreen = cls.Where(x => x.SkillId == 31750).ToList();
                    foreach (CastLog c in distributedMagicGreen)
                    {
                        int start = (int)c.Time;
                        int end = start + distributedMagicDuration;
                        replay.Actors.Add(new PieActor(true, start + distributedMagicDuration, arenaRadius, 151, 120, (start, end), "rgba(0,255,0,0.1)", new PositionConnector(new Point3D(-4749.838867f, -20607.296875f, 0.0f, 0))));
                        replay.Actors.Add(new PieActor(true, 0, arenaRadius, 151, 120, (end, end + impactDuration), "rgba(0,255,0,0.3)", new PositionConnector(new Point3D(-4749.838867f, -20607.296875f, 0.0f, 0))));
                        replay.Actors.Add(new CircleActor(true, 0, 180, (start, end), "rgba(0,255,0,0.2)", new PositionConnector(new Point3D(-5449.0f, -20219.0f, 0.0f, 0))));
                    }
                    List<CastLog> distributedMagicBlue = cls.Where(x => x.SkillId == 31340).ToList();
                    foreach (CastLog c in distributedMagicBlue)
                    {
                        int start = (int)c.Time;
                        int end = start + distributedMagicDuration;
                        replay.Actors.Add(new PieActor(true, start + distributedMagicDuration, arenaRadius, 31, 120, (start, end), "rgba(0,255,0,0.1)", new PositionConnector(new Point3D(-4749.838867f, -20607.296875f, 0.0f, 0))));
                        replay.Actors.Add(new PieActor(true, 0, arenaRadius, 31, 120, (end, end + impactDuration), "rgba(0,255,0,0.3)", new PositionConnector(new Point3D(-4749.838867f, -20607.296875f, 0.0f, 0))));
                        replay.Actors.Add(new CircleActor(true, 0, 180, (start, end), "rgba(0,255,0,0.2)", new PositionConnector(new Point3D(-4063.0f, -20195.0f, 0.0f, 0))));
                    }
                    List<CastLog> distributedMagicRed = cls.Where(x => x.SkillId == 31391).ToList();
                    foreach (CastLog c in distributedMagicRed)
                    {
                        int start = (int)c.Time;
                        int end = start + distributedMagicDuration;
                        replay.Actors.Add(new PieActor(true, start + distributedMagicDuration, arenaRadius, 271, 120, (start, end), "rgba(0,255,0,0.1)", new PositionConnector(new Point3D(-4749.838867f, -20607.296875f, 0.0f, 0))));
                        replay.Actors.Add(new PieActor(true, 0, arenaRadius, 271, 120, (end, end + impactDuration), "rgba(0,255,0,0.3)", new PositionConnector(new Point3D(-4749.838867f, -20607.296875f, 0.0f, 0))));
                        replay.Actors.Add(new CircleActor(true, 0, 180, (start, end), "rgba(0,255,0,0.2)", new PositionConnector(new Point3D(-4735.0f, -21407.0f, 0.0f, 0))));
                    }
                    break;
                case (ushort)BlueGuardian:
                    replay.Actors.Add(new CircleActor(false, 0, 1500, lifespan, "rgba(0, 0, 255, 0.5)", new AgentConnector(target)));
                    break;
                case (ushort)GreenGuardian:
                    replay.Actors.Add(new CircleActor(false, 0, 1500, lifespan, "rgba(0, 255, 0, 0.5)", new AgentConnector(target)));
                    break;
                case (ushort)RedGuardian:
                    replay.Actors.Add(new CircleActor(false, 0, 1500, lifespan, "rgba(255, 0, 0, 0.5)", new AgentConnector(target)));
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }

        public override void ComputeAdditionalPlayerData(Player p, ParsedLog log)
        {
        }
    }
}
