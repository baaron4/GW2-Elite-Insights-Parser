using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.LogLogic.LogCategories;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicTimeUtils;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal class Golem : LogLogic
{
    public Golem(int id) : base(id)
    {
        ParseMode = ParseModeEnum.Benchmark;
        SkillMode = SkillModeEnum.PvE;
        LogID |= LogIDs.LogMasks.GolemMask;
        LogID |= 0x000100;
        switch (GetTargetID(id))
        {
            case TargetID.MassiveGolem10M:
                Extension = "MassiveGolem10M";
                Icon = EncounterIconMassiveGolem;
                LogID |= 0x000001;
                break;
            case TargetID.MassiveGolem4M:
                Extension = "MassiveGolem4M";
                Icon = EncounterIconMassiveGolem;
                LogID |= 0x000002;
                break;
            case TargetID.MassiveGolem1M:
                Extension = "MassiveGolem1M";
                Icon = EncounterIconMassiveGolem;
                LogID |= 0x000003;
                break;
            case TargetID.VitalGolem:
                Extension = "VitalGolem";
                Icon = EncounterIconVitalGolem;
                LogID |= 0x000004;
                break;
            case TargetID.AvgGolem:
                Extension = "AvgGolem";
                Icon = EncounterIconAvgGolem;
                LogID |= 0x000005;
                break;
            case TargetID.StdGolem:
                Extension = "StdGolem";
                Icon = EncounterIconStdGolem;
                LogID |= 0x000006;
                break;
            case TargetID.ConditionGolem:
                Extension = "ToughGolem";
                Icon = EncounterIconCondiPowerMedGolem;
                LogID |= 0x000007;
                break;
            case TargetID.PowerGolem:
                Extension = "ResGolem";
                Icon = EncounterIconCondiPowerMedGolem;
                LogID |= 0x000008;
                break;
            case TargetID.LGolem:
                Extension = "LGolem";
                Icon = EncounterIconLGolem;
                LogID |= 0x000009;
                break;
            case TargetID.MedGolem:
                Extension = "MedGolem";
                Icon = EncounterIconCondiPowerMedGolem;
                LogID |= 0x00000A;
                break;
        }
        LogCategoryInformation.Category = LogCategory.Golem;
        LogCategoryInformation.SubCategory = SubLogCategory.Golem;
    }

    internal override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log, CombatReplayDecorationContainer arenaDecorations)
    {
        var crMap = new CombatReplayMap(
                        (895, 629),
                        (18115.12, -13978.016, 22590.12, -10833.016));
        AddArenaDecorationsPerEncounter(log, arenaDecorations, LogID, CombatReplayGolem, crMap);
        return crMap;
    }
    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        return
        [
            new BuffGainCastFinder(MushroomKingsBlessing, POV_MushroomKingsBlessingBuff).UsingICD(500),
        ];
    }
    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        AgentItem target = agentData.GetNPCsByID(GenericTriggerID).FirstOrDefault() ?? throw new MissingKeyActorsException("Golem not found");
        foreach (CombatItem c in combatData)
        {
            // redirect all attacks to the main golem
            if (c.DstAgent == 0 && c.DstInstid == 0 && c.IsDamage(extensions))
            {
                c.OverrideDstAgent(target);
            }
        }
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputePlayerCombatReplayActors(p, log, replay);
#if DEBUG_EFFECTS
        ProfHelper.DEBUG_ComputeProfessionCombatReplayActors(p, log, replay);
#endif
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(GenericTriggerID)) ?? throw new MissingKeyActorsException("Golem not found");
        phases[0].Name = "Final Number";
        phases[0].AddTarget(mainTarget, log);
        if (!requirePhases)
        {
            return phases;
        }
        IReadOnlyList<HealthUpdateEvent> hpUpdates = log.CombatData.GetHealthUpdateEvents(mainTarget.AgentItem);
        if (hpUpdates.Count > 0)
        {
            var thresholds = new List<double> { 80, 60, 40, 20, 0 };
            string[] numberNames = ["First Number", "Second Number", "Third Number", "Fourth Number"];
            // Fifth number would the equivalent of full fight phase
            for (int j = 0; j < thresholds.Count - 1; j++)
            {
                HealthUpdateEvent? hpUpdate = hpUpdates.FirstOrDefault(x => x.HealthPercent <= thresholds[j]);
                if (hpUpdate != null)
                {
                    var phase = new SubPhasePhaseData(log.LogData.LogStart, hpUpdate.Time, numberNames[j]);
                    phase.AddTarget(mainTarget, log);
                    phases.Add(phase);
                }
            }
            var subPhases = GetPhasesByHealthPercent(log, mainTarget, thresholds, phases[0].Start, phases[0].End);
            foreach (var subPhase in subPhases)
            {
                subPhase.AddParentPhases(phases);
            }
            phases.AddRange(subPhases);
        }
        AgentItem? pov = log.LogMetadata.PoV;
        if (pov != null)
        {
            int combatPhase = 0;
            EnterCombatEvent? firstEnterCombat = log.CombatData.GetEnterCombatEvents(pov).FirstOrDefault();
            ExitCombatEvent? firstExitCombat = log.CombatData.GetExitCombatEvents(pov).FirstOrDefault();
            if (firstExitCombat != null && (log.LogData.LogEnd - firstExitCombat.Time) > 1000 && (firstEnterCombat == null || firstEnterCombat.Time >= firstExitCombat.Time))
            {
                var phase = new SubPhasePhaseData(log.LogData.LogStart, firstExitCombat.Time, "In Combat " + (++combatPhase));
                phase.AddTarget(mainTarget, log);
                phases.Add(phase);
            }
            foreach (EnterCombatEvent ece in log.CombatData.GetEnterCombatEvents(pov))
            {
                ExitCombatEvent? exce = log.CombatData.GetExitCombatEvents(pov).FirstOrDefault(x => x.Time >= ece.Time);
                long phaseEndTime = exce != null ? exce.Time : log.LogData.LogEnd;
                var phase = new SubPhasePhaseData(Math.Max(ece.Time, log.LogData.LogStart), Math.Min(phaseEndTime, log.LogData.LogEnd), "PoV in Combat " + (++combatPhase));
                phase.AddTarget(mainTarget, log);
                phases.Add(phase);
            }
        }

        return phases;
    }

    internal override long GetLogOffset(EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData)
    {
        CombatItem? logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.LogNPCUpdate);
        if (logStartNPCUpdate != null)
        {
            AgentItem golem = (agentData.GetNPCsByIDAndAgent(GenericTriggerID, logStartNPCUpdate.DstAgent).FirstOrDefault() ?? agentData.GetNPCsByID(GenericTriggerID).FirstOrDefault()) ?? throw new MissingKeyActorsException("Golem not found");
            return GetFirstDamageEventTime(logData, agentData, combatData, golem);
        }
        return GetGenericLogOffset(logData);
    }

    internal override LogData.LogMode GetLogMode(CombatData combatData, AgentData agentData, LogData logData)
    {
        return LogData.LogMode.NotApplicable;
    }

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, LogData logData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        SingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(GenericTriggerID)) ?? throw new MissingKeyActorsException("Golem not found");
        long encounterEndTime = mainTarget.LastAware;
        bool success = false;
        DeadEvent? deadEvt = combatData.GetDeadEvents(mainTarget.AgentItem).LastOrDefault();
        if (deadEvt != null)
        {
            encounterEndTime = deadEvt.Time;
            success = true;
        }
        else
        {
            IReadOnlyList<HealthUpdateEvent> hpUpdates = combatData.GetHealthUpdateEvents(mainTarget.AgentItem);
            if (hpUpdates.Count > 0)
            {
                HealthDamageEvent? lastDamageTaken = combatData.GetDamageTakenData(mainTarget.AgentItem).LastOrDefault(x => x.HealthDamage > 0);
                success = hpUpdates.Last().HealthPercent < 2.00;
                if (success && lastDamageTaken != null)
                {
                    encounterEndTime = lastDamageTaken.Time;
                }
            }
        }
        logData.SetSuccess(success, encounterEndTime);
    }

    internal override IReadOnlyList<TargetID>  GetTargetsIDs()
    {
        return new[] { GetTargetID(GenericTriggerID) };
    }
}
