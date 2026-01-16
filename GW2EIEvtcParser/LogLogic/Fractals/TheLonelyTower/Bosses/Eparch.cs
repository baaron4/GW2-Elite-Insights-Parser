using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using GW2EIGW2API;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal class Eparch : LonelyTower
{
    public Eparch(int triggerID) : base(triggerID)
    {
        MechanicList.AddRange(
        [
            // Player Attunements
            new MechanicGroup([
                new PlayerDstBuffApplyMechanic(DespairAttunement, new MechanicPlotlySetting(Symbols.Circle, Colors.Blue), "Desp.Att", "Collected Despair Attunement", "Despair Attunement", 0),
                new PlayerDstBuffApplyMechanic(EnvyAttunement, new MechanicPlotlySetting(Symbols.Circle, Colors.Green), "Emvy.Att", "Collected Envy Attunement", "Envy Attunement", 0),
                new PlayerDstBuffApplyMechanic(GluttonyAttunement, new MechanicPlotlySetting(Symbols.Circle, Colors.Orange), "Glut.Att", "Collected Gluttony Attunement", "Gluttony Attunement", 0),
                new PlayerDstBuffApplyMechanic(MaliceAttunement, new MechanicPlotlySetting(Symbols.Circle, Colors.Purple), "Mali.Att", "Collected Malice Attunement", "Malice Attunement", 0),
                new PlayerDstBuffApplyMechanic(RageAttunement, new MechanicPlotlySetting(Symbols.Circle, Colors.Red), "Rage.Att", "Collected Rage Attunement", "Rage Attunement", 0),
                new PlayerDstBuffApplyMechanic(RegretAttunement, new MechanicPlotlySetting(Symbols.Circle, Colors.Yellow), "Regr.Att", "Collected Regret Attunement", "Regret Attunement", 0),
            ]),
            // Eparch Empowerments
            new MechanicGroup([
                new EnemyDstBuffApplyMechanic(DespairEmpowerment, new MechanicPlotlySetting(Symbols.CircleCross, Colors.Blue), "Desp.Emp", "Eparch Absorbed Despair Empowerment", "Despair Empowerment", 0),
                new EnemyDstBuffApplyMechanic(EnvyEmpowerment, new MechanicPlotlySetting(Symbols.CircleCross, Colors.Green), "Emvy.Emp", "Eparch Absorbed Envy Empowerment", "Envy Empowerment", 0),
                new EnemyDstBuffApplyMechanic(GluttonyEmpowerment, new MechanicPlotlySetting(Symbols.CircleCross, Colors.Orange), "Glut.Emp", "Eparch Absorbed Gluttony Empowerment", "Gluttony Empowerment", 0),
                new EnemyDstBuffApplyMechanic(MaliceEmpowerment, new MechanicPlotlySetting(Symbols.CircleCross, Colors.Purple), "Mali.Emp", "Eparch Absorbed Malice Empowerment", "Malice Empowerment", 0),
                new EnemyDstBuffApplyMechanic(RageEmpowerment, new MechanicPlotlySetting(Symbols.CircleCross, Colors.Red), "Rage.Emp", "Eparch Absorbed Rage Empowerment", "Rage Empowerment", 0),
                new EnemyDstBuffApplyMechanic(RegretEmpowerment, new MechanicPlotlySetting(Symbols.CircleCross, Colors.Yellow), "Regr.Emp", "Eparch Absorbed Regret Empowerment", "Regret Empowerment", 0),
            ]),
            // Eparch Attacks
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic([RainOfDespair, RainOfDespairPool], new MechanicPlotlySetting(Symbols.TriangleDown, Colors.RedSkin), "Desp.H", "Hit by Rain of Despair", "Rain of Despair Hit", 0),
                new PlayerDstHealthDamageHitMechanic(WaveOfEnvy, new MechanicPlotlySetting(Symbols.TriangleDown, Colors.DarkGreen), "Envy.H", "Hit by Wave of Envy", "Wave of Envy Hit", 0),
                new PlayerDstHealthDamageHitMechanic(Inhale, new MechanicPlotlySetting(Symbols.TriangleDown, Colors.Red), "Glut.H", "Hit by Inhile", "Inhile Hit", 0),
                new PlayerDstHealthDamageHitMechanic(SpikeOfMaliceHit, new MechanicPlotlySetting(Symbols.TriangleDown, Colors.DarkPurple), "Mali.H", "Hit by Spike of Malice", "Spike of Malice Hit", 0),
                new PlayerDstHealthDamageHitMechanic(RageFissure, new MechanicPlotlySetting(Symbols.TriangleDown, Colors.DarkRed), "Rage.H", "Hit by Rage Fissure", "Rage Fissure Hit", 0),
            ]),
            // Consume
            new MechanicGroup([
                new PlayerDstBuffApplyMechanic(Consumed, new MechanicPlotlySetting(Symbols.Pentagon, Colors.DarkGreen), "Consumed", "Consumed by Eparch", "Consumed", 0),
            ]),
            // Split Phase
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic([CruelDetonation1, CruelDetonation2], new MechanicPlotlySetting(Symbols.Square, Colors.Ice), "CruDeto.H", "Hit by Cruel Detonation", "Cruel Detonation Hit", 0),
                new PlayerDstHealthDamageHitMechanic(WallOfTalons, new MechanicPlotlySetting(Symbols.Octagon, Colors.Grey), "Wall.H", "Hit by Wall of Talons", "Wall of Talons Hit", 0),
                new PlayerDstHealthDamageHitMechanic(PoolOfDraining, new MechanicPlotlySetting(Symbols.CircleX, Colors.LightOrange), "PoolDra.H", "Hit by Pool of Draining (Boonstrip)", "Pool of Draining Hit", 0),
                new PlayerDstHealthDamageHitMechanic(UnliddedEye, new MechanicPlotlySetting(Symbols.CircleOpen, Colors.Yellow), "EyeWave.H", "Hit by Unlidded Eye (Shockwave)", "Unlidded Eye Hit", 0),
                new PlayerDstHealthDamageHitMechanic(EyeOfJudgment, new MechanicPlotlySetting(Symbols.DiamondWideOpen, Colors.LightOrange), "EyeArrow.H", "Hit by Eye of Judgment (Arrows)", "Eye of Judgment Hit", 0),
            ]),
            // Eparch Casts
            new MechanicGroup([
                new EnemyCastStartMechanic(BreakbarEparch, new MechanicPlotlySetting(Symbols.PentagonOpen, Colors.DarkGreen), "Breakbar", "Cast Breakbar", "Cast Breakbar", 0),
                new EnemyCastStartMechanic(RegretSkillEparch, new MechanicPlotlySetting(Symbols.StarDiamondOpen, Colors.Yellow), "Regret", "Cast Regret Skill", "Cast Regret Skill", 0),
            ])
        ]);
        Extension = "eparch";
        Icon = EncounterIconEparch;
        LogCategoryInformation.InSubCategoryOrder = 1;
        LogID |= 0x000002;
    }

    internal override LogData.Mode GetLogMode(CombatData combatData, AgentData agentData, LogData logData)
    {
        ulong build = combatData.GetGW2BuildEvent().Build;
        int healthCMRelease = build >= GW2Builds.June2024Balance ? 22_833_236 : 32_618_906;
        int healthThreshold = (int)(0.95 * healthCMRelease); // fractals lose hp as their scale lowers
        SingleActor eparch = GetEparchActor();
        if (build >= GW2Builds.June2024LonelyTowerCMRelease && eparch.GetHealth(combatData) >= healthThreshold)
        {
            return LogData.Mode.CM;
        }
        return LogData.Mode.Normal;
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData, GW2APIController apiController)
    {
        return "Eparch";
    }

    internal override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log, CombatReplayDecorationContainer arenaDecorations)
    {
        var crMap = new CombatReplayMap(
                        (1035, 934),
                        (-950, 1040, 2880, 4496));
        AddArenaDecorationsPerEncounter(log, arenaDecorations, LogID, CombatReplayEparch, crMap);
        return crMap;

    }
    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        var dummyEparchs = agentData.GetNPCsByID(TargetID.EparchLonelyTower).Where(eparch =>
        {
            return !combatData.Any(x => x.SrcMatchesAgent(eparch) && x.StartCasting() && x.SkillID != WeaponDraw && x.SkillID != WeaponStow);
        });
        foreach (var dummyEparch in dummyEparchs)
        {
            dummyEparch.OverrideID(IgnoredSpecies, agentData);
        }
        //
        var riftAgents = combatData.Where(x => MaxHealthUpdateEvent.GetMaxHealth(x) == 149400 && x.IsStateChange == StateChange.MaxHealthUpdate).Select(x => agentData.GetAgent(x.SrcAgent, x.Time)).Where(x => x.Type == AgentItem.AgentType.Gadget && x.FirstAware > logData.LogStart + 5000);
        foreach (var riftAgent in riftAgents)
        {
            riftAgent.OverrideID(TargetID.KryptisRift, agentData);
            riftAgent.OverrideType(AgentItem.AgentType.NPC, agentData);
        }
        //

        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);
    }

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, LogData logData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        SingleActor eparch = GetEparchActor();
        var determinedApplies = combatData.GetBuffApplyDataByIDByDst(Determined762, eparch.AgentItem).OfType<BuffApplyEvent>().ToList();
        var cmCheck = GetLogMode(combatData, agentData, logData) == LogData.Mode.CM || IsFakeCM(agentData);
        if (cmCheck && determinedApplies.Count >= 3)
        {
            logData.SetSuccess(true, determinedApplies[2].Time);
        }
        else if (!cmCheck && determinedApplies.Count >= 1)
        {
            logData.SetSuccess(true, determinedApplies[0].Time);
        } 
        else
        {
            logData.SetSuccess(false, eparch.LastAware);
        }
    }

    private static bool IsFakeCM(AgentData agentData)
    {
        return agentData.GetNPCsByIDs([TargetID.IncarnationOfCruelty, TargetID.IncarnationOfJudgement]).Count > 0;
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor eparch = GetEparchActor();
        var encounterPhase = (EncounterPhaseData)phases[0];
        phases[0].AddTarget(eparch, log);
        phases[0].AddTargets(Targets.Where(x => x.IsAnySpecies([TargetID.IncarnationOfCruelty, TargetID.IncarnationOfJudgement])), log, PhaseData.TargetPriority.Blocking);
        if (!requirePhases || (!encounterPhase.IsCM && !IsFakeCM(log.AgentData)))
        {
            return phases;
        }
        phases.AddRange(GetPhasesByInvul(log, Determined762, eparch, true, true));
        for (int i = 1; i < phases.Count; i++)
        {
            PhaseData phase = phases[i];
            phase.AddParentPhase(phases[0]);
            if (i % 2 == 0)
            {
                phase.Name = "Split " + i / 2;
                List<TargetID> ids =
                [
                    TargetID.IncarnationOfCruelty,
                    TargetID.IncarnationOfJudgement,
                    TargetID.KryptisRift,
                ];
                AddTargetsToPhase(phase, ids, log);
            }
            else
            {
                phase.Name = "Phase " + (i + 1) / 2;
                phase.AddTarget(eparch, log);
            }
        }
        return phases;
    }

    internal override IReadOnlyList<TargetID>  GetTargetsIDs()
    {
        return
        [
            TargetID.EparchLonelyTower,
            TargetID.IncarnationOfCruelty,
            TargetID.IncarnationOfJudgement,
            TargetID.AvatarOfSpite,
            TargetID.KryptisRift,
        ];
    }

    internal override Dictionary<TargetID, int> GetTargetsSortIDs()
    {
        return new Dictionary<TargetID, int>
        {
            {TargetID.EparchLonelyTower, 0},
            {TargetID.KryptisRift, 1},
            {TargetID.IncarnationOfCruelty, 2},
            {TargetID.IncarnationOfJudgement, 2},
            {TargetID.AvatarOfSpite, 3},
        };
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        return
        [
            TargetID.TheTormentedLonelyTower,
            TargetID.TheCravenLonelyTower,
        ];
    }

    private SingleActor GetEparchActor()
    {
        SingleActor eparch = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.EparchLonelyTower)) ?? throw new MissingKeyActorsException("Eparch not found");
        return eparch;
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor player, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputePlayerCombatReplayActors(player, log, replay);
        }

        // Consume fixations
        var consumes = player.GetBuffStatus(log, Consume).Where(x => x.Value > 0);
        var consumeEvents = GetBuffApplyRemoveSequence(log.CombatData, [Consume], player, true, true);
        replay.Decorations.AddOverheadIcons(consumes, player, ParserIcons.FixationRedOverhead);
        replay.Decorations.AddTether(consumeEvents, Colors.Red, 0.5);
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeNPCCombatReplayActors(target, log, replay);
        }

        (long start, long end) lifespan;
        long duration;

        switch (target.ID)
        {
            case (int)TargetID.EparchLonelyTower:
                // Pool of Despair - Indicator
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.EparchDespairPoolIndicator, out var poolsIndicators))
                {
                    foreach (EffectEvent effect in poolsIndicators)
                    {
                        lifespan = effect.ComputeLifespan(log, 2000);
                        var circle = new CircleDecoration(110, lifespan, Colors.Red, 0.2, new PositionConnector(effect.Position)).UsingFilled(false);
                        replay.Decorations.Add(circle);
                        if (target.TryGetCurrentPosition(log, effect.Time, out var eparchPos))
                        {
                            replay.Decorations.AddProjectile(eparchPos, effect.Position, lifespan, Colors.Black, 0.4);
                        }
                    }
                }

                // Consume Breakbar
                var breakbarUpdates = target.GetBreakbarPercentUpdates(log);
                var (breakbarNones, breakbarActives, breakbarImmunes, breakbarRecoverings) = target.GetBreakbarStatus(log);
                foreach (var segment in breakbarActives)
                {
                    replay.Decorations.AddActiveBreakbar(segment.TimeSpan, target, breakbarUpdates);
                }
                break;
            case (int)TargetID.KryptisRift:
                {
                    var events = GetBuffApplyRemoveSequence(log.CombatData, [KryptisRiftIncarnationTether], target, true, true);
                    replay.Decorations.AddTether(events, Colors.Red, 0.5);
                    break;
                }
            case (int)TargetID.IncarnationOfJudgement:
                // Unlidded Eye - Reverse Shockwave - Wave Warning
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.EparchJudgmentUnliddedEyeWarning, out var eyeWarning))
                {
                    foreach (EffectEvent effect in eyeWarning)
                    {
                        lifespan = effect.ComputeLifespan(log, 2000);
                        replay.Decorations.AddShockwave(new PositionConnector(effect.Position), lifespan, Colors.LightOrange, 0.3, 600, true);
                    }
                }

                // Unlidded Eye - Reverse Shockwave - Wave Hit
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.EparchJudgmentUnliddedEyeWave, out var eyeWave))
                {
                    foreach (EffectEvent effect in eyeWave)
                    {
                        lifespan = effect.ComputeLifespan(log, 2400);
                        replay.Decorations.AddShockwave(new AgentConnector(target), lifespan, Colors.Yellow, 0.5, 1200, true);
                    }
                }

                // Pool of Draining - AoE underneath boon removal
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.EparchJudgmentPoolOfDraining, out var drainingPools))
                {
                    foreach (EffectEvent effect in drainingPools)
                    {
                        duration = 1266;
                        long growing = effect.Time + duration;
                        lifespan = effect.ComputeLifespan(log, duration);
                        lifespan.end = ComputeEndCastTimeByBuffApplication(log, target, Stun, effect.Time, duration);
                        var circle = new CircleDecoration(200, lifespan, Colors.LightOrange, 0.2, new PositionConnector(effect.Position));
                        replay.Decorations.AddWithGrowing(circle, growing);
                    }
                }
                break;
            default:
                break;

        }
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);
        }

        (long start, long end) lifespan;

        AddGlobuleDecorations(log, environmentDecorations);

        // Rain of Despair - Pools
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.EparchDespairPool, out var pools))
        {
            foreach (EffectEvent effect in pools)
            {
                lifespan = effect.ComputeDynamicLifespan(log, 15000);
                var circle = new CircleDecoration(110, lifespan, Colors.RedSkin, 0.2, new PositionConnector(effect.Position));
                environmentDecorations.AddWithBorder(circle, Colors.Red, 0.2);
            }
        }

        // Rage Fissure - Shockwave
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.EparchRageImpact, out var rageWaves))
        {
            const float velocity = 450.0f; // units per second
            const uint maxRange = 1300;
            const long duration = (long)(1000.0f * maxRange / velocity);
            foreach (EffectEvent effect in rageWaves)
            {
                lifespan = (effect.Time, effect.Time + duration);
                environmentDecorations.AddShockwave(new PositionConnector(effect.Position), lifespan, Colors.Orange, 0.3, maxRange);
            }
        }

        // Rage Fissures
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.EparchRageFissure, out var fissures))
        {
            const uint width = 40;
            const uint length = 220;
            foreach (EffectEvent effect in fissures)
            {
                lifespan = effect.ComputeDynamicLifespan(log, 24000);
                GeographicalConnector position = new PositionConnector(effect.Position).WithOffset(new(0.0f, 0.5f * length, 0), true);
                environmentDecorations.Add(new RectangleDecoration(width, length, lifespan, Colors.Red, 0.2, position).UsingRotationConnector(new AngleConnector(effect.Rotation.Z)));
            }
        }

        // Wave of Envy - Arrows
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.EparchArrowIndicator, out var envyArrows))
        {
            const uint width = 60;
            const uint length = 800;
            foreach (EffectEvent effect in envyArrows)
            {
                lifespan = effect.ComputeLifespan(log, 1500);
                var rotation = new AngleConnector(effect.Rotation.Z);
                GeographicalConnector position = new PositionConnector(effect.Position).WithOffset(new(0.0f, 0.5f * length, 0), true);
                environmentDecorations.Add(new RectangleDecoration(width, length, lifespan, Colors.Orange, 0.2, position).UsingRotationConnector(rotation));
                if (effect.Src.IsSpecies(TargetID.EparchLonelyTower))
                {
                    environmentDecorations.Add(new RectangleDecoration(width, length, (lifespan.end, lifespan.end + 300), Colors.DarkGreen, 0.2, position).UsingRotationConnector(rotation));
                }
            }
        }

        // Inhale - Gluttony Indicator
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.EparchInhale, out var inhales))
        {
            foreach (EffectEvent effect in inhales)
            {
                lifespan = effect.ComputeDynamicLifespan(log, 5000);
                environmentDecorations.Add(new CircleDecoration(400, lifespan, Colors.Red, 0.2, new PositionConnector(effect.Position)));
            }
        }

        // Spike of Malice - Indicator
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.EparchCircleIndicator, out var spikeIndicators))
        {
            foreach (EffectEvent effect in spikeIndicators)
            {
                lifespan = effect.ComputeDynamicLifespan(log, 1000);
                var circle = new CircleDecoration(100, lifespan, Colors.Orange, 0.2, new PositionConnector(effect.Position)).UsingFilled(false);
                environmentDecorations.AddWithGrowing(circle, lifespan.end);
            }
        }

        // Spike of Malice - Damage
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.EparchSpikeOfMalice, out var spikes))
        {
            foreach (EffectEvent effect in spikes)
            {
                lifespan = effect.ComputeDynamicLifespan(log, 300);
                environmentDecorations.Add(new CircleDecoration(100, lifespan, Colors.Red, 0.2, new PositionConnector(effect.Position)));
            }
        }

        // Walls of Talons - Incarnation of Cruelty 
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.EparchCrueltyWallsOfTalons, out var crueltyWalls))
        {
            foreach (EffectEvent effect in crueltyWalls)
            {
                lifespan = effect.ComputeDynamicLifespan(log, 22000);
                var wall = (RectangleDecoration)new RectangleDecoration(200, 100, lifespan, Colors.Grey, 0.3, new PositionConnector(effect.Position)).UsingRotationConnector(new AngleConnector(effect.Rotation.Z));
                environmentDecorations.AddWithBorder(wall, Colors.Red, 0.2);
            }
        }

        // Eye of Judgment - Arrow circle hits
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.EparchEyeOfJudgmentArrowHits, out var eyeHits))
        {
            foreach (EffectEvent effect in eyeHits)
            {
                lifespan = (effect.Time, effect.Time + 300);
                var circle = new CircleDecoration(80, lifespan, Colors.Red, 0.2, new PositionConnector(effect.Position));
                environmentDecorations.Add(circle);
            }
        }
    }

    private void AddGlobuleDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        SingleActor eparch = GetEparchActor();
        IReadOnlyList<AnimatedCastEvent> eparchCasts = log.CombatData.GetAnimatedCastData(eparch.AgentItem);

        // globule gadgets as decorations
        var globuleColors = new Dictionary<long, Color>
        {
            { RainOfDespair, Colors.Blue },
            { WaveOfEnvy, Colors.Green },
            { Inhale, Colors.Orange },
            { SpikeOfMalice, Colors.Purple },
            { EnragedSmashEparch, Colors.Red },
            { RegretSkillEparch, Colors.Yellow },
        };
        foreach (AgentItem gadget in log.AgentData.GetAgentByType(AgentItem.AgentType.Gadget))
        {
            const int globuleHealth = 14_940;
            const uint globuleWidth = 16;
            const uint globuleHeight = 160;
            MaxHealthUpdateEvent? health = log.CombatData.GetMaxHealthUpdateEventsBySrc(gadget).LastOrDefault(); // may have max health 0 initially
            if (gadget.HitboxWidth == globuleWidth && gadget.HitboxHeight == globuleHeight && health?.MaxHealth == globuleHealth)
            {
                SpawnEvent? spawn = log.CombatData.GetSpawnEvents(gadget).FirstOrDefault();
                DespawnEvent? despawn = log.CombatData.GetDespawnEvents(gadget).FirstOrDefault();
                if (spawn != null && despawn != null)
                {
                    const long globuleDelay = 700;
                    AnimatedCastEvent? lastCast = eparchCasts.LastOrDefault(x => x.Time < spawn.Time - globuleDelay);
                    if (lastCast != null && globuleColors.TryGetValue(lastCast.SkillID, out var color))
                    {
                        if (gadget.TryGetCurrentPosition(log, gadget.LastAware, out var position))
                        {
                            (long, long) lifespan = (spawn.Time, despawn.Time);
                            environmentDecorations.Add(new CircleDecoration(globuleWidth, lifespan, color, 0.7, new PositionConnector(position)));
                        }
                    }
                }
            }
        }
    }

    internal override void SetInstanceBuffs(ParsedEvtcLog log, List<InstanceBuff> instanceBuffs)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.SetInstanceBuffs(log, instanceBuffs);
        }
    }
    internal override void ComputeAchievementEligibilityEvents(ParsedEvtcLog log, Player p, List<AchievementEligibilityEvent> achievementEligibilityEvents)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeAchievementEligibilityEvents(log, p, achievementEligibilityEvents);
        }
    }
}
