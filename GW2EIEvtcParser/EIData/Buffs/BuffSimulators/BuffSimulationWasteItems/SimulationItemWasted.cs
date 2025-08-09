using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators;

internal abstract class SimulationItemWasted : SimulationItem
{
    protected readonly AgentItem Src;
    private readonly long _waste;
    protected readonly long Time;
    protected SimulationItemWasted(AgentItem src, long waste, long time)
    {
        Src = src;
        _waste = waste;
        Time = time;
    }

    protected long GetValue(long start, long end)
    {
        return (start <= Time && Time <= end) ? _waste : 0;
    }
}
