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

    internal override string GetLogicName(CombatData combatData, AgentData agentData)
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
            var determinedBuffs = combatData.GetBuffApplyDataByIDByDst(SkillIDs.Determined895, lastDarkAi);
            var determinedApply = determinedBuffs.FirstOrDefault(x => x is BuffApplyEvent bae && bae.AppliedDuration > AiKeeperOfThePeak.Determined895DurationCheckForSuccess);
            if (determinedApply != null)
            {
                logData.SetSuccess(true, determinedApply.Time);
            }
        }
    }

    private List<EncounterPhaseData> HandleAiPhases(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, ParsedEvtcLog log, List<PhaseData> phases, TargetID aiID)
    {
        var encounterPhases = new List<EncounterPhaseData>();
        var mainPhase = phases[0];
        if (targetsByIDs.TryGetValue((int)aiID, out var ais))
        {
            int offset = 0;
            bool dark = aiID == TargetID.DarkAiKeeperOfThePeak;
            foreach (var ai in ais)
            {
                offset++;
                long start = ai.FirstAware;
                var determinedBuffs = log.CombatData.GetBuffDataByIDByDst(SkillIDs.Determined895, ai.AgentItem);
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
            }
        }
        NumericallyRenameEncounterPhases(encounterPhases);
        return encounterPhases;
    }

    private List<EncounterPhaseData> HandleElementalAiPhases(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, ParsedEvtcLog log, List<PhaseData> phases)
    {
        return HandleAiPhases(targetsByIDs, log, phases, TargetID.AiKeeperOfThePeak);
    }

    private List<EncounterPhaseData> HandleDarkAiPhases(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, ParsedEvtcLog log, List<PhaseData> phases)
    {
        return HandleAiPhases(targetsByIDs, log, phases, TargetID.DarkAiKeeperOfThePeak);
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        var targetsByIDs = Targets.GroupBy(x => x.ID).ToDictionary(x => x.Key, x => x.ToList());
        {
            var elementalAiPhases = HandleElementalAiPhases(targetsByIDs, log, phases);
            foreach (var elementalAiPhase in elementalAiPhases)
            {
                var elementalAi = elementalAiPhase.Targets.Keys.First(x => x.IsSpecies(TargetID.AiKeeperOfThePeak));
                phases.AddRange(AiKeeperOfThePeak.ComputeElementalPhases(log, elementalAi, elementalAiPhase, requirePhases));
            }
        }
        {
            var china = log.CombatData.GetLanguageEvent()?.Language == LanguageEvent.LanguageEnum.Chinese;
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

    internal override List<CastEvent> SpecialCastEventProcess(CombatData combatData, SkillData skillData)
    {
        return _aiKeeperOfThePeak.SpecialCastEventProcess(combatData, skillData);
    }

    internal override List<HealthDamageEvent> SpecialDamageEventProcess(CombatData combatData, AgentData agentData, SkillData skillData)
    {
        return _aiKeeperOfThePeak.SpecialDamageEventProcess(combatData, agentData, skillData);
    }
    // TODO: handle duplicates due multiple base method calls in Combat Replay methods
    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        _aiKeeperOfThePeak.ComputeNPCCombatReplayActors(target, log, replay);
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        _aiKeeperOfThePeak.ComputePlayerCombatReplayActors(p, log, replay);
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        _aiKeeperOfThePeak.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);
    }

    internal override Dictionary<TargetID, int> GetTargetsSortIDs()
    {
        return _aiKeeperOfThePeak.GetTargetsSortIDs();
    }
}
