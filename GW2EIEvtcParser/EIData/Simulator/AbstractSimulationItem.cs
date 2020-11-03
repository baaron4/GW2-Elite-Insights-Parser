using GW2EIEvtcParser.ParsedData;
using System.Collections.Generic;

namespace GW2EIEvtcParser.EIData
{
    internal abstract class AbstractSimulationItem
    {
        public abstract void SetBuffDistributionItem(BuffDistribution distribs, long start, long end, long boonid, ParsedEvtcLog log);
    }
}
