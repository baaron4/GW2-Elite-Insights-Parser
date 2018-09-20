using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models
{
    public class Gorseval : RaidLogic
    {
        public Gorseval()
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new Mechanic(31875, "Spectral Impact", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Gorseval, "symbol:'hexagram',color:'rgb(255,0,0)',", "Slam","Spectral Impact (KB Slam)", "Slam",4000),
            new Mechanic(31623, "Ghastly Prison", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Gorseval, "symbol:'circle',color:'rgb(255,140,0)',", "Egg","Ghastly Prison (Egged)", "Egged",500),
            new Mechanic(31498, "Spectral Darkness", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Gorseval, "symbol:'circle',color:'rgb(0,0,255)',", "OrbDbf","Spectral Darkness (Stood in Orb AoE)", "Orb Debuff",100),
            new Mechanic(31722, "Spirited Fusion", Mechanic.MechType.EnemyBoon, ParseEnum.BossIDS.Gorseval, "symbol:'square',color:'rgb(255,140,0)',", "SprtBf","Spirited Fusion (Consumed a Spirit)", "Ate Spirit",0),
            new Mechanic(31720, "Kick", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Gorseval, "symbol:'triangle-right',color:'rgb(255,0,255)',", "Kick","Kicked by small add", "Spirit Kick",0),
            new Mechanic(738, "Ghastly Rampage Black Goo Hit", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Gorseval, "symbol:'circle',color:'rgb(0,0,0)',", "Black","Hit by Black Goo","Black Goo",3000,(condition => condition.CombatItem.Value == 10000)),
            new Mechanic(31834, "Ghastly Rampage", Mechanic.MechType.EnemyCastStart, ParseEnum.BossIDS.Gorseval, "symbol:'diamond-tall',color:'rgb(0,160,150)',", "CC","Ghastly Rampage (Breakbar)", "Breakbar",0),
            new Mechanic(31834, "Ghastly Rampage", Mechanic.MechType.EnemyCastEnd, ParseEnum.BossIDS.Gorseval, "symbol:'diamond-tall',color:'rgb(255,0,0)',", "CC.End","Ghastly Rampage (Full duration)", "CC ran out",0,(condition => condition.CombatItem.Value > 21985)),
            new Mechanic(31834, "Ghastly Rampage", Mechanic.MechType.EnemyCastEnd, ParseEnum.BossIDS.Gorseval, "symbol:'diamond-tall',color:'rgb(0,160,0)',", "CCed","Ghastly Rampage (Breakbar broken)", "CCed",0,(condition => condition.CombatItem.Value <= 21985)),
            });
            Extension = "gors";
            IconUrl = "https://wiki.guildwars2.com/images/d/d1/Mini_Gorseval_the_Multifarious.png";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/nTueZcX.png",
                            Tuple.Create(1354, 1415),
                            Tuple.Create(-653, -6754, 3701, -2206),
                            Tuple.Create(-15360, -36864, 15360, 39936),
                            Tuple.Create(3456, 11012, 4736, 14212));
        }

        public override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            long start = 0;
            long end = 0;
            long fightDuration = log.FightData.FightDuration;
            List<PhaseData> phases = GetInitialPhase(log);
            if (!requirePhases)
            {
                return phases;
            }
            // Ghostly protection check
            List<CombatItem> invulsGorse = log.GetBoonData(31790).Where(x => x.IsBuffRemove != ParseEnum.BuffRemove.Manual).ToList();
            for (int i = 0; i < invulsGorse.Count; i++)
            {
                CombatItem c = invulsGorse[i];
                if (c.IsBuffRemove == ParseEnum.BuffRemove.None)
                {
                    end = c.Time - log.FightData.FightStart;
                    phases.Add(new PhaseData(start, end));
                    if (i == invulsGorse.Count - 1)
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
            string[] namesGorse = new [] { "Phase 1", "Split 1", "Phase 2", "Split 2", "Phase 3" };
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                phase.Name = namesGorse[i - 1];
                phase.DrawArea = i == 1 || i == 3 || i == 5;
                phase.DrawStart = i == 3 || i == 5;
                phase.DrawEnd = i == 1 || i == 3;
                if (i == 2 || i == 4)
                {
                    List<AgentItem> spirits = log.AgentData.NPCAgentList.Where(x => ParseEnum.GetTrashIDS(x.ID) == ParseEnum.TrashIDS.ChargedSoul).ToList();
                    foreach (AgentItem a in spirits)
                    {
                        long agentStart = a.FirstAware - log.FightData.FightStart;
                        if (phase.InInterval(agentStart))
                        {
                            phase.Redirection.Add(a);
                        }
                    }
                    phase.OverrideStart(log.FightData.FightStart);
                }
            }
            return phases;
        }

        public override List<ParseEnum.TrashIDS> GetAdditionalBossData(CombatReplay replay, List<CastLog> cls, ParsedLog log)
        {
            // TODO: doughnuts (rampage)
            List<ParseEnum.TrashIDS> ids = new List<ParseEnum.TrashIDS>
                    {
                        ParseEnum.TrashIDS.ChargedSoul,
                        ParseEnum.TrashIDS.EnragedSpirit,
                        ParseEnum.TrashIDS.AngeredSpirit
                    };
            List<CastLog> blooms = cls.Where(x => x.SkillId == 31616).ToList();
            foreach (CastLog c in blooms)
            {
                int start = (int)c.Time;
                int end = start + c.ActualDuration;
                replay.Actors.Add(new CircleActor(true, c.ExpectedDuration + (int)c.Time, 600, new Tuple<int, int>(start, end), "rgba(255, 125, 0, 0.5)"));
                replay.Actors.Add(new CircleActor(false, 0, 600, new Tuple<int, int>(start, end), "rgba(255, 125, 0, 0.5)"));
            }
            List<PhaseData> phases = log.FightData.GetPhases(log);
            if (phases.Count > 1)
            {
                List<CastLog> rampage = cls.Where(x => x.SkillId == 31834).ToList();
                Point3D pos = log.Boss.CombatReplay.Positions.First();
                foreach (CastLog c in rampage)
                {
                    int start = (int)c.Time;
                    int end = start + c.ActualDuration;
                    replay.Actors.Add(new CircleActor(true, 0, 180, new Tuple<int, int>(start, end), "rgba(0, 125, 255, 0.3)"));
                    // or spawn -> 3 secs -> explosion -> 0.5 secs -> fade -> 0.5  secs-> next
                    int ticks = (int)Math.Min(Math.Ceiling(c.ActualDuration / 4000.0),6);
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
                            replay.Actors.Add(new CircleActor(true, explosion, 360, new Tuple<int, int>(tickStart, tickEnd), "rgba(25,25,112, 0.2)", pos));
                            replay.Actors.Add(new CircleActor(true ,0 , 360, new Tuple<int, int>(tickStart, tickEnd), "rgba(25,25,112, 0.4)", pos));
                        }
                        if (pattern.Contains("2"))
                        {
                            replay.Actors.Add(new DoughnutActor(true, explosion, 360, 720, new Tuple<int, int>(tickStart, tickEnd), "rgba(25,25,112, 0.2)", pos));
                            replay.Actors.Add(new DoughnutActor(true, 0, 360, 720, new Tuple<int, int>(tickStart, tickEnd), "rgba(25,25,112, 0.4)", pos));
                        }
                        if (pattern.Contains("3"))
                        {
                            replay.Actors.Add(new DoughnutActor(true, explosion, 720, 1080, new Tuple<int, int>(tickStart, tickEnd), "rgba(25,25,112, 0.2)", pos));
                            replay.Actors.Add(new DoughnutActor(true, 0, 720, 1080, new Tuple<int, int>(tickStart, tickEnd), "rgba(25,25,112, 0.4)", pos));
                        }
                        if (pattern.Contains("4"))
                        {
                            replay.Actors.Add(new DoughnutActor(true, explosion, 1080, 1440, new Tuple<int, int>(tickStart, tickEnd), "rgba(25,25,112, 0.2)", pos));
                            replay.Actors.Add(new DoughnutActor(true, 0, 1080, 1440, new Tuple<int, int>(tickStart, tickEnd), "rgba(25,25,112, 0.4)", pos));
                        }
                        if (pattern.Contains("5"))
                        {
                            replay.Actors.Add(new DoughnutActor(true, explosion, 1440, 1800, new Tuple<int, int>(tickStart, tickEnd), "rgba(25,25,112, 0.2)", pos));
                            replay.Actors.Add(new DoughnutActor(true, 0, 1440, 1800, new Tuple<int, int>(tickStart, tickEnd), "rgba(25,25,112, 0.4)", pos));
                        }
                        if (pattern.Contains("Full"))
                        {
                            tickStart -= 1000;
                            explosion -= 1000;
                            tickEnd -= 1000;
                            replay.Actors.Add(new CircleActor(true, explosion, 1800, new Tuple<int, int>(tickStart, tickEnd), "rgba(25,25,112, 0.2)", pos));
                            replay.Actors.Add(new CircleActor(true, 0, 1800, new Tuple<int, int>(tickStart, tickEnd), "rgba(25,25,112, 0.4)", pos));
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
