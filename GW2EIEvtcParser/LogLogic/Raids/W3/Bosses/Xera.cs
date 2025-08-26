using System;
using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicTimeUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal class Xera : StrongholdOfTheFaithful
{

    internal readonly MechanicGroup Mechanics = new MechanicGroup([
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(TemporalShredOrb, new MechanicPlotlySetting(Symbols.Circle,Colors.Red), "Orb", "Temporal Shred (Hit by Red Orb)","Red Orb", 0),
                new PlayerDstHealthDamageHitMechanic(TemporalShredAoE, new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Red), "Orb Aoe", "Temporal Shred (Stood in Orb Aoe)","Orb AoE", 0),
            ]),
            new PlayerDstBuffApplyMechanic(BloodstoneProtection, new MechanicPlotlySetting(Symbols.HourglassOpen,Colors.DarkPurple), "In Bubble", "Bloodstone Protection (Stood in Bubble)","Inside Bubble", 0),
            new MechanicGroup([
                new EnemyCastStartMechanic(SummonFragments, new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkTeal), "CC", "Summon Fragment (Xera Breakbar)","Breakbar", 0),
                new EnemyCastEndMechanic(SummonFragments, new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Red), "CC Fail", "Summon Fragment (Failed CC)","CC Fail", 0)
                    .UsingChecker( (ce,log) => ce.ActualDuration > 11940),
                new EnemyCastEndMechanic(SummonFragments, new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkGreen), "CCed", "Summon Fragment (Breakbar broken)","CCed", 0)
                    .UsingChecker( (ce, log) => ce.ActualDuration <= 11940),
            ]),
            new PlayerDstBuffApplyMechanic(Derangement, new MechanicPlotlySetting(Symbols.SquareOpen,Colors.LightPurple), "Stacks", "Derangement (Stacking Debuff)","Derangement", 0),
            new MechanicGroup([
                new PlayerDstBuffApplyMechanic(BendingChaos, new MechanicPlotlySetting(Symbols.TriangleDownOpen,Colors.Yellow), "Button1", "Bending Chaos (Stood on 1st Button)","Button 1", 0),
                new PlayerDstBuffApplyMechanic(ShiftingChaos, new MechanicPlotlySetting(Symbols.TriangleNEOpen,Colors.Yellow), "Button2", "Bending Chaos (Stood on 2nd Button)","Button 2", 0),
                new PlayerDstBuffApplyMechanic(TwistingChaos, new MechanicPlotlySetting(Symbols.TriangleNWOpen,Colors.Yellow), "Button3", "Bending Chaos (Stood on 3rd Button)","Button 3", 0),
            ]),
            new PlayerDstBuffApplyMechanic(InterventionSkillOwnerBuff, new MechanicPlotlySetting(Symbols.Square,Colors.Blue), "Shield", "Intervention (got Special Action Key)","Shield", 0),
            new PlayerDstBuffApplyMechanic(GravityWellXera, new MechanicPlotlySetting(Symbols.CircleXOpen,Colors.Magenta), "Gravity Half", "Half-platform Gravity Well","Gravity Well", 4000),
            new MechanicGroup([
                new PlayerDstBuffApplyMechanic(HerosDeparture, new MechanicPlotlySetting(Symbols.Circle,Colors.DarkGreen), "TP Out", "Hero's Departure (Teleport to Platform)","TP",0),
                new PlayerDstBuffApplyMechanic(HerosReturn, new MechanicPlotlySetting(Symbols.Circle,Colors.Green), "TP Back", "Hero's Return (Teleport back)","TP back", 0),
            ]),
            /*new Mechanic(Intervention, "Intervention", ParseEnum.BossIDS.Xera, new MechanicPlotlySetting(Symbols.Hourglass,"rgb(128,0,128)"), "Bubble",0),*/
            //new Mechanic(Disruption, "Disruption", ParseEnum.BossIDS.Xera, new MechanicPlotlySetting(Symbols.Square,Colors.DarkGreen), "TP",0), 
            //Not sure what this (ID 350342,"Disruption") is. Looks like it is the pulsing "orb removal" from the orange circles on the 40% platform. Would fit the name although it's weird it can hit players. 
        ]);

    public Xera(int triggerID) : base(triggerID)
    {
        MechanicList.Add(Mechanics);
        Extension = "xera";
        GenericFallBackMethod = FallBackMethod.Death | FallBackMethod.CombatExit;
        Icon = EncounterIconXera;
        LogCategoryInformation.InSubCategoryOrder = 3;
        LogID |= 0x000004;
        ChestID = ChestID.XeraChest;
    }

    protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
    {
        return new CombatReplayMap(CombatReplayXera,
                        (1000, 897),
                        (-5992, -5992, 69, -522)/*,
                        (-12288, -27648, 12288, 27648),
                        (1920, 12160, 2944, 14464)*/);
    }

    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        return
        [
            new EffectCastFinder(InterventionSAK, EffectGUIDs.XeraIntervention1),
        ];
    }

    internal override List<BuffEvent> SpecialBuffEventProcess(CombatData combatData, SkillData skillData)
    {
        var res = new List<BuffEvent>();
        foreach (var determinedApply in combatData.GetBuffApplyData(Determined762))
        {
            if (determinedApply.To.IsSpecies(TargetID.Xera))
            {
                var xera = determinedApply.To;
                var mergedXera2 = GetXera2Merge(xera);
                var invulEnd = mergedXera2 != null ? mergedXera2.FirstAware : xera.LastAware;
                res.AddRange([
                    new BuffRemoveAllEvent(_unknownAgent, xera, invulEnd, int.MaxValue, skillData.Get(Determined762), IFF.Unknown, 1, int.MaxValue),
                    new BuffRemoveManualEvent(_unknownAgent, xera, invulEnd, int.MaxValue, skillData.Get(Determined762), IFF.Unknown)
                ]);
            }
        }
        return res;
    }

    internal static AgentItem? GetXera2Merge(AgentItem xera)
    {
        return xera.Merges.FirstOrNull((in AgentItem.MergedAgentItem x) => x.Merged.IsSpecies(TargetID.Xera2))?.Merged;
    }

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, LogData logData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        var xera = GetMainTarget().AgentItem;
        var mergedXera2 = GetXera2Merge(xera);
        if (mergedXera2 == null)
        {
            BuffEvent? invulXera = GetInvulXeraEvent(combatData, xera);
            if (invulXera == null)
            {
                logData.SetSuccess(false, xera.LastAware);
            }
            return;
        }
        base.CheckSuccess(combatData, agentData, logData, playerAgents);
        if (logData.Success && logData.LogEnd < mergedXera2.FirstAware)
        {
            logData.SetSuccess(false, mergedXera2.LastAware);
        }
    }

    private static long GetMainXeraFightStart(ParsedEvtcLog log, AgentItem xera, long encounterStart)
    {
        var fakeXera = log.AgentData.GetNPCsByID(TargetID.FakeXera).LastOrDefault(x => x.FirstAware <= xera.FirstAware);
        if (fakeXera != null)
        {
            var enterCombat = log.CombatData.GetEnterCombatEvents(xera).FirstOrDefault();
            if (enterCombat != null)
            {
                return Math.Max(enterCombat.Time, encounterStart);
            }
        }
        return encounterStart;
    }
    internal static List<PhaseData> ComputePhases(ParsedEvtcLog log, SingleActor? xera, IReadOnlyList<SingleActor> targets, PhaseData encounterPhase, bool requirePhases)
    {
        // If xera is null, the whole fight is in pre event
        if (!requirePhases || xera == null)
        {
            return [];
        }
        long encounterStart = encounterPhase.Start;
        long encounterEnd = encounterPhase.End;
        var phases = new List<PhaseData>(5);
        long xeraFightStart = GetMainXeraFightStart(log, xera.AgentItem, encounterStart);
        PhaseData? phase100to0 = null;
        if (xeraFightStart > encounterStart)
        {
            var phasePreEvent = new SubPhasePhaseData(encounterPhase.Start, xeraFightStart, "Pre Event");
            phasePreEvent.AddParentPhase(encounterPhase);
            phasePreEvent.AddTargets(targets.Where(x => x.IsSpecies(TargetID.BloodstoneShardButton) || x.IsSpecies(TargetID.BloodstoneShardRift)), log);
            if (phasePreEvent.Targets.Count == 0)
            {
                phasePreEvent.AddTarget(targets.FirstOrDefault(x => x.IsSpecies(TargetID.DummyTarget) && x.Character == "Xera Pre Event"), log);
            }
            phases.Add(phasePreEvent);
            phase100to0 = new SubPhasePhaseData(xeraFightStart, log.LogData.LogEnd, "Main Fight");
            phase100to0.AddParentPhase(encounterPhase);
            phase100to0.AddTarget(xera, log);
            phases.Add(phase100to0);
        }
        BuffEvent? invulXera = GetInvulXeraEvent(log.CombatData, xera);
        // split happened
        if (invulXera != null)
        {
            var phase1 = new SubPhasePhaseData(xeraFightStart, invulXera.Time, "Phase 1");
            if (phase100to0 != null)
            {
                phase1.AddParentPhase(phase100to0);
            }
            else
            {
                phase1.AddParentPhase(encounterPhase);
            }
            phase1.AddTarget(xera, log);
            phases.Add(phase1);
            var mergedXera2 = GetXera2Merge(xera.AgentItem);
            long glidingEndTime = encounterEnd;
            if (mergedXera2 != null)
            {
                var movement = log.CombatData.GetMovementData(xera.AgentItem).OfType<PositionEvent>().FirstOrDefault(x => x.Time >= mergedXera2.FirstAware + 500);
                if (movement != null)
                {
                    glidingEndTime = movement.Time;
                }
                else
                {
                    glidingEndTime = mergedXera2.FirstAware;
                }
                var phase2 = new SubPhasePhaseData(glidingEndTime, encounterEnd, "Phase 2");
                if (phase100to0 != null)
                {
                    phase2.AddParentPhase(phase100to0);
                }
                else
                {
                    phase2.AddParentPhase(encounterPhase);
                }
                phase2.AddTarget(xera, log);
                phase2.AddTargets(targets.Where(t => t.IsSpecies(TargetID.BloodstoneShardMainFight)), log);
                //mainTarget.AddCustomCastLog(end, -5, (int)(start - end), ParseEnum.Activation.None, (int)(start - end), ParseEnum.Activation.None, log);
                phases.Add(phase2);
            }
            var glidingPhase = new SubPhasePhaseData(invulXera.Time, glidingEndTime, "Gliding");
            if (phase100to0 != null)
            {
                glidingPhase.AddParentPhase(phase100to0);
            }
            else
            {
                glidingPhase.AddParentPhase(encounterPhase);
            }
            glidingPhase.AddTargets(targets.Where(t => t.IsSpecies(TargetID.ChargedBloodstone)), log);
            phases.Add(glidingPhase);
        }
        return phases;
    }
    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        long logEnd = log.LogData.LogEnd;
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor mainTarget = GetMainTarget();
        phases[0].AddTarget(mainTarget, log);
        phases.AddRange(ComputePhases(log, mainTarget, Targets, phases[0], requirePhases));
        return phases;
    }

    private SingleActor GetMainTarget() => Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Xera)) ?? throw new MissingKeyActorsException("Xera not found");

    internal static BuffEvent? GetInvulXeraEvent(CombatData combatData, AgentItem xera)
    {
        BuffEvent? determined = combatData.GetBuffApplyDataByIDByDst(Determined762, xera).FirstOrDefault() ?? combatData.GetBuffApplyDataByIDByDst(SpawnProtection, xera).FirstOrDefault();
        return determined;
    }

    private static BuffEvent? GetInvulXeraEvent(CombatData combatData, SingleActor xera)
    {
        return GetInvulXeraEvent(combatData, xera.AgentItem);
    }

    internal override long GetLogOffset(EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData)
    {
        if (!agentData.TryGetFirstAgentItem(TargetID.Xera, out var xera))
        {
            throw new MissingKeyActorsException("Xera not found");
        }
        // enter combat
        CombatItem? enterCombat = combatData.Find(x => x.SrcMatchesAgent(xera) && x.IsStateChange == StateChange.EnterCombat);
        if (enterCombat != null)
        {
            if (agentData.TryGetFirstAgentItem(TargetID.FakeXera, out var fakeXera))
            {
                long encounterStart = fakeXera.LastAware;
                CombatItem ?death = combatData.LastOrDefault(x => x.IsStateChange == StateChange.ChangeDead && x.SrcMatchesAgent(fakeXera));
                if (death != null)
                {
                    encounterStart = death.Time + 1000;
                } 
                else
                {
                    CombatItem? exitCombat = combatData.LastOrDefault(x => x.IsStateChange == StateChange.ExitCombat && x.SrcMatchesAgent(fakeXera));
                    if (exitCombat != null)
                    {
                        encounterStart = exitCombat.Time + 1000;
                    }
                }
                return encounterStart;
            }
            return enterCombat.Time;
        }
        return GetGenericLogOffset(logData);
    }

    internal static void FindBloodstones(AgentData agentData, List<CombatItem> combatData)
    {
        //
        var maxHPUpdates = combatData.Where(x => x.IsStateChange == StateChange.MaxHealthUpdate).Select(x => new MaxHealthUpdateEvent(x, agentData)).ToList();
        //
        var bloodstoneFragments = maxHPUpdates.Where(x => x.MaxHealth == 104580).Select(x => x.Src).Where(x => x.Type == AgentItem.AgentType.Gadget);
        foreach (AgentItem gadget in bloodstoneFragments)
        {
            gadget.OverrideType(AgentItem.AgentType.NPC, agentData);
            gadget.OverrideID(TargetID.BloodstoneFragment, agentData);
        }
        //
        var bloodstoneShardsMainFight = maxHPUpdates.Where(x => x.MaxHealth == 343620).Select(x => x.Src).Where(x => x.Type == AgentItem.AgentType.Gadget);
        foreach (AgentItem gadget in bloodstoneShardsMainFight)
        {
            gadget.OverrideType(AgentItem.AgentType.NPC, agentData);
            gadget.OverrideID(TargetID.BloodstoneShardMainFight, agentData);
        }
        //
        var bloodstoneShardsButton = maxHPUpdates.Where(x => x.MaxHealth == 597600).Select(x => x.Src).Where(x => x.Type == AgentItem.AgentType.Gadget);
        foreach (AgentItem gadget in bloodstoneShardsButton)
        {
            gadget.OverrideType(AgentItem.AgentType.NPC, agentData);
            gadget.OverrideID(TargetID.BloodstoneShardButton, agentData);
        }
        //
        var bloodstoneShardsRift = maxHPUpdates.Where(x => x.MaxHealth == 747000).Select(x => x.Src).Where(x => x.Type == AgentItem.AgentType.Gadget);
        foreach (AgentItem gadget in bloodstoneShardsRift)
        {
            gadget.OverrideType(AgentItem.AgentType.NPC, agentData);
            gadget.OverrideID(TargetID.BloodstoneShardRift, agentData);
        }
        //
        var chargedBloodStones = maxHPUpdates.Where(x => x.MaxHealth == 74700).Select(x => x.Src).Where(x => x.Type == AgentItem.AgentType.Gadget);
        foreach (AgentItem gadget in chargedBloodStones)
        {
            if (!combatData.Any(x => x.IsDamage() && x.DstMatchesAgent(gadget)))
            {
                continue;
            }
            gadget.OverrideType(AgentItem.AgentType.NPC, agentData);
            gadget.OverrideID(TargetID.ChargedBloodstone, agentData);
        }
    }

    internal static void RenameBloodStones(IReadOnlyList<SingleActor> targets)
    {
        foreach (var target in targets)
        {
            switch (target.ID)
            {
                case (int)TargetID.BloodstoneShardRift:
                    target.OverrideName("Rift " + target.Character);
                    break;
                case (int)TargetID.BloodstoneShardButton:
                    target.OverrideName("Button " + target.Character);
                    break;
                case (int)TargetID.BloodstoneShardMainFight:
                    target.OverrideName("Phase 2 " + target.Character);
                    break;
            }
        }
    }

    internal static void SetManualHPForXera(SingleActor Xera)
    {
        Xera.SetManualHealth(24085950, new List<(long hpValue, double percent)>()
        {
            (22611300, 100),
            (25560600, 50)
        });
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        // find target
        if (!agentData.TryGetFirstAgentItem(TargetID.Xera, out var firstXera))
        {
            throw new MissingKeyActorsException("Xera not found");
        }
        FindBloodstones(agentData, combatData);
        if (agentData.TryGetFirstAgentItem(TargetID.FakeXera, out _))
        {
            agentData.AddCustomNPCAgent(logData.LogStart, logData.LogEnd, "Xera Pre Event", Spec.NPC, TargetID.DummyTarget, true);
        }
        // find split
        if (agentData.TryGetFirstAgentItem(TargetID.Xera2, out var secondXera))
        {
            firstXera.OverrideAwareTimes(firstXera.FirstAware, secondXera.LastAware);
            AgentManipulationHelper.RedirectAllEvents(combatData, extensions, agentData, secondXera, firstXera);
        }
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);
        RenameBloodStones(Targets);
        // Xera gains hp at 50%, total hp of the encounter is not the initial hp of Xera
        SetManualHPForXera(GetMainTarget());
    }

    internal override LogData.LogStartStatus GetLogStartStatus(CombatData combatData, AgentData agentData, LogData logData)
    {
        // We expect pre event with logs with LogStartNPCUpdate events
        if (!agentData.TryGetFirstAgentItem(TargetID.FakeXera, out _) && combatData.GetLogNPCUpdateEvents().Any())
        {
            return LogData.LogStartStatus.NoPreEvent;
        }
        else
        {
            return LogData.LogStartStatus.Normal;
        }
    }

    internal override IReadOnlyList<TargetID>  GetTargetsIDs()
    {
        return [
            TargetID.Xera,
            TargetID.DummyTarget,
            TargetID.BloodstoneShardMainFight,
            TargetID.BloodstoneShardRift,
            TargetID.BloodstoneShardButton,
            TargetID.ChargedBloodstone,
        ];
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        return
        [
            TargetID.WhiteMantleSeeker1,
            TargetID.WhiteMantleSeeker2,
            TargetID.WhiteMantleKnight1,
            TargetID.WhiteMantleKnight2,
            TargetID.WhiteMantleBattleMage1,
            TargetID.WhiteMantleBattleMage2,
            TargetID.BloodstoneFragment,
            TargetID.ExquisiteConjunction,
            TargetID.XerasPhantasm,
        ];
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        switch (target.ID)
        {
            case (int)TargetID.Xera:
                foreach (CastEvent cast in target.GetAnimatedCastEvents(log))
                {
                    switch (cast.SkillID)
                    {
                        case SummonFragments:
                            replay.Decorations.Add(new CircleDecoration(180, (cast.Time, cast.EndTime), Colors.LightBlue, 0.3, new AgentConnector(target)));
                            break;
                        default:
                            break;
                    }
                }
                replay.AddHideByBuff(target, log, Determined762);
                break;
            case (int)TargetID.ChargedBloodstone:
                var activeXeras = log.AgentData.GetNPCsByID(TargetID.Xera).Where(x => target.AgentItem.InAwareTimes(x)).ToList();
                long hiddenStart = target.FirstAware;
                for (int i = 0; i < activeXeras.Count; i++)
                {
                    var activeXera = activeXeras[i];
                    var xeraInvulApply = log.CombatData.GetBuffApplyDataByIDByDst(Determined762, activeXera).FirstOrDefault();
                    if (xeraInvulApply != null)
                    {
                        long hiddenEnd = xeraInvulApply.Time + 14000;
                        replay.Hidden.Add(new Segment(hiddenStart, hiddenEnd));
                        var mergedXera2 = GetXera2Merge(activeXera);
                        if (mergedXera2 != null)
                        {
                            var deadEvent = log.CombatData.GetHealthUpdateEvents(target.AgentItem).LastOrDefault(x => x.HealthPercent < 1 && x.Time > activeXera.FirstAware && x.Time < mergedXera2.FirstAware);
                            hiddenStart = deadEvent != null ? deadEvent.Time : mergedXera2.FirstAware;
                        } 
                        else
                        {
                            var nextFakeXera = log.AgentData.GetNPCsByID(TargetID.FakeXera).FirstOrDefault(x => x.FirstAware > hiddenEnd);
                            long threshold = nextFakeXera != null ? nextFakeXera.FirstAware : target.LastAware;
                            var deadEvent = log.CombatData.GetHealthUpdateEvents(target.AgentItem).LastOrDefault(x => x.HealthPercent < 1 && x.Time > activeXera.FirstAware && x.Time < threshold);
                            hiddenStart = deadEvent != null ? deadEvent.Time : threshold;
                        }
                    }
                    replay.Hidden.Add(new Segment(hiddenStart, target.LastAware));
                }
                break;
            case (int)TargetID.BloodstoneFragment:
                replay.Decorations.Add(new CircleDecoration(760, (replay.TimeOffsets.start, replay.TimeOffsets.end), Colors.LightOrange, 0.2, new AgentConnector(target)));
                break;
            default:
                break;
        }
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor player, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputePlayerCombatReplayActors(player, log, replay);
        // Derangement - 0 to 29 nothing, 30 to 59 Silver, 60 to 89 Gold, 90 to 99 Red
        var derangements = player.GetBuffStatus(log, Derangement).Where(x => x.Value > 0);
        foreach (var segment in derangements)
        {
            if (segment.Value >= 90)
            {
                replay.Decorations.AddOverheadIcon(segment, player, ParserIcons.DerangementRedOverhead);
            }
            else if (segment.Value >= 60)
            {
                replay.Decorations.AddOverheadIcon(segment, player, ParserIcons.DerangementGoldOverhead);
            }
            else if (segment.Value >= 30)
            {
                replay.Decorations.AddOverheadIcon(segment, player, ParserIcons.DerangementSilverOverhead);
            }
        }
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);
        /*CombatReplay.DebugAllNPCEffects(log, environmentDecorations, [
            EffectGUIDs.XeraHalfArenaGravityWell,
            EffectGUIDs.XeraIntervention1,
            EffectGUIDs.XeraIntervention2,
            EffectGUIDs.XeraIntervention3,
            EffectGUIDs.XeraTemporalShredAoE,
            EffectGUIDs.XeraShardAoEs,
            EffectGUIDs.XeraSomething,
            EffectGUIDs.XeraBloodstoneFragmentSomething,
            EffectGUIDs.XeraSplitShardAoEsIndicator,
            EffectGUIDs.XeraSplitShardAoEsHit,
            EffectGUIDs.XeraUnstableLeyRiftClosed,
            new("F7BE4829C606B3459F747F667FB3D894"),
            new("FD2802E2DC92124F940DDDD998B8B57B"),
            new("86EDD215438A704196F540CBDB10D934"),
            new("1FD11FB1DB7C224D929CB797AD2101DD"),
            new("74D24FC59143004B8E6E73DBC9CAC1CE"),
            new("207E322FE91683469E2E4A0D841BB1B0"),
            new("F57C7DD4E9BDE348BF8583F97E1C01C1"),
            new("4E547E3AAD823B4187989810D14D834B"),
            new("3D26F89685240644B79FEA787F17C2B3"),
            new("3D26F89685240644B79FEA787F17C2B3"),
            new("B6BA0272B7786B4EB8E6C9C949A2EB3D"),
            ]);*/
        // Intervention Bubble
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.XeraIntervention1, out var interventions))
        {
            foreach (EffectEvent intervention in interventions)
            {
                // Effect has duration of 4294967295 but the skill lasts only 6000
                (long, long) lifespan = intervention.ComputeDynamicLifespan(log, 6000);
                var circle = new CircleDecoration(240, lifespan, Colors.Yellow, 0.3, new PositionConnector(intervention.Position));
                environmentDecorations.Add(circle);
                environmentDecorations.Add(circle.GetBorderDecoration(Colors.LightBlue, 0.4));
            }
        }

        if (log.CombatData.TryGetEffectEventsByGUIDs([EffectGUIDs.XeraShardAoEs, EffectGUIDs.XeraSplitShardAoEsHit], out var shardAoEs))
        {
            foreach (EffectEvent shardAoE in shardAoEs)
            {
                (long, long) lifespan = shardAoE.ComputeLifespan(log, shardAoE.GUIDEvent.ContentGUID == EffectGUIDs.XeraShardAoEs ? 3000 : 1000);
                var circle = new CircleDecoration(180, lifespan, Colors.Red, 0.2, new PositionConnector(shardAoE.Position)).UsingFilled(false);
                environmentDecorations.Add(circle);
            }
        }

        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.XeraSplitShardAoEsIndicator, out var splitShardAoEsIndicator))
        {
            foreach (EffectEvent splitShardAoEIndicator in splitShardAoEsIndicator)
            {
                (long, long) lifespan = splitShardAoEIndicator.ComputeLifespan(log, 2000);
                var circle = new CircleDecoration(180, lifespan, Colors.Orange, 0.15, new PositionConnector(splitShardAoEIndicator.Position));
                environmentDecorations.AddWithGrowing(circle, lifespan.Item2);
            }
        }

        // Gravity Well
        // TODO: Find the correct effect, this is wrong
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.XeraHalfArenaGravityWell, out var halfGravityWells))
        {
            var cur = 0;
            var pos = new PositionConnector(new Vector3(-3020.77f, -3153.32f, -20338.6f));
            bool resetCurForSecondPhase = true;
            foreach (var halfGravityWell in halfGravityWells)
            {
                // Float happens after 7000 ms, extra 500ms to display the hit
                float angle = 0;
                (long start, long end) lifespan = (halfGravityWell.Time, halfGravityWell.Time + 7500);
                (long start, long end) lifespanIndicator = (halfGravityWell.Time, halfGravityWell.Time + 7000);
                bool hasFired = true;
                var activeXera = log.AgentData.GetNPCsByID(TargetID.Xera).FirstOrDefault(x => x.FirstAware <= halfGravityWell.Time && x.LastAware >= halfGravityWell.Time);
                if (activeXera == null)
                {
                    continue;
                }
                var splitEvent = GetInvulXeraEvent(log.CombatData, activeXera);
                if (splitEvent == null || splitEvent.Time > halfGravityWell.Time)
                {
                    var timeLimit = splitEvent != null ? splitEvent.Time : activeXera.LastAware;
                    angle = -30 + (cur++) * 90;
                    if (lifespanIndicator.end > timeLimit)
                    {
                        hasFired = false;
                    }
                    lifespan.end = Math.Min(lifespan.end, timeLimit);
                }
                else
                {
                    if (cur > 0 && resetCurForSecondPhase)
                    {
                        cur = 0;
                        resetCurForSecondPhase = false;
                    }
                    var timeLimit = activeXera.LastAware;
                    angle = -210 - (cur++) * 90;
                    if (lifespanIndicator.end > timeLimit)
                    {
                        hasFired = false;
                    }
                    lifespan.end = Math.Min(lifespan.end, timeLimit);
                }
                var angleConnector = new AngleConnector(angle);
                environmentDecorations.AddWithFilledWithGrowing(
                        (PieDecoration)new PieDecoration(1150, 180, lifespan, Colors.Purple, 0.15, pos)
                            .UsingRotationConnector(angleConnector),
                        true,
                        lifespanIndicator.end
                );
                if (hasFired)
                {
                    environmentDecorations.Add((PieDecoration)new PieDecoration(1150, 180, (lifespanIndicator.end, lifespanIndicator.end + 500), Colors.Purple, 0.2, pos)
                            .UsingRotationConnector(angleConnector));
                }
            }
        }

        // Temporal Shred Projectiles
        var temporalShred = log.CombatData.GetMissileEventsBySkillID(TemporalShredOrb);
        environmentDecorations.AddNonHomingMissiles(log, temporalShred, Colors.Red, 0.3, 25);

        // Temporal Shred AoE
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.XeraTemporalShredAoE, out var temporalShredAoEs))
        {
            foreach (EffectEvent effect in temporalShredAoEs)
            {
                (long start, long end) lifespan = effect.ComputeLifespan(log, 1500);
                environmentDecorations.AddWithBorder(new CircleDecoration(120, lifespan, Colors.LightPurple, 0.2, new PositionConnector(effect.Position)), Colors.Red, 0.2);
            }
        }
    }
}
