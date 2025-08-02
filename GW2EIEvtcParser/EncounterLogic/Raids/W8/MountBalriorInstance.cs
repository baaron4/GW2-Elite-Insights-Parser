using System;
using System.Collections.Generic;
using System.Numerics;
using System.Xml.Schema;
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

internal class MountBalriorInstance : MountBalrior
{
    private readonly GreerTheBlightbringer _greer;
    private readonly DecimaTheStormsinger _decima;
    private readonly UraTheSteamshrieker _ura;

    private readonly IReadOnlyList<MountBalrior> _subLogics;
    public MountBalriorInstance(int triggerID) : base(triggerID)
    {
        EncounterID = EncounterIDs.EncounterMasks.Unsupported;
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

    private static void HandleGreerPhases(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, ParsedEvtcLog log, List<PhaseData> phases)
    {
        var encounterPhases = new List<PhaseData>();
        var mainPhase = phases[0];
        if (targetsByIDs.TryGetValue((int)TargetID.Greer, out var greers))
        {
            var chest = log.AgentData.GetGadgetsByID(ChestID.GreersChest).FirstOrDefault();
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
                var name = isCM ? "Godspoil Greer" : "Greer, the Blightbringer";
                greer.OverrideName(name);
                AddInstanceEncounterPhase(log, phases, encounterPhases, [greer], [], [], mainPhase, name, start, end, success, false);
            }
        }
        NumericallyRenamePhases(encounterPhases);
    }

    private static void HandleDecimaPhases(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, ParsedEvtcLog log, List<PhaseData> phases, TargetID decimaID)
    {
        var encounterPhases = new List<PhaseData>();
        var mainPhase = phases[0];
        if (targetsByIDs.TryGetValue((int)decimaID, out var decimas))
        {
            var chest = log.AgentData.GetGadgetsByID(ChestID.DecimasChest).FirstOrDefault();
            foreach (var decima in decimas)
            {
                long start = decima.FirstAware;
                var determinedBuffs = log.CombatData.GetBuffDataByIDByDst(SkillIDs.Determined762, decima.AgentItem);
                var determinedLost = determinedBuffs.OfType<BuffRemoveAllEvent>().FirstOrDefault();
                var determinedApply = determinedBuffs.OfType<BuffApplyEvent>().FirstOrDefault();
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
                if (targetsByIDs.TryGetValue((int)TargetID.TranscendentBoulder, out var boulders))
                {
                    AddInstanceEncounterPhase(log, phases, encounterPhases, [decima], boulders, [], mainPhase, name, start, end, success, false);
                } 
                else
                {
                    AddInstanceEncounterPhase(log, phases, encounterPhases, [decima], [], [], mainPhase, name, start, end, success, false);
                }
            }
        }
        NumericallyRenamePhases(encounterPhases);
    }

    private static void HandleUraPhases(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, ParsedEvtcLog log, List<PhaseData> phases)
    {
        var encounterPhases = new List<PhaseData>();
        var mainPhase = phases[0];
        if (targetsByIDs.TryGetValue((int)TargetID.Ura, out var uras))
        {
            long encounterThreshold = log.FightData.LogStart;
            var deterrences = log.CombatData.GetBuffData(SkillIDs.Deterrence).Where(x => x is BuffApplyEvent || x is BuffRemoveAllEvent);
            var chest = log.AgentData.GetGadgetsByID(ChestID.UrasChest).FirstOrDefault();
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
                foreach (var deterrence in deterrences)
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
                var name = isCM ? "Godscream Ura" : "Ura, the Steamshrieker";
                ura.OverrideName(name);
                AddInstanceEncounterPhase(log, phases, encounterPhases, [ura], [], [], mainPhase, name, start, end, success, false, maxHP > 100e6);
            }
        }
        NumericallyRenamePhases(encounterPhases);
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        var targetsByIDs = Targets.GroupBy(x => x.ID).ToDictionary(x => x.Key, x => x.ToList());
        HandleGreerPhases(targetsByIDs, log, phases);
        HandleDecimaPhases(targetsByIDs, log, phases, TargetID.Decima);
        HandleDecimaPhases(targetsByIDs, log, phases, TargetID.DecimaCM);
        HandleUraPhases(targetsByIDs, log, phases);
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

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        DecimaTheStormsinger.FindConduits(agentData, combatData);
        UraTheSteamshrieker.FindGeysers(evtcVersion, agentData, combatData);
        UraTheSteamshrieker.FindBloodstoneShards(evtcVersion, agentData, combatData);
        base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
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
