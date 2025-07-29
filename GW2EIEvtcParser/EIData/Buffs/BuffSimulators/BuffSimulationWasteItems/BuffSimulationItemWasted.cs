using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators;

internal class BuffSimulationItemWasted : AbstractBuffSimulationItemWasted
{

    public BuffSimulationItemWasted(AgentItem src, long waste, long time) : base(src, waste, time)
    {
    }

    public override long SetBuffDistributionItem(BuffDistribution distribs, long start, long end, long buffID)
    {
        long value = GetValue(start, end);
        if (value > 0)
        {
            Dictionary<AgentItem, BuffDistributionItem> distrib = distribs.GetDistrib(buffID);
            if (distrib.TryGetValue(Src, out var toModify))
            {
                toModify.IncrementWaste(value);
            }
            else
            {
                distrib.Add(Src, new BuffDistributionItem(
                    0,
                    0,
                    value,
                    0,
                    0,
                    0
                ));
            }

            foreach (var subSrc in Src.EnglobedAgentItems)
            {
                long subValue = GetValue(Math.Max(start, subSrc.FirstAware), Math.Min(end, subSrc.LastAware));
                if (subValue > 0)
                {
                    if (distrib.TryGetValue(subSrc, out toModify))
                    {
                        toModify.IncrementWaste(subValue);
                    }
                    else
                    {
                        distrib.Add(subSrc, new BuffDistributionItem(
                            0,
                            0,
                            subValue,
                            0,
                            0,
                            0
                         ));
                    }
                }
            }

        }
        return value;
    }
}
