using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators;

internal abstract class SimulationItem
{

    protected static void AddValue(Dictionary<AgentItem, BuffDistributionItem> distrib, long value, AgentItem src)
    {
        if (distrib.TryGetValue(src, out var toModify))
        {
            toModify.IncrementValue(value);
        }
        else
        {
            distrib.Add(src, new BuffDistributionItem(
                value,
                0,
                0,
                0,
                0,
                0
            ));
        }
    }
    protected static void AddExtension(Dictionary<AgentItem, BuffDistributionItem> distrib, long value, AgentItem src)
    {
        if (distrib.TryGetValue(src, out var toModify))
        {
            toModify.IncrementExtension(value);
        }
        else
        {
            distrib.Add(src, new BuffDistributionItem(
                0,
                0,
                0,
                0,
                value,
                0
            ));
        }
    }

    protected static void AddExtended(Dictionary<AgentItem, BuffDistributionItem> distrib, long value, AgentItem seedSrc)
    {
        if (distrib.TryGetValue(seedSrc, out var toModify))
        {
            toModify.IncrementExtended(value);
        }
        else
        {
            distrib.Add(seedSrc, new BuffDistributionItem(
                0,
                0,
                0,
                0,
                0,
                value
            ));
        }
    }
    protected static void AddUnknown(Dictionary<AgentItem, BuffDistributionItem> distrib, long value, AgentItem seedSrc)
    {
        if (distrib.TryGetValue(seedSrc, out var toModify))
        {
            toModify.IncrementUnknownExtension(value);
        }
        else
        {
            distrib.Add(seedSrc, new BuffDistributionItem(
                0,
                0,
                0,
                value,
                0,
                0
            ));
        }
    }
    protected static void AddOverstack(Dictionary<AgentItem, BuffDistributionItem> distrib, long value, AgentItem src)
    {
        if (distrib.TryGetValue(src, out var toModify))
        {
            toModify.IncrementOverstack(value);
        }
        else
        {
            distrib.Add(src, new BuffDistributionItem(
                0,
                value,
                0,
                0,
                0,
                0
            ));
        }
    }
    protected static void AddWaste(Dictionary<AgentItem, BuffDistributionItem> distrib, long value, AgentItem src)
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
    public abstract long SetBuffDistributionItem(BuffDistribution distribs, long start, long end, long boonid);
}
