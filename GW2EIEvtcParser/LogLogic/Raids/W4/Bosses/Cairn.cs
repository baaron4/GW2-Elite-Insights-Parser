using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicTimeUtils;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal class Cairn : BastionOfThePenitent
{
    internal readonly MechanicGroup Mechanics = new MechanicGroup([

            new PlayerDstHealthDamageHitMechanic(CairnDisplacement, new MechanicPlotlySetting(Symbols.Circle,Colors.LightOrange), "Port", "Orange Teleport Field","Orange TP", 0),
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic([SpatialManipulation1, SpatialManipulation2, SpatialManipulation3, SpatialManipulation4, SpatialManipulation5, SpatialManipulation6], new MechanicPlotlySetting(Symbols.Circle,Colors.DarkGreen), "Std.Green", "Stood in Green Spatial Manipulation Field","Green", 0)
                    .WithStabilitySubMechanic(
                        new PlayerDstHealthDamageHitMechanic([SpatialManipulation1, SpatialManipulation2, SpatialManipulation3, SpatialManipulation4, SpatialManipulation5, SpatialManipulation6], new MechanicPlotlySetting(Symbols.Circle,Colors.Green), "Green", "Green Spatial Manipulation Field (lift)","Green (lift)", 0),
                        false
                    )
                    .WithStabilitySubMechanic(
                        new PlayerDstHealthDamageHitMechanic([SpatialManipulation1, SpatialManipulation2, SpatialManipulation3, SpatialManipulation4, SpatialManipulation5, SpatialManipulation6], new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Green), "Stab.Green", "Green Spatial Manipulation Field while affected by stability","Stabilized Green", 0),
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
        arenaDecorations.Add(new ArenaDecoration(CombatReplayCairn, crMap));
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

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);

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

        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.CairnDashGreen, out var dashGreenEffects))
        {
            var spatialManipulations = log.CombatData.GetAnimatedCastData(SpatialManipulation6);
            foreach (EffectEvent dashGreen in dashGreenEffects)
            {
                long dashGreenStart = dashGreen.Time;
                long dashGreenEnd = log.LogData.LogEnd;
                CastEvent? endEvent = spatialManipulations.FirstOrDefault(x => x.EndTime >= dashGreenStart);
                if (endEvent != null)
                {
                    dashGreenEnd = Math.Min(dashGreenEnd, endEvent.Time + 3300); // from skill def
                }
                environmentDecorations.Add(new CircleDecoration(110, (dashGreenStart, dashGreenEnd), Colors.DarkGreen, 0.4, new PositionConnector(dashGreen.Position)));
                environmentDecorations.Add(new CircleDecoration(110, (dashGreenEnd - 200, dashGreenEnd), Colors.DarkGreen, 0.4, new PositionConnector(dashGreen.Position)));
            }
        }

        // Meteor Swarm
        var meteorSwarm = log.CombatData.GetMissileEventsBySkillID(MeteorSwarm);
        environmentDecorations.AddNonHomingMissiles(log, meteorSwarm, Colors.DarkPurple, 0.3, 100);
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
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
                    CombatReplay.DebugAllNPCEffects(log, replay, new HashSet<long>(), 50000, 63000);
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

    internal override List<CastEvent> SpecialCastEventProcess(CombatData combatData, SkillData skillData)
    {
        List<CastEvent> res = base.SpecialCastEventProcess(combatData, skillData);
        res.AddRange(ProfHelper.ComputeUnderBuffCastEvents(combatData, skillData, CelestialDashSAK, CelestialDashBuff));
        return res;
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputePlayerCombatReplayActors(p, log, replay);
        // shared agony
        var agony = log.CombatData.GetBuffDataByIDByDst(SharedAgony, p.AgentItem).Where(x => x is BuffApplyEvent);
        foreach (BuffEvent c in agony)
        {
            long agonyStart = c.Time;
            long agonyEnd = agonyStart + 62000;
            replay.Decorations.Add(new CircleDecoration(220, (agonyStart, agonyEnd), Colors.Red, 0.5, new AgentConnector(p)).UsingFilled(false));
        }
    }

    internal override LogData.LogMode GetLogMode(CombatData combatData, AgentData agentData, LogData logData)
    {
        return combatData.GetSkills().Contains(Countdown) ? LogData.LogMode.CM : LogData.LogMode.Normal;
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData)
    {
        return "Cairn";
    }
}
