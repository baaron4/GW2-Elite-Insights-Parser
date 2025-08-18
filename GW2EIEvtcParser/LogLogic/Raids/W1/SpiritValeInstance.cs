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

internal class SpiritValeInstance : SpiritVale
{
    private readonly ValeGuardian _valeGuardian;
    private readonly SpiritRace _spiritRace;
    private readonly Gorseval _gorseval;
    private readonly Sabetha _sabetha;

    private readonly IReadOnlyList<SpiritVale> _subLogics;

    public SpiritValeInstance(int triggerID) : base(triggerID)
    {
        LogID = LogIDs.LogMasks.Unsupported;
        Icon = InstanceIconSpiritVale;
        Extension = "sprtvale";

        _valeGuardian = new ValeGuardian((int)TargetID.ValeGuardian);
        _spiritRace = new SpiritRace((int)TargetID.EtherealBarrier);
        _gorseval = new Gorseval((int)TargetID.Gorseval);
        _sabetha = new Sabetha((int)TargetID.Sabetha);
        _subLogics = [_valeGuardian, _spiritRace, _gorseval, _sabetha];

        MechanicList.Add(_valeGuardian.Mechanics);
        MechanicList.Add(_spiritRace.Mechanics);
        MechanicList.Add(_gorseval.Mechanics);
        MechanicList.Add(_sabetha.Mechanics);
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData)
    {
        return "Spirit Vale";
    }

    private List<PhaseData> ProcessSpiritRacePhases_SingleGadgetInstances(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, ParsedEvtcLog log, List<PhaseData> phases)
    {
        var encounterPhases = new List<PhaseData>();
        if (targetsByIDs.TryGetValue((int)TargetID.EtherealBarrier, out var etherealBarriers))
        {
            var wallOfGhosts = log.AgentData.GetNPCsByID(TargetID.WallOfGhosts);
            for (int i = 0; i < wallOfGhosts.Count; i++)
            {
                var wallOfGhost = wallOfGhosts[i];
                long nextWallOfGhostStart = log.LogData.LogEnd;
                if (i < wallOfGhosts.Count - 1) 
                {
                    nextWallOfGhostStart = wallOfGhosts[i + 1].FirstAware;
                }
                long start = wallOfGhost.FirstAware;
                foreach (var velocityEvent in log.CombatData.GetMovementData(wallOfGhost).OfType<VelocityEvent>())
                {
                    if (velocityEvent.GetPointXY().Length() > 0)
                    {
                        start = velocityEvent.Time;
                        break;
                    }
                }
                long end = wallOfGhost.LastAware;
                foreach (var etherealBarrier in etherealBarriers)
                {
                    var lastHPUpdate = log.CombatData.GetHealthUpdateEvents(etherealBarrier.AgentItem).LastOrDefault(x => x.Time >= start && x.Time < nextWallOfGhostStart && x.HealthPercent < 100);
                    if (lastHPUpdate != null)
                    {
                        end = Math.Max(lastHPUpdate.Time, end);
                    }
                }
                RewardEvent? reward = GetOldRaidReward2Event(log.CombatData, start, end + 5000);
                bool success = false;
                if (reward != null)
                {
                    end = reward.Time;
                    success = true;
                }
                AddInstanceEncounterPhase(log, phases, encounterPhases, etherealBarriers, [], [], phases[0], "Spirit Race", start, end, success, _spiritRace);
            }
        }
        NumericallyRenameEncounterPhases(encounterPhases);
        return encounterPhases;
    }

    private List<PhaseData> ProcessSpiritRacePhases(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, ParsedEvtcLog log, List<PhaseData> phases)
    {
        var encounterPhases = new List<PhaseData>();
        if (targetsByIDs.TryGetValue((int)TargetID.EtherealBarrier, out var etherealBarriers))
        {
            var packedEtherealBarriers = new List<List<SingleActor>>(etherealBarriers.Count / 4 + 1);
            var currentPack = new List<SingleActor>();
            packedEtherealBarriers.Add(currentPack);
            foreach (var etherealBarrier in etherealBarriers)
            {
                if (currentPack.Count == 0)
                {
                    currentPack.Add(etherealBarrier);
                }
                else
                {
                    var firstOfPack = currentPack.First();
                    if (!(firstOfPack.FirstAware <= etherealBarrier.FirstAware && firstOfPack.LastAware >= etherealBarrier.FirstAware))
                    {
                        currentPack = new List<SingleActor>();
                        packedEtherealBarriers.Add(currentPack);
                    }
                    currentPack.Add(etherealBarrier);
                }
            }
            var lastPack = packedEtherealBarriers.Last();
            foreach (var etherealBarrierPack in packedEtherealBarriers)
            {
                var minFirstAware = etherealBarrierPack.Min(x => x.FirstAware);
                var maxLastAware = etherealBarrierPack.Max(x => x.LastAware);
                long start = minFirstAware;
                long end = maxLastAware;

                RewardEvent? reward = GetOldRaidReward2Event(log.CombatData, start, end + 5000);

                AgentItem? wallOfGhosts = log.AgentData.GetNPCsByID(TargetID.WallOfGhosts).LastOrDefault(x => x.FirstAware <= minFirstAware + 2000 && x.FirstAware <= maxLastAware);
                if (wallOfGhosts != null)
                {
                    foreach (var velocityEvent in log.CombatData.GetMovementData(wallOfGhosts).OfType<VelocityEvent>())
                    {
                        if (velocityEvent.GetPointXY().Length() > 0)
                        {
                            start = velocityEvent.Time;
                            break;
                        }
                    }
                }

                bool success = false;
                if (reward != null)
                {
                    end = reward.Time;
                    success = true;
                }
                AddInstanceEncounterPhase(log, phases, encounterPhases, etherealBarrierPack, [], [], phases[0], "Spirit Race", start, end, success, _spiritRace);
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
            var valeGuardianPhases = ProcessGenericEncounterPhasesForInstance(targetsByIDs, log, phases, TargetID.ValeGuardian, Targets.Where(x => x.IsAnySpecies([TargetID.RedGuardian, TargetID.BlueGuardian, TargetID.GreenGuardian])), "Vale Guardian", _valeGuardian);
            foreach (var valeGuardianPhase in valeGuardianPhases)
            {
                var valeGuardian = valeGuardianPhase.Targets.Keys.First(x => x.IsSpecies(TargetID.ValeGuardian));
                phases.AddRange(ValeGuardian.ComputePhases(log, valeGuardian, Targets, valeGuardianPhase, requirePhases));
            }
        }
        {
            // To be tested, gadgets
            //ProcessSpiritRacePhases(targetsByIDs, log, phases);
        }
        {
            var gorsevalPhases = ProcessGenericEncounterPhasesForInstance(targetsByIDs, log, phases, TargetID.Gorseval, Targets.Where(x => x.IsSpecies(TargetID.ChargedSoul)), "Gorseval", _gorseval);
            foreach (var gorsevalPhase in gorsevalPhases)
            {
                var gorseval = gorsevalPhase.Targets.Keys.First(x => x.IsSpecies(TargetID.Gorseval));
                phases.AddRange(Gorseval.ComputePhases(log, gorseval, Targets, gorsevalPhase, requirePhases));
            }
        }
        { 
            var sabethaPhases = ProcessGenericEncounterPhasesForInstance(targetsByIDs, log, phases, TargetID.Sabetha, Targets.Where(x => x.IsAnySpecies([TargetID.Karde, TargetID.Knuckles, TargetID.Kernan])), "Sabetha", _sabetha);
            foreach (var sabethaPhase in sabethaPhases)
            {
                var sabetha = sabethaPhase.Targets.Keys.First(x => x.IsSpecies(TargetID.Sabetha));
                phases.AddRange(Sabetha.ComputePhases(log, sabetha, Targets, sabethaPhase, requirePhases));
            }
        }
        return phases;
    }

    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        List<InstantCastFinder> finders = [
            .. _valeGuardian.GetInstantCastFinders(),
            .. _spiritRace.GetInstantCastFinders(),
            .. _gorseval.GetInstantCastFinders(),
            .. _sabetha.GetInstantCastFinders()
        ];
        return finders;
    }
    protected override HashSet<int> IgnoreForAutoNumericalRenaming()
    {
        return [
            (int)TargetID.ChargedSoul
        ];
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        List<TargetID> trashes = [
            .. _valeGuardian.GetTrashMobsIDs(),
            .. _spiritRace.GetTrashMobsIDs(),
            .. _gorseval.GetTrashMobsIDs(),
            .. _sabetha.GetTrashMobsIDs()
        ];
        return trashes.Distinct().ToList();
    }

    internal override IReadOnlyList<TargetID> GetTargetsIDs()
    {
        List<TargetID> targets = [
            .. _valeGuardian.GetTargetsIDs(),
            .. _spiritRace.GetTargetsIDs(),
            .. _gorseval.GetTargetsIDs(),
            .. _sabetha.GetTargetsIDs()
        ];
        return targets.Distinct().ToList();
    }

    internal override IReadOnlyList<TargetID> GetFriendlyNPCIDs()
    {
        List<TargetID> friendlies = [
            .. _valeGuardian.GetFriendlyNPCIDs(),
            .. _spiritRace.GetFriendlyNPCIDs(),
            .. _gorseval.GetFriendlyNPCIDs(),
            .. _sabetha.GetFriendlyNPCIDs()
        ];
        return friendlies.Distinct().ToList();
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        SpiritRace.FindEtherealBarriers(agentData, combatData);
        Sabetha.FindCannonsAndHeavyBombs(agentData, combatData);
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);
        SpiritRace.RenameEtherealBarriersAndOverrideID(Targets, agentData);
        Gorseval.RenameChargedSouls(Targets, combatData);
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
        foreach (SpiritVale logic in _subLogics)
        {
            logic.ComputeNPCCombatReplayActors(target, log, replay);
        }
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputePlayerCombatReplayActors(p, log, replay);
        foreach (SpiritVale logic in _subLogics)
        {
            logic.ComputePlayerCombatReplayActors(p, log, replay);
        }
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);
        foreach (SpiritVale logic in _subLogics)
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
