using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EncounterLogic;
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
            (IReadOnlyList<Segment>  dead, IReadOnlyList<Segment>  down, IReadOnlyList<Segment>  dc) = actor.GetStatus(log);

            //DownCount = log.MechanicData.GetMechanicLogs(log, FightLogic.DownMechanic).Count(x => x.Actor == actor && x.Time >= start && x.Time <= end); 
            // We can't use mechanics due to down event vs down buff desync
            if (actor.BaseSpec == ParserHelper.Spec.Elementalist)
            {
                var vaporFormRemoves = log.CombatData.GetBuffRemoveAllData(SkillIDs.VaporForm).Where(brae => brae.To == actor.AgentItem && brae.Time >= start && brae.Time <= end).ToList();
                var downEvents = log.CombatData.GetBuffData(SkillIDs.Downed).Where(be => be.To == actor.AgentItem && be.Time >= start && be.Time <= end && be is BuffApplyEvent).ToList();
                DownCount = downEvents.Count(downEvent => vaporFormRemoves.Any(vaporRemove => Math.Abs(vaporRemove.Time - downEvent.Time) < ParserHelper.ServerDelayConstant));
            } 
            else
            {
                DownCount = log.CombatData.GetBuffData(SkillIDs.Downed).Where(be => be.To == actor.AgentItem && be.Time >= start && be.Time <= end && be is BuffApplyEvent).Count();
            }
            DeadCount = log.MechanicData.GetMechanicLogs(log, FightLogic.DeathMechanic).Count(x => x.Actor == actor && x.Time >= start && x.Time <= end);
            DcCount = log.MechanicData.GetMechanicLogs(log, FightLogic.DespawnMechanic).Count(x => x.Actor == actor && x.Time >= start && x.Time <= end);

            DownDuration = (long)down.Sum(x => x.IntersectingArea(start, end));
            DeadDuration = (long)dead.Sum(x => x.IntersectingArea(start, end));
            DcDuration = (long)dc.Sum(x => x.IntersectingArea(start, end));
        }
    }
}
