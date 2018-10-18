using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static LuckParser.Models.DataModels.ParseEnum.TrashIDS;

namespace LuckParser.Models
{
    public class Qadim : RaidLogic
    {
        public Qadim(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>    
            {
            new Mechanic(51943, "Qadim CC", Mechanic.MechType.EnemyCastStart, ParseEnum.BossIDS.Qadim, "symbol:'diamond-tall',color:'rgb(0,160,150)'", "Q.BB","Qadim CC", "Qadim CC",0),
            new Mechanic(51943, "Qadim CC", Mechanic.MechType.EnemyCastEnd, ParseEnum.BossIDS.Qadim, "symbol:'diamond-tall',color:'rgb(0,160,0)'", "Q.CCed","Quadim Breakbar broken", "Quadim CCed",0, (condition => condition.CombatItem.Value < 6500)),
            new Mechanic(52265, "Riposte", Mechanic.MechType.EnemyCastStart, ParseEnum.BossIDS.Qadim, "symbol:'diamond-tall',color:'rgb(255,0,0)'", "Q.CC.Fail","Qadim Breakbar failed", "Quadim CC Fail",0),
            new Mechanic(52265, "Riposte", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Qadim, "symbol:'circle',color:'rgb(255,0,255)'", "NoCCAtk", "Riposte (Attack if CC on Qadim failed)", "Riposte (No CC)", 0),
            new Mechanic(52614, "Fiery Dance", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Qadim, "symbol:'asterisk-open',color:'rgb(255,100,0)'", "FrDnc", "Fiery Dance (Fire running along metal edges)", "Fire on Lines", 0),
            new Mechanic(52864, "Fiery Dance", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Qadim, "symbol:'asterisk-open',color:'rgb(255,100,0)'", "FrDnc", "Fiery Dance (Fire running along metal edges)", "Fire on Lines", 0),
            new Mechanic(53153, "Fiery Dance", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Qadim, "symbol:'asterisk-open',color:'rgb(255,100,0)'", "FrDnc", "Fiery Dance (Fire running along metal edges)", "Fire on Lines", 0),
            new Mechanic(52383, "Fiery Dance", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Qadim, "symbol:'asterisk-open',color:'rgb(255,100,0)'", "FrDnc", "Fiery Dance (Fire running along metal edges)", "Fire on Lines", 0),
            new Mechanic(52242, "Shattering Impact", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Qadim, "symbol:'circle',color:'rgb(255,200,0)'", "Stun","Shattering Impact (Stunning flame bolt)", "Flame Bolt Stun",0),
            new Mechanic(52814, "Flame Wave", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Qadim, "symbol:'star-triangle-up-open',color:'rgb(255,150,0)'", "KB","Flame Wave (Knockback Frontal Beam (burning))", "KB Burning Push",0),
            new Mechanic(52820, "Fire Wave", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Qadim, "symbol:'star-triangle-down-open',color:'rgb(255,150,0)'", "KB2","Fire Wave (Knockback Frontal Beam (vulnerability))", "KB Vuln Push",0),
            new Mechanic(52520, "Elemental Breath", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Qadim, "symbol:'triangle-left',color:'rgb(255,0,0)'", "H.Brth","Elemental Breath (Hydra Breath)", "Hydra Breath",0),
            new Mechanic(53013, "Fireball", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Qadim, "symbol:'circle-open',color:'rgb(255,150,0)',size:10", "H.Fb","Fireball (Hydra)", "Hydra Fireball",0),
            new Mechanic(52941, "Fiery Meteor", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Qadim, "symbol:'circle-open',color:'rgb(255,150,0)'", "H.Mtr","Fiery Meteor (Hydra)", "Hydra Meteor",0),
            new Mechanic(52941, "Fiery Meteor", Mechanic.MechType.EnemyCastStart, ParseEnum.BossIDS.Qadim, "symbol:'diamond-tall',color:'rgb(0,160,150)'", "H.BB","Fiery Meteor (Hydra Breakbar)", "Hydra CC",0),
            //new Mechanic(718, "Fiery Meteor (Spawn)", Mechanic.MechType.EnemyBoon, ParseEnum.BossIDS.Qadim, "symbol:'diamond-tall',color:'rgb(150,0,0)'", "H.CC.Fail","Fiery Meteor Spawned (Hydra Breakbar)", "Hydra CC Fail",0,(condition =>  condition.CombatItem.IFF == ParseEnum.IFF.Foe)),
            new Mechanic(52941, "Fiery Meteor", Mechanic.MechType.EnemyCastEnd, ParseEnum.BossIDS.Qadim, "symbol:'diamond-tall',color:'rgb(0,160,0)'", "H.CCed","Fiery Meteor (Hydra Breakbar broken)", "Hydra CCed",0,(condition => condition.CombatItem.Value < 12364)),
            new Mechanic(52941, "Fiery Meteor", Mechanic.MechType.EnemyCastEnd, ParseEnum.BossIDS.Qadim, "symbol:'diamond-tall',color:'rgb(0,160,0)'", "H.CC.Fail","Fiery Meteor (Hydra Breakbar not broken)", "Hydra CC Failed",0,(condition => condition.CombatItem.Value >= 12364)),
            new Mechanic(53051, "Teleport", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Qadim, "symbol:'circle',color:'rgb(150,0,200)'", "H.KB","Teleport Knockback (Hydra)", "Hydra TP KB",0),
            new Mechanic(52310, "Big Hit", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Qadim, "symbol:'circle',color:'rgb(255,0,0)'", "Mace","Big Hit (Mace Shockwave)", "Mace Shockwave",0),
            new Mechanic(52955, "Lava Pool Drop", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Qadim, "symbol:'square-open',color:'rgb(255,0,0)'", "LvPl","Lava Pool drop (on long platform spokes)", "Lava Pool Drop",0),
            new Mechanic(51958, "Slash (Wyvern)", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Qadim, "symbol:'triangle-down-open',color:'rgb(255,200,0)'", "Slsh","Wyvern Slash (Double attack: knock into pin down)", "KB/Pin down",0),
            new Mechanic(52705, "Tail Swipe", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Qadim, "symbol:'diamond-open',color:'rgb(255,200,0)'", "W.Pza","Wyvern Tail Swipe (Pizza attack)", "Tail Swipe",0),
            new Mechanic(52726, "Fire Breath", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Qadim, "symbol:'triangle-right-open',color:'rgb(255,100,0)'", "W.Brth","Fire Breath (Wyvern)", "Fire Breath",0),
            new Mechanic(52734, "Wing Buffet", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Qadim, "symbol:'star-diamond-open',color:'rgb(0,125,125)'", "W.WBft","Wing Buffet (Wyvern Launching Wing Storm)", "Wing Buffet",0),
            new Mechanic(52734, "Wing Buffet", Mechanic.MechType.EnemyCastStart, ParseEnum.BossIDS.Qadim, "symbol:'diamond-tall',color:'rgb(0,160,150)'", "M.BB","Wing Buffet (Matriarch CC)", "Matriarch CC",0),
            new Mechanic(52734, "Wing Buffet", Mechanic.MechType.EnemyCastEnd, ParseEnum.BossIDS.Qadim, "symbol:'diamond-tall',color:'rgb(0,160,0)'", "M.CCed","Wing Buffet (Matriarch Breakbar broken)", "Matriarch CCed",0, (condition => condition.CombatItem.Value < 6500)),
            new Mechanic(52734, "Wing Buffet", Mechanic.MechType.EnemyCastEnd, ParseEnum.BossIDS.Qadim, "symbol:'diamond-tall',color:'rgb(255,0,0)'", "M.CC.Fail","Wing Buffet (Matriarch Breakbar failed)", "Matriarch CC Fail",0, (condition => condition.CombatItem.Value >= 6500)),
            new Mechanic(53132, "Patriarch CC", Mechanic.MechType.EnemyCastStart, ParseEnum.BossIDS.Qadim, "symbol:'diamond-wide',color:'rgb(0,160,150)'", "P.BB","Wing Buffet (Patriarch CC)", "Patriarch CC",0),
            new Mechanic(53132, "Patriarch CC", Mechanic.MechType.EnemyCastEnd, ParseEnum.BossIDS.Qadim, "symbol:'diamond-wide',color:'rgb(0,160,0)'", "P.CCed","Wing Buffet (Patriarch Breakbar broken)", "Patriarch CCed",0, (condition => condition.CombatItem.Value < 6500)),
            new Mechanic(51984, "Patriarch CC (Jump into air)", Mechanic.MechType.EnemyCastStart, ParseEnum.BossIDS.Qadim, "symbol:'diamond-wide',color:'rgb(255,0,0)'", "P.CC.Fail","Wing Buffet (Patriarch Breakbar failed)", "Patriarch CC Fail",0),
            new Mechanic(52330, "Seismic Stomp", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Qadim, "symbol:'star-open',color:'rgb(255,255,0)'", "D.Stmp","Seismic Stomp (Destroyer Stomp)", "Seismic Stomp (Destroyer)",0),
            new Mechanic(51923, "Shattered Earth", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Qadim, "symbol:'hexagram-open',color:'rgb(255,0,0)'", "D.Slm","Shattered Earth (Destroyer Jump Slam)", "Jump Slam (Destroyer)",0),
            new Mechanic(51759, "Wave of Force", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Qadim, "symbol:'diamond-open',color:'rgb(255,200,0)'", "D.Pza","Wave of Force (Destroyer Pizza)", "Destroyer Auto",0),
            new Mechanic(52054, "Summon", Mechanic.MechType.EnemyCastStart, ParseEnum.BossIDS.Qadim, "symbol:'diamond-tall',color:'rgb(0,160,150)'", "D.BB","Summon (Destroyer Breakbar)", "Destroyer CC",0),
            new Mechanic(52054, "Summon", Mechanic.MechType.EnemyCastEnd, ParseEnum.BossIDS.Qadim, "symbol:'diamond-tall',color:'rgb(0,160,0)'", "D.CCed","Summon (Destroyer Breakbar broken)", "Destroyer CCed",0, (condition => condition.CombatItem.Value < 8332)),
            new Mechanic(52054, "Summon", Mechanic.MechType.EnemyCastEnd, ParseEnum.BossIDS.Qadim, "symbol:'diamond-tall',color:'rgb(255,0,0)'", "D.CC.Fail","Summon (Destroyer Breakbar failed)", "Destroyer CC Fail",0, (condition => condition.CombatItem.Value >= 8332)),
            new Mechanic(20944, "Summon (Spawn)", Mechanic.MechType.Spawn, ParseEnum.BossIDS.Qadim, "symbol:'diamond-tall',color:'rgb(150,0,0)'", "D.Spwn","Summon (Destroyer Trolls summoned)", "Destroyer Summoned",0),
            new Mechanic(52461, "Sea of Flame", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Qadim, "symbol:'circle-open',color:'rgb(255,0,0)'", "Q.Hbx","Sea of Flame (Stood in Qadim Hitbox)", "Qadim Hitbox AoE",0),
            new Mechanic(52035, "Power of the Lamp", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Qadim, "symbol:'triangle-up',color:'rgb(100,150,255)',size:10", "Lmp","Power of the Lamp (Returned from the Lamp)", "Lamp Return",0),
            }); 
            Extension = "qadim";
            IconUrl = "https://wiki.guildwars2.com/images/f/f2/Mini_Qadim.png";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            //return new CombatReplayMap("https://i.imgur.com/nGaCj1L.png",
            //                Tuple.Create(3437, 2978),
            //                Tuple.Create(-10966, 8825, -3870, 15289),
            return new CombatReplayMap("https://i.imgur.com/qvF3ClM.png",
                            Tuple.Create(3785, 3570),
                            Tuple.Create(-11676, 8825, -3870, 16582),
                            Tuple.Create(-21504, -21504, 24576, 24576),
                            Tuple.Create(13440, 14336, 15360, 16256));
        }
        

        protected override List<ushort> GetFightTargetsIDs()
        {
            return new List<ushort>
            {
                (ushort)ParseEnum.BossIDS.Qadim,
                (ushort)AncientInvokedHydra,
                (ushort)WyvernMatriarch,
                (ushort)WyvernPatriarch,
                (ushort)ApocalypseBringer,
            };
        }

        public override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            long start = 0;
            long end = 0;
            List<PhaseData> phases = GetInitialPhase(log);
            Boss qadim = Targets.Find(x => x.ID == (ushort)ParseEnum.BossIDS.Qadim);
            if (qadim == null)
            {
                throw new InvalidOperationException("Qadim not found");
            }
            phases[0].Targets.Add(qadim);
            if (!requirePhases)
            {
                return phases;
            }
            List<long> moltenArmor = GetFilteredList(log,52329,qadim).Select(x => x.Time - log.FightData.FightStart).Distinct().ToList();
            for (int i = 1; i < moltenArmor.Count; i++)
            {
                if (i % 2 == 0)
                {
                    end = Math.Min(moltenArmor[i], log.FightData.FightDuration);
                    phases.Add(new PhaseData(start, end));
                    if (i == moltenArmor.Count - 1)
                    {
                        phases.Add(new PhaseData(end, log.FightData.FightDuration));
                    }
                } else
                {
                    start = Math.Min(moltenArmor[i], log.FightData.FightDuration);
                    phases.Add(new PhaseData(end, start));
                    if (i == moltenArmor.Count - 1)
                    {
                        phases.Add(new PhaseData(start, log.FightData.FightDuration));
                    }
                }
            }
            string[] names = { "Hydra","Qadim P1","Apocalypse", "Qadim P2","Wyvern", "Qadim P3" };
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                phase.Name = names[i - 1];
                switch (i)
                {
                    case 2:
                    case 4:
                    case 6:
                        List<long> pyresFirstAware = log.AgentData.GetAgentsByID((ushort)PyreGuardian).Where(x => phase.InInterval(x.FirstAware - log.FightData.FightStart)).Select(x => x.FirstAware - log.FightData.FightStart).ToList();
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
                        phase.DrawArea = true;
                        phase.DrawEnd = true;
                        phase.DrawStart = true;
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
                IceElemental,
                Zommoros
            };
        }

        public override void ComputeAdditionalThrashMobData(Mob mob, ParsedLog log)
        {
            switch (mob.ID)
            {
                case (ushort)LavaElemental1:
                case (ushort)LavaElemental2:
                case (ushort)IcebornHydra:
                case (ushort)GreaterMagmaElemental1:
                case (ushort)GreaterMagmaElemental2:
                case (ushort)FireElemental:
                case (ushort)FireImp:
                case (ushort)PyreGuardian:
                case (ushort)ReaperofFlesh:
                case (ushort)IceElemental:
                case (ushort)Zommoros:
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }

        public override void ComputeAdditionalBossData(Boss boss, ParsedLog log)
        {
            CombatReplay replay = boss.CombatReplay;
            List<CastLog> cls = boss.GetCastLogs(log, 0, log.FightData.FightDuration);
            int ccRadius = 200;
            switch (boss.ID)
            {
                case (ushort)ParseEnum.BossIDS.Qadim:
                    //CC
                    List<CastLog> breakbar = cls.Where(x => x.SkillId == 51943).ToList();
                    foreach (CastLog c in breakbar)
                    {
                        int radius = ccRadius;
                        replay.Actors.Add(new CircleActor(true, 0, ccRadius, new Tuple<int, int>((int)c.Time, (int)c.Time + c.ActualDuration), "rgba(0, 180, 255, 0.3)", new AgentConnector(boss)));
                    }
                    //Riposte
                    List<CastLog> riposte = cls.Where(x => x.SkillId == 52265).ToList();
                    foreach (CastLog c in riposte)
                    {
                        int radius = 2200;
                        replay.Actors.Add(new CircleActor(true, 0, radius, new Tuple<int, int>((int)c.Time, (int)c.Time + c.ActualDuration), "rgba(255, 0, 0, 0.5)", new AgentConnector(boss)));
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
                        Point3D bossPosition = replay.Positions.LastOrDefault(x => x.Time <= start + 1000);
                        if (facing != null && bossPosition != null)
                        {
                            Point3D position = new Point3D(bossPosition.X + (facing.X * spellCenterDistance), bossPosition.Y + (facing.Y * spellCenterDistance), bossPosition.Z, bossPosition.Time);
                            replay.Actors.Add(new CircleActor(true, 0, impactRadius, new Tuple<int, int>(start, start + delay), "rgba(255, 100, 0, 0.2)", new PositionConnector(position)));
                            replay.Actors.Add(new CircleActor(true, 0, impactRadius, new Tuple<int, int>(start + delay - 10, start + delay + 100), "rgba(255, 100, 0, 0.7)", new PositionConnector(position)));
                            replay.Actors.Add(new CircleActor(false, start + delay + duration, radius, new Tuple<int, int>(start + delay, start + delay + duration), "rgba(255, 200, 0, 0.7)", new PositionConnector(position)));
                        }
                    }
                    break;
                case (ushort)AncientInvokedHydra:
                    //CC
                    List<CastLog> fieryMeteor = cls.Where(x => x.SkillId == 52941).ToList();
                    foreach (CastLog c in fieryMeteor)
                    {
                        int radius = ccRadius;
                        replay.Actors.Add(new CircleActor(true, 0, ccRadius, new Tuple<int, int>((int)c.Time, (int)c.Time + c.ActualDuration), "rgba(0, 180, 255, 0.3)", new AgentConnector(boss)));
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
                            replay.Actors.Add(new PieActor(true, 0, radius, facing, openingAngle, new Tuple<int, int>(start + delay, start + delay + duration), "rgba(255, 180, 0, 0.3)", new AgentConnector(boss)));
                        }
                    }
                    break;
                case (ushort)WyvernMatriarch:
                    //CC
                    List<CastLog> matCC = cls.Where(x => x.SkillId == 52734).ToList();
                    foreach (CastLog c in matCC)
                    {
                        int start = (int)c.Time;
                        int duration = Math.Min(6500,c.ActualDuration);
                        Tuple<int, int> lifespan = new Tuple<int, int>(start, start + duration);
                        int radius = ccRadius;
                        replay.Actors.Add(new CircleActor(true, 0, ccRadius, lifespan, "rgba(0, 180, 255, 0.3)", new AgentConnector(boss)));
                        Point3D facing = replay.Rotations.LastOrDefault(x => x.Time <= start + 1000);
                        int range = 2800;
                        int span = 2400;
                        if (facing != null)
                        {
                            int rotation = Point3D.GetRotationFromFacing(facing);
                            replay.Actors.Add(new RotatedRectangleActor(true, 0, range, span, rotation, range/2, lifespan, "rgba(0,100,255,0.4)", new AgentConnector(boss)));
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
                            replay.Actors.Add(new PieActor(true, 0, radius, facing, openingAngle, new Tuple<int, int>(start + delay, start + delay + duration), "rgba(255, 200, 0, 0.3)", new AgentConnector(boss)));
                            replay.Actors.Add(new PieActor(true, 0, radius, facing, openingAngle, new Tuple<int, int>(start + delay + duration, start + delay + fieldDuration), "rgba(255, 50, 0, 0.3)", new PositionConnector(pos)));
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
                                replay.Actors.Add(new PieActor(false, 0, maxRadius - (i * radiusDecrement), rotation - (i * angleIncrement), openingAngle, new Tuple<int, int>(start, start + delay), "rgba(255, 255, 0, 0.6)", new AgentConnector(boss)));
                                replay.Actors.Add(new PieActor(true, 0, maxRadius - (i * radiusDecrement), rotation - (i * angleIncrement), openingAngle, new Tuple<int, int>(start, start + delay), "rgba(255, 180, 0, 0.3)", new AgentConnector(boss)));

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
                        replay.Actors.Add(new CircleActor(true, 0, ccRadius, new Tuple<int, int>((int)c.Time, (int)c.Time + c.ActualDuration), "rgba(0, 180, 255, 0.3)", new AgentConnector(boss)));
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
                            replay.Actors.Add(new PieActor(true, 0, radius, facing, openingAngle, new Tuple<int, int>(start + delay, start + delay + duration), "rgba(255, 200, 0, 0.3)", new AgentConnector(boss)));
                            replay.Actors.Add(new PieActor(true, 0, radius, facing, openingAngle, new Tuple<int, int>(start + delay + duration, start + delay + fieldDuration), "rgba(255, 50, 0, 0.3)", new PositionConnector(pos)));
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
                                replay.Actors.Add(new PieActor(false, 0, maxRadius - (i * radiusDecrement), rotation - (i * angleIncrement), openingAngle, new Tuple<int, int>(start, start + delay), "rgba(255, 255, 0, 0.6)", new AgentConnector(boss)));
                                replay.Actors.Add(new PieActor(true, 0, maxRadius - (i * radiusDecrement), rotation - (i * angleIncrement), openingAngle, new Tuple<int, int>(start, start + delay), "rgba(255, 180, 0, 0.3)", new AgentConnector(boss)));
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
                        replay.Actors.Add(new CircleActor(false, start + delay + duration, maxRadius, new Tuple<int, int>(start + delay, start + delay + duration), "rgba(255, 200, 0, 0.5)", new AgentConnector(boss)));
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
                        Point3D bossPosition = replay.Positions.LastOrDefault(x => x.Time <= start + 1000);
                        if (facing != null && bossPosition != null)
                        {
                            Point3D position = new Point3D(bossPosition.X + facing.X * spellCenterDistance, bossPosition.Y + facing.Y * spellCenterDistance, bossPosition.Z, bossPosition.Time);
                            replay.Actors.Add(new CircleActor(true, 0, impactRadius, new Tuple<int, int>(start, start + delay), "rgba(255, 100, 0, 0.1)", new PositionConnector(position)));
                            replay.Actors.Add(new CircleActor(true, 0, impactRadius, new Tuple<int, int>(start+delay-10, start + delay+100), "rgba(255, 100, 0, 0.5)", new PositionConnector(position)));
                            replay.Actors.Add(new CircleActor(false, start + delay + duration, maxRadius, new Tuple<int, int>(start + delay, start + delay + duration), "rgba(255, 200, 0, 0.5)", new PositionConnector(position)));
                        }
                    }
                    //CC
                    List<CastLog> summon = cls.Where(x => x.SkillId == 52054).ToList();
                    foreach (CastLog c in summon)
                    {
                        int radius = ccRadius;
                        replay.Actors.Add(new CircleActor(true, 0, ccRadius, new Tuple<int, int>((int)c.Time, (int)c.Time + c.ActualDuration), "rgba(0, 180, 255, 0.3)", new AgentConnector(boss)));
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
                                replay.Actors.Add(new PieActor(false, 0, maxRadius - (i * radiusDecrement), rotation - (i * angleIncrement), openingAngle, new Tuple<int, int>(start, start + delay), "rgba(255, 255, 0, 0.6)", new AgentConnector(boss)));
                                replay.Actors.Add(new PieActor(true, 0, maxRadius - (i * radiusDecrement), rotation - (i * angleIncrement), openingAngle, new Tuple<int, int>(start, start + delay), "rgba(255, 180, 0, 0.3)", new AgentConnector(boss)));
                            }
                        }
                    }
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }

        public override void ComputeAdditionalPlayerData(Player p, ParsedLog log)
        {

        }

        public override int IsCM(ParsedLog log)
        {
            Boss target = Targets.Find(x => x.ID == (ushort)ParseEnum.BossIDS.Qadim);
            if (target == null)
            {
                throw new InvalidOperationException("Target for CM detection not found");
            }
            return (target.Health > 21e6) ? 1 : 0;
        }
        
    }
}
