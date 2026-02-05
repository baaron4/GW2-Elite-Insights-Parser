using System.Diagnostics;
using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using GW2EIGW2API;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicTimeUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;
using static GW2EIEvtcParser.AchievementEligibilityIDs;

namespace GW2EIEvtcParser.LogLogic;

internal class GuardiansGlade : VisionsOfEternityRaidEncounter
{
    internal readonly MechanicGroup Mechanics = new
    ([
        new PlayerDstBuffApplyMechanic(FixatedKela, new MechanicPlotlySetting(Symbols.Star,Colors.Magenta), "Fixated", "Fixated by Kela","Fixated", 0),
    ]);
    
    public GuardiansGlade(int triggerID) : base(triggerID)
    {
        MechanicList.Add(Mechanics);
        Icon = EncounterIconGuardiansGlade;
        Extension = "guardglade";
        GenericFallBackMethod = FallBackMethod.None;
        LogCategoryInformation.InSubCategoryOrder = 0;
        LogID |= 0x000001;
    }
    
    internal override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log, CombatReplayDecorationContainer arenaDecorations)
    {
        var crMap = new CombatReplayMap(
            (800, 800),
            (0, 0, 0, 0));
        AddArenaDecorationsPerEncounter(log, arenaDecorations, LogID, CombatReplayGuardiansGlade, crMap);
        return crMap;
    }
    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        return
        [
            new DamageCastFinder(KelaAura, KelaAura),
        ];
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData, GW2APIController apiController)
    {
        return "Guardian's Glade";
    }

    internal override IReadOnlyList<TargetID>  GetTargetsIDs()
    {
        return
        [
            TargetID.KelaSeneschalOfWaves,
        ];
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        return
        [
            TargetID.DownedEliteCrocodilianRazortooth,
            TargetID.EliteCrocodilianRazortooth,
            TargetID.VeteranCrocodilianRazortooth,
            TargetID.ExecutorOfWaves,
            TargetID.CursedArtefact_NPC,
        ];
    }

    internal override long GetLogOffset(EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData)
    {
        long startToUse = GetGenericLogOffset(logData);
        CombatItem? logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.LogNPCUpdate);
        if (logStartNPCUpdate != null)
        {
            long enterCombatTime = GetEnterCombatTime(logData, agentData, combatData, logStartNPCUpdate.Time, GenericTriggerID, logStartNPCUpdate.DstAgent);
            var firstFixation = combatData.FirstOrDefault(x => x.IsBuffApply() && x.SkillID == FixatedKela);
            // If fixation is missing or first seen fixation is after boss enters combat, fallback to enterCombatTime, log will be seen as late start
            if (firstFixation == null || firstFixation.Time > enterCombatTime)
            {
                return enterCombatTime;
            }
            return firstFixation.Time;
        }
        return startToUse;
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        FindChestGadgets([
            (ChestID.GrandRaidKelaChest, GrandRaidChestKelaPosition, (agentItem) => agentItem.HitboxHeight == 0 || (agentItem.HitboxHeight == 1200 && agentItem.HitboxWidth == 100)),
        ], agentData, combatData);
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);
        /*var chest = agentData.GetGadgetsByID(ChestID.GrandRaidKelaChest).FirstOrDefault();
        if (chest != null)
        {
            var kela = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.KelaSeneschalOfWaves)) ?? throw new MissingKeyActorsException("Kela not found");
            // Chest can be used for success if it appears after kela
            if (kela.FirstAware < chest.FirstAware + 5000)
            {
                ChestID = ChestID.GrandRaidKelaChest;
            }
        }*/
    }

    internal static IReadOnlyList<SubPhasePhaseData> ComputePhases(ParsedEvtcLog log, SingleActor kela, EncounterPhaseData encounterPhase, bool requirePhases)
    {
        if (!requirePhases)
        {
            return [];
        }
        var phases = new List<SubPhasePhaseData>(10);
        var kelaCasts = kela.GetAnimatedCastEvents(log, encounterPhase.Start, encounterPhase.End);
        var burrowCast = kelaCasts.Where(x => x.SkillID == KelaBurrow).ToList();
        var kelaNonBurrowRelatedCast = kelaCasts.Where(x => x.SkillID != KelaBurrow && x.SkillID != KelaAmbush1 && x.SkillID != KelaAmbush2 && x.SkillID != log.SkillData.DodgeID && x.ActualDuration > ServerDelayConstant).ToList();
        // Candidate phases
        var kelaPhases = GetSubPhasesByInvul(log, KelaBurrowed, kela, true, true, encounterPhase.Start, encounterPhase.End, false);
        List<SubPhasePhaseData> candidateMainPhases = [];
        List<SubPhasePhaseData> candidateStormPhases = [];
        for (int i = 0; i < kelaPhases.Count; i++)
        {
            SubPhasePhaseData subPhase = kelaPhases[i];
            subPhase.AddParentPhase(encounterPhase);
            if ((i % 2) == 0)
            {
                candidateMainPhases.Add(subPhase);
            }
            else
            {
                candidateStormPhases.Add(subPhase);
            }
            subPhase.AddTarget(kela, log);
        }
        // Split phases
        var stormPhaseCount = 1;
        List<SubPhasePhaseData> stormPhases = new(10);
        SubPhasePhaseData? curStormPhase = null;
        foreach (var candidateStormPhase in candidateStormPhases)
        {
            if (curStormPhase == null)
            {
                curStormPhase = candidateStormPhase;
                curStormPhase.Name = "Storm Phase " + (stormPhaseCount++);
                phases.Add(curStormPhase);
                stormPhases.Add(curStormPhase);
            } 
            else
            {
                var nextBurrowStart = candidateStormPhase.Start;
                var previousBurrowEnd = curStormPhase.End;
                var burrowCastQuickly = burrowCast.Any(x => x.Time <= previousBurrowEnd + 5000 && x.Time >= previousBurrowEnd);
                var nonBurrowCast = kelaNonBurrowRelatedCast.Any(x => x.Time >= previousBurrowEnd - ServerDelayConstant && x.Time <= nextBurrowStart + ServerDelayConstant);
                if (!nonBurrowCast && burrowCastQuickly)
                {
                    curStormPhase.OverrideEnd(candidateStormPhase.End);
                } 
                else
                {
                    curStormPhase = candidateStormPhase;
                    phases.Add(curStormPhase);
                    curStormPhase.Name = "Storm Phase " + (stormPhaseCount++);
                    stormPhases.Add(curStormPhase);
                }
            }
        }
        // Main phases
        var mainPhaseCount = 1;
        foreach (var candidateMainPhase in candidateMainPhases)
        {
            if (!stormPhases.Any(x => x.InInterval((candidateMainPhase.Start + candidateMainPhase.End) / 2)))
            {
                candidateMainPhase.Name = "Phase " + (mainPhaseCount++);
                phases.Add(candidateMainPhase);
            }
        }
        return phases;
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        var kela = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.KelaSeneschalOfWaves)) ?? throw new MissingKeyActorsException("Kela not found");
        var phases = GetInitialPhase(log);
        var fullFightPhase = (EncounterPhaseData)phases[0];
        fullFightPhase.AddTarget(kela, log);
        phases.AddRange(ComputePhases(log, kela, fullFightPhase, requirePhases));
        return phases;
    }

    internal override LogData.Mode GetLogMode(CombatData combatData, AgentData agentData, LogData logData)
    {
        return LogData.Mode.Normal;
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        // Fixated
        var kelaPhases = log.LogData.GetEncounterPhases(log).Where(x => x.ID == LogID).ToList();
        var fixateds = p.GetBuffStatus(log, FixatedKela).Where(x => x.Value > 0);
        foreach (Segment seg in fixateds)
        {
            var kela = kelaPhases.FirstOrDefault(x => x.IntersectsWindow(seg.Start, seg.End))?.Targets.First(x => x.Key.IsSpecies(TargetID.KelaSeneschalOfWaves)).Key;
            if (kela != null)
            {
                replay.Decorations.AddTether(seg.Start, seg.End, p.AgentItem, kela.AgentItem, Colors.FixationPurple.WithAlpha(0.3).ToString());
            }
            replay.Decorations.AddOverheadIcon(seg, p, ParserIcons.FixationPurpleOverhead);
        }
    }

    internal override LogData.StartStatus GetLogStartStatus(CombatData combatData, AgentData agentData, LogData logData)
    {
        var genericStatus = base.GetLogStartStatus(combatData, agentData, logData);
        if (genericStatus != LogData.StartStatus.Normal)
        {
            return genericStatus;
        }
        var firstFixation = combatData.GetBuffApplyData(FixatedKela).FirstOrDefault();
        if (firstFixation != null)
        {
            return firstFixation.Time <= logData.LogStart ? LogData.StartStatus.Normal : LogData.StartStatus.Late;
        }
        return LogData.StartStatus.Late;
    }

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, LogData logData, IReadOnlyCollection<AgentItem> playerAgents, LogData.LogSuccessHandler successHandler)
    {
        base.CheckSuccess(combatData, agentData, logData, playerAgents, successHandler);
        if (!successHandler.Success)
        {
            var kela = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.KelaSeneschalOfWaves)) ?? throw new MissingKeyActorsException("Kela not found");
            var determined762Applies = combatData.GetBuffApplyDataByIDByDst(Determined762, kela.AgentItem);
            if (determined762Applies.Count == 1)
            {
                successHandler.SetSuccess(true, determined762Applies[0].Time);
            }
        }
    }
}
