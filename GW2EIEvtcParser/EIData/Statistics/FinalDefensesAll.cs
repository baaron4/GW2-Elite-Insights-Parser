using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    public class FinalDefensesAll : FinalDefenses
    {
        public int DownCount { get; }
        public long DownDuration { get; }
        public int DeadCount { get; }
        public long DeadDuration { get; }
        public int DcCount { get; }
        public long DcDuration { get; }

        public FinalDefensesAll(ParsedEvtcLog log, long start, long end, AbstractSingleActor actor) : base(log, start, end, actor, null)
        {
            (IReadOnlyList<Segment> dead, IReadOnlyList<Segment> down, IReadOnlyList<Segment> dc) = actor.GetStatus(log);

            // We can't use mechanics due to down event vs down buff desync
            if (actor.AgentItem.Type == AgentItem.AgentType.Player)
            {
                if (actor.BaseSpec == ParserHelper.Spec.Elementalist)
                {
                    var vaporFormRemoves = log.CombatData.GetBuffRemoveAllData(SkillIDs.VaporForm).Where(brae => brae.To == actor.AgentItem && brae.Time >= start && brae.Time <= end).ToList();
                    var downEvents = log.CombatData.GetBuffDataByIDByDst(SkillIDs.Downed, actor.AgentItem).Where(be => be.Time >= start && be.Time <= end && be is BuffApplyEvent).ToList();
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
}
