using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Mechanic.MechanicSeverity;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.MechanicIDs;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal class Boneskinner : Bjora
{
    public Boneskinner(int triggerID) : base(triggerID)
    {
        MechanicList.Add(new MechanicGroup([
            new PlayerDstHealthDamageHitMechanic(Grasp, Mech_Grasp, new (Symbols.Circle, Colors.Grey), new ("Grasp.H", "Grasp (Claw AoE)", "Grasp Hit"), Sev2),
            new PlayerDstHealthDamageHitMechanic(Cascade, Mech_Cascade, new (Symbols.TriangleDown, Colors.DarkRed), new ("Cascade.H", "Cascade (Rectangle AoEs from paws stomp)", "Cascade Hit"), Sev2),
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(BoneskinnerCharge, Mech_BoneskinnerCharge, new (Symbols.TriangleUp, Colors.Red), new ("H.Charge", "Hit by Charge", "Charge hit"), Sev1),
                new EnemyCastEndMechanic(BoneskinnerCharge, Mech_BoneskinnerChargeCastEnd, new (Symbols.Hexagram, Colors.LightRed), new ("D.Torch", "Charged a torch", "Charge"), Sev0)
                    .UsingChecker((ce, log) => !ce.IsInterrupted),
            ]),
            new PlayerDstHealthDamageHitMechanic(CrushingCruelty, Mech_CrushingCruelty, new (Symbols.Star, Colors.DarkGreen), new ("Crush.Cru.H", "Hit by Crushing Cruelty (Jump middle after Charge)", "Crushing Cruelty Hit"), Sev0),
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(DeathWind, Mech_DeathWind, new (Symbols.TriangleUp, Colors.Orange), new ("Launched", "Hit by Death Wind", "Death Wind Hit"), Sev0), // This attack removes stability
                new EnemyCastEndMechanic(DeathWind, Mech_DeathWindCastEnd, new (Symbols.TriangleUpOpen, Colors.LightOrange), new ("D.Wind", "Cast Death Wind (extinguished one torch)", "Death Wind"), Sev0)
                    .UsingChecker((ce, log) => !ce.IsInterrupted),
            ]),
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(DouseInDarkness, Mech_DouseInDarkness, new (Symbols.Cross, Colors.DarkTeal), new ("DouseDarkness.H", "Hit by Douse in Darkness", "Douse in Darkness Hit"), Sev1),
                new EnemyCastEndMechanic(DouseInDarkness, Mech_DouseInDarknessCastEnd, new (Symbols.Cross, Colors.Teal), new ("D.Darkness", "Cast Douse in Darkness (extinguished all torches)", "Douse in Darkness"), Sev0)
                    .UsingChecker((ce, log) => !ce.IsInterrupted),
            ]),
            new PlayerDstHealthDamageHitMechanic(BarrageWispBoneskinner, Mech_BoneskinnerBarrageWisp, new (Symbols.TriangleRight, Colors.Green), new ("Barrage.H", "Hit by Barrage (Wisp AoE)", "Barrage Hit"), Sev0),
            new EnemyCastStartMechanic(BoneskinnerBreakbar, Mech_BoneskinnerBreakbarStart, new (Symbols.Square, Colors.Purple), new ("Breakbar", "Casting a Breakbar", "Breakbar"), Sev3),
            new PlayerDstBuffApplyMechanic(UnrelentingPainBuff, Mech_UnrelentingPainApply, new(Symbols.DiamondOpen, Colors.Pink), new("UnrelPain.A", "Unreleting Pain Applied", "Unrelenting Pain Applied"), Sev0),
            new EnemyDstBuffApplyMechanic(Exposed31589, Mech_BoneskinnerExposed, new (Symbols.SquareOpen, Colors.Pink), new ("Exposed.E", "Gained Exposed (Breakbar broken)", "Exposed"), Sev2),
        ])
        );
        Extension = "boneskin";
        Icon = EncounterIconBoneskinner;
        LogCategoryInformation.InSubCategoryOrder = 2;
        LogID |= 0x000004;
    }

    internal override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log, CombatReplayDecorationContainer arenaDecorations, CombatReplayMap? parentMap = null)
    {
        var crMap = new CombatReplayMap(
                        (905, 789),
                        (-1013, -1550, 2221, 1266));
        AddArenaDecorationsPerEncounter(log, arenaDecorations, LogID, CombatReplayBoneskinner, crMap, parentMap);
        return crMap;
    }

    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        return
        [
            new DamageCastFinder(UnnaturalAura, UnnaturalAura),
        ];
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        return
        [
            TargetID.VigilTactician,
            TargetID.VigilRecruit,
            TargetID.PrioryExplorer,
            TargetID.PrioryScholar,
            TargetID.AberrantWisp,
            TargetID.Torch,
        ];
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        var torches = combatData.Where(x => MaxHealthUpdateEvent.GetMaxHealth(x) == 14940 && x.IsStateChange == StateChange.MaxHealthUpdate).Select(x => agentData.GetAgent(x.SrcAgent, x.Time)).Where(x => x.Type == AgentItem.AgentType.VolatileSpecies && x.HitboxWidth >= 250);
        var invulApplies = combatData.Where(x => x.IsBuffApplyEvent() && x.SkillID == Invulnerability757).Select(x => agentData.GetAgent(x.DstAgent, x.Time)).ToHashSet();
        foreach (AgentItem torch in torches)
        {
            if (!invulApplies.Contains(torch))
            {
                continue;
            }
            torch.OverrideID(TargetID.Torch, agentData);
            torch.OverrideAwareTimes(logData.EvtcLogStart, logData.EvtcLogEnd);
        }
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);
    }

    internal override void SetInstanceBuffs(ParsedEvtcLog log, List<InstanceBuff> instanceBuffs)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.SetInstanceBuffs(log, instanceBuffs);
        }

        if (log.CombatData.GetBuffData(AchievementEligibilityHoldOntoTheLight).Any())
        {
            var encounterPhases = log.LogData.GetEncounterPhases(log, LogID);
            foreach (var encounterPhase in encounterPhases)
            {
                if (encounterPhase.Success)
                {
                    instanceBuffs.MaybeAdd(GetOnPlayerCustomInstanceBuff(log, encounterPhase, AchievementEligibilityHoldOntoTheLight));
                }
            }
        }
    }

    internal override void ComputeAchievementEligibilityEvents(ParsedEvtcLog log, Player p, List<AchievementEligibilityEvent> achievementEligibilityEvents)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeAchievementEligibilityEvents(log, p, achievementEligibilityEvents);
        }
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
        switch (target.ID)
        {
            case (int)TargetID.Boneskinner:
                foreach (CastEvent cast in target.GetAnimatedCastEvents(log))
                {
                    switch (cast.SkillID)
                    {
                        // Death Wind
                        case DeathWind:
                            {
                                int castTime = 3330;
                                int hitTime = 1179;
                                uint radius = 1500;
                                long endHitTime = cast.Time + hitTime;
                                long endCastTime = cast.Time + castTime;

                                var lastDirection = replay.PolledRotations.LastOrNull((in ParametricPoint3D x) => x.Time > cast.Time + 100 && x.Time < cast.Time + 100 + castTime);
                                if (lastDirection != null)
                                {
                                    var connector = new AgentConnector(target);
                                    var rotationConnector = new AngleConnector(lastDirection.Value.XYZ);
                                    // Growing Decoration
                                    var pie = (PieDecoration)new PieDecoration(radius, 30, (cast.Time, endHitTime), Colors.Orange, 0.2, connector).UsingRotationConnector(rotationConnector);
                                    replay.Decorations.AddWithGrowing(pie, endHitTime);
                                    // Lingering AoE to match in game display
                                    replay.Decorations.Add(new PieDecoration(radius, 30, (endHitTime, endCastTime), Colors.Orange, 0.1, connector).UsingRotationConnector(rotationConnector));
                                }
                            }
                            break;
                        // Crushing Cruelty - Jump back to the center
                        case CrushingCruelty:
                            {
                                int hitTime = 2833;
                                long endTime = cast.Time + hitTime;

                                // Position of the jump back
                                var jumpPosition = new Vector3(613.054f, -85.3458f, -7075.265f);
                                var circle = new CircleDecoration(1500, (cast.Time, endTime), Colors.LightOrange, 0.1, new PositionConnector(jumpPosition));
                                replay.Decorations.AddWithGrowing(circle, endTime);
                            }
                            break;
                        // Douse in Darkness - Jump in air
                        case DouseInDarkness:
                            {
                                int jumpTime = 2500;
                                uint radius = 1500;
                                long endJump = cast.Time + jumpTime;
                                int timings = 300;

                                // Jump up
                                var jumpUpCircle = new CircleDecoration(radius, (cast.Time, endJump), Colors.LightOrange, 0.1, new AgentConnector(target));
                                replay.Decorations.AddWithGrowing(jumpUpCircle, endJump);
                                // Pull
                                for (int i = 0; i < 4; i++)
                                {
                                    long duration = cast.Time + jumpTime + timings * i;
                                    long end = cast.Time + jumpTime + timings * (i + 1);
                                    replay.Decorations.Add(new CircleDecoration(radius, (endJump, end), Colors.Red, 0.2, new AgentConnector(target)).UsingFilled(false).UsingGrowingEnd(duration, true));
                                }
                                // Landing
                                long pullTime = cast.Time + jumpTime + 1700;
                                long finalTime = pullTime + 1500;
                                var landingCircle = new CircleDecoration(radius, (pullTime, finalTime), Colors.LightOrange, 0.1, new AgentConnector(target));
                                replay.Decorations.AddWithGrowing(landingCircle, finalTime);
                            }
                            break;
                        default:
                            break;
                    }
                }

                // Cascade
                AddCascadeDecoration(log, target, replay, EffectGUIDs.CascadeAoEIndicator1, 200, 40);
                AddCascadeDecoration(log, target, replay, EffectGUIDs.CascadeAoEIndicator2, 400, 80);
                AddCascadeDecoration(log, target, replay, EffectGUIDs.CascadeAoEIndicator3, 600, 120);
                AddCascadeDecoration(log, target, replay, EffectGUIDs.CascadeAoEIndicator4, 800, 160);
                AddCascadeDecoration(log, target, replay, EffectGUIDs.CascadeAoEIndicator5, 1000, 200);
                break;
            case (int)TargetID.AberrantWisp:
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

        // Grasp AoE Orange Indicator
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.GraspAoeIndicator, out var grasps))
        {
            foreach (EffectEvent effect in grasps)
            {
                lifespan = effect.ComputeLifespan(log, 1800);
                var circle = new CircleDecoration(100, lifespan, Colors.Orange, 0.2, new PositionConnector(effect.Position));
                environmentDecorations.AddWithGrowing(circle, lifespan.end);
            }
        }
        // Grasp Claws Effect / Dark Red AoE
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.GraspClaws1, out var claws))
        {
            foreach (EffectEvent effect in claws)
            {
                lifespan = effect.ComputeLifespan(log, 30000);
                var circle = new CircleDecoration(100, lifespan, Colors.RedBrownish, 0.2, new PositionConnector(effect.Position));
                environmentDecorations.Add(circle);
                environmentDecorations.Add(circle.GetBorderDecoration(Colors.Red, 0.2));
            }
        }
    }

    private static void AddCascadeDecoration(ParsedEvtcLog log, SingleActor actor, CombatReplay replay, Guid guid, uint width, uint height)
    {
        if (log.CombatData.TryGetEffectEventsByGUID(guid, out var rectangularIndicators))
        {
            foreach (EffectEvent indicator in rectangularIndicators)
            {
                long duration = 300;
                (long start, long end) lifespan = indicator.ComputeLifespan(log, duration);

                if (actor.TryGetCurrentFacingDirection(log, lifespan.start, out var rotation, duration))
                {
                    var rectangle = (RectangleDecoration)new RectangleDecoration(width, height, lifespan, Colors.Orange, 0.2, new PositionConnector(indicator.Position)).UsingRotationConnector(new AngleConnector(rotation.Value));
                    replay.Decorations.AddWithBorder(rectangle, Colors.Red, 0.2);
                }
            }
        }
    }
}
