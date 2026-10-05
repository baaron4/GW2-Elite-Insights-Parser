using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIGW2API;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal class SolitaryThroneInstance : SolitaryThrone
{
    private readonly EternalTyrant _eternalTyrant;

    public SolitaryThroneInstance(int triggerID) : base(triggerID)
    {
        LogID = LogIDs.LogMasks.Unsupported;
        Icon = InstanceIconSolitaryThrone;
        Extension = "throne";

        _eternalTyrant = new EternalTyrant((int)TargetID.EternalTyrant);

        MechanicList.Add(_eternalTyrant.Mechanics);
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData, GW2APIController apiController)
    {
        return "Solitary Throne Fractal";
    }

    internal override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log, CombatReplayDecorationContainer arenaDecorations, CombatReplayMap? parentMap = null)
    {
        var crMap = new CombatReplayMap((1053, 1053), (-9216, -9216, 12288, 12288));
        var parentCRMap = CombatReplayMap.CreateSquareMapFrom(crMap);
        arenaDecorations.Add(new ArenaDecoration((log.LogData.LogStart, log.LogData.LogEnd), CombatReplaySolitaryThrone, crMap));
        _eternalTyrant.GetCombatMapInternal(log, arenaDecorations, parentCRMap);
        return parentCRMap;
    }
    internal override void CheckSuccess(CombatData combatData, AgentData agentData, LogData logData, IReadOnlyCollection<AgentItem> playerAgents, LogData.LogSuccessHandler successHandler)
    {
        var lastEternalTyrant = agentData.GetStableSpeciesByID(TargetID.EternalTyrant).LastOrDefault(x => combatData.GetEnterCombatEvents(x).Any());
        if (lastEternalTyrant != null)
        {
            var (success, end) = EternalTyrant.CheckSuccess(lastEternalTyrant, combatData);
            if (success)
            {
                successHandler.SetSuccess(true, end);
            }
        }
    }

    private List<EncounterPhaseData> HandleEternalTyrantPhases(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, ParsedEvtcLog log, List<PhaseData> phases)
    {
        var encounterPhases = new List<EncounterPhaseData>();
        var mainPhase = phases[0];
        if (targetsByIDs.TryGetValue((int)TargetID.EternalTyrant, out var eternalTyrants))
        {
            foreach (var eternalTyrant in eternalTyrants)
            {
                var enterCombat = log.CombatData.GetEnterCombatEvents(eternalTyrant.AgentItem).FirstOrDefault();
                if (enterCombat != null)
                {
                    long start = enterCombat.Time;
                    var (success, end) = EternalTyrant.CheckSuccess(eternalTyrant.AgentItem, log.CombatData);
                    var name = "Eternal Tyrant";
                    var mode = EternalTyrant.GetLogModeForEternalTyrant(eternalTyrant, log.CombatData);
                    AddInstanceEncounterPhase(log, phases, encounterPhases, [eternalTyrant], [], [], mainPhase, name, start, end, success, _eternalTyrant, mode);
                }
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
            var eternalTyrantPhases = HandleEternalTyrantPhases(targetsByIDs, log, phases);
            foreach (var eternalTyrantPhase in eternalTyrantPhases)
            {
                var eternalTyrant = eternalTyrantPhase.Targets.Keys.First(x => x.IsSpecies(TargetID.EternalTyrant));
                phases.AddRange(EternalTyrant.ComputePhases(log, eternalTyrant, Targets, eternalTyrantPhase, requirePhases));
            }
        }
        return phases;
    }

    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        List<InstantCastFinder> finders = [
            .. _eternalTyrant.GetInstantCastFinders(),
        ];
        return finders;
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        List<TargetID> trashes = [
            .. _eternalTyrant.GetTrashMobsIDs(),
        ];
        return trashes.Distinct().ToList();
    }

    internal override IReadOnlyList<TargetID> GetTargetsIDs()
    {
        List<TargetID> targets = [
            .. _eternalTyrant.GetTargetsIDs(),
        ];
        return targets.Distinct().ToList();
    }

    internal override IReadOnlyList<TargetID> GetFriendlyNPCIDs()
    {
        List<TargetID> friendlies = [
            .. _eternalTyrant.GetFriendlyNPCIDs(),
        ];
        return friendlies.Distinct().ToList();
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);
    }

    internal override List<BuffEvent> SpecialBuffEventProcess(CombatData combatData, SkillData skillData)
    {
        return _eternalTyrant.SpecialBuffEventProcess(combatData, skillData);
    }

    internal override List<CastEvent> SpecialCastEventProcess(CombatData combatData, AgentData agentData, SkillData skillData, Dictionary<long, List<AnimatedCastEvent>> animatedCastDataByID)
    {
        return _eternalTyrant.SpecialCastEventProcess(combatData, agentData, skillData, animatedCastDataByID);
    }

    internal override List<HealthDamageEvent> SpecialDamageEventProcess(CombatData combatData, AgentData agentData, SkillData skillData)
    {
        return _eternalTyrant.SpecialDamageEventProcess(combatData, agentData, skillData);
    }
    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputeNPCCombatReplayActors(target, log, replay);
        _eternalTyrant.ComputeNPCCombatReplayActors(target, log, replay);
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputePlayerCombatReplayActors(p, log, replay);
        _eternalTyrant.ComputePlayerCombatReplayActors(p, log, replay);
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);
        _eternalTyrant.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);
    }

    internal override Dictionary<TargetID, int> GetTargetsSortIDs()
    {
        return _eternalTyrant.GetTargetsSortIDs();
    }

    internal override void SetInstanceBuffs(ParsedEvtcLog log, List<InstanceBuff> instanceBuffs)
    {
        base.SetInstanceBuffs(log, instanceBuffs);
        _eternalTyrant.SetInstanceBuffs(log, instanceBuffs);
    }

    internal override void ComputeAchievementEligibilityEvents(ParsedEvtcLog log, Player p, List<AchievementEligibilityEvent> achievementEligibilityEvents)
    {
        base.ComputeAchievementEligibilityEvents(log, p, achievementEligibilityEvents);
        _eternalTyrant.ComputeAchievementEligibilityEvents(log, p, achievementEligibilityEvents);
    }
}
