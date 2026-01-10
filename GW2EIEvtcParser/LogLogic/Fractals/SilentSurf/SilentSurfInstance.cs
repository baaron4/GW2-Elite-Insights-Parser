using System;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIGW2API;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicTimeUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal class SilentSurfInstance : SilentSurf
{
    private readonly Kanaxai _kanaxai;

    public SilentSurfInstance(int triggerID) : base(triggerID)
    {
        LogID = LogIDs.LogMasks.Unsupported;
        Icon = InstanceIconSilentSurf;
        Extension = "slntsrf";

        _kanaxai = new Kanaxai((int)TargetID.KanaxaiScytheOfHouseAurkusCM);

        MechanicList.Add(_kanaxai.Mechanics);
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData, GW2APIController apiController)
    {
        return "Silent Surf Fractal";
    }

    internal override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log, CombatReplayDecorationContainer arenaDecorations)
    {
        var crMap = new CombatReplayMap((800, 960), (-15360, -18432, 15360, 18432));
        arenaDecorations.Add(new ArenaDecoration((log.LogData.LogStart, log.LogData.LogEnd), CombatReplaySilentSurf, crMap));
        _kanaxai.GetCombatMapInternal(log, arenaDecorations);
        return crMap;
    }
    internal override void CheckSuccess(CombatData combatData, AgentData agentData, LogData logData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        var lastKanaxai = agentData.GetNPCsByID(TargetID.KanaxaiScytheOfHouseAurkusCM).LastOrDefault(x => combatData.GetEnterCombatEvents(x).Any());
        if (lastKanaxai != null)
        {
            var determinedBuffs = combatData.GetBuffDataByIDByDst(SkillIDs.Determined762, lastKanaxai);
            var enterCombat = combatData.GetEnterCombatEvents(lastKanaxai).First();
            var determinedApply = determinedBuffs.FirstOrDefault(x => x is BuffApplyEvent && x.Time > enterCombat.Time);
            if (determinedApply != null && !combatData.GetDespawnEvents(lastKanaxai).Any(x => Math.Abs(x.Time - determinedApply.Time) < ServerDelayConstant))
            {
                logData.SetSuccess(true, determinedApply.Time);
            }
        }
    }

    private List<EncounterPhaseData> HandleKanaxaiPhases(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, ParsedEvtcLog log, List<PhaseData> phases)
    {
        var encounterPhases = new List<EncounterPhaseData>();
        var mainPhase = phases[0];
        if (targetsByIDs.TryGetValue((int)TargetID.KanaxaiScytheOfHouseAurkusCM, out var kanaxais))
        {
            foreach (var kanaxai in kanaxais)
            {
                long start = kanaxai.FirstAware;
                var determinedBuffs = log.CombatData.GetBuffDataByIDByDst(SkillIDs.Determined762, kanaxai.AgentItem);
                var determinedLost = determinedBuffs.FirstOrDefault(x => x is BuffRemoveAllEvent);
                var enterCombat = log.CombatData.GetEnterCombatEvents(kanaxai.AgentItem).FirstOrDefault();
                if (determinedLost != null && enterCombat != null && enterCombat.Time >= determinedLost.Time)
                {
                    start = determinedLost.Time;
                } 
                else
                {
                    continue;
                }
                bool success = false;
                long end = kanaxai.LastAware;
                var determinedApply = determinedBuffs.FirstOrDefault(x => x is BuffApplyEvent && x.Time > start);
                if (determinedApply != null && !log.CombatData.GetDespawnEvents(kanaxai.AgentItem).Any(x => Math.Abs(x.Time - determinedApply.Time) < ServerDelayConstant))
                {
                    success = true;
                    end = determinedApply.Time;
                }
                var name = "Kanaxai";
                var mode = LogData.LogMode.CMNoName;
                AddInstanceEncounterPhase(log, phases, encounterPhases, [kanaxai], [], [], mainPhase, name, start, end, success, _kanaxai, mode);
            }
        }
        NumericallyRenameEncounterPhases(encounterPhases);
        return encounterPhases;
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        var targetsByIDs = Targets.GroupBy(x => x.ID).ToDictionary(x => x.Key, x => x.ToList());
        {
            var kanaxaiPhases = HandleKanaxaiPhases(targetsByIDs, log, phases);
            foreach (var kanaxaiPhase in kanaxaiPhases)
            {
                var kanaxai = kanaxaiPhase.Targets.Keys.First(x => x.IsAnySpecies([TargetID.KanaxaiScytheOfHouseAurkusNM, TargetID.KanaxaiScytheOfHouseAurkusCM]));
                phases.AddRange(Kanaxai.ComputePhases(log, kanaxai, Targets, kanaxaiPhase, requirePhases));
            }
        }
        return phases;
    }

    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        List<InstantCastFinder> finders = [
            .. _kanaxai.GetInstantCastFinders(),
        ];
        return finders;
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        List<TargetID> trashes = [
            .. _kanaxai.GetTrashMobsIDs(),
        ];
        return trashes.Distinct().ToList();
    }

    internal override IReadOnlyList<TargetID> GetTargetsIDs()
    {
        List<TargetID> targets = [
            .. _kanaxai.GetTargetsIDs(),
        ];
        return targets.Distinct().ToList();
    }

    internal override IReadOnlyList<TargetID> GetFriendlyNPCIDs()
    {
        List<TargetID> friendlies = [
            .. _kanaxai.GetFriendlyNPCIDs(),
        ];
        return friendlies.Distinct().ToList();
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);
    }

    internal override List<BuffEvent> SpecialBuffEventProcess(CombatData combatData, SkillData skillData)
    {
        return _kanaxai.SpecialBuffEventProcess(combatData, skillData);
    }

    internal override List<CastEvent> SpecialCastEventProcess(CombatData combatData, AgentData agentData, SkillData skillData)
    {
        return _kanaxai.SpecialCastEventProcess(combatData, agentData, skillData);
    }

    internal override List<HealthDamageEvent> SpecialDamageEventProcess(CombatData combatData, AgentData agentData, SkillData skillData)
    {
        return _kanaxai.SpecialDamageEventProcess(combatData, agentData, skillData);
    }
    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputeNPCCombatReplayActors (target, log, replay);
        _kanaxai.ComputeNPCCombatReplayActors(target, log, replay);
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputePlayerCombatReplayActors (p, log, replay);
        _kanaxai.ComputePlayerCombatReplayActors(p, log, replay);
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        base.ComputeEnvironmentCombatReplayDecorations (log, environmentDecorations);
        _kanaxai.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);
    }

    internal override void SetInstanceBuffs(ParsedEvtcLog log, List<InstanceBuff> instanceBuffs)
    {
        base.SetInstanceBuffs(log, instanceBuffs);
        _kanaxai.SetInstanceBuffs(log, instanceBuffs);
    }
    internal override void ComputeAchievementEligibilityEvents(ParsedEvtcLog log, Player p, List<AchievementEligibilityEvent> achievementEligibilityEvents)
    {
        base.ComputeAchievementEligibilityEvents(log, p, achievementEligibilityEvents);
        _kanaxai.ComputeAchievementEligibilityEvents(log, p, achievementEligibilityEvents);

    internal override Dictionary<TargetID, int> GetTargetsSortIDs()
    {
        return _kanaxai.GetTargetsSortIDs();
    }
}
