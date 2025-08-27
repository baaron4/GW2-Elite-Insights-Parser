using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal static class LogLogicPhaseUtils
{

    internal static void AddPhasesPerTarget(ParsedEvtcLog log, List<PhaseData> phases, IEnumerable<SingleActor> targets)
    {
        phases[0].AddTargets(targets, log);
        foreach (SingleActor target in targets)
        {
            long start = target.FirstAware;
            long end = target.LastAware;
            var enterCombat = log.CombatData.GetEnterCombatEvents(target.AgentItem).FirstOrDefault();
            if (enterCombat != null)
            {
                var exitCombat = log.CombatData.GetExitCombatEvents(target.AgentItem).FirstOrDefault(x => x.Time < enterCombat.Time);
                if (exitCombat == null)
                {
                    start = enterCombat.Time;
                }
            }
            var dead = log.CombatData.GetDeadEvents(target.AgentItem).FirstOrDefault();
            bool success = false;
            if (dead != null)
            {
                success = true;
                end = dead.Time;
            }
            var phase = new EncounterPhaseData(Math.Max(log.LogData.LogStart, start), Math.Min(target.LastAware, end), target.Character, success, log.LogData.Logic.Icon, LogData.LogMode.Normal, log.LogData.Logic.LogID);
            phase.AddTarget(target, log);
            phase.AddParentPhase(phases[0]);
            phases.Add(phase);
        }
    }
    internal static List<PhaseData> GetPhasesBySquadCombatStartEnd(ParsedEvtcLog log)
    {
        var phases = new List<PhaseData>();
        int sequence = 1;
        foreach (var startEvent in log.CombatData.GetSquadCombatStartEvents())
        {
            var logEndEvent = log.CombatData.GetSquadCombatEndEvents().FirstOrDefault(x => x.ServerUnixTimeStamp >= startEvent.ServerUnixTimeStamp);
            if (logEndEvent != null)
            {
                var fightPhase = new EncounterPhaseData(startEvent.Time, logEndEvent.Time, "Fight " + (sequence++), true, log.LogData.Logic.Icon, LogData.LogMode.Normal, log.LogData.Logic.LogID);
                phases.Add(fightPhase);
            }
            else
            {
                var fightPhase = new EncounterPhaseData(startEvent.Time, phases[0].End, "Fight " + (sequence++), true, log.LogData.Logic.Icon, LogData.LogMode.Normal, log.LogData.Logic.LogID);
                phases.Add(fightPhase);
                break;
            }
        }
        return phases;
    }

    internal static List<PhaseData> GetPhasesByHealthPercent(ParsedEvtcLog log, SingleActor mainTarget, IReadOnlyList<double> thresholds, long start, long end)
    {
        var phases = new List<PhaseData>();
        if (thresholds.Count == 0)
        {
            return phases;
        }
        long phaseStart = start;
        double offset = 100.0 / thresholds.Count;
        IReadOnlyList<HealthUpdateEvent> hpUpdates = log.CombatData.GetHealthUpdateEvents(mainTarget.AgentItem);
        for (int i = 0; i < thresholds.Count; i++)
        {
            HealthUpdateEvent? evt = hpUpdates.FirstOrDefault(x => x.HealthPercent <= thresholds[i]);
            if (evt == null)
            {
                break;
            }
            var phase = new SubPhasePhaseData(phaseStart, Math.Min(evt.Time, end), (offset + thresholds[i]) + "% - " + thresholds[i] + "%");
            phase.AddTarget(mainTarget, log);
            phases.Add(phase);
            phaseStart = Math.Max(evt.Time, start);
        }
        if (phases.Count > 0 && phases.Count < thresholds.Count)
        {
            var lastPhase = new SubPhasePhaseData(phaseStart, end, (offset + thresholds[phases.Count]) + "% - " + thresholds[phases.Count] + "%");
            lastPhase.AddTarget(mainTarget, log);
            phases.Add(lastPhase);
        }
        return phases;
    }

    internal static List<PhaseData> GetPhasesByInvul(ParsedEvtcLog log, IEnumerable<long> skillIDs, SingleActor mainTarget, bool addSkipPhases, bool beginWithStart, long start, long end, bool filterSmallPhases = true)
    {
        long last = start;
        var invuls = GetBuffApplyRemoveSequence(log.CombatData, skillIDs, mainTarget, beginWithStart, true)
            .Where(x => x.Time >= 0)
            .ToList();
        invuls.SortByTime(); // Sort in case there were multiple skillIDs

        var phases = new List<PhaseData>(invuls.Count);
        bool nextToAddIsSkipPhase = !beginWithStart;
        foreach (BuffEvent c in invuls)
        {
            if (c is BuffApplyEvent)
            {
                long curEnd = Math.Min(c.Time, end);
                phases.Add(new SubPhasePhaseData(last, curEnd));
                last = curEnd;
                nextToAddIsSkipPhase = true;
            }
            else
            {
                long curEnd = Math.Min(c.Time, end);
                if (addSkipPhases)
                {
                    phases.Add(new SubPhasePhaseData(last, curEnd));
                }
                last = curEnd;
                nextToAddIsSkipPhase = false;
            }
        }
        if (!nextToAddIsSkipPhase || (nextToAddIsSkipPhase && addSkipPhases))
        {
            phases.Add(new SubPhasePhaseData(last, end));
        }
        long filterThreshold = filterSmallPhases ? 100 : 0;
        return phases.Where(x => x.DurationInMS > filterThreshold).ToList(); // only filter unrealistically short phases, otherwise it may mess with phase names
    }


    internal static List<PhaseData> GetPhasesByInvul(ParsedEvtcLog log, IEnumerable<long> skillIDs, SingleActor mainTarget, bool addSkipPhases, bool beginWithStart, bool filterSmallPhases = true)
    {
        return GetPhasesByInvul(log, skillIDs, mainTarget, addSkipPhases, beginWithStart, log.LogData.LogStart, log.LogData.LogEnd, filterSmallPhases);
    }

    internal static List<PhaseData> GetPhasesByInvul(ParsedEvtcLog log, long skillID, SingleActor mainTarget, bool addSkipPhases, bool beginWithStart, long start, long end, bool filterSmallPhases = true)
    {
        return GetPhasesByInvul(log, [ skillID ], mainTarget, addSkipPhases, beginWithStart, start, end, filterSmallPhases);
    }

    internal static List<PhaseData> GetPhasesByInvul(ParsedEvtcLog log, long skillID, SingleActor mainTarget, bool addSkipPhases, bool beginWithStart, bool filterSmallPhases = true)
    {
        return GetPhasesByInvul(log, skillID, mainTarget, addSkipPhases, beginWithStart, log.LogData.LogStart, log.LogData.LogEnd, filterSmallPhases);
    }


    internal static List<PhaseData> GetPhasesByCast(ParsedEvtcLog log, IEnumerable<long> skillIDs, SingleActor mainTarget, bool addSkipPhases, bool mainBetweenCasts, long start, long end, bool filterSmallPhases = true)
    {
        long last = start;
        var casts = mainTarget.GetAnimatedCastEvents(log, start, end);
        var invuls = casts
            .Where(x => skillIDs.Contains(x.SkillID))
            .ToList();
        invuls.SortByTime(); // Sort in case there were multiple skillIDs

        var phases = new List<PhaseData>(invuls.Count);
        bool nextToAddIsSkipPhase = !mainBetweenCasts;
        foreach (CastEvent c in invuls)
        {
            long endTime = c.EndTime;
            if (c.IsUnknown && !casts.Any(x => x.Time >= endTime))
            {
                endTime = end;
            }
            if (mainBetweenCasts)
            {
                phases.Add(new SubPhasePhaseData(last, c.Time));
                if (addSkipPhases) {
                    phases.Add(new SubPhasePhaseData(c.Time, endTime));
                }
            } 
            else
            {
                if (addSkipPhases)
                {
                    phases.Add(new SubPhasePhaseData(last, c.Time));
                }
                phases.Add(new SubPhasePhaseData(c.Time, endTime));
            }
            last = endTime;
        }
        if (!nextToAddIsSkipPhase || (nextToAddIsSkipPhase && addSkipPhases))
        {
            phases.Add(new SubPhasePhaseData(last, end));
        }
        long filterThreshold = filterSmallPhases ? 100 : 0;
        return phases.Where(x => x.DurationInMS > filterThreshold).ToList(); // only filter unrealistically short phases, otherwise it may mess with phase names
    }

    internal static List<PhaseData> GetPhasesByCast(ParsedEvtcLog log, long skillID, SingleActor mainTarget, bool addSkipPhases, bool mainBetweenCast, long start, long end, bool filterSmallPhases = true)
    {
        return GetPhasesByCast(log, [skillID], mainTarget, addSkipPhases, mainBetweenCast, start, end, filterSmallPhases);
    }
    internal static List<PhaseData> GetPhasesByCast(ParsedEvtcLog log, long skillID, SingleActor mainTarget, bool addSkipPhases, bool mainBetweenCast, bool filterSmallPhases = true)
    {
        return GetPhasesByCast(log, skillID, mainTarget, addSkipPhases, mainBetweenCast, log.LogData.LogStart, log.LogData.LogEnd, filterSmallPhases);
    }

    internal static List<PhaseData> GetInitialPhase(ParsedEvtcLog log)
    {
        if (log.LogData.Logic.IsInstance)
        {
            return
            [
                new InstancePhaseData(log.LogData.LogStart, log.LogData.LogEnd, "Full Instance", log)
            ];
        }
        return
        [
            new EncounterPhaseData(log.LogData.LogStart, log.LogData.LogEnd, "Full Fight", log)
        ];
    }
    #region INSTANCE PHASES

    internal delegate LogData.LogMode LogModeChecker(ParsedEvtcLog log, SingleActor target);
    internal delegate LogData.LogStartStatus LogStartStatusChecker(SingleActor target, long time, CombatData combatData, double expectedInitialPercent = 100.0);

    internal static LogData.LogStartStatus DefaultLogStartStatusChecker(SingleActor? target, long time, CombatData combatData, double expectedInitialPercent = 100.0)
    {
        if (TargetHPPercentUnderThreshold(target, time, combatData, expectedInitialPercent))
        {
            return LogData.LogStartStatus.Late;
        }
        return LogData.LogStartStatus.Normal;
    }

    internal static LogData.LogStartStatus DefaultLogStartStatusChecker(IEnumerable<SingleActor?> targets, long time, CombatData combatData, double expectedInitialPercent = 100.0)
    {
        if (targets.Any(target => TargetHPPercentUnderThreshold(target, time, combatData, expectedInitialPercent)))
        {
            return LogData.LogStartStatus.Late;
        }
        return LogData.LogStartStatus.Normal;
    }
    #region INSTANCE ENCOUNTER
    internal static PhaseData? AddInstanceEncounterPhase(ParsedEvtcLog log, List<PhaseData> phases, List<EncounterPhaseData> encounterPhases, IEnumerable<SingleActor?> targets, IEnumerable<SingleActor?> blockingBosses, IEnumerable<SingleActor?> nonBlockingBosses, PhaseData instancePhase, string phaseName, long start, long end, bool success, string icon, long encounterID, LogData.LogMode logMode, LogData.LogStartStatus logStartStatus)
    {
        if (!success && end - start < log.ParserSettings.TooShortLimit)
        {
            return null;
        }
        var phase = new EncounterPhaseData(start, end, phaseName, success, icon, logMode, logStartStatus, encounterID);
        phases.Add(phase);
        encounterPhases.Add(phase);
        phase.AddParentPhase(instancePhase);
        phase.AddTargets(targets, log);
        phase.AddTargets(blockingBosses, log, PhaseData.TargetPriority.Blocking);
        phase.AddTargets(nonBlockingBosses, log, PhaseData.TargetPriority.NonBlocking);
        instancePhase.AddTargets(targets.Where(x => x != null && !x.IsSpecies(TargetID.DummyTarget)), log);
        return phase;
    }
    internal static PhaseData? AddInstanceEncounterPhase(ParsedEvtcLog log, List<PhaseData> phases, List<EncounterPhaseData> encounterPhases, IEnumerable<SingleActor?> targets, IEnumerable<SingleActor?> blockingBosses, IEnumerable<SingleActor?> nonBlockingBosses, PhaseData instancePhase, string phaseName, long start, long end, bool success, LogLogic encounterLogic, LogData.LogMode logMode, LogData.LogStartStatus logStartStatus)
    {

        return AddInstanceEncounterPhase(log, phases, encounterPhases, targets, blockingBosses, nonBlockingBosses, instancePhase, phaseName, start, end, success, encounterLogic.Icon, encounterLogic.LogID, logMode, logStartStatus);
    }

    internal static PhaseData? AddInstanceEncounterPhase(ParsedEvtcLog log, List<PhaseData> phases, List<EncounterPhaseData> encounterPhases, IEnumerable<SingleActor?> targets, IEnumerable<SingleActor?> blockingBosses, IEnumerable<SingleActor?> nonBlockingBosses, PhaseData instancePhase, string phaseName, long start, long end, bool success, LogLogic encounterLogic)
    {

        return AddInstanceEncounterPhase(log, phases, encounterPhases, targets, blockingBosses, nonBlockingBosses, instancePhase, phaseName, start, end, success, encounterLogic.Icon, encounterLogic.LogID, LogData.LogMode.Normal, DefaultLogStartStatusChecker(targets, start, log.CombatData));
    }

    internal static PhaseData? AddInstanceEncounterPhase(ParsedEvtcLog log, List<PhaseData> phases, List<EncounterPhaseData> encounterPhases, IEnumerable<SingleActor?> targets, IEnumerable<SingleActor?> blockingBosses, IEnumerable<SingleActor?> nonBlockingBosses, PhaseData instancePhase, string phaseName, long start, long end, bool success, LogLogic encounterLogic, LogData.LogMode logMode)
    {

        return AddInstanceEncounterPhase(log, phases, encounterPhases, targets, blockingBosses, nonBlockingBosses, instancePhase, phaseName, start, end, success, encounterLogic.Icon, encounterLogic.LogID, logMode, DefaultLogStartStatusChecker(targets, start, log.CombatData));
    }

    internal static PhaseData? AddInstanceEncounterPhase(ParsedEvtcLog log, List<PhaseData> phases, List<EncounterPhaseData> encounterPhases, IEnumerable<SingleActor?> targets, IEnumerable<SingleActor?> blockingBosses, IEnumerable<SingleActor?> nonBlockingBosses, PhaseData instancePhase, string phaseName, long start, long end, bool success, LogLogic encounterLogic, LogData.LogStartStatus logStartStatus)
    {

        return AddInstanceEncounterPhase(log, phases, encounterPhases, targets, blockingBosses, nonBlockingBosses, instancePhase, phaseName, start, end, success, encounterLogic.Icon, encounterLogic.LogID, LogData.LogMode.Normal, logStartStatus);
    }

    internal static PhaseData? AddInstanceEncounterPhase(ParsedEvtcLog log, List<PhaseData> phases, List<EncounterPhaseData> encounterPhases, IEnumerable<SingleActor?> targets, IEnumerable<SingleActor?> blockingBosses, IEnumerable<SingleActor?> nonBlockingBosses, PhaseData instancePhase, string phaseName, long start, long end, bool success, string icon, long encounterID)
    {
        return AddInstanceEncounterPhase(log, phases, encounterPhases, targets, blockingBosses, nonBlockingBosses, instancePhase, phaseName, start, end, success, icon, encounterID, LogData.LogMode.Normal, DefaultLogStartStatusChecker(targets, start, log.CombatData));
    }

    internal static PhaseData? AddInstanceEncounterPhase(ParsedEvtcLog log, List<PhaseData> phases, List<EncounterPhaseData> encounterPhases, IEnumerable<SingleActor?> targets, IEnumerable<SingleActor?> blockingBosses, IEnumerable<SingleActor?> nonBlockingBosses, PhaseData instancePhase, string phaseName, long start, long end, bool success, string icon, long encounterID, LogData.LogMode logMode)
    {
        return AddInstanceEncounterPhase(log, phases, encounterPhases, targets, blockingBosses, nonBlockingBosses, instancePhase, phaseName, start, end, success, icon, encounterID, logMode, DefaultLogStartStatusChecker(targets, start, log.CombatData));
    }
    internal static PhaseData? AddInstanceEncounterPhase(ParsedEvtcLog log, List<PhaseData> phases, List<EncounterPhaseData> encounterPhases, IEnumerable<SingleActor?> targets, IEnumerable<SingleActor?> blockingBosses, IEnumerable<SingleActor?> nonBlockingBosses, PhaseData instancePhase, string phaseName, long start, long end, bool success, string icon, long encounterID, LogData.LogStartStatus logStartStatus)
    {
        return AddInstanceEncounterPhase(log, phases, encounterPhases, targets, blockingBosses, nonBlockingBosses, instancePhase, phaseName, start, end, success, icon, encounterID, LogData.LogMode.Normal, logStartStatus);
    }
    #endregion INSTANCE ENCOUNTER

    #region INSTANCE ENCOUNTERS
    internal static List<EncounterPhaseData> ProcessGenericEncounterPhasesForInstance(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, ParsedEvtcLog log, List<PhaseData> phases, TargetID targetID, IEnumerable<SingleActor> blockingBosses, ChestID chestID, string phaseName, string icon, long encounterID, LogModeChecker? fightModeChecker, LogStartStatusChecker? fightStartStatusChecker)
    {
        if (chestID == ChestID.None)
        {
            throw new InvalidOperationException("ProcessGenericEncounterPhasesForInstance requires a chest ID");
        }
        var mainPhase = phases[0];
        var encounterPhases = new List<EncounterPhaseData>();
        if (targetsByIDs.TryGetValue((int)targetID, out var targets))
        {
            var chest = log.AgentData.GetGadgetsByID(chestID).FirstOrDefault();
            foreach (var target in targets)
            {
                var enterCombat = log.CombatData.GetEnterCombatEvents(target.AgentItem).FirstOrDefault();
                if (enterCombat == null && !log.CombatData.GetDamageTakenData(target.AgentItem).Any(x => x.HealthDamage > 0 && x.CreditedFrom.IsPlayer))
                {
                    continue;
                }
                long start = enterCombat != null ? enterCombat.Time : target.FirstAware;
                bool success = false;
                long end = target.LastAware; 
                if (chest != null && chest.InAwareTimes(end + 500))
                {
                    end = chest.FirstAware;
                    success = true;
                }
                AddInstanceEncounterPhase(log, phases, encounterPhases, [target], blockingBosses, [], mainPhase, phaseName, start, end, success, icon, encounterID, fightModeChecker != null ? fightModeChecker(log, target) : LogData.LogMode.Normal, fightStartStatusChecker != null ? fightStartStatusChecker(target, start, log.CombatData) : DefaultLogStartStatusChecker(target, start, log.CombatData));
            }
        }
        NumericallyRenameEncounterPhases(encounterPhases);
        return encounterPhases;
    }

    internal static List<EncounterPhaseData> ProcessGenericEncounterPhasesForInstance(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, ParsedEvtcLog log, List<PhaseData> phases, TargetID targetID, IEnumerable<SingleActor> blockingBosses, ChestID chestID, string phaseName, string icon, long encounterID)
    {
        return ProcessGenericEncounterPhasesForInstance(targetsByIDs, log, phases, targetID, blockingBosses, chestID, phaseName, icon, encounterID, null, null);
    }
    internal static List<EncounterPhaseData> ProcessGenericEncounterPhasesForInstance(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, ParsedEvtcLog log, List<PhaseData> phases, TargetID targetID, IEnumerable<SingleActor> blockingBosses, ChestID chestID, string phaseName, string icon, long encounterID, LogStartStatusChecker? fightStartStatusChecker)
    {
        return ProcessGenericEncounterPhasesForInstance(targetsByIDs, log, phases, targetID, blockingBosses, chestID, phaseName, icon, encounterID, null, fightStartStatusChecker);
    }
    internal static List<EncounterPhaseData> ProcessGenericEncounterPhasesForInstance(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, ParsedEvtcLog log, List<PhaseData> phases, TargetID targetID, IEnumerable<SingleActor> blockingBosses, ChestID chestID, string phaseName, string icon, long encounterID, LogModeChecker? fightModeChecker)
    {
        return ProcessGenericEncounterPhasesForInstance(targetsByIDs, log, phases, targetID, blockingBosses, chestID, phaseName, icon, encounterID, fightModeChecker, null);
    }

    internal static List<EncounterPhaseData> ProcessGenericEncounterPhasesForInstance(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, ParsedEvtcLog log, List<PhaseData> phases, TargetID targetID, IEnumerable<SingleActor> blockingBosses, string phaseName, LogLogic encounterLogic, LogModeChecker? fightModeChecker, LogStartStatusChecker? fightStartStatusChecker )
    {
        return ProcessGenericEncounterPhasesForInstance(targetsByIDs, log, phases, targetID, blockingBosses, encounterLogic.ChestID, phaseName, encounterLogic.Icon, encounterLogic.LogID, fightModeChecker, fightStartStatusChecker);
    }
    internal static List<EncounterPhaseData> ProcessGenericEncounterPhasesForInstance(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, ParsedEvtcLog log, List<PhaseData> phases, TargetID targetID, IEnumerable<SingleActor> blockingBosses, string phaseName, LogLogic encounterLogic)
    {
        return ProcessGenericEncounterPhasesForInstance(targetsByIDs, log, phases, targetID, blockingBosses, encounterLogic.ChestID, phaseName, encounterLogic.Icon, encounterLogic.LogID, null, null);
    }
    internal static List<EncounterPhaseData> ProcessGenericEncounterPhasesForInstance(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, ParsedEvtcLog log, List<PhaseData> phases, TargetID targetID, IEnumerable<SingleActor> blockingBosses, string phaseName, LogLogic encounterLogic, LogStartStatusChecker? fightStartStatusChecker = null)
    {
        return ProcessGenericEncounterPhasesForInstance(targetsByIDs, log, phases, targetID, blockingBosses, encounterLogic.ChestID, phaseName, encounterLogic.Icon, encounterLogic.LogID, null, fightStartStatusChecker);
    }
    internal static List<EncounterPhaseData> ProcessGenericEncounterPhasesForInstance(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, ParsedEvtcLog log, List<PhaseData> phases, TargetID targetID, IEnumerable<SingleActor> blockingBosses, string phaseName, LogLogic encounterLogic, LogModeChecker? fightModeChecker = null)
    {
        return ProcessGenericEncounterPhasesForInstance(targetsByIDs, log, phases, targetID, blockingBosses, encounterLogic.ChestID, phaseName, encounterLogic.Icon, encounterLogic.LogID, fightModeChecker, null);
    }
    #endregion INSTANCE ENCOUNTERS
    internal static void NumericallyRenameEncounterPhases(IReadOnlyList<EncounterPhaseData> phases)
    {
        if (phases.Count > 1)
        {
            for (int i = 0; i < phases.Count; i++)
            {
                phases[i].Name += " " + (i + 1);
                phases[i].NameNoMode += " " + (i + 1);
            }
        }
    }
    #endregion INSTANCE PHASES
}
