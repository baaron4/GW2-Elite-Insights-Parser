using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class Boneskinner : Bjora
{
    public Boneskinner(int triggerID) : base(triggerID)
    {
        MechanicList.Add(new MechanicGroup([
            new PlayerDstHitMechanic(Grasp, "Grasp", new MechanicPlotlySetting(Symbols.Circle, Colors.Grey), "Grasp.H", "Grasp (Claw AoE)", "Grasp Hit", 0),
            new PlayerDstHitMechanic(Cascade, "Cascade", new MechanicPlotlySetting(Symbols.TriangleDown, Colors.DarkRed), "Cascade.H", "Cascade (Rectangle AoEs from paws stomp)", "Cascade Hit", 0),
            new MechanicGroup([
                new PlayerDstHitMechanic(BoneskinnerCharge, "Charge", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Red), "H.Charge", "Hit by Charge", "Charge hit", 0),
                new EnemyCastEndMechanic(BoneskinnerCharge, "Charge", new MechanicPlotlySetting(Symbols.Hexagram, Colors.LightRed), "D.Torch", "Charged a torch", "Charge", 0)
                    .UsingChecker((ce, log) => !ce.IsInterrupted),
            ]),
            new PlayerDstHitMechanic(CrushingCruelty, "Crushing Cruelty", new MechanicPlotlySetting(Symbols.Star, Colors.DarkGreen), "Crush.Cru.H", "Hit by Crushing Cruelty (Jump middle after Charge)", "Crushing Cruelty Hit", 0),
            new MechanicGroup([
                new PlayerDstHitMechanic(DeathWind, "Death Wind", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Orange), "Launched", "Hit by Death Wind", "Death Wind Hit", 0), // This attack removes stability
                new EnemyCastEndMechanic(DeathWind, "Death Wind", new MechanicPlotlySetting(Symbols.TriangleUpOpen, Colors.LightOrange), "D.Wind", "Cast Death Wind (extinguished one torch)", "Death Wind", 0)
                    .UsingChecker((ce, log) => !ce.IsInterrupted),
            ]),
            new MechanicGroup([
                new PlayerDstHitMechanic(DouseInDarkness, "Douse in Darkness", new MechanicPlotlySetting(Symbols.Cross, Colors.DarkTeal), "DouseDarkness.H", "Hit by Douse in Darkness", "Douse in Darkness Hit", 0),
                new EnemyCastEndMechanic(DouseInDarkness, "Douse in Darkness", new MechanicPlotlySetting(Symbols.Cross, Colors.Teal), "D.Darkness", "Cast Douse in Darkness (extinguished all torches)", "Douse in Darkness", 0)
                    .UsingChecker((ce, log) => !ce.IsInterrupted),
            ]),
            new PlayerDstHitMechanic(BarrageWispBoneskinner, "Barrage", new MechanicPlotlySetting(Symbols.TriangleRight, Colors.Green), "Barrage.H", "Hit by Barrage (Wisp AoE)", "Barrage Hit", 0),
            new PlayerDstBuffApplyMechanic(UnrelentingPainBuff, "Unrelenting Pain", new MechanicPlotlySetting(Symbols.DiamondOpen, Colors.Pink), "UnrelPain.A", "Unreleting Pain Applied", "Unrelenting Pain Applied", 0),
            new EnemyCastStartMechanic(BoneskinnerBreakbar, "Breakbar", new MechanicPlotlySetting(Symbols.Square, Colors.Purple), "Breakbar", "Casting a Breakbar", "Breakbar", 0),
            new EnemyDstBuffApplyMechanic(Exposed31589, "Exposed" , new MechanicPlotlySetting(Symbols.SquareOpen, Colors.Pink), "Exposed", "Gained Exposed (Breakbar broken)", "Exposed", 0),
        ])
        );
        Extension = "boneskin";
        Icon = EncounterIconBoneskinner;
        EncounterCategoryInformation.InSubCategoryOrder = 2;
        EncounterID |= 0x000004;
    }

    protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
    {
        return new CombatReplayMap(CombatReplayBoneskinner,
                        (905, 789),
                        (-1013, -1600, 2221, 1416)/*,
                        (-0, -0, 0, 0),
                        (0, 0, 0, 0)*/);
    }

    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        return
        [
            new DamageCastFinder(UnnaturalAura, UnnaturalAura),
        ];
    }

    protected override List<TrashID> GetTrashMobsIDs()
    {
        return
        [
            TrashID.VigilTactician,
            TrashID.VigilRecruit,
            TrashID.PrioryExplorer,
            TrashID.PrioryScholar,
            TrashID.AberrantWisp,
            TrashID.Torch,
        ];
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        var torches = combatData.Where(x => MaxHealthUpdateEvent.GetMaxHealth(x) == 14940 && x.IsStateChange == StateChange.MaxHealthUpdate).Select(x => agentData.GetAgent(x.SrcAgent, x.Time)).Where(x => x.Type == AgentItem.AgentType.Gadget && x.HitboxHeight == 500 && x.HitboxWidth >= 250);
        foreach (AgentItem torch in torches)
        {
            torch.OverrideType(AgentItem.AgentType.NPC, agentData);
            torch.OverrideID(TrashID.Torch, agentData);
            torch.OverrideAwareTimes(fightData.LogStart, fightData.LogEnd);
        }
        base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
    }

    protected override void SetInstanceBuffs(ParsedEvtcLog log)
    {
        base.SetInstanceBuffs(log);

        if (log.FightData.Success && log.CombatData.GetBuffData(AchievementEligibilityHoldOntoTheLight).Any())
        {
            InstanceBuffs.MaybeAdd(GetOnPlayerCustomInstanceBuff(log, AchievementEligibilityHoldOntoTheLight));
        }
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {

        switch (target.ID)
        {
            case (int)TargetID.Boneskinner:
                var casts = target.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd);
                // Death Wind
                var deathWind = casts.Where(x => x.SkillId == DeathWind);
                foreach (CastEvent c in deathWind)
                {
                    int castTime = 3330;
                    int hitTime = 1179;
                    uint radius = 1500;
                    int endHitTime = (int)c.Time + hitTime;
                    int endCastTime = (int)c.Time + castTime;

                    var lastDirection = replay.PolledRotations.LastOrNull((in ParametricPoint3D x) => x.Time > c.Time + 100 && x.Time < c.Time + 100 + castTime);
                    if (lastDirection != null)
                    {
                        var connector = new AgentConnector(target);
                        var rotationConnector = new AngleConnector(lastDirection.Value.XYZ);
                        // Growing Decoration
                        var pie = (PieDecoration)new PieDecoration(radius, 30, (c.Time, endHitTime), Colors.Orange, 0.2, connector).UsingRotationConnector(rotationConnector);
                        replay.Decorations.AddWithGrowing(pie, endHitTime);
                        // Lingering AoE to match in game display
                        replay.Decorations.Add(new PieDecoration(radius, 30, (endHitTime, endCastTime), Colors.Orange, 0.1, connector).UsingRotationConnector(rotationConnector));
                    }
                }

                // Crushing Cruelty
                var crushingCruelty = casts.Where(x => x.SkillId == CrushingCruelty);
                foreach (CastEvent c in crushingCruelty)
                {
                    int hitTime = 2833;
                    uint radius = 1500;
                    long endTime = c.Time + hitTime;

                    // Position of the jump back
                    var jumpPosition = new Vector3(613.054f, -85.3458f, -7075.265f);
                    var circle = new CircleDecoration(radius, (c.Time, endTime), Colors.LightOrange, 0.1, new PositionConnector(jumpPosition));
                    replay.Decorations.AddWithGrowing(circle, endTime);
                }

                // Douse in Darkness
                var douseInDarkness = casts.Where(x => x.SkillId == DouseInDarkness);
                foreach (CastEvent c in douseInDarkness)
                {
                    int jumpTime = 2500;
                    uint radius = 1500;
                    long endJump = c.Time + jumpTime;
                    int timings = 300;

                    // Jump up
                    var jumpUpCircle = new CircleDecoration(radius, (c.Time, endJump), Colors.LightOrange, 0.1, new AgentConnector(target));
                    replay.Decorations.AddWithGrowing(jumpUpCircle, endJump);
                    // Pull
                    for (int i = 0; i < 4; i++)
                    {
                        long duration = c.Time + jumpTime + timings * i;
                        long end = c.Time + jumpTime + timings * (i + 1);
                        replay.Decorations.Add(new CircleDecoration(radius, (endJump, end), Colors.Red, 0.2, new AgentConnector(target)).UsingFilled(false).UsingGrowingEnd(duration, true));
                    }
                    // Landing
                    long pullTime = c.Time + jumpTime + 1700;
                    long finalTime = pullTime + 1500;
                    var landingCircle = new CircleDecoration(radius, (pullTime, finalTime), Colors.LightOrange, 0.1, new AgentConnector(target));
                    replay.Decorations.AddWithGrowing(landingCircle, finalTime);
                }
                // Cascade
                AddCascadeDecoration(log, target, replay, EffectGUIDs.CascadeAoEIndicator1, 200, 40);
                AddCascadeDecoration(log, target, replay, EffectGUIDs.CascadeAoEIndicator2, 400, 80);
                AddCascadeDecoration(log, target, replay, EffectGUIDs.CascadeAoEIndicator3, 600, 120);
                AddCascadeDecoration(log, target, replay, EffectGUIDs.CascadeAoEIndicator4, 800, 160);
                AddCascadeDecoration(log, target, replay, EffectGUIDs.CascadeAoEIndicator5, 1000, 200);
                break;
            case (int)TrashID.AberrantWisp:
                break;
            default:
                break;
        }
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log)
    {
        base.ComputeEnvironmentCombatReplayDecorations(log);

        // Grasp AoE Orange Indicator
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.GraspAoeIndicator, out var indicators))
        {
            foreach (EffectEvent indicator in indicators)
            {
                int duration = 1800;
                int start = (int)indicator.Time;
                int end = (int)indicator.Time + duration;
                var circle = new CircleDecoration(100, (start, end), Colors.Orange, 0.2, new PositionConnector(indicator.Position));
                EnvironmentDecorations.Add(circle.Copy().UsingGrowingEnd(end));
                EnvironmentDecorations.Add(circle);
            }
        }
        // Grasp Claws Effect / Dark Red AoE
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.GraspClaws1, out var claws))
        {
            foreach (EffectEvent claw in claws)
            {
                int duration = 30000;
                int start = (int)claw.Time;
                int end = (int)claw.Time + duration;
                var circle = new CircleDecoration(100, (start, end), Colors.RedBrownish, 0.2, new PositionConnector(claw.Position));
                EnvironmentDecorations.Add(circle);
                EnvironmentDecorations.Add(circle.GetBorderDecoration(Colors.Red, 0.2));
            }
        }
    }

    private static void AddCascadeDecoration(ParsedEvtcLog log, SingleActor actor, CombatReplay replay, GUID guid, uint width, uint height)
    {
        if (log.CombatData.TryGetEffectEventsByGUID(guid, out var rectangularIndicators))
        {
            foreach (EffectEvent indicator in rectangularIndicators)
            {
                int duration = 360;
                int start = (int)indicator.Time;
                int end = (int)indicator.Time + duration;

                if (actor.TryGetCurrentFacingDirection(log, start, out var rotation, duration))
                {
                    var connector = new PositionConnector(indicator.Position);
                    var rotationConnector = new AngleConnector(rotation);
                    replay.Decorations.AddWithBorder((RectangleDecoration)new RectangleDecoration(width, height, (start, end), Colors.Orange, 0.2, connector).UsingRotationConnector(rotationConnector), Colors.Red, 0.2);
                }
            }
        }
    }
}
