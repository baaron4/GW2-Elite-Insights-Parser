using System;
using System.Collections.Generic;
using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIGW2API;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicTimeUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal class TheKeyOfAhdashimInstance : TheKeyOfAhdashim
{
    private readonly Adina _adina;
    private readonly Sabir _sabir;
    private readonly PeerlessQadim _peerlessQadim;

    private readonly IReadOnlyList<TheKeyOfAhdashim> _subLogics;
    public TheKeyOfAhdashimInstance(int triggerID) : base(triggerID)
    {
        LogID = LogIDs.LogMasks.Unsupported;
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

    internal override string GetLogicName(CombatData combatData, AgentData agentData, GW2APIController apiController)
    {
        return "The Key Of Ahdashim";
    }

    internal override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log, CombatReplayDecorationContainer arenaDecorations)
    {
        var crMap = new CombatReplayMap((800, 800), (-21504, -21504, 24576, 24576));
        arenaDecorations.Add(new ArenaDecoration((log.LogData.LogStart, log.LogData.LogEnd), CombatReplayKeyOfAhdashim, crMap));
        foreach (var subLogic in _subLogics)
        {
            subLogic.GetCombatMapInternal(log, arenaDecorations);
        }
        return crMap;
    }

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, LogData logData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        var chest = agentData.GetGadgetsByID(_peerlessQadim.ChestID).FirstOrDefault();
        if (chest != null)
        {
            logData.SetSuccess(true, chest.FirstAware);
            return;
        }
        base.CheckSuccess(combatData, agentData, logData, playerAgents);
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        var targetsByIDs = Targets.GroupBy(x => x.ID).ToDictionary(x => x.Key, x => x.ToList());
        {

            var adinaPhases = ProcessGenericEncounterPhasesForInstance(targetsByIDs, log, phases, TargetID.Adina, [], "Cardinal Adina", _adina, (log, adina) => adina.GetHealth(log.CombatData) > 23e6 ? LogData.LogMode.CM : LogData.LogMode.Normal);
            foreach (var adinaPhase in adinaPhases)
            {
                var adina = adinaPhase.Targets.Keys.First(x => x.IsSpecies(TargetID.Adina));
                var lastInvuln = adina.GetBuffStatus(log, SkillIDs.Determined762).LastOrNull();
                long lastBossPhaseStart = lastInvuln != null && lastInvuln.Value.Value == 0 ? lastInvuln.Value.Start : adinaPhase.End; // if log ends with any boss phase, ignore hands after that point
                phases[0].AddTargets(Targets.Where(x => x.IsAnySpecies(Adina.HandIDs) && x.FirstAware < lastBossPhaseStart && x.FirstAware > adinaPhase.Start), log, PhaseData.TargetPriority.Blocking);
                phases.AddRange(Adina.ComputePhases(log, adina, Targets, adinaPhase, requirePhases));
            }
        }
        {
            var sabirPhases = ProcessGenericEncounterPhasesForInstance(targetsByIDs, log, phases, TargetID.Sabir, [], "Cardinal Sabir", _sabir, (log, sabir) => sabir.GetHealth(log.CombatData) > 32e6 ? LogData.LogMode.CM : LogData.LogMode.Normal);
            foreach (var sabirPhase in sabirPhases)
            {
                var sabir = sabirPhase.Targets.Keys.First(x => x.IsSpecies(TargetID.Sabir));
                phases.AddRange(Sabir.ComputePhases(log, sabir, sabirPhase, requirePhases));
            }
        }
        {
            var qtpPhases = ProcessGenericEncounterPhasesForInstance(targetsByIDs, log, phases, TargetID.PeerlessQadim, [], "Qadim the Peerless", _peerlessQadim, (log, qtp) => qtp.GetHealth(log.CombatData) > 48e6 ? LogData.LogMode.CM : LogData.LogMode.Normal);
            foreach (var qtpPhase in qtpPhases)
            {
                var qtp = qtpPhase.Targets.Keys.First(x => x.IsSpecies(TargetID.PeerlessQadim));
                phases.AddRange(PeerlessQadim.ComputePhases(log, qtp, qtpPhase, requirePhases));
            }
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
    internal override HashSet<TargetID> ForbidBreakbarPhasesFor()
    {
        HashSet<TargetID> forbidBreakbarPhasesFor = [
            .. _adina.ForbidBreakbarPhasesFor(),
            .. _sabir.ForbidBreakbarPhasesFor(),
            .. _peerlessQadim.ForbidBreakbarPhasesFor()
        ];
        return forbidBreakbarPhasesFor;
    }
    protected override HashSet<int> IgnoreForAutoNumericalRenaming()
    {
        return [
            (int)TargetID.HandOfErosion,
            (int)TargetID.HandOfEruption,
            (int)TargetID.PeerlessQadimPylon,
        ];
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        Adina.FindPlatforms(agentData);
        Adina.FindHands(logData, agentData, combatData, extensions);
        Sabir.FindPlateforms(agentData);
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);
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

    internal override List<CastEvent> SpecialCastEventProcess(CombatData combatData, AgentData agentData, SkillData skillData)
    {
        var res = new List<CastEvent>();
        foreach (var subLogic in _subLogics)
        {
            res.AddRange(subLogic.SpecialCastEventProcess(combatData, agentData, skillData));
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
    internal override void SetInstanceBuffs(ParsedEvtcLog log, List<InstanceBuff> instanceBuffs)
    {
        base.SetInstanceBuffs(log, instanceBuffs);
        foreach (var logic in _subLogics)
        {
            logic.SetInstanceBuffs(log, instanceBuffs);
        }
    }
    internal override void ComputeAchievementEligibilityEvents(ParsedEvtcLog log, Player p, List<AchievementEligibilityEvent> achievementEligibilityEvents)
    {
        base.ComputeAchievementEligibilityEvents(log, p, achievementEligibilityEvents);
        foreach (var logic in _subLogics)
        {
            logic.ComputeAchievementEligibilityEvents(log, p, achievementEligibilityEvents);
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
