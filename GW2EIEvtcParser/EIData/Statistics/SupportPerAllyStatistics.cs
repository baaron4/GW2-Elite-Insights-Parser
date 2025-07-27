using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

public class SupportPerAllyStatistics
{
    private readonly Dictionary<long, (int count, long time)> _friendlyRemovals = [];
    public IReadOnlyDictionary<long, (int count, long time)> FriendlyRemovals => _friendlyRemovals;
    private readonly Dictionary<long, (int count, long time)> _foeRemovals = [];
    public IReadOnlyDictionary<long, (int count, long time)> FoeRemovals => _foeRemovals;
    private readonly Dictionary<long, (int count, long time)> _foeRemovalsDownContribution = [];
    public IReadOnlyDictionary<long, (int count, long time)> FoeRemovalsDownContribution => _foeRemovalsDownContribution;
    private readonly Dictionary<long, (int count, long time)> _unknownRemovals = [];
    public IReadOnlyDictionary<long, (int count, long time)> UnknownRemovals => _unknownRemovals;

    internal SupportPerAllyStatistics(ParsedEvtcLog log, long start, long end, SingleActor actor, SingleActor? to)
    {
        foreach (long buffID in log.Buffs.BuffsByIDs.Keys)
        {
            int foeCount = 0;
            long foeTime = 0;
            int foeDownContributionCount = 0;
            long foeDownContributionTime = 0;
            int friendlyCount = 0;
            long friendlyTime = 0;
            int unknownCount = 0;
            long unknownTime = 0;
            foreach (BuffRemoveAllEvent brae in log.CombatData.GetBuffRemoveAllDataBySrc(buffID, actor.AgentItem))
            {
                if (brae.Time >= start && brae.Time <= end)
                {
                    if (to != null && brae.To != to.AgentItem)
                    {
                        continue;
                    }
                    if (brae.ToFriendly)
                    {
                        friendlyCount++;
                        friendlyTime = Math.Max(friendlyTime + brae.RemovedDuration, log.FightData.FightDuration);
                    }
                    else if (brae.ToFoe)
                    {
                        foeCount++;
                        foeTime = Math.Max(foeTime + brae.RemovedDuration, log.FightData.FightDuration);
                        if (brae.To.IsDownedBeforeNext90(log, brae.Time))
                        {
                            foeDownContributionCount++;
                            foeDownContributionTime = Math.Max(foeTime + brae.RemovedDuration, log.FightData.FightDuration);
                        }
                    }
                    else
                    {
                        unknownCount++;
                        unknownTime = Math.Max(unknownTime + brae.RemovedDuration, log.FightData.FightDuration);
                    }
                }
            }
            if (foeCount > 0)
            {
                _foeRemovals[buffID] = (foeCount, foeTime);
            }
            if (foeDownContributionCount > 0)
            {
                _foeRemovalsDownContribution[buffID] = (foeDownContributionCount, foeDownContributionTime);
            }
            if (friendlyCount > 0)
            {
                _friendlyRemovals[buffID] = (friendlyCount, friendlyTime);
            }
            if (unknownCount > 0)
            {
                _unknownRemovals[buffID] = (unknownCount, unknownTime);
            }
        }
    }

}
