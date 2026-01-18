using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

public class SupportAllStatistics : SupportPerAllyStatistics
{
    //public long allHeal;
    public readonly int ResurrectCount;
    public readonly double ResurrectTime;

    public readonly int StunBreakCount;
    public readonly double RemovedStunDuration;

    private static (int Count, long Duration) GetReses(ParsedEvtcLog log, SingleActor actor, long start, long end)
    {
        var cls = actor.GetCastEvents(log, start, end);
        (int Count, long Duration) reses = (0, 0);
        foreach (CastEvent cl in cls)
        {
            if (cl.SkillID == SkillIDs.Resurrect)
            {
                reses.Count++;
                reses.Duration += cl.ActualDuration;
            }
        }
        return reses;
    }

    internal SupportAllStatistics(ParsedEvtcLog log, long start, long end, SingleActor actor) : base(log, start, end, actor, null)
    {
        var (Count, Duration) = GetReses(log, actor, start, end);
        ResurrectCount = Count;
        ResurrectTime = Math.Round((double)Duration / 1000, ParserHelper.TimeDigit);
        foreach (StunBreakEvent sbe in log.CombatData.GetStunBreakEvents(actor.AgentItem))
        {
            StunBreakCount++;
            RemovedStunDuration += sbe.RemainingDuration;
        }
        RemovedStunDuration = Math.Round(RemovedStunDuration / 1000, ParserHelper.TimeDigit);
    }

}
