﻿using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class Golem : FightLogic
{
    public Golem(int id) : base(id)
    {
        ParseMode = ParseModeEnum.Benchmark;
        SkillMode = SkillModeEnum.PvE;
        EncounterID |= EncounterIDs.EncounterMasks.GolemMask;
        EncounterID |= 0x000100;
        switch (GetTargetID(id))
        {
            case TargetID.MassiveGolem10M:
                Extension = "MassiveGolem10M";
                Icon = EncounterIconMassiveGolem;
                EncounterID |= 0x000001;
                break;
            case TargetID.MassiveGolem4M:
                Extension = "MassiveGolem4M";
                Icon = EncounterIconMassiveGolem;
                EncounterID |= 0x000002;
                break;
            case TargetID.MassiveGolem1M:
                Extension = "MassiveGolem1M";
                Icon = EncounterIconMassiveGolem;
                EncounterID |= 0x000003;
                break;
            case TargetID.VitalGolem:
                Extension = "VitalGolem";
                Icon = EncounterIconVitalGolem;
                EncounterID |= 0x000004;
                break;
            case TargetID.AvgGolem:
                Extension = "AvgGolem";
                Icon = EncounterIconAvgGolem;
                EncounterID |= 0x000005;
                break;
            case TargetID.StdGolem:
                Extension = "StdGolem";
                Icon = EncounterIconStdGolem;
                EncounterID |= 0x000006;
                break;
            case TargetID.ConditionGolem:
                Extension = "ToughGolem";
                Icon = EncounterIconCondiPowerMedGolem;
                EncounterID |= 0x000007;
                break;
            case TargetID.PowerGolem:
                Extension = "ResGolem";
                Icon = EncounterIconCondiPowerMedGolem;
                EncounterID |= 0x000008;
                break;
            case TargetID.LGolem:
                Extension = "LGolem";
                Icon = EncounterIconLGolem;
                EncounterID |= 0x000009;
                break;
            case TargetID.MedGolem:
                Extension = "MedGolem";
                Icon = EncounterIconCondiPowerMedGolem;
                EncounterID |= 0x00000A;
                break;
        }
        EncounterCategoryInformation.Category = FightCategory.Golem;
        EncounterCategoryInformation.SubCategory = SubFightCategory.Golem;
    }

    protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
    {
        return new CombatReplayMap(CombatReplayGolem,
                        (895, 629),
                        (18115.12, -13978.016, 22590.12, -10833.016));
    }
    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        return
        [
            new BuffGainCastFinder(MushroomKingsBlessing, POV_MushroomKingsBlessingBuff).UsingICD(500),
        ];
    }
    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
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
        base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
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
                    var phase = new PhaseData(log.FightData.FightStart, hpUpdate.Time, numberNames[j]);
                    phase.AddTarget(mainTarget, log);
                    phases.Add(phase);
                }
            }
            var subPhases = GetPhasesByHealthPercent(log, mainTarget, thresholds);
            foreach (var subPhase in subPhases)
            {
                subPhase.AddParentPhases(phases);
            }
            phases.AddRange(subPhases);
        }
        AgentItem? pov = log.LogData.PoV;
        if (pov != null)
        {
            int combatPhase = 0;
            EnterCombatEvent? firstEnterCombat = log.CombatData.GetEnterCombatEvents(pov).FirstOrDefault();
            ExitCombatEvent? firstExitCombat = log.CombatData.GetExitCombatEvents(pov).FirstOrDefault();
            if (firstExitCombat != null && (log.FightData.FightEnd - firstExitCombat.Time) > 1000 && (firstEnterCombat == null || firstEnterCombat.Time >= firstExitCombat.Time))
            {
                var phase = new PhaseData(log.FightData.FightStart, firstExitCombat.Time, "In Combat " + (++combatPhase));
                phase.AddTarget(mainTarget, log);
                phases.Add(phase);
            }
            foreach (EnterCombatEvent ece in log.CombatData.GetEnterCombatEvents(pov))
            {
                ExitCombatEvent? exce = log.CombatData.GetExitCombatEvents(pov).FirstOrDefault(x => x.Time >= ece.Time);
                long phaseEndTime = exce != null ? exce.Time : log.FightData.FightEnd;
                var phase = new PhaseData(Math.Max(ece.Time, log.FightData.FightStart), Math.Min(phaseEndTime, log.FightData.FightEnd), "PoV in Combat " + (++combatPhase));
                phase.AddTarget(mainTarget, log);
                phases.Add(phase);
            }
        }

        return phases;
    }

    internal override long GetFightOffset(EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData)
    {
        CombatItem? logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.LogNPCUpdate);
        if (logStartNPCUpdate != null)
        {
            AgentItem golem = (agentData.GetNPCsByIDAndAgent(GenericTriggerID, logStartNPCUpdate.DstAgent).FirstOrDefault() ?? agentData.GetNPCsByID(GenericTriggerID).FirstOrDefault()) ?? throw new MissingKeyActorsException("Golem not found");
            return GetFirstDamageEventTime(fightData, agentData, combatData, golem);
        }
        return GetGenericFightOffset(fightData);
    }

    internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
    {
        return FightData.EncounterMode.NotApplicable;
    }

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        SingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(GenericTriggerID)) ?? throw new MissingKeyActorsException("Golem not found");
        long fightEndLogTime = mainTarget.LastAware;
        bool success = false;
        DeadEvent? deadEvt = combatData.GetDeadEvents(mainTarget.AgentItem).LastOrDefault();
        if (deadEvt != null)
        {
            fightEndLogTime = deadEvt.Time;
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
                    fightEndLogTime = lastDamageTaken.Time;
                }
            }
        }
        fightData.SetSuccess(success, fightEndLogTime);
    }

    internal override IReadOnlyList<TargetID>  GetTargetsIDs()
    {
        return new[] { GetTargetID(GenericTriggerID) };
    }
}
