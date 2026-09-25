using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.EIData.Mechanic.MechanicSeverity;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.MechanicIDs;
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
        ];
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        return [
            .. base.GetTrashMobsIDs(),
            TargetID.SparkEternalTyrant,
            TargetID.Ember,
            TargetID.FrostElemental,
        ];
    }

    protected SingleActor GetEternalTyrant()
    {
        return Targets.FirstOrDefault(x => x.IsSpecies(TargetID.EternalTyrant)) ?? throw new MissingKeyActorsException("Eternal Tyrant not found");
    }

    internal override LogData.Mode GetLogMode(CombatData combatData, AgentData agentData, LogData logData)
    {
        const int healthT4 = 40_000_000;
        const int healthThreshold = healthT4 + 1_000_000;

        ulong build = combatData.GetGW2BuildEvent().Build;
        var tyrant = GetEternalTyrant();
        if (build > GW2Builds.September2026NexusOfEternitySolitaryThrone && tyrant.GetHealth(combatData) >= healthThreshold)
        {
            return LogData.Mode.CM;
        }
        return LogData.Mode.Normal;
    }

    internal static IReadOnlyList<SubPhasePhaseData> ComputePhases(ParsedEvtcLog log, SingleActor tyrant, EncounterPhaseData encounterPhase, bool requirePhases)
    {
        if (!requirePhases)
        {
            return [];
        }
        var phases = GetSubPhasesByInvul(log, Determined762, tyrant, true, true, encounterPhase.Start, encounterPhase.End, 4000);
        for (int i = 1; i < phases.Count; i++)
        {
            PhaseData phase = phases[i];
            phase.AddParentPhase(phases[0]);
            if (i % 2 == 0)
            {
                phase.Name = "Split " + i / 2;
            }
            else
            {
                phase.Name = "Phase " + (i + 1) / 2;
                phase.AddTarget(tyrant, log);
            }
        }
        return phases;
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        var phases = GetInitialPhase(log);
        var tyrant = GetEternalTyrant();
        phases[0].AddTarget(tyrant, log);
        phases.AddRange(ComputePhases(log, tyrant, (EncounterPhaseData)phases[0], requirePhases));
        return phases;
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor player, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputePlayerCombatReplayActors(player, log, replay);

        // cosmic blast (launch aoe) target
        foreach (Segment seg in player.GetBuffStatus(log, TargetedEternalTyrant).Where(x => x.Value > 0))
        {
            replay.Decorations.AddOverheadIcon(seg, player, ParserIcons.TargetOverhead);
        }

        // cosmic blast (launch aoe) low gravity
        foreach (Segment seg in player.GetBuffStatus(log, LowGravity).Where(x => x.Value > 0))
        {
            replay.Decorations.AddOverheadIcon(seg, player, ParserIcons.GenericBlueArrowUp);
        }

        // gravity field (placed aoe) indicator
        if (log.CombatData.TryGetEffectEventsByDstWithGUID(player.AgentItem, EffectGUIDs.AoEIndicatorFilling280, out var gravityFieldIndicators))
        {
            foreach (var effect in gravityFieldIndicators)
            {
                var lifespan = effect.ComputeLifespan(log, 3000);
                var decoration = new CircleDecoration(280, lifespan, Colors.LightOrange, 0.2, new AgentConnector(player.AgentItem));
                replay.Decorations.AddWithFilledWithGrowing(decoration, true, lifespan.end);
            }
        }
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);

        // cosmic blast (low gravity aoe)
        var cosmicBlastMissiles = log.CombatData.GetMissileEventsBySkillID(CosmicBlast);
        environmentDecorations.AddNonHomingMissiles(log, cosmicBlastMissiles, Colors.White, 0.2, 100);
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.EternalTyrantCosmicBlast, out var cosmicBlastFields))
        {
            foreach (var effect in cosmicBlastFields)
            {
                var lifespan = effect.ComputeLifespan(log, 10000);
                var decoration = new CircleDecoration(100, lifespan, Colors.White, 0.2, new PositionConnector(effect.Position));
                environmentDecorations.AddWithBorder(decoration, Colors.Red, 0.2);
            }
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

        // astral barrage (small aoes)
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.EternalTyrantAstralBarrage1, out var astralBarrages))
        {
            foreach (var effect in astralBarrages)
            {
                var lifespan = effect.ComputeLifespan(log, 1500);
                var decoration = new CircleDecoration(100, lifespan, Colors.LightOrange, 0.2, new PositionConnector(effect.Position));
                environmentDecorations.Add(decoration);
            }
        }

        // cosmic wave (frontal)
        const uint waveRadius = 600;
        const uint waveAngle = 45;
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.EternalTyrantCosmicWaveIndicator, out var cosmicWaveIndicators))
        {
            foreach (var effect in cosmicWaveIndicators)
            {
                var lifespan = effect.ComputeLifespan(log, 2000);
                var decoration = new PieDecoration(waveRadius, waveAngle, lifespan, Colors.LightOrange, 0.2, new PositionConnector(effect.Position))
                     .UsingRotationConnector(new AngleConnector(effect.Rotation.Z + 90));
                environmentDecorations.Add(decoration);
            }
        }
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.EternalTyrantCosmicWave, out var cosmicWaves))
        {
            foreach (var effect in cosmicWaves)
            {
                var lifespan = effect.ComputeLifespan(log, 2000);
                var decoration = new PieDecoration(waveRadius, waveAngle, lifespan, Colors.Red, 0.2, new PositionConnector(effect.Position))
                       .UsingRotationConnector(new AngleConnector(effect.Rotation.Z + 90));
                environmentDecorations.Add(decoration);
            }
        }

        // astral orb (basic attack projectiles)
        var astralOrbs = log.CombatData.GetMissileEventsBySkillID(AstralOrb);
        environmentDecorations.AddReflectableNonHomingMissiles(log, astralOrbs, Colors.LightBlue, 0.2, Colors.Grey, 0.3, 50);
    }
}
