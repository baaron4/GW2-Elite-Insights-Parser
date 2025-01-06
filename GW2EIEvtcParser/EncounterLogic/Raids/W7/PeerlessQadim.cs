using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class PeerlessQadim : TheKeyOfAhdashim
{
    public PeerlessQadim(int triggerID) : base(triggerID)
    {
        MechanicList.AddRange(new List<Mechanic>()
        {
            new PlayerDstHitMechanic(PylonDebrisField, "Pylon Debris Field", new MechanicPlotlySetting(Symbols.CircleOpenDot,Colors.Orange), "P.Magma", "Hit by Pylon Magma", "Pylon Magma", 0),
            new PlayerDstHitMechanic(EnergizedAffliction, "Energized Affliction", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Green), "E.Aff", "Energized Affliction", "Energized Affliction", 0),
            new PlayerDstHitMechanic(ForceOfRetaliation, "Force of Retaliation", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Black), "Pushed", "Pushed by Shockwave", "Shockwave Push", 1000).UsingChecker((de, log) => !de.To.HasBuff(log, Stability, de.Time - ServerDelayConstant)),
            new PlayerDstHitMechanic(ExponentialRepercussionPylon, "Exponential Repercussion", new MechanicPlotlySetting(Symbols.DiamondOpen,Colors.Magenta), "P.KB", "Pushed by Pylon Knockback", "Pylon Knockback", 1000),
            new PlayerDstHitMechanic(ExponentialRepercussionQadimShield, "Exponential Repercussion", new MechanicPlotlySetting(Symbols.DiamondOpen,Colors.DarkPurple), "Dome.KB", "Pushed by Dome Shield Knockback", "Dome Knockback", 1000),
            new PlayerDstHitMechanic(ForceOfHavoc, "Force of Havoc", new MechanicPlotlySetting(Symbols.SquareOpen,Colors.Purple), "P.Rect", "Hit by Purple Rectangle", "Purple Rectangle", 0),
            new PlayerDstHitMechanic(ChaosCalled, "Chaos Called", new MechanicPlotlySetting(Symbols.CircleXOpen,Colors.Purple), "Pattern.H", "Hit by Energy on Pattern", "Pattern Energy Hit", 0),
            new PlayerDstHitMechanic(RainOfChaos, "Rain of Chaos", new MechanicPlotlySetting(Symbols.StarSquare,Colors.Purple), "Lght.H", "Hit by Expanding Lightning", "Lightning Hit", 0),
            new PlayerDstHitMechanic(BrandstormLightning, "Brandstorm Lightning", new MechanicPlotlySetting(Symbols.TriangleUp,Colors.Yellow), "S.Lght.H", "Hit by Small Lightning", "Small Lightning Hit", 0),
            new PlayerDstHitMechanic(ResidualImpactMagmaField, "Residual Impact", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Orange), "Magma.F", "Hit by Magma Field", "Magma Field", 500),
            new PlayerDstHitMechanic(ResidualImpactSmallMagmaField, "Residual Impact", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Orange,10), "S.Magma.F", "Hit by Small Magma Field", "Small Magma Field", 500),
            new PlayerDstHitMechanic(BatteringBlitz, "Battering Blitz", new MechanicPlotlySetting(Symbols.Bowtie,Colors.Orange), "Rush.H", "Hit by Qadim Rush", "Qadim Rush", 500),
            new PlayerDstHitMechanic(CausticChaosProjectile, "Caustic Chaos", new MechanicPlotlySetting(Symbols.TriangleRight,Colors.Red), "A.Prj.H", "Hit by Aimed Projectile", "Aimed Projectile", 0),
            new EnemySrcHitMechanic(ShowerOfChaos, "Shower of Chaos", new MechanicPlotlySetting(Symbols.Circle,Colors.Black), "Orb.D", "Pylon Orb not caught", "Shower of Chaos", 1000),
            new EnemySrcHitMechanic(EclipsedBacklash, "Eclipsed Backlash", new MechanicPlotlySetting(Symbols.Circle,Colors.Orange), "Entropic.Expl", "Entropic Distortion exploded", "Eclipsed Backlash", 1000),
            new PlayerCastStartMechanic(PlayerLiftUpQadimThePeerless, "Lift Up", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Orange), "Up", "Player lifted up", "Player lifted up", 0),
            new PlayerCastEndMechanic(FluxDisruptorActivateCast, "Flux Disruptor: Activate", new MechanicPlotlySetting(Symbols.CircleOpenDot, Colors.Blue), "Flux.Act", "Flux Disruptor Activated", "Flux Disruptor Activated", 0),
            new PlayerCastEndMechanic(FluxDisruptorDeactivateCast, "Flux Disruptor: Deactivate", new MechanicPlotlySetting(Symbols.CircleOpenDot, Colors.LightBlue), "Flux.Dea", "Flux Disruptor Deactivated", "Flux Disruptor Deactivated", 0),
            new PlayerDstBuffApplyMechanic(FixatedQadimThePeerless, "Fixated", new MechanicPlotlySetting(Symbols.Star,Colors.Magenta), "Fixated", "Fixated", "Fixated", 0),
            new PlayerDstBuffApplyMechanic(CriticalMass, "Critical Mass", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Red), "Orb caught", "Collected a Pylon Orb", "Critical Mass", 0),
            new PlayerDstBuffApplyMechanic(SappingSurge, "Sapping Surge", new MechanicPlotlySetting(Symbols.YDownOpen,Colors.Red), "B.Tether", "25% damage reduction", "Bad Tether", 0),
            new PlayerDstHitMechanic(CausticChaosExplosion, "Caustic Chaos", new MechanicPlotlySetting(Symbols.TriangleRightOpen,Colors.Red), "A.Prj.E", "Hit by Aimed Projectile Explosion", "Aimed Projectile Explosion", 0),
        });
        Extension = "prlqadim";
        Icon = EncounterIconPeerlessQadim;
        EncounterCategoryInformation.InSubCategoryOrder = 1;
        EncounterID |= 0x000003;
    }

    protected override ReadOnlySpan<int> GetTargetsIDs()
    {
        return
        [
            (int)ArcDPSEnums.TargetID.PeerlessQadim,
            (int)ArcDPSEnums.TrashID.EntropicDistortion,
            (int)ArcDPSEnums.TrashID.PeerlessQadimPylon,
        ];
    }

    protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
    {
        return
        [
            //ArcDPSEnums.TrashID.PeerlessQadimAuraPylon,
            ArcDPSEnums.TrashID.BigKillerTornado,
            ArcDPSEnums.TrashID.EnergyOrb,
            //ArcDPSEnums.TrashID.Brandstorm,
            ArcDPSEnums.TrashID.GiantQadimThePeerless,
            //ArcDPSEnums.TrashID.DummyPeerlessQadim,
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

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);

        // Update pylon names with their cardinal locations.
        foreach (NPC target in Targets.Where(x => x.IsSpecies(ArcDPSEnums.TrashID.PeerlessQadimPylon)).Cast<NPC>())
        {
            AddNameSuffixBasedOnInitialPosition(target, combatData, PylonLocations);
        }
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.PeerlessQadim)) ?? throw new MissingKeyActorsException("Peerless Qadim not found");
        phases[0].AddTarget(mainTarget);
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
            phase.AddTarget(mainTarget);
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
            phase.AddTarget(mainTarget);
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
        int start = (int)replay.TimeOffsets.start;
        int end = (int)replay.TimeOffsets.end;
        switch (target.ID)
        {
            case (int)ArcDPSEnums.TargetID.PeerlessQadim:
                var cls = target.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd);
                var cataCycle = cls.Where(x => x.SkillId == BigMagmaDrop);
                foreach (CastEvent c in cataCycle)
                {
                    uint magmaRadius = 850;
                    start = (int)c.Time;
                    end = (int)c.EndTime;
                    if (target.TryGetCurrentPosition(log, end, out var pylonPosition))
                    {
                        continue;
                    }
                    replay.Decorations.AddWithGrowing(new CircleDecoration(magmaRadius, (start, end), Colors.LightRed, 0.2, new PositionConnector(pylonPosition)), end);
                    replay.Decorations.Add(new CircleDecoration(magmaRadius, (end, log.FightData.FightEnd), Colors.Red, 0.5, new PositionConnector(pylonPosition)));
                }
                var forceOfHavoc = cls.Where(x => x.SkillId == ForceOfHavoc2);
                foreach (CastEvent c in forceOfHavoc)
                {
                    uint roadLength = 2400;
                    uint roadWidth = 360;
                    int hitboxOffset = 200;
                    uint subdivisions = 100;
                    int rollOutTime = 3250;
                    start = (int)c.Time;
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
                var forceOfRetal = cls.Where(x => x.SkillId == ForceOfRetaliationCast);
                foreach (CastEvent c in forceOfRetal)
                {
                    uint radius = 650;
                    double radiusIncrement = 433.3;
                    int preCastTime = 1800;
                    int timeBetweenCascades = 200;
                    int cascades = 5;
                    start = (int)c.Time + 1400;
                    if (target.TryGetCurrentPosition(log, start + 1000, out var position))
                    {
                        replay.Decorations.AddWithGrowing(new CircleDecoration(radius, (start, start + preCastTime), "rgba(255, 220, 50, 0.2)", new PositionConnector(position)), start + preCastTime);
                        for (uint i = 0; i < cascades; i++)
                        {
                            replay.Decorations.Add(new DoughnutDecoration(radius + (uint)(radiusIncrement * i), radius + (uint)(radiusIncrement * (i + 1)), (start + preCastTime + timeBetweenCascades * i, start + preCastTime + timeBetweenCascades * (i + 1)), "rgba(30, 30, 30, 0.5)", new PositionConnector(position)));
                            replay.Decorations.Add(new DoughnutDecoration(radius + (uint)(radiusIncrement * i), radius + (uint)(radiusIncrement * (i + 1)), (start + preCastTime + timeBetweenCascades * (i + 1), start + preCastTime + timeBetweenCascades * (i + 2)), "rgba(50, 20, 50, 0.25)", new PositionConnector(position)));
                        }
                    }
                }
                var etherStrikes = cls.Where(x => x.SkillId == EtherStrikes1 || x.SkillId == EtherStrikes2);
                foreach (CastEvent c in etherStrikes)
                {
                    uint coneRadius = 2600;
                    int coneAngle = 60;
                    start = (int)c.Time;
                    end = start + 250;
                    if (target.TryGetCurrentFacingDirection(log, start + 300, out var facing))
                    {
                        var connector = new AgentConnector(target);
                        var rotationConnector = new AngleConnector(facing);
                        replay.Decorations.AddWithBorder((PieDecoration)new PieDecoration(coneRadius, coneAngle, (start, end), Colors.Orange, 0.2, connector).UsingRotationConnector(rotationConnector));
                    }
                }
                var causticChaos = cls.Where(x => x.SkillId == CausticChaosProjectile);
                foreach (CastEvent c in causticChaos)
                {
                    double acceleration = c.Acceleration;
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
                    start = (int)c.Time;
                    end = (int)c.EndTime;
                    int aimTime = (int)(c.ExpectedDuration * ratio);
                    if (replay.Rotations.Count != 0)
                    {
                        var connector = (AgentConnector)new AgentConnector(target).WithOffset(new(chaosLength / 2, 0, 0), true);
                        var rotationConnector = new AgentFacingConnector(target);
                        replay.Decorations.Add(new RectangleDecoration(chaosLength, chaosWidth, (start, end), Colors.Orange, 0.3, connector).UsingRotationConnector(rotationConnector));
                        if (end > start + aimTime)
                        {
                            replay.Decorations.Add(new RectangleDecoration(chaosLength, chaosWidth, (start + aimTime, end), Colors.LightGrey, 0.7, connector).UsingRotationConnector(rotationConnector));
                        }
                    }
                }
                var expoReperc = cls.Where(x => x.SkillId == ExponentialRepercussion);
                foreach (CastEvent c in expoReperc)
                {
                    uint radius = 650;
                    start = (int)c.Time;
                    end = (int)c.EndTime;
                    if (target.TryGetCurrentPosition(log, start + 1000, out var position))
                    {
                        replay.Decorations.AddWithGrowing(new CircleDecoration(radius, (start, end), Colors.Yellow, 0.2, new PositionConnector(position)), end);

                        foreach (NPC pylon in TrashMobs.Where(x => x.IsSpecies(ArcDPSEnums.TrashID.PeerlessQadimAuraPylon)))
                        {
                            replay.Decorations.AddWithGrowing(new CircleDecoration(radius, (start, end), Colors.Yellow, 0.2, new AgentConnector(pylon)), end);
                        }
                    }
                }
                break;
            case (int)ArcDPSEnums.TrashID.EntropicDistortion:
                // Sapping Surge, bad red tether
                AddTetherDecorations(log, target, replay, SappingSurge, Colors.Red, 0.4);

                // Stun icon
                IEnumerable<Segment> stuns = target.GetBuffStatus(log, Stun, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0);
                replay.Decorations.AddOverheadIcons(stuns, target, BuffImages.Stun);

                // Spawn animation
                if (replay.PolledPositions.Count > 0)
                {
                    uint radiusAnomaly = target.HitboxWidth / 2;
                    replay.Decorations.AddWithGrowing(new CircleDecoration(radiusAnomaly, (start - 5000, start), Colors.Red, 0.3, new PositionConnector(replay.PolledPositions[0].XYZ)), start);
                }
                break;
            case (int)ArcDPSEnums.TrashID.BigKillerTornado:
                replay.Decorations.Add(new CircleDecoration(450, (start, end), Colors.LightOrange, 0.4, new AgentConnector(target)));
                break;
            case (int)ArcDPSEnums.TrashID.PeerlessQadimPylon:
                // Red tether from Qadim to the Pylon during breakbar
                var breakbarBuffs = GetFilteredList(log.CombatData, QadimThePeerlessBreakbarTargetBuff, target, true, true);
                replay.Decorations.AddTether(breakbarBuffs, Colors.Red, 0.4);
                break;
            case (int)ArcDPSEnums.TrashID.PeerlessQadimAuraPylon:
                break;
            case (int)ArcDPSEnums.TrashID.EnergyOrb:
                replay.Decorations.Add(new CircleDecoration(200, (start, end), Colors.Green, 0.3, new AgentConnector(target)));
                break;
            case (int)ArcDPSEnums.TrashID.GiantQadimThePeerless:
                // Trim the first giant Qadim, it exists since log start.
                var firstGiantQadim = log.AgentData.GetNPCsByID(ArcDPSEnums.TrashID.GiantQadimThePeerless).FirstByAware();
                var firstLiftUp = log.CombatData.GetAnimatedCastData(PlayerLiftUpQadimThePeerless).FirstByNonZeroTime();
                if (firstGiantQadim != null && firstLiftUp != null && target.AgentItem == firstGiantQadim)
                {
                    replay.Trim(firstLiftUp.Time, firstGiantQadim.LastAware);
                }
                break;
            default:
                break;
        }

    }

     const string MagmaColor1 = "rgba(255, 215,  0, ";
     const string MagmaColor2 = "rgba(255, 130, 50, ";

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
            replay.Decorations.AddWithGrowing(new CircleDecoration(magmaRadius, seg, "rgba(255, 50, 0, 0.2)", new AgentConnector(p)), magmaDropEnd);
            if (!log.CombatData.HasEffectData)
            {
                if (p.TryGetCurrentInterpolatedPosition(log, magmaDropEnd, out var position) && !hasPlayerDownedInAir)
                {
                    string magmaColor = magmaCounter == 0 ? MagmaColor1 : MagmaColor2;
                    magmaCounter = (magmaCounter + 1) & 1;

                    long magmaWarningEnd = magmaDropEnd + magmaOffset;
                    replay.Decorations.AddWithGrowing(new CircleDecoration(magmaRadius, (magmaDropEnd, magmaWarningEnd), magmaColor + "0.2)", new PositionConnector(position)), magmaDropEnd + magmaOffset);
                    replay.Decorations.Add(new CircleDecoration(magmaRadius, (magmaWarningEnd, magmaWarningEnd + magmaDuration), magmaColor + "0.5)", new PositionConnector(position)));
                }
            }
        }

        // Sapping Surge, bad red tether
        AddTetherDecorations(log, p, replay, SappingSurge, Colors.Red, 0.4);
        // Kinetic Abundance, good blue tether
        AddTetherDecorations(log, p, replay, KineticAbundance, Colors.Green, 0.4);

        // Add custom arrow overhead for the player lifted up
        var castsUnleash = p.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.SkillId == UnleashSAK);
        var deadEvents = log.CombatData.GetDeadEvents(p.AgentItem);

        var castsLiftUp = p.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.SkillId == PlayerLiftUpQadimThePeerless);
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

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log)
    {
        base.ComputeEnvironmentCombatReplayDecorations(log);

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
                EnvironmentDecorations.Add(circle);
                EnvironmentDecorations.Add(circle.Copy().UsingGrowingEnd(lifespan.Item2));
            }
        }

        // Residual Impact Fires
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.QadimPeerlessResidualImpactFireAoE, out var residualImpact))
        {
            foreach (EffectEvent effect in residualImpact)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 600000);
                var circle = new CircleDecoration(75, lifespan, "rgba(230, 40, 0, 0.1)", new PositionConnector(effect.Position));
                EnvironmentDecorations.Add(circle);
                EnvironmentDecorations.Add(circle.Copy().GetBorderDecoration(Colors.Red, 0.4));
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
                EnvironmentDecorations.Add(new CircleDecoration(50, lifespan, "rgba(108, 122, 137, 0.4)", new PositionConnector(effect.Position)));
            }
        }

        // Ether Strikes - AoEs
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.QadimPeerlessEtherStrikesAoEs, out var etherStrikes))
        {
            foreach (EffectEvent effect in etherStrikes)
            {
                // The actual effect duration is 0
                (long, long) lifespan = effect.ComputeLifespan(log, 1500);
                EnvironmentDecorations.Add(new CircleDecoration(150, lifespan, "rgba(108, 122, 137, 0.1)", new PositionConnector(effect.Position)));
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
                EnvironmentDecorations.Add(circle);
                EnvironmentDecorations.Add(circle.Copy().GetBorderDecoration(Colors.White, 0.5));
                EnvironmentDecorations.Add(circle.Copy().UsingGrowingEnd(lifespan.Item2, true));
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
                EnvironmentDecorations.Add(new CircleDecoration(60, lifespan, Colors.White, 0.5, connector).UsingFilled(false));
                EnvironmentDecorations.Add(new CircleDecoration(30, lifespan, "rgba(255, 215, 0, 0.4)", connector));
            }
        }

        // Brandstorm Lightning
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.QadimPeerlessBrandstormLightning2, out var brandstormLightning))
        {
            foreach (EffectEvent effect in brandstormLightning)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 3000);
                var circle = new CircleDecoration(220, lifespan, Colors.Orange, 0.1, new PositionConnector(effect.Position));
                EnvironmentDecorations.Add(circle);
                EnvironmentDecorations.Add(circle.Copy().UsingGrowingEnd(lifespan.Item2));
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
                string magmaColor = magmaCounter == 0 ? MagmaColor1 : MagmaColor2;
                magmaCounter = (magmaCounter + 1) & 1;
                foreach (EffectEvent effect in effects)
                {
                    var connector = new PositionConnector(effect.Position);
                    (long, long) lifespan = effect.ComputeLifespan(log, 4000);
                    var circle = new CircleDecoration(420, lifespan, magmaColor + "0.2)", connector);
                    EnvironmentDecorations.Add(circle);
                    EnvironmentDecorations.Add(circle.Copy().UsingGrowingEnd(lifespan.Item2));
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
                string magmaColor = magmaCounter == 0 ? MagmaColor1 : MagmaColor2;
                magmaCounter = (magmaCounter + 1) & 1;
                foreach (EffectEvent effect in pair.Value)
                {
                    var connector = new PositionConnector(effect.Position);
                    (long, long) lifespan = effect.ComputeLifespan(log, 600000);
                    var circle = new CircleDecoration(420, lifespan, magmaColor + "0.5)", connector);
                    EnvironmentDecorations.Add(circle);
                }
            }
        }
    }

    private static void AddTetherDecorations(ParsedEvtcLog log, SingleActor actor, CombatReplay replay, long buffId, Color color, double opacity)
    {
        var tethers = log.CombatData.GetBuffDataByIDByDst(buffId, actor.AgentItem).Where(x => x is not BuffRemoveManualEvent);
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
        SingleActor target = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.PeerlessQadim)) ?? throw new MissingKeyActorsException("Peerless Qadim not found");
        return (target.GetHealth(combatData) > 48e6) ? FightData.EncounterMode.CM : FightData.EncounterMode.Normal;
    }

}
