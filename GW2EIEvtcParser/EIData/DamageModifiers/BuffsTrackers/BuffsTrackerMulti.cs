namespace GW2EIEvtcParser.EIData;

internal class BuffsTrackerMulti(HashSet<long> buffsIDs) : BuffsTracker
{
    private readonly HashSet<long> _ids = buffsIDs;

    public override int GetStack(IReadOnlyDictionary<long, BuffGraph> bgms, long time)
    {
        int stack = 0;
        foreach (long key in bgms.Keys.Intersect(_ids))
        {
            stack += bgms[key].GetStackCount(time) > 0 ? 1 : 0;
        }
        return stack;
    }

    public override bool IsEmpty(IReadOnlyDictionary<long, BuffGraph> bgms)
    {
        return _ids.All(x => !bgms.TryGetValue(x, out var bgm) || bgm.IsEmpty);
    }

    public override bool IsFull(IReadOnlyDictionary<long, BuffGraph> bgms)
    {
        return _ids.All(x => bgms.TryGetValue(x, out var bgm) && bgm.IsFull);
    }
}
