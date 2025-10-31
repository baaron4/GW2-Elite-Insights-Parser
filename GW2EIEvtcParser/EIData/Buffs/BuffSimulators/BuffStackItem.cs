using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators;

internal class BuffStackItem
{
    public long Start { get; private set; }
    public long Duration { get; private set; }
    public AgentItem Src { get; private set; }
    public AgentItem SeedSrc { get; private set; }
    public bool IsExtension { get; private set; }
    public long StackID { get; private set; }

    public long TotalDuration //TODO_PERF(Rennorb)
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

    public List<(AgentItem src, long value)> Extensions { get; private set; } = [];

    public BuffStackItem(long start, long boonDuration, AgentItem src, AgentItem seedSrc, bool isExtension, long stackID)
    {
        Reset(start, boonDuration, src, seedSrc, isExtension, stackID);
    }

    public void Reset(long start, long boonDuration, AgentItem src, AgentItem seedSrc, bool isExtension, long stackID)
    {
        Start = start;
        SeedSrc = seedSrc;
        Duration = boonDuration;
        Src = src;
        IsExtension = isExtension;
        StackID = stackID;
        Extensions = [];
    }

    public BuffStackItem(long start, long boonDuration, AgentItem src, long stackID)
    {
        Reset(start, boonDuration, src, stackID);
    }

    public void Reset(long start, long boonDuration, AgentItem src, long stackID)
    {
        Start = start;
        SeedSrc = src;
        Duration = boonDuration;
        Src = src;
        IsExtension = false;
        StackID = stackID;
        Extensions = [];
    }

    public virtual void Shift(long startShift, long durationShift)
    {
        Start += startShift;
        Duration -= durationShift;
        if (Duration == 0 && Extensions.Count != 0)
        {
            (Src, Duration) = Extensions[0];
            Extensions.RemoveAt(0);
            IsExtension = true;
        }
    }

    public void Extend(long value, AgentItem src)
    {
        Extensions.Add((src, value));
    }
}
internal class BuffStackItemID : BuffStackItem
{

    public bool Active { get; protected set; } = false;

    public BuffStackItemID(long start, long boonDuration, AgentItem src, bool active, long stackID) : base(start, boonDuration, src, stackID)
    {
        Active = active;
    }


    public void Reset(long start, long boonDuration, AgentItem src, bool active, long stackID)
    {
        Reset(start, boonDuration, src, stackID);
        Active = active;
    }

    public void Activate()
    {
        Active = true;
    }

    public void Disable()
    {
        Active = false;
    }

    public override void Shift(long startShift, long durationShift)
    {
        if (!Active)
        {
            base.Shift(startShift, 0);
            return;
        }
        base.Shift(startShift, durationShift);
    }
}


internal class BuffStackItemPool
{
    private Queue<BuffStackItem>? _stackItems = null;
    private Queue<BuffStackItemID>? _stackItemsWithID = null;

    public void InitializeBuffStackItemPool()
    {
        _stackItems ??= new Queue<BuffStackItem>(300);
    }

    public void InitializeBuffStackItemWithIDPool()
    {
        _stackItemsWithID ??= new Queue<BuffStackItemID>(300);
    }

    public BuffStackItem GetBuffStackItem(long start, long boonDuration, AgentItem src, AgentItem seedSrc, bool isExtension, long stackID)
    {
        var first = _stackItems!.Count > 0 ? _stackItems!.Dequeue() : null;
        if (first != null)
        {
            first.Reset(start, boonDuration, src, seedSrc, isExtension, stackID);
            return first;
        }
        return new BuffStackItem(start, boonDuration, src, seedSrc, isExtension, stackID);
    }

    public BuffStackItem GetBuffStackItem(long start, long boonDuration, AgentItem src, long stackID)
    {
        var first = _stackItems!.Count > 0 ? _stackItems!.Dequeue() : null;
        if (first != null)
        {
            first.Reset(start, boonDuration, src, stackID);
            return first;
        }
        return new BuffStackItem(start, boonDuration, src, stackID);
    }

    public void ReleaseBuffStackItem(BuffStackItem buffStackItem)
    {
        _stackItems!.Enqueue(buffStackItem);
    }
    public void ReleaseBuffStackItems(IEnumerable<BuffStackItem> buffStackItems)
    {
        foreach (var buffStackItem in buffStackItems)
        {
            _stackItems!.Enqueue(buffStackItem);
        }
    }

    public BuffStackItemID GetBuffStackItemID(long start, long boonDuration, AgentItem src, bool active, long stackID)
    {
        var first = _stackItemsWithID!.Count > 0 ? _stackItemsWithID!.Dequeue() : null;
        if (first != null)
        {
            first.Reset(start, boonDuration, src, active, stackID);
            return first;
        }
        return new BuffStackItemID(start, boonDuration, src, active, stackID);
    }

    public void ReleaseBuffStackItemID(BuffStackItemID buffStackItemID)
    {
        _stackItemsWithID!.Enqueue(buffStackItemID);
    }
    public void ReleaseBuffStackItemsID(IEnumerable<BuffStackItemID> buffStackItemsID)
    {
        foreach (var buffStackItemID in buffStackItemsID)
        {
            _stackItemsWithID!.Enqueue(buffStackItemID);
        }
    }
}

