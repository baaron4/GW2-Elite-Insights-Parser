#define EI_TRACING

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Tracing;

#pragma warning disable IDE0060 // Remove unused parameter

public static class Trace
{
    /// For now this is just printf, bot one day we could properly expand it.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void TrackAverageStat<T>(string name, T stat, [CallerMemberName] string? sourceName = null)
    {
        #if EI_TRACING
        Console.WriteLine($"Stat {sourceName} {name}: {stat}");
        #endif
    }
}

public struct AutoTrace : IDisposable
{
#if EI_TRACING
    readonly string name;
    readonly Stopwatch stopwatch;
    long lstMsgMs;


    static readonly ThreadLocal<Stack<AutoTrace>> s_parentTrace = new(() => new(), false);

    public AutoTrace(string name)
    {
        this.name = name;
        this.stopwatch = new();
        this.stopwatch.Start();
        if(s_parentTrace.Value.Count > 0)
        {
            var parent = s_parentTrace.Value.Peek();
            Console.WriteLine($"[{Environment.CurrentManagedThreadId}] {parent.name} @ {parent.stopwatch.ElapsedMilliseconds}ms -> enter {this.name}");
        }
        else
        {
            Console.WriteLine($"[{Environment.CurrentManagedThreadId}] enter {this.name}");
        }
        s_parentTrace.Value.Push(this);
    }

    public void Log(string message)
    {
        var elapsed = stopwatch.ElapsedMilliseconds;
        Console.WriteLine($"[{Environment.CurrentManagedThreadId}] {this.name} @ {elapsed}ms -> {message}: {elapsed - lstMsgMs}ms");
        lstMsgMs = stopwatch.ElapsedMilliseconds;
    }

    void IDisposable.Dispose()
    {
        this.stopwatch.Stop();
        Debug.Assert(this.Equals(s_parentTrace.Value.Peek()));
        s_parentTrace.Value.Pop();
        if(s_parentTrace.Value.Count > 0)
        {
            var parent = s_parentTrace.Value.Peek();
            Console.WriteLine($"[{Environment.CurrentManagedThreadId}] {parent.name} @ {parent.stopwatch.ElapsedMilliseconds}ms <- exit {this.name}: {this.stopwatch.ElapsedMilliseconds}ms");
        }
        else
        {
            Console.WriteLine($"[{Environment.CurrentManagedThreadId}] exit {this.name}: {this.stopwatch.ElapsedMilliseconds}ms");
        }
    }
#else
    public AutoTrace(string name) { }
    public void Log(string message) { }
    void IDisposable.Dispose() { }
#endif
}

#pragma warning restore IDE0060 // Remove unused parameter
