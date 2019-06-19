using LuckParser.Parser;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static LuckParser.Parser.ParseEnum.TrashIDS;

namespace LuckParser.Models.Logic
{
    public class Gorseval : RaidLogic
    {
        public Gorseval(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new HitOnPlayerMechanic(31875, "Spectral Impact", new MechanicPlotlySetting("hexagram","rgb(255,0,0)"), "Slam","Spectral Impact (KB Slam)", "Slam",4000, new List<SkillMechanic.SkillChecker>{(de, log) => !de.To.HasBuff(log, 1122, de.Time)}, Mechanic.TriggerRule.AND ),
            new PlayerBoonApplyMechanic(31623, "Ghastly Prison", new MechanicPlotlySetting("circle","rgb(255,140,0)"), "Egg","Ghastly Prison (Egged)", "Egged",500),
            new PlayerBoonApplyMechanic(31498, "Spectral Darkness", new MechanicPlotlySetting("circle","rgb(0,0,255)"), "Orb Debuff","Spectral Darkness (Stood in Orb AoE)", "Orb Debuff",100),
            new EnemyBoonApplyMechanic(31722, "Spirited Fusion", new MechanicPlotlySetting("square","rgb(255,140,0)"), "Spirit Buff","Spirited Fusion (Consumed a Spirit)", "Ate Spirit",0),
            new HitOnPlayerMechanic(31720, "Kick", new MechanicPlotlySetting("triangle-right","rgb(255,0,255)"), "Kick","Kicked by small add", "Spirit Kick",0, new List<SkillMechanic.SkillChecker>{(de, log) => !de.To.HasBuff(log, 1122, de.Time)}, Mechanic.TriggerRule.AND),
            new PlayerBoonApplyMechanic(738, "Ghastly Rampage Black Goo Hit", new MechanicPlotlySetting("circle","rgb(0,0,0)"), "Black","Hit by Black Goo","Black Goo",3000, new List<BoonApplyMechanic.BoonApplyChecker>{ (ba,log) => ba.AppliedDuration == 10000 }, Mechanic.TriggerRule.AND),
            new EnemyCastMechanic(31834, "Ghastly Rampage", new MechanicPlotlySetting("diamond-tall","rgb(0,160,150)"), "CC","Ghastly Rampage (Breakbar)", "Breakbar",0, false),
            new EnemyCastMechanic(31834, "Ghastly Rampage", new MechanicPlotlySetting("diamond-tall","rgb(255,0,0)"), "CC End","Ghastly Rampage (Full duration)", "CC ran out",0, true, new List<CastMechanic.CastChecker>{ (ce,log) => ce.ActualDuration > 21985 }, Mechanic.TriggerRule.AND),
            new EnemyCastMechanic(31834, "Ghastly Rampage", new MechanicPlotlySetting("diamond-tall","rgb(0,160,0)"), "CCed","Ghastly Rampage (Breakbar broken)", "CCed",0, true, new List<CastMechanic.CastChecker>{ (ce, log) => ce.ActualDuration <= 21985 }, Mechanic.TriggerRule.AND),
            });
            Extension = "gors";
            IconUrl = "https://wiki.guildwars2.com/images/d/d1/Mini_Gorseval_the_Multifarious.png";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/nTueZcX.png",
                            (1354, 1415),
                            (-653, -6754, 3701, -2206),
                            (-15360, -36864, 15360, 39936),
                            (3456, 11012, 4736, 14212));
        }

        public override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            Target mainTarget = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.Gorseval);
            if (mainTarget == null)
            {
                throw new InvalidOperationException("Main target of the fight not found");
            }
            phases[0].Targets.Add(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            phases.AddRange(GetPhasesByInvul(log, 31877, mainTarget, true, true));
            string[] namesGorse = new[] { "Phase 1", "Split 1", "Phase 2", "Split 2", "Phase 3" };
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                phase.Name = namesGorse[i - 1];
                if (i == 1 || i == 3 || i == 5)
                {
                    phase.Targets.Add(mainTarget);
                }
                else
                {
                    List<ushort> ids = new List<ushort>
                    {
                       (ushort) ChargedSoul
                    };
                    AddTargetsToPhase(phase, ids, log);
                }
            }
            return phases;
        }

        protected override List<ushort> GetFightTargetsIDs()
        {
            return new List<ushort>
            {
                (ushort)ParseEnum.TargetIDS.Gorseval,
                (ushort)ChargedSoul
            };
        }

        protected override List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>
            {
                EnragedSpirit,
                AngeredSpirit
            };
        }

        public override void ComputeTargetCombatReplayActors(Target target, ParsedLog log, CombatReplay replay)
        {
            List<AbstractCastEvent> cls = target.GetCastLogs(log, 0, log.FightData.FightDuration);
            switch (target.ID)
            {
                case (ushort)ParseEnum.TargetIDS.Gorseval:
                    List<AbstractCastEvent> blooms = cls.Where(x => x.SkillId == 31616).ToList();
                    foreach (AbstractCastEvent c in blooms)
                    {
                        int start = (int)c.Time;
                        int end = start + c.ActualDuration;
                        replay.Actors.Add(new CircleActor(true, c.ExpectedDuration + start, 600, (start, end), "rgba(255, 125, 0, 0.5)", new AgentConnector(target)));
                        replay.Actors.Add(new CircleActor(false, 0, 600, (start, end), "rgba(255, 125, 0, 0.5)", new AgentConnector(target)));
                    }
                    List<PhaseData> phases = log.FightData.GetPhases(log);
                    if (phases.Count > 1)
                    {
                        List<AbstractCastEvent> rampage = cls.Where(x => x.SkillId == 31834).ToList();
                        Point3D pos = replay.PolledPositions.First();
                        foreach (AbstractCastEvent c in rampage)
                        {
                            int start = (int)c.Time;
                            int end = start + c.ActualDuration;
                            replay.Actors.Add(new CircleActor(true, 0, 180, (start, end), "rgba(0, 125, 255, 0.3)", new AgentConnector(target)));
                            // or spawn -> 3 secs -> explosion -> 0.5 secs -> fade -> 0.5  secs-> next
                            int ticks = (int)Math.Min(Math.Ceiling(c.ActualDuration / 4000.0), 6);
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
                                    replay.Actors.Add(new CircleActor(true, explosion, 360, (tickStart, tickEnd), "rgba(25,25,112, 0.2)", new PositionConnector(pos)));
                                    replay.Actors.Add(new CircleActor(true, 0, 360, (tickStart, tickEnd), "rgba(25,25,112, 0.4)", new PositionConnector(pos)));
                                }
                                if (pattern.Contains("2"))
                                {
                                    replay.Actors.Add(new DoughnutActor(true, explosion, 360, 720, (tickStart, tickEnd), "rgba(25,25,112, 0.2)", new PositionConnector(pos)));
                                    replay.Actors.Add(new DoughnutActor(true, 0, 360, 720, (tickStart, tickEnd), "rgba(25,25,112, 0.4)", new PositionConnector(pos)));
                                }
                                if (pattern.Contains("3"))
                                {
                                    replay.Actors.Add(new DoughnutActor(true, explosion, 720, 1080, (tickStart, tickEnd), "rgba(25,25,112, 0.2)", new PositionConnector(pos)));
                                    replay.Actors.Add(new DoughnutActor(true, 0, 720, 1080, (tickStart, tickEnd), "rgba(25,25,112, 0.4)", new PositionConnector(pos)));
                                }
                                if (pattern.Contains("4"))
                                {
                                    replay.Actors.Add(new DoughnutActor(true, explosion, 1080, 1440, (tickStart, tickEnd), "rgba(25,25,112, 0.2)", new PositionConnector(pos)));
                                    replay.Actors.Add(new DoughnutActor(true, 0, 1080, 1440, (tickStart, tickEnd), "rgba(25,25,112, 0.4)", new PositionConnector(pos)));
                                }
                                if (pattern.Contains("5"))
                                {
                                    replay.Actors.Add(new DoughnutActor(true, explosion, 1440, 1800, (tickStart, tickEnd), "rgba(25,25,112, 0.2)", new PositionConnector(pos)));
                                    replay.Actors.Add(new DoughnutActor(true, 0, 1440, 1800, (tickStart, tickEnd), "rgba(25,25,112, 0.4)", new PositionConnector(pos)));
                                }
                                if (pattern.Contains("Full"))
                                {
                                    tickStart -= 1000;
                                    explosion -= 1000;
                                    tickEnd -= 1000;
                                    replay.Actors.Add(new CircleActor(true, explosion, 1800, (tickStart, tickEnd), "rgba(25,25,112, 0.2)", new PositionConnector(pos)));
                                    replay.Actors.Add(new CircleActor(true, 0, 1800, (tickStart, tickEnd), "rgba(25,25,112, 0.4)", new PositionConnector(pos)));
                                }
                            }
                        }
                    }
                    List<AbstractCastEvent> slam = cls.Where(x => x.SkillId == 31875).ToList();
                    foreach (AbstractCastEvent c in slam)
                    {
                        int start = (int)c.Time;
                        int impactPoint = 1185;
                        int impactTime = start + impactPoint;
                        int end = (int)Math.Min(start + c.ActualDuration, impactTime);
                        int radius = 320;
                        replay.Actors.Add(new CircleActor(true, 0, radius, (start, end), "rgba(255, 0, 0, 0.2)", new AgentConnector(target)));
                        replay.Actors.Add(new CircleActor(true, 0, radius, (impactTime - 10, impactTime + 100), "rgba(255, 0, 0, 0.4)", new AgentConnector(target)));
                    }
                    List<AbstractBuffEvent> protection = GetFilteredList(log.CombatData, 31877, target, true);
                    int protectionStart = 0;
                    foreach (AbstractBuffEvent c in protection)
                    {
                        if (c is BuffApplyEvent)
                        {
                            protectionStart = (int)c.Time;
                        }
                        else
                        {
                            int protectionEnd = (int)c.Time;
                            replay.Actors.Add(new CircleActor(true, 0, 300, (protectionStart, protectionEnd), "rgba(0, 180, 255, 0.5)", new AgentConnector(target)));
                        }
                    }
                    break;
                case (ushort)ChargedSoul:
                    (int, int) lifespan = ((int)replay.TimeOffsets.start, (int)replay.TimeOffsets.end);
                    replay.Actors.Add(new CircleActor(false, 0, 220, lifespan, "rgba(255, 150, 0, 0.5)", new AgentConnector(target)));
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }
    }
}
