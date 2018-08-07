using System.Collections.Generic;

namespace LuckParser.Models.ParseModels
{
    public class PhaseData
    {
        private long start;
        private long end;
        private string name;
        private List<long> redirection = new List<long>();

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

        public void addRedirection(long agent)
        {
            redirection.Add(agent);
        }

        public List<long> getRedirection()
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
