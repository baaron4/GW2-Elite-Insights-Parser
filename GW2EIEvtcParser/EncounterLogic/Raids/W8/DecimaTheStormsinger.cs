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

internal class DecimaTheStormsinger : MountBalrior
{
    private readonly bool isCM = false;

    private SingleActor Decima => Targets.FirstOrDefault(x => isCM ? x.IsSpecies(TargetID.DecimaCM) : x.IsSpecies(TargetID.Decima)) ?? throw new MissingKeyActorsException("Decima not found");
    public DecimaTheStormsinger(int triggerID) : base(triggerID)
    {
        MechanicList.AddRange(new List<Mechanic>()
        {
            new PlayerDstHitMechanic(ChorusOfThunderDamage, "Chorus of Thunder", new MechanicPlotlySetting(Symbols.Circle, Colors.LightOrange), "ChorThun.H", "Hit by Chorus of Thunder (Spreads AoE / Conduit AoE)", "Chorus of Thunder Hit", 0),
            new PlayerDstHitMechanic(SeismicCrashDamage, "Seismic Crash", new MechanicPlotlySetting(Symbols.Hourglass, Colors.White), "SeisCrash.H", "Hit by Seismic Crash (Concentric Rings)", "Seismic Crash Hit", 0),
            new PlayerDstHitMechanic(SeismicCrashDamage, "Seismic Crash", new MechanicPlotlySetting(Symbols.Hourglass, Colors.DarkWhite), "SeisCrash.CC", "CC by Seismic Crash (Concentric Rings)", "Seismic Crash CC", 0).UsingChecker((hde, log) => !hde.To.HasBuff(log, Stability, hde.Time, ServerDelayConstant)),
            new PlayerDstHitMechanic(Earthrend, "Earthrend", new MechanicPlotlySetting(Symbols.CircleOpen, Colors.Blue), "Earthrend.H", "Hit by Earthrend (Outer Doughnut)", "Earthrend Hit", 0),
            new PlayerDstHitMechanic(Earthrend, "Earthrend", new MechanicPlotlySetting(Symbols.CircleOpen, Colors.DarkBlue), "Earthrend.CC", "CC by Earthrend (Outer Doughnut)", "Earthrend CC", 0).UsingChecker((hde, log) => !hde.To.HasBuff(log, Stability, hde.Time, ServerDelayConstant)),
            new PlayerDstHitMechanic(Fluxlance, "Fluxlance", new MechanicPlotlySetting(Symbols.StarSquare, Colors.LightOrange), "Fluxlance.H", "Hit by Fluxlance (Single Orange Arrow)", "Fluxlance Hit", 0),
            new PlayerDstHitMechanic(FluxlanceFusillade, "Fluxlance Fusillade", new MechanicPlotlySetting(Symbols.StarDiamond, Colors.LightOrange), "FluxFusi.H", "Hit by Fluxlance Fusillade (Sequential Orange Arrows)", "Fluxlance Fusillade Hit", 0),
            new PlayerDstHitMechanic([FluxlanceSalvo1, FluxlanceSalvo2, FluxlanceSalvo3, FluxlanceSalvo4, FluxlanceSalvo5], "Fluxlance Salvo", new MechanicPlotlySetting(Symbols.StarDiamondOpen, Colors.LightOrange), "FluxSalvo.H", "Hit by Fluxlance Salvo (Simultaneous Orange Arrows)", "Fluxlance Salvo Hit", 0),
            new PlayerDstHitMechanic([Fluxlance, FluxlanceFusillade, FluxlanceSalvo1, FluxlanceSalvo2, FluxlanceSalvo3, FluxlanceSalvo4, FluxlanceSalvo5], "Fluxlance", new MechanicPlotlySetting(Symbols.DiamondWide, Colors.DarkMagenta), "FluxInc.H", "Hit by Fluxlance with Harmonic Sensitivity", "Fluxlance with Harmonic Sensitivity Hit", 0),
            new PlayerDstHitMechanic(SparkingAuraTier1, "Sparking Aura", new MechanicPlotlySetting(Symbols.CircleX, Colors.Green), "SparkAura1.H", "Sparking Aura (Absorbed Tier 1 Green Damage)", "Absorbed Tier 1 Green", 0),
            new PlayerDstHitMechanic(SparkingAuraTier2, "Sparking Aura", new MechanicPlotlySetting(Symbols.CircleX, Colors.LightMilitaryGreen), "SparkAura2.H", "Sparking Aura (Absorbed Tier 2 Green Damage)", "Absorbed Tier 2 Green", 0),
            new PlayerDstHitMechanic(SparkingAuraTier3, "Sparking Aura", new MechanicPlotlySetting(Symbols.CircleX, Colors.DarkGreen), "SparkAura3.H", "Sparking Aura (Absorbed Tier 3 Green Damage)", "Absorbed Tier 3 Green", 0),
            new PlayerDstHitMechanic([SparkingAuraTier1, SparkingAuraTier2, SparkingAuraTier3], "Sparking Aura", new MechanicPlotlySetting(Symbols.CircleX, Colors.MilitaryGreen), "SparkAuraInc.H", "Hit by Sparking Aura with Galvanic Sensitivity", "Sparking Aura with Galvanic Sensitivity Hit", 0),
            new PlayerDstHitMechanic(FulgentFence, "Fulgent Fence", new MechanicPlotlySetting(Symbols.Octagon, Colors.Purple), "FulFence.H", "Hit by Fulgent Fence (Barriers between Conduits)", "Fulgence Fence Hit", 0),
            new PlayerDstHitMechanic(ReverberatingImpact, "Reverberating Impact", new MechanicPlotlySetting(Symbols.StarOpen, Colors.LightBlue), "RevImpact.H", "Hit by Reverberating Impact (Hit a Conduit)", "Reverberating Impact Hit", 0),
            new PlayerDstHitMechanic([FulgentAuraTier1, FulgentAuraTier2, FulgentAuraTier3], "Fulgent Aura", new MechanicPlotlySetting(Symbols.CircleXOpen, Colors.Purple), "FulAura.H", "Hit by Fulgent Aura (Conduit AoE)", "Fulgent Aura Hit", 0),
            new PlayerDstSkillMechanic(Earthrend, "Earthrend", new MechanicPlotlySetting(Symbols.CircleCrossOpen, Colors.LightRed), "Earthrend.Dwn", "Downed by Earthrend (Hitbox)", "Earthrend Downed", 0).UsingChecker((hde, log) => hde.To.IsDowned(log, hde.Time)).WithBuilds(GW2Builds.December2024MountBalriorNerfs),
            new PlayerDstSkillMechanic(SeismicCrashHitboxDamage, "Seismic Crash", new MechanicPlotlySetting(Symbols.CircleCross, Colors.LightRed), "SeisCrash.Dwn", "Downed by Seismic Crash (Hitbox)", "Seismic Crash Downed", 0).UsingChecker((hde, log) => hde.To.IsDowned(log, hde.Time)).WithBuilds(GW2Builds.December2024MountBalriorNerfs),
            new PlayerDstSkillMechanic(Earthrend, "Earthrend", new MechanicPlotlySetting(Symbols.CircleCrossOpen, Colors.Red), "Earthrend.D", "Earthrend Death (Hitbox)", "Earthrend Death", 0).UsingChecker((hde, log) => hde.To.IsDead(log, hde.Time)), // If a player is already in downstate they get killed
            new PlayerDstSkillMechanic(SeismicCrashHitboxDamage, "Seismic Crash", new MechanicPlotlySetting(Symbols.CircleCross, Colors.Red), "SeisCrash.D", "Seismic Crash Death (Hitbox)", "Seismic Crash Death", 0).UsingChecker((hde, log) => hde.To.IsDead(log, hde.Time)), // If a player is already in downstate they get killed
            new PlayerDstBuffApplyMechanic([TargetOrder1JW, TargetOrder2JW, TargetOrder3JW, TargetOrder4JW, TargetOrder5JW], "Target Order", new MechanicPlotlySetting(Symbols.StarTriangleDown, Colors.LightOrange), "FluxOrder.T", "Targeted by Fluxlance (Target Order)", "Fluxlance Target (Sequential)", 0),
            new PlayerDstBuffApplyMechanic(FluxlanceTargetBuff1, "Fluxlance", new MechanicPlotlySetting(Symbols.StarTriangleDown, Colors.Orange), "Fluxlance.T", "Targeted by Fluxlance", "Fluxlance Target", 0),
            new PlayerDstBuffApplyMechanic(FluxlanceRedArrowTargetBuff, "Fluxlance", new MechanicPlotlySetting(Symbols.StarTriangleDown, Colors.Red), "FluxRed.T", "Targeted by Fluxlance (Red Arrow)", "Fluxlance (Red Arrow)", 0),
            new PlayerDstEffectMechanic(EffectGUIDs.DecimaChorusOfThunderAoE, "Chorus of Thunder", new MechanicPlotlySetting(Symbols.Circle, Colors.LightGrey), "ChorThun.T", "Targeted by Chorus of Thunder (Spreads)", "Chorus of Thunder Target", 0),
            new EnemyDstBuffApplyMechanic(ChargeDecima, "Charge", new MechanicPlotlySetting(Symbols.BowtieOpen, Colors.DarkMagenta), "Charge", "Charge Stacks", "Charge Stack", 0),
            new EnemyDstBuffApplyMechanic(Exposed31589, "Exposed", new MechanicPlotlySetting(Symbols.BowtieOpen, Colors.LightPurple), "Exposed", "Got Exposed (Broke Breakbar)", "Exposed", 0),
        });
        Extension = "decima";
        Icon = EncounterIconDecima;
        EncounterCategoryInformation.InSubCategoryOrder = 1;
        EncounterID |= 0x000002;
        isCM = triggerID == (int)TargetID.DecimaCM;
    }
    protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
    {
        return new CombatReplayMap(CombatReplayDecimaTheStormsinger,
                        (1602, 1602),
                        (-13068, 10300, -7141, 16227));
    }

    protected override ReadOnlySpan<int> GetTargetsIDs()
    {
        return
        [
            (int)TargetID.Decima,
            (int)TargetID.DecimaCM,
            (int)TrashID.TranscendentBoulder,
        ];
    }

    protected override List<TrashID> GetTrashMobsIDs()
    {
        return
        [
            TrashID.GreenOrb1Player,
            TrashID.GreenOrb1PlayerCM,
            TrashID.GreenOrb2Players,
            TrashID.GreenOrb2PlayersCM,
            TrashID.GreenOrb3Players,
            TrashID.GreenOrb3PlayersCM,
            TrashID.EnlightenedConduitCM,
            TrashID.EnlightenedConduit,
            TrashID.EnlightenedConduitGadget,
            TrashID.DecimaBeamStart,
            TrashID.DecimaBeamEnd,
        ];
    }

    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        return
        [
            new DamageCastFinder(ThrummingPresenceBuff, ThrummingPresenceDamage),
            new DamageCastFinder(ThrummingPresenceBuffCM, ThrummingPresenceDamageCM),
        ];
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        var conduitsGadgets = combatData
            .Where(x => x.IsStateChange == StateChange.MaxHealthUpdate && MaxHealthUpdateEvent.GetMaxHealth(x) == 15276)
            .Select(x => agentData.GetAgent(x.SrcAgent, x.Time))
            .Where(x => x.Type == AgentItem.AgentType.Gadget && x.HitboxWidth == 100 && x.HitboxHeight == 200)
            .Distinct();
        var effects = isCM ? combatData.Where(x => x.IsEffect) : [];
        foreach (var conduitGadget in conduitsGadgets)
        {
            if (isCM)
            {
                var effectByConduitOnGadget = effects
                    .Where(x => x.DstMatchesAgent(conduitGadget)
                                && agentData.GetAgent(x.SrcAgent, x.Time).IsSpecies(TrashID.EnlightenedConduitCM)
                    ).FirstOrDefault();
                if (effectByConduitOnGadget != null)
                {
                    conduitGadget.OverrideID(TrashID.EnlightenedConduitGadget, agentData);
                    conduitGadget.OverrideType(AgentItem.AgentType.NPC, agentData);
                    conduitGadget.SetMaster(agentData.GetAgent(effectByConduitOnGadget.SrcAgent, effectByConduitOnGadget.Time));
                }
            } 
            else
            {
                conduitGadget.OverrideID(TrashID.EnlightenedConduitGadget, agentData);
                conduitGadget.OverrideType(AgentItem.AgentType.NPC, agentData);
            }
        }
        base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
        int cur = 1;
        foreach (SingleActor target in Targets)
        {
            if (target.IsSpecies(TrashID.TranscendentBoulder))
            {
                target.OverrideName(target.Character + " " + cur++);
            }
        }
    }

    private static PhaseData GetBoulderPhase(ParsedEvtcLog log, IEnumerable<SingleActor> boulders, string name, SingleActor decima)
    {
        long start = long.MaxValue;
        long end = long.MinValue;
        foreach (SingleActor boulder in boulders) {
            start = Math.Min(boulder.FirstAware, start);
            var deadEvent = log.CombatData.GetDeadEvents(boulder.AgentItem).FirstOrDefault();
            if (deadEvent != null)
            {
                end = Math.Max(deadEvent.Time, end);
            } 
            else
            {
                end = Math.Max(boulder.LastAware, end);
            }
        }
        var phase = new PhaseData(start, end, name);
        phase.AddTargets(boulders);
        phase.AddTarget(decima, PhaseData.TargetPriority.Blocking);
        return phase;
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor decima = Decima;
        phases[0].AddTarget(decima);
        if (!requirePhases)
        {
            return phases;
        }
        // Invul check
        phases.AddRange(GetPhasesByInvul(log, isCM ? NovaShieldCM : NovaShield, decima, true, true));
        for (int i = 1; i < phases.Count; i++)
        {
            PhaseData phase = phases[i];
            if (i % 2 == 0)
            {
                phase.Name = "Split " + (i) / 2;
                phase.AddTarget(decima);
            }
            else
            {
                phase.Name = "Phase " + (i + 1) / 2;
                phase.AddTarget(decima);
            }
        }
        // Boulder phases
        if (isCM)
        {
            var boulders = Targets.Where(x => x.IsSpecies(TrashID.TranscendentBoulder)).OrderBy(x => x.FirstAware);
            var firstBoulders = boulders.Take(new Range(0, 2));
            if (firstBoulders.Any())
            {
                phases.Add(GetBoulderPhase(log, firstBoulders, "Boulders 1", decima));
                var secondBoulders = boulders.Take(new Range(2, 4));
                if (secondBoulders.Any())
                {
                    phases.Add(GetBoulderPhase(log, secondBoulders, "Boulders 2", decima));
                }
            }
        }
        return phases;
    }


    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        var lifespan = ((int)replay.TimeOffsets.start, (int)replay.TimeOffsets.end);
        switch (target.ID)
        {
            case (int)TargetID.Decima:
            case (int)TargetID.DecimaCM:
                var casts = target.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd).ToList();

                // Thrumming Presence - Red Ring around Decima
                var thrummingSegments = target.GetBuffStatus(log, ThrummingPresenceBuff, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0);
                foreach (var segment in thrummingSegments)
                {
                    replay.Decorations.Add(new CircleDecoration(700, segment.TimeSpan, Colors.Red, 0.2, new AgentConnector(target)).UsingFilled(false));
                }

                // Add the Charge indicator on top right of the replay
                var chargeSegments = target.GetBuffStatus(log, ChargeDecima, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0);
                foreach (Segment seg in chargeSegments)
                {
                    replay.Decorations.Add(new TextDecoration((seg.Start, seg.End), "Decima Charge(s) " + seg.Value + " out of 10", 15, Colors.Red, 1.0, new ScreenSpaceConnector(new Vector2(600, 60))));
                }

                // Mainshock - Pizza Indicator
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.DecimaMainshockIndicator, out var mainshockSlices))
                {
                    foreach (EffectEvent effect in mainshockSlices)
                    {
                        long duration = 2300;
                        long growing = effect.Time + duration;
                        (long start, long end) lifespan2 = effect.ComputeLifespan(log, duration);
                        var rotation = new AngleConnector(effect.Rotation.Z + 90);
                        var slice = (PieDecoration)new PieDecoration(1200, 32, lifespan2, Colors.LightOrange, 0.4, new PositionConnector(effect.Position)).UsingRotationConnector(rotation);
                        replay.Decorations.AddWithBorder(slice, Colors.LightOrange, 0.6);
                    }
                }

                // For some reason the effects all start at the same time
                // We sequence them using the skill cast
                var foreshock = casts.Where(x => x.SkillId == Foreshock);
                foreach (var cast in foreshock)
                {
                    (long start, long end) = (cast.Time, cast.Time + cast.ActualDuration + 3000); // 3s padding as safety
                    long nextStartTime = 0;

                    // Decima's Left Side
                    if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.DecimaForeshockLeft, out var foreshockLeft))
                    {
                        foreach (EffectEvent effect in foreshockLeft.Where(x => x.Time >= start && x.Time < end))
                        {
                            (long start, long end) lifespanLeft = effect.ComputeLifespan(log, 1967);
                            nextStartTime = lifespanLeft.end;
                            var rotation = new AngleConnector(effect.Rotation.Z + 90);
                            var leftHalf = (PieDecoration)new PieDecoration(1185, 180, lifespanLeft, Colors.LightOrange, 0.4, new PositionConnector(effect.Position)).UsingRotationConnector(rotation);
                            replay.Decorations.AddWithBorder(leftHalf, Colors.LightOrange, 0.6);
                        }
                    }

                    // Decima's Right Side
                    if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.DecimaForeshockRight, out var foreshockRight))
                    {
                        foreach (EffectEvent effect in foreshockRight.Where(x => x.Time >= start && x.Time < end))
                        {
                            (long start, long end) lifespanRight = effect.ComputeLifespan(log, 3000);
                            lifespanRight.start = nextStartTime - 700; // Trying to match in game timings
                            nextStartTime = lifespanRight.end;
                            var rotation = new AngleConnector(effect.Rotation.Z + 90);
                            var rightHalf = (PieDecoration)new PieDecoration(1185, 180, lifespanRight, Colors.LightOrange, 0.4, new PositionConnector(effect.Position)).UsingRotationConnector(rotation);
                            replay.Decorations.AddWithBorder(rightHalf, Colors.LightOrange, 0.6);
                        }
                    }

                    // Decima's Frontal
                    if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.DecimaForeshockFrontal, out var foreshockFrontal))
                    {
                        foreach (EffectEvent effect in foreshockFrontal.Where(x => x.Time >= start && x.Time < end))
                        {
                            (long start, long end) lifespanFrontal = effect.ComputeLifespan(log, 5100);
                            lifespanFrontal.start = nextStartTime;
                            var frontalCircle = new CircleDecoration(600, lifespanFrontal, Colors.LightOrange, 0.4, new PositionConnector(effect.Position));
                            replay.Decorations.AddWithBorder(frontalCircle, Colors.LightOrange, 0.6);
                        }
                    }
                }

                // Earthrend - Outer Sliced Doughnut - 8 Slices
                if (log.CombatData.TryGetGroupedEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.DecimaEarthrendDoughnutSlice, out var earthrend))
                {
                    // Since we don't have a decoration shaped like this, we regroup the 8 effects and use Decima position as the center for a doughnut sliced by lines.
                    foreach (List<EffectEvent> group in earthrend)
                    {
                        uint inner = 1200;
                        uint outer = 3000;
                        int lineAngle = 45;
                        var offset = new Vector3(0, inner + (outer - inner) / 2, 0);
                        (long start, long end) lifespanRing = group[0].ComputeLifespan(log, 2800);

                        if (target.TryGetCurrentFacingDirection(log, group[0].Time, out Vector3 facing, 100))
                        {
                            for (int i = 0; i < 360; i += lineAngle)
                            {
                                var rotation = facing.GetRoundedZRotationDeg() + i;
                                var line = new RectangleDecoration(10, outer - inner, lifespanRing, Colors.LightOrange, 0.6, new AgentConnector(target).WithOffset(offset, true)).UsingRotationConnector(new AngleConnector(rotation));
                                replay.Decorations.Add(line);
                            }
                        }

                        var doughnut = new DoughnutDecoration(inner, outer, lifespanRing, Colors.LightOrange, 0.2, new AgentConnector(target));
                        replay.Decorations.AddWithBorder(doughnut, Colors.LightOrange, 0.6);
                    }
                }

                // Seismic Crash - Jump with rings
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.DecimaSeismicCrashRings, out var seismicCrash))
                {
                    foreach (var effect in seismicCrash)
                    {
                        (long start, long end) lifespanCrash = effect.ComputeLifespan(log, 3000);
                        replay.Decorations.AddContrenticRings(300, 140, lifespanCrash, effect.Position, Colors.LightOrange, 0.30f, 6, false);
                    }
                }

                // Jump Death Zone
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.DecimaJumpAoEUnderneath, out var deathZone))
                {
                    foreach (var effect in deathZone)
                    {
                        // Logged effect has 2 durations depending on attack - 3000 and 2500
                        (long start, long end) lifespanDeathzone = effect.ComputeLifespan(log, effect.Duration);
                        var zone = new CircleDecoration(300, lifespanDeathzone, Colors.Red, 0.2, new PositionConnector(effect.Position));
                        replay.Decorations.AddWithGrowing(zone, effect.Time + effect.Duration);
                    }
                }

                // Aftershock - Moving AoEs - 4 Cascades 
                if (log.CombatData.TryGetGroupedEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.DecimaAftershockAoE, out var aftershock, 12000))
                {
                    // All the AoEs take roughly 11-12 seconds to appear
                    // There are 10 AoEs of radius 200, then 10 of 240, 10 of 280 and 10 of 320. When they bounce back to Decima they restart at 200 radius.
                    uint radius = 200;
                    float distance = 0;
                    EffectEvent first = aftershock.First().First();
                    long groupStartTime = first.Time;

                    // Because the x9th and the x0th can happen at the same timestamp, we need to check the distance of the from Decima.
                    // A simple increase every 10 can happen to increase the x9th instead of the following x0th.
                    if (target.TryGetCurrentPosition(log, first.Time, out Vector3 decimaPosition))
                    {
                        foreach (var group in aftershock)
                        {
                            foreach (var effect in group)
                            {
                                distance = (effect.Position - decimaPosition).XY().Length();
                                if (distance > 1074 && distance < 1076 || distance > 1759 && distance < 1761)
                                {
                                    radius = 200;
                                }
                                if (distance > 1324 && distance < 1326 || distance > 1528 && distance < 1530)
                                {
                                    radius = 240;
                                }
                                if (distance > 1574 && distance < 1576 || distance > 1297 && distance < 1299)
                                {
                                    radius = 280;
                                }
                                if (distance > 1824 && distance < 1826 || distance > 1066 && distance < 1068)
                                {
                                    radius = 320;
                                }
                                (long start, long end) lifespanAftershock = effect.ComputeLifespan(log, 1500);
                                var zone = (CircleDecoration)new CircleDecoration(radius, lifespanAftershock, Colors.Red, 0.2, new PositionConnector(effect.Position)).UsingFilled(false);
                                replay.Decorations.Add(zone);
                            }
                        }
                    }
                }
                break;
            case (int)TrashID.GreenOrb1Player:
            case (int)TrashID.GreenOrb1PlayerCM:
                // Green Circle
                replay.Decorations.Add(new CircleDecoration(90, lifespan, Colors.DarkGreen, 0.2, new AgentConnector(target)));

                // Overhead Number
                replay.Decorations.AddOverheadIcon(lifespan, target, ParserIcons.TargetOrder1Overhead);

                // Hp Bar
                var hpUpdatesOrb1 = target.GetHealthUpdates(log);
                replay.Decorations.Add(
                    new OverheadProgressBarDecoration(CombatReplayOverheadProgressBarMinorSizeInPixel, lifespan, Colors.SligthlyDarkGreen, 0.8, Colors.Black, 0.6, hpUpdatesOrb1.Select(x => (x.Start, x.Value)).ToList(), new AgentConnector(target))
                    .UsingInterpolationMethod(Connector.InterpolationMethod.Step)
                    .UsingRotationConnector(new AngleConnector(180))
                );
                break;
            case (int)TrashID.GreenOrb2Players:
            case (int)TrashID.GreenOrb2PlayersCM:
                // Green Circle
                replay.Decorations.Add(new CircleDecoration(185, lifespan, Colors.DarkGreen, 0.2, new AgentConnector(target)));

                // Overhead Number
                replay.Decorations.AddOverheadIcon(lifespan, target, ParserIcons.TargetOrder2Overhead);

                // Hp Bar
                var hpUpdatesOrb2 = target.GetHealthUpdates(log);
                replay.Decorations.Add(
                    new OverheadProgressBarDecoration(CombatReplayOverheadProgressBarMinorSizeInPixel, lifespan, Colors.SligthlyDarkGreen, 0.8, Colors.Black, 0.6, hpUpdatesOrb2.Select(x => (x.Start, x.Value)).ToList(), new AgentConnector(target))
                    .UsingInterpolationMethod(Connector.InterpolationMethod.Step)
                    .UsingRotationConnector(new AngleConnector(180))
                );
                break;
            case (int)TrashID.GreenOrb3Players:
            case (int)TrashID.GreenOrb3PlayersCM:
                // Green Circle
                replay.Decorations.Add(new CircleDecoration(285, lifespan, Colors.DarkGreen, 0.2, new AgentConnector(target)));

                // Overhead Number
                replay.Decorations.AddOverheadIcon(lifespan, target, ParserIcons.TargetOrder3Overhead);

                // Hp Bar
                var hpUpdatesOrb3 = target.GetHealthUpdates(log);
                replay.Decorations.Add(
                    new OverheadProgressBarDecoration(CombatReplayOverheadProgressBarMinorSizeInPixel, lifespan, Colors.SligthlyDarkGreen, 0.8, Colors.Black, 0.6, hpUpdatesOrb3.Select(x => (x.Start, x.Value)).ToList(), new AgentConnector(target))
                    .UsingInterpolationMethod(Connector.InterpolationMethod.Step)
                    .UsingRotationConnector(new AngleConnector(180))
                );
                break;
            case (int)TrashID.EnlightenedConduit:
            case (int)TrashID.EnlightenedConduitCM:
                // Chorus of Thunder / Discordant Thunder - Orange AoE
                AddThunderAoE(target, log, replay);
                // Focused Fluxlance - Green Arrow from Decima to the Conduit
                var greenArrow = GetFilteredList(log.CombatData, isCM ? FluxlanceTargetBuffCM1 : FluxlanceTargetBuff1, target, true, true).Where(x => x is BuffApplyEvent);
                foreach (var apply in greenArrow)
                {
                    replay.Decorations.Add(new LineDecoration((apply.Time, apply.Time + 5500), Colors.DarkGreen, 0.2, new AgentConnector(apply.To), new AgentConnector(apply.By)).WithThickess(80, true));
                    replay.Decorations.Add(new LineDecoration((apply.Time + 5500, apply.Time + 6500), Colors.DarkGreen, 0.5, new AgentConnector(apply.To), new AgentConnector(apply.By)).WithThickess(80, true));
                }

                // Warning indicator of walls spawning between Conduits.
                var wallsWarnings = GetFilteredList(log.CombatData, isCM ? DecimaConduitWallWarningBuffCM : DecimaConduitWallWarningBuff, target, true, true);
                replay.Decorations.AddTether(wallsWarnings, Colors.Red, 0.2, 30, true);

                // Walls connecting Conduits to each other.
                var walls = GetFilteredList(log.CombatData, isCM ? DecimaConduitWallBuffCM: DecimaConduitWallBuff, target, true, true);
                replay.Decorations.AddTether(walls, Colors.Purple, 0.4, 60, true);
                break;
            case (int)TrashID.EnlightenedConduitGadget:
                if (isCM && target.AgentItem.Master == null)
                {
                    break;
                }
                var gadgetConnectorAgent = (isCM ? target.AgentItem.Master : target.AgentItem)!;
                // Fulgent Aura - Tier 1 Charge
                var gadgetEffectConnector = new AgentConnector(gadgetConnectorAgent);
                var tier1 = target.GetBuffStatus(log, isCM ? EnlightenedConduitGadgetChargeTier1BuffCM :EnlightenedConduitGadgetChargeTier1Buff, log.FightData.FightStart, log.FightData.FightEnd);
                foreach (var segment in tier1.Where(x => x.Value > 0))
                {
                    replay.Decorations.AddWithBorder(new CircleDecoration(100, segment.TimeSpan, Colors.DarkPurple, 0.4, gadgetEffectConnector), Colors.Red, 0.4);
                    replay.Decorations.AddOverheadIcon(segment.TimeSpan, gadgetConnectorAgent, ParserIcons.TargetOrder1Overhead);
                }

                // Fulgent Aura - Tier 2 Charge
                var tier2 = target.GetBuffStatus(log, EnlightenedConduitGadgetChargeTier2Buff, log.FightData.FightStart, log.FightData.FightEnd);
                foreach (var segment in tier2.Where(x => x.Value > 0))
                {
                    replay.Decorations.AddWithBorder(new CircleDecoration(200, segment.TimeSpan, Colors.DarkPurple, 0.4, gadgetEffectConnector), Colors.Red, 0.4);
                    replay.Decorations.AddOverheadIcon(segment.TimeSpan, gadgetConnectorAgent, ParserIcons.TargetOrder2Overhead);
                }

                // Fulgent Aura - Tier 3 Charge
                var tier3 = target.GetBuffStatus(log, EnlightenedConduitGadgetChargeTier3Buff, log.FightData.FightStart, log.FightData.FightEnd);
                foreach (var segment in tier3.Where(x => x.Value > 0))
                {
                    replay.Decorations.AddWithBorder(new CircleDecoration(400, segment.TimeSpan, Colors.DarkPurple, 0.4, gadgetEffectConnector), Colors.Red, 0.4);
                    replay.Decorations.AddOverheadIcon(segment.TimeSpan, gadgetConnectorAgent, ParserIcons.TargetOrder3Overhead);
                }
                break;
            case (int)TrashID.DecimaBeamStart:
                const uint beamLength = 3900;
                const uint orangeBeamWidth = 80;
                const uint redBeamWidth = 160;
                var orangeBeams = GetFilteredList(log.CombatData, DecimaBeamTargeting, target.AgentItem, true, true);
                AddBeamWarning(log, target, replay, DecimaBeamLoading, orangeBeamWidth, beamLength, orangeBeams.OfType<BuffApplyEvent>(), Colors.LightOrange);
                AddBeam(log, replay, orangeBeamWidth, orangeBeams, Colors.LightOrange);
                var redBeams = GetFilteredList(log.CombatData, DecimaRedBeamTargeting, target.AgentItem, true, true);
                AddBeamWarning(log, target, replay, DecimaRedBeamLoading, redBeamWidth, beamLength, redBeams.OfType<BuffApplyEvent>(), Colors.Red);
                AddBeam(log, replay, redBeamWidth, redBeams, Colors.Red);
                break;
            default:
                break;
        }
    }

    private static void AddBeam(ParsedEvtcLog log, CombatReplay replay, uint beamWidth, IEnumerable<BuffEvent> beams, Color color)
    {
        int tetherStart = 0;
        AgentItem src = _unknownAgent;
        AgentItem dst = _unknownAgent;
        foreach (BuffEvent tether in beams)
        {
            if (tether is BuffApplyEvent)
            {
                tetherStart = (int)tether.Time;
                src = tether.By;
                dst = tether.To;
            }
            else if (tether is BuffRemoveAllEvent)
            {
                int tetherEnd = (int)tether.Time;
                if (!src.IsUnknown && !dst.IsUnknown)
                {
                    if (src.TryGetCurrentInterpolatedPosition(log, tetherStart, out var posSrc))
                    {
                        // Get the position before movement happened
                        if (dst.TryGetCurrentInterpolatedPosition(log, tetherStart - 500, out var posDst))
                        {
                            replay.Decorations.Add(new LineDecoration((tetherStart, tetherEnd), color, 0.5, new PositionConnector(posSrc), new PositionConnector(posDst)).WithThickess(beamWidth, true));
                        }
                        src = _unknownAgent;
                        dst = _unknownAgent;
                    }
                }
            }
        }
    }

    private static void AddBeamWarning(ParsedEvtcLog log, SingleActor target, CombatReplay replay, long buffID, uint beamWidth, uint beamLength, IEnumerable<BuffApplyEvent> beamFireds, Color color)
    {
        var beamWarnings = target.AgentItem.GetBuffStatus(log, buffID, log.FightData.FightStart, log.FightData.FightEnd);
        foreach (var beamWarning in beamWarnings)
        {
            if (beamWarning.Value > 0)
            {
                long start = beamWarning.Start;
                long end = beamFireds.FirstOrDefault(x => x.Time >= start)?.Time ?? beamWarning.End;
                // We ignore the movement of the agent, it moves closer to target before firing
                if (target.TryGetCurrentInterpolatedPosition(log, start, out var posDst))
                {
                    var connector = new PositionConnector(posDst).WithOffset(new(beamLength / 2, 0, 0), true);
                    var rotationConnector = new AgentFacingConnector(target);
                    replay.Decorations.Add(new RectangleDecoration(beamLength, beamWidth, (start, end), color, 0.2, connector).UsingRotationConnector(rotationConnector));
                }
            }
        }
    }


    internal override void ComputePlayerCombatReplayActors(PlayerActor player, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputePlayerCombatReplayActors(player, log, replay);

        // Target Overhead
        // In phase 2 you get the Fluxlance Target Buff but also Target Order, in game only Target Order is displayed overhead, so we filter those out.
        var p2Targets = player.GetBuffStatus(log, [TargetOrder1JW, TargetOrder2JW, TargetOrder3JW, TargetOrder4JW, TargetOrder5JW], log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0);
        var allTargets = player.GetBuffStatus(log, FluxlanceTargetBuff1, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0);
        var filtered = allTargets.Where(x => !p2Targets.Any(y => Math.Abs(x.Start - y.Start) < ServerDelayConstant));
        foreach (var segment in filtered)
        {
            replay.Decorations.AddOverheadIcon(segment, player, ParserIcons.TargetOverhead);
        }

        // Target Order Overhead
        replay.Decorations.AddOverheadIcons(player.GetBuffStatus(log, TargetOrder1JW, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0), player, ParserIcons.TargetOrder1Overhead);
        replay.Decorations.AddOverheadIcons(player.GetBuffStatus(log, TargetOrder2JW, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0), player, ParserIcons.TargetOrder2Overhead);
        replay.Decorations.AddOverheadIcons(player.GetBuffStatus(log, TargetOrder3JW, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0), player, ParserIcons.TargetOrder3Overhead);
        replay.Decorations.AddOverheadIcons(player.GetBuffStatus(log, TargetOrder4JW, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0), player, ParserIcons.TargetOrder4Overhead);
        replay.Decorations.AddOverheadIcons(player.GetBuffStatus(log, TargetOrder5JW, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0), player, ParserIcons.TargetOrder5Overhead);

        // Chorus of Thunder / Discordant Thunder - Orange AoE
        AddThunderAoE(player, log, replay);
    }

    /// <summary>
    /// Chorus of Thunder / Discordant Thunder - Orange spread AoE on players or on Conduits.
    /// </summary>
    private static void AddThunderAoE(SingleActor actor, ParsedEvtcLog log, CombatReplay replay)
    {
        if (log.CombatData.TryGetEffectEventsByDstWithGUID(actor.AgentItem, EffectGUIDs.DecimaChorusOfThunderAoE, out var thunders))
        {
            foreach (var effect in thunders)
            {
                long duration = 5000;
                long growing = effect.Time + duration;
                (long start, long end) lifespan = effect.ComputeLifespan(log, duration);
                replay.Decorations.AddWithGrowing(new CircleDecoration(285, lifespan, Colors.LightOrange, 0.2, new AgentConnector(actor)), growing);
            }
        }
    }

    internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
    {
        return isCM ? FightData.EncounterMode.CMNoName : FightData.EncounterMode.Normal;
    }
}
