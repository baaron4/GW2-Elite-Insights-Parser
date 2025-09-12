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

internal class KinfallInstance : Kinfall
{
    private readonly WhisperingShadow _whisperingShadow;

    public KinfallInstance(int triggerID) : base(triggerID)
    {
        LogID = LogIDs.LogMasks.Unsupported;
        Icon = InstanceIconKinfall;
        Extension = "knfll";

        _whisperingShadow = new WhisperingShadow((int)TargetID.WhisperingShadow);

        MechanicList.Add(_whisperingShadow.Mechanics);
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData)
    {
        return "Kinfall Fractal";
    }

    internal override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log, CombatReplayDecorationContainer arenaDecorations)
    {
        var crMap = new CombatReplayMap((800, 800), (-18432, -18432, 21504, 21504));
        arenaDecorations.Add(new ArenaDecoration((log.LogData.LogStart, log.LogData.LogEnd), CombatReplayKinfall, crMap));
        _whisperingShadow.GetCombatMapInternal(log, arenaDecorations);
        return crMap;
    }
    internal override void CheckSuccess(CombatData combatData, AgentData agentData, LogData logData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        var lastWhisperingShadow = agentData.GetNPCsByID(TargetID.WhisperingShadow).LastOrDefault();
        if (lastWhisperingShadow != null)
        {
            var death = combatData.GetDeadEvents(lastWhisperingShadow).FirstOrDefault();
            if (death != null)
            {
                logData.SetSuccess(true, death.Time);
            }
        }
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        var targetsByIDs = Targets.GroupBy(x => x.ID).ToDictionary(x => x.Key, x => x.ToList());
        {
            var whisperingShadowPhases = ProcessGenericEncounterPhasesForInstance(targetsByIDs, log, phases, TargetID.WhisperingShadow, [], "Whispering Shadow", _whisperingShadow, (log, whisperingShadow) =>
            {
                return log.CombatData.GetBuffApplyData(SkillIDs.LifeFireCircleCM).Any(x => whisperingShadow.InAwareTimes(x.Time)) ? LogData.LogMode.CM : LogData.LogMode.Normal;
            });
            foreach (var whisperingShadowPhase in whisperingShadowPhases)
            {
                var whisperingShadow = whisperingShadowPhase.Targets.Keys.First(x => x.IsSpecies(TargetID.WhisperingShadow));
                phases.AddRange(WhisperingShadow.ComputePhases(log, whisperingShadow, whisperingShadowPhase, requirePhases));
            }
        }
        return phases;
    }

    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        List<InstantCastFinder> finders = [
            .. _whisperingShadow.GetInstantCastFinders(),
        ];
        return finders;
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        List<TargetID> trashes = [
            .. _whisperingShadow.GetTrashMobsIDs(),
        ];
        return trashes.Distinct().ToList();
    }

    internal override IReadOnlyList<TargetID> GetTargetsIDs()
    {
        List<TargetID> targets = [
            .. _whisperingShadow.GetTargetsIDs(),
        ];
        return targets.Distinct().ToList();
    }

    internal override IReadOnlyList<TargetID> GetFriendlyNPCIDs()
    {
        List<TargetID> friendlies = [
            .. _whisperingShadow.GetFriendlyNPCIDs(),
        ];
        return friendlies.Distinct().ToList();
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);
    }

    internal override List<BuffEvent> SpecialBuffEventProcess(CombatData combatData, SkillData skillData)
    {
        return _whisperingShadow.SpecialBuffEventProcess(combatData, skillData);
    }

    internal override List<CastEvent> SpecialCastEventProcess(CombatData combatData, SkillData skillData)
    {
        return _whisperingShadow.SpecialCastEventProcess(combatData, skillData);
    }

    internal override List<HealthDamageEvent> SpecialDamageEventProcess(CombatData combatData, AgentData agentData, SkillData skillData)
    {
        return _whisperingShadow.SpecialDamageEventProcess(combatData, agentData, skillData);
    }
    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputeNPCCombatReplayActors(target, log, replay);
        _whisperingShadow.ComputeNPCCombatReplayActors(target, log, replay);
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputePlayerCombatReplayActors(p, log, replay);
        _whisperingShadow.ComputePlayerCombatReplayActors(p, log, replay);
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);
        _whisperingShadow.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);
    }

    internal override Dictionary<TargetID, int> GetTargetsSortIDs()
    {
        return _whisperingShadow.GetTargetsSortIDs();
    }
}
