using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class BuffSourceFinder20210921 : BuffSourceFinder20210511
    {
        public BuffSourceFinder20210921(HashSet<long> boonIds) : base(boonIds)
        {
            ExtensionIDS = new HashSet<long>()
            {
                10236,
                51696,
                29453,
                62859
            };
            DurationToIDs = new Dictionary<long, HashSet<long>>
            {
                {3000, new HashSet<long> { 51696 , 10236 , 29453 } }, // SoI, Treated TN, SandSquall
                {2000, new HashSet<long> { 51696 , 62859 } }, // TN, Imperial Impact (Vassals of the Empire)
            };
        }
    }
}
