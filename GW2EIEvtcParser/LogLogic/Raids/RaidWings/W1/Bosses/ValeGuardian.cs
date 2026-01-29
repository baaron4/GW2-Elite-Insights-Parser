using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal class ValeGuardian : SpiritVale
{
    internal readonly MechanicGroup Mechanics = new([
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(GreenGuardianUnstableMagicSpike, new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Blue), "Split TP", "Unstable Magic Spike (Green Guard Teleport)","Green Guard TP",500),
                new PlayerDstHealthDamageHitMechanic(UnstableMagicSpike, new MechanicPlotlySetting(Symbols.Circle,Colors.Blue), "Boss TP", "Unstable Magic Spike (Boss Teleport)","Boss TP", 500),
                new PlayerDstHealthDamageHitMechanic(BulletStorm, new MechanicPlotlySetting(Symbols.Circle, Colors.White), "Orbs", "Bullet Storm (Orbs during split)", "Bullet Storm Orbs", 0),
            ]),
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic([DistributedMagicBlue, DistributedMagicRed, DistributedMagic, DistributedMagicGreen], new MechanicPlotlySetting(Symbols.Circle,Colors.DarkGreen), "Green", "Distributed Magic (Stood in Green)","Green Team", 0),
                new EnemyCastStartMechanic(DistributedMagicBlue, new MechanicPlotlySetting(Symbols.CircleOpen,Colors.LightBlue) , "Green Cast B", "Distributed Magic (Green Field appeared in Blue Sector)","Green in Blue", 0),
                new EnemyCastStartMechanic(DistributedMagicRed, new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Orange), "Green Cast R", "Distributed Magic (Green Field appeared in Red Sector)","Green in Red", 0),
                new EnemyCastStartMechanic(DistributedMagicGreen, new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Green), "Green Cast G", "Distributed Magic (Green Field appeared in Green Sector)","Green in Green", 0),
            ]),
            new PlayerDstHealthDamageHitMechanic(MagicPulse, new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Red), "Seeker", "Magic Pulse (Hit by Seeker)","Seeker", 0),
            new MechanicGroup([
                new PlayerDstBuffApplyMechanic(PylonAttunementRed, new MechanicPlotlySetting(Symbols.Square,Colors.Red), "Attune R", "Pylon Attunement: Red","Red Attuned", 0),
                new PlayerDstBuffApplyMechanic(PylonAttunementBlue, new MechanicPlotlySetting(Symbols.Square,Colors.Blue), "Attune B", "Pylon Attunement: Blue","Blue Attuned", 0),
                new PlayerDstBuffApplyMechanic(PylonAttunementGreen, new MechanicPlotlySetting(Symbols.Square,Colors.DarkGreen), "Attune G", "Pylon Attunement: Green","Green Attuned", 0),
            ]),
            new EnemyDstBuffRemoveMechanic(BluePylonPower, new MechanicPlotlySetting(Symbols.SquareOpen,Colors.Blue), "Invuln Strip", "Blue Guard Invuln was stripped","Blue Invuln Strip", 0),
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(UnstablePylonRed, new MechanicPlotlySetting(Symbols.HexagramOpen,Colors.Red), "Floor R", "Unstable Pylon (Red Floor dmg)","Floor dmg", 0),
                new PlayerDstHealthDamageHitMechanic(UnstablePylonBlue, new MechanicPlotlySetting(Symbols.HexagramOpen,Colors.Blue), "Floor B", "Unstable Pylon (Blue Floor dmg)","Floor dmg", 0),
                new PlayerDstHealthDamageHitMechanic(UnstablePylonGreen, new MechanicPlotlySetting(Symbols.HexagramOpen,Colors.DarkGreen), "Floor G", "Unstable Pylon (Green Floor dmg)","Floor dmg", 0),
            ]),
            new MechanicGroup([
                new EnemyCastStartMechanic(MagicStorm, new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkTeal), "CC", "Magic Storm (Breakbar)","Breakbar",0),
                new EnemyCastEndMechanic(MagicStorm, new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkGreen), "CCed", "Magic Storm (Breakbar broken) ","CCed", 0)
                    .UsingChecker((c, log) => c.ActualDuration <= 8544),
                new EnemyCastEndMechanic(MagicStorm, new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Red), "CC Fail", "Magic Storm (Breakbar failed) ","CC Fail", 0)
                    .UsingChecker((c, log) => c.ActualDuration > 8544),
            ]),
        ]);

    public ValeGuardian(int triggerID) : base(triggerID)
    {
        MechanicList.Add(Mechanics);
        Extension = "vg";
        Icon = EncounterIconValeGuardian;
        LogCategoryInformation.InSubCategoryOrder = 0;
        LogID |= 0x000001;
        ChestID = ChestID.GuardianChest;
    }

    internal override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log, CombatReplayDecorationContainer arenaDecorations)
    {
        var crMap = new CombatReplayMap(
                        (889, 889),
                        (-6365, -22213, -3150, -18999));
        AddArenaDecorationsPerEncounter(log, arenaDecorations, LogID, CombatReplayValeGuardian, crMap);
        return crMap;
    }
    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        return
        [
            new DamageCastFinder(MagicAuraValeGuardian, MagicAuraValeGuardian),
            new DamageCastFinder(MagicAuraRedGuardian, MagicAuraRedGuardian),
            new DamageCastFinder(MagicAuraBlueGuardian, MagicAuraBlueGuardian),
            new DamageCastFinder(MagicAuraGreenGuardian, MagicAuraGreenGuardian),
        ];
    }

    internal override IReadOnlyList<TargetID>  GetTargetsIDs()
    {
        return
        [
            TargetID.ValeGuardian,
            TargetID.RedGuardian,
            TargetID.BlueGuardian,
            TargetID.GreenGuardian
        ];
    }
    internal override Dictionary<TargetID, int> GetTargetsSortIDs()
    {
        return new Dictionary<TargetID, int>()
        {
            {TargetID.ValeGuardian, 0 },
            {TargetID.RedGuardian, 1 },
            {TargetID.BlueGuardian, 1 },
            {TargetID.GreenGuardian, 1 },
        };
    }

    private static readonly List<TargetID> SplitGuardianIDs =
    [
        TargetID.BlueGuardian,
        TargetID.GreenGuardian,
        TargetID.RedGuardian
    ];

    internal static IReadOnlyList<SubPhasePhaseData> ComputePhases(ParsedEvtcLog log, SingleActor valeGuardian, IReadOnlyList<SingleActor> targets, EncounterPhaseData encounterPhase, bool requirePhases)
    {
        if (!requirePhases)
        {
            return [];
        }
        var phases = GetSubPhasesByInvul(log, Invulnerability757, valeGuardian, true, true, encounterPhase.Start, encounterPhase.End);
        for (int i = 0; i < phases.Count; i++)
        {
            int index = i + 1;
            PhaseData phase = phases[i];
            phase.AddParentPhase(encounterPhase);
            if (index % 2 == 0)
            {
                phase.Name = "Split " + (index) / 2;
                AddTargetsToPhaseAndFit(phase, targets, SplitGuardianIDs, log);
            }
            else
            {
                phase.Name = "Phase " + (index + 1) / 2;
                phase.AddTarget(valeGuardian, log);
            }
        }
        return phases;
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.ValeGuardian)) ?? throw new MissingKeyActorsException("Vale Guardian not found");
        phases[0].AddTarget(mainTarget, log);
        phases[0].AddTargets(Targets.Where(x => x.IsAnySpecies(SplitGuardianIDs)), log, PhaseData.TargetPriority.Blocking);
        phases.AddRange(ComputePhases(log, mainTarget, Targets, (EncounterPhaseData)phases[0], requirePhases));
        return phases;
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        return
        [
           TargetID.Seekers
        ];
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);
        }

        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.ValeGuardianDistributedMagic, out var distributedMagicEvents))
        {
            foreach (EffectEvent distributedMagic in distributedMagicEvents)
            {
                // Effect duration is 6000, added 700 for the damage.
                (long start, long end) lifespan = distributedMagic.ComputeLifespan(log, 6000);
                lifespan.end = Math.Min(lifespan.end + 700, distributedMagic.Src.LastAware);
                var circle = new CircleDecoration(180, lifespan, Colors.DarkGreen, 0.2, new PositionConnector(distributedMagic.Position));
                environmentDecorations.AddWithGrowing(circle, lifespan.end);
            }
        }

        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.ValeGuardianMagicSpike, out var magicSpikeEvents))
        {
            foreach (EffectEvent magicSpike in magicSpikeEvents)
            {
                (long start, long end) lifespan = magicSpike.ComputeLifespan(log, 2000);
                var circle = new CircleDecoration(90, lifespan, Colors.Blue, 0.2, new PositionConnector(magicSpike.Position));
                environmentDecorations.AddWithGrowing(circle, lifespan.end);
            }
        }

        var bulletStorm = log.CombatData.GetMissileEventsBySkillID(BulletStorm);
        environmentDecorations.AddNonHomingMissiles(log, bulletStorm, Colors.White, 0.2, 40);
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeNPCCombatReplayActors(target, log, replay);
        }
        (long start, long end) lifespan = (replay.TimeOffsets.start, replay.TimeOffsets.end);

        switch (target.ID)
        {
            case (int)TargetID.ValeGuardian:
                foreach (CastEvent cast in target.GetAnimatedCastEvents(log))
                {
                    switch (cast.SkillID)
                    {
                        // Magic Storm - Breakbar
                        case MagicStorm:
                            lifespan = (cast.Time, cast.EndTime);
                            replay.Decorations.Add(new OverheadProgressBarDecoration(
                                ParserHelper.CombatReplayOverheadProgressBarMajorSizeInPixel, lifespan, Colors.LightBlue, 0.6, Colors.Black, 0.2, 
                                [(lifespan.start, 0), (lifespan.start + 30000, 100)], new AgentConnector(target)
                            ).UsingRotationConnector(new AngleConnector(180)));
                            break;
                        // Distributed Magic - Green circle in green area
                        case DistributedMagicGreen:
                            AddDistributedMagicDecoration(log, replay, cast.Time, new(-5449.0f, -20219.0f, 0.0f), 151);
                            break;
                        // Distributed Magic - Green circle in blue area
                        case DistributedMagicBlue:
                            AddDistributedMagicDecoration(log, replay, cast.Time, new(-4063.0f, -20195.0f, 0.0f), 31);
                            break;
                        // Distributed Magic - Green circle in red area
                        case DistributedMagicRed:
                            AddDistributedMagicDecoration(log, replay, cast.Time, new(-4735.0f, -21407.0f, 0.0f), 271);
                            break;
                        default:
                            break;
                    }
                }
                #if DEBUG_EFFECTS
                    CombatReplay.DebugEffects(target, log, replay.Decorations, [], target.FirstAware, target.LastAware);
                    CombatReplay.DebugUnknownEffects(log, replay.Decorations, [], target.FirstAware, target.LastAware);
                #endif
                break;
            case (int)TargetID.BlueGuardian:
                replay.Decorations.Add(new CircleDecoration(1500, lifespan, Colors.Blue, 0.5, new AgentConnector(target)).UsingFilled(false));
                break;
            case (int)TargetID.GreenGuardian:
                replay.Decorations.Add(new CircleDecoration(1500, lifespan, Colors.Green, 0.5, new AgentConnector(target)).UsingFilled(false));
                break;
            case (int)TargetID.RedGuardian:
                replay.Decorations.Add(new CircleDecoration(1500, lifespan, Colors.Red, 0.5, new AgentConnector(target)).UsingFilled(false));
                break;
            case (int)TargetID.Seekers:
                replay.Decorations.Add(new CircleDecoration(180, lifespan, Colors.Red, 0.5, new AgentConnector(target)).UsingFilled(false));
                break;
            default:
                break;
        }
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputePlayerCombatReplayActors(p, log, replay);
        }
        // Attunements Overhead
        replay.Decorations.AddOverheadIcons(p.GetBuffStatus(log, PylonAttunementBlue).Where(x => x.Value > 0), p, ParserIcons.SensorBlueOverhead);
        replay.Decorations.AddOverheadIcons(p.GetBuffStatus(log, PylonAttunementGreen).Where(x => x.Value > 0), p, ParserIcons.SensorGreenOverhead);
        replay.Decorations.AddOverheadIcons(p.GetBuffStatus(log, PylonAttunementRed).Where(x => x.Value > 0), p, ParserIcons.SensorRedOverhead);
    }
    internal override void SetInstanceBuffs(ParsedEvtcLog log, List<InstanceBuff> instanceBuffs)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.SetInstanceBuffs(log, instanceBuffs);
        }
    }

    /// <summary>
    /// Adds Distributed Magic green circle for older logs without Effect Data.
    /// </summary>
    private static void AddDistributedMagicDecoration(ParsedEvtcLog log, CombatReplay replay, long time, Vector3 circlePosition, float angle)
    {
        if (!log.CombatData.HasEffectData)
        {
            int duration = 6700;
            int impactDuration = 110;
            uint arenaRadius = 1600;
            (long start, long end) lifespan = (time, time + duration);
            var piePositionConnector = new PositionConnector(new(-4749.838867f, -20607.296875f, 0.0f));
            var circlePositionConnector = new PositionConnector(circlePosition);
            var rotationConnector = new AngleConnector(angle);
            replay.Decorations.Add(new PieDecoration(arenaRadius, 120, lifespan, Colors.Green, 0.1, piePositionConnector).UsingGrowingEnd(lifespan.end).UsingRotationConnector(rotationConnector));
            replay.Decorations.Add(new PieDecoration(arenaRadius, 120, (lifespan.end, lifespan.end + impactDuration), Colors.Green, 0.3, piePositionConnector).UsingRotationConnector(rotationConnector));
            replay.Decorations.Add(new CircleDecoration(180, lifespan, Colors.Green, 0.2, circlePositionConnector));
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
