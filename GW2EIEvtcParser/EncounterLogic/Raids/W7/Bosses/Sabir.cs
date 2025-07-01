using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class Sabir : TheKeyOfAhdashim
{
    internal readonly MechanicGroup Mechanics = new MechanicGroup([
            new MechanicGroup([
                new PlayerDstSkillMechanic(DireDrafts, new MechanicPlotlySetting(Symbols.Circle,Colors.Orange), "B.Tornado", "Hit by big tornado", "Big Tornado Hit", 500)
                    .UsingChecker((de, log) => de.HasDowned || de.HasKilled),
                new PlayerDstSkillMechanic(UnbridledTempest, new MechanicPlotlySetting(Symbols.Hexagon,Colors.Pink), "Shockwave", "Hit by Shockwave", "Shockwave Hit", 0)
                    .UsingChecker((de, log) => de.HasDowned || de.HasKilled),
                new PlayerDstSkillMechanic(FuryOfTheStorm, new MechanicPlotlySetting(Symbols.Circle,Colors.Purple), "Arena AoE", "Hit by Arena wide AoE", "Arena AoE hit", 0)
                    .UsingChecker( (de, log) => de.HasDowned || de.HasKilled ),
            ]),
            new MechanicGroup([
                new PlayerDstHitMechanic([ DynamicDeterrentNM, DynamicDeterrentCM ], new MechanicPlotlySetting(Symbols.YUpOpen,Colors.Pink), "Pushed", "Pushed by rotating breakbar", "Pushed", 0)
                .UsingChecker((de, log) => !de.To.HasBuff(log, Stability, de.Time - ServerDelayConstant)),
                new EnemyCastStartMechanic([ DynamicDeterrentNM, DynamicDeterrentCM ], new MechanicPlotlySetting(Symbols.Star, Colors.Yellow), "Dynamic Deterrent", "Casted Dynamic Deterrent", "Cast Dynamic Deterrent", 0),
            ]),
            new PlayerDstHitMechanic([ StormsEdgeLeftHand, StormsEdgeRightHand ], new MechanicPlotlySetting(Symbols.BowtieOpen, Colors.Blue), "Storm's Edge", "Hit by Storm's Edge", "Storm's Edge", 0),
            new PlayerDstHitMechanic(ChainLightning, new MechanicPlotlySetting(Symbols.HexagonOpen, Colors.White), "Chain Lightning", "Hit by Chain Lightning", "Chain Lightning Hit", 0),
            new MechanicGroup([
                new PlayerDstHitMechanic(Electrospark, new MechanicPlotlySetting(Symbols.CircleCross, Colors.Orange), "Electrospark", "Hit by Electrospark", "Electrospark", 0),
                new PlayerDstHitMechanic(Electrospark, new MechanicPlotlySetting(Symbols.CircleCrossOpen, Colors.Orange), "Charged Winds", "Achievement Elegibility: Charged Winds", "Charged Winds", 0)
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
        EncounterCategoryInformation.InSubCategoryOrder = 0;
        EncounterID |= 0x000002;
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        return
        [
            TargetID.ParalyzingWisp,
            TargetID.VoltaicWisp,
            TargetID.SmallKillerTornado,
            TargetID.SmallJumpyTornado,
            TargetID.BigKillerTornado
        ];
    }

    internal override FightLogic AdjustLogic(AgentData agentData, List<CombatItem> combatData, EvtcParserSettings parserSettings)
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
                    BuffRemoveAllEvent? buffRemove = combatData.GetBuffRemoveAllData(ViolentCurrents)
                        .Where(x => Math.Abs(effect.Time - x.Time) < ServerDelayConstant && x.To == effect.Src)
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

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Sabir)) ?? throw new MissingKeyActorsException("Sabir not found");
        phases[0].AddTarget(mainTarget, log);
        if (!requirePhases)
        {
            return phases;
        }

        var casts = mainTarget.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd);
        var wallopingWinds = casts.Where(x => x.SkillID == WallopingWind);
        long start = 0;
        int i = 0;
        foreach (var wallopingWind in wallopingWinds)
        {
            var phase = new PhaseData(start, wallopingWind.Time, "Phase " + (i + 1));
            phase.AddParentPhase(phases[0]);
            phase.AddTarget(mainTarget, log);
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
            var phase = new PhaseData(start, log.FightData.FightEnd, "Phase " + (i + 1));
            phase.AddParentPhase(phases[0]);
            phase.AddTarget(mainTarget, log);
            phases.Add(phase);
        }

        return phases;
    }

    protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
    {
        return new CombatReplayMap(CombatReplaySabir,
                        (1000, 910),
                        (-14122, 142, -9199, 4640)/*,
                        (-21504, -21504, 24576, 24576),
                        (33530, 34050, 35450, 35970)*/);
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputePlayerCombatReplayActors(p, log, replay);
        var boltBreaks = p.GetBuffStatus(log, BoltBreak, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0);
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
                foreach (CastEvent cast in target.GetAnimatedCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd))
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
                var repulsionFields = target.GetBuffStatus(log, RepulsionField, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0);
                replay.Decorations.AddOverheadIcons(repulsionFields, target, BuffImages.TargetedLocust);

                // Ion Shield
                var ionShields = target.GetBuffStatus(log, IonShield, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0);
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
            default:
                break;

        }
    }
    internal override long GetFightOffset(EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData)
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
                return GetGenericFightOffset(fightData);
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
                    return fightData.LogEnd;
                }
            }
        }
        return enterCombat.Time;
    }

    internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
    {
        SingleActor target = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Sabir)) ?? throw new MissingKeyActorsException("Sabir not found");
        return (target.GetHealth(combatData) > 32e6) ? FightData.EncounterMode.CM : FightData.EncounterMode.Normal;
    }
}
