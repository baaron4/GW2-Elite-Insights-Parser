using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class SpiritValeInstance : SpiritVale
{
    private readonly ValeGuardian _valeGuardian;
    private readonly SpiritRace _spiritRace;
    private readonly Gorseval _gorseval;
    private readonly Sabetha _sabetha;

    private readonly IReadOnlyList<SpiritVale> _subLogics;

    public SpiritValeInstance(int triggerID) : base(triggerID)
    {
        EncounterID = EncounterIDs.EncounterMasks.Unsupported;
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

    private void ProcessSpiritRacePhases(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, ParsedEvtcLog log, List<PhaseData> phases)
    {
        if (targetsByIDs.TryGetValue((int)TargetID.EtherealBarrier, out var etherealBarriers))
        {
            bool hasMultiple = etherealBarriers.Count > 4;
            int encounterCount = 1;
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

                RewardEvent? reward = SpiritRace.GetRewardEvent(log.CombatData, start, end + 5000);

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
                var phase = new PhaseData(start, end, "Spirit Race");
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
                phases.Add(phase);
                phase.AddTargets(etherealBarrierPack, log);
                phase.AddParentPhase(phases[0]);
            }
        }
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        var targetsByIDs = Targets.GroupBy(x => x.ID).ToDictionary(x => x.Key, x => x.ToList());
        ProcessGenericCombatPhasesForInstance(targetsByIDs, log, phases, TargetID.ValeGuardian, Targets.Where(x => x.IsAnySpecies([TargetID.RedGuardian, TargetID.BlueGuardian, TargetID.GreenGuardian])), ChestID.GuardianChest, "Vale Guardian");
        // To be tested, gadgets
        //ProcessSpiritRacePhases(targetsByIDs, log, phases);
        ProcessGenericCombatPhasesForInstance(targetsByIDs, log, phases, TargetID.Gorseval, Targets.Where(x => x.IsSpecies(TargetID.ChargedSoul)), ChestID.GorsevalChest, "Gorseval");
        ProcessGenericCombatPhasesForInstance(targetsByIDs, log, phases, TargetID.Sabetha, Targets.Where(x => x.IsAnySpecies([TargetID.Karde, TargetID.Knuckles, TargetID.Kernan])), ChestID.SabethaChest, "Sabetha");
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

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        SpiritRace.FindEtherealBarriers(agentData, combatData);
        Sabetha.FindCannonsAndHeavyBombs(agentData, combatData);
        base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
        SpiritRace.RenameEtherealBarriersAndOverrideID(Targets, agentData);
        Gorseval.RenameChargedSouls(Targets, combatData);
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
    internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
    {
        return FightData.EncounterMode.NotApplicable;
    }
}
