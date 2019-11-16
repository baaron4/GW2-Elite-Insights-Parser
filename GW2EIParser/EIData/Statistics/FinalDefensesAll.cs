using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using static GW2EIParser.EIData.Buff;

namespace GW2EIParser.EIData
{
    public class FinalDefensesAll : FinalDefenses
    {
        //public long allHealReceived;
        public int DownCount { get; set; }
        public int DownDuration { get; set; }
        public int DeadCount { get; set; }
        public int DeadDuration { get; set; }
        public int DcCount { get; set; }
        public int DcDuration { get; set; }

        public FinalDefensesAll(ParsedLog log, PhaseData phase, AbstractSingleActor actor) : base(log, phase, actor, null)
        {
            var dead = new List<(long start, long end)>();
            var down = new List<(long start, long end)>();
            var dc = new List<(long start, long end)>();
            actor.AgentItem.GetAgentStatus(dead, down, dc, log);
            long start = phase.Start;
            long end = phase.End;

            DownCount = log.MechanicData.GetMechanicLogs(log, SkillItem.DownId).Count(x => x.Actor == actor && x.Time >= start && x.Time <= end);
            DeadCount = log.MechanicData.GetMechanicLogs(log, SkillItem.DeathId).Count(x => x.Actor == actor && x.Time >= start && x.Time <= end);
            DcCount = log.MechanicData.GetMechanicLogs(log, SkillItem.DCId).Count(x => x.Actor == actor && x.Time >= start && x.Time <= end);

            DownDuration = (int)down.Where(x => x.end >= start && x.start <= end).Sum(x => Math.Min(end, x.end) - Math.Max(x.start, start));
            DeadDuration = (int)dead.Where(x => x.end >= start && x.start <= end).Sum(x => Math.Min(end, x.end) - Math.Max(x.start, start));
            DcDuration = (int)dc.Where(x => x.end >= start && x.start <= end).Sum(x => Math.Min(end, x.end) - Math.Max(x.start, start));
        }
    }
}
