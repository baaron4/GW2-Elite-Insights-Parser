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

internal class ShatteredObservatoryInstance : ShatteredObservatory
{
    private readonly Skorvald _skorvald;
    private readonly Artsariiv _artsariiv;
    private readonly Arkk _arkk;

    private readonly IReadOnlyList<ShatteredObservatory> _subLogics;
    public ShatteredObservatoryInstance(int triggerID) : base(triggerID)
    {
        LogID = LogIDs.LogMasks.Unsupported;
        Icon = InstanceIconShatteredObservatory;
        Extension = "shtrdobs";

        _skorvald = new Skorvald((int)TargetID.Skorvald);
        _artsariiv = new Artsariiv((int)TargetID.Artsariiv);
        _arkk = new Arkk((int)TargetID.Arkk);
        _subLogics = [_skorvald, _artsariiv, _arkk];

        MechanicList.Add(_skorvald.Mechanics);
        MechanicList.Add(_artsariiv.Mechanics);
        MechanicList.Add(_arkk.Mechanics);
    }

    internal override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log, CombatReplayDecorationContainer arenaDecorations)
    {
        var crMap = new CombatReplayMap((800, 800), (-24576, -24576, 24576, 24576));
        arenaDecorations.Add(new ArenaDecoration((log.LogData.LogStart, log.LogData.LogEnd), CombatReplayShatteredObservatory, crMap));
        foreach (var subLogic in _subLogics)
        {
            subLogic.GetCombatMapInternal(log, arenaDecorations);
        }
        return crMap;
    }
    internal override void CheckSuccess(CombatData combatData, AgentData agentData, LogData logData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        var lastArkk = agentData.GetNPCsByID(TargetID.Arkk).LastOrDefault();
        if (lastArkk != null)
        {
            var death = combatData.GetDeadEvents(lastArkk).FirstOrDefault();
            if (death != null)
            {
                logData.SetSuccess(true, death.Time);
            }
        }
    }
    internal override string GetLogicName(CombatData combatData, AgentData agentData, GW2APIController apiController)
    {
        return "Shattered Observatory";
    }

    private List<EncounterPhaseData> HandleSkorvaldPhases(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, ParsedEvtcLog log, List<PhaseData> phases)
    {
        var encounterPhases = new List<EncounterPhaseData>();
        var mainPhase = phases[0];
        if (targetsByIDs.TryGetValue((int)TargetID.Skorvald, out var skorvalds))
        {
            var anomalies = Targets.Where(x => x.IsAnySpecies(Skorvald.FluxAnomalies));
            var cmAnomalies = anomalies.Where(x => x.IsAnySpecies([TargetID.FluxAnomalyCM1, TargetID.FluxAnomalyCM2, TargetID.FluxAnomalyCM3, TargetID.FluxAnomalyCM4]));
            foreach (var skorvald in skorvalds)
            {
                var firstNonZeroHPUpdate = log.CombatData.GetHealthUpdateEvents(skorvald.AgentItem).FirstOrDefault(x => x.HealthPercent > 0);
                if (firstNonZeroHPUpdate == null)
                {
                    continue;
                }
                var enterCombat = log.CombatData.GetEnterCombatEvents(skorvald.AgentItem).FirstOrDefault();
                long start = Math.Min(enterCombat != null ? enterCombat.Time : long.MaxValue, firstNonZeroHPUpdate.Time);
                bool success = false;
                long end = skorvald.LastAware;
                var death = log.CombatData.GetDeadEvents(skorvald.AgentItem).FirstOrDefault();
                if (death != null)
                {
                    success = true;
                    end = death.Time;
                }
                else
                {
                    var lastDamageTaken = skorvald.GetDamageTakenEvents(null, log).LastOrDefault(x => x.CreditedFrom.IsPlayer);
                    if (lastDamageTaken != null)
                    {
                        var invul895Apply = log.CombatData.GetBuffApplyDataByIDByDst(SkillIDs.Determined895, skorvald.AgentItem).Where(x => x.Time > lastDamageTaken.Time - 500).LastOrDefault();
                        if (invul895Apply != null)
                        {
                            end = invul895Apply.Time;
                            success = true;
                        }
                    }
                }
                var isCM = cmAnomalies.Any(x => skorvald.InAwareTimes(x.FirstAware));
                AddInstanceEncounterPhase(log, phases, encounterPhases, [skorvald], anomalies, [], mainPhase, "Skorvald", start, end, success, _skorvald, isCM ? LogData.LogMode.CM : LogData.LogMode.Normal);
            }
        }
        NumericallyRenameEncounterPhases(encounterPhases);
        return encounterPhases;
    }
    private List<EncounterPhaseData> HandleArtsariivPhases(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, ParsedEvtcLog log, List<PhaseData> phases)
    {
        var encounterPhases = new List<EncounterPhaseData>();
        var mainPhase = phases[0];
        if (targetsByIDs.TryGetValue((int)TargetID.Artsariiv, out var artsariivs))
        {
            foreach (var artsariiv in artsariivs)
            {
                long start = artsariiv.FirstAware;
                var determinedBuffs = log.CombatData.GetBuffDataByIDByDst(SkillIDs.Determined762, artsariiv.AgentItem);
                var determinedLost = determinedBuffs.FirstOrDefault(x => x is BuffRemoveAllEvent);
                var enterCombat = log.CombatData.GetEnterCombatEvents(artsariiv.AgentItem).FirstOrDefault();
                if (determinedLost != null && enterCombat != null && enterCombat.Time >= determinedLost.Time)
                {
                    start = determinedLost.Time;
                }
                bool success = false;
                long end = artsariiv.LastAware;
                var death = log.CombatData.GetDeadEvents(artsariiv.AgentItem).FirstOrDefault();
                if (death != null)
                {
                    success = true;
                    end = death.Time;
                } else if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.ArtsariivDeadExplosion, out var effects))
                {
                    var effect = effects.FirstOrDefault(x => artsariiv.InAwareTimes(x.Time - 500));
                    if (effect != null)
                    {
                        success = true;
                        end = effect.Time;
                    }
                }
                AddInstanceEncounterPhase(log, phases, encounterPhases, [artsariiv], [], [], mainPhase, "Artsariiv", start, end, success, _artsariiv, LogData.LogMode.CMNoName );
            }
        }
        NumericallyRenameEncounterPhases(encounterPhases);
        return encounterPhases;
    }

    private List<EncounterPhaseData> HandleArkkPhases(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, ParsedEvtcLog log, List<PhaseData> phases)
    {
        var encounterPhases = new List<EncounterPhaseData>();
        var mainPhase = phases[0];
        if (targetsByIDs.TryGetValue((int)TargetID.Arkk, out var arkks))
        {
            var subBosses = Targets.Where(x => x.IsAnySpecies([TargetID.Archdiviner, TargetID.EliteBrazenGladiator]));
            foreach (var arkk in arkks)
            {
                long start = arkk.FirstAware;
                var arkkStartBuffs = log.CombatData.GetBuffDataByIDByDst(SkillIDs.ArkkStartBuff, arkk.AgentItem);
                var arkkStartBuffApply = arkkStartBuffs.FirstOrDefault(x => x is BuffApplyEvent);
                if (arkkStartBuffApply != null)
                {
                    start = arkkStartBuffApply.Time;
                }
                else
                {
                    var arkkSpawn = log.CombatData.GetSpawnEvents(arkk.AgentItem).FirstOrDefault();
                    if (arkkSpawn != null)
                    {
                        start = arkkSpawn.Time;
                    }
                }
                bool success = false;
                long end = arkk.LastAware;
                var death = log.CombatData.GetDeadEvents(arkk.AgentItem).FirstOrDefault();
                if (death != null)
                {
                    success = true;
                    end = death.Time;
                }
                AddInstanceEncounterPhase(log, phases, encounterPhases, [arkk], subBosses, [], mainPhase, "Arkk", start, end, success, _arkk, LogData.LogMode.CMNoName);
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
            var skorvaldPhases = HandleSkorvaldPhases(targetsByIDs, log, phases);
            foreach (var skorvaldPhase in skorvaldPhases)
            {
                var skorvald = skorvaldPhase.Targets.Keys.First(x => x.IsSpecies(TargetID.Skorvald));
                phases.AddRange(Skorvald.ComputePhases(log, skorvald, Targets, skorvaldPhase, requirePhases));
            }
        }
        {
            var artsariivPhases = HandleArtsariivPhases(targetsByIDs, log, phases);
            foreach (var artsariivPhase in artsariivPhases)
            {
                var artsariiv = artsariivPhase.Targets.Keys.First(x => x.IsSpecies(TargetID.Artsariiv));
                phases.AddRange(Artsariiv.ComputePhases(log, artsariiv, Targets, artsariivPhase, requirePhases));
            }
        }
        {
            var arkkPhases = HandleArkkPhases(targetsByIDs, log, phases);
            foreach (var arkkPhase in arkkPhases)
            {
                var arkk = arkkPhase.Targets.Keys.First(x => x.IsSpecies(TargetID.Arkk));
                phases.AddRange(Arkk.ComputePhases(log, arkk, Targets, TrashMobs, arkkPhase, requirePhases));
            }
        }
        return phases;
    }

    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        List<InstantCastFinder> finders = [
            .. _skorvald.GetInstantCastFinders(),
            .. _artsariiv.GetInstantCastFinders(),
            .. _arkk.GetInstantCastFinders()
        ];
        return finders;
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        List<TargetID> trashes = [
            .. _skorvald.GetTrashMobsIDs(),
            .. _artsariiv.GetTrashMobsIDs(),
            .. _arkk.GetTrashMobsIDs()
        ];
        return trashes.Distinct().ToList();
    }
    internal override IReadOnlyList<TargetID> GetTargetsIDs()
    {
        List<TargetID> targets = [
            .. _skorvald.GetTargetsIDs(),
            .. _artsariiv.GetTargetsIDs(),
            .. _arkk.GetTargetsIDs()
        ];
        return targets.Distinct().ToList();
    }

    internal override IReadOnlyList<TargetID> GetFriendlyNPCIDs()
    {
        List<TargetID> friendlies = [
            .. _skorvald.GetFriendlyNPCIDs(),
            .. _artsariiv.GetFriendlyNPCIDs(),
            .. _arkk.GetFriendlyNPCIDs()
        ];
        return friendlies.Distinct().ToList();
    }
    protected override HashSet<int> IgnoreForAutoNumericalRenaming()
    {
        return [
            ..Skorvald.FluxAnomalies.Select(x => (int)x),
            (int)TargetID.CloneArtsariiv
        ];
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        Skorvald.DetectUnknownAnomalies(agentData, combatData);
        Artsariiv.DetectCloneArtsariivs(evtcVersion, agentData, combatData);
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);
        Skorvald.RenameAnomalies(Targets);
        Artsariiv.RenameSmallArtsariivs(TrashMobs);
        Artsariiv.RenameCloneArtsariivs(Targets, combatData);
    }

    internal override List<BuffEvent> SpecialBuffEventProcess(CombatData combatData, SkillData skillData)
    {
        var res = new List<BuffEvent>();
        foreach (var subLogic in _subLogics)
        {
            res.AddRange(subLogic.SpecialBuffEventProcess(combatData, skillData));
        }
        return res;
    }

    internal override List<CastEvent> SpecialCastEventProcess(CombatData combatData, SkillData skillData)
    {
        var res = new List<CastEvent>();
        foreach (var subLogic in _subLogics)
        {
            res.AddRange(subLogic.SpecialCastEventProcess(combatData, skillData));
        }
        return res;
    }

    internal override List<HealthDamageEvent> SpecialDamageEventProcess(CombatData combatData, AgentData agentData, SkillData skillData)
    {
        var res = new List<HealthDamageEvent>();
        foreach (var subLogic in _subLogics)
        {
            res.AddRange(subLogic.SpecialDamageEventProcess(combatData, agentData, skillData));
        }
        return res;
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputeNPCCombatReplayActors(target, log, replay);
        foreach (var logic in _subLogics)
        {
            logic.ComputeNPCCombatReplayActors(target, log, replay);
        }
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputePlayerCombatReplayActors(p, log, replay);
        foreach (var logic in _subLogics)
        {
            logic.ComputePlayerCombatReplayActors(p, log, replay);
        }
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);
        foreach (var logic in _subLogics)
        {
            logic.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);
        }
    }

    internal override void SetInstanceBuffs(ParsedEvtcLog log, List<InstanceBuff> instanceBuffs)
    {
        base.SetInstanceBuffs(log, instanceBuffs);
        foreach (var logic in _subLogics)
        {
            logic.SetInstanceBuffs(log, instanceBuffs);
        }
    }

    internal override Dictionary<TargetID, int> GetTargetsSortIDs()
    {
        var sortIDs = new Dictionary<TargetID, int>();
        int offset = 0;
        foreach (var logic in _subLogics)
        {
            offset = AddSortIDWithOffset(sortIDs, logic.GetTargetsSortIDs(), offset);
        }
        return sortIDs;
    }
}
