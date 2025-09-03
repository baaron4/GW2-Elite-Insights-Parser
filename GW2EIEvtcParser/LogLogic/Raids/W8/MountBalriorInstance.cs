using System;
using System.Collections.Generic;
using System.Numerics;
using System.Xml.Schema;
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

internal class MountBalriorInstance : MountBalrior
{
    private readonly GreerTheBlightbringer _greer;
    private readonly DecimaTheStormsinger _decima;
    private readonly UraTheSteamshrieker _ura;

    private readonly IReadOnlyList<MountBalrior> _subLogics;
    public MountBalriorInstance(int triggerID) : base(triggerID)
    {
        LogID = LogIDs.LogMasks.Unsupported;
        Icon = InstanceIconMountBalrior;
        Extension = "mntbalr";

        _greer = new GreerTheBlightbringer((int)TargetID.Greer);
        _decima = new DecimaTheStormsinger((int)TargetID.Decima);
        _ura = new UraTheSteamshrieker((int)TargetID.Ura);
        _subLogics = [_greer, _decima, _ura];

        MechanicList.Add(_greer.Mechanics);
        MechanicList.Add(_decima.Mechanics);
        MechanicList.Add(_ura.Mechanics);
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData)
    {
        return "Mount Balrior";
    }
    internal override void CheckSuccess(CombatData combatData, AgentData agentData, LogData logData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        var chest = agentData.GetGadgetsByID(_ura.ChestID).FirstOrDefault();
        if (chest != null)
        {
            logData.SetSuccess(true, chest.FirstAware);
            return;
        }
        base.CheckSuccess(combatData, agentData, logData, playerAgents);
    }

    private List<EncounterPhaseData> HandleGreerPhases(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, ParsedEvtcLog log, List<PhaseData> phases)
    {
        var encounterPhases = new List<EncounterPhaseData>();
        var mainPhase = phases[0];
        if (targetsByIDs.TryGetValue((int)TargetID.Greer, out var greers))
        {
            var chest = log.AgentData.GetGadgetsByID(_greer.ChestID).FirstOrDefault();
            var greeAndRegs = Targets.Where(x => x.IsAnySpecies([TargetID.Reeg, TargetID.Gree]));
            var protoGreelings = Targets.Where(x => x.IsSpecies(TargetID.ProtoGreerling));
            var eregs = Targets.Where(x => x.IsSpecies(TargetID.Ereg));
            foreach (var greer in greers)
            {
                // TBC
                var enterCombat = log.CombatData.GetEnterCombatEvents(greer.AgentItem).FirstOrDefault();
                if (enterCombat == null && !log.CombatData.GetDamageTakenData(greer.AgentItem).Any(x => x.HealthDamage > 0 && x.CreditedFrom.IsPlayer))
                {
                    continue;
                }
                long start = enterCombat != null ? enterCombat.Time : greer.FirstAware;
                bool success = false;
                long end = greer.LastAware;
                if (chest != null && chest.InAwareTimes(greer.LastAware + 500))
                {
                    end = chest.FirstAware;
                    success = true;
                }
                var isCM = greer.GetHealth(log.CombatData) > 35e6;
                var name = (isCM ? "Godspoil Greer " : "Greer, the Blightbringer ") + greer.Character.Last();
                greer.OverrideName(name);
                AddInstanceEncounterPhase(log, phases, encounterPhases, [greer], [..greeAndRegs, ..protoGreelings], eregs, mainPhase, name, start, end, success, _greer, isCM ? LogData.LogMode.CMNoName : LogData.LogMode.Normal);
            }
        }
        NumericallyRenameEncounterPhases(encounterPhases);
        return encounterPhases;
    }

    private List<EncounterPhaseData> HandleDecimaPhases(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, ParsedEvtcLog log, List<PhaseData> phases, TargetID decimaID)
    {
        var encounterPhases = new List<EncounterPhaseData>();
        var mainPhase = phases[0];
        if (targetsByIDs.TryGetValue((int)decimaID, out var decimas))
        {
            var chest = log.AgentData.GetGadgetsByID(_decima.ChestID).FirstOrDefault();
            foreach (var decima in decimas)
            {
                long start = decima.FirstAware;
                var determinedBuffs = log.CombatData.GetBuffDataByIDByDst(SkillIDs.Determined762, decima.AgentItem);
                var determinedLost = determinedBuffs.FirstOrDefault(x => x is BuffRemoveAllEvent);
                var determinedApply = determinedBuffs.FirstOrDefault(x => x is BuffApplyEvent);
                var enterCombat = log.CombatData.GetEnterCombatEvents(decima.AgentItem).FirstOrDefault();
                if (determinedLost != null && enterCombat != null && enterCombat.Time >= determinedLost.Time)
                {
                    start = determinedLost.Time;
                } 
                else if (determinedApply != null)
                {
                    continue;
                }
                bool success = false;
                long end = decima.LastAware;
                if (chest != null && chest.InAwareTimes(decima.LastAware + 500))
                {
                    end = chest.FirstAware;
                    success = true;
                }
                var isCM = decimaID == TargetID.DecimaCM;
                var name = isCM ? "Godsqual Decima" : "Decima, the Stormsinger";
                var mode = isCM ? LogData.LogMode.CMNoName : LogData.LogMode.Normal;
                if (targetsByIDs.TryGetValue((int)TargetID.TranscendentBoulder, out var boulders))
                {
                    AddInstanceEncounterPhase(log, phases, encounterPhases, [decima], boulders, [], mainPhase, name, start, end, success, _decima, mode);
                } 
                else
                {
                    AddInstanceEncounterPhase(log, phases, encounterPhases, [decima], [], [], mainPhase, name, start, end, success, _decima, mode);
                }
            }
        }
        NumericallyRenameEncounterPhases(encounterPhases);
        return encounterPhases;
    }

    private List<EncounterPhaseData> HandleUraPhases(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, ParsedEvtcLog log, List<PhaseData> phases)
    {
        var encounterPhases = new List<EncounterPhaseData>();
        var mainPhase = phases[0];
        if (targetsByIDs.TryGetValue((int)TargetID.Ura, out var uras))
        {
            long encounterThreshold = log.LogData.EvtcLogStart;
            var deterrences = log.CombatData.GetBuffData(SkillIDs.Deterrence).Where(x => x is BuffApplyEvent || x is BuffRemoveAllEvent);
            var chest = log.AgentData.GetGadgetsByID(_ura.ChestID).FirstOrDefault();
            foreach (var ura in uras)
            {
                var enterCombat = log.CombatData.GetEnterCombatEvents(ura.AgentItem).FirstOrDefault();
                if (enterCombat == null && !log.CombatData.GetDamageTakenData(ura.AgentItem).Any(x => x.HealthDamage > 0 && x.CreditedFrom.IsPlayer))
                {
                    encounterThreshold = ura.LastAware;
                    continue;
                }
                var deterrencesToConsider = deterrences.Where(x => x.Time > encounterThreshold && x.Time < ura.LastAware);
                var activeDeterrences = new Dictionary<AgentItem, long>();
                foreach (var deterrence in deterrencesToConsider)
                {
                    if (deterrence is BuffApplyEvent)
                    {
                        activeDeterrences[deterrence.To] = deterrence.Time;
                    }
                    else
                    {
                        activeDeterrences.Remove(deterrence.To);
                    }
                    if (activeDeterrences.Count == 2)
                    {
                        break;
                    }
                }
                long start = activeDeterrences.Count == 2 ? activeDeterrences.Values.Max() : (enterCombat != null ? enterCombat.Time : ura.FirstAware);
                bool success = false;
                long end = ura.LastAware;
                if (chest != null && chest.InAwareTimes(ura.LastAware + 500))
                {
                    end = chest.FirstAware;
                    success = true;
                }
                encounterThreshold = end;
                var maxHP = ura.GetHealth(log.CombatData);
                var isCM = maxHP > 70e6;
                var name = (isCM ? "Godscream Ura " : "Ura, the Steamshrieker ") + ura.Character.Last();
                ura.OverrideName(name);
                AddInstanceEncounterPhase(log, phases, encounterPhases, [ura], [], [], mainPhase, name, start, end, success, _ura, isCM ? (maxHP > 100e6 ? LogData.LogMode.LegendaryCM : LogData.LogMode.CMNoName) : LogData.LogMode.Normal);
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
            var greerPhases = HandleGreerPhases(targetsByIDs, log, phases);
            foreach (var greerPhase in greerPhases)
            {
                var greer = greerPhase.Targets.Keys.First(x => x.IsSpecies(TargetID.Greer));
                var greeAndReeg = greerPhase.Targets.Keys.Where(x => x.IsAnySpecies([TargetID.Gree, TargetID.Reeg]));
                var ereg = greerPhase.Targets.Keys.FirstOrDefault(x => x.IsSpecies(TargetID.Ereg));
                var protoGreerlings = greerPhase.Targets.Keys.Where(x => x.IsSpecies(TargetID.ProtoGreerling));
                phases.AddRange(GreerTheBlightbringer.ComputePhases(log, greer, greeAndReeg, ereg, protoGreerlings, greerPhase, requirePhases));
            }
        }
        {
            var decimaPhases = HandleDecimaPhases(targetsByIDs, log, phases, TargetID.Decima);
            decimaPhases.AddRange(HandleDecimaPhases(targetsByIDs, log, phases, TargetID.DecimaCM));
            foreach (var decimaPhase in decimaPhases)
            {
                var decima = decimaPhase.Targets.Keys.First(x => x.IsAnySpecies([TargetID.Decima, TargetID.DecimaCM]));
                phases.AddRange(DecimaTheStormsinger.ComputePhases(log, decima, Targets, decimaPhase, requirePhases));
            }
        }
        {
            var uraPhases = HandleUraPhases(targetsByIDs, log, phases);
            foreach (var uraPhase in uraPhases)
            {
                var ura = uraPhase.Targets.Keys.First(x => x.IsSpecies(TargetID.Ura));
                phases.AddRange(UraTheSteamshrieker.ComputePhases(log, ura, uraPhase, requirePhases));
            }
        }
        return phases;
    }

    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        List<InstantCastFinder> finders = [
            .. _greer.GetInstantCastFinders(),
            .. _decima.GetInstantCastFinders(),
            .. _ura.GetInstantCastFinders()
        ];
        return finders;
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        List<TargetID> trashes = [
            .. _greer.GetTrashMobsIDs(),
            .. _decima.GetTrashMobsIDs(),
            .. _ura.GetTrashMobsIDs()
        ];
        return trashes.Distinct().ToList();
    }
    internal override IReadOnlyList<TargetID> GetTargetsIDs()
    {
        List<TargetID> targets = [
            .. _greer.GetTargetsIDs(),
            .. _decima.GetTargetsIDs(),
            .. _ura.GetTargetsIDs()
        ];
        return targets.Distinct().ToList();
    }

    internal override IReadOnlyList<TargetID> GetFriendlyNPCIDs()
    {
        List<TargetID> friendlies = [
            .. _greer.GetFriendlyNPCIDs(),
            .. _decima.GetFriendlyNPCIDs(),
            .. _ura.GetFriendlyNPCIDs()
        ];
        return friendlies.Distinct().ToList();
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        DecimaTheStormsinger.FindConduits(agentData, combatData);
        UraTheSteamshrieker.FindGeysers(evtcVersion, agentData, combatData);
        UraTheSteamshrieker.FindBloodstoneShards(evtcVersion, agentData, combatData);
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);
        GreerTheBlightbringer.RenameProtoGreerlings(Targets);
        UraTheSteamshrieker.RenameFumarollers(Targets);
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
