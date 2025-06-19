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

    private static void HandleCairnPhases(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, ParsedEvtcLog log, List<PhaseData> phases)
    {
        if (targetsByIDs.TryGetValue((int)TargetID.Cairn, out var cairns))
        {
            bool hasMultiple = cairns.Count > 1;
            int encounterCount = 1;
            var lastTarget = cairns.Last();
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
                if (cairn == lastTarget && chest != null)
                {
                    end = chest.FirstAware;
                    success = true;
                }
                var phase = new PhaseData(start, end, "Cairn");
                phases.Add(phase);
                if (log.CombatData.GetBuffApplyData(SkillIDs.Countdown).Any(x => x.Time >= cairn.FirstAware && x.Time <= cairn.LastAware))
                {
                    phase.Name += " CM";
                }
                if (hasMultiple)
                {
                    phase.Name += " " + (encounterCount++);
                }
                if (success)
                {
                    phase.Name += " (Success)";
                }
                else
                {
                    phase.Name += " (Failure)";
                }
                phase.AddParentPhase(phases[0]);
                phase.AddTarget(cairn, log);
                phases[0].AddTarget(cairn, log);
            }
        }
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        var targetsByIDs = Targets.GroupBy(x => x.ID).ToDictionary(x => x.Key, x => x.ToList());
        HandleCairnPhases(targetsByIDs, log, phases);
        ProcessGenericEncounterPhasesForInstance(targetsByIDs, log, phases, TargetID.MursaatOverseer, [], ChestID.RecreationRoomChest, "Mursaat Overseer", (log, mursaat) => mursaat.GetHealth(log.CombatData) > 25e6);
        ProcessGenericEncounterPhasesForInstance(targetsByIDs, log, phases, TargetID.Samarog, Targets.Where(x => x.IsAnySpecies([TargetID.Guldhem, TargetID.Rigom])), ChestID.SamarogChest, "Samarog", (log, samarog) => samarog.GetHealth(log.CombatData) > 30e6);
        if (phases[0].Targets.Count == 0)
        {
            phases[0].AddTarget(Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Instance)), log);
        }
        return phases;
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
        var targetableEvents = combatData.Where(x => x.IsStateChange == StateChange.Targetable).Select(x => new TargetableEvent(x, agentData)).Where(x => attackTargetEvents.Any(y => y.AttackTarget == x.Src));
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

    internal override List<HealthDamageEvent> SpecialDamageEventProcess(CombatData combatData, SkillData skillData)
    {
        var res = new List<HealthDamageEvent>();
        foreach (var subLogic in _subLogics)
        {
            res.AddRange(subLogic.SpecialDamageEventProcess(combatData, skillData));
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
        return FightData.EncounterMode.NotApplicable;
    }
}
