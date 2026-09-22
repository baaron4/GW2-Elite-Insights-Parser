using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using GW2EIGW2API;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Mechanic.MechanicSeverity;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicTimeUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.MechanicIDs;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

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
            new PlayerDstEffectMechanic(EffectGUIDs.NexusOfEternityProbabilityDistributionSpreadAndPuddleDrop, Mech_NexusOfEternitySpreadAndPuddleSelect, new(Symbols.Diamond, Colors.Orange), new("Pddl.Drp", "Selected for spread + Probability Distribution drop", "Spread + Probability Distribution"), Sev1),
            new PlayerDstHealthDamageHitMechanic(ProbabilityDistribution, Mech_ProbabilityDistribution, new (Symbols.CircleXOpen, Colors.Sand), new ("ProbDist.H", "Hit by Probability Distribution", "Probability Distribution Hit"), Sev1),
        ]),
        new PlayerDstHealthDamageHitMechanic(SliceThroughReality, Mech_SliceThroughReality, new (Symbols.CircleOpenDot, Colors.DarkBlue), new ("SlicReal.H", "Hit by Slice Through Reality", "Slice Through Reality Hit"), Sev0),
        new PlayerDstHealthDamageHitMechanic(DivisionEternal, Mech_DivisionEternal, new (Symbols.BowtieOpen, Colors.DarkRed), new ("DivEter.H", "Hit by Division Eternal", "Division Eternal Hit"), Sev0),
        new PlayerDstHealthDamageHitMechanic([VisionsOfEternityInStaff, VisionsOfEternityInSword,  VisionsOfEternityInSpear], Mech_VisionsOfEternity, new (Symbols.CircleX, Colors.LightBlue), new ("VisEter.H", "Hit by Visions of Eternity", "Visions of Eternity Hit"), Sev2),
        new PlayerDstHealthDamageHitMechanic([ExcisionExtremis1, ExcisionExtremis2], Mech_ExcisionExtremis, new (Symbols.CrossOpen, Colors.DarkerLime), new ("ExciExtr.H", "Hit by Excision Extremis", "Excision Extremis Hit"), Sev0),
        new PlayerDstHealthDamageHitMechanic(Excision, Mech_Excision, new (Symbols.Square, Colors.DarkPurpleBlue), new MechanicDescription("Exci.H", "Hit by Excision", "Excision Hit"), Sev2),
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
        var crMap = new CombatReplayMap(
            (1800, 1800),
            (9718, 12826, 14718, 17826));
        AddArenaDecorationsPerEncounter(log, arenaDecorations, LogID, CombatReplayNexusOfEternity, crMap, parentMap);
        return crMap;
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
            TargetID.Vloxx,
            TargetID.ChampionCosmicPiercer,
            TargetID.SomethingCosmicPiercer,
            TargetID.ChampionAspectOfTheStaff,
            TargetID.ChampionAspectOfTheSpear,
            TargetID.ChampionCosmicBulwark,
            TargetID.ChampionCosmicSunderer,
            TargetID.EliteCosmicPiercer,
            TargetID.EliteCosmicBulwark,
        ];
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        return
        [
            TargetID.AscensionOrb,
        ];
    }

    internal override long GetLogOffset(EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData)
    {
        long startToUse = GetGenericLogOffset(logData);

        CombatItem? logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.LogNPCUpdate);
        if (logStartNPCUpdate != null)
        {
            var vloxx = agentData.GetAgent(logStartNPCUpdate.DstAgent, logStartNPCUpdate.Time);
            if (!vloxx.IsSpecies(TargetID.Vloxx))
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

        AscensionOrbStabilization(agentData, combatData);

        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);

        RenameAdds(Targets);
        RenameAdds(TrashMobs);
    }

    internal static IReadOnlyList<SubPhasePhaseData> ComputePhases(ParsedEvtcLog log, SingleActor vloxx, IReadOnlyList<SingleActor> targets, EncounterPhaseData encounterPhase, bool requirePhases)
    {
        var aspectChampions = targets.Where(x => x.IsAnySpecies(
        [
            TargetID.ChampionAspectOfTheSpear,
            TargetID.ChampionAspectOfTheStaff,
            // TargetID.SomethingCosmicPiercer,
        ])).ToList();
        var cosmicChamps = targets.Where(x => x.IsAnySpecies(
        [
            TargetID.ChampionCosmicBulwark,
            TargetID.ChampionCosmicPiercer,
            TargetID.ChampionCosmicSunderer,
            // TargetID.SomethingCosmicPiercer,
        ])).ToList();
        encounterPhase.AddTargets(aspectChampions, log, PhaseData.TargetPriority.NonBlocking);
        encounterPhase.AddTargets(cosmicChamps, log, PhaseData.TargetPriority.Blocking);
        if (!requirePhases)
        {
            return [];
        }
        var phases = GetSubPhasesByInvul(log, DamageImmunity, vloxx, true, true, encounterPhase.Start, encounterPhase.End, 4000);
        var cosmicElites = targets.Where(x => x.IsAnySpecies(
        [
            TargetID.EliteCosmicBulwark,
            TargetID.EliteCosmicPiercer,
            //TargetID.EliteCosmicSunderer, // Elite sunderer?
        ])).ToList();

        for (int i = 0; i < phases.Count; i++)
        {
            int index = i + 1;
            PhaseData phase = phases[i];
            phase.AddParentPhase(encounterPhase);
            phase.AddTargets(aspectChampions, log, PhaseData.TargetPriority.NonBlocking);
            if (index % 2 == 0)
            {
                phase.AddTargets(cosmicChamps, log);
                phase.AddTargets(cosmicElites, log, PhaseData.TargetPriority.Blocking);
                phase.Name = "Split " + (index) / 2;
                phase.AddTarget(vloxx, log, PhaseData.TargetPriority.Blocking);
            }
            else
            {
                var hpPercent = vloxx.GetCurrentHealthPercent(log, phase.Start);
                phase.Name = hpPercent switch
                {
                    > 70 => "Staff Phase",
                    > 40 => "Spear Phase",
                    > 10 => "Sword Phase",
                    _ => "Final Form Phase",
                };
                phase.AddTarget(vloxx, log);
            }
        }

        return phases;
    }

    internal override Dictionary<TargetID, int> GetTargetsSortIDs()
    {
        return new Dictionary<TargetID, int>()
        {
            {TargetID.Vloxx, 0},
            {TargetID.ChampionCosmicPiercer, 1},
            {TargetID.SomethingCosmicPiercer, 1},
            {TargetID.ChampionCosmicBulwark, 1},
            {TargetID.ChampionCosmicSunderer, 1},
            {TargetID.EliteCosmicPiercer, 2},
            {TargetID.EliteCosmicBulwark, 2},
            {TargetID.ChampionAspectOfTheStaff, 3},
            {TargetID.ChampionAspectOfTheSpear, 3},
        };
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        var vloxx = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Vloxx)) ?? throw new MissingKeyActorsException("Vloxx not found");
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

        // Fixation
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
            case (int)TargetID.Vloxx:

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
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.NexusOfEternityCosmicChargeTrailAndProbabilityDistributionAoE, out var vloxxPuddles))
                {
                    foreach (var effect in vloxxPuddles)
                    {
                        // Duration 10000 for trail, 12000 for puddle
                        // Scale 1.0 for trail, 1.7 for puddle, roughly 160 and 280 radius
                        uint radius = (uint)(effect.Scale * 160);
                        lifespan = effect.ComputeLifespan(log, effect.Duration);
                        var circle = new CircleDecoration(radius, lifespan, Colors.DarkBlue, 0.4, new PositionConnector(effect.Position));
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

                // Annihilating Orb - Moving orb
                var orbs = log.CombatData.GetMissileEventsBySkillID(AnnihilatingOrbVloxx);
                foreach (var orb in orbs)
                {
                    CombatReplayDecorationContainer.AddNonHomingMissile(log, orb, (launch, lifespan, connector) =>
                    {
                        replay.Decorations.Add(new CircleDecoration(180, lifespan, Colors.Blue, 0.2, connector));
                        replay.Decorations.Add(new DoughnutDecoration(180, 240, lifespan, Colors.Red, 0.3, connector));
                    });
                }

                // Blue shield effect, combine with Red AoE + shockwave when no VisionsOfEternity cast
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.NexusOfEternityBlueShield, out var blueShields))
                {
                    var visionsOfEternityCast = target.GetAnimatedCastEvents(log).Where(x => x.SkillID == VisionsOfEternityInStaff || x.SkillID == VisionsOfEternityInSword || x.SkillID == VisionsOfEternityInSpear).ToList();
                    foreach (var effect in blueShields)
                    {
                        lifespan = effect.ComputeLifespan(log, effect.Duration);
                        var barrier = new CircleDecoration(240, lifespan, Colors.Blue, 0.2, new PositionConnector(effect.Position));
                        replay.Decorations.Add(barrier);

                        if (!visionsOfEternityCast.Any(x => x.IntersectsActualCastWindow(effect.Time)))
                        {
                            var doughnut = new DoughnutDecoration(240, 300, lifespan, Colors.Red, 0.3, new PositionConnector(effect.Position)); // Doughnut for better color representation
                            replay.Decorations.Add(doughnut);
                            // Shockwave - Don't see it as a separated effect
                            lifespan = (effect.Time, effect.Time + 3333);
                            replay.Decorations.AddShockwave(new PositionConnector(effect.Position), lifespan, Colors.LightGrey, 0.6, 1850); // Extends to the original position of Vloxx
                        }
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
                            for (int y = 0; y < 10; y++)
                            {
                                float rotation = 0f;
                                for (int z = 0; z <= 16; z++)
                                {
                                    (long start, long end) lifespanAnim = (time, time + 500);
                                    float angle = rotation * (float)Math.PI / 180f;
                                    var direction = new Vector3((float)Math.Cos(angle), (float)Math.Sin(angle), 0);
                                    var initialPosition = new ParametricPoint3D(entry.Position + direction * 200f, time);
                                    var finalPosition = new ParametricPoint3D(entry.Position, time + animDuration);
                                    var connector = new InterpolationConnector([initialPosition, finalPosition]).WithOffset(new(1000, 0, 0), true);
                                    var rect = new RectangleDecoration(150, 20, lifespanAnim, Colors.DarkYellow, 0.6, connector).UsingRotationConnector(new AngleConnector(rotation));
                                    replay.Decorations.Add(rect);
                                    rotation += 22.5f;
                                }
                                time += 1000;
                            }
                        }
                    }
                }

                // Echoing Blade - Red AoEs with spinning sword - Reflectable
                var echoingBlade = log.CombatData.GetMissileEventsBySkillID(EchoingBlade);
                replay.Decorations.AddNonHomingMissiles(log, echoingBlade, Colors.Red, 0.3, 200);

                // Ascension - Orbs spawned from Vloxx after defiance bar is broken
                var ascension = log.CombatData.GetMissileEventsBySkillID(VloxxAscensionOrb);
                replay.Decorations.AddNonHomingMissiles(log, ascension, Colors.DarkPurple, 0.3, 100);

                // Worldpiercer - Missile
                var worldpiercer = log.CombatData.GetMissileEventsBySkillID(WorldpiercerVloxx);
                replay.Decorations.AddNonHomingMissiles(log, worldpiercer, Colors.CobaltBlue, 0.4, 100);

                // Raging Storm - Missile
                AddRagingStormMissiles(log, replay, [RagingStormVloxx, RagingStormVloxx2]);

                AddEternalReflectionSurroundingCurse(log, replay, target.AgentItem, [EternalReflectionVloxx, SurroundingCurseVloxx]);
                AddSurroundingCurseAoe(log, replay, target.AgentItem);
                AddEchoingBladeExcisionExtremisIndicators(log, replay, target.AgentItem);
                AddThousandStrikes(log, replay, target.AgentItem, ThousandStrikesVloxx);
                AddRagingStorm(log, replay, target.AgentItem);
                // Swords last - above other decorations
                AddSwordSwings(log, replay, target.AgentItem, EffectGUIDs.NexusOfEternityVloxxEchoingBladeSwordSwing, 600);
                AddSwordSwings(log, replay, target.AgentItem, EffectGUIDs.NexusOfEternityVloxxExcisionExtremisDivisionEternalSwordSwing, 500);

                break;
            case (int)TargetID.ChampionAspectOfTheStaff:

                AddEternalReflectionSurroundingCurse(log, replay, target.AgentItem, [EternalReflectionAspectOfTheStaff, SurroundingCurseAspectOfTheStaff]);
                AddSurroundingCurseAoe(log, replay, target.AgentItem);

                break;
            case (int)TargetID.ChampionAspectOfTheSpear:

                AddThousandStrikes(log, replay, target.AgentItem, ThousandStrikesAspectOfTheSpear);

                // Cosmic Charge
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.NexusOfEternityCosmicChargeTrailAndProbabilityDistributionAoE, out var spearPuddles))
                {
                    foreach (var effect in spearPuddles)
                    {
                        // Duration 5000 - scale 1.0
                        lifespan = effect.ComputeLifespan(log, effect.Duration);
                        var circle = new CircleDecoration(160, lifespan, Colors.DarkBlue, 0.4, new PositionConnector(effect.Position));
                        replay.Decorations.Add(circle);
                    }
                }

                break;
            case (int)TargetID.EliteCosmicPiercer:

                AddEternalReflectionSurroundingCurse(log, replay, target.AgentItem, [EternalReflectionCosmicPiercerElite]);
                AddCosmicPiercerAnnhilatingOrb(log, replay);

                break;
            case (int)TargetID.ChampionCosmicPiercer:

                AddEternalReflectionSurroundingCurse(log, replay, target.AgentItem, [EternalReflectionCosmicPiercerChamp]);
                AddCosmicPiercerAnnhilatingOrb(log, replay);

                break;
            case (int)TargetID.EliteCosmicBulwark:

                // NOTE: Cosmic Charge does not leave a trail like Vloxx and Aspect of the Spear
                AddRagingStorm(log, replay, target.AgentItem);
                AddCosmicBulwarkWorldpiercer(log, replay, target.AgentItem);

                break;
            case (int)TargetID.ChampionCosmicBulwark:

                // NOTE: Cosmic Charge does not leave a trail like Vloxx and Aspect of the Spear
                AddRagingStorm(log, replay, target.AgentItem);
                AddCosmicBulwarkWorldpiercer(log, replay, target.AgentItem);

                break;
            case (int)TargetID.ChampionCosmicSunderer:

                AddEchoingBladeExcisionExtremisIndicators(log, replay, target.AgentItem);
                AddSwordSwings(log, replay, target.AgentItem, EffectGUIDs.NexusOfEternityChampionSundererEchoingAttackExcisionSwordSwing, 400);

                break;
            default:
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

    internal static void AscensionOrbStabilization(AgentData agentData, List<CombatItem> combatData)
    {
        var candidates = combatData
            .Where(x => x.IsStateChange == StateChange.MaxHealthUpdate && MaxHealthUpdateEvent.GetMaxHealth(x) == 14940)
            .Select(x => agentData.GetAgent(x.SrcAgent, x.Time))
            .Where(x => x.Type == AgentItem.AgentType.VolatileSpecies && x.HitboxWidth == 16)
            .Distinct()
            .ToHashSet();

        foreach (var orb in candidates)
        {
            orb.OverrideID(TargetID.AscensionOrb, agentData);
            orb.OverrideName("Ascension Orb");
        }
    }

    internal static void RenameAdds(IReadOnlyList<SingleActor> actors)
    {
        foreach (SingleActor actor in actors)
        {
            switch (actor.ID)
            {
                case (int)TargetID.ChampionCosmicBulwark:
                case (int)TargetID.ChampionCosmicPiercer:
                case (int)TargetID.ChampionCosmicSunderer:
                case (int)TargetID.ChampionAspectOfTheSpear:
                case (int)TargetID.ChampionAspectOfTheStaff:
                    actor.OverrideName("Champion " + actor.Character);
                    break;
                case (int)TargetID.EliteCosmicBulwark:
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
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.NexusOfEternitySurroundingCurseRagingStormExplosions, out var surrCurseDamage))
        {
            foreach (var effect in surrCurseDamage)
            {
                // Duration is 2666, for the replay it's way too long
                // Override it to 250 for a brief visual
                // Conflicts with Raging Storm which is 150 radius
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
                // Base radius 200
                // Duration 1500 - Scale 3.0 - Echoing Blade - 600 radius - Vloxx
                // Duration 2500 - Scale 2.5 - Excision Extremis - 500 radius - Vloxx
                // Duration 1500 - Scale 2.0 - Excision - 400 radius - Sunderer
                uint radius = (uint)(200 * effect.Scale);
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

    private static void AddThousandStrikes(ParsedEvtcLog log, CombatReplay replay, AgentItem agent, long skill)
    {
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(agent, EffectGUIDs.NexusOfEternityThousandStrikesIndicator, out var thousandStrikesIndicators))
        {
            foreach (var effect in thousandStrikesIndicators)
            {
                // Base radius 100
                // Duration 4000 - Scale 16.5 - Vloxx
                // Duration 2500 - Scale 11.5 - Spear
                uint radius = (uint)(100 * effect.Scale);
                (long start, long end) lifespan = (effect.Time, effect.Time + effect.Duration);
                var pie = (PieDecoration)new PieDecoration(radius, 135, lifespan, Colors.LightOrange, 0.2, new PositionConnector(effect.Position)).UsingRotationConnector(new AngleConnector(effect.Rotation.Z + 90));
                replay.Decorations.AddWithBorder(pie, Colors.LightOrange, 0.2);
            }

            // Decoration above indicator
            var thousandStrikes = log.CombatData.GetMissileEventsBySkillID(skill);
            replay.Decorations.AddNonHomingMissiles(log, thousandStrikes, Colors.LightBlue, 0.3, 20);
        }
    }

    private static void AddRagingStorm(ParsedEvtcLog log, CombatReplay replay, AgentItem agent)
    {
        // Vloxx - 3000 duration - Scale 1.5
        // Elite Bulwark - 1500 duration - Scale 1.5
        // Champion Bulwark - 1500 duration - Scale 1.5
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(agent, EffectGUIDs.NexusOfEternityRagingStormIndicator, out var ragingStorm))
        {
            foreach (var effect in ragingStorm)
            {
                (long start, long end) lifespan = effect.ComputeLifespan(log, effect.Duration);
                var circle = new CircleDecoration(150, lifespan, Colors.LightOrange, 0.2, new PositionConnector(effect.Position));
                replay.Decorations.Add(circle);
            }
        }

        AddRagingStormMissiles(log, replay, [RagingStormCosmicBulwark]);
    }

    private static void AddRagingStormMissiles(ParsedEvtcLog log, CombatReplay replay, long[] ids)
    {
        // The missile in game is quite long and comes vertically, in the replay it disappears before hitting the AoE
        var ragingStorm = log.CombatData.GetMissileEventsBySkillIDs(ids);
        replay.Decorations.AddNonHomingMissiles(log, ragingStorm, Colors.MidTeal, 0.5, 20);
    }

    private static void AddCosmicPiercerAnnhilatingOrb(ParsedEvtcLog log, CombatReplay replay)
    {
        var orbs = log.CombatData.GetMissileEventsBySkillID(AnnihilatingOrbCosmicPiercer);
        replay.Decorations.AddNonHomingMissiles(log, orbs, Colors.LightBlue, 0.1, 240);
    }

    /// <summary>
    /// Elite Cosmic Bulwark and Champion Cosmic Bulwark
    /// </summary>
    private static void AddCosmicBulwarkWorldpiercer(ParsedEvtcLog log, CombatReplay replay, AgentItem agent)
    {
        (long start, long end) lifespan;

        // Worldpiercer - Arrow
        if (log.CombatData.TryGetEffectEventsByDstWithGUID(agent, EffectGUIDs.NexusOfEternityCosmicBulwarkWorldpiercerArrowIndicator, out var arrows))
        {
            foreach (var effect in arrows)
            {
                if (agent.TryGetCurrentFacingDirection(log, effect.Time, out var facing))
                {
                    lifespan = effect.ComputeLifespan(log, 5000);
                    var offset = new Vector3(700, 0, 0);
                    var arrow = new RectangleDecoration(1400, 100, lifespan, Colors.LightOrange, 0.2, new AgentConnector(agent).WithOffset(offset, true)).UsingRotationConnector(new AngleConnector(facing.Value));
                    replay.Decorations.Add(arrow);
                }
            }
        }

        // Worldpiercer - Puddle
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(agent, EffectGUIDs.NexusOfEternityCosmicBulwarkWorldpiercerAoE, out var bluePuddle))
        {
            foreach (var effect in bluePuddle)
            {
                lifespan = effect.ComputeDynamicLifespan(log, 8000);
                var circle = new CircleDecoration(180, lifespan, Colors.CobaltBlue, 0.4, new PositionConnector(effect.Position));
                replay.Decorations.Add(circle);
            }
        }

        // Worldpiercer - Red ring - Using a separated effect because the AoE might deal damage for 10s instead of 8s
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(agent, EffectGUIDs.NexusOfEternityCosmicBulwarkWorldpiercerAoE, out var redRing))
        {
            foreach (var effect in redRing)
            {
                lifespan = effect.ComputeDynamicLifespan(log, 10000);
                var circle = new DoughnutDecoration(175, 180, lifespan, Colors.Red, 0.4, new PositionConnector(effect.Position));
                replay.Decorations.Add(circle);
            }
        }
    }
}
