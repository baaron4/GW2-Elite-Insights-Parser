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

internal class Adina : TheKeyOfAhdashim
{
    internal readonly MechanicGroup Mechanics = new MechanicGroup([
            new PlayerDstBuffApplyMechanic(RadiantBlindness, new MechanicPlotlySetting(Symbols.Circle,Colors.Magenta), "R.Blind", "Unremovable blindness", "Radiant Blindness", 0),
            new PlayerDstBuffApplyMechanic(ErodingCurse, new MechanicPlotlySetting(Symbols.Square,Colors.LightPurple), "Curse", "Stacking damage debuff from Hand of Erosion", "Eroding Curse", 0),
            new PlayerDstHitMechanic(BoulderBarrage, new MechanicPlotlySetting(Symbols.Hexagon,Colors.Red), "Boulder", "Hit by boulder thrown during pillars", "Boulder Barrage", 0),
            new PlayerDstHitMechanic(PerilousPulse, new MechanicPlotlySetting(Symbols.TriangleRight,Colors.Pink), "Perilous Pulse", "Perilous Pulse", "Perilous Pulse", 0)
                .UsingChecker( (de, log) => !de.To.HasBuff(log, Stability, de.Time - ServerDelayConstant)),
            new PlayerDstHitMechanic(StalagmitesDetonation, new MechanicPlotlySetting(Symbols.Pentagon,Colors.Red), "Mines", "Hit by mines", "Mines", 0),
            new PlayerDstHitMechanic(DiamondPalisadeEye, new MechanicPlotlySetting(Symbols.StarDiamond,Colors.Pink), "Eye", "Looked at Eye", "Looked at Eye", 0),
            new PlayerDstSkillMechanic([DoubleRotatingEarthRays, TripleRotatingEarthRays], new MechanicPlotlySetting(Symbols.Hourglass,Colors.Brown), "S.Thrower", "Hit by rotating SandThrower", "SandThrower", 0).UsingChecker((de, log) => de.HasKilled),
        ]);
    public Adina(int triggerID) : base(triggerID)
    {
        MechanicList.Add(Mechanics);
        Extension = "adina";
        Icon = EncounterIconAdina;
        ChestID = ChestID.AdinasChest;
        EncounterCategoryInformation.InSubCategoryOrder = 0;
        EncounterID |= 0x000001;
    }

    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        return
        [
            new DamageCastFinder(SeismicSuffering, SeismicSuffering), // Seismic Suffering
        ];
    }

    internal override FightLogic AdjustLogic(AgentData agentData, List<CombatItem> combatData, EvtcParserSettings parserSettings)
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

    internal static void FindHands(FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
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
        long final = fightData.FightEnd;
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
                long end = final;
                TargetableEvent? attackOff = attackOffs.FirstOrDefault(x => x.Time > start);
                if (attackOff != null)
                {
                    end = attackOff.Time;
                }
                AgentItem extra = agentData.AddCustomNPCAgent(start, end, hand.Name, hand.Spec, id, false, hand.Toughness, hand.Healing, hand.Condition, hand.Concentration, hand.HitboxWidth, hand.HitboxHeight);
                RedirectEventsAndCopyPreviousStates(combatData, extensions, agentData, hand, copyEventsFrom, extra, true);
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

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        FindHands(fightData, agentData, combatData, extensions);
        base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
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
        base.ComputePlayerCombatReplayActors(p, log, replay);
        var radiantBlindnesses = p.GetBuffStatus(log, RadiantBlindness, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0);
        replay.Decorations.AddOverheadIcons(radiantBlindnesses, p, BuffImages.PersistentlyBlinded);
    }


    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        long castDuration;
        (long start, long end) lifespan;

        switch (target.ID)
        {
            case (int)TargetID.Adina:
                foreach (CastEvent cast in target.GetAnimatedCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd))
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
                            replay.Decorations.Add(new CircleDecoration(1100, lifespan, Colors.LightOrange, 0.4, new AgentConnector(target)).UsingGrowingEnd(lifespan.end));
                            break;
                        default:
                            break;
                    }
                }

                // Diamond Palisade
                var diamondPalisades = target.GetBuffStatus(log, DiamondPalisade, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0);
                foreach (Segment seg in diamondPalisades)
                {
                    replay.Decorations.Add(new CircleDecoration(90, seg, Colors.Red, 0.2, new AgentConnector(target)));
                    replay.Decorations.AddOverheadIcon(seg, target, SkillImages.MonsterSkill);
                }
                break;
            default:
                break;
        }
    }


    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor adina = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Adina)) ?? throw new MissingKeyActorsException("Adina not found");
        phases[0].AddTarget(adina, log);
        var handIDs = new TargetID[] { TargetID.HandOfErosion, TargetID.HandOfEruption };
        var invuls = GetFilteredList(log.CombatData, Determined762, adina, true, true).ToList();
        BuffEvent? lastInvuln = invuls.LastOrDefault();
        long lastBossPhaseStart = lastInvuln is BuffRemoveAllEvent ? lastInvuln.Time : log.FightData.LogEnd; // if log ends with any boss phase, ignore hands after that point
        phases[0].AddTargets(Targets.Where(x => x.IsAnySpecies(handIDs) && x.FirstAware < lastBossPhaseStart), log, PhaseData.TargetPriority.Blocking);
        if (!requirePhases)
        {
            return phases;
        }
        // Split phases
        long start = 0;
        var splitPhases = new List<PhaseData>();
        var splitPhaseEnds = new List<long>();
        for (int i = 0; i < invuls.Count; i++)
        {
            PhaseData splitPhase;
            BuffEvent be = invuls[i];
            if (be is BuffApplyEvent)
            {
                start = be.Time;
                if (i == invuls.Count - 1)
                {
                    splitPhase = new PhaseData(start, log.FightData.FightEnd, "Split " + (i / 2 + 1));
                    splitPhase.AddParentPhase(phases[0]);
                    splitPhaseEnds.Add(log.FightData.FightEnd);
                    AddTargetsToPhaseAndFit(splitPhase, [TargetID.HandOfErosion, TargetID.HandOfEruption], log);
                    splitPhases.Add(splitPhase);
                }
            }
            else
            {
                long end = be.Time;
                splitPhase = new PhaseData(start, end, "Split " + (i / 2 + 1));
                splitPhase.AddParentPhase(phases[0]);
                splitPhaseEnds.Add(end);
                AddTargetsToPhaseAndFit(splitPhase, [TargetID.HandOfErosion, TargetID.HandOfEruption], log);
                splitPhases.Add(splitPhase);
            }
        }
        // Main phases
        var mainPhases = new List<PhaseData>();
        var pillarApplies = log.CombatData.GetBuffDataByIDByDst(PillarPandemonium, adina.AgentItem).OfType<BuffApplyEvent>();
        Dictionary<long, List<BuffApplyEvent>> pillarAppliesGroupByTime = GroupByTime(pillarApplies);
        var mainPhaseEnds = new List<long>();
        foreach (KeyValuePair<long, List<BuffApplyEvent>> pair in pillarAppliesGroupByTime)
        {
            if (pair.Value.Count == 6)
            {
                mainPhaseEnds.Add(pair.Key);
            }
        }
        CastEvent? boulderBarrage = adina.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd).FirstOrDefault(x => x.SkillID == BoulderBarrage && x.Time < 6000);
        start = boulderBarrage == null ? 0 : boulderBarrage.EndTime;
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
                mainPhases.Add(new PhaseData(curPhaseStart, quantumQake, "Phase " + phaseIndex));
            }
            if (start != mainPhases.Last().Start)
            {
                mainPhases.Add(new PhaseData(start, log.FightData.FightEnd, "Phase " + (phaseIndex + 1)));
            }
        }
        else if (start > 0 && invuls.Count == 0)
        {
            // no split
            mainPhases.Add(new PhaseData(start, log.FightData.FightEnd, "Phase 1"));
        }

        foreach (PhaseData phase in mainPhases)
        {
            phase.AddTarget(adina, log);
            phase.AddParentPhase(phases[0]);
            phase.AddTargets(Targets.Where(x => x.IsAnySpecies(handIDs) && phase.InInterval(x.FirstAware)), log, PhaseData.TargetPriority.NonBlocking);
        }
        phases.AddRange(mainPhases);
        phases.AddRange(splitPhases);
        phases.Sort((x, y) => x.Start.CompareTo(y.Start));
        //
        return phases;
    }

    protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
    {
        var map = new CombatReplayMap(CombatReplayAdinaMainPhase1,
                        (866, 1000),
                        (13840, -2698, 15971, -248)/*,
                        (-21504, -21504, 24576, 24576),
                        (33530, 34050, 35450, 35970)*/);
        //
        try
        {
            var splitPhasesMap = new List<string>()
            {
                    CombatReplayAdinaSplitPhase1,
                    CombatReplayAdinaSplitPhase2,
                    CombatReplayAdinaSplitPhase3,
            };
            var mainPhasesMap = new List<string>()
            {
                    CombatReplayAdinaMainPhase1,
                    CombatReplayAdinaMainPhase2,
                    CombatReplayAdinaMainPhase3,
                    CombatReplayAdinaMainPhase4
            };
            var crMaps = new List<string>();
            int mainPhaseIndex = 0;
            int splitPhaseIndex = 0;
            var phases = log.FightData.GetPhases(log).Where(x => !x.BreakbarPhase).ToList();
            var mainPhases = phases.Where(x => x.Name.Contains("Phase"));
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phaseData = phases[i];
                if (mainPhases.Contains(phaseData))
                {
                    if (mainPhasesMap.Contains(crMaps.LastOrDefault()!))
                    {
                        splitPhaseIndex++;
                    }
                    crMaps.Add(mainPhasesMap[mainPhaseIndex++]);
                }
                else
                {
                    if (splitPhasesMap.Contains(crMaps.LastOrDefault()!))
                    {
                        mainPhaseIndex++;
                    }
                    crMaps.Add(splitPhasesMap[splitPhaseIndex++]);
                }
            }
            map.MatchMapsToPhases(crMaps, phases, log.FightData.FightEnd);
        }
        catch (Exception)
        {
            log.UpdateProgressWithCancellationCheck("Parsing: Failed to associate Adina Combat Replay maps");
        }
        //
        return map;
    }

    internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
    {
        SingleActor target = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Adina)) ?? throw new MissingKeyActorsException("Adina not found");
        return (target.GetHealth(combatData) > 23e6) ? FightData.EncounterMode.CM : FightData.EncounterMode.Normal;
    }

    protected override void SetInstanceBuffs(ParsedEvtcLog log)
    {
        base.SetInstanceBuffs(log);

        if (log.FightData.Success && log.CombatData.GetBuffData(AchievementEligibilityConserveTheLand).Any())
        {
            InstanceBuffs.MaybeAdd(GetOnPlayerCustomInstanceBuff(log, AchievementEligibilityConserveTheLand));
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
