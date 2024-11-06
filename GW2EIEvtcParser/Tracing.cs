#define EI_TRACING
#define EI_TRACING_STATS

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Tracing;

#pragma warning disable IDE0060 // Remove unused parameter

public static class Trace
{
#if EI_TRACING && EI_TRACING_STATS
    internal static readonly ThreadLocal<Dictionary<string, (long Count, long Total, long Min, long Max)>> s_avgDicts = new(() => new(), true);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void TrackAverageStat(string name, long stat)
    {
        var values = s_avgDicts.Value;
        if(values.TryGetValue(name, out var old))
        {
            values[name] = (old.Count +1, old.Total + stat, Math.Min(old.Min, stat), Math.Max(old.Max, stat));
        }
        else
        {
            Console.WriteLine($"[{Environment.CurrentManagedThreadId}]  First occurrence of avg stat {name}: {stat}");
            values[name] = (1, stat, stat, stat);
        }
    }

#else

    public static void TrackAverageStat(string name, long stat) { }

#endif
}

public struct AutoTrace : IDisposable
{
#if EI_TRACING
    readonly string name;
    readonly Stopwatch stopwatch;
    readonly bool isAveraging;
    long lstMsgMs;
    long lstAvgMs;


    static readonly ThreadLocal<Stack<AutoTrace>> s_parentTrace = new(() => new(), false);

    public AutoTrace(string name, bool isAveraging = false)
    {
        this.name = name;
        this.isAveraging = isAveraging;
        this.stopwatch = new();
        this.stopwatch.Start();
        if(!isAveraging) {
            if(s_parentTrace.Value.Count > 0)
            {
                var parent = s_parentTrace.Value.Peek();
                Console.WriteLine($"[{Environment.CurrentManagedThreadId}] {parent.name} @ {parent.stopwatch.ElapsedMilliseconds,5}ms -> enter {this.name}");
            }
            else
            {
                Console.WriteLine($"[{Environment.CurrentManagedThreadId}] enter {this.name}");
            }
        }
        s_parentTrace.Value.Push(this);
    }

    public void Log(string message)
    {
        var elapsed = stopwatch.ElapsedMilliseconds;
        Console.WriteLine($"[{Environment.CurrentManagedThreadId}] {this.name} @ {elapsed,5}ms -> {message}: {elapsed - lstMsgMs,5}ms");
        lstMsgMs = stopwatch.ElapsedMilliseconds;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetAverageTimeStart()
    {
        #if EI_TRACING_STATS
        lstAvgMs = stopwatch.ElapsedMilliseconds;
        #endif
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void TrackAverageTime(string name)
    {
        #if EI_TRACING_STATS
        var elapsed = stopwatch.ElapsedMilliseconds - lstAvgMs;
        Trace.TrackAverageStat(name, elapsed);
        lstAvgMs = stopwatch.ElapsedMilliseconds;
        #endif
    }

    void IDisposable.Dispose()
    {
        this.stopwatch.Stop();
        var parents = s_parentTrace.Value;
        parents.Pop(); // pop ourself

        if(this.isAveraging)
        {
            Trace.TrackAverageStat(this.name, stopwatch.ElapsedMilliseconds);
        }

        if(parents.Count > 0)
        {
            if(!this.isAveraging)
            {
                var parent = parents.Peek();
                Console.WriteLine($"[{Environment.CurrentManagedThreadId}] {parent.name} @ {parent.stopwatch.ElapsedMilliseconds,5}ms <- exit  {this.name}: {this.stopwatch.ElapsedMilliseconds,5}ms");
            }
        }
        else
        {
            if(!this.isAveraging)
            {
                Console.WriteLine($"[{Environment.CurrentManagedThreadId}] exit  {this.name}: {this.stopwatch.ElapsedMilliseconds,5}ms");
            }

            #if EI_TRACING_STATS
            if(Environment.CurrentManagedThreadId == 1)
            {
                Console.WriteLine($"[{Environment.CurrentManagedThreadId}] Root tracer destroyed, here are Stats:");
                var stats = Trace.s_avgDicts.Values.SelectMany(d => d).GroupBy(p => p.Key, p => p.Value);
                foreach(var group in stats)
                {
                    var (count, total, min, max) = group.AsEnumerable().Aggregate((0L, 0L, long.MaxValue, long.MinValue), (old, curr) => (old.Item1 + curr.Count, old.Item2 + curr.Total, Math.Min(old.Item3, curr.Min), Math.Max(old.Item4, curr.Max)));
                    Console.WriteLine($"\t{group.Key,-20}: {min,5} Min, {(float)total/count,8:F2} Avg, {max,5} Max  ({count,4} Samples)");
                }
            }
            #endif
        }
    }
#else
    public AutoTrace(string name) { }
    public void Log(string message) { }
    public void TrackAverageTime(string name) { }
    void IDisposable.Dispose() { }
#endif
}

#pragma warning restore IDE0060 // Remove unused parameter
