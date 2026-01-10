using System;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicTimeUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SpeciesIDs;
using static GW2EIEvtcParser.SkillIDs;
using GW2EIGW2API;

namespace GW2EIEvtcParser.LogLogic;
internal class SunquaPeakInstance : SunquaPeak
{
    private readonly AiKeeperOfThePeak _aiKeeperOfThePeak;

    public SunquaPeakInstance(int triggerID) : base(triggerID)
    {
        LogID = LogIDs.LogMasks.Unsupported;
        Icon = InstanceIconSilentSurf;
        Extension = "snqpk";

        _aiKeeperOfThePeak = new AiKeeperOfThePeak((int)TargetID.AiKeeperOfThePeak);

        MechanicList.Add(_aiKeeperOfThePeak.Mechanics);
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData, GW2APIController apiController)
    {
        return "Sunqua Peak Fractal";
    }

    internal override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log, CombatReplayDecorationContainer arenaDecorations)
    {
        var crMap = new CombatReplayMap((800, 800), (-12288, -12288, 12288, 12288));
        arenaDecorations.Add(new ArenaDecoration((log.LogData.LogStart, log.LogData.LogEnd), CombatReplaySunquaPeak, crMap));
        var subCrMap = _aiKeeperOfThePeak.GetCombatMapInternal(log, arenaDecorations);
        AddArenaDecorationsPerEncounter(log, arenaDecorations, _aiKeeperOfThePeak.LogID | AiKeeperOfThePeak.ElementalAiMask, CombatReplayAi, subCrMap);
        AddArenaDecorationsPerEncounter(log, arenaDecorations, _aiKeeperOfThePeak.LogID | AiKeeperOfThePeak.DarkAiMask, CombatReplayAi, subCrMap);
        return crMap;
    }
    internal override void CheckSuccess(CombatData combatData, AgentData agentData, LogData logData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        var lastDarkAi = agentData.GetNPCsByID(TargetID.DarkAiKeeperOfThePeak).LastOrDefault();
        if (lastDarkAi != null)
        {
            var determinedBuffs = combatData.GetBuffApplyDataByIDByDst(Determined895, lastDarkAi);
            var determinedApply = determinedBuffs.FirstOrDefault(x => x is BuffApplyEvent bae && bae.AppliedDuration > AiKeeperOfThePeak.Determined895DurationCheckForSuccess);
            if (determinedApply != null)
            {
                logData.SetSuccess(true, determinedApply.Time);
            }
        }
    }

    private int HandleSingleAiPhase(List<EncounterPhaseData> encounterPhases, PhaseData mainPhase, IReadOnlyList<SingleActor> ais, SingleActor ai, ParsedEvtcLog log, List<PhaseData> phases, TargetID aiID, int offset)
    {
        offset++;
        bool dark = aiID == TargetID.DarkAiKeeperOfThePeak;
        long start = ai.FirstAware;
        var determinedBuffs = log.CombatData.GetBuffDataByIDByDst(Determined895, ai.AgentItem);
        var determinedLost = determinedBuffs.FirstOrDefault(x => x is BuffRemoveAllEvent);
        var enterCombat = log.CombatData.GetEnterCombatEvents(ai.AgentItem).FirstOrDefault();
        if (determinedLost != null && enterCombat != null && enterCombat.Time >= determinedLost.Time)
        {
            start = determinedLost.Time;
        }
        else
        {
            var firstCast = ai.GetAnimatedCastEvents(log).FirstOrDefault();
            if (firstCast != null)
            {
                start = firstCast.Time;
            }
        }
        bool success = false;
        long end = ai.LastAware;
        var determinedApply = determinedBuffs.FirstOrDefault(x => x is BuffApplyEvent bae && bae.AppliedDuration > AiKeeperOfThePeak.Determined895DurationCheckForSuccess);
        if (determinedApply != null)
        {
            success = true;
            end = determinedApply.Time;
        }
        var encounterName = dark ? "Dark Ai, Keeper of the Peak" : "Elemental Ai, Keeper of the Peak";
        var encounterIcon = dark ? EncounterIconAiDark : EncounterIconAiElemental;
        var encounterID = dark ? (_aiKeeperOfThePeak.LogID | AiKeeperOfThePeak.DarkAiMask) : (_aiKeeperOfThePeak.LogID | AiKeeperOfThePeak.ElementalAiMask);
        var mode = LogData.LogMode.CMNoName;
        ai.OverrideName(encounterName + (ais.Count > 1 ? " " + offset : ""));
        AddInstanceEncounterPhase(log, phases, encounterPhases, [ai], [], [], mainPhase, encounterName, start, end, success, encounterIcon, encounterID, mode);
        return offset;
    }

    private int HandleFullAiPhase(List<EncounterPhaseData> encounterPhases, PhaseData mainPhase, IReadOnlyList<SingleActor> ais, SingleActor elementalAi, SingleActor darkAi, ParsedEvtcLog log, List<PhaseData> phases, int offset)
    {
        offset++;
        long start = elementalAi.FirstAware;
        var elementalDeterminedLost = log.CombatData.GetBuffDataByIDByDst(Determined895, elementalAi.AgentItem).FirstOrDefault(x => x is BuffRemoveAllEvent);
        var elementalEnterCombat = log.CombatData.GetEnterCombatEvents(elementalAi.AgentItem).FirstOrDefault();
        if (elementalDeterminedLost != null && elementalEnterCombat != null && elementalEnterCombat.Time >= elementalDeterminedLost.Time)
        {
            start = elementalDeterminedLost.Time;
        }
        else
        {
            var firstCast = elementalAi.GetAnimatedCastEvents(log).FirstOrDefault();
            if (firstCast != null)
            {
                start = firstCast.Time;
            }
        }
        bool success = false;
        long end = darkAi.LastAware;
        var darkDeterminedApply = log.CombatData.GetBuffDataByIDByDst(Determined895, darkAi.AgentItem).FirstOrDefault(x => x is BuffApplyEvent bae && bae.AppliedDuration > AiKeeperOfThePeak.Determined895DurationCheckForSuccess);
        if (darkDeterminedApply != null)
        {
            success = true;
            end = darkDeterminedApply.Time;
        }
        var encounterName = "Ai, Keeper of the Peak";
        var encounterIcon = EncounterIconAi;
        var encounterID = (_aiKeeperOfThePeak.LogID | AiKeeperOfThePeak.FullAiMask);
        var mode = LogData.LogMode.CMNoName;
        elementalAi.OverrideName("Elemental Ai, Keeper of the Peak" + (ais.Count > 1 ? " " + offset : "") + " Full");
        darkAi.OverrideName("Dark Ai, Keeper of the Peak" + (ais.Count > 1 ? " " + offset : "") + " Full");
        AddInstanceEncounterPhase(log, phases, encounterPhases, [elementalAi, darkAi], [], [], mainPhase, encounterName, start, end, success, encounterIcon, encounterID, mode);
        return offset;
    }

    private List<EncounterPhaseData> HandleSingleAiPhases(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, ParsedEvtcLog log, List<PhaseData> phases, TargetID aiID)
    {
        var singlePhases = new List<EncounterPhaseData>();
        var mainPhase = phases[0];
        if (targetsByIDs.TryGetValue((int)aiID, out var ais))
        {
            var nonEnglobedAis = ais.Where(x => !x.AgentItem.IsEnglobedAgent).ToList();
            bool dark = aiID == TargetID.DarkAiKeeperOfThePeak;
            int offset = 0;
            foreach (var ai in nonEnglobedAis)
            {
                offset = HandleSingleAiPhase(singlePhases, mainPhase, nonEnglobedAis, ai, log, phases, aiID, offset);       
            }
        }
        NumericallyRenameEncounterPhases(singlePhases);
        return singlePhases;
    }


    private List<EncounterPhaseData> HandleFullAiPhases(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, ParsedEvtcLog log, List<PhaseData> phases)
    {
        var fullPhases = new List<EncounterPhaseData>();
        var mainPhase = phases[0];
        if (targetsByIDs.TryGetValue((int)TargetID.AiKeeperOfThePeak, out var ais))
        {
            int fullOffset = 0;
            var englobedElAis = ais.Where(x => x.AgentItem.IsEnglobedAgent).ToList();
            if (englobedElAis.Count > 0 && targetsByIDs.TryGetValue((int)TargetID.DarkAiKeeperOfThePeak, out var darkAis))
            {
                var englobedDarkAis = darkAis.Where(x => x.AgentItem.IsEnglobedAgent);
                foreach (var elementalAi in englobedElAis)
                {
                    var darkAi = englobedDarkAis.FirstOrDefault(x => x.AgentItem.Is(elementalAi.AgentItem));
                    if (darkAi != null)
                    {
                        fullOffset = HandleFullAiPhase(fullPhases, mainPhase, englobedElAis, elementalAi, darkAi, log, phases, fullOffset);
                    }
                }
            }
        }
        NumericallyRenameEncounterPhases(fullPhases);
        return fullPhases;
    }
    private List<EncounterPhaseData> HandleElementalAiPhases(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, ParsedEvtcLog log, List<PhaseData> phases)
    {
        return HandleSingleAiPhases(targetsByIDs, log, phases, TargetID.AiKeeperOfThePeak);
    }

    private List<EncounterPhaseData> HandleDarkAiPhases(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, ParsedEvtcLog log, List<PhaseData> phases)
    {
        return HandleSingleAiPhases(targetsByIDs, log, phases, TargetID.DarkAiKeeperOfThePeak);
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        var china = log.CombatData.GetLanguageEvent()?.Language == LanguageEvent.LanguageEnum.Chinese;
        var targetsByIDs = Targets.GroupBy(x => x.ID).ToDictionary(x => x.Key, x => x.ToList());
        {
            var fullAiPhases = HandleFullAiPhases(targetsByIDs, log, phases);
            foreach (var fullAiPhase in fullAiPhases)
            {
                {
                    var darkAi = fullAiPhase.Targets.Keys.First(x => x.IsSpecies(TargetID.DarkAiKeeperOfThePeak));
                    var darkAiPhase = AiKeeperOfThePeak.GetFightPhase(log, darkAi, fullAiPhase, "Dark Phase");
                    phases.Add(darkAiPhase);
                    phases.AddRange(AiKeeperOfThePeak.ComputeDarkPhases(log, darkAi, darkAiPhase, china, requirePhases));
                }
                {
                    var elementalAi = fullAiPhase.Targets.Keys.First(x => x.IsSpecies(TargetID.AiKeeperOfThePeak));
                    var elementalAiPhase = AiKeeperOfThePeak.GetFightPhase(log, elementalAi, fullAiPhase, "Elemental Phase");
                    phases.Add(elementalAiPhase);
                    phases.AddRange(AiKeeperOfThePeak.ComputeElementalPhases(log, elementalAi, elementalAiPhase, requirePhases));
                }
            }
        }
        {
            var elementalAiPhases = HandleElementalAiPhases(targetsByIDs, log, phases);
            foreach (var elementalAiPhase in elementalAiPhases)
            {
                var elementalAi = elementalAiPhase.Targets.Keys.First(x => x.IsSpecies(TargetID.AiKeeperOfThePeak));
                phases.AddRange(AiKeeperOfThePeak.ComputeElementalPhases(log, elementalAi, elementalAiPhase, requirePhases));

            }
        }
        {
            var darkAiPhases = HandleDarkAiPhases(targetsByIDs, log, phases);
            foreach (var darkAiPhase in darkAiPhases)
            {
                var darkAi = darkAiPhase.Targets.Keys.First(x => x.IsSpecies(TargetID.DarkAiKeeperOfThePeak));
                phases.AddRange(AiKeeperOfThePeak.ComputeDarkPhases(log, darkAi, darkAiPhase, china, requirePhases));
            }
        }
        return phases;
    }

    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        List<InstantCastFinder> finders = [
            .. _aiKeeperOfThePeak.GetInstantCastFinders(),
        ];
        return finders;
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        List<TargetID> trashes = [
            .. _aiKeeperOfThePeak.GetTrashMobsIDs(),
        ];
        return trashes.Distinct().ToList();
    }

    internal override IReadOnlyList<TargetID> GetTargetsIDs()
    {
        List<TargetID> targets = [
            .. _aiKeeperOfThePeak.GetTargetsIDs(),
        ];
        return targets.Distinct().ToList();
    }

    internal override IReadOnlyList<TargetID> GetFriendlyNPCIDs()
    {
        List<TargetID> friendlies = [
            .. _aiKeeperOfThePeak.GetFriendlyNPCIDs(),
        ];
        return friendlies.Distinct().ToList();
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        AiKeeperOfThePeak.DetectAis(agentData, combatData, extensions);
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);
    }

    internal override List<BuffEvent> SpecialBuffEventProcess(CombatData combatData, SkillData skillData)
    {
        return _aiKeeperOfThePeak.SpecialBuffEventProcess(combatData, skillData);
    }

    internal override List<CastEvent> SpecialCastEventProcess(CombatData combatData, AgentData agentData, SkillData skillData)
    {
        return _aiKeeperOfThePeak.SpecialCastEventProcess(combatData, agentData, skillData);
    }

    internal override List<HealthDamageEvent> SpecialDamageEventProcess(CombatData combatData, AgentData agentData, SkillData skillData)
    {
        return _aiKeeperOfThePeak.SpecialDamageEventProcess(combatData, agentData, skillData);
    }
    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputeNPCCombatReplayActors(target, log, replay);
        _aiKeeperOfThePeak.ComputeNPCCombatReplayActors(target, log, replay);
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputePlayerCombatReplayActors(p, log, replay);
        _aiKeeperOfThePeak.ComputePlayerCombatReplayActors(p, log, replay);
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);
        _aiKeeperOfThePeak.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);
    }

    internal override void SetInstanceBuffs(ParsedEvtcLog log, List<InstanceBuff> instanceBuffs)
    {
        base.SetInstanceBuffs(log, instanceBuffs);
        _aiKeeperOfThePeak.SetInstanceBuffs(log, instanceBuffs);
    }
    internal override void ComputeAchievementEligibilityEvents(ParsedEvtcLog log, Player p, List<AchievementEligibilityEvent> achievementEligibilityEvents)
    {
        base.ComputeAchievementEligibilityEvents(log, p, achievementEligibilityEvents);
        _aiKeeperOfThePeak.ComputeAchievementEligibilityEvents(log, p, achievementEligibilityEvents);
    }

    internal override Dictionary<TargetID, int> GetTargetsSortIDs()
    {
        return _aiKeeperOfThePeak.GetTargetsSortIDs();
    }
}
