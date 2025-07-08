using System;
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

internal class TheKeyOfAhdashimInstance : TheKeyOfAhdashim
{
    private readonly Adina _adina;
    private readonly Sabir _sabir;
    private readonly PeerlessQadim _peerlessQadim;

    private readonly IReadOnlyList<TheKeyOfAhdashim> _subLogics;
    public TheKeyOfAhdashimInstance(int triggerID) : base(triggerID)
    {
        EncounterID = EncounterIDs.EncounterMasks.Unsupported;
        Icon = InstanceIconTheKeyOfAhdashim;
        Extension = "keyadash"; 
        
        _adina = new Adina((int)TargetID.Adina);
        _sabir = new Sabir((int)TargetID.Sabir);
        _peerlessQadim = new PeerlessQadim((int)TargetID.PeerlessQadim);
        _subLogics = [_adina, _sabir, _peerlessQadim];

        MechanicList.Add(_adina.Mechanics);
        MechanicList.Add(_sabir.Mechanics);
        MechanicList.Add(_peerlessQadim.Mechanics);
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData)
    {
        return "The Key Of Ahdashim";
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        var targetsByIDs = Targets.GroupBy(x => x.ID).ToDictionary(x => x.Key, x => x.ToList());
        ProcessGenericEncounterPhasesForInstance(targetsByIDs, log, phases, TargetID.Adina, [], ChestID.AdinasChest, "Cardinal Adina", (log, adina) => adina.GetHealth(log.CombatData) > 23e6);
        ProcessGenericEncounterPhasesForInstance(targetsByIDs, log, phases, TargetID.Sabir, [], ChestID.SabirsChest, "Cardinal Sabir", (log, sabir) => sabir.GetHealth(log.CombatData) > 32e6);
        ProcessGenericEncounterPhasesForInstance(targetsByIDs, log, phases, TargetID.Sabir, [], ChestID.QadimThePeerlessChest, "Qadim the Peerless", (log, qtp) => qtp.GetHealth(log.CombatData) > 48e6);
        if (phases[0].Targets.Count == 0)
        {
            phases[0].AddTarget(Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Instance)), log);
        }
        return phases;
    }

    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        List<InstantCastFinder> finders = [
            .. _adina.GetInstantCastFinders(),
            .. _sabir.GetInstantCastFinders(),
            .. _peerlessQadim.GetInstantCastFinders()
        ];
        return finders;
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        List<TargetID> trashes = [
            .. _adina.GetTrashMobsIDs(),
            .. _sabir.GetTrashMobsIDs(),
            .. _peerlessQadim.GetTrashMobsIDs()
        ];
        return trashes.Distinct().ToList();
    }
    internal override IReadOnlyList<TargetID> GetTargetsIDs()
    {
        List<TargetID> targets = [
            .. _adina.GetTargetsIDs(),
            .. _sabir.GetTargetsIDs(),
            .. _peerlessQadim.GetTargetsIDs()
        ];
        targets.Add(TargetID.Instance);
        return targets.Distinct().ToList();
    }

    internal override IReadOnlyList<TargetID> GetFriendlyNPCIDs()
    {
        List<TargetID> friendlies = [
            .. _adina.GetFriendlyNPCIDs(),
            .. _sabir.GetFriendlyNPCIDs(),
            .. _peerlessQadim.GetFriendlyNPCIDs()
        ];
        return friendlies.Distinct().ToList();
    }
    protected override HashSet<int> IgnoreForAutoNumericalRenaming()
    {
        return [
            (int)TargetID.HandOfErosion,
            (int)TargetID.HandOfEruption,
            (int)TargetID.PeerlessQadimPylon,
        ];
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        Adina.FindPlatforms(agentData);
        Adina.FindHands(fightData, agentData, combatData, extensions);
        Sabir.FindPlateforms(agentData);
        base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
        Adina.RenameHands(Targets, combatData);
        PeerlessQadim.RenamePylons(Targets, combatData);
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
}
