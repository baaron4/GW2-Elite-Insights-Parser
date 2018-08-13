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

        public void setName(string name)
        {
            this.name = name;
        }

        public string getName()
        {
            return name;
        }

        public void addRedirection(AgentItem agent)
        {
            redirection.Add(agent);
        }

        public List<AgentItem> getRedirection()
        {
            return redirection;
        }

        public long getDuration(string format = "ms")
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

        public bool inInterval(long time, long offset = 0)
        {
            return start <= time - offset && time - offset <= end;
        }

        public void overrideStart(long offset)
        {
            if (redirection.Count > 0)
            {
                start = redirection.Min(x => x.getFirstAware())- offset;
            }
        }

        public long getStart()
        {
            return start;
        }

        public long getEnd()
        {
            return end;
        }
    }
}
