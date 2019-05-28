using LuckParser.Parser;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static LuckParser.Parser.ParseEnum.TrashIDS;

namespace LuckParser.Models.Logic
{
    public class Qadim : RaidLogic
    {
        public Qadim(ushort triggerID, AgentData agentData) : base(triggerID, agentData)
        {
            MechanicList.AddRange(new List<Mechanic>
            { 
            new EnemyCastStartMechanic(51943, "Qadim CC", new MechanicPlotlySetting("diamond-tall","rgb(0,160,150)"), "Qadim CC","Qadim CC", "Qadim CC",0),
            new EnemyCastEndMechanic(51943, "Qadim CC", new MechanicPlotlySetting("diamond-tall","rgb(0,160,0)"), "Qadim CCed","Quadim Breakbar broken", "Quadim CCed",0, new List<MechanicChecker>{ new CombatItemValueChecker(6500, MechanicChecker.ValueCompare.L) }, Mechanic.TriggerRule.AND),
            new EnemyCastStartMechanic(52265, "Riposte", new MechanicPlotlySetting("diamond-tall","rgb(255,0,0)"), "Qadim CC Fail","Qadim Breakbar failed", "Quadim CC Fail",0),
            new SkillOnPlayerMechanic(52265, "Riposte", new MechanicPlotlySetting("circle","rgb(255,0,255)"), "NoCC Attack", "Riposte (Attack if CC on Qadim failed)", "Riposte (No CC)", 0),
            new SkillOnPlayerMechanic(52614, "Fiery Dance", new MechanicPlotlySetting("asterisk-open","rgb(255,100,0)"), "Fiery Dance", "Fiery Dance (Fire running along metal edges)", "Fire on Lines", 0),
            new SkillOnPlayerMechanic(52864, "Fiery Dance", new MechanicPlotlySetting("asterisk-open","rgb(255,100,0)"), "Fiery Dance", "Fiery Dance (Fire running along metal edges)", "Fire on Lines", 0),
            new SkillOnPlayerMechanic(53153, "Fiery Dance", new MechanicPlotlySetting("asterisk-open","rgb(255,100,0)"), "Fiery Dance", "Fiery Dance (Fire running along metal edges)", "Fire on Lines", 0),
            new SkillOnPlayerMechanic(52383, "Fiery Dance", new MechanicPlotlySetting("asterisk-open","rgb(255,100,0)"), "Fiery Dance", "Fiery Dance (Fire running along metal edges)", "Fire on Lines", 0),
            new SkillOnPlayerMechanic(52242, "Shattering Impact", new MechanicPlotlySetting("circle","rgb(255,200,0)"), "Stun","Shattering Impact (Stunning flame bolt)", "Flame Bolt Stun",0),
            new SkillOnPlayerMechanic(52814, "Flame Wave", new MechanicPlotlySetting("star-triangle-up-open","rgb(255,150,0)"), "KB","Flame Wave (Knockback Frontal Beam)", "KB Push",0),
            new SkillOnPlayerMechanic(52820, "Fire Wave", new MechanicPlotlySetting("circle-open","rgb(255,100,0)"), "Qadim Wave","Fire Wave (Shockwave after Qadim's Mace attack)", "Mace Shockwave",0),
            new SkillOnPlayerMechanic(52224, "Fire Wave", new MechanicPlotlySetting("circle-open","rgb(255,100,0)"), "Destr Wave","Fire Wave (Shockwave after Destroyer's Jump or Stomp)", "Destroyer Shockwave",0),
            new SkillOnPlayerMechanic(52520, "Elemental Breath", new MechanicPlotlySetting("triangle-left","rgb(255,0,0)"), "Hydra Breath","Elemental Breath (Hydra Breath)", "Hydra Breath",0),
            new SkillOnPlayerMechanic(53013, "Fireball", new MechanicPlotlySetting("circle-open","rgb(255,200,0)",10), "Hydra FBall","Fireball (Hydra)", "Hydra Fireball",0),
            new SkillOnPlayerMechanic(52941, "Fiery Meteor", new MechanicPlotlySetting("circle-open","rgb(255,150,0)"), "Hydra Meteor","Fiery Meteor (Hydra)", "Hydra Meteor",0),
            new EnemyCastStartMechanic(52941, "Fiery Meteor", new MechanicPlotlySetting("diamond-tall","rgb(0,160,150)"), "Hydra CC","Fiery Meteor (Hydra Breakbar)", "Hydra CC",0),
            //new Mechanic(718, "Fiery Meteor (Spawn)", Mechanic.MechType.EnemyBoon, ParseEnum.BossIDS.Qadim, new MechanicPlotlySetting("diamond-tall","rgb(150,0,0)"), "H.CC.Fail","Fiery Meteor Spawned (Hydra Breakbar)", "Hydra CC Fail",0,(condition =>  condition.CombatItem.IFF == ParseEnum.IFF.Foe)),
            new EnemyCastEndMechanic(52941, "Fiery Meteor", new MechanicPlotlySetting("diamond-tall","rgb(0,160,0)"), "Hydra CCed","Fiery Meteor (Hydra Breakbar broken)", "Hydra CCed",0,new List<MechanicChecker>{ new CombatItemValueChecker(12364, MechanicChecker.ValueCompare.L) }, Mechanic.TriggerRule.AND),
            new EnemyCastEndMechanic(52941, "Fiery Meteor", new MechanicPlotlySetting("diamond-tall","rgb(0,160,0)"), "Hydra CC Fail","Fiery Meteor (Hydra Breakbar not broken)", "Hydra CC Failed",0,new List<MechanicChecker>{ new CombatItemValueChecker(12364, MechanicChecker.ValueCompare.GEQ) }, Mechanic.TriggerRule.AND),
            new SkillOnPlayerMechanic(53051, "Teleport", new MechanicPlotlySetting("circle","rgb(150,0,200)"), "Hydra KB","Teleport Knockback (Hydra)", "Hydra TP KB",0),
            new SkillOnPlayerMechanic(52310, "Big Hit", new MechanicPlotlySetting("circle","rgb(255,0,0)"), "Mace","Big Hit (Mace Impact)", "Mace Impact",0),
            new SkillOnPlayerMechanic(52587, "Inferno", new MechanicPlotlySetting("triangle-down-open","rgb(255,0,0)"), "Inferno","Inferno (Lava Pool drop  on long platform spokes)", "Inferno Pool",0),
            new SkillOnPlayerMechanic(51958, "Slash (Wyvern)", new MechanicPlotlySetting("triangle-down-open","rgb(255,200,0)"), "Slash","Wyvern Slash (Double attack: knock into pin down)", "KB/Pin down",0),
            new SkillOnPlayerMechanic(52705, "Tail Swipe", new MechanicPlotlySetting("diamond-open","rgb(255,200,0)"), "Wyv Pizza","Wyvern Tail Swipe (Pizza attack)", "Tail Swipe",0),
            new SkillOnPlayerMechanic(52726, "Fire Breath", new MechanicPlotlySetting("triangle-right-open","rgb(255,100,0)"), "Wyv Breath","Fire Breath (Wyvern)", "Fire Breath",0),
            new SkillOnPlayerMechanic(52734, "Wing Buffet", new MechanicPlotlySetting("star-diamond-open","rgb(0,125,125)"), "Wyv Wing","Wing Buffet (Wyvern Launching Wing Storm)", "Wing Buffet",0),
            new EnemyCastStartMechanic(53132, "Patriarch CC", new MechanicPlotlySetting("diamond-tall","rgb(0,160,150)"), "Wyv BB","Platform Destruction (Patriarch CC)", "Patriarch CC",0),
            new EnemyCastEndMechanic(53132, "Patriarch CC", new MechanicPlotlySetting("diamond-tall","rgb(0,160,0)"), "Wyv CCed","Platform Destruction (Patriarch Breakbar broken)", "Patriarch CCed",0, new List<MechanicChecker>{ new CombatItemValueChecker(6500, MechanicChecker.ValueCompare.L) }, Mechanic.TriggerRule.AND),
            new EnemyCastStartMechanic(51984, "Patriarch CC (Jump into air)", new MechanicPlotlySetting("diamond-tall","rgb(255,0,0)"), "Wyv CC Fail","Platform Destruction (Patriarch Breakbar failed)", "Patriarch CC Fail",0),
            new SkillOnPlayerMechanic(52330, "Seismic Stomp", new MechanicPlotlySetting("star-open","rgb(255,255,0)"), "Dest Stomp","Seismic Stomp (Destroyer Stomp)", "Seismic Stomp (Destroyer)",0),
            new SkillOnPlayerMechanic(51923, "Shattered Earth", new MechanicPlotlySetting("hexagram-open","rgb(255,0,0)"), "Dest Slam","Shattered Earth (Destroyer Jump Slam)", "Jump Slam (Destroyer)",0),
            new SkillOnPlayerMechanic(51759, "Wave of Force", new MechanicPlotlySetting("diamond-open","rgb(255,200,0)"), "Dest Pizza","Wave of Force (Destroyer Pizza)", "Destroyer Auto",0),
            new EnemyCastStartMechanic(52054, "Summon", new MechanicPlotlySetting("diamond-tall","rgb(0,160,150)"), "Dest CC","Summon (Destroyer Breakbar)", "Destroyer CC",0),
            new EnemyCastEndMechanic(52054, "Summon", new MechanicPlotlySetting("diamond-tall","rgb(0,160,0)"), "Dest CCed","Summon (Destroyer Breakbar broken)", "Destroyer CCed",0, new List<MechanicChecker>{ new CombatItemValueChecker(8332, MechanicChecker.ValueCompare.L) }, Mechanic.TriggerRule.AND),
            new EnemyCastEndMechanic(52054, "Summon", new MechanicPlotlySetting("diamond-tall","rgb(255,0,0)"), "Dest CC Fail","Summon (Destroyer Breakbar failed)", "Destroyer CC Fail",0, new List<MechanicChecker>{ new CombatItemValueChecker(8332, MechanicChecker.ValueCompare.GEQ) }, Mechanic.TriggerRule.AND),
            new SpawnMechanic(20944, "Summon (Spawn)", new MechanicPlotlySetting("diamond-tall","rgb(150,0,0)"), "Dest Spwn","Summon (Destroyer Trolls summoned)", "Destroyer Summoned",0),
            new SkillOnPlayerMechanic(51879, "Body of Flame", new MechanicPlotlySetting("star-open","rgb(255,150,0)",10), "Pyre AoE","Body of Flame (Pyre Ground AoE (CM))", "Pyre Hitbox AoE",0),
            new SkillOnPlayerMechanic(52461, "Sea of Flame", new MechanicPlotlySetting("circle-open","rgb(255,0,0)"), "Qadim Hitbox","Sea of Flame (Stood in Qadim Hitbox)", "Qadim Hitbox AoE",0),
            new SkillOnPlayerMechanic(52221, "Claw", new MechanicPlotlySetting("triangle-left-open","rgb(0,150,150)",10), "Claw","Claw (Reaper of Flesh attack)", "Reaper Claw",0),
            new SkillOnPlayerMechanic(52281, "Swap", new MechanicPlotlySetting("circle-cross-open","rgb(170,0,170)"), "Port","Swap (Ported from below Legendary Creature to Qadim)", "Port to Qadim",0),
            new PlayerBoonApplyMechanic(52035, "Power of the Lamp", new MechanicPlotlySetting("triangle-up","rgb(100,150,255)",10), "Lamp","Power of the Lamp (Returned from the Lamp)", "Lamp Return",0),
            });
            Extension = "qadim";
            IconUrl = "https://wiki.guildwars2.com/images/f/f2/Mini_Qadim.png";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/gHq0j79.png",
                            (3903, 3878),
                            (-11676, 8825, -3870, 16582),
                            (-21504, -21504, 24576, 24576),
                            (13440, 14336, 15360, 16256));
        }


        protected override List<ushort> GetFightTargetsIDs()
        {
            return new List<ushort>
            {
                (ushort)ParseEnum.TargetIDS.Qadim,
                (ushort)AncientInvokedHydra,
                (ushort)WyvernMatriarch,
                (ushort)WyvernPatriarch,
                (ushort)ApocalypseBringer,
            };
        }

        protected override HashSet<ushort> GetUniqueTargetIDs()
        {
            return new HashSet<ushort>
            {
                (ushort)ParseEnum.TargetIDS.Qadim,
                (ushort)AncientInvokedHydra,
                (ushort)ApocalypseBringer,
                (ushort)WyvernMatriarch,
                (ushort)WyvernPatriarch
            };
        }

        public override void SpecialParse(FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            // Find target
            Target target = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.Qadim);
            if (target == null)
            {
                throw new InvalidOperationException("Main target of the fight not found");
            }
            CombatItem startCast = combatData.FirstOrDefault(x => x.SkillID == 52496 && x.IsActivation.StartCasting());
            if (startCast == null)
            {
                throw new Exceptions.TooShortException();
            }
            fightData.OverrideStart(startCast.Time);
        }

        public override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            // Warning: Combat replay relies on these phases.
            // If changing phase detection, combat replay platform timings may have to be updated.

            List<PhaseData> phases = GetInitialPhase(log);
            Target qadim = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.Qadim);
            if (qadim == null)
            {
                throw new InvalidOperationException("Qadim not found");
            }
            phases[0].Targets.Add(qadim);
            if (!requirePhases)
            {
                return phases;
            }
            phases.AddRange(GetPhasesByInvul(log, 52329, qadim, true, false));
            string[] names = { "Hydra", "Qadim P1", "Apocalypse", "Qadim P2", "Wyvern", "Qadim P3" };
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                phase.Name = names[i - 1];
                switch (i)
                {
                    case 2:
                    case 4:
                    case 6:
                        List<long> pyresFirstAware = log.AgentData.GetAgentsByID((ushort)PyreGuardian).Where(x => phase.InInterval(log.FightData.ToFightSpace(x.FirstAware))).Select(x => log.FightData.ToFightSpace(x.FirstAware)).ToList();
                        if (pyresFirstAware.Count > 0 && pyresFirstAware.Max() > phase.Start)
                        {
                            phase.OverrideStart(pyresFirstAware.Max());
                        }
                        phase.Targets.Add(qadim);
                        break;
                    default:
                        List<ushort> ids = new List<ushort>
                        {
                           (ushort) WyvernMatriarch,
                           (ushort) WyvernPatriarch,
                           (ushort) AncientInvokedHydra,
                           (ushort) ApocalypseBringer
                        };
                        AddTargetsToPhase(phase, ids, log);
                        break;
                }
            }
            phases.RemoveAll(x => x.Start >= x.End);
            return phases;
        }

        protected override List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>()
            {
                LavaElemental1,
                LavaElemental2,
                IcebornHydra,
                GreaterMagmaElemental1,
                GreaterMagmaElemental2,
                FireElemental,
                FireImp,
                PyreGuardian,
                ReaperofFlesh,
                DestroyerTroll,
                IceElemental,
                Zommoros
            };
        }

        public override void ComputeTargetCombatReplayActors(Target target, ParsedLog log, CombatReplay replay)
        {
            List<CastLog> cls = target.GetCastLogs(log, 0, log.FightData.FightDuration);
            int ccRadius = 200;
            switch (target.ID)
            {
                case (ushort)ParseEnum.TargetIDS.Qadim:
                    //CC
                    AddPlatformsToCombatReplay(target, log, replay);
                    List<CastLog> breakbar = cls.Where(x => x.SkillId == 51943).ToList();
                    foreach (CastLog c in breakbar)
                    {
                        int radius = ccRadius;
                        replay.Actors.Add(new CircleActor(true, 0, ccRadius, ((int)c.Time, (int)c.Time + c.ActualDuration), "rgba(0, 180, 255, 0.3)", new AgentConnector(target)));
                    }
                    //Riposte
                    List<CastLog> riposte = cls.Where(x => x.SkillId == 52265).ToList();
                    foreach (CastLog c in riposte)
                    {
                        int radius = 2200;
                        replay.Actors.Add(new CircleActor(true, 0, radius, ((int)c.Time, (int)c.Time + c.ActualDuration), "rgba(255, 0, 0, 0.5)", new AgentConnector(target)));
                    }
                    //Big Hit
                    List<CastLog> maceShockwave = cls.Where(x => x.SkillId == 52310).ToList();
                    foreach (CastLog c in maceShockwave)
                    {
                        int start = (int)c.Time;
                        int delay = 2230;
                        int duration = 2680;
                        int radius = 2000;
                        int impactRadius = 40;
                        int spellCenterDistance = 300;
                        Point3D facing = replay.Rotations.LastOrDefault(x => x.Time <= start + 1000);
                        Point3D targetPosition = replay.Positions.LastOrDefault(x => x.Time <= start + 1000);
                        if (facing != null && targetPosition != null)
                        {
                            Point3D position = new Point3D(targetPosition.X + (facing.X * spellCenterDistance), targetPosition.Y + (facing.Y * spellCenterDistance), targetPosition.Z, targetPosition.Time);
                            replay.Actors.Add(new CircleActor(true, 0, impactRadius, (start, start + delay), "rgba(255, 100, 0, 0.2)", new PositionConnector(position)));
                            replay.Actors.Add(new CircleActor(true, 0, impactRadius, (start + delay - 10, start + delay + 100), "rgba(255, 100, 0, 0.7)", new PositionConnector(position)));
                            replay.Actors.Add(new CircleActor(false, start + delay + duration, radius, (start + delay, start + delay + duration), "rgba(255, 200, 0, 0.7)", new PositionConnector(position)));
                        }
                    }
                    break;
                case (ushort)AncientInvokedHydra:
                    //CC
                    List<CastLog> fieryMeteor = cls.Where(x => x.SkillId == 52941).ToList();
                    foreach (CastLog c in fieryMeteor)
                    {
                        int radius = ccRadius;
                        replay.Actors.Add(new CircleActor(true, 0, ccRadius, ((int)c.Time, (int)c.Time + c.ActualDuration), "rgba(0, 180, 255, 0.3)", new AgentConnector(target)));
                    }
                    List<CastLog> eleBreath = cls.Where(x => x.SkillId == 52520).ToList();
                    foreach (CastLog c in eleBreath)
                    {
                        int start = (int)c.Time;
                        int radius = 1300;
                        int delay = 2600;
                        int duration = 1000;
                        int openingAngle = 70;
                        Point3D facing = replay.Rotations.LastOrDefault(x => x.Time <= start + 1000);
                        if (facing != null)
                        {
                            replay.Actors.Add(new PieActor(true, 0, radius, facing, openingAngle, (start + delay, start + delay + duration), "rgba(255, 180, 0, 0.3)", new AgentConnector(target)));
                        }
                    }
                    break;
                case (ushort)WyvernMatriarch:
                    //Wing Buffet
                    List<CastLog> wingBuffet = cls.Where(x => x.SkillId == 52734).ToList();
                    foreach (CastLog c in wingBuffet)
                    {
                        int start = (int)c.Time;
                        int preCast = Math.Min(3500, c.ActualDuration);
                        int duration = Math.Min(6500, c.ActualDuration);
                        Point3D facing = replay.Rotations.LastOrDefault(x => x.Time <= start + 1000);
                        int range = 2800;
                        int span = 2400;
                        if (facing != null)
                        {
                            int rotation = Point3D.GetRotationFromFacing(facing);
                            replay.Actors.Add(new RotatedRectangleActor(true, 0, range, span, rotation, range / 2, (start, start + preCast), "rgba(0,100,255,0.2)", new AgentConnector(target)));
                            replay.Actors.Add(new RotatedRectangleActor(true, 0, range, span, rotation, range / 2, (start + preCast, start + duration), "rgba(0,100,255,0.5)", new AgentConnector(target)));
                        }
                    }
                    //Breath
                    List<CastLog> matBreath = cls.Where(x => x.SkillId == 52726).ToList();
                    foreach (CastLog c in matBreath)
                    {
                        int start = (int)c.Time;
                        int radius = 1000;
                        int delay = 1600;
                        int duration = 3000;
                        int openingAngle = 70;
                        int fieldDuration = 10000;
                        Point3D facing = replay.Rotations.LastOrDefault(x => x.Time <= start + 1000);
                        Point3D pos = replay.Positions.LastOrDefault(x => x.Time <= start + 1000);
                        if (facing != null && pos != null)
                        {
                            replay.Actors.Add(new PieActor(true, 0, radius, facing, openingAngle, (start + delay, start + delay + duration), "rgba(255, 200, 0, 0.3)", new AgentConnector(target)));
                            replay.Actors.Add(new PieActor(true, 0, radius, facing, openingAngle, (start + delay + duration, start + delay + fieldDuration), "rgba(255, 50, 0, 0.3)", new PositionConnector(pos)));
                        }
                    }
                    //Tail Swipe
                    List<CastLog> matSwipe = cls.Where(x => x.SkillId == 52705).ToList();
                    foreach (CastLog c in matSwipe)
                    {
                        int start = (int)c.Time;
                        int maxRadius = 700;
                        int radiusDecrement = 100;
                        int delay = 1435;
                        int openingAngle = 59;
                        int angleIncrement = 60;
                        int coneAmount = 4;
                        Point3D facing = replay.Rotations.LastOrDefault(x => x.Time <= start + 1000);
                        if (facing != null)
                        {
                            for (int i = 0; i < coneAmount; i++)
                            {
                                int rotation = Point3D.GetRotationFromFacing(facing);
                                replay.Actors.Add(new PieActor(false, 0, maxRadius - (i * radiusDecrement), rotation - (i * angleIncrement), openingAngle, (start, start + delay), "rgba(255, 255, 0, 0.6)", new AgentConnector(target)));
                                replay.Actors.Add(new PieActor(true, 0, maxRadius - (i * radiusDecrement), rotation - (i * angleIncrement), openingAngle, (start, start + delay), "rgba(255, 180, 0, 0.3)", new AgentConnector(target)));

                            }
                        }
                    }
                    break;
                case (ushort)WyvernPatriarch:
                    //CC
                    List<CastLog> patCC = cls.Where(x => x.SkillId == 53132).ToList();
                    foreach (CastLog c in patCC)
                    {
                        int radius = ccRadius;
                        replay.Actors.Add(new CircleActor(true, 0, ccRadius, ((int)c.Time, (int)c.Time + c.ActualDuration), "rgba(0, 180, 255, 0.3)", new AgentConnector(target)));
                    }
                    //Breath
                    List<CastLog> patBreath = cls.Where(x => x.SkillId == 52726).ToList();
                    foreach (CastLog c in patBreath)
                    {
                        int start = (int)c.Time;
                        int radius = 1000;
                        int delay = 1600;
                        int duration = 3000;
                        int openingAngle = 60;
                        int fieldDuration = 10000;
                        Point3D facing = replay.Rotations.LastOrDefault(x => x.Time <= start + 1000);
                        Point3D pos = replay.Positions.LastOrDefault(x => x.Time <= start + 1000);
                        if (facing != null && pos != null)
                        {
                            replay.Actors.Add(new PieActor(true, 0, radius, facing, openingAngle, (start + delay, start + delay + duration), "rgba(255, 200, 0, 0.3)", new AgentConnector(target)));
                            replay.Actors.Add(new PieActor(true, 0, radius, facing, openingAngle, (start + delay + duration, start + delay + fieldDuration), "rgba(255, 50, 0, 0.3)", new PositionConnector(pos)));
                        }
                    }
                    //Tail Swipe
                    List<CastLog> patSwipe = cls.Where(x => x.SkillId == 52705).ToList();
                    foreach (CastLog c in patSwipe)
                    {
                        int start = (int)c.Time;
                        int maxRadius = 700;
                        int radiusDecrement = 100;
                        int delay = 1435;
                        int openingAngle = 59;
                        int angleIncrement = 60;
                        int coneAmount = 4;
                        Point3D facing = replay.Rotations.LastOrDefault(x => x.Time <= start + 1000);
                        if (facing != null)
                        {
                            for (int i = 0; i < coneAmount; i++)
                            {
                                int rotation = Point3D.GetRotationFromFacing(facing);
                                replay.Actors.Add(new PieActor(false, 0, maxRadius - (i * radiusDecrement), rotation - (i * angleIncrement), openingAngle, (start, start + delay), "rgba(255, 255, 0, 0.6)", new AgentConnector(target)));
                                replay.Actors.Add(new PieActor(true, 0, maxRadius - (i * radiusDecrement), rotation - (i * angleIncrement), openingAngle, (start, start + delay), "rgba(255, 180, 0, 0.3)", new AgentConnector(target)));
                            }
                        }
                    }
                    break;
                case (ushort)ApocalypseBringer:
                    List<CastLog> jumpShockwave = cls.Where(x => x.SkillId == 51923).ToList();
                    foreach (CastLog c in jumpShockwave)
                    {
                        int start = (int)c.Time;
                        int delay = 1800;
                        int duration = 3000;
                        int maxRadius = 2000;
                        replay.Actors.Add(new CircleActor(false, start + delay + duration, maxRadius, (start + delay, start + delay + duration), "rgba(255, 200, 0, 0.5)", new AgentConnector(target)));
                    }
                    List<CastLog> stompShockwave = cls.Where(x => x.SkillId == 52330).ToList();
                    foreach (CastLog c in stompShockwave)
                    {
                        int start = (int)c.Time;
                        int delay = 1600;
                        int duration = 3500;
                        int maxRadius = 2000;
                        int impactRadius = 500;
                        int spellCenterDistance = 270; //hitbox radius
                        Point3D facing = replay.Rotations.LastOrDefault(x => x.Time <= start + 1000);
                        Point3D targetPosition = replay.Positions.LastOrDefault(x => x.Time <= start + 1000);
                        if (facing != null && targetPosition != null)
                        {
                            Point3D position = new Point3D(targetPosition.X + facing.X * spellCenterDistance, targetPosition.Y + facing.Y * spellCenterDistance, targetPosition.Z, targetPosition.Time);
                            replay.Actors.Add(new CircleActor(true, 0, impactRadius, (start, start + delay), "rgba(255, 100, 0, 0.1)", new PositionConnector(position)));
                            replay.Actors.Add(new CircleActor(true, 0, impactRadius, (start + delay - 10, start + delay + 100), "rgba(255, 100, 0, 0.5)", new PositionConnector(position)));
                            replay.Actors.Add(new CircleActor(false, start + delay + duration, maxRadius, (start + delay, start + delay + duration), "rgba(255, 200, 0, 0.5)", new PositionConnector(position)));
                        }
                    }
                    //CC
                    List<CastLog> summon = cls.Where(x => x.SkillId == 52054).ToList();
                    foreach (CastLog c in summon)
                    {
                        int radius = ccRadius;
                        replay.Actors.Add(new CircleActor(true, 0, ccRadius, ((int)c.Time, (int)c.Time + c.ActualDuration), "rgba(0, 180, 255, 0.3)", new AgentConnector(target)));
                    }
                    //Pizza
                    List<CastLog> forceWave = cls.Where(x => x.SkillId == 51759).ToList();
                    foreach (CastLog c in forceWave)
                    {
                        int start = (int)c.Time;
                        int maxRadius = 1000;
                        int radiusDecrement = 200;
                        int delay = 1560;
                        int openingAngle = 44;
                        int angleIncrement = 45;
                        int coneAmount = 3;
                        Point3D facing = replay.Rotations.LastOrDefault(x => x.Time <= start + 1000);
                        if (facing != null)
                        {
                            for (int i = 0; i < coneAmount; i++)
                            {
                                int rotation = Point3D.GetRotationFromFacing(facing);
                                replay.Actors.Add(new PieActor(false, 0, maxRadius - (i * radiusDecrement), rotation - (i * angleIncrement), openingAngle, (start, start + delay), "rgba(255, 255, 0, 0.6)", new AgentConnector(target)));
                                replay.Actors.Add(new PieActor(true, 0, maxRadius - (i * radiusDecrement), rotation - (i * angleIncrement), openingAngle, (start, start + delay), "rgba(255, 180, 0, 0.3)", new AgentConnector(target)));
                            }
                        }
                    }
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }

        public override int IsCM(ParsedEvtcContainer evtcContainer)
        {
            Target target = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.Qadim);
            if (target == null)
            {
                throw new InvalidOperationException("Target for CM detection not found");
            }
            OverrideMaxHealths(evtcContainer);
            return (target.Health > 21e6) ? 1 : 0;
        }

        private void AddPlatformsToCombatReplay(Target target, ParsedLog log, CombatReplay replay)
        {
            // We later use the target to find out the timing of the last move
            Debug.Assert(target.ID == (int) ParseEnum.TargetIDS.Qadim);

            // These values were all calculated by hand.
            // It would be way nicer to calculate them here, but we don't have a nice vector library
            // and it would double the amount of work.

            const string platformImageUrl = "https://i.imgur.com/DbXr5Fo.png";
            const double hiddenOpacity = 0.2;

            const int xLeft = -7975;
            const int xLeftLeft = -8537;
            const int xLeftLeftLeft = -9661;
            const int xRight = -6851;
            const int xRightRight = -6289;
            const int xRightRightRight = -5165;
            const int yMid = 12077;
            const int yUp = 13050;
            const int yUpUp = 14023;
            const int yDown = 11104;
            const int yDownDown = 10131;
            const int xGapsLeft = -8018;
            const int xGapsLeftLeft = -8618;
            const int xGapsLeftLeftLeft = -9822;
            const int xGapsRight = -6815;
            const int xGapsRightRight = -6215;
            const int xGapsRightRightRight = -5011;
            const int yGapsUp = 13118;
            const int yGapsUpUp = 14161;
            const int yGapsDown = 11037;
            const int yGapsDownDown = 9993;

            const int xDestroyerLeftLeftLeft = -9732;
            const int xDestroyerLeftLeft = xGapsLeftLeft + 5;
            const int xDestroyerLeft = -8047;
            const int xDestroyerRight = -6778;
            const int xDestroyerRightRight = xGapsRightRight - 5;
            const int xDestroyerRightRightRight = -5101;

            (int x, int y) retaliationPyre1 = (-8951, 9429);
            (int x, int y) protectionPyre1 = (-8947, 14728);
            (int x, int y) stabilityPyre1 = (-4356, yMid);

            (int x, int y) retaliationPyre2 = (-5717, 9325);
            (int x, int y) protectionPyre2 = (-10834, 12477);
            (int x, int y) stabilityPyre2 = (-5889, 14723);

            const double wyvernPhaseMiddleRotation = 0.34;

            const int yJumpingPuzzleOffset1 = 12077 - 11073; // Easternmost two platforms
            const int yJumpingPuzzleOffset2 = 12077 - 10612; // Two platforms on each side, including pyres
            const int yJumpingPuzzleOffset3 = 12077 - 10056; // Northernmost and southernmost rotating platforms
            const int xJumpingPuzzleQadim = -10237; // Qadim's platform
            const int xJumpingPuzzlePreQadim = -8808;
            const int xJumpingPuzzlePyres = -7851;
            const int xJumpingPuzzlePrePyres = -6289;
            const int xJumpingPuzzleRotatingPrePyres = -5736;
            const int xJumpingPuzzleFirstRotating = -5736;
            const int xJumpingPuzzleFirstPlatform = -4146;

            const double jumpingPuzzleRotationRate = 2 * Math.PI / 30; // rad/sec, TODO: Not perfect, it's a bit off

            const int xFinalPlatform = -8297;
            const int qadimFinalX = -7356;
            const int qadimFinalY = 12077;

            const int zDefault = -4731;
            const int zJumpingPuzzlePyres = -4871;
            const int zJumpingPuzzlePrePyres = -4801;
            const int zJumpingPuzzlePreQadim = -4941;
            const int zJumpingPuzzleFirstPlatform = -4591; // The first platform Zommoros visits
            const int zJumpingPuzzleSecondPlatform = -4661; // The second platform Zommoros visits
            const int zFinalPlatforms = -5011;

            const int timeAfterPhase2 = 4000;
            const int timeAfterWyvernPhase = 25000;
            const int jumpingPuzzleShuffleDuration = 11000;
            const int lastPhasePreparationDuration = 13000;

            // If phase data is not calculated, only the first layout is used
            var phases = log.FightData.GetPhases(log);

            int qadimPhase1Time = (int) (phases.Count > 1 ? phases[1].End : int.MaxValue);
            int destroyerPhaseTime = (int) (phases.Count > 2 ? phases[2].End : int.MaxValue);
            int qadimPhase2Time = (int) (phases.Count > 3 ? phases[3].End : int.MaxValue);
            int wyvernPhaseTime = (int) (phases.Count > 4 ? phases[4].End + timeAfterPhase2 : int.MaxValue);
            int jumpingPuzzleTime = (int) (phases.Count > 5 ? phases[5].End + timeAfterWyvernPhase : int.MaxValue);
            int finalPhaseTime = int.MaxValue;
            int startOffset = -(int)(phases.First().Start - log.FightData.ToFightSpace(log.CombatData.AllCombatItems.Min(x => x.Time)));
            if (phases.Count > 6)
            {
                var lastPhase = phases[6];

                List<Point3D> qadimMovement = target.GetCombatReplayPositions(log);

                var lastMove = qadimMovement.FirstOrDefault(
                    pt =>
                    {
                        return Math.Abs(pt.X - qadimFinalX) < 5 && Math.Abs(pt.Y - qadimFinalY) < 5;
                    });

                if (lastMove != null)
                {
                    finalPhaseTime = (int) log.FightData.ToFightSpace(lastMove.Time);
                }
            }

            int jumpingPuzzleDuration = finalPhaseTime - lastPhasePreparationDuration - jumpingPuzzleShuffleDuration - jumpingPuzzleTime;

            const int platformCount = 12;

            // The following monstrosity is needed to avoid the final platform rotating all the way back
            int finalPlatformHalfRotationCount =
                (int) Math.Round((Math.PI + jumpingPuzzleDuration / 1000.0 * jumpingPuzzleRotationRate) / Math.PI,
                    MidpointRounding.AwayFromZero);
            double finalPlatformRotation = Math.PI * finalPlatformHalfRotationCount;


            // Proper skipping of phases (if even possible) is not implemented.
            // Right now transitioning to another state while still moving behaves weirdly.
            // Interpolating to find the position to stop in would be necessary.

            (int start, int duration, (int x, int y, int z, double angle, double opacity)[] platforms)[] movements =
            {
                (
                    // Initial position, all platforms tightly packed

                    startOffset, 0, new[]
                    {
                        (xLeftLeftLeft, yMid, zDefault, 0.0, 1.0),
                        (xLeftLeft, yUpUp, zDefault, Math.PI, 1.0),
                        (xRightRight, yUpUp, zDefault, 0.0, 1.0),
                        (xRightRightRight, yMid, zDefault, Math.PI, 1.0),
                        (xRightRight, yDownDown, zDefault, 0.0, 1.0),
                        (xLeftLeft, yDownDown, zDefault, Math.PI, 1.0),
                        (xLeftLeft, yMid, zDefault, Math.PI, 1.0),
                        (xLeft, yUp, zDefault, 0.0, 1.0),
                        (xRight, yUp, zDefault, Math.PI, 1.0),
                        (xRightRight, yMid, zDefault, 0.0, 1.0),
                        (xRight, yDown, zDefault, Math.PI, 1.0),
                        (xLeft, yDown, zDefault, 0.0, 1.0),
                    }
                ),
                (
                    // Hydra phase, all platforms have a small gap between them
                    startOffset, 12000, new[]
                    {
                        (xGapsLeftLeftLeft, yMid, zDefault, 0.0, 1.0),
                        (xGapsLeftLeft, yGapsUpUp, zDefault, Math.PI, 1.0),
                        (xGapsRightRight, yGapsUpUp, zDefault, 0.0, 1.0),
                        (xGapsRightRightRight, yMid, zDefault, Math.PI, 1.0),
                        (xGapsRightRight, yGapsDownDown, zDefault, 0.0, 1.0),
                        (xGapsLeftLeft, yGapsDownDown, zDefault, Math.PI, 1.0),
                        (xGapsLeftLeft, yMid, zDefault, Math.PI, 1.0),
                        (xGapsLeft, yGapsUp, zDefault, 0.0, 1.0),
                        (xGapsRight, yGapsUp, zDefault, Math.PI, 1.0),
                        (xGapsRightRight, yMid, zDefault, 0.0, 1.0),
                        (xGapsRight, yGapsDown, zDefault, Math.PI, 1.0),
                        (xGapsLeft, yGapsDown, zDefault, 0.0, 1.0),
                    }
                ),
                (
                    // First Qadim phase, packed together except for pyre platforms
                    qadimPhase1Time, 10000, new[]
                    {
                        (xLeftLeftLeft, yMid, zDefault, 0.0, 1.0),
                        (protectionPyre1.x, protectionPyre1.y, zDefault, Math.PI, 1.0),
                        (xRightRight, yUpUp, zDefault, 0.0, 1.0),
                        (stabilityPyre1.x, stabilityPyre1.y, zDefault, Math.PI, 1.0),
                        (xRightRight, yDownDown, zDefault, 0.0, 1.0),
                        (retaliationPyre1.x, retaliationPyre1.y, zDefault, Math.PI, 1.0),
                        (xLeftLeft, yMid, zDefault, Math.PI, 1.0),
                        (xLeft, yUp, zDefault, 0.0, 1.0),
                        (xRight, yUp, zDefault, Math.PI, 1.0),
                        (xRightRight, yMid, zDefault, 0.0, 1.0),
                        (xRight, yDown, zDefault, Math.PI, 1.0),
                        (xLeft, yDown, zDefault, 0.0, 1.0),
                    }
                ),
                (
                    // Destroyer phase, packed together, bigger vertical gap in the middle, 4 platforms hidden
                    destroyerPhaseTime, 15000, new[]
                    {
                        (xDestroyerLeftLeftLeft, yMid, zDefault, 0.0, 1.0),
                        (xGapsLeftLeft, yGapsUpUp, zDefault, Math.PI, hiddenOpacity), // TODO: Unknown position while hidden
                        (xGapsRightRight, yGapsUpUp, zDefault, 0.0, hiddenOpacity), // TODO: Unknown position while hidden
                        (xDestroyerRightRightRight, yMid, zDefault, Math.PI, 1.0),
                        (xGapsRightRight, yGapsDownDown, zDefault, 0.0, hiddenOpacity), // TODO: Unknown position while hidden
                        (xGapsLeftLeft, yGapsDownDown, zDefault, Math.PI, hiddenOpacity), // TODO: Unknown position while hidden
                        (xDestroyerLeftLeft, yMid, zDefault, Math.PI, 1.0),
                        (xDestroyerLeft, yUp, zDefault, 0.0, 1.0),
                        (xDestroyerRight, yUp, zDefault, Math.PI, 1.0),
                        (xDestroyerRightRight, yMid, zDefault, 0.0, 1.0),
                        (xDestroyerRight, yDown, zDefault, Math.PI, 1.0),
                        (xDestroyerLeft, yDown, zDefault, 0.0, 1.0),
                    }
                ),
                (
                    // Second Qadim phase
                    qadimPhase2Time, 10000, new[]
                    {
                        (protectionPyre2.x, protectionPyre2.y, zDefault, 0.0, 1.0),
                        (-8540, 14222, zDefault, Math.PI, 1.0),
                        (stabilityPyre2.x, stabilityPyre2.y, zDefault, 0.0, 1.0),
                        (-5160, yMid, zDefault, Math.PI, 1.0),
                        (retaliationPyre2.x, retaliationPyre2.y, zDefault, 0.0, 1.0),
                        (-8369, 9640, zDefault, Math.PI, 1.0),
                        (protectionPyre2.x + 1939, protectionPyre2.y, zDefault, Math.PI, 1.0),
                        (-7978, 13249, zDefault, 0.0, 1.0),
                        (-6846, 13050, zDefault, Math.PI, 1.0),
                        (-6284, yMid, zDefault, 0.0, 1.0),
                        (retaliationPyre2.x - 1931 / 2, retaliationPyre2.y + 1672, zDefault, Math.PI, 1.0),
                        (-7807, 10613, zDefault, 0.0, 1.0),
                    }
                ),
                (
                    // TODO: Heights are not correct, they differ here, currently not important for the replay
                    // Wyvern phase
                    wyvernPhaseTime, 11000, new[]
                    {
                        (protectionPyre2.x, protectionPyre2.y, zDefault, 0.0, hiddenOpacity), // TODO: Unknown position while hidden
                        (-9704, 15323, zDefault, Math.PI, 1.0),
                        (-7425, 15312, zDefault, 0.0, 1.0),
                        (-5160, yMid, zDefault, Math.PI, hiddenOpacity), // TODO: Unknown position while hidden
                        (-5169, 8846, zDefault, 0.0, 1.0),
                        (-7414, 8846, zDefault, Math.PI, hiddenOpacity),
                        (-7728, 11535, zDefault, Math.PI + wyvernPhaseMiddleRotation, 1.0),
                        (-9108, 14335, zDefault, 0.0, 1.0),
                        (-7987, 14336, zDefault, Math.PI, 1.0),
                        (-7106, 12619, zDefault, wyvernPhaseMiddleRotation, 1.0),
                        (-5729, 9821, zDefault, Math.PI, 1.0),
                        (-6854, 9821, zDefault, 0.0, 1.0),
                    }
                ),
                (
                    // Jumping puzzle preparation, platforms hide
                    jumpingPuzzleTime - 500, 0, new[]
                    {
                        (protectionPyre2.x, protectionPyre2.y, zDefault, 0.0, hiddenOpacity),
                        (-9704, 15323, zDefault, Math.PI, hiddenOpacity),
                        (-7425, 15312, zDefault, 0.0, hiddenOpacity),
                        (-5160, yMid, zDefault, Math.PI, hiddenOpacity),
                        (-5169, 8846, zDefault, 0.0, hiddenOpacity),
                        (-7414, 8846, zDefault, Math.PI, hiddenOpacity),
                        (-7728, 11535, zDefault, Math.PI + wyvernPhaseMiddleRotation, hiddenOpacity),
                        (-9108, 14335, zDefault, 0.0, hiddenOpacity),
                        (-7987, 14336, zDefault, Math.PI, hiddenOpacity),
                        (-7106, 12619, zDefault, wyvernPhaseMiddleRotation, hiddenOpacity),
                        (-5729, 9821, zDefault, Math.PI, 1.0),
                        (-6854, 9821, zDefault, 0.0, hiddenOpacity),
                    }
                ),
                (
                    // Jumping puzzle, platforms move
                    jumpingPuzzleTime, jumpingPuzzleShuffleDuration - 1, new[]
                    {
                        (xJumpingPuzzleQadim, yMid, zFinalPlatforms, 0.0, hiddenOpacity),
                        (xJumpingPuzzleFirstRotating, yMid, zDefault, Math.PI, hiddenOpacity),
                        (xJumpingPuzzleRotatingPrePyres, yMid + yJumpingPuzzleOffset3, zJumpingPuzzlePyres, 0.0, hiddenOpacity),
                        (xJumpingPuzzlePyres, yMid - yJumpingPuzzleOffset2, zJumpingPuzzlePyres, Math.PI, hiddenOpacity),
                        (xJumpingPuzzleRotatingPrePyres, yMid - yJumpingPuzzleOffset3, zJumpingPuzzlePyres, 0.0, hiddenOpacity),
                        (xJumpingPuzzlePyres, yMid + yJumpingPuzzleOffset2, zJumpingPuzzlePyres, Math.PI, hiddenOpacity),
                        (xJumpingPuzzlePreQadim, yMid, zJumpingPuzzlePreQadim, Math.PI, hiddenOpacity),
                        (xJumpingPuzzlePrePyres, yMid + yJumpingPuzzleOffset2, zJumpingPuzzlePrePyres, Math.PI, hiddenOpacity),
                        (xJumpingPuzzleFirstPlatform, yMid + yJumpingPuzzleOffset1, zJumpingPuzzleSecondPlatform, Math.PI, hiddenOpacity),
                        (xJumpingPuzzlePyres, yMid, zJumpingPuzzlePyres, Math.PI, hiddenOpacity),
                        (xJumpingPuzzleFirstPlatform, yMid - yJumpingPuzzleOffset1, zJumpingPuzzleFirstPlatform, Math.PI, 1.0),
                        (xJumpingPuzzlePrePyres, yMid - yJumpingPuzzleOffset2, zJumpingPuzzlePrePyres, Math.PI, hiddenOpacity),
                    }
                ),
                (
                    // Jumping puzzle, platforms appear
                    jumpingPuzzleTime + jumpingPuzzleShuffleDuration - 1, 1, new[]
                    {
                        (xJumpingPuzzleQadim, yMid, zFinalPlatforms, 0.0, 1.0),
                        (xJumpingPuzzleFirstRotating, yMid, zDefault, Math.PI, 1.0),
                        (xJumpingPuzzleRotatingPrePyres, yMid + yJumpingPuzzleOffset3, zJumpingPuzzlePyres, 0.0, 1.0),
                        (xJumpingPuzzlePyres, yMid - yJumpingPuzzleOffset2, zJumpingPuzzlePyres, Math.PI, 1.0),
                        (xJumpingPuzzleRotatingPrePyres, yMid - yJumpingPuzzleOffset3, zJumpingPuzzlePyres, 0.0, 1.0),
                        (xJumpingPuzzlePyres, yMid + yJumpingPuzzleOffset2, zJumpingPuzzlePyres, Math.PI, 1.0),
                        (xJumpingPuzzlePreQadim, yMid, zJumpingPuzzlePreQadim, Math.PI, 1.0),
                        (xJumpingPuzzlePrePyres, yMid + yJumpingPuzzleOffset2, zJumpingPuzzlePrePyres, Math.PI, 1.0),
                        (xJumpingPuzzleFirstPlatform, yMid + yJumpingPuzzleOffset1, zJumpingPuzzleSecondPlatform, Math.PI, 1.0),
                        (xJumpingPuzzlePyres, yMid, zJumpingPuzzlePyres, Math.PI, hiddenOpacity),
                        (xJumpingPuzzleFirstPlatform, yMid - yJumpingPuzzleOffset1, zJumpingPuzzleFirstPlatform, Math.PI, 1.0),
                        (xJumpingPuzzlePrePyres, yMid - yJumpingPuzzleOffset2, zJumpingPuzzlePrePyres, Math.PI, 1.0),
                    }
                ),
                (
                    // Jumping puzzle appears, platforms rotate...
                    // Jumping puzzle platform breaks are not shown for now because their timing is rather tricky.
                    jumpingPuzzleTime + jumpingPuzzleShuffleDuration, jumpingPuzzleDuration, new[]
                    {
                        (xJumpingPuzzleQadim, yMid, zFinalPlatforms, 0.0, 1.0),
                        (xJumpingPuzzleFirstRotating, yMid, zDefault, Math.PI + jumpingPuzzleDuration / 1000.0 * jumpingPuzzleRotationRate, 1.0),
                        (xJumpingPuzzleRotatingPrePyres, yMid + yJumpingPuzzleOffset3, zJumpingPuzzlePyres, -jumpingPuzzleDuration / 1000.0 * jumpingPuzzleRotationRate, 1.0),
                        (xJumpingPuzzlePyres, yMid - yJumpingPuzzleOffset2, zJumpingPuzzlePyres, Math.PI, 1.0),
                        (xJumpingPuzzleRotatingPrePyres, yMid - yJumpingPuzzleOffset3, zJumpingPuzzlePyres, jumpingPuzzleDuration / 1000.0 * jumpingPuzzleRotationRate, 1.0),
                        (xJumpingPuzzlePyres, yMid + yJumpingPuzzleOffset2, zJumpingPuzzlePyres, Math.PI, 1.0),
                        (xJumpingPuzzlePreQadim, yMid, zJumpingPuzzlePreQadim, Math.PI + jumpingPuzzleDuration / 1000.0 * jumpingPuzzleRotationRate, 1.0),
                        (xJumpingPuzzlePrePyres, yMid + yJumpingPuzzleOffset2, zJumpingPuzzlePrePyres, Math.PI, 1.0),
                        (xJumpingPuzzleFirstPlatform, yMid + yJumpingPuzzleOffset1, zJumpingPuzzleSecondPlatform, Math.PI, 1.0),
                        (xJumpingPuzzlePyres, yMid, zJumpingPuzzlePyres, Math.PI, hiddenOpacity),
                        (xJumpingPuzzleFirstPlatform, yMid - yJumpingPuzzleOffset1, zJumpingPuzzleFirstPlatform, Math.PI, 1.0),
                        (xJumpingPuzzlePrePyres, yMid - yJumpingPuzzleOffset2, zJumpingPuzzlePrePyres, Math.PI, 1.0),
                    }
                ),
                (
                    // Final phase preparation.
                    finalPhaseTime - lastPhasePreparationDuration, lastPhasePreparationDuration, new[]
                    {
                        (xJumpingPuzzleQadim, yMid, zFinalPlatforms, 0.0, 1.0),
                        (xJumpingPuzzleFirstRotating, yMid, zDefault, Math.PI + jumpingPuzzleDuration / 1000.0 * jumpingPuzzleRotationRate, hiddenOpacity),
                        (xJumpingPuzzleRotatingPrePyres, yMid + yJumpingPuzzleOffset3, zJumpingPuzzlePyres, -jumpingPuzzleDuration / 1000.0 * jumpingPuzzleRotationRate, hiddenOpacity),
                        (xJumpingPuzzlePyres, yMid - yJumpingPuzzleOffset2, zJumpingPuzzlePyres, Math.PI, hiddenOpacity),
                        (xJumpingPuzzleRotatingPrePyres, yMid - yJumpingPuzzleOffset3, zJumpingPuzzlePyres, jumpingPuzzleDuration / 1000.0 * jumpingPuzzleRotationRate, hiddenOpacity),
                        (xJumpingPuzzlePyres, yMid + yJumpingPuzzleOffset2, zJumpingPuzzlePyres, Math.PI, hiddenOpacity),
                        (xFinalPlatform, yMid, zJumpingPuzzlePreQadim, finalPlatformRotation, hiddenOpacity),
                        (xJumpingPuzzlePrePyres, yMid + yJumpingPuzzleOffset2, zJumpingPuzzlePrePyres, Math.PI, hiddenOpacity),
                        (xJumpingPuzzleFirstPlatform, yMid + yJumpingPuzzleOffset1, zJumpingPuzzleSecondPlatform, Math.PI, hiddenOpacity),
                        (xJumpingPuzzlePyres, yMid, zJumpingPuzzlePyres, Math.PI, hiddenOpacity),
                        (xJumpingPuzzleFirstPlatform, yMid - yJumpingPuzzleOffset1, zJumpingPuzzleFirstPlatform, Math.PI, hiddenOpacity),
                        (xJumpingPuzzlePrePyres, yMid - yJumpingPuzzleOffset2, zJumpingPuzzlePrePyres, Math.PI, hiddenOpacity),
                    }
                ),
                (
                    // Final phase.
                    finalPhaseTime, 0, new[]
                    {
                        (xJumpingPuzzleQadim, yMid, zFinalPlatforms, 0.0, 1.0),
                        (xJumpingPuzzleFirstRotating, yMid, zDefault, Math.PI + jumpingPuzzleDuration / 1000.0 * jumpingPuzzleRotationRate, hiddenOpacity),
                        (xJumpingPuzzleRotatingPrePyres, yMid + yJumpingPuzzleOffset3, zJumpingPuzzlePyres, -jumpingPuzzleDuration / 1000.0 * jumpingPuzzleRotationRate, hiddenOpacity),
                        (xJumpingPuzzlePyres, yMid - yJumpingPuzzleOffset2, zJumpingPuzzlePyres, Math.PI, hiddenOpacity),
                        (xJumpingPuzzleRotatingPrePyres, yMid - yJumpingPuzzleOffset3, zJumpingPuzzlePyres, jumpingPuzzleDuration / 1000.0 * jumpingPuzzleRotationRate, hiddenOpacity),
                        (xJumpingPuzzlePyres, yMid + yJumpingPuzzleOffset2, zJumpingPuzzlePyres, Math.PI, hiddenOpacity),
                        (xFinalPlatform, yMid, zJumpingPuzzlePreQadim, finalPlatformRotation, 1.0),
                        (xJumpingPuzzlePrePyres, yMid + yJumpingPuzzleOffset2, zJumpingPuzzlePrePyres, Math.PI, hiddenOpacity),
                        (xJumpingPuzzleFirstPlatform, yMid + yJumpingPuzzleOffset1, zJumpingPuzzleSecondPlatform, Math.PI, hiddenOpacity),
                        (xJumpingPuzzlePyres, yMid, zJumpingPuzzlePyres, Math.PI, hiddenOpacity),
                        (xJumpingPuzzleFirstPlatform, yMid - yJumpingPuzzleOffset1, zJumpingPuzzleFirstPlatform, Math.PI, hiddenOpacity),
                        (xJumpingPuzzlePrePyres, yMid - yJumpingPuzzleOffset2, zJumpingPuzzlePrePyres, Math.PI, hiddenOpacity),
                    }
                ),
                (
                    // Second to last platform is destroyed
                    finalPhaseTime, 7000, new[]
                    {
                        (xJumpingPuzzleQadim, yMid, zFinalPlatforms, 0.0, hiddenOpacity),
                        (xJumpingPuzzleFirstRotating, yMid, zDefault, Math.PI + jumpingPuzzleDuration / 1000.0 * jumpingPuzzleRotationRate, hiddenOpacity),
                        (xJumpingPuzzleRotatingPrePyres, yMid + yJumpingPuzzleOffset3, zJumpingPuzzlePyres, -jumpingPuzzleDuration / 1000.0 * jumpingPuzzleRotationRate, hiddenOpacity),
                        (xJumpingPuzzlePyres, yMid - yJumpingPuzzleOffset2, zJumpingPuzzlePyres, Math.PI, hiddenOpacity),
                        (xJumpingPuzzleRotatingPrePyres, yMid - yJumpingPuzzleOffset3, zJumpingPuzzlePyres, jumpingPuzzleDuration / 1000.0 * jumpingPuzzleRotationRate, hiddenOpacity),
                        (xJumpingPuzzlePyres, yMid + yJumpingPuzzleOffset2, zJumpingPuzzlePyres, Math.PI, hiddenOpacity),
                        (xFinalPlatform, yMid, zJumpingPuzzlePreQadim, finalPlatformRotation, 1.0),
                        (xJumpingPuzzlePrePyres, yMid + yJumpingPuzzleOffset2, zJumpingPuzzlePrePyres, Math.PI, hiddenOpacity),
                        (xJumpingPuzzleFirstPlatform, yMid + yJumpingPuzzleOffset1, zJumpingPuzzleSecondPlatform, Math.PI, hiddenOpacity),
                        (xJumpingPuzzlePyres, yMid, zJumpingPuzzlePyres, Math.PI, hiddenOpacity),
                        (xJumpingPuzzleFirstPlatform, yMid - yJumpingPuzzleOffset1, zJumpingPuzzleFirstPlatform, Math.PI, hiddenOpacity),
                        (xJumpingPuzzlePrePyres, yMid - yJumpingPuzzleOffset2, zJumpingPuzzlePrePyres, Math.PI, hiddenOpacity),
                    }
                ),
            };

            // All platforms have to have positions in all phases
            Debug.Assert(movements.All(x => x.platforms.Length == platformCount));

            var platforms = new MovingPlatformActor[platformCount];
            for (int i = 0; i < platformCount; i++)
            {
                platforms[i] = new MovingPlatformActor(platformImageUrl, 245, 245, (int.MinValue, int.MaxValue));
                replay.Actors.Add(platforms[i]);
            }

            // Add movement "keyframes" on a movement end and on the start of the next one.
            // This approach requires one extra movement at the start for initial positions (should be of duration 0)
            for (var i = 0; i < movements.Length; i++)
            {
                var movement = movements[i];
                var positions = movement.platforms;

                for (var platformIndex = 0; platformIndex < platformCount; platformIndex++)
                {
                    var platform = platforms[platformIndex];
                    (int x, int y, int z, double angle, double opacity) = positions[platformIndex];

                    // Add a keyframe for movement end.
                    platform.AddPosition(x, y, z, angle, opacity, movement.start + movement.duration);

                    if (i != movements.Length - 1)
                    {
                        // Add a keyframe for next movement start to ensure that there is no change
                        // between the end of this movement and the start of the next one
                        platform.AddPosition(x, y, z, angle, opacity, movements[i + 1].start);
                    }
                }
            }
        }
    }
}
