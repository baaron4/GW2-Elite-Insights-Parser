using System;
using System.Collections.Generic;
using System.Numerics;
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

internal class MythwrightGambitInstance : MythwrightGambit
{
    private readonly ConjuredAmalgamate _conjuredAmalgamate;
    private readonly TwinLargos _twinLargos;
    private readonly Qadim _qadim;

    private readonly IReadOnlyList<MythwrightGambit> _subLogics;
    public MythwrightGambitInstance(int triggerID) : base(triggerID)
    {
        LogID = LogIDs.LogMasks.Unsupported;
        Icon = InstanceIconMythwrightGambit;
        Extension = "mythgamb";

        _conjuredAmalgamate = new ConjuredAmalgamate((int)TargetID.ConjuredAmalgamate);
        _twinLargos = new TwinLargos((int)TargetID.Nikare);
        _qadim = new Qadim((int)TargetID.Qadim);
        _subLogics = [_conjuredAmalgamate, _twinLargos, _qadim];

        MechanicList.Add(_conjuredAmalgamate.Mechanics);
        MechanicList.Add(_twinLargos.Mechanics);
        MechanicList.Add(_qadim.Mechanics);
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData)
    {
        return "Mythwright Gambit";
    }

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, LogData logData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        var chest = agentData.GetGadgetsByID(_qadim.ChestID).FirstOrDefault();
        if (chest != null)
        {
            logData.SetSuccess(true, chest.FirstAware);
            return;
        }
        base.CheckSuccess(combatData, agentData, logData, playerAgents);
    }
    private List<EncounterPhaseData> HandleConjuredAmalgamatePhases(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, ParsedEvtcLog log, List<PhaseData> phases)
    {
        var encounterPhases = new List<EncounterPhaseData>();
        var mainPhase = phases[0];
        if (targetsByIDs.TryGetValue((int)TargetID.ConjuredAmalgamate, out var conjuredAmalgamates) && 
            targetsByIDs.TryGetValue((int)TargetID.CALeftArm, out var leftArms) && 
            targetsByIDs.TryGetValue((int)TargetID.CARightArm, out var rightArms) && 
            log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.CAArmSmash, out var armSmashes))
        {
            var chest = log.AgentData.GetGadgetsByID(_conjuredAmalgamate.ChestID).FirstOrDefault();
            long lowerThreshold = 0;
            foreach (var armSmash in armSmashes)
            {
                if (armSmash.Time < lowerThreshold)
                {
                    continue;
                }
                long start = armSmash.Time;
                var conjuredAmalgamate = conjuredAmalgamates.FirstOrDefault(x => x.InAwareTimes(start));
                var leftArm = leftArms.FirstOrDefault(x => x.InAwareTimes(start));
                var rightArm = rightArms.FirstOrDefault(x => x.InAwareTimes(start));
                if (conjuredAmalgamate != null && leftArm != null && rightArm != null)
                {
                    bool success = false;
                    // CA will be at 90% due to an arm being killed and that should be its first in combat health update
                    double prevHPUpdate = 99;
                    long end = conjuredAmalgamate.LastAware;
                    foreach (var hpUpdate in log.CombatData.GetHealthUpdateEvents(conjuredAmalgamate.AgentItem))
                    {
                        if (hpUpdate.Time < start + 1000)
                        {
                            continue;
                        }
                        if (prevHPUpdate < hpUpdate.HealthPercent)
                        {
                            end = hpUpdate.Time;
                        } 
                        else
                        {
                            prevHPUpdate = hpUpdate.HealthPercent;
                        }
                    }
                    if (chest != null && chest.InAwareTimes(end + 500))
                    {
                        end = chest.FirstAware;
                        success = true;
                    }
                    lowerThreshold = end;
                    AddInstanceEncounterPhase(log, phases, encounterPhases, [conjuredAmalgamate], [leftArm, rightArm], [], mainPhase, "Conjured Amalgamate", start, end, success, _conjuredAmalgamate, log.CombatData.GetBuffApplyData(SkillIDs.LockedOn).Any(x => x.Time >= start && x.Time <= end) ? LogData.LogMode.CM : LogData.LogMode.Normal);
                }
            }
        }
        NumericallyRenameEncounterPhases(encounterPhases);
        return encounterPhases;
    }

    private List<EncounterPhaseData> HandleTwinLargosPhases(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, ParsedEvtcLog log, List<PhaseData> phases)
    {
        var encounterPhases = new List<EncounterPhaseData>();
        var mainPhase = phases[0];
        if (!targetsByIDs.TryGetValue((int)TargetID.Kenut, out var kenuts))
        {
            kenuts = [];
        }
        if (targetsByIDs.TryGetValue((int)TargetID.Nikare, out var nikares))
        {
            var chest = log.AgentData.GetGadgetsByID(_twinLargos.ChestID).FirstOrDefault();
            foreach (var nikare in nikares)
            {
                var kenut = kenuts.FirstOrDefault(x => x.InAwareTimes(nikare));
                long start = nikare.FirstAware;
                var nikareEnterCombat = log.CombatData.GetEnterCombatEvents(nikare.AgentItem).FirstOrDefault();
                if (nikareEnterCombat != null)
                {
                    start = nikareEnterCombat.Time;
                }
                long end = nikare.LastAware;
                if (kenut != null)
                {
                    var kenutEnterCombat = log.CombatData.GetEnterCombatEvents(kenut.AgentItem).FirstOrDefault();
                    if (kenutEnterCombat != null)
                    {
                        start = Math.Min(start, kenutEnterCombat.Time);
                    }
                    else
                    {
                        start = Math.Min(start, kenut.FirstAware);
                    }
                    end = Math.Max(nikare.LastAware, kenut.LastAware);
                }
                bool success = false;
                if (chest != null && chest.InAwareTimes(end + 500))
                {
                    end = chest.FirstAware;
                    success = true;
                }
                AddInstanceEncounterPhase(log, phases, encounterPhases, [nikare, kenut], [], [], mainPhase, "Twin Largos", start, end, success, _twinLargos, TwinLargos.HasCastAquaticDomainOrCMHP(log.CombatData, nikare, kenut) ? LogData.LogMode.CM : LogData.LogMode.Normal);
            }
        }
        NumericallyRenameEncounterPhases(encounterPhases);
        return encounterPhases;
    }

    private List<EncounterPhaseData> HandleQadimPhases(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, ParsedEvtcLog log, List<PhaseData> phases)
    {
        var encounterPhases = new List<EncounterPhaseData>();
        var mainPhase = phases[0];
        if (targetsByIDs.TryGetValue((int)TargetID.Qadim, out var qadims))
        {
            var chest = log.AgentData.GetGadgetsByID(_qadim.ChestID).FirstOrDefault();
            var subBosses = Targets.Where(x => x.IsAnySpecies([TargetID.AncientInvokedHydra, TargetID.ApocalypseBringer, TargetID.WyvernPatriarch, TargetID.WyvernMatriarch]));
            foreach (var qadim in qadims)
            {
                var qadimAnimatedCasts = qadim.GetAnimatedCastEvents(log, qadim.FirstAware, qadim.LastAware);
                var qadimFirstCast = qadimAnimatedCasts.FirstOrDefault(x => x.SkillID == SkillIDs.QadimInitialCast);
                var qadimSanityCheckCast = qadimAnimatedCasts.FirstOrDefault(x => (x.SkillID == SkillIDs.FlameSlash3 || x.SkillID == SkillIDs.FlameSlash || x.SkillID == SkillIDs.FlameWave));
                if (qadimFirstCast == null || qadimSanityCheckCast == null || qadimSanityCheckCast.Time <= qadimFirstCast.Time)
                {
                    continue;
                }
                long start = qadimFirstCast.Time;
                long end = qadim.LastAware;
                bool success = false;
                if (chest != null && chest.InAwareTimes(end + 500))
                {
                    end = chest.FirstAware;
                    success = true;
                }
                AddInstanceEncounterPhase(log, phases, encounterPhases, [qadim], subBosses, [], mainPhase, "Qadim", start, end, success, _qadim, qadim.GetHealth(log.CombatData) > 21e6 ? LogData.LogMode.CM : LogData.LogMode.Normal);
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

            var caPhases = HandleConjuredAmalgamatePhases(targetsByIDs, log, phases);
            foreach (var caPhase in caPhases)
            {
                var ca = caPhase.Targets.Keys.First(x => x.IsSpecies(TargetID.ConjuredAmalgamate));
                var rightArm = caPhase.Targets.Keys.FirstOrDefault(x => x.IsSpecies(TargetID.CALeftArm));
                var leftArm = caPhase.Targets.Keys.FirstOrDefault(x => x.IsSpecies(TargetID.CARightArm));
                phases.AddRange(ConjuredAmalgamate.ComputePhases(log, ca, rightArm, leftArm, caPhase, requirePhases));
            }
        }
        {
            var twinLargosPhases = HandleTwinLargosPhases(targetsByIDs, log, phases);
            foreach (var twinLargosPhase in twinLargosPhases)
            {
                var nikare = twinLargosPhase.Targets.Keys.First(x => x.IsSpecies(TargetID.Nikare));
                var kenut = twinLargosPhase.Targets.Keys.FirstOrDefault(x => x.IsSpecies(TargetID.Kenut));
                phases.AddRange(TwinLargos.ComputePhases(log, nikare, kenut, twinLargosPhase, requirePhases));
            }
        }
        {
            var qadimPhases = HandleQadimPhases(targetsByIDs, log, phases);
            foreach (var qadimPhase in qadimPhases)
            {
                var nikare = qadimPhase.Targets.Keys.First(x => x.IsSpecies(TargetID.Qadim));
                phases.AddRange(Qadim.ComputePhases(log, nikare, Targets, qadimPhase, requirePhases));
            }
        }
        
        return phases;
    }

    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        List<InstantCastFinder> finders = [
            .. _conjuredAmalgamate.GetInstantCastFinders(),
            .. _twinLargos.GetInstantCastFinders(),
            .. _qadim.GetInstantCastFinders()
        ];
        return finders;
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        List<TargetID> trashes = [
            .. _conjuredAmalgamate.GetTrashMobsIDs(),
            .. _twinLargos.GetTrashMobsIDs(),
            .. _qadim.GetTrashMobsIDs()
        ];
        return trashes.Distinct().ToList();
    }
    internal override IReadOnlyList<TargetID> GetTargetsIDs()
    {
        List<TargetID> targets = [
            .. _conjuredAmalgamate.GetTargetsIDs(),
            .. _twinLargos.GetTargetsIDs(),
            .. _qadim.GetTargetsIDs()
        ];
        return targets.Distinct().ToList();
    }

    internal override IReadOnlyList<TargetID> GetFriendlyNPCIDs()
    {
        List<TargetID> friendlies = [
            .. _conjuredAmalgamate.GetFriendlyNPCIDs(),
            .. _twinLargos.GetFriendlyNPCIDs(),
            .. _qadim.GetFriendlyNPCIDs()
        ];
        return friendlies.Distinct().ToList();
    }
    protected override HashSet<int> IgnoreForAutoNumericalRenaming()
    {
        return [
            (int)TargetID.QadimPlatform
        ];
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        ConjuredAmalgamate.HandleCAAgents(agentData, combatData);
        var sword = ConjuredAmalgamate.CreateCustomSwordAgent(logData, agentData);
        var maxHPUpdates = combatData
            .Where(x => x.IsStateChange == StateChange.MaxHealthUpdate)
            .Select(x => new MaxHealthUpdateEvent(x, agentData))
            .GroupBy(x => x.MaxHealth).ToDictionary(x => x.Key, x => x.ToList());
        Qadim.FindPlateforms(evtcVersion, maxHPUpdates, agentData, combatData);
        Qadim.FindLamps(evtcVersion, maxHPUpdates, agentData, combatData);
        Qadim.FindPyres(gw2Build, agentData, combatData);
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);
        ConjuredAmalgamate.RedirectSwordDamageToSwordAgent(sword, combatData, extensions);
        Qadim.RenamePyres(Targets);
        foreach (SingleActor actor in Targets)
        {
            switch (actor.ID)
            {
                case (int)TargetID.Kenut:
                case (int)TargetID.Nikare:
                    TwinLargos.AdjustFinalHPEvents(combatData, actor.AgentItem);
                    break;
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
