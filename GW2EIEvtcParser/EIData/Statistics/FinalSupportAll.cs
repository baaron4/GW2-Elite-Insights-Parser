using System;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

public class FinalSupportAll : FinalSupport
{
    //public long allHeal;
    public readonly int Resurrects;
    public readonly double ResurrectTime;

    public readonly int StunBreak;
    public readonly double RemovedStunDuration;

    private static (int Count, long Duration) GetReses(ParsedEvtcLog log, AbstractSingleActor actor, long start, long end)
    {
        var cls = actor.GetCastEvents(log, start, end);
        (int Count, long Duration) reses = (0, 0);
        foreach (AbstractCastEvent cl in cls)
        {
            if (cl.SkillId == SkillIDs.Resurrect)
            {
                reses.Count++;
                reses.Duration += cl.ActualDuration;
            }
        }
        return reses;
    }

    internal FinalSupportAll(ParsedEvtcLog log, long start, long end, AbstractSingleActor actor) : base(log, start, end, actor, null)
    {
        var resArray = GetReses(log, actor, start, end);
        Resurrects = resArray.Count;
        ResurrectTime = Math.Round((double)resArray.Duration / 1000, ParserHelper.TimeDigit);
        foreach (StunBreakEvent sbe in log.CombatData.GetStunBreakEvents(actor.AgentItem))
        {
            StunBreak++;
            RemovedStunDuration += sbe.RemainingDuration;
        }
        RemovedStunDuration = Math.Round(RemovedStunDuration / 1000, ParserHelper.TimeDigit);
    }

}
