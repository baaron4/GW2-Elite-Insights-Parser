using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal static class EncounterLogicPhaseUtils
    {

        internal static List<PhaseData> GetPhasesByHealthPercent(ParsedEvtcLog log, AbstractSingleActor mainTarget, IReadOnlyList<double> thresholds)
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
                HealthUpdateEvent evt = hpUpdates.FirstOrDefault(x => x.HealthPercent <= thresholds[i]);
                if (evt == null)
                {
                    break;
                }
                var phase = new PhaseData(start, Math.Min(evt.Time, fightEnd), (offset + thresholds[i]) + "% - " + thresholds[i] + "%");
                phase.AddTarget(mainTarget);
                phases.Add(phase);
                start = Math.Max(evt.Time, log.FightData.FightStart);
            }
            if (phases.Count > 0 && phases.Count < thresholds.Count)
            {
                var lastPhase = new PhaseData(start, fightEnd, (offset + thresholds[phases.Count]) + "% -" + thresholds[phases.Count] + "%");
                lastPhase.AddTarget(mainTarget);
                phases.Add(lastPhase);
            }
            return phases;
        }

        internal static List<PhaseData> GetPhasesByInvul(ParsedEvtcLog log, IEnumerable<long> skillIDs, AbstractSingleActor mainTarget, bool addSkipPhases, bool beginWithStart, long start, long end)
        {
            var phases = new List<PhaseData>();
            long last = start;
            List<AbstractBuffEvent> invuls = GetFilteredList(log.CombatData, skillIDs, mainTarget, beginWithStart, true);
            invuls.RemoveAll(x => x.Time < 0);
            invuls.Sort((event1, event2) => event1.Time.CompareTo(event2.Time)); // Sort in case there were multiple skillIDs
            bool nextToAddIsSkipPhase = !beginWithStart;
            foreach (AbstractBuffEvent c in invuls)
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

        internal static List<PhaseData> GetPhasesByInvul(ParsedEvtcLog log, long skillID, AbstractSingleActor mainTarget, bool addSkipPhases, bool beginWithStart, long start, long end)
        {
            return GetPhasesByInvul(log, new[] { skillID }, mainTarget, addSkipPhases, beginWithStart, start, end);
        }

        internal static List<PhaseData> GetPhasesByInvul(ParsedEvtcLog log, long skillID, AbstractSingleActor mainTarget, bool addSkipPhases, bool beginWithStart)
        {
            return GetPhasesByInvul(log, skillID, mainTarget, addSkipPhases, beginWithStart, log.FightData.FightStart, log.FightData.FightEnd);
        }

        internal static List<PhaseData> GetInitialPhase(ParsedEvtcLog log)
        {
            return new List<PhaseData>
            {
                new PhaseData(log.FightData.FightStart, log.FightData.FightEnd, "Full Fight")
                {
                    CanBeSubPhase = false
                }
            };
        }
    }
}
