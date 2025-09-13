using System.Collections.Generic;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.LogLogic.LogCategories;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicTimeUtils;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.MapIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal class UnknownInstanceLogic : UnknownEncounterLogic
{
    private readonly List<TargetID> _targetIDs = [];
    private readonly List<TargetID> _trashIDs = [];
    public UnknownInstanceLogic(int id) : base(id)
    {
        Extension = "instance";
        SkillMode = SkillModeEnum.PvE;
        Icon = InstanceIconGeneric;
    }

    private void FindGenericTargetIDs(AgentData agentData, IReadOnlyList<CombatItem> combatData)
    {
        var allTargetIDs = Enum.GetValues(typeof(TargetID));
        var maxHPUpdates = combatData.Where(x => x.IsStateChange == StateChange.MaxHealthUpdate && agentData.GetAgent(x.SrcAgent, x.Time).Type == AgentItem.AgentType.NPC && MaxHealthUpdateEvent.GetMaxHealth(x) > 0).GroupBy(x => agentData.GetAgent(x.SrcAgent, x.Time).ID).ToDictionary(x => x.Key, x => x.ToList());
        var blackList = new HashSet<TargetID>()
        {
            TargetID.Environment,
            TargetID.Artsariiv,
            TargetID.Deimos,
            TargetID.ConjuredAmalgamate,
            TargetID.CALeftArm_CHINA,
            TargetID.CARightArm_CHINA,
            TargetID.ConjuredAmalgamate_CHINA,
        };
        foreach (TargetID targetID in allTargetIDs)
        {
            //TODO(Rennorb) @perf: invert this iteration?  make the agentData the outer loop and then just test the enum for isDefined?
            if (agentData.GetNPCsByID(targetID).Any())
            {
                if (blackList.Contains(targetID) || !maxHPUpdates.TryGetValue((int)targetID, out var maxHPs) || !maxHPs.Any(x => MaxHealthUpdateEvent.GetMaxHealth(x) > 5e5))
                {
                    continue;
                }
                if (maxHPs.Any(x => MaxHealthUpdateEvent.GetMaxHealth(x) > 1e6))
                {
                    _targetIDs.Add(targetID);
                } else
                {
                    _trashIDs.Add(targetID);
                }
            }
        }
    }

    internal override LogLogic AdjustLogic(AgentData agentData, List<CombatItem> combatData, EvtcParserSettings parserSettings)
    {
        CombatItem? mapIDEvent = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.MapID);
        // Handle potentially wrongly associated logs, we don't officially support pre-effect logs
        if (mapIDEvent != null && combatData.Any(x => x.IsEffect))
        {
            switch (MapIDEvent.GetMapID(mapIDEvent))
            {
                // Fractals
                case NightmareFractal:
                    return new NightmareInstance(GenericTriggerID);
                case ShatteredObservatoryFractal:
                    return new ShatteredObservatoryInstance(GenericTriggerID);
                case SunquaPeakFractal:
                    return new SunquaPeakInstance(GenericTriggerID);
                case CaptainMaiTrinBossFractal:
                    return new CaptainMaiTrinBossInstance(GenericTriggerID);
                case DeepstoneFractal:
                    return new DeepstoneInstance(GenericTriggerID);
                case SilentSurfFractal:
                    return new SilentSurfInstance(GenericTriggerID);
                case KinfallFractal:
                    return new KinfallInstance(GenericTriggerID);
                case LonelyTowerFractal:
                    return new LonelyTowerInstance(GenericTriggerID);
                // Raids
                case SpiritValeRaid:
                    return new SpiritValeInstance(GenericTriggerID);
                case SalvationPassRaid:
                    return new SalvationPassInstance(GenericTriggerID);
                case StrongholdOfTheFaithfulRaid:
                    return new StrongholdOfTheFaithfulInstance(GenericTriggerID);
                case BastionOfThePenitentRaid:
                    return new BastionOfThePenitentInstance(GenericTriggerID);
                case HallOfChainsRaid:
                    return new HallOfChainsInstance(GenericTriggerID);
                case MythwrightGambitRaid:
                    return new MythwrightGambitInstance(GenericTriggerID);
                case TheKeyOfAhdashimRaid:
                    return new TheKeyOfAhdashimInstance(GenericTriggerID);
                case MountBalriorRaid:
                    return new MountBalriorInstance(GenericTriggerID);
                // WvW
                case EternalBattleground:
                case GreenAlpineBorderland:
                case BlueAlpineBorderland:
                case RedDesertBorderland:
                case EdgeOfTheMists:
                case ArmisticeBastion:
                    return new WvWLogic(GenericTriggerID, parserSettings.DetailedWvWParse, true);
                // Convergences
                case OuterNayosPublicConvergence:
                case OuterNayosPrivateConvergence:
                    return new OuterNayosConvergenceInstance(GenericTriggerID);
                case MountBalriorPublicConvergence:
                case MountBalriorPrivateConvergence:
                    return new MountBalriorConvergenceInstance(GenericTriggerID);
            }
        }
        return base.AdjustLogic(agentData, combatData, parserSettings);
    }

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, LogData logData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        logData.SetSuccess(true, logData.LogEnd);
    }
    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases;
        if (_targetIDs.Count == 0)
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
        AddPhasesPerTarget(log, phases, Targets.Where(x => x.GetHealth(log.CombatData) > 3e6 && x.LastAware - x.FirstAware > ParserHelper.MinimumInCombatDuration));
        return phases;
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        FindGenericTargetIDs(agentData, combatData);
        Targetless = _targetIDs.Count == 0;
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);
    }
    internal override IReadOnlyList<TargetID> GetTargetsIDs()
    {
        return _targetIDs.Count > 0 ? _targetIDs : [TargetID.Instance];
    }
    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        return _trashIDs;
    }

    internal override LogData.InstancePrivacyMode GetInstancePrivacyMode(CombatData combatData, AgentData agentData, LogData logData)
    {
        return LogData.InstancePrivacyMode.Unknown;
    }
}
