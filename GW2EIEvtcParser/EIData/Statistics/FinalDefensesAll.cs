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

            var dead = new List<(long start, long end)>();
            var down = new List<(long start, long end)>();
            var dc = new List<(long start, long end)>();
            (dead, down, dc) = actor.GetStatus(log);
            DownCount = log.MechanicData.GetMechanicLogs(log, SkillItem.DownId).Count(x => x.Actor == actor && x.Time >= start && x.Time <= end);
            DeadCount = log.MechanicData.GetMechanicLogs(log, SkillItem.DeathId).Count(x => x.Actor == actor && x.Time >= start && x.Time <= end);
            DcCount = log.MechanicData.GetMechanicLogs(log, SkillItem.DCId).Count(x => x.Actor == actor && x.Time >= start && x.Time <= end);

            DownDuration = down.Where(x => x.end >= start && x.start <= end).Sum(x => Math.Min(end, x.end) - Math.Max(x.start, start));
            DeadDuration = dead.Where(x => x.end >= start && x.start <= end).Sum(x => Math.Min(end, x.end) - Math.Max(x.start, start));
            DcDuration = dc.Where(x => x.end >= start && x.start <= end).Sum(x => Math.Min(end, x.end) - Math.Max(x.start, start));
        }
    }
}
