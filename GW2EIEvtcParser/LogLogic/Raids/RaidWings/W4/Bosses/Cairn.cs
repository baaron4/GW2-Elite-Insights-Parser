using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicTimeUtils;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;
using GW2EIGW2API;

namespace GW2EIEvtcParser.LogLogic;

internal class Cairn : BastionOfThePenitent
{
    internal readonly MechanicGroup Mechanics = new([

            new PlayerDstHealthDamageHitMechanic(CairnDisplacement, new MechanicPlotlySetting(Symbols.Circle,Colors.LightOrange), "Port", "Orange Teleport Field","Orange TP", 0),
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic([SpatialManipulation1, SpatialManipulationInitial, SpatialManipulation2, SpatialManipulation3, SpatialManipulation4, SpatialManipulationFastTrigger], new MechanicPlotlySetting(Symbols.Circle,Colors.DarkGreen), "Std.Green", "Stood in Green Spatial Manipulation Field","Green", 0)
                    .WithStabilitySubMechanic(
                        new SubMechanic(new MechanicPlotlySetting(Symbols.Circle,Colors.Green), "Green", "Green Spatial Manipulation Field (lift)","Green (lift)", 0),
                        false
                    )
                    .WithStabilitySubMechanic(
                        new SubMechanic(new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Green), "Stab.Green", "Green Spatial Manipulation Field while affected by stability","Stabilized Green", 0),
                        true
                    )
                    .UsingIgnored()
                ,
            ]),
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(MeteorSwarm, new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Red), "KB", "Knockback Crystals","KB Crystal", 1000),
                new EnemySrcMissileMechanic(MeteorSwarm, new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Orange), "Refl.KB","Reflected Knockback Crystals", "Reflected KB Crystal", 0)
                    .UsingReflected(),
            ]),
            new MechanicGroup([
                new PlayerDstBuffApplyMechanic(SharedAgony, new MechanicPlotlySetting(Symbols.Circle,Colors.Red), "Agony", "Shared Agony Debuff Application","Shared Agony", 0),//could flip
                new PlayerDstBuffApplyMechanic(SharedAgony25, new MechanicPlotlySetting(Symbols.StarTriangleUpOpen,Colors.Pink), "Agony 25", "Shared Agony Damage (25% Player's HP)","SA dmg 25%", 0), // Seems to be a (invisible) debuff application for 1 second from the Agony carrier to the closest(?) person in the circle.
                new PlayerDstBuffApplyMechanic(SharedAgony50, new MechanicPlotlySetting(Symbols.StarDiamondOpen,Colors.Orange), "Agony 50", "Shared Agony Damage (50% Player's HP)","SA dmg 50%", 0), //Chaining from the first person hit by 38170, applying a 1 second debuff to the next person.
                new PlayerDstBuffApplyMechanic(SharedAgony75, new MechanicPlotlySetting(Symbols.StarOpen,Colors.Red), "Agony 75", "Shared Agony Damage (75% Player's HP)","SA dmg 75%", 0), //Chaining from the first person hit by 37768, applying a 1 second debuff to the next person.
            ]),
            new PlayerDstHealthDamageHitMechanic(EnergySurge, new MechanicPlotlySetting(Symbols.TriangleLeft,Colors.DarkGreen), "Leap", "Jump between green fields","Leap", 100),
            new PlayerDstHealthDamageHitMechanic(OrbitalSweep, new MechanicPlotlySetting(Symbols.DiamondWide,Colors.Magenta), "Sweep", "Sword Spin (Knockback)","Sweep", 100),//short cooldown because of multihits. Would still like to register second hit at the end of spin though, thus only 0.1s
            new PlayerDstHealthDamageHitMechanic(GravityWave, new MechanicPlotlySetting(Symbols.Octagon,Colors.Magenta), "Donut", "Expanding Crystal Donut Wave (Knockback)","Crystal Donut", 0)
            // Spatial Manipulation IDs correspond to the following: 1st green when starting the fight: 37629;
            // Greens after Energy Surge/Orbital Sweep: 38302
            //100% - 75%: 37611
            // 75% - 50%: 38074
            // 50% - 25%: 37673
            // 25% -  0%: 37642
        ]);
    public Cairn(int triggerID) : base(triggerID)
    {
        MechanicList.Add(Mechanics);
        Extension = "cairn";
        Icon = EncounterIconCairn;
        LogCategoryInformation.InSubCategoryOrder = 0;
        LogID |= 0x000001;
        ChestID = ChestID.CairnChest;
    }

    internal override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log, CombatReplayDecorationContainer arenaDecorations)
    {
        var crMap = new CombatReplayMap(
                        (607, 607),
                        (12981, 642, 15725, 3386));
        AddArenaDecorationsPerEncounter(log, arenaDecorations, LogID, CombatReplayCairn, crMap);
        return crMap;
    }

    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        return
        [
            new DamageCastFinder(CosmicAura, CosmicAura),
        ];
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor cairn = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Cairn)) ?? throw new MissingKeyActorsException("Cairn not found");
        phases[0].AddTarget(cairn, log);
        if (!requirePhases)
        {
            return phases;
        }
        BuffApplyEvent? enrageApply = log.CombatData.GetBuffApplyDataByIDByDst(EnragedCairn, cairn.AgentItem).OfType<BuffApplyEvent>().FirstOrDefault();
        if (enrageApply != null)
        {
            var normalPhase = new SubPhasePhaseData(log.LogData.LogStart, enrageApply.Time)
            {
                Name = "Calm"
            };
            normalPhase.AddTarget(cairn, log);
            normalPhase.AddParentPhase(phases[0]);

            var enragePhase = new SubPhasePhaseData(enrageApply.Time, log.LogData.LogEnd)
            {
                Name = "Angry"
            };
            enragePhase.AddTarget(cairn, log);
            enragePhase.AddParentPhase(phases[0]);

            phases.Add(normalPhase);
            phases.Add(enragePhase);
        }
        return phases;
    }

    private static readonly Dictionary<long, long> SpatialManipulationTriggerTime = new() // from skill def
    {
        {SpatialManipulation1, 6300},
        {SpatialManipulation2, 6300},
        {SpatialManipulation3, 6300},
        {SpatialManipulation4, 6300},
        {SpatialManipulationFastTrigger, 3300},
        {SpatialManipulationInitial, 3300},
    };

    private static void AddGreenDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations, Span<GUID> greenGUIDs)
    {
        if (log.CombatData.TryGetEffectEventsByGUIDs(greenGUIDs, out var greenEffects))
        {
            List<AnimatedCastEvent> spatialManipulations = [];
            foreach (var castSkillID in SpatialManipulationTriggerTime.Keys)
            {
                spatialManipulations.AddRange(log.CombatData.GetAnimatedCastData(castSkillID));
            }
            var cairns = log.AgentData.GetNPCsByID(TargetID.Cairn);
            spatialManipulations.SortByTime();
            foreach (EffectEvent greenEffect in greenEffects)
            {
                long greenStart = greenEffect.Time;
                var activeCairn = cairns.FirstOrDefault(x => x.InAwareTimes(greenStart));
                if (activeCairn == null)
                {
                    continue;
                }
                long greenEnd = activeCairn.LastAware;
                bool triggered = false;
                CastEvent? endEvent = spatialManipulations.FirstOrDefault(x => x.EndTime >= greenStart);
                if (endEvent != null)
                {
                    triggered = true;
                    greenEnd = Math.Min(greenEnd, endEvent.Time + SpatialManipulationTriggerTime[endEvent.SkillID]);
                }
                // Initial green and dash radius
                uint radius = 120;
                var guid = greenEffect.GUIDEvent.GUID;
                if (guid == EffectGUIDs.CairnGreen4PlayersOrBigNoCount)
                {
                    radius = 180;
                }
                else if (guid == EffectGUIDs.CairnGreen2Players)
                {
                    radius = 80;
                }
                else if (guid == EffectGUIDs.CairnGreen1Player)
                {
                    radius = 60;
                }
                GeographicalConnector positionConnector;
                if (greenEffect.IsAroundDst)
                {
                    if (greenEffect.Dst.TryGetCurrentPosition(log, greenEffect.Time, out var position))
                    {
                        positionConnector = new PositionConnector(position);
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    positionConnector = new PositionConnector(greenEffect.Position);
                }
                if (triggered)
                {
                    environmentDecorations.AddWithGrowing(new CircleDecoration(radius, (greenStart, greenEnd), Colors.DarkGreen, 0.3, positionConnector), greenEnd);
                    environmentDecorations.Add(new CircleDecoration(radius, (greenEnd - 200, greenEnd), Colors.DarkGreen, 0.4, positionConnector));
                } 
                else
                {
                    environmentDecorations.Add(new CircleDecoration(radius, (greenStart, greenEnd), Colors.DarkGreen, 0.3, positionConnector));
                }
            }
        }
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);
        }

        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.CairnDisplacement, out var displacementEffects))
        {
            foreach (EffectEvent displacement in displacementEffects)
            {
                var expectedDisplacementDuration = 3000;
                var circle = new CircleDecoration(90, displacement.ComputeLifespan(log, expectedDisplacementDuration), Colors.Orange, 0.4, new PositionConnector(displacement.Position));
                environmentDecorations.Add(circle);
                environmentDecorations.Add(circle.Copy().UsingGrowingEnd(displacement.Time + expectedDisplacementDuration));
            }
        }
        AddGreenDecorations(log, environmentDecorations, [EffectGUIDs.CairnDashGreenNoCount, EffectGUIDs.CairnInitialGreen, EffectGUIDs.CairnGreen1Player, EffectGUIDs.CairnGreen2Players, EffectGUIDs.CairnGreen4PlayersOrBigNoCount]);

        // Meteor Swarm
        var meteorSwarm = log.CombatData.GetMissileEventsBySkillID(MeteorSwarm);
        environmentDecorations.AddNonHomingMissiles(log, meteorSwarm, Colors.DarkPurple, 0.3, 100);
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeNPCCombatReplayActors(target, log, replay);
        }
        long preCastTime;

        switch (target.ID)
        {
            case (int)TargetID.Cairn:
                foreach (CastEvent cast in target.GetAnimatedCastEvents(log))
                {
                    switch (cast.SkillID)
                    {
                        // Orbital Sweep - Arm rotation swipe
                        case OrbitalSweep:
                            preCastTime = 1400;
                            long initialHitDuration = 850;
                            long sweepDuration = 1100;
                            (long start, long end) lifespanWarning = (cast.Time, cast.Time + preCastTime);
                            (long start, long end) lifespanHit = (lifespanWarning.start, lifespanWarning.start + initialHitDuration);
                            (long start, long end) lifespanSwipe = (lifespanHit.start, lifespanHit.start + sweepDuration);
                            uint width = 1400;
                            uint height = 80;
                            if (target.TryGetCurrentFacingDirection(log, lifespanWarning.start, out var facing))
                            {
                                var positionConnector = (AgentConnector)new AgentConnector(target).WithOffset(new(width / 2, 0, 0), true);
                                var rotationConnector = new AngleConnector(facing);
                                replay.Decorations.Add(new RectangleDecoration(width, height, lifespanWarning, Colors.Purple, 0.1, positionConnector).UsingRotationConnector(rotationConnector));
                                replay.Decorations.Add(new RectangleDecoration(width, height, lifespanHit, Colors.DarkPurple, 0.5, positionConnector).UsingRotationConnector(rotationConnector));
                                replay.Decorations.Add(new RectangleDecoration(width, height, lifespanSwipe, Colors.DarkPurple, 0.5, positionConnector).UsingRotationConnector(new SpinningConnector(facing, 360)));
                            }
                            break;
                        // Gravity Wave - Doughnuts
                        case GravityWave:
                            long start = cast.Time;
                            preCastTime = 1200;
                            long duration = 600;
                            (long start, long end) lifespanFirst = (start + preCastTime, start + preCastTime + duration);
                            (long start, long end) lifespanSecond = (start + preCastTime + 2 * duration, start + preCastTime + 3 * duration);
                            (long start, long end) lifespanThird = (start + preCastTime + 5 * duration, start + preCastTime + 6 * duration);
                            uint firstRadius = 400;
                            uint secondRadius = 700;
                            uint thirdRadius = 1000;
                            uint fourthRadius = 1300;
                            replay.Decorations.Add(new DoughnutDecoration(firstRadius, secondRadius, lifespanFirst, Colors.Purple, 0.3, new AgentConnector(target)));
                            replay.Decorations.Add(new DoughnutDecoration(secondRadius, thirdRadius, lifespanSecond, Colors.Purple, 0.3, new AgentConnector(target)));
                            replay.Decorations.Add(new DoughnutDecoration(thirdRadius, fourthRadius, lifespanThird, Colors.Purple, 0.3, new AgentConnector(target)));
                            break;
                        default:
                            break;
                    }
                }
                #if DEBUG_EFFECTS
                    CombatReplay.DebugAllNPCEffects(log, replay.Decorations, [], 50000, 63000);
                #endif
                break;
            default:
                break;
        }
    }

    internal override long GetLogOffset(EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData)
    {
        if (!agentData.TryGetFirstAgentItem(TargetID.Cairn, out var cairn))
        {
            throw new MissingKeyActorsException("Cairn not found");
        }
        // spawn protection loss -- most reliable
        CombatItem? spawnProtectionLoss = combatData.Find(x => x.IsBuffRemove == BuffRemove.All && x.SrcMatchesAgent(cairn) && x.SkillID == SpawnProtection);
        if (spawnProtectionLoss != null)
        {
            return spawnProtectionLoss.Time;
        }
        else
        {
            // get first end casting
            CombatItem? firstCastEnd = combatData.FirstOrDefault(x => x.EndCasting() && (x.Time - logData.EvtcLogStart) < 2000 && x.SrcMatchesAgent(cairn));
            // It has to Impact(38102), otherwise anomaly, player may have joined mid fight, do nothing
            if (firstCastEnd != null && firstCastEnd.SkillID == CairnImpact)
            {
                // Action 4 from skill dump for 38102
                long actionHappened = 1025;
                // Adds around 10 to 15 ms diff compared to buff loss
                if (firstCastEnd.BuffDmg > 0)
                {
                    double nonScaledToScaledRatio = (double)firstCastEnd.Value / firstCastEnd.BuffDmg;
                    return firstCastEnd.Time - firstCastEnd.Value + (long)Math.Round(nonScaledToScaledRatio * actionHappened) - 1;
                }
                // Adds around 15 to 20 ms diff compared to buff loss
                else
                {
                    return firstCastEnd.Time - firstCastEnd.Value + actionHappened;
                }
            }
        }
        return GetGenericLogOffset(logData);
    }

    internal override List<CastEvent> SpecialCastEventProcess(CombatData combatData, AgentData agentData, SkillData skillData)
    {
        List<CastEvent> res = [];
        res.AddRange(ProfHelper.ComputeUnderBuffCastEvents(combatData, skillData, CelestialDashSAK, CelestialDashBuff));
        return res;
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputePlayerCombatReplayActors(p, log, replay);
        }
        // shared agony
        var agony = log.CombatData.GetBuffDataByIDByDst(SharedAgony, p.AgentItem).Where(x => x is BuffApplyEvent);
        foreach (BuffEvent c in agony)
        {
            long agonyStart = c.Time;
            long agonyEnd = agonyStart + 62000;
            replay.Decorations.Add(new CircleDecoration(200, (agonyStart, agonyEnd), Colors.Red, 0.5, new AgentConnector(p)).UsingFilled(false));
        }
    }
    internal override void SetInstanceBuffs(ParsedEvtcLog log, List<InstanceBuff> instanceBuffs)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.SetInstanceBuffs(log, instanceBuffs);
        }
    }

    internal static bool HasActiveCountdownOnAllParticipatingPlayersOrPetrified(CombatData combatData, AgentData agentData, long start, long end)
    {
        if (combatData.GetBuffApplyData(CairnPetrifed).Any(x => x.Time >= start && x.Time <= end))
        {
            return true;
        }
        var countdowns = combatData.GetBuffApplyData(Countdown).Where(x => x.Time >= start && x.Time <= end).ToList();
        if (countdowns.Count > 0)
        {
            var players = agentData.GetAgentByType(AgentItem.AgentType.Player).Where(x => x.InAwareTimes(start, end) && x.IsActive(combatData, start, end, 5000));
            foreach (var player in players)
            {
                if (!countdowns.Any(x => x.To.Is(player)))
                {
                    return false;
                }
            }
            return true;
        }
        return false;
    }

    internal override LogData.Mode GetLogMode(CombatData combatData, AgentData agentData, LogData logData)
    {
        return HasActiveCountdownOnAllParticipatingPlayersOrPetrified(combatData, agentData, logData.LogStart, logData.LogEnd) ? LogData.Mode.CM : LogData.Mode.Normal;
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData, GW2APIController apiController)
    {
        return "Cairn";
    }
    internal override void ComputeAchievementEligibilityEvents(ParsedEvtcLog log, Player p, List<AchievementEligibilityEvent> achievementEligibilityEvents)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeAchievementEligibilityEvents(log, p, achievementEligibilityEvents);
        }
    }
}
