using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicTimeUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal class BastionOfThePenitentInstance : BastionOfThePenitent
{
    private readonly Cairn _cairn;
    private readonly MursaatOverseer _mursaatOverseer;
    private readonly Samarog _samarog;
    private readonly Deimos _deimos;

    private readonly IReadOnlyList<BastionOfThePenitent> _subLogics;
    public BastionOfThePenitentInstance(int triggerID) : base(triggerID)
    {
        LogID = LogIDs.LogMasks.Unsupported;
        Icon = InstanceIconBastionOfThePenitent;
        Extension = "bstpen";

        _cairn = new Cairn((int)TargetID.Cairn);
        _mursaatOverseer = new MursaatOverseer((int)TargetID.MursaatOverseer);
        _samarog = new Samarog((int)TargetID.Samarog);
        _deimos = new Deimos((int)TargetID.Deimos);
        _subLogics = [_cairn, _mursaatOverseer, _samarog, _deimos];

        MechanicList.Add(_cairn.Mechanics);
        MechanicList.Add(_mursaatOverseer.Mechanics);
        MechanicList.Add(_samarog.Mechanics);
        MechanicList.Add(_deimos.Mechanics);
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData)
    {
        return "Bastion Of The Penitent";
    }
    internal override void CheckSuccess(CombatData combatData, AgentData agentData, LogData logData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        var chest = agentData.GetGadgetsByID(_deimos.ChestID).FirstOrDefault();
        if (chest != null)
        {
            logData.SetSuccess(true, chest.FirstAware);
            return;
        }
        base.CheckSuccess(combatData, agentData, logData, playerAgents);
    }

    private List<EncounterPhaseData> HandleCairnPhases(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, ParsedEvtcLog log, List<PhaseData> phases)
    {
        var encounterPhases = new List<EncounterPhaseData>();
        var mainPhase = phases[0];
        if (targetsByIDs.TryGetValue((int)TargetID.Cairn, out var cairns))
        {
            var chest = log.AgentData.GetGadgetsByID(_cairn.ChestID).FirstOrDefault();
            foreach (var cairn in cairns)
            {
                var enterCombat = log.CombatData.GetEnterCombatEvents(cairn.AgentItem).FirstOrDefault();
                var spawnProtectLost = log.CombatData.GetBuffRemoveAllDataByIDByDst(SkillIDs.SpawnProtection, cairn.AgentItem).FirstOrDefault();
                bool hasEnteredCombat = enterCombat != null || spawnProtectLost != null;
                if (!hasEnteredCombat && !log.CombatData.GetDamageTakenData(cairn.AgentItem).Any(x => x.HealthDamage > 0 && x.CreditedFrom.IsPlayer))
                {
                    continue;
                }
                long start = cairn.FirstAware;
                if (hasEnteredCombat)
                {
                    if (enterCombat != null && spawnProtectLost != null)
                    {
                        start = Math.Max(enterCombat.Time, spawnProtectLost.Time);
                    } 
                    else if (enterCombat != null)
                    {
                        start = enterCombat.Time;
                    } 
                    else if (spawnProtectLost != null)
                    {
                        start = spawnProtectLost.Time;
                    }
                }
                bool success = false;
                long end = cairn.LastAware;
                if (chest != null && chest.InAwareTimes(cairn.LastAware + 500))
                {
                    end = chest.FirstAware;
                    success = true;
                }
                AddInstanceEncounterPhase(log, phases, encounterPhases, [cairn], [], [], mainPhase, "Cairn", start, end, success, _cairn, log.CombatData.GetBuffApplyData(SkillIDs.Countdown).Any(x => x.Time >= start && x.Time <= end) ? LogData.LogMode.CM : LogData.LogMode.Normal);
            }
        }
        NumericallyRenameEncounterPhases(encounterPhases);
        return encounterPhases;
    }
    private List<EncounterPhaseData> HandleDeimosPhases(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, ParsedEvtcLog log, List<PhaseData> phases)
    {
        var mainPhase = phases[0];
        var encounterPhases = new List<EncounterPhaseData>();
        HashSet<SingleActor>? handledDeimoss = null;
        if (targetsByIDs.TryGetValue((int)TargetID.DummyTarget, out var dummies) && targetsByIDs.TryGetValue((int)TargetID.DemonicBond, out var demonicBonds))
        {
            var deimosDummy = dummies.FirstOrDefault(x => x.Character == "Deimos Pre Event");
            if (deimosDummy != null)
            {
                var chest = log.AgentData.GetGadgetsByID(_deimos.ChestID).FirstOrDefault();
                var nonBlockingSubBosses = Targets.Where(x => x.IsAnySpecies([TargetID.Thief, TargetID.Gambler, TargetID.Drunkard]));
                long encounterStartThreshold = 0;
                var greenApplies = log.CombatData.GetBuffApplyData(SkillIDs.GreenTeleport);
                foreach (AbstractBuffApplyEvent buffApplyEvent in greenApplies)
                {
                    if (buffApplyEvent.Time >= encounterStartThreshold)
                    {
                        long start = long.MaxValue;
                        long end = long.MinValue;
                        // Find encounter start based on demonic bonds being targetable
                        foreach (var demonicBond in demonicBonds)
                        {
                            var attackTargetEvents = log.CombatData.GetAttackTargetEventsBySrc(demonicBond.AgentItem);
                            foreach (var attackTargetEvent in attackTargetEvents)
                            {
                                var targetableEvent = attackTargetEvent.GetTargetableEvents(log).FirstOrDefault(x => x.Time >= buffApplyEvent.Time - 2000 && x.Time <= buffApplyEvent.Time);
                                if (targetableEvent != null)
                                {
                                    start = targetableEvent.Time;
                                    break;
                                }
                            }
                        }
                        if (start == long.MaxValue)
                        {
                            continue;
                        }
                        // Find deimos or remain on dummy
                        SingleActor target = deimosDummy;
                        if (targetsByIDs.TryGetValue((int)TargetID.Deimos, out var deimoss))
                        {
                            var deimos = deimoss.FirstOrDefault(x => x.FirstAware > start || x.AgentItem.InAwareTimes(start));
                            if (deimos != null)
                            {
                                handledDeimoss ??= new HashSet<SingleActor>(deimoss.Count);
                                handledDeimoss.Add(deimos);
                                target = deimos;
                            }
                        }
                        // Verify that it is the deimos of current encounter and not a following one
                        // Check if the following batch of targetable demonic bonds are before its first aware
                        if (target.IsSpecies(TargetID.Deimos))
                        {
                            foreach (var demonicBond in demonicBonds)
                            {
                                var attackTargetEvents = log.CombatData.GetAttackTargetEventsBySrc(demonicBond.AgentItem);
                                foreach (var attackTargetEvent in attackTargetEvents)
                                {
                                    var targetableEvent = attackTargetEvent.GetTargetableEvents(log).FirstOrDefault(x => x.Time >= start + 5000);
                                    if (targetableEvent != null)
                                    {
                                        if (target.FirstAware > targetableEvent.Time)
                                        {
                                            target = deimosDummy;
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        // Wiped during pre event
                        if (!target.IsSpecies(TargetID.Deimos))
                        {
                            var prides = log.AgentData.GetNPCsByID(TargetID.Pride);
                            if (prides.Any())
                            {
                                end = Math.Max(prides.Max(x => x.LastAware), end);
                            }
                            var greeds = log.AgentData.GetNPCsByID(TargetID.Greed);
                            if (greeds.Any())
                            {
                                end = Math.Max(greeds.Max(x => x.LastAware), end);
                            }
                        }
                        if (end == long.MinValue)
                        {
                            continue;
                        }
                        bool success = false;
                        if (chest != null && chest.InAwareTimes(start, end + 2000))
                        {
                            end = chest.FirstAware;
                            success = true;
                        }
                        AddInstanceEncounterPhase(log, phases, encounterPhases, [target], demonicBonds, nonBlockingSubBosses, mainPhase, "Deimos", start, end, success, _deimos, target.GetHealth(log.CombatData) > 40e6 ? LogData.LogMode.CM : LogData.LogMode.Normal);
                    }
                }
            }
        }
        // Handle deimoss without pre events
        {
            if (targetsByIDs.TryGetValue((int)TargetID.Deimos, out var deimoss))
            {
                if (handledDeimoss == null || handledDeimoss.Count != deimoss.Count)
                {
                    var chest = log.AgentData.GetGadgetsByID(_deimos.ChestID).FirstOrDefault();
                    var nonBlockingSubBosses = Targets.Where(x => x.IsAnySpecies([TargetID.Thief, TargetID.Gambler, TargetID.Drunkard]));
                    foreach (var deimos in deimoss)
                    {
                        var spawnProtectionRemove = log.CombatData.GetBuffRemoveAllDataByIDByDst(SpawnProtection, deimos.AgentItem).FirstOrDefault();
                        long start = deimos.FirstAware;
                        bool skip = !log.CombatData.GetDamageTakenData(deimos.AgentItem).Any(x => x.CreditedFrom.IsPlayer);
                        if (spawnProtectionRemove != null)
                        {
                            skip = false;
                            start = spawnProtectionRemove.Time;
                        }
                        if (skip)
                        {
                            continue;
                        }
                        long end = deimos.LastAware;
                        bool success = false;
                        if (chest != null && chest.InAwareTimes(start, end + 2000))
                        {
                            end = chest.FirstAware;
                            success = true;
                        }
                        AddInstanceEncounterPhase(log, phases, encounterPhases, [deimos], [], nonBlockingSubBosses, mainPhase, "Deimos", start, end, success, _deimos, deimos.GetHealth(log.CombatData) > 40e6 ? LogData.LogMode.CM : LogData.LogMode.Normal, LogData.LogStartStatus.NoPreEvent);
                    }
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
        HandleCairnPhases(targetsByIDs, log, phases);
        {
            var moPhases = ProcessGenericEncounterPhasesForInstance(targetsByIDs, log, phases, TargetID.MursaatOverseer, [], "Mursaat Overseer", _mursaatOverseer, (log, mursaat) => mursaat.GetHealth(log.CombatData) > 25e6 ? LogData.LogMode.CM : LogData.LogMode.Normal);
            foreach (var moPhase in moPhases)
            {
                var mursaatOverseer = moPhase.Targets.Keys.First(x => x.IsSpecies(TargetID.MursaatOverseer));
                phases.AddRange(MursaatOverseer.ComputePhases(log, mursaatOverseer, moPhase, requirePhases));
            }
        }
        {
            var samarogPhases = ProcessGenericEncounterPhasesForInstance(targetsByIDs, log, phases, TargetID.Samarog, Targets.Where(x => x.IsAnySpecies([TargetID.Guldhem, TargetID.Rigom])), "Samarog", _samarog, (log, samarog) => samarog.GetHealth(log.CombatData) > 30e6 ? LogData.LogMode.CM : LogData.LogMode.Normal);
            foreach (var samarogPhase in samarogPhases)
            {
                var samarog = samarogPhase.Targets.Keys.First(x => x.IsSpecies(TargetID.Samarog));
                phases.AddRange(Samarog.ComputePhases(log, samarog, Targets, samarogPhase, requirePhases));
            }
        }
        {
            var deimosPhases = HandleDeimosPhases(targetsByIDs, log, phases);
            foreach (var deimosPhase in deimosPhases)
            {
                var deimos = deimosPhase.Targets.Keys.First(x => x.IsSpecies(TargetID.Deimos));
                phases.AddRange(Deimos.ComputePhases(log, deimos, Targets, deimosPhase, requirePhases));
            }
        }
        return phases;
    }

    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        List<InstantCastFinder> finders = [
            .. _cairn.GetInstantCastFinders(),
            .. _mursaatOverseer.GetInstantCastFinders(),
            .. _samarog.GetInstantCastFinders(),
            .. _deimos.GetInstantCastFinders()
        ];
        return finders;
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        List<TargetID> trashes = [
            .. _cairn.GetTrashMobsIDs(),
            .. _mursaatOverseer.GetTrashMobsIDs(),
            .. _samarog.GetTrashMobsIDs(),
            .. _deimos.GetTrashMobsIDs()
        ];
        return trashes.Distinct().ToList();
    }
    internal override IReadOnlyList<TargetID> GetTargetsIDs()
    {
        List<TargetID> targets = [
            .. _cairn.GetTargetsIDs(),
            .. _mursaatOverseer.GetTargetsIDs(),
            .. _samarog.GetTargetsIDs(),
            .. _deimos.GetTargetsIDs()
        ];
        return targets.Distinct().ToList();
    }

    internal override IReadOnlyList<TargetID> GetFriendlyNPCIDs()
    {
        List<TargetID> friendlies = [
            .. _cairn.GetFriendlyNPCIDs(),
            .. _mursaatOverseer.GetFriendlyNPCIDs(),
            .. _samarog.GetFriendlyNPCIDs(),
            .. _deimos.GetFriendlyNPCIDs()
        ];
        return friendlies.Distinct().ToList();
    }

    private void HandleDeimosAndItsGadgets(LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        var attackTargetEvents = combatData.Where(x => x.IsStateChange == StateChange.AttackTarget).Select(x => new AttackTargetEvent(x, agentData));
        var targetableEvents = new List<TargetableEvent>();
        foreach (var attackTarget in attackTargetEvents)
        {
            targetableEvents.AddRange(attackTarget.GetTargetableEvents(combatData, agentData));
        }
        targetableEvents.SortByTime();
        foreach (var deimos in Targets.Where(x => x.IsSpecies(TargetID.Deimos)))
        {
            (AgentItem? deimosStructBody, HashSet<AgentItem> gadgetAgents, long deimos10PercentTargetable, long notTargetable) = Deimos.FindDeimos10PercentBodyStructWithAttackTargets(deimos, logData, agentData, combatData, attackTargetEvents, targetableEvents);
            Deimos.HandleDeimosAndItsGadgets(deimos, deimosStructBody, gadgetAgents, agentData, combatData, extensions, deimos10PercentTargetable, notTargetable + 1000);
        }
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        Samarog.HandleSpears(evtcVersion, agentData, combatData);
        Deimos.HandleDemonicBonds(agentData, combatData);
        Deimos.HandleShackledPrisoners(agentData, combatData);
        agentData.AddCustomNPCAgent(logData.LogStart, logData.LogEnd, "Deimos Pre Event", Spec.NPC, TargetID.DummyTarget, true);
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);
        Deimos.RenameTargetSauls(Targets);
        HandleDeimosAndItsGadgets(logData, agentData, combatData, extensions);
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

    // TODO: handle duplicates due multiple base method calls in Combat Replay methods
    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputeNPCCombatReplayActors(target, log, replay);
        foreach (BastionOfThePenitent logic in _subLogics)
        {
            logic.ComputeNPCCombatReplayActors(target, log, replay);
        }
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputePlayerCombatReplayActors(p, log, replay);
        foreach (BastionOfThePenitent logic in _subLogics)
        {
            logic.ComputePlayerCombatReplayActors(p, log, replay);
        }
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);
        foreach (BastionOfThePenitent logic in _subLogics)
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
    internal override LogData.LogMode GetLogMode(CombatData combatData, AgentData agentData, LogData logData)
    {
       foreach (var deimos in Targets.Where(x => x.IsSpecies(TargetID.Deimos)))
        {
            Deimos.AdjustDeimosHP(deimos, deimos.GetHealth(combatData) > 40e6);
        }
        return base.GetLogMode(combatData, agentData, logData);
    }
}
