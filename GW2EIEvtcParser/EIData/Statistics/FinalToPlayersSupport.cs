using System;
using static GW2EIEvtcParser.EIData.Buff;

namespace GW2EIEvtcParser.EIData
{
    // to match non generic support stats
    public class FinalToPlayersSupport
    {
        //public long allHeal;
        public int Resurrects { get; }
        public double ResurrectTime { get; }
        public int CondiCleanse { get; }
        public double CondiCleanseTime { get; }
        public int CondiCleanseSelf { get; }
        public double CondiCleanseTimeSelf { get; }
        public int BoonStrips { get; }
        public double BoonStripsTime { get; }

        internal FinalToPlayersSupport(ParsedEvtcLog log, AbstractSingleActor actor, long start, long end)
        {
            FinalSupportAll totals = actor.GetSupportStats(log, start, end);
            Resurrects = totals.Resurrects;
            ResurrectTime = Math.Round(totals.ResurrectTime / 1000.0, ParserHelper.TimeDigit);
            FinalSupport self = actor.GetSupportStats(actor, log, start, end);
            foreach (Buff boon in log.Buffs.BuffsByClassification[BuffClassification.Boon])
            {
                // add everything from total
                if (totals.Removals.TryGetValue(boon.ID, out (int count, long time) item))
                {
                    BoonStrips += item.count;
                    BoonStripsTime += item.time;
                }
                // remove everything from self
                if (self.Removals.TryGetValue(boon.ID, out item))
                {
                    BoonStrips -= item.count;
                    BoonStripsTime -= item.time;
                }
                // Remove everything from other, security check, no in game mechanics do such thing today
                foreach (Player p in log.PlayerList)
                {
                    if (p == actor)
                    {
                        continue;
                    }
                    FinalSupport other = actor.GetSupportStats(p, log, start, end);
                    if (other.Removals.TryGetValue(boon.ID, out item))
                    {
                        BoonStrips -= item.count;
                        BoonStripsTime -= item.time;
                    }
                }
            }
            foreach (Buff condition in log.Buffs.BuffsByClassification[BuffClassification.Condition])
            {
                // add everything from self
                if (self.Removals.TryGetValue(condition.ID, out (int count, long time) item))
                {
                    CondiCleanseSelf += item.count;
                    CondiCleanseTimeSelf += item.time;
                }
                foreach (Player p in log.PlayerList)
                {
                    if (p == actor)
                    {
                        continue;
                    }
                    FinalSupport other = actor.GetSupportStats(p, log, start, end);
                    // Add everything from other
                    if (other.Removals.TryGetValue(condition.ID, out item))
                    {
                        CondiCleanse += item.count;
                        CondiCleanseTime += item.time;
                    }
                }
            }
            CondiCleanseTime = Math.Round(CondiCleanseTime / 1000.0, ParserHelper.TimeDigit);
            CondiCleanseTimeSelf = Math.Round(CondiCleanseTimeSelf / 1000.0, ParserHelper.TimeDigit);
            BoonStripsTime = Math.Round(BoonStripsTime / 1000.0, ParserHelper.TimeDigit);
        }

    }
}
