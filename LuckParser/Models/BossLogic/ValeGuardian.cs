using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models
{
    public class ValeGuardian : RaidLogic
    {
        public ValeGuardian()
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new Mechanic(31860, "Unstable Magic Spike", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.ValeGuardian, "symbol:'circle',color:'rgb(0,0,255)',", "G.TP","Unstable Magic Spike (Green Guard Teleport)","Green Guard TP",500),
            new Mechanic(31392, "Unstable Magic Spike", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.ValeGuardian, "symbol:'circle',color:'rgb(0,0,255)',", "B.TP","Unstable Magic Spike (Boss Teleport)", "Boss TP",500),
            new Mechanic(31340, "Distributed Magic", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.ValeGuardian, "symbol:'circle',color:'rgb(0,128,0)',", "Grn","Distributed Magic (Stood in Green)", "Green Team",0),
            new Mechanic(31391, "Distributed Magic", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.ValeGuardian, "symbol:'circle',color:'rgb(0,128,0)',", "Grn","Distributed Magic (Stood in Green)", "Green Team",0),
            new Mechanic(31529, "Distributed Magic", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.ValeGuardian, "symbol:'circle',color:'rgb(0,128,0)',", "Grn","Distributed Magic (Stood in Green)", "Green Team", 0),
            new Mechanic(31750, "Distributed Magic", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.ValeGuardian, "symbol:'circle',color:'rgb(0,128,0)',", "Grn","Distributed Magic (Stood in Green)", "Green Team",0), 
            new Mechanic(31886, "Magic Pulse", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.ValeGuardian, "symbol:'circle-open',color:'rgb(255,0,0)',", "Skr","Magic Pulse (Hit by Seeker)", "Seeker",0),
            new Mechanic(31695, "Pylon Attunement: Red", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.ValeGuardian, "symbol:'square',color:'rgb(255,0,0)',", "Att.R","Pylon Attunement: Red", "Red Attuned",0),
            new Mechanic(31317, "Pylon Attunement: Blue", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.ValeGuardian, "symbol:'square',color:'rgb(0,0,255)',", "Att.B","Pylon Attunement: Blue", "Blue Attuned",0),
            new Mechanic(31852, "Pylon Attunement: Green", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.ValeGuardian, "symbol:'square',color:'rgb(0,128,0)',", "Att.G","Pylon Attunement: Green", "Green Attuned",0),
            new Mechanic(31413, "Blue Pylon Power", Mechanic.MechType.EnemyBoonStrip, ParseEnum.BossIDS.ValeGuardian, "symbol:'square-open',color:'rgb(0,0,255)',", "InvlnStrp","Stripped Blue Guard Invuln", "Blue Invuln Strip",0),
            new Mechanic(31539, "Unstable Pylon", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.ValeGuardian, "symbol:'hexagram-open',color:'rgb(255,0,0)',", "Flr.R","Unstable Pylon (Red Floor dmg)", "Floor dmg",0),
            new Mechanic(31828, "Unstable Pylon", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.ValeGuardian, "symbol:'hexagram-open',color:'rgb(0,0,255)',", "Flr.B","Unstable Pylon (Blue Floor dmg)", "Floor dmg",0),
            new Mechanic(31884, "Unstable Pylon", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.ValeGuardian, "symbol:'hexagram-open',color:'rgb(0,128,0)',", "Flr.G","Unstable Pylon (Green Floor dmg)", "Floor dmg",0),
            new Mechanic(31419, "Magic Storm", Mechanic.MechType.EnemyCastStart, ParseEnum.BossIDS.ValeGuardian, "symbol:'diamond-tall',color:'rgb(0,160,150)',", "CC","Magic Storm (Breakbar)","Breakbar",0),
            new Mechanic(31419, "Magic Storm", Mechanic.MechType.EnemyCastEnd, ParseEnum.BossIDS.ValeGuardian, "symbol:'diamond-tall',color:'rgb(0,160,0)',", "CCed","Magic Storm (Breakbar broken) ", "CCed",0,(value => value <=8544)),
            new Mechanic(31419, "Magic Storm", Mechanic.MechType.EnemyCastEnd, ParseEnum.BossIDS.ValeGuardian, "symbol:'diamond-tall',color:'rgb(255,0,0)',", "CC.Fail","Magic Storm (Breakbar failed) ", "CC Fail",0,(value => value >8544)),
            });
        }

        public override CombatReplayMap GetCombatMap()
        {
            return new CombatReplayMap("https://i.imgur.com/W7MocGz.png",
                            Tuple.Create(889, 889),
                            Tuple.Create(-6365, -22213, -3150, -18999),
                            Tuple.Create(-15360, -36864, 15360, 39936),
                            Tuple.Create(3456, 11012, 4736, 14212));
        }

        public override List<PhaseData> GetPhases(Boss boss, ParsedLog log, List<CastLog> castLogs)
        {
            long start = 0;
            long end = 0;
            long fightDuration = log.GetBossData().GetAwareDuration();
            List<PhaseData> phases = GetInitialPhase(log);
            // Invul check
            List<CombatItem> invulsVG = GetFilteredList(log, 757, boss.GetInstid());
            for (int i = 0; i < invulsVG.Count; i++)
            {
                CombatItem c = invulsVG[i];
                if (c.IsBuffRemove == ParseEnum.BuffRemove.None)
                {
                    end = c.Time - log.GetBossData().GetFirstAware();
                    phases.Add(new PhaseData(start, end));
                    if (i == invulsVG.Count - 1)
                    {
                        castLogs.Add(new CastLog(end, -5, (int)(fightDuration - end), ParseEnum.Activation.None, (int)(fightDuration - end), ParseEnum.Activation.None));
                    }
                }
                else
                {
                    start = c.Time - log.GetBossData().GetFirstAware();
                    phases.Add(new PhaseData(end, start));
                    castLogs.Add(new CastLog(end, -5, (int)(start - end), ParseEnum.Activation.None, (int)(start - end), ParseEnum.Activation.None));
                }
            }
            if (fightDuration - start > 5000 && start >= phases.Last().GetEnd())
            {
                phases.Add(new PhaseData(start, fightDuration));
            }
            string[] namesVG = new [] { "Phase 1", "Split 1", "Phase 2", "Split 2", "Phase 3" };
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                phase.SetName(namesVG[i - 1]);
                if (i == 2 || i == 4)
                {
                    List<ParseEnum.ThrashIDS> ids = new List<ParseEnum.ThrashIDS>
                    {
                       ParseEnum.ThrashIDS.BlueGuardian,
                       ParseEnum.ThrashIDS.GreenGuardian,
                       ParseEnum.ThrashIDS.RedGuardian
                    };
                    List<AgentItem> guardians = log.GetAgentData().GetNPCAgentList().Where(x => ids.Contains(ParseEnum.GetThrashIDS(x.ID))).ToList();
                    foreach (AgentItem a in guardians)
                    {
                        long agentStart = a.FirstAware - log.GetBossData().GetFirstAware();
                        if (phase.InInterval(agentStart))
                        {
                            phase.AddRedirection(a);
                        }
                    }
                    phase.OverrideStart(log.GetBossData().GetFirstAware());
                }
            }
            return phases;
        }

        public override List<ParseEnum.ThrashIDS> GetAdditionalData(CombatReplay replay, List<CastLog> cls, ParsedLog log)
        {
            List<ParseEnum.ThrashIDS> ids = new List<ParseEnum.ThrashIDS>
            {
               ParseEnum.ThrashIDS.Seekers,
               ParseEnum.ThrashIDS.BlueGuardian,
               ParseEnum.ThrashIDS.GreenGuardian,
               ParseEnum.ThrashIDS.RedGuardian
            };
            List<CastLog> magicStorms = cls.Where(x => x.GetID() == 31419).ToList();
            foreach (CastLog c in magicStorms)
            {
                replay.AddCircleActor(new CircleActor(true, 0, 100, new Tuple<int, int>((int)c.GetTime(), (int)c.GetTime() + c.GetActDur()), "rgba(0, 180, 255, 0.3)"));
            }
            return ids;
        }

        public override string GetReplayIcon()
        {
            return "https://i.imgur.com/MIpP5pK.png";
        }
    }
}
