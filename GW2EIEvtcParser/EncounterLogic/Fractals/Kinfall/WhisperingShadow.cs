using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SpeciesIDs;
using static GW2EIEvtcParser.SkillIDs;
using GW2EIEvtcParser.ParserHelpers;

namespace GW2EIEvtcParser.EncounterLogic;

internal class WhisperingShadow : Kinfall
{
    public WhisperingShadow(int triggerID) : base(triggerID)
    {
        MechanicList.Add(new MechanicGroup([
            new MechanicGroup([
                new PlayerDstBuffApplyMechanic(DeathlyGrime, new MechanicPlotlySetting(Symbols.Diamond, Colors.Purple), "DeathGr.A", "Gained Deathly Grime", "Deathly Grime Application", 0),
                new PlayerDstBuffApplyMechanic([LifeFireCircleT1, LifeFireCircleT2, LifeFireCircleT3, LifeFireCircleT4], new MechanicPlotlySetting(Symbols.Pentagon, Colors.LightBlue), "LifeFire.A", "Gained Life-Fire Circle", "Life-Fire Circle Apply", 0),
                new PlayerDstBuffRemoveMechanic([LifeFireCircleT1, LifeFireCircleT2, LifeFireCircleT3, LifeFireCircleT4], new MechanicPlotlySetting(Symbols.PentagonOpen, Colors.LightBlue), "LifeFire.R", "Lost Life-Fire Circle", "Life-Fire Circle Remove", 0),
            ]),
            new MechanicGroup([
                new PlayerDstHitMechanic([VitreousSpikeHit1, VitreousSpikeHit2], new MechanicPlotlySetting(Symbols.TriangleUp, Colors.SkyBlue), "Spike.H", "Hit by Vitreous Spike", "Vitreous Spike Hit", 0),
            ]),
            new MechanicGroup([
                new PlayerDstHitMechanic(FrozenTeeth, new MechanicPlotlySetting(Symbols.XThinOpen, Colors.SkyBlue), "Fissure.H", "Hit by Frozen Teeth (Fissures)", "Frozen Teeth Hit", 0),
                new PlayerDstHitMechanic(LoftedCryoflash, new MechanicPlotlySetting(Symbols.StarTriangleDownOpen, Colors.Red), "HighCryo.H", "Hit by Lofted Cryoflash (High Shockwave)", "Lofted Cryoflash Hit", 0),
                new PlayerDstHitMechanic(TerrestialCryoflash, new MechanicPlotlySetting(Symbols.StarTriangleUpOpen, Colors.Red), "LowCryo.H", "Hit by Terrestrial Cryoflash (Low Shockwave)", "Terrestrial Cryoflash Hit", 0),
            ]),
            new MechanicGroup([
                new PlayerDstBuffApplyMechanic(GorefrostTarget, new MechanicPlotlySetting(Symbols.CircleCrossOpen, Colors.Red), "Arrow.A", "Targeted by Gorefrost (Arrows)", "Gorefrost Target", 0),
                new PlayerDstHitMechanic(Gorefrost, new MechanicPlotlySetting(Symbols.BowtieOpen, Colors.Orange), "Arrow.H", "Hit by Gorefrost (Arrows)", "Gorefrost Hit", 0),
            ]),
            new MechanicGroup([
                new PlayerDstHitMechanic([FreezingFan, FreezingFan2], new MechanicPlotlySetting(Symbols.DiamondWide, Colors.Orange), "Frontal.H", "Hit by Freezing Fan (Frontal)", "Freezing Fan Hit", 0),
            ]),
            new MechanicGroup([
                new EnemyDstBuffApplyMechanic(EmpoweredWatchknightTriumverate, new MechanicPlotlySetting(Symbols.Square, Colors.Red), "Emp.A", "Gained Empowered", "Empowered Application", 0),
            ]),
        ]));
        Extension = "whispshadow";
        Icon = EncounterIconGeneric;
        EncounterID |= 0x000001;
    }

    protected override IReadOnlyList<TargetID> GetTargetsIDs()
    {
        return [
            TargetID.WhisperingShadow,
        ];
    }

    protected SingleActor GetWhisperingShadow()
    {
        return Targets.FirstOrDefault(x => x.IsSpecies(TargetID.WhisperingShadow)) ?? throw new MissingKeyActorsException("Whispering Shadow not found");
    }

    internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
    {
        const uint healthT4 = 19_082_024;
        var shadow = GetWhisperingShadow();
        if (shadow.GetHealth(combatData) > healthT4 + 100_000)
        {
            return FightData.EncounterMode.CM;
        }
        return FightData.EncounterMode.Normal;
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        var phases = GetInitialPhase(log);
        var shadow = GetWhisperingShadow();
        phases[0].AddTarget(shadow, log);
        if (!requirePhases)
        {
            return phases;
        }

        // breakbars queue up at 80%, 50%, 20%
        var (_, breakbarActives, _, _) = shadow.GetBreakbarStatus(log);
        if (breakbarActives.Count > 0)
        {
            int i = 1;
            var start = phases[0].Start;
            foreach (var breakbarActive in breakbarActives)
            {
                var phase = new PhaseData(start, breakbarActive.Start, "Phase " + i);
                phase.AddTarget(shadow, log);
                phases.Add(phase);
                start = phase.End;
                i++;
            }
            var finalPhase = new PhaseData(start, phases[0].End, "Phase " + i);
            finalPhase.AddTarget(shadow, log);
            phases.Add(finalPhase);
        }
        return phases;
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor player, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputePlayerCombatReplayActors(player, log, replay);

        // life-fire (protective circle)
        // buff & radius are different per tier
        AddLifeFireCircle(player, log, replay, LifeFireCircleT1, 400);
        AddLifeFireCircle(player, log, replay, LifeFireCircleT2, 350);
        AddLifeFireCircle(player, log, replay, LifeFireCircleT3, 325);
        AddLifeFireCircle(player, log, replay, LifeFireCircleT4, 300);

        // gorefrost (arrow) target
        var gorefrosts = player.GetBuffStatus(log, GorefrostTarget, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0);
        replay.Decorations.AddOverheadIcons(gorefrosts, player, ParserIcons.TargetOverhead);

        // inevitable darkness (tether) target
        var inevitableDarkness = player.GetBuffStatus(log, InevitableDarknessPlayer, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0);
        var inevitableDarknessEvents = GetFilteredList(log.CombatData, InevitableDarknessPlayer, player, true, false);
        replay.Decorations.AddOverheadIcons(inevitableDarkness, player, BuffImages.SpiritsConsumed);
        replay.Decorations.AddTether(inevitableDarknessEvents, Colors.LightPurple, 0.5);
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputeNPCCombatReplayActors(target, log, replay);

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

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log)
    {
        base.ComputeEnvironmentCombatReplayDecorations(log);

        // vitreous spike
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.WhisperingShadowVitreousSpike, out var spikes))
        {
            foreach (var effect in spikes)
            {
                var lifespan = effect.ComputeLifespan(log, 1500);
                var decoration = new CircleDecoration(130, lifespan, Colors.Orange, 0.2, new PositionConnector(effect.Position));
                EnvironmentDecorations.AddWithGrowing(decoration, lifespan.end);
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
                EnvironmentDecorations.Add(decoration);
            }
        }
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.WhisperingShadowFrozenTeethFissure, out var fissures))
        {
            foreach (var effect in fissures)
            {
                var lifespan = effect.ComputeLifespan(log, 6333);
                var position = new PositionConnector(effect.Position);
                var rotation = new AngleConnector(effect.Rotation.Z);
                var decoration = new RectangleDecoration(150, 380, lifespan, Colors.Red, 0.2, position).UsingRotationConnector(rotation);
                EnvironmentDecorations.Add(decoration);
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
                EnvironmentDecorations.AddShockwave(new PositionConnector(effect.Position), lifespan, color, 0.5, 5000);
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
                EnvironmentDecorations.Add(decoration);
            }
        }
        var gorefrostMissiles = log.CombatData.GetMissileEventsBySkillID(Gorefrost);
        EnvironmentDecorations.AddNonHomingMissiles(log, gorefrostMissiles, Colors.SkyBlue, 0.2, 50);

        // freezing vortex (rotating aoes)
        var freezingVortexMissiles = log.CombatData.GetMissileEventsBySkillID(FreezingVortex);
        EnvironmentDecorations.AddNonHomingMissiles(log, freezingVortexMissiles, Colors.Red, 0.1, 200);
    }

    protected static void AddLifeFireCircle(PlayerActor player, ParsedEvtcLog log, CombatReplay replay, long buff, uint radius)
    {
        var lifefires = player.GetBuffStatus(log, buff, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0);
        foreach (var lifefire in lifefires)
        {
            replay.Decorations.Add(new CircleDecoration(radius, lifefire, Colors.Ice, 0.07, new AgentConnector(player.AgentItem)));
        }
    }
}
