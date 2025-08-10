using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators;

internal class BuffSimulationItemWasted : SimulationItemWasted
{

    public BuffSimulationItemWasted(AgentItem src, long waste, long time) : base(src, waste, time)
    {
    }
    private static void Add(Dictionary<AgentItem, BuffDistributionItem> distrib, long value, AgentItem src)
    {
        if (distrib.TryGetValue(src, out var toModify))
        {
            toModify.IncrementWaste(value);
        }
        else
        {
            distrib.Add(src, new BuffDistributionItem(
                0,
                0,
                value,
                0,
                0,
                0
            ));
        }
    }
    public override long SetBuffDistributionItem(BuffDistribution distribs, long start, long end, long buffID)
    {
        long value = GetValue(start, end);
        if (value > 0)
        {
            Dictionary<AgentItem, BuffDistributionItem> distrib = distribs.GetDistrib(buffID);
            Add(distrib, value, Src);
            foreach (var subSrc in Src.EnglobedAgentItems)
            {
                long subValue = GetValue(Math.Max(start, subSrc.FirstAware), Math.Min(end, subSrc.LastAware));
                if (subValue > 0)
                {
                    Add(distrib, subValue, subSrc);
                }
            }

        }
        return value;
    }
}
