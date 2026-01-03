using System.Collections.Generic;
using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicTimeUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SpeciesIDs;
using GW2EIGW2API;

namespace GW2EIEvtcParser.LogLogic;

internal class HallOfChainsInstance : HallOfChains
{
    private readonly SoullessHorror _soullessHorror;
    private readonly River _river;
    private readonly StatueOfIce _statueOfIce;
    private readonly StatueOfDeath _statueOfDeath;
    private readonly StatueOfDarkness _statueOfDarkness;
    private readonly Dhuum _dhuum;

    private readonly IReadOnlyList<HallOfChains> _subLogics;
    public HallOfChainsInstance(int triggerID) : base(triggerID)
    {
        LogID = LogIDs.LogMasks.Unsupported;
        Icon = InstanceIconHallOfChains;
        Extension = "hallchains";

        _soullessHorror = new SoullessHorror((int)TargetID.SoullessHorror);
        _river = new River((int)TargetID.DummyTarget);
        _statueOfIce = new StatueOfIce((int)TargetID.BrokenKing);
        _statueOfDeath = new StatueOfDeath((int)TargetID.EaterOfSouls);
        _statueOfDarkness = new StatueOfDarkness((int)TargetID.EyeOfFate);
        _dhuum = new Dhuum((int)TargetID.Dhuum);
        _subLogics = [_soullessHorror, _river, _statueOfIce, _statueOfDeath, _statueOfDarkness, _dhuum];

        MechanicList.Add(_soullessHorror.Mechanics);
        MechanicList.Add(_river.Mechanics);
        MechanicList.Add(_statueOfIce.Mechanics);
        MechanicList.Add(_statueOfDeath.Mechanics);
        MechanicList.Add(_statueOfDarkness.Mechanics);
        MechanicList.Add(_dhuum.Mechanics);
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData, GW2APIController apiController)
    {
        return "Hall Of Chains";
    }

    internal override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log, CombatReplayDecorationContainer arenaDecorations)
    {
        var crMap = new CombatReplayMap((800, 426), (-21504, -12288, 24576, 12288));
        arenaDecorations.Add(new ArenaDecoration((log.LogData.LogStart, log.LogData.LogEnd), CombatReplayHallOfChains, crMap));
        foreach (var subLogic in _subLogics)
        {
            subLogic.GetCombatMapInternal(log, arenaDecorations);
        }
        return crMap;
    }
    internal override void CheckSuccess(CombatData combatData, AgentData agentData, LogData logData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        var chest = agentData.GetGadgetsByID(_dhuum.ChestID).FirstOrDefault();
        if (chest != null)
        {
            logData.SetSuccess(true, chest.FirstAware);
            return;
        }
        base.CheckSuccess(combatData, agentData, logData, playerAgents);
    }

    private List<EncounterPhaseData> HandleRiverOfSoulsPhases(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, IReadOnlyDictionary<int, List<SingleActor>> friendliesByIDs, ParsedEvtcLog log, List<PhaseData> phases)
    {
        var encounterPhases = new List<EncounterPhaseData>();
        var mainPhase = phases[0];
        if (friendliesByIDs.TryGetValue((int)TargetID.Desmina, out var desminas) 
            && targetsByIDs.TryGetValue((int)TargetID.DummyTarget, out var dummies))
        {
            var dummy = dummies.FirstOrDefault(x => x.Character == "River of Souls");
            if (dummy != null)
            {
                var enervators = log.AgentData.GetNPCsByID(TargetID.Enervator);
                var chest = log.AgentData.GetGadgetsByID(_river.ChestID).FirstOrDefault();
                foreach (var desmina in desminas)
                {
                    var currentEnervators = enervators.Where(x => x.InAwareTimes(desmina));
                    if (!currentEnervators.Any())
                    {
                        continue;
                    }
                    long start = currentEnervators.Min(x => x.FirstAware);
                    bool success = false;
                    long end = desmina.LastAware;
                    if (chest != null && desmina.InAwareTimes(chest.LastAware - 500))
                    {
                        end = chest.FirstAware;
                        success = true;
                    }
                    AddInstanceEncounterPhase(log, phases, encounterPhases, [dummy], [], [], mainPhase, "River of Souls", start, end, success, _river);
                }
            }
        }
        NumericallyRenameEncounterPhases(encounterPhases);
        return encounterPhases;
    }

    private List<EncounterPhaseData> HandleStatueOfIcePhases(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, ParsedEvtcLog log, List<PhaseData> phases)
    {
        var encounterPhases = new List<EncounterPhaseData>();
        var mainPhase = phases[0];
        if (targetsByIDs.TryGetValue((int)TargetID.BrokenKing, out var brokenKings))
        {
            foreach (var brokenKing in brokenKings)
            {
                var firstCombatCast = brokenKing.GetCastEvents(log).FirstOrDefault(x => x.SkillID == BrokenKingFirstCast);
                if (firstCombatCast == null)
                {
                    continue;
                } 
                long start = firstCombatCast.Time;
                bool success = false;
                long end = brokenKing.LastAware;
                var deadEvent = log.CombatData.GetDeadEvents(brokenKing.AgentItem).LastOrDefault();
                if (deadEvent != null)
                {
                    end = deadEvent.Time;
                    success = true;
                }
                AddInstanceEncounterPhase(log, phases, encounterPhases, [brokenKing], [], [], mainPhase, "Statue of Ice", start, end, success, _statueOfIce);
            }
        }
        NumericallyRenameEncounterPhases(encounterPhases);
        return encounterPhases;
    }
    private List<EncounterPhaseData> HandleStatueOfDeathPhases(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, ParsedEvtcLog log, List<PhaseData> phases)
    {
        var encounterPhases = new List<EncounterPhaseData>();
        var mainPhase = phases[0];
        if (targetsByIDs.TryGetValue((int)TargetID.EaterOfSouls, out var eaterOfSouls))
        {
            var peasants = new List<AgentItem>(log.AgentData.GetNPCsByID(TargetID.AscalonianPeasant1));
            peasants.AddRange(log.AgentData.GetNPCsByID(TargetID.AscalonianPeasant2));
            foreach (var eaterOfSoul in eaterOfSouls)
            {
                if (!eaterOfSoul.GetDamageTakenEvents(null, log).Any(x => x.CreditedFrom.IsPlayer))
                {
                    continue;
                }
                var currentPeasants = peasants.Where(x => x.InAwareTimes(eaterOfSoul));
                if (!currentPeasants.Any())
                {
                    continue;
                }
                long start = currentPeasants.Max(x => x.LastAware);
                bool success = false;
                long end = eaterOfSoul.LastAware;
                var deadEvent = log.CombatData.GetDeadEvents(eaterOfSoul.AgentItem).LastOrDefault();
                if (deadEvent != null)
                {
                    end = deadEvent.Time;
                    success = true;
                }
                AddInstanceEncounterPhase(log, phases, encounterPhases, [eaterOfSoul], [], [], mainPhase, "Statue of Death", start, end, success, _statueOfDeath);
            }
        }
        NumericallyRenameEncounterPhases(encounterPhases);
        return encounterPhases;
    }

    private List<EncounterPhaseData> HandleStatueOfDarknessPhases(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, ParsedEvtcLog log, List<PhaseData> phases)
    {
        var encounterPhases = new List<EncounterPhaseData>();
        var mainPhase = phases[0];
        if (targetsByIDs.TryGetValue((int)TargetID.EyeOfFate, out var eyeOfFates) &&
            targetsByIDs.TryGetValue((int)TargetID.EyeOfJudgement, out var eyeOfJudgements))
        {
            var lightThieves = log.AgentData.GetNPCsByID(TargetID.LightThieves);
            foreach (var eyeOfFate in eyeOfFates)
            {
                var eyeOfJudgement = eyeOfJudgements.FirstOrDefault(x => x.InAwareTimes(eyeOfFate));
                if (eyeOfJudgement == null)
                {
                    continue;
                }
                var currentLightThieves = lightThieves.Where(x => x.InAwareTimes(eyeOfFate));
                if (!currentLightThieves.Any())
                {
                    continue;
                }
                long start = currentLightThieves.Min(x => x.FirstAware);
                bool success = false;
                long end = Math.Max(eyeOfFate.LastAware, eyeOfJudgement.LastAware);
                if (StatueOfDarkness.HasIntersectingLastGrasps(log.CombatData, eyeOfFate, eyeOfJudgement, out long intersectTime))
                {
                    success = true;
                    end = intersectTime;
                }
                AddInstanceEncounterPhase(log, phases, encounterPhases, [eyeOfFate, eyeOfJudgement], [], [], mainPhase, "Statue of Darkness", start, end, success, _statueOfDarkness);
            }
        }
        NumericallyRenameEncounterPhases(encounterPhases);
        return encounterPhases;
    }

    private List<EncounterPhaseData> HandleDhuumPhases(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, ParsedEvtcLog log, List<PhaseData> phases)
    {
        var encounterPhases = new List<EncounterPhaseData>();
        var mainPhase = phases[0];
        if (targetsByIDs.TryGetValue((int)TargetID.Dhuum, out var dhuums))
        {
            var messengers = log.AgentData.GetNPCsByID(TargetID.Messenger);
            var chest = log.AgentData.GetGadgetsByID(_dhuum.ChestID).FirstOrDefault();
            foreach (var dhuum in dhuums)
            {
                var currentMessengers = messengers.Where(x => x.InAwareTimes(dhuum));
                if (!currentMessengers.Any())
                {
                    continue;
                }
                long start = currentMessengers.Min(x => x.FirstAware);
                bool success = false;
                long end = dhuum.LastAware;
                if (chest != null && chest.InAwareTimes(end - 500, end + 500))
                {
                    end = chest.FirstAware;
                    success = true;
                }
                if (dhuum.GetAnimatedCastEvents(log).Any(x => (x.SkillID != WeaponStow && x.SkillID != WeaponDraw) && x.Time >= start && x.Time <= start + 40000))
                {
                    AddInstanceEncounterPhase(log, phases, encounterPhases, [dhuum], [], [], mainPhase, "Dhuum", start, end, success, _dhuum, dhuum.GetHealth(log.CombatData) > 35e6 ? LogData.LogMode.CM : LogData.LogMode.Normal, LogData.LogStartStatus.NoPreEvent);
                } 
                else
                {
                    AddInstanceEncounterPhase(log, phases, encounterPhases, [dhuum], [], [], mainPhase, "Dhuum", start, end, success, _dhuum, dhuum.GetHealth(log.CombatData) > 35e6 ? LogData.LogMode.CM : LogData.LogMode.Normal);
                }
            }
        }
        NumericallyRenameEncounterPhases(encounterPhases);
        return encounterPhases;
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        var targetsByIDs = Targets.GroupBy(x => x.ID).ToDictionary(x => x.Key, x => x.ToList());
        var friendliesByIDs = NonSquadFriendlies.Where(x => x.AgentItem.IsNPC).GroupBy(x => x.ID).ToDictionary(x => x.Key, x => x.ToList());
        {
            var shPhases = ProcessGenericEncounterPhasesForInstance(targetsByIDs, log, phases, TargetID.SoullessHorror, [], "Soulless Horror", _soullessHorror, 
                (log, soullessHorror) => 
                SoullessHorror.HasFastNecrosis(log.CombatData, soullessHorror.FirstAware, soullessHorror.LastAware) ? LogData.LogMode.CM : LogData.LogMode.Normal);
            foreach (var shPhase in shPhases)
            {
                var soullessHorror = shPhase.Targets.Keys.First(x => x.IsSpecies(TargetID.SoullessHorror));
                phases.AddRange(SoullessHorror.ComputePhases(log, soullessHorror, Targets, shPhase, requirePhases));
            }
        }
        HandleRiverOfSoulsPhases(targetsByIDs, friendliesByIDs, log, phases);
        HandleStatueOfIcePhases(targetsByIDs, log, phases);
        HandleStatueOfDeathPhases(targetsByIDs, log, phases);
        {
            var statueOfDeathPhases = HandleStatueOfDarknessPhases(targetsByIDs, log, phases);
            foreach (var statueOfDeathPhase in statueOfDeathPhases)
            {
                var eyeFate = statueOfDeathPhase.Targets.Keys.First(x => x.IsSpecies(TargetID.EyeOfFate));
                var eyeJudgment = statueOfDeathPhase.Targets.Keys.First(x => x.IsSpecies(TargetID.EyeOfJudgement));
                phases.AddRange(StatueOfDarkness.ComputePhases(log, eyeFate, eyeJudgment, statueOfDeathPhase, requirePhases));
            }
        }
        {
            var dhuumPhases = HandleDhuumPhases(targetsByIDs, log, phases);
            foreach (var dhuumPhase in dhuumPhases)
            {
                var dhuum = dhuumPhase.Targets.Keys.First(x => x.IsSpecies(TargetID.Dhuum));
                phases.AddRange(Dhuum.ComputePhases(log, dhuum, Targets, dhuumPhase, requirePhases));
            }
        }
        return phases;
    }
    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        List<InstantCastFinder> finders = [
            .. _soullessHorror.GetInstantCastFinders(),
            .. _river.GetInstantCastFinders(),
            .. _statueOfIce.GetInstantCastFinders(),
            .. _statueOfDeath.GetInstantCastFinders(),
            .. _statueOfDarkness.GetInstantCastFinders(),
            .. _dhuum.GetInstantCastFinders()
        ];
        return finders;
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        List<TargetID> trashes = [
            .. _soullessHorror.GetTrashMobsIDs(),
            .. _river.GetTrashMobsIDs(),
            .. _statueOfIce.GetTrashMobsIDs(),
            .. _statueOfDeath.GetTrashMobsIDs(),
            .. _statueOfDarkness.GetTrashMobsIDs(),
            .. _dhuum.GetTrashMobsIDs()
        ];
        return trashes.Distinct().ToList();
    }
    internal override IReadOnlyList<TargetID> GetTargetsIDs()
    {
        List<TargetID> targets = [
            .. _soullessHorror.GetTargetsIDs(),
            .. _river.GetTargetsIDs(),
            .. _statueOfIce.GetTargetsIDs(),
            .. _statueOfDeath.GetTargetsIDs(),
            .. _statueOfDarkness.GetTargetsIDs(),
            .. _dhuum.GetTargetsIDs()
        ];
        return targets.Distinct().ToList();
    }

    internal override IReadOnlyList<TargetID> GetFriendlyNPCIDs()
    {
        List<TargetID> friendlies = [
            .. _soullessHorror.GetFriendlyNPCIDs(),
            .. _river.GetFriendlyNPCIDs(),
            .. _statueOfIce.GetFriendlyNPCIDs(),
            .. _statueOfDeath.GetFriendlyNPCIDs(),
            .. _statueOfDarkness.GetFriendlyNPCIDs(),
            .. _dhuum.GetFriendlyNPCIDs()
        ];
        return friendlies.Distinct().ToList();
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        agentData.AddCustomNPCAgent(logData.LogStart, logData.LogEnd, "River of Souls", Spec.NPC, (int)TargetID.DummyTarget, true);
        Dhuum.HandleYourSouls(agentData, combatData);
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);
        foreach (var target in Targets)
        {
            switch (target.ID)
            {
                case (int)TargetID.SoullessHorror:
                    SoullessHorror.HandleSoullessHorrorFinalHPUpdate(combatData, target);
                    break;
                default:
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
