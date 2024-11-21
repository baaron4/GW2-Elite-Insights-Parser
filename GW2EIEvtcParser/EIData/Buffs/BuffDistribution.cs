using System.Diagnostics.CodeAnalysis;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

internal class BuffDistributionItem
{
    public long Value { get; private set; }
    public long Overstack { get; private set; }
    public long Waste { get; private set; }
    public long UnknownExtension { get; private set; }
    public long Extension { get; private set; }
    public long Extended { get; private set; }

    internal BuffDistributionItem(long value, long overstack, long waste, long unknownExtension, long extension, long extended)
    {
        Value = value;
        Overstack = overstack;
        Waste = waste;
        UnknownExtension = unknownExtension;
        Extension = extension;
        Extended = extended;
    }

    internal void IncrementValue(long value)
    {
        Value += value;
    }

    internal void IncrementOverstack(long value)
    {
        Overstack += value;
    }

    internal void IncrementWaste(long value)
    {
        Waste += value;
    }

    internal void IncrementUnknownExtension(long value)
    {
        UnknownExtension += value;
    }

    internal void IncrementExtension(long value)
    {
        Extension += value;
    }

    internal void IncrementExtended(long value)
    {
        Extended += value;
    }
}

public class BuffDistribution(int initialPrimaryCapacity, int initialSecondaryCapacity)
{
    readonly int _initialSecondaryCapacity = initialSecondaryCapacity;

    private readonly Dictionary<long, Dictionary<AgentItem, BuffDistributionItem>> _distribution = new(initialPrimaryCapacity);
    public ICollection<long> BuffIDs => _distribution.Keys;

    internal Dictionary<AgentItem, BuffDistributionItem> GetDistrib(long buffID)
    {
        if (!_distribution.TryGetValue(buffID, out var distrib))
        {
            distrib = new Dictionary<AgentItem, BuffDistributionItem>(_initialSecondaryCapacity);
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

    public List<SingleActor> GetSrcs(long buffID, ParsedEvtcLog log)
    {
        var actors = new List<SingleActor>();
        if (_distribution.TryGetValue(buffID, out var buffsByAgent))
        {
            actors.AddRange(buffsByAgent.Keys.Select(x => log.FindActor(x)));
        }
        return actors;
    }

    private bool TryGetBuffDistribution(long buffID, AgentItem src, [NotNullWhen(returnValue: true)] out BuffDistributionItem? distrib)
    {
        distrib = null;
        if (_distribution.TryGetValue(buffID, out var buffsByAgent))
        {
            if (buffsByAgent.TryGetValue(src, out distrib))
            {
                return true;
            }
        }
        return false;
    }

    public long GetUptime(long buffID)
    {
        return !_distribution.TryGetValue(buffID, out var buffsByAgent) ? 0 : buffsByAgent.Sum(x => x.Value.Value);
    }

    public long GetGeneration(long buffID, AgentItem src)
    {
        return !TryGetBuffDistribution(buffID, src, out var distrib) ? 0 : distrib.Value;
    }

    public long GetOverstack(long buffID, AgentItem src)
    {
        return !TryGetBuffDistribution(buffID, src, out var distrib) ? 0 : distrib.Overstack;
    }

    public long GetWaste(long buffID, AgentItem src)
    {
        return !TryGetBuffDistribution(buffID, src, out var distrib) ? 0 : distrib.Waste;
    }

    public long GetUnknownExtension(long buffID, AgentItem src)
    {
        return !TryGetBuffDistribution(buffID, src, out var distrib) ? 0 : distrib.UnknownExtension;
    }

    public long GetExtension(long buffID, AgentItem src)
    {
        return !TryGetBuffDistribution(buffID, src, out var distrib) ? 0 : distrib.Extension;
    }

    public long GetExtended(long buffID, AgentItem src)
    {
        return !TryGetBuffDistribution(buffID, src, out var distrib) ? 0 : distrib.Extended;
    }
}
