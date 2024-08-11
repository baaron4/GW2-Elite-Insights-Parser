using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class Skorvald : ShatteredObservatory
    {
        public Skorvald(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new PlayerDstHitMechanic(new long[]{ CombustionRush1, CombustionRush2, CombustionRush3 }, "Combustion Rush", new MechanicPlotlySetting(Symbols.TriangleLeft,Colors.Magenta), "Charge","Combustion Rush", "Charge",0),
            new PlayerDstHitMechanic(new long[] { PunishingKickAnomaly, PunishingKickSkorvald }, "Punishing Kick", new MechanicPlotlySetting(Symbols.TriangleRightOpen,Colors.Magenta), "Add Kick","Punishing Kick (Single purple Line, Add)", "Kick (Add)",0),
            new PlayerDstHitMechanic(new long[] { CranialCascadeAnomaly,CranialCascade2 }, "Cranial Cascade", new MechanicPlotlySetting(Symbols.TriangleRightOpen,Colors.Yellow), "Add Cone KB","Cranial Cascade (3 purple Line Knockback, Add)", "Small Cone KB (Add)",0),
            new PlayerDstHitMechanic(new long[] { RadiantFurySkorvald, RadiantFury2 }, "Radiant Fury", new MechanicPlotlySetting(Symbols.Octagon,Colors.Red), "Burn Circle","Radiant Fury (expanding burn circles)", "Expanding Circles",0),
            new PlayerDstHitMechanic(FocusedAnger, "Focused Anger", new MechanicPlotlySetting(Symbols.TriangleDown,Colors.Orange), "Large Cone KB","Focused Anger (Large Cone Overhead Crosshair Knockback)", "Large Cone Knockback",0),
            new PlayerDstHitMechanic(new long[] { HorizonStrikeSkorvald1, HorizonStrikeSkorvald2 }, "Horizon Strike", new MechanicPlotlySetting(Symbols.Circle,Colors.LightOrange), "Horizon Strike","Horizon Strike (turning pizza slices)", "Horizon Strike",0), // 
            new PlayerDstHitMechanic(CrimsonDawn, "Crimson Dawn", new MechanicPlotlySetting(Symbols.Circle,Colors.DarkRed), "Horizon Strike End","Crimson Dawn (almost Full platform attack after Horizon Strike)", "Horizon Strike (last)",0),
            new PlayerDstHitMechanic(SolarCyclone, "Solar Cyclone", new MechanicPlotlySetting(Symbols.BowtieOpen,Colors.DarkMagenta), "Cyclone","Solar Cyclone (Circling Knockback)", "KB Cyclone",0),
            new PlayerDstBuffApplyMechanic(Fear, "Fear", new MechanicPlotlySetting(Symbols.SquareOpen,Colors.Red), "Eye","Hit by the Overhead Eye Fear", "Eye (Fear)",0).UsingChecker((ba, log) => ba.AppliedDuration == 3000), //not triggered under stab, still get blinded/damaged, seperate tracking desired?
            new PlayerDstBuffApplyMechanic(FixatedBloom1, "Fixate", new MechanicPlotlySetting(Symbols.StarOpen,Colors.Magenta), "Bloom Fix","Fixated by Solar Bloom", "Bloom Fixate",0),
            new PlayerDstHitMechanic(BloomExplode, "Explode", new MechanicPlotlySetting(Symbols.Circle,Colors.Yellow), "Bloom Expl","Hit by Solar Bloom Explosion", "Bloom Explosion",0), //shockwave, not damage? (damage is 50% max HP, not tracked)
            new PlayerDstHitMechanic(SpiralStrike, "Spiral Strike", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.DarkGreen), "Spiral","Hit after Warp (Jump to Player with overhead bomb)", "Spiral Strike",0),
            new PlayerDstHitMechanic(WaveOfMutilation, "Wave of Mutilation", new MechanicPlotlySetting(Symbols.TriangleSW,Colors.DarkGreen), "KB Jump","Hit by KB Jump (player targeted)", "Knockback jump",0),
            new PlayerDstBuffApplyMechanic(SkorvaldsIre, "Skorvald Fixate", new MechanicPlotlySetting(Symbols.CircleOpenDot, Colors.Purple), "Skor Fixate", "Fixated by Skorvald's Ire",  "Skorvald's Fixate", 0),
            });
            Extension = "skorv";
            Icon = EncounterIconSkorvald;
            EncounterCategoryInformation.InSubCategoryOrder = 0;
            EncounterID |= 0x000001;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap(CombatReplaySkorvald,
                            (987, 1000),
                            (-22267, 14955, -17227, 20735)/*,
                            (-24576, -24576, 24576, 24576),
                            (11204, 4414, 13252, 6462)*/);
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            // generic method for fractals
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor skorvald = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Skorvald)) ?? throw new MissingKeyActorsException("Skorvald not found");
            phases[0].AddTarget(skorvald);
            if (!requirePhases)
            {
                return phases;
            }
            phases.AddRange(GetPhasesByInvul(log, Determined762, skorvald, true, true));
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                if (i % 2 == 0)
                {
                    phase.Name = "Split " + (i) / 2;
                    var ids = new List<int>
                    {
                        (int)TrashID.FluxAnomaly1,
                        (int)TrashID.FluxAnomaly2,
                        (int)TrashID.FluxAnomaly3,
                        (int)TrashID.FluxAnomaly4,
                        (int)TrashID.FluxAnomalyCM1,
                        (int)TrashID.FluxAnomalyCM2,
                        (int)TrashID.FluxAnomalyCM3,
                        (int)TrashID.FluxAnomalyCM4,
                    };
                    AddTargetsToPhaseAndFit(phase, ids, log);
                }
                else
                {
                    phase.Name = "Phase " + (i + 1) / 2;
                    phase.AddTarget(skorvald);
                }
            }
            return phases;
        }

        internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            var manualFractalScaleSet = false;
            if (!combatData.Any(x => x.IsStateChange == StateChange.FractalScale))
            {
                manualFractalScaleSet = true;
            }
            var fluxAnomalies = new List<AgentItem>();
            var fluxIds = new List<int>
                    {
                        (int)TrashID.FluxAnomaly1,
                        (int)TrashID.FluxAnomaly2,
                        (int)TrashID.FluxAnomaly3,
                        (int)TrashID.FluxAnomaly4,
                        (int)TrashID.FluxAnomalyCM1,
                        (int)TrashID.FluxAnomalyCM2,
                        (int)TrashID.FluxAnomalyCM3,
                        (int)TrashID.FluxAnomalyCM4,
                    };
            for (int i = 0; i < fluxIds.Count; i++)
            {
                fluxAnomalies.AddRange(agentData.GetNPCsByID(fluxIds[i]));
            }
            var refresh = false;
            foreach (AgentItem fluxAnomaly in fluxAnomalies)
            {
                if (combatData.Any(x => x.SkillID == Determined762 && x.IsBuffApply() && x.DstMatchesAgent(fluxAnomaly)))
                {
                    refresh = true;
                    fluxAnomaly.OverrideID(TrashID.UnknownAnomaly);
                }
            }
            if (refresh)
            {
                agentData.Refresh();
            }
            base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
            AbstractSingleActor skorvald = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Skorvald)) ?? throw new MissingKeyActorsException("Skorvald not found");
            skorvald.OverrideName("Skorvald");
            if (manualFractalScaleSet && combatData.Any(x => x.IsStateChange == StateChange.MaxHealthUpdate && x.SrcMatchesAgent(skorvald.AgentItem) && MaxHealthUpdateEvent.GetMaxHealth(x) < 5e6 && MaxHealthUpdateEvent.GetMaxHealth(x) > 0))
            {
                // Remove manual scale from T1 to T3 for now
                combatData.FirstOrDefault(x => x.IsStateChange == StateChange.FractalScale).OverrideSrcAgent(0);
                // Once we have the hp thresholds, simply apply -75, -50, -25 to the srcAgent of existing event
            }

            int[] nameCount = new[] { 0, 0, 0, 0 };
            foreach (NPC target in _targets)
            {
                switch (target.ID)
                {
                    case (int)TrashID.FluxAnomaly1:
                    case (int)TrashID.FluxAnomalyCM1:
                        target.OverrideName(target.Character + " " + (1 + 4 * nameCount[0]++));
                        break;
                    case (int)TrashID.FluxAnomaly2:
                    case (int)TrashID.FluxAnomalyCM2:
                        target.OverrideName(target.Character + " " + (2 + 4 * nameCount[1]++));
                        break;
                    case (int)TrashID.FluxAnomaly3:
                    case (int)TrashID.FluxAnomalyCM3:
                        target.OverrideName(target.Character + " " + (3 + 4 * nameCount[2]++));
                        break;
                    case (int)TrashID.FluxAnomaly4:
                    case (int)TrashID.FluxAnomalyCM4:
                        target.OverrideName(target.Character + " " + (4 + 4 * nameCount[3]++));
                        break;
                }
            }
        }

        internal override long GetFightOffset(EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            CombatItem logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.LogNPCUpdate);
            if (logStartNPCUpdate != null)
            {
                AgentItem skorvald = agentData.GetNPCsByID(TargetID.Skorvald).FirstOrDefault() ?? throw new MissingKeyActorsException("Skorvald not found");
                long upperLimit = GetPostLogStartNPCUpdateDamageEventTime(fightData, agentData, combatData, logStartNPCUpdate.Time, skorvald);
                // Skorvald may spawns with 0% hp
                CombatItem firstNonZeroHPUpdate = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.HealthUpdate && x.SrcMatchesAgent(skorvald) && HealthUpdateEvent.GetHealthPercent(x) > 0);
                CombatItem enterCombat = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.EnterCombat && x.SrcMatchesAgent(skorvald) && x.Time <= upperLimit + ServerDelayConstant);
                return firstNonZeroHPUpdate != null ? Math.Min(firstNonZeroHPUpdate.Time, enterCombat != null ? enterCombat.Time : long.MaxValue) : GetGenericFightOffset(fightData);
            }
            return GetGenericFightOffset(fightData);
        }

        internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
        {
            AbstractSingleActor target = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Skorvald)) ?? throw new MissingKeyActorsException("Skorvald not found");
            if (combatData.GetGW2BuildEvent().Build >= GW2Builds.September2020SunquaPeakRelease)
            {
                // Agent check not reliable, produces false positives and regular false negatives
                /*if (agentData.GetNPCsByID(16725).Any() && agentData.GetNPCsByID(11245).Any())
                {
                    return FightData.CMStatus.CM;
                }*/
                // Check some CM skills instead, not perfect but helps, 
                // Solar Bolt is the first thing he tries to cast, that looks very consistent
                // If the phase 1 is super fast to the point skorvald does not cast anything, supernova should be there
                // Otherwise we are looking at a super fast phase 1 (< 7 secondes) where the team ggs just before supernova
                // Joining the encounter mid fight may also yield a false negative but at that point the log is incomplete already
                // WARNING: Skorvald seems to cast SupernovaCM on T4 regardless of the mode since an unknown amount of time, removing that id check
                // and adding split thrash mob check
                var cmSkills = new HashSet<long>
                {
                    SolarBoltCM,
                    //SupernovaCM,
                };
                if (combatData.GetSkills().Intersect(cmSkills).Any() ||
                    agentData.GetNPCsByID(TrashID.FluxAnomalyCM1).Any(x => x.FirstAware >= target.FirstAware) ||
                    agentData.GetNPCsByID(TrashID.FluxAnomalyCM2).Any(x => x.FirstAware >= target.FirstAware) ||
                    agentData.GetNPCsByID(TrashID.FluxAnomalyCM3).Any(x => x.FirstAware >= target.FirstAware) ||
                    agentData.GetNPCsByID(TrashID.FluxAnomalyCM4).Any(x => x.FirstAware >= target.FirstAware))
                {
                    return FightData.EncounterMode.CM;
                }
                return FightData.EncounterMode.Normal;
            }
            else
            {
                return (target.GetHealth(combatData) == 5551340) ? FightData.EncounterMode.CM : FightData.EncounterMode.Normal;
            }
        }

        protected override List<int> GetTargetsIDs()
        {
            return new List<int>()
            {
                (int)TargetID.Skorvald,
                (int)TrashID.FluxAnomaly1,
                (int)TrashID.FluxAnomaly2,
                (int)TrashID.FluxAnomaly3,
                (int)TrashID.FluxAnomaly4,
                (int)TrashID.FluxAnomalyCM1,
                (int)TrashID.FluxAnomalyCM2,
                (int)TrashID.FluxAnomalyCM3,
                (int)TrashID.FluxAnomalyCM4,
            };
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            base.CheckSuccess(combatData, agentData, fightData, playerAgents);
            // reward or death worked
            if (fightData.Success)
            {
                return;
            }
            AbstractSingleActor skorvald = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Skorvald)) ?? throw new MissingKeyActorsException("Skorvald not found");
            AbstractHealthDamageEvent lastDamageTaken = combatData.GetDamageTakenData(skorvald.AgentItem).LastOrDefault(x => (x.HealthDamage > 0) && playerAgents.Contains(x.From.GetFinalMaster()));
            if (lastDamageTaken != null)
            {
                BuffApplyEvent invul895Apply = combatData.GetBuffDataByIDByDst(Determined895, skorvald.AgentItem).OfType<BuffApplyEvent>().Where(x => x.Time > lastDamageTaken.Time - 500).LastOrDefault();
                if (invul895Apply != null)
                {
                    fightData.SetSuccess(true, Math.Min(invul895Apply.Time, lastDamageTaken.Time));
                }
            }
        }

        protected override List<TrashID> GetTrashMobsIDs()
        {
            var trashIDs = new List<TrashID>
            {
                TrashID.SolarBloom
            };
            trashIDs.AddRange(base.GetTrashMobsIDs());
            return trashIDs;
        }

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            IReadOnlyList<AbstractCastEvent> casts = target.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd);

            switch (target.ID)
            {
                case (int)TargetID.Skorvald:
                    // Horizon Strike
                    var horizonStrike = casts.Where(x => x.SkillId == HorizonStrikeSkorvald2 || x.SkillId == HorizonStrikeSkorvald4).ToList();
                    foreach (AbstractCastEvent c in horizonStrike)
                    {
                        int castDuration = 3900;
                        int shiftingAngle = 45;
                        int sliceSpawnInterval = 750;
                        (long start, long end) lifespan = (c.Time + 100, ComputeEndCastTimeByBuffApplication(log, target, Stun, c.Time, castDuration));
                        lifespan.end = Math.Min(lifespan.end, ComputeEndCastTimeByBuffApplication(log, target, Determined762, c.Time, castDuration));

                        Point3D facingDirection = target.GetCurrentRotation(log, c.Time + 100, castDuration);
                        if (facingDirection != null)
                        {
                            float degree = Point3D.GetZRotationFromFacing(facingDirection);

                            // Horizon Strike starting at Skorvald's facing point
                            if (c.SkillId == HorizonStrikeSkorvald4)
                            {
                                for (int i = 0; i < 4; i++)
                                {
                                    AddHorizonStrikeDecoration(replay, target, lifespan, degree);
                                    lifespan.start += sliceSpawnInterval;
                                    lifespan.end += sliceSpawnInterval;
                                    degree -= shiftingAngle;
                                }
                            }
                            // Starting at Skorvald's 90° of facing point
                            if (c.SkillId == HorizonStrikeSkorvald2)
                            {
                                degree -= 90;
                                for (int i = 0; i < 4; i++)
                                {
                                    AddHorizonStrikeDecoration(replay, target, lifespan, degree);
                                    lifespan.start += sliceSpawnInterval;
                                    lifespan.end += sliceSpawnInterval;
                                    degree += shiftingAngle;
                                }
                            }
                        }
                    }

                    // Crimson Dawn
                    IReadOnlyList<long> skillIds = new List<long>() { CrimsonDawnSkorvaldCM1, CrimsonDawnSkorvaldCM2, CrimsonDawnSkorvaldCM3, CrimsonDawnSkorvaldCM4 };
                    var crimsonDawn = casts.Where(x => skillIds.Contains(x.SkillId)).ToList();
                    foreach (AbstractCastEvent c in crimsonDawn)
                    {
                        uint radius = 1200;
                        int angle = 295;
                        int castDuration = 3000;
                        (long start, long end) lifespan = (c.Time + 100, ComputeEndCastTimeByBuffApplication(log, target, Stun, c.Time, castDuration));
                        lifespan.end = Math.Min(lifespan.end, ComputeEndCastTimeByBuffApplication(log, target, Determined762, c.Time, castDuration));

                        Point3D facingDirection = target.GetCurrentRotation(log, c.Time + 100, castDuration);
                        if (facingDirection != null)
                        {
                            float degree = Point3D.GetZRotationFromFacing(facingDirection);

                            if (c.SkillId == CrimsonDawnSkorvaldCM2)
                            {
                                degree += 90;
                            }
                            if (c.SkillId == CrimsonDawnSkorvaldCM1)
                            {
                                degree += 270;
                            }
                            var connector = new AgentConnector(target);
                            var rotationConnector = new AngleConnector(degree);
                            replay.Decorations.Add(new PieDecoration(radius, angle, lifespan, Colors.Orange, 0.2, connector).UsingRotationConnector(rotationConnector));
                            replay.Decorations.Add(new PieDecoration(radius, angle, (lifespan.end, lifespan.end + 500), Colors.Red, 0.2, connector).UsingRotationConnector(rotationConnector));
                        }
                    }

                    // Punishing Kick
                    var punishingKick = casts.Where(x => x.SkillId == PunishingKickSkorvald).ToList();
                    foreach (AbstractCastEvent c in punishingKick)
                    {
                        int castDuration = 1850;
                        (long start, long end) lifespan = (c.Time + 100, ComputeEndCastTimeByBuffApplication(log, target, Stun, c.Time, castDuration));
                        lifespan.end = Math.Min(lifespan.end, ComputeEndCastTimeByBuffApplication(log, target, Determined762, c.Time, castDuration));
                        long expectedEndCast = c.Time + castDuration;

                        Point3D frontalPoint = target.GetCurrentRotation(log, c.Time + 100, castDuration);
                        if (frontalPoint != null)
                        {
                            float rotation = Point3D.GetZRotationFromFacing(frontalPoint);
                            // Frontal
                            AddKickIndicatorDecoration(replay, target, lifespan, expectedEndCast, rotation);
                        }
                    }

                    // Radiant Fury
                    var radiantFury = casts.Where(x => x.SkillId == RadiantFurySkorvald).ToList();
                    foreach (AbstractCastEvent c in radiantFury)
                    {
                        int duration = 2700;
                        long expectedEndCast = c.Time + duration;
                        (long start, long end) lifespan = (c.Time, ComputeEndCastTimeByBuffApplication(log, target, Stun, c.Time, duration));
                        lifespan.end = Math.Min(lifespan.end, ComputeEndCastTimeByBuffApplication(log, target, Determined762, c.Time, duration));
                        (long start, long end) lifespanWave = (lifespan.end, lifespan.end + 900);

                        if (expectedEndCast <= lifespan.end)
                        {
                            GeographicalConnector connector = new AgentConnector(target);
                            replay.AddShockwave(connector, lifespanWave, Colors.Red, 0.6, 1200);
                        }
                    }

                    // Supernova - Phase Oneshot
                    var supernova = casts.Where(x => x.SkillId == SupernovaSkorvaldCM).ToList();
                    foreach (AbstractCastEvent c in supernova)
                    {
                        int duration = 75000;
                        long expectedEndCast = c.Time + duration;
                        (long start, long end) lifespan = (c.Time, ComputeEndCastTimeByBuffApplication(log, target, Stun, c.Time, duration));
                        replay.AddDecorationWithGrowing(new CircleDecoration(1200, lifespan, Colors.Red, 0.2, new AgentConnector(target)), expectedEndCast);
                    }

                    // Cranial Cascade
                    var cranialCascadeSkorvald = casts.Where(x => x.SkillId == CranialCascadeSkorvald).ToList();
                    foreach (AbstractCastEvent c in cranialCascadeSkorvald)
                    {
                        int castDuration = 1750;
                        int angle = 35;
                        long expectedEndCast = c.Time + castDuration;

                        (long start, long end) lifespan = (c.Time, ComputeEndCastTimeByBuffApplication(log, target, Stun, c.Time, castDuration));
                        lifespan.end = Math.Min(lifespan.end, ComputeEndCastTimeByBuffApplication(log, target, Determined762, c.Time, castDuration));

                        Point3D frontalPoint = target.GetCurrentRotation(log, c.Time + 100, castDuration);
                        if (frontalPoint != null)
                        {
                            float rotation = Point3D.GetZRotationFromFacing(frontalPoint);

                            // Frontal
                            AddKickIndicatorDecoration(replay, target, lifespan, expectedEndCast, rotation);
                            // Left
                            AddKickIndicatorDecoration(replay, target, lifespan, expectedEndCast, rotation - angle);
                            // Right
                            AddKickIndicatorDecoration(replay, target, lifespan, expectedEndCast, rotation + angle);
                        }
                    }
                    break;
                case (int)TrashID.FluxAnomalyCM1:
                case (int)TrashID.FluxAnomalyCM2:
                case (int)TrashID.FluxAnomalyCM3:
                case (int)TrashID.FluxAnomalyCM4:
                    // Solar Stomp
                    var solarStomp = casts.Where(x => x.SkillId == SolarStomp).ToList();
                    foreach (AbstractCastEvent c in solarStomp)
                    {
                        uint radius = 280;
                        int castDuration = 2250;
                        (long start, long end) lifespan = (c.Time, c.Time + castDuration);
                        (long start, long end) lifespanShockwave = (lifespan.end, lifespan.end + castDuration);

                        // Stomp
                        GeographicalConnector connector = new AgentConnector(target);
                        replay.AddDecorationWithGrowing(new CircleDecoration(radius, lifespan, Colors.LightOrange, 0.2, connector), lifespan.end);
                        replay.AddShockwave(connector, lifespanShockwave, Colors.Red, 0.6, 1200);
                    }

                    // Punishing Kick
                    var punishingKickAnomaly = casts.Where(x => x.SkillId == PunishingKickAnomaly).ToList();
                    foreach (AbstractCastEvent c in punishingKickAnomaly)
                    {
                        int castDuration = 1850;
                        long expectedEndCast = c.Time + castDuration;
                        (long start, long end) lifespan = (c.Time, expectedEndCast);

                        Point3D frontalPoint = target.GetCurrentRotation(log, c.Time + 100, castDuration);
                        if (frontalPoint != null)
                        {
                            float rotation = Point3D.GetZRotationFromFacing(frontalPoint);
                            // Frontal
                            AddKickIndicatorDecoration(replay, target, lifespan, expectedEndCast, rotation);
                        }
                    }

                    // Cranial Cascade
                    var cranialCascadeAnomaly = casts.Where(x => x.SkillId == CranialCascadeAnomaly).ToList();
                    foreach (AbstractCastEvent c in cranialCascadeAnomaly)
                    {
                        int castDuration = 1750;
                        int angle = 35;
                        long expectedEndCast = c.Time + castDuration;
                        (long start, long end) lifespan = (c.Time, expectedEndCast);

                        Point3D frontalPoint = target.GetCurrentRotation(log, c.Time + 100, castDuration);
                        if (frontalPoint != null)
                        {
                            float rotation = Point3D.GetZRotationFromFacing(frontalPoint);

                            // Left
                            AddKickIndicatorDecoration(replay, target, lifespan, expectedEndCast, rotation - angle);
                            // Right
                            AddKickIndicatorDecoration(replay, target, lifespan, expectedEndCast, rotation + angle);
                        }
                    }

                    // Mist Smash
                    var mistSmash = casts.Where(x => x.SkillId == MistSmash).ToList();
                    foreach (AbstractCastEvent c in mistSmash)
                    {
                        int castDuration = 1933;
                        (long start, long end) lifespan = (c.Time, c.Time + castDuration);
                        (long start, long end) lifespanShockwave = (lifespan.end, lifespan.end + 2250);
                        replay.AddDecorationWithGrowing(new CircleDecoration(160, lifespan, Colors.Orange, 0.2, new AgentConnector(target)), lifespan.end);
                        // Nightmare Discharge Shockwave
                        GeographicalConnector connector = new AgentConnector(target);
                        replay.AddShockwave(connector, lifespanShockwave, Colors.Yellow, 0.3, 1200);
                    }

                    // Wave of Mutilation
                    var waveOfMutilation = casts.Where(x => x.SkillId == WaveOfMutilation).ToList();
                    foreach (AbstractCastEvent c in waveOfMutilation)
                    {
                        int castDuration = 1850;
                        int angle = 18;
                        long expectedEndCast = c.Time + castDuration;
                        (long start, long end) lifespan = (c.Time, expectedEndCast);

                        Point3D frontalPoint = target.GetCurrentRotation(log, c.Time + 100, castDuration);
                        if (frontalPoint != null)
                        {
                            float rotation = Point3D.GetZRotationFromFacing(frontalPoint);

                            float startingDegree = rotation - angle * 2;
                            for (int i = 0; i < 5; i++)
                            {
                                AddKickIndicatorDecoration(replay, target, lifespan, expectedEndCast, startingDegree);
                                startingDegree += angle;
                            }
                        }
                    }
                    break;
                case (int)TrashID.SolarBloom:
                    break;
                default:
                    break;
            }
        }

        internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log)
        {
            base.ComputeEnvironmentCombatReplayDecorations(log);

            // Mist Bomb - Both for Skorvald and Flux Anomalies
            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.MistBomb, out IReadOnlyList<EffectEvent> mistBombs))
            {
                foreach (EffectEvent effect in mistBombs)
                {
                    (long start, long end) lifespan = effect.ComputeLifespan(log, 1000);
                    EnvironmentDecorations.Add(new CircleDecoration(130, lifespan, Colors.Orange, 0.2, new PositionConnector(effect.Position)));
                }
            }

            // Solar Bolt - Indicator
            AddDistanceCorrectedOrbDecorations(log, EnvironmentDecorations, EffectGUIDs.SolarBoltIndicators, TargetID.Skorvald, 310, 1800, 1300);

            // Solar Bolt - Damage
            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.SkorvaldSolarBoltDamage, out IReadOnlyList<EffectEvent> solarBolts))
            {
                foreach (EffectEvent effect in solarBolts)
                {
                    (long start, long end) lifespan = effect.ComputeLifespan(log, 12000);
                    EnvironmentDecorations.Add(new CircleDecoration(100, lifespan, Colors.Red, 0.2, new PositionConnector(effect.Position)));
                }
            }

            // Solar Cyclone
            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.KickGroundEffect, out IReadOnlyList<EffectEvent> kickEffects))
            {
                foreach (EffectEvent effect in kickEffects)
                {
                    (long start, long end) lifespan = (effect.Time, effect.Time + 300);
                    EnvironmentDecorations.Add(new RectangleDecoration(300, 180, lifespan, Colors.Red, 0.2, new PositionConnector(effect.Position)).UsingRotationConnector(new AngleConnector(effect.Rotation.Z - 90)));
                }
            }
        }

        internal override void ComputePlayerCombatReplayActors(AbstractPlayer p, ParsedEvtcLog log, CombatReplay replay)
        {
            base.ComputePlayerCombatReplayActors(p, log, replay);
            // Fixations
            IEnumerable<Segment> fixations = p.GetBuffStatus(log, new long[] { FixatedBloom1, SkorvaldsIre }, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0);
            replay.AddOverheadIcons(fixations, p, ParserIcons.FixationPurpleOverhead);
        }

        /// <summary>
        /// Add Horizon Strike decoration.
        /// </summary>
        /// <param name="replay">Combat Replay.</param>
        /// <param name="target">Actor.</param>
        /// <param name="lifespan">Start and End of cast.</param>
        /// <param name="degree">Degree of the strike.</param>
        private static void AddHorizonStrikeDecoration(CombatReplay replay, AbstractSingleActor target, (long start, long end) lifespan, float degree)
        {
            var connector = new AgentConnector(target);
            var frontRotationConnector = new AngleConnector(degree);
            var flipRotationConnector = new AngleConnector(degree + 180);
            // Indicator
            var pieIndicator = new PieDecoration(1200, 70, lifespan, Colors.Orange, 0.2, connector);
            replay.Decorations.Add(pieIndicator.UsingRotationConnector(frontRotationConnector));
            replay.Decorations.Add(pieIndicator.Copy().UsingRotationConnector(flipRotationConnector));
            // Attack hit
            (long start, long end) lifespanHit = (lifespan.end, lifespan.end + 300);
            var pieHit = (PieDecoration)new PieDecoration(1200, 70, lifespanHit, Colors.Red, 0.2, connector).UsingGrowingEnd(lifespanHit.end);
            replay.Decorations.Add(pieHit.UsingRotationConnector(frontRotationConnector));
            replay.Decorations.Add(pieHit.Copy().UsingRotationConnector(flipRotationConnector));
        }

        /// <summary>
        /// Add Kick decoration.
        /// </summary>
        /// <param name="replay">Combat Replay.</param>
        /// <param name="target">Actor.</param>
        /// <param name="lifespan">Start and End of cast.</param>
        /// <param name="expectedEndCast">Expected end of the cast.</param>
        /// <param name="rotation">Rotation degree.</param>
        private static void AddKickIndicatorDecoration(CombatReplay replay, AbstractSingleActor target, (long start, long end) lifespan, long expectedEndCast, float rotation)
        {
            int translation = 150;
            var rotationConnector = new AngleConnector(rotation);
            var positionConnector = (AgentConnector)new AgentConnector(target).WithOffset(new Point3D(translation, 0), true);
            replay.AddDecorationWithGrowing((RectangleDecoration)new RectangleDecoration(300, target.HitboxWidth, lifespan, Colors.LightOrange, 0.2, positionConnector).UsingRotationConnector(rotationConnector), expectedEndCast);

            // Cascade count => 4
            for (int i = 0; i < 4; i++)
            {
                replay.Decorations.Add(new RectangleDecoration(300, target.HitboxWidth, (lifespan.end, lifespan.end + 300), Colors.Red, 0.2, positionConnector).UsingRotationConnector(rotationConnector));
                lifespan.end += 300;
                translation += 300;
            }
        }
    }
}
