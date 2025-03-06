using static GW2EIEvtcParser.EIData.Buff;

namespace GW2EIEvtcParser.EIData;

// to match non generic support stats
public class SupportStatistics
{
    //public long allHeal;
    public readonly int ResurrectCount;
    public readonly double ResurrectTime;
    public readonly int ConditionCleanseCount;
    public readonly double ConditionCleanseTime;
    public readonly int ConditionCleanseSelfCount;
    public readonly double ConditionCleanseTimeSelf;
    public readonly int BoonStripCount;
    public readonly double BoonStripTime;
    public readonly int BoonStripDownContribution;
    public readonly double BoonStripDownContributionTime;
    public readonly int StunBreakCount;
    public readonly double RemovedStunDuration;

    internal SupportStatistics(ParsedEvtcLog log, SingleActor actor, long start, long end)
    {
        var totals = actor.GetSupportStats(log, start, end);
        ResurrectCount = totals.ResurrectCount;
        ResurrectTime = totals.ResurrectTime;
        StunBreakCount = totals.StunBreakCount;
        RemovedStunDuration = totals.RemovedStunDuration;
        SupportPerAllyStatistics self = actor.GetSupportStats(actor, log, start, end);
        foreach (Buff boon in log.Buffs.BuffsByClassification[BuffClassification.Boon])
        {
            // add everything from total
            if (totals.FoeRemovals.TryGetValue(boon.ID, out (int count, long time) itemFoe))
            {
                BoonStripCount += itemFoe.count;
                BoonStripTime += itemFoe.time;
            }
            if (totals.FoeRemovalsDownContribution.TryGetValue(boon.ID, out (int count, long time) itemFoeDownContribution))
            {
                BoonStripDownContribution += itemFoeDownContribution.count;
                BoonStripDownContributionTime += itemFoeDownContribution.time;
            }
            if (totals.UnknownRemovals.TryGetValue(boon.ID, out (int count, long time) itemUnknown))
            {
                BoonStripCount += itemUnknown.count;
                BoonStripTime += itemUnknown.time;
            }
        }
        foreach (Buff condition in log.Buffs.BuffsByClassification[BuffClassification.Condition])
        {
            // add everything from self
            if (self.FriendlyRemovals.TryGetValue(condition.ID, out (int count, long time) itemFriend))
            {
                ConditionCleanseSelfCount += itemFriend.count;
                ConditionCleanseTimeSelf += itemFriend.time;
            }
            foreach (Player p in log.PlayerList)
            {
                if (p == actor)
                {
                    continue;
                }
                SupportPerAllyStatistics other = actor.GetSupportStats(p, log, start, end);
                // Add everything from other
                if (other.FriendlyRemovals.TryGetValue(condition.ID, out itemFriend))
                {
                    ConditionCleanseCount += itemFriend.count;
                    ConditionCleanseTime += itemFriend.time;
                }
            }
        }
        ConditionCleanseTime = Math.Round(ConditionCleanseTime / 1000.0, ParserHelper.TimeDigit);
        ConditionCleanseTimeSelf = Math.Round(ConditionCleanseTimeSelf / 1000.0, ParserHelper.TimeDigit);
        BoonStripTime = Math.Round(BoonStripTime / 1000.0, ParserHelper.TimeDigit);
        BoonStripDownContributionTime = Math.Round(BoonStripDownContributionTime / 1000.0, ParserHelper.TimeDigit);
    }

}
