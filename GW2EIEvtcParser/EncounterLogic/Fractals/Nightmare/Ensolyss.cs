using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class Ensolyss : Nightmare
    {
        public Ensolyss(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new PlayerDstHitMechanic(new long[] { LungeEnsolyss, LungeNightmareHallucination }, "Lunge", new MechanicPlotlySetting(Symbols.TriangleRightOpen,Colors.LightOrange), "Charge","Lunge (KB charge over arena)", "Charge",150),
            new PlayerDstHitMechanic(UpswingEnsolyss, "Upswing", new MechanicPlotlySetting(Symbols.Circle,Colors.Orange), "Smash 1","High damage Jump hit", "First Smash",0),
            new PlayerDstHitMechanic(UpswingHallucination, "Upswing", new MechanicPlotlySetting(Symbols.Circle, Colors.LightOrange), "Hall.AoE", "Hit by Hallucination Explosion", "Hallu Explosion", 0),
            new PlayerDstHitMechanic(new long[] { NigthmareMiasmaEnsolyss1, NigthmareMiasmaEnsolyss2, NigthmareMiasmaEnsolyss3 }, "Nightmare Miasma", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Magenta), "Goo","Nightmare Miasma (Goo)", "Miasma",0),
            new EnemyCastStartMechanic(CausticExplosionEnsolyss, "Caustic Explosion", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkTeal), "CC","After Phase CC", "Breakbar", 0),
            new EnemyCastEndMechanic(CausticExplosionEnsolyss, "Caustic Explosion", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Red), "CC Fail","After Phase CC Failed", "CC Fail", 0).UsingChecker( (ce,log) => ce.ActualDuration >= 15260),
            new EnemyCastEndMechanic(CausticExplosionEnsolyss, "Caustic Explosion", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkGreen), "CCed","After Phase CC Success", "CCed", 0).UsingChecker( (ce, log) => ce.ActualDuration < 15260),
            new PlayerDstHitMechanic(CausticExplosionEnsolyss, "Caustic Explosion", new MechanicPlotlySetting(Symbols.Bowtie,Colors.Yellow), "CC KB","Knockback hourglass during CC", "CC KB", 0),
            new EnemyCastStartMechanic(new long[] { NightmareDevastation1, NightmareDevastation2 }, "Nightmare Devastation", new MechanicPlotlySetting(Symbols.SquareOpen,Colors.Blue), "Bubble","Nightmare Devastation (bubble attack)", "Bubble",0),
            new PlayerDstHitMechanic(TailLashEnsolyss, "Tail Lash", new MechanicPlotlySetting(Symbols.TriangleLeft,Colors.Yellow), "Tail","Tail Lash (half circle Knockback)", "Tail Lash",0),
            new PlayerDstHitMechanic(RampageEnsolyss, "Rampage", new MechanicPlotlySetting(Symbols.BowtieOpen,Colors.Red), "Rampage","Rampage (asterisk shaped Arrow attack)", "Rampage",150),
            new PlayerDstHitMechanic(CausticGrasp, "Caustic Grasp", new MechanicPlotlySetting(Symbols.StarDiamond,Colors.LightOrange), "Pull","Caustic Grasp (Arena Wide Pull)", "Pull",0),
            new PlayerDstHitMechanic(TormentingBlast, "Tormenting Blast", new MechanicPlotlySetting(Symbols.Diamond,Colors.Yellow), "Quarter","Tormenting Blast (Two Quarter Circle attacks)", "Quarter circle",0),
            });
            Extension = "ensol";
            Icon = EncounterIconEnsolyss;
            EncounterCategoryInformation.InSubCategoryOrder = 2;
            EncounterID |= 0x000003;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap(CombatReplayEnsolyss,
                            (366, 366),
                            (252, 1, 2892, 2881)/*,
                            (-6144, -6144, 9216, 9216),
                            (11804, 4414, 12444, 5054)*/);
        }

        internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
        {
            return FightData.EncounterMode.CMNoName;
        }

        protected override List<int> GetTargetsIDs()
        {
            return new List<int>
            {
                (int)TargetID.Ensolyss,
                //(int)TrashID.NightmareAltar,
            };
        }

        protected override List<TrashID> GetTrashMobsIDs()
        {
            var trashIDs = new List<TrashID>
            {
                TrashID.NightmareHallucination1,
                TrashID.NightmareHallucination2,
                //TrashID.NightmareAltar,
            };
            trashIDs.AddRange(base.GetTrashMobsIDs());
            return trashIDs;
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor enso = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Ensolyss)) ?? throw new MissingKeyActorsException("Ensolyss not found");
            phases[0].AddTarget(enso);
            if (!requirePhases)
            {
                return phases;
            }
            phases.AddRange(GetPhasesByInvul(log, Determined762, enso, true, true));
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                if (i % 2 == 0)
                {
                    phase.Name = "Nightmare Altars";
                    phase.AddTarget(enso);
                }
                else
                {
                    phase.Name = "Phase " + (i + 1) / 2;
                    phase.AddTarget(enso);
                }
            }
            return phases;
        }

        private static void AddTormentingBlastDecoration(CombatReplay replay, AbstractSingleActor target, (long start, long end) lifespan, Point3D point, int quarterAoE, int quarterHit)
        {
            long startQuarter = lifespan.start + quarterAoE;
            long endQuarter = lifespan.start + quarterHit;
            long growingQuarter = lifespan.start + quarterHit;
            if (lifespan.end >= endQuarter) // If the attack started
            {
                var connector = new AgentConnector(target);
                var rotationConnector = new AngleConnector(point);
                replay.AddDecorationWithGrowing((PieDecoration)new PieDecoration(700, 90, (startQuarter, endQuarter), Colors.LightOrange, 0.2, connector).UsingRotationConnector(rotationConnector), growingQuarter);
                if (endQuarter == growingQuarter) // If the attack went off
                {
                    replay.Decorations.Add(new PieDecoration(700, 90, (endQuarter, endQuarter + 1000), Colors.LightPink, 0.2, connector).UsingRotationConnector(rotationConnector)); // Lingering
                }
            }
        }

        private static void AddCausticExplosionDecoration(CombatReplay replay, AbstractSingleActor target, Point3D point, long attackEnd, (long start, long end) lifespan, long growing)
        {
            if (attackEnd >= lifespan.end) // If the attack started
            {
                Point3D flipPoint = -1 * point;
                var connector = new AgentConnector(target);
                var rotationConnector = new AngleConnector(point);
                var flippedRotationConnector = new AngleConnector(flipPoint);
                (long start, long end) lifespanLingering = (lifespan.end, lifespan.end + 1000);

                replay.AddDecorationWithGrowing((PieDecoration)new PieDecoration(1200, 90, lifespan, Colors.LightOrange, 0.2, connector).UsingRotationConnector(rotationConnector), growing); // Frontal
                replay.AddDecorationWithGrowing((PieDecoration)new PieDecoration(1200, 90, lifespan, Colors.LightOrange, 0.2, connector).UsingRotationConnector(flippedRotationConnector), growing); // Retro
                if (lifespan.end == growing) // If the attack went off
                {
                    replay.Decorations.Add(new PieDecoration(1200, 90, lifespanLingering, Colors.LightPink, 0.2, connector).UsingRotationConnector(rotationConnector)); // Frontal Lingering
                    replay.Decorations.Add(new PieDecoration(1200, 90, lifespanLingering, Colors.LightPink, 0.2, connector).UsingRotationConnector(flippedRotationConnector)); // Retro Lingering
                }
            }
        }

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            IReadOnlyList<AbstractCastEvent> casts = target.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd);

            switch (target.ID)
            {
                case (int)TargetID.Ensolyss:
                    IReadOnlyList<Segment> healthUpdates = target.GetHealthUpdates(log);
                    Segment percent66treshhold = healthUpdates.FirstOrDefault(x => x.Value <= 66);
                    Segment percent15treshhold = healthUpdates.FirstOrDefault(x => x.Value <= 15);
                    bool shield15_0Added = false; // This is used to also check wether the attack has been skipped or not

                    // Arkk's Shield
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.ArkkShieldIndicator, out IReadOnlyList<EffectEvent> shieldEffects))
                    {
                        foreach (EffectEvent shieldEffect in shieldEffects)
                        {
                            if (shieldEffect != null)
                            {
                                // The position check is necessary because with very high dps you can skip spawning the middle bubble, position is roughly X 1573 Y 1467
                                // In that case, we need to use the other set of decorations
                                if (!shield15_0Added && percent15treshhold != null && shieldEffect.Position.X < 1574 && shieldEffect.Position.X > 1572)
                                {
                                    int effectEnd = (int)target.LastAware;
                                    replay.Decorations.Add(new CircleDecoration(280, ((int)shieldEffect.Time, effectEnd), Colors.Blue, 0.4, new PositionConnector(shieldEffect.Position)));
                                    shield15_0Added = true;
                                }
                                else if (!shield15_0Added)
                                {
                                    int duration = 5000;
                                    int start = (int)shieldEffect.Time;
                                    int expectedHitEnd = start + duration;
                                    int attackEnd = start + duration;
                                    Segment stunSegment = target.GetBuffStatus(log, Stun, shieldEffect.Time, shieldEffect.Time + duration).FirstOrDefault(x => x.Value > 0);

                                    // Modify the attackEnd if:
                                    // Ensolyss reaches 15% during the bubble attack, interrupt it and start 15% phase
                                    // Ensolyss reaches 15% while stunned (stunSegment.End < percent15treshhold.Start)
                                    if (percent15treshhold != null && stunSegment != null && percent15treshhold.Start < attackEnd && stunSegment.End < percent15treshhold.Start)
                                    {
                                        attackEnd = (int)percent15treshhold.Start;
                                    }
                                    replay.Decorations.Add(new CircleDecoration(300, (start, attackEnd), Colors.Blue, 0.4, new PositionConnector(shieldEffect.Position)));
                                    replay.AddDecorationWithGrowing(new DoughnutDecoration(300, 2000, (start, attackEnd), Colors.Red, 0.2, new PositionConnector(shieldEffect.Position)), expectedHitEnd, true);
                                }
                            }
                        }
                    }

                    // 100% to 66% Doughnut
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.EnsolyssMiasmaDoughnut100_66, out IReadOnlyList<EffectEvent> miasmaEffects))
                    {
                        EffectEvent miasmaEffect = miasmaEffects.FirstOrDefault();
                        if (miasmaEffect != null)
                        {
                            if (percent66treshhold != null)
                            {
                                int start = (int)miasmaEffect.Time;
                                int effectEnd = (int)percent66treshhold.Start;
                                replay.Decorations.Add(new DoughnutDecoration(850, 1150, (start, effectEnd), Colors.Red, 0.2, new PositionConnector(miasmaEffect.Position)));
                            }
                            else // Wipe before 66%
                            {
                                int start = (int)miasmaEffect.Time;
                                int effectEnd = (int)target.LastAware;
                                replay.Decorations.Add(new DoughnutDecoration(850, 1150, (start, effectEnd), Colors.Red, 0.2, new PositionConnector(miasmaEffect.Position)));
                            }
                        }
                    }
                    // 66% to 15% Doughnut
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.EnsolyssMiasmaDoughnut66_15, out miasmaEffects))
                    {
                        EffectEvent miasmaEffect = miasmaEffects.FirstOrDefault();
                        if (miasmaEffect != null)
                        {
                            // Check if the Arkk's shield attack has been skipped with high dps
                            if (shield15_0Added && percent15treshhold != null)
                            {
                                int start = (int)miasmaEffect.Time;
                                int effectEnd = (int)percent15treshhold.Start;
                                replay.Decorations.Add(new DoughnutDecoration(595, 1150, (start, effectEnd), Colors.Red, 0.2, new PositionConnector(miasmaEffect.Position)));
                            }
                            else // Wipe before 15%
                            {
                                int start = (int)miasmaEffect.Time;
                                int effectEnd = (int)target.LastAware;
                                replay.Decorations.Add(new DoughnutDecoration(595, 1150, (start, effectEnd), Colors.Red, 0.2, new PositionConnector(miasmaEffect.Position)));
                            }
                        }
                    }
                    // 15% to 0% Doughnut
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.EnsolyssMiasmaDoughnut15_0, out miasmaEffects))
                    {
                        EffectEvent miasmaEffect = miasmaEffects.FirstOrDefault();
                        if (miasmaEffect != null)
                        {
                            // If Arkk's shield has been skipped at 15% this decoration should never be added
                            if (shield15_0Added)
                            {
                                int start = (int)miasmaEffect.Time;
                                int effectEnd = (int)target.LastAware;
                                replay.Decorations.Add(new DoughnutDecoration(280, 1150, (start, effectEnd), Colors.Red, 0.2, new PositionConnector(miasmaEffect.Position)));
                            }
                        }
                    }

                    // Tail Lash
                    var tailLash = casts.Where(x => x.SkillId == TailLashEnsolyss).ToList();
                    foreach (AbstractCastEvent c in tailLash)
                    {
                        int castDuration = 1550;
                        long expectedEndCast = c.Time + castDuration;
                        (long start, long end) lifespan = (c.Time, ComputeEndCastTimeByBuffApplication(log, target, Stun, c.Time, castDuration));
                        Point3D facing = target.GetCurrentRotation(log, c.Time + castDuration);
                        if (facing != null)
                        {
                            var rotation = new AngleConnector(facing);
                            var cone = (PieDecoration)new PieDecoration(600, 144, lifespan, Colors.LightOrange, 0.2, new AgentConnector(target)).UsingRotationConnector(rotation);
                            replay.AddDecorationWithGrowing(cone, expectedEndCast);
                        }
                    }

                    // Tormenting Blast (Quarter attacks)
                    var tormentingBlast = casts.Where(x => x.SkillId == TormentingBlast).ToList();
                    foreach (AbstractCastEvent c in tormentingBlast)
                    {
                        int firstQuarterAoe = 400;
                        int secondQuarterAoe = 900;
                        int firstQuarterHit = 1635;
                        int secondQuarterHit = 1900;
                        int castDuration = 1900;
                        (long start, long end) lifespan = (c.Time, ComputeEndCastTimeByBuffApplication(log, target, Stun, c.Time, castDuration));

                        // Facing point
                        Point3D facingDirection = target.GetCurrentRotation(log, c.Time, castDuration);
                        if (facingDirection != null)
                        {
                            // Calculated points
                            var frontalPoint = new Point3D(facingDirection.X, facingDirection.Y);
                            var leftPoint = new Point3D(facingDirection.Y * -1, facingDirection.X);

                            AddTormentingBlastDecoration(replay, target, lifespan, frontalPoint, firstQuarterAoe, firstQuarterHit); // Frontal
                            AddTormentingBlastDecoration(replay, target, lifespan, leftPoint, secondQuarterAoe, secondQuarterHit); // Left of frontal
                        }
                    }

                    // Caustic Grasp (AoE Pull)
                    var causticGrasp = casts.Where(x => x.SkillId == CausticGrasp).ToList();
                    foreach (AbstractCastEvent c in causticGrasp)
                    {
                        int castDuration = 1500;
                        long expectedEndCast = c.Time + castDuration;
                        (long start, long end) lifespan = (c.Time, ComputeEndCastTimeByBuffApplication(log, target, Stun, c.Time, castDuration));
                        replay.AddDecorationWithGrowing(new CircleDecoration(1300, lifespan, Colors.LightOrange, 0.2, new AgentConnector(target)), expectedEndCast);
                    }

                    // Upswing
                    var upswingEnso = casts.Where(x => x.SkillId == UpswingEnsolyss).ToList();
                    foreach (AbstractCastEvent c in upswingEnso)
                    {
                        int castDuration = 1333;
                        long expectedEndCast = c.Time + castDuration;
                        (long start, long end) lifespan = (c.Time, ComputeEndCastTimeByBuffApplication(log, target, Stun, c.Time, castDuration));
                        (long start, long end) lifespanShockwave = (lifespan.end, c.Time + 3100);
                        GeographicalConnector connector = new AgentConnector(target);
                        replay.AddDecorationWithGrowing(new CircleDecoration(600, lifespan, Colors.LightOrange, 0.2, connector), expectedEndCast);
                        // Shockwave
                        replay.AddShockwave(connector, lifespanShockwave, Colors.Yellow, 0.4, 1500);
                    }

                    // 66% & 33% Breakbars
                    var causticExplosion = casts.Where(x => x.SkillId == CausticExplosionEnsolyss).ToList();
                    foreach (AbstractCastEvent c in causticExplosion)
                    {
                        int castDuration = 15000;
                        long expectedEndCast = c.Time + castDuration;
                        int durationQuarter = 3000;
                        (long start, long end) lifespan = (c.Time, ComputeEndCastTimeByBuffApplication(log, target, Stun, c.Time, castDuration));

                        // Circle going in
                        replay.AddDecorationWithGrowing(new DoughnutDecoration(0, 2000, lifespan, Colors.Red, 0.2, new AgentConnector(target)), expectedEndCast, true);

                        if (lifespan.end == expectedEndCast)
                        {
                            // Explosion
                            replay.Decorations.Add(new CircleDecoration(2000, (lifespan.end, lifespan.end + 300), Colors.Red, 0.4, new AgentConnector(target)));
                        }

                        // Initial facing point
                        Point3D facingDirection = target.GetCurrentRotation(log, c.Time, castDuration);
                        if (facingDirection != null)
                        {
                            // Calculated other quarters from initial point
                            var frontalPoint = new Point3D(facingDirection.X, facingDirection.Y);
                            var leftPoint = new Point3D(facingDirection.Y * -1, facingDirection.X);
                            int initialDelay = 1500;

                            // First quarters
                            (long start, long end) lifespanFirst = (c.Time + initialDelay, Math.Min(c.Time + initialDelay + durationQuarter, lifespan.end));
                            long growingFirst = lifespanFirst.start + durationQuarter;
                            AddCausticExplosionDecoration(replay, target, frontalPoint, lifespan.end, lifespanFirst, growingFirst);
                            // Second quarters
                            (long start, long end) lifespanSecond = (lifespanFirst.end, Math.Min(lifespanFirst.end + durationQuarter, lifespan.end));
                            long growingSecond = lifespanSecond.start + durationQuarter;
                            AddCausticExplosionDecoration(replay, target, leftPoint, lifespan.end, lifespanSecond, growingSecond);
                            // Third quarters
                            (long start, long end) lifespanThird = (lifespanSecond.end, Math.Min(lifespanSecond.end + durationQuarter, lifespan.end));
                            long growingThird = lifespanThird.start + durationQuarter;
                            AddCausticExplosionDecoration(replay, target, frontalPoint, lifespan.end, lifespanThird, growingThird);
                            // Fourth quarters
                            (long start, long end) lifespanFourth = (lifespanThird.end, Math.Min(lifespanThird.end + durationQuarter, lifespan.end));
                            long growingFourth = lifespanFourth.start + durationQuarter;
                            AddCausticExplosionDecoration(replay, target, leftPoint, lifespan.end, lifespanFourth, growingFourth);
                        }
                    }

                    // Lunge (Dash)
                    var lungeEnso = casts.Where(x => x.SkillId == LungeEnsolyss).ToList();
                    foreach (AbstractCastEvent c in lungeEnso)
                    {
                        int castDuration = 1000;
                        Point3D facing = target.GetCurrentRotation(log, c.Time + castDuration);
                        if (facing != null)
                        {
                            var rotation = new AngleConnector(facing);
                            (long start, long end) lifespan = (c.Time, c.Time + castDuration);
                            replay.Decorations.Add(new RectangleDecoration(1700, target.HitboxWidth, lifespan, Colors.LightOrange, 0.2, new AgentConnector(target).WithOffset(new Point3D(850, 0), true)).UsingRotationConnector(rotation));
                        }
                    }

                    // Rampage - 8 Arrows attack
                    var rampage = casts.Where(x => x.SkillId == RampageEnsolyss).ToList();
                    foreach (AbstractCastEvent c in rampage)
                    {
                        // Cast duration is 4050 but visually fits better 4450
                        int castDuration = 4050;
                        int visualDuration = 4450;
                        int warningDuration = 1800;
                        (long start, long end) lifespan = (c.Time, c.Time + visualDuration);
                        (long start, long end) lifespanWarning = (c.Time, c.Time + warningDuration);
                        (long start, long end) lifespanShockwave = (c.Time, c.Time + castDuration);
                        // Red outline
                        var outline = (CircleDecoration)new CircleDecoration(380, lifespan, Colors.Red, 0.4, new AgentConnector(target)).UsingFilled(false);
                        replay.Decorations.Add(outline);
                        // Orange warning circle
                        var warning = new CircleDecoration(380, lifespanWarning, Colors.LightOrange, 0.2, new AgentConnector(target));
                        replay.AddDecorationWithGrowing(warning, lifespanWarning.end);
                        // Growing inwards shockwave
                        var shockwave = (CircleDecoration)new CircleDecoration(1200, lifespanShockwave, Colors.Yellow, 0.4, new AgentConnector(target)).UsingFilled(false).UsingGrowingEnd(lifespanShockwave.end, true);
                        replay.Decorations.Add(shockwave);
                        // 8 Arrows
                        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.EnsolyssArrow, out IReadOnlyList<EffectEvent> arrows))
                        {
                            foreach (EffectEvent effect in arrows.Where(x => x.Time >= c.Time && x.Time < c.Time + visualDuration))
                            {
                                if (effect is EffectEventCBTS51)
                                {
                                    (long start, long end) lifespanArrow = (effect.Time, effect.Time + effect.Duration);
                                    var rotation = new AngleConnector(effect.Rotation.Z);
                                    var arrow = (RectangleDecoration)new RectangleDecoration(30, 600, lifespanArrow, Colors.LightOrange, 0.2, new PositionConnector(effect.Position).WithOffset(new Point3D(0, 300), true)).UsingRotationConnector(rotation);
                                    replay.Decorations.Add(arrow);
                                }
                            }
                        }
                    }
                    break;
                case (int)TrashID.NightmareHallucination1:
                    // Lunge (Dash)
                    var lungeHallu = casts.Where(x => x.SkillId == LungeNightmareHallucination).ToList();
                    foreach (AbstractCastEvent c in lungeHallu)
                    {
                        int castDuration = 1000;
                        Point3D facing = target.GetCurrentRotation(log, c.Time + castDuration);
                        if (facing != null)
                        {
                            var rotation = new AngleConnector(facing);
                            (long start, long end) lifespan = (c.Time, c.Time + castDuration);
                            replay.Decorations.Add(new RectangleDecoration(1700, target.HitboxWidth, lifespan, Colors.LightOrange, 0.2, new AgentConnector(target).WithOffset(new Point3D(850, 0), true)).UsingRotationConnector(rotation));
                        }
                    }

                    // Upswing
                    var upswingHallu = casts.Where(x => x.SkillId == UpswingHallucination).ToList();
                    foreach (AbstractCastEvent c in upswingHallu)
                    {
                        int castDuration = 1333;
                        (long start, long end) lifespan = (c.Time, c.Time + castDuration);
                        replay.AddDecorationWithGrowing(new CircleDecoration(300, lifespan, Colors.LightOrange, 0.1, new AgentConnector(target)), lifespan.end);
                    }
                    break;
                case (int)TrashID.NightmareHallucination2:
                    break;
                default:
                    break;
            }
        }

        internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log)
        {
            base.ComputeEnvironmentCombatReplayDecorations(log);

            // Nightmare Altar Orb AoE 1
            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.EnsolyssNightmareAltarOrangeAoE, out IReadOnlyList<EffectEvent> indicators1))
            {
                foreach (EffectEvent effect in indicators1)
                {
                    (long start, long end) lifespan = (effect.Time, effect.Time + effect.Duration);
                    EnvironmentDecorations.Add(new CircleDecoration(120, lifespan, Colors.Orange, 0.3, new PositionConnector(effect.Position)));
                }
            }

            // Nightmare Altar Orb AoE 2
            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.EnsolyssNightmareAltarLightOrangeAoE, out IReadOnlyList<EffectEvent> indicators2))
            {
                foreach (EffectEvent effect in indicators2)
                {
                    (long start, long end) lifespan = (effect.Time, effect.Time + effect.Duration);
                    EnvironmentDecorations.Add(new CircleDecoration(180, lifespan, Colors.LightOrange, 0.2, new PositionConnector(effect.Position)));
                }
            }

            // Altar Shockwave
            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.EnsolyssNightmareAltarShockwave, out IReadOnlyList<EffectEvent> waveEffects))
            {
                foreach (EffectEvent effect in waveEffects)
                {
                    (long start, long end) lifespan = (effect.Time, effect.Time + 2000);
                    EnvironmentDecorations.Add(new CircleDecoration(1200, lifespan, Colors.Yellow, 0.4, new PositionConnector(effect.Position)).UsingFilled(false).UsingGrowingEnd(lifespan.end));
                }
            }

            // Caustic Barrage
            AddDistanceCorrectedOrbDecorations(log, EnvironmentDecorations, EffectGUIDs.CausticBarrageIndicator, TargetID.Ensolyss, 210, 1000, 1300);
        }
    }
}
