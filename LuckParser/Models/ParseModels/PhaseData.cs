using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class PhaseData
    {
        public long Start { get; private set; }
        public readonly long End;
        public string Name { get; set; }
        public readonly List<AgentItem> Redirection = new List<AgentItem>();

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

        public void OverrideStart(long offset)
        {
            if (Redirection.Count > 0)
            {
                Start = Redirection.Min(x => x.FirstAware)- offset;
            }
        }
    }
}
