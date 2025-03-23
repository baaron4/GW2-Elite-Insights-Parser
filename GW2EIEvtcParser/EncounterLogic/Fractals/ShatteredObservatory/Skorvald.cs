using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class Skorvald : ShatteredObservatory
{
    public Skorvald(int triggerID) : base(triggerID)
    {
        MechanicList.Add(new MechanicGroup(
        [
            new PlayerDstHitMechanic([CombustionRush1, CombustionRush2, CombustionRush3], "Combustion Rush", new MechanicPlotlySetting(Symbols.TriangleLeft,Colors.Magenta), "Charge","Combustion Rush", "Charge",0),
            new PlayerDstHitMechanic([PunishingKickAnomaly, PunishingKickSkorvald], "Punishing Kick", new MechanicPlotlySetting(Symbols.TriangleRightOpen,Colors.Magenta), "Add Kick","Punishing Kick (Single purple Line, Add)", "Kick (Add)",0),
            new PlayerDstHitMechanic([CranialCascadeAnomaly,CranialCascade2], "Cranial Cascade", new MechanicPlotlySetting(Symbols.TriangleRightOpen,Colors.Yellow), "Add Cone KB","Cranial Cascade (3 purple Line Knockback, Add)", "Small Cone KB (Add)",0),
            new PlayerDstHitMechanic([RadiantFurySkorvald, RadiantFury2], "Radiant Fury", new MechanicPlotlySetting(Symbols.Octagon,Colors.Red), "Burn Circle","Radiant Fury (expanding burn circles)", "Expanding Circles",0),
            new PlayerDstHitMechanic(FocusedAnger, "Focused Anger", new MechanicPlotlySetting(Symbols.TriangleDown,Colors.Orange), "Large Cone KB","Focused Anger (Large Cone Overhead Crosshair Knockback)", "Large Cone Knockback",0),
            new MechanicGroup(
                [
                    new PlayerDstHitMechanic([HorizonStrikeSkorvald1, HorizonStrikeSkorvald2], "Horizon Strike", new MechanicPlotlySetting(Symbols.Circle,Colors.LightOrange), "Horizon Strike","Horizon Strike (turning pizza slices)", "Horizon Strike",0), // 
                    new PlayerDstHitMechanic(CrimsonDawn, "Crimson Dawn", new MechanicPlotlySetting(Symbols.Circle,Colors.DarkRed), "Horizon Strike End","Crimson Dawn (almost Full platform attack after Horizon Strike)", "Horizon Strike (last)",0),
                ]
            ),
            new PlayerDstHitMechanic(SolarCyclone, "Solar Cyclone", new MechanicPlotlySetting(Symbols.BowtieOpen,Colors.DarkMagenta), "Cyclone","Solar Cyclone (Circling Knockback)", "KB Cyclone",0),
            new PlayerDstBuffApplyMechanic(Fear, "Fear", new MechanicPlotlySetting(Symbols.SquareOpen,Colors.Red), "Eye","Hit by the Overhead Eye Fear", "Eye (Fear)",0)
                .UsingChecker((ba, log) => ba.AppliedDuration == 3000), //not triggered under stab, still get blinded/damaged, seperate tracking desired?
            new MechanicGroup(
                [
                    new PlayerDstBuffApplyMechanic(SkorvaldsIre, "Skorvald Fixate", new MechanicPlotlySetting(Symbols.CircleOpenDot, Colors.Purple), "Skor Fixate", "Fixated by Skorvald's Ire",  "Skorvald's Fixate", 0),
                    new PlayerDstBuffApplyMechanic(FixatedBloom1, "Fixate", new MechanicPlotlySetting(Symbols.StarOpen,Colors.Magenta), "Bloom Fix","Fixated by Solar Bloom", "Bloom Fixate",0),
                ]
            ),
            new PlayerDstHitMechanic(BloomExplode, "Explode", new MechanicPlotlySetting(Symbols.Circle,Colors.Yellow), "Bloom Expl","Hit by Solar Bloom Explosion", "Bloom Explosion",0), //shockwave, not damage? (damage is 50% max HP, not tracked)
            new PlayerDstHitMechanic(SpiralStrike, "Spiral Strike", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.DarkGreen), "Spiral","Hit after Warp (Jump to Player with overhead bomb)", "Spiral Strike",0),
            new PlayerDstHitMechanic(WaveOfMutilation, "Wave of Mutilation", new MechanicPlotlySetting(Symbols.TriangleSW,Colors.DarkGreen), "KB Jump","Hit by KB Jump (player targeted)", "Knockback jump",0),
        ]));
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
        SingleActor skorvald = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Skorvald)) ?? throw new MissingKeyActorsException("Skorvald not found");
        phases[0].AddTarget(skorvald);
        var anomalyIds = new List<int>
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
        phases[0].AddTargets(Targets.Where(x => x.IsAnySpecies(anomalyIds)), PhaseData.TargetPriority.Blocking);
        if (!requirePhases)
        {
            return phases;
        }
        phases.AddRange(GetPhasesByInvul(log, Determined762, skorvald, true, true));
        for (int i = 1; i < phases.Count; i++)
        {
            PhaseData phase = phases[i];
            phase.AddParentPhase(phases[0]);
            if (i % 2 == 0)
            {
                phase.Name = "Split " + (i) / 2;
                AddTargetsToPhaseAndFit(phase, anomalyIds, log);
            }
            else
            {
                phase.Name = "Phase " + (i + 1) / 2;
                phase.AddTarget(skorvald);
            }
        }
        return phases;
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
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
        foreach (AgentItem fluxAnomaly in fluxAnomalies)
        {
            if (combatData.Any(x => x.SkillID == Determined762 && x.IsBuffApply() && x.DstMatchesAgent(fluxAnomaly)))
            {
                fluxAnomaly.OverrideID(TrashID.UnknownAnomaly, agentData);
            }
        }
        base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
        SingleActor skorvald = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Skorvald)) ?? throw new MissingKeyActorsException("Skorvald not found");
        skorvald.OverrideName("Skorvald");
        if (manualFractalScaleSet && combatData.Any(x => x.IsStateChange == StateChange.MaxHealthUpdate && x.SrcMatchesAgent(skorvald.AgentItem) && MaxHealthUpdateEvent.GetMaxHealth(x) < 5e6 && MaxHealthUpdateEvent.GetMaxHealth(x) > 0))
        {
            // Remove manual scale from T1 to T3 for now
            combatData.FirstOrDefault(x => x.IsStateChange == StateChange.FractalScale)!.OverrideSrcAgent(0);
            // Once we have the hp thresholds, simply apply -75, -50, -25 to the srcAgent of existing event
        }

        int[] nameCount = [0, 0, 0, 0];
        foreach (NPC target in Targets)
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
        CombatItem? logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.LogNPCUpdate);
        if (logStartNPCUpdate != null)
        {
            AgentItem skorvald = agentData.GetNPCsByID(TargetID.Skorvald).FirstOrDefault() ?? throw new MissingKeyActorsException("Skorvald not found");
            long upperLimit = GetPostLogStartNPCUpdateDamageEventTime(fightData, agentData, combatData, logStartNPCUpdate.Time, skorvald);
            // Skorvald may spawns with 0% hp
            CombatItem? firstNonZeroHPUpdate = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.HealthUpdate && x.SrcMatchesAgent(skorvald) && HealthUpdateEvent.GetHealthPercent(x) > 0);
            CombatItem? enterCombat = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.EnterCombat && x.SrcMatchesAgent(skorvald) && x.Time <= upperLimit + ServerDelayConstant);
            return firstNonZeroHPUpdate != null ? Math.Min(firstNonZeroHPUpdate.Time, enterCombat != null ? enterCombat.Time : long.MaxValue) : GetGenericFightOffset(fightData);
        }
        return GetGenericFightOffset(fightData);
    }

    internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
    {
        SingleActor target = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Skorvald)) ?? throw new MissingKeyActorsException("Skorvald not found");
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

    protected override ReadOnlySpan<int> GetTargetsIDs()
    {
        return
        [
            (int)TargetID.Skorvald,
            (int)TrashID.FluxAnomaly1,
            (int)TrashID.FluxAnomaly2,
            (int)TrashID.FluxAnomaly3,
            (int)TrashID.FluxAnomaly4,
            (int)TrashID.FluxAnomalyCM1,
            (int)TrashID.FluxAnomalyCM2,
            (int)TrashID.FluxAnomalyCM3,
            (int)TrashID.FluxAnomalyCM4,
        ];
    }

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        base.CheckSuccess(combatData, agentData, fightData, playerAgents);
        // reward or death worked
        if (fightData.Success)
        {
            return;
        }
        SingleActor skorvald = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Skorvald)) ?? throw new MissingKeyActorsException("Skorvald not found");
        HealthDamageEvent? lastDamageTaken = combatData.GetDamageTakenData(skorvald.AgentItem).LastOrDefault(x => (x.HealthDamage > 0) && playerAgents.Contains(x.From.GetFinalMaster()));
        if (lastDamageTaken != null)
        {
            BuffApplyEvent? invul895Apply = combatData.GetBuffDataByIDByDst(Determined895, skorvald.AgentItem).OfType<BuffApplyEvent>().Where(x => x.Time > lastDamageTaken.Time - 500).LastOrDefault();
            if (invul895Apply != null)
            {
                fightData.SetSuccess(true, Math.Min(invul895Apply.Time, lastDamageTaken.Time));
            }
        }
    }

    protected override List<TrashID> GetTrashMobsIDs()
    {
        var trashIDs = base.GetTrashMobsIDs();
        trashIDs.Add(TrashID.SolarBloom);
        return trashIDs;
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        var casts = target.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd);

        switch (target.ID)
        {
            case (int)TargetID.Skorvald:
                // Horizon Strike
                var horizonStrike = casts.Where(x => x.SkillId == HorizonStrikeSkorvald2 || x.SkillId == HorizonStrikeSkorvald4);
                foreach (CastEvent c in horizonStrike)
                {
                    int castDuration = 3900;
                    int shiftingAngle = 45;
                    int sliceSpawnInterval = 750;
                    (long start, long end) lifespan = (c.Time + 100, ComputeEndCastTimeByBuffApplication(log, target, Stun, c.Time, castDuration));
                    lifespan.end = Math.Min(lifespan.end, ComputeEndCastTimeByBuffApplication(log, target, Determined762, c.Time, castDuration));

                    if (target.TryGetCurrentFacingDirection(log, c.Time + 100, out var facingDirection, castDuration))
                    {
                        float degree = facingDirection.GetRoundedZRotationDeg();

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
                IReadOnlyList<long> skillIds = [ CrimsonDawnSkorvaldCM1, CrimsonDawnSkorvaldCM2, CrimsonDawnSkorvaldCM3, CrimsonDawnSkorvaldCM4 ];
                var crimsonDawn = casts.Where(x => skillIds.Contains(x.SkillId));
                foreach (CastEvent c in crimsonDawn)
                {
                    uint radius = 1200;
                    int angle = 295;
                    int castDuration = 3000;
                    (long start, long end) lifespan = (c.Time + 100, ComputeEndCastTimeByBuffApplication(log, target, Stun, c.Time, castDuration));
                    lifespan.end = Math.Min(lifespan.end, ComputeEndCastTimeByBuffApplication(log, target, Determined762, c.Time, castDuration));

                    if (target.TryGetCurrentFacingDirection(log, c.Time + 100, out var facingDirection, castDuration))
                    {
                        float degree = facingDirection.GetRoundedZRotationDeg();

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
                var punishingKick = casts.Where(x => x.SkillId == PunishingKickSkorvald);
                foreach (CastEvent c in punishingKick)
                {
                    int castDuration = 1850;
                    (long start, long end) lifespan = (c.Time + 100, ComputeEndCastTimeByBuffApplication(log, target, Stun, c.Time, castDuration));
                    lifespan.end = Math.Min(lifespan.end, ComputeEndCastTimeByBuffApplication(log, target, Determined762, c.Time, castDuration));
                    long expectedEndCast = c.Time + castDuration;

                    if (target.TryGetCurrentFacingDirection(log, c.Time + 100, out var frontalPoint, castDuration))
                    {
                        float rotation = frontalPoint.GetRoundedZRotationDeg();
                        // Frontal
                        AddKickIndicatorDecoration(replay, target, lifespan, expectedEndCast, rotation);
                    }
                }

                // Radiant Fury
                var radiantFury = casts.Where(x => x.SkillId == RadiantFurySkorvald);
                foreach (CastEvent c in radiantFury)
                {
                    int duration = 2700;
                    long expectedEndCast = c.Time + duration;
                    (long start, long end) lifespan = (c.Time, ComputeEndCastTimeByBuffApplication(log, target, Stun, c.Time, duration));
                    lifespan.end = Math.Min(lifespan.end, ComputeEndCastTimeByBuffApplication(log, target, Determined762, c.Time, duration));
                    (long start, long end) lifespanWave = (lifespan.end, lifespan.end + 900);

                    if (expectedEndCast <= lifespan.end)
                    {
                        GeographicalConnector connector = new AgentConnector(target);
                        replay.Decorations.AddShockwave(connector, lifespanWave, Colors.Red, 0.6, 1200);
                    }
                }

                // Supernova - Phase Oneshot
                var supernova = casts.Where(x => x.SkillId == SupernovaSkorvaldCM);
                foreach (CastEvent c in supernova)
                {
                    int duration = 75000;
                    long expectedEndCast = c.Time + duration;
                    (long start, long end) lifespan = (c.Time, ComputeEndCastTimeByBuffApplication(log, target, Stun, c.Time, duration));
                    replay.Decorations.AddWithGrowing(new CircleDecoration(1200, lifespan, Colors.Red, 0.2, new AgentConnector(target)), expectedEndCast);
                }

                // Cranial Cascade
                var cranialCascadeSkorvald = casts.Where(x => x.SkillId == CranialCascadeSkorvald);
                foreach (CastEvent c in cranialCascadeSkorvald)
                {
                    int castDuration = 1750;
                    int angle = 35;
                    long expectedEndCast = c.Time + castDuration;

                    (long start, long end) lifespan = (c.Time, ComputeEndCastTimeByBuffApplication(log, target, Stun, c.Time, castDuration));
                    lifespan.end = Math.Min(lifespan.end, ComputeEndCastTimeByBuffApplication(log, target, Determined762, c.Time, castDuration));

                    if (target.TryGetCurrentFacingDirection(log, c.Time + 100, out var frontalPoint, castDuration))
                    {
                        float rotation = frontalPoint.GetRoundedZRotationDeg();

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
                var solarStomp = casts.Where(x => x.SkillId == SolarStomp);
                foreach (CastEvent c in solarStomp)
                {
                    uint radius = 280;
                    int castDuration = 2250;
                    (long start, long end) lifespan = (c.Time, c.Time + castDuration);
                    (long start, long end) lifespanShockwave = (lifespan.end, lifespan.end + castDuration);

                    // Stomp
                    GeographicalConnector connector = new AgentConnector(target);
                    replay.Decorations.AddWithGrowing(new CircleDecoration(radius, lifespan, Colors.LightOrange, 0.2, connector), lifespan.end);
                    replay.Decorations.AddShockwave(connector, lifespanShockwave, Colors.Red, 0.6, 1200);
                }

                // Punishing Kick
                var punishingKickAnomaly = casts.Where(x => x.SkillId == PunishingKickAnomaly);
                foreach (CastEvent c in punishingKickAnomaly)
                {
                    int castDuration = 1850;
                    long expectedEndCast = c.Time + castDuration;
                    (long start, long end) lifespan = (c.Time, expectedEndCast);

                    if (target.TryGetCurrentFacingDirection(log, c.Time + 100, out var frontalPoint, castDuration))
                    {
                        float rotation =  frontalPoint.GetRoundedZRotationDeg();
                        // Frontal
                        AddKickIndicatorDecoration(replay, target, lifespan, expectedEndCast, rotation);
                    }
                }

                // Cranial Cascade
                var cranialCascadeAnomaly = casts.Where(x => x.SkillId == CranialCascadeAnomaly);
                foreach (CastEvent c in cranialCascadeAnomaly)
                {
                    int castDuration = 1750;
                    int angle = 35;
                    long expectedEndCast = c.Time + castDuration;
                    (long start, long end) lifespan = (c.Time, expectedEndCast);

                    if (target.TryGetCurrentFacingDirection(log, c.Time + 100, out var frontalPoint, castDuration))
                    {
                        float rotation = frontalPoint.GetRoundedZRotationDeg();

                        // Left
                        AddKickIndicatorDecoration(replay, target, lifespan, expectedEndCast, rotation - angle);
                        // Right
                        AddKickIndicatorDecoration(replay, target, lifespan, expectedEndCast, rotation + angle);
                    }
                }

                // Mist Smash
                var mistSmash = casts.Where(x => x.SkillId == MistSmash);
                foreach (CastEvent c in mistSmash)
                {
                    int castDuration = 1933;
                    (long start, long end) lifespan = (c.Time, c.Time + castDuration);
                    (long start, long end) lifespanShockwave = (lifespan.end, lifespan.end + 2250);
                    replay.Decorations.AddWithGrowing(new CircleDecoration(160, lifespan, Colors.Orange, 0.2, new AgentConnector(target)), lifespan.end);
                    // Nightmare Discharge Shockwave
                    GeographicalConnector connector = new AgentConnector(target);
                    replay.Decorations.AddShockwave(connector, lifespanShockwave, Colors.Yellow, 0.3, 1200);
                }

                // Wave of Mutilation
                var waveOfMutilation = casts.Where(x => x.SkillId == WaveOfMutilation);
                foreach (CastEvent c in waveOfMutilation)
                {
                    int castDuration = 1850;
                    int angle = 18;
                    long expectedEndCast = c.Time + castDuration;
                    (long start, long end) lifespan = (c.Time, expectedEndCast);

                    if (target.TryGetCurrentFacingDirection(log, c.Time + 100, out var frontalPoint, castDuration))
                    {
                        float rotation = frontalPoint.GetRoundedZRotationDeg();

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
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.MistBomb, out var mistBombs))
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
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.SkorvaldSolarBoltDamage, out var solarBolts))
        {
            foreach (EffectEvent effect in solarBolts)
            {
                (long start, long end) lifespan = effect.ComputeLifespan(log, 12000);
                EnvironmentDecorations.Add(new CircleDecoration(100, lifespan, Colors.Red, 0.2, new PositionConnector(effect.Position)));
            }
        }

        // Solar Cyclone
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.KickGroundEffect, out var kickEffects))
        {
            foreach (EffectEvent effect in kickEffects)
            {
                (long start, long end) lifespan = (effect.Time, effect.Time + 300);
                EnvironmentDecorations.Add(new RectangleDecoration(300, 180, lifespan, Colors.Red, 0.2, new PositionConnector(effect.Position)).UsingRotationConnector(new AngleConnector(effect.Rotation.Z - 90)));
            }
        }
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputePlayerCombatReplayActors(p, log, replay);
        // Fixations
        var fixations = p.GetBuffStatus(log, [FixatedBloom1, SkorvaldsIre], log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0);
        replay.Decorations.AddOverheadIcons(fixations, p, ParserIcons.FixationPurpleOverhead);
    }

    /// <summary>
    /// Add Horizon Strike decoration.
    /// </summary>
    /// <param name="replay">Combat Replay.</param>
    /// <param name="target">Actor.</param>
    /// <param name="lifespan">Start and End of cast.</param>
    /// <param name="degree">Degree of the strike.</param>
    private static void AddHorizonStrikeDecoration(CombatReplay replay, SingleActor target, (long start, long end) lifespan, float degree)
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
    private static void AddKickIndicatorDecoration(CombatReplay replay, SingleActor target, (long start, long end) lifespan, long expectedEndCast, float rotation)
    {
        int translation = 150;
        var rotationConnector = new AngleConnector(rotation);
        var positionConnector = (AgentConnector)new AgentConnector(target).WithOffset(new(translation, 0, 0), true);
        replay.Decorations.AddWithGrowing((RectangleDecoration)new RectangleDecoration(300, target.HitboxWidth, lifespan, Colors.LightOrange, 0.2, positionConnector).UsingRotationConnector(rotationConnector), expectedEndCast);

        // Cascade count => 4
        for (int i = 0; i < 4; i++)
        {
            replay.Decorations.Add(new RectangleDecoration(300, target.HitboxWidth, (lifespan.end, lifespan.end + 300), Colors.Red, 0.2, positionConnector).UsingRotationConnector(rotationConnector));
            lifespan.end += 300;
            translation += 300;
        }
    }
}
