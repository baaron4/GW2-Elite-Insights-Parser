using System.Collections.Generic;

namespace GW2EIEvtcParser.EIData.BuffSourceFinders
{
    internal class BuffSourceFinder20190305 : BuffSourceFinder20181211
    {
        public BuffSourceFinder20190305(HashSet<long> boonIds) : base(boonIds)
        {
            DurationToIDs = new Dictionary<long, HashSet<long>>
            {
                {3000, new HashSet<long> { 51696 , 10236 , 29453 } }, // SoI, Treated TN, SandSquall
                {2000, new HashSet<long> { 51696 } }, // TN
            };
        }
    }
}
