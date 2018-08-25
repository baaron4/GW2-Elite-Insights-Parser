using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models
{
    public class Gorseval : BossLogic
    {
        public Gorseval()
        {
            Mode = ParseMode.Raid;
            MechanicList.AddRange(new List<Mechanic>
            {
            new Mechanic(31875, "Spectral Impact", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Gorseval, "symbol:'hexagram',color:'rgb(255,0,0)',", "Slam",4000), // Spectral Impact (KB Slam), Slam
            new Mechanic(31623, "Ghastly Prison", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Gorseval, "symbol:'circle',color:'rgb(255,140,0)',", "Egg",500), //Ghastly Prison (Egged), Egged
            new Mechanic(31498, "Spectral Darkness", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Gorseval, "symbol:'circle',color:'rgb(0,0,255)',", "OrbDbf",100), // Spectral Darkness (Stood in Orb AoE), Orb Debuff
            new Mechanic(31722, "Spirited Fusion", Mechanic.MechType.EnemyBoon, ParseEnum.BossIDS.Gorseval, "symbol:'square',color:'rgb(255,140,0)',", "SprtBf",0), // Spirited Fusion (Consumed a Spirit), Ate Spirit
            new Mechanic(31720, "Kick", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Gorseval, "symbol:'triangle-right',color:'rgb(255,0,255)',", "Kick",0), // Kicked by small add, Spirit Kick
            new Mechanic(738, "Ghastly Rampage", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Gorseval, "symbol:'circle',color:'rgb(0,0,0)',", "Black",3000,(value => value == 10000))// //stood in black? Trigger via (25 stacks) vuln (ID 738) application would be possible
            });
        }

        public override CombatReplayMap GetCombatMap()
        {
            return new CombatReplayMap("https://i.imgur.com/nTueZcX.png",
                            Tuple.Create(1354, 1415),
                            Tuple.Create(-653, -6754, 3701, -2206),
                            Tuple.Create(-15360, -36864, 15360, 39936),
                            Tuple.Create(3456, 11012, 4736, 14212));
        }

        public override List<PhaseData> GetPhases(Boss boss, ParsedLog log, List<CastLog> castLogs)
        {
            long start = 0;
            long end = 0;
            long fightDuration = log.GetBossData().GetAwareDuration();
            List<PhaseData> phases = GetInitialPhase(log);
            // Ghostly protection check
            List<CombatItem> invulsGorse = log.GetCombatList().Where(x => x.GetSkillID() == 31790 && x.IsBuffremove() != ParseEnum.BuffRemove.Manual).ToList();
            for (int i = 0; i < invulsGorse.Count; i++)
            {
                CombatItem c = invulsGorse[i];
                if (c.IsBuffremove() == ParseEnum.BuffRemove.None)
                {
                    end = c.GetTime() - log.GetBossData().GetFirstAware();
                    phases.Add(new PhaseData(start, end));
                    if (i == invulsGorse.Count - 1)
                    {
                        castLogs.Add(new CastLog(end, -5, (int)(fightDuration - end), ParseEnum.Activation.None, (int)(fightDuration - end), ParseEnum.Activation.None));
                    }
                }
                else
                {
                    start = c.GetTime() - log.GetBossData().GetFirstAware();
                    phases.Add(new PhaseData(end, start));
                    castLogs.Add(new CastLog(end, -5, (int)(start - end), ParseEnum.Activation.None, (int)(start - end), ParseEnum.Activation.None));
                }
            }
            if (fightDuration - start > 5000 && start >= phases.Last().GetEnd())
            {
                phases.Add(new PhaseData(start, fightDuration));
            }
            string[] namesGorse = new [] { "Phase 1", "Split 1", "Phase 2", "Split 2", "Phase 3" };
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                phase.SetName(namesGorse[i - 1]);
                if (i == 2 || i == 4)
                {
                    List<AgentItem> spirits = log.GetAgentData().GetNPCAgentList().Where(x => ParseEnum.GetThrashIDS(x.GetID()) == ParseEnum.ThrashIDS.ChargedSoul).ToList();
                    foreach (AgentItem a in spirits)
                    {
                        long agentStart = a.GetFirstAware() - log.GetBossData().GetFirstAware();
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
            // TODO: doughnuts (rampage)
            List<ParseEnum.ThrashIDS> ids = new List<ParseEnum.ThrashIDS>
                    {
                        ParseEnum.ThrashIDS.ChargedSoul,
                        ParseEnum.ThrashIDS.EnragedSpirit,
                        ParseEnum.ThrashIDS.AngeredSpirit
                    };
            List<CastLog> blooms = cls.Where(x => x.GetID() == 31616).ToList();
            foreach (CastLog c in blooms)
            {
                int start = (int)c.GetTime();
                int end = start + c.GetActDur();
                replay.AddCircleActor(new CircleActor(true, c.GetExpDur() + (int)c.GetTime(), 600, new Tuple<int, int>(start, end), "rgba(255, 125, 0, 0.5)"));
                replay.AddCircleActor(new CircleActor(false, 0, 600, new Tuple<int, int>(start, end), "rgba(255, 125, 0, 0.5)"));
            }
            List<PhaseData> phases = log.GetBoss().GetPhases(log, true);
            if (phases.Count > 1)
            {
                List<CastLog> rampage = cls.Where(x => x.GetID() == 31834).ToList();
                Point3D pos = log.GetBoss().GetCombatReplay().GetPositions().First();
                foreach (CastLog c in rampage)
                {
                    int start = (int)c.GetTime();
                    int end = start + c.GetActDur();
                    replay.AddCircleActor(new CircleActor(true, 0, 180, new Tuple<int, int>(start, end), "rgba(0, 125, 255, 0.3)"));
                    // or spawn -> 3 secs -> explosion -> 0.5 secs -> fade -> 0.5  secs-> next
                    int ticks = (int)Math.Min(Math.Ceiling(c.GetActDur() / 4000.0),6);
                    int phaseIndex;
                    for (phaseIndex = 1; phaseIndex < phases.Count; phaseIndex++)
                    {
                        if (phases[phaseIndex].InInterval(start))
                        {
                            break;
                        }
                    }
                    if (pos == null)
                    {
                        break;
                    }
                    List<string> patterns;
                    switch (phaseIndex)
                    {
                        case 1:
                            patterns = new List<string>
                            {
                                "2+3+5",
                                "2+3+4",
                                "1+4+5",
                                "1+2+5",
                                "1+3+5",
                                "Full"
                            };
                            break;
                        case 3:
                            patterns = new List<string>
                            {
                                "2+3+4",
                                "1+4+5",
                                "1+3+4",
                                "1+2+5",
                                "1+2+3",
                                "Full"
                            };
                            break;
                        case 5:
                            patterns = new List<string>
                            {
                                "1+4+5",
                                "1+2+5",
                                "2+3+5",
                                "3+4+5",
                                "3+4+5",
                                "Full"
                            };
                            break;
                        default:
                            throw new Exception("how the fuck");
                    }
                    start += 2200;
                    for (int i = 0; i < ticks; i++)
                    {
                        int tickStart = start + 4000 * i;
                        int explosion = tickStart + 3000;
                        int tickEnd = tickStart + 3500;
                        string pattern = patterns[i];
                        if (pattern.Contains("1"))
                        {
                            replay.AddCircleActor(new CircleActor(true, explosion, 360, new Tuple<int, int>(tickStart, tickEnd), "rgba(25,25,112, 0.2)", pos));
                            replay.AddCircleActor(new CircleActor(true,0 , 360, new Tuple<int, int>(tickStart, tickEnd), "rgba(25,25,112, 0.4)", pos));
                        }
                        if (pattern.Contains("2"))
                        {
                            replay.AddDoughnutActor(new DoughnutActor(explosion, 360, 720, new Tuple<int, int>(tickStart, tickEnd), "rgba(25,25,112, 0.2)", pos));
                            replay.AddDoughnutActor(new DoughnutActor(0, 360, 720, new Tuple<int, int>(tickStart, tickEnd), "rgba(25,25,112, 0.4)", pos));
                        }
                        if (pattern.Contains("3"))
                        {
                            replay.AddDoughnutActor(new DoughnutActor(explosion, 720, 1080, new Tuple<int, int>(tickStart, tickEnd), "rgba(25,25,112, 0.2)", pos));
                            replay.AddDoughnutActor(new DoughnutActor(0, 720, 1080, new Tuple<int, int>(tickStart, tickEnd), "rgba(25,25,112, 0.4)", pos));
                        }
                        if (pattern.Contains("4"))
                        {
                            replay.AddDoughnutActor(new DoughnutActor(explosion, 1080, 1440, new Tuple<int, int>(tickStart, tickEnd), "rgba(25,25,112, 0.2)", pos));
                            replay.AddDoughnutActor(new DoughnutActor(0, 1080, 1440, new Tuple<int, int>(tickStart, tickEnd), "rgba(25,25,112, 0.4)", pos));
                        }
                        if (pattern.Contains("5"))
                        {
                            replay.AddDoughnutActor(new DoughnutActor(explosion, 1440, 1800, new Tuple<int, int>(tickStart, tickEnd), "rgba(25,25,112, 0.2)", pos));
                            replay.AddDoughnutActor(new DoughnutActor(0, 1440, 1800, new Tuple<int, int>(tickStart, tickEnd), "rgba(25,25,112, 0.4)", pos));
                        }
                        if (pattern.Contains("Full"))
                        {
                            tickStart -= 1000;
                            explosion -= 1000;
                            tickEnd -= 1000;
                            replay.AddCircleActor(new CircleActor(true, explosion, 1800, new Tuple<int, int>(tickStart, tickEnd), "rgba(25,25,112, 0.2)", pos));
                            replay.AddCircleActor(new CircleActor(true, 0, 1800, new Tuple<int, int>(tickStart, tickEnd), "rgba(25,25,112, 0.4)", pos));
                        }
                    }
                }
            }

            return ids;
        }

        public override string GetReplayIcon()
        {
            return "https://i.imgur.com/5hmMq12.png";
        }
    }
}
