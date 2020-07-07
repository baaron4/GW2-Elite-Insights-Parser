using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.EIData
{
    public class FinalDefensesAll : FinalDefenses
    {
        //public long allHealReceived;
        public int DownCount { get; set; }
        public long DownDuration { get; set; }
        public int DeadCount { get; set; }
        public long DeadDuration { get; set; }
        public int DcCount { get; set; }
        public long DcDuration { get; set; }

        public FinalDefensesAll(ParsedLog log, PhaseData phase, AbstractSingleActor actor) : base(log, phase, actor, null)
        {
            var dead = new List<(long start, long end)>();
            var down = new List<(long start, long end)>();
            var dc = new List<(long start, long end)>();
            (dead, down, dc) = actor.GetStatus(log);
            long start = phase.Start;
            long end = phase.End;

            DownCount = log.MechanicData.GetMechanicLogs(log, SkillItem.DownId).Count(x => x.Actor == actor && x.Time >= start && x.Time <= end);
            DeadCount = log.MechanicData.GetMechanicLogs(log, SkillItem.DeathId).Count(x => x.Actor == actor && x.Time >= start && x.Time <= end);
            DcCount = log.MechanicData.GetMechanicLogs(log, SkillItem.DCId).Count(x => x.Actor == actor && x.Time >= start && x.Time <= end);

            DownDuration = down.Where(x => x.end >= start && x.start <= end).Sum(x => Math.Min(end, x.end) - Math.Max(x.start, start));
            DeadDuration = dead.Where(x => x.end >= start && x.start <= end).Sum(x => Math.Min(end, x.end) - Math.Max(x.start, start));
            DcDuration = dc.Where(x => x.end >= start && x.start <= end).Sum(x => Math.Min(end, x.end) - Math.Max(x.start, start));
        }
    }
}
