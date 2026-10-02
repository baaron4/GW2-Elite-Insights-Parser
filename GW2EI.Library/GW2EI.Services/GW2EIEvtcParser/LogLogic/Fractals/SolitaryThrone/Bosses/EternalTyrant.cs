using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.EIData.Mechanic.MechanicSeverity;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.MechanicIDs;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal class EternalTyrant : SolitaryThrone
{
    internal readonly MechanicGroup Mechanics = new([
        new MechanicGroup([
            new PlayerDstBuffApplyMechanic(TargetedEternalTyrant, Mech_CosmicBlastTarget, new (Symbols.CircleXOpen, Colors.White), new("Blast.T", "Targeted by Cosmic Blast (Low Gravity)", "Cosmic Blast Target"), Sev0),
            new PlayerDstHealthDamageHitMechanic(CosmicBlastHit, Mech_CosmicBlastHit, new (Symbols.TriangleUp, Colors.White), new("Blast.H", "Hit by Cosmic Blast (Low Gravity)", "Cosmic Blast Hit"), Sev0),
        ]),
        new MechanicGroup([
            new PlayerDstEffectMechanic(EffectGUIDs.AoEIndicatorFilling280, Mech_GravityFieldTarget, new (Symbols.CircleCrossOpen, Colors.DarkBlue), new("Field.T", "Targeted by Gravity Field", "Gravity Field Target"), Sev0),
            new PlayerDstHealthDamageHitMechanic(GravityFieldHit, Mech_GravityFieldHit, new (Symbols.Circle, Colors.DarkBlue), new("Field.H", "Hit by Gravity Field", "Gravity Field Hit"), Sev1),
        ]),
        new MechanicGroup([
            new PlayerDstHealthDamageHitMechanic(AstralBarrageHit, Mech_AstralBarrageHit, new (Symbols.Star, Colors.Orange), new("Barrage.H", "Hit by Astral Barrage (Small AoE)", "Astral Barrage Hit"), Sev1),
        ]),
        new MechanicGroup([
            new PlayerDstHealthDamageHitMechanic(CosmicWave, Mech_CosmicWaveHit, new (Symbols.TriangleDown, Colors.Orange), new("Wave.H", "Hit by Cosmic Wave (Frontal)", "Cosmic Wave Hit"), Sev0),
        ]),
        new MechanicGroup([
            new PlayerDstHealthDamageHitMechanic(AstralOrb, Mech_AstralOrbHit, new (Symbols.StarTriangleDownOpen, Colors.SkyBlue), new("Orb.H", "Hit by Astral Orb", "Astral Orb Hit"), Sev2),
            new PlayerSrcHealthDamageHitMechanic(AstralOrb, Mech_AstralOrbReflect, new (Symbols.StarTriangleDown, Colors.SkyBlue), new("Orb.Rfl", "Hit reflected Astral Orb", "Astral Orb Reflect"), Sev0),
        ]),
        new MechanicGroup([
            new PlayerDstHealthDamageHitMechanic(AstralPulse, Mech_AstralPulseHit, new (Symbols.BowtieOpen, Colors.SkyBlue), new("Pulse.H", "Hit by Astral Pulse", "Astral Pulse Hit"), Sev2),
        ]),
        new MechanicGroup([
            new PlayerDstHealthDamageHitMechanic(SelfDestruct, Mech_SelfDestructHit, new (Symbols.Bowtie, Colors.Red), new("Destruct.H", "Hit by Self-Destruct (Add Explosion)", "Self-Destruct Hit"), Sev0),
            new PlayerDstHealthDamageHitMechanic(ExplodingEnergy, Mech_ExplodingEnergyHit, new (Symbols.BowtieOpen, Colors.Red), new("Energy.H", "Hit by Exploding Energy (Add Kill)", "Exploding Energy Hit"), Sev2),
        ]),
        new MechanicGroup([
            new PlayerDstHealthDamageHitMechanic(ArcDPSGenericKill, Mech_CelestialImpactHit, new (Symbols.YDown, Colors.Red), new("Impact.H", "Hit by Celestial Impact (Instant Kill)", "Celestial Impact Hit"), Sev0)
                .UsingChecker((hit, log) => hit.From.IsSpecies(TargetID.EternalTyrant) && log.CombatData.GetAnimatedCastData(hit.From).Any(x => (x.SkillID == CelestialImpact || x.SkillID == CelestialImpactFinal) && hit.Time > x.Time && hit.Time <= x.EndTime + ServerDelayConstant)),
        ]),
        new MechanicGroup([
            new PlayerDstHealthDamageHitMechanic(StormSummoning, Mech_StormSummoningHit, new (Symbols.StarDiamond, Colors.Red), new("Summ.H", "Hit by Storm Summoning (Rime Sprite Spawn)", "Storm Summoning Hit"), Sev0),
            new PlayerDstBuffApplyMechanic(AttractingRimeSprites, Mech_AttracingRimeSprites, new (Symbols.Y, Colors.White), new("Fixate", "Fixated by Rime Sprite", "Rime Sprite Fixate"), Sev0),
            new PlayerDstHealthDamageHitMechanic(FrigidWinds, Mech_FrigidWindsHit, new (Symbols.StarDiamondOpen, Colors.White), new("Winds.H", "Hit by Frigid Winds (Rime Sprite)", "Frigid Winds Hit"), Sev1),
        ]),
        new MechanicGroup([
            new PlayerDstHealthDamageHitMechanic(JadeShards, Mech_JadeShardsHit, new (Symbols.StarSquareOpen, Colors.DarkYellow), new("Jade.H", "Hit by Jade Shards (Earth Rings)", "Jade Shards Hit"), Sev1),
            new PlayerDstHealthDamageHitMechanic(SearingRadial, Mech_SearingRadialHit, new (Symbols.StarDiamond, Colors.Orange), new("Sear.H", "Hit by Searing Radial (Fire Wall)", "Searing Radial Hit"), Sev0),
            new PlayerDstHealthDamageHitMechanic(LightningStrikeEternalTyrant, Mech_LightningStrikeHit, new (Symbols.CircleOpenDot, Colors.CobaltBlue), new("Lightning.H", "Hit by Lightning Strike", "Lightning Strike Hit"), Sev0),
        ]),
    ]);

    public EternalTyrant(int triggerID) : base(triggerID)
    {
        MechanicList.Add(Mechanics);
        Extension = "tyrant";
        Icon = EncounterIconEternalTyrant;
        LogID |= 0x000001;
    }

    internal override IReadOnlyList<TargetID> GetTargetsIDs()
    {
        return [
            TargetID.EternalTyrant,
            TargetID.ConstructOfGaldra,
            TargetID.RimeSprite,
            TargetID.FrostElemental,
        ];
    }

    internal override Dictionary<TargetID, int> GetTargetsSortIDs()
    {
        return new()
        {
            { TargetID.EternalTyrant, 0 },
            { TargetID.ConstructOfGaldra, 1 },
            { TargetID.RimeSprite, 2 },
            { TargetID.FrostElemental, 3 },
        };
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        return [
            .. base.GetTrashMobsIDs(),
            TargetID.EarthElemental,
            TargetID.Ember,
            TargetID.SparkEternalTyrant,
        ];
    }

    protected SingleActor GetEternalTyrant()
    {
        return Targets.FirstOrDefault(x => x.IsSpecies(TargetID.EternalTyrant)) ?? throw new MissingKeyActorsException("Eternal Tyrant not found");
    }

    internal override LogData.Mode GetLogMode(CombatData combatData, AgentData agentData, LogData logData)
    {
        const int healthT4 = 12_664_274;
        const int healthThreshold = healthT4 + 1_000_000;

        ulong build = combatData.GetGW2BuildEvent().Build;
        var tyrant = GetEternalTyrant();
        if (build > GW2Builds.September2026NexusOfEternitySolitaryThrone && tyrant.GetHealth(combatData) >= healthThreshold)
        {
            return LogData.Mode.CM;
        }
        return LogData.Mode.Normal;
    }

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, LogData logData, IReadOnlyCollection<AgentItem> playerAgents, LogData.LogSuccessHandler successHandler)
    {
        var tyrant = GetEternalTyrant();
        var dead = combatData.GetDeadEvents(tyrant.AgentItem).FirstOrDefault();
        var invulnEnd = combatData.GetBuffRemoveAllDataByIDByDst(InvulnerabilityEternalTyrant, tyrant.AgentItem).LastOrDefault();
        if (dead != null)
        {
            successHandler.SetSuccess(true, dead.Time);
            return;
        }
        else if (invulnEnd != null)
        {
            var health = combatData.GetHealthUpdateEvents(tyrant.AgentItem).LastOrDefault(x => x.Time <= invulnEnd?.Time);
            if (health?.HealthPercent <= 1.0)
            {
                successHandler.SetSuccess(true, invulnEnd.Time);
                return;
            }
        }
        successHandler.SetSuccess(false, tyrant.LastAware);
    }

    internal static IReadOnlyList<SubPhasePhaseData> ComputePhases(ParsedEvtcLog log, SingleActor tyrant, IReadOnlyList<SingleActor> targets, EncounterPhaseData encounterPhase, bool requirePhases)
    {
        if (!requirePhases)
        {
            return [];
        }
        var phases = GetSubPhasesByInvul(log, InvulnerabilityEternalTyrant, tyrant, true, true);
        var finalCast = log.CombatData.GetAnimatedCastData(tyrant.AgentItem).FirstOrDefault(x => x.SkillID == CelestialImpactFinal);
        var constructs = targets.Where(x => x.IsSpecies(TargetID.ConstructOfGaldra));
        var sprites = targets.Where(x => x.IsSpecies(TargetID.RimeSprite));
        var elementals = targets.Where(x => x.IsSpecies(TargetID.FrostElemental));
        for (var i = 0; i < phases.Count; i++)
        {
            PhaseData phase = phases[i];
            phase.AddParentPhase(encounterPhase);
            phase.AddTarget(tyrant, log);
            phase.AddTargets(constructs, log, PhaseData.TargetPriority.NonBlocking);
            if (i % 2 == 0)
            {
                phase.Name = "Phase " + (i + 2) / 2;
                phase.AddTargets(sprites, log, PhaseData.TargetPriority.NonBlocking);
            }
            else
            {
                if (finalCast != null && phase.InInterval(finalCast.Time))
                {
                    phase.Name = "Final";
                }
                else
                {
                    phase.Name = "Split " + (i + 1) / 2;
                    phase.AddTargets(elementals, log, PhaseData.TargetPriority.NonBlocking);
                }
            }
        }
        return phases;
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        var phases = GetInitialPhase(log);
        var tyrant = GetEternalTyrant();
        var constructs = Targets.Where(x => x.IsSpecies(TargetID.ConstructOfGaldra));
        phases[0].AddTarget(tyrant, log);
        phases[0].AddTargets(constructs, log, PhaseData.TargetPriority.NonBlocking);
        phases.AddRange(ComputePhases(log, tyrant, Targets, (EncounterPhaseData)phases[0], requirePhases));
        return phases;
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor player, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputePlayerCombatReplayActors(player, log, replay);
        }

        // above platform
        const float platformZ = -2447.3f;
        const float aboveThreshold = 150f;
        long? aboveStart = null;
        foreach (var pos in player.GetCombatReplayPolledPositions(log))
        {
            var height = platformZ - pos.XYZ.Z;
            if (height >= aboveThreshold)
            {
                aboveStart ??= pos.Time;
            }
            else if (aboveStart != null)
            {
                replay.Decorations.AddOverheadIcon(new Segment(aboveStart.Value, pos.Time), player, ParserIcons.GenericBlueArrowUp);
                aboveStart = null;
            }
        }
        if (aboveStart != null)
        {
            replay.Decorations.AddOverheadIcon(new Segment(aboveStart.Value, player.LastAware), player, ParserIcons.GenericBlueArrowUp);
        }

        // cosmic blast (launch aoe) target
        foreach (var seg in player.GetBuffStatus(log, TargetedEternalTyrant).Where(x => x.Value > 0))
        {
            replay.Decorations.AddOverheadIcon(seg, player, ParserIcons.TargetOverhead);
        }

        // gravity field (placed aoe) indicator
        if (log.CombatData.TryGetEffectEventsByDstWithGUID(player.AgentItem, EffectGUIDs.EternalTyrantGravityFieldIndicator, out var gravityFieldIndicators))
        {
            foreach (var effect in gravityFieldIndicators)
            {
                var lifespan = effect.ComputeLifespan(log, 6000);
                var decoration = new CircleDecoration(450, lifespan, Colors.LightOrange, 0.2, new AgentConnector(player));
                replay.Decorations.AddWithGrowing(decoration, lifespan.end);
            }
        }

        // attracting rime sprites (storm fixation)
        foreach (var seg in player.GetBuffStatus(log, AttractingRimeSprites).Where(x => x.Value > 0))
        {
            replay.Decorations.AddOverheadIcon(seg, player, ParserIcons.FixationPurpleOverhead);
        }

        // lightning strike
        if (log.CombatData.TryGetEffectEventsByDstWithGUID(player.AgentItem, EffectGUIDs.EternalTyrantLightningStrike, out var lightningStrikes))
        {
            foreach (var effect in lightningStrikes)
            {
                var lifespan = effect.ComputeLifespan(log, 2000);
                var decoration = new CircleDecoration(180, lifespan, Colors.LightOrange, 0.2, new AgentConnector(player)).UsingFilled(false);
                replay.Decorations.AddWithFilledWithGrowing(decoration, true, lifespan.end);
            }
        }
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeNPCCombatReplayActors(target, log, replay);
        }

        switch (target.ID)
        {
            case (int)TargetID.EternalTyrant:

                // cosmic blast (low gravity aoe)
                var cosmicBlastMissiles = log.CombatData.GetMissileEventsBySrcBySkillID(target.AgentItem, CosmicBlast);
                replay.Decorations.AddNonHomingMissiles(log, cosmicBlastMissiles, Colors.White, 0.2, 100);
                AddCosmisBlastFields(log, target, replay);
                // astral orb (basic attack projectiles)
                var astralOrbs = log.CombatData.GetMissileEventsBySkillID(AstralOrb);
                replay.Decorations.AddReflectableNonHomingMissiles(log, astralOrbs, Colors.LightBlue, 0.2, Colors.Grey, 0.3, 50);

                // astral pulse (rotating projectile beam)
                var astralPulses = log.CombatData.GetMissileEventsBySkillID(AstralPulse);
                replay.Decorations.AddNonHomingMissiles(log, astralPulses, Colors.White, 0.2, 25);

                // astral barrage (small aoes)
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.EternalTyrantAstralBarrage1, out var astralBarrages))
                {
                    foreach (var effect in astralBarrages)
                    {
                        var lifespan = effect.ComputeLifespan(log, 1500);
                        var decoration = new CircleDecoration(100, lifespan, Colors.LightOrange, 0.2, new PositionConnector(effect.Position));
                        replay.Decorations.Add(decoration);
                    }
                }

                // cosmic wave (frontal)
                const uint waveRadius = 600;
                const uint waveAngle = 45;
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.EternalTyrantCosmicWaveIndicator, out var cosmicWaveIndicators))
                {
                    foreach (var effect in cosmicWaveIndicators)
                    {
                        var lifespan = effect.ComputeLifespan(log, 2000);
                        var decoration = new PieDecoration(waveRadius, waveAngle, lifespan, Colors.LightOrange, 0.2, new PositionConnector(effect.Position))
                             .UsingRotationConnector(new AngleConnector(effect.Rotation.Z + 90));
                        replay.Decorations.Add(decoration);
                    }
                }
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.EternalTyrantCosmicWave, out var cosmicWaves))
                {
                    foreach (var effect in cosmicWaves)
                    {
                        var lifespan = effect.ComputeLifespan(log, 2000);
                        var decoration = new PieDecoration(waveRadius, waveAngle, lifespan, Colors.Red, 0.2, new PositionConnector(effect.Position))
                               .UsingRotationConnector(new AngleConnector(effect.Rotation.Z + 90));
                        replay.Decorations.Add(decoration);
                    }
                }

                // celestial impact (instant kill)
                const uint impactRadius = 2500;
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.EternalTyrantCelestialImpactIndicator, out var celestialImpactIndicators))
                {
                    foreach (var effect in celestialImpactIndicators)
                    {
                        var lifespan = effect.ComputeLifespan(log, 43000);
                        var decoration = new CircleDecoration(impactRadius, lifespan, Colors.Orange, 0.1, new PositionConnector(effect.Position));
                        replay.Decorations.AddWithGrowing(decoration, lifespan.end);
                    }
                }
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.EternalTyrantCelestialImpactIndicatorFast, out var celestialImpactFastIndicators))
                {
                    foreach (var effect in celestialImpactFastIndicators)
                    {
                        var lifespan = effect.ComputeLifespan(log, 6200);
                        var decoration = new CircleDecoration(impactRadius, lifespan, Colors.Orange, 0.1, new PositionConnector(effect.Position));
                        replay.Decorations.AddWithGrowing(decoration, lifespan.end);
                    }
                }
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.EternalTyrantCelestialImpact, out var celestialImpacts))
                {
                    foreach (var effect in celestialImpacts)
                    {
                        var lifespan = effect.ComputeLifespan(log, 3333);
                        var decoration = new CircleDecoration(impactRadius, lifespan, Colors.Red, 0.2, new PositionConnector(effect.Position));
                        replay.Decorations.Add(decoration);
                    }
                }
                break;
            case (int)TargetID.RimeSprite:
                {
                    foreach (var seg in target.GetBuffStatus(log, RimeSpriteAura).Where(x => x.Value > 0))
                    {
                        var decoration = new CircleDecoration(300, (seg.Start, seg.End), Colors.Red, 0.2, new AgentConnector(target))
                            .UsingFilled(false);
                        replay.Decorations.Add(decoration);
                    }
                    break;
                }
            case (int)TargetID.EarthElemental:
                {
                    AddCosmisBlastFields(log, target, replay);
                    var breakbarUpdates = target.GetBreakbarPercentUpdates(log);
                    var (_, breakbarActives, _, _) = target.GetBreakbarStatus(log);
                    foreach (var seg in breakbarActives)
                    {
                        replay.Decorations.AddActiveBreakbar(seg.TimeSpan, target, breakbarUpdates);
                    }
                    break;
                }
            case (int)TargetID.Ember:
            case (int)TargetID.FrostElemental:
            case (int)TargetID.SparkEternalTyrant:
                {
                    AddCosmisBlastFields(log, target, replay);
                    var lifespan = (target.FirstAware, target.LastAware);
                    var width = CombatReplayOverheadProgressBarMajorSizeInPixel;
                    var progress = target.GetHealthUpdates(log).Select(x => (x.Start, x.Value)).ToList();
                    var decoration = new OverheadProgressBarDecoration(width, lifespan, Colors.Green, 0.8, Colors.Black, 0.6, progress, new AgentConnector(target))
                        .UsingInterpolationMethod(Connector.InterpolationMethod.Step)
                        .UsingRotationConnector(new AngleConnector(180));
                    replay.Decorations.Add(decoration);
                    break;
                }
        }
    }

    private static void AddCosmisBlastFields(ParsedEvtcLog log, SingleActor target, CombatReplay replay)
    {
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.EternalTyrantCosmicBlast, out var cosmicBlastFields))
        {
            foreach (var effect in cosmicBlastFields)
            {
                var lifespan = effect.ComputeLifespan(log, 10000);
                var decoration = new CircleDecoration(100, lifespan, Colors.White, 0.2, new PositionConnector(effect.Position));
                replay.Decorations.AddWithBorder(decoration, Colors.Red, 0.2);
            }
        }
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);
        }

        // gravity field (placed aoe)
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.EternalTyrantGravityFieldOutline, out var gravityFields))
        {
            foreach (var effect in gravityFields)
            {
                var lifespan = effect.ComputeLifespan(log, 120000);
                var decoration = new CircleDecoration(450, lifespan, Colors.DarkBlue, 0.1, new PositionConnector(effect.Position));
                environmentDecorations.AddWithBorder(decoration, Colors.Red, 0.2);
            }
        }


        // searing radial (fire wall)
        // TODO: indicator collision with construct
        const uint wallWidth = 50;
        const uint wallLength = 2000;
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.ArrowIndicator, out var searingRadialIndiactors))
        {
            foreach (var effect in searingRadialIndiactors)
            {
                var lifespan = effect.ComputeLifespan(log, 2000);
                var position = new PositionConnector(effect.Position).WithOffset(wallLength / 2f * Vector3.UnitY, true);
                var decoration = new RectangleDecoration(wallWidth, wallLength, lifespan, Colors.LightOrange, 0.2, position)
                    .UsingRotationConnector(new AngleConnector(effect.Rotation.Z + 90));
                environmentDecorations.Add(decoration);
            }
        }
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.EternalTyrantSearingRadial, out var searingRadials))
        {
            foreach (var effect in searingRadials)
            {
                var lifespan = effect.ComputeLifespan(log, 24590);
                var position = new PositionConnector(effect.Position).WithOffset(wallLength / 2f * Vector3.UnitY, true);
                var decoration = new RectangleDecoration(wallWidth, wallLength, lifespan, Colors.Red, 0.2, position)
                    .UsingRotationConnector(new SpinningConnector(effect.Rotation.Z + 90, 360));
                environmentDecorations.Add(decoration);
            }
        }

        // storm summoning (rime sprite spawn)
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.EternalTyrantStormSummoning, out var stormSummons))
        {
            foreach (var effect in stormSummons)
            {
                var lifespan = effect.ComputeLifespan(log, 3000);
                var decoration = new CircleDecoration(180, lifespan, Colors.Orange, 0.2, new PositionConnector(effect.Position));
                environmentDecorations.Add(decoration);
            }
        }

        // jade shards (earth rings)
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.EternalTyrantJadeShards, out var jadeShards))
        {
            uint[] rings = [140, 260, 440, 780, 1340]; // TODO: double check
            const long delay = 2000;
            const long interval = 1000;
            foreach (var effect in jadeShards)
            {
                var position = new PositionConnector(effect.Position);
                var time = effect.Time + delay;
                for (var i = 0; i < rings.Length; i++)
                {
                    var inner = i > 0 ? rings[i - 1] : 0;
                    var outer = rings[i];
                    var decoration = new DoughnutDecoration(inner, outer, (time, time + 500), Colors.Red, 0.2, position);
                    environmentDecorations.Add(decoration);
                    time += interval;
                }
            }
        }
    }
}
