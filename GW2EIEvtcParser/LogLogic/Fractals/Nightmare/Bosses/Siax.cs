using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicTimeUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal class Siax : Nightmare
{
    internal readonly MechanicGroup Mechanics = new(
        [
            new PlayerDstHealthDamageHitMechanic(VileSpit, new MechanicPlotlySetting(Symbols.Circle,Colors.DarkGreen), "Spit", "Vile Spit (green goo)","Poison Spit", 0),
            new PlayerDstHealthDamageHitMechanic(TailLashSiax, new MechanicPlotlySetting(Symbols.TriangleLeft,Colors.Yellow), "Tail", "Tail Lash Siax (half circle Knockback)","Tail Lash (Siax)", 0),
            new SpawnMechanic((int)TargetID.NightmareHallucinationSiax, new MechanicPlotlySetting(Symbols.StarOpen,Colors.Black), "Hallu", "Nightmare Hallucination Spawn","Hallucination", 0),
            new MechanicGroup(
                [
                    new EnemyCastStartMechanic([CausticExplosionSiaxPhase66, CausticExplosionSiaxPhase33], new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Yellow), "Phase", "Phase Start","Phase", 0),
                    new EnemyCastEndMechanic([CausticExplosionSiaxPhase66, CausticExplosionSiaxPhase33], new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Red), "Phase Fail", "Phase Fail (Failed to kill Echos in time)","Phase Fail", 0)
                        .UsingChecker((ce,log) => ce.ActualDuration >= 20649), //
                    new EnemyCastStartMechanic(CausticExplosionSiaxBreakbar, new MechanicPlotlySetting(Symbols.DiamondWide,Colors.DarkTeal), "CC", "Breakbar Start","Breakbar", 0),
                    new EnemyCastEndMechanic(CausticExplosionSiaxBreakbar, new MechanicPlotlySetting(Symbols.DiamondWide,Colors.Red), "CC Fail", "Failed to CC in time","CC Fail", 0)
                        .UsingChecker( (ce,log) => ce.ActualDuration >= 15232),
                ]
            ),
            new PlayerDstBuffApplyMechanic(FixatedNightmare, new MechanicPlotlySetting(Symbols.StarOpen,Colors.Magenta), "Fixate", "Fixated by Volatile Hallucination", "Fixated", 0),
        ]);
    public Siax(int triggerID) : base(triggerID)
    {
        MechanicList.Add(Mechanics);
        Extension = "siax";
        Icon = EncounterIconSiax;
        LogCategoryInformation.InSubCategoryOrder = 1;
        LogID |= 0x000002;
    }

    internal override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log, CombatReplayDecorationContainer arenaDecorations)
    {
        var crMap = new CombatReplayMap(
                        (476, 548),
                        (663, -4127, 3515, -997));
        AddArenaDecorationsPerEncounter(log, arenaDecorations, LogID, CombatReplaySiax, crMap);
        return crMap;
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        var trashIDs = new List<TargetID>(2 + base.GetTrashMobsIDs().Count);
        trashIDs.AddRange(base.GetTrashMobsIDs());
        trashIDs.Add(TargetID.VolatileHallucinationSiax);
        trashIDs.Add(TargetID.NightmareHallucinationSiax);
        return trashIDs;
    }
    internal override IReadOnlyList<TargetID>  GetTargetsIDs()
    {
        return
        [
            TargetID.Siax,
            TargetID.EchoOfTheUnclean,
        ];
    }

    internal override LogData.Mode GetLogMode(CombatData combatData, AgentData agentData, LogData logData)
    {
        return LogData.Mode.CMNoName;
    }
    internal static List<PhaseData> ComputePhases(ParsedEvtcLog log, SingleActor siax, IReadOnlyList<SingleActor> targets, EncounterPhaseData encounterPhase, bool requirePhases)
    {
        if (!requirePhases)
        {
            return [];
        }
        var phases = new List<PhaseData>(5);
        phases.AddRange(GetPhasesByInvul(log, Determined762, siax, true, true, encounterPhase.Start, encounterPhase.End));
        for (int i = 0; i < phases.Count; i++)
        {
            PhaseData phase = phases[i];
            var index = i + 1;
            phase.AddParentPhase(encounterPhase);
            if (index % 2 == 0)
            {
                var ids = new List<TargetID>
                {
                   TargetID.EchoOfTheUnclean,
                };
                AddTargetsToPhaseAndFit(phase, targets,ids, log);
                phase.Name = "Caustic Explosion " + (index / 2);
            }
            else
            {
                phase.Name = "Phase " + (index + 1) / 2;
                phase.AddTarget(siax, log);
            }
        }
        return phases;
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor siax = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Siax)) ?? throw new MissingKeyActorsException("Siax not found");
        phases[0].AddTarget(siax, log);
        phases[0].AddTargets(Targets.Where(x => x.IsSpecies(TargetID.EchoOfTheUnclean)), log, PhaseData.TargetPriority.Blocking);
        phases.AddRange(ComputePhases(log, siax, Targets, (EncounterPhaseData)phases[0], requirePhases));
        return phases;
    }

    static readonly List<(string, Vector2)> EchoLocations =
    [
        ("N" , new(1870.630f, -2205.379f)),
        ("E" , new(2500.260f, -3288.280f)),
        ("S" , new(1572.040f, -3992.580f)),
        ("W" , new(907.199f, -2976.850f)),
        ("NW", new(1036.980f, -2237.050f)),
        ("NE", new(2556.450f, -2628.590f)),
        ("SE", new(2293.149f, -3912.510f)),
        ("SW", new(891.370f, -3722.450f)),
    ];

    internal static void RenameSiaxAndEchoes(IReadOnlyList<SingleActor> targets, List<CombatItem> combatData)
    {
        foreach (SingleActor target in targets)
        {
            if (target.IsSpecies(TargetID.Siax))
            {
                target.OverrideName("Siax the Corrupted");
            }
            else if (target.IsSpecies(TargetID.EchoOfTheUnclean))
            {
                AddNameSuffixBasedOnInitialPosition(target, combatData, EchoLocations);
            }
        }
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);
        RenameSiaxAndEchoes(Targets, combatData);
    }

    internal override long GetLogOffset(EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData)
    {
        var siax = agentData.GetNPCsByID(TargetID.Siax).FirstOrDefault() ?? throw new MissingKeyActorsException("Siax not found");
        return GetLogOffsetBySpawn(logData, combatData, siax);
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeNPCCombatReplayActors(target, log, replay);
        }
        long castDuration;
        long growing;
        (long start, long end) lifespan;

        switch (target.ID)
        {
            case (int)TargetID.Siax:
                foreach (CastEvent cast in target.GetAnimatedCastEvents(log))
                {
                    switch (cast.SkillID)
                    {
                        // Caustic Explosion - Breakbar
                        case CausticExplosionSiaxBreakbar:
                            castDuration = 15000;
                            growing = cast.Time + castDuration;
                            lifespan = (cast.Time, ComputeEndCastTimeByBuffApplication(log, target, Stun, cast.Time, castDuration));
                            lifespan.end = Math.Min(lifespan.end, ComputeEndCastTimeByBuffApplication(log, target, Determined762, cast.Time, castDuration));
                            var doughnut = new DoughnutDecoration(0, 1500, lifespan, Colors.Red, 0.2, new AgentConnector(target));
                            replay.Decorations.AddWithGrowing(doughnut, growing, true);
                            break;
                        // Caustic Explosion - 66% and 33% phases
                        case CausticExplosionSiaxPhase66:
                        case CausticExplosionSiaxPhase33:
                            castDuration = 20000;
                            growing = cast.Time + castDuration;
                            lifespan = (cast.Time, ComputeEndCastTimeByBuffApplication(log, target, Determined762, cast.Time, castDuration));
                            var circle = new CircleDecoration(1500, lifespan, Colors.Red, 0.2, new AgentConnector(target));
                            replay.Decorations.AddWithGrowing(circle, growing);
                            break;
                        // Tail Swipe
                        case TailLashSiax:
                            castDuration = 1550;
                            lifespan = (cast.Time, cast.Time + castDuration);
                            if (target.TryGetCurrentFacingDirection(log, cast.Time + castDuration, out var facing))
                            {
                                var rotation = new AngleConnector(facing);
                                replay.Decorations.Add(new PieDecoration(600, 144, lifespan, Colors.LightOrange, 0.2, new AgentConnector(target)).UsingRotationConnector(rotation));
                            }
                            break;
                        default:
                            break;
                    }
                }
                break;
            case (int)TargetID.EchoOfTheUnclean:
                foreach (CastEvent cast in target.GetAnimatedCastEvents(log))
                {
                    switch (cast.SkillID)
                    {
                        // Caustic Explosion
                        case CausticExplosionSiaxEcho:
                            // Duration is the same as Siax's explosion but starts 2 seconds later
                            // Display the explosion for a brief time
                            lifespan = (cast.Time + 18000, cast.Time + 20000);
                            replay.Decorations.Add(new CircleDecoration(3000, lifespan, Colors.Orange, 0.2, new AgentConnector(target)));
                            break;
                        default:
                            break;
                    }
                }
                break;
            case (int)TargetID.VolatileHallucinationSiax:
                // Volatile Hallucinations Explosions
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.VolatileExpulsionIndicator, out var expulsionEffects))
                {
                    foreach (EffectEvent effect in expulsionEffects)
                    {
                        lifespan = effect.ComputeLifespan(log, 300);
                        var circle = new CircleDecoration(240, lifespan, Colors.LightOrange, 0.2, new AgentConnector(target));
                        replay.Decorations.AddWithGrowing(circle, lifespan.end);
                    }
                }
                break;
            case (int)TargetID.NightmareHallucinationSiax:
                break;
            default: break;
        }
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputePlayerCombatReplayActors(p, log, replay);
        }
        // Fixations
        IEnumerable<Segment> fixations = p.GetBuffStatus(log, FixatedNightmare).Where(x => x.Value > 0);
        var fixationEvents = GetBuffApplyRemoveSequence(log.CombatData, FixatedNightmare, p, true, true);
        replay.Decorations.AddOverheadIcons(fixations, p, ParserIcons.FixationPurpleOverhead);
        replay.Decorations.AddTether(fixationEvents, Colors.Magenta, 0.5);
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);
        }

        (long start, long end) lifespan;

        // Vile Spit - Indicators
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.SiaxVileSpitIndicator, out var vileSpitIndicators))
        {
            foreach (EffectEvent effect in vileSpitIndicators)
            {
                // Indicator effect has variable duration
                lifespan = (effect.Time, effect.Time + effect.Duration);
                environmentDecorations.Add(new CircleDecoration(240, lifespan, Colors.LightOrange, 0.2, new PositionConnector(effect.Position)));
            }
        }

        // Vile Spit - Poison
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.SiaxVileSpitPoison, out var vileSpitPoisons))
        {
            foreach (EffectEvent effect in vileSpitPoisons)
            {
                lifespan = effect.ComputeDynamicLifespan(log, 15600);
                environmentDecorations.Add(new CircleDecoration(240, lifespan, Colors.GreenishYellow, 0.2, new PositionConnector(effect.Position)));
            }
        }

        // Nightmare Hallucinations Spawn Event
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.SiaxNightmareHallucinationsSpawnIndicator, out var spawnEffects))
        {
            int duration = 3000;
            foreach (EffectEvent effect in spawnEffects)
            {
                lifespan = (effect.Time, effect.Time + duration);
                var circle = new CircleDecoration(360, lifespan, Colors.Orange, 0.2, new PositionConnector(effect.Position));
                environmentDecorations.AddWithGrowing(circle, lifespan.end);
            }
        }

        // Toxic Blast - Indicator
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.SiaxToxicBlastIndicator, out var toxicBlastIndicators))
        {
            foreach (EffectEvent effect in toxicBlastIndicators)
            {
                lifespan = (effect.Time, effect.Time + effect.Duration);
                var circle = new CircleDecoration(120, lifespan, Colors.LightOrange, 0.2, new PositionConnector(effect.Position));
                if (!log.CombatData.HasMissileData)
                {
                    environmentDecorations.AddWithGrowing(circle, lifespan.end);
                }
                else
                {
                    environmentDecorations.Add(circle);
                }
            }
        }

        // Caustic Barrage
        AddDistanceCorrectedOrbAoEDecorations(log, environmentDecorations, EffectGUIDs.CausticBarrageIndicator, TargetID.Siax, 210, 1000, 966);

        // Cascade Of Torment
        AddCascadeOfTormentDecoration(log, environmentDecorations, EffectGUIDs.CascadeOfTormentRing0, 0, 150);
        AddCascadeOfTormentDecoration(log, environmentDecorations, EffectGUIDs.CascadeOfTormentRing1, 150, 250);
        AddCascadeOfTormentDecoration(log, environmentDecorations, EffectGUIDs.CascadeOfTormentRing2, 250, 350);
        AddCascadeOfTormentDecoration(log, environmentDecorations, EffectGUIDs.CascadeOfTormentRing3, 350, 450);
        AddCascadeOfTormentDecoration(log, environmentDecorations, EffectGUIDs.CascadeOfTormentRing4, 450, 550);
        AddCascadeOfTormentDecoration(log, environmentDecorations, EffectGUIDs.CascadeOfTormentRing5, 550, 650);

        // Causting Barrage, Toxic Blast & Dire Torment - Orbs
        var redOrbs = log.CombatData.GetMissileEventsBySkillIDs([CausticBarrage, CausticBarrage2, ToxicBlast, DireTorment]);
        environmentDecorations.AddNonHomingMissiles(log, redOrbs, Colors.Red, 0.3, 50);

        // Vile Spit
        var vileSpit = log.CombatData.GetMissileEventsBySkillID(VileSpit);
        environmentDecorations.AddNonHomingMissiles(log, vileSpit, Colors.DarkGreen, 0.3, 50);
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
