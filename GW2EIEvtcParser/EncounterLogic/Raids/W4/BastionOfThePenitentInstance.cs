using System.Collections.Generic;
using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class BastionOfThePenitentInstance : BastionOfThePenitent
{
    private readonly Cairn _cairn;
    private readonly MursaatOverseer _mursaatOverseer;
    private readonly Samarog _samarog;
    private readonly Deimos _deimos;

    private readonly IReadOnlyList<BastionOfThePenitent> _subLogics;
    public BastionOfThePenitentInstance(int triggerID) : base(triggerID)
    {
        EncounterID = EncounterIDs.EncounterMasks.Unsupported;
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

    private static void HandleCairnPhases(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, ParsedEvtcLog log, List<PhaseData> phases)
    {
        var encounterPhases = new List<PhaseData>();
        var mainPhase = phases[0];
        if (targetsByIDs.TryGetValue((int)TargetID.Cairn, out var cairns))
        {
            var chest = log.AgentData.GetGadgetsByID(ChestID.CairnChest).FirstOrDefault();
            foreach (var cairn in cairns)
            {
                var enterCombat = log.CombatData.GetEnterCombatEvents(cairn.AgentItem).FirstOrDefault();
                var spawnProtectLost = log.CombatData.GetBuffRemoveAllData(SkillIDs.SpawnProtection).Where(x => x.To == cairn.AgentItem).FirstOrDefault();
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
                var phase = new PhaseData(start, end, "Cairn");
                phases.Add(phase);
                encounterPhases.Add(phase);
                if (log.CombatData.GetBuffApplyData(SkillIDs.Countdown).Any(x => x.Time >= start && x.Time <= end))
                {
                    phase.Name += " CM";
                }
                if (success)
                {
                    phase.Name += " (Success)";
                }
                else
                {
                    phase.Name += " (Failure)";
                }
                phase.AddParentPhase(mainPhase);
                phase.AddTarget(cairn, log);
                mainPhase.AddTarget(cairn, log);
            }
        }
        NumericallyRenamePhases(encounterPhases);
    }

    private void HandleDeimosPhases(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, ParsedEvtcLog log, List<PhaseData> phases)
    {
        var mainPhase = phases[0];
        var encounterPhases = new List<PhaseData>();
        if (targetsByIDs.TryGetValue((int)TargetID.DummyTarget, out var dummies) && targetsByIDs.TryGetValue((int)TargetID.DemonicBond, out var demonicBonds))
        {
            var deimosDummy = dummies.FirstOrDefault(x => x.Character == "Deimos Pre Event");
            var chest = log.AgentData.GetGadgetsByID(ChestID.SaulsTreasureChest).FirstOrDefault();
            if (deimosDummy != null)
            {
                long encounterStartThreshold = 0;
                var greenApplies = log.CombatData.GetBuffApplyData(SkillIDs.GreenTeleport);
                foreach (AbstractBuffApplyEvent buffApplyEvent in greenApplies)
                {
                    if (buffApplyEvent.Time >= encounterStartThreshold)
                    {
                        long encounterStart = long.MaxValue;
                        long encounterEnd = long.MinValue;
                        // Find encounter start based on demonic bonds being targetable
                        foreach (var demonicBond in demonicBonds)
                        {
                            var attackTargetEvents = log.CombatData.GetAttackTargetEventsBySrc(demonicBond.AgentItem);
                            foreach (var attackTargetEvent in attackTargetEvents)
                            {
                                var targetableEvent = attackTargetEvent.GetTargetableEvents(log).FirstOrDefault(x => x.Time >= buffApplyEvent.Time - 2000 && x.Time <= buffApplyEvent.Time);
                                if (targetableEvent != null)
                                {
                                    encounterStart = targetableEvent.Time;
                                    break;
                                }
                            }
                        }
                        if (encounterStart == long.MaxValue)
                        {
                            continue;
                        }
                        // Find deimos or remain on dummy
                        SingleActor target = deimosDummy;
                        if (targetsByIDs.TryGetValue((int)TargetID.Deimos, out var deimoss))
                        {
                            var deimos = deimoss.FirstOrDefault(x => x.FirstAware > encounterStart || x.AgentItem.InAwareTimes(encounterStart));
                            if (deimos != null)
                            {
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
                                    var targetableEvent = attackTargetEvent.GetTargetableEvents(log).FirstOrDefault(x => x.Time >= encounterStart + 5000);
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
                                encounterEnd = Math.Max(prides.Max(x => x.LastAware), encounterEnd);
                            }
                            var greeds = log.AgentData.GetNPCsByID(TargetID.Greed);
                            if (greeds.Any())
                            {
                                encounterEnd = Math.Max(greeds.Max(x => x.LastAware), encounterEnd);
                            }
                        }
                        if (encounterEnd == long.MinValue)
                        {
                            continue;
                        }
                        bool success = false;
                        if (chest != null && chest.InAwareTimes(encounterStart, encounterEnd + 2000))
                        {
                            encounterEnd = chest.FirstAware;
                            success = true;
                        }
                        var phase = new PhaseData(encounterStart, encounterEnd, "Deimos");
                        phases.Add(phase);
                        encounterPhases.Add(phase);
                        if (target.IsSpecies(TargetID.Deimos))
                        {
                            mainPhase.AddTarget(target, log);
                            if (target.GetHealth(log.CombatData) > 40e6)
                            {
                                phase.Name += " CM";
                            }
                        }
                        if (success)
                        {
                            phase.Name += " (Success)";
                        }
                        else
                        {
                            phase.Name += " (Failure)";
                        }
                        phase.AddParentPhase(mainPhase);
                        phase.AddTarget(target, log);
                        phase.AddTargets(demonicBonds, log, PhaseData.TargetPriority.Blocking);
                        AddTargetsToPhase(phase, [TargetID.Thief, TargetID.Gambler, TargetID.Drunkard], log, PhaseData.TargetPriority.NonBlocking);
                        encounterStartThreshold = encounterEnd;
                    }
                }
            }
        }
        NumericallyRenamePhases(encounterPhases);
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        var targetsByIDs = Targets.GroupBy(x => x.ID).ToDictionary(x => x.Key, x => x.ToList());
        HandleCairnPhases(targetsByIDs, log, phases);
        ProcessGenericEncounterPhasesForInstance(targetsByIDs, log, phases, TargetID.MursaatOverseer, [], ChestID.RecreationRoomChest, "Mursaat Overseer", (log, mursaat) => mursaat.GetHealth(log.CombatData) > 25e6);
        ProcessGenericEncounterPhasesForInstance(targetsByIDs, log, phases, TargetID.Samarog, Targets.Where(x => x.IsAnySpecies([TargetID.Guldhem, TargetID.Rigom])), ChestID.SamarogChest, "Samarog", (log, samarog) => samarog.GetHealth(log.CombatData) > 30e6);
        HandleDeimosPhases(targetsByIDs, log, phases);
        if (phases[0].Targets.Count == 0)
        {
            phases[0].AddTarget(Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Instance)), log);
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
        targets.Add(TargetID.Instance);
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

    private void HandleDeimosAndItsGadgets(FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
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
            (AgentItem? deimosStructBody, HashSet<AgentItem> gadgetAgents, long deimos10PercentTargetable, long notTargetable) = Deimos.FindDeimos10PercentBodyStructWithAttackTargets(deimos, fightData, agentData, combatData, attackTargetEvents, targetableEvents);
            Deimos.HandleDeimosAndItsGadgets(deimos, deimosStructBody, gadgetAgents, agentData, combatData, extensions, deimos10PercentTargetable, notTargetable + 1000);
        }
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        Samarog.HandleSpears(evtcVersion, agentData, combatData);
        Deimos.HandleDemonicBonds(agentData, combatData);
        Deimos.HandleShackledPrisoners(agentData, combatData);
        agentData.AddCustomNPCAgent(fightData.FightStart, fightData.FightEnd, "Deimos Pre Event", Spec.NPC, TargetID.DummyTarget, true);
        base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
        Deimos.RenameTargetSauls(Targets);
        HandleDeimosAndItsGadgets(fightData, agentData, combatData, extensions);
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
    internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
    {
       foreach (var deimos in Targets.Where(x => x.IsSpecies(TargetID.Deimos)))
        {
            Deimos.AdjustDeimosHP(deimos, deimos.GetHealth(combatData) > 40e6);
        }
        return base.GetEncounterMode(combatData, agentData, fightData);
    }
}
