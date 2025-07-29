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
            if (Src.EnglobedAgentItems.Count == 0)
            {

                if (distrib.TryGetValue(Src, out var toModify))
                {
                    toModify.IncrementOverstack(value);
                }
                else
                {
                    distrib.Add(Src, new BuffDistributionItem(
                        0,
                        value, 
                        0, 
                        0,
                        0, 
                        0
                    ));
                }
            } 
            else
            {
                foreach (var subSrc in Src.EnglobedAgentItems)
                {
                    long subValue = GetValue(Math.Max(start, subSrc.FirstAware), Math.Min(end, subSrc.LastAware));
                    if (subValue > 0)
                    {
                        if (distrib.TryGetValue(subSrc, out var toModify))
                        {
                            toModify.IncrementOverstack(subValue);
                        }
                        else
                        {
                            distrib.Add(subSrc, new BuffDistributionItem(
                                0,
                                subValue, 
                                0, 
                                0, 
                                0, 
                                0
                            ));
                        }
                    }
                }
            }
        }
        return value;
    }
}
