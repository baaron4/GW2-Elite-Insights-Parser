namespace GW2EIEvtcParser.EIData;

internal class BuffsTrackerSingle : BuffsTracker
{
    private readonly long _id;

    public BuffsTrackerSingle(long id)
    {
        _id = id;
    }

    public override int GetStack(IReadOnlyDictionary<long, BuffGraph> bgms, long time)
    {
        return bgms.TryGetValue(_id, out var bgm) ? bgm.GetStackCount(time) : 0;
    }

    public override bool IsEmpty(IReadOnlyDictionary<long, BuffGraph> bgms)
    {
        return !bgms.TryGetValue(_id, out var bgm) || bgm.IsEmpty;
    }

    public override bool IsFull(IReadOnlyDictionary<long, BuffGraph> bgms)
    {
        return bgms.TryGetValue(_id, out var bgm) && bgm.IsFull;
    }
}
