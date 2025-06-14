using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class OuterNayosConvergenceInstance : ConvergenceLogic
{
    public OuterNayosConvergenceInstance(int triggerID) : base(triggerID)
    {
        EncounterCategoryInformation.SubCategory = SubFightCategory.OuterNayosConvergence;
        EncounterID |= EncounterIDs.ConvergenceMasks.OuterNayosMask;
        Icon = InstanceIconOuterNayos;
        Extension = "outnayconv";
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData)
    {
        return "Convergence: Outer Nayos";
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

    internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
    {
        return combatData.GetBuffApplyData(UnstableAttunementSotO).Any(x => x.To.IsPlayer) ? FightData.EncounterMode.CM : FightData.EncounterMode.Normal;
    }

    internal override FightData.InstancePrivacyMode GetInstancePrivacyMode(CombatData combatData, AgentData agentData, FightData fightData)
    {
        return combatData.GetMapIDEvents().Any(x => x.MapID == MapIDs.OuterNayosPublicConvergence) ? FightData.InstancePrivacyMode.Public : FightData.InstancePrivacyMode.Private;
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        var phases = GetInitialPhase(log);
        phases[0].AddTargets(Targets, log);
        SingleActor target = Targets.FirstOrDefault(x => x.IsAnySpecies([
            TargetID.DemonKnight,
            TargetID.Sorrow,
            TargetID.Dreadwing,
            TargetID.HellSister,
            TargetID.UmbrielHalberdOfHouseAurkus
            ])) ?? throw new MissingKeyActorsException("Demon Knight / Sorrow / Dreadwing / Hell Sister / Umbriel not found");
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
        var fullPhase = new PhaseData(Math.Max(log.FightData.FightStart, target.FirstAware), Math.Min(target.LastAware, log.FightData.FightEnd), phaseName).WithParentPhase(phases[0]);
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
        var phase1 = new PhaseData(start.Start, Math.Min(end75.Start, log.FightData.FightEnd), "Phase 1").WithParentPhase(fullPhase);
        var phase2 = new PhaseData(start75.Start, Math.Min(end50.Start, log.FightData.FightEnd), "Phase 2").WithParentPhase(fullPhase);
        var phase3 = new PhaseData(start50.Start, Math.Min(end25.Start, log.FightData.FightEnd), "Phase 3").WithParentPhase(fullPhase);
        var phase4 = new PhaseData(final.Start, Math.Min(target.AgentItem.LastAware, log.FightData.FightEnd), "Phase 4").WithParentPhase(fullPhase);

        phase1.AddTarget(target, log);
        phase2.AddTarget(target, log);
        phase3.AddTarget(target, log);
        phase4.AddTarget(target, log);

        phases.AddRange([phase1, phase2, phase3, phase4]);

        return phases;
    }
}
