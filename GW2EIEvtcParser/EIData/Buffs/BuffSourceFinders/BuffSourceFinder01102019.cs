using System.Collections.Generic;

namespace GW2EIEvtcParser.EIData
{
    public class BuffSourceFinder01102019 : BuffSourceFinder05032019
    {
        public BuffSourceFinder01102019(HashSet<long> boonIds) : base(boonIds)
        {
            ImbuedMelodies = -1;
        }
    }
}
