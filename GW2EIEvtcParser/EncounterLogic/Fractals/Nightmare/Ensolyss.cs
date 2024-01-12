using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;
using GW2EIEvtcParser.Extensions;
using System.Collections;
using static GW2EIEvtcParser.ArcDPSEnums;

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
            new PlayerDstHitMechanic(RampageEnsolyss, "Rampage", new MechanicPlotlySetting(Symbols.AsteriskOpen,Colors.Red), "Rampage","Rampage (asterisk shaped Arrow attack)", "Rampage",150),
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
                //(int)ArcDPSEnums.TrashID.NightmareAltar,
            };
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            var trashIDs = new List<ArcDPSEnums.TrashID>
            {
                TrashID.NightmareHallucination1,
                TrashID.NightmareHallucination2,
                //ArcDPSEnums.TrashID.NightmareAltar,
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

        private static void AddTormentingBlastDecoration(CombatReplay replay, AbstractSingleActor target, int start, int attackEnd, Point3D point, int quarterAoE, int quarterHit)
        {
            int startQuarter = start + quarterAoE;
            int endQuarter = start + quarterHit;
            int growingQuarter = start + quarterHit;
            if (attackEnd >= endQuarter) // If the attack started
            {
                var connector = new AgentConnector(target);
                var rotationConnector = new AngleConnector(point);
                replay.AddDecorationWithGrowing((PieDecoration)new PieDecoration(700, 90, (startQuarter, endQuarter), Colors.Orange, 0.2, connector).UsingRotationConnector(rotationConnector), growingQuarter);
                if (endQuarter == growingQuarter) // If the attack went off
                {
                    replay.Decorations.Add(new PieDecoration(700, 90, (endQuarter, endQuarter + 1000), Colors.LightPink, 0.2,connector).UsingRotationConnector(rotationConnector)); // Lingering
                }
            }
        }

        private static void AddCausticExplosionDecoration(CombatReplay replay, AbstractSingleActor target, Point3D point, int attackEnd, int start, int end, int growing)
        {
            if (attackEnd >= end) // If the attack started
            {
                Point3D flipPoint = -1 * point;
                var connector = new AgentConnector(target);
                var rotationConnector = new AngleConnector(point);
                // Frontal
                replay.AddDecorationWithGrowing((PieDecoration)new PieDecoration(1200, 90, (start, end), Colors.Orange, 0.2, connector).UsingRotationConnector(rotationConnector), growing);
                if (end == growing) // If the attack went off
                {
                    replay.Decorations.Add(new PieDecoration(1200, 90, (end, end + 1000), Colors.LightPink, 0.2, connector).UsingRotationConnector(rotationConnector)); // Lingering
                }
                // Retro
                var flippedRotationConnector = new AngleConnector(flipPoint);
                replay.AddDecorationWithGrowing((PieDecoration)new PieDecoration(1200, 90, (start, end), Colors.Orange, 0.2, connector).UsingRotationConnector(flippedRotationConnector), growing);
                if (end == growing) // If the attack went off
                {
                    replay.Decorations.Add(new PieDecoration(1200, 90, (end, end + 1000), Colors.LightPink, 0.2, connector).UsingRotationConnector(flippedRotationConnector)); // Lingering
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
                                replay.Decorations.Add(new DoughnutDecoration( 595, 1150, (start, effectEnd), Colors.Red, 0.2, new PositionConnector(miasmaEffect.Position)));
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
                        int duration = 1550;
                        int openingAngle = 144;
                        uint radius = 600;
                        int start = (int)c.Time;
                        int attackEnd = (int)c.Time + duration;
                        Segment stunSegment = target.GetBuffStatus(log, Stun, c.Time, c.Time + duration).FirstOrDefault(x => x.Value > 0);
                        if (stunSegment != null)
                        {
                            attackEnd = Math.Min((int)stunSegment.Start, attackEnd); // Start of Stun
                        }
                        if (replay.Rotations.Any())
                        {
                            replay.Decorations.Add(new PieDecoration(radius, openingAngle, (start, attackEnd), Colors.Orange, 0.2,new AgentConnector(target)).UsingRotationConnector(new AgentFacingConnector(target)));
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
                        int duration = 1900;
                        int start = (int)c.Time;
                        int attackEnd = start + duration;

                        Segment stunSegment = target.GetBuffStatus(log, Stun, c.Time, c.Time + duration).FirstOrDefault(x => x.Value > 0);
                        if (stunSegment != null)
                        {
                            attackEnd = Math.Min((int)stunSegment.Start, attackEnd); // Start of Stun
                        }

                        // Facing point
                        Point3D facingDirection = target.GetCurrentRotation(log, c.Time, duration);
                        if (facingDirection != null)
                        {
                            // Calculated points
                            var frontalPoint = new Point3D(facingDirection.X, facingDirection.Y);
                            var leftPoint = new Point3D(facingDirection.Y * -1, facingDirection.X);

                            AddTormentingBlastDecoration(replay, target, start, attackEnd, frontalPoint, firstQuarterAoe, firstQuarterHit); // Frontal
                            AddTormentingBlastDecoration(replay, target, start, attackEnd, leftPoint, secondQuarterAoe, secondQuarterHit); // Left of frontal
                        }
                    }

                    // Caustic Grasp (AoE Pull)
                    var causticGrasp = casts.Where(x => x.SkillId == CausticGrasp).ToList();
                    foreach (AbstractCastEvent c in causticGrasp)
                    {
                        int duration = 1500;
                        int start = (int)c.Time;
                        int expectedHitEnd = start + duration;
                        int attackEnd = start + duration;
                        Segment stunSegment = target.GetBuffStatus(log, Stun, c.Time, c.Time + duration).FirstOrDefault(x => x.Value > 0);
                        if (stunSegment != null)
                        {
                            attackEnd = Math.Min((int)stunSegment.Start, attackEnd); // Start of Stun
                        }
                        replay.AddDecorationWithGrowing(new CircleDecoration(1300, (start, attackEnd), Colors.Orange, 0.2, new AgentConnector(target)), expectedHitEnd);
                    }

                    // Upswing
                    var upswingEnso = casts.Where(x => x.SkillId == UpswingEnsolyss).ToList();
                    foreach (AbstractCastEvent c in upswingEnso)
                    {
                        int duration = 1333;
                        int start = (int)c.Time;
                        int expectedHitEnd = start + duration;
                        int attackEnd = start + duration;
                        int endTimeWave = start + 3100;
                        Segment stunSegment = target.GetBuffStatus(log, Stun, c.Time, c.Time + duration).FirstOrDefault(x => x.Value > 0);
                        if (stunSegment != null)
                        {
                            attackEnd = Math.Min((int)stunSegment.Start, attackEnd); // Start of Stun
                        }
                        replay.AddDecorationWithGrowing(new CircleDecoration(600, (start, attackEnd), Colors.Orange, 0.2, new AgentConnector(target)), expectedHitEnd);
                        // Shockwave
                        replay.Decorations.Add(new CircleDecoration( 1500, (attackEnd, endTimeWave), Colors.Yellow, 0.4, new AgentConnector(target)).UsingFilled(false).UsingGrowingEnd(endTimeWave));
                    }

                    // 66% & 33% Breakbars
                    var causticExplosion = casts.Where(x => x.SkillId == CausticExplosionEnsolyss).ToList();
                    foreach (AbstractCastEvent c in causticExplosion)
                    {
                        int duration = 15000;
                        int start = (int)c.Time;
                        int expectedHitEnd = start + duration;
                        int attackEnd = start + duration;
                        int durationQuarter = 3000;

                        Segment stunSegment = target.GetBuffStatus(log, Stun, c.Time, c.Time + duration).FirstOrDefault(x => x.Value > 0);
                        if (stunSegment != null)
                        {
                            attackEnd = Math.Min((int)stunSegment.Start, attackEnd); // Start of Stun
                        }
                        // Circle going in
                        replay.AddDecorationWithGrowing(new DoughnutDecoration(0, 2000, (start, attackEnd), Colors.Red, 0.2, new AgentConnector(target)), expectedHitEnd, true);
                        if (attackEnd == expectedHitEnd)
                        {
                            replay.Decorations.Add(new CircleDecoration(2000, (attackEnd, attackEnd + 300), Colors.Red, 0.4, new AgentConnector(target)));
                        }
                        // Initial facing point
                        Point3D facingDirection = target.GetCurrentRotation(log, c.Time, duration);
                        if (facingDirection != null)
                        {
                            // Calculated other quarters from initial point
                            var frontalPoint = new Point3D(facingDirection.X, facingDirection.Y);
                            var leftPoint = new Point3D(facingDirection.Y * -1, facingDirection.X);
                            // First quarters
                            int startFirstQuarter = start + 1500;
                            int endFirstQuarter = Math.Min(startFirstQuarter + durationQuarter, attackEnd);
                            int growingFirstQuarter = startFirstQuarter + durationQuarter;
                            AddCausticExplosionDecoration(replay, target, frontalPoint, attackEnd, startFirstQuarter, endFirstQuarter, growingFirstQuarter);
                            // Second quarters
                            int startSecondQuarter = endFirstQuarter;
                            int endSecondQuarter = Math.Min(startSecondQuarter + durationQuarter, attackEnd);
                            int growingSecondQuarter = startSecondQuarter + durationQuarter;
                            AddCausticExplosionDecoration(replay, target, leftPoint, attackEnd, startSecondQuarter, endSecondQuarter, growingSecondQuarter);
                            // Third quarters
                            int startThirdQuarter = endSecondQuarter;
                            int endThirdQuarter = Math.Min(startThirdQuarter + durationQuarter, attackEnd);
                            int growingThirdQuarter = startThirdQuarter + durationQuarter;
                            AddCausticExplosionDecoration(replay, target, frontalPoint, attackEnd, startThirdQuarter, endThirdQuarter, growingThirdQuarter);
                            // Fourth quarters
                            int startFourthQuarter = endThirdQuarter;
                            int endFourthQuarter = Math.Min(startFourthQuarter + durationQuarter, attackEnd);
                            int growingFourthQuarter = startFourthQuarter + durationQuarter;
                            AddCausticExplosionDecoration(replay, target, leftPoint, attackEnd, startFourthQuarter, endFourthQuarter, growingFourthQuarter);
                        }
                    }

                    // Lunge (Dash)
                    var lungeEnso = casts.Where(x => x.SkillId == LungeEnsolyss).ToList();
                    foreach (AbstractCastEvent c in lungeEnso)
                    {
                        int startLine = (int)c.Time;
                        int lineEffectEnd = (int)c.Time + 1000;
                        if (replay.Rotations.Any())
                        {
                            replay.Decorations.Add(new RectangleDecoration(1700, target.HitboxWidth, (startLine, lineEffectEnd), Colors.Orange, 0.2, new AgentConnector(target).WithOffset(new Point3D(850, 0), true)).UsingRotationConnector(new AgentFacingConnector(target)));
                        }
                    }

                    break;
                case (int)TrashID.NightmareHallucination1:
                    // Lunge (Dash)
                    var lungeHallu = casts.Where(x => x.SkillId == LungeNightmareHallucination).ToList();
                    foreach (AbstractCastEvent c in lungeHallu)
                    {
                        int startLine = (int)c.Time;
                        int lineEffectEnd = (int)c.Time + 1000;
                        if (replay.Rotations.Any())
                        {
                            replay.Decorations.Add(new RectangleDecoration(1700, target.HitboxWidth, (startLine, lineEffectEnd), Colors.Orange, 0.2, new AgentConnector(target).WithOffset(new Point3D(850, 0), true)).UsingRotationConnector(new AgentFacingConnector(target)));
                        }
                    }

                    // Upswing
                    var upswingHallu = casts.Where(x => x.SkillId == UpswingHallucination).ToList();
                    foreach (AbstractCastEvent c in upswingHallu)
                    {
                        int start = (int)c.Time;
                        int endTime = (int)c.Time + 1333;
                        replay.AddDecorationWithGrowing(new CircleDecoration( 300, (start, endTime), Colors.LightOrange, 0.1, new AgentConnector(target)), endTime);
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

            // Caustic Barrage
            //EffectGUIDEvent causticBarrage = log.CombatData.GetEffectGUIDEvent(EffectGUIDs.CausticBarrageHitEffect);
            //if (causticBarrage != null)
            //{
            //    var barrageEffects = log.CombatData.GetEffectEventsByEffectID(causticBarrage.ContentID).ToList();
            //    foreach (EffectEvent barrageEffect in barrageEffects)
            //    {
            //        int duration = 500;
            //        int start = (int)barrageEffect.Time;
            //        int effectEnd = start + duration;
            //        EnvironmentDecorations.Add(new CircleDecoration(true, 0, 100, (start, effectEnd), Colors.Orange, 0.2, new PositionConnector(barrageEffect.Position)));
            //    }
            //}

            // Altar Shockwave
            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.EnsolyssNightmareAltarShockwave, out IReadOnlyList<EffectEvent> waveEffects))
            {
                foreach (EffectEvent waveEffect in waveEffects)
                {
                    int duration = 2000;
                    int start = (int)waveEffect.Time;
                    int effectEnd = start + duration;
                    EnvironmentDecorations.Add(new CircleDecoration(1150, (start, effectEnd), Colors.Yellow, 0.4, new PositionConnector(waveEffect.Position)).UsingFilled(false).UsingGrowingEnd(effectEnd));
                }
            }
        }
    }
}
