using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.Buff;

namespace GW2EIEvtcParser.EIData;

public class GameplayStatistics
{
    // Rates
    public readonly int SkillAnimationInterruptedCount;
    public readonly double SkillAnimationInterruptedDuration;
    public readonly int SkillAnimationAfterCastInterruptedCount;
    public readonly double SkillAnimationAfterCastInterruptedDuration;
    public readonly double DistanceToCenterOfSquad;
    public readonly double DistanceToCommander;

    // boons
    public readonly double AverageBoons;
    public readonly double AverageActiveBoons;
    public readonly double AverageConditions;
    public readonly double AverageActiveConditions;

    // Counts
    public readonly int WeaponSwapCount;

    public readonly double SkillCastUptime;
    public readonly double SkillCastUptimeNoAutoAttack;

    private static double GetDistanceToTarget(SingleActor actor, ParsedEvtcLog log, long start, long end, IReadOnlyList<ParametricPoint3D?> references)
    {

        var positions = actor.GetCombatReplayPolledPositions(log).Where(x => x.Time >= start && x.Time <= end).ToList();
        if (positions.Count > 0 && references.Count > 0)
        {
            var firstPos = positions[0];
            var lastPos = positions[^1];
            long firstTime = 0;
            long firstIndex = 0;
            var distances = new List<float>();
            for (int time = 0; time < references.Count; time++)
            {
                var referencePoint = references[time];
                if (referencePoint != null)
                {
                    firstTime = referencePoint.Value.Time;
                    firstIndex = time;
                    break;
                }
            }
            firstTime -= firstIndex * ParserHelper.CombatReplayPollingRate;
            int positionStartOffset = 0;
            for (int time = 0; time < positions.Count; time++)
            {
                var pos = positions[time];
                if (pos.Time >= firstTime)
                {
                    break;
                }
                positionStartOffset++;
            }
            //TODO(Rennorb) @cleanup: Dual indexing requires us to generate sparse lists with the same amount of entries and null values. 
            // These are already parametric points and they are sorted, we don't need to fill up the list to equal lengths. Investigate perf. 
            int offset = 0;
            for (int time = 0; time < references.Count; time++)
            {
                var referencePoint = references[time];
                if (referencePoint == null) 
                {
                    continue;
                }
                if (referencePoint.Value.Time < firstPos.Time)
                {
                    offset++;
                    continue;
                }
                if (referencePoint.Value.Time > lastPos.Time)
                {
                    break;
                }
                distances.Add((positions[time - offset + positionStartOffset].XYZ - referencePoint.Value.XYZ).XY().Length());
            }
            return distances.Count == 0 ? -1 : distances.Sum() / distances.Count;
        }
        else
        {
            return -1;
        }
    }

    internal GameplayStatistics(ParsedEvtcLog log, long start, long end, SingleActor actor)
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
                    SkillAnimationInterruptedCount++;
                    SkillAnimationInterruptedDuration += cl.SavedDuration;
                    break;
                case CastEvent.AnimationStatus.Reduced:
                    SkillAnimationAfterCastInterruptedCount++;
                    SkillAnimationAfterCastInterruptedDuration += cl.SavedDuration;
                    break;
            }
            if (cl.Skill.IsSwap)
            {
                WeaponSwapCount++;
            }
        }
        SkillAnimationAfterCastInterruptedDuration = Math.Round(SkillAnimationAfterCastInterruptedDuration / 1000.0, ParserHelper.TimeDigit);
        SkillAnimationInterruptedDuration = -Math.Round(SkillAnimationInterruptedDuration / 1000.0, ParserHelper.TimeDigit);
        //
        foreach (CastEvent cl in actor.GetIntersectingCastEvents(log, start, end))
        {
            if (cl.IsInterrupted || cl.IsUnknown)
            {
                continue;
            }
            long value = Math.Min(cl.EndTime, end) - Math.Max(cl.Time, start);
            SkillCastUptime += value;
            if (!cl.Skill.IsAutoAttack(log))
            {
                SkillCastUptimeNoAutoAttack += value;
            }
        }
        long timeInCombat = Math.Max(actor.GetTimeSpentInCombat(log, start, end), 1);
        SkillCastUptime /= timeInCombat;
        SkillCastUptimeNoAutoAttack /= timeInCombat;
        SkillCastUptime = Math.Round(100.0 * SkillCastUptime, ParserHelper.TimeDigit);
        SkillCastUptimeNoAutoAttack = Math.Round(100.0 * SkillCastUptimeNoAutoAttack, ParserHelper.TimeDigit);
        //
        double avgBoons = 0;
        foreach (long boonDuration in actor.GetBuffPresence(log, start, end).Where(x => log.Buffs.BuffsByIDs[x.Key].Classification == BuffClassification.Boon).Select(x => x.Value))
        {
            avgBoons += boonDuration;
        }
        AverageBoons = Math.Round(avgBoons / duration, ParserHelper.BuffDigit);
        long activeDuration = actor.GetActiveDuration(log, start, end);
        AverageActiveBoons = activeDuration > 0 ? Math.Round(avgBoons / activeDuration, ParserHelper.BuffDigit) : 0.0;
        //
        double avgCondis = 0;
        foreach (long conditionDuration in actor.GetBuffPresence(log, start, end).Where(x => log.Buffs.BuffsByIDs[x.Key].Classification == BuffClassification.Condition).Select(x => x.Value))
        {
            avgCondis += conditionDuration;
        }
        AverageConditions = Math.Round(avgCondis / duration, ParserHelper.BuffDigit);
        AverageActiveConditions = activeDuration > 0 ? Math.Round(avgCondis / activeDuration, ParserHelper.BuffDigit) : 0.0;
        //
        if (log.CombatData.HasMovementData && log.FriendlyAgents.Contains(actor.AgentItem) && actor.HasCombatReplayPositions(log))
        {
            DistanceToCenterOfSquad = GetDistanceToTarget(actor, log, start, end, log.StatisticsHelper.GetStackCenterPositions(log));
            DistanceToCommander = GetDistanceToTarget(actor, log, start, end, log.StatisticsHelper.GetStackCommanderPositions(log));
        }
    }
}
