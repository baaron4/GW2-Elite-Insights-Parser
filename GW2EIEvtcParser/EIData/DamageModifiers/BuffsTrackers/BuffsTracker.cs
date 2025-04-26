namespace GW2EIEvtcParser.EIData;

internal abstract class BuffsTracker
{
    public abstract int GetStack(IReadOnlyDictionary<long, BuffGraph> bgms, long time);
    public abstract bool IsEmpty(IReadOnlyDictionary<long, BuffGraph> bgms);
    public abstract bool IsFull(IReadOnlyDictionary<long, BuffGraph> bgms);
}
