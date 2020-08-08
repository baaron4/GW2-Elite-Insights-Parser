using System.Collections.Generic;

namespace GW2EIEvtcParser.EIData
{
    internal class BuffSourceFinder05032019 : BuffSourceFinder
    {
        public BuffSourceFinder05032019(HashSet<long> boonIds) : base(boonIds)
        {
            ExtensionIDS = new HashSet<long>()
            {
                10236,
                51696,
                29453
            };
            DurationToIDs = new Dictionary<long, HashSet<long>>
            {
                {3000, new HashSet<long> { 51696 , 10236 , 29453 } }, // SoI, Treated TN, SandSquall
                {2000, new HashSet<long> { 51696 } }, // TN
            };
            EssenceOfSpeed = 2000;
            ImbuedMelodies = 2000;
        }
    }
}
