/// <summary> A segment of time with type <see cref="double"/> with inclusive start and inclusive end. </summary>
global using Segment = GW2EIEvtcParser.EIData.GenericSegment<double>;

using System.Runtime.CompilerServices;

namespace GW2EIEvtcParser.EIData;

/// <summary> A generic segment of time with inclusive start and inclusive end. </summary>
public struct GenericSegment<T>(long start, long end, T? value)
{
    public long Start = start;
    public long End   = end;
    public T?   Value = value; //TODO(Rennorb) @perf @cleanup
    public readonly (long, long) TimeSpan => (Start, End);

    public GenericSegment((long start, long end) lifespan, T? value) : this(lifespan.start, lifespan.end, value)
    {

    }    

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public GenericSegment(long start, long end) : this(start, end, default) { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly GenericSegment<O> WithOtherType<O>(O? value = default) => new(Start, End, value);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool IsEmpty() => Start >= End;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Intersects(in GenericSegment<T> other) => Intersects(other.Start, other.End);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Intersects(long start, long end) => !(end < Start || End < start);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool ContainsPoint(long time) => Start <= time && time <= End;

}
