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

internal class NightmareInstance : Nightmare
{
    private readonly MAMA _mama;
    private readonly Siax _siax;
    private readonly Ensolyss _ensolyss;

    private readonly IReadOnlyList<Nightmare> _subLogics;
    public NightmareInstance(int triggerID) : base(triggerID)
    {
        LogID = LogIDs.LogMasks.Unsupported;
        Icon = InstanceIconNightmare;
        Extension = "nightmare";

        _mama = new MAMA((int)TargetID.MAMA);
        _siax = new Siax((int)TargetID.Siax);
        _ensolyss = new Ensolyss((int)TargetID.Ensolyss);
        _subLogics = [_mama, _siax, _ensolyss];

        MechanicList.Add(_mama.Mechanics);
        MechanicList.Add(_siax.Mechanics);
        MechanicList.Add(_ensolyss.Mechanics);
    }

    internal override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log, CombatReplayDecorationContainer arenaDecorations)
    {
        var crMap = new CombatReplayMap((800, 800), (-6144, -6144, 9216, 9216));
        arenaDecorations.Add(new ArenaDecoration((log.LogData.LogStart, log.LogData.LogEnd), CombatReplayNightmare, crMap));
        foreach (var subLogic in _subLogics)
        {
            subLogic.GetCombatMapInternal(log, arenaDecorations);
        }
        return crMap;
    }

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, LogData logData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        var lastEnsolyss = agentData.GetNPCsByID(TargetID.Ensolyss).LastOrDefault();
        if (lastEnsolyss != null)
        {
            var death = combatData.GetDeadEvents(lastEnsolyss).FirstOrDefault();
            if (death != null)
            {
                logData.SetSuccess(true, death.Time);
            }
        }
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData, GW2APIController apiController)
    {
        return "Nightmare";
    }

    private List<EncounterPhaseData> HandleMAMAPhases(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, ParsedEvtcLog log, List<PhaseData> phases)
    {
        var encounterPhases = new List<EncounterPhaseData>();
        var mainPhase = phases[0];
        if (targetsByIDs.TryGetValue((int)TargetID.MAMA, out var mamas))
        {
            var knightIDs = MAMA.KnightPhases.Select(pair => pair.Item1).ToList();
            var knights = Targets.Where(x => x.IsAnySpecies(knightIDs));
            foreach (var mama in mamas)
            {
                // Make sure encounter was started
                var firstDamageTaken = log.CombatData.GetDamageTakenData(mama.AgentItem).FirstOrDefault(x => x.CreditedFrom.IsPlayer && x.HealthDamage > 0);
                if (firstDamageTaken == null)
                {
                    continue;
                }
                var enterCombat = log.CombatData.GetEnterCombatEvents(mama.AgentItem).FirstOrDefault();
                long start = enterCombat != null ? enterCombat.Time : mama.FirstAware;
                bool success = false;
                long end = mama.LastAware;
                var death = log.CombatData.GetDeadEvents(mama.AgentItem).FirstOrDefault();
                if (death != null)
                {
                    success = true;
                    end = death.Time;
                }
                AddInstanceEncounterPhase(log, phases, encounterPhases, [mama], knights, [], mainPhase, "MAMA", start, end, success, _mama, LogData.LogMode.CMNoName);
            }
        }
        NumericallyRenameEncounterPhases(encounterPhases);
        return encounterPhases;
    }
    private List<EncounterPhaseData> HandleSiaxPhases(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, ParsedEvtcLog log, List<PhaseData> phases)
    {
        var encounterPhases = new List<EncounterPhaseData>();
        var mainPhase = phases[0];
        if (targetsByIDs.TryGetValue((int)TargetID.Siax, out var siaxes))
        {
            var echoes = Targets.Where(x => x.IsSpecies(TargetID.EchoOfTheUnclean));
            foreach (var siax in siaxes)
            {
                // Make sure encounter was started
                var firstDamageTaken = log.CombatData.GetDamageTakenData(siax.AgentItem).FirstOrDefault(x => x.CreditedFrom.IsPlayer && x.HealthDamage > 0);
                if (firstDamageTaken == null)
                {
                    continue;
                }
                // TODO: test this start more
                var buffApply = log.CombatData.GetBuffApplyDataByDst(siax.AgentItem).FirstOrDefault(x => x.CreditedBy.IsPlayer);
                // Put initial start to when players start doing something to the boss
                long start = Math.Min(firstDamageTaken.Time, buffApply != null ? buffApply.Time : long.MaxValue);
                var enterCombat = log.CombatData.GetEnterCombatEvents(siax.AgentItem).FirstOrDefault();
                // If enter combat present, keep it if before initial start
                if (enterCombat != null)
                {
                    start = Math.Min(enterCombat.Time, start);
                }
                bool success = false;
                long end = siax.LastAware;
                var death = log.CombatData.GetDeadEvents(siax.AgentItem).FirstOrDefault();
                if (death != null)
                {
                    success = true;
                    end = death.Time;
                }
                AddInstanceEncounterPhase(log, phases, encounterPhases, [siax], echoes, [], mainPhase, "Siax the Corrupted", start, end, success, _siax, LogData.LogMode.CMNoName);
            }
        }
        NumericallyRenameEncounterPhases(encounterPhases);
        return encounterPhases;
    }
    private List<EncounterPhaseData> HandleEnsolyssPhases(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, ParsedEvtcLog log, List<PhaseData> phases)
    {
        var encounterPhases = new List<EncounterPhaseData>();
        var mainPhase = phases[0];
        if (targetsByIDs.TryGetValue((int)TargetID.Ensolyss, out var ensolysses))
        {
            foreach (var ensolyss in ensolysses)
            {
                long start = ensolyss.FirstAware;
                var determinedBuffs = log.CombatData.GetBuffDataByIDByDst(SkillIDs.Determined762, ensolyss.AgentItem);
                var determinedLost = determinedBuffs.FirstOrDefault(x => x is BuffRemoveAllEvent);
                var enterCombat = log.CombatData.GetEnterCombatEvents(ensolyss.AgentItem).FirstOrDefault();
                if (determinedLost != null && enterCombat != null && enterCombat.Time >= determinedLost.Time)
                {
                    start = determinedLost.Time;
                    // ensolyss exits combat during split phases and reenters after
                    // make sure we dont have a late start via caustic explosion, expected to happen 2s after invuln remove in later phases
                    var causticExplosion = ensolyss.GetAnimatedCastEvents(log).FirstOrDefault(x => x.SkillID == SkillIDs.CausticExplosionEnsolyss && x.Time > start);
                    if (causticExplosion != null && causticExplosion.Time <= start + 3000)
                    {
                        start = ensolyss.FirstAware;
                    }
                }
                else
                {
                    continue;
                }
                bool success = false;
                long end = ensolyss.LastAware;
                var death = log.CombatData.GetDeadEvents(ensolyss.AgentItem).FirstOrDefault();
                if (death != null)
                {
                    success = true;
                    end = death.Time;
                }
                AddInstanceEncounterPhase(log, phases, encounterPhases, [ensolyss], [], [], mainPhase, "Ensolyss of the Endless Torment", start, end, success, _ensolyss, LogData.LogMode.CMNoName);
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
            var mamaPhases = HandleMAMAPhases(targetsByIDs, log, phases);
            foreach (var mamaPhase in mamaPhases)
            {
                var mama = mamaPhase.Targets.Keys.First(x => x.IsSpecies(TargetID.MAMA));
                phases.AddRange(MAMA.ComputePhases(log, mama, Targets, mamaPhase, requirePhases));
            }
        }
        {
            var siaxPhases = HandleSiaxPhases(targetsByIDs, log, phases);
            foreach (var siaxPhase in siaxPhases)
            {
                var siax = siaxPhase.Targets.Keys.First(x => x.IsSpecies(TargetID.Siax));
                phases.AddRange(Siax.ComputePhases(log, siax, Targets, siaxPhase, requirePhases));
            }
        }
        {
            var ensolyssPhases = HandleEnsolyssPhases(targetsByIDs, log, phases);
            foreach (var ensolyssPhase in ensolyssPhases)
            {
                var ensolyss = ensolyssPhase.Targets.Keys.First(x => x.IsSpecies(TargetID.Ensolyss));
                phases.AddRange(Ensolyss.ComputePhases(log, ensolyss, ensolyssPhase, requirePhases));
            }
        }
        return phases;
    }

    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        List<InstantCastFinder> finders = [
            .. _mama.GetInstantCastFinders(),
            .. _siax.GetInstantCastFinders(),
            .. _ensolyss.GetInstantCastFinders()
        ];
        return finders;
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        List<TargetID> trashes = [
            .. _mama.GetTrashMobsIDs(),
            .. _siax.GetTrashMobsIDs(),
            .. _ensolyss.GetTrashMobsIDs()
        ];
        return trashes.Distinct().ToList();
    }
    internal override IReadOnlyList<TargetID> GetTargetsIDs()
    {
        List<TargetID> targets = [
            .. _mama.GetTargetsIDs(),
            .. _siax.GetTargetsIDs(),
            .. _ensolyss.GetTargetsIDs()
        ];
        return targets.Distinct().ToList();
    }

    internal override IReadOnlyList<TargetID> GetFriendlyNPCIDs()
    {
        List<TargetID> friendlies = [
            .. _mama.GetFriendlyNPCIDs(),
            .. _siax.GetFriendlyNPCIDs(),
            .. _ensolyss.GetFriendlyNPCIDs()
        ];
        return friendlies.Distinct().ToList();
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        Ensolyss.IgnoreFakeEnsolysses(agentData, combatData);
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);
        Siax.RenameSiaxAndEchoes(Targets, combatData);
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

    internal override List<CastEvent> SpecialCastEventProcess(CombatData combatData, AgentData agentData, SkillData skillData)
    {
        var res = new List<CastEvent>();
        foreach (var subLogic in _subLogics)
        {
            res.AddRange(subLogic.SpecialCastEventProcess(combatData, agentData, skillData));
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

    internal override void SetInstanceBuffs(ParsedEvtcLog log, List<InstanceBuff> instanceBuffs)
    {
        base.SetInstanceBuffs(log, instanceBuffs);
        foreach (var logic in _subLogics)
        {
            logic.SetInstanceBuffs(log, instanceBuffs);
        }
    }
}
