using GW2EIEvtcParser.EIData;
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

internal class MountBalriorConvergenceInstance : ConvergenceLogic
{
    public MountBalriorConvergenceInstance(int triggerID) : base(triggerID)
    {
        EncounterID = 0;
        EncounterCategoryInformation.SubCategory = SubFightCategory.MountBalriorConvergence;
        EncounterID |= EncounterIDs.ConvergenceMasks.MountBalriorMask;
        Icon = InstanceIconMountBalrior;
        Extension = "mntbalrconv";
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData)
    {
        return "Convergence: Mount Balrior";
    }

    protected override IReadOnlyList<TargetID> GetTargetsIDs()
    {
        return
        [
            TargetID.GreerTheBlightbringerConv,
            TargetID.GreeTheBingerConv,
            TargetID.ReegTheBlighterConv,
            TargetID.DecimaTheStormsingerConv,
            TargetID.UraTheSteamshriekerConv,
        ];
    }

    protected override IReadOnlyList<TargetID> GetUniqueNPCIDs()
    {
        return
        [
            TargetID.GreerTheBlightbringerConv,
            TargetID.GreeTheBingerConv,
            TargetID.ReegTheBlighterConv,
            TargetID.DecimaTheStormsingerConv,
            TargetID.UraTheSteamshriekerConv,
        ];
    }

    internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
    {
        return combatData.GetBuffApplyData(UnstableAttunementJW).Any(x => x.To.IsPlayer) ? FightData.EncounterMode.CM : FightData.EncounterMode.Normal;
    }

    internal override long GetFightOffset(EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData)
    {
        return GetGenericFightOffset(fightData);
    }

    internal override FightData.InstancePrivacyMode GetInstancePrivacyMode(CombatData combatData, AgentData agentData, FightData fightData)
    {
        return combatData.GetMapIDEvents().Any(x => x.MapID == MapIDs.MountBalriorPublicConvergence) ? FightData.InstancePrivacyMode.Public : FightData.InstancePrivacyMode.Private;
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases;
        if (Targets.Count == 0)
        {
            phases = base.GetPhases(log, requirePhases);
            if (log.CombatData.GetEvtcVersionEvent().Build >= ArcDPSBuilds.LogStartLogEndPerCombatSequenceOnInstanceLogs)
            {
                var fightPhases = GetPhasesBySquadCombatStartEnd(log);
                fightPhases.ForEach(x =>
                {
                    x.AddTargets(phases[0].Targets.Keys, log);
                    x.AddParentPhase(phases[0]);
                });
                phases.AddRange(fightPhases);
            }
            return phases;
        }

        phases = GetInitialPhase(log);
        phases[0].AddTargets(Targets, log);
        SingleActor target = Targets.FirstOrDefault(x => x.IsAnySpecies([TargetID.DecimaTheStormsingerConv, TargetID.GreerTheBlightbringerConv, TargetID.UraTheSteamshriekerConv])) ?? throw new MissingKeyActorsException("Decima / Greer / Ura not found");
        IReadOnlyList<Segment> hpUpdates = target.GetHealthUpdates(log);

        // Full Fight Phase
        string phaseName = "";
        switch (target.ID)
        {
            case (int)TargetID.DecimaTheStormsingerConv:
                phaseName = "Full Decima";
                break;
            case (int)TargetID.GreerTheBlightbringerConv:
                phaseName = "Full Greer";
                break;
            case (int)TargetID.UraTheSteamshriekerConv:
                phaseName = "Full Ura";
                break;
        }
        var fullPhase = new PhaseData(Math.Max(log.FightData.FightStart, target.FirstAware), Math.Min(target.LastAware, log.FightData.FightEnd), phaseName);
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

        // 100-75, Warclaw, 75-50, Warclaw, 50-25, Warclaw, 25-0
        PhaseData phase1 = new(start.Start, Math.Min(end75.Start, log.FightData.FightEnd), "Phase 1");
        PhaseData phase2 = new(start75.Start, Math.Min(end50.Start, log.FightData.FightEnd), "Phase 2");
        PhaseData phase3 = new(start50.Start, Math.Min(end25.Start, log.FightData.FightEnd), "Phase 3");
        PhaseData phase4 = new(final.Start, Math.Min(target.AgentItem.LastAware, log.FightData.FightEnd), "Phase 4");
        PhaseData warclaw1 = new(end75.Start, Math.Min(start75.Start, log.FightData.FightEnd), "Warclaw 1");
        PhaseData warclaw2 = new(end50.Start, Math.Min(start50.Start, log.FightData.FightEnd), "Warclaw 2");
        PhaseData warclaw3 = new(end25.Start, Math.Min(final.Start, log.FightData.FightEnd), "Warclaw 3");

        phase1.AddTarget(target, log);
        phase2.AddTarget(target, log);
        phase3.AddTarget(target, log);
        phase4.AddTarget(target, log);
        warclaw1.AddTarget(target, log);
        warclaw2.AddTarget(target, log);
        warclaw3.AddTarget(target, log);
        phase1.CanBeASubPhaseOf(fullPhase);
        phase2.CanBeASubPhaseOf(fullPhase);
        phase3.CanBeASubPhaseOf(fullPhase);
        phase4.CanBeASubPhaseOf(fullPhase);
        warclaw1.CanBeASubPhaseOf(fullPhase);
        warclaw2.CanBeASubPhaseOf(fullPhase);
        warclaw3.CanBeASubPhaseOf(fullPhase);

        phases.AddRange([phase1, phase2, phase3, phase4, warclaw1, warclaw2, warclaw3]);

        return phases;
    }

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        base.CheckSuccess(combatData, agentData, fightData, playerAgents);

        RewardEvent? reward = combatData.GetRewardEvents().FirstOrDefault(x => 
            x.RewardType == RewardTypes.ConvergenceReward1 ||
            x.RewardType == RewardTypes.ConvergenceReward2);
        if (reward != null)
        {
            fightData.SetSuccess(true, reward.Time);
        }
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        AgentItem? ura = agentData.GetNPCsByID(TargetID.UraTheSteamshriekerConv).FirstOrDefault();
        ura?.OverrideName("Ura, the Steamshrieker");

        base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
    }
}
