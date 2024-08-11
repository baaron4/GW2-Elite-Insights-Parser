using System.Collections.Generic;

namespace GW2EIEvtcParser.EIData.BuffSourceFinders
{
    internal class BuffSourceFinder20221018 : BuffSourceFinder20210921
    {
        public BuffSourceFinder20221018(HashSet<long> boonIds) : base(boonIds)
        {
            ImperialImpactExtension = 1000;
        }

    }
}
