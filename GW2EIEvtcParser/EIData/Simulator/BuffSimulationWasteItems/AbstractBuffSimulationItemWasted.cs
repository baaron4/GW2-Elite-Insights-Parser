using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal abstract class AbstractBuffSimulationItemWasted : AbstractSimulationItem
    {
        protected AgentItem Src { get; }
        private readonly long _waste;
        protected long Time { get; }
        protected AbstractBuffSimulationItemWasted(AgentItem src, long waste, long time)
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
}
