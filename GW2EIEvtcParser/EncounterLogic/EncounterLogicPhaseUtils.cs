using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal static class EncounterLogicPhaseUtils
{
    internal static List<PhaseData> GetPhasesBySquadCombatStartEnd(ParsedEvtcLog log)
    {
        var phases = new List<PhaseData>();
        int sequence = 1;
        foreach (var startEvent in log.CombatData.GetSquadCombatStartEvents())
        {
            var logEndEvent = log.CombatData.GetSquadCombatEndEvents().FirstOrDefault(x => x.ServerUnixTimeStamp >= startEvent.ServerUnixTimeStamp);
            if (logEndEvent != null)
            {
                var fightPhase = new EncounterPhaseData(startEvent.Time, logEndEvent.Time, "Fight " + (sequence++), true, log.FightData.Logic.Icon, FightData.EncounterMode.Normal);
                phases.Add(fightPhase);
            }
            else
            {
                var fightPhase = new EncounterPhaseData(startEvent.Time, phases[0].End, "Fight " + (sequence++), true, log.FightData.Logic.Icon, FightData.EncounterMode.Normal);
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
        if (!filterSmallPhases)
        {
            return phases;
        }
        return phases.Where(x => x.DurationInMS > 100).ToList(); // only filter unrealistically short phases, otherwise it may mess with phase names
    }


    internal static List<PhaseData> GetPhasesByInvul(ParsedEvtcLog log, IEnumerable<long> skillIDs, SingleActor mainTarget, bool addSkipPhases, bool beginWithStart, bool filterSmallPhases = true)
    {
        return GetPhasesByInvul(log, skillIDs, mainTarget, addSkipPhases, beginWithStart, log.FightData.FightStart, log.FightData.FightEnd, filterSmallPhases);
    }

    internal static List<PhaseData> GetPhasesByInvul(ParsedEvtcLog log, long skillID, SingleActor mainTarget, bool addSkipPhases, bool beginWithStart, long start, long end, bool filterSmallPhases = true)
    {
        return GetPhasesByInvul(log, [ skillID ], mainTarget, addSkipPhases, beginWithStart, start, end, filterSmallPhases);
    }

    internal static List<PhaseData> GetPhasesByInvul(ParsedEvtcLog log, long skillID, SingleActor mainTarget, bool addSkipPhases, bool beginWithStart, bool filterSmallPhases = true)
    {
        return GetPhasesByInvul(log, skillID, mainTarget, addSkipPhases, beginWithStart, log.FightData.FightStart, log.FightData.FightEnd, filterSmallPhases);
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
                phases.Add(new PhaseData(last, c.Time));
                if (addSkipPhases) {
                    phases.Add(new PhaseData(c.Time, endTime));
                }
            } 
            else
            {
                if (addSkipPhases)
                {
                    phases.Add(new PhaseData(last, c.Time));
                }
                phases.Add(new PhaseData(c.Time, endTime));
            }
            last = endTime;
        }
        if (!nextToAddIsSkipPhase || (nextToAddIsSkipPhase && addSkipPhases))
        {
            phases.Add(new PhaseData(last, end));
        }
        if (!filterSmallPhases)
        {
            return phases;
        }
        return phases.Where(x => x.DurationInMS > 100).ToList(); // only filter unrealistically short phases, otherwise it may mess with phase names
    }

    internal static List<PhaseData> GetPhasesByCast(ParsedEvtcLog log, long skillID, SingleActor mainTarget, bool addSkipPhases, bool mainBetweenCast, long start, long end, bool filterSmallPhases = true)
    {
        return GetPhasesByCast(log, [skillID], mainTarget, addSkipPhases, mainBetweenCast, start, end, filterSmallPhases);
    }
    internal static List<PhaseData> GetPhasesByCast(ParsedEvtcLog log, long skillID, SingleActor mainTarget, bool addSkipPhases, bool mainBetweenCast, bool filterSmallPhases = true)
    {
        return GetPhasesByCast(log, skillID, mainTarget, addSkipPhases, mainBetweenCast, log.FightData.FightStart, log.FightData.FightEnd, filterSmallPhases);
    }

    internal static List<PhaseData> GetInitialPhase(ParsedEvtcLog log)
    {
        if (log.FightData.Logic.IsInstance)
        {
            return
            [
                new InstancePhaseData(log.FightData.FightStart, log.FightData.FightEnd, "Full Instance", log)
            ];
        }
        return
        [
            new EncounterPhaseData(log.FightData.FightStart, log.FightData.FightEnd, "Full Fight", log)
        ];
    }

    internal static PhaseData AddInstanceEncounterPhase(ParsedEvtcLog log, List<PhaseData> phases, List<PhaseData> encounterPhases, IEnumerable<SingleActor?> targets, IEnumerable<SingleActor?> blockingBosses, IEnumerable<SingleActor?> nonBlockingBosses, PhaseData instancePhase, string phaseName, long start, long end, bool success, string icon, FightData.EncounterMode fightMode = FightData.EncounterMode.Normal)
    {

        var phase = new EncounterPhaseData(start, end, phaseName, success, icon, fightMode);
        phases.Add(phase);
        encounterPhases.Add(phase);
        phase.AddParentPhase(instancePhase);
        phase.AddTargets(targets, log);
        phase.AddTargets(blockingBosses, log, PhaseData.TargetPriority.Blocking);
        phase.AddTargets(nonBlockingBosses, log, PhaseData.TargetPriority.NonBlocking);
        instancePhase.AddTargets(targets.Where(x => x != null && !x.IsSpecies(TargetID.DummyTarget)), log);
        return phase;
    }

    internal delegate FightData.EncounterMode FightModeChecker(ParsedEvtcLog log, SingleActor target);
    internal static void ProcessGenericEncounterPhasesForInstance(IReadOnlyDictionary<int, List<SingleActor>> targetsByIDs, ParsedEvtcLog log, List<PhaseData> phases, TargetID targetID, IEnumerable<SingleActor> blockingBosses, ChestID chestID, string phaseName, string icon, FightModeChecker? fightModeChecker = null)
    {
        var mainPhase = phases[0];
        var encounterPhases = new List<PhaseData>();
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
                AddInstanceEncounterPhase(log, phases, encounterPhases, [target], blockingBosses, [], mainPhase, phaseName, start, end, success, icon, fightModeChecker != null ? fightModeChecker(log, target) : FightData.EncounterMode.Normal);
            }
        }
        NumericallyRenamePhases(encounterPhases);
    }

    internal static void NumericallyRenamePhases(IReadOnlyList<PhaseData> phases)
    {
        if (phases.Count > 1)
        {
            for (int i = 0; i < phases.Count; i++)
            {
                phases[i].Name += " " + (i + 1);
            }
        }
    }
}
