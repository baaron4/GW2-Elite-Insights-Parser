using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class PhaseData
    {
        public long Start { get; private set; }
        public long End { get; private set; }
        public string Name { get; set; }
        public bool DrawStart { get; set; }
        public bool DrawEnd { get; set; }
        public bool DrawArea { get; set; }
        public List<Boss> Targets { get; } = new List<Boss>();

        public PhaseData(long start, long end)
        {
            Start = start;
            End = end;
        }
        
        public long GetDuration(string format = "ms")
        {
            switch (format)
            {
                case "m":
                    return (End - Start) / 60000;
                case "s":
                    return (End - Start) / 1000;
                default:
                    return (End - Start);
            }

        }

        public bool InInterval(long time, long offset = 0)
        {
            return Start <= time - offset && time - offset <= End;
        }

        public void OverrideTimes(long offset)
        {
            if (Targets.Count > 0)
            {
                Start = Math.Max(Start, Targets.Min(x => x.FirstAware)- offset);
                End = Math.Min(End, Targets.Min(x => x.LastAware) - offset);
            }
        }
    }
}
