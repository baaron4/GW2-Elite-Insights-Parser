namespace GW2EIEvtcParser.EIData.BuffSimulators;

internal abstract class AbstractSimulationItem
{
    public abstract bool SetBuffDistributionItem(BuffDistribution distribs, long start, long end, long boonid);
}
