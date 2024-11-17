using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators;

internal class BuffSimulationItemBaseWithExtension : BuffSimulationItemBase
{

    protected internal BuffSimulationItemBaseWithExtension(BuffStackItem buffStackItem) : base(buffStackItem)
    {
    }
    public override bool SetBuffDistributionItem(BuffDistribution distribs, long start, long end, long buffID)
    {
        if (base.SetBuffDistributionItem(distribs, start, end, buffID))
        {
            long cDur = GetClampedDuration(start, end);
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
            return true;
        }
        return false;
    }
}
