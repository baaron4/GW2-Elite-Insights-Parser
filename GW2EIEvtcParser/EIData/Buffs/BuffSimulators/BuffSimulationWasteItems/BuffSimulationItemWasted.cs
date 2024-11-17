using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators;

internal class BuffSimulationItemWasted : AbstractBuffSimulationItemWasted
{

    public BuffSimulationItemWasted(AgentItem src, long waste, long time) : base(src, waste, time)
    {
    }

    public override bool SetBuffDistributionItem(BuffDistribution distribs, long start, long end, long buffID)
    {
        Dictionary<AgentItem, BuffDistributionItem> distrib = distribs.GetDistrib(buffID);
        AgentItem agent = Src;
        long value = GetValue(start, end);
        if (value == 0)
        {
            return false;
        }
        if (distrib.TryGetValue(agent, out BuffDistributionItem toModify))
        {
            toModify.IncrementWaste(value);
        }
        else
        {
            distrib.Add(agent, new BuffDistributionItem(
                0,
                0, value, 0, 0, 0));
        }
        return true;
    }
}
