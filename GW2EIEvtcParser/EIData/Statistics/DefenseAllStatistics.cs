using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

public class DefenseAllStatistics : DefensePerTargetStatistics
{
    public readonly int DownCount;
    public readonly long DownDuration;
    public readonly int DeadCount;
    public readonly long DeadDuration;
    public readonly int DcCount;
    public readonly long DcDuration;

    public DefenseAllStatistics(ParsedEvtcLog log, long start, long end, SingleActor actor) : base(log, start, end, actor, null)
    {
        (IReadOnlyList<Segment> dead, IReadOnlyList<Segment> down, IReadOnlyList<Segment> dc, _) = actor.GetStatus(log);

        // We can't use mechanics due to down event vs down buff desync
        if (actor.AgentItem.Type == AgentItem.AgentType.Player)
        {
            if (actor.BaseSpec == ParserHelper.Spec.Elementalist)
            {
                var vaporFormRemoves = log.CombatData.GetBuffRemoveAllDataByDst(SkillIDs.VaporForm, actor.AgentItem).Where(brae => brae.Time >= start && brae.Time <= end);
                var downEvents = log.CombatData.GetBuffDataByIDByDst(SkillIDs.Downed, actor.AgentItem).Where(be => be.Time >= start && be.Time <= end && be is BuffApplyEvent);
                DownCount = downEvents.Count(downEvent => !vaporFormRemoves.Any(vaporRemove => Math.Abs(vaporRemove.Time - downEvent.Time) < ParserHelper.ServerDelayConstant));
            }
            else
            {
                DownCount = log.CombatData.GetBuffDataByIDByDst(SkillIDs.Downed, actor.AgentItem).Where(be => be.Time >= start && be.Time <= end && be is BuffApplyEvent).Count();
            }
        }
        else
        {
            DownCount = log.CombatData.GetDownEvents(actor.AgentItem).Count(x => x.Time >= start && x.Time <= end);
        }
        DeadCount = log.CombatData.GetDeadEvents(actor.AgentItem).Count(x => x.Time >= start && x.Time <= end);
        DcCount = log.CombatData.GetDespawnEvents(actor.AgentItem).Count(x => x.Time >= start && x.Time <= end);

        DownDuration = (long)down.Sum(x => x.IntersectingArea(start, end));
        DeadDuration = (long)dead.Sum(x => x.IntersectingArea(start, end));
        DcDuration = (long)dc.Sum(x => x.IntersectingArea(start, end));
    }
}
