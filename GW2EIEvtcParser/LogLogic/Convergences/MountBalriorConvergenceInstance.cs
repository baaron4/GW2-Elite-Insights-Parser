using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.LogLogic.LogCategories;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicTimeUtils;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal class MountBalriorConvergenceInstance : ConvergenceLogic
{
    public MountBalriorConvergenceInstance(int triggerID) : base(triggerID)
    {
        LogCategoryInformation.SubCategory = SubLogCategory.MountBalriorConvergence;
        LogID |= LogIDs.ConvergenceMasks.MountBalriorMask;
        Icon = InstanceIconMountBalrior;
        Extension = "mntbalrconv";
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData)
    {
        var mainBoss = Targets.FirstOrDefault(x => x.IsAnySpecies([TargetID.DecimaTheStormsingerConv, TargetID.GreerTheBlightbringerConv, TargetID.UraTheSteamshriekerConv]));
        var name = "Convergence: Mount Balrior";
        if (mainBoss != null)
        {
            switch (mainBoss.ID)
            {
                case (int)TargetID.GreerTheBlightbringerConv:
                    LogID |= 0x000001;
                    name += " - Greer";
                    Extension += "greer";
                    break;
                case (int)TargetID.DecimaTheStormsingerConv:
                    LogID |= 0x000002;
                    name += " - Decima";
                    Extension += "dec";
                    break;
                case (int)TargetID.UraTheSteamshriekerConv:
                    LogID |= 0x000003;
                    name += " - Ura";
                    Extension += "ura";
                    break;

            }
        }
        return name;
    }

    protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
    {
        return new CombatReplayMap(CombatReplayMountBalrior, 
            (1280, 1280),
            (-15454, -22004, 20326, 20076));
    }

    internal override IReadOnlyList<TargetID> GetTargetsIDs()
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

    internal override LogData.LogMode GetLogMode(CombatData combatData, AgentData agentData, LogData logData)
    {
        return combatData.GetBuffApplyData(UnstableAttunementJW).Any(x => x.To.IsPlayer) ? LogData.LogMode.CM : LogData.LogMode.Normal;
    }

    internal override LogData.InstancePrivacyMode GetInstancePrivacyMode(CombatData combatData, AgentData agentData, LogData logData)
    {
        return combatData.GetMapIDEvents().Any(x => x.MapID == MapIDs.MountBalriorPublicConvergence) ? LogData.InstancePrivacyMode.Public : LogData.InstancePrivacyMode.Private;
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        var phases = GetInitialPhase(log);
        phases[0].AddTargets(Targets.Where(x => x.IsAnySpecies([TargetID.DecimaTheStormsingerConv, TargetID.GreerTheBlightbringerConv, TargetID.UraTheSteamshriekerConv])), log);
        var target = Targets.FirstOrDefault(x => x.IsAnySpecies([TargetID.DecimaTheStormsingerConv, TargetID.GreerTheBlightbringerConv, TargetID.UraTheSteamshriekerConv]));
        if (target == null)
        {
            return phases;
        }
        IReadOnlyList<Segment> hpUpdates = target.GetHealthUpdates(log);

        // Full Fight Phase
        string phaseName = "";
        string icon = "";
        switch (target.ID)
        {
            case (int)TargetID.DecimaTheStormsingerConv:
                phaseName = "Full Decima";
                icon = EncounterIconDecima;
                break;
            case (int)TargetID.GreerTheBlightbringerConv:
                phaseName = "Full Greer";
                icon = EncounterIconGreer;
                break;
            case (int)TargetID.UraTheSteamshriekerConv:
                phaseName = "Full Ura";
                icon = EncounterIconUra;
                break;
        }
        var fullPhase = new EncounterPhaseData(Math.Max(log.LogData.LogStart, target.FirstAware), Math.Min(target.LastAware, log.LogData.LogEnd), phaseName, log.LogData.Success, icon, log.LogData.Mode, log.LogData.Logic.LogID).WithParentPhase(phases[0]);
        fullPhase.AddTarget(target, log);
        phases.Add(fullPhase);
        if (!requirePhases)
        {
            return phases;
        }

        // Sub Phases
        Segment start = hpUpdates.FirstOrDefault(x => x.Value <= 100.0 && x.Value != 0 && x.Start != 0);
        Segment end75 = hpUpdates.FirstOrDefault(x => x.Value < 75.0 && x.Value != 0);
        Segment start75 = hpUpdates.FirstOrDefault(x => x.Value < 75.0 && x.Value != 0 && x.Start > end75.End);
        Segment end50 = hpUpdates.FirstOrDefault(x => x.Value < 50.0 && x.Value != 0);
        Segment start50 = hpUpdates.FirstOrDefault(x => x.Value < 50.0 && x.Value != 0 && x.Start > end50.End);
        Segment end25 = hpUpdates.FirstOrDefault(x => x.Value < 25.0 && x.Value != 0);
        Segment final = hpUpdates.FirstOrDefault(x => x.Value < 25.0 && x.Start > end25.End);

        // 100-75, Warclaw, 75-50, Warclaw, 50-25, Warclaw, 25-0
        var phase1 = new SubPhasePhaseData(start.Start, Math.Min(end75.Start, log.LogData.LogEnd), "Phase 1").WithParentPhase(fullPhase);
        var phase2 = new SubPhasePhaseData(start75.Start, Math.Min(end50.Start, log.LogData.LogEnd), "Phase 2").WithParentPhase(fullPhase);
        var phase3 = new SubPhasePhaseData(start50.Start, Math.Min(end25.Start, log.LogData.LogEnd), "Phase 3").WithParentPhase(fullPhase);
        var phase4 = new SubPhasePhaseData(final.Start, Math.Min(target.AgentItem.LastAware, log.LogData.LogEnd), "Phase 4").WithParentPhase(fullPhase);
        var warclaw1 = new SubPhasePhaseData(end75.Start, Math.Min(start75.Start, log.LogData.LogEnd), "Warclaw 1").WithParentPhase(fullPhase);
        var warclaw2 = new SubPhasePhaseData(end50.Start, Math.Min(start50.Start, log.LogData.LogEnd), "Warclaw 2").WithParentPhase(fullPhase);
        var warclaw3 = new SubPhasePhaseData(end25.Start, Math.Min(final.Start, log.LogData.LogEnd), "Warclaw 3").WithParentPhase(fullPhase);

        phase1.AddTarget(target, log);
        phase2.AddTarget(target, log);
        phase3.AddTarget(target, log);
        phase4.AddTarget(target, log);
        warclaw1.AddTarget(target, log);
        warclaw2.AddTarget(target, log);
        warclaw3.AddTarget(target, log);

        phases.AddRange([phase1, phase2, phase3, phase4, warclaw1, warclaw2, warclaw3]);

        return phases;
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);

        SingleActor? ura = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.UraTheSteamshriekerConv));
        ura?.OverrideName("Ura, the Steamshrieker");
    }
}
