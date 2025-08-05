using System.Drawing;
using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;
using static GW2EIEvtcParser.EIData.Trigonometry;

namespace GW2EIEvtcParser.EncounterLogic;

internal class IcebroodConstruct : Grothmar
{
    public IcebroodConstruct(int triggerID) : base(triggerID)
    {
        MechanicList.Add(new MechanicGroup([   
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(IceArmSwing, new MechanicPlotlySetting(Symbols.Star, Colors.Orange), "A.Swing", "Hit by Ice Arm Swing (Spin)", "Ice Arm Swing", 0)
                .WithStabilitySubMechanic(
                    new PlayerDstHealthDamageHitMechanic(IceArmSwing, new MechanicPlotlySetting(Symbols.Star, Colors.Yellow), "ArmSwing.CC", "Knocked by Ice Arm Swing (Spin)", "Ice Arm Swing", 0),
                    false
                ),
                new EnemyCastEndMechanic(IceArmSwing, new MechanicPlotlySetting(Symbols.Star, Colors.White), "Ice Arm Swing", "Cast Ice Arm Swing (Spin)", "Cast Ice Arm Swing", 0),
            ]),
            new PlayerDstHealthDamageHitMechanic(IceShatter, new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Pink), "Ice Orbs", "Hit by Rotating Ice Shatter (Orbs)", "Ice Shatter (Orbs)", 50),
            new PlayerDstHealthDamageHitMechanic(IceCrystal, new MechanicPlotlySetting(Symbols.Circle, Colors.LightOrange), "I.Crystal", "Hit by Ice Crystal (Chill AoE)", "Ice Crystal", 50),
            new PlayerDstHealthDamageHitMechanic(Frostbite, new MechanicPlotlySetting(Symbols.Square, Colors.Blue), "Frostbite.H", "Hit by Frostbite", "Frostbite", 0),
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic([IceFrail1, IceFrail2], new MechanicPlotlySetting(Symbols.Square, Colors.Orange), "I.Flail", "Hit by Ice Flail (Arm Swipe)", "Ice Flail", 50)
                .WithStabilitySubMechanic(
                    new PlayerDstHealthDamageHitMechanic([IceFrail1, IceFrail2], new MechanicPlotlySetting(Symbols.Square, Colors.Yellow), "IceFlail.CC", "Knocked by Ice Flail (Arm Swipe)", "Ice Flail", 50),
                    false
                )
            ]),
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(DeadlyIceShockWave, new MechanicPlotlySetting(Symbols.CircleOpen, Colors.Red), "D.IceWave", "Hit by Deadly Ice Shock Wave", "Deadly Ice Shock Wave", 0),
                new EnemyCastEndMechanic(DeadlyIceShockWave, new MechanicPlotlySetting(Symbols.CircleOpen, Colors.White), "Deadly Ice Shock Wave", "Cast Deadly Ice Shock Wave", "Cast Deadly Ice Shock Wave", 0),
                new PlayerDstHealthDamageHitMechanic([IceShockWave1, IceShockWave2, IceShockWave3], new MechanicPlotlySetting(Symbols.CircleOpen, Colors.LightOrange), "ShockWave.H", "Hit by Ice Shock Wave", "Ice Shock Wave", 0),
            ]),
            new PlayerDstHealthDamageHitMechanic([SpinningIce1, SpinningIce2, SpinningIce3, SpinningIce4], new MechanicPlotlySetting(Symbols.CircleOpenDot, Colors.White), "SpinIce.H", "Hit by Spinning Ice", "Spinning Ice", 0),
        ])
        );
        Extension = "icebrood";
        Icon = EncounterIconIcebroodConstruct;
        EncounterCategoryInformation.InSubCategoryOrder = 0;
        EncounterID |= 0x000001;
    }

    protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
    {
        return new CombatReplayMap(CombatReplayIcebroodConstruct,
                        (729, 581),
                        (-32118, -11470, -28924, -8274)/*,
                        (-0, -0, 0, 0),
                        (0, 0, 0, 0)*/);
    }
    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        return
        [
            new DamageCastFinder(FrostbiteAuraIcebroodConstruct, FrostbiteAuraIcebroodConstruct),
        ];
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.IcebroodConstruct)) ?? throw new MissingKeyActorsException("Icebrood Construct not found");
        phases[0].AddTarget(mainTarget, log);
        if (!requirePhases)
        {
            return phases;
        }
        // Invul check
        phases.AddRange(GetPhasesByInvul(log, Invulnerability757, mainTarget, false, true));
        for (int i = 1; i < phases.Count; i++)
        {
            PhaseData phase = phases[i];
            phase.AddParentPhase(phases[0]);
            phase.Name = "Phase " + i;
            phase.AddTarget(mainTarget, log);
        }
        return phases;
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputeNPCCombatReplayActors(target, log, replay);

        //(long start, long end) lifespan;

        // TODO finish the replay

        switch (target.ID)
        {
            case (int)TargetID.IcebroodConstruct:
                var casts = target.GetAnimatedCastEvents(log, target.FirstAware, target.LastAware);
                foreach (AnimatedCastEvent cast in casts)
                {
                    switch (cast.SkillID)
                    {
                        case IceArmSwing:
                            /* 
                            if (!log.CombatData.HasEffectData)
                            {
                                var duration = 2250;
                                lifespan = (cast.Time, cast.Time + duration);
                                if (target.TryGetCurrentFacingDirection(log, cast.Time + 500, out Vector3 facing))
                                {
                                    // TODO rotate this
                                    // var cone = new PieDecoration(1600, 195, lifespan, Colors.Orange, 0.2, new AgentConnector(target.AgentItem)).UsingRotationConnector(new AngleConnector(facing));
                                    // replay.Decorations.Add(cone);
                                }
                            }
                           */
                            break;
                        default:
                            break;
                    }
                }
                break;
            default:
                break;
        }
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);

        (long start, long end) lifespan;

        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.IcebroodConstructIceShockWave1, out var iceShockWave))
        {
            foreach (EffectEvent effect in iceShockWave)
            {
                int pulseCycle = 1000;
                lifespan = effect.ComputeLifespan(log, 3000);

                var connector = new PositionConnector(effect.Position);
                var circle = new CircleDecoration(200, lifespan, Colors.LightOrange, 0.2, connector);
                environmentDecorations.Add(circle);
                environmentDecorations.AddShockwave(connector, (lifespan.end, lifespan.end + 2000), Colors.Blue, 0.3, 1200);

                (long start, long end) pulse = (lifespan.start, lifespan.start + pulseCycle);
                for (int i = 0; i < 3; i++)
                {
                    environmentDecorations.AddShockwave(connector, pulse, Colors.LightOrange, 0.2, 500);
                    pulse = (pulse.end, pulse.end + pulseCycle);
                }
            }
        }

        var spinningIce = log.CombatData.GetMissileEventsBySkillIDs([SpinningIce1, SpinningIce2, SpinningIce3, SpinningIce4]);
        environmentDecorations.AddRotatingAroundTargetMissiles(log, spinningIce, Colors.White, 0.4, 50, (float)(Math.PI / 2.0), true);

        var iceShatter = log.CombatData.GetMissileEventsBySkillID(IceShatter);
        environmentDecorations.AddNonHomingMissiles(log, iceShatter, Colors.Ice, 0.5, 25);
    }
}
