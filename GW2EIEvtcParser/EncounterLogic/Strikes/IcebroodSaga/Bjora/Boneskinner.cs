using System;
using System.Linq;
using System.Collections.Generic;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class Boneskinner : Bjora
    {
        public Boneskinner(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
                new PlayerDstHitMechanic(Grasp, "Grasp", new MechanicPlotlySetting(Symbols.Circle, Colors.Grey), "Grasp.H", "Grasp (Claw AoE)", "Grasp Hit", 0),
                new PlayerDstHitMechanic(Cascade, "Cascade", new MechanicPlotlySetting(Symbols.TriangleDown, Colors.DarkRed), "Cascade.H", "Cascade (Rectangle AoEs from paws stomp)", "Cascade Hit", 0),
                new PlayerDstHitMechanic(BoneskinnerCharge, "Charge", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Red), "H.Charge", "Hit by Charge", "Charge hit", 0),
                new PlayerDstHitMechanic(CrushingCruelty, "Crushing Cruelty", new MechanicPlotlySetting(Symbols.Star, Colors.DarkGreen), "Crush.Cru.H", "Hit by Crushing Cruelty (Jump middle after Charge)", "Crushing Cruelty Hit", 0),
                new PlayerDstHitMechanic(DeathWind, "Death Wind", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Orange), "Launched", "Hit by Death Wind", "Death Wind Hit", 0), // This attack removes stability
                new PlayerDstHitMechanic(DouseInDarkness, "Douse in Darkness", new MechanicPlotlySetting(Symbols.Cross, Colors.DarkTeal), "DouseDarkness.H", "Hit by Douse in Darkness", "Douse in Darkness Hit", 0),
                new PlayerDstHitMechanic(BarrageWispBoneskinner, "Barrage", new MechanicPlotlySetting(Symbols.TriangleRight, Colors.Green), "Barrage.H", "Hit by Barrage (Wisp AoE)", "Barrage Hit", 0),
                new PlayerDstBuffApplyMechanic(UnrelentingPainBuff, "Unrelenting Pain", new MechanicPlotlySetting(Symbols.DiamondOpen, Colors.Pink), "UnrelPain.A", "Unreleting Pain Applied", "Unrelenting Pain Applied", 0),
                new EnemyCastEndMechanic(BoneskinnerCharge, "Charge", new MechanicPlotlySetting(Symbols.Hexagram, Colors.LightRed), "D.Torch", "Charged a torch", "Charge", 0).UsingChecker((ce, log) => ce.Status != AbstractCastEvent.AnimationStatus.Interrupted),
                new EnemyCastEndMechanic(DeathWind, "Death Wind", new MechanicPlotlySetting(Symbols.TriangleUpOpen, Colors.LightOrange), "D.Wind", "Cast Death Wind (extinguished one torch)", "Death Wind", 0).UsingChecker((ce, log) => ce.Status != AbstractCastEvent.AnimationStatus.Interrupted),
                new EnemyCastEndMechanic(DouseInDarkness, "Douse in Darkness", new MechanicPlotlySetting(Symbols.Cross, Colors.Teal), "D.Darkness", "Cast Douse in Darkness (extinguished all torches)", "Douse in Darkness", 0).UsingChecker((ce, log) => ce.Status != AbstractCastEvent.AnimationStatus.Interrupted),
                new EnemyCastStartMechanic(BoneskinnerBreakbar, "Breakbar", new MechanicPlotlySetting(Symbols.Square, Colors.Purple), "Breakbar", "Casting a Breakbar", "Breakbar", 0),
                new EnemyDstBuffApplyMechanic(Exposed31589, "Exposed" , new MechanicPlotlySetting(Symbols.SquareOpen, Colors.Pink), "Exposed", "Gained Exposed (Breakbar broken)", "Exposed", 0),
            }
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
            return new List<InstantCastFinder>()
            {
                new DamageCastFinder(UnnaturalAura, UnnaturalAura),
            };
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.VigilTactician,
                ArcDPSEnums.TrashID.VigilRecruit,
                ArcDPSEnums.TrashID.PrioryExplorer,
                ArcDPSEnums.TrashID.PrioryScholar,
                ArcDPSEnums.TrashID.AberrantWisp,
                ArcDPSEnums.TrashID.Torch,
            };
        }

        internal override void EIEvtcParse(ulong gw2Build, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            var torches = combatData.Where(x => x.DstAgent == 14940 && x.IsStateChange == ArcDPSEnums.StateChange.MaxHealthUpdate).Select(x => agentData.GetAgent(x.SrcAgent, x.Time)).Where(x => x.Type == AgentItem.AgentType.Gadget && x.HitboxHeight == 500 && x.HitboxWidth >= 250).ToList();
            foreach (AgentItem torch in torches)
            {
                torch.OverrideType(AgentItem.AgentType.NPC);
                torch.OverrideID(ArcDPSEnums.TrashID.Torch);
                torch.OverrideAwareTimes(fightData.LogStart, fightData.LogEnd);
            }
            agentData.Refresh();
            ComputeFightTargets(agentData, combatData, extensions);
        }

        protected override void SetInstanceBuffs(ParsedEvtcLog log)
        {
            base.SetInstanceBuffs(log);
            IReadOnlyList<AbstractBuffEvent> holdOntoTheLight = log.CombatData.GetBuffData(AchievementEligibilityHoldOntoTheLight);

            if (holdOntoTheLight.Any() && log.FightData.Success)
            {
                foreach (Player p in log.PlayerList)
                {
                    if (p.HasBuff(log, AchievementEligibilityHoldOntoTheLight, log.FightData.FightEnd - ServerDelayConstant))
                    {
                        InstanceBuffs.Add((log.Buffs.BuffsByIds[AchievementEligibilityHoldOntoTheLight], 1));
                        break;
                    }
                }
            }
        }

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            IReadOnlyList<AbstractCastEvent> casts = target.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd);

            switch (target.ID)
            {
                case (int)ArcDPSEnums.TargetID.Boneskinner:
                    // Death Wind
                    var deathWind = casts.Where(x => x.SkillId == DeathWind).ToList();
                    foreach (AbstractCastEvent c in deathWind)
                    {
                        int castTime = 3330;
                        int hitTime = 1179;
                        int radius = 1500;
                        int endHitTime = (int)c.Time + hitTime;
                        int endCastTime = (int)c.Time + castTime;

                        ParametricPoint3D lastDirection = replay.PolledRotations.LastOrDefault(x => x.Time > c.Time + 100 && x.Time < c.Time + 100 + castTime);
                        if (lastDirection != null)
                        {
                            var connector = new AgentConnector(target);
                            var rotationConnector = new AngleConnector(lastDirection);
                            // Growing Decoration
                            var pie = (PieDecoration)new PieDecoration(radius, 30, (c.Time, endHitTime), "rgba(250, 120, 0, 0.2)", connector).UsingRotationConnector(rotationConnector);
                            replay.AddDecorationWithGrowing(pie, endHitTime);
                            // Lingering AoE to match in game display
                            replay.Decorations.Add(new PieDecoration(radius, 30, (endHitTime, endCastTime), "rgba(250, 60, 0, 0.1)", connector).UsingRotationConnector(rotationConnector));
                        }
                    }
                    // Crushing Cruelty
                    var crushingCruelty = casts.Where(x => x.SkillId == CrushingCruelty).ToList();
                    foreach (AbstractCastEvent c in crushingCruelty)
                    {
                        int hitTime = 2833;
                        int radius = 1500;
                        long endTime = c.Time + hitTime;

                        // Position of the jump back
                        var jumpPosition = new Point3D((float)613.054, (float)-85.3458, (float)-7075.265);
                        var circle = new CircleDecoration(radius, (c.Time, endTime), "rgba(250, 120, 0, 0.1)", new PositionConnector(jumpPosition));
                        replay.AddDecorationWithGrowing(circle, endTime);
                    }
                    // Douse in Darkness
                    var douseInDarkness = casts.Where(x => x.SkillId == DouseInDarkness).ToList();
                    foreach (AbstractCastEvent c in douseInDarkness)
                    {
                        int jumpTime = 2500;
                        int radius = 1500;
                        long endJump = c.Time + jumpTime;
                        int timings = 300;

                        // Jump up
                        var jumpUpCircle = new CircleDecoration(radius, (c.Time, endJump), "rgba(250, 120, 0, 0.1)", new AgentConnector(target));
                        replay.AddDecorationWithGrowing(jumpUpCircle, endJump);
                        // Pull
                        for (int i = 0; i < 4; i++)
                        {
                            long duration = c.Time + jumpTime + timings * i;
                            long end = c.Time + jumpTime + timings * (i + 1);
                            replay.Decorations.Add(new CircleDecoration(radius, (endJump, end), "rgba(255, 0, 0, 0.2)", new AgentConnector(target)).UsingFilled(false).UsingGrowingEnd(duration, true));
                        }
                        // Landing
                        long pullTime = c.Time + jumpTime + 1700;
                        long finalTime = pullTime + 1500;
                        var landingCircle = new CircleDecoration(radius, (pullTime, finalTime), "rgba(250, 120, 0, 0.1)", new AgentConnector(target));
                        replay.AddDecorationWithGrowing(landingCircle, finalTime);
                    }
                    // Cascade
                    AddCascadeDecoration(log, target,replay, EffectGUIDs.CascadeAoEIndicator1, 200, 40);
                    AddCascadeDecoration(log, target, replay, EffectGUIDs.CascadeAoEIndicator2, 400, 80);
                    AddCascadeDecoration(log, target, replay, EffectGUIDs.CascadeAoEIndicator3, 600, 120);
                    AddCascadeDecoration(log, target, replay, EffectGUIDs.CascadeAoEIndicator4, 800, 160);
                    AddCascadeDecoration(log, target, replay, EffectGUIDs.CascadeAoEIndicator5, 1000, 200);
                    break;
                case (int)ArcDPSEnums.TrashID.AberrantWisp:
                    break;
                default:
                    break;
            }
        }

        internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log)
        {
            // Grasp AoE Orange Indicator
            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.GraspAoeIndicator, out IReadOnlyList<EffectEvent> indicators))
            {
                foreach (EffectEvent indicator in indicators)
                {
                    int duration = 1800;
                    int start = (int)indicator.Time;
                    int end = (int)indicator.Time + duration;
                    var circle = new CircleDecoration(100, (start, end), "rgba(250, 120, 0, 0.2)", new PositionConnector(indicator.Position));
                    EnvironmentDecorations.Add(circle.Copy().UsingGrowingEnd(end));
                    EnvironmentDecorations.Add(circle);
                }
            }
            // Grasp Claws Effect / Dark Red AoE
            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.GraspClaws1, out IReadOnlyList<EffectEvent> claws))
            {
                foreach (EffectEvent claw in claws)
                {
                    int duration = 30000;
                    int start = (int)claw.Time;
                    int end = (int)claw.Time + duration;
                    var circle = new CircleDecoration(100, (start, end), "rgba(71, 35, 32, 0.2)", new PositionConnector(claw.Position));
                    EnvironmentDecorations.Add(circle);
                    EnvironmentDecorations.Add(circle.GetBorderDecoration("rgba(255, 0, 0, 0.2)"));
                }
            }
        }

        private static void AddCascadeDecoration(ParsedEvtcLog log, AbstractSingleActor actor, CombatReplay replay, string guid, int width, int height)
        {
            if (log.CombatData.TryGetEffectEventsByGUID(guid, out IReadOnlyList<EffectEvent> rectangularIndicators))
            {
                foreach (EffectEvent indicator in rectangularIndicators)
                {
                    int duration = 360;
                    int start = (int)indicator.Time;
                    int end = (int)indicator.Time + duration;

                    Point3D rotation = actor.GetCurrentRotation(log, start, duration);
                    if (rotation != null)
                    {
                        var connector = new PositionConnector(indicator.Position);
                        var rotationConnector = new AngleConnector(rotation);
                        replay.AddDecorationWithBorder((RectangleDecoration)new RectangleDecoration(width, height, (start, end), "rgba(250, 120, 0, 0.2)", connector).UsingRotationConnector(rotationConnector), "rgba(255, 0, 0, 0.2)");
                    }
                }
            }
        }
    }
}
