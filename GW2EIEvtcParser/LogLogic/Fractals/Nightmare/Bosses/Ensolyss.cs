using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicTimeUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal class Ensolyss : Nightmare
{
    internal readonly MechanicGroup Mechanics = new MechanicGroup([
            new MechanicGroup(
                [
                    new PlayerDstHealthDamageHitMechanic([ LungeEnsolyss, LungeNightmareHallucination ], new MechanicPlotlySetting(Symbols.TriangleRightOpen,Colors.LightOrange), "Charge", "Lunge (KB charge over arena)","Charge", 150),
                ]
            ),
            new MechanicGroup(
                [
                    new PlayerDstHealthDamageHitMechanic(UpswingEnsolyss, new MechanicPlotlySetting(Symbols.Circle,Colors.Orange), "Smash 1", "High damage Jump hit","First Smash", 0),
                    new PlayerDstHealthDamageHitMechanic(UpswingHallucination, new MechanicPlotlySetting(Symbols.Circle, Colors.LightOrange), "Hall.AoE", "Hit by Hallucination Explosion", "Hallu Explosion", 0),
                ]
            ),
            new PlayerDstHealthDamageHitMechanic([ NigthmareMiasmaEnsolyss1, NigthmareMiasmaEnsolyss2, NigthmareMiasmaEnsolyss3 ], new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Magenta), "Goo", "Nightmare Miasma (Goo)","Miasma", 0),
            new MechanicGroup(
                [
                    new EnemyCastStartMechanic(CausticExplosionEnsolyss, new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkTeal), "CC", "After Phase CC","Breakbar", 0),
                    new EnemyCastEndMechanic(CausticExplosionEnsolyss, new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Red), "CC Fail", "After Phase CC Failed","CC Fail", 0).UsingChecker( (ce,log) => ce.ActualDuration >= 15260),
                    new EnemyCastEndMechanic(CausticExplosionEnsolyss, new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkGreen), "CCed", "After Phase CC Success","CCed", 0).UsingChecker( (ce, log) => ce.ActualDuration < 15260),
                    new PlayerDstHealthDamageHitMechanic(CausticExplosionEnsolyss, new MechanicPlotlySetting(Symbols.Bowtie,Colors.Yellow), "CC KB", "Knockback hourglass during CC","CC KB", 0),
                ]
            ),
            new EnemyCastStartMechanic([ NightmareDevastation1, NightmareDevastation2 ], new MechanicPlotlySetting(Symbols.SquareOpen,Colors.Blue), "Bubble", "Nightmare Devastation (bubble attack)","Bubble", 0),
            new PlayerDstHealthDamageHitMechanic(TailLashEnsolyss, new MechanicPlotlySetting(Symbols.TriangleLeft,Colors.Yellow), "Tail", "Tail Lash Ensolyss (half circle Knockback)","Tail Lash (Ensolyss)", 0),
            new PlayerDstHealthDamageHitMechanic(RampageEnsolyss, new MechanicPlotlySetting(Symbols.BowtieOpen,Colors.Red), "Rampage", "Rampage (asterisk shaped Arrow attack)","Rampage", 150),
            new PlayerDstHealthDamageHitMechanic(CausticGrasp, new MechanicPlotlySetting(Symbols.StarDiamond,Colors.LightOrange), "Pull", "Caustic Grasp (Arena Wide Pull)","Pull", 0),
            new PlayerDstHealthDamageHitMechanic(TormentingBlast, new MechanicPlotlySetting(Symbols.Diamond,Colors.Yellow), "Quarter", "Tormenting Blast (Two Quarter Circle attacks)","Quarter circle", 0),
        ]);
    public Ensolyss(int triggerID) : base(triggerID)
    {
        MechanicList.Add(Mechanics);
        Extension = "ensol";
        Icon = EncounterIconEnsolyss;
        LogCategoryInformation.InSubCategoryOrder = 2;
        LogID |= 0x000003;
    }

    internal override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log, CombatReplayDecorationContainer arenaDecorations)
    {
        var crMap = new CombatReplayMap((366, 366),
                        (132, 1, 3012, 2881));
        AddArenaDecorationsPerEncounter(log, arenaDecorations, LogID, CombatReplayEnsolyss, crMap);
        return crMap;
    }

    internal override LogData.LogMode GetLogMode(CombatData combatData, AgentData agentData, LogData logData)
    {
        return LogData.LogMode.CMNoName;
    }

    internal override IReadOnlyList<TargetID>  GetTargetsIDs()
    {
        return
        [
            TargetID.Ensolyss,
            //(int)TargetID.NightmareAltar,
        ];
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        var trashIDs = new List<TargetID>(2 + base.GetTrashMobsIDs().Count);
        trashIDs.AddRange(base.GetTrashMobsIDs());
        trashIDs.Add(TargetID.NightmareHallucination1);
        trashIDs.Add(TargetID.NightmareHallucination2);
        //trashIDs.Add(TargetID.NightmareAltar);
        return trashIDs;
    }

    internal static void IgnoreFakeEnsolysses(AgentData agentData, List<CombatItem> combatData)
    {
        var validMaxHPUpdatesForEnsolyss = combatData.Where(x => x.IsStateChange == StateChange.MaxHealthUpdate && MaxHealthUpdateEvent.GetMaxHealth(x) >= 7e6).Select(x => agentData.GetAgent(x.SrcAgent, x.Time)).ToHashSet();
        foreach (var ensolyss in agentData.GetNPCsByID(TargetID.Ensolyss))
        {
            if (!validMaxHPUpdatesForEnsolyss.Contains(ensolyss))
            {
                ensolyss.OverrideID(IgnoredSpecies, agentData);
            }
        }
    }

    internal override void HandleCriticalAgents(EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        IgnoreFakeEnsolysses(agentData, combatData);
    }

    internal override long GetLogOffset(EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData)
    {
        // ensolyss spawns with invulnerability
        var ensolyss = agentData.GetNPCsByID(TargetID.Ensolyss).FirstOrDefault() ?? throw new MissingKeyActorsException("Ensolyss not found");
        long start = GetLogOffsetByInvulnStart(logData, combatData, ensolyss, Determined762);

        // ensolyss exits combat during split phases and reenters after
        // make sure we dont have a late start via caustic explosion, expected to happen 2s after invuln remove in later phases
        var causticExplosion = combatData.FirstOrDefault(x => x.SkillID == CausticExplosionEnsolyss && x.StartCasting() && x.SrcMatchesAgent(ensolyss) && x.Time > start);
        if (causticExplosion != null && causticExplosion.Time <= start + 3000)
        {
            return GetGenericLogOffset(logData);
        }
        return start;
    }
    internal static List<PhaseData> ComputePhases(ParsedEvtcLog log, SingleActor ensolyss, EncounterPhaseData encounterPhase, bool requirePhases)
    {
        if (!requirePhases)
        {
            return [];
        }
        var phases = new List<PhaseData>(5);
        phases.AddRange(GetPhasesByInvul(log, Determined762, ensolyss, true, true, encounterPhase.Start, encounterPhase.End));
        for (int i = 0; i < phases.Count; i++)
        {
            PhaseData phase = phases[i];
            var index = i + 1;
            phase.AddParentPhase(encounterPhase);
            if (index % 2 == 0)
            {
                phase.Name = "Nightmare Altars " +(index / 2);
                phase.AddTarget(ensolyss, log);
            }
            else
            {
                phase.Name = "Phase " + (index + 1) / 2;
                phase.AddTarget(ensolyss, log);
            }
        }
        return phases;
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor ensolyss = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Ensolyss)) ?? throw new MissingKeyActorsException("Ensolyss not found");
        phases[0].AddTarget(ensolyss, log);
        phases.AddRange(ComputePhases(log, ensolyss, (EncounterPhaseData)phases[0], requirePhases));
        return phases;
    }

    private static void AddTormentingBlastDecoration(CombatReplay replay, SingleActor target, (long start, long end) lifespan, in Vector3 point, int quarterAoE, int quarterHit)
    {
        long startQuarter = lifespan.start + quarterAoE;
        long endQuarter = lifespan.start + quarterHit;
        long growingQuarter = lifespan.start + quarterHit;
        if (lifespan.end >= endQuarter) // If the attack started
        {
            var connector = new AgentConnector(target);
            var rotationConnector = new AngleConnector(point);
            replay.Decorations.AddWithGrowing((PieDecoration)new PieDecoration(700, 90, (startQuarter, endQuarter), Colors.LightOrange, 0.2, connector).UsingRotationConnector(rotationConnector), growingQuarter);
            if (endQuarter == growingQuarter) // If the attack went off
            {
                replay.Decorations.Add(new PieDecoration(700, 90, (endQuarter, endQuarter + 1000), Colors.LightPink, 0.2, connector).UsingRotationConnector(rotationConnector)); // Lingering
            }
        }
    }

    private static void AddCausticExplosionDecoration(CombatReplay replay, SingleActor target, in Vector3 point, long attackEnd, (long start, long end) lifespan, long growing)
    {
        if (attackEnd >= lifespan.end) // If the attack started
        {
            var flipPoint = -1 * point;
            var connector = new AgentConnector(target);
            var rotationConnector = new AngleConnector(point);
            var flippedRotationConnector = new AngleConnector(flipPoint);
            (long start, long end) lifespanLingering = (lifespan.end, lifespan.end + 1000);

            replay.Decorations.AddWithGrowing((PieDecoration)new PieDecoration(1200, 90, lifespan, Colors.LightOrange, 0.2, connector).UsingRotationConnector(rotationConnector), growing); // Frontal
            replay.Decorations.AddWithGrowing((PieDecoration)new PieDecoration(1200, 90, lifespan, Colors.LightOrange, 0.2, connector).UsingRotationConnector(flippedRotationConnector), growing); // Retro
            if (lifespan.end == growing) // If the attack went off
            {
                replay.Decorations.Add(new PieDecoration(1200, 90, lifespanLingering, Colors.LightPink, 0.2, connector).UsingRotationConnector(rotationConnector)); // Frontal Lingering
                replay.Decorations.Add(new PieDecoration(1200, 90, lifespanLingering, Colors.LightPink, 0.2, connector).UsingRotationConnector(flippedRotationConnector)); // Retro Lingering
            }
        }
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IsInstance)
        {
            base.ComputeNPCCombatReplayActors(target, log, replay);
        }
        long castDuration;
        long growing;
        (long start, long end) lifespan;

        switch (target.ID)
        {
            case (int)TargetID.Ensolyss:
                IReadOnlyList<Segment> healthUpdates = target.GetHealthUpdates(log);
                Segment? percent66treshhold = healthUpdates.FirstOrNull((in Segment x) => x.Value <= 66);
                Segment? percent15treshhold = healthUpdates.FirstOrNull((in Segment x) => x.Value <= 15);
                bool shield15_0Added = false; // This is used to also check wether the attack has been skipped or not

                // Arkk's Shield
                if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.ArkkShieldIndicator, out var shieldEffects))
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
                                Segment? stunSegment = target.GetBuffStatus(log, Stun, shieldEffect.Time, shieldEffect.Time + duration).FirstOrNull((in Segment x) => x.Value > 0);

                                // Modify the attackEnd if:
                                // Ensolyss reaches 15% during the bubble attack, interrupt it and start 15% phase
                                // Ensolyss reaches 15% while stunned (stunSegment.End < percent15treshhold.Start)
                                if (percent15treshhold is Segment _percent15treshhold && stunSegment != null && _percent15treshhold.Start < attackEnd && _percent15treshhold.End < _percent15treshhold.Start)
                                {
                                    attackEnd = (int)_percent15treshhold.Start;
                                }
                                replay.Decorations.Add(new CircleDecoration(300, (start, attackEnd), Colors.Blue, 0.4, new PositionConnector(shieldEffect.Position)));
                                replay.Decorations.AddWithGrowing(new DoughnutDecoration(300, 2000, (start, attackEnd), Colors.Red, 0.2, new PositionConnector(shieldEffect.Position)), expectedHitEnd, true);
                            }
                        }
                    }
                }

                // 100% to 66% Doughnut
                if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.EnsolyssMiasmaDoughnut100_66, out var miasmaEffects))
                {
                    EffectEvent? miasmaEffect = miasmaEffects.FirstOrDefault();
                    if (miasmaEffect != null)
                    {
                        if (percent66treshhold != null)
                        {
                            int start = (int)miasmaEffect.Time;
                            int effectEnd = (int)percent66treshhold.Value.Start;
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
                    EffectEvent? miasmaEffect = miasmaEffects.FirstOrDefault();
                    if (miasmaEffect != null)
                    {
                        // Check if the Arkk's shield attack has been skipped with high dps
                        if (shield15_0Added && percent15treshhold != null)
                        {
                            int start = (int)miasmaEffect.Time;
                            int effectEnd = (int)percent15treshhold.Value.Start;
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
                    EffectEvent? miasmaEffect = miasmaEffects.FirstOrDefault();
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

                foreach (CastEvent cast in target.GetAnimatedCastEvents(log))
                {
                    switch (cast.SkillID)
                    {
                        // Tail Lash - Cone Swipe
                        case TailLashEnsolyss:
                            castDuration = 1550;
                            growing = cast.Time + castDuration;
                            lifespan = (cast.Time, ComputeEndCastTimeByBuffApplication(log, target, Stun, cast.Time, castDuration));
                            if (target.TryGetCurrentFacingDirection(log, cast.Time + castDuration, out var facing))
                            {
                                var rotation = new AngleConnector(facing);
                                var cone = (PieDecoration)new PieDecoration(530, 160, lifespan, Colors.LightOrange, 0.2, new AgentConnector(target)).UsingRotationConnector(rotation);
                                replay.Decorations.AddWithGrowing(cone, growing);
                            }
                            break;
                        // Tormenting Blast - Quarter attacks
                        case TormentingBlast:
                            castDuration = 1900;
                            int firstQuarterAoe = 400;
                            int secondQuarterAoe = 900;
                            int firstQuarterHit = 1635;
                            int secondQuarterHit = 1900;
                            lifespan = (cast.Time, ComputeEndCastTimeByBuffApplication(log, target, Stun, cast.Time, castDuration));

                            // Facing point
                            if (target.TryGetCurrentFacingDirection(log, cast.Time, out var facingDirection, castDuration))
                            {
                                // Calculated points
                                var frontalPoint = new Vector3(facingDirection.X, facingDirection.Y, 0);
                                var leftPoint = new Vector3(facingDirection.Y * -1, facingDirection.X, 0);

                                AddTormentingBlastDecoration(replay, target, lifespan, frontalPoint, firstQuarterAoe, firstQuarterHit); // Frontal
                                AddTormentingBlastDecoration(replay, target, lifespan, leftPoint, secondQuarterAoe, secondQuarterHit); // Left of frontal
                            }
                            break;
                        // Caustic Grasp - AoE Pull
                        case CausticGrasp:
                            castDuration = 1500;
                            growing = cast.Time + castDuration;
                            lifespan = (cast.Time, ComputeEndCastTimeByBuffApplication(log, target, Stun, cast.Time, castDuration));
                            replay.Decorations.AddWithGrowing(new CircleDecoration(1300, lifespan, Colors.LightOrange, 0.2, new AgentConnector(target)), growing);
                            break;
                        // Upswing
                        case UpswingEnsolyss:
                            castDuration = 1333;
                            growing = cast.Time + castDuration;
                            lifespan = (cast.Time, ComputeEndCastTimeByBuffApplication(log, target, Stun, cast.Time, castDuration));
                            (long start, long end) lifespanShockwave = (lifespan.end, cast.Time + 3100);
                            GeographicalConnector connector = new AgentConnector(target);
                            replay.Decorations.AddWithGrowing(new CircleDecoration(600, lifespan, Colors.LightOrange, 0.2, connector), growing);
                            // Shockwave
                            replay.Decorations.AddShockwave(connector, lifespanShockwave, Colors.Yellow, 0.4, 1500);
                            break;
                        // Caustic Explosion - 66% & 33% Breakbars
                        case CausticExplosionEnsolyss:
                            castDuration = 15000;
                            growing = cast.Time + castDuration;
                            int durationQuarter = 3000;
                            lifespan = (cast.Time, ComputeEndCastTimeByBuffApplication(log, target, Stun, cast.Time, castDuration));

                            // Circle going in
                            replay.Decorations.AddWithGrowing(new DoughnutDecoration(0, 2000, lifespan, Colors.Red, 0.2, new AgentConnector(target)), growing, true);

                            if (lifespan.end == growing)
                            {
                                // Explosion
                                replay.Decorations.Add(new CircleDecoration(2000, (lifespan.end, lifespan.end + 300), Colors.Red, 0.4, new AgentConnector(target)));
                            }

                            // Initial facing point
                            if (target.TryGetCurrentFacingDirection(log, cast.Time, out var facingCausticExplosion, castDuration))
                            {
                                // Calculated other quarters from initial point
                                var frontalPoint = new Vector3(facingCausticExplosion.X, facingCausticExplosion.Y, 0);
                                var leftPoint = new Vector3(facingCausticExplosion.Y * -1, facingCausticExplosion.X, 0);
                                int initialDelay = 1500;

                                // First quarters
                                (long start, long end) lifespanFirst = (cast.Time + initialDelay, Math.Min(cast.Time + initialDelay + durationQuarter, lifespan.end));
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
                            break;
                        // Lunge - Dash
                        case LungeEnsolyss:
                            castDuration = 1000;
                            if (target.TryGetCurrentFacingDirection(log, cast.Time + castDuration, out var facingLunge))
                            {
                                var rotation = new AngleConnector(facingLunge);
                                lifespan = (cast.Time, cast.Time + castDuration);
                                replay.Decorations.Add(new RectangleDecoration(1700, target.HitboxWidth, lifespan, Colors.LightOrange, 0.2, new AgentConnector(target).WithOffset(new Vector3(850, 0, 0), true)).UsingRotationConnector(rotation));
                            }
                            break;
                        // Rampage - 8 Arrows attack
                        case RampageEnsolyss:
                            // Cast duration is 4050 but visually fits better 4450
                            castDuration = 4050;
                            int visualDuration = 4450;
                            int warningDuration = 1800;
                            lifespan = (cast.Time, cast.Time + visualDuration);
                            (long start, long end) lifespanWarning = (cast.Time, cast.Time + warningDuration);
                            (long start, long end) lifespanShockwave2 = (cast.Time, cast.Time + castDuration);
                            // Red outline
                            var outline = (CircleDecoration)new CircleDecoration(380, lifespan, Colors.Red, 0.4, new AgentConnector(target)).UsingFilled(false);
                            replay.Decorations.Add(outline);
                            // Orange warning circle
                            var warning = new CircleDecoration(380, lifespanWarning, Colors.LightOrange, 0.2, new AgentConnector(target));
                            replay.Decorations.AddWithGrowing(warning, lifespanWarning.end);
                            // Growing inwards shockwave
                            var shockwave = (CircleDecoration)new CircleDecoration(1200, lifespanShockwave2, Colors.Yellow, 0.4, new AgentConnector(target)).UsingFilled(false).UsingGrowingEnd(lifespanShockwave2.end, true);
                            replay.Decorations.Add(shockwave);
                            // 8 Arrows
                            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.EnsolyssArrow, out var arrows))
                            {
                                foreach (EffectEvent effect in arrows.Where(x => x.Time >= cast.Time && x.Time < cast.Time + visualDuration))
                                {
                                    if (effect is EffectEventCBTS51)
                                    {
                                        (long start, long end) lifespanArrow = (effect.Time, effect.Time + effect.Duration);
                                        var rotation = new AngleConnector(effect.Rotation.Z);
                                        var arrow = (RectangleDecoration)new RectangleDecoration(30, 600, lifespanArrow, Colors.LightOrange, 0.2, new PositionConnector(effect.Position).WithOffset(new(0, 300, 0), true)).UsingRotationConnector(rotation);
                                        replay.Decorations.Add(arrow);
                                    }
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
                break;
            case (int)TargetID.NightmareHallucination1:
                foreach (CastEvent cast in target.GetAnimatedCastEvents(log))
                {
                    switch (cast.SkillID)
                    {
                        // Lunge - Dash
                        case LungeNightmareHallucination:
                            castDuration = 1000;
                            if (target.TryGetCurrentFacingDirection(log, cast.Time + castDuration, out var facingLunge))
                            {
                                var rotation = new AngleConnector(facingLunge);
                                lifespan = (cast.Time, cast.Time + castDuration);
                                replay.Decorations.Add(new RectangleDecoration(1700, target.HitboxWidth, lifespan, Colors.LightOrange, 0.2, new AgentConnector(target).WithOffset(new(850, 0, 0), true)).UsingRotationConnector(rotation));
                            }
                            break;
                        // Upswing
                        case UpswingHallucination:
                            castDuration = 1333;
                            lifespan = (cast.Time, cast.Time + castDuration);
                            replay.Decorations.AddWithGrowing(new CircleDecoration(300, lifespan, Colors.LightOrange, 0.1, new AgentConnector(target)), lifespan.end);
                            break;
                        default:
                            break;
                    }
                }
                break;
            case (int)TargetID.NightmareHallucination2:
                break;
            default:
                break;
        }
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IsInstance)
        {
            base.ComputePlayerCombatReplayActors(p, log, replay);
        }
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        if (!log.LogData.IsInstance)
        {
            base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);
        }

        (long start, long end) lifespan;

        // Nightmare Altar Orb AoE 1
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.EnsolyssNightmareAltarOrangeAoE, out var indicators1))
        {
            foreach (EffectEvent effect in indicators1)
            {
                lifespan = (effect.Time, effect.Time + effect.Duration);
                environmentDecorations.Add(new CircleDecoration(120, lifespan, Colors.Orange, 0.3, new PositionConnector(effect.Position)));
            }
        }

        // Nightmare Altar Orb AoE 2
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.EnsolyssNightmareAltarLightOrangeAoE, out var indicators2))
        {
            foreach (EffectEvent effect in indicators2)
            {
                lifespan = (effect.Time, effect.Time + effect.Duration);
                environmentDecorations.Add(new CircleDecoration(180, lifespan, Colors.LightOrange, 0.2, new PositionConnector(effect.Position)));
            }
        }

        // Altar Shockwave
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.EnsolyssNightmareAltarShockwave, out var waveEffects))
        {
            foreach (EffectEvent effect in waveEffects)
            {
                lifespan = (effect.Time, effect.Time + 2000);
                environmentDecorations.Add(new CircleDecoration(1200, lifespan, Colors.Yellow, 0.4, new PositionConnector(effect.Position)).UsingFilled(false).UsingGrowingEnd(lifespan.end));
            }
        }

        // Nightmare Blast
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.EnsolyssNightmareBlastIndicator, out var nightmareBlastIndicators))
        {
            foreach (EffectEvent effect in nightmareBlastIndicators)
            {
                lifespan = (effect.Time, effect.Time + effect.Duration);
                environmentDecorations.Add(new CircleDecoration(185, lifespan, Colors.LightOrange, 0.2, new PositionConnector(effect.Position)));
            }
        }

        // Tail Lash - Circle AoEs
        if (log.CombatData.TryGetEffectEventsByGUIDs(
            [EffectGUIDs.EnsolyssTailLashSmallCircleAoE,
            EffectGUIDs.EnsolyssTailLashMediumCircleAoE,
            EffectGUIDs.EnsolyssTailLashBigCircleAoE], out var tailLashCircles))
        {
            foreach (EffectEvent effect in tailLashCircles)
            {
                lifespan = (effect.Time, effect.Time + effect.Duration);
                uint radius = 0;

                switch (effect.GUIDEvent.ContentGUID)
                {
                    case var small when small == EffectGUIDs.EnsolyssTailLashSmallCircleAoE:
                        lifespan = effect.ComputeLifespan(log, 1850);
                        radius = 100;
                        break;
                    case var medium when medium == EffectGUIDs.EnsolyssTailLashMediumCircleAoE:
                        lifespan = effect.ComputeLifespan(log, 800);
                        radius = 200;
                        break;
                    case var big when big == EffectGUIDs.EnsolyssTailLashBigCircleAoE:
                        lifespan = effect.ComputeLifespan(log, 800);
                        radius = 400;
                        break;
                    default:
                        break;
                }

                environmentDecorations.Add(new CircleDecoration(radius, lifespan, Colors.LightOrange, 0.2, new PositionConnector(effect.Position)));
            }
        }

        // Caustic Barrage
        AddDistanceCorrectedOrbAoEDecorations(log, environmentDecorations, EffectGUIDs.CausticBarrageIndicator, TargetID.Ensolyss, 210, 1000, 1300);

        // Nightmare Respite
        long[] idsHealingOrbs =
            [
                EnsolyssNightmareRespite1,
                EnsolyssNightmareRespite2,
                EnsolyssNightmareRespite3,
                EnsolyssNightmareRespite4,
                EnsolyssNightmareRespite5,
                EnsolyssNightmareRespite6,
                EnsolyssNightmareRespite7,
            ];
        var healingOrbs = log.CombatData.GetMissileEventsBySkillIDs(idsHealingOrbs);
        environmentDecorations.AddNonHomingMissiles(log, healingOrbs, Colors.Blue, 0.5, 10);

        // Nightmare Bomb, Nightmare Bullet & Nightmare Blast
        long[] idsRedOrbs =
            [
                EnsolyssNightmareBomb1,
                EnsolyssNightmareBomb2,
                EnsolyssNightmareBomb3,
                EnsolyssNightmareBomb4,
                EnsolyssNightmareBomb5,
                EnsolyssNightmareBomb6,
                EnsolyssNightmareBomb7,
                EnsolyssNightmareBomb8,
                EnsolyssNightmareBomb9,
                EnsolyssNightmareBullet1,
                EnsolyssNightmareBullet2,
                EnsolyssNightmareBullet3,
                EnsolyssNightmareBullet4,
                EnsolyssNightmareBullet7,
                EnsolyssNightmareBullet8,
                EnsolyssNightmareBullet9,
                EnsolyssNightmareBullet12,
                EnsolyssNightmareBullet13,
                EnsolyssNightmareBullet14,
                EnsolyssNightmareBullet15,
                NightmareBlast,
            ];
        var redOrbs = log.CombatData.GetMissileEventsBySkillIDs(idsRedOrbs);
        environmentDecorations.AddNonHomingMissiles(log, redOrbs, Colors.Red, 0.5, 20);
    }

    internal override void SetInstanceBuffs(ParsedEvtcLog log, List<InstanceBuff> instanceBuffs)
    {
        if (!log.LogData.IsInstance)
        {
            base.SetInstanceBuffs(log, instanceBuffs);
        }
    }
}
