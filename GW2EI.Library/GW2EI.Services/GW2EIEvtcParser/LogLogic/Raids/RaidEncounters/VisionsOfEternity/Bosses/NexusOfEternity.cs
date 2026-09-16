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
            new PlayerDstHealthDamageHitMechanic([AnnihilatingOrb1, AnnihilatingOrb2], Mech_AnnihilatingOrb, new MechanicPlotlySetting(Symbols.Circle, Colors.Black), new MechanicDescription("AnnOrb.H", "Hit by Annihilating Orb (Vloxx)", "Annihilating Orb Hit (Vloxx)"), Sev0),
            new PlayerDstHealthDamageHitMechanic(AnnihilatingOrb3, Mech_AnnihilatingOrb2, new MechanicPlotlySetting(Symbols.Circle, Colors.Blue), new MechanicDescription("AnnOrb2.H", "Hit by Annihilating Orb (Add?)", "Annihilating Orb Hit (Add?)"), Sev0),
        ]),
        new PlayerDstHealthDamageHitMechanic([JudgmentOfEternity1, JudgmentOfEternity2], Mech_JudgmentOfEternity, new MechanicPlotlySetting(Symbols.Bowtie, Colors.DarkMagenta), new MechanicDescription("JudgEter.H", "Hit by Judgment of Eternity", "Judgment of Eternity Hit"), Sev0),
        new PlayerDstHealthDamageHitMechanic(DivisionEternal, Mech_DivisionEternal, new MechanicPlotlySetting(Symbols.BowtieOpen, Colors.DarkRed), new MechanicDescription("DivEter.H", "Hit by Division Eternal", "Division Eternal Hit"), Sev0),
        new MechanicGroup([
            new PlayerDstHealthDamageHitMechanic(SurroundingCurse, Mech_SurroundingCurse, new MechanicPlotlySetting(Symbols.CircleCross, Colors.Teal), new MechanicDescription("SurrCur.H", "Hit by Surrounding Curse (Vloxx)", "Surrounding Curse Hit (Vloxx)"), Sev0),
            new PlayerDstHealthDamageHitMechanic(SurroundingCurse2, Mech_SurroundingCurse2, new MechanicPlotlySetting(Symbols.CircleCross, Colors.White), new MechanicDescription("SurrCur2.H", "Hit by Surrounding Curse (Add?)", "Surrounding Curse Hit (Add?)"), Sev0),
        ]),
        new MechanicGroup([
            new PlayerDstHealthDamageHitMechanic(CosmicCharge, Mech_CosmicCharge, new MechanicPlotlySetting(Symbols.CircleCrossOpen, Colors.Teal), new MechanicDescription("CosmChar.H", "Hit by Cosmic Charge (Vloxx)", "Cosmic Charge Hit (Vloxx)"), Sev0),
            new PlayerDstHealthDamageHitMechanic([CosmicCharge1, CosmicCharge2], Mech_CosmicCharge2, new MechanicPlotlySetting(Symbols.CircleCrossOpen, Colors.White), new MechanicDescription("CosmChar2.H", "Hit by Cosmic Charge (Add?)", "Cosmic Charge Hit (Add?)"), Sev0),
        ]),
        new MechanicGroup([
            new PlayerDstHealthDamageHitMechanic(ThousandStrikes1, Mech_ThousandStrikes, new MechanicPlotlySetting(Symbols.CircleOpen, Colors.LightPink), new MechanicDescription("ThouStr.H", "Hit by Thousand Strikes (Vloxx)", "Thousand Strikes Hit (Vloxx)"), Sev0),
            new PlayerDstHealthDamageHitMechanic(ThousandStrikes2, Mech_ThousandStrikes2, new MechanicPlotlySetting(Symbols.CircleOpen, Colors.Orange), new MechanicDescription("ThouStr2.H", "Hit by Thousand Strikes (Add?)", "Thousand Strikes Hit (Add?)"), Sev0),
        ]),
        new PlayerDstHealthDamageHitMechanic(SliceThroughReality, Mech_SliceThroughReality, new MechanicPlotlySetting(Symbols.CircleOpenDot, Colors.DarkBlue), new MechanicDescription("SlicReal.H", "Hit by Slice Through Reality", "Slice Through Reality Hit"), Sev0),
        new MechanicGroup([
            new PlayerDstHealthDamageHitMechanic(VisionsOfEternity2, Mech_VisionsOfEternity, new MechanicPlotlySetting(Symbols.CircleX, Colors.LightBlue), new MechanicDescription("VisEter.H", "Hit by Visions of Eternity (Vloxx)", "Visions of Eternity Hit (Vloxx)"), Sev0),
            new PlayerDstHealthDamageHitMechanic([VisionsOfEternity1, VisionsOfEternity3], Mech_VisionsOfEternity2, new MechanicPlotlySetting(Symbols.CircleX, Colors.Lime), new MechanicDescription("VisEter2.H", "Hit by Visions of Eternity (Add?)", "Visions of Eternity Hit (Add?)"), Sev0),
        ]),
        new PlayerDstHealthDamageHitMechanic(ProbabilityDistribution, Mech_ProbabilityDistribution, new MechanicPlotlySetting(Symbols.CircleXOpen, Colors.Sand), new MechanicDescription("ProbDist.H", "Hit by Probability Distribution", "Probability Distribution Hit"), Sev0),
        new MechanicGroup([
            new PlayerDstHealthDamageHitMechanic([RagingStorm, RagingStorm2], Mech_RagingStorm, new MechanicPlotlySetting(Symbols.Cross, Colors.RedBrownish), new MechanicDescription("RagStor.H", "Hit by Raging Storm (Vloxx)", "Raging Storm Hit (Vloxx)"), Sev0),
            new PlayerDstHealthDamageHitMechanic(RagingStorm1, Mech_RagingStorm2, new MechanicPlotlySetting(Symbols.Cross, Colors.LightRed), new MechanicDescription("RagStor2.H", "Hit by Raging Storm (Add?)", "Raging Storm Hit (Add?)"), Sev0),
        ]),
        new PlayerDstHealthDamageHitMechanic([ExcisionExtremis1, ExcisionExtremis2], Mech_ExcisionExtremis, new MechanicPlotlySetting(Symbols.CrossOpen, Colors.DarkerLime), new MechanicDescription("ExciExtr.H", "Hit by Excision Extremis", "Excision Extremis Hit"), Sev0),
        new MechanicGroup([
            new PlayerDstHealthDamageHitMechanic(Worldpiercer1, Mech_Worldpiercer, new MechanicPlotlySetting(Symbols.Diamond, Colors.FluoOrange), new MechanicDescription("Worldpier.H", "Hit by Worldpiercer (Vloxx)", "Worldpiercer Hit (Vloxx)"), Sev0),
            new PlayerDstHealthDamageHitMechanic(Worldpiercer2, Mech_Worldpiercer2, new MechanicPlotlySetting(Symbols.Diamond, Colors.LightBlue), new MechanicDescription("Worldpier2.H", "Hit by Worldpiercer (Add?)", "Worldpiercer Hit (Add?)"), Sev0),
        ]),
        new PlayerDstHealthDamageHitMechanic(AscensionsSacrifice, Mech_AscensionsSacrifice, new MechanicPlotlySetting(Symbols.DiamondOpen, Colors.Chocolate), new MechanicDescription("AscSac.H", "Hit by Ascension's Sacrifice", "Ascension's Sacrifice Hit"), Sev0),
        new MechanicGroup([
            new PlayerDstHealthDamageHitMechanic(EternalReflection3, Mech_EternalReflection, new MechanicPlotlySetting(Symbols.DiamondTall, Colors.DarkMagenta), new MechanicDescription("EterRefl.H", "Hit by Eternal Reflection (Vloxx)", "Eternal Reflection Hit (Vloxx)"), Sev0),
            new PlayerDstHealthDamageHitMechanic([EternalReflection, EternalReflection1, EternalReflection2], Mech_EternalReflection2, new MechanicPlotlySetting(Symbols.DiamondTall, Colors.DesaturatedPink), new MechanicDescription("EterRefl2.H", "Hit by Eternal Reflection (Add?)", "Eternal Reflection Hit (Add?)"), Sev0),
        ]),
        new PlayerDstHealthDamageHitMechanic(EchoingBlade, Mech_EchoingBlade, new MechanicPlotlySetting(Symbols.DiamondWide, Colors.DarkYellow), new MechanicDescription("EchoBlad.H", "Hit by Echoing Blade", "Echoing Blade Hit"), Sev0),
        new EnemyDstBuffApplyMechanic(EmpoweredNexusOfEternity, Mech_Empowered, new MechanicPlotlySetting(Symbols.DiamondWideOpen, Colors.Red), new MechanicDescription("Emp.A", "Applied Empowered", "Empowered Applied"), Sev0),
        new EnemyDstBuffApplyMechanic(DamageImmunity, Mech_DamageImmunity, new MechanicPlotlySetting(Symbols.Hexagon, Colors.LightBlue), new MechanicDescription("DmgImm.A", "Applied Damage Immunity", "Damage Immunity Applied"), Sev0),
        new PlayerDstBuffApplyMechanic(Ascension, Mech_Ascension, new MechanicPlotlySetting(Symbols.HexagonOpen, Colors.Grey), new MechanicDescription("Ascen.A", "Applied Ascension", "Ascension Applied"), Sev0),
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

        List<TargetID> champions =
        [
            TargetID.ChampionCosmicBulwark,
            TargetID.ChampionCosmicPiercer,
            TargetID.ChampionCosmicSunderer,
            TargetID.ChampionAspectOfTheSpear,
            TargetID.ChampionAspectOfTheStaff,
            // TargetID.SomethingCosmicPiercer,
        ];

        for (int i = 0; i < phases.Count; i++)
        {
            int index = i + 1;
            PhaseData phase = phases[i];
            phase.AddParentPhase(encounterPhase);
            if (index % 2 == 0)
            {
                phase.Name = "Split " + (index) / 2;
                AddTargetsToPhaseAndFit(phase, targets, champions, log);
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
        return [];
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

    }
    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeNPCCombatReplayActors(target, log, replay);
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
}
