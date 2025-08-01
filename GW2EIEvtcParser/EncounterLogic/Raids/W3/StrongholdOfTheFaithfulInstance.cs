﻿using System.Numerics;
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

internal class StrongholdOfTheFaithfulInstance : StrongholdOfTheFaithful
{
    private readonly Escort _escort;
    private readonly KeepConstruct _keepConstruct;
    private readonly TwistedCastle _twistedCastle;
    private readonly Xera _xera;

    private readonly IReadOnlyList<StrongholdOfTheFaithful> _subLogics;
    public StrongholdOfTheFaithfulInstance(int triggerID) : base(triggerID)
    {
        EncounterID = EncounterIDs.EncounterMasks.Unsupported;
        Icon = InstanceIconStrongholdOfTheFaithful;
        Extension = "strgldfaith";

        _escort = new Escort((int)TargetID.McLeodTheSilent);
        _keepConstruct = new KeepConstruct((int)TargetID.KeepConstruct);
        _twistedCastle = new TwistedCastle((int)TargetID.DummyTarget);
        _xera = new Xera((int)TargetID.Xera);
        _subLogics = [_escort, _keepConstruct, _twistedCastle, _xera];

        MechanicList.Add(_escort.Mechanics);
        MechanicList.Add(_keepConstruct.Mechanics);
        MechanicList.Add(_twistedCastle.Mechanics);
        MechanicList.Add(_xera.Mechanics);
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData)
    {
        return "Stronghold Of The Faithful";
    }
    private static void HandleEscortPhases(IReadOnlyList<SingleActor> targets, IReadOnlyList<SingleActor> glennas, ParsedEvtcLog log, List<PhaseData> phases)
    {
        var encounterPhases = new List<PhaseData>();
        var mainPhase = phases[0];
        var mcLeods = targets.Where(x => x.IsSpecies(TargetID.McLeodTheSilent));
        var dummy = targets.FirstOrDefault(x => x.IsSpecies(TargetID.DummyTarget) && x.Character == "Escort");
        var subMcLeods = targets.Where(x => x.IsAnySpecies([TargetID.CrimsonMcLeod, TargetID.RadiantMcLeod]));
        List<AbstractBuffApplyEvent> surveilledAppliesPerGlenna = glennas.Select(x => log.CombatData.GetBuffApplyDataByIDByDst(SkillIDs.EscortSurveilled, x.AgentItem).FirstOrDefault()).Where(x => x != null).ToList()!;
        foreach (var surveilledApply in surveilledAppliesPerGlenna)
        {
            var glenna = surveilledApply.To;
            var chest = log.AgentData.GetGadgetsByID(ChestID.SiegeChest).FirstOrDefault();
            if (surveilledApply != null)
            {
                long start = surveilledApply.Time;
                long end = glenna.LastAware;
                bool success = false;
                if (chest != null && chest.FirstAware >= glenna.FirstAware && chest.FirstAware <= glenna.LastAware + 5000)
                {
                    end = chest!.FirstAware;
                    success = true;
                }
                var phase = AddInstanceEncounterPhase(log, phases, encounterPhases, mcLeods, subMcLeods, [], mainPhase, "Siege the Stronghold", start, end, success, false);
                if (phase.Targets.Count == 0)
                {
                    phase.AddTarget(dummy, log);
                }
            }
        }
        mainPhase.AddTargets(mcLeods, log);
        NumericallyRenamePhases(encounterPhases);
    }

    private static void HandleTwistedCastlePhases(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, ParsedEvtcLog log, List<PhaseData> phases)
    {
        var encounterPhases = new List<PhaseData>();
        if (targetsByIDs.TryGetValue((int)TargetID.HauntingStatue, out var statues))
        {
            var dummy = targetsByIDs[(int)TargetID.DummyTarget].FirstOrDefault(x => x.Character == "Twisted Castle");
            if (dummy == null)
            {
                return;
            }
            var mainPhase = phases[0];
            var packedStatus = new List<List<SingleActor>>();
            var currentPack = new List<SingleActor>();
            SingleActor? prevStatue = null;
            foreach (var statue in statues)
            {
                if (prevStatue == null)
                {
                    currentPack.Add(statue);
                } 
                else
                {
                    if (statue.FirstAware > prevStatue.LastAware)
                    {
                        packedStatus.Add(currentPack);
                        currentPack = new List<SingleActor>();
                    }
                    currentPack.Add(statue);
                }
                prevStatue = statue;
            }
            packedStatus.Add(currentPack);
            foreach (var statuePack in packedStatus)
            {
                long start = long.MaxValue;
                foreach (var statue in statuePack)
                {
                    EnterCombatEvent? enterCombat = log.CombatData.GetEnterCombatEvents(statue.AgentItem).FirstOrDefault();
                    if (enterCombat != null)
                    {
                        start = Math.Min(start, enterCombat.Time);
                    }
                }
                if (start == long.MaxValue)
                {
                    continue;
                }
                long end = statuePack.Max(x => x.LastAware);
                RewardEvent? reward = GetOldRaidReward2Event(log.CombatData, start, end + 5000);
                var success = false;
                if (reward != null)
                {
                    success = true;
                    end = reward.Time;
                }
                AddInstanceEncounterPhase(log, phases, encounterPhases, [dummy], [], [], mainPhase, "Twisted Castle", start, end, success, false);
            }
        }
        NumericallyRenamePhases(encounterPhases);
    }
    private static void HandleXeraPhases(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, ParsedEvtcLog log, List<PhaseData> phases)
    {
        var dummy = targetsByIDs[(int)TargetID.DummyTarget].FirstOrDefault(x => x.Character == "Xera Pre Event");
        if (dummy == null)
        {
            return;
        }
        var encounterPhases = new List<PhaseData>();
        var mainPhase = phases[0];
        var fakeXeras = log.AgentData.GetNPCsByID(TargetID.FakeXera);
        var xeras = log.AgentData.GetNPCsByID(TargetID.Xera);
        var chest = log.AgentData.GetGadgetsByID(ChestID.XeraChest).FirstOrDefault();
        for (int i = 0; i < fakeXeras.Count; i++) 
        {
            var fakeXera = fakeXeras[i];
            AgentItem? xera = null;
            if (i < fakeXeras.Count - 1)
            {
                xera = xeras.FirstOrDefault(x => x.FirstAware >= fakeXera.LastAware && x.FirstAware < fakeXeras[i + 1].FirstAware );
            } 
            else
            {
                xera = xeras.FirstOrDefault(x => x.FirstAware >= fakeXera.LastAware);
            }
            long start = fakeXera.LastAware;
            DeadEvent? death = log.CombatData.GetDeadEvents(fakeXera).LastOrDefault();
            if (death != null)
            {
                start = death.Time + 1000;
            }
            else
            {
                ExitCombatEvent? exitCombat = log.CombatData.GetExitCombatEvents(fakeXera).LastOrDefault();
                if (exitCombat != null)
                {
                    start = exitCombat.Time + 1000;
                }
            }
            long end;
            bool success = false;
            if (xera != null)
            {
                end = xera.LastAware;
                if (chest != null && chest.FirstAware >= xera.FirstAware && chest.FirstAware <= xera.LastAware)
                {
                    success = true;
                    end = chest.FirstAware;
                }
            } 
            else
            {
                if (i < fakeXeras.Count - 1)
                {
                    end = fakeXeras[i + 1].FirstAware - 500;
                } 
                else
                {
                    end = log.FightData.FightEnd;
                }
            }
            AddInstanceEncounterPhase(log, phases, encounterPhases, [xera != null ? log.FindActor(xera) : dummy], [], [], mainPhase, "Xera", start, end, success, false);
        }
        NumericallyRenamePhases(encounterPhases);
    }
    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        var targetsByIDs = Targets.GroupBy(x => x.ID).ToDictionary(x => x.Key, x => x.ToList());
        HandleEscortPhases(Targets, NonSquadFriendlies.Where(x => x.IsSpecies(TargetID.Glenna)).ToList(), log, phases);
        HashSet<TargetID> kcStatus = [
            TargetID.Jessica,
            TargetID.Olson,
            TargetID.Engul,
            TargetID.Faerla,
            TargetID.Caulle,
            TargetID.Henley,
            TargetID.Galletta,
            TargetID.Ianim
        ];
        ProcessGenericEncounterPhasesForInstance(targetsByIDs, log, phases, TargetID.KeepConstruct, Targets.Where(x => x.IsAnySpecies(kcStatus)), ChestID.KeepConstructChest, "Keep Construct", (log, kc) => log.CombatData.GetBuffApplyData(SkillIDs.AchievementEligibilityDownDownDowned).Any(x => x.Time >= kc.FirstAware && x.Time <= kc.LastAware));
        HandleTwistedCastlePhases(targetsByIDs, log, phases);
        HandleXeraPhases(targetsByIDs, log, phases);
        return phases;
    }

    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        List<InstantCastFinder> finders = [
            .. _escort.GetInstantCastFinders(),
            .. _keepConstruct.GetInstantCastFinders(),
            .. _twistedCastle.GetInstantCastFinders(),
            .. _xera.GetInstantCastFinders()
        ];
        return finders;
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        List<TargetID> trashes = [
            .. _escort.GetTrashMobsIDs(),
            .. _keepConstruct.GetTrashMobsIDs(),
            .. _twistedCastle.GetTrashMobsIDs(),
            .. _xera.GetTrashMobsIDs()
        ];
        return trashes.Distinct().ToList();
    }

    internal override IReadOnlyList<TargetID> GetTargetsIDs()
    {
        List<TargetID> targets = [
            .. _escort.GetTargetsIDs(),
            .. _keepConstruct.GetTargetsIDs(),
            .. _twistedCastle.GetTargetsIDs(),
            .. _xera.GetTargetsIDs()
        ];
        return targets.Distinct().ToList();
    }

    internal override IReadOnlyList<TargetID> GetFriendlyNPCIDs()
    {
        List<TargetID> friendlies = [
            .. _escort.GetFriendlyNPCIDs(),
            .. _keepConstruct.GetFriendlyNPCIDs(),
            .. _twistedCastle.GetFriendlyNPCIDs(),
            .. _xera.GetFriendlyNPCIDs()
        ];
        return friendlies.Distinct().ToList();
    }

    private static void MergeXeraAgents(AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        var xeras = agentData.GetNPCsByID(TargetID.Xera);
        var xeras2 = agentData.GetNPCsByID(TargetID.Xera2);
        long start = 0;
        foreach (var xera2 in xeras2)
        {
            var attachedXera = xeras.LastOrDefault(x => x.FirstAware < xera2.FirstAware && x.FirstAware >= start);
            if (attachedXera != null)
            {
                attachedXera.OverrideAwareTimes(attachedXera.FirstAware, xera2.LastAware);
                AgentManipulationHelper.RedirectAllEvents(combatData, extensions, agentData, xera2, attachedXera);
            }
            start = xera2.LastAware;
        }
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        Escort.FindMines(agentData, combatData);
        // For encounters before reaching McLeod
        agentData.AddCustomNPCAgent(fightData.FightStart, fightData.FightEnd, "Escort", Spec.NPC, TargetID.DummyTarget, true);
        agentData.AddCustomNPCAgent(fightData.FightStart, fightData.FightEnd, "Twisted Castle", Spec.NPC, TargetID.DummyTarget, true);
        // For encounters before reaching Xera
        agentData.AddCustomNPCAgent(fightData.FightStart, fightData.FightEnd, "Xera Pre Event", Spec.NPC, TargetID.DummyTarget, true);
        // TODO: verify charged bloodstones, it is possible there is only a single instance of each in the map and they get hidden/put back to 100% hp as they are present from log start to log end in boss logs
        Xera.FindBloodstones(agentData, combatData);
        MergeXeraAgents(agentData, combatData, extensions);
        base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
        Escort.RenameSubMcLeods(Targets);
        Xera.RenameBloodStones(Targets);
        foreach (var target in Targets)
        {
            if (target.IsSpecies(TargetID.Xera))
            {
                Xera.SetManualHPForXera(target);
            }
        }
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
        foreach (StrongholdOfTheFaithful logic in _subLogics)
        {
            logic.ComputeNPCCombatReplayActors(target, log, replay);
        }
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputePlayerCombatReplayActors(p, log, replay);
        foreach (StrongholdOfTheFaithful logic in _subLogics)
        {
            logic.ComputePlayerCombatReplayActors(p, log, replay);
        }
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);
        foreach (StrongholdOfTheFaithful logic in _subLogics)
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
}
