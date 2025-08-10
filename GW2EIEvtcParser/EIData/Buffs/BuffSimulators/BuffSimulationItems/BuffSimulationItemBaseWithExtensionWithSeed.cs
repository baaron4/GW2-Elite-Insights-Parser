using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators;

internal class BuffSimulationItemBaseWithExtensionWithSeed : BuffSimulationItemBaseWithSeed
{

    protected internal BuffSimulationItemBaseWithExtensionWithSeed(BuffStackItem buffStackItem) : base(buffStackItem)
    {
    }
    public override long SetBuffDistributionItem(BuffDistribution distribs, long start, long end, long buffID)
    {
        long cDur = base.SetBuffDistributionItem(distribs, start, end, buffID);
        if (cDur > 0)
        {
            Dictionary<AgentItem, BuffDistributionItem> distribution = distribs.GetDistrib(buffID);
            AddExtension(distribution, cDur, Src);
            foreach (var subSrc in Src.EnglobedAgentItems)
            {
                long subcDur = GetClampedDuration(Math.Max(start, subSrc.FirstAware), Math.Min(end, subSrc.LastAware));
                if (subcDur > 0)
                {
                    AddExtension(distribution, subcDur, subSrc);
                }
            }

        }
        return cDur;
    }
}
