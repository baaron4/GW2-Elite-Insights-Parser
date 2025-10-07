using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal class Boneskinner : Bjora
{
    public Boneskinner(int triggerID) : base(triggerID)
    {
        MechanicList.Add(new MechanicGroup([
            new PlayerDstHealthDamageHitMechanic(Grasp, new MechanicPlotlySetting(Symbols.Circle, Colors.Grey), "Grasp.H", "Grasp (Claw AoE)", "Grasp Hit", 0),
            new PlayerDstHealthDamageHitMechanic(Cascade, new MechanicPlotlySetting(Symbols.TriangleDown, Colors.DarkRed), "Cascade.H", "Cascade (Rectangle AoEs from paws stomp)", "Cascade Hit", 0),
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(BoneskinnerCharge, new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Red), "H.Charge", "Hit by Charge", "Charge hit", 0),
                new EnemyCastEndMechanic(BoneskinnerCharge, new MechanicPlotlySetting(Symbols.Hexagram, Colors.LightRed), "D.Torch", "Charged a torch", "Charge", 0)
                    .UsingChecker((ce, log) => !ce.IsInterrupted),
            ]),
            new PlayerDstHealthDamageHitMechanic(CrushingCruelty, new MechanicPlotlySetting(Symbols.Star, Colors.DarkGreen), "Crush.Cru.H", "Hit by Crushing Cruelty (Jump middle after Charge)", "Crushing Cruelty Hit", 0),
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(DeathWind, new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Orange), "Launched", "Hit by Death Wind", "Death Wind Hit", 0), // This attack removes stability
                new EnemyCastEndMechanic(DeathWind, new MechanicPlotlySetting(Symbols.TriangleUpOpen, Colors.LightOrange), "D.Wind", "Cast Death Wind (extinguished one torch)", "Death Wind", 0)
                    .UsingChecker((ce, log) => !ce.IsInterrupted),
            ]),
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(DouseInDarkness, new MechanicPlotlySetting(Symbols.Cross, Colors.DarkTeal), "DouseDarkness.H", "Hit by Douse in Darkness", "Douse in Darkness Hit", 0),
                new EnemyCastEndMechanic(DouseInDarkness, new MechanicPlotlySetting(Symbols.Cross, Colors.Teal), "D.Darkness", "Cast Douse in Darkness (extinguished all torches)", "Douse in Darkness", 0)
                    .UsingChecker((ce, log) => !ce.IsInterrupted),
            ]),
            new PlayerDstHealthDamageHitMechanic(BarrageWispBoneskinner, new MechanicPlotlySetting(Symbols.TriangleRight, Colors.Green), "Barrage.H", "Hit by Barrage (Wisp AoE)", "Barrage Hit", 0),
            new PlayerDstBuffApplyMechanic(UnrelentingPainBuff, new MechanicPlotlySetting(Symbols.DiamondOpen, Colors.Pink), "UnrelPain.A", "Unreleting Pain Applied", "Unrelenting Pain Applied", 0),
            new EnemyCastStartMechanic(BoneskinnerBreakbar, new MechanicPlotlySetting(Symbols.Square, Colors.Purple), "Breakbar", "Casting a Breakbar", "Breakbar", 0),
            new EnemyDstBuffApplyMechanic(Exposed31589, new MechanicPlotlySetting(Symbols.SquareOpen, Colors.Pink), "Exposed", "Gained Exposed (Breakbar broken)", "Exposed", 0),
        ])
        );
        Extension = "boneskin";
        Icon = EncounterIconBoneskinner;
        LogCategoryInformation.InSubCategoryOrder = 2;
        LogID |= 0x000004;
    }

    internal override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log, CombatReplayDecorationContainer arenaDecorations)
    {
        var crMap = new CombatReplayMap(
                        (905, 789),
                        (-1013, -1600, 2221, 1416));
        AddArenaDecorationsPerEncounter(log, arenaDecorations, LogID, CombatReplayBoneskinner, crMap);
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
        var torches = combatData.Where(x => MaxHealthUpdateEvent.GetMaxHealth(x) == 14940 && x.IsStateChange == StateChange.MaxHealthUpdate).Select(x => agentData.GetAgent(x.SrcAgent, x.Time)).Where(x => x.Type == AgentItem.AgentType.Gadget && x.HitboxHeight == 500 && x.HitboxWidth >= 250);
        foreach (AgentItem torch in torches)
        {
            torch.OverrideType(AgentItem.AgentType.NPC, agentData);
            torch.OverrideID(TargetID.Torch, agentData);
            torch.OverrideAwareTimes(logData.EvtcLogStart, logData.EvtcLogEnd);
        }
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);
    }

    protected override void SetInstanceBuffs(ParsedEvtcLog log, List<InstanceBuff> instanceBuffs)
    {
        base.SetInstanceBuffs(log, instanceBuffs);

        if (log.CombatData.GetBuffData(AchievementEligibilityHoldOntoTheLight).Any())
        {
            var encounterPhases = log.LogData.GetPhases(log).OfType<EncounterPhaseData>().Where(x => x.LogID == LogID);
            foreach (var encounterPhase in encounterPhases)
            {
                if (encounterPhase.Success)
                {
                    instanceBuffs.MaybeAdd(GetOnPlayerCustomInstanceBuff(log, encounterPhase, AchievementEligibilityHoldOntoTheLight));
                }
            }
        }
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
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
        base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);

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

    private static void AddCascadeDecoration(ParsedEvtcLog log, SingleActor actor, CombatReplay replay, GUID guid, uint width, uint height)
    {
        if (log.CombatData.TryGetEffectEventsByGUID(guid, out var rectangularIndicators))
        {
            foreach (EffectEvent indicator in rectangularIndicators)
            {
                long duration = 300;
                (long start, long end) lifespan = indicator.ComputeLifespan(log, duration);

                if (actor.TryGetCurrentFacingDirection(log, lifespan.start, out var rotation, duration))
                {
                    var rectangle = (RectangleDecoration)new RectangleDecoration(width, height, lifespan, Colors.Orange, 0.2, new PositionConnector(indicator.Position)).UsingRotationConnector(new AngleConnector(rotation));
                    replay.Decorations.AddWithBorder(rectangle, Colors.Red, 0.2);
                }
            }
        }
    }
}
