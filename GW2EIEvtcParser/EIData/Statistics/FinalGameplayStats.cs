using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.Buff;

namespace GW2EIEvtcParser.EIData;

public class FinalGameplayStats
{
    // Rates
    public readonly int Wasted;
    public readonly double TimeWasted;
    public readonly int Saved;
    public readonly double TimeSaved;
    public readonly double StackDist;
    public readonly double DistToCom;

    // boons
    public readonly double AvgBoons;
    public readonly double AvgActiveBoons;
    public readonly double AvgConditions;
    public readonly double AvgActiveConditions;

    // Counts
    public readonly int SwapCount;

    public readonly double SkillCastUptime;
    public readonly double SkillCastUptimeNoAA;

    private static double GetDistanceToTarget(SingleActor actor, ParsedEvtcLog log, long start, long end, IReadOnlyList<ParametricPoint3D?> reference)
    {

        var positions = actor.GetCombatReplayPolledPositions(log).Where(x => x.Time >= start && x.Time <= end).ToList();
        int offset = actor.GetCombatReplayPolledPositions(log).Count(x => x.Time < start);
        if (positions.Count > 1 && reference.Count > 0)
        {
            var distances = new List<float>();
            //TODO(Rennorb) @cleanup: Dual indexing requires us to generate sparse lists with the same amount of entries and null values. 
            // These are already parametric points and they are sorted, we don't need to fill up the list to equal lengths. Investigate perf. 
            for (int time = 0; time < positions.Count; time++)
            {
                if (time + offset >= reference.Count || reference[time + offset] == null)
                {
                    continue;
                }

                distances.Add((positions[time].XYZ - reference[time + offset]!.Value.XYZ).XY().Length());
            }
            return distances.Count == 0 ? -1 : distances.Sum() / distances.Count;
        }
        else
        {
            return -1;
        }
    }

    internal FinalGameplayStats(ParsedEvtcLog log, long start, long end, SingleActor actor)
    {
        // If dummy actor, stop
        if (actor.IsFakeActor)
        {
            return;
        }
        long duration = end - start;
        foreach (CastEvent cl in actor.GetCastEvents(log, start, end))
        {
            switch (cl.Status)
            {
                case CastEvent.AnimationStatus.Interrupted:
                    Wasted++;
                    TimeWasted += cl.SavedDuration;
                    break;
                case CastEvent.AnimationStatus.Reduced:
                    Saved++;
                    TimeSaved += cl.SavedDuration;
                    break;
            }
            if (cl.Skill.IsSwap)
            {
                SwapCount++;
            }
        }
        TimeSaved = Math.Round(TimeSaved / 1000.0, ParserHelper.TimeDigit);
        TimeWasted = -Math.Round(TimeWasted / 1000.0, ParserHelper.TimeDigit);
        //
        foreach (CastEvent cl in actor.GetIntersectingCastEvents(log, start, end))
        {
            if (cl.Status == CastEvent.AnimationStatus.Interrupted || cl.Status == CastEvent.AnimationStatus.Unknown)
            {
                continue;
            }
            long value = Math.Min(cl.EndTime, end) - Math.Max(cl.Time, start);
            SkillCastUptime += value;
            if (!cl.Skill.IsAutoAttack(log))
            {
                SkillCastUptimeNoAA += value;
            }
        }
        long timeInCombat = Math.Max(actor.GetTimeSpentInCombat(log, start, end), 1);
        SkillCastUptime /= timeInCombat;
        SkillCastUptimeNoAA /= timeInCombat;
        SkillCastUptime = Math.Round(100.0 * SkillCastUptime, ParserHelper.TimeDigit);
        SkillCastUptimeNoAA = Math.Round(100.0 * SkillCastUptimeNoAA, ParserHelper.TimeDigit);
        //
        double avgBoons = 0;
        foreach (long boonDuration in actor.GetBuffPresence(log, start, end).Where(x => log.Buffs.BuffsByIds[x.Key].Classification == BuffClassification.Boon).Select(x => x.Value))
        {
            avgBoons += boonDuration;
        }
        AvgBoons = Math.Round(avgBoons / duration, ParserHelper.BuffDigit);
        long activeDuration = actor.GetActiveDuration(log, start, end);
        AvgActiveBoons = activeDuration > 0 ? Math.Round(avgBoons / activeDuration, ParserHelper.BuffDigit) : 0.0;
        //
        double avgCondis = 0;
        foreach (long conditionDuration in actor.GetBuffPresence(log, start, end).Where(x => log.Buffs.BuffsByIds[x.Key].Classification == BuffClassification.Condition).Select(x => x.Value))
        {
            avgCondis += conditionDuration;
        }
        AvgConditions = Math.Round(avgCondis / duration, ParserHelper.BuffDigit);
        AvgActiveConditions = activeDuration > 0 ? Math.Round(avgCondis / activeDuration, ParserHelper.BuffDigit) : 0.0;
        //
        if (log.CombatData.HasMovementData && log.FriendlyAgents.Contains(actor.AgentItem) && actor.HasCombatReplayPositions(log))
        {
            StackDist = GetDistanceToTarget(actor, log, start, end, log.StatisticsHelper.GetStackCenterPositions(log));
            DistToCom = GetDistanceToTarget(actor, log, start, end, log.StatisticsHelper.GetStackCommanderPositions(log));
        }
    }
}
