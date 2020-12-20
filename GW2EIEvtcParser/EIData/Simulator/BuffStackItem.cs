using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class BuffStackItem
    {
        public long Start { get; private set; }
        public long Duration { get; private set; }
        public AgentItem Src { get; private set; }
        public AgentItem SeedSrc { get; }
        public bool IsExtension { get; private set; }

        public long TotalDuration
        {
            get
            {
                long res = Duration;
                foreach ((AgentItem src, long value) in Extensions)
                {
                    res += value;
                }
                return res;
            }
        }

        public List<(AgentItem src, long value)> Extensions { get; } = new List<(AgentItem src, long value)>();

        public BuffStackItem(long start, long boonDuration, AgentItem src, AgentItem seedSrc, bool isExtension)
        {
            Start = start;
            SeedSrc = seedSrc;
            Duration = boonDuration;
            Src = src;
            IsExtension = isExtension;
        }

        public BuffStackItem(long start, long boonDuration, AgentItem src)
        {
            Start = start;
            SeedSrc = src;
            Duration = boonDuration;
            Src = src;
            IsExtension = false;
        }

        public virtual void Shift(long startShift, long durationShift)
        {
            Start += startShift;
            Duration -= durationShift;
            if (Duration == 0 && Extensions.Any() && durationShift > 0)
            {
                (AgentItem src, long value) = Extensions.First();
                Extensions.RemoveAt(0);
                Src = src;
                Duration = value;
                IsExtension = true;
            }
        }

        public void Extend(long value, AgentItem src)
        {
            Extensions.Add((src, value));
        }
    }

}
