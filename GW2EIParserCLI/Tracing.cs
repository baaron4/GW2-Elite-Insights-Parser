using System;
using System.Diagnostics;

namespace GW2EIParser;

readonly struct AutoTrace : IDisposable
{
#if true
    readonly string name;
    readonly Stopwatch stopwatch;

    public AutoTrace(string name)
    {
        this.name = name;
        this.stopwatch = new();
        this.stopwatch.Start();
    }

    void IDisposable.Dispose()
    {
        this.stopwatch.Stop();
        Console.WriteLine($"{this.name}: {this.stopwatch.ElapsedMilliseconds}ms");
    }
#else
    #pragma warning disable IDE0060 // Remove unused parameter
    public AutoTrace(string name) { }
    #pragma warning restore IDE0060 // Remove unused parameter
    void IDisposable.Dispose() { }
#endif
}
