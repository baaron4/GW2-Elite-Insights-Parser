using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.Mechanic.MechanicSeverity;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.MechanicIDs;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal class IcebroodConstruct : Grothmar
{
    public IcebroodConstruct(int triggerID) : base(triggerID)
    {
        MechanicList.Add(new MechanicGroup([
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(IceArmSwing, Mech_IceArmSwing, new (Symbols.Star, Colors.Orange), new ("A.Swing", "Hit by Ice Arm Swing (Spin)", "Ice Arm Swing"), Sev2)
                .WithStabilitySubMechanic(
                    new SubMechanic(Mech_IceArmSwingCC, new (Symbols.Star, Colors.Yellow), new ("ArmSwing.CC", "Knocked by Ice Arm Swing (Spin)", "Ice Arm Swing"), Sev0),
                    false
                ),
                new EnemyCastEndMechanic(IceArmSwing, Mech_IceArmSwingCast, new (Symbols.Star, Colors.White), new ("Ice Arm Swing", "Cast Ice Arm Swing (Spin)", "Cast Ice Arm Swing"), Sev3),
            ]),
            new PlayerDstHealthDamageHitMechanic(IceShatter, Mech_IceShatter, new (Symbols.TriangleUp, Colors.Pink), new ("Ice Orbs", "Hit by Rotating Ice Shatter (Orbs)", "Ice Shatter (Orbs)"), Sev2, 50),
            new PlayerDstHealthDamageHitMechanic(IceCrystal, Mech_IceCrystal, new (Symbols.Circle, Colors.LightOrange), new ("I.Crystal", "Hit by Ice Crystal (Chill AoE)", "Ice Crystal"), Sev1, 50),
            new PlayerDstHealthDamageHitMechanic(Frostbite, Mech_FrostBite, new (Symbols.Square, Colors.Blue), new ("Frostbite.H", "Hit by Frostbite", "Frostbite"), Sev1),
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic([IceFrail1, IceFrail2], Mech_IceFrail, new (Symbols.Square, Colors.Orange), new ("I.Flail", "Hit by Ice Flail (Arm Swipe)", "Ice Flail"), Sev1, 50)
                .WithStabilitySubMechanic(
                    new SubMechanic(Mech_IceFrailCC, new (Symbols.Square, Colors.Yellow), new ("IceFlail.CC", "Knocked by Ice Flail (Arm Swipe)", "Ice Flail"), Sev0, 50),
                    false
                )
            ]),
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(DeadlyIceShockWave, Mech_DeadlyIceShockwave, new (Symbols.CircleOpen, Colors.Red), new ("D.IceWave", "Hit by Deadly Ice Shock Wave", "Deadly Ice Shock Wave"), Sev1),
                new EnemyCastEndMechanic(DeadlyIceShockWave, Mech_DeadlyIceShockwaveCast, new (Symbols.CircleOpen, Colors.White), new ("Deadly Ice Shock Wave", "Cast Deadly Ice Shock Wave", "Cast Deadly Ice Shock Wave"), Sev3),
                new PlayerDstHealthDamageHitMechanic([IceShockWave1, IceShockWave2, IceShockWave3], Mech_IceShockwave, new (Symbols.CircleOpen, Colors.LightOrange), new ("ShockWave.H", "Hit by Ice Shock Wave", "Ice Shock Wave"), Sev1),
            ]),
            new PlayerDstHealthDamageHitMechanic([SpinningIce1, SpinningIce2, SpinningIce3, SpinningIce4], Mech_SpinningIce, new (Symbols.CircleOpenDot, Colors.White), new ("SpinIce.H", "Hit by Spinning Ice", "Spinning Ice"), Sev1),
        ])
        );
        Extension = "icebrood";
        Icon = EncounterIconIcebroodConstruct;
        LogCategoryInformation.InSubCategoryOrder = 0;
        LogID |= 0x000001;
    }

    internal override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log, CombatReplayDecorationContainer arenaDecorations, CombatReplayMap? parentMap = null)
    {
        var crMap = new CombatReplayMap(
                        (729, 581),
                        (-32118, -11470, -28924, -8924));
        AddArenaDecorationsPerEncounter(log, arenaDecorations, LogID, CombatReplayIcebroodConstruct, crMap, parentMap);
        return crMap;
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
        phases.AddRange(GetSubPhasesByInvul(log, Invulnerability757, mainTarget, false, true));
        for (int i = 1; i < phases.Count; i++)
        {
            PhaseData phase = phases[i];
            phase.AddParentPhase(phases[0]);
            phase.Name = "Phase " + i;
            phase.AddTarget(mainTarget, log);
        }
        return phases;
    }
    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputePlayerCombatReplayActors(p, log, replay);
        }
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeNPCCombatReplayActors(target, log, replay);
        }

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
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);
        }

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
