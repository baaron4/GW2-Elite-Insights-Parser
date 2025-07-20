using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class Siax : Nightmare
{
    public Siax(int triggerID) : base(triggerID)
    {
        MechanicList.Add(new MechanicGroup(
        [
            new PlayerDstHealthDamageHitMechanic(VileSpit, new MechanicPlotlySetting(Symbols.Circle,Colors.DarkGreen), "Spit", "Vile Spit (green goo)","Poison Spit", 0),
            new PlayerDstHealthDamageHitMechanic(TailLashSiax, new MechanicPlotlySetting(Symbols.TriangleLeft,Colors.Yellow), "Tail", "Tail Lash (half circle Knockback)","Tail Lash", 0),
            new SpawnMechanic((int)TargetID.NightmareHallucinationSiax, new MechanicPlotlySetting(Symbols.StarOpen,Colors.Black), "Hallu", "Nightmare Hallucination Spawn","Hallucination", 0),
            new PlayerDstHealthDamageHitMechanic([CascadeOfTorment1, CascadeOfTorment2], new MechanicPlotlySetting(Symbols.CircleOpen,Colors.LightOrange), "Rings", "Cascade of Torment (Alternating Rings)","Rings", 0),
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
        ]));
        Extension = "siax";
        Icon = EncounterIconSiax;
        EncounterCategoryInformation.InSubCategoryOrder = 1;
        EncounterID |= 0x000002;
    }

    protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
    {
        return new CombatReplayMap(CombatReplaySiax,
                        (476, 548),
                        (663, -4127, 3515, -997)/*,
                        (-6144, -6144, 9216, 9216),
                        (11804, 4414, 12444, 5054)*/);
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

    internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
    {
        return FightData.EncounterMode.CMNoName;
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor siax = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Siax)) ?? throw new MissingKeyActorsException("Siax not found");
        phases[0].AddTarget(siax, log);
        phases[0].AddTargets(Targets.Where(x => x.IsSpecies(TargetID.EchoOfTheUnclean)), log, PhaseData.TargetPriority.Blocking);
        if (!requirePhases)
        {
            return phases;
        }
        phases.AddRange(GetPhasesByInvul(log, Determined762, siax, true, true));
        for (int i = 1; i < phases.Count; i++)
        {
            PhaseData phase = phases[i];
            phase.AddParentPhase(phases[0]);
            if (i % 2 == 0)
            {
                var ids = new List<TargetID>
                {
                   TargetID.EchoOfTheUnclean,
                };
                AddTargetsToPhaseAndFit(phase, ids, log);
                phase.Name = "Caustic Explosion " + (i / 2);
            }
            else
            {
                phase.Name = "Phase " + (i + 1) / 2;
                phase.AddTarget(siax, log);
            }
        }
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

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
        foreach (SingleActor target in Targets)
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

    internal override long GetFightOffset(EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData)
    {
        var siax = agentData.GetNPCsByID(TargetID.Siax).FirstOrDefault() ?? throw new MissingKeyActorsException("Siax not found");
        return GetFightOffsetBySpawn(fightData, combatData, siax);
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        long castDuration;
        long growing;
        (long start, long end) lifespan;

        switch (target.ID)
        {
            case (int)TargetID.Siax:
                foreach (CastEvent cast in target.GetAnimatedCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd))
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
                foreach (CastEvent cast in target.GetAnimatedCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd))
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
        base.ComputePlayerCombatReplayActors(p, log, replay);
        // Fixations
        IEnumerable<Segment> fixations = p.GetBuffStatus(log, FixatedNightmare).Where(x => x.Value > 0);
        var fixationEvents = GetBuffApplyRemoveSequence(log.CombatData, FixatedNightmare, p, true, true);
        replay.Decorations.AddOverheadIcons(fixations, p, ParserIcons.FixationPurpleOverhead);
        replay.Decorations.AddTether(fixationEvents, Colors.Magenta, 0.5);
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);

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
}
