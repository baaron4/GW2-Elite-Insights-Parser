using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class Gorseval : SpiritVale
    {
        public Gorseval(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new PlayerDstHitMechanic(SpectralImpact, "Spectral Impact", new MechanicPlotlySetting(Symbols.Hexagram,Colors.Red), "Slam","Spectral Impact (KB Slam)", "Slam",4000).UsingChecker((de, log) => !de.To.HasBuff(log, Stability, de.Time - ParserHelper.ServerDelayConstant)),
            new PlayerDstBuffApplyMechanic(GhastlyPrison, "Ghastly Prison", new MechanicPlotlySetting(Symbols.Circle,Colors.LightOrange), "Egg","Ghastly Prison (Egged)", "Egged",500),
            new PlayerDstBuffApplyMechanic(SpectralDarkness, "Spectral Darkness", new MechanicPlotlySetting(Symbols.Circle,Colors.Blue), "Orb Debuff","Spectral Darkness (Stood in Orb AoE)", "Orb Debuff",100),
            new EnemyDstBuffApplyMechanic(SpiritedFusion, "Spirited Fusion", new MechanicPlotlySetting(Symbols.Square,Colors.LightOrange), "Spirit Buff","Spirited Fusion (Consumed a Spirit)", "Ate Spirit",0),
            new PlayerDstHitMechanic(SpiritKick, "Kick", new MechanicPlotlySetting(Symbols.TriangleRight,Colors.Magenta), "Kick","Kicked by small add", "Spirit Kick",0).UsingChecker((de, log) => !de.To.HasBuff(log, Stability, de.Time - ParserHelper.ServerDelayConstant)),
            new PlayerDstBuffApplyMechanic(Vulnerability, "Ghastly Rampage Black Goo Hit", new MechanicPlotlySetting(Symbols.Circle,Colors.Black), "Black","Hit by Black Goo","Black Goo",3000).UsingChecker( (ba,log) => ba.AppliedDuration == 10000),
            new EnemyCastStartMechanic(GhastlyRampage, "Ghastly Rampage", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkTeal), "CC","Ghastly Rampage (Breakbar)", "Breakbar",0),
            new EnemyCastEndMechanic(GhastlyRampage, "Ghastly Rampage", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Red), "CC End","Ghastly Rampage (Full duration)", "CC ran out",0).UsingChecker( (ce,log) => ce.ActualDuration > 21985),
            new EnemyCastEndMechanic(GhastlyRampage, "Ghastly Rampage", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkGreen), "CCed","Ghastly Rampage (Breakbar broken)", "CCed",0).UsingChecker((ce, log) => ce.ActualDuration <= 21985),
            });
            Extension = "gors";
            Icon = EncounterIconGorseval;
            EncounterCategoryInformation.InSubCategoryOrder = 1;
            EncounterID |= 0x000002;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap(CombatReplayGorseval,
                            (957, 1000),
                            (-653, -6754, 3701, -2206)/*,
                            (-15360, -36864, 15360, 39936),
                            (3456, 11012, 4736, 14212)*/);
        }
        internal override List<InstantCastFinder> GetInstantCastFinders()
        {
            return new List<InstantCastFinder>()
            {
                new DamageCastFinder(HauntingAura, HauntingAura),
            };
        }
        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Gorseval));
            if (mainTarget == null)
            {
                throw new MissingKeyActorsException("Gorseval not found");
            }
            phases[0].AddTarget(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            phases.AddRange(GetPhasesByInvul(log, ProtectiveShadow, mainTarget, true, true));
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
                    AddTargetsToPhaseAndFit(phase, ids, log);
                }
            }
            return phases;
        }

        protected override List<int> GetTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.Gorseval,
                (int)ArcDPSEnums.TrashID.ChargedSoul
            };
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.EnragedSpirit,
                ArcDPSEnums.TrashID.AngeredSpirit
            };
        }

        internal override void ComputePlayerCombatReplayActors(AbstractPlayer p, ParsedEvtcLog log, CombatReplay replay)
        {
            base.ComputePlayerCombatReplayActors(p, log, replay);
            // Ghastly Prison - Eggs AoEs
            var eggs = p.GetBuffStatus(log, GhastlyPrison, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0).ToList();
            foreach (Segment seg in eggs)
            {
                replay.Decorations.Add(new CircleDecoration(180, seg, "rgba(255, 160, 0, 0.3)", new AgentConnector(p)));
            }

            // Spectral Darkness - Orbs Debuff Overhead
            IEnumerable<Segment> spectralDarknesses = p.GetBuffStatus(log, SpectralDarkness, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0);
            replay.AddOverheadIcons(spectralDarknesses, p, ParserIcons.SpectralDarknessOverhead);
        }

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            IReadOnlyList<AbstractCastEvent> cls = target.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd);
            switch (target.ID)
            {
                case (int)ArcDPSEnums.TargetID.Gorseval:
                    var blooms = cls.Where(x => x.SkillId == GorsevalBloom).ToList();
                    foreach (AbstractCastEvent c in blooms)
                    {
                        int start = (int)c.Time;
                        int end = (int)c.EndTime;
                        replay.AddDecorationWithFilledWithGrowing(new CircleDecoration(600, (start, end), "rgba(255, 125, 0, 0.5)", new AgentConnector(target)).UsingFilled(false), true, c.ExpectedDuration + start);
                    }
                    IReadOnlyList<PhaseData> phases = log.FightData.GetPhases(log);
                    if (phases.Count > 1)
                    {
                        var rampage = cls.Where(x => x.SkillId == GhastlyRampage).ToList();
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
                            replay.Decorations.Add(new CircleDecoration(180, (start, end), "rgba(0, 125, 255, 0.3)", new AgentConnector(target)));
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
                                byte pattern = patterns[i];
                                var connector = new PositionConnector(pos);
                                var color = "rgba(25,25,112, 0.25)";
                                //
                                var nonFullDecorations = new List<FormDecoration>();
                                int tickStartNonFull = start + 4000 * i;
                                int explosionNonFull = tickStartNonFull + 3000;
                                int tickEndNonFull = tickStartNonFull + 3500;
                                (int, int) lifespanRampageNonFull = (tickStartNonFull, tickEndNonFull);
                                if ((pattern & first) > 0)
                                {
                                    nonFullDecorations.Add(new CircleDecoration(360, lifespanRampageNonFull, color, connector));
                                }
                                if ((pattern & second) > 0)
                                {
                                    nonFullDecorations.Add(new DoughnutDecoration(360, 720, lifespanRampageNonFull, color, connector));
                                }
                                if ((pattern & third) > 0)
                                {
                                    nonFullDecorations.Add(new DoughnutDecoration(720, 1080, lifespanRampageNonFull, color, connector));
                                }
                                if ((pattern & fourth) > 0)
                                {
                                    nonFullDecorations.Add(new DoughnutDecoration(1080, 1440, lifespanRampageNonFull, color, connector));
                                }
                                if ((pattern & fifth) > 0)
                                {
                                    nonFullDecorations.Add(new DoughnutDecoration(1440, 1800, lifespanRampageNonFull, color, connector));
                                }
                                foreach (FormDecoration decoration in nonFullDecorations)
                                {
                                    replay.AddDecorationWithGrowing(decoration, explosionNonFull);
                                }
                                // Full a different timings
                                if ((pattern & full) > 0)
                                {
                                    (int, int) fullLifespanRampage = (tickStartNonFull - 1000, tickEndNonFull - 1000);
                                    int fullExplosion = explosionNonFull - 1000;
                                    replay.AddDecorationWithGrowing(new CircleDecoration(1800, fullLifespanRampage, color, connector), fullExplosion);
                                }
                            }
                        }
                    }
                    var slam = cls.Where(x => x.SkillId == SpectralImpact).ToList();
                    foreach (AbstractCastEvent c in slam)
                    {
                        int start = (int)c.Time;
                        int impactPoint = 1185;
                        int impactTime = start + impactPoint;
                        int end = Math.Min((int)c.EndTime, impactTime);
                        int radius = 320;
                        replay.Decorations.Add(new CircleDecoration(radius, (start, end), "rgba(255, 0, 0, 0.2)", new AgentConnector(target)));
                        replay.Decorations.Add(new CircleDecoration(radius, (impactTime, impactTime + 100), "rgba(255, 0, 0, 0.4)", new AgentConnector(target)));
                    }
                    var protection = target.GetBuffStatus(log, ProtectiveShadow, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0).ToList();
                    foreach (Segment seg in protection)
                    {
                        replay.Decorations.Add(new CircleDecoration(300, seg, "rgba(0, 180, 255, 0.5)", new AgentConnector(target)));
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.ChargedSoul:
                    var lifespan = ((int)replay.TimeOffsets.start, (int)replay.TimeOffsets.end);
                    replay.Decorations.Add(new CircleDecoration(220, lifespan, "rgba(255, 150, 0, 0.5)", new AgentConnector(target)).UsingFilled(false));
                    break;
                default:
                    break;
            }
        }
    }
}
