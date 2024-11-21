using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators;

internal class BuffSimulationItemOverstack : AbstractBuffSimulationItemWasted
{

    public BuffSimulationItemOverstack(AgentItem src, long overstack, long time) : base(src, overstack, time)
    {
    }

    public override long SetBuffDistributionItem(BuffDistribution distribs, long start, long end, long buffID)
    {
        Dictionary<AgentItem, BuffDistributionItem> distrib = distribs.GetDistrib(buffID);
        long value = GetValue(start, end);
        if (value > 0)
        {
            AgentItem agent = Src;
            if (distrib.TryGetValue(agent, out var toModify))
            {
                toModify.IncrementOverstack(value);
            }
            else
            {
                distrib.Add(agent, new BuffDistributionItem(
                    0,
                    value, 0, 0, 0, 0));
            }
        }
        return value;
    }
}
