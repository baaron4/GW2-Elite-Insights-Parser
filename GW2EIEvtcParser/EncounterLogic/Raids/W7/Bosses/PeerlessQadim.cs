using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class PeerlessQadim : TheKeyOfAhdashim
{
    internal readonly MechanicGroup Mechanics = new MechanicGroup([
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(EnergizedAffliction, new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Green), "E.Aff", "Energized Affliction", "Energized Affliction", 0),
                new PlayerDstHealthDamageHitMechanic(ForceOfRetaliation, new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Black), "Pushed", "Pushed by Shockwave", "Shockwave Push", 1000)
                    .UsingChecker((de, log) => !de.To.HasBuff(log, Stability, de.Time - ServerDelayConstant)),
                new PlayerDstHealthDamageHitMechanic(BatteringBlitz, new MechanicPlotlySetting(Symbols.Bowtie,Colors.Orange), "Rush.H", "Hit by Qadim Rush", "Qadim Rush", 500),
                new PlayerDstHealthDamageHitMechanic(ForceOfHavoc, new MechanicPlotlySetting(Symbols.SquareOpen,Colors.Purple), "P.Rect", "Hit by Purple Rectangle", "Purple Rectangle", 0),
                new PlayerDstHealthDamageHitMechanic(ChaosCalled, new MechanicPlotlySetting(Symbols.CircleXOpen,Colors.Purple), "Pattern.H", "Hit by Energy on Pattern", "Pattern Energy Hit", 0),
                new EnemySrcHealthDamageHitMechanic(EclipsedBacklash, new MechanicPlotlySetting(Symbols.Circle,Colors.Orange), "Entropic.Expl", "Entropic Distortion exploded", "Eclipsed Backlash", 1000),
            ]),
            new PlayerDstHealthDamageHitMechanic(ExponentialRepercussionQadimShield, new MechanicPlotlySetting(Symbols.DiamondOpen,Colors.DarkPurple), "Dome.KB", "Pushed by Dome Shield Knockback", "Dome Knockback", 1000),
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(RainOfChaos, new MechanicPlotlySetting(Symbols.StarSquare,Colors.Purple), "Lght.H", "Hit by Expanding Lightning", "Lightning Hit", 0),
                new PlayerDstHealthDamageHitMechanic(BrandstormLightning, new MechanicPlotlySetting(Symbols.TriangleUp,Colors.Yellow), "S.Lght.H", "Hit by Small Lightning", "Small Lightning Hit", 0),
            ]),
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(ResidualImpactMagmaField, new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Orange), "Magma.F", "Hit by Magma Field", "Magma Field", 500),
                new PlayerDstHealthDamageHitMechanic(ResidualImpactSmallMagmaField, new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Orange,10), "S.Magma.F", "Hit by Small Magma Field", "Small Magma Field", 500),
            ]),
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(CausticChaosProjectile, new MechanicPlotlySetting(Symbols.TriangleRight,Colors.Red), "A.Prj.H", "Hit by Aimed Projectile", "Aimed Projectile", 0),
                new PlayerDstHealthDamageHitMechanic(CausticChaosExplosion, new MechanicPlotlySetting(Symbols.TriangleRightOpen,Colors.Red), "A.Prj.E", "Hit by Aimed Projectile Explosion", "Aimed Projectile Explosion", 0),
            ]),
            new PlayerCastStartMechanic(PlayerLiftUpQadimThePeerless, new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Orange), "Up", "Player lifted up", "Player lifted up", 0),
            new MechanicGroup([
                new PlayerCastEndMechanic(FluxDisruptorActivateCast, new MechanicPlotlySetting(Symbols.CircleOpenDot, Colors.Blue), "Flux.Act", "Flux Disruptor Activated", "Flux Disruptor Activated", 0),
                new PlayerCastEndMechanic(FluxDisruptorDeactivateCast, new MechanicPlotlySetting(Symbols.CircleOpenDot, Colors.LightBlue), "Flux.Dea", "Flux Disruptor Deactivated", "Flux Disruptor Deactivated", 0),
            ]),
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(ExponentialRepercussionPylon, new MechanicPlotlySetting(Symbols.DiamondOpen,Colors.Magenta), "P.KB", "Pushed by Pylon Knockback", "Pylon Knockback", 1000),
                new PlayerDstHealthDamageHitMechanic(PylonDebrisField, new MechanicPlotlySetting(Symbols.CircleOpenDot,Colors.Orange), "P.Magma", "Hit by Pylon Magma", "Pylon Magma", 0),
                new PlayerDstBuffApplyMechanic(CriticalMass, new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Red), "Orb caught", "Collected a Pylon Orb", "Critical Mass", 0),
                new EnemySrcHealthDamageHitMechanic(ShowerOfChaos, new MechanicPlotlySetting(Symbols.Circle,Colors.Black), "Orb.D", "Pylon Orb not caught", "Shower of Chaos", 1000),
            ]),
            new PlayerDstBuffApplyMechanic(FixatedQadimThePeerless, new MechanicPlotlySetting(Symbols.Star,Colors.Magenta), "Fixated", "Fixated", "Fixated", 0),
            new PlayerDstBuffApplyMechanic(SappingSurge, new MechanicPlotlySetting(Symbols.YDownOpen,Colors.Red), "B.Tether", "25% damage reduction", "Bad Tether", 0),
        ]);
    public PeerlessQadim(int triggerID) : base(triggerID)
    {
        MechanicList.Add(Mechanics);
        Extension = "prlqadim";
        Icon = EncounterIconPeerlessQadim;
        ChestID = ChestID.QadimThePeerlessChest;
        EncounterCategoryInformation.InSubCategoryOrder = 1;
        EncounterID |= 0x000003;
    }

    internal override IReadOnlyList<TargetID>  GetTargetsIDs()
    {
        return
        [
            TargetID.PeerlessQadim,
            TargetID.EntropicDistortion,
            TargetID.PeerlessQadimPylon,
        ];
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        return
        [
            //TargetID.PeerlessQadimAuraPylon,
            TargetID.BigKillerTornado,
            TargetID.EnergyOrb,
            //TargetID.Brandstorm,
            TargetID.GiantQadimThePeerless,
            //TargetID.DummyPeerlessQadim,
        ];
    }
    protected override HashSet<int> IgnoreForAutoNumericalRenaming()
    {
        return [
            (int)TargetID.PeerlessQadimPylon
        ];
    }

    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        return [ new DamageCastFinder(UnbrearablePower, UnbrearablePower) ];
    }

    private static readonly IReadOnlyList<(string, Vector2)> PylonLocations = 
    [
        ("(N)", new(1632.32837f, 11588.4014f)),
        ("(SW)", new(322.5202f, 9321.848f)),
        ("(SE)", new(2941.51514f, 9321.848f)),
    ];

    internal override FightData.EncounterStartStatus GetEncounterStartStatus(CombatData combatData, AgentData agentData, FightData fightData)
    {
        // Can be improved
        return base.GetEncounterStartStatus(combatData, agentData, fightData);
    }

    internal static void RenamePylons(IReadOnlyList<SingleActor> targets, List<CombatItem> combatData)
    {
        // Update pylon names with their cardinal locations.
        var nameCount = new Dictionary<string, int> { { "(N)", 1 }, { "(SW)", 1 }, { "(SE)", 1 } };
        var pylons = targets.Where(x => x.IsSpecies(TargetID.PeerlessQadimPylon)).ToList();
        foreach (SingleActor target in pylons)
        {
            string? suffix = AddNameSuffixBasedOnInitialPosition(target, combatData, PylonLocations);
            if (pylons.Count > 3 && suffix != null && nameCount.ContainsKey(suffix))
            {
                // deduplicate name
                target.OverrideName(target.Character + " " + (nameCount[suffix]++));
            }
        }
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
        RenamePylons(Targets, combatData);
    }

    protected override HashSet<TargetID> ForbidBreakbarPhasesFor()
    {
        return [
            TargetID.EntropicDistortion,
            TargetID.PeerlessQadimPylon
        ];
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.PeerlessQadim)) ?? throw new MissingKeyActorsException("Peerless Qadim not found");
        phases[0].AddTarget(mainTarget, log);
        if (!requirePhases)
        {
            return phases;
        }
        var phaseStarts = new List<long>();
        var phaseEnds = new List<long>();
        //
        var magmaDrops = log.CombatData.GetBuffData(MagmaDrop).Where(x => x is BuffApplyEvent);
        foreach (BuffEvent magmaDrop in magmaDrops)
        {
            if (phaseEnds.Count > 0)
            {
                if (Math.Abs(phaseEnds.Last() - magmaDrop.Time) > 1000)
                {
                    phaseEnds.Add(magmaDrop.Time);
                }
            }
            else
            {
                phaseEnds.Add(magmaDrop.Time);
            }
        }
        IReadOnlyList<AnimatedCastEvent> pushes = log.CombatData.GetAnimatedCastData(ForceOfRetaliationCast);
        if (pushes.Count > 0)
        {
            CastEvent? push = pushes[0];
            phaseStarts.Add(push.Time);
            foreach (long magmaDrop in phaseEnds)
            {
                push = pushes.FirstOrDefault(x => x.Time >= magmaDrop);
                if (push == null)
                {
                    break;
                }
                phaseStarts.Add(push.Time);
            }
        }
        // rush to pylon
        phaseEnds.AddRange(log.CombatData.GetAnimatedCastData(BatteringBlitz).Select(x => x.Time));
        phaseEnds.Add(log.FightData.FightEnd);
        // tp to middle after pylon destruction
        phaseStarts.AddRange(log.CombatData.GetAnimatedCastData(PeerlessQadimTPCenter).Select(x => x.EndTime));
        // There should be at least as many starts as ends, otherwise skip phases
        if (phaseEnds.Count < phaseStarts.Count)
        {
            return phases;
        }
        for (int i = 0; i < phaseStarts.Count; i++)
        {
            var phase = new PhaseData(phaseStarts[i], phaseEnds[i], "Phase " + (i + 1));
            phase.AddParentPhase(phases[0]);
            phase.AddTarget(mainTarget, log);
            phases.Add(phase);
        }
        // intermission phase never finished, add a "dummy" log end
        if (phaseEnds.Count - 1 == phaseStarts.Count)
        {
            phaseStarts.Add(log.FightData.FightEnd);
        }
        // There should be as many ends as starts, otherwise anomaly, skip intermission phases
        if (phaseEnds.Count != phaseStarts.Count)
        {
            return phases;
        }
        string[] intermissionNames = ["Magma Drop 1", "Magma Drop 2", "North Pylon", "SouthWest Pylon", "SouthEast Pylon"];
        bool skipNames = intermissionNames.Length < phaseEnds.Count - 1;
        for (int i = 0; i < phaseEnds.Count - 1; i++)
        {
            var phase = new PhaseData(phaseEnds[i], Math.Min(phaseStarts[i + 1], log.FightData.FightEnd), skipNames ? "Intermission " + (i + 1) : intermissionNames[i]);
            phase.AddParentPhase(phases[0]);
            phase.AddTarget(mainTarget, log);
            phases.Add(phase);
        }
        return phases;
    }

    protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
    {
        return new CombatReplayMap(CombatReplayPeerlessQadim,
                        (1000, 1000),
                        (-968, 7480, 4226, 12676)/*,
                        (-21504, -21504, 24576, 24576),
                        (33530, 34050, 35450, 35970)*/);
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        long castDuration;
        (long start, long end) lifespan = (replay.TimeOffsets.start, replay.TimeOffsets.end);

        switch (target.ID)
        {
            case (int)TargetID.PeerlessQadim:
                foreach (CastEvent cast in target.GetAnimatedCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd))
                {
                    switch (cast.SkillID)
                    {
                        // Big Magma Drop - Pylon Destroyed
                        case BigMagmaDrop:
                            castDuration = 3000;
                            long magmaDuration = 600000;
                            uint magmaRadius = 850;
                            lifespan = (cast.Time, cast.Time + castDuration);
                            (long start, long end) lifespanMagma = (lifespan.end, lifespan.end + magmaDuration);
                            if (target.TryGetCurrentPosition(log, lifespan.end, out var pylonPosition))
                            {
                                replay.Decorations.AddWithGrowing(new CircleDecoration(magmaRadius, lifespan, Colors.LightRed, 0.2, new PositionConnector(pylonPosition)), lifespan.end);
                                replay.Decorations.Add(new CircleDecoration(magmaRadius, lifespanMagma, Colors.Red, 0.5, new PositionConnector(pylonPosition)));
                            }
                            break;
                        // Force of Havoc
                        case ForceOfHavoc2:
                            {
                                uint roadLength = 2400;
                                uint roadWidth = 360;
                                int hitboxOffset = 200;
                                uint subdivisions = 100;
                                int rollOutTime = 3250;
                                long start = cast.Time;
                                int preCastTime = 1500;
                                int duration = 22500;
                                if (target.TryGetCurrentFacingDirection(log, start + 1000, out var facing)
                                    && target.TryGetCurrentPosition(log, start + 1000, out var position))
                                {
                                    replay.Decorations.Add(new RectangleDecoration(roadLength, roadWidth, (start, start + preCastTime), Colors.Red, 0.1, new PositionConnector(position).WithOffset(new(roadLength / 2 + 200, 0, 0), true)).UsingRotationConnector(new AngleConnector(facing)));
                                    for (uint i = 0; i < subdivisions; i++)
                                    {
                                        var translation = (int)((i + 0.5) * roadLength / subdivisions + hitboxOffset);
                                        replay.Decorations.Add(new RectangleDecoration(roadLength / subdivisions, roadWidth, (start + preCastTime + i * (rollOutTime / subdivisions), start + preCastTime + i * (rollOutTime / subdivisions) + duration), "rgba(143, 0, 179, 0.6)", new PositionConnector(position).WithOffset(new(translation, 0, 0), true)).UsingRotationConnector(new AngleConnector(facing)));
                                    }
                                }
                            }
                            break;
                        // Force of Retaliation
                        case ForceOfRetaliationCast:
                            {
                                uint radius = 650;
                                double radiusIncrement = 433.3;
                                int preCastTime = 1800;
                                int timeBetweenCascades = 200;
                                int cascades = 5;
                                long start = cast.Time + 1400;
                                if (target.TryGetCurrentPosition(log, start + 1000, out var position))
                                {
                                    replay.Decorations.AddWithGrowing(new CircleDecoration(radius, (start, start + preCastTime), Colors.Yellow, 0.2, new PositionConnector(position)), start + preCastTime);
                                    for (uint i = 0; i < cascades; i++)
                                    {
                                        replay.Decorations.Add(new DoughnutDecoration(radius + (uint)(radiusIncrement * i), radius + (uint)(radiusIncrement * (i + 1)), (start + preCastTime + timeBetweenCascades * i, start + preCastTime + timeBetweenCascades * (i + 1)), "rgba(30, 30, 30, 0.5)", new PositionConnector(position)));
                                        replay.Decorations.Add(new DoughnutDecoration(radius + (uint)(radiusIncrement * i), radius + (uint)(radiusIncrement * (i + 1)), (start + preCastTime + timeBetweenCascades * (i + 1), start + preCastTime + timeBetweenCascades * (i + 2)), "rgba(50, 20, 50, 0.25)", new PositionConnector(position)));
                                    }
                                }
                            }
                            break;
                        // Ether Strikes
                        case EtherStrikes2:
                            {
                                castDuration = 250;
                                lifespan = (cast.Time, cast.Time + castDuration);
                                if (target.TryGetCurrentFacingDirection(log, lifespan.start + 300, out var facing))
                                {
                                    replay.Decorations.AddWithBorder((PieDecoration)new PieDecoration(2600, 60, lifespan, Colors.Orange, 0.2, new AgentConnector(target)).UsingRotationConnector(new AngleConnector(facing)));
                                }
                            }
                            break;
                        // Caustic Chaos - Arrow Projectile
                        case CausticChaosProjectile:
                            {
                                double acceleration = cast.Acceleration;
                                double ratio = 1.0;
                                if (acceleration > 0)
                                {
                                    ratio = acceleration * 0.5 + 1;
                                }
                                else
                                {
                                    ratio = acceleration * 0.6 + 1;
                                }
                                uint chaosLength = 2600;
                                uint chaosWidth = 100;
                                int aimTime = (int)(cast.ExpectedDuration * ratio);
                                lifespan = (cast.Time, cast.EndTime);
                                (long start, long end) lifespanProj = (lifespan.start + aimTime, lifespan.end);
                                if (replay.Rotations.Count != 0)
                                {
                                    var connector = (AgentConnector)new AgentConnector(target).WithOffset(new(chaosLength / 2, 0, 0), true);
                                    var rotationConnector = new AgentFacingConnector(target);
                                    replay.Decorations.Add(new RectangleDecoration(chaosLength, chaosWidth, lifespan, Colors.Orange, 0.3, connector).UsingRotationConnector(rotationConnector));
                                    if (lifespan.end > lifespan.start + aimTime)
                                    {
                                        replay.Decorations.Add(new RectangleDecoration(chaosLength, chaosWidth, lifespanProj, Colors.LightGrey, 0.7, connector).UsingRotationConnector(rotationConnector));
                                    }
                                }
                            }
                            break;
                        // Exponential Repercussion
                        case ExponentialRepercussion:
                            {
                                uint radius = 650;
                                castDuration = 2600;
                                lifespan = (cast.Time, cast.Time + castDuration);
                                if (target.TryGetCurrentPosition(log, lifespan.start + 1000, out var position))
                                {
                                    replay.Decorations.AddWithGrowing(new CircleDecoration(radius, lifespan, Colors.Yellow, 0.2, new PositionConnector(position)), lifespan.end);

                                    foreach (NPC pylon in TrashMobs.Where(x => x.IsSpecies(TargetID.PeerlessQadimAuraPylon)))
                                    {
                                        replay.Decorations.AddWithGrowing(new CircleDecoration(radius, lifespan, Colors.Yellow, 0.2, new AgentConnector(pylon)), lifespan.end);
                                    }
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
                break;
            case (int)TargetID.EntropicDistortion:
                {
                    // Sapping Surge, bad red tether
                    AddTetherDecorations(log, target, replay, SappingSurge, Colors.Red, 0.4);

                    // Stun icon
                    IEnumerable<Segment> stuns = target.GetBuffStatus(log, Stun, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0);
                    replay.Decorations.AddOverheadIcons(stuns, target, BuffImages.Stun);

                    // Spawn animation
                    if (replay.PolledPositions.Count > 0)
                    {
                        uint radiusAnomaly = target.HitboxWidth / 2;
                        lifespan = (lifespan.start - 5000, lifespan.start);
                        replay.Decorations.AddWithGrowing(new CircleDecoration(radiusAnomaly, lifespan, Colors.Red, 0.3, new PositionConnector(replay.PolledPositions[0].XYZ)), lifespan.start);
                    }
                    var breakbarUpdates = target.GetBreakbarPercentUpdates(log);
                    var (breakbarNones, breakbarActives, breakbarImmunes, breakbarRecoverings) = target.GetBreakbarStatus(log);
                    foreach (var segment in breakbarActives)
                    {
                        replay.Decorations.AddActiveBreakbar(segment.TimeSpan, target, breakbarUpdates);
                    }
                }
                break;
            case (int)TargetID.BigKillerTornado:
                replay.Decorations.Add(new CircleDecoration(450, lifespan, Colors.LightOrange, 0.4, new AgentConnector(target)));
                break;
            case (int)TargetID.PeerlessQadimPylon:
                {
                    // Red tether from Qadim to the Pylon during breakbar
                    var breakbarBuffs = GetFilteredList(log.CombatData, QadimThePeerlessBreakbarTargetBuff, target, true, true);
                    replay.Decorations.AddTether(breakbarBuffs, Colors.Red, 0.4);
                    var breakbarUpdates = target.GetBreakbarPercentUpdates(log);
                    var (breakbarNones, breakbarActives, breakbarImmunes, breakbarRecoverings) = target.GetBreakbarStatus(log);
                    foreach (var segment in breakbarActives)
                    {
                        replay.Decorations.AddActiveBreakbar(segment.TimeSpan, target, breakbarUpdates);
                    }
                }
                break;
            case (int)TargetID.PeerlessQadimAuraPylon:
                break;
            case (int)TargetID.EnergyOrb:
                replay.Decorations.Add(new CircleDecoration(200, lifespan, Colors.Green, 0.3, new AgentConnector(target)));
                break;
            case (int)TargetID.GiantQadimThePeerless:
                // Trim giant Qadim, based on matching lift up event.
                var firstLiftUp = log.CombatData.GetAnimatedCastData(PlayerLiftUpQadimThePeerless).FirstOrDefault(x => x.Time >= target.FirstAware && x.Time <= target.LastAware);
                if (firstLiftUp != null)
                {
                    // Add 15s of wiggle room at the start
                    replay.Trim(firstLiftUp.Time - 15000, target.LastAware);
                }
                break;
            default:
                break;
        }

    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputePlayerCombatReplayActors(p, log, replay);
        // Fixated
        var fixated = p.GetBuffStatus(log, FixatedQadimThePeerless, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0);
        foreach (Segment seg in fixated)
        {
            replay.Decorations.AddOverheadIcon(seg, p, ParserIcons.FixationPurpleOverhead);
        }
        // Chaos Corrosion
        var chaosCorrosion = p.GetBuffStatus(log, ChaosCorrosion, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0);
        foreach (Segment seg in chaosCorrosion)
        {
            replay.Decorations.AddRotatedOverheadIcon(seg, p, BuffImages.Fractured, -40f);
        }
        // Critical Mass, debuff while carrying an orb
        var criticalMass = p.GetBuffStatus(log, CriticalMass, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0);
        foreach (Segment seg in criticalMass)
        {
            replay.Decorations.AddRotatedOverheadIcon(seg, p, BuffImages.OrbOfAscension, 40f);
        }
        // Magma drop
        var magmaDrop = p.GetBuffStatus(log, MagmaDrop, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0);
        uint magmaRadius = 420;
        int magmaOffset = 4000;
        int magmaDuration = 600000;
       
        int magmaCounter = 0;
        foreach (Segment seg in magmaDrop)
        {
            // If a player has gone into downstate while in air, the magma field will not spawn
            var hasPlayerDownedInAir = log.CombatData.GetDownEvents(p.AgentItem).Any(x => x.Time >= seg.Start && x.Time <= seg.End);
            long magmaDropEnd = seg.End;
            replay.Decorations.AddWithGrowing(new CircleDecoration(magmaRadius, seg, Colors.Red, 0.15, new AgentConnector(p)), magmaDropEnd);
            if (!log.CombatData.HasEffectData)
            {
                if (p.TryGetCurrentInterpolatedPosition(log, magmaDropEnd, out var position) && !hasPlayerDownedInAir)
                {
                    Color magmaColor = magmaCounter == 0 ? Colors.Yellow : Colors.LightOrange;
                    magmaCounter = (magmaCounter + 1) & 1;

                    long magmaWarningEnd = magmaDropEnd + magmaOffset;
                    replay.Decorations.AddWithGrowing(new CircleDecoration(magmaRadius, (magmaDropEnd, magmaWarningEnd), magmaColor, 0.2, new PositionConnector(position)), magmaDropEnd + magmaOffset);
                    replay.Decorations.Add(new CircleDecoration(magmaRadius, (magmaWarningEnd, magmaWarningEnd + magmaDuration), magmaColor, 0.5, new PositionConnector(position)));
                }
            }
        }

        // Sapping Surge, bad red tether
        AddTetherDecorations(log, p, replay, SappingSurge, Colors.Red, 0.4);
        // Kinetic Abundance, good blue tether
        AddTetherDecorations(log, p, replay, KineticAbundance, Colors.Green, 0.4);

        // Add custom arrow overhead for the player lifted up
        var castsUnleash = p.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.SkillID == UnleashSAK);
        var deadEvents = log.CombatData.GetDeadEvents(p.AgentItem);

        var castsLiftUp = p.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.SkillID == PlayerLiftUpQadimThePeerless);
        foreach (CastEvent cast in castsLiftUp)
        {
            long liftUpEnd = log.FightData.LogEnd;
            var unleashCast = castsUnleash.FirstOrDefault(x => x.Time > cast.Time);
            if (unleashCast != null)
            {
                liftUpEnd = Math.Min(liftUpEnd, unleashCast.Time);
            }
            var deadEvent = deadEvents.FirstOrDefault(x => x.Time > cast.Time);
            if (deadEvent != null)
            {
                liftUpEnd = Math.Min(liftUpEnd, deadEvent.Time);
            }
            replay.Decorations.AddOverheadIcon(new Segment(cast.Time, liftUpEnd, 1), p, ParserIcons.GenericBlueArrowUp);
        }
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);

        // Rain of Chaos
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.QadimPeerlessRainOfChaos, out var rainOfChaos))
        {
            foreach (EffectEvent effect in rainOfChaos)
            {
                // By skill definition, baseRadius = 210 with a scaleMultiplier of 2.0
                uint radius = 210;
                // Check if and count how many effects are present before the current one.
                // Each effect spawns every 3200ms - using 7000ms as a treshold to find a maximum of two.
                int previousAoeCounter = rainOfChaos.Count(x => x.Time < effect.Time && Math.Abs(x.Time - effect.Time) < 7000);
                if (previousAoeCounter == 1) { radius *= 2; } // 420
                if (previousAoeCounter == 2) { radius = 680; } // In game it's not 840 as it's meant to be

                (long, long) lifespan = effect.ComputeLifespan(log, 3000);
                var circle = new CircleDecoration(radius, lifespan, Colors.Orange, 0.2, new PositionConnector(effect.Position));
                environmentDecorations.Add(circle);
                environmentDecorations.Add(circle.Copy().UsingGrowingEnd(lifespan.Item2));
            }
        }

        // Residual Impact Fires
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.QadimPeerlessResidualImpactFireAoE, out var residualImpact))
        {
            foreach (EffectEvent effect in residualImpact)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 600000);
                var circle = new CircleDecoration(75, lifespan, Colors.Red, 0.1, new PositionConnector(effect.Position));
                environmentDecorations.Add(circle);
                environmentDecorations.Add(circle.Copy().GetBorderDecoration(Colors.Red, 0.4));
            }
        }

        // Chaos Called (Electric Shark)
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.QadimPeerlessChaosCalledElectricShark, out var chaosCalled))
        {
            foreach (EffectEvent effect in chaosCalled)
            {
                // Each shark effect appears every 120 ms
                // The logged duration is 0, we set it at 120 to give it the impression of a single effect moving around
                (long, long) lifespan = effect.ComputeLifespan(log, 120);
                environmentDecorations.Add(new CircleDecoration(50, lifespan, Colors.BlueishGrey, 0.4, new PositionConnector(effect.Position)));
            }
        }

        // Ether Strikes - AoEs
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.QadimPeerlessEtherStrikesAoEs, out var etherStrikes))
        {
            foreach (EffectEvent effect in etherStrikes)
            {
                // The actual effect duration is 0
                (long, long) lifespan = effect.ComputeLifespan(log, 1500);
                environmentDecorations.Add(new CircleDecoration(150, lifespan, Colors.BlueishGrey, 0.1, new PositionConnector(effect.Position)));
            }
        }

        // Caught orb
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.QadimPeerlessShowerOfChaosAoE, out var whiteOrbAoEs))
        {
            // Missed orb - Explosion
            log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.QadimPeerlessShowerOfChaosExplosion, out var landedOrbExplosions);
            foreach (EffectEvent effect in whiteOrbAoEs)
            {
                bool failedOrb = false;
                Color color = Colors.LightGrey;
                (long, long) lifespan = effect.ComputeLifespan(log, 5000);
                if (landedOrbExplosions != null)
                {
                    failedOrb = landedOrbExplosions.Any(x => (effect.Position - x.Position).XY().Length() < 1e-6);
                }
                if (failedOrb)
                {
                    color = Colors.DarkRed;
                }
                // Main circle
                var circle = new CircleDecoration(190, lifespan, color, 0.3, new PositionConnector(effect.Position));
                environmentDecorations.Add(circle);
                environmentDecorations.Add(circle.Copy().GetBorderDecoration(Colors.White, 0.5));
                environmentDecorations.Add(circle.Copy().UsingGrowingEnd(lifespan.Item2, true));
            }
        }

        // Meteor Illusion - 40/30/20 % CM Orbs
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.QadimPeerlessMeteorIllusion2, out var meteorIllusionOrbs))
        {
            foreach (EffectEvent effect in meteorIllusionOrbs)
            {
                // The actual effect duration is 4294967295
                (long, long) lifespan = effect.ComputeDynamicLifespan(log, 0);
                var connector = new PositionConnector(effect.Position);
                environmentDecorations.Add(new CircleDecoration(60, lifespan, Colors.White, 0.5, connector).UsingFilled(false));
                environmentDecorations.Add(new CircleDecoration(30, lifespan, Colors.Yellow, 0.4, connector));
            }
        }

        // Brandstorm Lightning
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.QadimPeerlessBrandstormLightning2, out var brandstormLightning))
        {
            foreach (EffectEvent effect in brandstormLightning)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 3000);
                var circle = new CircleDecoration(220, lifespan, Colors.Orange, 0.1, new PositionConnector(effect.Position));
                environmentDecorations.Add(circle);
                environmentDecorations.Add(circle.Copy().UsingGrowingEnd(lifespan.Item2));
            }
        }

        // Magma Drop warning
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.QadimPeerlessMagmaWarningAoE, out var magmaWarnings))
        {
            var dict = new Dictionary<long, List<EffectEvent>>(magmaWarnings.Count);
            long previousTime = int.MinValue;
            foreach (EffectEvent effect in magmaWarnings)
            {
                if (effect.Time - previousTime > 1000)
                {
                    previousTime = effect.Time;
                    //TODO(Rennorb) @perf
                    dict[previousTime] = [];
                }
                dict[previousTime].Add(effect);
            }

            int magmaCounter = 0;
            foreach (var effects in dict.Values)
            {
                // Yellow for first, orange for second
                Color magmaColor = magmaCounter == 0 ? Colors.Yellow : Colors.LightOrange;
                magmaCounter = (magmaCounter + 1) & 1;
                foreach (EffectEvent effect in effects)
                {
                    var connector = new PositionConnector(effect.Position);
                    (long, long) lifespan = effect.ComputeLifespan(log, 4000);
                    var circle = new CircleDecoration(420, lifespan, magmaColor, 0.2, connector);
                    environmentDecorations.Add(circle);
                    environmentDecorations.Add(circle.Copy().UsingGrowingEnd(lifespan.Item2));
                }
            }
        }

        // Magma Drop Activated
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.QadimPeerlessMagmaDamagingAoE, out var magmas))
        {
            var dict = new Dictionary<long, List<EffectEvent>>();
            long previousTime = int.MinValue;
            foreach (EffectEvent effect in magmas)
            {
                if (effect.Time - previousTime > 1000)
                {
                    previousTime = effect.Time;
                    dict[previousTime] = [];
                }
                dict[previousTime].Add(effect);
            }

            int magmaCounter = 0;
            foreach (KeyValuePair<long, List<EffectEvent>> pair in dict)
            {
                // Yellow for first, orange for second
                Color magmaColor = magmaCounter == 0 ? Colors.Yellow : Colors.LightOrange;
                magmaCounter = (magmaCounter + 1) & 1;
                foreach (EffectEvent effect in pair.Value)
                {
                    var connector = new PositionConnector(effect.Position);
                    (long, long) lifespan = effect.ComputeLifespan(log, 600000);
                    var circle = new CircleDecoration(420, lifespan, magmaColor, 0.5, connector);
                    environmentDecorations.Add(circle);
                }
            }
        }
    }

    private static void AddTetherDecorations(ParsedEvtcLog log, SingleActor actor, CombatReplay replay, long buffID, Color color, double opacity)
    {
        var tethers = log.CombatData.GetBuffDataByIDByDst(buffID, actor.AgentItem).Where(x => x is not BuffRemoveManualEvent);
        var tethersRemoves = new HashSet<AbstractBuffRemoveEvent>(tethers.OfType<AbstractBuffRemoveEvent>());
        foreach (var appliedTether in tethers.OfType<BuffApplyEvent>())
        {
            var src = log.FindActor(appliedTether.By);
            if (src != null)
            {
                int tetherStart = (int)appliedTether.Time;
                var abre = tethersRemoves.FirstOrDefault(x => x.Time >= tetherStart);
                int tetherEnd;
                if (abre != null)
                {
                    tetherEnd = (int)abre.Time;
                    tethersRemoves.Remove(abre);
                }
                else
                {
                    tetherEnd = (int)log.FightData.FightEnd;
                }
                replay.Decorations.Add(new LineDecoration((tetherStart, tetherEnd), color, opacity, new AgentConnector(actor), new AgentConnector(src)));
            }
        }
    }

    internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
    {
        SingleActor target = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.PeerlessQadim)) ?? throw new MissingKeyActorsException("Peerless Qadim not found");
        return (target.GetHealth(combatData) > 48e6) ? FightData.EncounterMode.CM : FightData.EncounterMode.Normal;
    }

}
