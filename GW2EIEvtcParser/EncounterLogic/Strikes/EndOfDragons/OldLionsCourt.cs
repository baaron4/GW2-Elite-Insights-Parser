using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class OldLionsCourt : EndOfDragonsStrike
{
    public OldLionsCourt(int triggerID) : base(triggerID)
    {
        MechanicList.Add(new MechanicGroup([
            new PlayerDstHitMechanic([BoilingAetherRedBlueNM, BoilingAetherRedBlueCM], "Boiling Aether (Vermilion & Indigo)", new MechanicPlotlySetting(Symbols.Circle, Colors.LightRed), "Red.VermIndi.H", "Hit by Boiling Aether (Vermilion & Indigo)", "Boiling Aether Hit (Vermilion & Indigo)", 0),
            // Vermilion
            new MechanicGroup([
                new PlayerDstBuffApplyMechanic(FixatedOldLionsCourt, "Fixated (Vermilion)", new MechanicPlotlySetting(Symbols.Diamond, Colors.Red), "Fix.Verm.A", "Fixated Applied", "Fixated Applied", 0)
                    .UsingChecker((bae, log) => bae.CreditedBy.IsAnySpecies([TargetID.PrototypeVermilion, TargetID.PrototypeVermilionCM])),
                new MechanicGroup([
                    new MechanicGroup([
                        new PlayerDstHitMechanic([DualHorizon, DualHorizonCM], "Dual Horizon", new MechanicPlotlySetting(Symbols.CircleOpenDot, Colors.LightRed), "DualHrz.H", "Hit by Dual Horizon", "Dual Horizon Hit", 0),
                        new EnemyCastStartMechanic([DualHorizon, DualHorizonCM], "Dual Horizon", new MechanicPlotlySetting(Symbols.CircleOpenDot, Colors.Red), "DualHrz.C", "Casted Dual Horizon", "Dual Horizon Cast", 0),
                    ]),
                    new PlayerDstBuffApplyMechanic([TidalTorment, TidalTormentCM], "Tidal Torment", new MechanicPlotlySetting(Symbols.Star, Colors.Red), "TidTorm.A", "Tidal Torment Applied", "Tidal Torment Applied", 0),
                    new PlayerDstBuffApplyMechanic([ErgoShear, ErgoShearCM], "Ergo Shear", new MechanicPlotlySetting(Symbols.StarOpen, Colors.Red), "ErgShr.A", "Ergo Shear Applied", "Ergo Shear Applied", 0),
                    new EnemySrcEffectMechanic(EffectGUIDs.OldLionsCourtGravitationalWave, "Gravitational Wave", new MechanicPlotlySetting(Symbols.CircleOpen, Colors.Red), "GravWave.C", "Casted Gravitational Wave", "Gravitational Wave", 0),
                ]),
                new PlayerDstBuffApplyMechanic(Spaghettification, "Spaghettification", new MechanicPlotlySetting(Symbols.Bowtie, Colors.DarkRed), "Spgt.H", "Hit by Spaghettification", "Spaghettification Hit", 0),
                new PlayerDstSkillMechanic(ExhaustPlume, "Exhaust Plume", new MechanicPlotlySetting(Symbols.TriangleDown, Colors.Red), "VermFall.H", "Hit by Exhaust Plume (Vermilion Fall)", "Exhaust Plume Hit (Vermilion)", 150)
                    .UsingChecker((de, log) => de.CreditedFrom.IsAnySpecies(new List<TargetID> { TargetID.PrototypeVermilion, TargetID.PrototypeVermilionCM })),
            ]),
            // Arsenite
            new MechanicGroup([
                new PlayerDstBuffApplyMechanic(FixatedOldLionsCourt, "Fixated (Arsenite)", new MechanicPlotlySetting(Symbols.Diamond, Colors.Green), "Fix.Arse.A", "Fixated Applied", "Fixated Applied", 0)
                    .UsingChecker((bae, log) => bae.CreditedBy.IsAnySpecies([TargetID.PrototypeArsenite, TargetID.PrototypeArseniteCM])),
                new PlayerDstHitMechanic([BoilingAetherGreenNM, BoilingAetherGreenCM], "Boiling Aether (Arsenite)", new MechanicPlotlySetting(Symbols.Circle, Colors.DarkRed), "Red.Arse.H", "Hit by Boiling Aether (Arsenite)", "Boiling Aether Hit (Arsenite)", 0),
                new MechanicGroup([
                    new PlayerDstHitMechanic([NoxiousVaporBlade, NoxiousVaporBladeCM], "Noxious Vapor Blade", new MechanicPlotlySetting(Symbols.CircleXOpen, Colors.Green), "BladeOut.H", "Hit by Noxious Vapor Blade (to player)", "Noxious Vapor Blade Hit", 150),
                    new PlayerDstHitMechanic([NoxiousReturn, NoxiousReturnCM], "Noxious Return", new MechanicPlotlySetting(Symbols.CircleX, Colors.Green), "BladeBack.H", "Hit by Noxious Return (to Arsenite)", "Noxious Return Hit", 150),
                    new PlayerDstBuffApplyMechanic(NoxiousVaporBladeTargetBuff, "Noxious Vapor Blade", new MechanicPlotlySetting(Symbols.CircleCross, Colors.Green), "Blade.A", "Targeted for Noxious Vapor Blade", "Noxious Vapor Blade Target", 0),
                ]),
                new MechanicGroup([
                    new PlayerDstHitMechanic([PerniciousVortex, PerniciousVortexCM], "Pernicious Vortex", new MechanicPlotlySetting(Symbols.CircleX, Colors.DarkGreen), "PernVort.H", "Hit by Pernicious Vortex (Pull)", "Pernicious Vortex Hit", 0),
                    new EnemyCastStartMechanic([PerniciousVortexSkillNM, PerniciousVortexSkillCM], "Pernicious Vortex", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Green), "PrnVrx.C", "Casted Pernicious Vortex", "Pernicious Vortex Cast", 0),
                ]),
                new PlayerDstBuffApplyMechanic(Dysapoptosis, "Dysapoptosis", new MechanicPlotlySetting(Symbols.BowtieOpen, Colors.DarkRed), "Dysp.H", "Hit by Dysapoptosis", "Dysapoptosis Hit", 0),
                new PlayerDstSkillMechanic(ExhaustPlume, "Exhaust Plume", new MechanicPlotlySetting(Symbols.TriangleDown, Colors.Green), "ArseFall.H", "Hit by Exhaust Plume (Arsenite Fall)", "Exhaust Plume Hit (Arsenite)", 150)
                    .UsingChecker((de, log) => de.CreditedFrom.IsAnySpecies(new List<TargetID> { TargetID.PrototypeArsenite, TargetID.PrototypeArseniteCM })),
            ]),
            // Indigo
            new MechanicGroup([
                new PlayerDstBuffApplyMechanic(FixatedOldLionsCourt, "Fixated (Indigo)", new MechanicPlotlySetting(Symbols.Diamond, Colors.Blue), "Fix.Indi.A", "Fixated Applied", "Fixated Applied", 0)
                    .UsingChecker((bae, log) => bae.CreditedBy.IsAnySpecies([TargetID.PrototypeIndigo, TargetID.PrototypeIndigoCM])),
                new PlayerDstHitMechanic([TriBolt, TriBoltCM], "Tri Bolt", new MechanicPlotlySetting(Symbols.Circle, Colors.LightOrange), "TriBolt.H", "Hit by Tri Bolt (Spread AoEs)", "Tri Bolt Hit", 150),
                new PlayerDstHitMechanic([Tribocharge, TribochargeCM], "Tribocharge", new MechanicPlotlySetting(Symbols.CircleCrossOpen, Colors.LightOrange), "TriChg.H", "Hit by Tribocharge", "Tribocharge Hit", 150),
                new MechanicGroup([
                    new PlayerDstHitMechanic([CracklingWind, CracklingWindCM], "Crackling Wind", new MechanicPlotlySetting(Symbols.Hexagon, Colors.CobaltBlue), "CrackWind.H", "Hit by Crackling Wind (Push)", "Crackling Wind Hit", 0),
                    new EnemyCastStartMechanic([CracklingWindSkillNM, CracklingWindSkillCM], "Crackling Wind", new MechanicPlotlySetting(Symbols.Star, Colors.Blue), "CrckWind.C", "Casted Crackling Wind", "Cracking Wind Cast", 0),
                ]),
                new PlayerDstBuffApplyMechanic(ThunderingUltimatum, "Thundering Ultimatum", new MechanicPlotlySetting(Symbols.TriangleDown, Colors.DarkRed), "ThunUlti.H", "Hit by Thundering Ultimatum", "Thunderin gUltimatum Hit", 0),
                new PlayerDstSkillMechanic(ExhaustPlume, "Exhaust Plume", new MechanicPlotlySetting(Symbols.TriangleDown, Colors.Blue), "IndiFall.H", "Hit by Exhaust Plume (Indigo Fall)", "Exhaust Plume Hit (Indigo)", 150)
                    .UsingChecker((de, log) => de.CreditedFrom.IsAnySpecies(new List<TargetID> { TargetID.PrototypeIndigo, TargetID.PrototypeIndigoCM })),
            ]),
            new PlayerDstHitMechanic([BoilingAetherRedBlueNM, BoilingAetherRedBlueCM, BoilingAetherGreenNM, BoilingAetherGreenCM], "Boiling Aether", new MechanicPlotlySetting(Symbols.CircleCrossOpen, Colors.Red), "AethAver.Achiv", "Achievement Eligibility: Aether Aversion", "Achiv Aether Aversion", 150)
                .UsingAchievementEligibility(true),
            new EnemyDstBuffApplyMechanic(EmpoweredWatchknightTriumverate, "Empowered", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Blue), "Empowered.A", "Knight gained Empowered", "Empowered Applied", 0),
            new EnemyDstBuffApplyMechanic(PowerTransfer, "Power Transfer", new MechanicPlotlySetting(Symbols.TriangleRight, Colors.Blue), "PwrTrns.A", "Knight gained Power Transfer", "Power Transfer Applied", 0),
            new EnemyDstBuffApplyMechanic(LeyWovenShielding, "Ley-Woven Shielding", new MechanicPlotlySetting(Symbols.Pentagon, Colors.Teal), "WovShld.A", "Knight gained Ley-Woven Shielding", "Ley-Woven Shielding Applied", 0),
            new EnemyDstBuffApplyMechanic(MalfunctioningLeyWovenShielding, "Malfunctioning Ley-Woven Shielding", new MechanicPlotlySetting(Symbols.PentagonOpen, Colors.DarkTeal), "MalfWovShld.A", "Knight gained Malfunctioning Ley-Woven Shielding", "Malfunctioning Ley-Woven Shielding Applied", 0),
            new EnemyDstBuffApplyMechanic(Exposed31589, "Exposed (Knight)", new MechanicPlotlySetting(Symbols.HexagonOpen, Colors.Purple), "Expo.A", "Exposed Applied to Knight", "Exposed Applied to Knight", 0),
        ])
        );
        Icon = EncounterIconOldLionsCourt;
        Extension = "lioncourt";
        EncounterCategoryInformation.InSubCategoryOrder = 4;
        EncounterID |= 0x000005;
    }

    protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
    {
        return new CombatReplayMap(CombatReplayOldLionsCourt,
                        (1008, 1008),
                        (-1420, 3010, 1580, 6010));
    }
    protected override ReadOnlySpan<int> GetTargetsIDs()
    {
        return
        [
            (int)TargetID.PrototypeVermilion,
            (int)TargetID.PrototypeIndigo,
            (int)TargetID.PrototypeArsenite,
            (int)TargetID.PrototypeVermilionCM,
            (int)TargetID.PrototypeIndigoCM,
            (int)TargetID.PrototypeArseniteCM,
        ];
    }

    protected override List<TrashID> GetTrashMobsIDs()
    {
        return [ TrashID.Tribocharge ];
    }

    protected override List<int> GetSuccessCheckIDs()
    {
        return [ ];
    }

    internal override FightData.EncounterStartStatus GetEncounterStartStatus(CombatData combatData, AgentData agentData, FightData fightData)
    {
        // Can be improved
        if (fightData.IsCM)
        {
            if (TargetHPPercentUnderThreshold(TargetID.PrototypeVermilionCM, fightData.FightStart, combatData, Targets) ||
                TargetHPPercentUnderThreshold(TargetID.PrototypeIndigoCM, fightData.FightStart, combatData, Targets) ||
                TargetHPPercentUnderThreshold(TargetID.PrototypeArseniteCM, fightData.FightStart, combatData, Targets))
            {
                return FightData.EncounterStartStatus.Late;
            }

        }
        else
        {
            if (TargetHPPercentUnderThreshold(TargetID.PrototypeVermilion, fightData.FightStart, combatData, Targets) ||
                TargetHPPercentUnderThreshold(TargetID.PrototypeIndigo, fightData.FightStart, combatData, Targets) ||
                TargetHPPercentUnderThreshold(TargetID.PrototypeArsenite, fightData.FightStart, combatData, Targets))
            {
                return FightData.EncounterStartStatus.Late;
            }
        }
        return FightData.EncounterStartStatus.Normal;
    }

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        base.CheckSuccess(combatData, agentData, fightData, playerAgents);
        if (!fightData.Success)
        {
            List<int> idsToCheck;
            if (fightData.IsCM)
            {
                idsToCheck =
                [
                    (int)TargetID.PrototypeVermilionCM,
                    (int)TargetID.PrototypeIndigoCM,
                    (int)TargetID.PrototypeArseniteCM,
                ];
            }
            else
            {
                idsToCheck =
                [
                    (int)TargetID.PrototypeVermilion,
                    (int)TargetID.PrototypeIndigo,
                    (int)TargetID.PrototypeArsenite,
                ];
            }
            SetSuccessByDeath(Targets.Where(x => idsToCheck.Contains(x.ID)), combatData, fightData, playerAgents, true);
        }
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData)
    {
        return "Old Lion's Court";
    }

    protected override ReadOnlySpan<int> GetUniqueNPCIDs()
    {
        return
        [
            (int)TargetID.PrototypeVermilion,
            (int)TargetID.PrototypeIndigo,
            (int)TargetID.PrototypeArsenite,
            (int)TargetID.PrototypeVermilionCM,
            (int)TargetID.PrototypeIndigoCM,
            (int)TargetID.PrototypeArseniteCM,
        ];
    }

    private SingleActor? Vermilion()
    {
        return Targets.FirstOrDefault(x => x.IsSpecies(TargetID.PrototypeVermilionCM)) ?? Targets.FirstOrDefault(x => x.IsSpecies(TargetID.PrototypeVermilion));
    }
    private SingleActor? Indigo()
    {
        return Targets.FirstOrDefault(x => x.IsSpecies(TargetID.PrototypeIndigoCM)) ?? Targets.FirstOrDefault(x => x.IsSpecies(TargetID.PrototypeIndigo));
    }
    private SingleActor? Arsenite()
    {
        return Targets.FirstOrDefault(x => x.IsSpecies(TargetID.PrototypeArseniteCM)) ?? Targets.FirstOrDefault(x => x.IsSpecies(TargetID.PrototypeArsenite));
    }

    private static List<PhaseData> GetSubPhases(SingleActor target, ParsedEvtcLog log, string phaseName)
    {
        DeadEvent? dead = log.CombatData.GetDeadEvents(target.AgentItem).LastOrDefault();
        long end = log.FightData.FightEnd;
        long start = log.FightData.FightStart;
        if (dead != null && dead.Time < end)
        {
            end = dead.Time;
        }
        List<PhaseData> subPhases = GetPhasesByInvul(log, new[] { LeyWovenShielding, MalfunctioningLeyWovenShielding }, target, false, true, start, end);
        string[] phaseNames;
        if (log.FightData.IsCM)
        {
            if (subPhases.Count > 3)
            {
                return [];
            }
            phaseNames =
            [
                phaseName + " 100% - 60%",
                phaseName + " 60% - 20%",
                phaseName + " 20% - 0%",
            ];
        }
        else
        {
            if (subPhases.Count > 4)
            {
                return [];
            }
            phaseNames =
            [
                phaseName + " 100% - 80%",
                phaseName + " 80% - 40%",
                phaseName + " 40% - 10%",
                phaseName + " 10% - 0%",
            ];
        }
        for (int i = 0; i < subPhases.Count; i++)
        {
            subPhases[i].Name = phaseNames[i];
            subPhases[i].AddTarget(target);
        }
        return subPhases;
    }

    internal override long GetFightOffset(EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData)
    {
        long startToUse = base.GetFightOffset(evtcVersion, fightData, agentData, combatData);
        AgentItem? vermilion = agentData.GetNPCsByID(TargetID.PrototypeVermilionCM).FirstOrDefault() ?? agentData.GetNPCsByID(TargetID.PrototypeVermilion).FirstOrDefault();
        if (vermilion != null)
        {
            CombatItem? breakbarStateActive = combatData.FirstOrDefault(x => x.SrcMatchesAgent(vermilion) && x.IsStateChange == StateChange.BreakbarState && BreakbarStateEvent.GetBreakbarState(x) == BreakbarState.Active);
            if (breakbarStateActive != null)
            {
                startToUse = breakbarStateActive.Time;
            }
        }
        return startToUse;
    }

    internal override List<BuffEvent> SpecialBuffEventProcess(CombatData combatData, SkillData skillData)
    {
        List<BuffEvent> toAdd = base.SpecialBuffEventProcess(combatData, skillData);
        var shields = combatData.GetBuffData(LeyWovenShielding).GroupBy(x => x.To);
        foreach (var group in shields)
        {
            // Missing Buff Initial
            if (group.FirstOrDefault() is AbstractBuffRemoveEvent)
            {
                toAdd.Add(new BuffApplyEvent(group.Key, group.Key, group.Key.FirstAware, int.MaxValue, skillData.Get(LeyWovenShielding), IFF.Friend, 1, true));
            }
        }
        return toAdd;
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor? vermilion = Vermilion();
        bool canComputePhases = vermilion != null && vermilion.HasBuff(log, LeyWovenShielding, 500); // check that vermilion is present and starts shielded, otherwise clearly incomplete log
        if (vermilion != null)
        {
            phases[0].AddTarget(vermilion);
            if (canComputePhases)
            {
                phases.AddRange(GetSubPhases(vermilion, log, "Vermilion"));
            }
        }
        SingleActor? indigo = Indigo();
        if (indigo != null)
        {
            phases[0].AddTarget(indigo);
            if (canComputePhases)
            {
                phases.AddRange(GetSubPhases(indigo, log, "Indigo"));
            }
        }
        SingleActor? arsenite = Arsenite();
        if (arsenite != null)
        {
            phases[0].AddTarget(arsenite);
            if (canComputePhases)
            {
                phases.AddRange(GetSubPhases(arsenite, log, "Arsenite"));
            }
        }
        return phases;
    }

    internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
    {
        SingleActor target = (Vermilion() ?? Indigo() ?? Arsenite()) ?? throw new MissingKeyActorsException("Main target not found");
        return target.GetHealth(combatData) > 20e6 ? FightData.EncounterMode.CM : FightData.EncounterMode.Normal;
    }

    protected override void SetInstanceBuffs(ParsedEvtcLog log)
    {
        base.SetInstanceBuffs(log);

        if (log.FightData.Success && log.CombatData.GetBuffData(AchievementEligibilityFearNotThisKnight).Any())
        {
            InstanceBuffs.MaybeAdd(GetOnPlayerCustomInstanceBuff(log, AchievementEligibilityFearNotThisKnight));
        }
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputePlayerCombatReplayActors(p, log, replay);
        // Fixation
        IEnumerable<BuffEvent> fixations = log.CombatData.GetBuffDataByIDByDst(FixatedOldLionsCourt, p.AgentItem);
        IEnumerable<BuffEvent> fixatedVermillion = fixations.Where(bae => bae.CreditedBy.IsAnySpecies(new List<TargetID> { TargetID.PrototypeVermilion, TargetID.PrototypeVermilionCM }));
        IEnumerable<BuffEvent> fixatedArsenite = fixations.Where(bae => bae.CreditedBy.IsAnySpecies(new List<TargetID> { TargetID.PrototypeArsenite, TargetID.PrototypeArseniteCM }));
        IEnumerable<BuffEvent> fixatedIndigo = fixations.Where(bae => bae.CreditedBy.IsAnySpecies(new List<TargetID> { TargetID.PrototypeIndigo, TargetID.PrototypeIndigoCM }));

        AddFixatedDecorations(p, log, replay, fixatedVermillion, ParserIcons.FixationRedOverhead);
        AddFixatedDecorations(p, log, replay, fixatedArsenite, ParserIcons.FixationGreenOverhead);
        AddFixatedDecorations(p, log, replay, fixatedIndigo, ParserIcons.FixationBlueOverhead);

        // Noxious Vapor Blade
        // The buff is applied from Arsenite to Player, lasts 2000ms.
        // In game, the green tether lasts for the entire duration of the blade, meanwhile the buff on the player displays the green border overlay and is hidden.
        // In the log, the tether effect can't be found, so this decoration is only indicative of who has been targeted, the duration is not correct.
        var noxiousBlade = GetFilteredList(log.CombatData, NoxiousVaporBladeTargetBuff, p, true, true);
        replay.Decorations.AddTether(noxiousBlade, Colors.Green, 0.5);

        // Tri-Bolt
        if (log.CombatData.TryGetEffectEventsByDstWithGUID(p.AgentItem, EffectGUIDs.OldLionsCourtTriBoltSpread, out var tribolt))
        {
            foreach (EffectEvent effect in tribolt)
            {
                (long start, long end) lifespan = effect.ComputeLifespan(log, 2000);
                replay.Decorations.AddWithGrowing(new CircleDecoration(220, lifespan, Colors.LightOrange, 0.2, new AgentConnector(effect.Dst)), lifespan.end);
            }
        }
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        switch (target.ID)
        {
            case (int)TargetID.PrototypeVermilion:
            case (int)TargetID.PrototypeVermilionCM:
                // Spaghettification Start
                if (log.CombatData.TryGetEffectEventsBySrcWithGUIDs(target.AgentItem, [EffectGUIDs.OldLionsCourtSpaghettificationDoughnutStart , EffectGUIDs.OldLionsCourtSpaghettificationCircleFlipped], out var spaghettificationStart))
                {

                    foreach (EffectEvent effect in spaghettificationStart)
                    {
                        (long start, long end) lifespan = effect.HasDynamicEndTime ? effect.ComputeDynamicLifespan(log, 30000) : effect.ComputeLifespan(log, 1500);
                        FormDecoration decoration;
                        if (effect.GUIDEvent.ContentGUID == EffectGUIDs.OldLionsCourtSpaghettificationDoughnutStart)
                        {
                            decoration = new DoughnutDecoration(600, 2000, lifespan, Colors.LightOrange, 0.2, new PositionConnector(effect.Position));
                        } 
                        else
                        {
                            decoration = new CircleDecoration(600, lifespan, Colors.LightOrange, 0.2, new PositionConnector(effect.Position));
                        }
                        replay.Decorations.Add(decoration);
                    }
                }
                if (log.CombatData.TryGetEffectEventsBySrcWithGUIDs(target.AgentItem, [EffectGUIDs.OldLionsCourtSpaghettificationDoughnutDetonation, EffectGUIDs.OldLionsCourtSpaghettificationCircleDetonation], out var spaghettificationDetonation))
                {
                    foreach (EffectEvent effect in spaghettificationDetonation)
                    {
                        (long start, long end) lifespan = effect.ComputeLifespan(log, 1500); // Override 0 duration
                        FormDecoration decoration;
                        if (effect.GUIDEvent.ContentGUID == EffectGUIDs.OldLionsCourtSpaghettificationDoughnutDetonation)
                        {
                            decoration = new DoughnutDecoration(600, 2000, lifespan, Colors.Red, 0.2, new PositionConnector(effect.Position));
                        }
                        else
                        {
                            decoration = new CircleDecoration(600, lifespan, Colors.Red, 0.2, new PositionConnector(effect.Position));
                        }
                        replay.Decorations.Add(decoration);
                    }
                }

                // Safe Zone - Semi Circle
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.OldLionsCourtSpaghettificationSafeZoneSemiCircle, out var safeZoneSemiCircle))
                {
                    foreach (EffectEvent effect in safeZoneSemiCircle)
                    {
                        (long start, long end) lifespan = effect.ComputeDynamicLifespan(log, 30000);
                        var rotation = new AngleConnector(effect.Rotation.Z + 90);
                        var circle = (PieDecoration)new PieDecoration(600, 180, lifespan, Colors.White, 0.2, new PositionConnector(effect.Position)).UsingRotationConnector(rotation);
                        replay.Decorations.Add(circle);
                    }
                }

                // Safe Zone - Full Circle
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.OldLionsCourtSpaghettificationSafeZoneFullCircle, out var safeZoneFullCircle))
                {
                    foreach (EffectEvent effect in safeZoneFullCircle)
                    {
                        (long start, long end) lifespan = effect.ComputeDynamicLifespan(log, 30000);
                        var circle = new CircleDecoration(600, lifespan, Colors.White, 0.2, new PositionConnector(effect.Position));
                        replay.Decorations.Add(circle);
                    }
                }

                // Dual Horizon - Orange Doughnut
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.OldLionsCourtDualHorizonOrange, out var dualHorizons))
                {
                    foreach (EffectEvent effect in dualHorizons)
                    {
                        (long start, long end) lifespan = effect.ComputeLifespan(log, 4100);
                        var orangeDoughnut = new DoughnutDecoration(340, 440, lifespan, Colors.Orange, 0.2, new PositionConnector(effect.Position));
                        replay.Decorations.Add(orangeDoughnut);
                    }
                    if (log.CombatData.TryGetEffectEventsByGUIDs([EffectGUIDs.OldLionsCourtDualHorizonWhiteInner, EffectGUIDs.OldLionsCourtDualHorizonWhiteOuter], out var horizonWhite  ))
                    {
                        foreach (EffectEvent effect in horizonWhite)
                        {
                            (long start, long end) lifespan = effect.ComputeLifespan(log, 4000);
                            FormDecoration white;
                            if (effect.GUIDEvent.ContentGUID == EffectGUIDs.OldLionsCourtDualHorizonWhiteInner)
                            {
                                white = new DoughnutDecoration(300, 340, lifespan, Colors.White, 0.2, new PositionConnector(effect.Position));
                            } 
                            else
                            {
                                white = new DoughnutDecoration(440, 500, lifespan, Colors.White, 0.2, new PositionConnector(effect.Position));
                            }
                            replay.Decorations.Add(white);
                        }
                    }
                }

                // Gravity Hammer
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.OldLionsCourtGravityHammer, out var gravityHammer))
                {
                    foreach (EffectEvent effect in gravityHammer)
                    {
                        (long start, long end) lifespan = effect.ComputeLifespan(log, 1000);
                        var circle = new CircleDecoration(400, lifespan, Colors.LightOrange, 0.2, new PositionConnector(effect.Position));
                        replay.Decorations.Add(circle);
                    }
                }

                // Gravitational Wave
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.OldLionsCourtGravitationalWave, out var gravitationalWaves))
                {
                    foreach (EffectEvent effect in gravitationalWaves)
                    {
                        int duration = 3000; // Logged duration of 0
                        (long start, long end) lifespan = (effect.Time, effect.Time + duration);
                        uint radius = 3000; // Radius is an estimate
                        replay.Decorations.AddShockwave(new PositionConnector(effect.Position), lifespan, Colors.White, 0.3, radius);
                    }
                }

                // Boiling Aether Spawn Indicator
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.OldLionsCourtBoilingAetherSpawnIndicator, out var boilingAetherIndicators))
                {
                    foreach (EffectEvent effect in boilingAetherIndicators)
                    {
                        uint radius = 100; // The diameter is the size of the Knight's hitbox, which is 200.
                        (long start, long end) lifespan = effect.ComputeLifespan(log, 1190);
                        var circle = new CircleDecoration(radius, lifespan, Colors.LightOrange, 0.2, new PositionConnector(effect.Position));
                        replay.Decorations.AddWithGrowing(circle, lifespan.end);
                    }
                }

                // Hide when inactive
                replay.AddHideByBuff(target, log, Determined762);
                break;
            case (int)TargetID.PrototypeArsenite:
            case (int)TargetID.PrototypeArseniteCM:
                // Dysapoptosis Indicator
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.OldLionsCourtDysapoptosisIndicator, out var leftSemiCircle))
                {
                    foreach (EffectEvent effect in leftSemiCircle)
                    {
                        (long start, long end) lifespan = effect.HasDynamicEndTime ? effect.ComputeDynamicLifespan(log, 30000) : effect.ComputeLifespan(log, 1500);
                        var rotation = new AngleConnector(effect.Rotation.Z + 90); // Position incorrect, get Arsenite's position
                        if (target.TryGetCurrentPosition(log, effect.Time, out var position))
                        {
                            var pie = (PieDecoration)new PieDecoration(3000, 180, lifespan, Colors.LightOrange, 0.2, new PositionConnector(position)).UsingRotationConnector(rotation);
                            replay.Decorations.Add(pie);
                        }
                    }
                }

                // Dysapoptosis Detonation
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.OldLionsCourtDysapoptosisDetonation, out var detonation))
                {
                    foreach (EffectEvent effect in detonation)
                    {
                        (long start, long end) lifespan = effect.ComputeLifespan(log, 1500); // Override
                        var rotation = new AngleConnector(effect.Rotation.Z - 180);
                        var pie = (PieDecoration)new PieDecoration(3000, 180, lifespan, Colors.DarkGreen, 0.2, new PositionConnector(effect.Position)).UsingRotationConnector(rotation);
                        replay.Decorations.Add(pie);
                    }
                }

                // Pernicious Vortex - First Indicator - Orange Doughnuts
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.OldLionsCourtPerniciousVortexWarning1, out var vortexWarnings1))
                {
                    foreach (EffectEvent effect in vortexWarnings1)
                    {
                        (long start, long end) lifespan = effect.ComputeLifespan(log, 4000);
                        replay.Decorations.AddContrenticRings(0, 120, lifespan, effect.Position, Colors.LightOrange);
                    }
                }
                
                // Pernicious Vortex - Second Indicator - Red Ring
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.OldLionsCourtPerniciousVortexWarning2, out var vortexWarnings2))
                {
                    foreach (EffectEvent effect in vortexWarnings2)
                    {
                        (long start, long end) lifespan = effect.ComputeLifespan(log, 4000);
                        var circle = (CircleDecoration)new CircleDecoration(300, lifespan, Colors.Red, 0.3, new PositionConnector(effect.Position)).UsingFilled(false);
                        replay.Decorations.Add(circle);
                    }
                }

                // Pernicious Vortex - Damage Indicator
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.OldLionsCourtPerniciousVortexActive, out var vortexDamage))
                {
                    foreach (EffectEvent effect in vortexDamage)
                    {
                        (long start, long end) lifespan = effect.ComputeLifespan(log, 5000);
                        var circle = new CircleDecoration(300, lifespan, Colors.Red, 0.3, new PositionConnector(effect.Position));
                        replay.Decorations.Add(circle);
                    }
                }

                // Rupture
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.OldLionsCourtRuptureIndicator, out var ruptures))
                {
                    foreach (EffectEvent effect in ruptures)
                    {
                        (long start, long end) lifespan = effect.ComputeDynamicLifespan(log, 2000);
                        var circle = new CircleDecoration(180, lifespan, Colors.LightOrange, 0.2, new PositionConnector(effect.Position));
                        replay.Decorations.AddWithGrowing(circle, lifespan.end);
                    }
                }

                // Hide when inactive
                replay.AddHideByBuff(target, log, Determined762);
                break;
            case (int)TargetID.PrototypeIndigo:
            case (int)TargetID.PrototypeIndigoCM:
                // Thundering Ultimatum - Indicators
                bool hasUltimatumIndicators = false;
                if (log.CombatData.TryGetEffectEventsBySrcWithGUIDs(target.AgentItem, [EffectGUIDs.OldLionsCourtThunderingUltimatumFrontalCone, EffectGUIDs.OldLionsCourtThunderingUltimatumFlipCone], out var ultimatumIndicators))
                {
                    hasUltimatumIndicators = true;
                    foreach (EffectEvent effect in ultimatumIndicators)
                    {
                        var flipped = effect.GUIDEvent.ContentGUID == EffectGUIDs.OldLionsCourtThunderingUltimatumFlipCone;
                        (long start, long end) lifespan = effect.HasDynamicEndTime ? effect.ComputeDynamicLifespan(log, 30000) : effect.ComputeLifespan(log, 1500);
                        var rotation = new AngleConnector(effect.Rotation.Z - (flipped ? 270 : 90));
                        int openingAngle = flipped ? 120 : 240;
                        var pie = (PieDecoration)new PieDecoration(3000, openingAngle, lifespan, Colors.LightOrange, 0.2, new PositionConnector(effect.Position)).UsingRotationConnector(rotation);
                        replay.Decorations.Add(pie);
                    }
                }

                // Thundering Ultimatum - Detonation
                // The effect can play twice with different rotation
                if (hasUltimatumIndicators && log.CombatData.TryGetGroupedEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.OldLionsCourtThunderingUltimatumDetonation, out var groupedUltimatumDetonation))
                {
                    foreach (IReadOnlyList<EffectEvent> ultimatumDetonation in groupedUltimatumDetonation)
                    {
                        // We only keep the first
                        EffectEvent effect = ultimatumDetonation[0];
                        (long start, long end) lifespan = effect.ComputeLifespan(log, 1500); // Override 0 duration to 1500
                        EffectEvent? previousIndicator = ultimatumIndicators.LastOrDefault(x => x.Time <= effect.Time);
                        if (target.TryGetCurrentFacingDirection(log, effect.Time, out var currentRotation) && previousIndicator != null)
                        {
                            var flipped = previousIndicator.GUIDEvent.ContentGUID == EffectGUIDs.OldLionsCourtThunderingUltimatumFlipCone;
                            var rotationOffset = flipped ? 180 : 0;
                            var rotation = new AngleConnector(currentRotation.GetRoundedZRotationDeg() + rotationOffset);
                            var openingAngle = flipped ? 120 : 240;
                            var pie = (PieDecoration)new PieDecoration(3000, openingAngle, lifespan, Colors.CobaltBlue, 0.2, new PositionConnector(effect.Position)).UsingRotationConnector(rotation);
                            replay.Decorations.Add(pie);
                        }
                    }
                }

                // Safe Zone - 120°
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.OldLionsCourtThunderingUltimatumSafeZone, out var safeZone))
                {
                    foreach (EffectEvent effect in safeZone)
                    {
                        (long start, long end) lifespan = effect.ComputeDynamicLifespan(log, 30000);
                        var rotation = new AngleConnector(effect.Rotation.Z + 90);
                        var pie = (PieDecoration)new PieDecoration(3000, 120, lifespan, Colors.White, 0.2, new PositionConnector(effect.Position)).UsingRotationConnector(rotation);
                        replay.Decorations.Add(pie);
                    }
                }

                // Crackling Wind
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.OldLionsCourtCracklingWindIndicator, out var crackingWind))
                {
                    foreach (EffectEvent effect in crackingWind)
                    {
                        (long start, long end) lifespan = effect.ComputeLifespan(log, 4000);
                        replay.Decorations.AddContrenticRings(0, 140, lifespan, effect.Position, Colors.LightOrange, 0.01f, 8, true);
                        // Add bigger doughnut past 1120 radius (140 * 8)
                        var doughnut = new DoughnutDecoration(1120, 2500, lifespan, Colors.LightOrange, 0.2, new PositionConnector(effect.Position));
                        replay.Decorations.Add(doughnut);
                    }
                }

                // Hide when inactive
                replay.AddHideByBuff(target, log, Determined762);
                break;
            case (int)TrashID.Tribocharge:
                // Tribocharge AoE on Player
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.OldLionsCourtTribocharge, out var tribocharge))
                {
                    foreach (EffectEvent effect in tribocharge)
                    {
                        // Effect has no Src - Override it to the minion's Master.
                        // Effect spawns 2 seconds after the NPC, overriding start time to FirstAware and end time to the 5000 ms duration.
                        uint radius = 100; // Approximated value
                        (long start, long end) lifespan = (target.FirstAware, target.FirstAware + 5000);
                        replay.Decorations.AddWithGrowing(new CircleDecoration(radius, lifespan, Colors.LightOrange, 0.2, new AgentConnector(target.AgentItem.GetFinalMaster())), lifespan.end);
                    }
                }
                break;
            default:
                break;
        }
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log)
    {
        base.ComputeEnvironmentCombatReplayDecorations(log);

        // Exhaust Plume - Knight Fall AoE
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.OldLionsCourtExhaustPlumeAoE, out var exhaustPlume))
        {
            foreach (EffectEvent effect in exhaustPlume)
            {
                (long start, long end) lifespan = effect.ComputeLifespan(log, 5000);
                var circle = new CircleDecoration(200, lifespan, Colors.LightOrange, 0.2, new PositionConnector(effect.Position));
                EnvironmentDecorations.Add(circle);
                EnvironmentDecorations.Add(circle.Copy().UsingFilled(true).UsingGrowingEnd(lifespan.end));
            }
        }

        // Boiling Aether - Expanding AoE
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.OldLionsCourtBoilingAetherExpanding, out var boilingAetherExpanding))
        {
            // Minimum Radius: 100 (Knight's Half Hitbox)
            // Maximum Radius: 320 (Normal Mode)
            // Maximum Radius: 400 (Challenge Mode)
            // Radius Expansion: 11 (Normal Mode)
            // Radius Expansion: 15 (Challenge Mode)
            // Expansion Timer: 500ms
            uint initialRadius = 100;
            uint timeInterval = 500;
            uint radiusIncreasePerInterval = (uint)(log.FightData.IsCM ? 15 : 11);

            foreach (EffectEvent effect in boilingAetherExpanding)
            {
                uint currentRadius = initialRadius;
                long totalIntervals = effect.Duration / timeInterval;
                (long start, long end) lifespan = (effect.Time, effect.Time + timeInterval);

                for (int i = 0; i < totalIntervals; i++)
                {
                    var circle = new CircleDecoration(currentRadius, lifespan, Colors.LightGrey, 0.2, new PositionConnector(effect.Position));
                    var border = (CircleDecoration)new CircleDecoration(currentRadius, lifespan, Colors.Red, 0.2, new PositionConnector(effect.Position)).UsingFilled(false);
                    currentRadius += radiusIncreasePerInterval;
                    lifespan = (lifespan.end, lifespan.end + timeInterval);
                    EnvironmentDecorations.Add(circle);
                    EnvironmentDecorations.Add(border);
                }
            }
        }

        // Boiling Aether - Fully Expanded
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.OldLionsCourtBoilingAetherFullyExpanded1, out var boilingAetherExpanded))
        {
            // Maximum Radius: 320 (Normal Mode)
            // Maximum Radius: 400 (Challenge Mode)
            uint radius = (uint)(log.FightData.IsCM ? 400 : 320);

            foreach (EffectEvent effect in boilingAetherExpanded)
            {
                (long start, long end) lifespan = effect.ComputeDynamicLifespan(log, 590000);
                var circle = new CircleDecoration(radius, lifespan, Colors.Red, 0.3, new PositionConnector(effect.Position));
                EnvironmentDecorations.Add(circle);
            }
        }
    }

    /// <summary>
    /// Adds the Fixated decorations.
    /// </summary>
    /// <param name="player">Player for the decoration.</param>
    /// <param name="replay">Combat Replay.</param>
    /// <param name="fixations">The <see cref="BuffEvent"/> where the buff appears.</param>
    /// <param name="icon">The icon related to the respective buff.</param>
    private static void AddFixatedDecorations(PlayerActor player, ParsedEvtcLog log, CombatReplay replay, IEnumerable<BuffEvent> fixations, string icon)
    {
        IEnumerable<BuffEvent> applications = fixations.Where(x => x is BuffApplyEvent);
        IEnumerable<BuffEvent> removals = fixations.Where(x => x is BuffRemoveAllEvent);
        foreach (BuffApplyEvent bae in applications.Cast<BuffApplyEvent>())
        {
            long start = bae.Time;
            var removal = removals.FirstOrDefault(x => x.Time > start);
            long end = removal != null ? removal.Time : log.FightData.LogEnd;
            replay.Decorations.Add(new IconOverheadDecoration(icon, 20, 1, ((int)start, (int)end), new AgentConnector(player)));
        }
    }
}
