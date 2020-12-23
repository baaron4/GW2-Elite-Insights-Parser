using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class Gorseval : RaidLogic
    {
        public Gorseval(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new HitOnPlayerMechanic(31875, "Spectral Impact", new MechanicPlotlySetting("hexagram","rgb(255,0,0)"), "Slam","Spectral Impact (KB Slam)", "Slam",4000, (de, log) => !de.To.HasBuff(log, 1122, de.Time)),
            new PlayerBuffApplyMechanic(31623, "Ghastly Prison", new MechanicPlotlySetting("circle","rgb(255,140,0)"), "Egg","Ghastly Prison (Egged)", "Egged",500),
            new PlayerBuffApplyMechanic(31498, "Spectral Darkness", new MechanicPlotlySetting("circle","rgb(0,0,255)"), "Orb Debuff","Spectral Darkness (Stood in Orb AoE)", "Orb Debuff",100),
            new EnemyBuffApplyMechanic(31722, "Spirited Fusion", new MechanicPlotlySetting("square","rgb(255,140,0)"), "Spirit Buff","Spirited Fusion (Consumed a Spirit)", "Ate Spirit",0),
            new HitOnPlayerMechanic(31720, "Kick", new MechanicPlotlySetting("triangle-right","rgb(255,0,255)"), "Kick","Kicked by small add", "Spirit Kick",0, (de, log) => !de.To.HasBuff(log, 1122, de.Time)),
            new PlayerBuffApplyMechanic(738, "Ghastly Rampage Black Goo Hit", new MechanicPlotlySetting("circle","rgb(0,0,0)"), "Black","Hit by Black Goo","Black Goo",3000, (ba,log) => ba.AppliedDuration == 10000),
            new EnemyCastStartMechanic(31834, "Ghastly Rampage", new MechanicPlotlySetting("diamond-tall","rgb(0,160,150)"), "CC","Ghastly Rampage (Breakbar)", "Breakbar",0),
            new EnemyCastEndMechanic(31834, "Ghastly Rampage", new MechanicPlotlySetting("diamond-tall","rgb(255,0,0)"), "CC End","Ghastly Rampage (Full duration)", "CC ran out",0, (ce,log) => ce.ActualDuration > 21985),
            new EnemyCastEndMechanic(31834, "Ghastly Rampage", new MechanicPlotlySetting("diamond-tall","rgb(0,160,0)"), "CCed","Ghastly Rampage (Breakbar broken)", "CCed",0, (ce, log) => ce.ActualDuration <= 21985),
            });
            Extension = "gors";
            Icon = "https://wiki.guildwars2.com/images/d/d1/Mini_Gorseval_the_Multifarious.png";
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/nTueZcX.png",
                            (1354, 1415),
                            (-653, -6754, 3701, -2206)/*,
                            (-15360, -36864, 15360, 39936),
                            (3456, 11012, 4736, 14212)*/);
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            NPC mainTarget = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.Gorseval);
            if (mainTarget == null)
            {
                throw new MissingKeyActorsException("Gorseval not found");
            }
            phases[0].AddTarget(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            phases.AddRange(GetPhasesByInvul(log, 31877, mainTarget, true, true));
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                if (i % 2 == 1)
                {
                    phase.Name = "Phase " + (i + 1) / 2;
                    phase.AddTarget(mainTarget);
                }
                else
                {
                    phase.Name = "Split " + (i) / 2;
                    var ids = new List<int>
                    {
                       (int) ArcDPSEnums.TrashID.ChargedSoul
                    };
                    AddTargetsToPhase(phase, ids, log);
                }
            }
            return phases;
        }

        protected override List<int> GetFightTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.Gorseval,
                (int)ArcDPSEnums.TrashID.ChargedSoul
            };
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDS()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.EnragedSpirit,
                ArcDPSEnums.TrashID.AngeredSpirit
            };
        }

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            IReadOnlyList<AbstractCastEvent> cls = target.GetCastEvents(log, 0, log.FightData.FightEnd);
            switch (target.ID)
            {
                case (int)ArcDPSEnums.TargetID.Gorseval:
                    var blooms = cls.Where(x => x.SkillId == 31616).ToList();
                    foreach (AbstractCastEvent c in blooms)
                    {
                        int start = (int)c.Time;
                        int end = (int)c.EndTime;
                        replay.Decorations.Add(new CircleDecoration(true, c.ExpectedDuration + start, 600, (start, end), "rgba(255, 125, 0, 0.5)", new AgentConnector(target)));
                        replay.Decorations.Add(new CircleDecoration(false, 0, 600, (start, end), "rgba(255, 125, 0, 0.5)", new AgentConnector(target)));
                    }
                    IReadOnlyList<PhaseData> phases = log.FightData.GetPhases(log);
                    if (phases.Count > 1)
                    {
                        var rampage = cls.Where(x => x.SkillId == 31834).ToList();
                        const byte first = 1 << 0;
                        const byte second = 1 << 1;
                        const byte third = 1 << 2;
                        const byte fourth = 1 << 3;
                        const byte fifth = 1 << 4;
                        const byte full = 1 << 5;

                        Point3D pos = replay.PolledPositions.First();
                        foreach (AbstractCastEvent c in rampage)
                        {
                            int start = (int)c.Time;
                            int end = (int)c.EndTime;
                            replay.Decorations.Add(new CircleDecoration(true, 0, 180, (start, end), "rgba(0, 125, 255, 0.3)", new AgentConnector(target)));
                            // or spawn -> 3 secs -> explosion -> 0.5 secs -> fade -> 0.5  secs-> next
                            int ticks = (int)Math.Min(Math.Ceiling(c.ActualDuration / 4000.0), 6);
                            int phaseIndex;
                            // get only phases where Gorseval is target (aka main phases)
                            var gorsevalPhases = phases.Where(x => x.Targets.Contains(target)).ToList();
                            for (phaseIndex = 1; phaseIndex < gorsevalPhases.Count; phaseIndex++)
                            {
                                if (gorsevalPhases[phaseIndex].InInterval(start))
                                {
                                    break;
                                }
                            }
                            if (pos == null)
                            {
                                break;
                            }
                            List<byte> patterns;
                            switch (phaseIndex)
                            {
                                case 1:
                                    patterns = new List<byte>
                                    {
                                        second | third | fifth,
                                        second | third | fourth,
                                        first | fourth | fifth,
                                        first | second | fifth,
                                        first | third | fifth,
                                        full
                                    };
                                    break;
                                case 2:
                                    patterns = new List<byte>
                                    {
                                        second | third | fourth,
                                        first | fourth | fifth,
                                        first | third | fourth,
                                        first | second | fifth,
                                        first | second | third,
                                        full
                                    };
                                    break;
                                case 3:
                                    patterns = new List<byte>
                                    {
                                        first | fourth | fifth,
                                        first | second | fifth,
                                        second | third | fifth,
                                        third | fourth | fifth,
                                        third | fourth | fifth,
                                        full
                                    };
                                    break;
                                default:
                                    // no reason to stop parsing because of CR, worst case, no rampage
                                    patterns = new List<byte>();
                                    ticks = 0;
                                    break;
                                    //throw new EIException("Gorseval cast rampage during a split phase");
                            }
                            start += 2200;
                            for (int i = 0; i < ticks; i++)
                            {
                                int tickStart = start + 4000 * i;
                                int explosion = tickStart + 3000;
                                int tickEnd = tickStart + 3500;
                                byte pattern = patterns[i];
                                if ((pattern & first) > 0)
                                {
                                    replay.Decorations.Add(new CircleDecoration(true, explosion, 360, (tickStart, tickEnd), "rgba(25,25,112, 0.2)", new PositionConnector(pos)));
                                    replay.Decorations.Add(new CircleDecoration(true, 0, 360, (tickStart, tickEnd), "rgba(25,25,112, 0.4)", new PositionConnector(pos)));
                                }
                                if ((pattern & second) > 0)
                                {
                                    replay.Decorations.Add(new DoughnutDecoration(true, explosion, 360, 720, (tickStart, tickEnd), "rgba(25,25,112, 0.2)", new PositionConnector(pos)));
                                    replay.Decorations.Add(new DoughnutDecoration(true, 0, 360, 720, (tickStart, tickEnd), "rgba(25,25,112, 0.4)", new PositionConnector(pos)));
                                }
                                if ((pattern & third) > 0)
                                {
                                    replay.Decorations.Add(new DoughnutDecoration(true, explosion, 720, 1080, (tickStart, tickEnd), "rgba(25,25,112, 0.2)", new PositionConnector(pos)));
                                    replay.Decorations.Add(new DoughnutDecoration(true, 0, 720, 1080, (tickStart, tickEnd), "rgba(25,25,112, 0.4)", new PositionConnector(pos)));
                                }
                                if ((pattern & fourth) > 0)
                                {
                                    replay.Decorations.Add(new DoughnutDecoration(true, explosion, 1080, 1440, (tickStart, tickEnd), "rgba(25,25,112, 0.2)", new PositionConnector(pos)));
                                    replay.Decorations.Add(new DoughnutDecoration(true, 0, 1080, 1440, (tickStart, tickEnd), "rgba(25,25,112, 0.4)", new PositionConnector(pos)));
                                }
                                if ((pattern & fifth) > 0)
                                {
                                    replay.Decorations.Add(new DoughnutDecoration(true, explosion, 1440, 1800, (tickStart, tickEnd), "rgba(25,25,112, 0.2)", new PositionConnector(pos)));
                                    replay.Decorations.Add(new DoughnutDecoration(true, 0, 1440, 1800, (tickStart, tickEnd), "rgba(25,25,112, 0.4)", new PositionConnector(pos)));
                                }
                                if ((pattern & full) > 0)
                                {
                                    tickStart -= 1000;
                                    explosion -= 1000;
                                    tickEnd -= 1000;
                                    replay.Decorations.Add(new CircleDecoration(true, explosion, 1800, (tickStart, tickEnd), "rgba(25,25,112, 0.2)", new PositionConnector(pos)));
                                    replay.Decorations.Add(new CircleDecoration(true, 0, 1800, (tickStart, tickEnd), "rgba(25,25,112, 0.4)", new PositionConnector(pos)));
                                }
                            }
                        }
                    }
                    var slam = cls.Where(x => x.SkillId == 31875).ToList();
                    foreach (AbstractCastEvent c in slam)
                    {
                        int start = (int)c.Time;
                        int impactPoint = 1185;
                        int impactTime = start + impactPoint;
                        int end = Math.Min((int)c.EndTime, impactTime);
                        int radius = 320;
                        replay.Decorations.Add(new CircleDecoration(true, 0, radius, (start, end), "rgba(255, 0, 0, 0.2)", new AgentConnector(target)));
                        replay.Decorations.Add(new CircleDecoration(true, 0, radius, (impactTime, impactTime + 100), "rgba(255, 0, 0, 0.4)", new AgentConnector(target)));
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
                            replay.Decorations.Add(new CircleDecoration(true, 0, 300, (protectionStart, protectionEnd), "rgba(0, 180, 255, 0.5)", new AgentConnector(target)));
                        }
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.ChargedSoul:
                    var lifespan = ((int)replay.TimeOffsets.start, (int)replay.TimeOffsets.end);
                    replay.Decorations.Add(new CircleDecoration(false, 0, 220, lifespan, "rgba(255, 150, 0, 0.5)", new AgentConnector(target)));
                    break;
                default:
                    break;
            }
        }
    }
}
