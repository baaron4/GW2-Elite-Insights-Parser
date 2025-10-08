using System;
using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal class Adina : TheKeyOfAhdashim
{
    internal readonly MechanicGroup Mechanics = new MechanicGroup([
            new MechanicGroup([
                new PlayerDstBuffApplyMechanic(RadiantBlindness, new MechanicPlotlySetting(Symbols.Circle,Colors.Magenta), "R.Blind", "Unremovable blindness", "Radiant Blindness", 0),
                new PlayerDstHealthDamageHitMechanic(DiamondPalisadeEye, new MechanicPlotlySetting(Symbols.StarDiamond,Colors.Pink), "Eye", "Looked at Eye", "Looked at Eye", 0),
            ]),
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(PerilousPulse, new MechanicPlotlySetting(Symbols.TriangleRight,Colors.Pink), "Perilous Pulse", "Perilous Pulse", "Perilous Pulse", 0)
                    .UsingBuffChecker(Stability, false),
                new PlayerDstHealthDamageHitMechanic(StalagmitesDetonation, new MechanicPlotlySetting(Symbols.Pentagon,Colors.Red), "Mines", "Hit by mines", "Mines", 0),
                new PlayerDstHealthDamageMechanic([DoubleRotatingEarthRays, TripleRotatingEarthRays], new MechanicPlotlySetting(Symbols.Hourglass,Colors.Brown), "S.Thrower", "Hit by rotating SandThrower", "SandThrower", 0).UsingChecker((de, log) => de.HasKilled),
            ]),
            new MechanicGroup([
                new PlayerDstEffectMechanic(EffectGUIDs.AdinaSelectedForPillar,new MechanicPlotlySetting(Symbols.Circle,Colors.Brown), "Slctd.Pillar", "Selected for dropping a Pillar", "Selected for Pillar", 0)
            ]),
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(BoulderBarrage, new MechanicPlotlySetting(Symbols.Hexagon,Colors.Red), "Boulder", "Hit by boulder thrown during pillars", "Boulder Barrage", 0),
                new PlayerDstBuffApplyMechanic(ErodingCurse, new MechanicPlotlySetting(Symbols.Square,Colors.LightPurple), "Curse", "Stacking damage debuff from Hand of Erosion", "Eroding Curse", 0),
            ]),
        ]);
    public Adina(int triggerID) : base(triggerID)
    {
        MechanicList.Add(Mechanics);
        Extension = "adina";
        Icon = EncounterIconAdina;
        ChestID = ChestID.AdinasChest;
        LogCategoryInformation.InSubCategoryOrder = 0;
        LogID |= 0x000001;
    }

    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        return
        [
            new DamageCastFinder(SeismicSuffering, SeismicSuffering), // Seismic Suffering
        ];
    }

    internal override LogLogic AdjustLogic(AgentData agentData, List<CombatItem> combatData, EvtcParserSettings parserSettings)
    {
        CombatItem? logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.LogNPCUpdate);
        // Handle potentially wrongly associated logs
        if (logStartNPCUpdate != null)
        {
            if (agentData.GetNPCsByID(TargetID.Sabir).Any(sabir => combatData.Any(evt => evt.IsDamagingDamage() && evt.DstMatchesAgent(sabir) && agentData.GetAgent(evt.SrcAgent, evt.Time).GetFinalMaster().IsPlayer)))
            {
                return new Sabir((int)TargetID.Sabir);
            }
        }
        return base.AdjustLogic(agentData, combatData, parserSettings);
    }

    // note: these are the attack target not gadget locations
    static readonly List<(string, Vector2)> HandLocations =
    [
        ("NW", new(14359.6f, -789.288f)), // erosion
        ("NE", new(15502.5f, -841.978f)), // eruption
        ("SW", new(14316.6f, -2080.17f)), // eruption
        ("SE", new(15478.0f, -2156.67f)), // erosion
    ];
    protected override HashSet<int> IgnoreForAutoNumericalRenaming()
    {
        return [
            (int)TargetID.HandOfErosion,
            (int)TargetID.HandOfEruption,
        ];
    }

    internal static void FindHands(LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        var attackTargetEvents = combatData
            .Where(x => x.IsStateChange == StateChange.AttackTarget)
            .Select(x => new AttackTargetEvent(x, agentData));
        var targetableEvents = new Dictionary<AgentItem, IEnumerable<TargetableEvent>>();
        foreach (var attackTarget in attackTargetEvents)
        {
            targetableEvents[attackTarget.AttackTarget] = attackTarget.GetTargetableEvents(combatData, agentData);
        }
        attackTargetEvents = attackTargetEvents.Where(x =>
        {
            AgentItem atAgent = x.AttackTarget;
            if (targetableEvents.TryGetValue(atAgent, out var targetables))
            {
                return targetables.Any(y => y.Targetable);
            }
            return false;
        }).ToList();
        var handOfEruptionPositions = new Vector2[] { new(15570.5f, -693.117f), new(14277.2f, -2202.52f) }; // gadget locations
        var processedAttackTargets = new HashSet<AgentItem>();
        foreach (AttackTargetEvent attackTargetEvent in attackTargetEvents)
        {
            AgentItem atAgent = attackTargetEvent.AttackTarget;
            if (processedAttackTargets.Contains(atAgent) || !targetableEvents.TryGetValue(atAgent, out var targetables))
            {
                continue;
            }

            processedAttackTargets.Add(atAgent);
            AgentItem hand = attackTargetEvent.Src;
            var copyEventsFrom = new List<AgentItem>() { hand };
            var attackOns = targetables.Where(x => x.Targetable);
            var attackOffs = targetables.Where(x => !x.Targetable);
            CombatItem? posEvt = combatData.FirstOrDefault(x => x.SrcMatchesAgent(hand) && x.IsStateChange == StateChange.Position);
            var id = TargetID.HandOfErosion;
            if (posEvt != null)
            {
                var pos = MovementEvent.GetPoint3D(posEvt);
                if (handOfEruptionPositions.Any(x => (x - pos.XY()).Length() < InchDistanceThreshold))
                {
                    id = TargetID.HandOfEruption;
                }
            }

            foreach (TargetableEvent targetableEvent in attackOns)
            {
                long start = targetableEvent.Time;
                long end = logData.LogEnd;
                TargetableEvent? attackOff = attackOffs.FirstOrDefault(x => x.Time > start);
                if (attackOff != null)
                {
                    end = attackOff.Time;
                }
                AgentItem extra = agentData.AddCustomNPCAgent(start, end, hand.Name, hand.Spec, id, false, hand.Toughness, hand.Healing, hand.Condition, hand.Concentration, hand.HitboxWidth, hand.HitboxHeight);
                AgentManipulationHelper.RedirectNPCEventsAndCopyPreviousStates(combatData, extensions, agentData, hand, copyEventsFrom, extra, true);
            }
        }
    }

    internal static void RenameHands(IReadOnlyList<SingleActor> targets, List<CombatItem> combatData)
    {
        var nameCount = new Dictionary<string, int> { { "NE", 1 }, { "NW", 1 }, { "SW", 1 }, { "SE", 1 } };
        foreach (SingleActor target in targets)
        {
            if (target.IsAnySpecies(new[] { TargetID.HandOfErosion, TargetID.HandOfEruption }))
            {
                string? suffix = AddNameSuffixBasedOnInitialPosition(target, combatData, HandLocations);
                if (suffix != null && nameCount.ContainsKey(suffix))
                {
                    // deduplicate name
                    target.OverrideName(target.Character + " " + (nameCount[suffix]++));
                }
            }
        }
    }

    internal static void FindPlatforms(AgentData agentData)
    {
        foreach (var agent in agentData.GetAgentByType(AgentItem.AgentType.Gadget))
        {
            if (agent.IsUnamedSpecies() && (agent.HitboxWidth == 170 || agent.HitboxWidth == 232) && agent.HitboxHeight == 2)
            {
                agent.OverrideID(TargetID.AdinaPlateform, agentData);
            }
        }
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        FindPlatforms(agentData);
        FindHands(logData, agentData, combatData, extensions);
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);
        RenameHands(Targets, combatData);
    }

    internal override IReadOnlyList<TargetID>  GetTargetsIDs()
    {
        return
        [
            TargetID.Adina,
            TargetID.HandOfErosion,
            TargetID.HandOfEruption
        ];
    }

    internal override Dictionary<TargetID, int> GetTargetsSortIDs()
    {
        return new Dictionary<TargetID, int>()
        {
            { TargetID.Adina, 0 },
            { TargetID.HandOfErosion, 1 },
            { TargetID.HandOfEruption, 1 },
        };
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IsInstance)
        {
            base.ComputePlayerCombatReplayActors(p, log, replay);
        }
        var radiantBlindnesses = p.GetBuffStatus(log, RadiantBlindness).Where(x => x.Value > 0);
        replay.Decorations.AddOverheadIcons(radiantBlindnesses, p, BuffImages.PersistentlyBlinded);
        if (log.CombatData.TryGetEffectEventsByDstWithGUID(p.AgentItem, EffectGUIDs.AdinaSelectedForPillar, out var selectedForPillars))
        {
            var connector = new AgentConnector(p);
            foreach (var selectedForPillar in selectedForPillars)
            {
                var lifespan = selectedForPillar.ComputeLifespan(log, 6000);
                var circle = new CircleDecoration(60, lifespan, Colors.Orange, 0.3, connector);
                replay.Decorations.AddWithGrowing(circle, lifespan.end);
            }
        }
    }

    private static void ComputePillarLifecycle(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        var candidateEndEvents = new List<EffectEvent>();
        if (log.CombatData.TryGetEffectEventsByGUIDs([EffectGUIDs.AdinaGroundRetracted0ms, EffectGUIDs.AdinaPillarDestroyedByProjectiles0ms, EffectGUIDs.AdinaPillarDestroyedByAdina], out var destroyeds))
        {
            candidateEndEvents = destroyeds.OrderBy(x => x.Time).ToList();
        }
        var startEvents = new List<EffectEvent>();
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.AdinaPillarShockwave, out var pillarShockwaves))
        {
            startEvents = pillarShockwaves.ToList();
            foreach (var pillarShockwave in pillarShockwaves)
            {
                long start = pillarShockwave.Time;
                var currentAdina = log.AgentData.GetNPCsByID(TargetID.Adina).FirstOrDefault(x => x.InAwareTimes(start));
                if (currentAdina == null)
                {
                    continue;
                }
                var connector = new PositionConnector(pillarShockwave.Position);
                long end = currentAdina.LastAware;
                var potentialEndEvent = candidateEndEvents.FirstOrDefault(x => x.Time >= start - ServerDelayConstant && (x.Position - pillarShockwave.Position).XY().Length() < 10);
                if (potentialEndEvent != null)
                {
                    end = potentialEndEvent.Time;
                }
                environmentDecorations.Add(new PolygonDecoration(60, 6, (start, end), Colors.DarkBrown, 0.7, connector));
            }
        }
        if (log.CombatData.TryGetEffectEventsByGUIDs([EffectGUIDs.AdinaPillarDestroyedByProjectiles0ms, EffectGUIDs.AdinaPillarDestroyedByAdina], out var explicitelyDestroyed))
        {
            foreach ( var destroyed in explicitelyDestroyed)
            {
                long end = destroyed.Time;
                var currentAdina = log.AgentData.GetNPCsByID(TargetID.Adina).FirstOrDefault(x => x.InAwareTimes(end));
                if (currentAdina == null)
                {
                    continue;
                }
                var connector = new PositionConnector(destroyed.Position);
                long start = currentAdina.FirstAware;
                var potentialDropEvent = startEvents.LastOrDefault(x => x.Time < end + ServerDelayConstant && (x.Position - destroyed.Position).XY().Length() < 10);
                if (potentialDropEvent != null)
                {
                    // already while iterating shockwave
                    continue;
                }
                environmentDecorations.Add(new PolygonDecoration(60, 6, (start, end), Colors.DarkBrown, 0.7, connector));
            }
        }
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        if (!log.LogData.IsInstance)
        {
            base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);
        }
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.AdinaPillarDropLocationWarning, out var pillarDrops))
        {
            foreach (var pillarDrop in pillarDrops)
            {
                var connector = new PositionConnector(pillarDrop.Position);
                // effect has a 6000ms duration but drop happens 2000ms after via dynamic end time, use 2000ms as default for pre proper duration logs
                var lifespan = pillarDrop.ComputeLifespan(log, 2000);
                var circle = new CircleDecoration(80, lifespan, Colors.Orange, 0.4, connector);
                environmentDecorations.AddWithGrowing(circle, lifespan.end);
            }
        }
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.AdinaPillarShockwave, out var pillarShockwaves))
        {
            foreach (var pillarShockwave in pillarShockwaves)
            {
                var connector = new PositionConnector(pillarShockwave.Position);
                environmentDecorations.AddShockwave(connector, pillarShockwave.ComputeLifespan(log, 1333), Colors.DarkBrown, 0.7, 220);
            }
        }
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.AdinaGroundRetractedWarning, out var groundRetractedWarnings))
        {
            foreach (var groundRetractedWarning in groundRetractedWarnings)
            {
                var effectLifespan = groundRetractedWarning.ComputeLifespan(log, 4271);
                environmentDecorations.AddWithFilledWithGrowing(new PolygonDecoration(60, 6, effectLifespan, Colors.Brown, 0.3, new PositionConnector(groundRetractedWarning.Position)), true, effectLifespan.end);
            }
        }
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.AdinaGroundRetracted, out var groundRetracteds))
        {
            foreach (var groundRetracted in groundRetracteds)
            {
                var effectLifespan = groundRetracted.ComputeLifespan(log, 1000);
                environmentDecorations.Add(new PolygonDecoration(60, 6, effectLifespan, Colors.Brown, 0.6, new PositionConnector(groundRetracted.Position)));
            }
        }
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.AdinaMineWarning2, out var mineWarnings))
        {
            foreach (var mineWarning in mineWarnings)
            {
                var effectLifespan = mineWarning.ComputeLifespan(log, 3000);
                environmentDecorations.AddWithFilledWithGrowing(new PolygonDecoration(60, 6, effectLifespan, Colors.Orange, 0.3, new PositionConnector(mineWarning.Position)), true, effectLifespan.end);
            }
        }
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.AdinaMine, out var mines))
        {
            foreach (var mine in mines)
            {
                var effectLifespan = mine.ComputeDynamicLifespan(log, 0);
                environmentDecorations.Add(new PolygonDecoration(60, 6, effectLifespan, Colors.Orange, 0.6, new PositionConnector(mine.Position)));
            }
        }
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.AdinaMineExplosion, out var mineExplosions))
        {
            foreach (var mineExplosion in mineExplosions)
            {
                var effectLifespan = (mineExplosion.Time, mineExplosion.Time + 100);
                environmentDecorations.Add(new PolygonDecoration(60, 6, effectLifespan, Colors.DarkRed, 0.6, new PositionConnector(mineExplosion.Position)));
            }
        }
        /*if (log.CombatData.TryGetEffectEventsByGUIDs([EffectGUIDs.AdinaPillarDestroyedByProjectiles, EffectGUIDs.AdinaPillarDestroyedByAdina], out var pillarsDestroyed))
        {
            foreach (var pillarDestroyed in pillarsDestroyed)
            {
                var effectLifespan = pillarDestroyed.ComputeLifespan(log, 1000);
                environmentDecorations.Add(new CircleDecoration(90, 6, effectLifespan, Colors.DarkBrown, 0.6, new PositionConnector(pillarDestroyed.Position)));
            }
        }*/
        ComputePillarLifecycle(log, environmentDecorations);
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IsInstance)
        {
            base.ComputeNPCCombatReplayActors(target, log, replay);
        }
        long castDuration;
        (long start, long end) lifespan;

        switch (target.ID)
        {
            case (int)TargetID.Adina:
                var adinaCasts = target.GetAnimatedCastEvents(log);
                foreach (CastEvent cast in adinaCasts)
                {
                    switch (cast.SkillID)
                    {
                        // Quantum Quakes - Double Rotating Earth Rays (Normal Mode)
                        case DoubleRotatingEarthRays:
                            AddQuantumQuakesDecoration(replay, target, cast, [90, 270]);
                            break;
                        // Quantum Quakes - Triple Rotating Earth Rays (Challenge Mode)
                        case TripleRotatingEarthRays:
                            AddQuantumQuakesDecoration(replay, target, cast, [30, 150, 270]);
                            break;
                        // Terraform - Phase shockwave
                        case Terraform:
                            long delay = 2000; // casttime 0 from skill def
                            castDuration = 5000;
                            uint radius = 1100;
                            lifespan = (cast.Time + delay, cast.Time + castDuration);
                            GeographicalConnector connector = new AgentConnector(target);
                            replay.Decorations.AddShockwave(connector, lifespan, Colors.LightOrange, 0.6, radius);
                            break;
                        // Boulder Barrage
                        case BoulderBarrage:
                            castDuration = 4600; // cycle 3 from skill def
                            lifespan = (cast.Time, cast.Time + castDuration);
                            var growingEnd = lifespan.end;
                            var interruptEvent = adinaCasts.FirstOrDefault(x =>x.Time <= lifespan.end);
                            if (interruptEvent != null)
                            {
                                lifespan.end = interruptEvent.Time;
                            }
                            replay.Decorations.Add(new CircleDecoration(1100, lifespan, Colors.LightOrange, 0.4, new AgentConnector(target)).UsingGrowingEnd(growingEnd));
                            break;
                        default:
                            break;
                    }
                }

                // Diamond Palisade
                var diamondPalisades = target.GetBuffStatus(log, DiamondPalisade).Where(x => x.Value > 0);
                foreach (Segment seg in diamondPalisades)
                {
                    replay.Decorations.Add(new CircleDecoration(90, seg, Colors.Red, 0.2, new AgentConnector(target)));
                    replay.Decorations.AddOverheadIcon(seg, target, SkillImages.MonsterSkill);
                }

                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.AdinaSweep, out var sweeps))
                {
                    foreach (var sweep in sweeps)
                    {
                        var sweepLifespan = sweep.ComputeLifespan(log, 450);
                        replay.Decorations.Add(new PolygonDecoration(60, 6, sweepLifespan, Colors.Red, 0.2, new PositionConnector(sweep.Position)));
                    }
                }
                var boulderBarrages = log.CombatData.GetMissileEventsBySrcBySkillID(target.AgentItem, BoulderBarrage);
                replay.Decorations.AddNonHomingMissiles(log, boulderBarrages, Colors.Red, 0.4, 30);
                break;
            default:
                break;
        }
    }

    internal static readonly List<TargetID> HandIDs = [ TargetID.HandOfErosion, TargetID.HandOfEruption];

    internal static List<PhaseData> ComputePhases(ParsedEvtcLog log, SingleActor adina, IReadOnlyList<SingleActor> targets, EncounterPhaseData encounterPhase, bool requirePhases)
    {
        if (!requirePhases)
        {
            return [];
        }
        var invuls = adina.GetBuffStatus(log, Determined762);
        var phases = new List<PhaseData>(7);
        // Split phases
        var splitPhases = new List<PhaseData>();
        var splitPhaseEnds = new List<long>();
        for (int i = 0; i < invuls.Count; i++)
        {
            var invul = invuls[i];
            if (invul.Value > 0)
            {
                var splitPhase = new SubPhasePhaseData(invul.Start, invul.End, "Split " + (i / 2 + 1));
                splitPhase.AddParentPhase(encounterPhase);
                splitPhaseEnds.Add(invul.End);
                AddTargetsToPhaseAndFit(splitPhase, targets, HandIDs, log);
                splitPhases.Add(splitPhase);
            }
        }
        // Main phases
        var mainPhases = new List<PhaseData>();
        var pillarApplies = log.CombatData.GetBuffApplyDataByIDByDst(PillarPandemonium, adina.AgentItem).OfType<BuffApplyEvent>();
        Dictionary<long, List<BuffApplyEvent>> pillarAppliesGroupByTime = GroupByTime(pillarApplies);
        var mainPhaseEnds = new List<long>();
        foreach (KeyValuePair<long, List<BuffApplyEvent>> pair in pillarAppliesGroupByTime)
        {
            if (pair.Value.Count == 6)
            {
                mainPhaseEnds.Add(pair.Key);
            }
        }
        CastEvent? boulderBarrage = adina.GetCastEvents(log).FirstOrDefault(x => x.SkillID == BoulderBarrage && x.Time < encounterPhase.Start + 6000);
        long start = boulderBarrage == null ? encounterPhase.Start : boulderBarrage.EndTime;
        if (mainPhaseEnds.Count != 0)
        {
            int phaseIndex = 1;
            foreach (long quantumQake in mainPhaseEnds)
            {
                long curPhaseStart = splitPhaseEnds.LastOrDefault(x => x < quantumQake);
                if (curPhaseStart == 0)
                {
                    curPhaseStart = start;
                }
                long nextPhaseStart = splitPhaseEnds.FirstOrDefault(x => x > quantumQake);
                if (nextPhaseStart != 0)
                {
                    start = nextPhaseStart;
                    phaseIndex = splitPhaseEnds.IndexOf(start) + 1;
                }
                mainPhases.Add(new SubPhasePhaseData(curPhaseStart, quantumQake, "Phase " + phaseIndex));
            }
            if (start != mainPhases.Last().Start)
            {
                mainPhases.Add(new SubPhasePhaseData(start, encounterPhase.End, "Phase " + (phaseIndex + 1)));
            }
        }
        else if (start > 0 && invuls.Count == 0)
        {
            // no split
            mainPhases.Add(new SubPhasePhaseData(start, encounterPhase.End, "Phase 1"));
        }

        foreach (PhaseData phase in mainPhases)
        {
            phase.AddTarget(adina, log);
            phase.AddParentPhase(encounterPhase);
            phase.AddTargets(targets.Where(x => x.IsAnySpecies(HandIDs) && phase.InInterval(x.FirstAware)), log, PhaseData.TargetPriority.NonBlocking);
        }
        phases.AddRange(mainPhases);
        phases.AddRange(splitPhases);
        phases.Sort((x, y) => x.Start.CompareTo(y.Start));
        return phases;
    }
    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor adina = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Adina)) ?? throw new MissingKeyActorsException("Adina not found");
        phases[0].AddTarget(adina, log);
        var lastInvuln = adina.GetBuffStatus(log, Determined762).LastOrNull();
        long lastBossPhaseStart = lastInvuln != null && lastInvuln.Value.Value == 0 ? lastInvuln.Value.Start : log.LogData.EvtcLogEnd; // if log ends with any boss phase, ignore hands after that point
        phases[0].AddTargets(Targets.Where(x => x.IsAnySpecies(HandIDs) && x.FirstAware < lastBossPhaseStart), log, PhaseData.TargetPriority.Blocking);
        phases.AddRange(ComputePhases(log, adina, Targets, (EncounterPhaseData)phases[0], requirePhases));
        return phases;
    }

    internal override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log, CombatReplayDecorationContainer arenaDecorations)
    {
        string mainPhase1;
        if (log.CombatData.TryGetEffectEventsByGUIDs([EffectGUIDs.AdinaPillarDestroyedByProjectiles, EffectGUIDs.AdinaPillarDestroyedByAdina], out _))
        {
            mainPhase1 = CombatReplayAdinaMainPhase1NoPillars;
        }      
        else
        {
            mainPhase1 = CombatReplayAdinaMainPhase1;
        }
        var crMap = new CombatReplayMap(
                        (866, 1000),
                        (13860, -2678, 15951, -268));
        //
        try
        {
            var allPhases = log.LogData.GetPhases(log);
            var adinaPhases = log.LogData.GetPhases(log).OfType<EncounterPhaseData>().Where(x => x.LogID == LogID);
            var splitPhasesMap = new List<string>()
            {
                    CombatReplayAdinaSplitPhase1,
                    CombatReplayAdinaSplitPhase2,
                    CombatReplayAdinaSplitPhase3,
            };
            var mainPhasesMap = new List<string>()
            {
                    mainPhase1,
                    CombatReplayAdinaMainPhase2,
                    CombatReplayAdinaMainPhase3,
                    CombatReplayAdinaMainPhase4
            };
            var subPhases = allPhases.OfType<SubPhasePhaseData>().Where(x => !x.BreakbarPhase);
            long start = log.LogData.LogStart;
            foreach (var adinaPhase in adinaPhases)
            {
                var crMaps = new List<string>();
                int mainPhaseIndex = 0;
                int splitPhaseIndex = 0;
                var phases = subPhases.Where(x => x.EncounterPhase == adinaPhase).ToList();
                var mainPhases = phases.Where(x => x.Name.Contains("Phase"));
                for (int i = 0; i < phases.Count; i++)
                {
                    PhaseData phaseData = phases[i];
                    long end = phaseData.End;
                    if (i < phases.Count - 1)
                    {
                        end = phases[i + 1].Start;
                    }
                    if (mainPhases.Contains(phaseData))
                    {
                        if (mainPhasesMap.Contains(crMaps.LastOrDefault()!))
                        {
                            splitPhaseIndex++;
                        }
                        var url = mainPhasesMap[mainPhaseIndex++];
                        arenaDecorations.Add(new ArenaDecoration((start, end), url, crMap));
                        crMaps.Add(url);
                    }
                    else
                    {
                        if (splitPhasesMap.Contains(crMaps.LastOrDefault()!))
                        {
                            mainPhaseIndex++;
                        }
                        var url = splitPhasesMap[splitPhaseIndex++];
                        arenaDecorations.Add(new ArenaDecoration((start, end), url, crMap));
                        crMaps.Add(url);
                    }
                    start = end;
                }
            }
            if (!adinaPhases.Any())
            {
                arenaDecorations.Add(new ArenaDecoration((log.LogData.LogStart, log.LogData.LogEnd), mainPhase1, crMap));
            } 
            else
            {
                arenaDecorations.Add(new ArenaDecoration((start, log.LogData.LogEnd), mainPhase1, crMap));
            }

        }
        catch (Exception)
        {
            log.UpdateProgressWithCancellationCheck("Parsing: Failed to associate Adina Combat Replay maps");
        }
        //
        return crMap;
    }

    internal override LogData.LogMode GetLogMode(CombatData combatData, AgentData agentData, LogData logData)
    {
        SingleActor target = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Adina)) ?? throw new MissingKeyActorsException("Adina not found");
        return (target.GetHealth(combatData) > 23e6) ? LogData.LogMode.CM : LogData.LogMode.Normal;
    }

    internal override void SetInstanceBuffs(ParsedEvtcLog log, List<InstanceBuff> instanceBuffs)
    {
        if (!log.LogData.IsInstance)
        {
            base.SetInstanceBuffs(log, instanceBuffs);
        }
        if (log.CombatData.GetBuffData(AchievementEligibilityConserveTheLand).Any())
        {
            var encounterPhases = log.LogData.GetPhases(log).OfType<EncounterPhaseData>().Where(x => x.LogID == LogID);
            foreach (var encounterPhase in encounterPhases)
            {
                if (encounterPhase.Success)
                {
                    instanceBuffs.MaybeAdd(GetOnPlayerCustomInstanceBuff(log, encounterPhase, AchievementEligibilityConserveTheLand));
                }
            }
        }
    }

    private static void AddQuantumQuakesDecoration(CombatReplay replay, NPC target, CastEvent cast, List<int> angles)
    {
        long preCastTime = 2990;
        long duration = 13400;
        uint width = 1100;
        uint height = 60;
        (long start, long end) lifespanWarning = (cast.Time, cast.Time + preCastTime);
        (long start, long end) lifespanRotating = (cast.Time + preCastTime, cast.Time + duration);
        foreach (int angle in angles)
        {
            var positionConnector = (AgentConnector)new AgentConnector(target).WithOffset(new(width / 2, 0, 0), true);
            replay.Decorations.Add(new RectangleDecoration(width, height, lifespanWarning, Colors.Orange, 0.2, positionConnector).UsingRotationConnector(new AngleConnector(angle)));
            replay.Decorations.Add(new RectangleDecoration(width, height, lifespanRotating, Colors.Red, 0.5, positionConnector).UsingRotationConnector(new SpinningConnector(angle, 360)));
        }
    }
}
