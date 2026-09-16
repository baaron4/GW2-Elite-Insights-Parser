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
}
