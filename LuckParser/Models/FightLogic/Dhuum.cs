using LuckParser.Parser;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static LuckParser.Parser.ParseEnum.TrashIDS;

namespace LuckParser.Models.Logic
{
    public class Dhuum : RaidLogic
    {
        public Dhuum(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new SkillOnPlayerMechanic(48172, "Hateful Ephemera", new MechanicPlotlySetting("square","rgb(255,140,0)"), "Golem","Hateful Ephemera (Golem AoE dmg)", "Golem Dmg",0),
            new SkillOnPlayerMechanic(48121, "Arcing Affliction", new MechanicPlotlySetting("circle-open","rgb(255,0,0)"), "Bomb dmg","Arcing Affliction (Bomb) hit", "Bomb dmg",0),
            new PlayerBoonApplyMechanic(47646, "Arcing Affliction", new MechanicPlotlySetting("circle","rgb(255,0,0)"), "Bomb","Arcing Affliction (Bomb) application", "Bomb",0),
            //new Mechanic(47476, "Residual Affliction", ParseEnum.BossIDS.Dhuum, new MechanicPlotlySetting("star-diamond","rgb(255,200,0)"), "Bomb",0), //not needed, imho, applied at the same time as Arcing Affliction
            new PlayerOnPlayerMechanic(47335, "Soul Shackle", new MechanicPlotlySetting("diamond","rgb(0,255,255)"), "Shackles","Soul Shackle (Tether) application", "Shackles",0),//  //also used for removal.
            new SkillOnPlayerMechanic(47164, "Soul Shackle", new MechanicPlotlySetting("diamond-open","rgb(0,255,255)"), "Shackles dmg","Soul Shackle (Tether) dmg ticks", "Shackles Dmg",0,  new List<MechanicChecker>{ new CombatItemValueChecker(0, MechanicChecker.ValueCompare.G) }, Mechanic.TriggerRule.AND),
            new SkillOnPlayerMechanic(47561, "Slash", new MechanicPlotlySetting("triangle","rgb(0,128,0)"), "Cone","Boon ripping Cone Attack", "Cone",0),
            new SkillOnPlayerMechanic(48752, "Cull", new MechanicPlotlySetting("asterisk-open","rgb(0,255,255)"), "Crack","Cull (Fearing Fissures)", "Cracks",0),
            new SkillOnPlayerMechanic(48760, "Putrid Bomb", new MechanicPlotlySetting("circle","rgb(0,128,0)"), "Mark","Necro Marks during Scythe attack", "Necro Marks",0),
            new SkillOnPlayerMechanic(48398, "Cataclysmic Cycle", new MechanicPlotlySetting("circle-open","rgb(255,140,0)"), "Suck dmg","Damage when sucked to close to middle", "Suck dmg",0),
            new SkillOnPlayerMechanic(48176, "Death Mark", new MechanicPlotlySetting("hexagon","rgb(255,140,0)"), "Dip","Lesser Death Mark hit (Dip into ground)", "Dip AoE",0),
            new SkillOnPlayerMechanic(48210, "Greater Death Mark", new MechanicPlotlySetting("circle","rgb(255,140,0)"), "KB dmg","Knockback damage during Greater Deathmark (mid port)", "Knockback dmg",0),
          //  new Mechanic(48281, "Mortal Coil", ParseEnum.BossIDS.Dhuum, new MechanicPlotlySetting("circle","rgb(0,128,0)"), "Green Orbs",
            new PlayerBoonApplyMechanic(46950, "Fractured Spirit", new MechanicPlotlySetting("square","rgb(0,255,0)"), "Orb CD","Applied when taking green", "Green port",0),
            new SkillOnPlayerMechanic(47076 , "Echo's Damage", new MechanicPlotlySetting("square","rgb(255,0,0)"), "Echo","Damaged by Ender's Echo (pick up)", "Ender's Echo",5000),
            });
            Extension = "dhuum";
            IconUrl = "https://wiki.guildwars2.com/images/e/e4/Mini_Dhuum.png";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/CLTwWBJ.png",
                            (3763, 3383),
                            (13524, -1334, 18039, 2735),
                            (-21504, -12288, 24576, 12288),
                            (19072, 15484, 20992, 16508));
        }

        private void ComputeFightPhases(Target mainTarget, List<PhaseData> phases, ParsedLog log, List<CastLog> castLogs, long fightDuration, long start)
        {
            CastLog shield = castLogs.Find(x => x.SkillId == 47396);
            if (shield != null)
            {
                long end = shield.Time;
                phases.Add(new PhaseData(start, end));
                CastLog firstDamage = castLogs.FirstOrDefault(x => x.SkillId == 47304 && x.Time >= end);
                if (firstDamage != null)
                {
                    phases.Add(new PhaseData(firstDamage.Time, fightDuration));
                }
            }
            else
            {
                phases.Add(new PhaseData(start, fightDuration));
            }
        }

        private List<PhaseData> GetInBetweenSoulSplits(ParsedLog log, Target dhuum, long mainStart, long mainEnd)
        {
            List<CastLog> cls = dhuum.GetCastLogs(log, 0, log.FightData.FightDuration);
            List<CastLog> cataCycle = cls.Where(x => x.SkillId == 48398).ToList();
            List<CastLog> gDeathmark = cls.Where(x => x.SkillId == 48210).ToList();
            if (gDeathmark.Count < cataCycle.Count)
            {
                return new List<PhaseData>();
            }
            List<PhaseData> phases = new List<PhaseData>();
            long start = mainStart;
            long end = 0;
            int i = 1;
            foreach (CastLog cl in cataCycle)
            {
                CastLog clDeathmark = gDeathmark[i - 1];
                end = Math.Min(clDeathmark.Time, mainEnd);
                phases.Add(new PhaseData(start, end)
                {
                    Name = "Pre-Soulsplit " + i++
                });
                start = cl.Time + cl.ActualDuration;
            }
            phases.Add(new PhaseData(start, mainEnd)
            {
                Name = "Pre-Ritual"
            });
            foreach (PhaseData phase in phases)
            {
                phase.Targets.Add(dhuum);
            }
            phases.RemoveAll(x => x.DurationInMS <= 2200);
            return phases;
        }

        public override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            long fightDuration = log.FightData.FightDuration;
            List<PhaseData> phases = GetInitialPhase(log);
            Target mainTarget = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.Dhuum);
            if (mainTarget == null)
            {
                throw new InvalidOperationException("Main target of the fight not found");
            }
            phases[0].Targets.Add(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            // Sometimes the preevent is not in the evtc
            List<CastLog> castLogs = mainTarget.GetCastLogs(log, 0, log.FightData.FightEnd);
            List<CastLog> dhuumCast = mainTarget.GetCastLogs(log, 0, 20000);
            string[] namesDh;
            if (dhuumCast.Count > 0)
            {
                namesDh = new[] { "Main Fight", "Ritual" };
                ComputeFightPhases(mainTarget, phases, log, castLogs, fightDuration, 0);
            }
            else
            {
                CombatItem invulDhuum = log.CombatData.GetBoonData(762).FirstOrDefault(x => x.IsBuffRemove != ParseEnum.BuffRemove.None && x.SrcInstid == mainTarget.InstID && x.Time > 115000 + log.FightData.FightStart);
                if (invulDhuum != null)
                {
                    long end = log.FightData.ToFightSpace(invulDhuum.Time);
                    phases.Add(new PhaseData(0, end));
                    ComputeFightPhases(mainTarget, phases, log, castLogs, fightDuration, end + 1);
                }
                else
                {
                    phases.Add(new PhaseData(0, fightDuration));
                }
                namesDh = new[] { "Roleplay", "Main Fight", "Ritual" };
            }
            for (int i = 1; i < phases.Count; i++)
            {
                phases[i].Name = namesDh[i - 1];
                phases[i].Targets.Add(mainTarget);
            }
            if (dhuumCast.Count > 0 && phases.Count > 1)
            {
                phases.AddRange(GetInBetweenSoulSplits(log, mainTarget, phases[1].Start, phases[1].End));
                phases.Sort((x, y) => x.Start.CompareTo(y.Start));
            }
            else if (phases.Count > 2)
            {
                phases.AddRange(GetInBetweenSoulSplits(log, mainTarget, phases[2].Start, phases[2].End));
                phases.Sort((x, y) => x.Start.CompareTo(y.Start));
            }
            return phases;
        }

        protected override List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>
            {
                Echo,
                Enforcer,
                Messenger,
                Deathling,
                UnderworldReaper
            };
        }

        public override void ComputeAdditionalTargetData(Target target, ParsedLog log)
        {
            // TODO: correct position
            CombatReplay replay = target.CombatReplay;
            List<CastLog> cls = target.GetCastLogs(log, 0, log.FightData.FightDuration);
            switch (target.ID)
            {
                case (ushort)ParseEnum.TargetIDS.Dhuum:
                    List<CastLog> deathmark = cls.Where(x => x.SkillId == 48176).ToList();
                    CastLog majorSplit = cls.Find(x => x.SkillId == 47396);
                    foreach (CastLog c in deathmark)
                    {
                        int start = (int)c.Time;
                        int zoneActive = start + 1550;
                        int zoneDeadly = zoneActive + 6000; //point where the zone becomes impossible to walk through unscathed
                        int zoneEnd = zoneActive + 120000;
                        int radius = 450;
                        if (majorSplit != null)
                        {
                            zoneEnd = Math.Min(zoneEnd, (int)majorSplit.Time);
                            zoneDeadly = Math.Min(zoneDeadly, (int)majorSplit.Time);
                        }
                        int spellCenterDistance = 200; //hitbox radius
                        Point3D facing = replay.Rotations.LastOrDefault(x => x.Time <= start + 3000);
                        Point3D targetPosition = replay.Positions.LastOrDefault(x => x.Time <= start + 3000);
                        if (facing != null && targetPosition != null)
                        {
                            Point3D position = new Point3D(targetPosition.X + (facing.X * spellCenterDistance), targetPosition.Y + (facing.Y * spellCenterDistance), targetPosition.Z, targetPosition.Time);
                            replay.Actors.Add(new CircleActor(true, zoneActive, radius, (start, zoneActive), "rgba(200, 255, 100, 0.5)", new PositionConnector(position)));
                            replay.Actors.Add(new CircleActor(false, 0, radius, (start, zoneActive), "rgba(200, 255, 100, 0.5)", new PositionConnector(position)));
                            replay.Actors.Add(new CircleActor(true, 0, radius, (zoneActive, zoneDeadly), "rgba(200, 255, 100, 0.5)", new PositionConnector(position)));
                            replay.Actors.Add(new CircleActor(true, 0, radius, (zoneDeadly, zoneEnd), "rgba(255, 100, 0, 0.5)", new PositionConnector(position)));

                        }
                    }
                    List<CastLog> cataCycle = cls.Where(x => x.SkillId == 48398).ToList();
                    foreach (CastLog c in cataCycle)
                    {
                        int start = (int)c.Time;
                        int end = start + c.ActualDuration;
                        replay.Actors.Add(new CircleActor(true, end, 300, (start, end), "rgba(255, 150, 0, 0.7)", new AgentConnector(target)));
                        replay.Actors.Add(new CircleActor(true, 0, 300, (start, end), "rgba(255, 150, 0, 0.5)", new AgentConnector(target)));
                    }
                    List<CastLog> slash = cls.Where(x => x.SkillId == 47561).ToList();
                    foreach (CastLog c in slash)
                    {
                        int start = (int)c.Time;
                        int end = start + c.ActualDuration;
                        Point3D facing = replay.Rotations.FirstOrDefault(x => x.Time >= start);
                        if (facing == null)
                        {
                            continue;
                        }
                        replay.Actors.Add(new PieActor(false, 0, 850, facing, 60, (start, end), "rgba(255, 150, 0, 0.5)", new AgentConnector(target)));
                    }

                    if (majorSplit != null)
                    {
                        int start = (int)majorSplit.Time;
                        int end = (int)log.FightData.FightDuration;
                        replay.Actors.Add(new CircleActor(true, 0, 320, (start, end), "rgba(0, 180, 255, 0.2)", new AgentConnector(target)));
                    }
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }

        }

        public override void ComputeAdditionalTrashMobData(Mob mob, ParsedLog log)
        {
            CombatReplay replay = mob.CombatReplay;
            int start = (int)replay.TimeOffsets.start;
            int end = (int)replay.TimeOffsets.end;
            switch (mob.ID)
            {
                case (ushort)Echo:
                    replay.Actors.Add(new CircleActor(true, 0, 120, (start, end), "rgba(255, 0, 0, 0.5)", new AgentConnector(mob)));
                    break;
                case (ushort)Enforcer:
                    break;
                case (ushort)Messenger:
                    replay.Actors.Add(new CircleActor(true, 0, 180, (start, end), "rgba(255, 125, 0, 0.5)", new AgentConnector(mob)));
                    break;
                case (ushort)Deathling:
                case (ushort)UnderworldReaper:
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");

            }
        }

        public override void ComputeAdditionalPlayerData(Player p, ParsedLog log)
        {
            // spirit transform
            CombatReplay replay = p.CombatReplay;
            List<CombatItem> spiritTransform = log.CombatData.GetBoonData(46950).Where(x => x.DstInstid == p.InstID && x.IsBuffRemove == ParseEnum.BuffRemove.None).ToList();
            Target mainTarget = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.Dhuum);
            if (mainTarget == null)
            {
                throw new InvalidOperationException("Main target of the fight not found");
            }
            foreach (CombatItem c in spiritTransform)
            {
                int duration = 15000;
                int start = (int)(log.FightData.ToFightSpace(c.Time));
                if (mainTarget.HealthOverTime.FirstOrDefault(x => x.X > start).Y < 1050)
                {
                    duration = 30000;
                }
                CombatItem removedBuff = log.CombatData.GetBoonData(48281).FirstOrDefault(x => x.SrcInstid == p.InstID && x.IsBuffRemove == ParseEnum.BuffRemove.All && x.Time > c.Time && x.Time < c.Time + duration);
                int end = start + duration;
                if (removedBuff != null)
                {
                    end = (int)(log.FightData.ToFightSpace(removedBuff.Time));
                }
                replay.Actors.Add(new CircleActor(true, 0, 100, (start, end), "rgba(0, 50, 200, 0.3)", new AgentConnector(p)));
                replay.Actors.Add(new CircleActor(true, start + duration, 100, (start, end), "rgba(0, 50, 200, 0.5)", new AgentConnector(p)));
            }
            // bomb
            List<CombatItem> bombDhuum = GetFilteredList(log, 47646, p, true);
            int bombDhuumStart = 0;
            foreach (CombatItem c in bombDhuum)
            {
                if (c.IsBuffRemove == ParseEnum.BuffRemove.None)
                {
                    bombDhuumStart = (int)(log.FightData.ToFightSpace(c.Time));
                }
                else
                {
                    int bombDhuumEnd = (int)(log.FightData.ToFightSpace(c.Time));
                    replay.Actors.Add(new CircleActor(true, 0, 100, (bombDhuumStart, bombDhuumEnd), "rgba(80, 180, 0, 0.3)", new AgentConnector(p)));
                    replay.Actors.Add(new CircleActor(true, bombDhuumStart + 13000, 100, (bombDhuumStart, bombDhuumEnd), "rgba(80, 180, 0, 0.5)", new AgentConnector(p)));
                }
            }
            // shackles connection
            List<CombatItem> shackles = GetFilteredList(log, 47335, p, true).Concat(GetFilteredList(log, 48591, p, true)).ToList();
            int shacklesStart = 0;
            Player shacklesTarget = null;
            foreach (CombatItem c in shackles)
            {
                if (c.IsBuffRemove == ParseEnum.BuffRemove.None)
                {
                    shacklesStart = (int)(log.FightData.ToFightSpace(c.Time));
                    shacklesTarget = log.PlayerList.FirstOrDefault(x => x.Agent == c.SrcAgent);
                }
                else
                {
                    int shacklesEnd = (int)(log.FightData.ToFightSpace(c.Time));
                    if (shacklesTarget != null)
                    {
                        replay.Actors.Add(new LineActor(0, (shacklesStart, shacklesEnd), "rgba(0, 255, 255, 0.5)", new AgentConnector(p), new AgentConnector(shacklesTarget)));
                    }
                }
            }
            // shackles damage (identical to the connection for now, not yet properly distinguishable from the pure connection, further investigation needed due to inconsistent behavior (triggering too early, not triggering the damaging skill though)
            // shackles start with buff 47335 applied from one player to the other, this is switched over to buff 48591 after mostly 2 seconds, sometimes later. This is switched to 48042 usually 4 seconds after initial application and the damaging skill 47164 starts to deal damage from that point on.
            // Before that point, 47164 is only logged when evaded/blocked, but doesn't deal damage. Further investigation needed.
            List<CombatItem> shacklesDmg = GetFilteredList(log, 48042, p, true);
            int shacklesDmgStart = 0;
            Player shacklesDmgTarget = null;
            foreach (CombatItem c in shacklesDmg)
            {
                if (c.IsBuffRemove == ParseEnum.BuffRemove.None)
                {
                    shacklesDmgStart = (int)(log.FightData.ToFightSpace(c.Time));
                    shacklesDmgTarget = log.PlayerList.FirstOrDefault(x => x.Agent == c.SrcAgent);
                }
                else
                {
                    int shacklesDmgEnd = (int)(log.FightData.ToFightSpace(c.Time));
                    if (shacklesDmgTarget != null)
                    {
                        replay.Actors.Add(new LineActor(0, (shacklesDmgStart, shacklesDmgEnd), "rgba(0, 255, 255, 0.5)", new AgentConnector(p), new AgentConnector(shacklesDmgTarget)));
                    }
                }
            }
        }

        public override int IsCM(ParsedLog log)
        {
            Target target = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.Dhuum);
            if (target == null)
            {
                throw new InvalidOperationException("Target for CM detection not found");
            }
            OverrideMaxHealths(log);
            return (target.Health > 35e6) ? 1 : 0;
        }
    }
}
