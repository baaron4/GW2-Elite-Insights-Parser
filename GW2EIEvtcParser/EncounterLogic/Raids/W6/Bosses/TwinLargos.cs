using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class TwinLargos : MythwrightGambit
{
    internal readonly MechanicGroup Mechanics = new MechanicGroup([
            new MechanicGroup([
                new EnemyCastStartMechanic(AquaticBarrage, new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkTeal), "CC", "Breakbar","Breakbar", 0),
                new EnemyCastEndMechanic(AquaticBarrage, new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkGreen), "CCed", "Breakbar broken","CCed", 0),
            ]),
            new MechanicGroup([
                new PlayerDstBuffApplyMechanic(Waterlogged, new MechanicPlotlySetting(Symbols.HexagonOpen,Colors.LightBlue), "Debuff", "Waterlogged (stacking water debuff)","Waterlogged", 0),
                new PlayerDstHealthDamageHitMechanic(VaporRush, new MechanicPlotlySetting(Symbols.TriangleLeftOpen,Colors.LightBlue), "Charge", "Vapor Rush (Triple Charge)","Vapor Rush Charge", 0),
                new PlayerDstHealthDamageHitMechanic(TidalPoolSkill, new MechanicPlotlySetting(Symbols.Circle,Colors.LightBlue), "Pool", "Tidal Pool","Tidal Pool", 0),
                new PlayerDstBuffApplyMechanic(TidalPoolBuff, new MechanicPlotlySetting(Symbols.Diamond,Colors.Teal), "Poison", "Expanding Water Field","Water Poison", 0),
                new PlayerDstHealthDamageHitMechanic(AquaticDetainmentHit, new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Blue), "Float", "Aquatic Detainment (Float Bubble)","Float Bubble", 6000),
                new PlayerDstBuffApplyMechanic(AquaticAuraNikare, new MechanicPlotlySetting(Symbols.DiamondOpen,Colors.Teal), "Nik Aura", "Increasing Damage Debuff on Nikare's Last Platform","Nikare Aura Debuff", 0),
            ]),
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(SeaSwell, new MechanicPlotlySetting(Symbols.CircleOpen,Colors.DarkPurple), "Wave", "Sea Swell (Shockwave)","Shockwave", 0),
                new PlayerDstHealthDamageHitMechanic(AquaticVortex, new MechanicPlotlySetting(Symbols.StarSquareOpenDot,Colors.LightBlue), "Tornado", "Aquatic Vortex (Water Tornados)","Tornado", 0),
                new PlayerDstHealthDamageHitMechanic(VaporJet, new MechanicPlotlySetting(Symbols.Square,Colors.Pink), "Steal", "Vapor Jet (Boon Steal)","Boon Steal", 0),
                new PlayerDstBuffApplyMechanic(AquaticAuraKenut, new MechanicPlotlySetting(Symbols.SquareOpen,Colors.Teal), "Ken Aura", "Increasing Damage Debuff on Kenut's Last Platform","Kenut Aura Debuff", 0),
                new PlayerDstHealthDamageHitMechanic(CycloneBurst, new MechanicPlotlySetting(Symbols.YUpOpen,Colors.Pink), "Y Field", "Cyclone Burst (triangular rotating fields on Kenut)","Cyclone Burst", 0),
            ]),
            new PlayerDstHealthDamageHitMechanic(Geyser, new MechanicPlotlySetting(Symbols.Hexagon,Colors.Teal), "KB/Launch", "Geyser (Launching Aoes)","Launch Field", 0),
            new EnemyDstBuffApplyMechanic(EnragedTwinLargos, new MechanicPlotlySetting(Symbols.StarDiamond,Colors.Red), "Enrage", "Enraged","Enrage", 0),
        ]);
    public TwinLargos(int triggerID) : base(triggerID)
    {
        MechanicList.Add(Mechanics);
        Extension = "twinlargos";
        Icon = EncounterIconTwinLargos;
        ChestID = ChestID.TwinLargosChest;
        EncounterCategoryInformation.InSubCategoryOrder = 1;
        EncounterID |= 0x000002;
    }

    protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
    {
        return new CombatReplayMap(CombatReplayTwinLargos,
                        (765, 1000),
                        (10846, -3878, 18086, 5622)/*,
                        (-21504, -21504, 24576, 24576),
                        (13440, 14336, 15360, 16256)*/);
    }

    internal override IReadOnlyList<TargetID>  GetTargetsIDs()
    {
        return
        [
            TargetID.Nikare,
            TargetID.Kenut
        ];
    }

    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        return
        [
            new DamageCastFinder(NikareAquaticAura, NikareAquaticAura),
            new DamageCastFinder(KenutAquaticAura, KenutAquaticAura),
        ];
    }

    protected override IReadOnlyList<TargetID> GetSuccessCheckIDs()
    {
        return
        [
            TargetID.Nikare,
            TargetID.Kenut
        ];
    }

    internal override List<HealthDamageEvent> SpecialDamageEventProcess(CombatData combatData, AgentData agentData, SkillData skillData)
    {
        NegateDamageAgainstBarrier(combatData, agentData, [TargetID.Nikare, TargetID.Kenut]);
        return [];
    }

    private static List<PhaseData> GetTargetPhases(ParsedEvtcLog log, SingleActor target, string baseName, PhaseData fullFightPhase)
    {
        long start = 0;
        long end = 0;
        long fightEnd = log.FightData.FightEnd;
        var targetPhases = new List<PhaseData>();
        var states = new List<TimeCombatEvent>();
        states.AddRange(log.CombatData.GetEnterCombatEvents(target.AgentItem));
        states.AddRange(GetBuffApplyRemoveSequence(log.CombatData, Determined762, target, true, true).Where(x => x is BuffApplyEvent));
        states.AddRange(log.CombatData.GetDeadEvents(target.AgentItem));
        states.Sort((x, y) => x.Time.CompareTo(y.Time));
        for (int i = 0; i < states.Count; i++)
        {
            TimeCombatEvent state = states[i];
            if (state is EnterCombatEvent)
            {
                start = state.Time;
                if (i == states.Count - 1)
                {
                    targetPhases.Add(new PhaseData(start, fightEnd));
                }
            }
            else
            {
                end = Math.Min(state.Time, fightEnd);
                targetPhases.Add(new PhaseData(start, end));
                if (i == states.Count - 1 && targetPhases.Count < 3)
                {
                    targetPhases.Add(new PhaseData(end, fightEnd));
                }
            }
        }
        targetPhases.RemoveAll(x => x.DurationInMS < ParserHelper.PhaseTimeLimit);
        for (int i = 0; i < targetPhases.Count; i++)
        {
            PhaseData phase = targetPhases[i];
            phase.Name = baseName + " P" + (i + 1);
            phase.AddParentPhase(fullFightPhase);
            phase.AddTarget(target, log);
        }
        return targetPhases;
    }

    private static void FallBackPhases(SingleActor target, List<PhaseData> phases, ParsedEvtcLog log, bool firstPhaseAt0)
    {
        IReadOnlyCollection<AgentItem> pAgents = log.PlayerAgents;
        // clean Nikare/Kenut missing enter combat events related bugs
        switch (phases.Count)
        {
            case 2:
                {
                    PhaseData p1 = phases[0];
                    PhaseData p2 = phases[1];
                    // P1 and P2 merged
                    if (p1.Start == p2.Start)
                    {
                        HealthDamageEvent? hit = log.CombatData.GetDamageTakenData(target.AgentItem).FirstOrDefault(x => x.Time >= p1.End + 5000 && pAgents.Contains(x.From.GetFinalMaster()) && x.HealthDamage > 0 && x is DirectHealthDamageEvent);
                        if (hit != null)
                        {
                            p2.OverrideStart(hit.Time);
                            p2.Name += " (Fallback)";
                        }
                        else
                        {
                            p2.OverrideStart(p1.End);
                            p2.Name += " (Bad Fallback)";
                        }
                    }
                }
                break;
            case 3:
                {
                    PhaseData p1 = phases[0];
                    PhaseData p2 = phases[1];
                    PhaseData p3 = phases[2];
                    // P1 and P2 merged
                    if (p1.Start == p2.Start)
                    {
                        HealthDamageEvent? hit = log.CombatData.GetDamageTakenData(target.AgentItem).FirstOrDefault(x => x.Time >= p1.End + 5000 && pAgents.Contains(x.From.GetFinalMaster()) && x.HealthDamage > 0 && x is DirectHealthDamageEvent);
                        if (hit != null)
                        {
                            p2.OverrideStart(hit.Time);
                            p2.Name += " (Fallback)";
                        }
                        else
                        {
                            p2.OverrideStart(p1.End);
                            p2.Name += " (Bad Fallback)";
                        }
                    }
                    // P1/P2 and P3 are merged
                    if (p1.Start == p3.Start || p2.Start == p3.Start)
                    {
                        HealthDamageEvent? hit = log.CombatData.GetDamageTakenData(target.AgentItem).FirstOrDefault(x => x.Time >= p2.End + 5000 && pAgents.Contains(x.From.GetFinalMaster()) && x.HealthDamage > 0 && x is DirectHealthDamageEvent);
                        if (hit != null)
                        {
                            p3.OverrideStart(hit.Time);
                            p3.Name += " (Fallback)";
                        }
                        else
                        {
                            p3.OverrideStart(p2.End);
                            p3.Name += " (Bad Fallback)";
                        }
                    }
                }
                break;
            default:
                break;
        }
        if (!firstPhaseAt0 && phases.Count > 0 && phases.First().Start == 0)
        {
            PhaseData p1 = phases[0];
            HealthDamageEvent? hit = log.CombatData.GetDamageTakenData(target.AgentItem).FirstOrDefault(x => x.Time >= 0 && pAgents.Contains(x.From.GetFinalMaster()) && x.HealthDamage > 0 && x is DirectHealthDamageEvent);
            if (hit != null)
            {
                p1.OverrideStart(hit.Time);
                p1.Name += " (Fallback)";
            }
            else
            {
                p1.Name += " (Bad Fallback)";
            }
        }
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor nikare = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Nikare)) ?? throw new MissingKeyActorsException("Nikare not found");
        phases[0].AddTarget(nikare, log);
        SingleActor? kenut = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Kenut));
        if (kenut != null)
        {
            phases[0].AddTarget(kenut, log);
        }
        if (!requirePhases)
        {
            return phases;
        }
        List<PhaseData> nikPhases = GetTargetPhases(log, nikare, "Nikare", phases[0]);
        FallBackPhases(nikare, nikPhases, log, true);
        phases.AddRange(nikPhases);
        if (kenut != null)
        {
            List<PhaseData> kenPhases = GetTargetPhases(log, kenut, "Kenut", phases[0]);
            FallBackPhases(kenut, kenPhases, log, false);
            phases.AddRange(kenPhases);
        }
        return phases;
    }

    internal override FightData.EncounterStartStatus GetEncounterStartStatus(CombatData combatData, AgentData agentData, FightData fightData)
    {
        if (TargetHPPercentUnderThreshold(TargetID.Kenut, fightData.FightStart, combatData, Targets) ||
            TargetHPPercentUnderThreshold(TargetID.Nikare, fightData.FightStart, combatData, Targets))
        {
            return FightData.EncounterStartStatus.Late;
        }
        return FightData.EncounterStartStatus.Normal;
    }

    internal static void AdjustFinalHPEvents(List<CombatItem> combatData, AgentItem agentItem)
    {
        var hpUpdates = combatData.Where(x => x.IsStateChange == StateChange.HealthUpdate && x.SrcMatchesAgent(agentItem));
        if (hpUpdates.Any(x => HealthUpdateEvent.GetHealthPercent(x) != 100 && HealthUpdateEvent.GetHealthPercent(x) != 0))
        {
            CombatItem lastHPUpdate = hpUpdates.Last();
            if (lastHPUpdate.DstAgent == 10000)
            {
                lastHPUpdate.OverrideSrcAgent(0);
            }
        }
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
        // discard hp update events after determined apply
        SingleActor nikare = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Nikare)) ?? throw new MissingKeyActorsException("Nikare not found");
        AdjustFinalHPEvents(combatData, nikare.AgentItem);
        SingleActor? kenut = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Kenut));
        if (kenut != null)
        {
            AdjustFinalHPEvents(combatData, kenut.AgentItem);
        }
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        (long start, long end) lifespan;

        var cls = target.GetCastEvents(log);
        switch (target.ID)
        {
            case (int)TargetID.Nikare:
                foreach (CastEvent cast in target.GetAnimatedCastEvents(log))
                {
                    switch (cast.SkillID)
                    {
                        // Aquatic Barrage - Breakbar CC
                        case AquaticBarrage:
                            replay.Decorations.Add(new OverheadProgressBarDecoration(ParserHelper.CombatReplayOverheadProgressBarMajorSizeInPixel, (cast.Time, cast.EndTime), Colors.Red, 0.6, Colors.Black, 0.2, [(cast.Time, 0), (cast.ExpectedEndTime, 100)], new AgentConnector(target))
                            .UsingRotationConnector(new AngleConnector(180)));
                            break;
                        // Aquatic Domain - Platform wipe (CM only)
                        case AquaticDomain:
                            lifespan = (cast.Time, cast.EndTime);
                            replay.Decorations.Add(new CircleDecoration(800, lifespan, Colors.Yellow, 0.3, new AgentConnector(target)).UsingGrowingEnd(lifespan.end));
                            break;
                        default:
                            break;
                    }
                }
                break;
            case (int)TargetID.Kenut:
                foreach (CastEvent cast in target.GetAnimatedCastEvents(log))
                {
                    switch (cast.SkillID)
                    {
                        // Aquatic Barrage - Breakbar CC
                        case AquaticBarrage:
                            replay.Decorations.Add(new OverheadProgressBarDecoration(ParserHelper.CombatReplayOverheadProgressBarMajorSizeInPixel, (cast.Time, cast.EndTime), Colors.Red, 0.6, Colors.Black, 0.2, [(cast.Time, 0), (cast.ExpectedEndTime, 100)], new AgentConnector(target))
                            .UsingRotationConnector(new AngleConnector(180)));
                            break;
                        // Aquatic Domain - Platform wipe (CM only)
                        case AquaticDomain:
                            lifespan = (cast.Time, cast.EndTime);
                            replay.Decorations.Add(new CircleDecoration(800, lifespan, Colors.Yellow, 0.3, new AgentConnector(target)).UsingGrowingEnd(lifespan.end));
                            break;
                        // Sea Swell - Shockwave
                        case SeaSwell:
                            {
                                int delay = 960;
                                int duration = 3000;
                                uint radius = 1200;
                                lifespan = (cast.Time + delay, cast.Time + delay + duration);
                                GeographicalConnector connector = new AgentConnector(target);
                                replay.Decorations.AddShockwave(connector, lifespan, Colors.SkyBlue, 0.5, radius);
                            }
                            break;
                        // Vapor Jet - Boon steal
                        case VaporJet:
                            {
                                int delay = 1000;
                                int duration = 500;
                                uint width = 500;
                                uint height = 250;
                                lifespan = (cast.Time + delay, cast.Time + delay + duration);
                                if (target.TryGetCurrentFacingDirection(log, cast.Time + 250, out var facing))
                                {
                                    var positionConnector = (AgentConnector)new AgentConnector(target).WithOffset(new(width / 2, 0, 0), true);
                                    var rotationConnextor = new AngleConnector(facing);
                                    replay.Decorations.AddWithBorder((RectangleDecoration)new RectangleDecoration(width, height, lifespan, Colors.LightOrange, 0.4, positionConnector).UsingRotationConnector(rotationConnextor));
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
                break;
            default:
                break;
        }
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputePlayerCombatReplayActors(p, log, replay);
        // Water "Poison Bomb"
        var waterToDrop = p.GetBuffStatus(log, TidalPoolBuff).Where(x => x.Value > 0);
        foreach (Segment seg in waterToDrop)
        {
            int timer = 5000;
            int duration = 83000;
            uint debuffRadius = 100;
            uint radius = 500;
            int toDropStart = (int)seg.Start;
            int toDropEnd = (int)seg.End;
            replay.Decorations.AddWithFilledWithGrowing(new CircleDecoration(debuffRadius, seg, Colors.Orange, 0.4, new AgentConnector(p)).UsingFilled(false), true, toDropStart + timer);
            if (p.TryGetCurrentInterpolatedPosition(log, toDropEnd, out var position))
            {
                replay.Decorations.AddWithGrowing(new CircleDecoration(radius, debuffRadius, (toDropEnd, toDropEnd + duration), Colors.DarkWhite, 0.5, new PositionConnector(position)).UsingFilled(false), toDropStart + duration);
            }
            replay.Decorations.AddOverheadIcon(seg, p, ParserIcons.TidalPoolOverhead);
        }
        // Bubble (Aquatic Detainment)
        var bubble = p.GetBuffStatus(log, AquaticDetainmentBuff).Where(x => x.Value > 0);
        uint bubbleRadius = 100;
        foreach (Segment seg in bubble)
        {
            replay.Decorations.Add(new CircleDecoration(bubbleRadius, seg, Colors.LightBlue, 0.3, new AgentConnector(p)));
        }
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);

        // Aquatic Barrage - CC Projectiles - Rendering affected by player distance.
        var aquaticBarrage = log.CombatData.GetMissileEventsBySkillID(AquaticBarrage);
        environmentDecorations.AddNonHomingMissiles(log, aquaticBarrage, Colors.White, 0.3, 15);
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData)
    {
        return "Twin Largos";
    }
    private static bool HasCastAquaticDomainOrCMHP(CombatData combatData, SingleActor actor, double hpThresholdForCM)
    {
        bool hasCastAquaticDomain = combatData.GetAnimatedCastData(actor.AgentItem).Any(x => x.SkillID == AquaticDomain);
        if (hasCastAquaticDomain) // aquatic domain only present in 
        {
            return true;
        }
        if (combatData.GetHealthUpdateEvents(actor.AgentItem).Any(x => x.HealthPercent < 70)) // reached below 75% but did not cast aquatic domain, not possible
        {
            return false;
        }
        bool hasCMHP = actor.GetHealth(combatData) > hpThresholdForCM;
        return hasCMHP;
    }
    internal static bool HasCastAquaticDomainOrCMHP(CombatData combatData, SingleActor? nikare, SingleActor? kenut)
    {
        if (nikare != null && HasCastAquaticDomainOrCMHP(combatData, nikare, 18e6))
        {
            return true;
        }
        if (kenut != null && HasCastAquaticDomainOrCMHP(combatData, kenut, 16e6))
        {
            return true;
        }
        return false;
    }

    internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
    {
        SingleActor nikare = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Nikare)) ?? throw new MissingKeyActorsException("Nikare not found");
        SingleActor? kenut = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Kenut));
        return HasCastAquaticDomainOrCMHP(combatData, nikare, kenut) ? FightData.EncounterMode.CM : FightData.EncounterMode.Normal;
    }
}
