
namespace GW2EIEvtcParser.EIData;
public class StateGraph<T>
{
    public IReadOnlyList<GenericSegment<T>> Values => _values;
    private List<GenericSegment<T>> _values;

    public StateGraph()
    {
        _values = [];
    }

    public StateGraph(IEnumerable<GenericSegment<T>> values)
    {
        _values = values.ToList();
    }


    //TODO(Rennorb) @perf
    /// <summary>
    /// Fuse consecutive segments with same value
    /// </summary>
    internal void FuseSegments()
    {
        _values.RemoveAll(x => x.Start > x.End);
        _values.FuseConsecutive();
    }
}
