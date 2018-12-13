using System;
using System.Collections.Generic;

namespace LuckParser.Models.ParseModels
{
    public abstract class AbstractBoonSimulationItem
    {
        public abstract void SetBoonDistributionItem(Dictionary<ushort, BoonDistributionItem> distrib);
    }
}
