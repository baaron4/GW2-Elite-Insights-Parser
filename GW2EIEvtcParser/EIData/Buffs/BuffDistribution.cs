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
        private Dictionary<long, Dictionary<AgentItem, BuffDistributionItem>> _distributions = new Dictionary<long, Dictionary<AgentItem, BuffDistributionItem>>();

        internal Dictionary<AgentItem, BuffDistributionItem> GetDistribution(long buffID)
        {
            if (!_distributions.TryGetValue(buffID, out Dictionary<AgentItem, BuffDistributionItem> distrib))
            {
                distrib = new Dictionary<AgentItem, BuffDistributionItem>();
                _distributions.Add(buffID, distrib);
            }
            return distrib;
        }

        public IReadOnlyCollection<long> PresentBuffIDs()
        {
            return _distributions.Keys;
        }

        public bool ContainsBuffID(long buffID)
        {
            return _distributions.ContainsKey(buffID);
        }

        public bool HasSrc(long buffID, AgentItem src)
        {
            return _distributions.ContainsKey(buffID) && _distributions[buffID].ContainsKey(src);
        }

        public IReadOnlyList<AbstractSingleActor> GetSrcs(long buffID, ParsedEvtcLog log)
        {
            if (!_distributions.ContainsKey(buffID))
            {
                return new List<AbstractSingleActor>();
            }
            var actors = new List<AbstractSingleActor>();
            foreach (AgentItem agent in _distributions[buffID].Keys)
            {
                actors.Add(log.FindActor(agent, true));
            }
            return actors;
        }

        public long GetUptime(long buffID)
        {
            if (!_distributions.ContainsKey(buffID))
            {
                return 0;
            }
            return _distributions[buffID].Sum(x => x.Value.Value);
        }

        public long GetGeneration(long buffID, AgentItem src)
        {
            if (!_distributions.ContainsKey(buffID) || !_distributions[buffID].ContainsKey(src))
            {
                return 0;
            }
            return _distributions[buffID][src].Value;
        }

        public long GetOverstack(long buffID, AgentItem src)
        {
            if (!_distributions.ContainsKey(buffID) || !_distributions[buffID].ContainsKey(src))
            {
                return 0;
            }
            return _distributions[buffID][src].Overstack;
        }

        public long GetWaste(long buffID, AgentItem src)
        {
            if (!_distributions.ContainsKey(buffID) || !_distributions[buffID].ContainsKey(src))
            {
                return 0;
            }
            return _distributions[buffID][src].Waste;
        }

        public long GetUnknownExtension(long buffID, AgentItem src)
        {
            if (!_distributions.ContainsKey(buffID) || !_distributions[buffID].ContainsKey(src))
            {
                return 0;
            }
            return _distributions[buffID][src].UnknownExtension;
        }

        public long GetExtension(long buffID, AgentItem src)
        {
            if (!_distributions.ContainsKey(buffID) || !_distributions[buffID].ContainsKey(src))
            {
                return 0;
            }
            return _distributions[buffID][src].Extension;
        }

        public long GetExtended(long buffID, AgentItem src)
        {
            if (!_distributions.ContainsKey(buffID) || !_distributions[buffID].ContainsKey(src))
            {
                return 0;
            }
            return _distributions[buffID][src].Extended;
        }
    }
}
