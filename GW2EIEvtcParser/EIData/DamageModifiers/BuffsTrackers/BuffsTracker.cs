namespace GW2EIEvtcParser.EIData;

internal abstract class BuffsTracker
{
    public abstract int GetStack(IReadOnlyDictionary<long, BuffGraph> bgms, long time);
    public abstract bool Has(IReadOnlyDictionary<long, BuffGraph> bgms);
}
