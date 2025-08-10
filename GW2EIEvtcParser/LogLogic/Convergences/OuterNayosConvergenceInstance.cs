using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.LogLogic.LogCategories;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicTimeUtils;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal class OuterNayosConvergenceInstance : ConvergenceLogic
{
    public OuterNayosConvergenceInstance(int triggerID) : base(triggerID)
    {
        LogCategoryInformation.SubCategory = SubLogCategory.OuterNayosConvergence;
        LogID |= LogIDs.ConvergenceMasks.OuterNayosMask;
        Icon = InstanceIconOuterNayos;
        Extension = "outnayconv";
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData)
    {
        var mainBoss = Targets.FirstOrDefault(x => x.IsAnySpecies([
            TargetID.DemonKnight,
            TargetID.Sorrow,
            TargetID.Dreadwing,
            TargetID.HellSister,
            TargetID.UmbrielHalberdOfHouseAurkus
            ]));
        var name = "Convergence: Outer Nayos";
        if (mainBoss != null)
        {
            switch (mainBoss.ID)
            {
                case (int)TargetID.DemonKnight:
                    LogID |= 0x000001;
                    name += " - Demon Knight";
                    Extension += "dmnknght";
                    break;
                case (int)TargetID.Sorrow:
                    LogID |= 0x000002;
                    name += " - Sorrow";
                    Extension += "srrw";
                    break;
                case (int)TargetID.Dreadwing:
                    LogID |= 0x000003;
                    name += " - Dreadwing";
                    Extension += "drdwng";
                    break;
                case (int)TargetID.HellSister:
                    LogID |= 0x000004;
                    name += " - Hell Sister";
                    Extension += "sister";
                    break;
                case (int)TargetID.UmbrielHalberdOfHouseAurkus:
                    LogID |= 0x000005;
                    name += " - Umbriel";
                    Extension += "umbriel";
                    break;

            }
        }
        return name;
    }

    protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
    {
        return new CombatReplayMap(CombatReplayOuterNayos,
            (1800, 2000),
             (-15360, -15360, 15360, 15360)); // TODO Fix values
    }

    internal override IReadOnlyList<TargetID> GetTargetsIDs()
    {
        return
        [
            TargetID.DemonKnight,
            TargetID.Sorrow,
            TargetID.Dreadwing,
            TargetID.HellSister,
            TargetID.UmbrielHalberdOfHouseAurkus,
        ];
    }

    internal override IReadOnlyList<TargetID> GetFriendlyNPCIDs()
    {
        return
        [
            TargetID.ZojjaNayos,
        ];
    }

    internal override LogData.LogMode GetLogMode(CombatData combatData, AgentData agentData, LogData logData)
    {
        return combatData.GetBuffApplyData(UnstableAttunementSotO).Any(x => x.To.IsPlayer) ? LogData.LogMode.CM : LogData.LogMode.Normal;
    }

    internal override LogData.InstancePrivacyMode GetInstancePrivacyMode(CombatData combatData, AgentData agentData, LogData logData)
    {
        return combatData.GetMapIDEvents().Any(x => x.MapID == MapIDs.OuterNayosPublicConvergence) ? LogData.InstancePrivacyMode.Public : LogData.InstancePrivacyMode.Private;
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        var phases = GetInitialPhase(log);
        phases[0].AddTargets(Targets.Where(x => x.IsAnySpecies([TargetID.DemonKnight,
            TargetID.Sorrow,
            TargetID.Dreadwing,
            TargetID.HellSister,
            TargetID.UmbrielHalberdOfHouseAurkus])), log);
        var target = Targets.FirstOrDefault(x => x.IsAnySpecies([
            TargetID.DemonKnight,
            TargetID.Sorrow,
            TargetID.Dreadwing,
            TargetID.HellSister,
            TargetID.UmbrielHalberdOfHouseAurkus
            ]));
        if (target == null)
        {
            return phases;
        }
        IReadOnlyList<Segment> hpUpdates = target.GetHealthUpdates(log);

        // Full Fight Phase
        string phaseName = "";
        switch (target.ID)
        {
            case (int)TargetID.DemonKnight:
                phaseName = "Full Demon Knight";
                break;
            case (int)TargetID.Sorrow:
                phaseName = "Full Sorrow";
                break;
            case (int)TargetID.Dreadwing:
                phaseName = "Full Dreadwing";
                break;
            case (int)TargetID.HellSister:
                phaseName = "Full Hell Sister";
                break;
            case (int)TargetID.UmbrielHalberdOfHouseAurkus:
                phaseName = "Full Umbriel";
                break;
        }
        var fullPhase = new EncounterPhaseData(Math.Max(log.LogData.LogStart, target.FirstAware), Math.Min(target.LastAware, log.LogData.LogEnd), phaseName, log).WithParentPhase(phases[0]);
        fullPhase.AddTarget(target, log);
        phases.Add(fullPhase);

        // Sub Phases
        Segment start = hpUpdates.FirstOrDefault(x => x.Value <= 100.0 && x.Value != 0 && x.Start != 0);
        Segment end75 = hpUpdates.FirstOrDefault(x => x.Value < 75.0 && x.Value != 0);
        Segment start75 = hpUpdates.FirstOrDefault(x => x.Value < 75.0 && x.Value != 0 && x.Start > end75.End);
        Segment end50 = hpUpdates.FirstOrDefault(x => x.Value < 50.0 && x.Value != 0);
        Segment start50 = hpUpdates.FirstOrDefault(x => x.Value < 50.0 && x.Value != 0 && x.Start > end50.End);
        Segment end25 = hpUpdates.FirstOrDefault(x => x.Value < 25.0 && x.Value != 0);
        Segment final = hpUpdates.FirstOrDefault(x => x.Value < 25.0 && x.Start > end25.End);

        // 100-75, 75-50, 50-25, 25-0
        var phase1 = new PhaseData(start.Start, Math.Min(end75.Start, log.LogData.LogEnd), "Phase 1").WithParentPhase(fullPhase);
        var phase2 = new PhaseData(start75.Start, Math.Min(end50.Start, log.LogData.LogEnd), "Phase 2").WithParentPhase(fullPhase);
        var phase3 = new PhaseData(start50.Start, Math.Min(end25.Start, log.LogData.LogEnd), "Phase 3").WithParentPhase(fullPhase);
        var phase4 = new PhaseData(final.Start, Math.Min(target.AgentItem.LastAware, log.LogData.LogEnd), "Phase 4").WithParentPhase(fullPhase);

        phase1.AddTarget(target, log);
        phase2.AddTarget(target, log);
        phase3.AddTarget(target, log);
        phase4.AddTarget(target, log);

        phases.AddRange([phase1, phase2, phase3, phase4]);

        return phases;
    }
}
