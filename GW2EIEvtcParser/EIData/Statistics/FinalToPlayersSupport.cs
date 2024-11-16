using static GW2EIEvtcParser.EIData.Buff;

namespace GW2EIEvtcParser.EIData;

// to match non generic support stats
public class FinalToPlayersSupport
{
    //public long allHeal;
    public readonly int Resurrects;
    public readonly double ResurrectTime;
    public readonly int CondiCleanse;
    public readonly double CondiCleanseTime;
    public readonly int CondiCleanseSelf;
    public readonly double CondiCleanseTimeSelf;
    public readonly int BoonStrips;
    public readonly double BoonStripsTime;
    public readonly int StunBreak;
    public readonly double RemovedStunDuration;

    internal FinalToPlayersSupport(ParsedEvtcLog log, SingleActor actor, long start, long end)
    {
        var totals = actor.GetSupportStats(log, start, end);
        Resurrects = totals.Resurrects;
        ResurrectTime = totals.ResurrectTime;
        StunBreak = totals.StunBreak;
        RemovedStunDuration = totals.RemovedStunDuration;
        FinalSupport self = actor.GetSupportStats(actor, log, start, end);
        foreach (Buff boon in log.Buffs.BuffsByClassification[BuffClassification.Boon])
        {
            // add everything from total
            if (totals.FoeRemovals.TryGetValue(boon.ID, out (int count, long time) itemFoe))
            {
                BoonStrips += itemFoe.count;
                BoonStripsTime += itemFoe.time;
            }
            if (totals.UnknownRemovals.TryGetValue(boon.ID, out (int count, long time) itemUnknown))
            {
                BoonStrips += itemUnknown.count;
                BoonStripsTime += itemUnknown.time;
            }
        }
        foreach (Buff condition in log.Buffs.BuffsByClassification[BuffClassification.Condition])
        {
            // add everything from self
            if (self.FriendlyRemovals.TryGetValue(condition.ID, out (int count, long time) itemFriend))
            {
                CondiCleanseSelf += itemFriend.count;
                CondiCleanseTimeSelf += itemFriend.time;
            }
            foreach (Player p in log.PlayerList)
            {
                if (p == actor)
                {
                    continue;
                }
                FinalSupport other = actor.GetSupportStats(p, log, start, end);
                // Add everything from other
                if (other.FriendlyRemovals.TryGetValue(condition.ID, out itemFriend))
                {
                    CondiCleanse += itemFriend.count;
                    CondiCleanseTime += itemFriend.time;
                }
            }
        }
        CondiCleanseTime = Math.Round(CondiCleanseTime / 1000.0, ParserHelper.TimeDigit);
        CondiCleanseTimeSelf = Math.Round(CondiCleanseTimeSelf / 1000.0, ParserHelper.TimeDigit);
        BoonStripsTime = Math.Round(BoonStripsTime / 1000.0, ParserHelper.TimeDigit);
    }

}
