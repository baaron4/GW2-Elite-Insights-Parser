
namespace GW2EIEvtcParser.EIData;
public class InterpolatedGraph<T>
{
    public IReadOnlyList<T> Values => _values;

    internal T[] _values;

    public readonly int First;
    public readonly int Last;
    public readonly int Polling;

    public InterpolatedGraph(int first, int last, int polling)
    {
        First = first; 
        Last = last; 
        Polling = polling;
        int durationInMS = (int)(last - first);
        int durationInS = durationInMS / 1000;
        _values = durationInS * 1000 != durationInMS ? new T[durationInS + 2] : new T[durationInS + 1];
    }

    public InterpolatedGraph(long first, long last, int polling) : this((int)first, (int)last, polling)
    {
    }
}
