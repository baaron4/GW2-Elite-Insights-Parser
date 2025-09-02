using System.Collections.Generic;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicTimeUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal class Sabir : TheKeyOfAhdashim
{
    internal readonly MechanicGroup Mechanics = new MechanicGroup([
            new MechanicGroup([
                new PlayerDstHealthDamageMechanic(DireDrafts, new MechanicPlotlySetting(Symbols.Circle,Colors.Orange), "B.Tornado", "Hit by big tornado", "Big Tornado Hit", 500)
                    .UsingChecker((de, log) => de.HasDowned || de.HasKilled),
                new PlayerDstHealthDamageMechanic(UnbridledTempest, new MechanicPlotlySetting(Symbols.Hexagon,Colors.Pink), "Shockwave", "Hit by Shockwave", "Shockwave Hit", 0)
                    .UsingChecker((de, log) => de.HasDowned || de.HasKilled),
                new PlayerDstHealthDamageMechanic(FuryOfTheStorm, new MechanicPlotlySetting(Symbols.Circle,Colors.Purple), "Arena AoE", "Hit by Arena wide AoE", "Arena AoE hit", 0)
                    .UsingChecker( (de, log) => de.HasDowned || de.HasKilled ),
            ]),
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic([ DynamicDeterrentNM, DynamicDeterrentCM ], new MechanicPlotlySetting(Symbols.YUpOpen,Colors.Pink), "Pushed", "Pushed by rotating breakbar", "Pushed", 0)
                    .UsingBuffChecker(Stability, false),
                new EnemyCastStartMechanic([ DynamicDeterrentNM, DynamicDeterrentCM ], new MechanicPlotlySetting(Symbols.Star, Colors.Yellow), "Dynamic Deterrent", "Casted Dynamic Deterrent", "Cast Dynamic Deterrent", 0),
            ]),
            new PlayerDstHealthDamageHitMechanic([ StormsEdgeLeftHand, StormsEdgeRightHand ], new MechanicPlotlySetting(Symbols.BowtieOpen, Colors.Blue), "Storm's Edge", "Hit by Storm's Edge", "Storm's Edge", 0),
            new PlayerDstHealthDamageHitMechanic(ChainLightning, new MechanicPlotlySetting(Symbols.HexagonOpen, Colors.White), "Chain Lightning", "Hit by Chain Lightning", "Chain Lightning Hit", 0),
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(Electrospark, new MechanicPlotlySetting(Symbols.CircleCross, Colors.Orange), "Electrospark", "Hit by Electrospark", "Electrospark", 0),
                new PlayerDstHealthDamageHitMechanic(Electrospark, new MechanicPlotlySetting(Symbols.CircleCrossOpen, Colors.Orange), "Charged Winds", "Achievement Elegibility: Charged Winds", "Charged Winds", 0)
                    .UsingAchievementEligibility(),
            ]),
            new MechanicGroup([
                new MechanicGroup([
                    new EnemyCastStartMechanic(RegenerativeBreakbar, new MechanicPlotlySetting(Symbols.DiamondWide,Colors.Magenta), "Reg.Breakbar", "Regenerating Breakbar","Regenerative Breakbar", 0),
                    new EnemyDstBuffRemoveMechanic(IonShield, new MechanicPlotlySetting(Symbols.DiamondWide,Colors.DarkTeal), "Reg.Breakbar Brkn", "Regenerative Breakbar Broken", "Regenerative Breakbar Broken", 2000),
                ]),
                new MechanicGroup([
                    new EnemyDstBuffApplyMechanic(RepulsionField, new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Magenta), "Rot.Breakbar", "Rotating Breakbar","Rotating Breakbar", 0),
                    new EnemyDstBuffRemoveMechanic(RepulsionField, new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkTeal), "Rot.Breakbar Brkn", "Rotating Breakbar Broken","Rotating Breakbar Broken", 0),
                ]),
            ]),
        ]);
    public Sabir(int triggerID) : base(triggerID)
    {
        MechanicList.Add(Mechanics);
        // rotating cc 56403
        Extension = "sabir";
        Icon = EncounterIconSabir;
        ChestID = ChestID.SabirsChest;
        LogCategoryInformation.InSubCategoryOrder = 0;
        LogID |= 0x000002;
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        return
        [
            TargetID.ParalyzingWisp,
            TargetID.VoltaicWisp,
            TargetID.SmallKillerTornado,
            TargetID.SmallJumpyTornado,
            TargetID.SabirMainPlateform,
            TargetID.SabirBigRectanglePlateform,
            TargetID.SabirRectanglePlateform,
            TargetID.SabirSquarePlateform
        ];
    }

    internal override LogLogic AdjustLogic(AgentData agentData, List<CombatItem> combatData, EvtcParserSettings parserSettings)
    {
        CombatItem? logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.LogNPCUpdate);
        // Handle potentially wrongly associated logs
        if (logStartNPCUpdate != null)
        {
            if (agentData.GetNPCsByID(TargetID.Adina).Any(adina => combatData.Any(evt => evt.IsDamagingDamage() && evt.DstMatchesAgent(adina) && agentData.GetAgent(evt.SrcAgent, evt.Time).GetFinalMaster().IsPlayer)))
            {
                return new Adina((int)TargetID.Adina);
            }
        }
        return base.AdjustLogic(agentData, combatData, parserSettings);
    }

    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        return
        [
            new DamageCastFinder(BoltBreakSabir, BoltBreakSabir),
            new EffectCastFinder(FlashDischargeSAK, EffectGUIDs.SabirFlashDischarge)
                .UsingChecker((effect, combatData, agentData, skillData) =>
                {
                    BuffRemoveAllEvent? buffRemove = combatData.GetBuffRemoveAllDataByDst(ViolentCurrents, effect.Src)
                        .Where(x => Math.Abs(effect.Time - x.Time) < ServerDelayConstant)
                        .FirstOrDefault();
                    return buffRemove != null;
                }),
        ];
    }

    internal override List<HealthDamageEvent> SpecialDamageEventProcess(CombatData combatData, AgentData agentData, SkillData skillData)
    {
        NegateDamageAgainstBarrier(combatData, agentData, [TargetID.Sabir]);
        return [];
    }
    internal static List<PhaseData> ComputePhases(ParsedEvtcLog log, SingleActor sabir, EncounterPhaseData encounterPhase, bool requirePhases)
    {
        if (!requirePhases)
        {
            return [];
        }
        var phases = new List<PhaseData>(3);

        var casts = sabir.GetCastEvents(log);
        var wallopingWinds = casts.Where(x => x.SkillID == WallopingWind);
        long start = encounterPhase.Start;
        int i = 0;
        foreach (var wallopingWind in wallopingWinds)
        {
            var phase = new SubPhasePhaseData(start, wallopingWind.Time, "Phase " + (i + 1));
            phase.AddParentPhase(encounterPhase);
            phase.AddTarget(sabir, log);
            phases.Add(phase);
            CastEvent? nextAttack = casts.FirstOrDefault(x => x.Time >= wallopingWind.EndTime && (x.SkillID == StormsEdgeRightHand || x.SkillID == StormsEdgeLeftHand || x.SkillID == ChainLightning));
            if (nextAttack == null)
            {
                break;
            }
            start = nextAttack.Time;

            i++;
        }
        if (i > 0)
        {
            var phase = new SubPhasePhaseData(start, encounterPhase.End, "Phase " + (i + 1));
            phase.AddParentPhase(encounterPhase);
            phase.AddTarget(sabir, log);
            phases.Add(phase);
        }
        return phases;
    }
    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Sabir)) ?? throw new MissingKeyActorsException("Sabir not found");
        phases[0].AddTarget(mainTarget, log);
        phases.AddRange(ComputePhases(log, mainTarget, (EncounterPhaseData)phases[0], requirePhases));
        return phases;
    }

    internal override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log, CombatReplayDecorationContainer arenaDecorations)
    {
        string mapUrl = log.AgentData.GetNPCsByID(TargetID.SabirMainPlateform).Count > 0 &&
            log.AgentData.GetNPCsByID(TargetID.SabirSquarePlateform).Count > 0 &&
            log.AgentData.GetNPCsByID(TargetID.SabirBigRectanglePlateform).Count > 0 &&
            log.AgentData.GetNPCsByID(TargetID.SabirRectanglePlateform).Count > 0 ?
                CombatReplayNoImage : CombatReplaySabir;
        var crMap = new CombatReplayMap(
                        (1000, 910),
                        (-14122, 142, -9199, 4640));
        AddArenaDecorationsPerEncounter(log, arenaDecorations, LogID, mapUrl, crMap);
        return crMap;
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputePlayerCombatReplayActors(p, log, replay);
        var boltBreaks = p.GetBuffStatus(log, BoltBreak).Where(x => x.Value > 0);
        uint boltBreakRadius = 180;
        foreach (Segment seg in boltBreaks)
        {
            var circle = new CircleDecoration(boltBreakRadius, seg, Colors.LightOrange, 0.2, new AgentConnector(p));
            replay.Decorations.AddWithGrowing(circle, seg.End);
        }
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        long castDuration;
        (long start, long end) lifespan = (replay.TimeOffsets.start, replay.TimeOffsets.end);

        switch (target.ID)
        {
            case (int)TargetID.Sabir:
                foreach (CastEvent cast in target.GetAnimatedCastEvents(log))
                {
                    switch (cast.SkillID)
                    {
                        // Fury of the Storm
                        case FuryOfTheStorm:
                            lifespan = (cast.Time, cast.EndTime);
                            replay.Decorations.Add(new CircleDecoration(1200, lifespan, Colors.LightBlue, 0.3, new AgentConnector(target)).UsingGrowingEnd(lifespan.end));
                            break;
                        // Unbridled Tempest
                        case UnbridledTempest:
                            castDuration = 5000;
                            long delay = 3000; // casttime 0 from skill def
                            uint radius = 1200;
                            lifespan = (cast.Time, cast.Time + delay);
                            (long start, long end) lifespanShockwave = (lifespan.end, cast.Time + castDuration);
                            GeographicalConnector connector = new AgentConnector(target);
                            replay.Decorations.Add(new CircleDecoration(radius, lifespan, Colors.Orange, 0.2, connector));
                            replay.Decorations.Add(new CircleDecoration(radius, (lifespan.end - 10, lifespan.end + 100), Colors.Orange, 0.5, connector));
                            replay.Decorations.AddShockwave(connector, lifespanShockwave, Colors.Grey, 0.7, radius);
                            break;
                        default:
                            break;
                    }
                }

                // Repulsion Field
                var repulsionFields = target.GetBuffStatus(log, RepulsionField).Where(x => x.Value > 0);
                replay.Decorations.AddOverheadIcons(repulsionFields, target, BuffImages.TargetedLocust);

                // Ion Shield
                var ionShields = target.GetBuffStatus(log, IonShield).Where(x => x.Value > 0);
                replay.Decorations.AddOverheadIcons(repulsionFields, target, BuffImages.IonShield);
                break;
            case (int)TargetID.BigKillerTornado:
                replay.Decorations.Add(new CircleDecoration(480, lifespan, Colors.LightOrange, 0.4, new AgentConnector(target)));
                break;
            case (int)TargetID.SmallKillerTornado:
                replay.Decorations.Add(new CircleDecoration(120, lifespan, Colors.LightOrange, 0.4, new AgentConnector(target)));
                break;
            case (int)TargetID.SmallJumpyTornado:
            case (int)TargetID.ParalyzingWisp:
            case (int)TargetID.VoltaicWisp:
                break;
            // Placeholder decorations for plateforms
            case (int)TargetID.SabirMainPlateform:
                var mainPlateformOpacities = new List<ParametricPoint1D> { new(0, target.FirstAware) };
                var plateformPosition = replay.Positions.Last();
                foreach (var sabir in log.AgentData.GetNPCsByID(TargetID.Sabir))
                {
                    var positions = log.FindActor(sabir).GetCombatReplayNonPolledPositions(log);
                    foreach (var position in positions)
                    {
                        if (Math.Abs(position.XYZ.Z - plateformPosition.XYZ.Z) < 200 && mainPlateformOpacities.Last().X != 1)
                        {
                            mainPlateformOpacities.Add(new(1, position.Time - 8000));

                        } 
                        else if (Math.Abs(position.XYZ.Z - plateformPosition.XYZ.Z) >= 200 && mainPlateformOpacities.Last().X != 0)
                        {
                            mainPlateformOpacities.Add(new(0, Math.Min(position.Time + 20000, sabir.LastAware)));
                        }
                    }
                }
                var successSabirPhase = log.LogData.GetPhases(log).OfType<EncounterPhaseData>().LastOrDefault(x => x.LogID == LogID && x.Success);
                if (successSabirPhase != null)
                {
                    mainPlateformOpacities.Add(new(1, successSabirPhase.End));
                }
                AddPlateformDecoration(target, replay, ParserIcons.SabirMainPlatform, 1660, mainPlateformOpacities);
                break;
            case (int)TargetID.SabirSquarePlateform:
                AddSmallPlateformDecoration(target, replay, ParserIcons.SabirSquarePlateform, 580);
                break;
            case (int)TargetID.SabirRectanglePlateform:
                AddSmallPlateformDecoration(target, replay, ParserIcons.SabirRectanglePlateform, 800);
                break;
            case (int)TargetID.SabirBigRectanglePlateform:
                AddSmallPlateformDecoration(target, replay, ParserIcons.SabirBigRectanglePlateform, 1640);
                break;
            default:
                break;

        }
    }

    private static void AddPlateformDecoration(SingleActor plateform, CombatReplay replay, string imageUrl, uint height, IReadOnlyList<ParametricPoint1D> opacities)
    {
        var plateformDecoration = new BackgroundIconDecoration(
            imageUrl, 0, height,
            opacities, replay.Positions.Select(x => new ParametricPoint1D(x.XYZ.Z, x.Time)),
            (plateform.FirstAware, plateform.LastAware),
            new AgentConnector(plateform)
        );
        RotationConnector plateformRotationConnector = new AgentFacingConnector(plateform, 180, AgentFacingConnector.RotationOffsetMode.AddToMaster);
        replay.Decorations.Add(plateformDecoration.UsingRotationConnector(plateformRotationConnector));
    }

    private static void AddSmallPlateformDecoration(SingleActor plateform, CombatReplay replay, string imageUrl, uint height)
    {
        var smallPlateformOpacities = new List<ParametricPoint1D> { new(1, plateform.FirstAware) };
        AddPlateformDecoration(plateform, replay, imageUrl, height, smallPlateformOpacities);
    }

    internal static void FindPlateforms(AgentData agentData)
    {
        // Disabled until we get nice looking assets for them
        //return;
        foreach (var candidate in agentData.GetAgentByType(AgentItem.AgentType.Gadget))
        {
            switch (candidate.HitboxWidth)
            {
                case 2350:
                    candidate.OverrideID(TargetID.SabirMainPlateform, agentData);
                    candidate.OverrideType(AgentItem.AgentType.NPC, agentData);
                    break;
                case 806:
                    candidate.OverrideID(TargetID.SabirSquarePlateform, agentData);
                    candidate.OverrideType(AgentItem.AgentType.NPC, agentData);
                    break;
                case 950:
                    candidate.OverrideID(TargetID.SabirRectanglePlateform, agentData);
                    candidate.OverrideType(AgentItem.AgentType.NPC, agentData);
                    break;
                case 1752:
                    candidate.OverrideID(TargetID.SabirBigRectanglePlateform, agentData);
                    candidate.OverrideType(AgentItem.AgentType.NPC, agentData);
                    break;
            }
        }
    }
    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        FindPlateforms(agentData);
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);
    }
    internal override long GetLogOffset(EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData)
    {
        // Find target
        if (!agentData.TryGetFirstAgentItem(TargetID.Sabir, out var sabir))
        {
            throw new MissingKeyActorsException("Sabir not found");
        }
        CombatItem? enterCombat = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.EnterCombat && x.SrcMatchesAgent(sabir));
        if (enterCombat == null)
        {
            CombatItem? logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.LogNPCUpdate);
            if (logStartNPCUpdate == null)
            {
                return GetGenericLogOffset(logData);
            }
            else
            {
                CombatItem? firstDamageEvent = combatData.FirstOrDefault(x => x.DstMatchesAgent(sabir) && x.IsDamagingDamage());
                if (firstDamageEvent != null)
                {
                    return firstDamageEvent.Time;
                }
                else
                {
                    return logData.EvtcLogEnd;
                }
            }
        }
        return enterCombat.Time;
    }

    internal override LogData.LogMode GetLogMode(CombatData combatData, AgentData agentData, LogData logData)
    {
        SingleActor target = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Sabir)) ?? throw new MissingKeyActorsException("Sabir not found");
        return (target.GetHealth(combatData) > 32e6) ? LogData.LogMode.CM : LogData.LogMode.Normal;
    }
}
