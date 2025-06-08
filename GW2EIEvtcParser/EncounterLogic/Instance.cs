using System.Collections.Generic;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.MapIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class Instance : FightLogic
{
    public bool StartedLate { get; private set; }
    public bool EndedBeforeExpectedEnd { get; private set; }
    private readonly List<TargetID> _targetIDs = [];
    public Instance(int id) : base(id)
    {
        Extension = "instance";
        ParseMode = ParseModeEnum.FullInstance;
        SkillMode = SkillModeEnum.PvE;
        Icon = InstanceIconGeneric;
        EncounterCategoryInformation.Category = FightCategory.UnknownEncounter;
        EncounterCategoryInformation.SubCategory = SubFightCategory.UnknownEncounter;
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
                if (blackList.Contains(targetID) || !maxHPUpdates.TryGetValue((int)targetID, out var maxHPs) || !maxHPs.Any(x => MaxHealthUpdateEvent.GetMaxHealth(x) > 500000))
                {
                    continue;
                }
                _targetIDs.Add(targetID);
            }
        }
    }

    internal override FightLogic AdjustLogic(AgentData agentData, List<CombatItem> combatData, EvtcParserSettings parserSettings)
    {
        CombatItem? mapIDEvent = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.MapID);
        // Handle potentially wrongly associated logs
        if (mapIDEvent != null)
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
                    return new WvWFight(GenericTriggerID, parserSettings.DetailedWvWParse, true);
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

    internal override long GetFightOffset(EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData)
    {
        return GetGenericFightOffset(fightData);
    }

    internal override void UpdatePlayersSpecAndGroup(IReadOnlyList<Player> players, CombatData combatData, FightData fightData)
    {
        // Nothing to do
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
        phases[0].AddTargets(Targets, log);
        int phaseCount = 0;
        foreach (SingleActor target in Targets)
        {
            var phase = new PhaseData(Math.Max(log.FightData.FightStart, target.FirstAware), Math.Min(target.LastAware, log.FightData.FightEnd), "Phase " + (++phaseCount));
            phase.AddTarget(target, log);
            phases.Add(phase);
        }
        return phases;
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        FindGenericTargetIDs(agentData, combatData);
        Targetless = _targetIDs.Count == 0;
        if (Targetless)
        {
            agentData.AddCustomNPCAgent(fightData.FightStart, fightData.FightEnd, "Dummy Instance Target", ParserHelper.Spec.NPC, TargetID.Instance, true);
        }
        base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
        // Generic name override
        if (!Targetless)
        {
            var speciesCount = new Dictionary<long, int>();
            foreach (SingleActor target in Targets)
            {
                if (!speciesCount.TryGetValue(target.ID, out var species))
                {
                    species = 1;
                    speciesCount[target.ID] = species;
                }
                target.OverrideName(target.Character + " " + species);
                speciesCount[target.ID] = species++;
            }
        }
    }

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        fightData.SetSuccess(true, fightData.FightEnd);
    }

    internal override FightData.EncounterStartStatus GetEncounterStartStatus(CombatData combatData, AgentData agentData, FightData fightData)
    {
        InstanceStartEvent? evt = combatData.GetInstanceStartEvent();
        if (evt == null)
        {
            return FightData.EncounterStartStatus.Normal;
        }
        else
        {
            return evt.TimeOffsetFromInstanceCreation > ParserHelper.MinimumInCombatDuration ? FightData.EncounterStartStatus.Late : FightData.EncounterStartStatus.Normal;
        }
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData)
    {
        return base.GetLogicName(combatData, agentData);
    }
    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        return [];
    }
    internal override IEnumerable<ErrorEvent> GetCustomWarningMessages(FightData fightData, EvtcVersionEvent evtcVersion)
    {
        return base.GetCustomWarningMessages(fightData, evtcVersion);
    }
    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
    }
    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
    }
    internal override List<BuffEvent> SpecialBuffEventProcess(CombatData combatData, SkillData skillData)
    {
        return [];
    }
    internal override List<CastEvent> SpecialCastEventProcess(CombatData combatData, SkillData skillData)
    {
        return [];
    }
    internal override List<HealthDamageEvent> SpecialDamageEventProcess(CombatData combatData, SkillData skillData)
    {
        return [];
    }
    protected override IReadOnlyList<TargetID> GetTargetsIDs()
    {
        return _targetIDs.Count > 0 ? _targetIDs : [TargetID.Instance];
    }
    protected override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        return [];
    }
    protected override IReadOnlyList<TargetID>  GetUniqueNPCIDs()
    {
        return [];
    }
    protected override IReadOnlyList<TargetID>  GetFriendlyNPCIDs()
    {
        return [];
    }

    protected override Dictionary<TargetID, int> GetTargetsSortIDs()
    {
        return [];
    }

    internal override List<PhaseData> GetBreakbarPhases(ParsedEvtcLog log, bool requirePhases)
    {
        return [];
    }

    internal override FightData.InstancePrivacyMode GetInstancePrivacyMode(CombatData combatData, AgentData agentData, FightData fightData)
    {
        return FightData.InstancePrivacyMode.Unknown;
    }
}
