using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static LuckParser.Models.DataModels.ParseEnum.TrashIDS;

namespace LuckParser.Models
{
    public class ValeGuardian : RaidLogic
    {
        public ValeGuardian(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new Mechanic(31860, "Unstable Magic Spike", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.ValeGuardian, "symbol:'circle',color:'rgb(0,0,255)'", "G.TP","Unstable Magic Spike (Green Guard Teleport)","Green Guard TP",500),
            new Mechanic(31392, "Unstable Magic Spike", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.ValeGuardian, "symbol:'circle',color:'rgb(0,0,255)'", "B.TP","Unstable Magic Spike (Boss Teleport)", "Boss TP",500),
            new Mechanic(31340, "Distributed Magic", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.ValeGuardian, "symbol:'circle',color:'rgb(0,128,0)'", "Grn","Distributed Magic (Stood in Green)", "Green Team",0),
            new Mechanic(31391, "Distributed Magic", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.ValeGuardian, "symbol:'circle',color:'rgb(0,128,0)'", "Grn","Distributed Magic (Stood in Green)", "Green Team",0),
            new Mechanic(31529, "Distributed Magic", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.ValeGuardian, "symbol:'circle',color:'rgb(0,128,0)'", "Grn","Distributed Magic (Stood in Green)", "Green Team", 0),
            new Mechanic(31750, "Distributed Magic", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.ValeGuardian, "symbol:'circle',color:'rgb(0,128,0)'", "Grn","Distributed Magic (Stood in Green)", "Green Team",0), 
            new Mechanic(31886, "Magic Pulse", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.ValeGuardian, "symbol:'circle-open',color:'rgb(255,0,0)'", "Skr","Magic Pulse (Hit by Seeker)", "Seeker",0),
            new Mechanic(31695, "Pylon Attunement: Red", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.ValeGuardian, "symbol:'square',color:'rgb(255,0,0)'", "Att.R","Pylon Attunement: Red", "Red Attuned",0),
            new Mechanic(31317, "Pylon Attunement: Blue", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.ValeGuardian, "symbol:'square',color:'rgb(0,0,255)'", "Att.B","Pylon Attunement: Blue", "Blue Attuned",0),
            new Mechanic(31852, "Pylon Attunement: Green", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.ValeGuardian, "symbol:'square',color:'rgb(0,128,0)'", "Att.G","Pylon Attunement: Green", "Green Attuned",0),
            new Mechanic(31413, "Blue Pylon Power", Mechanic.MechType.EnemyBoonStrip, ParseEnum.BossIDS.ValeGuardian, "symbol:'square-open',color:'rgb(0,0,255)'", "InvlnStrp","Stripped Blue Guard Invuln", "Blue Invuln Strip",0),
            new Mechanic(31539, "Unstable Pylon", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.ValeGuardian, "symbol:'hexagram-open',color:'rgb(255,0,0)'", "Flr.R","Unstable Pylon (Red Floor dmg)", "Floor dmg",0),
            new Mechanic(31828, "Unstable Pylon", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.ValeGuardian, "symbol:'hexagram-open',color:'rgb(0,0,255)'", "Flr.B","Unstable Pylon (Blue Floor dmg)", "Floor dmg",0),
            new Mechanic(31884, "Unstable Pylon", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.ValeGuardian, "symbol:'hexagram-open',color:'rgb(0,128,0)'", "Flr.G","Unstable Pylon (Green Floor dmg)", "Floor dmg",0),
            new Mechanic(31419, "Magic Storm", Mechanic.MechType.EnemyCastStart, ParseEnum.BossIDS.ValeGuardian, "symbol:'diamond-tall',color:'rgb(0,160,150)'", "CC","Magic Storm (Breakbar)","Breakbar",0),
            new Mechanic(31419, "Magic Storm", Mechanic.MechType.EnemyCastEnd, ParseEnum.BossIDS.ValeGuardian, "symbol:'diamond-tall',color:'rgb(0,160,0)'", "CCed","Magic Storm (Breakbar broken) ", "CCed",0,(condition => condition.CombatItem.Value <=8544)),
            new Mechanic(31419, "Magic Storm", Mechanic.MechType.EnemyCastEnd, ParseEnum.BossIDS.ValeGuardian, "symbol:'diamond-tall',color:'rgb(255,0,0)'", "CC.Fail","Magic Storm (Breakbar failed) ", "CC Fail",0,(condition => condition.CombatItem.Value >8544)),
            });
            Extension = "vg";
            IconUrl = "https://wiki.guildwars2.com/images/f/fb/Mini_Vale_Guardian.png";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/W7MocGz.png",
                            Tuple.Create(889, 889),
                            Tuple.Create(-6365, -22213, -3150, -18999),
                            Tuple.Create(-15360, -36864, 15360, 39936),
                            Tuple.Create(3456, 11012, 4736, 14212));
        }

        protected override List<ushort> GetFightTargetsIDs()
        {
            return new List<ushort>
            {
                (ushort)ParseEnum.BossIDS.ValeGuardian,
                (ushort)RedGuardian,
                (ushort)BlueGuardian,
                (ushort)GreenGuardian
            };
        }

        public override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            long start = 0;
            long end = 0;
            long fightDuration = log.FightData.FightDuration;
            List<PhaseData> phases = GetInitialPhase(log);
            Boss mainTarget = Targets.Find(x => x.ID == (ushort)ParseEnum.BossIDS.ValeGuardian);
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
            List<CombatItem> invulsVG = GetFilteredList(log, 757, log.Boss.InstID);
            for (int i = 0; i < invulsVG.Count; i++)
            {
                CombatItem c = invulsVG[i];
                if (c.IsBuffRemove == ParseEnum.BuffRemove.None)
                {
                    end = c.Time - log.FightData.FightStart;
                    phases.Add(new PhaseData(start, end));
                    if (i == invulsVG.Count - 1)
                    {
                        log.Boss.AddCustomCastLog(new CastLog(end, -5, (int)(fightDuration - end), ParseEnum.Activation.None, (int)(fightDuration - end), ParseEnum.Activation.None), log);
                    }
                }
                else
                {
                    start = c.Time - log.FightData.FightStart;
                    phases.Add(new PhaseData(end, start));
                    log.Boss.AddCustomCastLog(new CastLog(end, -5, (int)(start - end), ParseEnum.Activation.None, (int)(start - end), ParseEnum.Activation.None), log);
                }
            }
            if (fightDuration - start > 5000 && start >= phases.Last().End)
            {
                phases.Add(new PhaseData(start, fightDuration));
            }
            string[] namesVG = new [] { "Phase 1", "Split 1", "Phase 2", "Split 2", "Phase 3" };
            bool[] drawStartVG = { false, false, true, false, true };
            bool[] drawEndVG = { true, false, true, false, false };
            bool[] drawAreaVG = { true, false, true, false, true };
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                phase.Name = namesVG[i - 1];
                phase.DrawStart = drawStartVG[i - 1];
                phase.DrawEnd = drawEndVG[i - 1];
                phase.DrawArea = drawAreaVG[i - 1];
                if (i == 2 || i == 4)
                {
                    List<ushort> ids = new List<ushort>
                    {
                       (ushort) BlueGuardian,
                       (ushort) GreenGuardian,
                       (ushort) RedGuardian
                    };
                    AddTargetsToPhase(phase, ids, log);
                } else
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
                    Tuple<int, int> lifespan = new Tuple<int, int>((int)mob.CombatReplay.TimeOffsets.Item1, (int)mob.CombatReplay.TimeOffsets.Item2);
                    mob.CombatReplay.Icon = "https://i.imgur.com/FrPoluz.png";
                    mob.CombatReplay.Actors.Add(new CircleActor(false, 0, 180, lifespan, "rgba(255, 0, 0, 0.5)"));
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }       
        }

        public override void ComputeAdditionalBossData(Boss boss, ParsedLog log)
        {
            CombatReplay replay = boss.CombatReplay;
            List<CastLog> cls = boss.GetCastLogs(log, 0, log.FightData.FightDuration);
            Tuple<int, int> lifespan = new Tuple<int, int>((int)boss.CombatReplay.TimeOffsets.Item1, (int)boss.CombatReplay.TimeOffsets.Item2);
            switch (boss.ID)
            {
                case (ushort)ParseEnum.BossIDS.ValeGuardian:
                    replay.Icon = "https://i.imgur.com/MIpP5pK.png";
                    List<CastLog> magicStorms = cls.Where(x => x.SkillId == 31419).ToList();
                    foreach (CastLog c in magicStorms)
                    {
                        replay.Actors.Add(new CircleActor(true, 0, 100, new Tuple<int, int>((int)c.Time, (int)c.Time + c.ActualDuration), "rgba(0, 180, 255, 0.3)"));
                    }
                    break;
                case (ushort)BlueGuardian:
                    replay.Actors.Add(new CircleActor(false, 0, 1500, lifespan, "rgba(0, 0, 255, 0.5)"));
                    replay.Icon = "https://i.imgur.com/6CefnkP.png";
                    break;
                case (ushort)GreenGuardian:
                    replay.Actors.Add(new CircleActor(false, 0, 1500, lifespan, "rgba(0, 255, 0, 0.5)"));
                    replay.Icon = "https://i.imgur.com/nauDVYP.png";
                    break;
                case (ushort)RedGuardian:
                    replay.Actors.Add(new CircleActor(false, 0, 1500, lifespan, "rgba(255, 0, 0, 0.5)"));
                    replay.Icon = "https://i.imgur.com/73Uj4lG.png";
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }
    }
}
