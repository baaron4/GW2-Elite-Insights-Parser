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
        EncounterID = EncounterIDs.EncounterMasks.Unsupported;
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

    internal override string GetLogicName(CombatData combatData, AgentData agentData)
    {
        return "Hall Of Chains";
    }

    private static void HandleRiverOfSoulsPhases(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, ParsedEvtcLog log, List<PhaseData> phases)
    {
        var encounterPhases = new List<PhaseData>();
        var mainPhase = phases[0];
        if (targetsByIDs.TryGetValue((int)TargetID.Desmina, out var desminas) 
            && targetsByIDs.TryGetValue((int)TargetID.DummyTarget, out var dummies))
        {
            var dummy = dummies.FirstOrDefault(x => x.Character == "River of Souls");
            if (dummy != null)
            {
                var enervators = log.AgentData.GetNPCsByID(TargetID.Enervator);
                var chest = log.AgentData.GetGadgetsByID(ChestID.ChestOfSouls).FirstOrDefault();
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
                    if (chest != null && chest.InAwareTimes(desmina.LastAware + 500))
                    {
                        end = chest.FirstAware;
                        success = true;
                    }
                    var phase = new PhaseData(start, end, "River of Souls");
                    phases.Add(phase);
                    encounterPhases.Add(phase);
                    if (success)
                    {
                        phase.Name += " (Success)";
                    }
                    else
                    {
                        phase.Name += " (Failure)";
                    }
                    phase.AddParentPhase(mainPhase);
                    phase.AddTarget(dummy, log);
                }
            }
        }
        NumericallyRenamePhases(encounterPhases);
    }

    private static void HandleStatueOfIcePhases(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, ParsedEvtcLog log, List<PhaseData> phases)
    {
        var encounterPhases = new List<PhaseData>();
        var mainPhase = phases[0];
        if (targetsByIDs.TryGetValue((int)TargetID.BrokenKing, out var brokenKings))
        {
            foreach (var brokenKing in brokenKings)
            {
                var firstCombatCast = brokenKing.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd).FirstOrDefault(x => x.SkillId == SkillIDs.BrokenKingFirstCast);
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
                var phase = new PhaseData(start, end, "Statue of Ice");
                phases.Add(phase);
                encounterPhases.Add(phase);
                if (success)
                {
                    phase.Name += " (Success)";
                }
                else
                {
                    phase.Name += " (Failure)";
                }
                phase.AddParentPhase(mainPhase);
                phase.AddTarget(brokenKing, log);
                mainPhase.AddTarget(brokenKing, log);
            }
        }
        NumericallyRenamePhases(encounterPhases);
    }
    private static void HandleStatueOfDeathPhases(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, ParsedEvtcLog log, List<PhaseData> phases)
    {
        var encounterPhases = new List<PhaseData>();
        var mainPhase = phases[0];
        if (targetsByIDs.TryGetValue((int)TargetID.EaterOfSouls, out var eaterOfSouls))
        {
            var peasants = new List<AgentItem>(log.AgentData.GetNPCsByID(TargetID.AscalonianPeasant1));
            peasants.AddRange(log.AgentData.GetNPCsByID(TargetID.AscalonianPeasant2));
            foreach (var eaterOfSoul in eaterOfSouls)
            {
                if (!eaterOfSoul.GetDamageEvents(null, log, log.FightData.FightStart, log.FightData.FightEnd).Any(x => x.CreditedFrom.IsPlayer))
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
                var phase = new PhaseData(start, end, "Statue of Death");
                phases.Add(phase);
                encounterPhases.Add(phase);
                if (success)
                {
                    phase.Name += " (Success)";
                }
                else
                {
                    phase.Name += " (Failure)";
                }
                phase.AddParentPhase(mainPhase);
                phase.AddTarget(eaterOfSoul, log);
                mainPhase.AddTarget(eaterOfSoul, log);
            }
        }
        NumericallyRenamePhases(encounterPhases);
    }

    private static void HandleStatueOfDarknessPhases(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, ParsedEvtcLog log, List<PhaseData> phases)
    {
        var encounterPhases = new List<PhaseData>();
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
                var phase = new PhaseData(start, end, "Statue of Darkness");
                phases.Add(phase);
                encounterPhases.Add(phase);
                if (success)
                {
                    phase.Name += " (Success)";
                }
                else
                {
                    phase.Name += " (Failure)";
                }
                phase.AddParentPhase(mainPhase);
                phase.AddTarget(eyeOfFate, log);
                phase.AddTarget(eyeOfJudgement, log);
                mainPhase.AddTarget(eyeOfFate, log);
                mainPhase.AddTarget(eyeOfJudgement, log);
            }
        }
        NumericallyRenamePhases(encounterPhases);
    }

    private static void HandleDhuumPhases(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, ParsedEvtcLog log, List<PhaseData> phases)
    {
        var encounterPhases = new List<PhaseData>();
        var mainPhase = phases[0];
        if (targetsByIDs.TryGetValue((int)TargetID.Dhuum, out var dhuums))
        {
            var messengers = log.AgentData.GetNPCsByID(TargetID.Messenger);
            var chest = log.AgentData.GetGadgetsByID(ChestID.DhuumChest).FirstOrDefault();
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
                if (chest != null && chest.InAwareTimes(dhuum.LastAware + 500))
                {
                    end = chest.FirstAware;
                    success = true;
                }
                var phase = new PhaseData(start, end, "Dhuum");
                phases.Add(phase);
                encounterPhases.Add(phase);
                if (dhuum.GetHealth(log.CombatData) > 35e6)
                {
                    phase.Name += " CM";
                }
                if (success)
                {
                    phase.Name += " (Success)";
                }
                else
                {
                    phase.Name += " (Failure)";
                }
                phase.AddParentPhase(mainPhase);
                phase.AddTarget(dhuum, log);
                mainPhase.AddTarget(dhuum, log);
            }
        }
        NumericallyRenamePhases(encounterPhases);
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        var targetsByIDs = Targets.GroupBy(x => x.ID).ToDictionary(x => x.Key, x => x.ToList());
        ProcessGenericEncounterPhasesForInstance(targetsByIDs, log, phases, TargetID.SoullessHorror, [], ChestID.ChestOfDesmina, "Soulless Horror", (log, soullessHorror) => {
            return SoullessHorror.HasFastNecrosis(log.CombatData, soullessHorror.FirstAware, soullessHorror.LastAware);
        });
        HandleRiverOfSoulsPhases(targetsByIDs, log, phases);
        HandleStatueOfIcePhases(targetsByIDs, log, phases);
        HandleStatueOfDeathPhases(targetsByIDs, log, phases);
        HandleStatueOfDarknessPhases(targetsByIDs, log, phases);
        if (phases[0].Targets.Count == 0)
        {
            phases[0].AddTarget(Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Instance)), log);
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
        targets.Add(TargetID.Instance);
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

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        agentData.AddCustomNPCAgent(fightData.FightStart, fightData.FightEnd, "River of Souls", Spec.NPC, (int)TargetID.DummyTarget, true);
        Dhuum.HandleYourSouls(agentData, combatData);
        base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
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
