using System.Collections.Generic;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class UnknownInstanceLogic : UnknownFightLogic
{
    public bool StartedLate { get; private set; }
    public bool EndedBeforeExpectedEnd { get; private set; }
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
        var maxHPUpdates = combatData.Where(x => x.IsStateChange == ArcDPSEnums.StateChange.MaxHealthUpdate && agentData.GetAgent(x.SrcAgent, x.Time).Type == AgentItem.AgentType.NPC && MaxHealthUpdateEvent.GetMaxHealth(x) > 0).GroupBy(x => agentData.GetAgent(x.SrcAgent, x.Time).ID).ToDictionary(x => x.Key, x => x.ToList());
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

    internal override FightLogic AdjustLogic(AgentData agentData, List<CombatItem> combatData, EvtcParserSettings parserSettings)
    {
        CombatItem? mapIDEvent = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.MapID);
        // Handle potentially wrongly associated logs
        if (mapIDEvent != null)
        {
            switch (MapIDEvent.GetMapID(mapIDEvent))
            {
                // EB
                case 38:
                // Green Alpine
                case 95:
                // Blue Alpine
                case 96:
                // Red Desert
                case 1099: 
                // EoM
                case 968:
                // Bastion
                case 1315:
                    return new WvWFight(GenericTriggerID, parserSettings.DetailedWvWParse, true);
            }
        }
        return base.AdjustLogic(agentData, combatData, parserSettings);
    }

    internal override long GetFightOffset(EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData)
    {
        return GetGenericFightOffset(fightData);
    }

    internal static void AddPhasesPerTarget(ParsedEvtcLog log, List<PhaseData> phases, IEnumerable<SingleActor> targets)
    {
        phases[0].AddTargets(targets, log);
        foreach (SingleActor target in targets)
        {
            var phase = new PhaseData(Math.Max(log.FightData.FightStart, target.FirstAware), Math.Min(target.LastAware, log.FightData.FightEnd), target.Character);
            phase.AddTarget(target, log);
            phase.AddParentPhase(phases[0]);
            phases.Add(phase);
        }
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
            EncounterLogicUtils.NumericallyRenameSpecies(Targets);
        }
    }

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        fightData.SetSuccess(true, fightData.FightEnd);
    }

    internal static FightData.EncounterStartStatus GetInstanceStartStatus(CombatData combatData, long threshold = 10000)
    {
        InstanceStartEvent? evt = combatData.GetInstanceStartEvent();
        if (evt == null)
        {
            return FightData.EncounterStartStatus.Normal;
        }
        else
        {
            return evt.TimeOffsetFromInstanceCreation > threshold ? FightData.EncounterStartStatus.Late : FightData.EncounterStartStatus.Normal;
        }
    }

    internal override FightData.EncounterStartStatus GetEncounterStartStatus(CombatData combatData, AgentData agentData, FightData fightData)
    {
        return GetInstanceStartStatus(combatData);
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData)
    {
        MapIDEvent? mapID = combatData.GetMapIDEvents().LastOrDefault();
        if (mapID == null)
        {
            return base.GetLogicName(combatData, agentData);
        }
        switch (mapID.MapID)
        {
            // Raids
            case 1062:
                EncounterCategoryInformation.Category = FightCategory.Raid;
                EncounterCategoryInformation.SubCategory = SubFightCategory.SpiritVale;
                Icon = InstanceIconSpiritVale;
                Extension = "sprtvale";
                return "Spirit Vale";
            case 1149:
                EncounterCategoryInformation.Category = FightCategory.Raid;
                EncounterCategoryInformation.SubCategory = SubFightCategory.SalvationPass;
                Icon = InstanceIconSalvationPass;
                Extension = "salvpass";
                return "Salvation Pass";
            case 1156:
                EncounterCategoryInformation.Category = FightCategory.Raid;
                EncounterCategoryInformation.SubCategory = SubFightCategory.StrongholdOfTheFaithful;
                Icon = InstanceIconStrongholdOfTheFaithful;
                Extension = "strgldfaith";
                return "Stronghold Of The Faithful";
            case 1188:
                EncounterCategoryInformation.Category = FightCategory.Raid;
                EncounterCategoryInformation.SubCategory = SubFightCategory.BastionOfThePenitent;
                Icon = InstanceIconBastionOfThePenitent;
                Extension = "bstpen";
                return "Bastion Of The Penitent";
            case 1264:
                EncounterCategoryInformation.Category = FightCategory.Raid;
                EncounterCategoryInformation.SubCategory = SubFightCategory.HallOfChains;
                Icon = InstanceIconHallOfChains;
                Extension = "hallchains";
                return "Hall Of Chains";
            case 1303:
                EncounterCategoryInformation.Category = FightCategory.Raid;
                EncounterCategoryInformation.SubCategory = SubFightCategory.MythwrightGambit;
                Icon = InstanceIconMythwrightGambit;
                Extension = "mythgamb";
                return "Mythwright Gambit";
            case 1323:
                EncounterCategoryInformation.Category = FightCategory.Raid;
                EncounterCategoryInformation.SubCategory = SubFightCategory.TheKeyOfAhdashim;
                Icon = InstanceIconTheKeyOfAhdashim;
                Extension = "keyadash";
                return "The Key Of Ahdashim";
            // Fractals
            case 960:
                EncounterCategoryInformation.Category = FightCategory.Fractal;
                EncounterCategoryInformation.SubCategory = SubFightCategory.CaptainMaiTrinBossFractal;
                Icon = InstanceIconCaptainMaiTrin;
                Extension = "captnmai";
                return "Captain Mai Trin Boss Fractal";
            case 1177:
                EncounterCategoryInformation.Category = FightCategory.Fractal;
                EncounterCategoryInformation.SubCategory = SubFightCategory.Nightmare;
                Icon = InstanceIconNightmare;
                Extension = "nightmare";
                return "Nightmare";
            case 1205:
                EncounterCategoryInformation.Category = FightCategory.Fractal;
                EncounterCategoryInformation.SubCategory = SubFightCategory.ShatteredObservatory;
                Icon = InstanceIconShatteredObservatory;
                Extension = "shatrdobs";
                return "Shattered Observatory";
            case 1290:
                EncounterCategoryInformation.Category = FightCategory.Fractal;
                EncounterCategoryInformation.SubCategory = SubFightCategory.Deepstone;
                Icon = InstanceIconDeepstone;
                Extension = "deepstone";
                return "Deepstone";
            case 1384:
                EncounterCategoryInformation.Category = FightCategory.Fractal;
                EncounterCategoryInformation.SubCategory = SubFightCategory.SunquaPeak;
                Icon = InstanceIconSunquaPeak;
                Extension = "snqpeak";
                return "Sunqua Peak";
        }
        return base.GetLogicName(combatData, agentData);
    }
    protected override IReadOnlyList<TargetID> GetTargetsIDs()
    {
        return _targetIDs.Count > 0 ? _targetIDs : [TargetID.Instance];
    }
    protected override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        return _trashIDs;
    }

    internal override List<PhaseData> GetBreakbarPhases(ParsedEvtcLog log, bool requirePhases)
    {
        return [];
    }
}
