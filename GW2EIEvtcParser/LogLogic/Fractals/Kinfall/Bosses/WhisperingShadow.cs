using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SpeciesIDs;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.AchievementEligibilityIDs;
using GW2EIEvtcParser.ParserHelpers;

namespace GW2EIEvtcParser.LogLogic;

internal class WhisperingShadow : Kinfall
{
    internal readonly MechanicGroup Mechanics = new MechanicGroup([
            new MechanicGroup([
                new PlayerDstBuffApplyMechanic(DeathlyGrime, new MechanicPlotlySetting(Symbols.Diamond, Colors.Purple), "DeathGr.A", "Gained Deathly Grime", "Deathly Grime Application", 0),
                new PlayerDstBuffApplyMechanic([LifeFireCircleT1, LifeFireCircleT2, LifeFireCircleT3, LifeFireCircleT4, LifeFireCircleCM], new MechanicPlotlySetting(Symbols.Pentagon, Colors.LightBlue), "LifeFire.A", "Gained Life-Fire Circle", "Life-Fire Circle Apply", 0),
                new PlayerDstBuffRemoveMechanic([LifeFireCircleT1, LifeFireCircleT2, LifeFireCircleT3, LifeFireCircleT4, LifeFireCircleCM], new MechanicPlotlySetting(Symbols.PentagonOpen, Colors.LightBlue), "LifeFire.R", "Lost Life-Fire Circle", "Life-Fire Circle Remove", 0),
            ]),
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic([VitreousSpikeHit1, VitreousSpikeHit2, VitreosSpikeHit3], new MechanicPlotlySetting(Symbols.TriangleUp, Colors.SkyBlue), "Spike.H", "Hit by Vitreous Spike", "Vitreous Spike Hit", 0),
                new PlayerDstHealthDamageHitMechanic([FallingIce, FallingIceCM], new MechanicPlotlySetting(Symbols.DiamondTall, Colors.LightBlue), "Fall.H", "Hit by Falling Ice", "Falling Ice Hit", 0),
            ]),
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic([FrozenTeeth, FrozenTeethCM], new MechanicPlotlySetting(Symbols.XThinOpen, Colors.SkyBlue), "Fissure.H", "Hit by Frozen Teeth (Fissures)", "Frozen Teeth Hit", 0),
                new PlayerDstHealthDamageHitMechanic(LoftedCryoflash, new MechanicPlotlySetting(Symbols.StarTriangleDownOpen, Colors.Red), "HighCryo.H", "Hit by Lofted Cryoflash (High Shockwave)", "Lofted Cryoflash Hit", 0),
                new PlayerDstHealthDamageHitMechanic(TerrestialCryoflash, new MechanicPlotlySetting(Symbols.StarTriangleUpOpen, Colors.Red), "LowCryo.H", "Hit by Terrestrial Cryoflash (Low Shockwave)", "Terrestrial Cryoflash Hit", 0),
            ]),
            new MechanicGroup([
                new PlayerDstBuffApplyMechanic(GorefrostTarget, new MechanicPlotlySetting(Symbols.CircleCrossOpen, Colors.Red), "Arrow.T", "Targeted by Gorefrost (Arrows)", "Gorefrost Target", 0),
                new PlayerDstHealthDamageHitMechanic(Gorefrost, new MechanicPlotlySetting(Symbols.BowtieOpen, Colors.Orange), "Arrow.H", "Hit by Gorefrost (Arrows)", "Gorefrost Hit", 0),
            ]),
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic([FreezingFan, FreezingFan2], new MechanicPlotlySetting(Symbols.DiamondWide, Colors.Orange), "Frontal.H", "Hit by Freezing Fan (Frontal)", "Freezing Fan Hit", 0),
            ]),
            new MechanicGroup([
                new PlayerDstBuffApplyMechanic(LethalCoalescenceBuff, new MechanicPlotlySetting(Symbols.CircleOpenDot, Colors.Green), "Green.T", "Targeted by Wintry Orb (Green)", "Wintry Orb Target", 500),
                new PlayerDstHealthDamageHitMechanic(WintryOrb, new MechanicPlotlySetting(Symbols.Circle, Colors.Green), "Green.H", "Hit by Wintry Orb (Green)", "Wintry Orb Hit", 0),
                new PlayerDstHealthDamageHitMechanic(HailstormWhisperingShadow, new MechanicPlotlySetting(Symbols.CircleX, Colors.Red), "Spread.H", "Hit by Hailstorm (Spread)", "Hailstorm Hit", 0),
            ]),
            new MechanicGroup([
                new EnemyDstBuffApplyMechanic(EmpoweredWatchknightTriumverate, new MechanicPlotlySetting(Symbols.Square, Colors.Red), "Emp.A", "Gained Empowered", "Empowered Application", 0),
            ]),
            new MechanicGroup([
                new AchievementEligibilityMechanic(Ach_Shatterstep, new MechanicPlotlySetting(Symbols.YDown, Colors.DarkYellow), "Shatterstep.Achiv.L", "Achievement Eligibility: Shatterstep Lost", "Achiv Shatterstep Lost", 0)
                    .UsingChecker((evt, log) => evt.Lost),
                new AchievementEligibilityMechanic(Ach_Shatterstep, new MechanicPlotlySetting(Symbols.YDown, Colors.Yellow), "Shatterstep.Achiv.K", "Achievement Eligibility: Shatterstep Kept", "Achiv Shatterstep Kept", 0)
                    .UsingChecker((evt, log) => !evt.Lost)
            ]),
        ]);
    public WhisperingShadow(int triggerID) : base(triggerID)
    {
        MechanicList.Add(Mechanics);
        Extension = "whispshadow";
        Icon = EncounterIconWhisperingShadow;
        LogID |= 0x000001;
    }

    internal override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log, CombatReplayDecorationContainer arenaDecorations)
    {
        var crMap = new CombatReplayMap(
                        (800, 800),
                        (519, -9425.5, 4214, -5730.5));
        AddArenaDecorationsPerEncounter(log, arenaDecorations, LogID, CombatReplayNoImage, crMap);
        return crMap;
    }

    internal override IReadOnlyList<TargetID> GetTargetsIDs()
    {
        return [
            TargetID.WhisperingShadow,
        ];
    }

    protected SingleActor GetWhisperingShadow()
    {
        return Targets.FirstOrDefault(x => x.IsSpecies(TargetID.WhisperingShadow)) ?? throw new MissingKeyActorsException("Whispering Shadow not found");
    }

    internal override LogData.Mode GetLogMode(CombatData combatData, AgentData agentData, LogData logData)
    {
        if (combatData.GetBuffApplyData(LifeFireCircleCM).Any())
        {
            return LogData.Mode.CM;
        }
        return LogData.Mode.Normal;
    }

    internal static List<PhaseData> ComputePhases(ParsedEvtcLog log, SingleActor shadow, EncounterPhaseData encounterPhase, bool requirePhases)
    {
        if (!requirePhases)
        {
            return [];
        }
        var phases = new List<PhaseData>(7);
        // guttering light queues up at 80%, 50%, 20%
        // we use the first cast as start and stun/breakbar as end
        int i = 1;
        var start = encounterPhase.Start;
        bool isFirst = true;
        var breakbarEnds = log.CombatData.GetBreakbarStateEvents(shadow.AgentItem).Where(x => x.State != ArcDPSEnums.BreakbarState.Active);
        var stuns = log.CombatData.GetBuffApplyDataByIDByDst(Stun, shadow.AgentItem);
        foreach (var cast in shadow.GetAnimatedCastEvents(log).Where(x => x.ActualDuration > 0))
        {
            if (cast.SkillID == GutteringLight || cast.SkillID == GutteringLightCM)
            {
                if (isFirst)
                {
                    var phase = new SubPhasePhaseData(start, cast.Time, "Phase " + i);
                    phase.AddParentPhase(encounterPhase);
                    phase.AddTarget(shadow, log);
                    phases.Add(phase);

                    var stunned = stuns.FirstOrDefault(x => x.Time > phase.End);
                    var broken = breakbarEnds.FirstOrDefault(x => x.Time > phase.End);
                    var end = Math.Min(stunned?.Time ?? encounterPhase.End, broken?.Time ?? long.MaxValue);
                    var split = new SubPhasePhaseData(cast.Time, end, "Darkness " + i);
                    split.AddParentPhase(encounterPhase);
                    split.AddTarget(shadow, log);
                    phases.Add(split);

                    start = end;
                    i++;
                }
                isFirst = false;
            }
            else
            {
                isFirst = true;
            }
        }
        if (start < encounterPhase.End)
        {
            var lastPhase = new SubPhasePhaseData(start, encounterPhase.End, "Phase " + i);
            lastPhase.AddParentPhase(encounterPhase);
            lastPhase.AddTarget(shadow, log);
            phases.Add(lastPhase);
        }
        return phases;
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        var phases = GetInitialPhase(log);
        var shadow = GetWhisperingShadow();
        phases[0].AddTarget(shadow, log);
        phases.AddRange(ComputePhases(log, shadow, (EncounterPhaseData)phases[0], requirePhases));
       
        return phases;
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor player, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputePlayerCombatReplayActors(player, log, replay);
        }

        // life-fire (protective circle)
        // buff & radius are different per tier
        AddLifeFireCircle(player, log, replay, LifeFireCircleT1, 400);
        AddLifeFireCircle(player, log, replay, LifeFireCircleT2, 350);
        AddLifeFireCircle(player, log, replay, LifeFireCircleT3, 325);
        AddLifeFireCircle(player, log, replay, LifeFireCircleT4, 300);
        AddLifeFireCircle(player, log, replay, LifeFireCircleCM, 250);

        // gorefrost (arrow) target
        var gorefrosts = player.GetBuffStatus(log, GorefrostTarget).Where(x => x.Value > 0);
        replay.Decorations.AddOverheadIcons(gorefrosts, player, ParserIcons.TargetOverhead);

        // inevitable darkness (tether) target
        var inevitableDarkness = player.GetBuffStatus(log, InevitableDarknessPlayer).Where(x => x.Value > 0);
        var inevitableDarknessEvents = GetBuffApplyRemoveSequence(log.CombatData, InevitableDarknessPlayer, player, true, false);
        replay.Decorations.AddOverheadIcons(inevitableDarkness, player, BuffImages.SpiritsConsumed);
        replay.Decorations.AddTether(inevitableDarknessEvents, Colors.LightPurple, 0.5);

        // wintry orb (green)
        var wintryOrbs = player.GetBuffStatus(log, LethalCoalescenceBuff).Where(x => x.Value > 0);
        foreach (var segment in wintryOrbs)
        {
            var decoration = new CircleDecoration(240, segment.TimeSpan, Colors.DarkGreen, 0.2, new AgentConnector(player.AgentItem));
            replay.Decorations.AddWithGrowing(decoration, segment.End, true);
        }

        // hailstorm (spread)
        if (log.CombatData.TryGetEffectEventsByDstWithGUID(player.AgentItem, EffectGUIDs.WhisperingShadowHailstorm1, out var hailstorms))
        {
            foreach (var effect in hailstorms)
            {
                var lifespan = effect.ComputeLifespan(log, 6667);
                var decoration = new CircleDecoration(250, lifespan, Colors.Orange, 0.2, new AgentConnector(player.AgentItem));
                replay.Decorations.AddWithGrowing(decoration, lifespan.end);
            }
        }
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeNPCCombatReplayActors(target, log, replay);
        }

        if (target.IsSpecies(TargetID.WhisperingShadow))
        {
            // freezing fan (frontal)
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.WhisperingShadowFreezingFan, out var freezingFans))
            {
                foreach (var effect in freezingFans)
                {
                    var lifespan = effect.ComputeLifespan(log, 1500);
                    if (target.TryGetCurrentFacingDirection(log, effect.Time, out var facing, 300))
                    {
                        var position = new PositionConnector(effect.Position);
                        var rotation = new AngleConnector(facing);
                        var decoration = (FormDecoration)new PieDecoration(1200, 190f, lifespan, Colors.LightOrange, 0.2, position).UsingRotationConnector(rotation);
                        replay.Decorations.AddWithGrowing(decoration, lifespan.end);
                    }
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

        // falling ice
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.WhisperingShadowFallingIce, out var fallingIce))
        {
            foreach (var effect in fallingIce)
            {
                var lifespan = effect.ComputeLifespan(log, 3000);
                var decoration = new CircleDecoration(180, lifespan, Colors.Orange, 0.2, new PositionConnector(effect.Position));
                environmentDecorations.Add(decoration);
            }
        }

        // vitreous spike
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.WhisperingShadowVitreousSpike, out var spikes))
        {
            foreach (var effect in spikes)
            {
                var lifespan = effect.ComputeLifespan(log, 1500);
                var position = new PositionConnector(effect.Position);
                var decoration = new CircleDecoration(130, lifespan, Colors.Orange, 0.2, position);
                environmentDecorations.AddWithGrowing(decoration, lifespan.end);
                environmentDecorations.Add(new IconDecoration(ParserIcons.RedArrowUpOverhead, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.7f, lifespan, position));
            }
        }
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.WhisperingShadowVitreousSpikeDanger, out var spikesDanger))
        {
            foreach (var effect in spikesDanger)
            {
                var lifespan = effect.ComputeLifespan(log, 2000);
                var position = new PositionConnector(effect.Position);
                var decoration = new CircleDecoration(130, lifespan, Colors.Red, 0.2, position);
                environmentDecorations.AddWithGrowing(decoration, lifespan.end);
                environmentDecorations.Add(new IconDecoration(ParserIcons.RedXMarkerOverhead, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.7f, lifespan, position));
            }
        }

        // frozen teeth (fissures)
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.WhisperingShadowFrozenTeethArrows, out var fissureArrows))
        {
            foreach (var effect in fissureArrows)
            {
                // TODO: support arrow decorations
                const uint length = 1100;
                var lifespan = effect.ComputeLifespan(log, 10000);
                var position = new PositionConnector(effect.Position).WithOffset(new(0f, length / 2f, 0f), true);
                var rotation = new AngleConnector(effect.Rotation.Z);
                var decoration = new RectangleDecoration(150, length, lifespan, Colors.Orange, 0.2, position).UsingRotationConnector(rotation);
                environmentDecorations.Add(decoration);
            }
        }
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.WhisperingShadowFrozenTeethFissure, out var fissures))
        {
            foreach (var effect in fissures)
            {
                var lifespan = effect.ComputeLifespan(log, 6333);
                var position = new PositionConnector(effect.Position);
                var rotation = new AngleConnector(effect.Rotation.Z);
                var decoration = new RectangleDecoration(150, 380, lifespan, Colors.LightBlue, 0.2, position).UsingRotationConnector(rotation);
                environmentDecorations.Add(decoration);
            }
        }

        // cryoflash (shockwave)
        // we use the shared shockwave effect and check effect height to distinguish
        // high is at z -3760, low at z -3460
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.WhisperingShadowCryoflashShockwave, out var cryoflashs))
        {
            foreach (var effect in cryoflashs)
            {
                var color = Colors.LightBlue;
                var height = effect.Position.Z;
                if (height < -3600.0f)
                {
                    color = Colors.Ice;
                }
                var lifespan = effect.ComputeLifespan(log, 3033);
                environmentDecorations.AddShockwave(new PositionConnector(effect.Position), lifespan, color, 0.5, 5000);
            }
        }

        // gorefrost (arrow)
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.WhisperingShadowGorefrost1, out var gorefrostArrows))
        {
            foreach (var effect in gorefrostArrows)
            {
                // TODO: support arrow decorations
                const uint length = 1050;
                var lifespan = effect.ComputeLifespan(log, 1500);
                var position = new PositionConnector(effect.Position).WithOffset(new(0f, length / 2f, 0f), true);
                var rotation = new AngleConnector(effect.Rotation.Z);
                var decoration = new RectangleDecoration(50, length, lifespan, Colors.Orange, 0.2, position).UsingRotationConnector(rotation);
                environmentDecorations.Add(decoration);
            }
        }
        var gorefrostMissiles = log.CombatData.GetMissileEventsBySkillID(Gorefrost);
        environmentDecorations.AddNonHomingMissiles(log, gorefrostMissiles, Colors.SkyBlue, 0.2, 50);

        // freezing vortex (rotating aoes)
        var freezingVortexMissiles = log.CombatData.GetMissileEventsBySkillID(FreezingVortex);
        environmentDecorations.AddNonHomingMissiles(log, freezingVortexMissiles, Colors.Red, 0.1, 200);
    }

    private static void AddLifeFireCircle(PlayerActor player, ParsedEvtcLog log, CombatReplay replay, long buff, uint radius)
    {
        var lifefires = player.GetBuffStatus(log, buff).Where(x => x.Value > 0);
        foreach (var lifefire in lifefires)
        {
            var decoration = new CircleDecoration(radius, lifefire, Colors.Ice, 0.05, new AgentConnector(player.AgentItem));
            replay.Decorations.AddWithBorder(decoration);
        }
    }

    internal override void ComputeAchievementEligibilityEvents(ParsedEvtcLog log, Player p, List<AchievementEligibilityEvent> achievementEligibilityEvents)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeAchievementEligibilityEvents(log, p, achievementEligibilityEvents);
        }
        {
            var shatterstepEligibilityEvents = new List<AchievementEligibilityEvent>();
            var whisperingShadowPhases = log.LogData.GetEncounterPhases(log).Where(x => x.ID == LogID && x.IntersectsWindow(p.FirstAware, p.LastAware)).ToHashSet();
            List<HealthDamageEvent> damageData = [
                ..log.CombatData.GetDamageData(LoftedCryoflash),
                ..log.CombatData.GetDamageData(TerrestialCryoflash)
            ];
            damageData.SortByTime();
            foreach (var evt in damageData)
            {
                if (evt.HasHit && evt.To.Is(p.AgentItem) && p.InAwareTimes(evt.Time))
                {
                    InsertAchievementEligibityEventAndRemovePhase(whisperingShadowPhases, shatterstepEligibilityEvents, evt.Time, Ach_Shatterstep, p);
                }
            }
            AddSuccessBasedAchievementEligibityEvents(whisperingShadowPhases, shatterstepEligibilityEvents, Ach_Shatterstep, p);
            achievementEligibilityEvents.AddRange(shatterstepEligibilityEvents);
        }
    }

    internal override void SetInstanceBuffs(ParsedEvtcLog log, List<InstanceBuff> instanceBuffs)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.SetInstanceBuffs(log, instanceBuffs);
        }
        if (log.CombatData.GetBuffData(AchievementEligibilityUndyingLight).Any())
        {
            var encounterPhases = log.LogData.GetEncounterPhases(log).Where(x => x.ID == LogID);
            var lastEncounter = encounterPhases.LastOrDefault();
            if (lastEncounter != null && lastEncounter.Success && lastEncounter.IsCM)
            {
                // The achievement requires 5 players alive and in the instance from the moment challenge mode is activated until the end.
                // The buff is present only on the players that do not have the achievement yet.
                // If any player dies, the buff is removed from everyone.
                // We don't check if players died during the encounter because the elibility is valid for the entire fractal.
                foreach (Player p in log.PlayerList)
                {
                    if (p.HasBuff(log, AchievementEligibilityUndyingLight, lastEncounter.End - ServerDelayConstant))
                    {
                        instanceBuffs.Add(new(log.Buffs.BuffsByIDs[AchievementEligibilityUndyingLight], 1, log.LogData.GetMainPhase(log)));
                        break;
                    }
                }
            }
        }
    }
}
