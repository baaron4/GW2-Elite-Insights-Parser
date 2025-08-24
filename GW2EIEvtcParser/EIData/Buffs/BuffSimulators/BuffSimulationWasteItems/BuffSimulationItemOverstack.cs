using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators;

internal class BuffSimulationItemOverstack : SimulationItemWasted
{

    public BuffSimulationItemOverstack(AgentItem src, long overstack, long time) : base(src, overstack, time)
    {
    }

    public override void SetBuffDistributionItem(BuffDistribution distribs, long start, long end, long buffID)
    {
        long value = GetValue(start, end);
        if (value > 0)
        {
            Dictionary<AgentItem, BuffDistributionItem> distrib = distribs.GetDistrib(buffID);
            AddOverstack(distrib, value, Src);
            foreach (var subSrc in Src.EnglobedAgentItems)
            {
                long subValue = GetValue(Math.Max(start, subSrc.FirstAware), Math.Min(end, subSrc.LastAware));
                if (subValue > 0)
                {
                    AddOverstack(distrib, subValue, subSrc);
                }
            }
        }
    }
}
