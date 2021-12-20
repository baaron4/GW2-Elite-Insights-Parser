namespace GW2EIEvtcParser.EIData.BuffSimulators
{
    internal abstract class AbstractSimulationItem
    {
        public abstract void SetBuffDistributionItem(BuffDistribution distribs, long start, long end, long boonid);
    }
}
