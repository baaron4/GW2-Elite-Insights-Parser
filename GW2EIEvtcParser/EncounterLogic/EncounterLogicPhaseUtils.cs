using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal static class EncounterLogicPhaseUtils
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
            if (dead != null)
            {
                end = dead.Time;
            }
            var phase = new PhaseData(Math.Max(log.FightData.FightStart, start), Math.Min(target.LastAware, end), target.Character);
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
                var fightPhase = new PhaseData(startEvent.Time, logEndEvent.Time, "Fight " + (sequence++));
                phases.Add(fightPhase);
            }
            else
            {
                var fightPhase = new PhaseData(startEvent.Time, phases[0].End, "Fight " + (sequence++));
                phases.Add(fightPhase);
                break;
            }
        }
        return phases;
    }

    internal static List<PhaseData> GetPhasesByHealthPercent(ParsedEvtcLog log, SingleActor mainTarget, IReadOnlyList<double> thresholds)
    {
        var phases = new List<PhaseData>();
        if (thresholds.Count == 0)
        {
            return phases;
        }
        long fightEnd = log.FightData.FightEnd;
        long start = log.FightData.FightStart;
        double offset = 100.0 / thresholds.Count;
        IReadOnlyList<HealthUpdateEvent> hpUpdates = log.CombatData.GetHealthUpdateEvents(mainTarget.AgentItem);
        for (int i = 0; i < thresholds.Count; i++)
        {
            HealthUpdateEvent? evt = hpUpdates.FirstOrDefault(x => x.HealthPercent <= thresholds[i]);
            if (evt == null)
            {
                break;
            }
            var phase = new PhaseData(start, Math.Min(evt.Time, fightEnd), (offset + thresholds[i]) + "% - " + thresholds[i] + "%");
            phase.AddTarget(mainTarget, log);
            phases.Add(phase);
            start = Math.Max(evt.Time, log.FightData.FightStart);
        }
        if (phases.Count > 0 && phases.Count < thresholds.Count)
        {
            var lastPhase = new PhaseData(start, fightEnd, (offset + thresholds[phases.Count]) + "% -" + thresholds[phases.Count] + "%");
            lastPhase.AddTarget(mainTarget, log);
            phases.Add(lastPhase);
        }
        return phases;
    }

    internal static List<PhaseData> GetPhasesByInvul(ParsedEvtcLog log, IEnumerable<long> skillIDs, SingleActor mainTarget, bool addSkipPhases, bool beginWithStart, long start, long end)
    {
        long last = start;
        var invuls = GetFilteredList(log.CombatData, skillIDs, mainTarget, beginWithStart, true)
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
                phases.Add(new PhaseData(last, curEnd));
                last = curEnd;
                nextToAddIsSkipPhase = true;
            }
            else
            {
                long curEnd = Math.Min(c.Time, end);
                if (addSkipPhases)
                {
                    phases.Add(new PhaseData(last, curEnd));
                }
                last = curEnd;
                nextToAddIsSkipPhase = false;
            }
        }
        if (!nextToAddIsSkipPhase || (nextToAddIsSkipPhase && addSkipPhases))
        {
            phases.Add(new PhaseData(last, end));
        }
        return phases.Where(x => x.DurationInMS > 100).ToList(); // only filter unrealistically short phases, otherwise it may mess with phase names
    }


    internal static List<PhaseData> GetPhasesByInvul(ParsedEvtcLog log, IEnumerable<long> skillIDs, SingleActor mainTarget, bool addSkipPhases, bool beginWithStart)
    {
        return GetPhasesByInvul(log, skillIDs, mainTarget, addSkipPhases, beginWithStart, log.FightData.FightStart, log.FightData.FightEnd);
    }

    internal static List<PhaseData> GetPhasesByInvul(ParsedEvtcLog log, long skillID, SingleActor mainTarget, bool addSkipPhases, bool beginWithStart, long start, long end)
    {
        return GetPhasesByInvul(log, [ skillID ], mainTarget, addSkipPhases, beginWithStart, start, end);
    }

    internal static List<PhaseData> GetPhasesByInvul(ParsedEvtcLog log, long skillID, SingleActor mainTarget, bool addSkipPhases, bool beginWithStart)
    {
        return GetPhasesByInvul(log, skillID, mainTarget, addSkipPhases, beginWithStart, log.FightData.FightStart, log.FightData.FightEnd);
    }


    internal static List<PhaseData> GetPhasesByCast(ParsedEvtcLog log, IEnumerable<long> skillIDs, SingleActor mainTarget, bool addSkipPhases, bool mainBetweenCasts, long start, long end)
    {
        long last = start;
        var invuls = mainTarget.GetCastEvents(log, start, end)
            .Where(x => skillIDs.Contains(x.SkillId))
            .ToList();
        invuls.SortByTime(); // Sort in case there were multiple skillIDs

        var phases = new List<PhaseData>(invuls.Count);
        bool nextToAddIsSkipPhase = !mainBetweenCasts;
        foreach (CastEvent c in invuls)
        {
            if (mainBetweenCasts)
            {
                phases.Add(new PhaseData(last, c.Time));
                if (addSkipPhases) {
                    phases.Add(new PhaseData(c.Time, c.EndTime));
                }
            } 
            else
            {
                if (addSkipPhases)
                {
                    phases.Add(new PhaseData(last, c.Time));
                }
                phases.Add(new PhaseData(c.Time, c.EndTime));
            }
            last = c.EndTime;
        }
        if (!nextToAddIsSkipPhase || (nextToAddIsSkipPhase && addSkipPhases))
        {
            phases.Add(new PhaseData(last, end));
        }
        return phases.Where(x => x.DurationInMS > 100).ToList(); // only filter unrealistically short phases, otherwise it may mess with phase names
    }

    internal static List<PhaseData> GetPhasesByCast(ParsedEvtcLog log, long skillID, SingleActor mainTarget, bool addSkipPhases, bool mainBetweenCast, long start, long end)
    {
        return GetPhasesByCast(log, [skillID], mainTarget, addSkipPhases, mainBetweenCast, start, end);
    }
    internal static List<PhaseData> GetPhasesByCast(ParsedEvtcLog log, long skillID, SingleActor mainTarget, bool addSkipPhases, bool mainBetweenCast)
    {
        return GetPhasesByCast(log, skillID, mainTarget, addSkipPhases, mainBetweenCast, log.FightData.FightStart, log.FightData.FightEnd);
    }

    internal static List<PhaseData> GetInitialPhase(ParsedEvtcLog log)
    {
        if (log.FightData.Logic.IsInstance)
        {
            return
            [
                new PhaseData(log.FightData.FightStart, log.FightData.FightEnd, "Full Instance")
            ];
        }
        return
        [
            new PhaseData(log.FightData.FightStart, log.FightData.FightEnd, "Full Fight")
        ];
    }

    internal delegate bool CMChecker(ParsedEvtcLog log, SingleActor target);
    internal static void ProcessGenericEncounterPhasesForInstance(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, ParsedEvtcLog log, List<PhaseData> phases, TargetID targetID, IEnumerable<SingleActor> blockingBosses, ChestID chestID, string phaseName, CMChecker? cmChecker = null)
    {
        if (targetsByIDs.TryGetValue((int)targetID, out var targets))
        {
            bool hasMultiple = targets.Count > 1;
            int encounterCount = 1;
            var lastTarget = targets.Last();
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
                if (target == lastTarget && chest != null)
                {
                    end = chest.FirstAware;
                    success = true;
                }
                var phase = new PhaseData(start, end, phaseName);
                phases.Add(phase);
                if (cmChecker != null && cmChecker(log, target))
                {
                    phase.Name += " CM";
                }
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
                phase.AddParentPhase(phases[0]);
                phase.AddTarget(target, log);
                phase.AddTargets(blockingBosses, log, PhaseData.TargetPriority.Blocking);
                phases[0].AddTarget(target, log);
            }
        }
    }
}
