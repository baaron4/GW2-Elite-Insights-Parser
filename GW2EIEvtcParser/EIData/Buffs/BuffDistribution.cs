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
        private readonly Dictionary<long, Dictionary<AgentItem, BuffDistributionItem>> _distribution = new Dictionary<long, Dictionary<AgentItem, BuffDistributionItem>>();
        public ICollection<long> BuffIDs => _distribution.Keys;

        internal Dictionary<AgentItem, BuffDistributionItem> GetDistrib(long buffID)
        {
            if (!_distribution.TryGetValue(buffID, out Dictionary<AgentItem, BuffDistributionItem> distrib))
            {
                distrib = new Dictionary<AgentItem, BuffDistributionItem>();
                _distribution.Add(buffID, distrib);
            }
            return distrib;
        }

        public bool HasBuffID(long buffID)
        {
            return _distribution.ContainsKey(buffID);
        }

        public bool HasSrc(long buffID, AgentItem src)
        {
            return _distribution.ContainsKey(buffID) && _distribution[buffID].ContainsKey(src);
        }

        public List<AbstractSingleActor> GetSrcs(long buffID, ParsedEvtcLog log)
        {
            if (!_distribution.ContainsKey(buffID))
            {
                return new List<AbstractSingleActor>();
            }
            var actors = new List<AbstractSingleActor>();
            foreach (AgentItem agent in _distribution[buffID].Keys)
            {
                actors.Add(log.FindActor(agent));
            }
            return actors;
        }

        public long GetUptime(long buffID)
        {
            if (!_distribution.ContainsKey(buffID))
            {
                return 0;
            }
            return _distribution[buffID].Sum(x => x.Value.Value);
        }

        public long GetGeneration(long buffID, AgentItem src)
        {
            if (!_distribution.ContainsKey(buffID) || !_distribution[buffID].ContainsKey(src))
            {
                return 0;
            }
            return _distribution[buffID][src].Value;
        }

        public long GetOverstack(long buffID, AgentItem src)
        {
            if (!_distribution.ContainsKey(buffID) || !_distribution[buffID].ContainsKey(src))
            {
                return 0;
            }
            return _distribution[buffID][src].Overstack;
        }

        public long GetWaste(long buffID, AgentItem src)
        {
            if (!_distribution.ContainsKey(buffID) || !_distribution[buffID].ContainsKey(src))
            {
                return 0;
            }
            return _distribution[buffID][src].Waste;
        }

        public long GetUnknownExtension(long buffID, AgentItem src)
        {
            if (!_distribution.ContainsKey(buffID) || !_distribution[buffID].ContainsKey(src))
            {
                return 0;
            }
            return _distribution[buffID][src].UnknownExtension;
        }

        public long GetExtension(long buffID, AgentItem src)
        {
            if (!_distribution.ContainsKey(buffID) || !_distribution[buffID].ContainsKey(src))
            {
                return 0;
            }
            return _distribution[buffID][src].Extension;
        }

        public long GetExtended(long buffID, AgentItem src)
        {
            if (!_distribution.ContainsKey(buffID) || !_distribution[buffID].ContainsKey(src))
            {
                return 0;
            }
            return _distribution[buffID][src].Extended;
        }
    }
}
