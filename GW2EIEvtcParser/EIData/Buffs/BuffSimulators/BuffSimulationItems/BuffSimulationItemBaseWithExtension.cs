using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators;

internal class BuffSimulationItemBaseWithExtension : BuffSimulationItemBase
{

    protected internal BuffSimulationItemBaseWithExtension(BuffStackItem buffStackItem) : base(buffStackItem)
    {
    }
    public override long SetBuffDistributionItem(BuffDistribution distribs, long start, long end, long buffID)
    {
        long cDur = base.SetBuffDistributionItem(distribs, start, end, buffID);
        if (cDur > 0)
        {
            Dictionary<AgentItem, BuffDistributionItem> distribution = distribs.GetDistrib(buffID);
            if (distribution.TryGetValue(_src, out var toModify))
            {
                toModify.IncrementExtension(cDur);
            }
            else
            {
                distribution.Add(_src, new BuffDistributionItem(
                    0,
                    0, 0, 0, cDur, 0));
            }
            return cDur;
        }
        return cDur;
    }
}
