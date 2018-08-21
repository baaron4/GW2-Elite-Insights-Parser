using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class PhaseData
    {
        private long start;
        private long end;
        private string name;
        private List<AgentItem> redirection = new List<AgentItem>();

        public PhaseData(long start, long end)
        {
            this.start = start;
            this.end = end;
        }

        public void SetName(string name)
        {
            this.name = name;
        }

        public string GetName()
        {
            return name;
        }

        public void AddRedirection(AgentItem agent)
        {
            redirection.Add(agent);
        }

        public List<AgentItem> GetRedirection()
        {
            return redirection;
        }

        public long GetDuration(string format = "ms")
        {
            switch (format)
            {
                case "m":
                    return (end - start) / 60000;
                case "s":
                    return (end - start) / 1000;
                case "ms":
                default:
                    return (end - start);
            }

        }

        public bool InInterval(long time, long offset = 0)
        {
            return start <= time - offset && time - offset <= end;
        }

        public void OverrideStart(long offset)
        {
            if (redirection.Count > 0)
            {
                start = redirection.Min(x => x.GetFirstAware())- offset;
            }
        }

        public long GetStart()
        {
            return start;
        }

        public long GetEnd()
        {
            return end;
        }
    }
}
