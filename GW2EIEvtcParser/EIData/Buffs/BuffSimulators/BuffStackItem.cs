using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators;

internal class BuffStackItem
{
    public long Start { get; private set; }
    public long Duration { get; private set; }
    public AgentItem Src { get; private set; }
    public readonly AgentItem SeedSrc;
    public bool IsExtension { get; private set; }
    public readonly long StackID;

    public long TotalDuration //TODO(Rennorb) @perf
    {
        get
        {
            long res = Duration;
            foreach ((_, long value) in Extensions)
            {
                res += value;
            }
            return res;
        }
    }

    public readonly List<(AgentItem src, long value)> Extensions = [];

    public BuffStackItem(long start, long boonDuration, AgentItem src, AgentItem seedSrc, bool isExtension, long stackID)
    {
        Start = start;
        SeedSrc = seedSrc;
        Duration = boonDuration;
        Src = src;
        IsExtension = isExtension;
        StackID = stackID;
    }

    public BuffStackItem(long start, long boonDuration, AgentItem src, long stackID)
    {
        Start = start;
        SeedSrc = src;
        Duration = boonDuration;
        Src = src;
        IsExtension = false;
        StackID = stackID;
    }

    public virtual void Shift(long startShift, long durationShift)
    {
        Start += startShift;
        Duration -= durationShift;
        if (Duration == 0 && Extensions.Count != 0)
        {
            (this.Src, this.Duration) = Extensions[0];
            Extensions.RemoveAt(0);
            IsExtension = true;
        }
    }

    public void Extend(long value, AgentItem src)
    {
        Extensions.Add((src, value));
    }
}
