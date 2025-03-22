using System.Diagnostics;
using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class AetherbladeHideout : EndOfDragonsStrike
{
    public AetherbladeHideout(int triggerID) : base(triggerID)
    {
        MechanicList.AddRange(
            [
                // NOTE: Kaleidoscopic Chaos deals HP % damage - Normal Mode: 20% if hit once, 60% if hit twice - Challenge Mode: 33% if hit once, 200% if hit twice.
                new PlayerDstHitMechanic(NightmareFusilladeMain, "Nightmare Fusillade", new MechanicPlotlySetting(Symbols.TriangleRight, Colors.DarkRed), "Cone", "Hit by Cone attack", "Cone", 150),
                new PlayerDstHitMechanic(NightmareFusilladeSide, "Nightmare Fusillade Side", new MechanicPlotlySetting(Symbols.TriangleLeft, Colors.DarkRed), "Cone.S", "Hit by Side Cone attack", "Side Cone", 150),
                new PlayerDstHitMechanic(ElectricBlast, "Electric Blast", new MechanicPlotlySetting(Symbols.Circle, Colors.LightRed), "ElecBlast.H", "Hit by Electric Blast (Echo AoEs)", "Electric Blast Hit", 0),
                new PlayerDstHitMechanic(ToxicOrb, "Toxic Orb", new MechanicPlotlySetting(Symbols.CircleCross, Colors.Purple), "ToxOrb.H", "Hit by Toxic Orb", "Toxic Orb Hit", 0),
                new PlayerDstHitMechanic(Heartpiercer, "Heartpiercer", new MechanicPlotlySetting(Symbols.Octagon, Colors.White), "HrtPier.H", "Hit by Heartpiercer", "Heartpiercer Hit", 0),
                new PlayerDstHitMechanic(Heartpiercer, "Heartpiercer", new MechanicPlotlySetting(Symbols.Octagon, Colors.DarkWhite), "HrtPier.CC", "Knocked Down by Heartpiercer", "Heartpiercer Knockdown", 150).UsingChecker((evt, log) => !evt.To.HasBuff(log, Stability, evt.Time - 10)),
                new PlayerDstHitMechanic(FissureOfTorment, "Fissure of Torment", new MechanicPlotlySetting(Symbols.X, Colors.DarkRed), "FissTorm.H", "Hit by Fissure of Torment", "Fissure of Torment Hit", 0),
                new PlayerDstHitMechanic([TormentingWaveNM, TormentingWaveCM], "Tormenting Wave", new MechanicPlotlySetting(Symbols.Circle, Colors.DarkRed), "Shck.Wv", "Hit by Shockwave attack", "Shockwave", 150),
                new PlayerDstHitMechanic([LeyBreachNM, LeyBreachCM], "Ley Breach", new MechanicPlotlySetting(Symbols.Circle, Colors.LightOrange), "Puddle", "Hit by Ley Breach (Red Puddle)", "Puddle", 150),
                new PlayerDstHitMechanic([ToxicBulletNMCM1, ToxicBulletNMCM2, ToxicBulletCM], "Toxic Bullet", new MechanicPlotlySetting(Symbols.CircleOpenDot, Colors.LightPurple), "ToxBull.H", "Hit by Toxic Bullet", "Toxic Bullet Hit", 0),
                new PlayerDstSkillMechanic(FocusedDestructionNM, "Focused Destruction", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Red), "Green.Dwn", "Downed by Green", "Green Downed", 150).UsingChecker((evt, log) => evt.HasDowned),
                new PlayerDstSkillMechanic([KaleidoscopicChaosNM, KaleidoscopicChaosCM], "Kaleidoscopic Chaos", new MechanicPlotlySetting(Symbols.TriangleDown, Colors.Orange), "Spread.Dwn", "Downed by Kaleidoscopic Chaos (Spread)", "Kaleidoscopic Chaos Downed", 0).UsingChecker((evt, log) => evt.HasDowned),
                new PlayerDstSkillMechanic([TormentingWaveNM, TormentingWaveCM], "Tormenting Wave", new MechanicPlotlySetting(Symbols.BowtieOpen, Colors.Orange), "Smash", "Died to Echo's Smash", "Smash Death", 0).UsingChecker((evt, log) => evt.HasDowned && evt.To.IsPlayer && evt.To.IsDead(log, evt.Time - 5, evt.Time + 5)),
                new PlayerDstSkillMechanic(FocusedDestructionCM, "Focused Destruction", new MechanicPlotlySetting(Symbols.CircleXOpen, Colors.DarkGreen), "Green.Dth", "Died to Focused Destruction (Green)", "Green Death", 0).UsingChecker((evt, log) => evt.HasDowned && evt.To.IsPlayer && evt.To.IsDead(log, evt.Time - 5, evt.Time + 5) && evt.To.HasBuff(log, PhotonSaturation, evt.Time - 10)),
                new PlayerDstSkillMechanic(ChaosAndDestructionDamageNM, "Chaos and Destruction", new MechanicPlotlySetting(Symbols.Hourglass, Colors.Red), "Puzzle.Dth", "Died to Chaos and Destruction (Puzzle)", "Puzzle Death", 0).UsingChecker((evt, log) => evt.HasDowned && evt.To.IsPlayer && evt.To.IsDead(log, evt.Time - 30, evt.Time + 30)),
                new PlayerDstSkillMechanic(MagBeam, "Mag Beam", new MechanicPlotlySetting(Symbols.X, Colors.Red), "PuzzleCM.Dth", "Died to Mag Beam (Puzzle)", "Puzzle CM Death", 0).UsingChecker((evt, log) => evt.HasDowned && evt.To.IsPlayer && evt.To.IsDead(log, evt.Time - 5, evt.Time + 5)),
                new PlayerDstBuffApplyMechanic(PhotonSaturation, "Photon Saturation", new MechanicPlotlySetting(Symbols.TriangleDown, Colors.Green), "Green.D", "Received Photon Saturation (Green Debuff)", "Green Debuff", 150),
                new PlayerDstBuffApplyMechanic(MagneticBomb, "Magnetic Bomb", new MechanicPlotlySetting(Symbols.Circle, Colors.Magenta), "Bomb", "Selected for Bomb", "Bomb", 150),
                new PlayerDstBuffApplyMechanic(LeyBreachTargetBuff, "Ley Breach", new MechanicPlotlySetting(Symbols.DiamondOpen, Colors.CobaltBlue), "LeyBreach.T", "Targeted by Ley Breach (Red Puddle)", "Ley Breach Target", 0),
                new PlayerDstBuffApplyMechanic(MaiTrinCMBeamsTargetGreen, "Beam Target", new MechanicPlotlySetting(Symbols.DiamondWideOpen, Colors.Green), "BombGreen.A", "Received Green Bomb Target", "Green Bomb Target", 0),
                new PlayerDstBuffApplyMechanic(MaiTrinCMBeamsTargetRed, "Beam Target", new MechanicPlotlySetting(Symbols.DiamondWideOpen, Colors.Red), "BombRed.A", "Received Red Bomb Target", "Red Bomb Target", 0),
                new PlayerDstBuffApplyMechanic(MaiTrinCMBeamsTargetBlue, "Beam Target", new MechanicPlotlySetting(Symbols.DiamondWideOpen, Colors.Blue), "BombBlue.A", "Received Blue Bomb Target", "Blue Bomb Target", 0),
                new PlayerDstBuffApplyMechanic([SharedDestructionMaiTrinNM, SharedDestructionMaiTrinCM], "Shared Destruction", new MechanicPlotlySetting(Symbols.Circle, Colors.Green), "Green", "Selected for Green", "Green", 150),
            ]
        );
        Icon = EncounterIconAetherbladeHideout;
        Extension = "aetherhide";
        EncounterCategoryInformation.InSubCategoryOrder = 0;
        EncounterID |= 0x000001;
    }

    protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
    {
        return new CombatReplayMap(CombatReplayAetherbladeHideout,
                        (838, 639),
                        (1165, 540, 4194, 2850)/*,
                        (-15360, -36864, 15360, 39936),
                        (3456, 11012, 4736, 14212)*/);
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData)
    {
        return "Aetherblade Hideout";
    }

    protected override ReadOnlySpan<int> GetTargetsIDs()
    {
        return
        [
            (int)TargetID.MaiTrinStrike,
            (int)TargetID.EchoOfScarletBriarNM,
            (int)TargetID.EchoOfScarletBriarCM,
            (int)TrashID.ScarletPhantomBreakbar,
            (int)TrashID.ScarletPhantomHP,
            (int)TrashID.ScarletPhantomHPCM,
            (int)TrashID.ScarletPhantomBeamNM,
            (int)TrashID.FerrousBomb,
        ];
    }

    protected override List<int> GetSuccessCheckIDs()
    {
        return
        [
            (int)TargetID.MaiTrinStrike,
            (int)TargetID.EchoOfScarletBriarNM,
            (int)TargetID.EchoOfScarletBriarCM,
        ];
    }

    protected override List<TrashID> GetTrashMobsIDs()
    {
        return
        [
            TrashID.ScarletPhantom,
            TrashID.ScarletPhantomDeathBeamCM,
            TrashID.ScarletPhantomDeathBeamCM2,
            TrashID.MaiTrinStrikeDuringEcho,
        ];
    }

    protected override ReadOnlySpan<int> GetUniqueNPCIDs()
    {
        return
        [
            (int)TargetID.MaiTrinStrike,
            (int)TargetID.EchoOfScarletBriarNM,
            (int)TargetID.EchoOfScarletBriarCM,
        ];
    }

    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        return
        [
            new BuffLossCastFinder(ReverseThePolaritySAK, MaiTrinCMBeamsTargetGreen),
            new BuffLossCastFinder(ReverseThePolaritySAK, MaiTrinCMBeamsTargetBlue),
            new BuffLossCastFinder(ReverseThePolaritySAK, MaiTrinCMBeamsTargetRed),
        ];
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        switch (target.ID)
        {
            case (int)TargetID.MaiTrinStrike:
                var casts = target.GetCastEvents(log, log.FightData.FightStart, log.FightData.LogEnd);

                // Visually removing Mai Trin from the Combat Replay when we get the last HP update.
                HealthUpdateEvent? lastHPUpdate = log.CombatData.GetHealthUpdateEvents(target.AgentItem).LastOrDefault();
                if (lastHPUpdate != null)
                {
                    long maiTrinEnd = lastHPUpdate.Time;
                    replay.Trim(replay.TimeOffsets.start, maiTrinEnd);
                }

                // Nightmare Fusillade - Cone Attack
                var nightmareFusilladeMain = casts.Where(x => x.SkillId == NightmareFusilladeMain);
                foreach (CastEvent cast in nightmareFusilladeMain)
                {
                    int castDuration = 1767;
                    int waveDuration = 1066;
                    long growing = cast.Time + castDuration;
                    (long start, long end) lifespan = (cast.Time, ComputeEndCastTimeByBuffApplication(log, target, Stun, cast.Time, castDuration));
                    AddNightmareFusilladeDecorations(log, target, replay, lifespan, castDuration, waveDuration, growing, 1200, 90);
                }

                // Heartpiercer - Arrow attack with dash
                var heartpiercer = casts.Where(x => x.SkillId == Heartpiercer);
                foreach (CastEvent cast in heartpiercer)
                {
                    int castDuration = 2500;
                    uint range = 500;
                    long growing = cast.Time + castDuration;
                    (long start, long end) lifespan = (cast.Time, ComputeEndCastTimeByBuffApplication(log, target, Stun, cast.Time, castDuration));
                    var offset = new Vector3(range / 2, 0, 0);

                    // Get facing direction
                    if (target.TryGetCurrentFacingDirection(log, lifespan.start + 100, out var facingDirection, castDuration))
                    {
                        var indicator = (RectangleDecoration)new RectangleDecoration(range, 50, lifespan, Colors.LightOrange, 0.2, new AgentConnector(target.AgentItem).WithOffset(offset, true)).UsingRotationConnector(new AngleConnector(facingDirection));
                        replay.Decorations.AddWithGrowing(indicator, growing);
                    }

                }
                break;
            case (int)TrashID.MaiTrinStrikeDuringEcho:
                // Nightmare Fusillade - Cone Attack
                var nightmareFusilladeSide = target.GetCastEvents(log, log.FightData.FightStart, log.FightData.LogEnd).Where(x => x.SkillId == NightmareFusilladeSide);
                foreach (CastEvent cast in nightmareFusilladeSide)
                {
                    int castDuration = 3167;
                    int waveDuration = 1066;
                    long growing = cast.Time + castDuration;
                    (long start, long end) lifespan = (cast.Time, growing);
                    AddNightmareFusilladeDecorations(log, target, replay, lifespan, castDuration, waveDuration, growing, 1200, 90);
                }
                break;
            case (int)TargetID.EchoOfScarletBriarNM:
                AddTormentingWaveDecorations(target, log, replay);
                AddElectricBlastDecorations(target, log, replay);
                break;
            case (int)TargetID.EchoOfScarletBriarCM:
                AddTormentingWaveDecorations(target, log, replay);
                AddElectricBlastDecorations(target, log, replay);

                // Challenge Mode Puzzle
                const uint threshold = 5000;
                const uint innerRadius = 160;
                const uint outerRadius = 480;
                var initialPoint = new Vector3(3138.17456f, 1639.60657f, -1852.15894f); // The first cirle always spawns on the bomb on north east.

                // Filted bombs to select only 1 bomb per puzzle, with the max last aware
                var groupedBombs = AgentData.GetGroupedAgentsByTimeCondition(Targets.Where(x => x.IsSpecies(TrashID.FerrousBomb)).Select(x => x.AgentItem), (agent) => agent.FirstAware);
                var filteredBombs = groupedBombs.Select(x => x.MaxBy(y => y.LastAware));

                // Filter the detonations, we use them only for the end time
                var filteredDetonations = new List<EffectEvent>(2);
                if (log.CombatData.TryGetGroupedEffectEventsByGUID(EffectGUIDs.AetherbladeHideoutPuzzleCirclesDetonation, out var groupedDetonations))
                {
                    foreach (var effects in groupedDetonations)
                    {
                        filteredDetonations.Add(effects[0]);
                    }
                }

                foreach (var bomb in filteredBombs)
                {
                    (long start, long end) lifespanFirstCircle = (0, 0);

                    // In normal mode, the Echo gains Determined but in challenge mode it doesn't, we use the HP updates instead.
                    Segment? last60HpUpdate = target.GetHealthUpdates(log).FirstOrNull((in Segment x) => x.Value > 58 && x.Value <= 60);
                    Segment? last20HpUpdate = target.GetHealthUpdates(log).FirstOrNull((in Segment x) => x.Value > 18 && x.Value <= 20);

                    if (last60HpUpdate != null && Math.Abs(bomb.FirstAware - last60HpUpdate.Value.Start) < threshold)
                    {
                        EffectEvent? detonation = filteredDetonations.Where(x => Math.Abs(bomb.LastAware - x.Time) < threshold).FirstOrDefault();
                        if (detonation != null)
                        {
                            lifespanFirstCircle = (last60HpUpdate.Value.Start, Math.Min(bomb.LastAware, detonation.Time));
                        }
                        else // Fallback
                        {
                            lifespanFirstCircle = (last60HpUpdate.Value.Start, bomb.LastAware);
                        }
                    }
                    else if (last20HpUpdate != null)
                    {
                        EffectEvent? detonation = filteredDetonations.Where(x => Math.Abs(bomb.LastAware - x.Time) < threshold).FirstOrDefault();
                        if (detonation != null)
                        {
                            lifespanFirstCircle = (last20HpUpdate.Value.Start, Math.Min(bomb.LastAware, detonation.Time));
                        }
                        else // Fallback
                        {
                            lifespanFirstCircle = (last20HpUpdate.Value.Start, bomb.LastAware);
                        }
                    }

                    long duration = lifespanFirstCircle.end - lifespanFirstCircle.start; // time spent for the first circle to do a rotation
                    (long start, long end) lifespanSecondCircle = (lifespanFirstCircle.start + duration * 1 / 3, lifespanFirstCircle.end);
                    (long start, long end) lifespanThirdCircle = (lifespanFirstCircle.start + duration * 2 / 3, lifespanFirstCircle.end);

                    var firstCirclePoints = new List<ParametricPoint3D>();
                    var secondCirclePoints = new List<ParametricPoint3D>();
                    var thirdCirclePoints = new List<ParametricPoint3D>();

                    // Take the echo position as central point.
                    // The 3 circles always spawn in the same location, north east
                    // The second circle spawns when the first circle has complete a rotation of 240°
                    // The third circle spawns when the second circle has complete a rotation of 240°
                    if (target.TryGetCurrentPosition(log, lifespanFirstCircle.start, out var echoPosition))
                    {
                        var positionConnector = new PositionConnector(echoPosition).WithOffset(initialPoint - echoPosition, true, true);
                        var firstRotationConnector = new AngleConnector(0, 720);
                        var secondRotationConnector = new AngleConnector(0, 480);
                        var thirdRotationConnector = new AngleConnector(0, 240);

                        var lifespans = new List<(long, long)> {
                                lifespanFirstCircle,
                                lifespanSecondCircle,
                                lifespanThirdCircle
                            };

                        var rotationConnectors = new List<RotationConnector>()
                            {
                                firstRotationConnector,
                                secondRotationConnector,
                                thirdRotationConnector,
                            };

                        AddRotatingCirclesDecorations(replay.Decorations, rotationConnectors, lifespans, positionConnector, echoPosition, innerRadius, outerRadius);
                    }
                }
                break;
            case (int)TrashID.ScarletPhantomBeamNM:
                // Flanking Shot - Normal Mode Puzzle - Rectangular Beam
                var flankingShot = target.GetCastEvents(log, log.FightData.FightStart, log.FightData.LogEnd).Where(x => x.SkillId == FlankingShot);
                foreach (CastEvent cast in flankingShot)
                {
                    int duration = 500;
                    uint range = 1600;
                    (long start, long end) lifespanDamage = (cast.Time, cast.Time + duration);
                    var offset = new Vector3(range / 2, 0, 0);

                    // Get facing direction
                    if (target.TryGetCurrentFacingDirection(log, lifespanDamage.start + 100, out var facingDirection, duration))
                    {
                        (long start, long end) lifespanIndicator = (lifespanDamage.start - 1520, lifespanDamage.start);

                        var connector = new AgentConnector(target.AgentItem);
                        var angle = new AngleConnector(facingDirection);

                        var indicator = (RectangleDecoration)new RectangleDecoration(range, 300, lifespanIndicator, Colors.LightOrange, 0.2, connector.WithOffset(offset, true)).UsingRotationConnector(angle);
                        var damage = (RectangleDecoration)new RectangleDecoration(range, 300, lifespanDamage, Colors.LightBlue, 0.2, connector.WithOffset(offset, true)).UsingRotationConnector(angle);
                        replay.Decorations.Add(indicator);
                        replay.Decorations.Add(damage);
                    }

                }
                break;
            case (int)TrashID.FerrousBomb:
                // Mag Beam - Challenge Mode Puzzle - Rectangular Beams during the bomb puzzle.
                AddMagBeamDecorations(target, log, replay, MaiTrinCMBeamsBombTargetGreen, 30, 120);
                AddMagBeamDecorations(target, log, replay, MaiTrinCMBeamsBombTargetRed, 0, 90);
                AddMagBeamDecorations(target, log, replay, MaiTrinCMBeamsBombTargetBlue, 60, 150);
                break;
            default:
                break;
        }
    }
    internal override void ComputePlayerCombatReplayActors(PlayerActor player, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputePlayerCombatReplayActors(player, log, replay);

        // Mag Beam - Rectangular Beams during the bomb puzzle.
        AddMagBeamDecorations(player, log, replay, MaiTrinCMBeamsTargetGreen, 30, 120);
        AddMagBeamDecorations(player, log, replay, MaiTrinCMBeamsTargetRed, 0, 90);
        AddMagBeamDecorations(player, log, replay, MaiTrinCMBeamsTargetBlue, 60, 150);

        // Ley Breach - Visualizing the blue beam on the targeted player.
        var segments = player.GetBuffStatus(log, LeyBreachTargetBuff, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0);
        var offset = new Vector3(0, -100, 0);
        foreach (Segment segment in segments)
        {
            replay.Decorations.Add(new RectangleDecoration(25, 200, segment.TimeSpan, Colors.Blue, 0.2, new AgentConnector(player.AgentItem).WithOffset(offset, true)));
            replay.Decorations.AddWithGrowing(new CircleDecoration(80, segment.TimeSpan, Colors.Blue, 0.2, new AgentConnector(player.AgentItem)), segment.End);
        }

        // Kaleidoscopic Chaos - Spreads Normal Mode
        if (log.CombatData.TryGetEffectEventsByDstWithGUID(player.AgentItem, EffectGUIDs.AetherbladeHideoutKaleidoscopicChaosNM, out var spreadsNM))
        {
            foreach (EffectEvent effect in spreadsNM)
            {
                long duration = 5000;
                AddOnPlayerDecorations(log, player, replay, effect, Targets, duration, 200);
            }
        }

        // Kaleidoscopic Chaos - Spreads Challenge Mode
        if (log.CombatData.TryGetEffectEventsByDstWithGUID(player.AgentItem, EffectGUIDs.AetherbladeHideoutKaleidoscopicChaosCM, out var spreadsCM))
        {
            foreach (EffectEvent effect in spreadsCM)
            {
                long duration = 5000;
                AddOnPlayerDecorations(log, player, replay, effect, Targets, duration, 300);
            }
        }

        // Focused Destruction - Greens
        if (log.CombatData.TryGetEffectEventsByDstWithGUID(player.AgentItem, EffectGUIDs.AetherbladeHideoutFocusedDestructionGreen, out var greens))
        {
            foreach (EffectEvent effect in greens)
            {
                long duration = 6250;
                AddOnPlayerDecorations(log, player, replay, effect, Targets, duration, 165);
            }
        }
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log)
    {
        base.ComputeEnvironmentCombatReplayDecorations(log);

        // Ley Breach - Red Puddles Indicator
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.AetherbladeHideoutLeyBreachIndicator1, out var leyBreachIndicators))
        {
            foreach (EffectEvent effect in leyBreachIndicators)
            {
                long duration = 2000;
                uint radius = 240;
                (long start, long end) lifespan = effect.ComputeLifespan(log, duration);
                long growing = effect.Time + duration;
                var baseCircle = new CircleDecoration(radius, lifespan, Colors.LightOrange, 0.2, new PositionConnector(effect.Position));
                var growingCircle = (CircleDecoration)new CircleDecoration(radius, lifespan, Colors.LightOrange, 0.2, new PositionConnector(effect.Position)).UsingGrowingEnd(growing);
                EnvironmentDecorations.Add(baseCircle);
                EnvironmentDecorations.Add(growingCircle);
            }
        }

        // Ley Breach - Red Puddles
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.AetherbladeHideoutLeyBreachRedPuddle, out var leyBreachPuddle))
        {
            foreach (EffectEvent effect in leyBreachPuddle)
            {
                long duration = log.FightData.IsCM ? 30000 : 15000;
                (long start, long end) lifespan = effect.ComputeLifespan(log, duration);
                var circle = new CircleDecoration(240, lifespan, Colors.Red, 0.3, new PositionConnector(effect.Position));
                EnvironmentDecorations.Add(circle);
            }
        }

        // Fissure of Torment - Indicator
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.AetherbladeHideoutFissureOfTormentIndicator, out var fissureOfTormentIndicators))
        {
            foreach (EffectEvent effect in fissureOfTormentIndicators)
            {
                // Effect lasts slightly too long, we use the damage effect as end time if it's fully casted.
                // The effect doesn't have a Src, it will last the full 1300ms even if the Scarlet Phantom dies.
                (long start, long end) lifespan = effect.ComputeLifespan(log, 1300);
                (long start, long end) lifespan2 = effect.ComputeLifespanWithSecondaryEffectNoSrcCheck(log, EffectGUIDs.AetherbladeHideoutFissureOfTormentDamage);
                lifespan.end = Math.Min(lifespan.end, lifespan2.end);

                var rotationConnector = new AngleConnector(effect.Rotation.Z);
                var rectangle = (RectangleDecoration)new RectangleDecoration(600, 150, lifespan, Colors.LightOrange, 0.2, new PositionConnector(effect.Position)).UsingRotationConnector(rotationConnector);
                EnvironmentDecorations.Add(rectangle);
                EnvironmentDecorations.Add(rectangle.GetBorderDecoration(Colors.LightOrange, 0.2));
            }
        }

        // Fissure of Torment - Damage
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.AetherbladeHideoutFissureOfTormentDamage, out var fissureOfTormentDamage))
        {
            foreach (EffectEvent effect in fissureOfTormentDamage)
            {
                // We set 250ms duration as visual indication of the damage.
                (long start, long end) lifespanX = (effect.Time, effect.Time + 250);
                var rotationConnector = new AngleConnector(effect.Rotation.Z + 90);
                var rectangle = (RectangleDecoration)new RectangleDecoration(600, 150, lifespanX, Colors.Red, 0.2, new PositionConnector(effect.Position)).UsingRotationConnector(rotationConnector);
                EnvironmentDecorations.Add(rectangle);
            }
        }

        // Puzzle Normal Mode - The 3 rotating circles
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.AetherbladeHideoutPuzzleNormalMode, out var puzzleNM))
        {
            const uint radiusFromCenter = 375; // Found by calculating distance between detonation effect and echo position, original value is 374.999969
            const uint innerRadius = 160;
            const uint outerRadius = 480;
            SingleActor? echo = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.EchoOfScarletBriarNM));
            foreach (EffectEvent effect in puzzleNM)
            {
                // The effect position is outside of the arena, take the position of the Echo as central point.
                SingleActor? phantom = Targets.FirstOrDefault(x => x.IsSpecies(TrashID.ScarletPhantomBeamNM) && effect.Time <= x.LastAware);
                if (echo != null && phantom != null)
                {
                    if (echo.TryGetCurrentPosition(log, effect.Time, out var echoPosition) 
                        && phantom.TryGetCurrentFacingDirection(log, effect.Time + 100, out var phantomFacing, effect.Duration)
                        && phantom.TryGetCurrentPosition(log, effect.Time, out var phantomPosition))
                    {
                        (long start, long end) lifespanFirstCircle = effect.ComputeLifespan(log, 12000);
                        long duration = lifespanFirstCircle.end - lifespanFirstCircle.start; // time spent for the first circle to do a rotation
                        (long start, long end) lifespanSecondCircle = (lifespanFirstCircle.start + duration * 1 / 3, lifespanFirstCircle.end);
                        (long start, long end) lifespanThirdCircle = (lifespanFirstCircle.start + duration * 2 / 3, lifespanFirstCircle.end);

                        // Point on the Phantom facing direction, perpendicular from the Echo.
                        var pointOnLine = echoPosition.ProjectPointOn2DLine(phantomPosition, phantomFacing);
                        // Opposite point, our starting position
                        var initialPoint = echoPosition + Vector3.Normalize(echoPosition - pointOnLine) * radiusFromCenter;

                        // The 3 circles always spawn in the same location
                        // The second circle spawns when the first circle has complete a rotation of 120°
                        // The third circle spawns when the first circle has complete a rotation of 240° and the second circle 120°
                        var positionConnector = new PositionConnector(echoPosition).WithOffset(initialPoint - echoPosition, true, true);
                        var firstRotationConnector = new AngleConnector(0, 360);
                        var secondRotationConnector = new AngleConnector(0, 240);
                        var thirdRotationConnector = new AngleConnector(0, 120);

                        var lifespans = new List<(long, long)> {
                                lifespanFirstCircle,
                                lifespanSecondCircle,
                                lifespanThirdCircle
                            };

                        var rotationConnectors = new List<RotationConnector>()
                            {
                                firstRotationConnector,
                                secondRotationConnector,
                                thirdRotationConnector,
                            };

                        AddRotatingCirclesDecorations(EnvironmentDecorations, rotationConnectors, lifespans, positionConnector, echoPosition, innerRadius, outerRadius);
                    }
                }
            }
        }

        // The 3 rotating circles detonation
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.AetherbladeHideoutPuzzleCirclesDetonation, out var circlesDetonations))
        {
            foreach (EffectEvent effect in circlesDetonations)
            {
                (long start, long end) lifespan = (effect.Time, effect.Time + 500);
                var doughnut = new DoughnutDecoration(160, 480, lifespan, Colors.LightGrey, 0.2, new PositionConnector(effect.Position));
                EnvironmentDecorations.Add(doughnut);
            }
        }

        // Enrage
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.AetherbladeHideoutEnrage, out var enrage))
        {
            foreach (EffectEvent effect in enrage)
            {
                long duration = 8000;
                (long start, long end) lifespan = effect.ComputeLifespan(log, duration);
                EnvironmentDecorations.AddWithGrowing(new CircleDecoration(1000, lifespan, Colors.LightOrange, 0.2, new PositionConnector(effect.Position)), effect.Time + duration);
            }
        }
    }

    private SingleActor? GetEchoOfScarletBriar(FightData fightData)
    {
        return Targets.FirstOrDefault(x => x.IsSpecies(fightData.IsCM ? (int)TargetID.EchoOfScarletBriarCM : (int)TargetID.EchoOfScarletBriarNM));
    }

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        base.CheckSuccess(combatData, agentData, fightData, playerAgents);
        if (!fightData.Success)
        {
            SingleActor? echoOfScarlet = GetEchoOfScarletBriar(fightData);
            if (echoOfScarlet != null)
            {
                SingleActor maiTrin = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.MaiTrinStrike)) ?? throw new MissingKeyActorsException("Mai Trin not found");
                BuffApplyEvent? buffApply = combatData.GetBuffDataByIDByDst(Determined895, maiTrin.AgentItem).OfType<BuffApplyEvent>().LastOrDefault();
                if (buffApply != null && buffApply.Time > echoOfScarlet.FirstAware)
                {
                    fightData.SetSuccess(true, buffApply.Time);
                }
            }
        }
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor maiTrin = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.MaiTrinStrike)) ?? throw new MissingKeyActorsException("Mai Trin not found");
        phases[0].AddTarget(maiTrin);
        SingleActor? echoOfScarlet = GetEchoOfScarletBriar(log.FightData);
        if (echoOfScarlet != null)
        {
            phases[0].AddTarget(echoOfScarlet);
        }
        if (!requirePhases)
        {
            return phases;
        }
        if (log.CombatData.GetDamageTakenData(maiTrin.AgentItem).Any())
        {
            HealthUpdateEvent? lastHPUpdate = log.CombatData.GetHealthUpdateEvents(maiTrin.AgentItem).LastOrDefault();
            if (lastHPUpdate != null)
            {
                long maiTrinEnd = lastHPUpdate.Time;
                long maiTrinStart = 0;
                BuffRemoveAllEvent? buffRemove = log.CombatData.GetBuffDataByIDByDst(Determined895, maiTrin.AgentItem).OfType<BuffRemoveAllEvent>().Where(x => x.Time > maiTrinStart).FirstOrDefault();
                if (buffRemove != null)
                {
                    maiTrinStart = buffRemove.Time;
                }
                var mainPhase = new PhaseData(0, maiTrinEnd, "Mai Trin");
                mainPhase.AddTarget(maiTrin);
                phases.Add(mainPhase);
                List<PhaseData> maiPhases = GetPhasesByInvul(log, Untargetable, maiTrin, true, true, maiTrinStart, maiTrinEnd);
                for (int i = 0; i < maiPhases.Count; i++)
                {
                    PhaseData subPhase = maiPhases[i];
                    if ((i % 2) == 0)
                    {
                        subPhase.Name = "Mai Trin Phase " + ((i / 2) + 1);
                        subPhase.AddTarget(maiTrin);
                    }
                    else
                    {
                        subPhase.Name = "Mai Trin Split Phase " + ((i / 2) + 1);
                        AddTargetsToPhase(subPhase, [(int)TrashID.ScarletPhantomHP, (int)TrashID.ScarletPhantomHPCM]);
                    }
                }
                phases.AddRange(maiPhases);
            }
        }
        if (echoOfScarlet != null)
        {
            long echoStart = echoOfScarlet.FirstAware + 10000;
            var phase = new PhaseData(echoStart, log.FightData.FightEnd, "Echo of Scarlet Briar");
            phase.AddTarget(echoOfScarlet);
            phases.Add(phase);
            List<PhaseData> echoPhases = GetPhasesByInvul(log, Untargetable, echoOfScarlet, true, true, echoStart, log.FightData.FightEnd);
            for (int i = 0; i < echoPhases.Count; i++)
            {
                PhaseData subPhase = echoPhases[i];
                if ((i % 2) == 0)
                {
                    subPhase.Name = "Echo Phase " + ((i / 2) + 1);
                    subPhase.AddTarget(echoOfScarlet);
                }
                else
                {
                    subPhase.Name = "Echo Split Phase " + ((i / 2) + 1);
                    AddTargetsToPhase(subPhase, [(int)TrashID.ScarletPhantomHP, (int)TrashID.ScarletPhantomHPCM]);
                }
            }
            phases.AddRange(echoPhases);
        }
        return phases;
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        // Ferrous Bombs
        var bombs = combatData.Where(x => MaxHealthUpdateEvent.GetMaxHealth(x) == 89640 && x.IsStateChange == StateChange.MaxHealthUpdate).Select(x => agentData.GetAgent(x.SrcAgent, x.Time)).Where(x => x.Type == AgentItem.AgentType.Gadget);
        foreach (AgentItem bomb in bombs)
        {
            bomb.OverrideType(AgentItem.AgentType.NPC, agentData);
            bomb.OverrideID(TrashID.FerrousBomb, agentData);
        }
        // We remove extra Mai trins if present
        IReadOnlyList<AgentItem> maiTrins = agentData.GetNPCsByID(TargetID.MaiTrinStrike);
        if (maiTrins.Count > 1)
        {
            for (int i = 1; i < maiTrins.Count; i++)
            {
                maiTrins[i].OverrideID(IgnoredSpecies, agentData);
            }
        }
        if (agentData.GetNPCsByID(TargetID.EchoOfScarletBriarNM).Count + agentData.GetNPCsByID(TargetID.EchoOfScarletBriarCM).Count == 0)
        {
            agentData.AddCustomNPCAgent(int.MaxValue, int.MaxValue, "Echo of Scarlet Briar", Spec.NPC, TargetID.EchoOfScarletBriarNM, false);
            agentData.AddCustomNPCAgent(int.MaxValue, int.MaxValue, "Echo of Scarlet Briar", Spec.NPC, TargetID.EchoOfScarletBriarCM, false);
        }
        base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
        var echoesOfScarlet = Targets.Where(x => x.IsSpecies(TargetID.EchoOfScarletBriarNM) || x.IsSpecies(TargetID.EchoOfScarletBriarCM));
        foreach (SingleActor echoOfScarlet in echoesOfScarlet)
        {
            var hpUpdates = combatData.Where(x => x.SrcMatchesAgent(echoOfScarlet.AgentItem) && x.IsStateChange == StateChange.HealthUpdate).ToList();
            if (hpUpdates.Count > 1 && HealthUpdateEvent.GetHealthPercent(hpUpdates.LastOrDefault()!) == 100)
            {
                hpUpdates.Last().OverrideDstAgent(hpUpdates[^2].DstAgent);
            }
        }
        int curHP = 1;
        int curCC = 1;
        int curBomb = 1;
        int curBeam = 1;
        foreach (NPC target in Targets)
        {
            switch (target.ID)
            {
                case (int)TrashID.ScarletPhantomBreakbar:
                    target.OverrideName("Elite " + target.Character + " CC " + curCC++);
                    break;
                case (int)TrashID.ScarletPhantomHP:
                case (int)TrashID.ScarletPhantomHPCM:
                    target.OverrideName("Elite " + target.Character + " HP " + curHP++);
                    break;
                case (int)TrashID.FerrousBomb:
                    target.OverrideName("Ferrous Bomb " + curBomb++);
                    break;
                case (int)TrashID.ScarletPhantomBeamNM:
                    target.OverrideName("Scarlet Phantom " + curBeam++);
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// Mag Beam - The beams during the puzzle in Challenge Mode.<br></br>
    /// </summary>
    /// <param name="actor">Actor with the buff, can be the player or the Ferrous Bomb.</param>
    /// <param name="replay">Combat Replay.</param>
    /// <param name="log">The log.</param>
    /// <param name="skillId">The buff applied on the player or the Ferrous Bomb.</param>
    /// <param name="angle1">The rotation angle of the first rectangle.</param>
    /// <param name="angle2">The rotation angle of the second rectangle.</param>
    private static void AddMagBeamDecorations(SingleActor actor, ParsedEvtcLog log, CombatReplay replay, long skillId, int angle1, int angle2)
    {
        const uint width = 320;
        const uint length = 2000;
        var segments = actor.GetBuffStatus(log, skillId, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0);

        // If the actor is a player, add the overhead bomb icon.
        if (actor.AgentItem.IsPlayer)
        {
            replay.Decorations.AddOverheadIcons(segments, actor, ParserIcons.BombOverhead);
        }

        var agentConnector = new AgentConnector(actor.AgentItem);
        var angleConnector1 = new AngleConnector(angle1);
        var angleConnector2 = new AngleConnector(angle2);

        // Add the rectangles on the player and Ferrous Bomb.
        foreach (Segment segment in segments)
        {
            var indicator1 = (RectangleDecoration)new RectangleDecoration(width, length, segment.TimeSpan, Colors.LightOrange, 0.2, agentConnector).UsingRotationConnector(angleConnector1);
            var indicator2 = (RectangleDecoration)new RectangleDecoration(width, length, segment.TimeSpan, Colors.LightOrange, 0.2, agentConnector).UsingRotationConnector(angleConnector2);
            replay.Decorations.Add(indicator1);
            replay.Decorations.Add(indicator2);

            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.AetherbladeHideoutPuzzleCirclesDetonation, out var detonations) && actor.TryGetCurrentPosition(log, segment.End, out var position, 1000))
            {
                foreach (EffectEvent effect in detonations.Where(x => Math.Abs(segment.End - x.Time) < 100 && Vector2.Distance(position.XY(), x.Position.XY()) < 20))
                {
                    // Adding an effect for the damage like Normal Mode
                    // We use the circles detonations as timestamp
                    (long start, long end) lifespanDamage = (effect.Time, effect.Time + 500);

                    var positionConnector = new PositionConnector(effect.Position);

                    var damage1 = (RectangleDecoration)new RectangleDecoration(width, length, lifespanDamage, Colors.LightBlue, 0.2, positionConnector).UsingRotationConnector(angleConnector1);
                    var damage2 = (RectangleDecoration)new RectangleDecoration(width, length, lifespanDamage, Colors.LightBlue, 0.2, positionConnector).UsingRotationConnector(angleConnector2);
                    replay.Decorations.Add(damage1);
                    replay.Decorations.Add(damage2);
                }
            }
        }
    }

    /// <summary>
    /// Nightmare Fusillade - The cone attack with shockwave.
    /// </summary>
    private static void AddNightmareFusilladeDecorations(ParsedEvtcLog log, NPC target, CombatReplay replay, (long start, long end) lifespan, int castDuration, int waveDuration, long growing, uint radius, int angle)
    {
        // Get facing direction
        if (target.TryGetCurrentFacingDirection(log, lifespan.start + 100, out var facingDirection, castDuration))
        {
            // Add indicator
            var connector = new AgentConnector(target);
            var rotation = new AngleConnector(facingDirection);
            var pie = (PieDecoration)new PieDecoration(radius, angle, lifespan, Colors.Orange, 0.2, connector).UsingRotationConnector(rotation);
            replay.Decorations.AddWithGrowing(pie, growing);
            replay.Decorations.Add(pie.GetBorderDecoration());

            // If the indicator lifespan matches the growing or the side Mai Trin is casting, add the shockwave.
            if ((target.IsSpecies(TargetID.MaiTrinStrike) && lifespan.end == growing) || target.IsSpecies(TrashID.MaiTrinStrikeDuringEcho))
            {
                (long start, long end) lifespanWave = (lifespan.end, lifespan.end + waveDuration);
                var background = (PieDecoration)new PieDecoration(radius, angle, lifespanWave, Colors.Orange, 0.1, connector).UsingRotationConnector(rotation);
                var shockwave = (PieDecoration)new PieDecoration(radius, angle, lifespanWave, Colors.Red, 0.4, connector).UsingFilled(false).UsingRotationConnector(rotation);
                replay.Decorations.Add(background);
                replay.Decorations.AddWithGrowing(shockwave, lifespanWave.end);
            }
        }
    }

    /// <summary>
    /// Kaleidoscopic Chaos - Spread AoEs.<br></br>
    /// Focused Destruction - Green AoEs.<br></br>
    /// <remarks>As of EVTC20241030 effects on players have Dynamic End Time.</remarks>
    /// </summary>
    private static void AddOnPlayerDecorations(ParsedEvtcLog log, PlayerActor player, CombatReplay replay, EffectEvent effect, IReadOnlyList<SingleActor> targets, long duration, uint radius)
    {
        (long start, long end) lifespan = effect.ComputeLifespan(log, duration);
        long growing = effect.Time + duration;
        Color color = Colors.LightOrange;
        var species = new List<int>();

        switch (effect.GUIDEvent.ContentGUID)
        {
            case var value when value == EffectGUIDs.AetherbladeHideoutFocusedDestructionGreen:
                color = Colors.DarkGreen;
                species.Add((int)TrashID.ScarletPhantomHP);
                species.Add((int)TrashID.ScarletPhantomHPCM);
                break;
            case var value2 when value2 == EffectGUIDs.AetherbladeHideoutKaleidoscopicChaosNM:
            case var value3 when value3 == EffectGUIDs.AetherbladeHideoutKaleidoscopicChaosCM:
                color = Colors.LightOrange;
                species.Add((int)TrashID.ScarletPhantomBreakbar);
                break;
            default:
                break;
        }

        // If the Scarlet Phantom is killed before the spreads or green ends, the mechanic will despawn.
        // Find the minimum between Dead Event, Despawn Event and Last Aware, then override lifespan end time.
        if (!effect.HasDynamicEndTime)
        {
            lifespan.end = Math.Min(lifespan.end, effect.ComputeLifespanWithNPCRemoval(log, targets, species).end);
        }

        replay.Decorations.AddWithGrowing(new CircleDecoration(radius, lifespan, color, 0.2, new AgentConnector(player)), growing);
    }

    /// <summary>
    /// Tormenting Wave - Orange circle created when the Echo is smashing the ground, generating a shockwave.
    /// </summary>
    private static void AddTormentingWaveDecorations(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.AetherbladeHideoutTormentingWaveIndicator1, out var tormentingWaves))
        {
            foreach (EffectEvent effect in tormentingWaves)
            {
                long duration = 3000;
                (long start, long end) lifespan = effect.ComputeLifespan(log, duration);
                long growing = effect.Time + duration;

                // If the Echo is stunned, end the effect early.
                Segment? stun = target.GetBuffStatus(log, Stun, lifespan.start, lifespan.end).FirstOrNull((in Segment x) => x.Value == 1);
                if (stun != null)
                {
                    lifespan.end = Math.Min(stun.Value.Start, lifespan.end);
                }

                var indicator = new CircleDecoration(150, lifespan, Colors.LightOrange, 0.2, new PositionConnector(effect.Position));
                replay.Decorations.AddWithGrowing(indicator, growing);

                // If the echo isn't stunned, add the shockwave. Rendering order
                if (stun == null)
                {
                    (long start, long end) lifespanShockwave = (lifespan.end, lifespan.end + 3000);
                    replay.Decorations.AddShockwave(new PositionConnector(effect.Position), lifespanShockwave, Colors.Red, 0.2, 1200);
                }
            }
        }
    }

    /// <summary>
    /// Electric Blast - AoEs created when the Echo swipes her arms.
    /// </summary>
    private static void AddElectricBlastDecorations(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.AetherbladeHideoutElectricBlastIndicator, out var electricBlastIndicators))
        {
            uint initialRadius = 0;
            uint radiusIncrease = 0;
            uint radius = 0;

            int index = 0;
            int previousIndex = 0;

            var electricBlasts = target.GetCastEvents(log, log.FightData.FightStart, log.FightData.LogEnd).Where(x =>
                x.SkillId == ElectricBlastCastSkillLeftNM ||
                x.SkillId == ElectricBlastCastSkillRightNM ||
                x.SkillId == ElectricBlastCastSkillLeftCM ||
                x.SkillId == ElectricBlastCastSkillSpiralsCM).ToList();

            // Store the times for binary search
            var times = new long[electricBlasts.Count];
            for (int i = 0; i < electricBlasts.Count; i++)
            {
                times[i] = electricBlasts[i].Time;
            }

            foreach (EffectEvent indicator in electricBlastIndicators)
            {
                // Find the index of the array with the time closest to the effect time.
                // It starts searching from the previous found index instead of the beginning.
                // This only works for ordered casts and effects lists.
                index = Array.BinarySearch(times, previousIndex, times.Length - previousIndex, indicator.Time);
                if (index < 0)
                {
                    // No exact match, BinarySearch returns negative index of the next larger element,
                    // or array.Length, which is the "next larger element" of the last array element.
                    index = ~index - 1;
                }

                // Find the skill cast at the index and assign a radius based on the skill cast.
                // Skill definitions are empty, value found through testing and video review.
                switch (electricBlasts[index].SkillId)
                {
                    case ElectricBlastCastSkillLeftNM: // Swipe from the left side of the Echo, clockwise.
                        initialRadius = 100;
                        radiusIncrease = 2;
                        break;
                    case ElectricBlastCastSkillRightNM: // Swipe from the right side of the Echo, counter clockwise.
                        initialRadius = 110;
                        radiusIncrease = 10;
                        break;
                    case ElectricBlastCastSkillLeftCM: // Swipe from the left side of the Echo, clockwise.
                        initialRadius = 100;
                        radiusIncrease = 2;
                        break;
                    case ElectricBlastCastSkillSpiralsCM: // Swipe in a spiral pattern, counter clockwise.
                        initialRadius = 100;
                        radiusIncrease = 5;
                        break;
                    default:
                        Debug.Assert(false, $"Unknown indicator skill id {electricBlasts[index].SkillId}");
                        break;
                }

                // Set the initial radius or reset it if a new index has been found.
                if (radius == 0 || previousIndex != index)
                {
                    radius = initialRadius;
                }

                // Add indicator decoration
                (long start, long end) lifespan = indicator.ComputeLifespanWithSecondaryEffectAndPosition(log, EffectGUIDs.AetherbladeHideoutElectricBlastDetonation);
                var indicatorDeco = (CircleDecoration)new CircleDecoration(radius, lifespan, Colors.Red, 0.2, new PositionConnector(indicator.Position)).UsingFilled(false);
                replay.Decorations.Add(indicatorDeco);

                if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.AetherbladeHideoutElectricBlastDetonation, out var electricBlastDetonations))
                {
                    // Add detonation decoration
                    foreach (EffectEvent detonation in electricBlastDetonations.Where(x => Vector2.Distance(x.Position.XY(), indicator.Position.XY()) < 1e-6))
                    {
                        (long start, long end) lifespanDetonation = (detonation.Time, detonation.Time + 250); // Logged duration of 0, overriding to 250 for visual.
                        var detonationDeco = new CircleDecoration(radius, lifespanDetonation, Colors.Yellow, 0.1, new PositionConnector(detonation.Position));
                        replay.Decorations.AddWithBorder(detonationDeco, Colors.Red, 0.2);
                    }
                }

                // Increase radius and store index used.
                radius += radiusIncrease;
                previousIndex = index;
            }
        }
    }

    /// <summary>
    /// Adds the rotating circles during the Puzzle mechanic.
    /// </summary>
    private static void AddRotatingCirclesDecorations(CombatReplayDecorationContainer decorations, List<RotationConnector> rotationConnectors, List<(long, long)> lifespans, GeographicalConnector positionConnector, Vector3 echoPosition, uint innerRadius, uint outerRadius)
    {
        decorations.Add(new CircleDecoration(1000, lifespans[0], Colors.LightOrange, 0.2, new PositionConnector(echoPosition)));
        decorations.Add(new DoughnutDecoration(innerRadius, outerRadius, lifespans[0], Colors.LightOrange, 0.2, positionConnector).UsingRotationConnector(rotationConnectors[0]));
        decorations.Add(new CircleDecoration(innerRadius, lifespans[0], Colors.White, 0.5, positionConnector).UsingRotationConnector(rotationConnectors[0]));
        decorations.Add(new DoughnutDecoration(innerRadius, outerRadius, lifespans[1], Colors.LightOrange, 0.2, positionConnector).UsingRotationConnector(rotationConnectors[1]));
        decorations.Add(new CircleDecoration(innerRadius, lifespans[1], Colors.White, 0.5, positionConnector).UsingRotationConnector(rotationConnectors[1]));
        decorations.Add(new DoughnutDecoration(innerRadius, outerRadius, lifespans[2], Colors.LightOrange, 0.2, positionConnector).UsingRotationConnector(rotationConnectors[2]));
        decorations.Add(new CircleDecoration(innerRadius, lifespans[2], Colors.White, 0.5, positionConnector).UsingRotationConnector(rotationConnectors[2]));
    }

    internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
    {
        SingleActor maiTrin = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.MaiTrinStrike)) ?? throw new MissingKeyActorsException("Mai Trin not found");
        return maiTrin.GetHealth(combatData) > 8e6 ? FightData.EncounterMode.CM : FightData.EncounterMode.Normal;
    }

    protected override void SetInstanceBuffs(ParsedEvtcLog log)
    {
        base.SetInstanceBuffs(log);

        if (log.FightData.Success)
        {
            if (log.CombatData.GetBuffData(AchievementEligibilityTriangulation).Any())
            {
                InstanceBuffs.MaybeAdd(GetOnPlayerCustomInstanceBuff(log, AchievementEligibilityTriangulation));
            }
            else if (CustomCheckTriangulationEligibility(log))
            {
                InstanceBuffs.Add((log.Buffs.BuffsByIds[AchievementEligibilityTriangulation], 1));
            }
        }
    }

    private static bool CustomCheckTriangulationEligibility(ParsedEvtcLog log)
    {
        IReadOnlyList<long> beamsBuffs = new List<long>() { MaiTrinCMBeamsTargetBlue, MaiTrinCMBeamsTargetGreen, MaiTrinCMBeamsTargetRed };
        var beamsSegments = new List<Segment>();
        var bombInvulnSegments = new List<Segment>();

        foreach (AgentItem player in log.PlayerAgents)
        {
            IReadOnlyDictionary<long, BuffGraph> bgms = log.FindActor(player).GetBuffGraphs(log);
            foreach (long buff in beamsBuffs)
            {
                beamsSegments = GetBuffSegments(bgms, buff, beamsSegments).OrderBy(x => x.Start).ToList();
            }
        }

        foreach (AgentItem agent in log.AgentData.GetNPCsByID(TrashID.FerrousBomb))
        {
            IReadOnlyDictionary<long, BuffGraph> bgms = log.FindActor(agent).GetBuffGraphs(log);
            bombInvulnSegments = GetBuffSegments(bgms, FailSafeActivated, bombInvulnSegments).OrderBy(x => x.Start).ToList();
        }

        int counter = 0;

        // For each segment where a bomb is invulnerable, check if it has started between the assignment and loss of a beam effect on a player (through buff)
        // If the counter is == 8, it means every possible combination check has been met and it's eligible for the achievement.
        // The combinations are 2 players buffs for each bomb invulnerability buff, so 2 x 4 total.
        foreach (Segment invuln in bombInvulnSegments)
        {
            foreach (Segment s in beamsSegments)
            {
                if (s.Start < invuln.Start && invuln.Start < s.End)
                {
                    counter++;
                }
            }
        }

        return counter == 8;
    }

    private static List<Segment> GetBuffSegments(IReadOnlyDictionary<long, BuffGraph> bgms, long buff, List<Segment> segments)
    {
        if (bgms != null && bgms.TryGetValue(buff, out var bgm))
        {
            foreach (Segment s in bgm.Values)
            {
                if (s.Value == 1)
                {
                    segments.Add(s);
                }
            }
        }
        return segments;
    }
}
