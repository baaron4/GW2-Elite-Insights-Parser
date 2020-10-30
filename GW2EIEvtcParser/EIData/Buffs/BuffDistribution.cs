using GW2EIEvtcParser.ParsedData;
using System.Collections.Generic;
using System.Linq;

namespace GW2EIEvtcParser.EIData
{
    internal class BuffDistributionItem
    {
        public long Value { get; set; }
        public long Overstack { get; set; }
        public long Waste { get; set; }
        public long UnknownExtension { get; set; }
        public long Extension { get; set; }
        public long Extended { get; set; }

        internal BuffDistributionItem(long value, long overstack, long waste, long unknownExtension, long extension, long extended)
        {
            Value = value;
            Overstack = overstack;
            Waste = waste;
            UnknownExtension = unknownExtension;
            Extension = extension;
            Extended = extended;
        }
    }

    public class BuffDistribution
    {
        internal Dictionary<long, Dictionary<AgentItem, BuffDistributionItem>> Distributions = new Dictionary<long, Dictionary<AgentItem, BuffDistributionItem>>();

        public bool ContainsKey(long boonid)
        {
            return Distributions.ContainsKey(boonid);
        }

        public bool HasSrc(long boonid, AgentItem src)
        {
            return Distributions.ContainsKey(boonid) && Distributions[boonid].ContainsKey(src);
        }

        public IReadOnlyList<AbstractSingleActor> GetSrcs(long boonid, ParsedEvtcLog log)
        {
            if (!Distributions.ContainsKey(boonid))
            {
                return new List<AbstractSingleActor>();
            }
            var actors = new List<AbstractSingleActor>();
            foreach (AgentItem agent in Distributions[boonid].Keys)
            {
                actors.Add(log.FindActor(agent, true));
            }
            return actors;
        }

        public long GetUptime(long boonid)
        {
            if (!Distributions.ContainsKey(boonid))
            {
                return 0;
            }
            return Distributions[boonid].Sum(x => x.Value.Value);
        }

        public long GetGeneration(long boonid, AgentItem src)
        {
            if (!Distributions.ContainsKey(boonid) || !Distributions[boonid].ContainsKey(src))
            {
                return 0;
            }
            return Distributions[boonid][src].Value;
        }

        public long GetOverstack(long boonid, AgentItem src)
        {
            if (!Distributions.ContainsKey(boonid) || !Distributions[boonid].ContainsKey(src))
            {
                return 0;
            }
            return Distributions[boonid][src].Overstack;
        }

        public long GetWaste(long boonid, AgentItem src)
        {
            if (!Distributions.ContainsKey(boonid) || !Distributions[boonid].ContainsKey(src))
            {
                return 0;
            }
            return Distributions[boonid][src].Waste;
        }

        public long GetUnknownExtension(long boonid, AgentItem src)
        {
            if (!Distributions.ContainsKey(boonid) || !Distributions[boonid].ContainsKey(src))
            {
                return 0;
            }
            return Distributions[boonid][src].UnknownExtension;
        }

        public long GetExtension(long boonid, AgentItem src)
        {
            if (!Distributions.ContainsKey(boonid) || !Distributions[boonid].ContainsKey(src))
            {
                return 0;
            }
            return Distributions[boonid][src].Extension;
        }

        public long GetExtended(long boonid, AgentItem src)
        {
            if (!Distributions.ContainsKey(boonid) || !Distributions[boonid].ContainsKey(src))
            {
                return 0;
            }
            return Distributions[boonid][src].Extended;
        }
    }
}
