using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using GW2EIGW2API;
using static GW2EIEvtcParser.AchievementEligibilityIDs;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Mechanic;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicTimeUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;
using static GW2EIEvtcParser.EIData.Mechanic.MechanicSeverity;
using static GW2EIEvtcParser.MechanicIDs;

namespace GW2EIEvtcParser.LogLogic;

internal class NexusOfEternity : VisionsOfEternityRaidEncounter
{

    internal readonly MechanicGroup Mechanics = new([
        new MechanicGroup([
            new PlayerDstHealthDamageHitMechanic([AnnihilatingOrbVloxx, AnnihilatingOrbCosmicPiercer], Mech_AnnihilatingOrb, new (Symbols.Circle, Colors.Black), new ("AnnOrb.H", "Hit by Annihilating Orb", "Annihilating Orb Hit"), Sev1),
            new PlayerDstHealthDamageHitMechanic(AnnihilatingOrbTPShockwave, Mech_AnnihilatingOrbShockwave, new (Symbols.Circle, Colors.Blue), new ("AnnOrb.Shck.H", "Hit by Annihilating Orb Shockwave", "Annihilating Orb Shockwave Hit"), Sev1)
                    .WithStabilitySubMechanic(
                        new SubMechanic(Mech_AnnihilatingOrbShockwaveNoStab, new (Symbols.Circle,Colors.Green), new("AnnOrb.Shck.KD", "Hit by Annihilating Orb Shockwave without stability", "Annihilating Orb Shockwave Hit no stab"), Sev0),
                        false
                    )
        ]),
        new MechanicGroup([
            new MechanicGroup([
                new PlayerDstEffectMechanic(EffectGUIDs.NexusOfEternityJudgmentOfEternity3PeopleGreenSelect, Mech_NexusOfEternity3GreenSelect, new(Symbols.BowtieOpen, Colors.DarkMagenta), new("3Green.Slct", "Selected for 3-people green", "3-people green select"), Sev1),
                new PlayerDstHealthDamageHitMechanic(JudgmentOfEternityGreenFailDamage, Mech_JudgmentOfEternity, new (Symbols.Bowtie, Colors.DarkMagenta), new ("JudgEter.H", "Hit by Judgment of Eternity (3-people green failed)", "Judgment of Eternity Hit"), Sev0),
            ]),
            new MechanicGroup([
                new PlayerDstEffectMechanic(EffectGUIDs.NexusOfEternityAscensionsSacrifice2PeopleGreenSelect, Mech_NexusOfEternity2GreenSelect, new(Symbols.DiamondOpen, Colors.Chocolate), new("2Green.Slct", "Selected for 2-people green", "2-people green select"), Sev1),
                new PlayerDstHealthDamageHitMechanic(AscensionsSacrifice, Mech_AscensionsSacrifice, new (Symbols.Diamond, Colors.   Chocolate), new ("AscSac.H", "Hit by Ascension's Sacrifice (2-people green failed)", "Ascension's Sacrifice Hit"), Sev0),
            ]),
        ]),
        new MechanicGroup([
            // TODO add mechanics regarding getting hit by puddles
            new PlayerDstEffectMechanic(EffectGUIDs.NexusOfEternityProbabilityDistributionSpreadAndPuddleDrop, Mech_NexusOfEternitySpreadAndPuddleSelect, new(Symbols.Diamond, Colors.Orange), new("Pddl.Drp", "Selected for spread + puddle drop", "Spread + puddle"), Sev1),
        ]),
        new PlayerDstHealthDamageHitMechanic(SliceThroughReality, Mech_SliceThroughReality, new (Symbols.CircleOpenDot, Colors.DarkBlue), new ("SlicReal.H", "Hit by Slice Through Reality", "Slice Through Reality Hit"), Sev0),
        new PlayerDstHealthDamageHitMechanic(DivisionEternal, Mech_DivisionEternal, new (Symbols.BowtieOpen, Colors.DarkRed), new ("DivEter.H", "Hit by Division Eternal", "Division Eternal Hit"), Sev0),
        new PlayerDstHealthDamageHitMechanic([VisionsOfEternity1, VisionsOfEternity2,  VisionsOfEternity3], Mech_VisionsOfEternity, new (Symbols.CircleX, Colors.LightBlue), new ("VisEter.H", "Hit by Visions of Eternity", "Visions of Eternity Hit"), Sev2),
        new PlayerDstHealthDamageHitMechanic([ExcisionExtremis1, ExcisionExtremis2], Mech_ExcisionExtremis, new (Symbols.CrossOpen, Colors.DarkerLime), new ("ExciExtr.H", "Hit by Excision Extremis", "Excision Extremis Hit"), Sev0),
        new PlayerDstHealthDamageHitMechanic(ProbabilityDistribution, Mech_ProbabilityDistribution, new (Symbols.CircleXOpen, Colors.Sand), new ("ProbDist.H", "Hit by Probability Distribution", "Probability Distribution Hit"), Sev0),
        new PlayerDstHealthDamageHitMechanic(EchoingBlade, Mech_EchoingBlade, new (Symbols.DiamondWide, Colors.DarkYellow), new ("EchoBlad.H", "Hit by Echoing Blade", "Echoing Blade Hit"), Sev2),

        new MechanicGroup([
            new PlayerDstHealthDamageHitMechanic([SurroundingCurseAspectOfTheStaff, SurroundingCurseVloxx], Mech_SurroundingCurse, new (Symbols.CircleCross, Colors.Teal), new ("SurrCur.H", "Hit by Surrounding Curse", "Surrounding Curse Hit"), Sev1),
            new PlayerDstHealthDamageHitMechanic([CosmicChargeVloxx, CosmicChargeBulwark, CosmicChargeAspectOfTheSpear], Mech_CosmicCharge, new (Symbols.CircleCrossOpen, Colors.White), new ("CosmChar.H", "Hit by Cosmic Charge", "Cosmic Charge Hit"), Sev1),
            new PlayerDstHealthDamageHitMechanic([ThousandStrikesAspectOfTheSpear, ThousandStrikesVloxx], Mech_ThousandStrikes, new (Symbols.CircleOpen, Colors.LightPink), new ("ThouStr.H", "Hit by Thousand Strikes", "Thousand Strikes Hit"), Sev2),
            new PlayerDstHealthDamageHitMechanic([RagingStormCosmicBulwark, RagingStormVloxx, RagingStormVloxx2], Mech_RagingStorm, new (Symbols.Cross, Colors.RedBrownish), new ("RagStor.H", "Hit by Raging Storm", "Raging Storm Hit"), Sev1),
            new PlayerDstHealthDamageHitMechanic([WorldpiercerCosmicBullwark, WorldpiercerVloxx], Mech_Worldpiercer, new (Symbols.Diamond, Colors.FluoOrange), new ("WorldpierV.H", "Hit by Worldpiercer", "Worldpiercer Hit"), Sev0),
            new PlayerDstHealthDamageHitMechanic([EternalReflectionVloxx, EternalReflectionCosmicPiercerChamp, EternalReflectionAspectOfTheStaff, EternalReflectionCosmicPiercerElite], Mech_EternalReflection, new (Symbols.DiamondTall, Colors.DarkMagenta), new ("EterRefl.H", "Hit by Eternal Reflection", "Eternal Reflection Hit"), Sev2),
        ]),
        new MechanicGroup([
            new EnemyDstBuffRemoveSingleMechanic(EmpoweredNexusOfEternity, Mech_VloxxEmpoweredRemoved, new (Symbols.DiamondWideOpen, Colors.Red), new ("Emp.L", "Lost Empowered", "Empowered Lost"), Sev0),
            new EnemyDstBuffApplyMechanic(EmpoweredNexusOfEternity, Mech_VloxxEmpowered, new (Symbols.DiamondWide, Colors.Red), new ("Emp.A", "Applied Empowered", "Empowered Applied"), Sev0),
        ]),
        new EnemyDstBuffApplyMechanic(DamageImmunity, Mech_DamageImmunity, new (Symbols.Hexagon, Colors.LightBlue), new ("DmgImm.A", "Applied Damage Immunity", "Damage Immunity Applied"), Sev2),
        new MechanicGroup([
            new PlayerDstBuffApplyMechanic(Ascension, Mech_Ascension, new (Symbols.HexagonOpen, Colors.GreenishYellow), new ("Ascen.A", "Applied Ascension", "Ascension Applied"), Sev1),
            new PlayerDstBuffRemoveSingleMechanic(Ascension, Mech_AscensionRemove, new (Symbols.HexagonOpen, Colors.Green), new ("Ascen.R", "Removed Ascension", "Ascension Removed"), Sev0),
        ]),
    ]);

    public NexusOfEternity(int triggerID) : base(triggerID)
    {
        MechanicList.Add(Mechanics);
        Icon = EncounterIconNexusOfEternity;
        Extension = "noe";
        GenericFallBackMethod = FallBackMethod.None;
        LogCategoryInformation.InSubCategoryOrder = 1;
        LogID |= 0x000002;
        ChestID = ChestID.GrandRaidVloxxChest;
    }

    internal override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log, CombatReplayDecorationContainer arenaDecorations, CombatReplayMap? parentMap = null)
    {
        return base.GetCombatMapInternal(log, arenaDecorations, parentMap);
    }
    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        return
        [
        ];
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData, GW2APIController apiController)
    {
        return "Nexus of Eternity";
    }

    internal override IReadOnlyList<TargetID> GetTargetsIDs()
    {
        return
        [
            TargetID.NexusOfEternityVloxx,
            TargetID.ChampionCosmicPiercer,
            TargetID.SomethingCosmicPiercer,
            TargetID.ChampionAspectOfTheStaff,
            TargetID.ChampionAspectOfTheSpear,
            TargetID.ChampionCosmicBulwark,
            TargetID.ChampionCosmicSunderer,
        ];
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        return
        [
            TargetID.EliteCosmicPiercer,
            TargetID.EliteCosmicBulwark,
        ];
    }

    internal override long GetLogOffset(EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData)
    {
        long startToUse = GetGenericLogOffset(logData);

        CombatItem? logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.LogNPCUpdate);
        if (logStartNPCUpdate != null)
        {
            var vloxx = agentData.GetAgent(logStartNPCUpdate.DstAgent, logStartNPCUpdate.Time);
            if (!vloxx.IsSpecies(TargetID.NexusOfEternityVloxx))
            {
                throw new MissingKeyActorsException("Vloxx not found");
            }
            startToUse = GetEnterCombatTime(logData, agentData, combatData, logStartNPCUpdate.Time, GenericTriggerID, logStartNPCUpdate.DstAgent);
            var targetable = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.Targetable && x.SrcMatchesAgent(vloxx) && x.DstAgent > 0);
            if (targetable != null)
            {
                startToUse = Math.Min(startToUse, targetable.Time);
            }
        }
        return startToUse;
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        FindChestGadgets([
            (ChestID.GrandRaidVloxxChest, GrandRaidChestVloxxPosition, 100),
        ], agentData, combatData);
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);

        RenameAdds(Targets);
    }

    internal static IReadOnlyList<SubPhasePhaseData> ComputePhases(ParsedEvtcLog log, SingleActor vloxx, IReadOnlyList<SingleActor> targets, EncounterPhaseData encounterPhase, bool requirePhases)
    {
        if (!requirePhases)
        {
            return [];
        }
        var phases = GetSubPhasesByInvul(log, DamageImmunity, vloxx, true, true, encounterPhase.Start, encounterPhase.End);

        var champions = targets.Where(x => x.IsAnySpecies(
        [
            TargetID.ChampionCosmicBulwark,
            TargetID.ChampionCosmicPiercer,
            TargetID.ChampionCosmicSunderer,
            TargetID.ChampionAspectOfTheSpear,
            TargetID.ChampionAspectOfTheStaff,
            // TargetID.SomethingCosmicPiercer,
        ])).ToList();

        for (int i = 0; i < phases.Count; i++)
        {
            int index = i + 1;
            PhaseData phase = phases[i];
            phase.AddParentPhase(encounterPhase);
            if (index % 2 == 0)
            {
                phase.Name = "Split " + (index) / 2;
                phase.AddTargets(champions, log);
                phase.AddTarget(vloxx, log, PhaseData.TargetPriority.NonBlocking);
            }
            else
            {
                phase.Name = "Phase " + (index + 1) / 2;
                phase.AddTarget(vloxx, log);
            }
        }

        return phases;
    }

    internal override Dictionary<TargetID, int> GetTargetsSortIDs()
    {
        return new Dictionary<TargetID, int>()
        {
            {TargetID.NexusOfEternityVloxx, 0},
            {TargetID.ChampionCosmicPiercer, 1},
            {TargetID.SomethingCosmicPiercer, 1},
            {TargetID.ChampionAspectOfTheStaff, 1},
            {TargetID.ChampionAspectOfTheSpear, 1},
            {TargetID.ChampionCosmicBulwark, 1},
            {TargetID.ChampionCosmicSunderer, 1}
        };
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        var vloxx = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.NexusOfEternityVloxx)) ?? throw new MissingKeyActorsException("Vloxx not found");
        var phases = GetInitialPhase(log);
        var fullFightPhase = (EncounterPhaseData)phases[0];
        fullFightPhase.AddTarget(vloxx, log);
        phases.AddRange(ComputePhases(log, vloxx, Targets, fullFightPhase, requirePhases));
        return phases;
    }

    internal override LogData.Mode GetLogMode(CombatData combatData, AgentData agentData, LogData logData)
    {
        return LogData.Mode.Normal;
    }


    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputePlayerCombatReplayActors(p, log, replay);
        }

        var fixated = p.GetBuffStatus(log, FixatedTimed).Where(x => x.Value > 0);
        foreach (Segment seg in fixated)
        {
            replay.Decorations.AddOverheadIcon(seg, p, ParserIcons.FixationPurpleOverhead);
        }

        // Judgment of Eternity - Greens (3 people)
        if (log.CombatData.TryGetEffectEventsByDstWithGUID(p.AgentItem, EffectGUIDs.NexusOfEternityJudgmentOfEternity3PeopleGreenSelect, out var judgmentOfEternity))
        {
            foreach (var effect in judgmentOfEternity)
            {
                (long start, long end) lifespan = effect.ComputeLifespan(log, 8000);
                var circle = new CircleDecoration(240, lifespan, Colors.DarkGreen, 0.2, new AgentConnector(p.AgentItem));
                replay.Decorations.AddWithFilledWithGrowing(circle, true, lifespan.end, true);
                replay.Decorations.AddOverheadIcon(lifespan, p, ParserIcons.GreenMarkerSize3Overhead);
            }
        }

        // Ascension's Sacrifice - Greens (2 people)
        if (log.CombatData.TryGetEffectEventsByDstWithGUID(p.AgentItem, EffectGUIDs.NexusOfEternityAscensionsSacrifice2PeopleGreenSelect, out var ascensionsSacrifice))
        {
            foreach (var effect in ascensionsSacrifice)
            {
                (long start, long end) lifespan = effect.ComputeLifespan(log, 5000);
                var circle = new CircleDecoration(150, lifespan, Colors.DarkGreen, 0.2, new AgentConnector(p.AgentItem));
                replay.Decorations.AddWithFilledWithGrowing(circle, true, lifespan.end, true);
                replay.Decorations.AddOverheadIcon(lifespan, p, ParserIcons.GreenMarkerSize2Overhead);

                // Chain - Appears 500ms after the green, duration 4500 - The buff applied is POV only
                replay.Decorations.AddTetherByEffectGUID(effect, Colors.Yellow, 0.4, (lifespan.start + 500, lifespan.end));
            }
        }

        // Probability Distribution - Spread AoE
        if (log.CombatData.TryGetEffectEventsByDstWithGUID(p.AgentItem, EffectGUIDs.NexusOfEternityProbabilityDistributionSpreadAndPuddleDrop, out var probabilityDistribution))
        {
            foreach (var effect in probabilityDistribution)
            {
                (long start, long end) lifespan = effect.ComputeLifespan(log, 5000);
                var circle = new CircleDecoration(280, lifespan, Colors.LightOrange, 0.2, new AgentConnector(p.AgentItem));
                replay.Decorations.AddWithFilledWithGrowing(circle, true, lifespan.end);
            }
        }
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeNPCCombatReplayActors(target, log, replay);
        }

        (long start, long end) lifespan;

        switch (target.ID)
        {
            case (int)TargetID.NexusOfEternityVloxx:
                {
                    AddEternalReflectionSurroundingCurse(log, replay, target.AgentItem, [EternalReflectionVloxx, SurroundingCurseVloxx]);
                    AddSurroundingCurseAoe(log, replay, target.AgentItem);

                    // Probability Distribution - Placed AoE indicator
                    if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.NexusOfEternityProbabilityDistributionIndicator, out var puddlesIndicators))
                    {
                        foreach (var effect in puddlesIndicators)
                        {
                            lifespan = effect.ComputeLifespan(log, 3000);
                            var circle = new CircleDecoration(280, lifespan, Colors.LightOrange, 0.2, new PositionConnector(effect.Position));
                            replay.Decorations.AddWithFilledWithGrowing(circle, true, lifespan.end);
                        }
                    }

                    // Cosmic Charge & Probability Distribution - Damage field
                    if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.NexusOfEternityCosmicChargeTrailAndProbabilityDistributionAoE, out var puddles))
                    {
                        foreach (var effect in puddles)
                        {
                            // duration 10000 for trail, 12000 for puddle
                            // scale 1.0 for trail, 1.7 for puddle, roughly 160 and 280 radius
                            uint radius = (uint)(effect.Duration == 10000 ? 160 : 280);
                            lifespan = effect.ComputeLifespan(log, effect.Duration);
                            var circle = new CircleDecoration(radius, lifespan, Colors.CobaltBlue, 0.2, new PositionConnector(effect.Position));
                            replay.Decorations.Add(circle);
                        }
                    }

                    // Visions of Eternity - Indicator
                    if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.NexusOfEternityVisionsOfEternityIndicator, out var voeIndicator))
                    {
                        foreach (var effect in voeIndicator)
                        {
                            lifespan = effect.ComputeLifespan(log, 8000);
                            var circle = new CircleDecoration(560, lifespan, Colors.LightOrange, 0.2, new PositionConnector(effect.Position));
                            replay.Decorations.AddWithGrowing(circle, lifespan.end);
                        }
                    }

                    // Worldpiercer - Indicator
                    if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.NexusOfEternityWorldpiercerIndicator, out var worldpiercerIndicator))
                    {
                        foreach (var effect in worldpiercerIndicator)
                        {
                            lifespan = effect.ComputeLifespan(log, 2666);
                            var line = new RectangleDecoration(3650, 100, lifespan, Colors.Red, 0.5, new PositionConnector(effect.Position).WithOffset(new(1825, 0, 0), true)).UsingRotationConnector(new AngleConnector(effect.Rotation.Z - 90));
                            replay.Decorations.Add(line);
                        }
                    }

                    // Worldpiercer - Barrier
                    if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.NexusOfEternityWorldpiercerLineBarrier, out var worldpiercerBarrier))
                    {
                        foreach (var effect in worldpiercerBarrier)
                        {
                            // Up to 10 segments if it doesn't hit the arena border
                            lifespan = effect.ComputeDynamicLifespan(log, 10000);
                            var line = new RectangleDecoration(365, 10, lifespan, Colors.LightBlue, 0.3, new PositionConnector(effect.Position)).UsingRotationConnector(new AngleConnector(effect.Rotation.Z));
                            replay.Decorations.Add(line);
                        }
                    }

                    AddEchoingBladeExcisionExtremisIndicators(log, replay, target.AgentItem);
                    AddSwordSwings(log, replay, target.AgentItem, EffectGUIDs.NexusOfEternityVloxxEchoingBladeSwordSwing, 600);
                    AddSwordSwings(log, replay, target.AgentItem, EffectGUIDs.NexusOfEternityVloxxExcisionExtremisDivisionEternalSwordSwing, 500);

                    // Echoing Blade - Red AoEs with spinning sword - Reflectable
                    var echoingBlade = log.CombatData.GetMissileEventsBySkillID(EchoingBlade);
                    replay.Decorations.AddNonHomingMissiles(log, echoingBlade, Colors.Red, 0.3, 200);

                    // Annihilating Orb - Arrow indicator
                    if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.NexusOfEternityAnnihilatingOrbArrowIndicator, out var arrows))
                    {
                        foreach (var effect in arrows)
                        {
                            lifespan = effect.ComputeLifespan(log, 6000);
                            var line = new RectangleDecoration(1850, 100, lifespan, Colors.LightOrange, 0.2, new PositionConnector(effect.Position).WithOffset(new(925, 0, 0), true)).UsingRotationConnector(new AngleConnector(effect.Rotation.Z - 90));
                            replay.Decorations.Add(line);
                        }
                    }

                    // Annihilating Orb - Warnings rings for the teleport location
                    if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.NexusOfEternityAnnihilatingOrbTeleportRingsIndicator, out var rings))
                    {
                        foreach (var effect in rings)
                        {
                            lifespan = effect.ComputeLifespan(log, 4000);
                            int pulseCycle = 1000;
                            (long start, long end) pulse = (lifespan.start, lifespan.start + pulseCycle);
                            for (int i = 0; i < 4; i++)
                            {
                                replay.Decorations.AddShockwave(new PositionConnector(effect.Position), pulse, Colors.LightOrange, 0.2, 1200);
                                pulse.start = pulse.end;
                                pulse.end = pulse.start + pulseCycle;
                            }
                        }
                    }

                    // Annihilating Orb - End red AoE
                    if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.NexusOfEternityAnnihilatingOrbPostTeleportRedAoE, out var redAoEs))
                    {
                        var orbs = log.CombatData.GetMissileEventsBySkillID(AnnihilatingOrbVloxx);

                        foreach (var orb in orbs)
                        {
                            var endAoE = redAoEs.FirstOrDefault(x => x.Time >= orb.Time && x.Time <= orb.Time + 5000);
                            if (endAoE == null)
                            {
                                continue;
                            }
                            CombatReplayDecorationContainer.AddNonHomingMissile(log, orb, (launch, lifespan, connector) =>
                            {
                                replay.Decorations.Add(new CircleDecoration(180, lifespan, Colors.Blue, 0.2, connector));
                                replay.Decorations.Add(new DoughnutDecoration(180, 240, lifespan, Colors.Red, 0.3, connector));
                            }, endAoE.Time);
                        }

                        foreach (var effect in redAoEs)
                        {
                            lifespan = effect.ComputeLifespan(log, 5000);
                            var barrier = new CircleDecoration(240, lifespan, Colors.Blue, 0.2, new PositionConnector(effect.Position));
                            replay.Decorations.Add(barrier);
                            var doughnut = new DoughnutDecoration(240, 300, lifespan, Colors.Red, 0.3, new PositionConnector(effect.Position)); // Doughnut for better color representation
                            replay.Decorations.Add(doughnut);
                            // Shockwave - Don't see it as a separated effect
                            lifespan = (effect.Time, effect.Time + 3333);
                            replay.Decorations.AddShockwave(new PositionConnector(effect.Position), lifespan, Colors.LightGrey, 0.6, 1850); // Extends to the original position of Vloxx
                        }
                    }

                    // Division Eternal - Big rectangle
                    if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.NexusOfEternityDivisionEternalIndicator, out var divisionEternals))
                    {
                        foreach (var effect in divisionEternals)
                        {
                            lifespan = effect.ComputeLifespan(log, 3000);
                            var rectangle = (RectangleDecoration)new RectangleDecoration(2400, 1200, lifespan, Colors.LightOrange, 0.2, new PositionConnector(effect.Position)).UsingRotationConnector(new AngleConnector(effect.Rotation.Z));
                            replay.Decorations.AddWithBorder(rectangle, Colors.LightOrange, 0.2);
                        }
                    }

                    // Slice Through Reality - Teleport AoE
                    if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.NexusOfEternitySliceThroughRealityPortAndSuckAoE, out var tp))
                    {
                        for (int i = 0; i <= tp.Count - 1; i = i + 2)
                        {
                            var entry = tp[i];
                            var exit = tp[i + 1];
                            if (entry != null && exit != null && exit.Time > entry.Time && exit.Time < entry.Time + 3000)
                            {
                                (long start, long end) lifespanEntry = entry.ComputeDynamicLifespan(log, 10000);
                                (long start, long end) lifespanExit = entry.ComputeDynamicLifespan(log, 10000);
                                var entryCircle = new CircleDecoration(220, lifespanEntry, Colors.LightOrange, 0.2, new PositionConnector(entry.Position));
                                replay.Decorations.Add(entryCircle);
                                var exitCircle = new CircleDecoration(220, lifespanExit, Colors.LightOrange, 0.2, new PositionConnector(exit.Position));
                                replay.Decorations.Add(exitCircle);

                                // Suction animation
                                long time = entry.Time;
                                int animDuration = 500;
                                for (int z = 0; z < 10; z++)
                                {
                                    float rotation = 0f;
                                    for (int y = 0; y <= 16; y++)
                                    {
                                        (long start, long end) lifespanAnim = (time, time + 500);
                                        float angle = rotation * (float)Math.PI / 180f;
                                        var direction = new Vector3((float)Math.Cos(angle), (float)Math.Sin(angle), 0);
                                        var initialPosition = new ParametricPoint3D(entry.Position + direction * 200f, time);
                                        var finalPosition = new ParametricPoint3D(entry.Position, time + animDuration);
                                        var connector = new InterpolationConnector([initialPosition, finalPosition]).WithOffset(new(1000, 0, 0), true);
                                        var rect = new RectangleDecoration(100, 20, lifespanAnim, Colors.Yellow, 0.3, connector).UsingRotationConnector(new AngleConnector(rotation));
                                        replay.Decorations.Add(rect);
                                        rotation += 22.5f;
                                    }
                                    time += 1000;
                                }
                            }
                        }
                    }
                }
                break;
            case (int)TargetID.ChampionAspectOfTheStaff:
                {
                    AddEternalReflectionSurroundingCurse(log, replay, target.AgentItem, [EternalReflectionAspectOfTheStaff, SurroundingCurseAspectOfTheStaff]);
                    AddSurroundingCurseAoe(log, replay, target.AgentItem);
                }
                break;
            case (int)TargetID.ChampionAspectOfTheSpear:
                {
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.NexusOfEternityCosmicChargeTrailAndProbabilityDistributionAoE, out var puddles))
                    {
                        foreach (var effect in puddles)
                        {
                            // duration 5000
                            // TODO figure out which skill is this effect from for the spear
                        }
                    }
                }
                break;
            case (int)TargetID.ChampionCosmicPiercer:
                {
                    AddEternalReflectionSurroundingCurse(log, replay, target.AgentItem, [EternalReflectionCosmicPiercerChamp]);
                }
                break;
            case (int)TargetID.EliteCosmicPiercer:
                {
                    AddEternalReflectionSurroundingCurse(log, replay, target.AgentItem, [EternalReflectionCosmicPiercerElite]);
                }
                break;
            case (int)TargetID.ChampionCosmicSunderer:
                {
                    AddEchoingBladeExcisionExtremisIndicators(log, replay, target.AgentItem);
                    // TODO find and add Extremis hit effect
                }
                break;
        }
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);
        }
    }
    internal override void ComputeAchievementEligibilityEvents(ParsedEvtcLog log, Player p, List<AchievementEligibilityEvent> achievementEligibilityEvents)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeAchievementEligibilityEvents(log, p, achievementEligibilityEvents);
        }
    }

    internal override void SetInstanceBuffs(ParsedEvtcLog log, List<InstanceBuff> instanceBuffs)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.SetInstanceBuffs(log, instanceBuffs);
        }
    }

    private static void RenameAdds(IReadOnlyList<SingleActor> targets)
    {
        foreach (SingleActor actor in targets)
        {
            switch (actor.ID)
            {
                case (int)TargetID.ChampionCosmicBulwark:
                    actor.OverrideName("Champion " + actor.Character);
                    break;
                case (int)TargetID.ChampionCosmicPiercer:
                    actor.OverrideName("Champion " + actor.Character);
                    break;
                case (int)TargetID.ChampionCosmicSunderer:
                    actor.OverrideName("Champion " + actor.Character);
                    break;
                case (int)TargetID.ChampionAspectOfTheSpear:
                    actor.OverrideName("Champion " + actor.Character);
                    break;
                case (int)TargetID.ChampionAspectOfTheStaff:
                    actor.OverrideName("Champion " + actor.Character);
                    break;
                case (int)TargetID.EliteCosmicBulwark:
                    actor.OverrideName("Elite " + actor.Character);
                    break;
                case (int)TargetID.EliteCosmicPiercer:
                    actor.OverrideName("Elite " + actor.Character);
                    break;
                case (int)TargetID.SomethingCosmicPiercer:
                    //actor.OverrideName("" + actor.Character);
                    break;
            }
        }
    }

    private static void AddEternalReflectionSurroundingCurse(ParsedEvtcLog log, CombatReplay replay, AgentItem agent, long[] ids)
    {
        var missiles = log.CombatData.GetMissileEventsBySrcBySkillIDs(agent, ids);
        foreach (MissileEvent missile in missiles)
        {
            replay.Decorations.AddNonHomingMissileWithBorder(log, missile, Colors.Yellow, 0.3, 20, Colors.DarkPurpleBlue, 0.3);
        }
    }

    private static void AddSurroundingCurseAoe(ParsedEvtcLog log, CombatReplay replay, AgentItem agent)
    {
        // Surrounding Curse - Indicator
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(agent, EffectGUIDs.NexusOfEternitySurroundingCurseIndicator, out var surrCurseIndicators))
        {
            foreach (var effect in surrCurseIndicators)
            {
                (long start, long end) lifespan = effect.ComputeLifespan(log, 3500);
                var circle = new CircleDecoration(200, lifespan, Colors.LightOrange, 0.2, new PositionConnector(effect.Position));
                replay.Decorations.Add(circle);
            }
        }

        // Surrounding Curse - Explosion
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.NexusOfEternitySurroundingCurseExplosions, out var surrCurseDamage))
        {
            foreach (var effect in surrCurseDamage)
            {
                // Duration is 2666, for the replay it's way too long
                // Override it to 250 for a brief visual
                (long start, long end) lifespan = (effect.Time, effect.Time + 250);
                var circle = new CircleDecoration(200, lifespan, Colors.LightCobaltBlue, 0.1, new PositionConnector(effect.Position));
                replay.Decorations.Add(circle);
            }
        }
    }

    private static void AddEchoingBladeExcisionExtremisIndicators(ParsedEvtcLog log, CombatReplay replay, AgentItem agent)
    {
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(agent, EffectGUIDs.NexusOfEternityVloxxEchoingBladeExcisionExtremisIndicator, out var echoingBladeIndicator))
        {
            foreach (var effect in echoingBladeIndicator)
            {
                uint radius = 600;
                // Duration 1500 - Scale 3.0 - Echoing Blade - 600 radius - Vloxx
                // Duration 2500 - Scale 2.5 - Excision Extremis - 500 radius - Vloxx
                // Duration 1500 - Scale 2.0 - Excision - 400 radius - Sunderer
                if (effect.Duration == 2500 && effect.Src.IsSpecies(TargetID.NexusOfEternityVloxx))
                {
                    radius = 500;
                }
                else if (effect.Duration == 1500 && effect.Src.IsSpecies(TargetID.ChampionCosmicSunderer))
                {
                    radius = 400;
                }
                (long start, long end) lifespan = (effect.Time, effect.Time + effect.Duration);
                var pie = (PieDecoration)new PieDecoration(radius, 180, lifespan, Colors.LightOrange, 0.2, new PositionConnector(effect.Position)).UsingRotationConnector(new AngleConnector(effect.Rotation.Z + 90));
                replay.Decorations.AddWithBorder(pie, Colors.LightOrange, 0.2);
            }
        }
    }

    /// <summary>
    /// Used by Echoing Blade, Excision Extremis, Division Eternal
    /// </summary>
    private static void AddSwordSwings(ParsedEvtcLog log, CombatReplay replay, AgentItem agent, Guid guid, uint radius)
    {
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(agent, guid, out var swords))
        {
            foreach (var effect in swords)
            {
                (long start, long end) lifespan = effect.ComputeLifespan(log, 833);
                var line = new RectangleDecoration(radius, 10, lifespan, Colors.Blue, 0.4, new PositionConnector(effect.Position).WithOffset(new(-300, 0, 0), true)).UsingRotationConnector(new SpinningConnector(effect.Rotation.Z - 180, -180));
                replay.Decorations.Add(line);
            }
        }
    }
}
